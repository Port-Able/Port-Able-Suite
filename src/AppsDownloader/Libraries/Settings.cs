namespace AppsDownloader.Libraries
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using SilDev;

    internal static class Settings
    {
        internal const string Section = "Downloader";
#if x86
        internal const string Title = "Apps Downloader";
#else
        internal const string Title = "Apps Downloader (64-bit)";
#endif
        private static bool? _highlightInstalled, _showGroups, _showGroupColors;
        private static string _machineId;

        internal static bool DeveloperVersion =>
            Ini.Read("Launcher", nameof(DeveloperVersion), false);

        internal static bool HighlightInstalled
        {
            get
            {
                if (_highlightInstalled.HasValue)
                    return (bool)_highlightInstalled;
                _highlightInstalled = Ini.Read(Section, nameof(HighlightInstalled), true);
                return (bool)_highlightInstalled;
            }
            set
            {
                _highlightInstalled = value;
                WriteValue(Section, nameof(HighlightInstalled), (bool)_highlightInstalled, true);
            }
        }

        internal static string MachineId
        {
            get
            {
                if (_machineId == default(string))
                    _machineId = EnvironmentEx.MachineId.ToString().Encrypt().Substring(24);
                return _machineId;
            }
        }

        internal static bool ShowGroups
        {
            get
            {
                if (_showGroups.HasValue)
                    return (bool)_showGroups;
                _showGroups = Ini.Read(Section, nameof(ShowGroups), true);
                return (bool)_showGroups;
            }
            set
            {
                _showGroups = value;
                WriteValue(Section, nameof(ShowGroups), (bool)_showGroups, true);
            }
        }

        internal static bool ShowGroupColors
        {
            get
            {
                if (_showGroupColors.HasValue)
                    return (bool)_showGroupColors;
                _showGroupColors = Ini.Read(Section, nameof(ShowGroupColors), false);
                return (bool)_showGroupColors;
            }
            set
            {
                _showGroupColors = value;
                WriteValue(Section, nameof(ShowGroupColors), (bool)_showGroupColors);
            }
        }

        internal static bool SkipWriteValue { get; set; } = false;

        internal static void Initialize()
        {
#if x86
            if (Environment.Is64BitOperatingSystem)
            {
                var appsDownloader64 = PathEx.Combine(PathEx.LocalDir, $"{ProcessEx.CurrentName}64.exe");
                if (File.Exists(appsDownloader64))
                {
                    ProcessEx.Start(appsDownloader64, EnvironmentEx.CommandLine(false));
                    Environment.ExitCode = 0;
                    Environment.Exit(Environment.ExitCode);
                }
            }
#endif

            Log.FileDir = Path.Combine(CorePaths.TempDir, "Logs");

            Ini.SetFile(PathEx.LocalDir, "..", "Settings.ini");
            Ini.SortBySections = new[]
            {
                Section,
                "Launcher"
            };

            Log.AllowLogging(Ini.FilePath, "DebugMode", Ini.GetRegex(false));

            if (Recovery.AppsSuiteIsHealthy())
                return;
            Environment.ExitCode = 1;
            Environment.Exit(Environment.ExitCode);
        }

        internal static string GetConfigKey(params string[] keys)
        {
            if (keys == null || !keys.Any())
                throw new ArgumentNullException(nameof(keys));
            if (keys.Length == 1)
                return keys.First();
            var sb = new StringBuilder();
            var len = keys.Length - 1;
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                sb.Append(key);
                if (i < len)
                    sb.Append('.');
            }
            return sb.ToString();
        }

        internal static void WriteValue<TValue>(string section, string key, TValue value, TValue defValue = default(TValue))
        {
            if (SkipWriteValue)
                return;
            bool equals;
            try
            {
                equals = value.Equals(defValue);
            }
            catch (NullReferenceException)
            {
                equals = (dynamic)value == (dynamic)defValue;
            }
            if (equals)
            {
                Ini.RemoveKey(section, key);
                Ini.WriteDirect(section, key, null);
                if (!Ini.GetKeys(section).Any())
                {
                    Ini.RemoveSection(section);
                    Ini.WriteDirect(section, null, null);
                }
                CacheData.UpdateSettingsMerges(section);
                return;
            }
            Ini.Write(section, key, value);
            Ini.WriteDirect(section, key, value);
            CacheData.UpdateSettingsMerges(section);
        }

        private static int ValidateValue(int value, int minValue, int maxValue)
        {
            var current = Math.Max(value, minValue);
            return Math.Min(current, maxValue);
        }

        internal static class Window
        {
            private static FormWindowState? _state;

            internal static FormWindowState State
            {
                get
                {
                    if (!_state.HasValue)
                        _state = Ini.Read(Section, GetConfigKey(nameof(Window), nameof(State)), FormWindowState.Normal);
                    return (FormWindowState)_state;
                }
                set
                {
                    _state = value;
                    if (_state == FormWindowState.Minimized)
                        _state = FormWindowState.Normal;
                    WriteValue(Section, GetConfigKey(nameof(Window), nameof(State)), (FormWindowState)_state);
                }
            }

            internal static class Size
            {
                internal const int MinimumHeight = 125;
                internal const int MinimumWidth = 760;
                private static System.Drawing.Size _default, _maximum, _minimum;
                private static int _width, _height;

                internal static System.Drawing.Size Default
                {
                    get
                    {
                        if (_default == default(System.Drawing.Size))
                            _default = new System.Drawing.Size(MinimumWidth, (int)Math.Round(MaximumHeight / 1.5d));
                        return _default;
                    }
                }

                internal static System.Drawing.Size Maximum
                {
                    get
                    {
                        if (_maximum != default(System.Drawing.Size))
                            return _maximum;
                        var curPos = WinApi.NativeHelper.GetCursorPos();
                        var screen = Screen.PrimaryScreen;
                        foreach (var scr in Screen.AllScreens)
                        {
                            if (!scr.Bounds.Contains(curPos))
                                continue;
                            screen = scr;
                            break;
                        }
                        _maximum = screen.WorkingArea.Size;
                        return _maximum;
                    }
                }

                internal static System.Drawing.Size Minimum
                {
                    get
                    {
                        if (_minimum == default(System.Drawing.Size))
                            _minimum = new System.Drawing.Size(MinimumWidth, MinimumHeight);
                        return _minimum;
                    }
                }

                internal static int DefaultWidth =>
                    Default.Width;

                internal static int DefaultHeight =>
                    Default.Height;

                internal static int MaximumWidth =>
                    Maximum.Width;

                internal static int MaximumHeight =>
                    Maximum.Height;

                internal static int Width
                {
                    get
                    {
                        if (_width != default(int))
                            return _width;
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Width));
                        var value = Ini.Read(Section, key, DefaultWidth);
                        _width = ValidateValue(value, MinimumWidth, MaximumWidth);
                        return _width;
                    }
                    set
                    {
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Width));
                        _width = State != FormWindowState.Maximized ? ValidateValue(value, MinimumWidth, MaximumWidth) : DefaultWidth;
                        WriteValue(Section, key, _width, DefaultWidth);
                    }
                }

                internal static int Height
                {
                    get
                    {
                        if (_height != default(int))
                            return _height;
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Height));
                        var value = Ini.Read(Section, key, DefaultHeight);
                        _height = ValidateValue(value, MinimumHeight, MaximumHeight);
                        return _width;
                    }
                    set
                    {
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Height));
                        _height = State != FormWindowState.Maximized ? ValidateValue(value, MinimumHeight, MaximumHeight) : DefaultHeight;
                        WriteValue(Section, key, _height, DefaultHeight);
                    }
                }
            }
        }
    }
}
