namespace PortAble
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SilDev;
    using SilDev.Ini.Legacy;

    /// <summary>
    ///     Provides an app supply management.
    /// </summary>
    public static class AppSupply
    {
        private static List<AppData> _installedApps;

        /// <summary>
        ///     Determines whether the specified app is extracted and ready to run.
        /// </summary>
        /// <param name="appData">
        ///     The app to check.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the app is installed; otherwise,
        ///     <see langword="false"/>.
        /// </returns>
        public static bool AppIsInstalled(AppData appData)
        {
            var dir = appData.InstallDir;
            if (!Directory.Exists(dir))
                return false;
            const string pattern = "*.exe";
            switch (appData.Key)
            {
                case "Ghostscript":
                case "Java":
                case "Java64":
                case "JDK":
                case "JDK64":
                case "OpenJDK":
                case "OpenJDK64":
                case "OpenJDKJRE":
                case "OpenJDKJRE64":
                    return DirectoryEx.EnumerateFiles(Path.Combine(dir, "bin"), pattern)?.Any() == true;
                default:
                    if (DirectoryEx.EnumerateFiles(dir, pattern)?.Any() == true)
                        return true;
                    var iniName = $"{appData.Key}.ini";
                    return DirectoryEx.EnumerateFiles(dir, iniName)?.Any() == true &&
                           DirectoryEx.EnumerateFiles(dir, pattern, SearchOption.AllDirectories)?.Any() == true;
            }
        }

        /// <summary>
        ///     Searches for all downloaded app installers or archives and creates a list
        ///     with complete file paths.
        /// </summary>
        /// <returns>
        ///     A list of complete file paths to downloaded app installers or archives.
        /// </returns>
        public static List<string> FindAppInstaller()
        {
            var appInstaller = new List<string>();
            var searchPattern = new[]
            {
                "*.7z",
                "*.rar",
                "*.zip",
                "*.paf.exe"
            };
            if (Directory.Exists(CacheFiles.TransferDir))
                appInstaller.AddRange(searchPattern.SelectMany(x => DirectoryEx.EnumerateFiles(CacheFiles.TransferDir, x)));
            return appInstaller;
        }

        /// <summary>
        ///     Searches for all apps that are extracted and ready to run and retrieves a
        ///     list of their <see cref="AppData"/> instances.
        ///     <para>
        ///         It will only search once for all apps on disk, each subsequent call to
        ///         this method will load the data from the cache until
        ///         <paramref name="force"/> is set to true
        ///     </para>
        /// </summary>
        /// <param name="force">
        ///     <see langword="true"/> to force search for all installed apps instead of
        ///     loading data from cache; otherwise, <see langword="false"/>.
        /// </param>
        /// <returns>
        ///     A list with <see cref="AppData"/> instances of installed apps.
        /// </returns>
        public static List<AppData> FindInstalledApps(bool force = false)
        {
            if (force || _installedApps == default)
                _installedApps = CacheData.AppInfo.Where(AppIsInstalled).ToList();
            return _installedApps;
        }

        /// <summary>
        ///     Searches for all outdated apps.
        /// </summary>
        /// <returns>
        ///     A list with <see cref="AppData"/> instances of installed apps that needs an
        ///     update.
        /// </returns>
        public static List<AppData> FindOutdatedApps()
        {
            var outdatedApps = new List<AppData>();
            foreach (var appData in FindInstalledApps())
            {
                appData.Settings.LoadFromFile();
                if (appData.Settings.DisableUpdates)
                {
                    if (appData.Settings.DelayUpdates == default || Math.Abs((DateTime.Now - appData.Settings.DelayUpdates).TotalDays) <= 7d)
                        continue;
                    appData.Settings.DisableUpdates = false;
                    appData.Settings.DelayUpdates = default;
                    appData.Settings.SaveToFile();
                }

                if (appData.VersionData.Any())
                {
                    if (appData.VersionData
                               .Select(data => new
                               {
                                   data,
                                   path = Path.Combine(appData.InstallDir, data.Item1)
                               })
                               .Where(tuple =>
                               {
                                   if (!File.Exists(tuple.path))
                                       return true;
                                   string fileHash;
                                   switch (tuple.data.Item2.Length)
                                   {
                                       case Crypto.Md5.HashLength:
                                           fileHash = tuple.path.EncryptFile();
                                           break;
                                       case Crypto.Sha1.HashLength:
                                           fileHash = tuple.path.EncryptFile(ChecksumAlgorithm.Sha1);
                                           break;
                                       case Crypto.Sha256.HashLength:
                                           fileHash = tuple.path.EncryptFile(ChecksumAlgorithm.Sha256);
                                           break;
                                       case Crypto.Sha384.HashLength:
                                           fileHash = tuple.path.EncryptFile(ChecksumAlgorithm.Sha384);
                                           break;
                                       case Crypto.Sha512.HashLength:
                                           fileHash = tuple.path.EncryptFile(ChecksumAlgorithm.Sha512);
                                           break;
                                       default:
                                           return false;
                                   }
                                   return !fileHash.EqualsEx(tuple.data.Item2);
                               })
                               .Select(x => x.data).Any())
                        outdatedApps.Add(appData);
                    continue;
                }

                var appIniDir = Path.Combine(appData.InstallDir, "App", "AppInfo");
                var appIniPath = Path.Combine(appIniDir, "appinfo.ini");
                if (!File.Exists(appIniPath))
                {
                    appIniPath = Path.Combine(appIniDir, "plugininstaller.ini");
                    if (!File.Exists(appIniPath))
                        continue;
                }

                var srvPackageVersion = appData.PackageVersion;
                if (appData.PackageVersion == Version.Parse("1.0.0.0"))
                {
                    var archivePath = appData.DownloadCollection.FirstOrDefault().Value.FirstOrDefault()?.Item1;
                    if (string.IsNullOrEmpty(archivePath))
                        continue;
                    if (archivePath.StartsWith("{", StringComparison.InvariantCulture) && archivePath.EndsWith("}", StringComparison.InvariantCulture))
                        srvPackageVersion = AppTransferor.FindArchivePath(archivePath).Item1;
                }

                var curPackageVersion = Ini.Read("Version", nameof(appData.PackageVersion), Version.Parse("0.0.0.0"), appIniPath);
                if (curPackageVersion >= srvPackageVersion)
                    continue;

                if (outdatedApps.Any(x => x.Equals(appData)))
                    continue;
                if (Log.DebugMode > 0)
                    Log.Write($"Update: Outdated app has been found (Key: '{appData.Key}'; LocalVersion: '{curPackageVersion}'; ServerVersion: {srvPackageVersion}).");
                outdatedApps.Add(appData);
            }
            if (Log.DebugMode > 0 && outdatedApps.Any())
                Log.Write($"Update: {outdatedApps.Count} outdated apps have been found (Keys: '{outdatedApps.Select(x => x.Key).Join("'; '")}').");
            return outdatedApps;
        }
    }
}
