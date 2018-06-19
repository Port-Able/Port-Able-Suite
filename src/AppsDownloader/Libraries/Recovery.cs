namespace AppsDownloader.Libraries
{
    using System.IO;
    using SilDev;

    internal static class Recovery
    {
        internal static bool AppsSuiteIsHealthy(bool repair = true)
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator();
            while (true)
            {
                try
                {
                    if (!File.Exists(CorePaths.AppsLauncher) ||
                        !File.Exists(CorePaths.AppsSuiteUpdater) ||
                        !File.Exists(CorePaths.FileArchiver))
                        throw new FileNotFoundException();
                }
                catch (FileNotFoundException ex)
                {
                    Log.Write(ex);
                    if (!repair)
                        return false;
                    Repair(ActionGuid.RepairAppsSuite, true);
                }

                try
                {
                    foreach (var dir in CorePaths.AppDirs)
                        if (!Directory.Exists(dir))
                            throw new PathNotFoundException(dir);
                }
                catch (PathNotFoundException ex)
                {
                    Log.Write(ex);
                    if (!repair)
                        return false;
                    Repair(ActionGuid.RepairDirs, false);
                }

                if (!repair)
                    return true;
                repair = false;
            }
        }

        private static void Repair(string guid, bool elevated)
        {
            using (var process = ProcessEx.Start(PathEx.LocalPath, guid, elevated, false))
                if (process?.HasExited == false)
                    process.WaitForExit();
        }
    }
}
