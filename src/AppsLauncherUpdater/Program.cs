namespace Updater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using LangResources;
    using Properties;
    using SilDev;
    using SilDev.Forms;

    internal static class Program
    {
        private static readonly string HomePath = PathEx.Combine(PathEx.LocalDir, "..");

        [STAThread]
        private static void Main()
        {
            Log.FileDir = PathEx.Combine(PathEx.LocalDir, "..\\Documents\\.cache\\Logs");
            Ini.SetFile(HomePath, "Settings.ini");
            Log.AllowLogging(Ini.FilePath, "DebugMode", Ini.GetRegex(false));
            if (!RequirementsAvailable())
            {
                Language.ResourcesNamespace = typeof(Program).Namespace;
                if (MessageBox.Show(Language.GetText(nameof(en_US.RequirementsErrorMsg)), Resources.Titel, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    Process.Start(PathEx.AltCombine(Resources.GitProfileUri, Resources.GitReleasesPath));
                return;
            }
            var instanceKey = PathEx.LocalPath.GetHashCode().ToString();
            using (new Mutex(true, instanceKey, out var newInstance))
            {
                if (!newInstance)
                    return;
                MessageBoxEx.TopMost = true;
                Language.ResourcesNamespace = typeof(Program).Namespace;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm().Plus());
            }
        }

        private static bool RequirementsAvailable()
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator();
            string[] rArray =
            {
                "Helper\\7z\\7z.dll",
                "Helper\\7z\\7zG.exe",
                "Helper\\7z\\x64\\7z.dll",
                "Helper\\7z\\x64\\7zG.exe"
            };
            return rArray.Select(s => PathEx.Combine(PathEx.LocalDir, s)).All(File.Exists);
        }
    }
}
