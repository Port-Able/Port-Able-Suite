﻿namespace AppsDownloader.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SilDev;
    using SilDev.Legacy;
    using SilDev.Network;

    internal enum AppSuppliers
    {
        Internal,
        PortableApps,
        SourceForge
    }

    internal static class AppSupplierHosts
    {
        internal const string Internal = "port-a.de";
        internal const string PortableApps = "portableapps.com";
        internal const string SourceForge = "sourceforge.net";
    }

    internal static class AppSupply
    {
        private static List<AppData> _installedApps;
        private static Dictionary<AppSuppliers, List<string>> _mirrors;

        internal static bool AppIsInstalled(AppData appData)
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
                    dir = Path.Combine(dir, "bin");
                    if (DirectoryEx.EnumerateFiles(dir, pattern).Any())
                        return true;
                    break;
                default:
                    if (DirectoryEx.EnumerateFiles(dir, pattern).Any())
                        return true;
                    var iniName = $"{appData.Key}.ini";
                    if (DirectoryEx.EnumerateFiles(dir, iniName).Any() &&
                        DirectoryEx.EnumerateFiles(dir, pattern, SearchOption.AllDirectories).Any())
                        return true;
                    break;
            }
            return false;
        }

        internal static List<string> FindAppInstaller()
        {
            var appInstaller = new List<string>();
            var searchPattern = new[]
            {
                "*.7z",
                "*.rar",
                "*.zip",
                "*.paf.exe"
            };
            if (Directory.Exists(Settings.TransferDir))
                appInstaller.AddRange(searchPattern.SelectMany(x => DirectoryEx.EnumerateFiles(Settings.TransferDir, x)));
            return appInstaller;
        }

        internal static List<AppData> FindInstalledApps(bool force = false)
        {
            if (force || _installedApps == default)
                _installedApps = CacheData.AppInfo.Where(AppIsInstalled).ToList();
            return _installedApps;
        }

        internal static List<AppData> FindOutdatedApps()
        {
            var outdatedApps = new List<AppData>();
            foreach (var appData in FindInstalledApps())
            {
                if (appData.Settings.NoUpdates)
                {
                    if (appData.Settings.NoUpdatesTime == default || Math.Abs((DateTime.Now - appData.Settings.NoUpdatesTime).TotalDays) <= 7d)
                        continue;
                    appData.Settings.NoUpdates = false;
                    appData.Settings.NoUpdatesTime = default;
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

        internal static List<string> GetMirrors(AppSuppliers supplier)
        {
            if (_mirrors == default)
                _mirrors = new Dictionary<AppSuppliers, List<string>>();

            if (!_mirrors.ContainsKey(supplier))
                _mirrors.Add(supplier, new List<string>());

            if (_mirrors[supplier].Any())
                return _mirrors[supplier];

            switch (supplier)
            {
                // PortableApps.com
                case AppSuppliers.PortableApps:
                {
                    var mirrors = new[]
                    {
                        // IPv4
                        "http://portableapps.com",
                        "http://downloads.portableapps.com",
                        "http://downloads2.portableapps.com",
                        "http://download3.portableapps.com"
                    };
                    _mirrors[supplier].AddRange(mirrors);
                    break;
                }

                // SourceForge.net
                case AppSuppliers.SourceForge:
                {
                    var mirrors = new[]
                    {
                        // IPv4 + IPv6 (however, a download via IPv6 doesn't work)
                        "https://netcologne.dl.sourceforge.net",
                        "https://freefr.dl.sourceforge.net",
                        "https://heanet.dl.sourceforge.net",

                        // IPv4
                        "https://kent.dl.sourceforge.net",
                        "https://netix.dl.sourceforge.net",
                        "https://vorboss.dl.sourceforge.net",
                        "https://downloads.sourceforge.net"
                    };
                    if (NetEx.IPv4IsAvalaible)
                    {
                        var sortHelper = new Dictionary<string, long>();
                        if (Log.DebugMode > 0)
                            Log.Write($"{nameof(AppSuppliers.SourceForge)}: Try to find the best server . . .");
                        foreach (var mirror in mirrors)
                        {
                            if (sortHelper.Keys.ContainsItem(mirror))
                                continue;
                            var time = NetEx.Ping(mirror);
                            if (Log.DebugMode > 0)
                                Log.Write($"{nameof(AppSuppliers.SourceForge)}: Reply from '{mirror}'; time={time}ms.");
                            sortHelper.Add(mirror, time);
                        }
                        mirrors = sortHelper.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.ToArray();
                        if (Log.DebugMode > 0)
                            Log.Write($"{nameof(AppSuppliers.SourceForge)}: New sort order: '{mirrors.Join("'; '")}'.");
                    }
                    _mirrors[supplier].AddRange(mirrors);
                    break;
                }

                // Internal - port-a.de | p-able.de
                default:
                {
                    // IPv4 + IPv6
                    var mirrors = new[]
                    {
                        "https://port-a.de",
                        "https://p-able.de",

                        // Backup
                        "https://dl.si13n7.de/Port-Able",
                        "https://dl.si13n7.com/Port-Able",

                        // Reserved
                        "http://dl-0.de/Port-Able",
                        "http://dl-1.de/Port-Able",
                        "http://dl-2.de/Port-Able",
                        "http://dl-3.de/Port-Able",
                        "http://dl-4.de/Port-Able",
                        "http://dl-5.de/Port-Able"
                    };
                    _mirrors[supplier].AddRange(mirrors);
                    break;
                }
            }
            return _mirrors[supplier];
        }
    }
}
