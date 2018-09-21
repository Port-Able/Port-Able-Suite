namespace Updater.Libraries
{
    using System;
    using System.IO;
    using SilDev;

    internal static class CorePaths
    {
        private static string _appsLauncher, _fileArchiver, _homeDir, _repoCommitsUrl, _repoReleasesUrl, _repoSnapshotsUrl, _tempDir;

        internal static string AppsLauncher
        {
            get
            {
                if (_appsLauncher == default(string))
                    _appsLauncher = Path.Combine(PathEx.LocalDir, Environment.Is64BitOperatingSystem ? "AppsLauncher64.exe" : "AppsLauncher.exe");
                return _appsLauncher;
            }
        }

        internal static string FileArchiver
        {
            get
            {
                if (_fileArchiver != default(string))
                    return _fileArchiver;
                Compaction.SevenZipHelper.Location = Path.Combine(PathEx.LocalDir, Environment.Is64BitOperatingSystem ? "Helper\\7z\\x64" : "Helper\\7z");
                if (string.IsNullOrEmpty(Compaction.SevenZipHelper.FilePath))
                    Network.DownloadArchiver();
                else
                    foreach (var file in DirectoryEx.EnumerateFiles(Compaction.SevenZipHelper.Location))
                    {
                        var name = Path.GetFileName(file);
                        if (string.IsNullOrEmpty(name))
                            continue;
                        var path = Path.Combine(CachePaths.UpdateDir, name);
                        FileEx.Copy(file, path);
                    }
                Compaction.SevenZipHelper.Location = CachePaths.UpdateDir;
                _fileArchiver = Compaction.SevenZipHelper.FilePath;
                return _fileArchiver;
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

        internal static string RepoCommitsUrl
        {
            get
            {
                if (_repoCommitsUrl == default(string))
                    _repoCommitsUrl = PathEx.AltCombine(RepoProfileUrl, "Port-Able-Suite/commits/master.atom");
                return _repoCommitsUrl;
            }
        }

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
                    using (var process = ProcessEx.Start(AppsLauncher, ActionGuid.RepairDirs, Elevation.IsAdministrator, false))
                        if (process?.HasExited == false)
                            process.WaitForExit();
                    return _tempDir;
                }
                _tempDir = EnvironmentEx.GetVariableValue("TEMP");
                return _tempDir;
            }
        }

        internal static string VirusTotalUrl => "https://www.virustotal.com/#/file/{0}/";
    }
}
