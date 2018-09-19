namespace Updater.Libraries
{
    using System;
    using System.IO;
    using SilDev;

    internal static class CorePaths
    {
        private static string _appsLauncher, _fileArchiverDir, _homeDir, _repoReleasesUrl, _repoSnapshotsUrl, _tempDir;
        private static string[] _fileArchiverFiles;

        internal static string AppsLauncher
        {
            get
            {
                if (_appsLauncher == default(string))
                    _appsLauncher = Path.Combine(PathEx.LocalDir, Environment.Is64BitOperatingSystem ? "AppsLauncher64.exe" : "AppsLauncher.exe");
                return _appsLauncher;
            }
        }

        internal static string FileArchiverDir
        {
            get
            {
                if (_fileArchiverDir == default(string))
                    _fileArchiverDir = Path.Combine(PathEx.LocalDir, Environment.Is64BitOperatingSystem ? "Helper\\7z\\x64" : "Helper\\7z");
                return _fileArchiverDir;
            }
        }

        internal static string[] FileArchiverFiles
        {
            get
            {
                if (_fileArchiverFiles == default(string[]))
                    _fileArchiverFiles = new[]
                    {
                        Path.Combine(FileArchiverDir, "7zG.exe"),
                        Path.Combine(FileArchiverDir, "7z.dll")
                    };
                return _fileArchiverFiles;
            }
        }

        internal static string HomeDir
        {
            get
            {
                if (_homeDir == default(string))
                    _homeDir = PathEx.Combine(PathEx.LocalDir, "..");
                return _homeDir;
            }
        }

        internal static string PortAbleUrl => "http://www.port-a.de";

        internal static string RepoProfileUrl => "https://github.com/Port-Able";

        internal static string RepoReleasesUrl
        {
            get
            {
                if (_repoReleasesUrl == default(string))
                    _repoReleasesUrl = PathEx.AltCombine(RepoProfileUrl, "Port-Able-Suite/releases");
                return _repoReleasesUrl;
            }
        }

        internal static string RepoSnapshotsUrl
        {
            get
            {
                if (_repoSnapshotsUrl == default(string))
                    _repoSnapshotsUrl = PathEx.AltCombine(RepoProfileUrl, "Port-Able-Suite-Snapper/raw/master");
                return _repoSnapshotsUrl;
            }
        }

        internal static string TempDir
        {
            get
            {
                if (_tempDir != default(string))
                    return _tempDir;
                _tempDir = Path.Combine(HomeDir, "Documents", ".cache");
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

        internal static string VirusTotalUrl => "https://www.virustotal.com/#/file/{0}/";
    }
}
