namespace AppsLauncher.Libraries
{
    using System;
    using System.IO;
    using System.Linq;
    using SilDev;

    internal static class CorePaths
    {
        private static string[] _appDirs;
        private static string _appsDir, _appImages, _appsDownloader, _appsSuiteUpdater, _archiver, _homeDir, _restorePointDir, _repositoryPath, _systemExplorer, _systemRestore, _tempDir;
        private static string[][] _fullAppsSuitePathMap;

        internal static string AppsDir
        {
            get
            {
                if (_appsDir == default(string))
                    _appsDir = Path.Combine(HomeDir, "Apps");
                return _appsDir;
            }
        }

        internal static string[] AppDirs
        {
            get
            {
                if (_appDirs != default(string[]))
                    return _appDirs;
                _appDirs = new[]
                {
                    AppsDir,
                    Path.Combine(AppsDir, ".free"),
                    Path.Combine(AppsDir, ".repack"),
                    Path.Combine(AppsDir, ".share")
                };
                return _appDirs;
            }
        }

        internal static string AppImages
        {
            get
            {
                if (_appImages == default(string))
                    _appImages = Path.Combine(HomeDir, "Assets\\AppImages.dat");
                return _appImages;
            }
        }

        internal static string AppsDownloader
        {
            get
            {
                if (_appsDownloader == default(string))
#if x86
                    _appsDownloader = Path.Combine(HomeDir, "Binaries\\AppsDownloader.exe");
#else
                    _appsDownloader = Path.Combine(HomeDir, "Binaries\\AppsDownloader64.exe");
#endif
                return _appsDownloader;
            }
        }

        internal static string AppsSuiteUpdater
        {
            get
            {
                if (_appsSuiteUpdater == default(string))
                    _appsSuiteUpdater = Path.Combine(HomeDir, "Binaries\\Updater.exe");
                return _appsSuiteUpdater;
            }
        }

        internal static string FileArchiver
        {
            get
            {
                if (_archiver != default(string))
                    return _archiver;
#if x86
                Compaction.SevenZipHelper.Location = Path.Combine(HomeDir, "Binaries\\Helper\\7z");
#else
                Compaction.SevenZipHelper.Location = Path.Combine(HomeDir, "Binaries\\Helper\\7z\\x64");
#endif
                _archiver = Compaction.SevenZipHelper.FilePath;
                return _archiver;
            }
        }

        internal static string[][] FullAppsSuitePathMap
        {
            get
            {
                if (_fullAppsSuitePathMap != default(string[][]))
                    return _fullAppsSuitePathMap;
                _fullAppsSuitePathMap = new[]
                {
                    new[]
                    {
                        Path.Combine(HomeDir, "AppsLauncher.exe"),
                        Path.Combine(HomeDir, "AppsLauncher64.exe"),
                        Path.Combine(HomeDir, "Binaries\\AppsDownloader.exe"),
                        Path.Combine(HomeDir, "Binaries\\AppsDownloader64.exe"),
                        Path.Combine(HomeDir, "Binaries\\Updater.exe")
                    },
                    new[]
                    {
                        Path.Combine(HomeDir, "Binaries\\SilDev.CSharpLib.dll"),
                        Path.Combine(HomeDir, "Binaries\\SilDev.CSharpLib64.dll")
                    },
                    new[]
                    {
                        Path.Combine(HomeDir, "Binaries\\Helper\\7z\\7zG.exe"),
                        Path.Combine(HomeDir, "Binaries\\Helper\\7z\\x64\\7zG.exe")
                    }
                };
                return _fullAppsSuitePathMap;
            }
        }

        internal static string HomeDir
        {
            get
            {
                if (_homeDir == default(string))
                    _homeDir = PathEx.Combine(PathEx.LocalDir);
                return _homeDir;
            }
        }

        internal static string RepositoryUrl
        {
            get
            {
                if (_repositoryPath == default(string))
                    _repositoryPath = "https://github.com/Port-Able/Port-Able-Suite";
                return _repositoryPath;
            }
        }

        internal static string RestorePointDir
        {
            get
            {
                if (_restorePointDir == default(string))
                    _restorePointDir = Path.Combine(TempDir, "FileTypeAssoc", Settings.SystemInstallId);
                return _restorePointDir;
            }
        }

        internal static string SystemExplorer
        {
            get
            {
                if (_systemExplorer == default(string))
                    _systemExplorer = PathEx.Combine(Environment.SpecialFolder.Windows, "explorer.exe");
                return _systemExplorer;
            }
        }

        internal static string SystemRestore
        {
            get
            {
                if (_systemRestore == default(string))
                    _systemRestore = PathEx.Combine(Environment.SpecialFolder.System, "rstrui.exe");
                return _systemRestore;
            }
        }

        internal static string TempDir
        {
            get
            {
                if (_tempDir != default(string))
                    return _tempDir;
                _tempDir = Path.Combine(UserDirs.First(), ".cache");
                if (Directory.Exists(_tempDir))
                    return _tempDir;
                if (DirectoryEx.Create(_tempDir))
                {
                    Recovery.RepairAppsSuiteDirs();
                    return _tempDir;
                }
                _tempDir = EnvironmentEx.GetVariableValue("TEMP");
                return _tempDir;
            }
        }

        internal static string[] UserDirs { get; } =
        {
            Path.Combine(HomeDir, "Documents"),
            Path.Combine(HomeDir, "Documents\\Documents"),
            Path.Combine(HomeDir, "Documents\\Music"),
            Path.Combine(HomeDir, "Documents\\Pictures"),
            Path.Combine(HomeDir, "Documents\\Videos")
        };
    }
}
