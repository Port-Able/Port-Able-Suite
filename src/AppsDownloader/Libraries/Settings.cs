namespace AppsDownloader.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Properties;
    using SilDev;
    using SilDev.Compression;
    using SilDev.Drawing;
    using SilDev.Legacy;
    using DrawingSize = System.Drawing.Size;

    internal static class Settings
    {
        private static bool? _forceTransferRedirection;
        private static string _machineId, _transferDir;
        private static string[] _nsisButtons;

        internal static bool ForceTransferRedirection
        {
            get
            {
                if (!_forceTransferRedirection.HasValue)
                    _forceTransferRedirection = Ini.Read(Resources.ConfigSection, nameof(ForceTransferRedirection), false);
                return (bool)_forceTransferRedirection;
            }
        }

        internal static string MachineId =>
            _machineId ?? (_machineId = EnvironmentEx.MachineId.ToString(CultureInfo.InvariantCulture).Encrypt().Substring(24));

        internal static string[] NsisButtons
        {
            get
            {
                if (_nsisButtons != default)
                    return _nsisButtons;
                var buttonData = GZip.Decompress(Resources.NsisData).DeserializeObject<Dictionary<int, List<string>>>();
                if (buttonData == default)
                    return default;
                var langId = WinApi.NativeHelper.GetUserDefaultUILanguage();
                if (!buttonData.TryGetValue(langId, out var btnData))
                    return default;
                _nsisButtons = btnData.ToArray();
                if (langId != 1033 && langId != 2057 && buttonData.TryGetValue(1033, out btnData))
                    _nsisButtons = _nsisButtons.Concat(btnData).ToArray();
                return _nsisButtons;
            }
        }

        internal static string TransferDir
        {
            get
            {
                if (_transferDir != default)
                    return _transferDir;
                _transferDir = Ini.Read<string>(Resources.ConfigSection, nameof(TransferDir), CorePaths.TransferDir);
                if (DirectoryEx.Create(_transferDir))
                    return _transferDir;
                if (!_transferDir.EqualsEx(CorePaths.TransferDir))
                {
                    _transferDir = CorePaths.TransferDir;
                    if (DirectoryEx.Create(_transferDir))
                        return _transferDir;
                }
                _transferDir = Path.Combine(Path.GetTempPath(), "Port-Able Transfer");
                DirectoryEx.Create(_transferDir);
                return _transferDir;
            }
            set
            {
                var transferDir = value;
                if (!PathEx.IsValidPath(transferDir) || !Elevation.WritableLocation(transferDir))
                    return;
                if (!transferDir.EqualsEx(CorePaths.TransferDir))
                    transferDir = Path.Combine(transferDir, "Port-Able Transfer");
                if (!DirectoryEx.Create(transferDir))
                    return;
                DirectoryEx.TryDelete(_transferDir);
                _transferDir = transferDir;
                WriteValue(Resources.ConfigSection, nameof(TransferDir), _transferDir, CorePaths.TransferDir);
            }
        }

        internal static bool SkipWriteValue { get; set; } = false;

        internal static void Initialize()
        {
            WinApi.NativeHelper.SetProcessDPIAware();

            Log.FileDir = Path.Combine(CorePaths.TempDir, "Logs");

            Ini.SetFile(PathEx.LocalDir, "..", "Settings.ini");
            Ini.SortBySections = new[]
            {
                Resources.ConfigSection,
                "Launcher"
            };

            Log.AllowLogging(Ini.FilePath);

            if (Elevation.IsAdministrator)
            {
                var path = Path.Combine("HKCU\\Software\\Portable Apps Suite", CorePaths.HomeDir.Encrypt(ChecksumAlgorithm.Adler32), ProcessEx.CurrentId.ToString(CultureInfo.InvariantCulture));
                if (Reg.CreateNewSubKey(path))
                    AppDomain.CurrentDomain.ProcessExit += (s, e) => Reg.RemoveSubKey(path);
            }

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

        internal static void WriteValue<TValue>(string section, string key, TValue value, TValue defValue = default)
        {
            bool equals;
            try
            {
                equals = value.Equals(defValue);
            }
            catch (NullReferenceException ex) when (ex.IsCaught())
            {
                equals = (dynamic)value == (dynamic)defValue;
            }
            if (equals)
            {
                Ini.RemoveKey(section, key);
                if (!SkipWriteValue)
                    Ini.WriteDirect(section, key, null);
                if (!Ini.GetKeys(section).Any())
                {
                    Ini.RemoveSection(section);
                    if (!SkipWriteValue)
                        Ini.WriteDirect(section, null, null);
                }
                if (!SkipWriteValue)
                    CacheData.UpdateSettingsMerges(section);
                return;
            }
            Ini.Write(section, key, value);
            if (SkipWriteValue)
                return;
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
            private static int[] _customColors;
            private static bool? _highlightInstalled, _largeImages, _showGroups, _showGroupColors;
            private static FormWindowState? _state;

            internal static int[] CustomColors
            {
                get
                {
                    if (_customColors != default)
                        return _customColors;
                    var key = GetConfigKey(nameof(Window), nameof(CustomColors));
                    var value = FilterCostumColors(Json.Deserialize<int[]>(Ini.Read(Resources.ConfigSection, key)));
                    _customColors = value;
                    return _customColors;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(CustomColors));
                    _customColors = FilterCostumColors(value);
                    var colors = _customColors?.Where(x => x != 0xffffff).ToArray();
                    WriteValue(Resources.ConfigSection, key, colors?.Any() == true ? Json.Serialize(colors) : default);
                }
            }

            internal static bool HighlightInstalled
            {
                get
                {
                    if (_highlightInstalled.HasValue)
                        return (bool)_highlightInstalled;
                    var key = GetConfigKey(nameof(Window), nameof(HighlightInstalled));
                    _highlightInstalled = Ini.Read(Resources.ConfigSection, key, true);
                    return (bool)_highlightInstalled;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(HighlightInstalled));
                    _highlightInstalled = value;
                    WriteValue(Resources.ConfigSection, key, (bool)_highlightInstalled, true);
                }
            }

            internal static bool LargeImages
            {
                get
                {
                    if (_largeImages.HasValue)
                        return (bool)_largeImages;
                    var key = GetConfigKey(nameof(Window), nameof(LargeImages));
                    _largeImages = Ini.Read(Resources.ConfigSection, key, false);
                    return (bool)_largeImages;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(LargeImages));
                    _largeImages = value;
                    WriteValue(Resources.ConfigSection, key, (bool)_largeImages);
                }
            }

            internal static bool ShowGroups
            {
                get
                {
                    if (_showGroups.HasValue)
                        return (bool)_showGroups;
                    var key = GetConfigKey(nameof(Window), nameof(ShowGroups));
                    _showGroups = Ini.Read(Resources.ConfigSection, key, true);
                    return (bool)_showGroups;
                }
                set
                {
                    _showGroups = value;
                    var key = GetConfigKey(nameof(Window), nameof(ShowGroups));
                    WriteValue(Resources.ConfigSection, key, (bool)_showGroups, true);
                }
            }

            internal static bool ShowGroupColors
            {
                get
                {
                    if (_showGroupColors.HasValue)
                        return (bool)_showGroupColors;
                    var key = GetConfigKey(nameof(Window), nameof(ShowGroupColors));
                    _showGroupColors = Ini.Read(Resources.ConfigSection, key, false);
                    return (bool)_showGroupColors;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(ShowGroupColors));
                    _showGroupColors = value;
                    WriteValue(Resources.ConfigSection, key, (bool)_showGroupColors);
                }
            }

            internal static FormWindowState State
            {
                get
                {
                    if (_state.HasValue)
                        return (FormWindowState)_state;
                    var key = GetConfigKey(nameof(Window), nameof(State));
                    _state = Ini.Read(Resources.ConfigSection, key, FormWindowState.Normal);
                    return (FormWindowState)_state;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(State));
                    _state = value;
                    if (_state == FormWindowState.Minimized)
                        _state = FormWindowState.Normal;
                    WriteValue(Resources.ConfigSection, key, (FormWindowState)_state);
                }
            }

            private static int[] FilterCostumColors(params int[] colors)
            {
                var list = (colors ?? Array.Empty<int>()).ToList();
                var count = list.Count;
                if (count > 0)
                    list.Sort();
                while (list.Count < 16)
                    list.Add(0xffffff);
                if (count > 0)
                    list.Reverse();
                return list.ToArray();
            }

            internal static class Colors
            {
                private static Color _groupColor1, _groupColor2, _groupColor3, _groupColor4, _groupColor5, _groupColor6, _groupColor7, _groupColor8, _groupColor9, _groupColor11, _groupColor12;

                internal static Color GroupColor1
                {
                    get
                    {
                        if (_groupColor1 == default)
                            _groupColor1 = GetColor(nameof(GroupColor1));
                        return _groupColor1;
                    }
                    set
                    {
                        _groupColor1 = value;
                        if (_groupColor1 == default)
                            _groupColor1 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor1), _groupColor1);
                    }
                }

                internal static Color GroupColor2
                {
                    get
                    {
                        if (_groupColor2 == default)
                            _groupColor2 = GetColor(nameof(GroupColor2));
                        return _groupColor2;
                    }
                    set
                    {
                        _groupColor2 = value;
                        if (_groupColor2 == default)
                            _groupColor2 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor2), _groupColor2);
                    }
                }

                internal static Color GroupColor3
                {
                    get
                    {
                        if (_groupColor3 == default)
                            _groupColor3 = GetColor(nameof(GroupColor3));
                        return _groupColor3;
                    }
                    set
                    {
                        _groupColor3 = value;
                        if (_groupColor3 == default)
                            _groupColor3 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor3), _groupColor3);
                    }
                }

                internal static Color GroupColor4
                {
                    get
                    {
                        if (_groupColor4 == default)
                            _groupColor4 = GetColor(nameof(GroupColor4));
                        return _groupColor4;
                    }
                    set
                    {
                        _groupColor4 = value;
                        if (_groupColor4 == default)
                            _groupColor4 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor4), _groupColor4);
                    }
                }

                internal static Color GroupColor5
                {
                    get
                    {
                        if (_groupColor5 == default)
                            _groupColor5 = GetColor(nameof(GroupColor5));
                        return _groupColor5;
                    }
                    set
                    {
                        _groupColor5 = value;
                        if (_groupColor5 == default)
                            _groupColor5 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor5), _groupColor5);
                    }
                }

                internal static Color GroupColor6
                {
                    get
                    {
                        if (_groupColor6 == default)
                            _groupColor6 = GetColor(nameof(GroupColor6));
                        return _groupColor6;
                    }
                    set
                    {
                        _groupColor6 = value;
                        if (_groupColor6 == default)
                            _groupColor6 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor6), _groupColor6);
                    }
                }

                internal static Color GroupColor7
                {
                    get
                    {
                        if (_groupColor7 == default)
                            _groupColor7 = GetColor(nameof(GroupColor7));
                        return _groupColor7;
                    }
                    set
                    {
                        _groupColor7 = value;
                        if (_groupColor7 == default)
                            _groupColor7 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor7), _groupColor7);
                    }
                }

                internal static Color GroupColor8
                {
                    get
                    {
                        if (_groupColor8 == default)
                            _groupColor8 = GetColor(nameof(GroupColor8));
                        return _groupColor8;
                    }
                    set
                    {
                        _groupColor8 = value;
                        if (_groupColor8 == default)
                            _groupColor8 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor8), _groupColor8);
                    }
                }

                internal static Color GroupColor9
                {
                    get
                    {
                        if (_groupColor9 == default)
                            _groupColor9 = GetColor(nameof(GroupColor9));
                        return _groupColor9;
                    }
                    set
                    {
                        _groupColor9 = value;
                        if (_groupColor9 == default)
                            _groupColor9 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor9), _groupColor9);
                    }
                }

                internal static Color GroupColor11
                {
                    get
                    {
                        if (_groupColor11 == default)
                            _groupColor11 = GetColor(nameof(GroupColor11));
                        return _groupColor11;
                    }
                    set
                    {
                        _groupColor11 = value;
                        if (_groupColor11 == default)
                            _groupColor11 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor11), _groupColor11);
                    }
                }

                internal static Color GroupColor12
                {
                    get
                    {
                        if (_groupColor12 == default)
                            _groupColor12 = GetColor(nameof(GroupColor12));
                        return _groupColor12;
                    }
                    set
                    {
                        _groupColor12 = value;
                        if (_groupColor12 == default)
                            _groupColor12 = GetColor(nameof(GroupColor1));
                        WriteValue(nameof(GroupColor12), _groupColor12);
                    }
                }

                internal static Color GetDefColor(string key)
                {
                    switch (key)
                    {
                        case nameof(GroupColor1):
                            return Color.FromArgb(0xff, 0xff, 0x99);
                        case nameof(GroupColor2):
                            return Color.FromArgb(0xff, 0xff, 0xcc);
                        case nameof(GroupColor3):
                            return Color.FromArgb(0xd5, 0xd5, 0xdf);
                        case nameof(GroupColor4):
                            return Color.FromArgb(0xbb, 0xe9, 0xec);
                        case nameof(GroupColor5):
                            return Color.FromArgb(0xee, 0xd9, 0xce);
                        case nameof(GroupColor6):
                            return Color.FromArgb(0xff, 0xcc, 0xff);
                        case nameof(GroupColor7):
                            return Color.FromArgb(0xcc, 0xcc, 0xff);
                        case nameof(GroupColor8):
                            return Color.FromArgb(0xb5, 0xff, 0x99);
                        case nameof(GroupColor9):
                            return Color.FromArgb(0xc5, 0xe2, 0xe2);
                        case nameof(GroupColor11):
                            return Color.FromArgb(0xff, 0x95, 0x95);
                        case nameof(GroupColor12):
                            return Color.FromArgb(0xff, 0x75, 0x45);
                        default:
                            return Color.Empty;
                    }
                }

                private static Color GetColor(string key)
                {
                    var str = GetConfigKey(nameof(Window), nameof(Colors), key);
                    var html = Ini.Read(Resources.ConfigSection, str);
                    var color = ColorEx.FromHtml(html, GetDefColor(key), byte.MaxValue);
                    return color;
                }

                private static void WriteValue(string key, Color color)
                {
                    var str = GetConfigKey(nameof(Window), nameof(Colors), key);
                    if (color == GetDefColor(key))
                    {
                        WriteValue<string>(Resources.ConfigSection, str, null);
                        return;
                    }
                    var html = ColorEx.ToHtml(color);
                    WriteValue<string>(Resources.ConfigSection, str, html);
                }
            }

            internal static class Size
            {
                internal const int MinimumHeight = 125, MinimumWidth = 760;
                private static DrawingSize _default, _maximum, _minimum;
                private static int _width, _height;

                internal static DrawingSize Default
                {
                    get
                    {
                        if (_default == default)
                            _default = new DrawingSize(Minimum.Width, (int)Math.Round(Maximum.Height / 1.5d));
                        return _default;
                    }
                }

                internal static DrawingSize Maximum
                {
                    get
                    {
                        if (_maximum != default)
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

                internal static DrawingSize Minimum
                {
                    get
                    {
                        if (_minimum == default)
                            _minimum = DpiSize(MinimumWidth, MinimumHeight);
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
                        if (_width != default)
                            return _width;
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Width));
                        var value = Ini.Read(Resources.ConfigSection, key, DefaultWidth);
                        _width = ValidateValue(value, MinimumWidth, MaximumWidth);
                        return _width;
                    }
                    set
                    {
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Width));
                        _width = State != FormWindowState.Maximized ? ValidateValue(value, MinimumWidth, MaximumWidth) : DefaultWidth;
                        WriteValue(Resources.ConfigSection, key, _width, DefaultWidth);
                    }
                }

                internal static int Height
                {
                    get
                    {
                        if (_height != default)
                            return _height;
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Height));
                        var value = Ini.Read(Resources.ConfigSection, key, DefaultHeight);
                        _height = ValidateValue(value, MinimumHeight, MaximumHeight);
                        return _width;
                    }
                    set
                    {
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Height));
                        _height = State != FormWindowState.Maximized ? ValidateValue(value, MinimumHeight, MaximumHeight) : DefaultHeight;
                        WriteValue(Resources.ConfigSection, key, _height, DefaultHeight);
                    }
                }

                internal static void Refresh()
                {
                    _minimum = default;
                    _maximum = default;
                    _default = default;
                    _width = default;
                    _height = default;
                }

                private static DrawingSize DpiSize(int width, int height)
                {
                    var handle = WinApi.NativeHelper.GetDesktopWindow();
                    var size = DrawingSize.Empty;
                    using (var graphics = Graphics.FromHwnd(handle))
                    {
                        if (width > 0)
                            size.Width = (int)Math.Floor(graphics.DpiX / 96d * width);
                        if (height > 0)
                            size.Height = (int)Math.Floor(graphics.DpiY / 96d * height);
                    }
                    return size;
                }
            }
        }
    }
}
