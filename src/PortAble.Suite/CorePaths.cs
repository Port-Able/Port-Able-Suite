namespace PortAble;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using SilDev;
using SilDev.Compression.Archiver;
using SilDev.QuickWmi;

/// <summary>
///     Provides paths to static resources and directories that should never be
///     changed.
/// </summary>
public static class CorePaths
{
    private static string[] _appDirs, _appEnvDirs;

    private static string _homeDir,
                          _appsDir,
                          _appsEnvDir,
                          _appImages,
                          _appImagesLarge,
                          _appsLauncher,
                          _appsDownloader,
                          _appsSuiteUpdater,
                          _userDir,
                          _userDocumentsDir,
                          _userPicturesDir,
                          _userMusicDir,
                          _userVideosDir,
                          _dataDir,
                          _settingsDir,
                          _appSettingsDir,
                          _systemProfileDir,
                          _fileTypeAssocDir,
                          _logsDir,
                          _fileArchiver,
                          _transferDir,
                          _customAppSuppliersDir,
                          _nsisButtons;

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
    ///     The root directory where installed apps are located.
    /// </summary>
    public static string AppsDir => _appsDir ??= Path.Combine(HomeDir, "Apps");

    /// <summary>
    ///     A relative environment path of <see cref="AppsDir"/>.
    /// </summary>
    public static string AppsEnvDir => _appsEnvDir ??= EnvironmentEx.GetVariableWithPath(AppsDir);

    /// <summary>
    ///     A collection of subdirectories where installed apps are located.
    /// </summary>
    public static IReadOnlyList<string> AppDirs => _appDirs ??= new[]
    {
        AppsDir,
        Path.Combine(AppsDir, ".free"),
        Path.Combine(AppsDir, ".repack"),
        Path.Combine(AppsDir, ".share")
    };

    /// <summary>
    ///     A collection of relative environment variable paths of
    ///     <see cref="AppDirs"/>.
    /// </summary>
    public static IReadOnlyList<string> AppEnvDirs => _appEnvDirs ??= new[]
    {
        AppsEnvDir,
        Path.Combine(AppsEnvDir, ".free"),
        Path.Combine(AppsEnvDir, ".repack"),
        Path.Combine(AppsEnvDir, ".share")
    };

    /// <summary>
    ///     The path to the default database file where small app images are stored.
    /// </summary>
    public static string AppImages => _appImages ??= Path.Combine(HomeDir, "Assets\\AppImages.dat");

    /// <summary>
    ///     The path to the default database file where large app images are stored.
    /// </summary>
    public static string AppImagesLarge => _appImagesLarge ??= Path.Combine(HomeDir, "Assets\\AppImagesLarge.dat");

    /// <summary>
    ///     The path to the apps suite launcher executable.
    /// </summary>
    public static string AppsLauncher => _appsLauncher ??= Path.Combine(HomeDir, "AppsLauncher.exe");

    /// <summary>
    ///     The path to the apps downloader executable.
    /// </summary>
    public static string AppsDownloader => _appsDownloader ??= Path.Combine(HomeDir, "Binaries\\AppsDownloader.exe");

    /// <summary>
    ///     The path to the apps suite updater executable.
    /// </summary>
    public static string AppsSuiteUpdater => _appsSuiteUpdater ??= Path.Combine(HomeDir, "Binaries\\Updater.exe");

    /// <summary>
    ///     The users portable profile directory.
    /// </summary>
    public static string UserDir => _userDir ??= Path.Combine(HomeDir, "Documents");

    /// <summary>
    ///     The users portable documents directory.
    /// </summary>
    public static string UserDocumentsDir => _userDocumentsDir ??= Path.Combine(UserDir, "Documents");

    /// <summary>
    ///     The users portable music directory.
    /// </summary>
    public static string UserMusicDir => _userMusicDir ??= Path.Combine(UserDir, "Music");

    /// <summary>
    ///     The users portable pictures directory.
    /// </summary>
    public static string UserPicturesDir => _userPicturesDir ??= Path.Combine(UserDir, "Pictures");

    /// <summary>
    ///     The users portable videos directory.
    /// </summary>
    public static string UserVideosDir => _userVideosDir ??= Path.Combine(UserDir, "Videos");

    /// <summary>
    ///     The path to the directory used for temporary files, cached data, and
    ///     settings.
    /// </summary>
    public static string DataDir
    {
        get
        {
            if (_dataDir != default)
                return _dataDir;
            _dataDir = Path.Combine(UserDir, "AppsSuiteData");
            if (Directory.Exists(_dataDir))
                return _dataDir;
            if (!DirectoryEx.Create(_dataDir))
            {
                using var process = ProcessEx.Start(AppsLauncher, ActionGuid.RepairDirs, false, false);
                if (process?.HasExited == false)
                    process.WaitForExit();
            }
            if (!DirectoryEx.Create(_dataDir))
                throw new PathNotFoundException(_dataDir);
            return _dataDir;
        }
    }

    /// <summary>
    ///     The directory where all settings are stored.
    /// </summary>
    public static string SettingsDir => _settingsDir ??= Path.Combine(DataDir, "Settings");

    /// <summary>
    ///     The directory where app settings are stored.
    /// </summary>
    public static string AppSettingsDir => _appSettingsDir ??= Path.Combine(SettingsDir, "AppSettings");

    public static string SystemProfileDir => _systemProfileDir ??= Path.Combine(DataDir, "SystemProfile", GetUniqueId(true));

    public static string FileTypeAssocDir => _fileTypeAssocDir ??= Path.Combine(SystemProfileDir, "FileTypeAssoc");

    /// <summary>
    ///     The LOG file directory for extended information and debugging.
    ///     <para>
    ///         Please note that this directory must be created manually to enable
    ///         logging.
    ///     </para>
    /// </summary>
    public static string LogsDir => _logsDir ??= Path.Combine(DataDir, "Logs");

    /// <summary>
    ///     The URL to the Port-Able apps suite project`s GitHub repository.
    /// </summary>
    public static string RepositoryUrl => "https://github.com/Port-Able/Port-Able-Suite";

    /// <summary>
    ///     The URL format of the download redirection service, which is used in the
    ///     rare case when the client only has an IPv6 connection and the server only
    ///     offers an IPv4 connection.
    /// </summary>
    public static string RedirectDlUrlFormat => "https://transfer.0-9a-z.de/index.php?base={0}";

    /// <summary>
    ///     The path to the `7-Zip GUI` executable file used to install some apps.
    /// </summary>
    public static string FileArchiver
    {
        get
        {
            if (_fileArchiver != default)
                return _fileArchiver;
            var path = PathEx.Combine(HomeDir, "Binaries\\Helper\\7z");
            if (Environment.Is64BitProcess)
                path = Path.Combine(path, "x64");
            SevenZip.DefaultArchiver.Location = path;
            return _fileArchiver = SevenZip.DefaultArchiver.ExtractExePath;
        }
    }

    /// <summary>
    ///     The default directory for downloaded files.
    /// </summary>
    public static string TransferDir => _transferDir ??= Path.Combine(DataDir, "Transfer");

    /// <summary>
    ///     The path to the downloaded database file that stores app provider server
    ///     information.
    /// </summary>
    public static string CustomAppSuppliersDir => _customAppSuppliersDir ??= Path.Combine(DataDir, "CustomAppSuppliers");

    /// ReSharper disable CommentTypo
    /// <summary>
    ///     The path to the default database file that stores the logic of the Nullsoft
    ///     Scriptable Install System (NSIS) buttons used by some app installers.
    /// </summary>
    public static string NsisButtons => _nsisButtons ??= Path.Combine(HomeDir, "Assets\\NsisButtons.dat");

    // This ID should be different on each individual
    // device, optionally and for each system user.
    private static string GetUniqueId(bool includeUserSid)
    {
        var table = new HashSet<string>
        {
            Win32_OperatingSystem.SerialNumber,
            Win32_OperatingSystem.InstallDate.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)
        };
        if (!includeUserSid)
            return LocalReturn(table);
        var cur = WindowsIdentity.GetCurrent();
        var sid = cur.User?.Value;
        table.Add(sid ?? cur.Name);
        return LocalReturn(table);

        static string LocalReturn(IEnumerable<string> table)
        {
            return table.Aggregate(string.Empty, (c, n) => $"{c}{n}\n")
                        .Encrypt(ChecksumAlgorithm.Sha512)
                        .Substring(Crypto.Sha512.HashLength - 12)
                        .ToUpperInvariant();
        }
    }
}