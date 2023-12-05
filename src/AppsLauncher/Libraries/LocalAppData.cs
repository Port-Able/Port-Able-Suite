namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using LangResources;
    using Properties;
    using SilDev;
    using SilDev.Forms;
    using SilDev.Legacy;
    using GlobalSettings = Settings;

    [Serializable]
    public class LocalAppData : ISerializable
    {
        private string _fileDir, _filePath, _configPath, _appInfoPath;

        [NonSerialized]
        private LocalAppSettings _settings;

        public string Key { get; }

        public string Name { get; }

        public string FileDir
        {
            get => _fileDir;
            private set => _fileDir = GetFullPath(value);
        }

        public string FilePath
        {
            get => _filePath;
            private set => _filePath = GetFullPath(value);
        }

        public string ConfigPath
        {
            get => _configPath;
            private set => _configPath = GetFullPath(value);
        }

        public string AppInfoPath
        {
            get => _appInfoPath;
            private set => _appInfoPath = GetFullPath(value);
        }

        public LocalAppSettings Settings => _settings ??= new LocalAppSettings(Key);

        public LocalAppData(string key, string name, string fileDir, string filePath, string configPath, string appInfoPath)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(fileDir))
                throw new ArgumentNullException(nameof(fileDir));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentNullException(nameof(configPath));

            if (string.IsNullOrWhiteSpace(appInfoPath))
                throw new ArgumentNullException(nameof(appInfoPath));

            Key = key;
            Name = name;

            FileDir = fileDir;
            FilePath = filePath;

            ConfigPath = configPath;
            AppInfoPath = appInfoPath;
        }

        protected LocalAppData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            if (Log.DebugMode > 1)
                Log.Write($"{nameof(LocalAppData)}.ctor({nameof(SerializationInfo)}, {nameof(StreamingContext)}) => info: {Json.Serialize(context)}, context: {Json.Serialize(context)}");

            Key = info.GetString(nameof(Key));
            Name = info.GetString(nameof(Name));

            FileDir = info.GetString(nameof(FileDir));
            FilePath = info.GetString(nameof(FilePath));

            ConfigPath = info.GetString(nameof(ConfigPath));
            AppInfoPath = info.GetString(nameof(AppInfoPath));
        }

        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            if (Log.DebugMode > 1)
                Log.Write($"{nameof(LocalAppData)}.ctor({nameof(SerializationInfo)}, {nameof(StreamingContext)}) => info: {Json.Serialize(context)}, context: {Json.Serialize(context)}");

            info.AddValue(nameof(Key), Key);
            info.AddValue(nameof(Name), Name);

            info.AddValue(nameof(FileDir), GetSubPath(FileDir));
            info.AddValue(nameof(FilePath), GetSubPath(FilePath));

            info.AddValue(nameof(ConfigPath), GetSubPath(ConfigPath));
            info.AddValue(nameof(AppInfoPath), GetSubPath(AppInfoPath));
        }

        public void StartApplication(bool closeLauncher = false, bool runAsAdmin = false)
        {
            if (!runAsAdmin)
                runAsAdmin = Settings.RunAsAdmin;

            var srcConfigPath = Path.Combine(FileDir, "Other", "Source", "AppNamePortable.ini");
            if (!File.Exists(srcConfigPath))
            {
                var configName = Path.GetFileName(ConfigPath);
                if (!string.IsNullOrEmpty(configName))
                    srcConfigPath = Path.Combine(FileDir, "Other", "Source", configName);
            }
            if (File.Exists(srcConfigPath))
                FileEx.Copy(srcConfigPath, ConfigPath);

            foreach (var file in DirectoryEx.EnumerateFiles(FileDir, "*.ini"))
            {
                var content = FileEx.ReadAllText(file);
                if (!Regex.IsMatch(content, "DisableSplashScreen.*=.*false", RegexOptions.IgnoreCase))
                    continue;
                content = Regex.Replace(content, "DisableSplashScreen.*=.*false", "DisableSplashScreen=true", RegexOptions.IgnoreCase);
                FileEx.WriteAllText(file, content);
            }

            var arguments = Ini.Read("AppInfo", "Arg", ConfigPath);
            if (Arguments.ValidPaths.Any())
            {
                if (Settings.SortArgPaths)
                {
                    var comparer = new AlphaNumericComparer<string>();
                    Arguments.ValidPaths = Arguments.ValidPaths.OrderBy(x => x, comparer).ToList();
                }
                if (string.IsNullOrWhiteSpace(arguments))
                    arguments = $"{Settings.StartArgsFirst}{Arguments.ValidPathsStr}{Settings.StartArgsLast}".Trim();
                var curKey = Arguments.ValidPathsStr?.GetHashCode() ?? -1;
                var newValue = Key.GetHashCode();
                CacheData.UpdateCurrentTypeDataFile(curKey, newValue);
                GlobalSettings.LastItem = Name;
            }

            if (Arguments.FileTypes.Any())
            {
                var types = Arguments.FileTypes.Where(x => !Arguments.SavedFileTypes.Contains(x)).ToList();
                if (types.Any())
                {
                    var question = types.Count == 1 ? Language.GetText(nameof(en_US.AssociateQuestionMsg0)) : string.Format(CultureInfo.InvariantCulture, Language.GetText(nameof(en_US.AssociateQuestionMsg1)), types.Join("; "));
                    if (MessageBoxEx.Show(question, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        types = types.Select(x => $".{x.ToLowerInvariant()}").ToList();
                        Settings.FileTypes.ForEach(x =>
                        {
                            if (types.Any(y => y.TrimStart('.').EqualsEx(x.TrimStart('.'))))
                                return;
                            types.Add(x);
                        });
                        Settings.FileTypes = new Collection<string>(types);
                    }
                }
            }

            ProcessEx.Start(FilePath, arguments, runAsAdmin);
            if (closeLauncher)
                Application.Exit();
        }

        public bool Equals(LocalAppData appData) =>
            Equals(GetHashCode(), appData?.GetHashCode());

        public override bool Equals(object obj)
        {
            if (obj is LocalAppData appData)
                return Equals(appData);
            return false;
        }

        public override int GetHashCode() =>
            Tuple.Create(Key, Name, FileDir, FilePath, ConfigPath, AppInfoPath).GetHashCode();

        internal void OpenLocation(bool closeLauncher = false)
        {
            ProcessEx.Start(CorePaths.SystemExplorer, FileDir);
            if (closeLauncher)
                Application.Exit();
        }

        internal bool RemoveApplication(IWin32Window owner = default)
        {
            CacheData.ResetCurrent();
            Retry:
            if (GlobalSettings.AppDirs.Any(x => FileDir.StartsWithEx(x)) && Directory.Exists(FileDir))
            {
                if (!DirectoryEx.TryDelete(FileDir) && !PathEx.ForceDelete(FileDir) && !PathEx.ForceDelete(FileDir, true))
                    goto Failed;
                if (Ini.GetSections().ContainsItem(Key))
                {
                    Ini.RemoveSection(Key);
                    GlobalSettings.WriteToFileInQueue = true;
                }
                MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.OperationCompletedMsg)), Resources.GlobalTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return !Directory.Exists(FileDir);
            }
            Failed:
            if (MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.OperationFailedMsg)), Resources.GlobalTitle, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                goto Retry;
            return false;
        }

        private static string GetSubPath(string path) =>
            path.StartsWithEx(PathEx.LocalDir) ? path.Substring(PathEx.LocalDir.Length).Trim(Path.DirectorySeparatorChar) : path;

        private static string GetFullPath(string path) =>
            !PathEx.IsValidPath(path) ? PathEx.Combine(PathEx.LocalDir, path) : path;

        public static bool operator ==(LocalAppData left, LocalAppData right) =>
            Equals(left, right);

        public static bool operator !=(LocalAppData left, LocalAppData right) =>
            !Equals(left, right);
    }
}
