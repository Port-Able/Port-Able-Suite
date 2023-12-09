namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using SilDev;
    using SilDev.Compression.Archiver;

    public static class CorePaths
    {
        private static string _homeDir, _fileArchiver, _tempDir;

        private static string[][] _fullAppsSuitePathMap;

        /// <summary>
        ///     The apps suite root directory.
        /// </summary>
        public static string HomeDir
        {
            get
            {
                if (_homeDir != default)
                    return _homeDir;
                var dir = PathEx.LocalDir;
                var name = Path.GetFileName(dir);
                return _homeDir = name.EqualsEx("Binaries") ? PathEx.Combine(dir, "..") : dir;
            }
        }

        /// <summary>
        ///     The path to the directory used for temporary files and cached data.
        /// </summary>
        public static string DataDir
        {
            get
            {
                if (_tempDir != default)
                    return _tempDir;
                _tempDir = Path.Combine(HomeDir, "Documents\\Cache");
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

        /// <summary>
        ///     The root directory where installed apps are located.
        /// </summary>
        public static string AppsDir { get; } = Path.Combine(HomeDir, "Apps");

        /// <summary>
        ///     A collection of subdirectories where installed apps are located.
        /// </summary>
        public static ReadOnlyCollection<string> AppDirs { get; } = new(new[]
        {
            AppsDir,
            Path.Combine(AppsDir, ".free"),
            Path.Combine(AppsDir, ".repack"),
            Path.Combine(AppsDir, ".share")
        });

        /// <summary>
        ///     The path to the default database file where small app images are stored.
        /// </summary>
        public static string AppImages { get; } = Path.Combine(HomeDir, "Assets\\AppImages.dat");

        /// <summary>
        ///     The path to the default database file where large app images are stored.
        /// </summary>
        public static string AppImagesLarge { get; } = Path.Combine(HomeDir, "Assets\\AppImagesLarge.dat");

        /// ReSharper disable CommentTypo
        /// <summary>
        ///     The path to the default database file that stores the logic of the Nullsoft
        ///     Scriptable Install System (NSIS) buttons used by some app installers.
        /// </summary>
        public static string NsisButtons { get; } = Path.Combine(HomeDir, "Assets\\NsisButtons.dat");

        /// <summary>
        ///     The path to the apps suite launcher executable.
        /// </summary>
        public static string AppsLauncher { get; } = Path.Combine(HomeDir, "AppsLauncher.exe");

        /// <summary>
        ///     The path to the apps downloader executable.
        /// </summary>
        public static string AppsDownloader { get; } = Path.Combine(HomeDir, "Binaries\\AppsDownloader.exe");

        /// <summary>
        ///     The path to the apps suite updater executable.
        /// </summary>
        public static string AppsSuiteUpdater { get; } = Path.Combine(HomeDir, "Binaries\\Updater.exe");

        /// <summary>
        ///     The path to the `7-Zip GUI` executable file used to install some apps.
        /// </summary>
        public static string FileArchiver
        {
            get
            {
                if (_fileArchiver != default)
                    return _fileArchiver;
                var path = PathEx.Combine(CorePaths.HomeDir, "Binaries\\Helper\\7z");
                if (Environment.Is64BitProcess)
                    path = Path.Combine(path, "x64");
                SevenZip.DefaultArchiver.Location = path;
                return _fileArchiver = SevenZip.DefaultArchiver.ExtractExePath;
            }
        }

        public static string[][] FullAppsSuitePathMap
        {
            get
            {
                if (_fullAppsSuitePathMap != default)
                    return _fullAppsSuitePathMap;
                return _fullAppsSuitePathMap = new[]
                {
                    new[]
                    {
                        Path.Combine(HomeDir, "AppsLauncher.exe"),
                        AppsDownloader,
                        AppsSuiteUpdater
                    },
                    new[]
                    {
                        Path.Combine(HomeDir, "Binaries\\SilDev.CSharpLib.dll"),
                    },
                    new[]
                    {
                        Path.Combine(HomeDir, "Binaries\\Newtonsoft.Json.dll"),
                        Path.Combine(HomeDir, Environment.Is64BitProcess ? "Binaries\\Helper\\7z\\x64\\7zG.exe" : "Binaries\\Helper\\7z\\7zG.exe"),
                        Path.Combine(HomeDir, Environment.Is64BitProcess ? "Binaries\\Helper\\7z\\x64\\7z.dll" : "Binaries\\Helper\\7z\\7z.dll")
                    }
                };
            }
        }

        public static string[] UserDirs { get; } =
        {
            Path.Combine(HomeDir, "Documents"),
            Path.Combine(HomeDir, "Documents\\Documents"),
            Path.Combine(HomeDir, "Documents\\Music"),
            Path.Combine(HomeDir, "Documents\\Pictures"),
            Path.Combine(HomeDir, "Documents\\Videos")
        };

        /// <summary>
        ///     The URL for a download redirection service, which is used in the rare case
        ///     when the client only has an IPv6 connection and the server only offers an
        ///     IPv4 connection.
        /// </summary>
        public static string RedirectUrl => "https://transfer.0-9a-z.de/index.php?base=";

        public static string RepositoryUrl => "https://github.com/Port-Able/Port-Able-Suite";

        public static string RestorePointDir { get; } = Path.Combine(DataDir, "FileTypeAssoc", Settings.SystemInstallId);

        public static string SystemExplorer { get; } = PathEx.Combine(Environment.SpecialFolder.Windows, "explorer.exe");

        public static string SystemRestore { get; } = PathEx.Combine(Environment.SpecialFolder.System, "rstrui.exe");


        /// <summary>
        ///     The default directory for downloaded files.
        /// </summary>
        public static string TransferDir { get; } = Path.Combine(DataDir, "Transfer");
    }
}
