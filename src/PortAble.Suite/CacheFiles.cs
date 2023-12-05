namespace PortAble
{
    using System.IO;
    using SilDev;

    /// <summary>
    ///     Provides Port Able Suite paths to cached files.
    /// </summary>
    public static class CacheFiles
    {
        /// <summary>
        ///     The path to the downloaded database file that stores small app images.
        /// </summary>
        public static string AppImages { get; } = Path.Combine(CorePaths.DataDir, "AppImages.dat");

        /// <summary>
        ///     The path to the downloaded database file that stores large app images.
        /// </summary>
        public static string AppImagesLarge { get; } = Path.Combine(CorePaths.DataDir, "AppImagesLarge.dat");

        /// <summary>
        ///     The path to the downloaded database file that stores app information.
        /// </summary>
        public static string AppInfo { get; } = Path.Combine(CorePaths.DataDir, $"AppInfo{ActionGuid.IsUpdateInstance.ToInt32()}.dat");

        /// <summary>
        ///     The path to the downloaded database file that stores app provider server
        ///     information.
        /// </summary>
        public static string AppSuppliers { get; } = Path.Combine(CorePaths.DataDir, "AppSuppliers.dat");

        /// <summary>
        ///     ***WIP
        /// </summary>
        public static string TransferDir { get; } = Path.Combine(CorePaths.DataDir, "Transfer");

        /// ReSharper disable CommentTypo
        /// <summary>
        ///     The path to the downloaded database file that stores the logic of the
        ///     Nullsoft Scriptable Install System (NSIS) buttons used by some app
        ///     installers.
        /// </summary>
        public static string NsisButtons { get; } = Path.Combine(CorePaths.DataDir, "NsisButtons.dat");

        /// <summary>
        ///     The path to a database file that stores small app images currently in use.
        /// </summary>
        public static string CurrentImages { get; } = Path.Combine(CorePaths.DataDir, "CurrentImages.dat");

        /// <summary>
        ///     The path to the Apps Launcher`s serialized background to improve loading
        ///     time.
        /// </summary>
        public static string CurrentImageBg { get; } = Path.Combine(CorePaths.DataDir, "CurrentImageBg.dat");

        /// <summary>
        ///     The path to the database file that stores app information from currently
        ///     used apps.
        /// </summary>
        public static string CurrentAppInfo { get; } = Path.Combine(CorePaths.DataDir, "CurrentAppInfo.dat");

        /// <summary>
        ///     The path to the database file that stores information about which file type
        ///     was launched with which app to suggest the correct app next time.
        /// </summary>
        public static string CurrentTypeData { get; } = Path.Combine(CorePaths.DataDir, "CurrentTypeData.dat");
    }
}
