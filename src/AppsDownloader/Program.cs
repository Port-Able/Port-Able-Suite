namespace AppsDownloader
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using Forms;
    using PortAble;
    using PortAble.Properties;
    using SilDev;
    using SilDev.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Log.AllowLogging();
            if (Directory.Exists(CorePaths.LogsDir))
            {
                if (Log.DebugMode < 1)
                    Log.DebugMode = 1;
                Log.FileDir = CorePaths.LogsDir;
            }
            if (!File.Exists(CorePaths.FileArchiver))
                throw new PathNotFoundException(CorePaths.FileArchiver);
            Environment.SetEnvironmentVariable("AppsSuiteDir", CorePaths.HomeDir);

            /*
            if (!Recovery.AppsSuiteIsHealthy())
            {
                Environment.ExitCode = 1;
                Environment.Exit(Environment.ExitCode);
                return;
            }
            */

            var instanceKey = PathEx.LocalPath.GetHashCode().ToString(CultureInfo.InvariantCulture);
            using var mutex = new Mutex(true, instanceKey, out var newInstance);
            var allowInstance = newInstance;
            if (!allowInstance)
            {
                var instances = ProcessEx.GetInstances(PathEx.LocalPath);
                var count = 0;
                foreach (var instance in instances)
                {
                    if (instance?.GetCommandLine()?.ContainsEx(ActionGuid.UpdateInstance) == true)
                        count++;
                    instance?.Dispose();
                }
                allowInstance = count == 1;
            }
            if (!allowInstance)
                return;

            MessageBoxEx.TopMost = true;

            WinApi.NativeHelper.SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var notifyBox = default(NotifyBox);
            if (!ActionGuid.IsUpdateInstance)
            {
                notifyBox = new NotifyBox();
                notifyBox.Show(LangStrings.InitializingMsg, AssemblyInfo.Title, NotifyBoxStartPosition.Center);
            }
            using var form = new MainForm(notifyBox);
            Application.Run(form.Plus());
        }
    }
}
