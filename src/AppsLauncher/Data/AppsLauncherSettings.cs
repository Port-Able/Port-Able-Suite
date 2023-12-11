namespace AppsLauncher.Data
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Web.Script.Serialization;
    using System.Windows.Forms;
    using Windows;
    using PortAble;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;
    using static SilDev.WinApi;

    public enum StartPositionOption
    {
        CursorPosition,
        StartMenu
    }

    public enum ColorOption
    {
        Back,
        Control,
        ControlText,
        Highlight,
        HighlightText,
        SystemAccent,
        SystemWindow
    }

    public enum UpdateChannelOption
    {
        Release,
        Beta
    }

    public enum UpdateCheckOption
    {
        Never,
        HourlyFull,
        HourlyOnlyApps,
        HourlyOnlyAppsSuite,
        DailyFull,
        DailyOnlyApps,
        DailyOnlyAppsSuite,
        WeeklyFull,
        WeeklyOnlyApps,
        WeeklyOnlyAppsSuite,
        MonthlyFull,
        MonthlyOnlyApps,
        MonthlyOnlyAppsSuite
    }

    /// <summary>
    ///     Provides local app settings.
    /// </summary>
    [Serializable]
    public sealed class AppsLauncherSettings : IDisposable, ISerializable, IEquatable<AppsLauncherSettings>
    {
        [NonSerialized]
        private Image _desktopImage;

        [NonSerialized]
        private string _filePath;

        /// <summary>
        ///     Determines whether the main window has a caption bar.
        /// </summary>
        public bool ShowCaption { get; set; }

        public bool ShowInTaskbar { get; set; }

        /// <summary>
        ///     Determines whether the apps images are large.
        /// </summary>
        public bool ShowLargeImages { get; set; }

        public bool ShowHorScrollBar { get; set; }

        public double OpacityLevel { get; set; }

        public int BlurStrength { get; set; }

        public StartPositionOption StartPosition { get; set; }

        /// <summary>
        ///     The size of the <see cref="MenuViewForm"/> window.
        /// </summary>
        public Size WindowSize { get; set; }

        public Image WindowBackground { get; set; }

        public ImageLayout WindowBackgroundLayout { get; set; }

        public bool WindowColorsForceDefault { get; set; }

        /// <summary>
        ///     A list of colors used for the <see cref="MenuViewForm"/> window.
        /// </summary>
        public Dictionary<ColorOption, Color> WindowColors { get; set; }

        /// <summary>
        ///     The custom colors used for the color dialog.
        /// </summary>
        public List<Color> CustomColors { get; set; }

        public string LastApp { get; set; }

        public string LastIconResourcePath { get; set; }

        public bool SystemIntegration { get; set; }

        public bool SystemStartMenuIntegration { get; set; }

        public Version CurrentAssemblyVersion { get; set; }

        public UpdateCheckOption UpdateCheck { get; set; }

        public UpdateChannelOption UpdateChannel { get; set; }

        public DateTime LastUpdateCheck { get; set; }

        /// <summary>
        ///     The path on disk where the instance will be saved and loaded.
        /// </summary>
        [ScriptIgnore]
        public string FilePath
        {
            get
            {
                if (_filePath != null)
                    return _filePath;
                var dir = PathEx.Combine(CorePaths.DataDir, "Settings");
                return _filePath = Path.Combine(dir, $"{nameof(AppsLauncher)}.json");
            }
        }

        /// <summary>
        ///     The exact time when this instance was created, updated via the
        ///     <see cref="LoadFromFile"/> method, or when the <see cref="SaveToFile"/>
        ///     method was called. So this <see cref="DateTime"/> is used to synchronize
        ///     with the file on the hard drive.
        /// </summary>
        [ScriptIgnore]
        public DateTime InstanceTime { get; private set; }

        /// <summary>
        ///     Initialize the <see cref="AppsLauncherSettings"/> class.
        /// </summary>
        public AppsLauncherSettings()
        {
            SetDefaults();
            LoadFromFile(true);
        }

        private AppsLauncherSettings(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            // Set default values in case some properties have
            // been deleted from the configuration file.
            SetDefaults();

            // Only set properties that are also present in
            // the configuration file.
            foreach (var ent in info)
                switch (ent.Name)
                {
                    case nameof(ShowCaption):
                        ShowCaption = info.GetBoolean(nameof(ShowCaption));
                        break;
                    case nameof(ShowInTaskbar):
                        ShowInTaskbar = info.GetBoolean(nameof(ShowInTaskbar));
                        break;
                    case nameof(ShowLargeImages):
                        ShowLargeImages = info.GetBoolean(nameof(ShowLargeImages));
                        break;
                    case nameof(ShowHorScrollBar):
                        ShowHorScrollBar = info.GetBoolean(nameof(ShowHorScrollBar));
                        break;
                    case nameof(OpacityLevel):
                        OpacityLevel = info.GetDouble(nameof(OpacityLevel));
                        break;
                    case nameof(BlurStrength):
                        BlurStrength = info.GetInt32(nameof(BlurStrength));
                        break;
                    case nameof(StartPosition):
                        StartPosition = (StartPositionOption)info.GetValue(nameof(StartPosition), typeof(StartPositionOption));
                        break;
                    case nameof(WindowSize):
                        WindowSize = (Size)info.GetValue(nameof(WindowSize), typeof(Size));
                        break;
                    case nameof(WindowBackground):
                        WindowBackground = (Image)info.GetValue(nameof(WindowBackground), typeof(Image));
                        break;
                    case nameof(WindowBackgroundLayout):
                        WindowBackgroundLayout = (ImageLayout)info.GetValue(nameof(WindowBackgroundLayout), typeof(ImageLayout));
                        break;
                    case nameof(WindowColorsForceDefault):
                        WindowColorsForceDefault = info.GetBoolean(nameof(WindowColorsForceDefault));
                        break;
                    case nameof(WindowColors):
                        WindowColors = (Dictionary<ColorOption, Color>)info.GetValue(nameof(WindowColors), typeof(Dictionary<ColorOption, Color>));
                        break;
                    case nameof(CustomColors):
                        CustomColors = (List<Color>)info.GetValue(nameof(CustomColors), typeof(List<Color>));
                        break;
                    case nameof(LastApp):
                        LastApp = info.GetString(nameof(LastApp));
                        break;
                    case nameof(LastIconResourcePath):
                        LastIconResourcePath = info.GetString(nameof(LastIconResourcePath));
                        break;
                    case nameof(SystemIntegration):
                        SystemIntegration = info.GetBoolean(nameof(SystemIntegration));
                        break;
                    case nameof(SystemStartMenuIntegration):
                        SystemStartMenuIntegration = info.GetBoolean(nameof(SystemStartMenuIntegration));
                        break;
                    case nameof(CurrentAssemblyVersion):
                        CurrentAssemblyVersion = (Version)info.GetValue(nameof(CurrentAssemblyVersion), typeof(Version));
                        break;
                    case nameof(UpdateCheck):
                        UpdateCheck = (UpdateCheckOption)info.GetValue(nameof(UpdateCheck), typeof(UpdateCheckOption));
                        break;
                    case nameof(UpdateChannel):
                        UpdateChannel = (UpdateChannelOption)info.GetValue(nameof(UpdateChannel), typeof(UpdateChannelOption));
                        break;
                    case nameof(LastUpdateCheck):
                        LastUpdateCheck = info.GetDateTime(nameof(LastUpdateCheck));
                        break;
                }

            // Finally, set the instance creation time.
            InstanceTime = DateTime.Now;
        }

        /// <summary>
        ///     Allows an object to try to free resources and perform other cleanup
        ///     operations before it is reclaimed by garbage collection.
        /// </summary>
        ~AppsLauncherSettings() =>
            Dispose(false);

        /// <summary>
        ///     Sets the <see cref="SerializationInfo"/> object for this instance.
        /// </summary>
        /// <param name="info">
        ///     The object that holds the serialized object data.
        /// </param>
        /// <param name="context">
        ///     The contextual information about the source or destination.
        /// </param>
        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            foreach (var pi in GetType().GetProperties().Where(pi => !IsDefault(pi)))
                info.AddValue(pi.Name, pi.GetValue(this));

            /*
            info.AddValue(nameof(ShowCaption), ShowCaption);
            info.AddValue(nameof(ShowInTaskbar), ShowInTaskbar);
            info.AddValue(nameof(ShowLargeImages), ShowLargeImages);
            info.AddValue(nameof(ShowHorScrollBar), ShowHorScrollBar);
            info.AddValue(nameof(OpacityLevel), OpacityLevel);
            info.AddValue(nameof(BlurStrength), BlurStrength);
            info.AddValue(nameof(StartPosition), StartPosition);
            info.AddValue(nameof(WindowSize), WindowSize);
            info.AddValue(nameof(WindowBackground), WindowBackground);
            info.AddValue(nameof(WindowBackgroundLayout), WindowBackgroundLayout);
            info.AddValue(nameof(WindowColorsForceDefault), WindowColorsForceDefault);
            if (!WindowColorsForceDefault)
            {
                info.AddValue(nameof(WindowColors), WindowColors);
                info.AddValue(nameof(CustomColors), CustomColors);
            }
            info.AddValue(nameof(LastApp), LastApp);
            info.AddValue(nameof(LastIconResourcePath), LastIconResourcePath);
            info.AddValue(nameof(SystemIntegration), SystemIntegration);
            info.AddValue(nameof(SystemStartMenuIntegration), SystemStartMenuIntegration);
            info.AddValue(nameof(UpdateCheck), UpdateCheck);
            info.AddValue(nameof(UpdateChannel), UpdateChannel);
            info.AddValue(nameof(LastUpdateCheck), LastUpdateCheck);
            info.AddValue(nameof(CurrentAssemblyVersion), CurrentAssemblyVersion);
            */
        }

        /// <summary>
        ///     Determines whether this instance have same values as the specified
        ///     <see cref="AppsLauncherSettings"/> instance.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="AppsLauncherSettings"/> instance to compare.
        /// </param>
        public bool Equals(AppsLauncherSettings other) =>
            other != null &&
            ShowCaption == other.ShowCaption &&
            ShowInTaskbar == other.ShowInTaskbar &&
            ShowLargeImages == other.ShowLargeImages &&
            ShowHorScrollBar == other.ShowHorScrollBar &&
            Math.Abs(OpacityLevel - other.OpacityLevel) < 0d &&
            BlurStrength == other.BlurStrength &&
            StartPosition == other.StartPosition &&
            WindowSize == other.WindowSize &&
            WindowBackground == other.WindowBackground &&
            WindowBackgroundLayout == other.WindowBackgroundLayout &&
            LastApp == other.LastApp &&
            LastIconResourcePath == other.LastIconResourcePath &&
            SystemIntegration == other.SystemIntegration &&
            SystemStartMenuIntegration == other.SystemStartMenuIntegration &&
            CurrentAssemblyVersion == other.CurrentAssemblyVersion &&
            UpdateCheck == other.UpdateCheck &&
            UpdateChannel == other.UpdateChannel &&
            LastUpdateCheck == other.LastUpdateCheck &&
            (WindowColors?.SequenceEqual(other.WindowColors) ?? other.WindowColors == default) &&
            (CustomColors?.SequenceEqual(other.CustomColors) ?? other.CustomColors == default);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is AppsLauncherSettings other)
                return Equals(other);
            return false;
        }

        /// <inheritdoc cref="Type.GetHashCode()"/>
        public override int GetHashCode() =>
            Tuple.Create(GetType().FullName, nameof(AppsLauncherSettings)).GetHashCode();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Converts the values of this instance to a string.
        /// </summary>
        /// <returns>
        ///     A string representation of this instance.
        /// </returns>
        public override string ToString() =>
            Json.Serialize(this);

        /// <summary>
        ///     Reset all properties to their default values.
        /// </summary>
        public void SetDefaults()
        {
            ShowCaption = false;
            ShowInTaskbar = false;
            ShowLargeImages = false;
            ShowHorScrollBar = false;
            OpacityLevel = .85d;
            BlurStrength = 90;
            StartPosition = StartPositionOption.CursorPosition;
            WindowSize = new(252, 342);
            WindowBackground = null;
            WindowBackgroundLayout = ImageLayout.Tile;
            WindowColorsForceDefault = true;
            LastApp = string.Empty;
            LastIconResourcePath = "%system%";
            SystemIntegration = false;
            SystemStartMenuIntegration = false;
            CurrentAssemblyVersion = AssemblyInfo.Version.ToVersion();
            UpdateCheck = UpdateCheckOption.DailyFull;
            UpdateChannel = UpdateChannelOption.Release;
            LastUpdateCheck = DateTime.MinValue;
            SetDefaultColors();
        }

        /// <summary>
        ///     Reset <see cref="WindowColors"/> and <see cref="CustomColors"/> to default
        ///     values.
        /// </summary>
        public void SetDefaultColors()
        {
            var accentColor = NativeHelper.GetSystemThemeColor();
            var backColor = Color.FromArgb(0xf3, 0xf3, 0xf3);
            var foreColor = Color.FromArgb(0x20, 0x20, 0x20);
            var systemColor = Desktop.AppsUseDarkTheme switch
            {
                true => foreColor,
                _ => backColor,
            };
            if (systemColor == foreColor)
            {
                foreColor = backColor;
                backColor = systemColor;
            }
            WindowColors = new()
            {
                { ColorOption.Back, Desktop.AccentColorOnTitlebar ? accentColor : systemColor },
                { ColorOption.Control, backColor },
                { ColorOption.ControlText, foreColor },
                { ColorOption.Highlight, SystemColors.Highlight },
                { ColorOption.HighlightText, SystemColors.HighlightText },
                { ColorOption.SystemAccent, accentColor },
                { ColorOption.SystemWindow, systemColor }
            };
            CustomColors = WindowColors.Values.ToList();
        }

        public Image GetDesktopBehind(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));
            _desktopImage?.Dispose();
            _desktopImage = null;
            if (BlurStrength.IsBetween(1, 99) && OpacityLevel.IsBetween(.2d, .99d))
                _desktopImage ??= form.CaptureDesktopBehindWindow().Blur(BlurStrength).SetAlpha(Convert.ToSingle(1f - OpacityLevel));
            return _desktopImage;
        }

        /// <summary>
        ///     Load the data from a JSON file stored in the
        ///     <see cref="CorePaths.DataDir"/> folder if newer than this instance.
        /// </summary>
        /// <param name="force">
        ///     Determines whether to force an update from the file.
        /// </param>
        public void LoadFromFile(bool force = false)
        {
            if (!FileEx.Exists(FilePath))
            {
                InstanceTime = DateTime.MinValue;
                return;
            }
            if (!force && File.GetLastWriteTime(FilePath) < InstanceTime)
                return;
            using var item = CacheData.LoadJson<AppsLauncherSettings>(FilePath);
            if (item == default)
                return;
            ShowCaption = item.ShowCaption;
            ShowInTaskbar = item.ShowInTaskbar;
            ShowLargeImages = item.ShowLargeImages;
            ShowHorScrollBar = item.ShowHorScrollBar;
            OpacityLevel = item.OpacityLevel;
            BlurStrength = item.BlurStrength;
            StartPosition = item.StartPosition;
            WindowSize = item.WindowSize;
            WindowBackground = item.WindowBackground;
            WindowBackgroundLayout = item.WindowBackgroundLayout;
            WindowColorsForceDefault = item.WindowColorsForceDefault;
            LastApp = item.LastApp;
            LastIconResourcePath = item.LastIconResourcePath;
            SystemIntegration = item.SystemIntegration;
            SystemStartMenuIntegration = item.SystemStartMenuIntegration;
            CurrentAssemblyVersion = item.CurrentAssemblyVersion;
            UpdateCheck = item.UpdateCheck;
            UpdateChannel = item.UpdateChannel;
            LastUpdateCheck = item.LastUpdateCheck;
            if (!WindowColorsForceDefault)
            {
                if (item.WindowColors.Count == 7)
                    WindowColors = item.WindowColors;
                if (item.WindowColors.Any())
                    CustomColors = item.CustomColors;
            }
            InstanceTime = DateTime.Now;
        }

        /// <summary>
        ///     Saves the data into a JSON file stored in the
        ///     <see cref="CorePaths.DataDir"/> directory if newer than the file.
        /// </summary>
        /// <param name="force">
        ///     Determines whether to force an update to the file.
        /// </param>
        public void SaveToFile(bool force = false)
        {
            if (!force)
            {
                var item = CacheData.LoadJson<AppsLauncherSettings>(FilePath);
                if (this == item)
                    return;
            }
            CacheData.SaveJson(this, FilePath);
            InstanceTime = DateTime.Now;
        }

        /// <summary>
        ///     Releases all resources used by this <see cref="AppsLauncherSettings"/>.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true"/> to release both managed and unmanaged resources;
        ///     otherwise, <see langword="false"/> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
                _desktopImage?.Dispose();
            WindowBackground?.Dispose();
        }

        private bool IsDefault(PropertyInfo pi) =>
            pi?.Name switch
            {
                nameof(ShowCaption) => !ShowCaption,
                nameof(ShowInTaskbar) => !ShowInTaskbar,
                nameof(ShowLargeImages) => !ShowLargeImages,
                nameof(ShowHorScrollBar) => !ShowHorScrollBar,
                nameof(OpacityLevel) => Math.Abs(OpacityLevel - .85d) == 0,
                nameof(BlurStrength) => BlurStrength == 90,
                nameof(StartPosition) => StartPosition == StartPositionOption.CursorPosition,
                nameof(WindowSize) => WindowSize is { Width: 252, Height: 342 },
                nameof(WindowBackground) => WindowBackground == null,
                nameof(WindowBackgroundLayout) => WindowBackgroundLayout == ImageLayout.Tile,
                nameof(WindowColorsForceDefault) => WindowColorsForceDefault,
                nameof(LastApp) => string.IsNullOrWhiteSpace(LastApp),
                nameof(LastIconResourcePath) => LastIconResourcePath == "%system%",
                nameof(SystemIntegration) => !SystemIntegration,
                nameof(SystemStartMenuIntegration) => !SystemStartMenuIntegration,
                nameof(UpdateCheck) => UpdateCheck == UpdateCheckOption.DailyFull,
                nameof(UpdateChannel) => UpdateChannel == UpdateChannelOption.Release,
                nameof(LastUpdateCheck) => LastUpdateCheck == DateTime.MinValue,
                nameof(CurrentAssemblyVersion) => false,
                _ => true
            };

        /// <summary>
        ///     Determines whether two specified <see cref="AppsLauncherSettings"/>
        ///     instances have same values.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="AppsLauncherSettings"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="AppsLauncherSettings"/> instance to compare.
        /// </param>
        public static bool operator ==(AppsLauncherSettings left, AppsLauncherSettings right) =>
            Equals(left, right);

        /// <summary>
        ///     Determines whether two specified <see cref="AppsLauncherSettings"/>
        ///     instances have different values.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="AppsLauncherSettings"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="AppsLauncherSettings"/> instance to compare.
        /// </param>
        public static bool operator !=(AppsLauncherSettings left, AppsLauncherSettings right) =>
            !Equals(left, right);
    }
}
