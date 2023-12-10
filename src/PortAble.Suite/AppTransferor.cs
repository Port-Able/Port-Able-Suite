namespace PortAble
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using Properties;
    using SilDev;
    using SilDev.Compression.Archiver;
    using SilDev.Forms;
    using SilDev.Ini.Legacy;
    using SilDev.Network;
    using static SilDev.WinApi;

    /// <summary>
    ///     Provides functionality to download apps.
    /// </summary>
    public sealed class AppTransferor
    {
        private const int MaxTimeout = 60 * 1000;
        private static string _lastHostFromAll;
        private readonly List<string> _hostBlacklist = new();
        private readonly List<Tuple<string, string, string, bool>> _srcData = new();
        private int _curTimeout = 4 * 1000;

        /// <summary>
        ///     An initialized <see cref="AppData"/> class, which app information from a
        ///     server.
        /// </summary>
        public AppData AppData { get; }

        /// <summary>
        ///     The destination path where the app installer or archive will be downloaded.
        /// </summary>
        public string DestPath { get; }

        /// <summary>
        ///     The source data that provides multiple download information in a collection
        ///     of <see cref="Tuple{T1, T2, T3, T4}"/> for an app.
        ///     <para>
        ///         <see langword="T1"/>: The download URL to an app installer or archive.
        ///     </para>
        ///     <para>
        ///         <see langword="T2"/>: The file hash of the app installer or archive to
        ///         verify a successful download.
        ///     </para>
        ///     <para>
        ///         <see langword="T3"/>: The user agent used to download the app installer
        ///         or archive.
        ///     </para>
        ///     <para>
        ///         <see langword="T4"/>: Determines whether the app download was started
        ///         with the download data from this <see cref="Tuple{T1, T2, T3, T4}"/>.
        ///     </para>
        /// </summary>
        public IReadOnlyList<Tuple<string, string, string, bool>> SrcData => _srcData;

        /// <summary>
        ///     An initialized <see cref="WebTransferAsync"/> class, This provides the
        ///     functionality to download files.
        /// </summary>
        public WebTransferAsync Transfer { get; }

        /// <summary>
        ///     The username and password required to verify an app download.
        /// </summary>
        public Tuple<string, string> UserData { get; }

        /// <summary>
        ///     Determines whether to retry a failed download.
        /// </summary>
        public bool AutoRetry { get; private set; }

        /// <summary>
        ///     Determines whether the app download has started.
        /// </summary>
        public bool DownloadStarted { get; private set; }

        /// <summary>
        ///     The host address from which the current download was started.
        /// </summary>
        public string DownloadHost { get; private set; }

        /// <summary>
        ///     Determines whether the app installation has started.
        /// </summary>
        public bool InstallStarted { get; private set; }

        /// <summary>
        ///     The start date and time when the installation process started.
        /// </summary>
        public DateTime InstallProcessStartTime { get; private set; }

        /// <summary>
        ///     Initialize the <see cref="AppTransferor"/> class.
        /// </summary>
        /// <param name="appData">
        ///     The <see cref="AppData"/> instance from which to create this instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     appData is null.
        /// </exception>
        public AppTransferor(AppData appData)
        {
            AppData = appData ?? throw new ArgumentNullException(nameof(appData));
            DestPath = default;
            UserData = default;

            var downloadCollection = appData.DownloadCollection;
            var packageVersion = default(string);
            if (ActionGuid.IsUpdateInstance &&
                appData.UpdateCollection?.SelectMany(x => x.Value).All(x => x?.Item1?.StartsWithEx("http:", "https:") == true) == true)
            {
                var appIniDir = Path.Combine(appData.InstallDir, "App\\AppInfo");
                var appIniPath = Path.Combine(appIniDir, "appinfo.ini");
                if (!File.Exists(appIniPath))
                    appIniPath = Path.Combine(appIniDir, "plugininstaller.ini");
                packageVersion = Ini.Read("Version", nameof(appData.PackageVersion), default(string), appIniPath);
                if (!string.IsNullOrEmpty(packageVersion) && appData.UpdateCollection?.ContainsKey(packageVersion) == true)
                    downloadCollection = appData.UpdateCollection;
            }

            foreach (var pair in downloadCollection)
            {
                if (!pair.Key.EqualsEx(appData.Settings.ArchiveLang) && (string.IsNullOrEmpty(packageVersion) ||
                                                                         !pair.Key.EqualsEx(packageVersion)))
                    continue;

                foreach (var (serverFile, checkHash) in pair.Value)
                {
                    var srcUrl = serverFile;
                    if (srcUrl.StartsWith("{", StringComparison.InvariantCulture) && srcUrl.EndsWith("}", StringComparison.InvariantCulture))
                        srcUrl = FindArchivePath(srcUrl).Item2;

                    if (DestPath == default)
                    {
                        if (!DirectoryEx.Create(CacheFiles.TransferDir))
                            continue;
                        var fileName = Path.GetFileName(srcUrl);
                        if (string.IsNullOrEmpty(fileName))
                            continue;
                        DestPath = PathEx.Combine(CacheFiles.TransferDir, fileName);
                    }

                    var shortHost = appData.Supplier == default ? NetEx.GetShortHost(srcUrl) : default;
                    var redirect = !NetEx.IPv4IsAvalaible &&
                                   !string.IsNullOrWhiteSpace(shortHost) &&
                                   !shortHost.EqualsEx(AppSupplierHosts.Pa);

                    ReadOnlyDictionary<string, string> mirrors;
                    var userAgent = UserAgents.Wget;
                    switch (shortHost)
                    {
                        case AppSupplierHosts.Pa:
                            mirrors = AppSupplierMirrors.Pa;
                            userAgent = UserAgents.Pa;
                            break;
                        case AppSupplierHosts.Pac:
                            mirrors = AppSupplierMirrors.Pac;
                            break;
                        case AppSupplierHosts.Sf:
                            mirrors = AppSupplierMirrors.Sf;
                            break;
                        default:
                            if (appData.Supplier != default)
                            {
                                if (!srcUrl.StartsWithEx("http:", "https:"))
                                    srcUrl = PathEx.AltCombine(default(char[]), appData.Supplier.Address, srcUrl);
                                userAgent = appData.Supplier.UserAgent;
                                UserData = Tuple.Create(appData.Supplier.User, appData.Supplier.Password);
                            }
                            _srcData.Add(Tuple.Create(srcUrl, checkHash, userAgent, false));
                            continue;
                    }

                    var sHost = NetEx.GetShortHost(srcUrl);
                    var fhost = srcUrl.Substring(0, srcUrl.IndexOf(sHost, StringComparison.OrdinalIgnoreCase) + sHost.Length);
                    foreach (var mirror in mirrors.Where(x => x.Value != "DISABLED").Select(x => x.Key))
                    {
                        var url = srcUrl;
                        if (!fhost.EqualsEx(mirror))
                            url = url.Replace(fhost, mirror);
                        if (SrcData.Any(x => x.Item1.EqualsEx(url)))
                            continue;

                        // A download redirection service, which is used in the rare case when
                        // the client only has an IPv6 connection and the server only offers an
                        // IPv4 connection.
                        if (redirect)
                        {
                            userAgent = UserAgents.Pa;
                            url = CorePaths.RedirectDlUrlFormat.FormatInvariant(url.Encode());
                        }

                        _srcData.Add(Tuple.Create(url, checkHash, userAgent, false));
                        if (Log.DebugMode > 1)
                            Log.Write($"Transfer: '{url}' has been added.");
                    }
                }
                break;
            }
            UserData ??= Tuple.Create(default(string), default(string));
            Transfer = new WebTransferAsync();
        }

        /// <summary>
        ///     Downloads the <see cref="AppData"/> app installer or archive to the
        ///     <see cref="DestPath"/>.
        /// </summary>
        /// <param name="force">
        ///     <see langword="true"/> to force a restart of a download that is already in
        ///     progress; otherwise, <see langword="false"/>
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     If the file already exists under <see cref="DestPath"/> and cannot be
        ///     deleted.
        /// </exception>
        public void StartDownload(bool force = false)
        {
            DownloadStarted = false;
            if (Transfer.IsBusy)
            {
                if (!force)
                    return;
                Transfer.CancelAsync();
            }
            if (Transfer.HasCanceled && !_hostBlacklist.Contains(_lastHostFromAll) && _hostBlacklist.Count < SrcData.Count / 2)
            {
                _hostBlacklist.Add(_lastHostFromAll);
                if (Log.DebugMode > 0)
                    Log.Write($"Transfer: Host '{_lastHostFromAll}' blacklisted.");
            }
            for (var i = 0; i < SrcData.Count; i++)
            {
                var (srcUrl, checkHash, userAgent, started) = SrcData[i];
                if (started || !FileEx.TryDelete(DestPath))
                    break;

                _srcData[i] = Tuple.Create(srcUrl, checkHash, userAgent, true);

                var fullHost = NetEx.GetFullHost(srcUrl);
                if (_hostBlacklist.Contains(fullHost))
                    continue;

                var curUserAgent = userAgent;
                if (Log.DebugMode > 0)
                    Log.Write($"Transfer{(!string.IsNullOrEmpty(curUserAgent) ? $" [{curUserAgent}]" : string.Empty)}: Check with a timeout of '{_curTimeout}ms' if target '{srcUrl}' is available.");

                var (user, password) = UserData;
                if (!NetEx.FileIsAvailable(srcUrl, user, password, _curTimeout, curUserAgent))
                {
                    // Start with a slow timeout and increase it with each new mirror up
                    // to a maximum of 60 seconds to avoid connecting to slow servers.
                    if (_curTimeout < MaxTimeout)
                    {
                        LocalSetTimeout(ref _curTimeout);
                        continue;
                    }

                    // Otherwise, try a different user agent before
                    // going to the next mirror when retry is active.
                    curUserAgent = UserAgents.Browser;
                    if (Log.DebugMode > 0)
                        Log.Write($"Transfer: User agent changed for target '{srcUrl}'.");

                    if (!NetEx.FileIsAvailable(srcUrl, user, password, _curTimeout, curUserAgent))
                    {
                        if (Log.DebugMode > 0)
                            Log.Write($"Transfer{(!string.IsNullOrEmpty(curUserAgent) ? $" [{curUserAgent}]" : string.Empty)}: Could not find target '{srcUrl}'.");
                        continue;
                    }
                }
                if (Log.DebugMode > 0)
                    Log.Write($"Transfer{(!string.IsNullOrEmpty(curUserAgent) ? $" [{curUserAgent}]" : string.Empty)}: '{srcUrl}' has been found.");

                // Max timeout for download ensures that the download is as stable as possible.
                Transfer.DownloadFile(srcUrl, DestPath, UserData.Item1, UserData.Item2, true, MaxTimeout, curUserAgent, false);
                DownloadHost = _lastHostFromAll = fullHost;
                DownloadStarted = true;
            }
            return;

            static void LocalSetTimeout(ref int timeout)
            {
                if (timeout >= MaxTimeout)
                {
                    timeout = MaxTimeout;
                    return;
                }
                timeout = Math.Min((int)Math.Round(timeout * 2.5f), MaxTimeout);
            }
        }

        /// <summary>
        ///     Launches the downloaded <see cref="AppData"/> app installer or extracts the
        ///     app archive to the <see cref="AppData.InstallDir"/> directory if the file
        ///     transfer was successful.
        /// </summary>
        public bool StartInstall()
        {
            InstallStarted = false;
            if (Transfer.IsBusy || !File.Exists(DestPath))
                return false;

            const string nonHash = "None";
            var curHash = default(string);
            foreach (var (_, checkHash, _, started) in SrcData)
            {
                if (!started)
                    continue;

                if (curHash is null or nonHash)
                    curHash = checkHash.Length switch
                    {
                        Crypto.Md5.HashLength => DestPath.EncryptFile(),
                        Crypto.Sha1.HashLength => DestPath.EncryptFile(ChecksumAlgorithm.Sha1),
                        Crypto.Sha256.HashLength => DestPath.EncryptFile(ChecksumAlgorithm.Sha256),
                        Crypto.Sha384.HashLength => DestPath.EncryptFile(ChecksumAlgorithm.Sha384),
                        Crypto.Sha512.HashLength => DestPath.EncryptFile(ChecksumAlgorithm.Sha512),
                        _ => nonHash
                    };

                if (!curHash.EqualsEx(checkHash, nonHash))
                    switch (MessageBoxEx.Show(LangStrings.ChecksumErrorMsg.FormatInvariant(Path.GetFileName(DestPath)),
                                              AssemblyInfo.Title,
                                              MessageBoxButtons.AbortRetryIgnore,
                                              MessageBoxIcon.Warning,
                                              MessageBoxDefaultButton.Button3))
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
                        MessageBoxEx.Show(LangStrings.ChecksumErrorMsg.FormatInvariant(AppData.Name),
                                          AssemblyInfo.Title,
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);
                        continue;
                    }

                if (DestPath.EndsWithEx(".7z", ".rar", ".zip"))
                {
                    if (!File.Exists(CorePaths.FileArchiver))
                        throw new PathNotFoundException(CorePaths.FileArchiver);

                    using var process = SevenZip.DefaultArchiver.Extract(DestPath, AppData.InstallDir);
                    InstallProcessStartTime = DateTime.Now;
                    if (process?.HasExited == false)
                        process.WaitForExit();
                }
                else
                {
                    var appsDir = CorePaths.AppsDir;

                    using (var process = ProcessEx.Start(DestPath, appsDir, $"/DESTINATION=\"{appsDir}\\\"", Elevation.IsAdministrator, false))
                    {
                        InstallProcessStartTime = DateTime.Now;
                        SetupPilot(process);
                        if (process?.HasExited == false)
                            process.WaitForExit();
                    }

                    // dirty workaround for messy app installer
                    MessyAppInstallerWorkaround(appsDir);
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
                var sourceText = WebTransfer.DownloadString(sourceUrl);
                if (string.IsNullOrWhiteSpace(sourceText))
                {
                    if (!searchData.ContainsKey("mirror"))
                        throw new PathNotFoundException(sourceUrl);
                    sourceUrl = searchData["mirror"];
                    sourceText = WebTransfer.DownloadString(sourceUrl);
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

        private static IntPtr FindWindow(Process process)
        {
            if (process?.HasExited != false)
                return IntPtr.Zero;
            string title;
            using (var proc = Process.GetProcessById(process.Id))
            {
                if (proc.HasExited)
                    return IntPtr.Zero;
                title = proc.MainWindowTitle;
            }
            return string.IsNullOrEmpty(title) ? IntPtr.Zero : NativeHelper.FindWindow(null, title);
        }

        private static bool BreakFileLocks(string path, bool force = true)
        {
            if (!PathEx.DirOrFileExists(path))
                return true;
            for (var i = 0; i < 2; i++)
            {
                var locks = PathEx.GetLocks(path)?.ToArray();
                if (locks?.Any() != true)
                    return true;
                if (i > 0)
                {
                    ProcessEx.Terminate(locks);
                    return true;
                }
                if (!force)
                {
                    var data = locks.Select(p => $"ID: {p.Id:d5}; Name: '{p.ProcessName}.exe'").OrderBy(x => x).ToArray();
                    var info = (data.Length == 1 ? LangStrings.FileLockMsg : LangStrings.FileLocksMsg).FormatInvariant(data.Join(Environment.NewLine));
                    if (MessageBoxEx.Show(info, AssemblyInfo.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                        return false;
                }
                locks.Where(p => !p.ProcessName.EndsWithEx("64Portable", "Portable64", "Portable")).ForEach(p => ProcessEx.Close(p));
                Thread.Sleep(400);
            }
            return true;
        }

        private void SetupPilot(Process process)
        {
            if (process?.HasExited != false)
                return;

            if (Log.DebugMode > 0)
                Log.Write($"Install: Run setup pilot for '{AppData.Name}'!");

            process.WaitForInputIdle(10000);
            try
            {
                var buttonsDict = CacheData.NsisButtons ?? throw new NotSupportedException();
                var setupLangId = AppData.Settings.ArchiveLangCode;
                var systemLangId = NativeHelper.GetUserDefaultUILanguage();

                // Put all the button labels in a list and order the
                // labels by installer language and system language
                // so that important languages are listed at top.
                var buttons = buttonsDict.OrderBy(x => x.Key != setupLangId)
                                         .ThenBy(x => x.Key != systemLangId)
                                         .SelectMany(x => x.Value)
                                         .Distinct()
                                         .ToList();

                var stopwatch = Stopwatch.StartNew();
                var minimized = new List<IntPtr>();
                while (stopwatch.Elapsed.TotalMinutes < 8d)
                {
                    if (process.HasExited)
                        break;

                    var window = FindWindow(process);
                    if (window == IntPtr.Zero)
                        continue;

                    // Ensures that a window is minimized once
                    // so that the user does not lose control.
                    if (!minimized.Contains(window) && NativeHelper.ShowWindowAsync(window, ShowWindowFlags.ShowMinNoActive))
                        minimized.Add(window);

                    // Look for available buttons that advance
                    // the install progress and press them.
                    foreach (var control in buttons.Select(b => LocalFindControlByCaption(window, b)).Where(c => c != IntPtr.Zero))
                        NativeHelper.SendMessage(control, 0xf5u, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }

            // In the unlikely case that the pilot failed.
            SetupSmasher(process);
            return;

            static IntPtr LocalFindControlByCaption(IntPtr parent, string caption)
            {
                const string pattern = @"(\(?)&((.\))?)";
                var control = IntPtr.Zero;
                do
                {
                    control = NativeHelper.FindWindowEx(parent, control, null, null);
                    if (control == IntPtr.Zero)
                        continue;

                    var sb = new StringBuilder(byte.MaxValue);
                    NativeHelper.GetWindowText(control, sb, byte.MaxValue);
                    var current = sb.ToStringThenClear();
                    if (current == caption)
                        return control;

                    var cur = Regex.Replace(Regex.Escape(current), pattern, string.Empty, RegexOptions.CultureInvariant);
                    var cap = Regex.Replace(Regex.Escape(caption), pattern, string.Empty, RegexOptions.CultureInvariant);
                    if (cur == cap)
                        return control;
                }
                while (control != IntPtr.Zero);
                return IntPtr.Zero;
            }
        }

        private void SetupSmasher(Process process)
        {
            if (process?.HasExited != false)
                return;

            if (Log.DebugMode > 0)
                Log.Write($"Install: Run button smasher for '{AppData.Name}' as fallback!");

            const int waitMax = 1500;
            const int waitMin = 200;

            var stopwatch = Stopwatch.StartNew();
            var instDirExist = DirectoryEx.Exists(AppData.InstallDir);
            var instDirLastWriteTime = DateTime.MinValue;
            var instDirCount = 0;
            var instFileCount = 0;
            var instDirSize = 0L;
            var tmp7ZipDir = Path.Combine(AppData.InstallDir, "7zTemp");
            var tmpItems = LocalGetTempItems()?.ToArray();
            var tmpFile = tmpItems?.FirstOrDefault() ?? string.Empty;
            var tmpDir = tmpItems?.SecondOrDefault() ?? string.Empty;
            var tmpFileCount = 0;
            var tmpDirSize = 0L;
            var waitTime = waitMin;

            try
            {
                while (stopwatch.Elapsed.TotalMinutes < 8d)
                {
                    if (process.HasExited)
                        break;
                    process.WaitForInputIdle();
                    if (process.HasExited)
                        break;
                    process.WaitForExit(waitTime);
                    if (process.HasExited)
                        break;
                    if (!process.Responding)
                        continue;

                    // Let's check if the installation process does
                    // anything so we can wait before doing anything.
                    if (DirectoryEx.Exists(tmp7ZipDir))
                    {
                        waitTime = waitMax;
                        continue;
                    }

                    // Check install directory updates, which
                    // may only work on a fresh reinstall.
                    if (!instDirExist && DirectoryEx.Exists(AppData.InstallDir))
                        if (LocalHasChanged(ref instDirLastWriteTime, Directory.GetLastWriteTime(AppData.InstallDir)) ||
                            LocalHasChanged(ref instDirCount, DirectoryEx.EnumerateDirectories(AppData.InstallDir, SearchOption.AllDirectories).Count()) ||
                            LocalHasChanged(ref instFileCount, DirectoryEx.EnumerateFiles(AppData.InstallDir, SearchOption.AllDirectories).Count()) ||
                            LocalHasChanged(ref instDirSize, DirectoryEx.GetSize(AppData.InstallDir)))
                        {
                            waitTime = waitMax;
                            continue;
                        }

                    // Check for temp setup directory changes.
                    if (File.Exists(tmpFile) && Directory.Exists(tmpDir))
                        if (LocalHasChanged(ref tmpFileCount, DirectoryEx.EnumerateFiles(tmpDir, SearchOption.AllDirectories).Count()) ||
                            LocalHasChanged(ref tmpDirSize, DirectoryEx.GetSize(tmpDir)))
                        {
                            waitTime = waitMax;
                            continue;
                        }

                    // Send the `ENTER` keystroke to the window,
                    // hopefully only when it is no longer busy.
                    var window = FindWindow(process);
                    if (window == IntPtr.Zero)
                        continue;
                    NativeHelper.PostMessage(window, 0x100, (IntPtr)0xd, (IntPtr)0);
                    waitTime = waitMin;
                }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }

            // In the almost impossible case that the
            // smasher was not successful either.
            SetupManually(process);
            return;

            static bool LocalHasChanged<T>(ref T curValue, T newValue)
            {
                var changed = !EqualityComparer<T>.Default.Equals(curValue, newValue);
                curValue = newValue;
                return changed;
            }

            IEnumerable<string> LocalGetTempItems()
            {
                var tempDir = Path.GetTempPath();
                var pattern = @"ns[a-z][A-F0-9]{4}\.tmp";
                foreach (var file in DirectoryEx.EnumerateFiles(tempDir))
                {
                    var name = Path.GetFileName(file);
                    if (!Regex.Match(name, pattern).Success || !((InstallProcessStartTime - File.GetLastWriteTime(file)).TotalSeconds < 8))
                        continue;
                    pattern = @$"{name.Substring(0, 5)}[A-F0-9]{{2}}\.tmp";
                    yield return file;
                    break;
                }
                foreach (var dir in DirectoryEx.EnumerateDirectories(tempDir))
                {
                    var name = Path.GetFileName(dir);
                    if (!Regex.Match(name, pattern).Success)
                        continue;
                    yield return dir;
                    break;
                }
            }
        }

        private void SetupManually(Process process)
        {
            if (process?.HasExited != false)
                return;

            if (Log.DebugMode > 0)
                Log.Write($"Install: '{AppData.Name}' requires user attention!");

            try
            {
                using var proc = Process.GetProcessById(process.Id);
                var hWnd = NativeHelper.FindWindow(null, proc.MainWindowTitle);
                NativeHelper.ShowWindowAsync(hWnd, ShowWindowFlags.ShowNormal);
                NativeHelper.SetForegroundWindow(hWnd);
                NativeHelper.SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0, SetWindowPosFlags.NoMove |
                                                                            SetWindowPosFlags.NoSize);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            process.WaitForExit();
        }

        private void MessyAppInstallerWorkaround(string appsDir)
        {
            // I have come across a few cases where an app was installed
            // in the parent directory rather than the install directory.
            var dirNames = new[]
            {
                "App",
                "Other"
            };
            for (var i = 0; i < 16; i++)
                try
                {
                    var appDirs = new[]
                    {
                        Path.Combine(appsDir, "App"),
                        Path.Combine(appsDir, "Data"),
                        Path.Combine(appsDir, "Other")
                    };
                    if (!appDirs.Any(Directory.Exists))
                        continue;
                    if (!Directory.Exists(AppData.InstallDir))
                        Directory.CreateDirectory(AppData.InstallDir);
                    else
                    {
                        BreakFileLocks(AppData.InstallDir);
                        foreach (var dir in dirNames.Select(x => Path.Combine(AppData.InstallDir, x)).Where(Directory.Exists))
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
                    break;
                }
                catch (Exception ex) when (ex.IsCaught())
                {
                    Log.Write(ex);
                    Thread.Sleep(1000);
                }
        }
    }
}
