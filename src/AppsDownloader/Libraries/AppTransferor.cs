namespace AppsDownloader.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using LangResources;
    using Properties;
    using SilDev;
    using SilDev.Forms;
    using SilDev.Investment;

    public class AppTransferor
    {
        public AppTransferor(AppData appData)
        {
            AppData = appData ?? throw new ArgumentNullException(nameof(appData));
            DestPath = default;
            SrcData = new List<Tuple<string, string, string, bool>>();
            UserData = Tuple.Create(default(string), default(string));

            var downloadCollection = AppData.DownloadCollection;
            var packageVersion = default(string);
            if (ActionGuid.IsUpdateInstance && AppData?.UpdateCollection?.SelectMany(x => x.Value).All(x => x?.Item1?.StartsWithEx("http") == true) == true)
            {
                var appIniDir = Path.Combine(appData.InstallDir, "App", "AppInfo");
                var appIniPath = Path.Combine(appIniDir, "appinfo.ini");
                if (!File.Exists(appIniPath))
                    appIniPath = Path.Combine(appIniDir, "plugininstaller.ini");
                packageVersion = Ini.Read("Version", nameof(appData.PackageVersion), default(string), appIniPath);
                if (!string.IsNullOrEmpty(packageVersion) && AppData?.UpdateCollection?.ContainsKey(packageVersion) == true)
                    downloadCollection = AppData.UpdateCollection;
            }

            foreach (var pair in downloadCollection)
            {
                if (!pair.Key.EqualsEx(AppData.Settings.ArchiveLang) && (string.IsNullOrEmpty(packageVersion) || !pair.Key.EqualsEx(packageVersion)))
                    continue;

                foreach (var (item1, item2) in pair.Value)
                {
                    var srcUrl = item1;
                    if (srcUrl.StartsWith("{", StringComparison.InvariantCulture) && srcUrl.EndsWith("}", StringComparison.InvariantCulture))
                        srcUrl = FindArchivePath(srcUrl).Item2;

                    if (DestPath == default)
                    {
                        if (!DirectoryEx.Create(Settings.TransferDir))
                            continue;
                        var fileName = Path.GetFileName(srcUrl);
                        if (string.IsNullOrEmpty(fileName))
                            continue;
                        DestPath = PathEx.Combine(Settings.TransferDir, fileName);
                    }

                    var shortHost = NetEx.GetShortHost(srcUrl);
                    var redirect = Settings.ForceTransferRedirection || !NetEx.IPv4IsAvalaible && !string.IsNullOrWhiteSpace(shortHost) && !shortHost.EqualsEx(AppSupplierHosts.Internal);
                    string userAgent;
                    List<string> mirrors;
                    switch (shortHost)
                    {
                        case AppSupplierHosts.Internal:
                            userAgent = UserAgents.Internal;
                            mirrors = AppSupply.GetMirrors(AppSuppliers.Internal);
                            break;
                        case AppSupplierHosts.PortableApps:
                            userAgent = UserAgents.Empty;
                            mirrors = AppSupply.GetMirrors(AppSuppliers.PortableApps);
                            break;
                        case AppSupplierHosts.SourceForge:
                            userAgent = UserAgents.Default;
                            mirrors = AppSupply.GetMirrors(AppSuppliers.SourceForge);
                            break;
                        default:
                            userAgent = UserAgents.Default;
                            if (AppData.ServerKey != default)
                            {
                                var srv = Shareware.GetAddresses().FirstOrDefault(x => Shareware.FindAddressKey(x) == AppData.ServerKey.ToArray().Encode(BinaryToTextEncoding.Base85));
                                if (srv != default)
                                {
                                    if (!srcUrl.StartsWithEx("http://", "https://"))
                                        srcUrl = PathEx.AltCombine(srv, srcUrl);
                                    UserData = Tuple.Create(Shareware.GetUser(srv), Shareware.GetPassword(srv));
                                }
                            }
                            SrcData.Add(Tuple.Create(srcUrl, item2, userAgent, false));
                            continue;
                    }

                    var sHost = NetEx.GetShortHost(srcUrl);
                    var fhost = srcUrl.Substring(0, srcUrl.IndexOf(sHost, StringComparison.OrdinalIgnoreCase) + sHost.Length);
                    foreach (var mirror in mirrors)
                    {
                        if (!fhost.EqualsEx(mirror))
                            srcUrl = srcUrl.Replace(fhost, mirror);
                        if (SrcData.Any(x => x.Item1.EqualsEx(srcUrl)))
                            continue;
                        if (redirect)
                        {
                            userAgent = UserAgents.Internal;
                            srcUrl = CorePaths.RedirectUrl + srcUrl.Encode();
                        }
                        SrcData.Add(Tuple.Create(srcUrl, item2, userAgent, false));
                        if (Log.DebugMode > 1)
                            Log.Write($"Transfer: '{srcUrl}' has been added.");
                    }
                }
                break;
            }
            Transfer = new NetEx.AsyncTransfer();
        }

        public AppData AppData { get; }

        public string DestPath { get; }

        public List<Tuple<string, string, string, bool>> SrcData { get; }

        public NetEx.AsyncTransfer Transfer { get; }

        public Tuple<string, string> UserData { get; }

        public bool AutoRetry { get; private set; }

        public bool DownloadStarted { get; private set; }

        public bool InstallStarted { get; private set; }

        public void StartDownload(bool force = false)
        {
            DownloadStarted = false;
            if (Transfer.IsBusy)
            {
                if (!force)
                    return;
                Transfer.CancelAsync();
            }
            for (var i = 0; i < SrcData.Count; i++)
            {
                var (item1, item2, item3, item4) = SrcData[i];
                if (item4)
                    continue;
                if (!FileEx.Delete(DestPath))
                    throw new InvalidOperationException();

                SrcData[i] = Tuple.Create(item1, item2, item3, true);
                var userAgent = item3;
                if (!NetEx.FileIsAvailable(item1, UserData.Item1, UserData.Item2, 60000, userAgent))
                {
                    userAgent = UserAgents.WindowsChrome;
                    if (!NetEx.FileIsAvailable(item1, UserData.Item1, UserData.Item2, 60000, userAgent))
                    {
                        if (Log.DebugMode > 0)
                            Log.Write($"Transfer: Could not find target '{item1}'.");
                        continue;
                    }
                }
                if (Log.DebugMode > 0)
                    Log.Write($"Transfer{(!string.IsNullOrEmpty(userAgent) ? $" [{userAgent}]" : string.Empty)}: '{item1}' has been found.");

                Transfer.DownloadFile(item1, DestPath, UserData.Item1, UserData.Item2, true, 60000, userAgent, false);
                DownloadStarted = true;
            }
        }

        public bool StartInstall()
        {
            InstallStarted = false;
            if (Transfer.IsBusy || !File.Exists(DestPath))
                return false;

            const string nonHash = "None";
            var fileHash = default(string);
            foreach (var (_, item2, _, item4) in SrcData)
            {
                if (!item4)
                    continue;

                if (fileHash == default || fileHash == nonHash)
                    switch (item2.Length)
                    {
                        case Crypto.Md5.HashLength:
                            fileHash = DestPath.EncryptFile();
                            break;
                        case Crypto.Sha1.HashLength:
                            fileHash = DestPath.EncryptFile(ChecksumAlgorithm.Sha1);
                            break;
                        case Crypto.Sha256.HashLength:
                            fileHash = DestPath.EncryptFile(ChecksumAlgorithm.Sha256);
                            break;
                        case Crypto.Sha384.HashLength:
                            fileHash = DestPath.EncryptFile(ChecksumAlgorithm.Sha384);
                            break;
                        case Crypto.Sha512.HashLength:
                            fileHash = DestPath.EncryptFile(ChecksumAlgorithm.Sha512);
                            break;
                        default:
                            fileHash = nonHash;
                            break;
                    }

                if (fileHash != nonHash && !fileHash.EqualsEx(item2))
                    switch (MessageBoxEx.Show(string.Format(CultureInfo.InvariantCulture, Language.GetText(nameof(en_US.ChecksumErrorMsg)), Path.GetFileName(DestPath)), Resources.GlobalTitle, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3))
                    {
                        case DialogResult.Ignore:
                            break;
                        case DialogResult.Retry:
                            AutoRetry = true;
                            continue;
                        default:
                            continue;
                    }

                if (Directory.Exists(AppData.InstallDir))
                    if (!BreakFileLocks(AppData.InstallDir, false))
                    {
                        MessageBoxEx.Show(string.Format(CultureInfo.InvariantCulture, Language.GetText(nameof(en_US.InstallSkippedMsg)), AppData.Name), Resources.GlobalTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        continue;
                    }

                if (DestPath.EndsWithEx(".7z", ".rar", ".zip"))
                {
                    if (!File.Exists(CorePaths.FileArchiver))
                        throw new PathNotFoundException(CorePaths.FileArchiver);
                    using (var process = Compaction.SevenZipHelper.Unzip(DestPath, AppData.InstallDir))
                        if (process?.HasExited == false)
                            process.WaitForExit();
                }
                else
                {
                    var appsDir = CorePaths.AppsDir;
                    using (var process = ProcessEx.Start(DestPath, appsDir, $"/DESTINATION=\"{appsDir}\\\"", Elevation.IsAdministrator, false))
                        if (process?.HasExited == false)
                        {
                            process.WaitForInputIdle(0x1000);
                            try
                            {
                                var buttons = Settings.NsisButtons;
                                if (buttons == null)
                                    throw new NotSupportedException();
                                var okButton = buttons.Take(2).ToArray();
                                var langId = WinApi.NativeHelper.GetUserDefaultUILanguage();
                                var wndState = langId == 1031 || langId == 1033 || langId == 2057 ? WinApi.ShowWindowFlags.ShowMinNoActive : WinApi.ShowWindowFlags.ShowNa;
                                var stopwatch = Stopwatch.StartNew();
                                var minimized = new List<IntPtr>();
                                var counter = new CounterInvestor<int>();
                                while (stopwatch.Elapsed.TotalMinutes < 5d)
                                {
                                    if (process.HasExited)
                                        break;
                                    string title;
                                    using (var proc = Process.GetProcessById(process.Id))
                                        title = proc.MainWindowTitle;
                                    if (string.IsNullOrEmpty(title))
                                        continue;
                                    var parent = WinApi.NativeHelper.FindWindow(null, title);
                                    if (parent == IntPtr.Zero)
                                        continue;
                                    if (!minimized.Contains(parent) && WinApi.NativeHelper.ShowWindowAsync(parent, wndState))
                                        minimized.Add(parent);
                                    foreach (var button in buttons)
                                    {
                                        var child = WinApi.NativeHelper.FindWindowEx(parent, IntPtr.Zero, "Button", button);
                                        if (child == IntPtr.Zero)
                                            continue;
                                        if (counter.Increase(button.GetHashCode()) > 10)
                                        {
                                            if (button.EqualsEx(okButton))
                                                goto Manually;
                                            continue;
                                        }
                                        WinApi.NativeHelper.SendMessage(child, 0xf5u, IntPtr.Zero, IntPtr.Zero);
                                    }
                                }
                            }
                            catch (Exception ex) when (ex.IsCaught())
                            {
                                Log.Write(ex);
                            }
                            Manually:
                            if (!process.HasExited)
                            {
                                try
                                {
                                    using (var proc = Process.GetProcessById(process.Id))
                                    {
                                        var hWnd = WinApi.NativeHelper.FindWindow(null, proc.MainWindowTitle);
                                        WinApi.NativeHelper.ShowWindowAsync(hWnd, WinApi.ShowWindowFlags.ShowNormal);
                                        WinApi.NativeHelper.SetForegroundWindow(hWnd);
                                        WinApi.NativeHelper.SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0, WinApi.SetWindowPosFlags.NoMove | WinApi.SetWindowPosFlags.NoSize);
                                    }
                                }
                                catch (Exception ex) when (ex.IsCaught())
                                {
                                    Log.Write(ex);
                                }
                                process.WaitForExit();
                            }
                        }

                    // fix for messy app installer
                    var retries = 0;
                    retry:
                    try
                    {
                        var appDirs = new[]
                        {
                            Path.Combine(appsDir, "App"),
                            Path.Combine(appsDir, "Data"),
                            Path.Combine(appsDir, "Other")
                        };
                        if (appDirs.Any(Directory.Exists))
                        {
                            if (!Directory.Exists(AppData.InstallDir))
                                Directory.CreateDirectory(AppData.InstallDir);
                            else
                            {
                                BreakFileLocks(AppData.InstallDir);
                                foreach (var dir in new[] { "App", "Other" }.Select(name => Path.Combine(AppData.InstallDir, name)).Where(Directory.Exists))
                                    Directory.Delete(dir, true);
                                foreach (var file in Directory.EnumerateFiles(AppData.InstallDir, "*.*", SearchOption.TopDirectoryOnly))
                                    File.Delete(file);
                            }
                            foreach (var dir in appDirs)
                            {
                                if (!Directory.Exists(dir))
                                    continue;
                                var dirName = Path.GetFileName(dir);
                                if (string.IsNullOrEmpty(dirName))
                                    continue;
                                BreakFileLocks(dir);
                                if (dirName.EqualsEx("Data"))
                                {
                                    Directory.Delete(dir, true);
                                    continue;
                                }
                                var innerDir = Path.Combine(AppData.InstallDir, dirName);
                                Directory.Move(innerDir, dir);
                            }
                            foreach (var file in Directory.EnumerateFiles(appsDir, "*.*", SearchOption.TopDirectoryOnly))
                            {
                                if (FileEx.IsHidden(file) || file.EndsWithEx(".7z", ".rar", ".zip", ".paf.exe"))
                                    continue;
                                var fileName = Path.GetFileName(file);
                                if (string.IsNullOrEmpty(fileName))
                                    continue;
                                BreakFileLocks(file);
                                var innerFile = Path.Combine(AppData.InstallDir, fileName);
                                File.Move(innerFile, file);
                            }
                        }
                    }
                    catch (Exception ex) when (ex.IsCaught())
                    {
                        Log.Write(ex);
                        if (retries >= 15)
                            return false;
                        retries++;
                        Thread.Sleep(1000);
                        goto retry;
                    }
                }

                FileEx.TryDelete(DestPath);
                InstallStarted = true;
                return true;
            }
            return false;
        }

        internal static (Version, string) FindArchivePath(string jsonArchivePath)
        {
            try
            {
                var searchData = Json.Deserialize<Dictionary<string, string>>(jsonArchivePath);
                if (searchData == default || !searchData.ContainsKey("regex") || !searchData.ContainsKey("source"))
                    throw new ArgumentInvalidException(nameof(jsonArchivePath));
                var sourceUrl = searchData["source"];
                var sourceText = NetEx.Transfer.DownloadString(sourceUrl);
                if (string.IsNullOrWhiteSpace(sourceText))
                {
                    if (!searchData.ContainsKey("mirror"))
                        throw new PathNotFoundException(sourceUrl);
                    sourceUrl = searchData["mirror"];
                    sourceText = NetEx.Transfer.DownloadString(sourceUrl);
                }
                var foundData = new Dictionary<Version, string>();
                var regex = new Regex(searchData["regex"], RegexOptions.IgnoreCase);
                foreach (var match in regex.Matches(sourceText).Cast<Match>())
                {
                    var versions = match.Groups["Version"]?.Captures;
                    var fileNames = match.Groups["FileName"]?.Captures;
                    if (versions == null || fileNames == null || versions.Count != fileNames.Count)
                        continue;
                    for (var i = 0; i < versions.Count; i++)
                    {
                        var version = new Version(versions[i].Value.Trim());
                        if (foundData.ContainsKey(version))
                            continue;
                        var fileName = fileNames[i].Value.Trim();
                        foundData.Add(version, fileName);
                    }
                }
                foundData = foundData.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                var (lastVersion, lastArchivePath) = foundData.Select(x => (x.Key, PathEx.AltCombine(sourceUrl, x.Value))).FirstOrDefault();
                if (lastVersion == default || lastArchivePath == default)
                    throw new MissingFieldException();
                return (lastVersion, lastArchivePath);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
                return (new Version("1.0.0.0"), string.Empty);
            }
        }

        private static bool BreakFileLocks(string path, bool force = true)
        {
            if (!PathEx.DirOrFileExists(path))
                return true;
            var doubleTap = false;
            Check:
            var locks = PathEx.GetLocks(path)?.ToArray();
            if (locks?.Any() != true)
                return true;
            if (doubleTap)
            {
                ProcessEx.Terminate(locks);
                return true;
            }
            if (!force)
            {
                var lockData = locks.Select(p => $"ID: {p.Id:d5}; Name: '{p.ProcessName}.exe'").ToArray();
                var information = string.Format(CultureInfo.InvariantCulture, Language.GetText(lockData.Length == 1 ? nameof(en_US.FileLockMsg) : nameof(en_US.FileLocksMsg)), lockData.Join(Environment.NewLine));
                if (MessageBoxEx.Show(information, Resources.GlobalTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                    return false;
            }
            locks.Where(process => !process.ProcessName.EndsWithEx("64Portable", "Portable64", "Portable")).ForEach(process => ProcessEx.Close(process));
            Thread.Sleep(400);
            doubleTap = true;
            goto Check;
        }
    }
}
