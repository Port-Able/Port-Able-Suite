namespace AppsLauncher
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using Windows;
    using Libraries;
    using SilDev;
    using SilDev.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Settings.Initialize();

            var instanceKey = PathEx.LocalPath.GetHashCode().ToString(CultureInfo.InvariantCulture);
            using var mutex = new Mutex(true, instanceKey, out var newInstance);
            Language.ResourcesNamespace = typeof(Program).Namespace;
            MessageBoxEx.TopMost = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (newInstance && Arguments.ValidPaths.Any() && !ActionGuid.IsDisallowInterface)
            {
                using var form = new OpenWithForm();
                Application.Run(form.Plus());
                return;
            }

            if (EnvironmentEx.CommandLineArgs(false).Any())
            {
                var args = EnvironmentEx.CommandLineArgs(false);
                switch (EnvironmentEx.CommandLineArgs(false).Count)
                {
                    case 1:
                    {
                        switch (args.First())
                        {
                            case ActionGuid.FileTypeAssociationAll:
                                foreach (var appData in CacheData.CurrentAppInfo)
                                {
                                    var assocData = appData.Settings?.FileTypeAssoc;
                                    assocData?.SystemRegistryAccess?.AssociateFileTypes(true);
                                }
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
                            case ActionGuid.SystemIntegration:
                                SystemIntegration.Enable(second.ToBoolean(), third);
                                return;
                        }
                        break;
                    }
                }

                if (!Arguments.ValidPaths.Any())
                    return;

                IntPtr hWnd;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                    hWnd = Reg.Read(Settings.RegistryPath, "Handle", IntPtr.Zero);
                while (hWnd == IntPtr.Zero && stopwatch.Elapsed.TotalSeconds <= 10);
                if (hWnd != IntPtr.Zero)
                    WinApi.NativeHelper.SendArgs(hWnd, Arguments.ValidPathsStr);
            }

            if (!newInstance && !ActionGuid.IsAllowNewInstance)
                return;

            using (var form = new MenuViewForm())
                Application.Run(form.Plus());
        }
    }
}
