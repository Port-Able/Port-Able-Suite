namespace Updater.Libraries
{
    using System;
    using System.Drawing;
    using System.IO;
    using Properties;
    using SilDev;

    internal static class Settings
    {
        private static string _language, _registryPath;
        private static int? _updateChannel;

        internal static string Language =>
            _language ?? (_language = Ini.Read<string>(Resources.ConfigSection, nameof(Language), global::Language.SystemLang));

        internal static string RegistryPath =>
            _registryPath ?? (_registryPath = "HKCU\\Software\\Portable Apps Suite");

        internal static int ScreenDpi
        {
            get
            {
                int dpi;
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    var max = Math.Max(g.DpiX, g.DpiY);
                    dpi = (int)Math.Ceiling(max);
                }
                return dpi;
            }
        }

        internal static DateTime LastUpdateCheck =>
            Ini.Read<DateTime>(Resources.ConfigSection, nameof(LastUpdateCheck));

        internal static UpdateChannelOptions UpdateChannel
        {
            get
            {
                if (_updateChannel.HasValue)
                    return (UpdateChannelOptions)_updateChannel;
                _updateChannel = Ini.Read(Resources.ConfigSection, nameof(UpdateChannel), (int)UpdateChannelOptions.Release);
                return (UpdateChannelOptions)_updateChannel;
            }
        }

        internal static void Initialize()
        {
            Log.FileDir = Path.Combine(CorePaths.TempDir, "Logs");

            Ini.SetFile(CorePaths.HomeDir, "Settings.ini");
            Ini.SortBySections = new[]
            {
                "Downloader",
                Resources.ConfigSection
            };

            Log.AllowLogging(Ini.FilePath, "DebugMode", Ini.GetRegex(false));
        }

        internal enum UpdateChannelOptions
        {
            Release,
            Beta
        }
    }
}
