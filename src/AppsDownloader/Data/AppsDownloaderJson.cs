namespace AppsDownloader.Data;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Windows.Forms;
using Newtonsoft.Json;
using PortAble;
using PortAble.Model;
using SilDev;

/// <summary>
///     Provides Apps Downloader settings.
/// </summary>
[Serializable]
public sealed class AppsDownloaderJson : IObjectFile<AppsDownloaderJson>
{
    [NonSerialized]
    private string _filePath;

    /// <summary>
    ///     The transfer directory.
    /// </summary>
    public string TransferDir { get; set; }

    /// <summary>
    ///     Determines whether the apps images are large.
    /// </summary>
    public bool ShowLargeImages { get; set; }

    /// <summary>
    ///     Determines whether the apps are listed within categories.
    /// </summary>
    public bool ShowGroups { get; set; }

    /// <summary>
    ///     Determines whether categories are displayed in different colors.
    /// </summary>
    public bool ShowGroupColors { get; set; }

    /// <summary>
    ///     Determines whether installed apps are highlighted.
    /// </summary>
    public bool HighlightInstalled { get; set; }

    /// <summary>
    ///     A list of colors used to color app categories.
    /// </summary>
    public Dictionary<string, Color> GroupColors { get; set; }

    /// <summary>
    ///     The size of the main window.
    /// </summary>
    public Size WindowSize { get; set; }

    /// <summary>
    ///     The state of the main window.
    /// </summary>
    public FormWindowState WindowState { get; set; }

    /// <summary>
    ///     The custom colors used for the color dialog.
    /// </summary>
    public List<Color> CustomColors { get; set; }

    /// <summary>
    ///     The path on disk where the instance will be saved and loaded.
    /// </summary>
    [JsonIgnore]
    public string FilePath => _filePath ??= Path.Combine(CorePaths.SettingsDir, $"{nameof(AppsDownloader)}.json");

    /// <summary>
    ///     The exact time when this instance was created, updated via the
    ///     <see cref="LoadFromFile"/> method, or when the <see cref="SaveToFile"/>
    ///     method was called. So this <see cref="DateTime"/> is used to synchronize
    ///     with the file on the hard drive.
    /// </summary>
    [JsonIgnore]
    public DateTime InstanceTime { get; private set; }

    /// <summary>
    ///     Initialize the <see cref="AppsDownloaderJson"/> class.
    /// </summary>
    public AppsDownloaderJson()
    {
        SetDefaults();
        LoadFromFile(true);
    }

    private AppsDownloaderJson(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        // Set default values in case some properties have
        // been deleted from the configuration file.
        SetDefaults();

        // Only set properties that are also present in
        // the configuration file.
        var enumerator = info.GetEnumerator();
        while (enumerator.MoveNext())
            switch (enumerator.Name)
            {
                case nameof(TransferDir):
                    TransferDir = PathEx.Combine(info.GetString(nameof(TransferDir)));
                    if (PathEx.IsValidPath(TransferDir) && DirectoryEx.Create(TransferDir))
                        CachePaths.TransferDir = TransferDir;
                    break;
                case nameof(ShowLargeImages):
                    ShowLargeImages = info.GetBoolean(nameof(ShowLargeImages));
                    break;
                case nameof(ShowGroups):
                    ShowGroups = info.GetBoolean(nameof(ShowGroups));
                    break;
                case nameof(ShowGroupColors):
                    ShowGroupColors = info.GetBoolean(nameof(ShowGroupColors));
                    break;
                case nameof(HighlightInstalled):
                    HighlightInstalled = info.GetBoolean(nameof(HighlightInstalled));
                    break;
                case nameof(GroupColors):
                    GroupColors = (Dictionary<string, Color>)info.GetValue(nameof(GroupColors), typeof(Dictionary<string, Color>));
                    break;
                case nameof(WindowSize):
                    WindowSize = (Size)info.GetValue(nameof(WindowSize), typeof(Size));
                    break;
                case nameof(WindowState):
                    WindowState = (FormWindowState)info.GetValue(nameof(WindowState), typeof(FormWindowState));
                    break;
                case nameof(CustomColors):
                    CustomColors = (List<Color>)info.GetValue(nameof(CustomColors), typeof(List<Color>));
                    break;
            }

        // Finally, set the instance creation time.
        InstanceTime = DateTime.Now;
    }

    /// <inheritdoc cref="ISerializable.GetObjectData(SerializationInfo, StreamingContext)"/>
    [SecurityCritical]
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        // Do not serialize default values.
        foreach (var pi in GetType().GetPropertiesEx(typeof(JsonIgnoreAttribute)).Where(pi => !IsDefault(pi)))
        {
            if (pi.Name == nameof(TransferDir))
            {
                info.AddValue(pi.Name, EnvironmentEx.GetVariableWithPath((string)pi.GetValue(this)));
                continue;
            }
            info.AddValue(pi.Name, pi.GetValue(this));
        }
    }

    /// <summary>
    ///     Determines whether this instance have same values as the specified
    ///     <see cref="AppsDownloaderJson"/> instance.
    ///     <para>
    ///         &#9888; Not consistent with <see cref="GetHashCode"/>, which is
    ///         intentional.
    ///     </para>
    /// </summary>
    /// <param name="other">
    ///     The <see cref="AppsDownloaderJson"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the values of the specified
    ///     <see cref="AppsDownloaderJson"/> instance are equal to the values of
    ///     the current <see cref="AppsDownloaderJson"/> instance; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    public bool Equals(AppsDownloaderJson other) =>
        other != null &&
        TransferDir == other.TransferDir &&
        ShowLargeImages == other.ShowLargeImages &&
        ShowGroups == other.ShowGroups &&
        ShowGroupColors == other.ShowGroupColors &&
        HighlightInstalled == other.HighlightInstalled &&
        WindowSize == other.WindowSize &&
        WindowState == other.WindowState &&
        GroupColors.SequenceEqualEx(other.GroupColors) &&
        CustomColors.SequenceEqualEx(other.CustomColors);

    /// <inheritdoc cref="Equals(AppsDownloaderJson)"/>
    public override bool Equals(object other)
    {
        if (other is AppsDownloaderJson instance)
            return Equals(instance);
        return false;
    }

    /// <inheritdoc cref="Type.GetHashCode()"/>
    public override int GetHashCode() =>
        (typeof(AppsDownloaderJson).FullName, nameof(AppsDownloaderJson)).GetHashCode();

    /// <see cref="IObjectFile.ToString(bool)"/>
    public string ToString(bool formatted) =>
        JsonConvert.SerializeObject(this, formatted ? Formatting.Indented : Formatting.None);

    /// <inheritdoc cref="IObjectFile.ToString()"/>
    public override string ToString() =>
        ToString(false);

    /// <summary>
    ///     Reset all properties to their default values.
    /// </summary>
    public void SetDefaults()
    {
        TransferDir = CachePaths.TransferDir;
        ShowLargeImages = false;
        ShowGroups = true;
        ShowGroupColors = true;
        HighlightInstalled = true;
        WindowSize = new(880, 800);
        WindowState = FormWindowState.Normal;
        SetDefaultColors();
    }

    /// <summary>
    ///     Reset <see cref="GroupColors"/> and <see cref="CustomColors"/> to default
    ///     values.
    /// </summary>
    public void SetDefaultColors()
    {
        GroupColors = GetDefaultColors();
        CustomColors = GroupColors.Values.ToList();
    }

    /// <summary>
    ///     Determines whether the JSON file exists in the
    ///     <see cref="CorePaths.AppSettingsDir"/> folder.
    /// </summary>
    public bool FileExists() =>
        FileEx.Exists(FilePath);

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
        if (CacheData.LoadJson<AppsDownloaderJson>(FilePath) is not { } item)
            return;
        if (DirectoryEx.Exists(item.TransferDir))
            TransferDir = item.TransferDir;
        ShowLargeImages = item.ShowLargeImages;
        ShowGroups = item.ShowGroups;
        ShowGroupColors = item.ShowGroupColors;
        HighlightInstalled = item.HighlightInstalled;
        if (item.GroupColors?.Count is > 12)
            GroupColors = item.GroupColors;
        WindowSize = item.WindowSize;
        WindowState = item.WindowState;
        CustomColors = item.CustomColors;
        InstanceTime = DateTime.Now;
    }

    /// <summary>
    ///     Removes the JSON file from the <see cref="CorePaths.DataDir"/> directory.
    /// </summary>
    public void RemoveFile() =>
        FileEx.TryDelete(FilePath);

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
            var item = CacheData.LoadJson<AppsDownloaderJson>(FilePath) ?? new AppsDownloaderJson();
            if (this == item)
                return;
        }
        CacheData.SaveJson(this, FilePath);
        InstanceTime = DateTime.Now;
    }

    private static Dictionary<string, Color> GetDefaultColors() =>
        new()
        {
            { "listViewGroup0", SystemColors.Highlight }, // S E A R C H
            { "listViewGroup1", Color.FromArgb(0xff, 0xff, 0x99) }, // Accessibility
            { "listViewGroup2", Color.FromArgb(0xff, 0xff, 0xcc) }, // Education
            { "listViewGroup3", Color.FromArgb(0xd5, 0xd5, 0xdf) }, // Development
            { "listViewGroup4", Color.FromArgb(0xbb, 0xe9, 0xec) }, // Office
            { "listViewGroup5", Color.FromArgb(0xee, 0xd9, 0xce) }, // Internet
            { "listViewGroup6", Color.FromArgb(0xff, 0xcc, 0xff) }, // Graphics and Pictures
            { "listViewGroup7", Color.FromArgb(0xcc, 0xcc, 0xff) }, // Music and Video
            { "listViewGroup8", Color.FromArgb(0xb5, 0xff, 0x99) }, // Security
            { "listViewGroup9", Color.FromArgb(0xc5, 0xe2, 0xe2) }, // Utilities
            { "listViewGroup10", SystemColors.Window }, // Games
            { "listViewGroup11", Color.FromArgb(0xff, 0x95, 0x95) } // Advanced
        };

    private bool IsDefault(PropertyInfo pi) =>
        pi?.Name switch
        {
            nameof(TransferDir) => TransferDir.EqualsEx(CorePaths.TransferDir),
            nameof(ShowLargeImages) => !ShowLargeImages,
            nameof(ShowGroups) => ShowGroups,
            nameof(ShowGroupColors) => ShowGroupColors,
            nameof(HighlightInstalled) => HighlightInstalled,
            nameof(WindowSize) => WindowSize is { Width: 880, Height: 800 },
            nameof(WindowState) => WindowState == FormWindowState.Normal,
            nameof(GroupColors) => GroupColors.SequenceEqualEx(GetDefaultColors()),
            nameof(CustomColors) => CustomColors.SequenceEqualEx(GetDefaultColors().Values),
            _ => true
        };

    /// <summary>
    ///     Determines whether two specified <see cref="AppsDownloaderJson"/>
    ///     instances have same values.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="AppsDownloaderJson"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="AppsDownloaderJson"/> instance to compare.
    /// </param>
    public static bool operator ==(AppsDownloaderJson left, AppsDownloaderJson right) =>
        Equals(left, right);

    /// <summary>
    ///     Determines whether two specified <see cref="AppsDownloaderJson"/>
    ///     instances have different values.
    /// </summary>
    /// <inheritdoc cref="operator ==(AppsDownloaderJson, AppsDownloaderJson)"/>
    public static bool operator !=(AppsDownloaderJson left, AppsDownloaderJson right) =>
        !Equals(left, right);
}