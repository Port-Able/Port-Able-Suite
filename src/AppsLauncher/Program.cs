namespace AppsLauncher;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Forms;
using Libraries;
using PortAble;
using SilDev;
using SilDev.Drawing;
using SilDev.Forms;
using static SilDev.WinApi;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        _Settings.Initialize();

        Log.AllowLogging();
        if (Directory.Exists(CorePaths.LogsDir))
        {
            Log.DebugMode = 1;
            Log.FileDir = CorePaths.LogsDir;
        }

        Environment.SetEnvironmentVariable("AppsSuiteDir", CorePaths.HomeDir);

        if (!CacheData.CurrentAppInfo.Any())
        {
            using (var process = ProcessEx.Start(CorePaths.AppsDownloader, Elevation.IsAdministrator, false))
                if (process?.HasExited == false)
                    process.WaitForExit();
            if (!CacheData.CurrentAppInfo.Any())
            {
                Environment.ExitCode = 0;
                Environment.Exit(Environment.ExitCode);
            }
        }

        if (!Recovery.AppsSuiteIsHealthy())
        {
            Environment.ExitCode = 1;
            Environment.Exit(Environment.ExitCode);
        }


        var instanceKey = PathEx.LocalPath.GetHashCode().ToString(CultureInfo.InvariantCulture);
        using var mutex = new Mutex(true, instanceKey, out var newInstance);
        Language.ResourcesNamespace = typeof(Program).Namespace;
        MessageBoxEx.TopMost = true;

        NativeHelper.SetProcessDPIAware();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        if (newInstance && Arguments.ValidPaths.Count > 0 && !ActionGuid.IsDisallowInterface)
        {
            using var form = new OpenWithForm();
            Application.Run(form.Plus());
            return;
        }

        if (EnvironmentEx.CommandLineArgs(false).Count > 0)
        {
            var args = EnvironmentEx.CommandLineArgs(false);
            switch (EnvironmentEx.CommandLineArgs(false).Count)
            {
                case 1:
                {
                    switch (args.First())
                    {
                        case ActionGuid.FileTypeAssociationAll:
                            /*
                            foreach (var assocData in _CacheData.CurrentAppInfo.Select(a => a.Settings?.FileTypeAssoc))
                                assocData?.SystemRegistryAccess?.AssociateFileTypes(true);
                            */
                            return;
                        case ActionGuid.RepairAppsSuite:
                            Recovery.RepairAppsSuite();
                            return;
                        case ActionGuid.RepairDirs:
                            Recovery.RepairAppsSuiteDirs();
                            return;
                        case ActionGuid.RepairVariable:
                            Recovery.RepairEnvironmentVariable();
                            return;
                        case ActionGuid.VersionValidation:
                            Recovery.VersionValidation();
                            return;
                    }
                    break;
                }
                case 2:
                case 3:
                {
                    var second = args.SecondOrDefault();
                    var third = args.ThirdOrDefault().ToBoolean();
                    switch (args.First())
                    {
                        case ActionGuid.FileTypeAssociation:
                            FileTypeAssoc.Associate(second, third);
                            return;
                        case ActionGuid.RestoreFileTypes:
                            FileTypeAssoc.Restore(second, third);
                            return;
                        case ActionGuid.SystemIntegration when second.ToBoolean():
                            SystemIntegration.Apply(third);
                            return;
                        case ActionGuid.SystemIntegration:
                            SystemIntegration.Remove(third);
                            return;
                        }
                    break;
                }
            }

            if (Arguments.ValidPaths.Count < 1)
                return;

            IntPtr hWnd;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            do
                hWnd = CacheData.LoadDat(CachePaths.CurrentHost, IntPtr.Zero);
            while (hWnd == IntPtr.Zero && stopwatch.Elapsed.TotalSeconds < 2d);
            if (hWnd != IntPtr.Zero)
                NativeHelper.SendArgs(hWnd, Arguments.ValidPathsStr);
        }

        if (!newInstance && !ActionGuid.IsAllowNewInstance)
            return;

        using (var form = new MenuViewForm())
            Application.Run(form.Plus());
    }
}
