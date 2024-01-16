namespace AppsLauncher.Data;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Forms;
using Newtonsoft.Json;
using PortAble;
using PortAble.Model;
using SilDev;
using SilDev.Drawing;
using SilDev.Forms;
using static SilDev.WinApi;

/// <summary>
///     Specifies the start position of the <see cref="MenuViewForm"/> window.
/// </summary>
public enum StartPositionOption
{
    /// <summary>
    ///     The window starts at the position of the mouse pointer.
    /// </summary>
    CursorPosition,

    /// <summary>
    ///     The window starts in a location similar to that of the operating system's
    ///     Start Menu.
    /// </summary>
    StartMenu
}

/// <summary>
///     Specifies the color options for the <see cref="MenuViewForm"/> window.
/// </summary>
public enum ColorOption
{
    /// <summary>
    ///     The background color.
    /// </summary>
    Back,

    /// <summary>
    ///     The control color.
    /// </summary>
    Control,

    /// <summary>
    ///     The control text color.
    /// </summary>
    ControlText,

    /// <summary>
    ///     The button face color.
    /// </summary>
    ButtonFace,

    /// <summary>
    ///     The button highlight color.
    /// </summary>
    ButtonHighlight,

    /// <summary>
    ///     The highlight color.
    /// </summary>
    Highlight,

    /// <summary>
    ///     The highlight text color.
    /// </summary>
    HighlightText,

    /// <summary>
    ///     The button symbol color.
    /// </summary>
    ButtonSymbol,

    /// <summary>
    ///     The system accent color.
    /// </summary>
    SystemAccent,

    /// <summary>
    ///     The system window color.
    /// </summary>
    SystemWindow
}

/// <summary>
///     Specifies the update channel of the apps suite.
/// </summary>
public enum UpdateChannelOption
{
    /// <summary>
    ///     The release channel.
    /// </summary>
    Release,

    /// <summary>
    ///     The beta channel.
    /// </summary>
    Beta
}

/// <summary>
///     Specifies when and how to search for updates.
/// </summary>
public enum UpdateCheckOption
{
    /// <summary>
    ///     Never search for updates.
    /// </summary>
    Never,

    /// <summary>
    ///     Check for apps suite and app updates hourly.
    /// </summary>
    HourlyFull,

    /// <summary>
    ///     Just check for app updates hourly.
    /// </summary>
    HourlyOnlyApps,

    /// <summary>
    ///     Just check for apps suite updates hourly.
    /// </summary>
    HourlyOnlyAppsSuite,

    /// <summary>
    ///     Check for apps suite and app updates daily.
    /// </summary>
    DailyFull,

    /// <summary>
    ///     Just check for app updates daily.
    /// </summary>
    DailyOnlyApps,

    /// <summary>
    ///     Just check for apps suite updates daily.
    /// </summary>
    DailyOnlyAppsSuite,

    /// <summary>
    ///     Check for apps suite and app updates weekly.
    /// </summary>
    WeeklyFull,

    /// <summary>
    ///     Just check for app updates weekly.
    /// </summary>
    WeeklyOnlyApps,

    /// <summary>
    ///     Just check for apps suite updates weekly.
    /// </summary>
    WeeklyOnlyAppsSuite,

    /// <summary>
    ///     Check for apps suite and app updates monthly.
    /// </summary>
    MonthlyFull,

    /// <summary>
    ///     Just check for app updates monthly.
    /// </summary>
    MonthlyOnlyApps,

    /// <summary>
    ///     Just check for apps suite updates monthly.
    /// </summary>
    MonthlyOnlyAppsSuite
}

/// <summary>
///     Provides Apps Launcher settings.
/// </summary>
[Serializable]
public sealed class AppsLauncherJson : AJsonFile<AppsLauncherJson>, IDisposable
{
    [NonSerialized]
    private Image _desktopImage;

    /// <summary>
    ///     Determines whether the <see cref="MenuViewForm"/> window has a caption bar.
    /// </summary>
    public bool ShowCaption { get; set; }

    /// <summary>
    ///     Determines whether the <see cref="MenuViewForm"/> window is shown in the
    ///     taskbar.
    /// </summary>
    public bool ShowInTaskbar { get; set; }

    /// <summary>
    ///     The view style of the <see cref="MenuViewForm"/> window`s apps list.
    /// </summary>
    public View ViewStyle { get; set; }

    /// <summary>
    ///     The opacity level of the <see cref="MenuViewForm"/> window. Must be between
    ///     0.2 and 1, where 1 represents 100% visibility.
    /// </summary>
    public double OpacityLevel { get; set; }

    /// <summary>
    ///     The blur strength of the <see cref="MenuViewForm"/> window. Must be between
    ///     0 and 99, where 0 disables the blur effect.
    /// </summary>
    public int BlurStrength { get; set; }

    /// <summary>
    ///     The start position of the <see cref="MenuViewForm"/> window.
    /// </summary>
    public StartPositionOption StartPosition { get; set; }

    /// <summary>
    ///     The size of the <see cref="MenuViewForm"/> window.
    /// </summary>
    public Size WindowSize { get; set; }

    /// <summary>
    ///     The background image of the <see cref="MenuViewForm"/> window.
    /// </summary>
    public Image WindowBackground { get; set; }

    /// <summary>
    ///     The layout of the <see cref="MenuViewForm"/> window`s background image.
    /// </summary>
    public ImageLayout WindowBackgroundLayout { get; set; }

    /// <summary>
    ///     Determines whether to use the default color style for the
    ///     <see cref="MenuViewForm"/> window.
    /// </summary>
    public bool WindowColorsForceDefault { get; set; }

    /// <summary>
    ///     A list of colors used for the <see cref="MenuViewForm"/> window.
    /// </summary>
    public Dictionary<ColorOption, Color> WindowColors { get; set; }

    /// <summary>
    ///     The saved colors of the color dialog window.
    /// </summary>
    public List<Color> CustomColors { get; set; }

    /// <summary>
    ///     A list of app install locations.
    /// </summary>
    public List<string> AppInstallLocations { get; set; }

    /// <summary>
    ///     The most recently launched app, which is used for the
    ///     <see cref="OpenWithForm"/> window when no other app matches the open items.
    /// </summary>
    public string LastApp { get; set; }

    /// <summary>
    ///     The saved icon resource path of the <see cref="IconBrowserDialog"/> window.
    /// </summary>
    public string LastIconResourcePath { get; set; }

    /// <summary>
    ///     Determines whether integration into the system is activated, where entries
    ///     of the Apps Suite are created in various context menus of the operating
    ///     system.
    /// </summary>
    public bool SystemIntegration { get; set; }

    /// <summary>
    ///     Determines whether Start Menu integration is enabled, which creates
    ///     shortcuts to all portable apps in the operating system`s Start menu.
    /// </summary>
    public bool SystemStartMenuIntegration { get; set; }

    /// <summary>
    ///     The current version of this assembly, used to determine the version of the
    ///     saved config file.
    /// </summary>
    public Version CurrentAssemblyVersion { get; set; }

    /// <summary>
    ///     Determines when and how to check for updates.
    /// </summary>
    public UpdateCheckOption UpdateCheck { get; set; }

    /// <summary>
    ///     Determines whether to check for updates in the beta channel.
    /// </summary>
    public UpdateChannelOption UpdateChannel { get; set; }

    public DateTime LastUpdateCheck { get; set; }

    /// <summary>
    ///     Initialize the <see cref="AppsLauncherJson"/> class.
    /// </summary>
    public AppsLauncherJson()
    {
        SetDefaultAll();
        LoadFromFile(true);
    }

    private AppsLauncherJson(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        // Set default values in case some properties have
        // been deleted from the configuration file.
        SetDefaultAll();

        // Only set properties that are also present in
        // the configuration file.
        var enumerator = info.GetEnumerator();
        while (enumerator.MoveNext())
            switch (enumerator.Name)
            {
                case nameof(ShowCaption):
                    ShowCaption = info.GetBoolean(nameof(ShowCaption));
                    break;
                case nameof(ShowInTaskbar):
                    ShowInTaskbar = info.GetBoolean(nameof(ShowInTaskbar));
                    break;
                case nameof(ViewStyle):
                    ViewStyle = (View)info.GetValue(nameof(ViewStyle), typeof(View));
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
                case nameof(AppInstallLocations):
                    AppInstallLocations = (List<string>)info.GetValue(nameof(AppInstallLocations), typeof(List<string>));
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
    ~AppsLauncherJson() =>
        Dispose(false);

    /// <summary>
    ///     Determines whether this instance have same values as the specified
    ///     <see cref="AppsLauncherJson"/> instance.
    ///     <para>
    ///         &#9888; Not consistent with
    ///         <see cref="AJsonFile{TSettings}.GetHashCode"/>, which is intentional.
    ///     </para>
    /// </summary>
    /// <param name="other">
    ///     The <see cref="AppsLauncherJson"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the values of the specified
    ///     <see cref="AppsLauncherJson"/> instance are equal to the values of the
    ///     current <see cref="AppsLauncherJson"/> instance; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    public override bool Equals(AppsLauncherJson other) =>
        other != null &&
        ShowCaption == other.ShowCaption &&
        ShowInTaskbar == other.ShowInTaskbar &&
        ViewStyle == other.ViewStyle &&
        Math.Abs(OpacityLevel - other.OpacityLevel) < .01d &&
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
        WindowColors.SequenceEqualEx(other.WindowColors) &&
        CustomColors.SequenceEqualEx(other.CustomColors) &&
        AppInstallLocations.SequenceEqualEx(other.AppInstallLocations);

    /// <inheritdoc cref="Equals(AppsLauncherJson)"/>
    public override bool Equals(object other) =>
        other is AppsLauncherJson instance && Equals(instance);

    /// <inheritdoc cref="Type.GetHashCode()"/>
    public override int GetHashCode() =>
        (typeof(AppsLauncherJson).FullName, nameof(AppsLauncherJson)).GetHashCode();

    /// <inheritdoc cref="Dispose(bool)"/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="AJsonFile{TSettings}.GetDefault(string)"/>
    public override object GetDefault(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(ShowCaption): return false;
            case nameof(ShowInTaskbar): return false;
            case nameof(ViewStyle): return View.Tile;
            case nameof(OpacityLevel): return .85d;
            case nameof(BlurStrength): return 90;
            case nameof(StartPosition): return StartPositionOption.CursorPosition;
            case nameof(WindowSize): return new Size(252, 342);
            case nameof(WindowBackgroundLayout): return ImageLayout.Tile;
            case nameof(WindowColorsForceDefault): return true;
            case nameof(WindowColors):
                var accentColor = NativeHelper.GetSystemThemeColor();
                var backColor = Color.FromArgb(0xf3, 0xf3, 0xf3);
                var foreColor = Color.FromArgb(0x20, 0x20, 0x20);
                var systemColor = Desktop.AppsUseDarkTheme ? foreColor : backColor;
                if (systemColor == foreColor)
                {
                    foreColor = backColor;
                    backColor = systemColor;
                }
                var symbolColor = Desktop.AccentColorOnTitlebar ? systemColor : accentColor;
                if (!Desktop.AccentColorOnTitlebar)
                    symbolColor = backColor.IsDark()
                        ? symbolColor.EnsureLight()
                        : backColor.IsLight()
                            ? symbolColor.EnsureDark()
                            : backColor;
                return new Dictionary<ColorOption, Color>
                {
                    { ColorOption.Back, Desktop.AccentColorOnTitlebar ? accentColor : systemColor },
                    { ColorOption.Control, backColor },
                    { ColorOption.ControlText, foreColor },
                    { ColorOption.ButtonFace, SystemColors.ButtonFace },
                    { ColorOption.ButtonHighlight, SystemColors.ButtonHighlight },
                    { ColorOption.Highlight, SystemColors.Highlight },
                    { ColorOption.HighlightText, SystemColors.HighlightText },
                    { ColorOption.ButtonSymbol, symbolColor },
                    { ColorOption.SystemAccent, accentColor },
                    { ColorOption.SystemWindow, systemColor }
                };
            case nameof(CustomColors): return WindowColors.Values.ToList();
            case nameof(AppInstallLocations): return new List<string>();
            case nameof(LastApp): return string.Empty;
            case nameof(LastIconResourcePath): return "%system%";
            case nameof(SystemIntegration): return false;
            case nameof(SystemStartMenuIntegration): return false;
            case nameof(UpdateCheck): return UpdateCheckOption.DailyFull;
            case nameof(UpdateChannel): return UpdateChannelOption.Release;
            case nameof(LastUpdateCheck): return DateTime.MinValue;
            case nameof(CurrentAssemblyVersion): return AssemblyInfo.Version.ToVersion();
            case nameof(FilePath): return FilePath;
            case nameof(InstanceTime): return DateTime.Now;
            default: return null;
        }
    }

    /// <summary>
    ///     Reset <see cref="WindowColors"/> and <see cref="CustomColors"/> to default
    ///     values.
    /// </summary>
    public void ResetColors()
    {
        SetDefault(nameof(WindowColors));
        SetDefault(nameof(CustomColors));
    }

    /// <summary>
    ///     Gets a captured <see cref="Image"/> of the entire desktop under the
    ///     specified window if <see cref="BlurStrength"/> and
    ///     <see cref="OpacityLevel"/> are both within the recommended range.
    ///     <para>
    ///         &#9762; Please note that the form window must be hidden for the moment
    ///         of capture, which may cause flickering if this method is called on an
    ///         already visible window.
    ///     </para>
    /// </summary>
    /// <param name="form">
    ///     The form window behind which is the desktop that should be captured.
    /// </param>
    /// <returns>
    ///     If <see cref="BlurStrength"/> is between 1 and 99, just as
    ///     <see cref="OpacityLevel"/> is between 0.2 and 0.99, an <see cref="Image"/>
    ///     is created that represents the entire desktop behind the specified
    ///     <see cref="Form"/> window; otherwise <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     form is null.
    /// </exception>
    public Image GetDesktopBehind(Form form)
    {
        if (form == null)
            throw new ArgumentNullException(nameof(form));
        _desktopImage?.Dispose();
        _desktopImage = null;
        if (BlurStrength.IsBetween(1, 99) && OpacityLevel.IsBetween(.2d, .99d))
            _desktopImage ??= form.CaptureDesktopBehindWindow().Blur(BlurStrength).SetAlpha(Convert.ToSingle(1d - OpacityLevel));
        return _desktopImage;
    }

    /// <summary>
    ///     Load the data from a JSON file stored in the
    ///     <see cref="CorePaths.DataDir"/> folder if newer than this instance.
    /// </summary>
    /// <param name="force">
    ///     Determines whether to force an update from the file.
    /// </param>
    public override void LoadFromFile(bool force = false)
    {
        if (!FileEx.Exists(FilePath))
        {
            InstanceTime = DateTime.MinValue;
            return;
        }
        if (!force && File.GetLastWriteTime(FilePath) < InstanceTime)
            return;
        using var item = CacheData.LoadJson<AppsLauncherJson>(FilePath);
        if (item == default)
            return;
        foreach (var pi in typeof(AppsLauncherJson).GetPropertiesEx(typeof(JsonIgnoreAttribute)))
            switch (pi.Name)
            {
                case nameof(CurrentAssemblyVersion):
                    continue;
                case nameof(WindowColors):
                {
                    foreach (var color in item.WindowColors.Where(c => WindowColors.ContainsKey(c.Key)))
                        WindowColors[color.Key] = color.Value;
                    CustomColors = item.CustomColors;
                    continue;
                }
                default:
                    pi.SetValue(this, pi.GetValue(item));
                    break;
            }
        InstanceTime = DateTime.Now;
    }

    protected override bool IsDefault(PropertyInfo pi) =>
        pi?.Name switch
        {
            nameof(ShowCaption) => ShowCaption == GetDefault<bool>(nameof(ShowCaption)),
            nameof(ShowInTaskbar) => ShowInTaskbar == GetDefault<bool>(nameof(ShowInTaskbar)),
            nameof(ViewStyle) => ViewStyle == GetDefault<View>(nameof(ViewStyle)),
            nameof(OpacityLevel) => Math.Abs(OpacityLevel - GetDefault<double>(nameof(OpacityLevel))) < .01d,
            nameof(BlurStrength) => BlurStrength == GetDefault<int>(nameof(BlurStrength)),
            nameof(StartPosition) => StartPosition == GetDefault<StartPositionOption>(nameof(StartPosition)),
            nameof(WindowSize) => WindowSize == GetDefault<Size>(nameof(WindowSize)),
            nameof(WindowBackground) => WindowBackground == GetDefault<Image>(nameof(WindowBackground)),
            nameof(WindowBackgroundLayout) => WindowBackgroundLayout == GetDefault<ImageLayout>(nameof(WindowBackgroundLayout)),
            nameof(WindowColorsForceDefault) => WindowColorsForceDefault == GetDefault<bool>(nameof(WindowColorsForceDefault)),
            nameof(WindowColors) => WindowColors.SequenceEqualEx(GetDefault<Dictionary<ColorOption, Color>>(nameof(WindowColors))),
            nameof(CustomColors) => CustomColors.SequenceEqualEx(GetDefault<List<Color>>(nameof(CustomColors))),
            nameof(AppInstallLocations) => AppInstallLocations == GetDefault<List<string>>(nameof(AppInstallLocations)),
            nameof(LastApp) => LastApp == GetDefault<string>(nameof(LastApp)),
            nameof(LastIconResourcePath) => LastIconResourcePath == GetDefault<string>(nameof(LastIconResourcePath)),
            nameof(SystemIntegration) => SystemIntegration == GetDefault<bool>(nameof(SystemIntegration)),
            nameof(SystemStartMenuIntegration) => SystemStartMenuIntegration == GetDefault<bool>(nameof(SystemStartMenuIntegration)),
            nameof(UpdateCheck) => UpdateCheck == GetDefault<UpdateCheckOption>(nameof(UpdateCheck)),
            nameof(UpdateChannel) => UpdateChannel == GetDefault<UpdateChannelOption>(nameof(UpdateChannel)),
            nameof(LastUpdateCheck) => LastUpdateCheck == GetDefault<DateTime>(nameof(LastUpdateCheck)),
            nameof(CurrentAssemblyVersion) => false,
            _ => true
        };

    /// <summary>
    ///     Releases all resources used by this <see cref="AppsLauncherJson"/>.
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

    /// <summary>
    ///     Determines whether two specified <see cref="AppsLauncherJson"/>
    ///     instances have same values.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="AppsLauncherJson"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="AppsLauncherJson"/> instance to compare.
    /// </param>
    public static bool operator ==(AppsLauncherJson left, AppsLauncherJson right) =>
        Equals(left, right);

    /// <summary>
    ///     Determines whether two specified <see cref="AppsLauncherJson"/>
    ///     instances have different values.
    /// </summary>
    /// <inheritdoc cref="operator ==(AppsLauncherJson, AppsLauncherJson)"/>
    public static bool operator !=(AppsLauncherJson left, AppsLauncherJson right) =>
        !Equals(left, right);
}
