﻿namespace Updater.Libraries
{
    using System;
    using System.IO;
    using Properties;
    using SilDev;
    using SilDev.Legacy;

    internal static class Settings
    {
        internal enum UpdateChannelOptions
        {
            Release,
            Beta
        }

        private static string _language;
        private static int? _updateChannel;

        internal static string Language =>
            _language ??= Ini.Read<string>(Resources.ConfigSection, nameof(Language), global::Language.SystemLang);

        internal static string RegistryPath => "HKCU\\Software\\Portable Apps Suite";

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
            WinApi.NativeHelper.SetProcessDPIAware();

            if (Log.DebugMode > 0)
            {
                Log.FileDir = Path.Combine(CorePaths.TempDir, "Logs");
                DirectoryEx.Create(Log.FileDir);
            }

            Ini.SetFile(CorePaths.HomeDir, "Settings.ini");
            Ini.SortBySections = new[]
            {
                "Downloader",
                Resources.ConfigSection
            };

            Log.AllowLogging(Ini.FilePath);
        }
    }
}
