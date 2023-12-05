namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using SilDev;
    using SilDev.Legacy;

    public sealed class LocalAppSettings
    {
        private readonly string _section;
        private ReadOnlyCollection<string> _fileAbsoluteTypes;
        private FileTypeAssocData _fileTypeAssoc;
        private Collection<string> _fileTypes;
        private bool? _noConfirm, _noUpdates, _runAsAdmin, _sortArgPaths;
        private DateTime _noUpdatesTime;
        private string _startArgsFirst, _startArgsLast;

        public ReadOnlyCollection<string> FileAbsoluteTypes
        {
            get
            {
                if (FileTypes.Count == _fileAbsoluteTypes?.Count)
                    return _fileAbsoluteTypes;
                var types = FileTypes.ToArray();
                if (types.Any())
                    types = types.Select(x => x.TrimStart('.')).Distinct().ToArray();
                return _fileAbsoluteTypes = new ReadOnlyCollection<string>(types);
            }
        }

        public Collection<string> FileTypes
        {
            get
            {
                if (_fileTypes != default)
                    return _fileTypes;
                var types = Ini.Read<string>(_section, nameof(FileTypes));
                if (types != default)
                    _fileTypes = new Collection<string>(types.Contains(',') ? types.Split(',') : new[] { types });
                return _fileTypes ??= new Collection<string>();
            }
            set
            {
                if (_fileTypes == value)
                    return;
                _fileTypes = value;
                _fileAbsoluteTypes = default;
                if (_fileTypes?.Any() != true)
                    WriteValue(nameof(FileTypes), default(string));
                else
                {
                    var comparer = new AlphaNumericComparer<string>();
                    var types = _fileTypes.Distinct().OrderBy(x => x, comparer);
                    WriteValue(nameof(FileTypes), types.Join(',').Trim(','));
                }
            }
        }

        public FileTypeAssocData FileTypeAssoc
        {
            get
            {
                if (_fileTypeAssoc != default)
                    return _fileTypeAssoc;
                try
                {
                    _fileTypeAssoc = new FileTypeAssocData(Ini.Read(_section, nameof(FileTypeAssoc)));
                }
                catch (Exception ex) when (ex.IsCaught())
                {
                    if (Log.DebugMode > 1)
                        Log.Write(ex);
                }
                return _fileTypeAssoc;
            }
            set
            {
                _fileTypeAssoc = value;
                WriteValue(nameof(FileTypeAssoc), _fileTypeAssoc?.ToString(), default, true);
            }
        }

        public bool NoConfirm
        {
            get
            {
                _noConfirm ??= Ini.Read(_section, nameof(NoConfirm), false);
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
                _noUpdates ??= Ini.Read(_section, nameof(NoUpdates), false);
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
                if (_noUpdatesTime == default)
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
                _runAsAdmin ??= Ini.Read(_section, nameof(RunAsAdmin), false);
                return (bool)_runAsAdmin;
            }
            set
            {
                _runAsAdmin = value;
                WriteValue(nameof(RunAsAdmin), _runAsAdmin, false);
            }
        }

        public bool SortArgPaths
        {
            get
            {
                _sortArgPaths ??= Ini.Read(_section, nameof(SortArgPaths), false);
                return (bool)_sortArgPaths;
            }
            set
            {
                _sortArgPaths = value;
                WriteValue(nameof(SortArgPaths), _sortArgPaths, false);
            }
        }

        public string StartArgsFirst
        {
            get => _startArgsFirst ??= Ini.Read<string>(_section, "StartArgs.First")?.DecodeString();
            set
            {
                _startArgsFirst = value;
                if (string.IsNullOrWhiteSpace(_startArgsFirst))
                    _startArgsFirst = default;
                WriteValue("StartArgs.First", _startArgsFirst?.Encode());
            }
        }

        public string StartArgsLast
        {
            get => _startArgsLast ??= Ini.Read<string>(_section, "StartArgs.Last")?.DecodeString();
            set
            {
                _startArgsLast = value;
                if (string.IsNullOrWhiteSpace(_startArgsLast))
                    _startArgsLast = default;
                WriteValue("StartArgs.Last", _startArgsLast?.Encode());
            }
        }

        internal LocalAppSettings(string key) =>
            _section = key;

        internal void WriteValue<TValue>(string key, TValue value, TValue defValue = default, bool direct = false) =>
            Settings.WriteValue(_section, key, value, defValue, direct);
    }
}
