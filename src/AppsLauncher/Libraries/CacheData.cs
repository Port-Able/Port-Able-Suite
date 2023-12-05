namespace AppsLauncher.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Legacy;

    internal static class CacheData
    {
        private static Dictionary<string, Image> _appImages, _appImagesLarge, _currentImages;
        private static List<LocalAppData> _currentAppInfo;
        private static List<string> _currentAppSections;
        private static Image _currentImageBg;
        private static int _currentImagesCount;
        private static Dictionary<int, int> _currentTypeData;
        private static List<string> _settingsMerges;
        private static readonly List<Tuple<string, Color, int, Image>> Images = new();
        private static readonly List<Tuple<ImageResourceSymbol, bool, Icon>> SystemIcons = new();
        private static readonly List<Tuple<ImageResourceSymbol, bool, Image>> SystemImages = new();

        internal static Dictionary<string, Image> AppImages
        {
            get
            {
                if (_appImages != default(Dictionary<string, Image>))
                    return _appImages;
                _appImages = FileEx.Deserialize<Dictionary<string, Image>>(File.Exists(CacheFiles.AppImages) ? CacheFiles.AppImages : CorePaths.AppImages);
                _appImages = _appImages != default(Dictionary<string, Image>) ? _appImages.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase) : new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
                return _appImages;
            }
        }

        internal static Dictionary<string, Image> AppImagesLarge
        {
            get
            {
                if (_appImagesLarge != default(Dictionary<string, Image>))
                    return _appImagesLarge;
                _appImagesLarge = FileEx.Deserialize<Dictionary<string, Image>>(File.Exists(CacheFiles.AppImagesLarge) ? CacheFiles.AppImagesLarge : CorePaths.AppImagesLarge);
                _appImagesLarge = _appImagesLarge != default(Dictionary<string, Image>) ? _appImages.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase) : new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
                return _appImagesLarge;
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
                if (File.Exists(CacheFiles.CurrentImageBg))
                    _currentImageBg = FileEx.Deserialize<Image>(CacheFiles.CurrentImageBg);
                return _currentImageBg;
            }
            set
            {
                _currentImageBg = value;
                if (_currentImageBg != default(Image))
                {
                    FileEx.Serialize(CacheFiles.CurrentImageBg, _currentImageBg);
                    return;
                }
                FileEx.TryDelete(CacheFiles.CurrentImageBg);
            }
        }

        internal static Dictionary<string, Image> CurrentImages
        {
            get
            {
                if (_currentImages != default(Dictionary<string, Image>))
                    return _currentImages;
                if (File.Exists(CacheFiles.CurrentImages))
                    _currentImages = FileEx.Deserialize<Dictionary<string, Image>>(CacheFiles.CurrentImages);
                _currentImages ??= new Dictionary<string, Image>();
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
                if (File.Exists(CacheFiles.CurrentTypeData))
                    _currentTypeData = FileEx.Deserialize<Dictionary<int, int>>(CacheFiles.CurrentTypeData);
                _currentTypeData ??= new Dictionary<int, int>();
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
                if (File.Exists(CacheFiles.SettingsMerges))
                    _settingsMerges = FileEx.Deserialize<List<string>>(CacheFiles.SettingsMerges);
                return _settingsMerges ??= new List<string>();
            }
        }

        internal static void Serialize<T>(string path, T item)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!PathEx.IsValidPath(path))
                throw new IOException(nameof(path));
            using var sw = File.CreateText(path);
            var js = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            js.Serialize(sw, item);
        }

        internal static T Deserialize<T>(string path)
        {
            if (!FileEx.Exists(path))
                return default;
            using var sw = File.OpenText(path);
            var js = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            return (T)js.Deserialize(sw, typeof(T));
        }

        internal static Icon GetSystemIcon(ImageResourceSymbol index, bool large = false)
        {
            Icon icon;
            if (SystemIcons.Any())
            {
                icon = SystemIcons.FirstOrDefault(x => x.Item1.Equals(index) && x.Item2.Equals(large))?.Item3;
                if (icon != default(Icon))
                    goto Return;
            }
            icon = ResourcesEx.GetSystemIcon(index, large, Settings.IconResourcePath);
            if (icon == default(Icon))
                goto Return;
            var tuple = new Tuple<ImageResourceSymbol, bool, Icon>(index, large, icon);
            SystemIcons.Add(tuple);
            Return:
            return icon;
        }

        internal static Image GetSystemImage(ImageResourceSymbol index, bool large = false)
        {
            Image image;
            if (SystemImages.Any())
            {
                image = SystemImages.FirstOrDefault(x => x.Item1.Equals(index) && x.Item2.Equals(large))?.Item3;
                if (image != default(Image))
                    goto Return;
            }
            image = GetSystemIcon(index, large)?.ToBitmap();
            if (image == default(Image))
                goto Return;
            var tuple = new Tuple<ImageResourceSymbol, bool, Image>(index, large, image);
            SystemImages.Add(tuple);
            Return:
            return image;
        }

        internal static Image GetImage(Image image, string key, Color color = default, int sizeIndicator = default)
        {
            var img = Images.FirstOrDefault(x => x.Item1.Equals(key) &&
                                                 x.Item2.Equals(color) &&
                                                 x.Item3.Equals(sizeIndicator))?.Item4;
            if (img != default)
                return img;
            img = image;
            if (img == default)
                return default;
            if (color != default || 
                color != Color.Empty ||
                color != Color.Transparent)
                img = img.RecolorPixels(Color.Black, color);
            if (sizeIndicator > 0)
                img = img.Redraw(sizeIndicator);
            if (!string.IsNullOrEmpty(key))
                Images.Add(new(key, color, sizeIndicator, img));
            return img;
        }

        internal static void SetComponentImageColor(Component component, Color color, bool redraw = false)
        {
            switch (component)
            {
                case Button { Image: not null } item:
                    item.Image = GetImage(item.Image, default, color, redraw ? Math.Min(item.Width, item.Height) - 8 : 0);
                    break;
                case PictureBox { BackgroundImage: not null } item:
                    item.BackgroundImage = GetImage(item.BackgroundImage, default, color, redraw ? Math.Min(item.Width, item.Height) : 0);
                    break;
                case ToolStripMenuItem { Image: not null } item:
                    item.Image = GetImage(item.Image, default, color, redraw ? Math.Min(item.Width, item.Height) : 0);
                    break;
            }
        }

        internal static void SetComponentImageColor(Component component, bool redraw = false)
        {
            var color = component switch
            {
                ToolStripMenuItem { Image: not null } => SystemColors.Highlight,
                _ => Settings.Window.Colors.BaseLight
            };
            SetComponentImageColor(component, color, redraw);
        }

        internal static LocalAppData FindAppData(string appKeyOrName)
        {
            if (!CurrentAppInfo.Any() || string.IsNullOrWhiteSpace(appKeyOrName))
                return default;
            return CurrentAppInfo.FirstOrDefault(x => appKeyOrName.EqualsEx(x.Key, x.Name));
        }

        internal static void UpdateCurrentImagesFile()
        {
            if (!CurrentImages.Any() || (_currentImagesCount == _currentImages.Count && File.Exists(CacheFiles.CurrentImages)))
                return;
            FileEx.Serialize(CacheFiles.CurrentImages, CurrentImages);
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
            FileEx.Serialize(CacheFiles.CurrentTypeData, CurrentTypeData);
        }

        internal static void UpdateSettingsMerges(string section)
        {
            if (ProcessEx.InstancesCount(PathEx.LocalPath) <= 1)
                return;
            if (!File.Exists(CacheFiles.SettingsMerges))
                SettingsMerges.Clear();
            if (!SettingsMerges.Contains(section, StringComparer.CurrentCultureIgnoreCase))
                SettingsMerges.Add(section);
            FileEx.Serialize(CacheFiles.SettingsMerges, SettingsMerges);
        }

        internal static void ResetCurrent()
        {
            FileEx.TryDelete(CacheFiles.CurrentImages);
            FileEx.TryDelete(CacheFiles.CurrentAppInfo);
            CurrentAppInfo = default;
        }

        internal static void RemoveInvalidFiles()
        {
            if (Settings.CurrentDirectory.EqualsEx(PathEx.LocalDir))
                return;
            foreach (var type in new[] { "ini", "ixi" })
                foreach (var file in DirectoryEx.GetFiles(CorePaths.DataDir, $"*.{type}", SearchOption.AllDirectories))
                    FileEx.Delete(file);
            Settings.CurrentDirectory = PathEx.LocalDir;
        }

        private static void UpdateCurrentAppInfo()
        {
            _currentAppInfo = new List<LocalAppData>();
            var currentAppInfo = default(List<LocalAppData>);
            if (File.Exists(CacheFiles.CurrentAppInfo))
                currentAppInfo = FileEx.Deserialize<List<LocalAppData>>(CacheFiles.CurrentAppInfo);
            currentAppInfo ??= new List<LocalAppData>();

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
                nameRegEx ??= new Regex("(PortableApps.com Launcher)|, Portable Edition|Portable64|Portable", RegexOptions.IgnoreCase);
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
                FileEx.Serialize(CacheFiles.CurrentAppInfo, _currentAppInfo);
        }
    }
}
