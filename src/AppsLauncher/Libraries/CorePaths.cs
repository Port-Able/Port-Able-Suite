namespace AppsLauncher.Libraries
{
    using System;
    using System.IO;
    using System.Linq;
    using SilDev;

    internal static class CorePaths
    {
        private static string[] _appDirs;
        private static string _appsDir, _appImages, _appsDownloader, _appsSuiteUpdater, _fileArchiver, _homeDir, _restorePointDir, _repositoryPath, _systemExplorer, _systemRestore, _tempDir;
        private static string[][] _fullAppsSuitePathMap;

        internal static string AppsDir =>
            _appsDir ?? (_appsDir = Path.Combine(HomeDir, "Apps"));

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

        internal static string AppImages =>
            _appImages ?? (_appImages = Path.Combine(HomeDir, "Assets", Settings.Window.LargeImages ? "AppImagesLarge.dat" : "AppImages.dat"));

        internal static string AppsDownloader =>
            _appsDownloader ?? (_appsDownloader = Path.Combine(HomeDir, "Binaries\\AppsDownloader.exe"));

        internal static string AppsSuiteUpdater =>
            _appsSuiteUpdater ?? (_appsSuiteUpdater = Path.Combine(HomeDir, "Binaries\\Updater.exe"));

        internal static string FileArchiver
        {
            get
            {
                if (_fileArchiver != default)
                    return _fileArchiver;
                var path = PathEx.Combine(HomeDir, "Binaries\\Helper\\7z");
                if (Environment.Is64BitProcess)
                    path = Path.Combine(path, "x64");
                Compaction.SevenZipHelper.Location = path;
                _fileArchiver = Compaction.SevenZipHelper.FilePath;
                return _fileArchiver;
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
                        Path.Combine(HomeDir, "Binaries\\Helper\\7z\\x64\\7zG.exe"),
                        Path.Combine(HomeDir, "Binaries\\Helper\\7z\\7z.dll"),
                        Path.Combine(HomeDir, "Binaries\\Helper\\7z\\x64\\7z.dll")
                    }
                };
                return _fullAppsSuitePathMap;
            }
        }

        internal static string HomeDir =>
            _homeDir ?? (_homeDir = PathEx.Combine(PathEx.LocalDir));

        internal static string RepositoryUrl =>
            _repositoryPath ?? (_repositoryPath = "https://github.com/Port-Able/Port-Able-Suite");

        internal static string RestorePointDir =>
            _restorePointDir ?? (_restorePointDir = Path.Combine(TempDir, "FileTypeAssoc", Settings.SystemInstallId));

        internal static string SystemExplorer =>
            _systemExplorer ?? (_systemExplorer = PathEx.Combine(Environment.SpecialFolder.Windows, "explorer.exe"));

        internal static string SystemRestore =>
            _systemRestore ?? (_systemRestore = PathEx.Combine(Environment.SpecialFolder.System, "rstrui.exe"));

        internal static string TempDir
        {
            get
            {
                if (_tempDir != default)
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
