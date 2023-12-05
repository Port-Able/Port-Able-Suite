namespace Updater.Libraries
{
    using System;
    using System.IO;
    using SilDev;
    using SilDev.Compression.Archiver;

    internal static class CorePaths
    {
        private static string _appsLauncher, _fileArchiver, _homeDir, _repoCommitsUrl, _repoReleasesUrl, _repoSnapshotsUrl, _tempDir;

        internal static string AppsLauncher => _appsLauncher ??= Path.Combine(PathEx.LocalDir, "AppsLauncher.exe");

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
                if (string.IsNullOrEmpty(SevenZip.DefaultArchiver.ExtractExePath))
                    Network.DownloadArchiver();
                else
                    foreach (var file in DirectoryEx.EnumerateFiles(SevenZip.DefaultArchiver.Location))
                    {
                        var name = Path.GetFileName(file);
                        if (string.IsNullOrEmpty(name))
                            continue;
                        path = Path.Combine(CachePaths.UpdateDir, name);
                        FileEx.Copy(file, path);
                    }
                SevenZip.DefaultArchiver.Location = CachePaths.UpdateDir;
                _fileArchiver = SevenZip.DefaultArchiver.ExtractExePath;
                return _fileArchiver;
            }
        }

        internal static string HomeDir => _homeDir ??= PathEx.Combine(PathEx.LocalDir, "..");

        internal static string PortAbleUrl => "http://www.port-a.de";

        internal static string RepoProfileUrl => "https://github.com/Port-Able";

        internal static string RepoCommitsUrl => 
            _repoCommitsUrl ??= PathEx.AltCombine(RepoProfileUrl, "Port-Able-Suite/commits/master.atom");

        internal static string RepoReleasesUrl => 
            _repoReleasesUrl ??= PathEx.AltCombine(RepoProfileUrl, "Port-Able-Suite/releases");

        internal static string RepoSnapshotsUrl => 
            _repoSnapshotsUrl ??= PathEx.AltCombine(RepoProfileUrl, "Port-Able-Suite-Snapper/raw/master");

        internal static string TempDir
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
