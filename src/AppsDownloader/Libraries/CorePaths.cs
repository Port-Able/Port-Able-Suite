namespace AppsDownloader.Libraries
{
    using System;
    using System.IO;
    using SilDev;
    using SilDev.Compression.Archiver;

    internal static class CorePaths
    {
        private static string[] _appDirs;
        private static string _appsDir, _appImages, _appImagesLarge, _appsLauncher, _appsSuiteUpdater, _fileArchiver, _homeDir, _redirectUrl, _tempDir, _transferDir;

        internal static string AppsDir =>
            _appsDir ?? (_appsDir = Path.Combine(HomeDir, "Apps"));

        internal static string[] AppDirs
        {
            get
            {
                if (_appDirs != default)
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
            _appImages ?? (_appImages = Path.Combine(HomeDir, "Assets\\AppImages.dat"));

        internal static string AppImagesLarge =>
            _appImagesLarge ?? (_appImagesLarge = Path.Combine(HomeDir, "Assets\\AppImagesLarge.dat"));

        internal static string AppsLauncher =>
            _appsLauncher ?? (_appsLauncher = Path.Combine(HomeDir, "AppsLauncher.exe"));

        internal static string AppsSuiteUpdater =>
            _appsSuiteUpdater ?? (_appsSuiteUpdater = PathEx.Combine(PathEx.LocalDir, "Updater.exe"));

        internal static string FileArchiver
        {
            get
            {
                if (_fileArchiver != default)
                    return _fileArchiver;
                var path = PathEx.Combine(PathEx.LocalDir, "Helper\\7z");
                if (Environment.Is64BitProcess)
                    path = Path.Combine(path, "x64");
                SevenZip.DefaultArchiver.Location = path;
                _fileArchiver = SevenZip.DefaultArchiver.ExtractExePath;
                return _fileArchiver;
            }
        }

        internal static string HomeDir =>
            _homeDir ?? (_homeDir = PathEx.Combine(PathEx.LocalDir, ".."));

        internal static string RedirectUrl =>
            _redirectUrl ?? (_redirectUrl = "https://transfer.0-9a-z.de/index.php?base=");

        internal static string TempDir
        {
            get
            {
                if (_tempDir != default)
                    return _tempDir;
                _tempDir = Path.Combine(HomeDir, "Documents\\.cache");
                if (Directory.Exists(_tempDir))
                    return _tempDir;
                if (DirectoryEx.Create(_tempDir))
                {
                    using (var process = ProcessEx.Start(AppsLauncher, ActionGuid.RepairDirs, false, false))
                        if (process?.HasExited == false)
                            process.WaitForExit();
                    return _tempDir;
                }
                _tempDir = Path.Combine(Path.GetTempPath(), "Port-Able");
                DirectoryEx.Create(_tempDir);
                return _tempDir;
            }
        }

        internal static string TransferDir =>
            _transferDir ?? (_transferDir = Path.Combine(TempDir, "Transfer"));
    }
}
