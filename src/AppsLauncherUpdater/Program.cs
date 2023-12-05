namespace Updater
{
    using System;
    using System.Globalization;
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
            if (!newInstance)
                return;
            MessageBoxEx.TopMost = true;
            Language.ResourcesNamespace = typeof(Program).Namespace;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using var form = new MainForm();
            Application.Run(form.Plus());
        }
    }
}
