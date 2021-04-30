namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Windows;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Legacy;
    using SilDev.QuickWmi;
    using DrawingSize = System.Drawing.Size;

    internal static class Settings
    {
        internal enum UpdateChannelOptions
        {
            Release,
            Beta
        }

        internal enum UpdateCheckOptions
        {
            Never,
            HourlyFull,
            HourlyOnlyApps,
            HourlyOnlyAppsSuite,
            DailyFull,
            DailyOnlyApps,
            DailyOnlyAppsSuite,
            MonthlyFull,
            MonthlyOnlyApps,
            MonthlyOnlyAppsSuite
        }

        internal const string EnvironmentVariable = "AppsSuiteDir";
        private static string[] _appDirs;
        private static string _currentDirectory, _iconResourcePath, _language, _lastItem, _registryPath, _systemInstallId;
        private static bool? _startMenuIntegration;
        private static int? _updateChannel, _updateCheck;

        internal static string[] AppDirs
        {
            get
            {
                if (_appDirs != default)
                    return _appDirs;
                var value = Ini.Read<string>(Resources.ConfigSection, nameof(AppDirs))?.DecodeString();
                var dirs = value?.SplitNewLine();
                _appDirs = CorePaths.AppDirs;
                if (dirs?.Any() == true)
                    _appDirs = _appDirs.Concat(dirs.Select(PathEx.Combine)).Where(Directory.Exists).ToArray();
                return _appDirs;
            }
            set
            {
                _appDirs = CorePaths.AppDirs;
                var dirs = value;
                if (dirs?.Any() == true)
                    _appDirs = _appDirs.Concat(dirs.Select(PathEx.Combine)).Where(Directory.Exists).ToArray();
                var encoded = dirs?.Join(Environment.NewLine).Encode();
                WriteValue(Resources.ConfigSection, nameof(AppDirs), encoded);
            }
        }

        internal static string CurrentDirectory
        {
            get => _currentDirectory ?? (_currentDirectory = Ini.Read<string>(Resources.ConfigSection, nameof(CurrentDirectory)));
            set
            {
                _currentDirectory = value;
                WriteValueDirect(Resources.ConfigSection, nameof(CurrentDirectory), _currentDirectory);
            }
        }

        internal static bool DeveloperVersion { get; } = Ini.Read(Resources.ConfigSection, nameof(DeveloperVersion), false) || FileEx.Exists(".\\..\\Port-Able-Suite.sln");

        internal static string IconResourcePath
        {
            get
            {
                if (_iconResourcePath != default)
                    return _iconResourcePath;
                _iconResourcePath = Ini.Read<string>(Resources.ConfigSection, nameof(IconResourcePath), "%system%");
                return _iconResourcePath;
            }
            set
            {
                _iconResourcePath = value;
                WriteValue(Resources.ConfigSection, nameof(IconResourcePath), _iconResourcePath, "%system%");
            }
        }

        internal static string Language
        {
            get => _language ?? (_language = Ini.Read<string>(Resources.ConfigSection, nameof(Language), global::Language.SystemLang));
            set
            {
                _language = value;
                WriteValue(Resources.ConfigSection, nameof(Language), _language, global::Language.SystemLang);
            }
        }

        internal static string LastItem
        {
            get => _lastItem ?? (_lastItem = Ini.Read<string>(Resources.ConfigSection, nameof(LastItem)));
            set
            {
                _lastItem = value;
                WriteValueDirect(Resources.ConfigSection, nameof(LastItem), _lastItem);
            }
        }

        internal static string RegistryPath =>
            _registryPath ?? (_registryPath = Path.Combine("HKCU\\Software\\Portable Apps Suite", PathEx.LocalPath.GetHashCode().ToString(CultureInfo.InvariantCulture)));

        internal static bool StartMenuIntegration
        {
            get
            {
                if (_startMenuIntegration.HasValue)
                    return (bool)_startMenuIntegration;
                _startMenuIntegration = Ini.Read(Resources.ConfigSection, nameof(StartMenuIntegration), false);
                return (bool)_startMenuIntegration;
            }
            set
            {
                _startMenuIntegration = value;
                WriteValue(Resources.ConfigSection, nameof(StartMenuIntegration), _startMenuIntegration, false);
            }
        }

        internal static string SystemInstallId =>
            _systemInstallId ?? (_systemInstallId = (Win32_OperatingSystem.InstallDate?.ToString("F", CultureInfo.InvariantCulture) ?? EnvironmentEx.MachineId.ToString(CultureInfo.InvariantCulture)).Encrypt().Substring(24));

        internal static DateTime LastUpdateCheck
        {
            get => Ini.Read<DateTime>(Resources.ConfigSection, nameof(LastUpdateCheck));
            set => WriteValueDirect(Resources.ConfigSection, nameof(LastUpdateCheck), value);
        }

        internal static UpdateChannelOptions UpdateChannel
        {
            get
            {
                if (_updateChannel.HasValue)
                    return (UpdateChannelOptions)_updateChannel;
                _updateChannel = Ini.Read(Resources.ConfigSection, nameof(UpdateChannel), (int)UpdateChannelOptions.Release);
                return (UpdateChannelOptions)_updateChannel;
            }
            set
            {
                _updateChannel = (int)value;
                WriteValue(Resources.ConfigSection, nameof(UpdateChannel), (int)_updateChannel);
            }
        }

        internal static UpdateCheckOptions UpdateCheck
        {
            get
            {
                if (_updateCheck.HasValue)
                    return (UpdateCheckOptions)_updateCheck;
                _updateCheck = Ini.Read(Resources.ConfigSection, nameof(UpdateCheck), (int)UpdateCheckOptions.DailyFull);
                return (UpdateCheckOptions)_updateCheck;
            }
            set
            {
                _updateCheck = (int)value;
                WriteValue(Resources.ConfigSection, nameof(UpdateCheck), (int)_updateCheck, (int)UpdateCheckOptions.DailyFull);
            }
        }

        internal static string VersionValidation
        {
            get => Ini.Read(Resources.ConfigSection, nameof(VersionValidation));
            set => WriteValueDirect(Resources.ConfigSection, nameof(VersionValidation), value);
        }

        internal static bool SkipUpdateSearch { get; set; }

        internal static bool WriteToFileInQueue { get; set; }

        internal static void StartUpdateSearch()
        {
            if (SkipUpdateSearch)
                return;
            var i = (int)UpdateCheck;
            if (!i.IsBetween(1, 9))
                return;
            var lastCheck = LastUpdateCheck;
            if (lastCheck != default &&
                (i.IsBetween(1, 3) && (DateTime.Now - lastCheck).TotalHours < 1d ||
                 i.IsBetween(4, 6) && (DateTime.Now - lastCheck).TotalDays < 1d ||
                 i.IsBetween(7, 9) && (DateTime.Now - lastCheck).TotalDays < 30d))
                return;
            if (i != 2 && i != 5 && i != 8)
                ProcessEx.Start(CorePaths.AppsSuiteUpdater);
            if (i != 3 && i != 6 && i != 9)
                ProcessEx.Start(CorePaths.AppsDownloader, ActionGuid.UpdateInstance);
            LastUpdateCheck = DateTime.Now;
        }

        internal static string SearchItem(this List<string> items, string search)
        {
            try
            {
                string[] split = null;
                if (search.Contains("*") && !search.StartsWith("*", StringComparison.Ordinal) && !search.EndsWith("*", StringComparison.Ordinal))
                    split = search.Split('*');
                for (var i = 0; i < 2; i++)
                    foreach (var item in items)
                    {
                        bool match;
                        if (i < 1 && split != null && split.Length == 2)
                        {
                            var regex = new Regex($".*{split.First()}(.*){split.Second()}.*", RegexOptions.IgnoreCase);
                            match = regex.IsMatch(item);
                        }
                        else
                        {
                            match = item.StartsWithEx(search);
                            if (i > 0 && !match)
                                match = item.ContainsEx(search);
                        }
                        if (match)
                            return item;
                    }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return string.Empty;
        }

        internal static void Initialize()
        {
            WinApi.NativeHelper.SetProcessDPIAware();

            if (Log.DebugMode > 0)
            {
                Log.FileDir = Path.Combine(CorePaths.TempDir, "Logs");
                DirectoryEx.Create(Log.FileDir);
            }

            Ini.SetFile(PathEx.LocalDir, "Settings.ini");
            Ini.SortBySections = new[]
            {
                "Downloader",
                Resources.ConfigSection
            };

            Log.AllowLogging(Ini.FilePath);

            if (Elevation.IsAdministrator)
            {
                var path = Path.Combine("HKCU\\Software\\Portable Apps Suite", CorePaths.HomeDir.Encrypt(ChecksumAlgorithm.Adler32), ProcessEx.CurrentId.ToString(CultureInfo.InvariantCulture));
                if (Reg.CreateNewSubKey(path))
                    AppDomain.CurrentDomain.ProcessExit += (s, e) => Reg.RemoveSubKey(path);
            }

            if (!CacheData.CurrentAppInfo.Any())
            {
                using (var process = ProcessEx.Start(CorePaths.AppsDownloader, Elevation.IsAdministrator, false))
                    if (process?.HasExited == false)
                        process.WaitForExit();
                if (!CacheData.CurrentAppInfo.Any())
                {
                    Environment.ExitCode = 0;
                    Environment.Exit(Environment.ExitCode);
                }
            }

            CacheData.RemoveInvalidFiles();
            if (Recovery.AppsSuiteIsHealthy())
                return;
            Environment.ExitCode = 1;
            Environment.Exit(Environment.ExitCode);
        }

        internal static void WriteValue<TValue>(string section, string key, TValue value, TValue defValue = default, bool direct = false)
        {
            CacheData.UpdateSettingsMerges(section);
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
                if (direct)
                {
                    Ini.WriteDirect(section, key, null);
                    return;
                }
                WriteToFileInQueue = true;
                return;
            }
            Ini.Write(section, key, value);
            if (direct)
            {
                Ini.WriteDirect(section, key, value);
                return;
            }
            WriteToFileInQueue = true;
        }

        internal static void WriteValueDirect<TValue>(string section, string key, TValue value, TValue defValue = default) =>
            WriteValue(section, key, value, defValue, true);

        internal static void WriteToFile(bool forceMerge = false)
        {
            MergeSettings(forceMerge);
            if (!WriteToFileInQueue)
                return;
            Ini.WriteAll();
            WriteToFileInQueue = false;
        }

        private static string GetConfigKey(params string[] keys)
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

        private static void MergeSettings(bool force)
        {
            if (!force && (ProcessEx.InstancesCount(PathEx.LocalPath) > 1 || !File.Exists(CachePaths.SettingsMerges)))
                return;
            if (CacheData.SettingsMerges.Any())
            {
                var path = Path.GetTempFileName();
                if (FileEx.Copy(Ini.FilePath, path, true))
                {
                    foreach (var curSection in CacheData.SettingsMerges)
                    {
                        Ini.RemoveSection(curSection);
                        foreach (var curKey in Ini.GetKeys(curSection, path))
                        {
                            var curValue = Ini.Read(curSection, curKey, path);
                            Ini.Write(curSection, curKey, curValue);
                        }
                    }
                    Ini.Detach(path);
                    FileEx.Delete(path);
                    WriteToFileInQueue = true;
                }
            }
            FileEx.Delete(CachePaths.SettingsMerges);
        }

        private static int ValidateValue(int value, int minValue, int maxValue)
        {
            var current = Math.Max(value, minValue);
            return Math.Min(current, maxValue);
        }

        private static double ValidateValue(double value, double minValue, double maxValue)
        {
            var current = Math.Max(value, minValue);
            return Math.Min(current, maxValue);
        }

        internal static class Window
        {
            internal enum FadeInEffectOptions
            {
                Blend,
                Slide
            }

            private static WinApi.AnimateWindowFlags _animation;
            private static int? _backgroundImageLayout, _fadeInEffect;
            private static int[] _customColors;
            private static int _defaultPosition, _fadeInDuration;
            private static bool? _hideHScrollBar, _largeImages;
            private static double _opacity;

            internal static WinApi.AnimateWindowFlags Animation
            {
                get
                {
                    if (_animation != default)
                        return _animation;
                    if (FadeInEffect == FadeInEffectOptions.Blend)
                    {
                        _animation = WinApi.AnimateWindowFlags.Blend;
                        return _animation;
                    }
                    _animation = WinApi.AnimateWindowFlags.Slide;
                    if (DefaultPosition > 0)
                        _animation = WinApi.AnimateWindowFlags.Center;
                    else
                    {
                        var handle = Application.OpenForms.OfType<Form>().FirstOrDefault(x => x.Name?.EqualsEx(nameof(MenuViewForm)) == true)?.Handle ?? IntPtr.Zero;
                        switch (TaskBar.GetLocation(handle))
                        {
                            case TaskBarLocation.Left:
                                _animation |= WinApi.AnimateWindowFlags.HorPositive;
                                break;
                            case TaskBarLocation.Top:
                                _animation |= WinApi.AnimateWindowFlags.VerPositive;
                                break;
                            case TaskBarLocation.Right:
                                _animation |= WinApi.AnimateWindowFlags.HorNegative;
                                break;
                            case TaskBarLocation.Bottom:
                                _animation |= WinApi.AnimateWindowFlags.VerNegative;
                                break;
                            default:
                                _animation = WinApi.AnimateWindowFlags.Center;
                                break;
                        }
                    }
                    return _animation;
                }
            }

            internal static Image BackgroundImage =>
                CacheData.CurrentImageBg;

            internal static ImageLayout BackgroundImageLayout
            {
                get
                {
                    if (_backgroundImageLayout.HasValue)
                        return (ImageLayout)_backgroundImageLayout;
                    var key = GetConfigKey(nameof(Window), nameof(BackgroundImageLayout));
                    _backgroundImageLayout = Ini.Read(Resources.ConfigSection, key, 1);
                    return (ImageLayout)_backgroundImageLayout;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(BackgroundImageLayout));
                    _backgroundImageLayout = (int)value;
                    WriteValue(Resources.ConfigSection, key, _backgroundImageLayout, 1);
                }
            }

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

            internal static int DefaultPosition
            {
                get
                {
                    if (_defaultPosition != default)
                        return _defaultPosition;
                    var key = GetConfigKey(nameof(Window), nameof(DefaultPosition));
                    var value = Ini.Read(Resources.ConfigSection, key, default(int));
                    _defaultPosition = ValidateValue(value, 0, 1);
                    return _defaultPosition;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(DefaultPosition));
                    _defaultPosition = ValidateValue(value, 0, 1);
                    WriteValue(Resources.ConfigSection, key, _defaultPosition);
                }
            }

            internal static int FadeInDuration
            {
                get
                {
                    if (_fadeInDuration != default)
                        return _fadeInDuration;
                    var key = GetConfigKey(nameof(Window), nameof(FadeInDuration));
                    var value = Ini.Read(Resources.ConfigSection, key, 100);
                    _fadeInDuration = ValidateValue(value, 25, 750);
                    return _fadeInDuration;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(FadeInDuration));
                    _fadeInDuration = ValidateValue(value, 25, 750);
                    WriteValue(Resources.ConfigSection, key, _fadeInDuration, 100);
                }
            }

            internal static FadeInEffectOptions FadeInEffect
            {
                get
                {
                    if (_fadeInEffect.HasValue)
                        return (FadeInEffectOptions)_fadeInEffect;
                    var key = GetConfigKey(nameof(Window), nameof(FadeInEffect));
                    var value = Ini.Read(Resources.ConfigSection, key, default(int));
                    _fadeInEffect = ValidateValue(value, 0, 1);
                    return (FadeInEffectOptions)_fadeInEffect;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(FadeInEffect));
                    _fadeInEffect = ValidateValue((int)value, 0, 1);
                    WriteValue(Resources.ConfigSection, key, _fadeInEffect);
                }
            }

            internal static bool HideHScrollBar
            {
                get
                {
                    if (_hideHScrollBar.HasValue)
                        return (bool)_hideHScrollBar;
                    var key = GetConfigKey(nameof(Window), nameof(HideHScrollBar));
                    _hideHScrollBar = Ini.Read(Resources.ConfigSection, key, true);
                    return (bool)_hideHScrollBar;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(HideHScrollBar));
                    _hideHScrollBar = value;
                    WriteValue(Resources.ConfigSection, key, _hideHScrollBar, true);
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
                    if (_largeImages != value)
                        FileEx.TryDelete(CachePaths.CurrentImages);
                    _largeImages = value;
                    WriteValue(Resources.ConfigSection, key, _largeImages, false);
                }
            }

            internal static double Opacity
            {
                get
                {
                    if (_opacity > default(double))
                        return _opacity;
                    var key = GetConfigKey(nameof(Window), nameof(Opacity));
                    var value = Ini.Read(Resources.ConfigSection, key, .95d);
                    _opacity = ValidateValue(value, .2d, 1d);
                    return _opacity;
                }
                set
                {
                    var key = GetConfigKey(nameof(Window), nameof(Opacity));
                    _opacity = ValidateValue(value, .2d, 1d);
                    WriteValue(Resources.ConfigSection, key, _opacity, .95d);
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
                private const string DefWallpaperPath = "HKCU\\Software\\Microsoft\\Internet Explorer\\Desktop\\General";
                private const string WallpaperPath = "HKCU\\Control Panel\\Desktop";
                private static Color _system, _base, _baseText, _control, _controlText, _button, _buttonHover, _buttonText, _highlight, _highlightText;

                internal static Color System
                {
                    get
                    {
                        if (_system != default)
                            return _system;
                        _system = WinApi.NativeHelper.GetSystemThemeColor();
                        if (_system != Color.Black && _system != Color.White)
                            return _system;
                        var path = Reg.Read(WallpaperPath, "WallPaper", default(string));
                        if (!File.Exists(path))
                            path = Reg.Read(DefWallpaperPath, "WallpaperSource", default(string));
                        try
                        {
                            var image = Image.FromFile(path);
                            _system = image.GetAverageColor(true);
                        }
                        catch (Exception ex) when (ex.IsCaught())
                        {
                            Log.Write(ex);
                            _system = GetDefColor(nameof(Highlight));
                        }
                        return _system;
                    }
                }

                internal static Color Base
                {
                    get
                    {
                        if (_base == default)
                            _base = GetColor(nameof(Base));
                        return _base;
                    }
                    set
                    {
                        _base = value;
                        WriteValue(nameof(Base), _base);
                    }
                }

                internal static Color BaseDark =>
                    ControlPaint.Dark(Base, .25f);

                internal static Color BaseLight =>
                    ControlPaint.Light(Base, .25f);

                internal static Color BaseText
                {
                    get
                    {
                        if (_baseText != default)
                            return _baseText;
                        if (BackgroundImage != default(Image))
                        {
                            _baseText = BackgroundImage.GetAverageColor().InvertRgb().ToGrayScale();
                            return _baseText;
                        }
                        _baseText = BaseDark.InvertRgb().ToGrayScale();
                        return _baseText;
                    }
                }

                internal static Color Control
                {
                    get
                    {
                        if (_control == default)
                            _control = GetColor(nameof(Control));
                        return _control;
                    }
                    set
                    {
                        _control = value;
                        WriteValue(nameof(Control), _control);
                    }
                }

                internal static Color ControlText
                {
                    get
                    {
                        if (_controlText == default)
                            _controlText = GetColor(nameof(ControlText));
                        return _controlText;
                    }
                    set
                    {
                        _controlText = value;
                        WriteValue(nameof(ControlText), _controlText);
                    }
                }

                internal static Color Button
                {
                    get
                    {
                        if (_button == default)
                            _button = GetColor(nameof(Button));
                        return _button;
                    }
                    set
                    {
                        _button = value;
                        WriteValue(nameof(Button), _button);
                    }
                }

                internal static Color ButtonHover
                {
                    get
                    {
                        if (_buttonHover == default)
                            _buttonHover = GetColor(nameof(ButtonHover));
                        return _buttonHover;
                    }
                    set
                    {
                        _buttonHover = value;
                        WriteValue(nameof(ButtonHover), _buttonHover);
                    }
                }

                internal static Color ButtonText
                {
                    get
                    {
                        if (_buttonText == default)
                            _buttonText = GetColor(nameof(ButtonText));
                        return _buttonText;
                    }
                    set
                    {
                        _buttonText = value;
                        WriteValue(nameof(ButtonText), _buttonText);
                    }
                }

                internal static Color Highlight
                {
                    get
                    {
                        if (_highlight == default)
                            _highlight = GetColor(nameof(Highlight));
                        return _highlight;
                    }
                    set
                    {
                        _highlight = value;
                        WriteValue(nameof(Highlight), _highlight);
                    }
                }

                internal static Color HighlightText
                {
                    get
                    {
                        if (_highlightText == default)
                            _highlightText = GetColor(nameof(HighlightText));
                        return _highlightText;
                    }
                    set
                    {
                        _highlightText = value;
                        WriteValue(nameof(HighlightText), _highlightText);
                    }
                }

                private static Color GetDefColor(string key)
                {
                    switch (key)
                    {
                        case nameof(Base):
                            return System;
                        case nameof(Control):
                            return SystemColors.Window;
                        case nameof(ControlText):
                            return SystemColors.WindowText;
                        case nameof(Button):
                            return SystemColors.ButtonFace;
                        case nameof(ButtonHover):
                            return ProfessionalColors.ButtonSelectedHighlight;
                        case nameof(ButtonText):
                            return SystemColors.ControlText;
                        case nameof(Highlight):
                            return SystemColors.Highlight;
                        case nameof(HighlightText):
                            return SystemColors.HighlightText;
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
                internal const int MinimumWidth = 346, MinimumHeight = 320;
                private static DrawingSize _maximum, _minimum;
                private static int _width, _height;

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
                        var value = Ini.Read(Resources.ConfigSection, key, MinimumWidth);
                        _width = ValidateValue(value, MinimumWidth, MaximumWidth);
                        return _width;
                    }
                    set
                    {
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Width));
                        _width = ValidateValue(value, MinimumWidth, MaximumWidth);
                        WriteValue(Resources.ConfigSection, key, _width, MinimumWidth);
                    }
                }

                internal static int Height
                {
                    get
                    {
                        if (_height != default)
                            return _height;
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Height));
                        var value = Ini.Read(Resources.ConfigSection, key, MinimumHeight);
                        _height = ValidateValue(value, MinimumHeight, MaximumHeight);
                        return _width;
                    }
                    set
                    {
                        var key = GetConfigKey(nameof(Window), nameof(Size), nameof(Height));
                        _height = ValidateValue(value, MinimumHeight, MaximumHeight);
                        WriteValue(Resources.ConfigSection, key, _height, MinimumHeight);
                    }
                }

                internal static void Refresh()
                {
                    _minimum = default;
                    _maximum = default;
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
