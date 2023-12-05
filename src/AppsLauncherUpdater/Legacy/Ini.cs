namespace SilDev.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Compression;
    using SilDev.Ini;

    /// <summary>
    ///     ***WIP-OUTDATED-CLASS from https://github.com/SilDev/CSharpLib
    /// </summary>
    public static class Ini
    {
        private const string NonSectionId = "\0\u0002(NON-SECTION)\u0003\0";
        private const string ObjectPrefix = "\u0001Object\u0002";
        private const string ObjectSuffix = "\u0003";
        private static string _filePath, _tmpFileGuid;

        /// <summary>
        ///     Gets or sets the maximum number of cached files.
        /// </summary>
        public static int MaxCacheSize { get; set; } = 8;

        /// <summary>
        ///     Specifies a sequence of section names to be sorted first.
        /// </summary>
        public static IEnumerable<string> SortBySections { get; set; }

        /// <summary>
        ///     Gets or sets a default INI file.
        /// </summary>
        public static string FilePath
        {
            get => _filePath ?? string.Empty;
            set
            {
                _filePath = PathEx.Combine(value);
                if (File.Exists(_filePath))
                    return;
                try
                {
                    var fileDir = Path.GetDirectoryName(_filePath);
                    if (string.IsNullOrEmpty(fileDir))
                        return;
                    if (!Directory.Exists(fileDir))
                        Directory.CreateDirectory(fileDir);
                    File.Create(_filePath).Close();
                }
                catch (Exception ex) when (ex.IsCaught())
                {
                    Log.Write(ex);
                }
            }
        }

        private static Dictionary<int, Dictionary<string, Dictionary<string, List<string>>>> CachedFiles { get; set; }

        private static string TmpFileGuid
        {
            get
            {
                if (_tmpFileGuid != default)
                    return _tmpFileGuid;
                _tmpFileGuid = Guid.NewGuid().ToString();
                return _tmpFileGuid;
            }
        }

        /// <summary>
        ///     Save the cached data to the specified file.
        /// </summary>
        /// <param name="cacheFilePath">
        ///     The full file path of the cache file to create.
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file. If this parameter is NULL,
        ///     all cached data are saved.
        /// </param>
        /// <param name="compress">
        ///     <see langword="true"/> to compress the cache; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        public static void SaveCache(string cacheFilePath = null, string fileOrContent = null, bool compress = true)
        {
            try
            {
                if (!CachedFiles?.Any() ?? true)
                    throw new NullReferenceException();
                var path = PathEx.Combine(cacheFilePath ?? GetFile());
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException(nameof(cacheFilePath));
                if (!Path.HasExtension(path) || Path.GetExtension(path).EqualsEx(".ini"))
                    path = Path.ChangeExtension(path, ".ixi");
                if (!PathEx.IsValidPath(path))
                    throw new ArgumentInvalidException(nameof(cacheFilePath));
                var file = fileOrContent ?? GetFile();
                if (string.IsNullOrEmpty(file))
                    throw new ArgumentNullException(nameof(fileOrContent));
                var code = GetCode(file);
                if (!CodeExists(code))
                    ReadAll(fileOrContent);
                if (!CodeExists(code) || !CachedFiles[code].Any())
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));
                var bytes = CachedFiles[code]?.SerializeObject();
                if (bytes == null)
                    throw new NullReferenceException();
                if (compress)
                    bytes = GZip.Compress(bytes);
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
        }

        /// <summary>
        ///     Loads the data of a cache file into memory.
        ///     <para>
        ///         Please note that <see cref="MaxCacheSize"/> is ignored in this case.
        ///     </para>
        /// </summary>
        /// <param name="cacheFilePath">
        ///     The full path of a cache file.
        /// </param>
        public static void LoadCache(string cacheFilePath)
        {
            try
            {
                var path = PathEx.Combine(cacheFilePath ?? GetFile());
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException(nameof(cacheFilePath));
                if (!Path.HasExtension(path) || Path.GetExtension(path).EqualsEx(".ini"))
                    path = Path.ChangeExtension(path, ".ixi");
                if (!File.Exists(path))
                    throw new PathNotFoundException(path);
                var cache = GZip.Decompress(File.ReadAllBytes(path))?.DeserializeObject<Dictionary<string, Dictionary<string, List<string>>>>();
                var file = Path.ChangeExtension(path, ".ini");
                var code = GetCode(file);
                InitializeCache(code);
                CachedFiles[code] = cache ?? throw new ArgumentInvalidException(nameof(cacheFilePath));
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
        }

        /// <summary>
        ///     Gets the regular expression to convert the INI data into an accessible
        ///     format.
        /// </summary>
        /// <param name="allowEmptySection">
        ///     <see langword="true"/> to allow key value pairs without section; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        public static Regex GetRegex(bool allowEmptySection = true)
        {
            const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            return allowEmptySection ? new Regex(@"^((?:\[)(?<Section>[^\]]*)(?:\])(?:[\r\n]{0,}|\Z))((?!\[)(?<Key>[^=]*?)(?:=)(?<Value>[^\r\n]*)(?:[\r\n]{0,4}))*", options) : new Regex(@"^((?:\[)(?<Section>[^\]]*)(?:\])(?:[\r\n]{0,}|\Z))((?!\[)(?<Key>[^=]*?)(?:=)(?<Value>[^\r\n]*)(?:[\r\n]{0,4}))+", options);
        }

        /// <summary>
        ///     Gets the full path of the default INI file.
        /// </summary>
        public static string GetFile() =>
            FilePath;

        /// <summary>
        ///     Specifies an INI file to use as default.
        /// </summary>
        /// <param name="paths">
        ///     An array of parts of the path.
        /// </param>
        public static bool SetFile(params string[] paths) =>
            File.Exists(FilePath = PathEx.Combine(paths));

        /// <summary>
        ///     Removes the read content of an INI file from cache.
        /// </summary>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        public static bool Detach(string fileOrContent = null)
        {
            try
            {
                var code = GetCode(fileOrContent);
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));
                if (CachedFiles?.ContainsKey(code) ?? false)
                    CachedFiles.Remove(code);
                return true;
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return false;
        }

        /// <summary>
        ///     Retrieves all section names of an INI file or an INI file formatted string
        ///     value.
        /// </summary>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        /// <param name="sorted">
        ///     <see langword="true"/> to sort the sections; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        public static List<string> GetSections(string fileOrContent = null, bool sorted = true)
        {
            try
            {
                var code = GetCode(fileOrContent);
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));
                if (!CodeExists(code))
                    ReadAll(fileOrContent);
                if (CodeExists(code))
                {
                    var output = CachedFiles[code].Keys.ToList();
                    if (output.Contains(NonSectionId))
                        output.Remove(NonSectionId);
                    if (sorted)
                        output = output.SortHelper();
                    return output;
                }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return new List<string>();
        }

        /// <summary>
        ///     Retrieves all section names of an INI file or an INI file formatted string
        ///     value.
        /// </summary>
        /// <param name="sorted">
        ///     <see langword="true"/> to sort the sections; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        public static List<string> GetSections(bool sorted) =>
            GetSections(null, sorted);

        /// <summary>
        ///     Removes the specified section including all associated keys of an INI file
        ///     or an INI file formatted string value.
        /// </summary>
        /// <param name="section">
        ///     The name of the section to remove.
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        public static bool RemoveSection(string section, string fileOrContent = null)
        {
            try
            {
                var code = GetCode(fileOrContent);
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));
                if (!CodeExists(code))
                    ReadAll(fileOrContent);
                return RemoveSection(code, section);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
                return false;
            }
        }

        /// <summary>
        ///     Retrieves all key names of an INI file or an INI file formatted string
        ///     value.
        /// </summary>
        /// <param name="section">
        ///     The name of the section to get the key names. The value must be NULL to get
        ///     all the key names of the specified fileOrContent parameter.
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        /// <param name="sorted">
        ///     <see langword="true"/> to sort keys; otherwise, <see langword="false"/>.
        /// </param>
        public static List<string> GetKeys(string section, string fileOrContent = null, bool sorted = true)
        {
            try
            {
                var code = GetCode(fileOrContent);
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));
                if (!CodeExists(code))
                    ReadAll(fileOrContent);
                if (SectionExists(code, section))
                {
                    var output = CachedFiles[code][section].Keys.ToList();
                    if (output.Contains(NonSectionId))
                        output.Remove(NonSectionId);
                    if (sorted)
                        output = output.SortHelper();
                    return output;
                }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return new List<string>();
        }

        /// <summary>
        ///     Retrieves all key names of an INI file or an INI file formatted string
        ///     value.
        /// </summary>
        /// <param name="section">
        ///     The name of the section to get the key names. The value must be NULL to get
        ///     all the key names of the specified fileOrContent parameter.
        /// </param>
        /// <param name="sorted">
        ///     <see langword="true"/> to sort keys; otherwise, <see langword="false"/>.
        /// </param>
        public static List<string> GetKeys(string section, bool sorted) =>
            GetKeys(section, null, sorted);

        /// <summary>
        ///     Removes the specified key from the specified section, of an INI file or an
        ///     INI file formatted string value.
        /// </summary>
        /// <param name="section">
        ///     The name of the section containing the key to remove.
        /// </param>
        /// <param name="key">
        ///     The name of the key to remove.
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        public static bool RemoveKey(string section, string key, string fileOrContent = null)
        {
            try
            {
                var code = GetCode(fileOrContent);
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));
                if (!CachedFiles?.ContainsKey(code) ?? true)
                    ReadAll(fileOrContent);
                return RemoveKey(code, section, key);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
                return false;
            }
        }

        /// <summary>
        ///     Retrieves the full content of an INI file or an INI file formatted string
        ///     value.
        /// </summary>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        /// <param name="sorted">
        ///     <see langword="true"/> to sort the sections and keys; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        public static Dictionary<string, Dictionary<string, List<string>>> ReadAll(string fileOrContent = null, bool sorted = false)
        {
            var output = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var source = fileOrContent ?? GetFile();
                if (string.IsNullOrEmpty(source))
                    throw new ArgumentNullException(nameof(fileOrContent));
                var path = PathEx.Combine(source);
                if (File.Exists(path))
                    source = File.ReadAllText(path);
                else
                    path = TmpFileGuid;
                var code = path?.ToUpperInvariant().GetHashCode() ?? -1;
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));

                source = ForceFormat(source);
                var content = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);
                foreach (var match in GetRegex().Matches(source).Cast<Match>())
                {
                    var section = match.Groups["Section"]?.Value.Trim();
                    if (string.IsNullOrEmpty(section))
                        continue;
                    if (!content.ContainsKey(section))
                        content.Add(section, new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase));
                    var keys = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                    for (var i = 0; i < match.Groups["Key"].Captures.Count; i++)
                    {
                        var key = match.Groups["Key"]?.Captures[i].Value.Trim();
                        if (string.IsNullOrEmpty(key))
                            continue;
                        var value = match.Groups["Value"]?.Captures[i].Value.Trim();
                        if (string.IsNullOrEmpty(value))
                            continue;
                        if (!keys.ContainsKey(key))
                            keys.Add(key, new List<string>());
                        keys[key].Add(value);
                    }
                    content[section] = keys;
                }
                if (sorted)
                    content = content.SortHelper();
                output = content;
                if (output.Count > 0)
                {
                    InitializeCache(code);
                    if (CachedFiles.Count > 0 && CachedFiles.Count >= MaxCacheSize)
                    {
                        var defCode = FilePath?.ToUpperInvariant().GetHashCode() ?? -1;
                        var delCode = CachedFiles.Keys.FirstOrDefault(x => x != defCode);
                        if (CodeExists(delCode))
                            CachedFiles.Remove(delCode);
                    }
                    CachedFiles[code] = output;
                }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return output;
        }

        /// <summary>
        ///     Retrieves the full content of an INI file or an INI file formatted string
        ///     value.
        /// </summary>
        /// <param name="sorted">
        ///     <see langword="true"/> to sort the sections and keys; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        public static Dictionary<string, Dictionary<string, List<string>>> ReadAll(bool sorted) =>
            ReadAll(null, sorted);

        /// <summary>
        ///     Retrieves a <see cref="string"/> value from the specified section in an INI
        ///     file or an INI file formatted string value.
        /// </summary>
        /// <param name="section">
        ///     The name of the section containing the key name. The value must be NULL for
        ///     a non-section key.
        /// </param>
        /// <param name="key">
        ///     The name of the key whose associated value is to be retrieved.
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        /// <param name="reread">
        ///     <see langword="true"/> to reread the INI file; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="index">
        ///     The value index used to handle multiple key value pairs.
        /// </param>
        public static string Read(string section, string key, string fileOrContent = null, bool reread = false, int index = 0)
        {
            try
            {
                var code = GetCode(fileOrContent);
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));
                if (reread || !CodeExists(code))
                    ReadAll(fileOrContent);
                if (!CodeExists(code))
                    throw new ArgumentNullException(nameof(fileOrContent));
                if (string.IsNullOrEmpty(section))
                {
                    if (section != null)
                        throw new ArgumentNullException(nameof(section));
                    section = NonSectionId;
                }
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));
                if (KeyExists(code, section, key))
                {
                    var i = Math.Abs(index);
                    if (CachedFiles[code][section][key].Count > i)
                        return CachedFiles[code][section][key][i] ?? string.Empty;
                    return CachedFiles[code][section][key]?.FirstOrDefault() ?? string.Empty;
                }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return string.Empty;
        }

        /// <summary>
        ///     Retrieves a <see cref="string"/> value from the specified section in an INI
        ///     file or an INI file formatted string value and release all cached resources
        ///     used by the specified INI file or the INI file formatted string value.
        /// </summary>
        /// <param name="section">
        ///     The name of the section containing the key name. The value must be NULL for
        ///     a non-section key.
        /// </param>
        /// <param name="key">
        ///     The name of the key whose associated value is to be retrieved.
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        /// <param name="index">
        ///     The value index used to handle multiple key value pairs.
        /// </param>
        public static string ReadOnly(string section, string key, string fileOrContent, int index = 0)
        {
            var output = Read(section, key, fileOrContent, true, index);
            Detach(fileOrContent);
            return output;
        }

        /// <summary>
        ///     Retrieves a value from the specified section in an INI file or an INI file
        ///     formatted string value.
        /// </summary>
        /// <typeparam name="TValue">
        ///     The value type.
        /// </typeparam>
        /// <param name="section">
        ///     The name of the section containing the key name.
        /// </param>
        /// <param name="key">
        ///     The name of the key whose associated value is to be retrieved.
        /// </param>
        /// <param name="defValue">
        ///     The value that is used as default.
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        /// <param name="reread">
        ///     <see langword="true"/> to reread the INI file; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        public static TValue Read<TValue>(string section, string key, TValue defValue = default, string fileOrContent = null, bool reread = false)
        {
            try
            {
                var strValue = Read(section, key, fileOrContent, reread);
                object newValue;
                if (string.IsNullOrEmpty(strValue))
                {
                    if (Log.DebugMode > 1)
                        throw new WarningException("Value not found.");
                    newValue = (object)defValue ?? string.Empty;
                }
                else if (strValue.StartsWith(ObjectPrefix, StringComparison.Ordinal) && strValue.EndsWith(ObjectSuffix, StringComparison.Ordinal))
                {
                    var startIndex = ObjectPrefix.Length;
                    var length = strValue.Length - ObjectPrefix.Length - ObjectSuffix.Length;
                    var bytes = strValue.Substring(startIndex, length).Decode(BinaryToTextEncoding.Base85);
                    var unzipped = GZip.Decompress(bytes);
                    if (unzipped != null)
                        bytes = unzipped;
                    newValue = bytes?.DeserializeObject<object>() ?? defValue;
                }
                else
                {
                    var type = typeof(TValue);
                    if (type == typeof(string))
                        newValue = strValue;
                    else if (type == typeof(Rectangle))
                        newValue = strValue.ToRectangle();
                    else if (type == typeof(Point))
                        newValue = strValue.ToPoint();
                    else if (type == typeof(Size))
                        newValue = strValue.ToSize();
                    else if (type == typeof(Version))
                        newValue = Version.Parse(strValue);
                    else if (strValue.TryParse<TValue>(out var genValue))
                        newValue = genValue;
                    else
                        newValue = defValue;
                }
                return (TValue)newValue;
            }
            catch (FormatException ex)
            {
                if (Log.DebugMode > 1)
                    Log.Write(ex);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return defValue;
        }

        /// <summary>
        ///     Retrieves a value from the specified section in an INI file or an INI file
        ///     formatted string value and release all cached resources used by the
        ///     specified INI file or the INI file formatted string value.
        /// </summary>
        /// <typeparam name="TValue">
        ///     The value type.
        /// </typeparam>
        /// <param name="section">
        ///     The name of the section containing the key name.
        /// </param>
        /// <param name="key">
        ///     The name of the key whose associated value is to be retrieved.
        /// </param>
        /// <param name="defValue">
        ///     The value that is used as default.
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        public static TValue ReadOnly<TValue>(string section, string key, TValue defValue, string fileOrContent)
        {
            var output = Read(section, key, defValue, fileOrContent);
            Detach(fileOrContent);
            return output;
        }

        /// <summary>
        ///     Retrieves a <see cref="string"/> value from the specified section in an INI
        ///     file.
        ///     <para>
        ///         The Win32-API without file caching is used for reading in this case.
        ///     </para>
        /// </summary>
        /// <param name="section">
        ///     The name of the section containing the key name.
        /// </param>
        /// <param name="key">
        ///     The name of the key whose associated value is to be retrieved.
        /// </param>
        /// <param name="file">
        ///     The full file path of an INI file.
        /// </param>
        public static string ReadDirect(string section, string key, string file = null)
        {
            var output = string.Empty;
            try
            {
                var path = PathEx.Combine(file ?? GetFile());
                if (!File.Exists(path))
                    throw new PathNotFoundException(path);
                output = IniDirect.Read(path, section, key);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return output;
        }

        /// <summary>
        ///     Writes the specified content to an INI file on the disk.
        /// </summary>
        /// <param name="content">
        ///     The content based on <see cref="ReadAll(string,bool)"/>.
        ///     <para>
        ///         If this parameter is NULL, the function writes all the cached data from
        ///         the specified INI file to the disk.
        ///     </para>
        /// </param>
        /// <param name="file">
        ///     The full file path of an INI file.
        ///     <para>
        ///         If this parameter is NULL, the default INI file is used.
        ///     </para>
        /// </param>
        /// <param name="sorted">
        ///     <see langword="true"/> to sort the sections and keys; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="detach">
        ///     <see langword="true"/> to release all cached resources used by the
        ///     specified INI file; otherwise, <see langword="false"/>.
        /// </param>
        public static bool WriteAll(Dictionary<string, Dictionary<string, List<string>>> content = null, string file = null, bool sorted = true, bool detach = false)
        {
            try
            {
                var path = PathEx.Combine(file ?? GetFile());
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException(nameof(file));

                var code = path.ToUpperInvariant().GetHashCode();
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(file));

                var source = content ?? new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);
                if (source.Count == 0 && CodeExists(code))
                    source = CachedFiles[code];
                if (source.Count == 0 && File.Exists(path))
                    source = ReadAll(path);
                if (source.Count == 0 || source.Values.Count == 0)
                    throw new ArgumentNullException(nameof(content));
                if (sorted)
                    source = source.SortHelper();

                if (!File.Exists(path) && !PathEx.IsValidPath(path))
                    throw new ArgumentInvalidException(nameof(file));

                var hash = File.Exists(path) ? path.EncryptFile(ChecksumAlgorithm.Crc32) : null;
                var temp = FileEx.GetUniqueTempPath("tmp", ".ini");
                using (var sw = new StreamWriter(temp, true))
                    foreach (var dict in source)
                    {
                        if (string.IsNullOrWhiteSpace(dict.Key) || dict.Value.Count == 0)
                            continue;
                        if (!dict.Key.Equals(NonSectionId, StringComparison.Ordinal))
                        {
                            sw.Write('[');
                            sw.Write(dict.Key.Trim());
                            sw.Write(']');
                            sw.WriteLine();
                        }
                        foreach (var pair in dict.Value)
                        {
                            if (string.IsNullOrWhiteSpace(pair.Key) || pair.Value.Count == 0)
                                continue;
                            foreach (var value in pair.Value)
                            {
                                if (string.IsNullOrWhiteSpace(value))
                                    continue;
                                sw.Write(pair.Key.Trim());
                                sw.Write('=');
                                sw.Write(value.Trim());
                                sw.WriteLine();
                            }
                        }
                        sw.WriteLine();
                    }
                if (hash?.Equals(temp.EncryptFile(ChecksumAlgorithm.Crc32), StringComparison.Ordinal) ?? false)
                {
                    File.Delete(temp);
                    return true;
                }
                File.Delete(path);
                File.Move(temp, path);

                if (detach)
                    Detach(path);
                return true;
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return false;
        }

        /// <summary>
        ///     Writes all the cached data from the specified INI file to the disk.
        /// </summary>
        /// <param name="file">
        ///     The full file path of an INI file.
        ///     <para>
        ///         If this parameter is NULL, the default INI file is used.
        ///     </para>
        /// </param>
        /// <param name="sorted">
        ///     <see langword="true"/> to sort the sections and keys; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="detach">
        ///     <see langword="true"/> to release all cached resources used by the
        ///     specified INI file; otherwise, <see langword="false"/>.
        /// </param>
        public static bool WriteAll(string file, bool sorted = true, bool detach = false) =>
            WriteAll(null, file, sorted, detach);

        /// <summary>
        ///     Copies the specified value into the specified section of an INI file.
        ///     <para>
        ///         This function updates only the cache and has no effect on the file
        ///         until <see cref="WriteAll(string,bool,bool)"/> is called.
        ///     </para>
        /// </summary>
        /// <typeparam name="TValue">
        ///     The value type.
        /// </typeparam>
        /// <param name="section">
        ///     The name of the section to which the value will be copied.
        /// </param>
        /// <param name="key">
        ///     The name of the key to be associated with a value.
        ///     <para>
        ///         If this parameter is NULL, the entire section, including all entries
        ///         within the section, is deleted.
        ///     </para>
        /// </param>
        /// <param name="value">
        ///     The value to be written to the file.
        ///     <para>
        ///         If this parameter is NULL, the key pointed to by the key parameter is
        ///         deleted.
        ///     </para>
        /// </param>
        /// <param name="fileOrContent">
        ///     The full file path or content of an INI file.
        /// </param>
        /// <param name="forceOverwrite">
        ///     <see langword="true"/> to enable overwriting of a key with the same value
        ///     as specified; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="skipExistValue">
        ///     <see langword="true"/> to skip an existing value, even it is not the same
        ///     value as specified; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="index">
        ///     The value index used to handle multiple key value pairs.
        /// </param>
        public static bool Write<TValue>(string section, string key, TValue value, string fileOrContent = null, bool forceOverwrite = true, bool skipExistValue = false, int index = 0)
        {
            try
            {
                var code = GetCode(fileOrContent);
                if (code == -1)
                    throw new ArgumentOutOfRangeException(nameof(fileOrContent));

                if (!CodeExists(code))
                    ReadAll(fileOrContent);

                if (string.IsNullOrEmpty(section))
                {
                    if (section != null)
                        throw new ArgumentNullException(nameof(section));
                    section = NonSectionId;
                }
                if (section.Any(TextEx.IsLineSeparator))
                    throw new ArgumentOutOfRangeException(nameof(section));

                if (key.Any(TextEx.IsLineSeparator))
                    throw new ArgumentOutOfRangeException(nameof(key));

                var val = string.Empty;
                if (!string.IsNullOrEmpty(key) && Comparison.IsNotEmpty(value))
                {
                    var str = value.ToString();
                    var type = typeof(TValue);
                    if (type.IsSerializable && (type.ToString() == str || $"({type.Name})" == str || str.Any(TextEx.IsLineSeparator)))
                    {
                        var bytes = value.SerializeObject();
                        var zipped = GZip.Compress(bytes);
                        if (zipped?.Length < bytes?.Length)
                            bytes = zipped;
                        val = string.Concat(ObjectPrefix, bytes.Encode(BinaryToTextEncoding.Base85), ObjectSuffix);
                    }
                    else
                        val = str;
                }

                if (!forceOverwrite || skipExistValue)
                {
                    var c = Read(section, key, fileOrContent, false, index);
                    if (!forceOverwrite && c == val || skipExistValue && !string.IsNullOrWhiteSpace(c))
                        return false;
                }

                var i = Math.Abs(index);
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val))
                {
                    if (!SectionExists(code, section))
                        return true;
                    if (string.IsNullOrEmpty(key))
                        RemoveSection(code, section);
                    else if (KeyExists(code, section, key))
                    {
                        if (CachedFiles?[code][section][key].Count > i)
                            CachedFiles[code][section][key].RemoveAt(i);
                        if (CachedFiles?[code][section][key].Any() ?? false)
                            RemoveKey(code, section, key);
                        if (CachedFiles?[code][section].Any() ?? false)
                            RemoveSection(code, section);
                    }
                    return true;
                }

                InitializeCache(code, section, key);
                if (KeyExists(code, section, key))
                {
                    if (CachedFiles?[code][section][key].Count > i)
                    {
                        CachedFiles[code][section][key][i] = val;
                        return true;
                    }
                    if (CachedFiles?[code][section][key].Count != i)
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
                CachedFiles?[code][section][key].Add(val);
                return true;
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return false;
        }

        /// <summary>
        ///     Copies the specified value into the specified section of an INI file.
        ///     <para>
        ///         This function updates only the cache and has no effect on the file
        ///         until <see cref="WriteAll(string,bool,bool)"/> is called.
        ///     </para>
        /// </summary>
        /// <typeparam name="TValue">
        ///     The value type.
        /// </typeparam>
        /// <param name="section">
        ///     The name of the section to which the value will be copied.
        /// </param>
        /// <param name="key">
        ///     The name of the key to be associated with a value.
        ///     <para>
        ///         If this parameter is NULL, the entire section, including all entries
        ///         within the section, is deleted.
        ///     </para>
        /// </param>
        /// <param name="value">
        ///     The value to be written to the file.
        ///     <para>
        ///         If this parameter is NULL, the key pointed to by the key parameter is
        ///         deleted.
        ///     </para>
        /// </param>
        /// <param name="forceOverwrite">
        ///     <see langword="true"/> to enable overwriting of a key with the same value
        ///     as specified; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="skipExistValue">
        ///     <see langword="true"/> to skip an existing value, even it is not the same
        ///     value as specified; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="index">
        ///     The value index used to handle multiple key value pairs.
        /// </param>
        public static bool Write<TValue>(string section, string key, TValue value, bool forceOverwrite, bool skipExistValue = false, int index = 0) =>
            Write(section, key, value, null, forceOverwrite, skipExistValue, index);

        /// <summary>
        ///     Copies the <see cref="string"/> representation of the specified
        ///     <see cref="object"/> value into the specified section of an INI file. If
        ///     the file does not exist, it is created.
        ///     <para>
        ///         The Win32-API is used for writing in this case. Please note that this
        ///         function writes all changes directly on the disk. This causes many
        ///         write accesses when used incorrectly.
        ///     </para>
        /// </summary>
        /// <param name="section">
        ///     The name of the section to which the value will be copied.
        /// </param>
        /// <param name="key">
        ///     The name of the key to be associated with a value.
        ///     <para>
        ///         If this parameter is NULL, the entire section, including all entries
        ///         within the section, is deleted.
        ///     </para>
        /// </param>
        /// <param name="value">
        ///     The value to be written to the file.
        ///     <para>
        ///         If this parameter is NULL, the key pointed to by the key parameter is
        ///         deleted.
        ///     </para>
        /// </param>
        /// <param name="file">
        ///     The full path of an INI file.
        /// </param>
        /// <param name="forceOverwrite">
        ///     <see langword="true"/> to enable overwriting of a key with the same value
        ///     as specified; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="skipExistValue">
        ///     <see langword="true"/> to skip an existing value, even it is not the same
        ///     value as specified; otherwise, <see langword="false"/>.
        /// </param>
        public static bool WriteDirect(string section, string key, object value, string file = null, bool forceOverwrite = true, bool skipExistValue = false)
        {
            try
            {
                var path = PathEx.Combine(file ?? GetFile());
                if (string.IsNullOrWhiteSpace(section))
                    throw new ArgumentNullException(nameof(section));
                if (!File.Exists(path))
                {
                    if (string.IsNullOrWhiteSpace(key) || value == null || !Path.HasExtension(path) || !PathEx.IsValidPath(path))
                        throw new PathNotFoundException(path);
                    var dir = Path.GetDirectoryName(path);
                    if (string.IsNullOrWhiteSpace(dir))
                        throw new ArgumentInvalidException(nameof(file));
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    File.Create(path).Close();
                }
                var strValue = value?.ToString();
                if (!forceOverwrite || skipExistValue)
                {
                    var curValue = ReadDirect(section, key, path);
                    if (!forceOverwrite && curValue.Equals(strValue, StringComparison.Ordinal) || skipExistValue && !string.IsNullOrWhiteSpace(curValue))
                        return false;
                }
                if (string.Concat(section, key, value).All(TextEx.IsAscii))
                    goto Write;
                var encoding = EncodingEx.GetEncoding(path);
                if (!encoding.Equals(Encoding.Unicode) && !encoding.Equals(Encoding.BigEndianUnicode))
                    EncodingEx.ChangeEncoding(path, Encoding.Unicode);
                Write:
                return IniDirect.Write(path, section, key, strValue);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
                return false;
            }
        }

        private static void InitializeCache(int code, string section = null, string key = null)
        {
            CachedFiles ??= new Dictionary<int, Dictionary<string, Dictionary<string, List<string>>>>();

            if (!CachedFiles.ContainsKey(code))
                CachedFiles[code] = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(section))
                return;
            if (!CachedFiles[code].ContainsKey(section))
                CachedFiles[code][section] = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(key))
                return;
            if (!CachedFiles[code][section].ContainsKey(key))
                CachedFiles[code][section][key] = new List<string>();
        }

        private static Dictionary<string, Dictionary<string, List<string>>> SortHelper(this Dictionary<string, Dictionary<string, List<string>>> source)
        {
            var comparer = new AlphaNumericComparer<string>();
            var sorted = source.OrderBy(x => !x.Key.Equals(NonSectionId, StringComparison.Ordinal))
                               .ThenBy(x => !SortBySections.ContainsItem(x.Key))
                               .ThenBy(x => x.Key, comparer).ToDictionary(x => x.Key, x => x.Value.SortHelper());
            return sorted;
        }

        private static Dictionary<string, List<string>> SortHelper(this Dictionary<string, List<string>> source)
        {
            var comparer = new AlphaNumericComparer<string>();
            var sorted = source.OrderBy(d => d.Key, comparer).ToDictionary(d => d.Key, d => d.Value);
            return sorted;
        }

        private static List<string> SortHelper(this IEnumerable<string> source)
        {
            var comparer = new AlphaNumericComparer<string>();
            var sorted = source.OrderBy(x => x, comparer).ToList();
            return sorted;
        }

        private static bool CodeExists(int code) =>
            code != -1 && (CachedFiles?.ContainsKey(code) ?? false) && (CachedFiles[code]?.Any() ?? false);

        private static int GetCode(string fileOrContent)
        {
            var source = fileOrContent ?? GetFile();
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(fileOrContent));
            var path = PathEx.Combine(source);
            if (!File.Exists(path))
                path = TmpFileGuid;
            var code = path?.ToUpperInvariant().GetHashCode() ?? -1;
            return code;
        }

        private static bool SectionExists(int code, string section) =>
            !string.IsNullOrEmpty(section) && CodeExists(code) && (CachedFiles[code]?.ContainsKey(section) ?? false) && (CachedFiles[code][section]?.Any() ?? false);

        private static bool RemoveSection(int code, string section)
        {
            if (!CodeExists(code) || !CachedFiles[code].ContainsKey(section))
                return true;
            CachedFiles[code].Remove(section);
            return true;
        }

        private static bool KeyExists(int code, string section, string key) =>
            !string.IsNullOrEmpty(key) && SectionExists(code, section) && CachedFiles[code][section].ContainsKey(key) && (CachedFiles[code][section][key]?.Any() ?? false);

        private static bool RemoveKey(int code, string section, string key)
        {
            if (!KeyExists(code, section, key))
                return true;
            CachedFiles[code][section].Remove(key);
            return true;
        }

        private static bool LineIsValid(string str)
        {
            var s = str;
            if (string.IsNullOrWhiteSpace(s) || s.Length < 3)
                return false;
            if (s.StartsWith("[", StringComparison.Ordinal) && s.EndsWith("]", StringComparison.Ordinal) && s.Count(x => x == '[') == 1 && s.Count(x => x == ']') == 1 && s.Any(char.IsLetterOrDigit))
                return true;
            var c = s.First();
            if (!char.IsLetterOrDigit(c) && !c.IsBetween('$', '/') && !c.IsBetween('<', '@') && !c.IsBetween('{', '~') && c != '!' && c != '"' && c != ':' && c != '^' && c != '_')
                return false;
            var i = s.IndexOf('=');
            return i > 0 && s.Substring(0, i).Any(char.IsLetterOrDigit) && i + 1 < s.Length;
        }

        private static string ForceFormat(string str)
        {
            var builder = new StringBuilder();
            foreach (var text in TextEx.FormatNewLine(str.TrimStart()).SplitNewLine())
            {
                var line = text.Trim();
                if (line.StartsWith("[", StringComparison.Ordinal) && !line.EndsWith("]", StringComparison.Ordinal) && line.Contains(']') && line.IndexOf(']') > 1)
                    line = line.Substring(0, line.IndexOf(']') + 1);
                if (LineIsValid(line))
                    builder.AppendLine(line);
            }
            if (builder.Length < 1)
                throw new NullReferenceException();
            var first = builder.ToString(0, 1).First();
            if (first.Equals('['))
                return builder.ToString();
            builder.Insert(0, Environment.NewLine);
            builder.Insert(0, ']');
            builder.Insert(0, NonSectionId);
            builder.Insert(0, '[');
            return builder.ToStringThenClear();
        }
    }
}
