namespace Updater.Libraries
{
    using System.IO;
    using SilDev;

    internal static class CachePaths
    {
        private static string _updateDir, _updateHelperPath, _updatePath;

        internal static string UpdateDir
        {
            get
            {
                if (_updateDir != default(string))
                    return _updateDir;
                foreach (var dir in DirectoryEx.EnumerateDirectories(CorePaths.TempDir, "UpdateData-{*}"))
                    DirectoryEx.TryDelete(dir);
                _updateDir = Path.Combine(CorePaths.TempDir, $"UpdateData-{ActionGuid.CurrentAction}");
                return _updateDir;
            }
        }

        internal static string UpdateHelperPath
        {
            get
            {
                if (_updateHelperPath == default(string))
                    _updateHelperPath = Path.Combine(UpdateDir, "UpdateHelper.bat");
                return _updateHelperPath;
            }
        }

        internal static string UpdatePath
        {
            get
            {
                if (_updatePath == default(string))
                    _updatePath = Path.Combine(UpdateDir, "Update.7z");
                return _updatePath;
            }
        }
    }
}
