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
                if (_appImages == default)
                    _appImages = Path.Combine(CorePaths.TempDir, Settings.Window.LargeImages ? "AppImagesLarge.dat" : "AppImages.dat");
                if (!File.Exists(_appImages))
                    _appImages = CorePaths.AppImages;
                return _appImages;
            }
        }

        internal static string CurrentImages =>
            _currentImages ?? (_currentImages = Path.Combine(CorePaths.TempDir, "CurrentImages.dat"));

        internal static string CurrentImageBg =>
            _currentimageBg ?? (_currentimageBg = Path.Combine(CorePaths.TempDir, "CurrentImageBg.dat"));

        internal static string CurrentAppInfo =>
            _currentAppInfo ?? (_currentAppInfo = Path.Combine(CorePaths.TempDir, "CurrentAppInfo.dat"));

        internal static string CurrentTypeData =>
            _currentTypeData ?? (_currentTypeData = Path.Combine(CorePaths.TempDir, "CurrentTypeData.dat"));

        internal static string SettingsMerges =>
            _settingsMerges ?? (_settingsMerges = Path.Combine(CorePaths.TempDir, "SettingsMerges.dat"));
    }
}
