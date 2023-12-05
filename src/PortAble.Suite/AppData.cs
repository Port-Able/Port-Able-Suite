namespace PortAble
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text;
    using System.Web.Script.Serialization;
    using SilDev;
    using SilDev.Network;
    using DataCollection = System.Collections.ObjectModel.ReadOnlyCollection<string>;
    using DownloadCollection = System.Collections.ObjectModel.ReadOnlyDictionary<string, System.Collections.ObjectModel.ReadOnlyCollection<System.Tuple<string, string>>>;
    using VersionCollection = System.Collections.ObjectModel.ReadOnlyCollection<System.Tuple<string, string>>;

    /// <summary>
    ///     Provides app information from a server.
    /// </summary>
    [Serializable]
    public sealed class AppData : ISerializable
    {
        [NonSerialized]
        private string _installDir;

        [NonSerialized]
        private AppSettings _settings;

        /// <summary>
        ///     The app key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     The app name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The app description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     The app category.
        /// </summary>
        public string Category { get; }

        /// <summary>
        ///     The app website or the program website, depending on the source of the
        ///     download source.
        /// </summary>
        public string Website { get; }

        /// <summary>
        ///     The display version of the app.
        /// </summary>
        public string DisplayVersion { get; }

        /// <summary>
        ///     The app version number used to check for updates for some apps.
        /// </summary>
        public Version PackageVersion { get; }

        /// <summary>
        ///     An advanced data collection that stores file paths with their hash numbers,
        ///     which provide another way to check for app updates.
        ///     <para>
        ///         - The app installation folder is the root folder and is not specified
        ///         in the file path.
        ///     </para>
        ///     <para>
        ///         - The file path should not begin with a backslash.
        ///     </para>
        ///     <para>
        ///         - The hashes can be created using the MD5, SHA-1, SHA-256, SHA-384 or
        ///         SHA-512 standards, other algorithms are not supported.
        ///     </para>
        /// </summary>
        public VersionCollection VersionData { get; }

        /// <summary>
        ///     The default language, used only when more than one language is available
        ///     for download.
        /// </summary>
        public string DefaultLanguage { get; }

        /// <summary>
        ///     A collection of languages for which an app provides different downloads.
        /// </summary>
        public DataCollection Languages { get; }

        /// <summary>
        ///     A collection of download URL’s and their hash numbers to verify them.
        ///     <para>
        ///         An app can be made available in multiple languages through multiple
        ///         download sources.
        ///     </para>
        ///     <para>
        ///         Therefore the following hierarchy is used:
        ///     </para>
        ///     <code>
        ///         <see langword="this"/>["LANG_NAME"][INDEX].Item1 == "HTTPS_ADDRESS"
        ///     </code>
        ///     <code>
        ///         <see langword="this"/>["LANG_NAME"][INDEX].Item2 == "FILE_HASH"
        ///     </code>
        /// </summary>
        public DownloadCollection DownloadCollection { get; }

        /// <summary>
        ///     A collection of update URL’s and their hash numbers to verify them.
        ///     <para>
        ///         Some apps provide additional sources for their updates to reduce
        ///         download size. The hierarchy is the identical to the
        ///         <see cref="DownloadCollection"/> property.
        ///     </para>
        /// </summary>
        public DownloadCollection UpdateCollection { get; }

        /// <summary>
        ///     The app download size in bytes.
        /// </summary>
        public long DownloadSize { get; }

        /// <summary>
        ///     The app install size in bytes.
        /// </summary>
        public long InstallSize { get; }

        /// <summary>
        ///     The app install location.
        /// </summary>
        [ScriptIgnore]
        public string InstallDir
        {
            get
            {
                if (_installDir != default)
                    return _installDir;
                var appDir = CorePaths.AppsDir;
                switch (Key)
                {
                    case "Ghostscript":
                    case "GPG":
                    case "Java":
                    case "Java64":
                    case "JDK":
                    case "JDK64":
                    case "OpenJDK":
                    case "OpenJDK64":
                    case "OpenJDKJRE":
                    case "OpenJDKJRE64":
                        return _installDir = Path.Combine(appDir, "CommonFiles", Key);
                }
                if (Supplier != default)
                {
                    appDir = CorePaths.AppDirs.Last();
                    return _installDir = Path.Combine(appDir, Key.TrimEnd('#'));
                }
                if (!DownloadCollection.Any())
                    return default;
                var downloadUrl = DownloadCollection.First().Value.FirstOrDefault()?.Item1;
                if (downloadUrl?.Any() == true && NetEx.GetShortHost(downloadUrl)?.EqualsEx(AppSupplierHosts.Pa) == true)
                    appDir = downloadUrl.ContainsEx("/.repack/") ? CorePaths.AppDirs.Third() : CorePaths.AppDirs.Second();
                return _installDir = Path.Combine(appDir, Key);
            }
        }

        /// <summary>
        ///     The install requirements of an app.
        ///     <para>
        ///         Some apps may require the JRE to run, or have other dependencies. This
        ///         property is used to ensure that necessary dependencies are not
        ///         forgotten.
        ///     </para>
        /// </summary>
        public DataCollection Requirements { get; }

        /// <summary>
        ///     The release date of the portable package.
        /// </summary>
        public DateTime PackageReleaseDate { get; }

        /// <summary>
        ///     The update date of the portable package.
        /// </summary>
        public DateTime PackageUpdateDate { get; }

        /// <summary>
        ///     The version number of the installer.
        /// </summary>
        public Version InstallerVersion { get; }

        /// <summary>
        ///     Forces an app to be placed in the Advanced <see cref="Category"/>, which is
        ///     used for beta, discontinued, legacy apps or similar.
        /// </summary>
        public bool Advanced { get; set; }

        /// <summary>
        /// </summary>
        [ScriptIgnore]
        public CustomAppSupplier Supplier { get; }

        /// <summary>
        ///     An initialized <see cref="AppSettings"/> class, which provides user
        ///     settings for an app.
        /// </summary>
        [ScriptIgnore]
        public AppSettings Settings => _settings ??= new AppSettings(this);

        /// <summary>
        ///     Initialize the <see cref="AppData"/> class.
        /// </summary>
        /// <param name="key">
        ///     The app key.
        /// </param>
        /// <param name="name">
        ///     The app name.
        /// </param>
        /// <param name="description">
        ///     The app category.
        /// </param>
        /// <param name="category">
        ///     The app category.
        /// </param>
        /// <param name="website">
        ///     The app website or the program website, depending on the source of the
        ///     download source.
        /// </param>
        /// <param name="displayVersion">
        ///     The display version of the app.
        /// </param>
        /// <param name="packageVersion">
        ///     The app version number used to check for updates for some apps.
        /// </param>
        /// <param name="versionData">
        ///     An advanced data collection that stores file paths with their hash numbers,
        ///     which provide another way to check for app updates.
        ///     <para>
        ///         - The app installation folder is the root folder and is not specified
        ///         in the file path.
        ///     </para>
        ///     <para>
        ///         - The file path should not begin with a backslash.
        ///     </para>
        ///     <para>
        ///         - The hashes can be created using the MD5, SHA-1, SHA-256, SHA-384 or
        ///         SHA-512 standards, other algorithms are not supported.
        ///     </para>
        /// </param>
        /// <param name="downloadCollection">
        ///     A collection of download URL’s and their hash numbers to verify them.
        ///     <para>
        ///         An app can be made available in multiple languages through multiple
        ///         download sources. Therefore the following hierarchy is used:
        ///     </para>
        ///     <para>
        ///         - Dictionary[ LANG_STRING ][ INDEX ].Item1 == URL
        ///     </para>
        ///     <para>
        ///         - Dictionary[ LANG_STRING ][ INDEX ].Item2 == Hash
        ///     </para>
        /// </param>
        /// <param name="updateCollection">
        ///     A collection of update URL’s and their hash numbers to verify them.
        ///     <para>
        ///         Some apps provide additional sources for their updates to reduce
        ///         download size. The hierarchy is the identical to the
        ///         <see cref="DownloadCollection"/> property.
        ///     </para>
        /// </param>
        /// <param name="downloadSize">
        ///     The app download size in bytes.
        /// </param>
        /// <param name="installSize">
        ///     The app install size in bytes.
        /// </param>
        /// <param name="requirements">
        ///     The install requirements of an app.
        ///     <para>
        ///         Some apps may require the JRE to run, or have other dependencies. This
        ///         property is used to ensure that necessary dependencies are not
        ///         forgotten.
        ///     </para>
        /// </param>
        /// <param name="advanced">
        ///     The release date of the portable package.
        /// </param>
        /// <param name="packageReleaseDate">
        ///     The update date of the portable package.
        /// </param>
        /// <param name="packageUpdateDate">
        ///     Forces an app to be placed in the Advanced <see cref="Category"/>, which is
        ///     used for beta, discontinued, legacy apps or similar.
        /// </param>
        /// <param name="installerVersion">
        ///     The version number of the installer.
        /// </param>
        /// <param name="customAppSupplier">
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public AppData(string key, string name, string description, string category, string website,
                       string displayVersion, Version packageVersion, VersionCollection versionData,
                       DownloadCollection downloadCollection, DownloadCollection updateCollection,
                       long downloadSize, long installSize, DataCollection requirements,
                       DateTime packageReleaseDate, DateTime packageUpdateDate,
                       Version installerVersion, bool advanced,
                       CustomAppSupplier customAppSupplier = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException(nameof(category));

            if (downloadCollection == default(DownloadCollection) || !downloadCollection.Values.Any())
                throw new ArgumentNullException(nameof(downloadCollection));

            if (website?.StartsWithEx("http") != true)
                website = "https://duckduckgo.com/?q=" + WebUtility.UrlEncode(key.TrimEnd('#'));

            if (string.IsNullOrWhiteSpace(displayVersion))
                displayVersion = "1.0.0.0";

            if (packageVersion == default)
                packageVersion = new Version("1.0.0.0");

            versionData ??= new VersionCollection(Array.Empty<Tuple<string, string>>());

            if (downloadSize < 0x100000)
                downloadSize = 0x100000;

            if (installSize < 0x100000)
                installSize = 0x100000;

            requirements ??= new DataCollection(Array.Empty<string>());

            Key = key;
            Name = name;
            Description = description;
            Category = category;
            Website = website;

            DisplayVersion = displayVersion;
            PackageVersion = packageVersion;
            VersionData = versionData;

            DefaultLanguage = downloadCollection.Keys.First();
            Languages = new DataCollection(downloadCollection.Keys.ToArray());

            DownloadCollection = downloadCollection;
            UpdateCollection = updateCollection;
            DownloadSize = downloadSize;
            InstallSize = installSize;

            Requirements = requirements;

            PackageReleaseDate = packageReleaseDate;
            PackageUpdateDate = packageUpdateDate;
            InstallerVersion = installerVersion;

            Advanced = advanced;

            Supplier = customAppSupplier;
        }

        private AppData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            Key = info.GetString(nameof(Key));
            Name = info.GetString(nameof(Name));
            Description = info.GetString(nameof(Description));
            Category = info.GetString(nameof(Category));
            Website = info.GetString(nameof(Website));

            DisplayVersion = info.GetString(nameof(DisplayVersion));
            PackageVersion = (Version)info.GetValue(nameof(PackageVersion), typeof(Version));
            VersionData = (VersionCollection)info.GetValue(nameof(VersionData), typeof(VersionCollection));

            DefaultLanguage = info.GetString(nameof(DefaultLanguage));
            Languages = (DataCollection)info.GetValue(nameof(Languages), typeof(DataCollection));

            DownloadCollection = (DownloadCollection)info.GetValue(nameof(DownloadCollection), typeof(DownloadCollection));
            UpdateCollection = (DownloadCollection)info.GetValue(nameof(UpdateCollection), typeof(DownloadCollection));
            DownloadSize = info.GetInt64(nameof(DownloadSize));
            InstallSize = info.GetInt64(nameof(InstallSize));

            Requirements = (DataCollection)info.GetValue(nameof(Requirements), typeof(DataCollection));

            PackageReleaseDate = info.GetDateTime(nameof(PackageReleaseDate));
            PackageUpdateDate = info.GetDateTime(nameof(PackageUpdateDate));
            InstallerVersion = (Version)info.GetValue(nameof(InstallerVersion), typeof(Version));

            Advanced = info.GetBoolean(nameof(Advanced));

            Supplier = default;

            if (Log.DebugMode > 1)
                Log.Write($"{nameof(AppData)}.ctor({nameof(SerializationInfo)} => Key: {Key}, Name: {Name}, DisplayVersion: {DisplayVersion}");
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

            // used to verify custom server data, which should never be serialized
            if (Supplier != null)
                return;

            // cancel if required data is invalid
            if (Languages?.Any() != true ||
                DownloadCollection?.Any() != true ||
                DownloadCollection.Values.FirstOrDefault()?.Any() != true ||
                DownloadCollection.SelectMany(x => x.Value).Any(x => x?.Item1?.StartsWithEx("http", "{") != true ||
                                                                     x.Item2 == default ||
                                                                     (x.Item2.Length != Crypto.Md5.HashLength &&
                                                                      x.Item2.Length != Crypto.Sha1.HashLength &&
                                                                      x.Item2.Length != Crypto.Sha256.HashLength &&
                                                                      x.Item2.Length != Crypto.Sha384.HashLength &&
                                                                      x.Item2.Length != Crypto.Sha512.HashLength &&
                                                                      !x.Item2.EqualsEx("None"))))
                return;

            // finally serialize valid data
            info.AddValue(nameof(Key), Key);
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Description), Description);
            info.AddValue(nameof(Category), Category);
            info.AddValue(nameof(Website), Website);

            info.AddValue(nameof(DisplayVersion), DisplayVersion);
            info.AddValue(nameof(PackageVersion), PackageVersion);
            info.AddValue(nameof(VersionData), VersionData);

            info.AddValue(nameof(DefaultLanguage), DefaultLanguage);
            info.AddValue(nameof(Languages), Languages);

            info.AddValue(nameof(DownloadCollection), DownloadCollection);
            info.AddValue(nameof(UpdateCollection), UpdateCollection);
            info.AddValue(nameof(DownloadSize), DownloadSize);
            info.AddValue(nameof(InstallSize), InstallSize);

            info.AddValue(nameof(Requirements), Requirements);

            info.AddValue(nameof(PackageReleaseDate), PackageReleaseDate);
            info.AddValue(nameof(PackageUpdateDate), PackageUpdateDate);
            info.AddValue(nameof(InstallerVersion), InstallerVersion);

            info.AddValue(nameof(Advanced), Advanced);
        }

        /// <summary>
        ///     Determines whether this instance have same values as the specified
        ///     <see cref="AppData"/> instance.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="AppData"/> instance to compare.
        /// </param>
        public bool Equals(AppData other) =>
            Equals(GetHashCode(), other?.GetHashCode());

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is AppData appData)
                return Equals(appData);
            return false;
        }

        /// <inheritdoc cref="Type.GetHashCode()"/>
        public override int GetHashCode()
        {
            if (Supplier == default)
                return Tuple.Create(GetType().FullName, Key.ToUpperInvariant()).GetHashCode();
            return Tuple.Create(GetType().FullName, Key.ToUpperInvariant(), Supplier).GetHashCode();
        }

        /// <inheritdoc cref="ToString(bool)"/>
        /// <param name="sb">
        ///     The <see cref="StringBuilder"/> instance being written to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     sb is null
        /// </exception>
        public void ToString(StringBuilder sb)
        {
            if (sb == default(StringBuilder))
                throw new ArgumentNullException(nameof(sb));
            const int width = 2;
            foreach (var pi in GetType().GetProperties())
            {
                var obj = pi.GetValue(this);
                switch (obj)
                {
                    case AppSettings item:
                        if (DefaultLanguage == item.ArchiveLang && !item.DisableUpdates)
                            continue;

                        sb.Append(',');
                        sb.AppendLine();
                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}:", pi.Name);
                        sb.AppendLine();

                        sb.Append(' ', width);
                        sb.AppendLine("{");

                        if (DefaultLanguage != item.ArchiveLang)
                        {
                            sb.Append(' ', width * 2);
                            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", nameof(item.ArchiveLang), item.ArchiveLang);
                            sb.AppendLine();

                            sb.Append(' ', width * 2);
                            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", nameof(item.ArchiveLangCode), item.ArchiveLangCode);
                            sb.AppendLine();

                            if (item.ArchiveLangConfirmed)
                            {
                                sb.Append(' ', width * 2);
                                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", nameof(item.ArchiveLangConfirmed), item.ArchiveLangConfirmed);
                                sb.AppendLine();
                            }
                        }

                        if (item.DisableUpdates)
                        {
                            sb.Append(' ', width * 2);
                            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", nameof(item.DisableUpdates), item.DisableUpdates);
                            sb.AppendLine();

                            if (item.DelayUpdates > DateTime.MinValue)
                            {
                                sb.Append(' ', width * 2);
                                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1:yyyy-MM-dd HH:mm:ss}'", nameof(item.DelayUpdates), item.DelayUpdates);
                                sb.AppendLine();
                            }
                        }

                        sb.Append(' ', width);
                        sb.AppendLine("}");
                        break;
                    case DataCollection item:
                        if (!item.Any())
                            continue;

                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}:", pi.Name);
                        sb.AppendLine();

                        sb.Append(' ', width);
                        sb.AppendLine("{");
                        for (var i = 0; i < item.Count; i++)
                        {
                            sb.Append(' ', width * 2);
                            if (item.Count > 10 && i < 10)
                                sb.Append(' ');
                            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", i, item[i]);
                            if (i < item.Count - 1)
                                sb.AppendLine(",");
                        }
                        sb.AppendLine();
                        sb.Append(' ', width);
                        sb.AppendLine("},");
                        break;
                    case VersionCollection item:
                        if (!item.Any())
                            continue;

                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}:", pi.Name);
                        sb.AppendLine();

                        sb.Append(' ', width);
                        sb.AppendLine("{");

                        for (var i = 0; i < item.Count; i++)
                        {
                            var (item1, item2) = item[i];

                            sb.Append(' ', width * 2);
                            if (item.Count > 10 && i < 10)
                                sb.Append(' ');
                            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}:", i);
                            sb.AppendLine();

                            sb.Append(' ', width * 2);
                            sb.AppendLine("{");

                            sb.Append(' ', width * 3);
                            sb.AppendFormat(CultureInfo.InvariantCulture, "Item1: '{0}',", item1);
                            sb.AppendLine();

                            sb.Append(' ', width * 3);
                            sb.AppendFormat(CultureInfo.InvariantCulture, "Item2: '{0}'", item2);
                            sb.AppendLine();

                            sb.Append(' ', width * 2);
                            sb.Append('}');
                            if (i < item.Count - 1)
                                sb.AppendLine(",");
                        }
                        sb.AppendLine();

                        sb.Append(' ', width);
                        sb.AppendLine("},");
                        break;
                    case DownloadCollection item:
                        if (!item.Any())
                            continue;

                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}:", pi.Name);
                        sb.AppendLine();

                        sb.Append(' ', width);
                        sb.AppendLine("{");

                        var keys = item.Keys;
                        for (var i = 0; i < keys.Count; i++)
                        {
                            var key = keys.Just(i);

                            sb.Append(' ', width * 2);
                            sb.AppendFormat(CultureInfo.InvariantCulture, "'{0}':", key);
                            if (item.TryGetValue(key, out var value) && value.Any())
                            {
                                sb.AppendLine();

                                sb.Append(' ', width * 2);
                                sb.AppendLine("{");

                                for (var j = 0; j < value.Count; j++)
                                {
                                    var (item1, item2) = value[j];

                                    sb.Append(' ', width * 3);
                                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}:", j);
                                    sb.AppendLine();

                                    sb.Append(' ', width * 3);
                                    sb.AppendLine("{");

                                    sb.Append(' ', width * 4);
                                    sb.AppendFormat(CultureInfo.InvariantCulture, "Item1: '{0}',", item1);
                                    sb.AppendLine();

                                    sb.Append(' ', width * 4);
                                    sb.AppendFormat(CultureInfo.InvariantCulture, "Item2: '{0}'", item2);
                                    sb.AppendLine();

                                    sb.Append(' ', width * 3);
                                    sb.Append('}');
                                    if (j < value.Count - 1)
                                        sb.AppendLine(",");
                                }
                                sb.AppendLine();
                            }

                            sb.Append(' ', width * 2);
                            sb.Append('}');
                            if (i < keys.Count - 1)
                                sb.AppendLine(", ");
                        }
                        sb.AppendLine();

                        sb.Append(' ', width);
                        sb.AppendLine("},");
                        break;
                    case Version item:
                        if (item.Major == 0 &&
                            item.Minor == 0 &&
                            item.Build == 0 &&
                            item.Revision == 0)
                            continue;
                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", pi.Name, item);
                        sb.AppendLine(",");
                        break;
                    case DateTime item:
                        if (item == DateTime.MinValue)
                            continue;
                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1:yyyy-MM-dd}'", pi.Name, item);
                        sb.AppendLine(",");
                        break;
                    case long item:
                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", pi.Name, item.FormatSize(SizeOption.Trim));
                        sb.AppendLine(",");
                        break;
                    default:
                        if (obj == null || pi.Name == nameof(Supplier))
                            continue;
                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", pi.Name, obj);
                        if (pi.Name != nameof(Advanced))
                            sb.AppendLine(",");
                        break;
                }
            }
        }

        /// <summary>
        ///     Converts the values of this instance to a string.
        /// </summary>
        /// <param name="formatted">
        ///     <see langword="true"/> to format the result; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <returns>
        ///     A string representation of this instance.
        /// </returns>
        public string ToString(bool formatted)
        {
            if (!formatted)
                return Json.Serialize(this);
            var sb = new StringBuilder();
            ToString(sb);
            return sb.ToString();
        }

        /// <inheritdoc cref="ToString(bool)"/>
        public override string ToString() =>
            ToString(false);

        /// <summary>
        ///     Determines whether two specified <see cref="AppData"/> instances have same
        ///     values.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="AppData"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="AppData"/> instance to compare.
        /// </param>
        public static bool operator ==(AppData left, AppData right) =>
            Equals(left, right);

        /// <summary>
        ///     Determines whether two specified <see cref="AppData"/> instances have
        ///     different values.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="AppData"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="AppData"/> instance to compare.
        /// </param>
        public static bool operator !=(AppData left, AppData right) =>
            !Equals(left, right);
    }
}
