namespace Updater.Libraries
{
    using System;
    using SilDev;
    using System.IO;
    using System.Linq;

    internal static class Recovery
    {
        internal static bool AppsSuiteIsHealthy()
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator();
            try
            {
                if (!Directory.Exists(CorePaths.FileArchiverDir))
                    throw new DirectoryNotFoundException();
                if (CorePaths.FileArchiverFiles.Any(x => !File.Exists(x)))
                    throw new FileNotFoundException();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return false;
            }
            return true;
        }

        internal static void RepairAppsSuiteDirs() =>
            Repair(ActionGuid.RepairDirs, Elevation.IsAdministrator);

        private static void Repair(string guid, bool elevated)
        {
            using (var process = ProcessEx.Start(CorePaths.AppsLauncher, guid, elevated, false))
                if (process?.HasExited == false)
                    process.WaitForExit();
        }
    }
}
