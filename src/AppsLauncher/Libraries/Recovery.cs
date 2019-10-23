namespace AppsLauncher.Libraries
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using Microsoft.Win32;
    using Properties;
    using SilDev;

    internal static class Recovery
    {
        internal static bool AppsSuiteIsHealthy(bool repair = true)
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator();

            VersionValidation();

            while (true)
            {
                try
                {
                    if (!File.Exists(CorePaths.AppsDownloader) ||
                        !File.Exists(CorePaths.AppsSuiteUpdater) ||
                        !File.Exists(CorePaths.FileArchiver))
                        throw new FileNotFoundException();
                }
                catch (FileNotFoundException ex)
                {
                    Log.Write(ex);
                    if (!repair)
                        return false;
                    RepairAppsSuite();
                }

                try
                {
                    foreach (var dir in CorePaths.AppDirs.Where(dir => !Directory.Exists(dir)))
                        throw new PathNotFoundException(dir);
                }
                catch (PathNotFoundException ex)
                {
                    Log.Write(ex);
                    if (!repair)
                        return false;
                    RepairAppsSuiteDirs();
                }

                try
                {
                    var envDir = EnvironmentEx.GetVariableValue(Settings.EnvironmentVariable);
                    if (!Settings.DeveloperVersion && !string.IsNullOrWhiteSpace(envDir) && !envDir.EqualsEx(PathEx.LocalDir))
                        throw new ArgumentInvalidException(nameof(envDir));
                }
                catch (ArgumentInvalidException ex)
                {
                    Log.Write(ex);
                    if (!repair)
                        return false;
                    RepairEnvironmentVariable();
                }

                if (!repair)
                    return true;
                repair = false;
            }
        }

        internal static void RepairAppsSuite()
        {
            if (File.Exists(CorePaths.AppsSuiteUpdater) && File.Exists(CorePaths.FileArchiver))
                ProcessEx.Start(CorePaths.AppsSuiteUpdater);
            else
            {
                Language.ResourcesNamespace = typeof(Program).Namespace;
                if (MessageBox.Show(Language.GetText(nameof(en_US.RequirementsErrorMsg)), Resources.GlobalTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    var path = PathEx.AltCombine(CorePaths.RepositoryUrl, "releases/latest");
                    Process.Start(path);
                }
            }
            Environment.ExitCode = 1;
            Environment.Exit(Environment.ExitCode);
        }

        internal static void RepairAppsSuiteDirs()
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator(ActionGuid.RepairDirs);

            foreach (var dirs in new[]
            {
                CorePaths.AppDirs,
                CorePaths.UserDirs
            })
                foreach (var dir in dirs)
                    if (!DirectoryEx.Create(dir))
                        Elevation.RestartAsAdministrator(ActionGuid.RepairDirs);

            var iniMap = new[]
            {
                new[]
                {
                    CorePaths.AppDirs.First(),
                    "IconResource=..\\Assets\\FolderIcons.dll,3"
                },
                new[]
                {
                    CorePaths.AppDirs.Second(),
                    "LocalizedResourceName=\"Port-Able\" - Freeware",
                    "IconResource=..\\..\\Assets\\FolderIcons.dll,4"
                },
                new[]
                {
                    CorePaths.AppDirs.Third(),
                    "LocalizedResourceName=\"PortableApps\" - Repacks",
                    "IconResource=..\\..\\Assets\\FolderIcons.dll,2"
                },
                new[]
                {
                    CorePaths.AppDirs.Last(),
                    "LocalizedResourceName=\"Custom\" - Shareware",
                    "IconResource=..\\..\\Assets\\FolderIcons.dll,1"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Assets"),
                    "IconResource=FolderIcons.dll,5"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Binaries"),
                    "IconResource=..\\Assets\\FolderIcons.dll,5"
                },
                new[]
                {
                    CorePaths.UserDirs.First(),
                    "LocalizedResourceName=Profile",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,117",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-235"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Documents", ".cache"),
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,112"
                },
                new[]
                {
                    CorePaths.UserDirs.Second(),
                    "LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-21770",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,-112",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-235"
                },
                new[]
                {
                    CorePaths.UserDirs.Third(),
                    "LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-21790",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,-108",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-237",
                    "InfoTip=@%SystemRoot%\\system32\\shell32.dll,-12689"
                },
                new[]
                {
                    CorePaths.UserDirs.Fourth(),
                    "LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-21779",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,-113",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-236",
                    "InfoTip=@%SystemRoot%\\system32\\shell32.dll,-12688"
                },
                new[]
                {
                    CorePaths.UserDirs.Last(),
                    "LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-21791",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,-189",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-238",
                    "InfoTip=@%SystemRoot%\\system32\\shell32.dll,-12690"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Help"),
                    "IconResource=..\\Assets\\FolderIcons.dll,4"
                }
            };
            for (var i = 0; i < iniMap.Length; i++)
            {
                var array = iniMap[i];
                var dir = array.FirstOrDefault();
                if (!PathEx.IsValidPath(dir) || i >= iniMap.Length - 2 && !Directory.Exists(dir))
                    continue;
                if (!Elevation.WritableLocation(dir))
                    Elevation.RestartAsAdministrator(ActionGuid.RepairDirs);
                var path = PathEx.Combine(dir, "desktop.ini");
                foreach (var str in array.Skip(1))
                {
                    var ent = str?.Split('=');
                    if (ent?.Length != 2)
                        continue;
                    var key = ent.FirstOrDefault();
                    if (string.IsNullOrEmpty(key))
                        continue;
                    var val = ent.LastOrDefault();
                    if (string.IsNullOrEmpty(val))
                        continue;
                    Ini.WriteDirect(".ShellClassInfo", key, val, path);
                }
                FileEx.SetAttributes(path, FileAttributes.System | FileAttributes.Hidden);
                DirectoryEx.SetAttributes(dir, FileAttributes.ReadOnly);
            }
        }

        internal static void RepairEnvironmentVariable()
        {
            if (!Elevation.IsAdministrator)
            {
                using (var process = ProcessEx.Start(PathEx.LocalPath, ActionGuid.RepairVariable, true, false))
                    if (process?.HasExited == false)
                        process.WaitForExit();
                return;
            }
            if (!SystemIntegration.IsAccurate)
                SystemIntegration.Enable(true, true);
        }

        internal static void VersionValidation()
        {
            if (Settings.DeveloperVersion || Settings.LastUpdateCheck == DateTime.MinValue || Settings.VersionValidation == AssemblyInfo.Version)
                return;

            Settings.VersionValidation = AssemblyInfo.Version;
            Settings.WriteToFile();

            RepairAppsSuiteDirs();

            try
            {
                var subKeys = new[]
                {
                    "*",
                    "Folder"
                };
                if (subKeys.Select(x => Path.Combine(x, "shell\\portableapps")).Any(varKey => Reg.SubKeyExists(Registry.ClassesRoot, varKey)))
                    SystemIntegration.Enable(true, true);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }

            var path = PathEx.Combine(PathEx.LocalDir, "UpdateCompletion.bat");
            if (File.Exists(path))
                ProcessEx.Start(path);

            Environment.ExitCode = 0;
            Environment.Exit(Environment.ExitCode);
        }
    }
}
