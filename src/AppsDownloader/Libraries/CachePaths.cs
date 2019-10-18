namespace AppsDownloader.Libraries
{
    using System;
    using System.IO;

    internal static class CachePaths
    {
        private static string _appImages, _appImagesLarge, _appInfo, _settingsMerges, _swData, _swDataKey;

        internal static string AppImages => 
            _appImages ?? (_appImages = Path.Combine(CorePaths.TempDir, "AppImages.dat"));

        internal static string AppImagesLarge => 
            _appImagesLarge ?? (_appImagesLarge = Path.Combine(CorePaths.TempDir, "AppImagesLarge.dat"));

        internal static string AppInfo => 
            _appInfo ?? (_appInfo = Path.Combine(CorePaths.TempDir, $"AppInfo{Convert.ToInt32(ActionGuid.IsUpdateInstance)}.dat"));

        internal static string SettingsMerges => 
            _settingsMerges ?? (_settingsMerges = Path.Combine(CorePaths.TempDir, "SettingsMerges.dat"));

        internal static string SwData => 
            _swData ?? (_swData = Path.Combine(CorePaths.TempDir, "SwData.dat"));

        internal static string SwDataKey => 
            _swDataKey ?? (_swDataKey = Path.Combine(CorePaths.TempDir, "SwData.key"));
    }
}
