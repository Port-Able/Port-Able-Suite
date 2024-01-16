namespace PortAble;

using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Windows.Forms;
using Model;
using Newtonsoft.Json;
using Properties;
using SilDev;
using SilDev.Forms;
using SilDev.Ini.Legacy;

/// <summary>
///     Provides app information of installed apps.
/// </summary>
[Serializable]
public sealed class LocalAppData : IAppData, ISerializable, IEquatable<LocalAppData>
{
    [NonSerialized]
    private AppSettings _settings;

    /// <inheritdoc cref="IAppData.Key"/>
    public string Key { get; }

    /// <inheritdoc cref="IAppData.Name"/>
    public string Name { get; }

    /// <inheritdoc cref="IAppData.Description"/>
    public string Description { get; }

    /// <inheritdoc cref="IAppData.Category"/>
    public string Category { get; }

    /// <inheritdoc cref="IAppData.DefaultLanguage"/>
    public string DefaultLanguage { get; }

    /// <summary>
    ///     The app install location.
    /// </summary>
    public string InstallDir { get; }

    /// <summary>
    ///     The file path to the app`s EXE file.
    /// </summary>
    public string ExecutablePath { get; }

    /// <summary>
    ///     The file path to the app`s INI file.
    /// </summary>
    public string ConfigPath { get; }

    /// <summary>
    ///     The file path to the AppInfo INI file.
    /// </summary>
    public string AppInfoPath { get; }

    /// <inheritdoc cref="IAppData.Settings"/>
    [JsonIgnore]
    public AppSettings Settings => _settings ??= new AppSettings(this);

    /// <summary>
    ///     Creates a new instance of <see cref="LocalAppData"/>.
    /// </summary>
    /// <param name="key">
    ///     The app key.
    /// </param>
    /// <param name="name">
    ///     The app name.
    /// </param>
    /// <param name="description">
    ///     The app description.
    /// </param>
    /// <param name="category">
    ///     The app category.
    /// </param>
    /// <param name="installDir">
    ///     The app install location.
    /// </param>
    /// <param name="executablePath">
    ///     The file path to the app`s EXE file.
    /// </param>
    /// <param name="configPath">
    ///     The file path to the app`s INI file.
    /// </param>
    /// <param name="appInfoPath">
    ///     The file path to the AppInfo INI file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Any parameter is null or empty.
    /// </exception>
    public LocalAppData(string key, string name, string description, string category, string installDir, string executablePath, string configPath, string appInfoPath)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (string.IsNullOrWhiteSpace(installDir))
            throw new ArgumentNullException(nameof(installDir));

        if (string.IsNullOrWhiteSpace(executablePath))
            throw new ArgumentNullException(nameof(executablePath));

        if (string.IsNullOrWhiteSpace(configPath))
            throw new ArgumentNullException(nameof(configPath));

        if (string.IsNullOrWhiteSpace(appInfoPath))
            throw new ArgumentNullException(nameof(appInfoPath));

        Key = key;
        Name = name;

        Description = description;
        Category = category;

        DefaultLanguage = null;

        InstallDir = GetFullPath(installDir);
        ExecutablePath = GetFullPath(executablePath);

        ConfigPath = GetFullPath(configPath);
        AppInfoPath = GetFullPath(appInfoPath);
    }

    private LocalAppData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        Key = info.GetString(nameof(Key));
        Name = info.GetString(nameof(Name));

        Description = info.GetString(nameof(Description));
        Category = info.GetString(nameof(Category));

        InstallDir = GetFullPath(info.GetString(nameof(InstallDir)));
        ExecutablePath = GetFullPath(info.GetString(nameof(ExecutablePath)));

        ConfigPath = GetFullPath(info.GetString(nameof(ConfigPath)));
        AppInfoPath = GetFullPath(info.GetString(nameof(AppInfoPath)));
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
        info.AddValue(nameof(Name), Name);

        info.AddValue(nameof(Description), Description);
        info.AddValue(nameof(Category), Category);

        info.AddValue(nameof(InstallDir), GetSubPath(InstallDir));
        info.AddValue(nameof(ExecutablePath), GetSubPath(ExecutablePath));

        info.AddValue(nameof(ConfigPath), GetSubPath(ConfigPath));
        info.AddValue(nameof(AppInfoPath), GetSubPath(AppInfoPath));
    }

    public void StartApplication(bool closeLauncher = false, bool runAsAdmin = false)
    {
        if (!runAsAdmin)
            runAsAdmin = Settings.RunAsAdmin;

        /*
        var srcConfigPath = Path.Combine(InstallDir, "Other", "Source", "AppNamePortable.ini");
        if (!File.Exists(srcConfigPath))
        {
            var configName = Path.GetFileName(ConfigPath);
            if (!string.IsNullOrEmpty(configName))
                srcConfigPath = Path.Combine(InstallDir, "Other", "Source", configName);
        }
        if (File.Exists(srcConfigPath))
            FileEx.Copy(srcConfigPath, ConfigPath);

        foreach (var file in DirectoryEx.EnumerateFiles(InstallDir, "*.ini"))
        {
            var content = FileEx.ReadAllText(file);
            if (!Regex.IsMatch(content, "DisableSplashScreen.*=.*false", RegexOptions.IgnoreCase))
                continue;
            content = Regex.Replace(content, "DisableSplashScreen.*=.*false", "DisableSplashScreen=true", RegexOptions.IgnoreCase);
            FileEx.WriteAllText(file, content);
        }
        */

        var arguments = $"{Settings.StartArgsDef}{Ini.Read("AppInfo", "Arg", ConfigPath)}";
        if (Arguments.ValidPaths.Count > 0)
        {
            if (Settings.StartArgsDoSort)
            {
                var comparer = new AlphaNumericComparer<string>();
                Arguments.ValidPaths = Arguments.ValidPaths.OrderBy(x => x, comparer).ToList();
            }
            if (string.IsNullOrWhiteSpace(arguments))
                arguments = $"{Settings.StartArgsMod?.Item1}{Arguments.ValidPathsStr}{Settings.StartArgsMod?.Item2}".Trim();
            /*
            var curKey = Arguments.ValidPathsStr?.GetHashCode() ?? -1;
            var newValue = Key.GetHashCode();
            CacheData.UpdateCurrentTypeDataFile(curKey, newValue);
            GlobalSettings.LastItem = Name;
            */
        }

        /*
        if (Arguments.FileTypes.Any())
        {
            var types = Arguments.FileTypes.Where(x => !Arguments.SavedFileTypes.Contains(x)).ToList();
            if (types.Any())
            {
                var question = types.Count == 1 ? Language.GetText(nameof(en_US.AssociateQuestionMsg0)) : string.Format(CultureInfo.InvariantCulture, Language.GetText(nameof(en_US.AssociateQuestionMsg1)), types.Join("; "));
                if (MessageBoxEx.Show(question, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    types = types.Select(x => $".{x.ToLowerInvariant()}").ToList();
                    Settings.FileTypes.ForEach(x =>
                    {
                        if (types.Any(y => y.TrimStart('.').EqualsEx(x.TrimStart('.'))))
                            return;
                        types.Add(x);
                    });
                    Settings.FileTypes = new Collection<string>(types);
                }
            }
        }
        */

        if (File.Exists(AppInfoPath)) 
        {
            Environment.SetEnvironmentVariable("PortableApps.comRoot", CorePaths.HomeDir);
            Environment.SetEnvironmentVariable("PortableApps.comApps", CorePaths.AppsDir);
            Environment.SetEnvironmentVariable("PortableApps.comDocuments", CorePaths.UserDocumentsDir);
            Environment.SetEnvironmentVariable("PortableApps.comPictures", CorePaths.UserPicturesDir);
            Environment.SetEnvironmentVariable("PortableApps.comMusic", CorePaths.UserMusicDir);
            Environment.SetEnvironmentVariable("PortableApps.comVideos", CorePaths.UserVideosDir);
            Environment.SetEnvironmentVariable("PortableApps.comPlatformVersion", "99.99.99");
        }

        ProcessEx.Start(ExecutablePath, arguments, runAsAdmin);

        if (closeLauncher)
            Application.Exit();
    }

    /// <summary>
    ///     Determines whether the specified <see cref="LocalAppData"/> instance is
    ///     equal to the current <see cref="LocalAppData"/> instance.
    /// </summary>
    /// <param name="other">
    ///     The <see cref="LocalAppData"/> instance to compare with the current
    ///     <see cref="LocalAppData"/> instance.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="LocalAppData"/> instance
    ///     is equal to the current <see cref="LocalAppData"/> instance; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    public bool Equals(LocalAppData other) =>
        Key.EqualsEx(other?.Key) &&
        Name == other?.Name &&
        Description == other?.Description &&
        Category == other?.Category &&
        InstallDir == other?.InstallDir &&
        ExecutablePath == other?.ExecutablePath &&
        ConfigPath == other?.ConfigPath &&
        AppInfoPath == other?.AppInfoPath;

    /// <inheritdoc cref="Equals(LocalAppData)"/>
    public override bool Equals(object other)
    {
        if (other is LocalAppData instance)
            return Equals(instance);
        return false;
    }

    /// <inheritdoc cref="Type.GetHashCode()"/>
    public override int GetHashCode() => (
        Key.ToUpperInvariant(),
        Name,
        Description,
        Category,
        InstallDir,
        ExecutablePath,
        ConfigPath,
        AppInfoPath
    ).GetHashCode();

    public void OpenLocation(bool closeLauncher = false)
    {
        ProcessEx.Start("%WinDir%\\explorer.exe", InstallDir);
        if (closeLauncher)
            Application.Exit();
    }

    public bool RemoveApplication(IWin32Window owner = default)
    {
        CacheData.ResetCurrentAppInfo();
        while (true)
        {
            if (Settings.FileExists() && MessageBoxEx.Show(owner, LangStrings.RemoveAppSettingsQuestionMsg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Settings.RemoveFile();
            if (CacheData.AppDirs.Any(x => InstallDir.StartsWithEx(x)) && Directory.Exists(InstallDir))
                if (DirectoryEx.TryDelete(InstallDir) || PathEx.ForceDelete(InstallDir) || PathEx.ForceDelete(InstallDir, true))
                {
                    MessageBoxEx.Show(owner, LangStrings.OperationCompletedMsg, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return !Directory.Exists(InstallDir);
                }
            if (MessageBoxEx.Show(owner, LangStrings.OperationFailedMsg, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                continue;
            return false;
        }
    }

    private static string GetSubPath(string path) =>
        path.StartsWithEx(PathEx.LocalDir) ? path.Substring(PathEx.LocalDir.Length).Trim(Path.DirectorySeparatorChar) : path;

    private static string GetFullPath(string path) =>
        !PathEx.IsValidPath(path) ? PathEx.Combine(PathEx.LocalDir, path) : path;

    /// <summary>
    ///     Determines whether two specified <see cref="LocalAppData"/> instances have
    ///     same values.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="LocalAppData"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="LocalAppData"/> instance to compare.
    /// </param>
    public static bool operator ==(LocalAppData left, LocalAppData right) =>
        Equals(left, right);

    /// <summary>
    ///     Determines whether two specified <see cref="LocalAppData"/> instances have
    ///     different values.
    /// </summary>
    /// <inheritdoc cref="operator ==(LocalAppData, LocalAppData)"/>
    public static bool operator !=(LocalAppData left, LocalAppData right) =>
        !Equals(left, right);
}