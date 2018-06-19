namespace AppsLauncher.Libraries
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using LangResources;
    using SilDev;
    using SilDev.Forms;
    using GlobalSettings = Settings;

    [Serializable]
    public class LocalAppData : ISerializable
    {
        private string _fileDir, _filePath, _configPath, _appInfoPath;

        [NonSerialized]
        private AppSettings _settings;

        public LocalAppData(string key, string name, string fileDir, string filePath, string configPath, string appInfoPath)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(fileDir))
                throw new ArgumentNullException(nameof(fileDir));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentNullException(nameof(configPath));

            if (string.IsNullOrWhiteSpace(appInfoPath))
                throw new ArgumentNullException(nameof(appInfoPath));

            Key = key;
            Name = name;

            FileDir = fileDir;
            FilePath = filePath;

            ConfigPath = configPath;
            AppInfoPath = appInfoPath;
        }

        protected LocalAppData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            Key = info.GetString(nameof(Key));
            Name = info.GetString(nameof(Name));

            FileDir = info.GetString(nameof(FileDir));
            FilePath = info.GetString(nameof(FilePath));

            ConfigPath = info.GetString(nameof(ConfigPath));
            AppInfoPath = info.GetString(nameof(AppInfoPath));
        }

        public string Key { get; }

        public string Name { get; }

        public string FileDir
        {
            get => _fileDir;
            private set => _fileDir = GetFullPath(value);
        }

        public string FilePath
        {
            get => _filePath;
            private set => _filePath = GetFullPath(value);
        }

        public string ConfigPath
        {
            get => _configPath;
            private set => _configPath = GetFullPath(value);
        }

        public string AppInfoPath
        {
            get => _appInfoPath;
            private set => _appInfoPath = GetFullPath(value);
        }

        public AppSettings Settings
        {
            get
            {
                if (_settings == default(AppSettings))
                    _settings = new AppSettings(Key);
                return _settings;
            }
        }

        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Key), Key);
            info.AddValue(nameof(Name), Name);

            info.AddValue(nameof(FileDir), GetSubPath(FileDir));
            info.AddValue(nameof(FilePath), GetSubPath(FilePath));

            info.AddValue(nameof(ConfigPath), GetSubPath(ConfigPath));
            info.AddValue(nameof(AppInfoPath), GetSubPath(AppInfoPath));
        }

        internal void OpenLocation(bool closeLauncher = false)
        {
            ProcessEx.Start(CorePaths.SystemExplorer, FileDir);
            if (closeLauncher)
                Application.Exit();
        }

        public void StartApplication(bool closeLauncher = false, bool runAsAdmin = false)
        {
            if (!runAsAdmin)
                runAsAdmin = Settings.RunAsAdmin;

            var srcConfigPath = Path.Combine(FileDir, "Other", "Source", "AppNamePortable.ini");
            if (!File.Exists(srcConfigPath))
            {
                var configName = Path.GetFileName(ConfigPath);
                if (!string.IsNullOrEmpty(configName))
                    srcConfigPath = Path.Combine(FileDir, "Other", "Source", configName);
            }
            if (File.Exists(srcConfigPath))
                FileEx.Copy(srcConfigPath, ConfigPath);

            foreach (var file in DirectoryEx.EnumerateFiles(FileDir, "*.ini"))
            {
                var content = FileEx.ReadAllText(file);
                if (!Regex.IsMatch(content, "DisableSplashScreen.*=.*false", RegexOptions.IgnoreCase))
                    continue;
                content = Regex.Replace(content, "DisableSplashScreen.*=.*false", "DisableSplashScreen=true", RegexOptions.IgnoreCase);
                FileEx.WriteAllText(file, content);
            }

            var arguments = Ini.Read("AppInfo", "Arg", ConfigPath);
            if (Arguments.ValidPaths.Any())
            {
                if (string.IsNullOrWhiteSpace(arguments))
                    arguments = $"{Settings.StartArgsFirst}{Arguments.ValidPathsStr}{Settings.StartArgsLast}".Trim();
                var curKey = Arguments.ValidPathsStr?.GetHashCode() ?? -1;
                var newValue = Key.GetHashCode();
                CacheData.UpdateCurrentTypeDataFile(curKey, newValue);
                GlobalSettings.LastItem = Name;
            }

            if (Arguments.FileTypes.Any())
            {
                var types = Arguments.FileTypes.Where(x => !Arguments.SavedFileTypes.Contains(x)).ToList();
                if (types.Any())
                {
                    var question = types.Count == 1 ? Language.GetText(nameof(en_US.associateQuestionMsg0)) : string.Format(Language.GetText(nameof(en_US.associateQuestionMsg1)), $"{types.Join("; ")}");
                    if (MessageBoxEx.Show(question, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        types = types.Select(x => $".{x.ToLower()}").ToList();
                        Settings.FileTypes.ForEach(x =>
                        {
                            if (types.Any(y => y.TrimStart('.').EqualsEx(x.TrimStart('.'))))
                                return;
                            types.Add(x);
                        });
                        Settings.FileTypes = types.ToArray();
                    }
                }
            }

            ProcessEx.Start(FilePath, arguments, runAsAdmin);
            if (closeLauncher)
                Application.Exit();
        }

        internal bool RemoveApplication(IWin32Window owner = default(IWin32Window))
        {
            Retry:
            if (GlobalSettings.AppDirs.Any(x => FileDir.StartsWithEx(x)) && Directory.Exists(FileDir))
            {
                if (!DirectoryEx.TryDelete(FileDir) && !PathEx.ForceDelete(FileDir) && !PathEx.ForceDelete(FileDir, true))
                    goto Failed;
                CacheData.ResetCurrent();
                if (Ini.GetSections().ContainsEx(Key))
                {
                    Ini.RemoveSection(Key);
                    GlobalSettings.WriteToFileInQueue = true;
                }
                MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.OperationCompletedMsg)), GlobalSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return !Directory.Exists(FileDir);
            }
            Failed:
            if (MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.OperationFailedMsg)), GlobalSettings.Title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                goto Retry;
            return false;
        }

        public bool Equals(LocalAppData appData) =>
            Equals(GetHashCode(), appData.GetHashCode());

        public override bool Equals(object obj)
        {
            if (obj is LocalAppData appData)
                return Equals(appData);
            return false;
        }

        public override int GetHashCode() =>
            Tuple.Create(Key, Name, FileDir, FilePath, ConfigPath, AppInfoPath).GetHashCode();

        public static bool operator ==(LocalAppData left, LocalAppData right) =>
            Equals(left, right);

        public static bool operator !=(LocalAppData left, LocalAppData right) =>
            !Equals(left, right);

        private static string GetSubPath(string path) =>
            path.StartsWithEx(PathEx.LocalDir) ? path.Substring(PathEx.LocalDir.Length).Trim(Path.DirectorySeparatorChar) : path;

        private static string GetFullPath(string path) =>
            !PathEx.IsValidPath(path) ? PathEx.Combine(PathEx.LocalDir, path) : path;

        public sealed class AppSettings
        {
            private readonly string _section;
            private string[] _fileAboluteTypes, _fileTypes;
            private FileTypeAssocData _fileTypeAssoc;
            private bool? _noConfirm, _noUpdates, _runAsAdmin;
            private DateTime _noUpdatesTime;
            private string _startArgsFirst, _startArgsLast;

            internal AppSettings(string key) =>
                _section = key;

            public string[] FileAboluteTypes
            {
                get
                {
                    if (FileTypes.Length == _fileAboluteTypes?.Length)
                        return _fileAboluteTypes;
                    _fileAboluteTypes = FileTypes;
                    if (_fileAboluteTypes.Any())
                        _fileAboluteTypes = FileTypes.Select(x => x.TrimStart('.')).Distinct().ToArray();
                    return _fileAboluteTypes;
                }
            }

            public string[] FileTypes
            {
                get
                {
                    if (_fileTypes != default(string[]))
                        return _fileTypes;
                    var types = Ini.Read<string>(_section, nameof(FileTypes));
                    if (types != default(string))
                        _fileTypes = types.Contains(',') ? types.Split(',') : new[] { types };
                    if (_fileTypes == default(string[]))
                        _fileTypes = Array.Empty<string>();
                    return _fileTypes;
                }
                set
                {
                    if (_fileTypes == value)
                        return;
                    _fileTypes = value;
                    _fileAboluteTypes = default(string[]);
                    if (_fileTypes?.Any() != true)
                        WriteValue(nameof(FileTypes), default(string));
                    else
                    {
                        var comparer = new Comparison.AlphanumericComparer();
                        var types = _fileTypes.Distinct().OrderBy(x => x, comparer);
                        WriteValue(nameof(FileTypes), types.Join(',').Trim(','));
                    }
                }
            }

            public FileTypeAssocData FileTypeAssoc
            {
                get
                {
                    if (_fileTypeAssoc != default(FileTypeAssocData))
                        return _fileTypeAssoc;
                    try
                    {
                        _fileTypeAssoc = new FileTypeAssocData(Ini.Read(_section, nameof(FileTypeAssoc)));
                    }
                    catch (Exception ex)
                    {
                        if (Log.DebugMode > 1)
                            Log.Write(ex);
                    }
                    return _fileTypeAssoc;
                }
                set
                {
                    _fileTypeAssoc = value;
                    WriteValue(nameof(FileTypeAssoc), _fileTypeAssoc?.ToString(), default(string), true);
                }
            }

            public bool NoConfirm
            {
                get
                {
                    if (!_noConfirm.HasValue)
                        _noConfirm = Ini.Read(_section, nameof(NoConfirm), false);
                    return (bool)_noConfirm;
                }
                set
                {
                    _noConfirm = value;
                    WriteValue(nameof(NoConfirm), _noConfirm, false);
                }
            }

            public bool NoUpdates
            {
                get
                {
                    if (!_noUpdates.HasValue)
                        _noUpdates = Ini.Read(_section, nameof(NoUpdates), false);
                    return (bool)_noUpdates;
                }
                set
                {
                    _noUpdates = value;
                    WriteValue(nameof(NoUpdates), _noUpdates, false);
                }
            }

            public DateTime NoUpdatesTime
            {
                get
                {
                    if (_noUpdatesTime == default(DateTime))
                        _noUpdatesTime = Ini.Read(_section, nameof(NoUpdatesTime), default(DateTime));
                    return _noUpdatesTime;
                }
                set
                {
                    _noUpdatesTime = value;
                    WriteValue(nameof(NoUpdatesTime), _noUpdatesTime);
                }
            }

            public bool RunAsAdmin
            {
                get
                {
                    if (!_runAsAdmin.HasValue)
                        _runAsAdmin = Ini.Read(_section, nameof(RunAsAdmin), false);
                    return (bool)_runAsAdmin;
                }
                set
                {
                    _runAsAdmin = value;
                    WriteValue(nameof(RunAsAdmin), _runAsAdmin, false);
                }
            }

            public string StartArgsFirst
            {
                get
                {
                    if (_startArgsFirst == default(string))
                        _startArgsFirst = Ini.Read<string>(_section, "StartArgs.First")?.DecodeString();
                    return _startArgsFirst;
                }
                set
                {
                    _startArgsFirst = value;
                    if (string.IsNullOrWhiteSpace(_startArgsFirst))
                        _startArgsFirst = default(string);
                    WriteValue("StartArgs.First", _startArgsFirst?.Encode());
                }
            }

            public string StartArgsLast
            {
                get
                {
                    if (_startArgsLast == default(string))
                        _startArgsLast = Ini.Read<string>(_section, "StartArgs.Last")?.DecodeString();
                    return _startArgsLast;
                }
                set
                {
                    _startArgsLast = value;
                    if (string.IsNullOrWhiteSpace(_startArgsLast))
                        _startArgsLast = default(string);
                    WriteValue("StartArgs.Last", _startArgsLast?.Encode());
                }
            }

            internal void WriteValue<TValue>(string key, TValue value, TValue defValue = default(TValue), bool direct = false) =>
                GlobalSettings.WriteValue(_section, key, value, defValue, direct);
        }
    }
}
