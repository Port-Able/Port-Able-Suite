namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using SilDev;

    /// <summary>
    ///     Provides local app settings.
    /// </summary>
    [Serializable]
    public sealed class AppSettings : ISerializable
    {
        /// <summary>
        ///     The app key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     Determines whether this app should run with administrator privileges.
        /// </summary>
        public bool RunAsAdmin { get; set; }

        /// <summary>
        ///     Determines whether the command line arguments should be sorted
        ///     alphabetically, which can be very useful for file paths or similar.
        /// </summary>
        public bool SortArgPaths { get; set; }

        /// <summary>
        ///     The command line argument that is added before the command line arguments
        ///     that the user sends.
        /// </summary>
        public string StartArgsFirst { get; set; }

        /// <summary>
        ///     The command line argument that is added after the command line arguments
        ///     that the user sends.
        /// </summary>
        public string StartArgsLast { get; set; }

        /// <summary>
        ///     The file types associated with this app.
        /// </summary>
        public Collection<string> FileTypes { get; set; }

        /// <summary>
        ///     The install setup language.
        /// </summary>
        public string ArchiveLang { get; set; }

        /// <summary>
        ///     The install setup language code.
        /// </summary>
        public int ArchiveLangCode { get; set; }

        /// <summary>
        ///     Determines that the <see cref="ArchiveLang"/> is automatically confirmed
        ///     for each additional question.
        /// </summary>
        public bool ArchiveLangConfirmed { get; set; }

        /// <summary>
        ///     Skip update search for this app.
        /// </summary>
        public bool DisableUpdates { get; set; }

        /// <summary>
        ///     Skip the update search only until the defined date.
        /// </summary>
        public DateTime DelayUpdates { get; set; }

        /// <summary>
        ///     The exact time when this instance was created, updated via the
        ///     <see cref="LoadFromFile"/> method, or when the <see cref="SaveToFile"/>
        ///     method was called. So this <see cref="DateTime"/> is used to synchronize
        ///     with the file on the hard drive.
        /// </summary>
        public DateTime InstanceTime { get; private set; }

        /// <summary>
        ///     Initialize the <see cref="AppSettings"/> class.
        /// </summary>
        /// <param name="parent">
        ///     The <see cref="LocalAppData"/> instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     parent is null.
        /// </exception>
        public AppSettings(LocalAppData parent)
        {
            Key = parent?.Key ?? throw new ArgumentNullException(nameof(parent));
            RunAsAdmin = default;
            FileTypes = default;
            SortArgPaths = default;
            StartArgsFirst = default;
            StartArgsLast = default;
            ArchiveLang = "Default";
            ArchiveLangCode = 1033;
            ArchiveLangConfirmed = default;
            DisableUpdates = default;
            DelayUpdates = default;
            LoadFromFile(true);
        }

        private AppSettings(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            if (Log.DebugMode > 1)
                Log.Write($"{nameof(AppSettings)}.ctor({nameof(SerializationInfo)}, {nameof(StreamingContext)}) => info: {Json.Serialize(context)}, context: {Json.Serialize(context)}");

            Key = info.GetString(nameof(Key));
            RunAsAdmin = info.GetBoolean(nameof(DisableUpdates));
            FileTypes = (Collection<string>)info.GetValue(nameof(FileTypes), typeof(Collection<string>));
            SortArgPaths = info.GetBoolean(nameof(SortArgPaths));
            StartArgsFirst = info.GetString(nameof(StartArgsFirst));
            StartArgsLast = info.GetString(nameof(StartArgsLast));
            ArchiveLang = info.GetString(nameof(ArchiveLang));
            ArchiveLangCode = info.GetInt32(nameof(ArchiveLangCode));
            ArchiveLangConfirmed = info.GetBoolean(nameof(ArchiveLangConfirmed));
            DisableUpdates = info.GetBoolean(nameof(DisableUpdates));
            DelayUpdates = info.GetDateTime(nameof(DelayUpdates));
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

            if (Log.DebugMode > 1)
                Log.Write($"{nameof(AppSettings)}.get({nameof(SerializationInfo)}, {nameof(StreamingContext)}) => info: {Json.Serialize(context)}, context: {Json.Serialize(context)}");

            info.AddValue(nameof(Key), Key);
            info.AddValue(nameof(RunAsAdmin), RunAsAdmin);
            info.AddValue(nameof(FileTypes), FileTypes);
            info.AddValue(nameof(SortArgPaths), SortArgPaths);
            info.AddValue(nameof(StartArgsFirst), StartArgsFirst);
            info.AddValue(nameof(StartArgsLast), StartArgsLast);
            info.AddValue(nameof(ArchiveLang), ArchiveLang);
            info.AddValue(nameof(ArchiveLangCode), ArchiveLangCode);
            info.AddValue(nameof(ArchiveLangConfirmed), ArchiveLangConfirmed);
            info.AddValue(nameof(DisableUpdates), DisableUpdates);
            info.AddValue(nameof(DelayUpdates), DelayUpdates);
        }

        /// <summary>
        ///     Determines whether this instance have same values as the specified
        ///     <see cref="AppSettings"/> instance.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="AppSettings"/> instance to compare.
        /// </param>
        public bool Equals(AppSettings other)
        {
            if (other == null ||
                Key != other.Key ||
                RunAsAdmin != other.RunAsAdmin ||
                SortArgPaths != other.SortArgPaths ||
                StartArgsFirst != other.StartArgsFirst ||
                StartArgsLast != other.StartArgsLast ||
                ArchiveLang != other.ArchiveLang ||
                ArchiveLangCode != other.ArchiveLangCode ||
                ArchiveLangConfirmed != other.ArchiveLangConfirmed ||
                DisableUpdates != other.DisableUpdates ||
                DelayUpdates != other.DelayUpdates)
                return false;
            if (FileTypes == default)
                return other.FileTypes == default;
            return FileTypes.SequenceEqual(other.FileTypes);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is AppSettings other)
                return Equals(other);
            return false;
        }

        /// <inheritdoc cref="Type.GetHashCode()"/>
        public override int GetHashCode() =>
            Tuple.Create(GetType().FullName, Key).GetHashCode();

        /// <summary>
        ///     Load the data from a JSON file stored in the
        ///     <see cref="CacheFiles.AppSettingsDir"/> folder if newer than this instance.
        /// </summary>
        /// <param name="force">
        ///     Determines whether to force an update from the file.
        /// </param>
        public void LoadFromFile(bool force = false)
        {
            var path = PathEx.Combine(CacheFiles.AppSettingsDir, $"{Key}.json");
            if (!FileEx.Exists(path))
            {
                InstanceTime = DateTime.MinValue;
                return;
            }
            if (!force && File.GetLastWriteTime(path) < InstanceTime)
                return;
            var item = CacheData.Deserialize<AppSettings>(path);
            if (item == null)
                return;
            RunAsAdmin = item.RunAsAdmin;
            FileTypes = item.FileTypes;
            SortArgPaths = item.SortArgPaths;
            StartArgsFirst = item.StartArgsFirst;
            StartArgsLast = item.StartArgsLast;
            ArchiveLang = item.ArchiveLang;
            ArchiveLangCode = item.ArchiveLangCode;
            ArchiveLangConfirmed = item.ArchiveLangConfirmed;
            DisableUpdates = item.DisableUpdates;
            DelayUpdates = item.DelayUpdates;
            InstanceTime = DateTime.Now;
        }

        /// <summary>
        ///     Saves the data into a JSON file stored in the
        ///     <see cref="CacheFiles.AppSettingsDir"/> directory if newer than the file.
        /// </summary>
        /// <param name="force">
        ///     Determines whether to force an update to the file.
        /// </param>
        public void SaveToFile(bool force = false)
        {
            var path = PathEx.Combine(CacheFiles.AppSettingsDir, $"{Key}.json");
            if (!DirectoryEx.Create(CacheFiles.AppSettingsDir))
                throw new DirectoryNotFoundException(CacheFiles.AppSettingsDir);
            if (!force)
            {
                var item = CacheData.Deserialize<AppSettings>(path);
                if (this == item)
                    return;
            }
            CacheData.Serialize(path, this);
            InstanceTime = DateTime.Now;
        }

        /// <summary>
        ///     Determines whether two specified <see cref="AppSettings"/> instances have
        ///     same values.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="AppSettings"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="AppSettings"/> instance to compare.
        /// </param>
        public static bool operator ==(AppSettings left, AppSettings right) =>
            Equals(left, right);

        /// <summary>
        ///     Determines whether two specified <see cref="AppSettings"/> instances have
        ///     different values.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="AppSettings"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="AppSettings"/> instance to compare.
        /// </param>
        public static bool operator !=(AppSettings left, AppSettings right) =>
            !Equals(left, right);
    }
}
