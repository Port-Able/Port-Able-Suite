namespace AppsLauncher.Libraries
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using Properties;
    using SilDev;
    using SilDev.Forms;

    internal static class FileTypeAssoc
    {
        private static BackgroundWorker _bw;

        internal static void Associate(LocalAppData appData, bool quiet = false, Form owner = default)
        {
            if (appData?.Settings?.FileTypes?.Any() != true)
            {
                if (!quiet)
                    MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.associateBtnMsg)), Resources.GlobalTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!Elevation.IsAdministrator)
            {
                if (owner != default)
                {
                    owner.TopMost = false;
                    owner.Enabled = false;
                    TaskBarProgress.SetState(owner.Handle, TaskBarProgressState.Indeterminate);
                }
                _bw?.Dispose();
                _bw = new BackgroundWorker();
                _bw.DoWork += (sender, args) =>
                {
                    using (var process = ProcessEx.Start(PathEx.LocalPath, $"{ActionGuid.FileTypeAssociation} \"{appData.Key}\"", true, false))
                        if (!process?.HasExited == true)
                            process.WaitForExit();
                };
                _bw.RunWorkerCompleted += (sender, args) =>
                {
                    if (owner == default)
                        return;
                    TaskBarProgress.SetState(owner.Handle, TaskBarProgressState.NoProgress);
                    if (WinApi.NativeHelper.GetForegroundWindow() != owner.Handle)
                        WinApi.NativeHelper.SetForegroundWindow(owner.Handle);
                    owner.Enabled = true;
                    owner.TopMost = true;
                };
                _bw.RunWorkerAsync();
                return;
            }

            var assocData = new FileTypeAssocData
            {
                AppKey = appData.Key,
                InstallId = Settings.SystemInstallId
            };

            using (var dialog = new ResourcesEx.IconBrowserDialog(Settings.IconResourcePath, Settings.Window.Colors.BaseDark, Settings.Window.Colors.ControlText, Settings.Window.Colors.Button, Settings.Window.Colors.ButtonText, Settings.Window.Colors.ButtonHover))
            {
                dialog.TopMost = true;
                dialog.Plus();
                dialog.ShowDialog();
                if (!string.IsNullOrEmpty(dialog.IconPath))
                {
                    assocData.IconPath = dialog.IconPath;
                    assocData.IconId = dialog.IconId.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (!FileEx.Exists(assocData.IconPath) || string.IsNullOrEmpty(assocData.IconId))
                goto Cancel;

            MessageBoxEx.ButtonText.OverrideEnabled = true;
            MessageBoxEx.ButtonText.Yes = "App";
            MessageBoxEx.ButtonText.No = "Launcher";
            MessageBoxEx.ButtonText.Cancel = Language.GetText(nameof(en_US.Cancel));
            var result = !quiet ? MessageBoxEx.Show(Language.GetText(nameof(en_US.AssociateAppWayQuestion)), Resources.GlobalTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) : DialogResult.Yes;
            switch (result)
            {
                case DialogResult.Yes:
                    assocData.StarterPath = EnvironmentEx.GetVariablePathFull(appData.FilePath, false, false);
                    break;
                case DialogResult.No:
                    assocData.StarterPath = EnvironmentEx.GetVariablePathFull(PathEx.LocalPath, false, false);
                    break;
                default:
                    goto Cancel;
            }

            if (FileEx.Exists(assocData.StarterPath))
                appData.Settings.FileTypeAssoc = assocData;
            else
                goto Cancel;

            assocData = appData.Settings?.FileTypeAssoc;
            assocData?.SystemRegistryAccess?.AssociateFileTypes(false);
            return;

            Cancel:
            if (!quiet)
                MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.OperationCanceledMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal static void Associate(LocalAppData appData, Form owner) =>
            Associate(appData, false, owner);

        internal static void Associate(string appName, bool quiet = false, Form owner = default) =>
            Associate(CacheData.FindAppData(appName), quiet, owner);

        internal static void Associate(string appName, Form owner) =>
            Associate(CacheData.FindAppData(appName), false, owner);

        internal static void Restore(LocalAppData appData, bool quiet = false, Form owner = default)
        {
            if (appData == default)
                return;
            if (owner != default)
            {
                owner.TopMost = false;
                owner.Enabled = false;
                TaskBarProgress.SetState(owner.Handle, TaskBarProgressState.Indeterminate);
                Settings.WriteToFile(true);
            }
            var assocData = appData.Settings.FileTypeAssoc;
            assocData?.SystemRegistryAccess?.LoadRestorePoint(quiet);
            if (owner == default)
                return;
            TaskBarProgress.SetState(owner.Handle, TaskBarProgressState.NoProgress);
            if (WinApi.NativeHelper.GetForegroundWindow() != owner.Handle)
                WinApi.NativeHelper.SetForegroundWindow(owner.Handle);
            owner.Enabled = true;
            owner.TopMost = true;
        }

        internal static void Restore(LocalAppData appData, Form owner = default) =>
            Restore(appData, false, owner);

        internal static void Restore(string appName, bool quiet = false, Form owner = default) =>
            Restore(CacheData.FindAppData(appName), quiet, owner);

        internal static void Restore(string appName, Form owner) =>
            Restore(CacheData.FindAppData(appName), false, owner);
    }
}
