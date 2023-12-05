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
                if (_updateDir != default)
                    return _updateDir;
                var dir = Path.Combine(CorePaths.TempDir, "UpdateData");
                DirectoryEx.TryDelete(dir);
                dir = Path.Combine(dir, ActionGuid.CurrentAction.Encrypt(ChecksumAlgorithm.Adler32));
                if (DirectoryEx.Create(dir))
                    _updateDir = dir;
                return _updateDir;
            }
        }

        internal static string UpdateHelperPath => _updateHelperPath ??= Path.Combine(UpdateDir, "UpdateHelper.bat");

        internal static string UpdatePath => _updatePath ??= Path.Combine(UpdateDir, "Update.7z");
    }
}
