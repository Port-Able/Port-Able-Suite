namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using SilDev;

    internal static class Arguments
    {
        private static List<string> _fileTypes, _validPaths, _savedFileTypes;

        internal static string AppName { get; set; }

        internal static bool AppNameConflict { get; private set; }

        internal static List<string> FileTypes
        {
            get
            {
                if (_fileTypes == default(List<string>))
                    _fileTypes = new List<string>();
                if (_fileTypes.Any() || !ValidPaths.Any())
                    return _fileTypes;
                var comparer = new Comparison.AlphanumericComparer();
                _fileTypes = ValidPaths.Where(x => !PathEx.IsDir(x)).Select(x => Path.GetExtension(x)?.TrimStart('.').ToLower())
                                       .Where(Comparison.IsNotEmpty).Distinct().OrderBy(x => x, comparer).ToList();
                return _fileTypes;
            }
        }

        internal static List<string> SavedFileTypes
        {
            get
            {
                if (_savedFileTypes == null)
                    _savedFileTypes = new List<string>();
                if (_savedFileTypes.Count > 0)
                    return _savedFileTypes;
                var comparer = new Comparison.AlphanumericComparer();
                var types = CacheData.CurrentAppSections.Aggregate(string.Empty, (x, y) => x + $"{Ini.Read(y, "FileTypes").RemoveChar('*', '.')},").ToLower();
                _savedFileTypes = types.Split(',').Where(Comparison.IsNotEmpty).Distinct().OrderBy(x => x, comparer).ToList();
                return _savedFileTypes;
            }
        }

        internal static List<string> ValidPaths
        {
            get
            {
                if (_validPaths == default(List<string>))
                    _validPaths = new List<string>();
                if (_validPaths.Any() || Environment.GetCommandLineArgs().Length < 2)
                    return _validPaths;
                var comparer = new Comparison.AlphanumericComparer();
                var args = Environment.GetCommandLineArgs().Skip(1).Where(PathEx.IsValidPath).OrderBy(x => x, comparer);
                _validPaths = args.ToList();
                return _validPaths;
            }
        }

        internal static string ValidPathsStr
        {
            get
            {
                var str = string.Empty;
                if (ValidPaths.Any())
                    str = $"\"{ValidPaths.Join("\" \"")}\"";
                return str;
            }
        }

        internal static void DefineAppName()
        {
            if (!ValidPaths.Any())
                return;

            var hashCode = ValidPathsStr?.GetHashCode() ?? -1;
            if (hashCode == -1)
                return;

            if (CacheData.CurrentTypeData.TryGetValue(hashCode, out var keyHashCode))
            {
                var appData = CacheData.CurrentAppInfo.FirstOrDefault(x => x.Key.GetHashCode().Equals(keyHashCode));
                if (appData != default(LocalAppData))
                {
                    AppName = appData.Name;
                    return;
                }
            }

            var allTypes = SavedFileTypes?.Select(x => '.' + x).ToArray();
            var typeInfo = new Dictionary<string, int>();
            var stopwatch = new Stopwatch();
            foreach (var path in ValidPaths)
            {
                string ext;
                try
                {
                    if (PathEx.IsDir(path))
                    {
                        stopwatch.Start();
                        var dirInfo = new DirectoryInfo(path);
                        foreach (var fileInfo in dirInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Take(1024))
                        {
                            if (fileInfo.MatchAttributes(FileAttributes.Hidden))
                                continue;

                            ext = fileInfo.Extension;
                            if (typeInfo.ContainsKey(ext) || !ext.EndsWithEx(allTypes))
                                continue;

                            var len = dirInfo.GetFiles('*' + ext, SearchOption.AllDirectories).Length;
                            if (len == 0)
                                continue;

                            typeInfo.Add(ext, len);
                            if (stopwatch.ElapsedMilliseconds >= 4096)
                                break;
                        }
                        stopwatch.Reset();
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    continue;
                }

                if (!File.Exists(path) || FileEx.IsHidden(path) || !Path.HasExtension(path))
                    continue;

                ext = Path.GetExtension(path);
                if (string.IsNullOrEmpty(ext))
                    continue;

                if (!typeInfo.ContainsKey(ext))
                    typeInfo.Add(ext, 1);
                else
                    typeInfo[ext]++;
            }

            // check app settings for listed file types
            if (typeInfo.Any() != true)
                return;

            string typeApp = null;
            foreach (var type in typeInfo)
                foreach (var section in CacheData.CurrentAppSections)
                {
                    var fileTypes = Ini.Read(section, "FileTypes");
                    if (string.IsNullOrWhiteSpace(fileTypes))
                        continue;
                    fileTypes = $"|.{fileTypes.RemoveChar('*', '.')?.Replace(",", "|.")}|"; // Enforce format

                    // if file type settings found for a app, select this app as default
                    if (fileTypes.ContainsEx($"|{type.Key}|"))
                    {
                        AppName = section;
                        if (string.IsNullOrWhiteSpace(typeApp))
                            typeApp = section;
                    }
                    if (AppNameConflict || string.IsNullOrWhiteSpace(AppName) || string.IsNullOrWhiteSpace(typeApp) || AppName.EqualsEx(typeApp))
                        continue;
                    AppNameConflict = true;
                    break;
                }

            // If multiple file types with different app settings found, select the app with most listed file types
            if (!AppNameConflict)
                return;
            var query = typeInfo.OrderByDescending(x => x.Value);
            var count = 0;
            string topType = null;
            foreach (var item in query)
            {
                if (item.Value > count)
                    topType = item.Key;
                count = item.Value;
            }
            if (string.IsNullOrEmpty(topType))
                return;
            foreach (var section in CacheData.CurrentAppSections)
            {
                var fileTypes = Ini.Read(section, "FileTypes");
                if (string.IsNullOrWhiteSpace(fileTypes))
                    continue;
                fileTypes = $"|.{fileTypes.RemoveChar('*', '.').Replace(",", "|.")}|"; // Filter
                if (!fileTypes.ContainsEx($"|{topType}|"))
                    continue;
                AppName = section;
                break;
            }
        }
    }
}
