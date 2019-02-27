namespace AppsDownloader.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SilDev;
    using SilDev.QuickWmi;

    internal static class Shareware
    {
        private static string _computerId;
        private static Dictionary<string, Tuple<string, string>> _data;

        private static string ComputerId
        {
            get
            {
                if (_computerId == default(string))
                    _computerId = Win32_OperatingSystem.SerialNumber?.Encrypt(ChecksumAlgorithms.Sha256);
                return _computerId;
            }
        }

        internal static Dictionary<string, Tuple<string, string>> Data
        {
            get
            {
                if (_data != default(Dictionary<string, Tuple<string, string>>))
                    return _data;

                if (File.Exists(CachePaths.SwData))
                    _data = FileEx.Deserialize<Dictionary<string, Tuple<string, string>>>(CachePaths.SwData);

                if (_data?.Any() != true)
                    _data = new Dictionary<string, Tuple<string, string>>();

                var key = Settings.GetConfigKey(nameof(Shareware), nameof(Data), "Count");
                var count = Ini.Read(Settings.Section, key, 0);
                Ini.RemoveKey(Settings.Section, key);
                Ini.WriteDirect(Settings.Section, key, null);

                var write = false;
                for (var i = 0; i < count; i++)
                {
                    key = Settings.GetConfigKey(nameof(Shareware), nameof(Data), i.ToString(), "Server");
                    var srv = Encrypt(Ini.Read(Settings.Section, key));
                    Ini.RemoveKey(Settings.Section, key);
                    Ini.WriteDirect(Settings.Section, key, null);

                    key = Settings.GetConfigKey(nameof(Shareware), nameof(Data), i.ToString(), "User");
                    var usr = Encrypt(Ini.Read(Settings.Section, key));
                    Ini.RemoveKey(Settings.Section, key);
                    Ini.WriteDirect(Settings.Section, key, null);

                    key = Settings.GetConfigKey(nameof(Shareware), nameof(Data), i.ToString(), "Password");
                    var pwd = Encrypt(Ini.Read(Settings.Section, key));
                    Ini.RemoveKey(Settings.Section, key);
                    Ini.WriteDirect(Settings.Section, key, null);

                    CacheData.UpdateSettingsMerges(Settings.Section);

                    if (srv == default(string) || usr == default(string) || pwd == default(string))
                        continue;

                    _data[srv] = Tuple.Create(usr, pwd);
                    if (!write)
                        write = true;
                }

                if (write && _data.Any())
                {
                    DirectoryEx.Create(Path.GetDirectoryName(CachePaths.SwData));
                    FileEx.Serialize(CachePaths.SwData, _data);
                    return _data;
                }

                if (File.Exists(CachePaths.SwData) && !_data.Any())
                    FileEx.TryDelete(CachePaths.SwData);
                return _data;
            }
            set
            {
                _data = value;
                if (_data?.Any() == true)
                {
                    DirectoryEx.Create(Path.GetDirectoryName(CachePaths.SwData));
                    FileEx.Serialize(CachePaths.SwData, _data);
                    return;
                }
                if (File.Exists(CachePaths.SwData))
                    FileEx.TryDelete(CachePaths.SwData);
            }
        }

        internal static bool Enabled =>
            Data?.Any() == true;

        internal static string Decrypt(string data) => 
            !string.IsNullOrWhiteSpace(data) ? data.Decode(BinaryToTextEncodings.Base91)?.Decrypt(ComputerId).ToStringDefault() : default(string);

        internal static string Encrypt(string data) => 
            !string.IsNullOrWhiteSpace(data) ? data.Encrypt(ComputerId)?.Encode(BinaryToTextEncodings.Base91) : default(string);

        internal static string FindAddressKey(string address) =>
            Data.Keys.FirstOrDefault(key => address.EqualsEx(Decrypt(key)));

        internal static IEnumerable<string> GetAddresses() =>
            Data.Keys.Select(Decrypt);

        internal static string GetUser(string address) =>
            Data.TryGetValue(FindAddressKey(address), out var value) ? Decrypt(value.Item1) : default(string);

        internal static string GetPassword(string address) =>
            Data.TryGetValue(FindAddressKey(address), out var value) ? Decrypt(value.Item2) : default(string);
    }
}
