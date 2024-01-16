namespace PortAble;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SilDev;
using SilDev.Ini.Legacy;

public static class Arguments
{
    private static List<string> _fileTypes, _validPaths, _savedFileTypes;

    public static string AppName { get; set; }

    public static bool AppNameConflict { get; private set; }

    public static List<string> FileTypes
    {
        get
        {
            _fileTypes ??= new List<string>();
            if (_fileTypes.Count < 1 || ValidPaths.Count < 1)
                return _fileTypes;
            var comparer = new AlphaNumericComparer<string>();
            _fileTypes = ValidPaths.Where(x => !PathEx.IsDir(x))
                                   .Select(x => Path.GetExtension(x)?.TrimStart('.').ToLowerInvariant())
                                   .Where(Comparison.IsNotEmpty).Distinct().OrderBy(x => x, comparer).ToList();
            return _fileTypes;
        }
    }

    public static List<string> SavedFileTypes
    {
        get
        {
            if (_savedFileTypes?.Count is > 0)
                return _savedFileTypes;
            var comparer = new AlphaNumericComparer<string>();
            var types = CacheData.CurrentAppSettings?
                                 .SelectMany(p => p.Value.FileTypes.Select(s => s.TrimStart('*', '.').ToLowerInvariant()))
                                 .Where(Comparison.IsNotEmpty)
                                 .Distinct()
                                 .OrderBy(s => s, comparer);
            return _savedFileTypes = types?.ToList() ?? new List<string>();
        }
    }

    public static List<string> ValidPaths
    {
        get
        {
            _validPaths ??= new List<string>();
            if (_validPaths.Count > 0 || Environment.GetCommandLineArgs().Length < 2)
                return _validPaths;
            var comparer = new AlphaNumericComparer<string>();
            var args = Environment.GetCommandLineArgs().Skip(1).Where(PathEx.IsValidPath).OrderBy(x => x, comparer);
            _validPaths = args.ToList();
            return _validPaths;
        }
        set => _validPaths = value;
    }

    public static string ValidPathsStr
    {
        get
        {
            var str = string.Empty;
            if (ValidPaths.Count > 0)
                str = $"\"{ValidPaths.Join("\" \"")}\"";
            return str;
        }
    }

    public static void DefineAppName()
    {
        if (ValidPaths.Count < 1)
            return;

        var hashCode = ValidPathsStr?.GetHashCode() ?? -1;
        if (hashCode == -1)
            return;

        /*
        if (_CacheData.CurrentTypeData.TryGetValue(hashCode, out var keyHashCode))
        {
            var appData = _CacheData.CurrentAppInfo.FirstOrDefault(x => x.Key.GetHashCode().Equals(keyHashCode));
            if (appData != default)
            {
                AppName = appData.Name;
                return;
            }
        }
        */

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
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
                continue;
            }

            if (!File.Exists(path) || FileEx.IsHidden(path) || !Path.HasExtension(path))
                continue;

            ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext))
                continue;

            if (!typeInfo.TryGetValue(ext, out var value))
                typeInfo.Add(ext, 1);
            else
                typeInfo[ext] = ++value;
        }

        // check app settings for listed file types
        if (typeInfo.Count < 1)
            return;

        var sections = CacheData.CurrentAppSettings?.Where(x => x.Value.FileTypes?.Count is > 0).Select(x => x.Key).ToArray();
        if (sections?.Length < 1)
            return;
        string typeApp = null;
        foreach (var section in sections)
        {
            foreach (var type in typeInfo)
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
        foreach (var section in sections)
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