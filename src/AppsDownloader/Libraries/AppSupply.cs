namespace AppsDownloader.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using SilDev;

    internal enum AppSuppliers
    {
        Internal,
        PortableApps,
        SourceForge
    }

    internal static class AppSupplierHosts
    {
        internal const string Internal = "si13n7.com";
        internal const string PortableApps = "portableapps.com";
        internal const string SourceForge = "sourceforge.net";
    }

    internal static class AppSupply
    {
        private static Dictionary<AppSuppliers, List<string>> _mirrors;

        internal static bool AppIsInstalled(AppData appData)
        {
            var dir = appData.InstallDir;
            if (!Directory.Exists(dir))
                return false;
            const string pattern = "*.exe";
            switch (appData.Key)
            {
                case "Java":
                case "Java64":
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
            appInstaller.AddRange(CorePaths.AppDirs.SelectMany(x => searchPattern.SelectMany(y => DirectoryEx.EnumerateFiles(x, y))));
            return appInstaller;
        }

        internal static List<AppData> FindInstalledApps() =>
            CacheData.AppInfo.Where(AppIsInstalled).ToList();

        internal static List<AppData> FindOutdatedApps()
        {
            var outdatedApps = new List<AppData>();
            foreach (var appData in FindInstalledApps())
            {
                if (appData.Settings.NoUpdates)
                {
                    if (appData.Settings.NoUpdatesTime == default(DateTime) || Math.Abs((DateTime.Now - appData.Settings.NoUpdatesTime).TotalDays) <= 7d)
                        continue;
                    appData.Settings.NoUpdates = false;
                    appData.Settings.NoUpdatesTime = default(DateTime);
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
                                           fileHash = tuple.path.EncryptFile(ChecksumAlgorithms.Sha1);
                                           break;
                                       case Crypto.Sha256.HashLength:
                                           fileHash = tuple.path.EncryptFile(ChecksumAlgorithms.Sha256);
                                           break;
                                       case Crypto.Sha384.HashLength:
                                           fileHash = tuple.path.EncryptFile(ChecksumAlgorithms.Sha384);
                                           break;
                                       case Crypto.Sha512.HashLength:
                                           fileHash = tuple.path.EncryptFile(ChecksumAlgorithms.Sha512);
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

                var packageVersion = Ini.Read("Version", nameof(appData.PackageVersion), Version.Parse("0.0.0.0"), appIniPath);
                if (packageVersion >= appData.PackageVersion)
                    continue;

                if (outdatedApps.Any(x => x.Equals(appData)))
                    continue;
                if (Log.DebugMode > 0)
                    Log.Write($"Update: Outdated app has been found (Key: '{appData.Key}'; LocalVersion: '{packageVersion}'; ServerVersion: {appData.PackageVersion}).");
                outdatedApps.Add(appData);
            }
            if (Log.DebugMode > 0 && outdatedApps.Any())
                Log.Write($"Update: {outdatedApps.Count} outdated apps have been found (Keys: '{outdatedApps.Select(x => x.Key).Join("'; '")}').");
            return outdatedApps;
        }

        internal static List<string> GetMirrors(AppSuppliers supplier)
        {
            if (_mirrors == default(Dictionary<AppSuppliers, List<string>>))
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
                        "http://downloads.portableapps.com",
                        "http://downloads2.portableapps.com"
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
                    if (Network.IPv4IsAvalaible)
                    {
                        var sortHelper = new Dictionary<string, long>();
                        if (Log.DebugMode > 0)
                            Log.Write($"{nameof(AppSuppliers.SourceForge)}: Try to find the best server . . .");
                        foreach (var mirror in mirrors)
                        {
                            if (sortHelper.Keys.ContainsEx(mirror))
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

                // Internal ('si13n7.com')
                default:
                {
                    var servers = new[]
                    {
                        "https://www.si13n7.com/dns/info.ini",
                        "http://ns.0.si13n7.com",
                        "http://ns.1.si13n7.com",
                        "http://ns.2.si13n7.com",
                        "http://ns.3.si13n7.com",
                        "http://ns.4.si13n7.com",
                        "http://ns.5.si13n7.com"
                    };
                    if (!Network.IPv4IsAvalaible && Network.IPv6IsAvalaible)
                        servers = servers.Take(2).ToArray();

                    var info = default(string);
                    for (var i = 0; i < servers.Length; i++)
                    {
                        try
                        {
                            var server = servers[i];
                            if (!NetEx.FileIsAvailable(server, 20000))
                                throw new PathNotFoundException(server);
                            var data = NetEx.Transfer.DownloadString(server);
                            if (string.IsNullOrWhiteSpace(data))
                                throw new ArgumentNullException(nameof(data));
                            info = data;
                            if (Log.DebugMode > 0)
                                Log.Write($"{nameof(AppSuppliers.Internal)}: Domain names have been set successfully from '{server}'.");
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex);
                        }
                        if (string.IsNullOrWhiteSpace(info) && i < servers.Length - 1)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(info))
                    {
                        foreach (var section in Ini.GetSections(info))
                        {
                            var addr = string.Empty;
                            if (Network.IPv4IsAvalaible)
                                addr = Ini.Read(section, "addr", info);
                            if (string.IsNullOrEmpty(addr) && Network.IPv6IsAvalaible)
                                addr = Ini.Read(section, "ipv6", info);
                            if (string.IsNullOrEmpty(addr))
                                continue;
                            var domain = Ini.Read(section, "domain", info);
                            if (string.IsNullOrEmpty(domain))
                                continue;
                            var ssl = Ini.Read(section, "ssl", false, info);
                            if (Log.DebugMode > 0)
                                Log.Write($"{nameof(AppSuppliers.Internal)}: Section '{section}'; Address: '{addr}'; '{domain}'; SSL: '{ssl}';");
                            domain = PathEx.AltCombine(ssl ? "https:" : "http:", domain);
                            if (_mirrors[supplier].ContainsEx(domain))
                                continue;
                            _mirrors[supplier].Add(domain);
                            if (Log.DebugMode > 0)
                                Log.Write($"{nameof(AppSuppliers.Internal)}: Domain '{domain}' added.");
                        }
                        Ini.Detach(info);
                    }
                    break;
                }
            }
            return _mirrors[supplier];
        }
    }
}
