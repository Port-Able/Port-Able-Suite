namespace PortAble
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text;
    using System.Web.Script.Serialization;
    using SilDev;

    /// <summary>
    ///     Provides local app settings.
    /// </summary>
    [Serializable]
    public sealed class AppSettings : ISerializable, IEquatable<AppSettings>
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
        ///     The command line arguments that are always used when starting the app.
        /// </summary>
        public string StartArgsDef { get; set; }

        /// <summary>
        ///     The command line arguments that are added before and after the command line
        ///     arguments that have already been sent.
        ///     <para>
        ///         Please note that this feature is only used when starting the app with
        ///         command line arguments. In this case, `Item1` is added before and
        ///         `Item2` after the current arguments.
        ///     </para>
        /// </summary>
        public Tuple<string, string> StartArgsMod { get; set; }

        /// <summary>
        ///     Determines whether the command line arguments should be sorted
        ///     alphabetically, which can be very useful for file paths, for example, to
        ///     ensure the correct order in the app.
        /// </summary>
        public bool StartArgsDoSort { get; set; }

        /// <summary>
        ///     Determines whether to ensure that all command line arguments are enclosed
        ///     in double quotes when calling <see cref="GetStartArgsStr"/> method.
        /// </summary>
        public bool StartArgsForceQuotes { get; set; }

        /// <summary>
        ///     Determines whether to ensure that all command line arguments are valid
        ///     paths when calling <see cref="GetStartArgs"/> and
        ///     <see cref="GetStartArgsStr"/> methods.
        /// </summary>
        public bool StartArgsValidPaths { get; set; }

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
        ///     Skip update search for the app.
        /// </summary>
        public bool DisableUpdates { get; set; }

        /// <summary>
        ///     Skip the update search for the app until the defined date.
        /// </summary>
        public DateTime DelayUpdates { get; set; }

        /// <summary>
        ///     The exact time when this instance was created, updated via the
        ///     <see cref="LoadFromFile"/> method, or when the <see cref="SaveToFile"/>
        ///     method was called. So this <see cref="DateTime"/> is used to synchronize
        ///     with the file on the hard drive.
        /// </summary>
        [ScriptIgnore]
        public DateTime InstanceTime { get; private set; }

        /// <summary>
        ///     Initialize the <see cref="AppSettings"/> class.
        /// </summary>
        /// <param name="parent">
        ///     The <see cref="AppData"/> instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     parent is null.
        /// </exception>
        public AppSettings(AppData parent)
        {
            Key = parent.Key;
            SetDefaults(parent.DefaultLanguage);
            LoadFromFile(true);
        }

        private AppSettings(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            // The key cannot be missing.
            Key = info.GetString(nameof(Key));

            // Set default values in case some properties have
            // been deleted from the configuration file.
            SetDefaults(default);

            // Only set properties that are also present in
            // the configuration file.
            foreach (var ent in info)
                switch (ent.Name)
                {
                    case nameof(RunAsAdmin):
                        RunAsAdmin = info.GetBoolean(nameof(RunAsAdmin));
                        break;
                    case nameof(FileTypes):
                        FileTypes = (Collection<string>)info.GetValue(nameof(FileTypes), typeof(Collection<string>));
                        break;
                    case nameof(StartArgsDef):
                        StartArgsDef = info.GetString(nameof(StartArgsDef));
                        break;
                    case nameof(StartArgsMod):
                        StartArgsMod = (Tuple<string, string>)info.GetValue(nameof(StartArgsMod), typeof(Tuple<string, string>));
                        break;
                    case nameof(StartArgsDoSort):
                        StartArgsDoSort = info.GetBoolean(nameof(StartArgsDoSort));
                        break;
                    case nameof(StartArgsForceQuotes):
                        StartArgsForceQuotes = info.GetBoolean(nameof(StartArgsForceQuotes));
                        break;
                    case nameof(StartArgsValidPaths):
                        StartArgsValidPaths = info.GetBoolean(nameof(StartArgsValidPaths));
                        break;
                    case nameof(ArchiveLang):
                        ArchiveLang = info.GetString(nameof(ArchiveLang));
                        break;
                    case nameof(ArchiveLangCode):
                        ArchiveLangCode = info.GetInt32(nameof(ArchiveLangCode));
                        break;
                    case nameof(ArchiveLangConfirmed):
                        ArchiveLangConfirmed = info.GetBoolean(nameof(ArchiveLangConfirmed));
                        break;
                    case nameof(DisableUpdates):
                        DisableUpdates = info.GetBoolean(nameof(DisableUpdates));
                        break;
                    case nameof(DelayUpdates):
                        DelayUpdates = info.GetDateTime(nameof(DelayUpdates));
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

            info.AddValue(nameof(Key), Key);
            info.AddValue(nameof(RunAsAdmin), RunAsAdmin);
            info.AddValue(nameof(FileTypes), FileTypes);
            info.AddValue(nameof(StartArgsDef), StartArgsDef);
            info.AddValue(nameof(StartArgsMod), StartArgsMod);
            info.AddValue(nameof(StartArgsDoSort), StartArgsDoSort);
            info.AddValue(nameof(StartArgsForceQuotes), StartArgsForceQuotes);
            info.AddValue(nameof(StartArgsValidPaths), StartArgsValidPaths);
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
        public bool Equals(AppSettings other) =>
            other != null &&
            Key == other.Key &&
            RunAsAdmin == other.RunAsAdmin &&
            StartArgsDef == other.StartArgsDef &&
            StartArgsMod?.Item1 == other.StartArgsMod?.Item1 &&
            StartArgsMod?.Item2 == other.StartArgsMod?.Item2 &&
            StartArgsDoSort == other.StartArgsDoSort &&
            StartArgsForceQuotes == other.StartArgsForceQuotes &&
            StartArgsValidPaths == other.StartArgsValidPaths &&
            ArchiveLang == other.ArchiveLang &&
            ArchiveLangCode == other.ArchiveLangCode &&
            ArchiveLangConfirmed == other.ArchiveLangConfirmed &&
            DisableUpdates == other.DisableUpdates &&
            DelayUpdates == other.DelayUpdates &&
            (FileTypes?.SequenceEqual(other.FileTypes) ?? other.FileTypes == default);

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
                    case Collection<string> item:
                    {
                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", pi.Name, item);
                        break;
                    }
                    default:
                    {
                        if (obj == null)
                            continue;
                        var value = obj;
                        if (value is long num)
                            value = num.FormatSize(SizeOption.Trim);
                        sb.Append(' ', width);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: '{1}'", pi.Name, value);
                        if (pi.Name != nameof(InstanceTime))
                            sb.AppendLine(",");
                        break;
                    }
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
        ///     Gets the filtered command line arguments using
        ///     <see cref="Environment.GetCommandLineArgs"/> method and the
        ///     <see cref="StartArgsDef"/>, <see cref="StartArgsMod"/>,
        ///     <see cref="StartArgsDoSort"/> and <see cref="StartArgsValidPaths"/>
        ///     properties.
        /// </summary>
        /// <returns>
        ///     A sequence of strings representing command line arguments used to start the
        ///     app.
        /// </returns>
        public IEnumerable<string> GetStartArgs()
        {
            var addBefore = !string.IsNullOrWhiteSpace(StartArgsMod?.Item1);
            var addAfter = !string.IsNullOrWhiteSpace(StartArgsMod?.Item2);
            switch (Environment.GetCommandLineArgs().Length)
            {
                case < 2:
                    yield return StartArgsDef ?? string.Empty;
                    yield break;
                case < 3:
                    if (addBefore)
                        yield return StartArgsMod.Item1;
                    yield return Environment.GetCommandLineArgs().Second();
                    if (addAfter)
                        yield return StartArgsMod.Item2;
                    yield break;
            }
            var args = Environment.GetCommandLineArgs().Skip(1);
            if (StartArgsValidPaths)
                args = args.Where(PathEx.IsValidPath);
            if (StartArgsDoSort)
            {
                var comparer = new AlphaNumericComparer<string>();
                args = args.OrderBy(x => x, comparer);
            }
            if (addBefore)
                yield return StartArgsMod.Item1;
            foreach (var arg in args)
                yield return arg;
            if (addAfter)
                yield return StartArgsMod.Item2;
        }

        /// <summary>
        ///     Gets the filtered command line arguments using the
        ///     <see cref="GetStartArgs"/> method and the
        ///     <see cref="StartArgsForceQuotes"/> property, which are converted to a
        ///     string.
        ///     <para>
        ///         See also <seealso cref="GetStartArgs"/>.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     A string representing command line arguments used to start the app.
        /// </returns>
        public string GetStartArgsStr()
        {
            var args = GetStartArgs().ToArray();
            return args.Length switch
            {
                < 1 => string.Empty,
                < 2 => StartArgsForceQuotes ? $"\"{args.First()}\"" : args.First(),
                _ => StartArgsForceQuotes ? $"\"{args.Join("\" \"")}\"" : args.Join(" ")
            };
        }

        /// <summary>
        ///     Load the data from a JSON file stored in the
        ///     <see cref="CorePaths.AppSettingsDir"/> folder if newer than this instance.
        /// </summary>
        /// <param name="force">
        ///     Determines whether to force an update from the file.
        /// </param>
        public void LoadFromFile(bool force = false)
        {
            var path = PathEx.Combine(CorePaths.AppSettingsDir, $"{Key}.json");
            if (!FileEx.Exists(path))
            {
                InstanceTime = DateTime.MinValue;
                return;
            }
            if (!force && File.GetLastWriteTime(path) < InstanceTime)
                return;
            var item = CacheData.LoadJson<AppSettings>(path);
            if (item == default)
                return;
            RunAsAdmin = item.RunAsAdmin;
            FileTypes = item.FileTypes;
            StartArgsDef = item.StartArgsDef;
            StartArgsMod = item.StartArgsMod;
            StartArgsDoSort = item.StartArgsDoSort;
            StartArgsForceQuotes = item.StartArgsForceQuotes;
            StartArgsValidPaths = item.StartArgsValidPaths;
            ArchiveLang = item.ArchiveLang;
            ArchiveLangCode = item.ArchiveLangCode;
            ArchiveLangConfirmed = item.ArchiveLangConfirmed;
            DisableUpdates = item.DisableUpdates;
            DelayUpdates = item.DelayUpdates;
            InstanceTime = DateTime.Now;
        }

        /// <summary>
        ///     Saves the data into a JSON file stored in the
        ///     <see cref="CorePaths.AppSettingsDir"/> directory if newer than the file.
        /// </summary>
        /// <param name="force">
        ///     Determines whether to force an update to the file.
        /// </param>
        public void SaveToFile(bool force = false)
        {
            var path = PathEx.Combine(CorePaths.AppSettingsDir, $"{Key}.json");
            if (!DirectoryEx.Create(CorePaths.AppSettingsDir))
                throw new PathNotFoundException(CorePaths.AppSettingsDir);
            if (!force)
            {
                var item = CacheData.LoadJson<AppSettings>(path);
                if (this == item)
                    return;
            }
            CacheData.SaveJson(this, path);
            InstanceTime = DateTime.Now;
        }

        private void SetDefaults(string archiveLang)
        {
            RunAsAdmin = default;
            FileTypes = new();
            StartArgsDef = default;
            StartArgsMod = default;
            StartArgsDoSort = default;
            StartArgsForceQuotes = true;
            StartArgsValidPaths = true;
            ArchiveLang = archiveLang ?? "Default";
            ArchiveLangCode = 1033;
            ArchiveLangConfirmed = default;
            DisableUpdates = default;
            DelayUpdates = DateTime.MinValue;
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
