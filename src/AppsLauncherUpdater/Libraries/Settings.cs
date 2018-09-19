namespace Updater.Libraries
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using LangResources;
    using SilDev;

    internal static class Settings
    {
        internal const string Section = "Launcher";
        private static string _language, _registryPath;
        private static int? _updateChannel;

        internal static string Language
        {
            get
            {
                if (_language == default(string))
                    _language = Ini.Read<string>(Section, nameof(Language), global::Language.SystemLang);
                return _language;
            }
        }

        internal static string RegistryPath
        {
            get
            {
                if (_registryPath == default(string))
                    _registryPath = "HKCU\\Software\\Portable Apps Suite";
                return _registryPath;
            }
        }

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

            if (Recovery.AppsSuiteIsHealthy())
                return;
            if (MessageBox.Show(global::Language.GetText(nameof(en_US.RequirementsErrorMsg)), @"Port-Able Suite", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                Process.Start(CorePaths.RepoReleasesUrl);
            Environment.ExitCode = 1;
            Environment.Exit(Environment.ExitCode);
        }

        internal enum UpdateChannelOptions
        {
            Release,
            Beta
        }
    }
}
