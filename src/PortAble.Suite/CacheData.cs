namespace PortAble
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using SilDev;
    using SilDev.Compression.Archiver;
    using SilDev.Drawing;
    using SilDev.Ini.Legacy;
    using SilDev.Network;

    /// <summary>
    ///     Provides functionality to exchange resources between different sources.
    /// </summary>
    public static class CacheData
    {
        private static List<CustomAppSupplier> _customAppSuppliers;

        /// <summary>
        ///     Gets a collection of <see cref="AppData"/> instances for all apps.
        /// </summary>
        public static IReadOnlyList<AppData> AppInfo { get; } = InitAppInfo();

        /// <summary>
        ///     Gets a collection of filters that applies when creating <see cref="AppInfo"/>.
        /// </summary>
        public static IReadOnlyDictionary<string, string[]> AppInfoFilters { get; private set; }

        /// <summary>
        ///     Gets the database containing small app images of all apps.
        /// </summary>
        public static IReadOnlyDictionary<string, Image> AppImages { get; } =
            InitFromDat<Dictionary<string, Image>>(CacheFiles.AppImages,
                                                   CorePaths.AppImages,
                                                   StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Gets the database containing large app images of all apps.
        /// </summary>
        public static IReadOnlyDictionary<string, Image> AppImagesLarge { get; } =
            InitFromDat<Dictionary<string, Image>>(CacheFiles.AppImagesLarge,
                                                   CorePaths.AppImagesLarge,
                                                   StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Gets the database containing server information of official app suppliers.
        ///     <para>
        ///         &#9888; It is recommended to use <see cref="AppSupplierMirrors"/>
        ///         instead of this property as it already provides the information of this
        ///         property more easily.
        ///     </para>
        ///     <para>
        ///         Otherwise, the following hierarchy is used:
        ///     </para>
        ///     <code>
        ///         <see langword="this"/>["SUPPLIER_KEY"]["HTTPS_ADDRESS"] == "SERVER_LOCATION"
        ///     </code>
        ///     <para>
        ///         For example, to retrieve a collection of SourceForge server addresses,
        ///         apply:
        ///     </para>
        ///     <code>
        ///         <see langword="this"/>["Sf"].Keys
        ///      </code>
        /// </summary>
        public static IReadOnlyDictionary<string, Dictionary<string, string>> AppSuppliers { get; } =
            InitFromDat<Dictionary<string, Dictionary<string, string>>>(CacheFiles.AppSuppliers,
                                                                        CorePaths.AppSuppliers,
                                                                        StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Gets a collection of <see cref="CustomAppSupplier"/> instances.
        /// </summary>
        public static IReadOnlyList<CustomAppSupplier> CustomAppSuppliers
        {
            get
            {
                if (_customAppSuppliers != default)
                    return _customAppSuppliers;
                var dir = CorePaths.CustomAppSuppliersDir;
                if (Directory.Exists(dir))
                    _customAppSuppliers = DirectoryEx.EnumerateFiles(dir)
                                                     .Select(f => LoadDat<CustomAppSupplier>(f))
                                                     .Where(x => x != default).ToList();
                return _customAppSuppliers ?? new List<CustomAppSupplier>();
            }
        }

        /// ReSharper disable CommentTypo
        /// <summary>
        ///     Gets Nullsoft Scriptable Install System (NSIS) button strings in all
        ///     languages used for automated app installation.
        ///     <para>
        ///         Please note that only Agree texts are included, so you don't have to
        ///         worry about filtering out a text like Cancel.
        ///     </para>
        ///     <para>
        ///         The following hierarchy is used:
        ///     </para>
        ///     <code>
        ///         <see langword="this"/>[LANG_ID][INDEX] == "BUTTON_TEXT"
        ///     </code>
        ///     <para>
        ///         For example, to get a button text for US English, apply:
        ///     </para>
        ///     <code>
        ///         <see langword="this"/>[1033][2] == "&amp;Next &gt;"
        ///      </code>
        /// </summary>
        public static IReadOnlyDictionary<int, List<string>> NsisButtons { get; } =
            InitFromDat<Dictionary<int, List<string>>>(CacheFiles.NsisButtons,
                                                       CorePaths.NsisButtons);

        /// <summary>
        ///     Saves the data of the specified instance in binary form to the specified
        ///     destination file.
        ///     <para>
        ///         &#9762; Please note that an existing file is always overwritten.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the source.
        /// </typeparam>
        /// <param name="source">
        ///     The source instance to save.
        /// </param>
        /// <param name="destination">
        ///     The file to write.
        /// </param>
        /// <param name="compress">
        ///     Determines whether the file should be compressed, which can also speed up
        ///     reading time.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> on success; otherwise <see langword="false"/>.
        /// </returns>
        public static bool SaveDat<T>(T source, string destination, bool compress = true) where T : class =>
            DirectoryEx.CreateParent(destination) && FileEx.Serialize(destination, source, compress);

        /// <summary>
        ///     Loads a previous saved data of an instance from specified binary file.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the result.
        /// </typeparam>
        /// <param name="path">
        ///     The file to read.
        /// </param>
        /// <param name="defValue">
        ///     The default value returned if the file cannot be read.
        /// </param>
        /// <returns>
        ///     If successful, the saved instance of <typeparamref name="T"/>; otherwise
        ///     <paramref name="defValue"/>.
        /// </returns>
        public static T LoadDat<T>(string path, T defValue = default) where T : class =>
            FileEx.Deserialize(path, defValue);

        /// <summary>
        ///     Saves the data of the specified instance in human-readable JSON format to
        ///     the specified destination file.
        ///     <para>
        ///         &#9762; Please note that an existing file is always overwritten.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the source.
        /// </typeparam>
        /// <param name="source">
        ///     The source instance to save.
        /// </param>
        /// <param name="destination">
        ///     The file to write.
        /// </param>
        /// <param name="formatted">
        ///     Determines whether the file should be formatted to further improve
        ///     readability.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> on success; otherwise <see langword="false"/>.
        /// </returns>
        public static bool SaveJson<T>(T source, string destination, bool formatted = true) where T : class
        {
            try
            {
                if (destination == null)
                    throw new ArgumentNullException(nameof(destination));
                if (source == null)
                    throw new ArgumentNullException(nameof(source));
                if (!PathEx.IsValidPath(destination))
                    throw new IOException();
                if (!DirectoryEx.CreateParent(destination))
                    throw new DirectoryNotFoundException();
                using var sw = File.CreateText(destination);
                var js = new JsonSerializer
                {
                    Formatting = formatted ? Formatting.Indented : Formatting.None
                };
                js.Serialize(sw, source);
                return FileEx.Exists(destination) && (DateTime.Now - File.GetLastWriteTime(destination)).TotalSeconds < 10;
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
                return false;
            }
        }

        /// <summary>
        ///     Loads a previous saved data of an instance from specified JSON file.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the result.
        /// </typeparam>
        /// <param name="path">
        ///     The file to read.
        /// </param>
        /// <param name="defValue">
        ///     The default value returned if the file cannot be read.
        /// </param>
        /// <returns>
        ///     If successful, the saved instance of <typeparamref name="T"/>; otherwise
        ///     <paramref name="defValue"/>.
        /// </returns>
        public static T LoadJson<T>(string path, T defValue = default) where T : class
        {
            if (!FileEx.Exists(path))
                return default;
            try
            {
                using var sw = File.OpenText(path);
                var js = new JsonSerializer();
                return (T)js.Deserialize(sw, typeof(T)) ?? throw new IOException();
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return defValue;
        }

        /// <summary>
        ///     Searches for an app image in <see cref="AppImagesLarge"/> and/or
        ///     <see cref="AppImages"/> using the specified key and returns an
        ///     <see cref="Icon"/>-converted version of it.
        /// </summary>
        /// <param name="key">
        ///     The key of an app.
        /// </param>
        /// <param name="large">
        ///     <see langword="true"/> to first try to find the image in
        ///     <see cref="AppImagesLarge"/> and only use <see cref="AppImages"/> as a
        ///     fallback if the search fails; otherwise, <see langword="false"/> to search
        ///     only in <see cref="AppImages"/>.
        /// </param>
        /// <param name="defIcon">
        ///     The default value returned if the search was unsuccessful.
        /// </param>
        /// <returns>
        ///     If successful, the icon of an app image; otherwise
        ///     <paramref name="defIcon"/>.
        /// </returns>
        public static Icon GetAppIcon(string key, bool large = true, Icon defIcon = default)
        {
            if (!large || !AppImagesLarge.TryGetValue(key, out var image))
                AppImages.TryGetValue(key, out image);
            return image?.ToIcon() ?? defIcon;
        }

        /// <summary>
        ///     Searches for an app image in <see cref="AppImagesLarge"/> (fallback:
        ///     <see cref="AppImages"/>) using the specified key and returns an
        ///     <see cref="Icon"/>-converted version of it.
        /// </summary>
        /// <inheritdoc cref="GetAppIcon(string, bool, Icon)"/>
        public static Icon GetAppIcon(string key, Icon defIcon) =>
            GetAppIcon(key, true, defIcon);

        /// <summary>
        ///     Recolors the black pixels of the specified component's image to the
        ///     specified color.
        ///     <para>
        ///         &#9888; If the <see cref="Component"/> is of type <see cref="Button"/>
        ///         or <see cref="ToolStripMenuItem"/>, the Image property is used, if it
        ///         is of type <see cref="Control"/>, the BackgroundImage property is used.
        ///     </para>
        /// </summary>
        /// <param name="component">
        ///     The component whose image is to be recolored.
        /// </param>
        /// <param name="color">
        ///     The new color.
        /// </param>
        /// <param name="redraw">
        ///     Determines whether the image should be redrawn based on the maximum size
        ///     available in the <see cref="Component"/> object, which can increase visual
        ///     quality.
        /// </param>
        public static void SetComponentImageColor(Component component, Color color, bool redraw = false)
        {
            switch (component)
            {
                case Button { Image: not null } item:
                    item.Image = item.Image.RecolorPixels(Color.Black, color);
                    if (redraw)
                        item.Image = item.Image.Redraw(Math.Min(item.Width, item.Height) - 8);
                    break;
                case ToolStripMenuItem { Image: not null } item:
                    item.Image = item.Image.RecolorPixels(Color.Black, color);
                    if (redraw)
                        item.Image = item.Image.Redraw(Math.Min(item.Width, item.Height));
                    break;
                case Control { BackgroundImage: not null } item:
                    item.BackgroundImage = item.BackgroundImage.RecolorPixels(Color.Black, color);
                    if (redraw)
                        item.BackgroundImage = item.BackgroundImage.Redraw(Math.Min(item.Width, item.Height));
                    break;
            }
        }

        /// <summary>
        ///     Recolors the black pixels of the specified component's image to the
        ///     <see cref="SystemColors.Highlight"/> color.
        ///     <para>
        ///         &#9888; If the <see cref="Component"/> is of type <see cref="Button"/>
        ///         or <see cref="ToolStripMenuItem"/>, the Image property is used, if it
        ///         is of type <see cref="Control"/>, the BackgroundImage property is used.
        ///     </para>
        /// </summary>
        /// <inheritdoc cref="SetComponentImageColor(Component, Color, bool)"/>
        public static void SetComponentImageColor(Component component, bool redraw = false) =>
            SetComponentImageColor(component, SystemColors.Highlight, redraw);

        private static T InitFromDat<T>(string cachePath, string fallbackPath, params object[] parameters) where T : class
        {
            LocalUpdateFileByDateTime(cachePath);
            return LoadDat<T>(cachePath) ??
                   LoadDat<T>(fallbackPath) ??
                   parameters switch
                   {
                       null => (T)Activator.CreateInstance(typeof(T)),
                       _ => (T)Activator.CreateInstance(typeof(T), parameters)
                   };

            static void LocalUpdateFileByDateTime(string filePath)
            {
                var fileName = Path.GetFileName(filePath);
                if (string.IsNullOrEmpty(fileName))
                    return;
                var fileDate = File.Exists(filePath) ? File.GetLastWriteTime(filePath) : DateTime.MinValue;
                foreach (var link in AppSupplierMirrors.Pa.Keys.Select(url => PathEx.AltCombine(url, ".free", fileName)))
                {
                    if (Log.DebugMode > 0)
                        Log.Write($"Cache: Looking for '{link}'.");
                    if (!NetEx.FileIsAvailable(link, 30000, UserAgents.Pa))
                        continue;
                    if (!((NetEx.GetFileDate(link, 30000, UserAgents.Pa) - fileDate).TotalSeconds > 0d))
                        break;
                    if (!WebTransfer.DownloadFile(link, filePath, 60000, UserAgents.Pa, false))
                        continue;
                    File.SetLastWriteTime(filePath, DateTime.Now);
                    if (Log.DebugMode > 0)
                        Log.Write($"Cache: '{filePath}' updated.");
                    break;
                }
            }
        }

        private static List<AppData> InitAppInfo()
        {
            if (!DirectoryEx.Create(CorePaths.TransferDir))
                throw new IOException();

            IEnumerable<AppData> enumAppInfo;
            var appInfo = ApproveAppInfoDat();
            if (appInfo.Any())
            {
                enumAppInfo = CustomAppSupplierInfoCollector(appInfo);
                if (enumAppInfo != null)
                    appInfo.AddRange(enumAppInfo);
                return appInfo;
            }

            // Download all the data from PA servers.
            var pa = Path.Combine(CorePaths.TransferDir, "AppInfo.ini");
            var pac = Path.Combine(CorePaths.TransferDir, "AppInfo.7z");
            foreach (var file in new[] { pa, pac })
            {
                var name = Path.GetFileName(file);
                foreach (var link in AppSupplierMirrors.Pa.Keys.Select(x => PathEx.AltCombine(x, ".free", name)))
                {
                    if (Log.DebugMode > 0)
                        Log.Write($"Cache: Save '{link}' to '{file}'.");
                    if (!NetEx.FileIsAvailable(link, 30000, UserAgents.Pa))
                        continue;
                    if (WebTransfer.DownloadFile(link, file, 60000, UserAgents.Pa, false))
                        break;
                }
            }

            // Fallback, in the unlikely case
            // that all PA servers are down.
            if (!File.Exists(pac))
            {
                var link = PathEx.AltCombine(AppSupplierHosts.Pac, "updater", "update.7z");
                if (Log.DebugMode > 0)
                    Log.Write($"Cache: Save '{link}' to '{pac}'.");
                if (NetEx.FileIsAvailable(link, 60000, UserAgents.Empty))
                    WebTransfer.DownloadFile(link, pac, 60000, UserAgents.Empty, false);
            }

            // Load the PA`s INI config file
            if (File.Exists(pa))
            {
                enumAppInfo = AppInfoCollector(appInfo, pa);
                if (enumAppInfo != null)
                    appInfo.AddRange(enumAppInfo);

                if (FileEx.TryDelete(pa) && Log.DebugMode > 0)
                    Log.Write($"Cache: '{pa}' deleted.");
            }

            // Extract the PAC`s INI config file and load it.
            if (File.Exists(pac))
            {
                if (!File.Exists(CorePaths.FileArchiver))
                    throw new PathNotFoundException(CorePaths.FileArchiver);

                using (var process = SevenZip.DefaultArchiver.Extract(pac, CorePaths.TransferDir))
                {
                    if (process?.HasExited == false)
                        process.WaitForExit();
                    if (FileEx.TryDelete(pac) && Log.DebugMode > 0)
                        Log.Write($"Cache: '{pac}' deleted.");
                }

                pac = DirectoryEx.EnumerateFiles(CorePaths.TransferDir, "*.ini").FirstOrDefault(x => !x.EqualsEx(pa));
                if (!File.Exists(pac))
                    throw new PathNotFoundException(pac);

                enumAppInfo = AppInfoCollector(appInfo, pac);
                if (enumAppInfo != null)
                    appInfo.AddRange(enumAppInfo);

                if (FileEx.TryDelete(pac) && Log.DebugMode > 0)
                    Log.Write($"Cache: '{pac}' deleted.");
            }

            // Sort the instances in the list by app name
            // and place discontinued apps at the bottom.
            var comparer = new AlphaNumericComparer<string>();
            var keywords = new[]
            {
                "Legacy",
                "Discontinued"
            };
            if (appInfo.Any())
                appInfo = appInfo.OrderBy(a => a.Key.ContainsEx(keywords) ||
                                               a.Name.ContainsEx(keywords) ||
                                               a.DisplayVersion.ContainsEx(keywords))
                                 .ThenBy(x => x.Name, comparer)
                                 .ToList();

            switch (Log.DebugMode)
            {
                // Save cleaned app image databases.
                case > 1:
                {
                    var appImagesLarge = new Dictionary<string, Image>();
                    var appImages = new Dictionary<string, Image>();
                    foreach (var a in AppInfo.Where(a => !a.Key.ContainsEx(keywords) &&
                                                         !a.Name.ContainsEx(keywords) &&
                                                         !a.DisplayVersion.ContainsEx(keywords)))
                    {
                        if (AppImagesLarge.TryGetValue(a.Key, out var image))
                            appImagesLarge.Add(a.Key, image);
                        if (AppImages.TryGetValue(a.Key, out image))
                            appImages.Add(a.Key, image);
                    }
                    SaveDat(appImagesLarge, Path.ChangeExtension(CacheFiles.AppImagesLarge, "cleaned.dat"));
                    SaveDat(appImages, Path.ChangeExtension(CacheFiles.AppImages, "cleaned.dat"));
                    break;
                }

                // Finally write the updated data to cache.
                default:
                    SaveDat(appInfo, CacheFiles.AppInfo);
                    break;
            }

            // Handle shared data without caching as the server
            // data may also contain sensitive information.
            enumAppInfo = CustomAppSupplierInfoCollector(appInfo);
            if (enumAppInfo != null)
                appInfo.AddRange(enumAppInfo);
            return appInfo;
        }

        private static List<AppData> ApproveAppInfoDat()
        {
            if (ActionGuid.IsUpdateInstance || !File.Exists(CacheFiles.AppInfo))
                return ResetAppInfoDat();
            try
            {
                // Reset if the cached file is older than 30 minutes.
                var fileDate = File.GetLastWriteTime(CacheFiles.AppInfo);
                if ((DateTime.Now - fileDate).TotalMinutes > 30d)
                    return ResetAppInfoDat();

                // Otherwise take data from cache.
                var appInfo = LoadDat<List<AppData>>(CacheFiles.AppInfo);
                if (appInfo == default)
                    throw new ArgumentNullException(nameof(appInfo));
                if (appInfo.Count < 500)
                    throw new ArgumentOutOfRangeException(nameof(appInfo));
                return appInfo;
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                if (Log.DebugMode > 1)
                    Log.Write(ex);
            }
            return new List<AppData>();
        }

        private static List<AppData> ResetAppInfoDat()
        {
            FileEx.TryDelete(CacheFiles.AppInfo);
            return new List<AppData>();
        }

        private static IEnumerable<AppData> CustomAppSupplierInfoCollector(IReadOnlyList<AppData> current)
        {
            if (CustomAppSuppliers?.Any() != true)
                yield break;
            foreach (var data in CustomAppSuppliers)
            {
                var address = data.Address;
                var url = PathEx.AltCombine(default(char[]), address, "AppInfo.ini");
                var user = data.User;
                var password = data.Password;
                var userAgent = data.UserAgent;
                if (Log.DebugMode > 0)
                    Log.Write($"Custom: Looking for '{url}'.");
                if (!NetEx.FileIsAvailable(url, user, password, 60000, userAgent))
                    continue;
                var customAppInfo = WebTransfer.DownloadString(url, user, password, 60000, userAgent);
                if (string.IsNullOrWhiteSpace(customAppInfo))
                    continue;
                foreach (var instance in AppInfoCollector(current, customAppInfo, data))
                    yield return instance;
            }
        }

        private static IEnumerable<AppData> AppInfoCollector(IReadOnlyList<AppData> current, string config, CustomAppSupplier supplier = default)
        {
            var sections = Ini.GetSections(config, false);

            // Parsing the filter lists.
            var sectionEqualsThenSkip = Array.Empty<string>();
            var sectionContainsThenSkip = Array.Empty<string>();
            var anyContainsThenIsAdvanced = Array.Empty<string>();
            var anyContainsThenIsNotAdvanced = Array.Empty<string>();
            var nameStartsWithThenReplace = Array.Empty<string>();
            if (supplier == default)
            {
                if (AppInfoFilters == default)
                {
                    var filterKeys = new[]
                    {
                        "SectionEqualsThenSkip",
                        "SectionContainsThenSkip",
                        "AnyContainsThenIsAdvanced",
                        "AnyContainsThenIsNotAdvanced",
                        "NameStartsWithThenReplace"
                    };
                    var filters = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                    foreach (var section in sections.Where(x => filterKeys.Contains(x)))
                    {
                        var value = Ini.Read(section, "Filter", config);
                        if (!value.StartsWithEx('[') || !value.EndsWithEx(']'))
                            continue;
                        var array = JsonConvert.DeserializeObject<string[]>(value);
                        if (Log.DebugMode > 0 && array?.Length < 1)
                            Log.Write($"Cache: Could not parse filters from '{section}'.");
                        filters.Add(section, JsonConvert.DeserializeObject<string[]>(value));
                    }
                    AppInfoFilters = filters;
                }
                AppInfoFilters?.TryGetValue("SectionEqualsThenSkip", out sectionEqualsThenSkip);
                AppInfoFilters?.TryGetValue("SectionContainsThenSkip", out sectionContainsThenSkip);
                AppInfoFilters?.TryGetValue("AnyContainsThenIsAdvanced", out anyContainsThenIsAdvanced);
                AppInfoFilters?.TryGetValue("AnyContainsThenIsNotAdvanced", out anyContainsThenIsNotAdvanced);
                AppInfoFilters?.TryGetValue("NameStartsWithThenReplace", out nameStartsWithThenReplace);
            }

            var merged = 0;
            foreach (var section in sections)
            {
                if (sectionEqualsThenSkip.ContainsItem(section))
                {
                    if (Log.DebugMode > 0)
                        Log.Write($"Cache: Section '{section}' skipped because it is blacklisted.");
                    continue;
                }
                if (section.ContainsEx(sectionContainsThenSkip))
                {
                    if (Log.DebugMode > 0)
                        Log.Write($"Cache: Section '{section}' skipped because it contains blacklisted keywords.");
                    continue;
                }
                if (current.Any(x => x.Key.EqualsEx(section)))
                {
                    if (Log.DebugMode > 0)
                        Log.Write($"Cache: Section '{section}' skipped because an app with the same section already exists.");
                    continue;
                }

                // Parsing the app`s full name.
                var name = Ini.Read(section, "Name", config);
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                for (var i = 0; i < nameStartsWithThenReplace.Length - 1; i++)
                {
                    var j = i + 1;
                    if (j >= nameStartsWithThenReplace.Length)
                        continue;
                    var pattern = nameStartsWithThenReplace[i];
                    var replace = nameStartsWithThenReplace[j];
                    if (!name.StartsWith(pattern))
                        continue;
                    name = Regex.Replace(name, Regex.Escape(pattern), replace);
                    break;
                }

                if (section.ContainsEx("Legacy") && !name.ContainsEx("Legacy"))
                    name += " Legacy";

                if (!name.StartsWithEx(AppSupplierHosts.Pac))
                {
                    var newName = name.RemoveTextIgnoreCase(", Portable Edition", "Portable64", "Portable");
                    if (!string.IsNullOrWhiteSpace(newName))
                        newName = newName.ReduceWhiteSpace().TrimEnd(' ', ',');
                    if (!string.IsNullOrWhiteSpace(newName) && !newName.Equals(name, StringComparison.Ordinal))
                        name = newName;
                }

                // Parsing the app`s description.
                var description = Ini.Read(section, "Description", config);
                if (string.IsNullOrWhiteSpace(description))
                    continue;
                description = section switch
                {
                    "Java" => description.ToLowerInvariant(),
                    "Java64" => description.ToLowerInvariant(),
                    "JDK" => description.ToLowerInvariant(),
                    "JDK64" => description.ToLowerInvariant(),
                    "LibreCADPortable" => description.LowerText("tool"),
                    "Mp3spltPortable" => description.UpperText("mp3", "ogg"),
                    "SumatraPDFPortable" => description.LowerText("comic", "book", "e-", "reader"),
                    "WinCDEmuPortable" => description.UpperText("cd/dvd/bd"),
                    "WinDjViewPortable" => description.UpperText("djvu"),
                    _ => description
                };
                description = $"{description.Substring(0, 1).ToUpperInvariant()}{description.Substring(1)}";

                // Parsing the app`s category.
                var category = Ini.Read(section, "Category", config)?.ReduceWhiteSpace().Replace("&", "and");
                if (supplier != default)
                {
                    const string custom = "#Custom";
                    category = category.Trim('*', '#');
                    if (string.IsNullOrWhiteSpace(category))
                        category = custom;
                    if (!category.StartsWith(custom, StringComparison.Ordinal))
                        category = $"{custom}: {category}";
                }

                if (string.IsNullOrWhiteSpace(category))
                    continue;

                // Parsing the app`s website.
                var website = Ini.Read(section, "Website", config);
                if (string.IsNullOrWhiteSpace(website))
                    website = Ini.Read(section, "URL", config).ToLowerInvariant().Replace("https", "http");
                if (string.IsNullOrWhiteSpace(website) || website.Any(char.IsUpper))
                    website = default;

                // Parsing the app`s version data.
                var emptyVersion = Version.Parse("0.0.0.0");
                var defaultVersion = Version.Parse("1.0.0.0");
                var displayVersion = Ini.Read(section, "Version", config);
                var packageVersion = default(Version);
                var versionDataList = new List<Tuple<string, string>>();
                if (!string.IsNullOrWhiteSpace(displayVersion))
                {
                    if (!ActionGuid.IsUpdateInstance)
                        packageVersion = defaultVersion;
                    else
                    {
                        if (char.IsDigit(displayVersion.FirstOrDefault()))
                            try
                            {
                                var version = displayVersion;
                                while (version.Count(x => x == '.') < 3)
                                    version += ".0";
                                packageVersion = Version.Parse(version);
                            }
                            catch (Exception ex) when (ex.IsCaught())
                            {
                                packageVersion = defaultVersion;
                            }
                        else
                            packageVersion = emptyVersion;

                        var verData = Ini.Read(section, "VersionData", config);
                        if (!string.IsNullOrWhiteSpace(verData))
                        {
                            var verHash = Ini.Read(section, "VersionHash", config);
                            if (!string.IsNullOrWhiteSpace(verHash))
                            {
                                if (!verData.Contains(',') || !verHash.Contains(','))
                                    versionDataList.Add(Tuple.Create(verData, verHash));
                                else
                                {
                                    var dataArray = verData.Split(',');
                                    var hashArray = verHash.Split(',');
                                    if (dataArray.Length == hashArray.Length)
                                        versionDataList.AddRange(dataArray.Select((data, i) => Tuple.Create(data, hashArray[i])));
                                }
                            }
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(displayVersion) || packageVersion == default)
                {
                    displayVersion = Ini.Read(section, "DisplayVersion", config);
                    if (displayVersion.ContainsEx("Discontinued") && !name.ContainsEx("Discontinued"))
                        name += " (Discontinued)";
                    packageVersion = Ini.Read(section, "PackageVersion", default(Version), config);
                }

                var versionData = new ReadOnlyCollection<Tuple<string, string>>(versionDataList);

                // Parsing the app`s download links and hashes.
                var path1 = Ini.Read(section, "ArchivePath", config);
                var path2 = default(string);
                string hash;
                if (!string.IsNullOrWhiteSpace(path1))
                {
                    if (path1.StartsWithEx(".free", ".repack"))
                        path1 = PathEx.AltCombine(AppSupplierMirrors.Pa.Keys.First(), path1);
                    hash = Ini.Read(section, "ArchiveHash", config);
                }
                else
                {
                    var path = Ini.Read(section, "DownloadPath", config);
                    var file = Ini.Read(section, "DownloadFile", config);
                    path1 = PathEx.AltCombine(default(char[]), LocalGetAbsoluteUrl(path, section), file);
                    path2 = PathEx.AltCombine(default(char[]), path, file);
                    if (!path1.EndsWithEx(".paf.exe"))
                        continue;
                    hash = Ini.Read(section, "Hash", config).Trim();
                    if (hash.ContainsEx(' ', '\t'))
                        hash = hash.Split(' ', '\t').FirstOrDefault(x => x.Length == Crypto.Sha256.HashLength);
                }
                switch (hash?.Length)
                {
                    case Crypto.Md5.HashLength:
                    case Crypto.Sha1.HashLength:
                    case Crypto.Sha256.HashLength:
                    case Crypto.Sha384.HashLength:
                    case Crypto.Sha512.HashLength:
                        break;
                    default:
                        hash = "None";
                        break;
                }
                if (string.IsNullOrWhiteSpace(path1) || string.IsNullOrWhiteSpace(hash))
                    continue;

                var defLanguage = path1.ContainsEx("Multilingual") ? "Multilingual" : path1.ContainsEx("English") ? "English" : "Default";
                var downloadDict = new Dictionary<string, List<Tuple<string, string>>>
                {
                    {
                        defLanguage,
                        new List<Tuple<string, string>>
                        {
                            Tuple.Create(path1, hash)
                        }
                    }
                };
                if (path2.StartsWithEx("http") && !path2.EqualsEx(path1))
                    downloadDict[defLanguage].Add(Tuple.Create(path2, hash));

                foreach (var lang in LocalGetInstallerLangs())
                {
                    var langFile = Ini.Read(section, $"DownloadFile_{lang}", config);
                    if (!langFile.EndsWithEx(".paf.exe"))
                        continue;

                    var langPath = Ini.Read(section, "DownloadPath", config);
                    var langPath1 = PathEx.AltCombine(default(char[]), LocalGetAbsoluteUrl(langPath, section), langFile);
                    if (!langPath1.EndsWithEx(".paf.exe"))
                        continue;

                    var langHash = Ini.Read(section, $"Hash_{lang}", config);
                    if (string.IsNullOrWhiteSpace(langHash) || langHash.EqualsEx(hash))
                        continue;

                    downloadDict.Add(lang, new List<Tuple<string, string>>
                    {
                        Tuple.Create(langPath1, langHash)
                    });
                    var langPath2 = PathEx.AltCombine(default(char[]), langPath, langFile);
                    if (langPath2.StartsWithEx("http") && !langPath2.EqualsEx(langPath1))
                        downloadDict[lang].Add(Tuple.Create(langPath2, langHash));
                }
                var downloadCollection = new ReadOnlyDictionary<string, ReadOnlyCollection<Tuple<string, string>>>(downloadDict.ToDictionary(x => x.Key, x => new ReadOnlyCollection<Tuple<string, string>>(x.Value)));

                downloadDict.Clear();
                if (ActionGuid.IsUpdateInstance)
                {
                    downloadDict = new Dictionary<string, List<Tuple<string, string>>>();
                    foreach (var key in Ini.GetKeys(section, config, false))
                    {
                        if (!key.StartsWithEx("UpdateFrom") && !key.EndsWithEx("DownloadFile"))
                            continue;
                        var topKey = key.RemoveText("DownloadFile");
                        var version = topKey.RemoveText("UpdateFrom").Trim('_', '-');
                        if (string.IsNullOrEmpty(version))
                            continue;
                        var path = Ini.Read(section, $"{topKey}DownloadPath", config);
                        var file = Ini.Read(section, key, config);
                        path1 = PathEx.AltCombine(default(char[]), LocalGetAbsoluteUrl(path, section), file);
                        path2 = PathEx.AltCombine(default(char[]), path, file);
                        if (!path1.EndsWithEx(".paf.exe"))
                            continue;
                        hash = Ini.Read(section, $"{topKey}Hash", config);
                        if (string.IsNullOrWhiteSpace(path1) || string.IsNullOrWhiteSpace(hash))
                            continue;
                        downloadDict.Add(version, new List<Tuple<string, string>>
                        {
                            Tuple.Create(path1, hash)
                        });
                        if (path2.StartsWithEx("http") && !path2.EqualsEx(path1))
                            downloadDict[version].Add(Tuple.Create(path2, hash));
                    }
                }
                var updateCollection = default(ReadOnlyDictionary<string, ReadOnlyCollection<Tuple<string, string>>>);
                if (ActionGuid.IsUpdateInstance)
                    updateCollection = new ReadOnlyDictionary<string, ReadOnlyCollection<Tuple<string, string>>>(downloadDict.ToDictionary(x => x.Key, x => new ReadOnlyCollection<Tuple<string, string>>(x.Value)));

                // Parsing the app`s download size.
                var downloadSize = Ini.Read(section, "DownloadSize", 1L, config) * 1024 * 1024;
                if (downloadSize < 0x100000)
                    downloadSize = 0x100000;

                // Parsing the app`s install size.
                var installSize = Ini.Read(section, "InstallSizeTo", 0L, config);
                if (installSize == 0)
                    installSize = Ini.Read(section, "InstallSize", 1L, config);
                installSize = installSize * 1024 * 1024;
                if (installSize < 0x100000)
                    installSize = 0x100000;

                // Parsing the app`s requirements.
                var requires = Ini.Read(section, "Requires", default(string), config);
                if (string.IsNullOrWhiteSpace(requires))
                {
                    if (section == "jPortableBrowserSwitch")
                        requires = "Java|Java64";
                    else
                    {
                        foreach (var reqirement in from keyword in anyContainsThenIsNotAdvanced
                                                   where section.EndsWithEx(keyword)
                                                   select section.RemoveText(keyword)
                                                   into reqirement
                                                   where sections.Contains(reqirement)
                                                   select reqirement)
                        {
                            requires = reqirement;
                            break;
                        }
                    }
                }
                var requiresList = new List<string>();
                if (!string.IsNullOrEmpty(requires))
                {
                    if (!requires.Contains(","))
                        requires += ",";
                    foreach (var str in requires.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        var value = str;
                        if (!value.Contains("|"))
                            value += "|";
                        var items = value.Split('|');
                        string item;
                        if (Environment.Is64BitOperatingSystem && items.Any(x => x.EndsWith("64", StringComparison.Ordinal)))
                            item = items.FirstOrDefault(x => x.EndsWith("64", StringComparison.Ordinal))?.Trim();
                        else
                            item = items.FirstOrDefault(x => !x.EndsWith("64", StringComparison.Ordinal))?.Trim();
                        if (string.IsNullOrEmpty(item))
                            continue;
                        requiresList.Add(item);
                    }
                }
                var requirements = new ReadOnlyCollection<string>(requiresList);

                // Parsing the app`s package release and update dates
                var releaseDate = Ini.Read(section, "ReleaseDate", DateTime.MinValue, config);
                var updateDate = Ini.Read(section, "UpdateDate", DateTime.MinValue, config);

                // Parsing the app`s package installer version
                var installerVersion = Ini.Read(section, "InstallerVersion", emptyVersion, config);
                if (installerVersion == emptyVersion)
                    installerVersion = Ini.Read(section, "PAcInstaller", emptyVersion, config);

                // Determines whether the app should be listed in the Advanced category.
                var advanced = Ini.Read(section, "Advanced", false, config);
                if (!advanced)
                    advanced = section.ContainsEx(anyContainsThenIsAdvanced) ||
                               name.ContainsEx(anyContainsThenIsAdvanced) ||
                               displayVersion.ContainsEx(anyContainsThenIsAdvanced);
                else
                {
                    if (section.ContainsEx(anyContainsThenIsNotAdvanced) ||
                        name.ContainsEx(anyContainsThenIsNotAdvanced) ||
                        displayVersion.ContainsEx(anyContainsThenIsNotAdvanced))
                        advanced = false;
                }

                // Finally, create the AppData instance with all the parsed app information.
                yield return new AppData(section,
                                         name,
                                         description,
                                         category,
                                         website,
                                         displayVersion,
                                         packageVersion,
                                         versionData,
                                         downloadCollection,
                                         updateCollection,
                                         downloadSize,
                                         installSize,
                                         requirements,
                                         releaseDate,
                                         updateDate,
                                         installerVersion,
                                         advanced,
                                         supplier);
                merged++;
            }

            Ini.Detach(config);

            if (Log.DebugMode > 0)
                Log.Write($"Cache: Merged '{merged}' apps from {(File.Exists(config) ? $"'{config}'" : "custom supplier")} with '{CacheFiles.AppInfo}'.");

            yield break;

            static string LocalGetAbsoluteUrl(string url, string key = null)
            {
                var realUrl = url;
                var redirect = realUrl.ContainsEx("/redirect/");
                if (string.IsNullOrWhiteSpace(realUrl) || (redirect && realUrl.ContainsEx("&d=sfpa")))
                    realUrl = PathEx.AltCombine(default(char[]), "https:", $"downloads.{AppSupplierHosts.Sf}", "portableapps");
                else if (redirect && realUrl.ContainsEx("&d=pa&f="))
                    realUrl = PathEx.AltCombine(default(char[]), "https:", $"{AppSupplierHosts.Pac}", "portableapps", key);
                if (!url.EqualsEx(realUrl))
                    return realUrl;
                if (redirect)
                {
                    var filter = WebUtility.UrlDecode(realUrl)?
                                           .RemoveChar(':')
                                           .Replace("https", "http")
                                           .Split("http/")?
                                           .Last()?
                                           .RemoveText("/&d=pb&f=")
                                           .RemoveText("/&f=").Trim('/');
                    if (!string.IsNullOrEmpty(filter))
                        realUrl = PathEx.AltCombine(default(char[]), "https:", filter);
                }
                if (!realUrl.ContainsEx(AppSupplierHosts.Pac))
                    return realUrl;
                var mirrors = AppSupplierMirrors.Pac.Keys;
                var first = mirrors.First();
                return mirrors.Aggregate(realUrl, (c, m) => c.Replace(m, first));
            }

            static IEnumerable<string> LocalGetInstallerLangs() => new[]
            {
                "Afrikaans",
                "Albanian",
                "Arabic",
                "Armenian",
                "Basque",
                "Belarusian",
                "Bulgarian",
                "Catalan",
                "Croatian",
                "Czech",
                "Danish",
                "Dutch",
                "EnglishGB",
                "Estonian",
                "Farsi",
                "Filipino",
                "Finnish",
                "French",
                "Galician",
                "German",
                "Greek",
                "Hebrew",
                "Hungarian",
                "Indonesian",
                "Irish",
                "Italian",
                "Japanese",
                "Korean",
                "Latvian",
                "Lithuanian",
                "Luxembourgish",
                "Macedonian",
                "Malay",
                "Norwegian",
                "Polish",
                "Portuguese",
                "PortugueseBR",
                "Romanian",
                "Russian",
                "Serbian",
                "SerbianLatin",
                "SimpChinese",
                "Slovak",
                "Slovenian",
                "Spanish",
                "SpanishInternational",
                "Sundanese",
                "Swedish",
                "Thai",
                "TradChinese",
                "Turkish",
                "Ukrainian",
                "Vietnamese"
            };
        }
    }
}
