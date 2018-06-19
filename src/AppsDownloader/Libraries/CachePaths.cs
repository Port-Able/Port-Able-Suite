namespace AppsDownloader.Libraries
{
    using System;
    using System.IO;

    internal static class CachePaths
    {
        private static string _appImages, _appInfo, _settingsMerges, _swData;

        internal static string AppImages
        {
            get
            {
                if (_appImages == default(string))
                    _appImages = Path.Combine(CorePaths.TempDir, "AppImages.dat");
                return _appImages;
            }
        }

        internal static string AppInfo
        {
            get
            {
                if (_appInfo == default(string))
                    _appInfo = Path.Combine(CorePaths.TempDir, $"AppInfo{Convert.ToInt32(ActionGuid.IsUpdateInstance)}.dat");
                return _appInfo;
            }
        }

        internal static string SettingsMerges
        {
            get
            {
                if (_settingsMerges == default(string))
                    _settingsMerges = Path.Combine(CorePaths.TempDir, "SettingsMerges.dat");
                return _settingsMerges;
            }
        }

        internal static string SwData
        {
            get
            {
                if (_swData == default(string))
                    _swData = Path.Combine(CorePaths.TempDir, nameof(SwData), Settings.MachineId, "SwData.dat");
                return _swData;
            }
        }
    }
}
