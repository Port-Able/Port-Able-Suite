namespace AppsDownloader.Data
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Web.Script.Serialization;
    using System.Windows.Forms;
    using PortAble;
    using SilDev;

    /// <summary>
    ///     Provides local app settings.
    /// </summary>
    [Serializable]
    public sealed class AppsDownloaderSettings : ISerializable, IEquatable<AppsDownloaderSettings>
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
        [ScriptIgnore]
        public string FilePath
        {
            get
            {
                if (_filePath != null)
                    return _filePath;
                var dir = PathEx.Combine(CorePaths.DataDir, "Settings");
                return _filePath = Path.Combine(dir, $"{nameof(AppsDownloader)}.json");
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
        ///     Initialize the <see cref="AppsDownloaderSettings"/> class.
        /// </summary>
        public AppsDownloaderSettings()
        {
            SetDefaults();
            LoadFromFile(true);
        }

        private AppsDownloaderSettings(SerializationInfo info, StreamingContext context)
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
                    case nameof(TransferDir):
                        TransferDir = info.GetString(nameof(TransferDir));
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

            info.AddValue(nameof(TransferDir), TransferDir);
            info.AddValue(nameof(ShowLargeImages), ShowLargeImages);
            info.AddValue(nameof(ShowGroups), ShowGroups);
            info.AddValue(nameof(ShowGroupColors), ShowGroupColors);
            info.AddValue(nameof(HighlightInstalled), HighlightInstalled);
            info.AddValue(nameof(GroupColors), GroupColors);
            info.AddValue(nameof(WindowSize), WindowSize);
            info.AddValue(nameof(WindowState), WindowState);
            info.AddValue(nameof(CustomColors), CustomColors);
        }

        /// <summary>
        ///     Determines whether this instance have same values as the specified
        ///     <see cref="AppsDownloaderSettings"/> instance.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="AppsDownloaderSettings"/> instance to compare.
        /// </param>
        public bool Equals(AppsDownloaderSettings other) =>
            other != null &&
            TransferDir == other.TransferDir &&
            ShowLargeImages == other.ShowLargeImages &&
            ShowGroups == other.ShowGroups &&
            ShowGroupColors == other.ShowGroupColors &&
            HighlightInstalled == other.HighlightInstalled &&
            WindowSize == other.WindowSize &&
            WindowState == other.WindowState &&
            (GroupColors?.SequenceEqual(other.GroupColors) ?? other.GroupColors == default) &&
            (CustomColors?.SequenceEqual(other.CustomColors) ?? other.CustomColors == default);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is AppsDownloaderSettings other)
                return Equals(other);
            return false;
        }

        /// <inheritdoc cref="Type.GetHashCode()"/>
        public override int GetHashCode() =>
            Tuple.Create(GetType().FullName, nameof(AppsDownloaderSettings)).GetHashCode();

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
            TransferDir = EnvironmentEx.GetVariableWithPath(CorePaths.TransferDir);
            ShowLargeImages = false;
            ShowGroups = true;
            ShowGroupColors = true;
            HighlightInstalled = true;
            WindowSize = new Size(760, 700);
            WindowState = FormWindowState.Normal;
            SetDefaultColors();
        }

        /// <summary>
        ///     Reset <see cref="GroupColors"/> and <see cref="CustomColors"/> to default
        ///     values.
        /// </summary>
        public void SetDefaultColors()
        {
            GroupColors = new()
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
            CustomColors = GroupColors.Values.ToList();
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
            var item = CacheData.LoadJson<AppsDownloaderSettings>(FilePath);
            if (item == default)
                return;
            if (DirectoryEx.Exists(item.TransferDir))
                TransferDir = item.TransferDir;
            ShowLargeImages = item.ShowLargeImages;
            ShowGroups = item.ShowGroups;
            ShowGroupColors = item.ShowGroupColors;
            HighlightInstalled = item.HighlightInstalled;
            if (item.GroupColors?.Count > 12)
                GroupColors = item.GroupColors;
            WindowSize = item.WindowSize;
            WindowState = item.WindowState;
            CustomColors = item.CustomColors;
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
                var item = CacheData.LoadJson<AppsDownloaderSettings>(FilePath);
                if (this == item)
                    return;
            }
            CacheData.SaveJson(this, FilePath);
            InstanceTime = DateTime.Now;
        }

        /// <summary>
        ///     Determines whether two specified <see cref="AppsDownloaderSettings"/>
        ///     instances have same values.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="AppsDownloaderSettings"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="AppsDownloaderSettings"/> instance to compare.
        /// </param>
        public static bool operator ==(AppsDownloaderSettings left, AppsDownloaderSettings right) =>
            Equals(left, right);

        /// <summary>
        ///     Determines whether two specified <see cref="AppsDownloaderSettings"/>
        ///     instances have different values.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="AppsDownloaderSettings"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="AppsDownloaderSettings"/> instance to compare.
        /// </param>
        public static bool operator !=(AppsDownloaderSettings left, AppsDownloaderSettings right) =>
            !Equals(left, right);
    }
}
