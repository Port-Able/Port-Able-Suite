namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using LangResources;
    using Microsoft.Win32;
    using SilDev;

    internal static class SystemIntegration
    {
        internal static bool IsEnabled =>
            !Settings.DeveloperVersion && !string.IsNullOrEmpty(EnvironmentEx.GetVariableValue(Settings.EnvironmentVariable));

        internal static bool IsAccurate =>
            Settings.DeveloperVersion || !IsEnabled || EnvironmentEx.GetVariableValue(Settings.EnvironmentVariable).EqualsEx(PathEx.LocalDir);

        internal static void Enable(bool enabled, bool quiet = false)
        {
            if (!Elevation.IsAdministrator)
            {
                using (var process = ProcessEx.Start(PathEx.LocalPath, $"{ActionGuid.SystemIntegration} {enabled} {quiet}", true, false))
                    if (process?.HasExited == false)
                        process.WaitForExit();
                return;
            }
            var varKey = "SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment";
            var varDir = Reg.ReadString(Registry.LocalMachine, varKey, Settings.EnvironmentVariable);
            var curDir = PathEx.LocalDir;
            if (!enabled || !varDir.EqualsEx(curDir))
            {
                var curPath = EnvironmentEx.GetVariablePathFull(PathEx.LocalPath, false);
                var sendToPath = PathEx.Combine(Environment.SpecialFolder.SendTo, "Apps Launcher.lnk");
                if (enabled)
                {
                    Reg.Write(Registry.LocalMachine, varKey, Settings.EnvironmentVariable, curDir);
                    FileEx.CreateShellLink(curPath, sendToPath);
                }
                else
                {
                    Reg.RemoveEntry(Registry.LocalMachine, varKey, Settings.EnvironmentVariable);
                    PathEx.ForceDelete(sendToPath);
                }
                if (WinApi.NativeHelper.SendNotifyMessage((IntPtr)0xffff, (uint)WinApi.WindowMenuFlags.WmSettingChange, (UIntPtr)0, "Environment"))
                {
                    foreach (var baseKey in new[]
                    {
                        "*",
                        "Folder"
                    })
                    {
                        varKey = Path.Combine(baseKey, "shell\\portableapps");
                        if (enabled)
                        {
                            if (string.IsNullOrWhiteSpace(Reg.ReadString(Registry.ClassesRoot, varKey, null)))
                                Reg.Write(Registry.ClassesRoot, varKey, null, Language.GetText(nameof(en_US.shellText)));
                            Reg.Write(Registry.ClassesRoot, varKey, "Icon", $"\"{PathEx.LocalPath}\"");
                        }
                        else
                            Reg.RemoveSubKey(Registry.ClassesRoot, varKey);
                        varKey = Path.Combine(varKey, "command");
                        if (enabled)
                            Reg.Write(Registry.ClassesRoot, varKey, null, $"\"{PathEx.LocalPath}\" \"%1\"");
                        else
                            Reg.RemoveSubKey(Registry.ClassesRoot, varKey);
                    }
                    var desktopPath = PathEx.Combine(Environment.SpecialFolder.Desktop, "Apps Launcher.lnk");
                    if (enabled)
                        FileEx.CreateShellLink(curPath, desktopPath);
                    else
                        PathEx.ForceDelete(sendToPath);

                    if (enabled)
                        using (var process = ProcessEx.Start(PathEx.LocalPath, ActionGuid.FileTypeAssociationAll, true, false))
                            if (process?.HasExited == false)
                                process.WaitForExit();

                    if (!quiet)
                        MessageBox.Show(Language.GetText(nameof(en_US.OperationCompletedMsg)), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            if (!quiet)
                MessageBox.Show(Language.GetText(nameof(en_US.OperationCanceledMsg)), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal static void UpdateStartMenuShortcuts(IEnumerable<string> longAppNames)
        {
            var appNames = longAppNames?.ToArray();
            if (appNames?.Any() != true)
                return;
            try
            {
                var startMenuDir = PathEx.Combine(Environment.SpecialFolder.StartMenu, "Programs");
                var shortcutPath = Path.Combine(startMenuDir, "Apps Launcher.lnk");
                if (Directory.Exists(startMenuDir))
                {
                    var shortcuts = Directory.GetFiles(startMenuDir, "Apps Launcher*.lnk", SearchOption.TopDirectoryOnly);
                    if (shortcuts.Length > 0)
                        foreach (var shortcut in shortcuts)
                            File.Delete(shortcut);
                }
                if (!Directory.Exists(startMenuDir))
                    Directory.CreateDirectory(startMenuDir);
                FileEx.CreateShellLink(EnvironmentEx.GetVariablePathFull(PathEx.LocalPath, false), shortcutPath);
                startMenuDir = Path.Combine(startMenuDir, "Portable Apps");
                if (Directory.Exists(startMenuDir))
                {
                    var shortcuts = Directory.GetFiles(startMenuDir, "*.lnk", SearchOption.TopDirectoryOnly);
                    if (shortcuts.Length > 0)
                        foreach (var shortcut in shortcuts)
                            File.Delete(shortcut);
                }
                if (!Directory.Exists(startMenuDir))
                    Directory.CreateDirectory(startMenuDir);
                Parallel.ForEach(appNames, x => FileEx.CreateShellLink(EnvironmentEx.GetVariablePathFull(CacheData.FindAppData(x)?.FilePath, false, false), Path.Combine(startMenuDir, x)));
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
        }
    }
}
