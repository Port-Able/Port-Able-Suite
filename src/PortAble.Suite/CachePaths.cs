namespace PortAble;

using System.IO;
using SilDev;

/// <summary>
///     Provides Port Able Suite paths to cached files.
/// </summary>
public static class CachePaths
{
    private static string _appDirs,
                          _appImages,
                          _appImagesLarge,
                          _appInfo,
                          _appSuppliers,
                          _transferDir,
                          _nsisButtons,
                          _currentAppImages,
                          _currentAppInfo,
                          _currentBackgroundImage,
                          _currentTypeData,
                          _currentHost;

    /// <summary>
    ///     The path to the database file that stores a collection of directories in
    ///     which to search for installed apps.
    /// </summary>
    public static string AppDirs => _appDirs ??= Path.Combine(CorePaths.DataDir, "AppDirs.dat");

    /// <summary>
    ///     The path to the downloaded database file that stores small app images.
    /// </summary>
    public static string AppImages => _appImages ??= Path.Combine(CorePaths.DataDir, "AppImages.dat");

    /// <summary>
    ///     The path to the downloaded database file that stores large app images.
    /// </summary>
    public static string AppImagesLarge => _appImagesLarge ??= Path.Combine(CorePaths.DataDir, "AppImagesLarge.dat");

    /// <summary>
    ///     The path to the downloaded database file that stores app information.
    /// </summary>
    public static string AppInfo => _appInfo ??= Path.Combine(CorePaths.DataDir, $"AppInfo{ActionGuid.IsUpdateInstance.ToInt32()}.dat");

    /// <summary>
    ///     The path to the downloaded database file that stores app provider server
    ///     information.
    /// </summary>
    public static string AppSuppliers => _appSuppliers ??= Path.Combine(CorePaths.DataDir, "AppSuppliers.dat");

    /// <summary>
    ///     The directory for downloaded files.
    /// </summary>
    public static string TransferDir
    {
        get => _transferDir ??= CorePaths.TransferDir;
        set
        {
            var dir = PathEx.Combine(value);
            if (Directory.Exists(dir))
                _transferDir = dir;
        }
    }

    /// ReSharper disable CommentTypo
    /// <summary>
    ///     The path to the downloaded database file that stores the logic of the
    ///     Nullsoft Scriptable Install System (NSIS) buttons used by some app
    ///     installers.
    /// </summary>
    public static string NsisButtons => _nsisButtons ??= Path.Combine(CorePaths.DataDir, "NsisButtons.dat");

    /// <summary>
    ///     The path to a database file that stores small app images currently in use.
    /// </summary>
    public static string CurrentAppImages => _currentAppImages ??= Path.Combine(CorePaths.DataDir, "CurrentAppImages.dat");

    /// <summary>
    ///     The path to the database file that stores app information from currently
    ///     used apps.
    /// </summary>
    public static string CurrentAppInfo => _currentAppInfo ??= Path.Combine(CorePaths.DataDir, "CurrentAppInfo.dat");

    /// <summary>
    ///     The path to the Apps Launcher`s serialized background to improve loading
    ///     time.
    /// </summary>
    public static string CurrentBackgroundImage => _currentBackgroundImage ??= Path.Combine(CorePaths.DataDir, "CurrentImageBg.dat");

    /// <summary>
    ///     The path to the database file that stores information about which file type
    ///     was launched with which app to suggest the correct app next time.
    /// </summary>
    public static string CurrentTypeData => _currentTypeData ??= Path.Combine(CorePaths.DataDir, "CurrentTypeData.dat");

    /// <summary>
    ///     The path to the file containing the current handle that can receive command
    ///     line arguments from other instances.
    /// </summary>
    public static string CurrentHost => _currentHost ??= Path.Combine(CorePaths.DataDir, "CurrentHost.dat");
}