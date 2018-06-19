namespace AppsLauncher.Libraries
{
    using System.IO;

    internal static class CachePaths
    {
        private static string _appImages, _currentImages, _currentAppInfo, _currentimageBg, _currentTypeData, _settingsMerges;

        internal static string AppImages
        {
            get
            {
                if (_appImages == default(string))
                    _appImages = Path.Combine(CorePaths.TempDir, "AppImages.dat");
                if (!File.Exists(_appImages))
                    _appImages = CorePaths.AppImages;
                return _appImages;
            }
        }

        internal static string CurrentImages
        {
            get
            {
                if (_currentImages == default(string))
                    _currentImages = Path.Combine(CorePaths.TempDir, "CurrentImages.dat");
                return _currentImages;
            }
        }

        internal static string CurrentImageBg
        {
            get
            {
                if (_currentimageBg == default(string))
                    _currentimageBg = Path.Combine(CorePaths.TempDir, "CurrentImageBg.dat");
                return _currentimageBg;
            }
        }

        internal static string CurrentAppInfo
        {
            get
            {
                if (_currentAppInfo == default(string))
                    _currentAppInfo = Path.Combine(CorePaths.TempDir, "CurrentAppInfo.dat");
                return _currentAppInfo;
            }
        }

        internal static string CurrentTypeData
        {
            get
            {
                if (_currentTypeData == default(string))
                    _currentTypeData = Path.Combine(CorePaths.TempDir, "CurrentTypeData.dat");
                return _currentTypeData;
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
    }
}
