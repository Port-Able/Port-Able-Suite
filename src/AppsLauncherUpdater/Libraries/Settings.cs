namespace Updater.Libraries
{
    using System;
    using System.Drawing;
    using System.IO;
    using SilDev;

    internal static class Settings
    {
        internal const string Section = "Launcher";
        internal const string Title = "Port-Able Suite Updater";
        private static string _language, _registryPath;
        private static int? _updateChannel;

        internal static string Language => 
            _language ?? (_language = Ini.Read<string>(Section, nameof(Language), global::Language.SystemLang));

        internal static string RegistryPath => 
            _registryPath ?? (_registryPath = "HKCU\\Software\\Portable Apps Suite");

        internal static int ScreenDpi
        {
            get
            {
                int dpi;
                using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    var max = Math.Max(graphics.DpiX, graphics.DpiY);
                    dpi = (int)Math.Ceiling(max);
                }
                return dpi;
            }
        }

        internal static DateTime LastUpdateCheck =>
            Ini.Read<DateTime>(Section, nameof(LastUpdateCheck));

        internal static UpdateChannelOptions UpdateChannel
        {
            get
            {
                if (_updateChannel.HasValue)
                    return (UpdateChannelOptions)_updateChannel;
                _updateChannel = Ini.Read(Section, nameof(UpdateChannel), (int)UpdateChannelOptions.Release);
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
                Section
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
