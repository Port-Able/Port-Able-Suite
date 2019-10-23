namespace AppsDownloader.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SilDev;

    internal static class Shareware
    {
        private static Dictionary<string, Tuple<string, string>> _data;

        internal static Dictionary<string, Tuple<string, string>> Data
        {
            get
            {
                if (_data != default)
                    return _data;
                if (File.Exists(CachePaths.SwData))
                    _data = FileEx.Deserialize<Dictionary<string, Tuple<string, string>>>(CachePaths.SwData, true);
                if (_data?.Any() != true)
                    _data = new Dictionary<string, Tuple<string, string>>();
                return _data;
            }
            set
            {
                _data = value;
                if (_data?.Any() == true)
                {
                    DirectoryEx.Create(Path.GetDirectoryName(CachePaths.SwData));
                    FileEx.Serialize(CachePaths.SwData, _data, true);
                    return;
                }
                if (File.Exists(CachePaths.SwData))
                    FileEx.TryDelete(CachePaths.SwData);
            }
        }

        internal static bool Enabled =>
            Data?.Any() == true;

        internal static string Decrypt(string data) =>
            !string.IsNullOrWhiteSpace(data) ? data.Decode(BinaryToTextEncoding.Base85)?.Decrypt(CacheData.SwDataKey).ToStringDefault() : default;

        internal static string Encrypt(string data) =>
            !string.IsNullOrWhiteSpace(data) ? data.Encrypt(CacheData.SwDataKey)?.Encode(BinaryToTextEncoding.Base85) : default;

        internal static string FindAddressKey(string address) =>
            Data.Keys.FirstOrDefault(key => address.EqualsEx(Decrypt(key)));

        internal static IEnumerable<string> GetAddresses() =>
            Data.Keys.Select(Decrypt);

        internal static string GetUser(string address) =>
            Data.TryGetValue(FindAddressKey(address), out var value) ? Decrypt(value.Item1) : default;

        internal static string GetPassword(string address) =>
            Data.TryGetValue(FindAddressKey(address), out var value) ? Decrypt(value.Item2) : default;
    }
}
