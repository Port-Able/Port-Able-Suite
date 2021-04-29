namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using SilDev;
    using SilDev.Legacy;

    internal static class CacheData
    {
        private static Dictionary<string, Image> _appImages, _currentImages;
        private static List<LocalAppData> _currentAppInfo;
        private static List<string> _currentAppSections;
        private static Image _currentImageBg;
        private static int _currentImagesCount;
        private static Dictionary<int, int> _currentTypeData;
        private static List<string> _settingsMerges;
        private static readonly List<Tuple<ImageResourceSymbol, bool, Icon>> Icons = new List<Tuple<ImageResourceSymbol, bool, Icon>>();
        private static readonly List<Tuple<ImageResourceSymbol, bool, Image>> Images = new List<Tuple<ImageResourceSymbol, bool, Image>>();

        internal static Dictionary<string, Image> AppImages
        {
            get
            {
                if (_appImages != default(Dictionary<string, Image>))
                    return _appImages;
                _appImages = FileEx.Deserialize<Dictionary<string, Image>>(File.Exists(CachePaths.AppImages) ? CachePaths.AppImages : CorePaths.AppImages);
                _appImages = _appImages != default(Dictionary<string, Image>) ? _appImages.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase) : new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
                return _appImages;
            }
        }

        internal static List<LocalAppData> CurrentAppInfo
        {
            get
            {
                if (_currentAppInfo != default(List<LocalAppData>) && _currentAppInfo.Any())
                    return _currentAppInfo;
                UpdateCurrentAppInfo();
                return _currentAppInfo;
            }
            set => _currentAppInfo = value;
        }

        internal static List<string> CurrentAppSections
        {
            get
            {
                if (_currentAppSections != default(List<string>))
                    return _currentAppSections;
                _currentAppSections = Ini.GetSections().Where(x => CurrentAppInfo.Any(y => y.Key.EqualsEx(x))).ToList();
                return _currentAppSections;
            }
            set => _currentAppSections = value;
        }

        internal static Image CurrentImageBg
        {
            get
            {
                if (_currentImageBg != default(Image))
                    return _currentImageBg;
                if (File.Exists(CachePaths.CurrentImageBg))
                    _currentImageBg = FileEx.Deserialize<Image>(CachePaths.CurrentImageBg);
                return _currentImageBg;
            }
            set
            {
                _currentImageBg = value;
                if (_currentImageBg != default(Image))
                {
                    FileEx.Serialize(CachePaths.CurrentImageBg, _currentImageBg);
                    return;
                }
                FileEx.TryDelete(CachePaths.CurrentImageBg);
            }
        }

        internal static Dictionary<string, Image> CurrentImages
        {
            get
            {
                if (_currentImages != default(Dictionary<string, Image>))
                    return _currentImages;
                if (File.Exists(CachePaths.CurrentImages))
                    _currentImages = FileEx.Deserialize<Dictionary<string, Image>>(CachePaths.CurrentImages);
                if (_currentImages == default(Dictionary<string, Image>))
                    _currentImages = new Dictionary<string, Image>();
                _currentImagesCount = _currentImages.Count;
                return _currentImages;
            }
        }

        internal static Dictionary<int, int> CurrentTypeData
        {
            get
            {
                if (_currentTypeData != default(Dictionary<int, int>))
                    return _currentTypeData;
                if (File.Exists(CachePaths.CurrentTypeData))
                    _currentTypeData = FileEx.Deserialize<Dictionary<int, int>>(CachePaths.CurrentTypeData);
                if (_currentTypeData == default(Dictionary<int, int>))
                    _currentTypeData = new Dictionary<int, int>();
                if (_currentTypeData.Count >= short.MaxValue)
                    _currentTypeData.Clear();
                return _currentTypeData;
            }
        }

        internal static List<string> SettingsMerges
        {
            get
            {
                if (_settingsMerges != default(List<string>))
                    return _settingsMerges;
                if (File.Exists(CachePaths.SettingsMerges))
                    _settingsMerges = FileEx.Deserialize<List<string>>(CachePaths.SettingsMerges);
                return _settingsMerges ?? (_settingsMerges = new List<string>());
            }
        }

        internal static Icon GetSystemIcon(ImageResourceSymbol index, bool large = false)
        {
            Icon icon;
            if (Icons.Any())
            {
                icon = Icons.FirstOrDefault(x => x.Item1.Equals(index) && x.Item2.Equals(large))?.Item3;
                if (icon != default(Icon))
                    goto Return;
            }
            icon = ResourcesEx.GetSystemIcon(index, large, Settings.IconResourcePath);
            if (icon == default(Icon))
                goto Return;
            var tuple = new Tuple<ImageResourceSymbol, bool, Icon>(index, large, icon);
            Icons.Add(tuple);
            Return:
            return icon;
        }

        internal static Image GetSystemImage(ImageResourceSymbol index, bool large = false)
        {
            Image image;
            if (Images.Any())
            {
                image = Images.FirstOrDefault(x => x.Item1.Equals(index) && x.Item2.Equals(large))?.Item3;
                if (image != default(Image))
                    goto Return;
            }
            image = GetSystemIcon(index, large)?.ToBitmap();
            if (image == default(Image))
                goto Return;
            var tuple = new Tuple<ImageResourceSymbol, bool, Image>(index, large, image);
            Images.Add(tuple);
            Return:
            return image;
        }

        internal static LocalAppData FindAppData(string appKeyOrName)
        {
            if (!CurrentAppInfo.Any() || string.IsNullOrWhiteSpace(appKeyOrName))
                return default;
            return CurrentAppInfo.FirstOrDefault(x => appKeyOrName.EqualsEx(x.Key, x.Name));
        }

        internal static void UpdateCurrentImagesFile()
        {
            if (!CurrentImages.Any() || _currentImagesCount == _currentImages.Count && File.Exists(CachePaths.CurrentImages))
                return;
            FileEx.Serialize(CachePaths.CurrentImages, CurrentImages);
            _currentImagesCount = _currentImages.Count;
        }

        internal static void UpdateCurrentTypeDataFile(int key, int value)
        {
            if (key == -1)
                return;
            if (!CurrentTypeData.TryGetValue(key, out var curValue))
                curValue = -1;
            if (curValue == value)
                return;
            CurrentTypeData.Update(key, value);
            FileEx.Serialize(CachePaths.CurrentTypeData, CurrentTypeData);
        }

        internal static void UpdateSettingsMerges(string section)
        {
            if (ProcessEx.InstancesCount(PathEx.LocalPath) <= 1)
                return;
            if (!File.Exists(CachePaths.SettingsMerges))
                SettingsMerges.Clear();
            if (!SettingsMerges.Contains(section, StringComparer.CurrentCultureIgnoreCase))
                SettingsMerges.Add(section);
            FileEx.Serialize(CachePaths.SettingsMerges, SettingsMerges);
        }

        internal static void ResetCurrent()
        {
            FileEx.TryDelete(CachePaths.CurrentImages);
            FileEx.TryDelete(CachePaths.CurrentAppInfo);
            CurrentAppInfo = default;
        }

        internal static void RemoveInvalidFiles()
        {
            if (Settings.CurrentDirectory.EqualsEx(PathEx.LocalDir))
                return;
            foreach (var type in new[] { "ini", "ixi" })
                foreach (var file in DirectoryEx.GetFiles(CorePaths.TempDir, $"*.{type}", SearchOption.AllDirectories))
                    FileEx.Delete(file);
            Settings.CurrentDirectory = PathEx.LocalDir;
        }

        private static void UpdateCurrentAppInfo()
        {
            _currentAppInfo = new List<LocalAppData>();
            var currentAppInfo = default(List<LocalAppData>);
            if (File.Exists(CachePaths.CurrentAppInfo))
                currentAppInfo = FileEx.Deserialize<List<LocalAppData>>(CachePaths.CurrentAppInfo);
            if (currentAppInfo == default(List<LocalAppData>))
                currentAppInfo = new List<LocalAppData>();

            var nameRegEx = default(Regex);
            var writeToFile = false;

            var dirs = default(string[]);
            try
            {
                dirs = Settings.AppDirs.SelectMany(x => DirectoryEx.EnumerateDirectories(x)).ToArray();
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }

            if (dirs?.Any() != true)
                return;

            foreach (var dir in dirs)
            {
                var key = Path.GetFileName(dir);
                if (string.IsNullOrEmpty(key) || !key.ContainsEx("Portable"))
                    continue;

                var current = currentAppInfo.FirstOrDefault(x => x.Key.EqualsEx(key));
                if (current != default && File.Exists(current.FilePath))
                {
                    _currentAppInfo.Add(current);
                    continue;
                }

                // try to get the best file path
                var filePath = Path.Combine(dir, $"{key}.exe");
                var configPath = Path.Combine(dir, $"{key}.ini");
                var appInfoPath = Path.Combine(dir, "App", "AppInfo", "appinfo.ini");
                if (File.Exists(configPath) || File.Exists(appInfoPath))
                {
                    var fileName = Ini.Read("AppInfo", "File", configPath);
                    if (string.IsNullOrEmpty(fileName))
                        fileName = Ini.Read("Control", "Start", appInfoPath);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var dirPath = Ini.Read("AppInfo", "Dir", configPath);
                        if (string.IsNullOrEmpty(dirPath))
                        {
                            filePath = Path.Combine(dir, fileName);
                            if (!File.Exists(configPath))
                                configPath = filePath.Replace(".exe", ".ini");
                        }
                        else
                        {
                            var curDirEnvVars = new[]
                            {
                                "%CurrentDir%",
                                "%CurDir%"
                            };
                            foreach (var vars in curDirEnvVars)
                            {
                                if (!dirPath.StartsWithEx(vars))
                                    continue;
                                var curDir = Path.GetDirectoryName(configPath);
                                if (string.IsNullOrEmpty(curDir))
                                    continue;
                                var subDir = dirPath.Substring(vars.Length).Trim('\\');
                                dirPath = Path.Combine(curDir, subDir);
                            }
                            filePath = PathEx.Combine(dirPath, fileName);
                        }
                    }
                }
                if (!File.Exists(filePath))
                    filePath = Path.Combine(dir, $"{key}.exe");
                if (!File.Exists(filePath))
                    continue;

                // try to get the full app name
                var name = Ini.Read("AppInfo", "Name", configPath);
                if (string.IsNullOrWhiteSpace(name))
                    name = Ini.Read("Details", "Name", appInfoPath);
                if (string.IsNullOrWhiteSpace(name))
                    name = FileVersionInfo.GetVersionInfo(filePath).FileDescription;
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                // apply some filters to the found app name
                if (nameRegEx == default(Regex))
                    nameRegEx = new Regex("(PortableApps.com Launcher)|, Portable Edition|Portable64|Portable", RegexOptions.IgnoreCase);
                var newName = nameRegEx.Replace(name, string.Empty).Replace("\t", " ");
                newName = Regex.Replace(newName.Trim(' ', ','), " {2,}", " ");
                if (!name.Equals(newName, StringComparison.Ordinal))
                    name = newName;
                if (string.IsNullOrWhiteSpace(name) || !File.Exists(filePath) || _currentAppInfo.Any(x => x.Name.EqualsEx(name)))
                    continue;

                _currentAppInfo.Add(new LocalAppData(key, name, dir, filePath, configPath, appInfoPath));
                writeToFile = true;
            }

            if (_currentAppInfo.Any())
                _currentAppInfo = _currentAppInfo.OrderBy(x => x.Name, new AlphaNumericComparer<string>()).ToList();
            if (writeToFile)
                FileEx.Serialize(CachePaths.CurrentAppInfo, _currentAppInfo);
        }
    }
}
