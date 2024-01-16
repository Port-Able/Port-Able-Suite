namespace PortAble;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using Model;
using Newtonsoft.Json;
using SilDev;

/// <summary>
///     Provides local app settings.
/// </summary>
[Serializable]
public sealed class AppSettings : IObjectFile<AppSettings>
{
    [NonSerialized]
    private string _filePath;

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
    [JsonIgnore]
    public int ArchiveLangCode => GetLangCode(ArchiveLang);

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
    [JsonIgnore]
    public DateTime InstanceTime { get; private set; }

    /// <inheritdoc cref="IObjectFile.FilePath"/>
    [JsonIgnore]
    public string FilePath => _filePath ??= Path.Combine(CorePaths.AppSettingsDir, $"{Key}.json");

    /// <summary>
    ///     Initialize the <see cref="AppSettings"/> class.
    /// </summary>
    /// <param name="parent">
    ///     The <see cref="IAppData"/> instance.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     parent is null.
    /// </exception>
    public AppSettings(IAppData parent) : this(parent.Key, parent.DefaultLanguage, true) { }

    private AppSettings(string key, string language, bool loadFromFile)
    {
        Key = key;
        SetDefaults(language);
        if (loadFromFile)
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
        SetDefaults();

        // Only set properties that are also present in
        // the configuration file.
        var enumerator = info.GetEnumerator();
        while (enumerator.MoveNext())
            switch (enumerator.Name)
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

    /// <inheritdoc cref="ISerializable.GetObjectData(SerializationInfo, StreamingContext)"/>
    [SecurityCritical]
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        // Do not serialize default values.
        foreach (var pi in GetType().GetPropertiesEx(typeof(JsonIgnoreAttribute)).Where(pi => !IsDefault(pi)))
            info.AddValue(pi.Name, pi.GetValue(this));
    }

    /// <summary>
    ///     Determines whether this instance have same values as the specified
    ///     <see cref="AppSettings"/> instance.
    ///     <para>
    ///         &#9888; Not consistent with <see cref="GetHashCode"/>, which is
    ///         intentional.
    ///     </para>
    /// </summary>
    /// <param name="other">
    ///     The <see cref="AppSettings"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the values of the specified
    ///     <see cref="AppSettings"/> instance are equal to the values of the current
    ///     <see cref="AppSettings"/> instance; otherwise <see langword="false"/>.
    /// </returns>
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
        ArchiveLangConfirmed == other.ArchiveLangConfirmed &&
        DisableUpdates == other.DisableUpdates &&
        DelayUpdates == other.DelayUpdates &&
        FileTypes?.Count == other.FileTypes?.Count &&
        FileTypes.SequenceEqualEx(other.FileTypes);

    /// <inheritdoc cref="Equals(AppSettings)"/>
    public override bool Equals(object other)
    {
        if (other is AppSettings instance)
            return Equals(instance);
        return false;
    }

    /// <inheritdoc cref="Type.GetHashCode()"/>
    public override int GetHashCode() =>
        (typeof(AppSettings).FullName, Key).GetHashCode();

    /// <see cref="IObjectFile.ToString(bool)"/>
    public string ToString(bool formatted) =>
        JsonConvert.SerializeObject(this, formatted ? Formatting.Indented : Formatting.None);

    /// <inheritdoc cref="IObjectFile.ToString()"/>
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
    ///     Determines whether the JSON file exists in the
    ///     <see cref="CorePaths.AppSettingsDir"/> folder.
    /// </summary>
    public bool FileExists() =>
        FileEx.Exists(FilePath);

    /// <summary>
    ///     Load the data from a JSON file stored in the
    ///     <see cref="CorePaths.AppSettingsDir"/> folder if newer than this instance.
    /// </summary>
    /// <param name="force">
    ///     Determines whether to force an update from the file.
    /// </param>
    public void LoadFromFile(bool force = false)
    {
        if (!FileExists())
        {
            InstanceTime = DateTime.MinValue;
            return;
        }
        if (!force && File.GetLastWriteTime(FilePath) < InstanceTime)
            return;
        var item = CacheData.LoadJson<AppSettings>(FilePath);
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
        if (!DirectoryEx.CreateParent(FilePath))
            throw new PathNotFoundException(CorePaths.AppSettingsDir);
        if (!force)
        {
            var item = CacheData.LoadJson<AppSettings>(FilePath) ?? new AppSettings(Key, null, false);
            if (this == item)
                return;
        }
        CacheData.SaveJson(this, FilePath);
        InstanceTime = DateTime.Now;
    }

    /// <summary>
    ///     Removes the JSON file from the <see cref="CorePaths.AppSettingsDir"/>
    ///     directory.
    /// </summary>
    public void RemoveFile() =>
        FileEx.TryDelete(FilePath);

    private static int GetLangCode(string langName)
    {
        if (string.IsNullOrWhiteSpace(langName))
            return 1033;
        switch (langName)
        {
            case "Default":
            case "Multilingual":
                return 1033;
            case "SpanishInternational":
                return 3082;
            default:
                return CultureConfig.GetCultureInfo(langName).TextInfo.LCID;
        }
    }

    private void SetDefaults(string archiveLang = default)
    {
        RunAsAdmin = false;
        FileTypes = null;
        StartArgsDef = null;
        StartArgsMod = null;
        StartArgsDoSort = false;
        StartArgsForceQuotes = true;
        StartArgsValidPaths = true;
        ArchiveLang = archiveLang ?? "Default";
        ArchiveLangConfirmed = false;
        DisableUpdates = false;
        DelayUpdates = DateTime.MinValue;
    }

    private bool IsDefault(PropertyInfo pi) =>
        pi?.Name switch
        {
            nameof(Key) => false,
            nameof(RunAsAdmin) => !RunAsAdmin,
            nameof(FileTypes) => FileTypes?.Count is null or < 1,
            nameof(StartArgsDef) => StartArgsDef == null,
            nameof(StartArgsMod) => StartArgsMod == null,
            nameof(StartArgsDoSort) => !StartArgsDoSort,
            nameof(StartArgsForceQuotes) => StartArgsForceQuotes,
            nameof(StartArgsValidPaths) => StartArgsValidPaths,
            nameof(ArchiveLang) => ArchiveLang is "Default" or "English" or "Multilingual",
            nameof(ArchiveLangConfirmed) => !ArchiveLangConfirmed,
            nameof(DisableUpdates) => !DisableUpdates,
            nameof(DelayUpdates) => DelayUpdates == DateTime.MinValue,
            _ => true
        };

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
    /// <inheritdoc cref="operator ==(AppSettings, AppSettings)"/>
    public static bool operator !=(AppSettings left, AppSettings right) =>
        !Equals(left, right);
}