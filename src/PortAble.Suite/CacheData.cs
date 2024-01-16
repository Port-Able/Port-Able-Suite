namespace PortAble;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
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
    private static List<string> _appDirs;
    private static Dictionary<string, Image> _appImages;
    private static Dictionary<string, Image> _appImagesLarge;
    private static List<AppData> _appInfo;
    private static Dictionary<string, string> _appSuppliers;
    private static Dictionary<string, Image> _currentAppImages;
    private static List<LocalAppData> _currentAppInfo;
    private static Dictionary<string, AppSettings> _currentAppSettings;
    private static Image _currentBackgroundImage;
    private static List<CustomAppSupplier> _customAppSuppliers;
    private static Dictionary<int, List<string>> _nsisButtons;

    /// <summary>
    ///     A collection of subdirectories where installed apps are located.
    /// </summary>
    public static IReadOnlyList<string> AppDirs => _appDirs ??= InitAppDirs();

    /// <summary>
    ///     Gets a collection of <see cref="AppData"/> instances for all apps.
    /// </summary>
    public static IReadOnlyList<AppData> AppInfo => _appInfo ??= InitAppInfo();

    /// <summary>
    ///     Gets a collection of filters that applies when creating
    ///     <see cref="AppInfo"/>.
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> AppInfoFilters { get; private set; }

    /// <summary>
    ///     Gets the database containing small app images of all apps.
    /// </summary>
    public static IReadOnlyDictionary<string, Image> AppImages =>
        _appImages ??= InitFromDat<Dictionary<string, Image>>(CachePaths.AppImages,
                                                              CorePaths.AppImages,
                                                              StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Gets the database containing large app images of all apps.
    /// </summary>
    public static IReadOnlyDictionary<string, Image> AppImagesLarge =>
        _appImagesLarge ??= InitFromDat<Dictionary<string, Image>>(CachePaths.AppImagesLarge,
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
    ///         <see langword="this"/>["HTTPS_ADDRESS"] == "SERVER_INFO"
    ///     </code>
    /// </summary>
    public static IReadOnlyDictionary<string, string> AppSuppliers =>
        _appSuppliers ??= LoadDat<Dictionary<string, string>>(CachePaths.AppSuppliers);

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
    public static IReadOnlyDictionary<int, List<string>> NsisButtons =>
        _nsisButtons ??= InitFromDat<Dictionary<int, List<string>>>(CachePaths.NsisButtons,
                                                                    CorePaths.NsisButtons);

    /// <summary>
    ///     Gets a collection of <see cref="LocalAppData"/> instances for installed
    ///     apps.
    /// </summary>
    public static IReadOnlyList<LocalAppData> CurrentAppInfo => _currentAppInfo ??= InitCurrentAppInfo(null);

    /// <summary>
    /// </summary>
    public static Dictionary<string, Image> CurrentAppImages =>
        _currentAppImages ??= LoadDat<Dictionary<string, Image>>(CachePaths.CurrentAppImages) ?? new Dictionary<string, Image>();

    /// <summary>
    /// </summary>
    public static IReadOnlyDictionary<string, AppSettings> CurrentAppSettings =>
        _currentAppSettings ??= CurrentAppInfo?.ToDictionary(x => x.Key, x => x.Settings);

    /// <summary>
    /// </summary>
    public static Image CurrentBackgroundImage
    {
        get
        {
            if (_currentBackgroundImage != default)
                return _currentBackgroundImage;
            if (!File.Exists(CachePaths.CurrentBackgroundImage))
                return default;
            var image = FileEx.Deserialize<Image>(CachePaths.CurrentBackgroundImage);
            Interlocked.CompareExchange(ref _currentBackgroundImage, image, default);
            return _currentBackgroundImage;
        }
        set
        {
            Interlocked.Exchange(ref _currentBackgroundImage, value);
            FileEx.TryDelete(CachePaths.CurrentBackgroundImage);
            if (value != default)
                SaveDat(value, CachePaths.CurrentBackgroundImage, false);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="appKeyOrName">
    /// </param>
    /// <returns>
    /// </returns>
    public static LocalAppData FindInCurrentAppInfo(string appKeyOrName)
    {
        if (CurrentAppInfo.Count < 1 || string.IsNullOrWhiteSpace(appKeyOrName))
            return default;
        return CurrentAppInfo.FirstOrDefault(x => appKeyOrName.EqualsEx(x.Key, x.Name));
    }

    /// <summary>
    /// </summary>
    public static void ResetCurrentAppInfo()
    {
        FileEx.TryDelete(CachePaths.CurrentAppImages);
        FileEx.TryDelete(CachePaths.CurrentAppInfo);
        _currentAppInfo = default;
        _currentAppSettings = default;
    }

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
    public static bool SaveDat<T>(T source, string destination, bool compress = true) =>
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
    public static T LoadDat<T>(string path, T defValue = default)
    {
        if (Log.DebugMode > 1)
            Log.Write($"Cache: Load file from '{path}'.");
        return FileEx.Deserialize(path, defValue);
    }

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
    /// </summary>
    /// <param name="dirs">
    /// </param>
    public static void UpdateAppDirs(string dirs)
    {
        if (string.IsNullOrWhiteSpace(dirs))
            return;
        UpdateAppDirs(dirs.SplitNewLine());
    }

    /// <inheritdoc cref="UpdateAppDirs(string)"/>
    public static void UpdateAppDirs(string[] dirs)
    {
        var list = new List<string>();
        list.AddRange(CorePaths.AppEnvDirs);
        if (dirs != null)
        {
            var array = dirs.Select(PathEx.Combine).Where(PathEx.IsValidPath).ToArray();
            if (array.Length > 0)
                list.AddRange(array.Select(x => EnvironmentEx.GetVariableWithPath(x)));
        }
        if (Log.DebugMode > 0)
            Log.Write($"App Dirs: '{list.Join("', '")}' saved.");
        SaveDat(list, CachePaths.AppDirs);
        _appDirs = null;
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
    ///     Recolors the black pixels of the specified component`s image to the
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
                item.Image = GetImage(item.Name, item.Image, color, redraw, Math.Min(item.Width, item.Height) - 8);
                break;
            case ToolStripMenuItem { Image: not null } item:
                item.Image = GetImage(item.Name, item.Image, color, redraw, Math.Min(item.Width, item.Height));
                break;
            case Control { BackgroundImage: not null } item:
                item.BackgroundImage = GetImage(item.Name, item.BackgroundImage, color, redraw, Math.Min(item.Width, item.Height));
                break;
        }
    }

    /// <summary>
    ///     Recolors the black pixels of the specified component`s image to the
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

    /// <summary>
    /// </summary>
    /// <param name="key">
    /// </param>
    /// <param name="image">
    /// </param>
    /// <param name="color">
    /// </param>
    /// <param name="redraw">
    /// </param>
    /// <param name="indicator">
    /// </param>
    /// <returns>
    /// </returns>
    public static Image GetImage(string key, Image image, Color color, bool redraw = false, int indicator = 1024)
    {
        var colorNotEmpty = color != Color.Empty && color != Color.Transparent;
        if (colorNotEmpty)
            key += color;
        var cached = ImageEx.Cache(key);
        if (cached != default)
            return cached;
        if (image == default)
            return default;
        if (colorNotEmpty)
            image = image.RecolorPixels(Color.Black, color);
        if (redraw)
            image = image.Redraw(indicator);
        return image.Cache(key);
    }

    /// <inheritdoc cref="GetImage(string, Image, Color, bool, int)"/>
    public static Image GetImage(string key, Image image, Color color, int indicator) =>
        GetImage(key, image, SystemColors.Highlight, false, indicator);

    /// <inheritdoc cref="GetImage(string, Image, Color, bool, int)"/>
    public static Image GetImage(string key, Color color, bool redraw = false, int indicator = 1024) =>
        GetImage(key, default, color, redraw, indicator);

    /// <inheritdoc cref="GetImage(string, Image, Color, bool, int)"/>
    public static Image GetImage(string key, bool redraw = false, int indicator = 1024) =>
        GetImage(key, default, SystemColors.Highlight, redraw, indicator);

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

    private static List<string> InitAppDirs()
    {
        var appDirs = LoadDat<List<string>>(CachePaths.AppDirs)?.Select(PathEx.Combine).Distinct().ToList();
        return appDirs?.Count is null or < 4 ? CorePaths.AppDirs.ToList() : appDirs;
    }

    private static List<AppData> InitAppInfo()
    {
        if (!DirectoryEx.Create(CachePaths.TransferDir))
            throw new IOException();

        IEnumerable<AppData> enumAppInfo;
        var appInfo = ApproveAppInfoDat();
        if (appInfo.Count > 0)
        {
            enumAppInfo = CustomAppSupplierInfoCollector(appInfo);
            if (enumAppInfo != null)
                appInfo.AddRange(enumAppInfo);
            return appInfo;
        }

        // Download all the data from PA servers.
        var pa = Path.Combine(CachePaths.TransferDir, "AppInfo.ini");
        var pac = Path.Combine(CachePaths.TransferDir, "AppInfo.7z");
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

            using (var process = SevenZip.DefaultArchiver.Extract(pac, CachePaths.TransferDir))
            {
                if (process?.HasExited == false)
                    process.WaitForExit();
                if (FileEx.TryDelete(pac) && Log.DebugMode > 0)
                    Log.Write($"Cache: '{pac}' deleted.");
            }

            pac = DirectoryEx.EnumerateFiles(CachePaths.TransferDir, "*.ini").FirstOrDefault(x => !x.EqualsEx(pa));
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
        if (appInfo.Count > 0)
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
                SaveDat(appImagesLarge, Path.ChangeExtension(CachePaths.AppImagesLarge, "cleaned.dat"));
                SaveDat(appImages, Path.ChangeExtension(CachePaths.AppImages, "cleaned.dat"));
                break;
            }

            // Finally write the updated data to cache.
            default:
                SaveDat(appInfo, CachePaths.AppInfo);
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
        if (ActionGuid.IsUpdateInstance || !File.Exists(CachePaths.AppInfo))
            return ResetAppInfoDat();
        try
        {
            // Reset if the cached file is older than 30 minutes.
            var fileDate = File.GetLastWriteTime(CachePaths.AppInfo);
            if ((DateTime.Now - fileDate).TotalMinutes > 30d)
                return ResetAppInfoDat();

            // Otherwise take data from cache.
            var appInfo = LoadDat<List<AppData>>(CachePaths.AppInfo);
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
        FileEx.TryDelete(CachePaths.AppInfo);
        return new List<AppData>();
    }

    private static IEnumerable<AppData> CustomAppSupplierInfoCollector(IReadOnlyList<AppData> current)
    {
        if (CustomAppSuppliers?.Count is null or < 1)
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
        const string sectionEqualsThenSkipKey = "SectionEqualsThenSkip";
        const string sectionContainsThenSkipKey = "SectionContainsThenSkip";
        const string anyContainsThenIsAdvancedKey = "AnyContainsThenIsAdvanced";
        const string anyContainsThenIsNotAdvancedKey = "AnyContainsThenIsNotAdvanced";
        const string nameStartsWithThenReplaceKey = "NameStartsWithThenReplace";
        var filters = default(Dictionary<string, string[]>);
        if (supplier == default)
        {
            filters = AppInfoFilters as Dictionary<string, string[]>;
            if (filters == default)
            {
                var filterKeys = new[]
                {
                    sectionEqualsThenSkipKey,
                    sectionContainsThenSkipKey,
                    anyContainsThenIsAdvancedKey,
                    anyContainsThenIsNotAdvancedKey,
                    nameStartsWithThenReplaceKey
                };
                filters = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                foreach (var section in sections.Where(x => filterKeys.Contains(x)))
                {
                    var value = Ini.Read(section, "Filter", config);
                    if (!value.StartsWith("[") || !value.EndsWith("]"))
                        continue;
                    var array = JsonConvert.DeserializeObject<string[]>(value);
                    if (Log.DebugMode > 0 && array?.Length < 1)
                        Log.Write($"Cache: Could not parse filters from '{section}'.");
                    filters.Add(section, array);
                }
                AppInfoFilters = filters;
            }
        }
        var sectionEqualsThenSkip = filters?.TryGetValue(sectionEqualsThenSkipKey) ?? Array.Empty<string>();
        var sectionContainsThenSkip = filters?.TryGetValue(sectionContainsThenSkipKey) ?? Array.Empty<string>();
        var anyContainsThenIsAdvanced = filters?.TryGetValue(anyContainsThenIsAdvancedKey) ?? Array.Empty<string>();
        var anyContainsThenIsNotAdvanced = filters?.TryGetValue(anyContainsThenIsNotAdvancedKey) ?? Array.Empty<string>();
        var nameStartsWithThenReplace = filters?.TryGetValue(nameStartsWithThenReplaceKey) ?? Array.Empty<string>();

        string defUserAgent;
        if (!string.IsNullOrEmpty(supplier?.UserAgent))
            defUserAgent = supplier.UserAgent;
        else if (sections.Contains(nameStartsWithThenReplaceKey))
            defUserAgent = UserAgents.Pa;
        else
            defUserAgent = UserAgents.Wget;

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
            if (string.IsNullOrWhiteSpace(website) || !website.StartsWithEx("https:", "http:"))
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
            if (path2.StartsWithEx("https:", "http:") && !path2.EqualsEx(path1))
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

            // Parsing the app`s package release and update dates.
            var releaseDate = Ini.Read(section, "ReleaseDate", DateTime.MinValue, config);
            var updateDate = Ini.Read(section, "UpdateDate", DateTime.MinValue, config);

            // Parsing the app`s package installer version.
            var installerVersion = Ini.Read(section, "InstallerVersion", emptyVersion, config);
            if (installerVersion == emptyVersion)
                installerVersion = Ini.Read(section, "PAcInstaller", emptyVersion, config);

            // Parsing the app`s user agent.
            var userAgent = Ini.Read(section, "UserAgent", defUserAgent, config);

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
                                     userAgent,
                                     advanced,
                                     supplier);

            if (Log.DebugMode > 0)
                merged++;
        }

        Ini.Detach(config);

        if (Log.DebugMode > 0)
            Log.Write($"Cache: Merged '{merged}' apps from {(File.Exists(config) ? $"'{config}'" : "custom supplier")} with '{CachePaths.AppInfo}'.");

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

    private static List<LocalAppData> InitCurrentAppInfo(IReadOnlyCollection<string> furtherAppInstallLocations)
    {
        var appInfo = LoadDat(CachePaths.CurrentAppInfo, new List<LocalAppData>());

        var dirs = AppDirs.Select(PathEx.Combine).Where(Directory.Exists).ToList();
        if (furtherAppInstallLocations?.Count is > 0)
        {
            var furtherDirs = furtherAppInstallLocations.Where(Directory.Exists).Select(PathEx.Combine).AsArray();
            if (furtherDirs.Length > 0)
                dirs.AddRange(furtherDirs);
        }

        if (dirs.Count < 1)
            return appInfo;

        var nameRegex = default(Regex);
        var writeToFile = false;
        foreach (var dir in dirs.SelectMany(x => DirectoryEx.EnumerateDirectories(x)))
        {
            var key = Path.GetFileName(dir);
            if (string.IsNullOrEmpty(key) || !key.ContainsEx("Portable") || appInfo.Any(x => x.Key == key))
                continue;

            var current = appInfo.FirstOrDefault(x => x.Key.EqualsEx(key));
            if (current != default && File.Exists(current.ExecutablePath))
            {
                appInfo.Add(current);
                continue;
            }

            // Try to get the best file path.
            var exePath = Path.Combine(dir, $"{key}.exe");
            var configPath = Path.Combine(dir, $"{key}.ini");
            var appInfoPath = Path.Combine(dir, "App\\AppInfo\\appinfo.ini");
            if (File.Exists(configPath) || File.Exists(appInfoPath))
            {
                var exeName = Ini.Read("AppInfo", "File", configPath);
                if (string.IsNullOrEmpty(exeName))
                    exeName = Ini.Read("Control", "Start", appInfoPath);
                if (!string.IsNullOrEmpty(exeName))
                {
                    var dirPath = Ini.Read("AppInfo", "Dir", configPath);
                    if (string.IsNullOrEmpty(dirPath))
                    {
                        exePath = Path.Combine(dir, exeName);
                        if (!File.Exists(configPath))
                            configPath = exePath.Replace(".exe", ".ini");
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
                        exePath = PathEx.Combine(dirPath, exeName);
                    }
                }
            }
            if (!File.Exists(exePath))
                exePath = Path.Combine(dir, $"{key}.exe");
            if (!File.Exists(exePath))
                continue;

            // Try to get the full app name.
            var name = Ini.Read("AppInfo", "Name", configPath);
            if (string.IsNullOrWhiteSpace(name))
                name = Ini.Read("Details", "Name", appInfoPath);
            if (string.IsNullOrWhiteSpace(name))
                name = FileVersionInfo.GetVersionInfo(exePath).FileDescription;
            if (string.IsNullOrWhiteSpace(name))
                continue;

            // Try to get the app description.
            var description = Ini.Read("AppInfo", "Description", configPath);
            if (string.IsNullOrWhiteSpace(description))
                description = Ini.Read("Details", "Description", appInfoPath);
            if (string.IsNullOrWhiteSpace(description))
                description = name;

            // Try to get the app category.
            var category = Ini.Read("AppInfo", "Category", configPath);
            if (string.IsNullOrWhiteSpace(category))
                category = Ini.Read("Details", "Category", appInfoPath);
            if (string.IsNullOrWhiteSpace(category))
                category = "#";

            // Apply some filters to the found app name.
            nameRegex ??= new Regex("(PortableApps.com Launcher)|, Portable Edition|Portable64|Portable", RegexOptions.IgnoreCase);
            var newName = nameRegex.Replace(name, string.Empty).Replace("\t", " ");
            newName = Regex.Replace(newName.Trim(' ', ','), " {2,}", " ");
            if (!name.Equals(newName, StringComparison.Ordinal))
                name = newName;

            if (string.IsNullOrWhiteSpace(name) || !File.Exists(exePath) || appInfo.Any(x => x.Name.EqualsEx(name)))
                continue;

            // Disable splash screen if needed.
            var sourceDir = Path.Combine(dir, "Other\\Source");
            if (Directory.Exists(sourceDir))
            {
                var srcConfigPath = Path.Combine(sourceDir, "AppNamePortable.ini");
                if (!File.Exists(srcConfigPath))
                    srcConfigPath = Path.Combine(sourceDir, $"{key}.ini");
                if (!File.Exists(srcConfigPath))
                    srcConfigPath = DirectoryEx.EnumerateFiles(sourceDir, "*.ini")
                                               .FirstOrDefault(f => FileEx.ReadAllText(f).ContainsEx("DisableSplashScreen"));
                if (File.Exists(srcConfigPath))
                    foreach (var file in DirectoryEx.EnumerateFiles(dir, "*.exe"))
                        FileEx.Copy(srcConfigPath, Path.ChangeExtension(file, ".ini"));
            }
            foreach (var file in DirectoryEx.EnumerateFiles(dir, "*.ini"))
            {
                var content = FileEx.ReadAllText(file);
                if (!Regex.IsMatch(content, "DisableSplashScreen.*=.*false", RegexOptions.IgnoreCase))
                    continue;
                content = Regex.Replace(content, "DisableSplashScreen.*=.*false", "DisableSplashScreen=true", RegexOptions.IgnoreCase);
                FileEx.WriteAllText(file, content);
            }

            writeToFile = true;
            appInfo.Add(new LocalAppData(key, name, description, category, dir, exePath, configPath, appInfoPath));
        }

        if (appInfo.Count > 0)
        {
            var comparer = new AlphaNumericComparer<string>();
            appInfo = appInfo.OrderBy(x => x.Category == "#")
                             .ThenBy(x => x.Category, comparer)
                             .ThenBy(x => x.Name, comparer)
                             .ToList();
        }
        if (writeToFile)
            SaveDat(appInfo, CachePaths.CurrentAppInfo);

        return appInfo;
    }
}