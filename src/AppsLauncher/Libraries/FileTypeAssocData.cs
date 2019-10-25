namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using Microsoft.Win32;
    using Properties;
    using SilDev;
    using SilDev.Forms;

    public sealed class FileTypeAssocData
    {
        private readonly Dictionary<string, string> _assocData;

        public FileTypeAssocData()
        {
            _assocData = new Dictionary<string, string>();
            SystemRegistryAccess = new RegistryAccess(this);
        }

        public FileTypeAssocData(string json)
        {
            _assocData = Json.Deserialize<Dictionary<string, string>>(json) ?? throw new FormatException();
            SystemRegistryAccess = new RegistryAccess(this);
        }

        public string AppKey
        {
            get => GetData(nameof(AppKey));
            set => SetData(nameof(AppKey), value);
        }

        public string InstallId
        {
            get => GetData(nameof(InstallId));
            set => SetData(nameof(InstallId), value);
        }

        public string IconPath
        {
            get => GetData(nameof(IconPath));
            set => SetData(nameof(IconPath), value);
        }

        public string IconId
        {
            get => GetData(nameof(IconId));
            set => SetData(nameof(IconId), value);
        }

        public string StarterPath
        {
            get => GetData(nameof(StarterPath));
            set => SetData(nameof(StarterPath), value);
        }

        public RegistryAccess SystemRegistryAccess { get; }

        public override string ToString() =>
            Json.Serialize(_assocData);

        private string GetData(string key) =>
            _assocData.TryGetValue(key, out var value) ? value : default;

        private void SetData(string key, string value) =>
            _assocData.Update(key, value);

        public sealed class RegistryAccess
        {
            private readonly FileTypeAssocData _parent;

            internal RegistryAccess(FileTypeAssocData parent) =>
                _parent = parent;

            public void AssociateFileTypes(bool quiet)
            {
                if (_parent?.AppKey == null || _parent.IconId?.All(char.IsDigit) != true || !FileEx.Exists(_parent.IconPath) || !FileEx.Exists(_parent.StarterPath))
                    goto Abort;

                var appData = CacheData.FindAppData(_parent.AppKey);
                if (appData == default)
                    goto Abort;

                if (!Elevation.IsAdministrator)
                {
                    using (var process = ProcessEx.Start(PathEx.LocalPath, $"{ActionGuid.RestoreFileTypes} \"{appData.Key}\"", true, false))
                        if (!process?.HasExited == true)
                            process.WaitForExit();
                    return;
                }

                var restPointEnabled = quiet || MessageBoxEx.Show(Language.GetText(nameof(en_US.RestorePointMsg0)), Resources.GlobalTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                if (restPointEnabled && EnvironmentEx.SystemRestore.IsEnabled)
                {
                    var result = !quiet ? MessageBoxEx.Show(Language.GetText(nameof(en_US.RestorePointMsg1)), Resources.GlobalTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) : DialogResult.Yes;
                    if (result == DialogResult.Yes)
                        EnvironmentEx.SystemRestore.Create(string.Format(CultureInfo.InvariantCulture, en_US.AssociateRestPointLabel, appData.Name), EnvironmentEx.RestoreEventType.BeginSystemChange, EnvironmentEx.RestorePointType.ModifySettings);
                }

                var restPointDir = Path.Combine(CorePaths.RestorePointDir, appData.Key);
                var restPointCount = -1;
                if (restPointEnabled)
                    if (Directory.Exists(restPointDir))
                        restPointCount = Directory.GetFiles(restPointDir, "*.dat", SearchOption.TopDirectoryOnly).Length;
                    else
                    {
                        if (!DirectoryEx.Create(restPointDir))
                            goto Cancel;
                        restPointCount = 0;
                    }

                const string baseKeyName = "HKEY_CLASSES_ROOT";
                var appKeyName = $"PortableAppsSuite_{appData.Key}";
                var appKeyPath = Path.Combine(baseKeyName, appKeyName);

                var restPoint = default(Dictionary<string, List<string>>);
                if (restPointEnabled)
                {
                    restPoint = new Dictionary<string, List<string>>();
                    if (!Reg.SubKeyExists(appKeyPath))
                        restPoint.Add(appData.Key, new List<string>
                        {
                            $"[-{appKeyPath}]"
                        });
                }

                var curIconKeyPath = Path.Combine(appKeyPath, "DefaultIcon");
                var curIconData = Reg.ReadString(curIconKeyPath, null);
                var newIconData = $"\"{_parent.IconPath}\",{_parent.IconId}";
                if (!curIconData.EqualsEx(newIconData))
                    Reg.Write(curIconKeyPath, null, newIconData, RegistryValueKind.ExpandString);

                var curCmdKeyPath = Path.Combine(appKeyPath, "shell\\open\\command");
                var curCmdData = Reg.ReadString(curCmdKeyPath, null);
                var newCmdData = $"\"{_parent.StarterPath}\" \"%1\"";
                if (!curCmdData.EqualsEx(newCmdData))
                    Reg.Write(curCmdKeyPath, null, newCmdData, RegistryValueKind.ExpandString);
                Reg.RemoveEntry(curCmdKeyPath, "DelegateExecute");

                foreach (var type in appData.Settings.FileTypes.Where(x => !x.StartsWith(".", StringComparison.Ordinal)))
                {
                    if (string.IsNullOrWhiteSpace(type))
                        continue;

                    var typeKeyPath = Path.Combine(baseKeyName, $".{type}");
                    if (restPointEnabled)
                    {
                        restPoint.Add(type, new List<string>());
                        restPoint[type].Add($"[-{typeKeyPath}]");
                    }

                    if (Reg.SubKeyExists(typeKeyPath))
                    {
                        var keyPath = Path.Combine(CorePaths.TempDir, FileEx.GetUniqueName());
                        Reg.ExportKeys(keyPath, typeKeyPath);
                        if (File.Exists(keyPath))
                        {
                            var lines = FileEx.ReadAllLines(keyPath);
                            if (lines?.Length > 0)
                                lines = FileEx.ReadAllLines(keyPath)?.Skip(1).Where(Comparison.IsNotEmpty).ToArray();
                            if (restPointEnabled && restPoint.ContainsKey(type) && lines?.Any() == true)
                                restPoint[type].AddRange(lines);
                            File.Delete(keyPath);
                        }
                    }

                    Reg.Write(typeKeyPath, null, appKeyName, RegistryValueKind.ExpandString);
                }

                if (restPointEnabled && restPointCount >= 0)
                {
                    var restPointPath = Path.Combine(restPointDir, $"{restPointCount:X4}.dat");
                    FileEx.Serialize(restPointPath, restPoint, true);
                }

                if (!quiet)
                    MessageBoxEx.Show(Language.GetText(nameof(en_US.OperationCompletedMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

                Cancel:
                appData.Settings.FileTypeAssoc = default;

                Abort:
                if (!quiet)
                    MessageBoxEx.Show(Language.GetText(nameof(en_US.OperationCanceledMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            public void LoadRestorePoint(bool quiet)
            {
                if (_parent?.AppKey == null)
                    goto Cancel;

                var appData = CacheData.FindAppData(_parent.AppKey);
                if (appData == default)
                    goto Cancel;

                if (!quiet)
                {
                    var result = MessageBoxEx.Show(Language.GetText(nameof(en_US.RestorePointMsg2)), Resources.GlobalTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        goto Cancel;
                }

                if (!Elevation.IsAdministrator)
                {
                    using (var process = ProcessEx.Start(PathEx.LocalPath, $"{ActionGuid.RestoreFileTypes} \"{appData.Key}\"", true, false))
                        if (!process?.HasExited == true)
                            process.WaitForExit();
                    return;
                }

                var restPointDir = Path.Combine(CorePaths.RestorePointDir, appData.Key);
                if (Directory.Exists(restPointDir))
                {
                    var files = DirectoryEx.EnumerateFiles(restPointDir, "*.dat")?.Reverse().ToArray();
                    if (files?.Any() == true)
                        foreach (var file in files)
                        {
                            var restPoint = FileEx.Deserialize<Dictionary<string, List<string>>>(file, true);
                            if (restPoint?.Values.Any() == true)
                                Reg.ImportFile(restPoint.Values.SelectMany(x => x.ToArray()).ToArray());
                            FileEx.TryDelete(file);
                        }
                    DirectoryEx.TryDelete(restPointDir);
                }

                appData.Settings.FileTypeAssoc = default;
                if (quiet)
                    return;

                if (EnvironmentEx.SystemRestore.IsEnabled)
                {
                    var result = MessageBoxEx.Show(Language.GetText(nameof(en_US.RestorePointMsg3)), Resources.GlobalTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        using (var process = ProcessEx.Start(CorePaths.SystemRestore, false))
                            if (!process?.HasExited == true)
                                process.WaitForExit();
                }

                MessageBoxEx.Show(Language.GetText(nameof(en_US.OperationCompletedMsg)), Resources.GlobalTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

                Cancel:
                if (!quiet)
                    MessageBoxEx.Show(Language.GetText(nameof(en_US.OperationCanceledMsg)), Resources.GlobalTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
