namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using LangResources;
    using Microsoft.Win32;
    using PortAble;
    using PortAble.Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;
    using static SilDev.WinApi;

    public static class SystemIntegration
    {
        private const string SysEnvRegPath = "HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment";

        public static string EnvVarName => "AppsSuiteDir";

        public static string CurEnvVarPath => Environment.GetEnvironmentVariable(EnvVarName);

        public static string SysEnvVarPath => Reg.ReadString(SysEnvRegPath, EnvVarName);

        public static bool IsEnabled =>
            !FileEx.Exists(".\\..\\Port-Able-Suite.sln") && PathEx.IsValidPath(SysEnvVarPath);

        public static bool IsAccurate => !IsEnabled || SysEnvVarPath.EqualsEx(CorePaths.HomeDir);

        public static void Apply(bool quiet = false) =>
            ApplyRemove(true, quiet);

        public static void Remove(bool quiet = false) =>
            ApplyRemove(false, quiet);

        public static bool CreateAppShortcut(string longAppName, string destDir)
        {
            var appData = CacheData.FindInCurrentAppInfo(longAppName);
            if (appData == null)
                return false;
            if (!SysEnvVarPath.EqualsEx(CorePaths.HomeDir))
                Environment.SetEnvironmentVariable(EnvVarName, null);
            var exe = appData.ExecutablePath;
            var ini = appData.ConfigPath;
            var lnk = Path.Combine(destDir, longAppName);
            var args = appData.Settings.StartArgsDef;
            return exe.EndsWithEx(".exe")
                ? LocalCreateLink(exe, lnk, args, default)
                : LocalSetFromIco(exe, out var icon) ||
                  LocalSetFromIco(ini, out icon) ||
                  LocalSetFromPng(exe, out icon) ||
                  LocalSetFromPng(ini, out icon)
                    ? LocalCreateLink(exe, lnk, args, icon)
                    : LocalCreateLink(exe, lnk, args, CorePaths.AppsLauncher);

            static bool LocalCreateLink(string path, string link, string args, string icon)
            {
                var result = FileEx.CreateShellLink(path, link, args, icon);
                if (!SysEnvVarPath.EqualsEx(CorePaths.HomeDir))
                    Environment.SetEnvironmentVariable(EnvVarName, CorePaths.HomeDir);
                return result;
            }

            static bool LocalSetFromIco(string source, out string icon)
            {
                icon = default;

                var ico = Path.ChangeExtension(source, ".ico");
                if (!File.Exists(ico))
                    return false;

                icon = ico;
                return true;
            }

            static bool LocalSetFromPng(string source, out string icon)
            {
                icon = default;

                var png = Path.ChangeExtension(source, ".png");
                if (!File.Exists(png))
                    return false;

                var img = default(Image);
                try
                {
                    img = Image.FromFile(png);

                    var ico = Path.ChangeExtension(source, ".ico");
                    IconFactory.Save(img, ico);

                    icon = ico;
                    return true;
                }
                catch (Exception ex) when (ex.IsCaught())
                {
                    Log.Write(ex);
                }
                finally
                {
                    img?.Dispose();
                }

                return false;
            }
        }

        public static void UpdateStartMenuShortcuts(IEnumerable<string> longAppNames, bool waitToComplete)
        {
            var appNames = longAppNames?.ToArray();
            if (appNames?.Length is null or < 1)
                return;

            try
            {
                var startMenuDir = PathEx.Combine(Environment.SpecialFolder.StartMenu, "Programs");
                if (!DirectoryEx.Create(startMenuDir))
                    throw new PathNotFoundException(startMenuDir);

                var shortcutPath = Path.Combine(startMenuDir, "Apps Launcher.lnk");
                var shortcuts = Directory.GetFiles(startMenuDir, "Apps Launcher*.lnk");
                if (shortcuts.Length > 0)
                    foreach (var shortcut in shortcuts)
                        File.Delete(shortcut);
                FileEx.CreateShellLink(CorePaths.AppsLauncher, shortcutPath);

                var appsStartMenuDir = Path.Combine(startMenuDir, "Portable Apps");
                if (!DirectoryEx.Create(appsStartMenuDir))
                    throw new PathNotFoundException(appsStartMenuDir);
                shortcuts = Directory.GetFiles(appsStartMenuDir, "*.lnk");
                if (shortcuts.Length > 0)
                    foreach (var shortcut in shortcuts)
                        File.Delete(shortcut);

                var result = Parallel.ForEach(appNames, name => CreateAppShortcut(name, startMenuDir));
                while (waitToComplete)
                    waitToComplete = !result.IsCompleted;
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
        }

        private static void ApplyRemove(bool apply, bool quiet = false)
        {
            if (!Elevation.IsAdministrator)
            {
                using var process = ProcessEx.Start(CorePaths.AppsLauncher, $"{ActionGuid.SystemIntegration} {apply} {quiet}", true, false);
                if (process?.HasExited == false)
                    process.WaitForExit();
                return;
            }
            var varDir = Reg.ReadString(SysEnvRegPath, EnvVarName);
            var curDir = PathEx.LocalDir;
            if (!apply || !varDir.EqualsEx(curDir))
            {
                var curPath = EnvironmentEx.GetVariableWithPath(CorePaths.AppsLauncher, false);
                var sendToPath = PathEx.Combine(Environment.SpecialFolder.SendTo, "Apps Launcher.lnk");
                if (apply)
                {
                    Reg.Write(SysEnvRegPath, EnvVarName, curDir);
                    FileEx.CreateShellLink(curPath, sendToPath);
                }
                else
                {
                    Reg.RemoveEntry(SysEnvRegPath, EnvVarName);
                    PathEx.ForceDelete(sendToPath);
                }
                if (NativeHelper.SendNotifyMessage((IntPtr)0xffff, (uint)WindowMenuFlags.WmSettingChange, (UIntPtr)0, "Environment"))
                {
                    foreach (var shellKey in new[] { "*", "Folder" }.Select(k => Path.Combine(k, "shell\\portableapps")))
                    {
                        var cmdKey = Path.Combine(shellKey, "command");
                        if (apply)
                        {
                            if (string.IsNullOrWhiteSpace(Reg.ReadString(Registry.ClassesRoot, shellKey, null)))
                                Reg.Write(Registry.ClassesRoot, shellKey, null, Language.GetText(nameof(en_US.shellText)));
                            Reg.Write(Registry.ClassesRoot, shellKey, "Icon", $"\"{CorePaths.AppsLauncher}\"");
                            Reg.Write(Registry.ClassesRoot, cmdKey, null, $"\"{CorePaths.AppsLauncher}\" \"%1\"");
                            continue;
                        }
                        Reg.RemoveSubKey(Registry.ClassesRoot, cmdKey);
                        Reg.RemoveSubKey(Registry.ClassesRoot, shellKey);
                    }
                    var desktopPath = PathEx.Combine(Environment.SpecialFolder.Desktop, "Apps Launcher.lnk");
                    if (apply)
                        FileEx.CreateShellLink(curPath, desktopPath);
                    else
                        PathEx.ForceDelete(sendToPath);

                    if (apply)
                        using (var process = ProcessEx.Start(CorePaths.AppsLauncher, ActionGuid.FileTypeAssociationAll, true, false))
                            if (process?.HasExited == false)
                                process.WaitForExit();

                    if (!quiet)
                        MessageBoxEx.Show(LangStrings.OperationCompletedMsg, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }
            }

            if (!quiet)
                MessageBoxEx.Show(LangStrings.OperationCanceledMsg, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
