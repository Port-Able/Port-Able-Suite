namespace AppsLauncher.Libraries
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Forms;
    using LangResources;
    using PortAble;
    using Properties;
    using SilDev;
    using SilDev.Forms;

    internal static class FileTypeAssoc
    {
        private static BackgroundWorker _bw;

        internal static void Associate(LocalAppData appData, bool quiet = false, Form owner = default)
        {
            if (appData?.Settings?.FileTypes?.Count is null or < 1)
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
                _bw.DoWork += (_, _) =>
                {
                    using var process = ProcessEx.Start(PathEx.LocalPath, $"{ActionGuid.FileTypeAssociation} \"{appData.Key}\"", true, false);
                    if (!process?.HasExited == true)
                        process.WaitForExit();
                };
                _bw.RunWorkerCompleted += (_, _) =>
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
                InstallId = _Settings.SystemInstallId
            };

            using (var dialog = new IconBrowserDialog(_Settings.IconResourcePath))
            {
                dialog.TopMost = true;
                dialog.Plus();
                dialog.ShowDialog(owner);
                if (!string.IsNullOrEmpty(dialog.IconPath))
                {
                    assocData.IconPath = dialog.IconPath;
                    assocData.IconId = dialog.IconId.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (!FileEx.Exists(assocData.IconPath) || string.IsNullOrEmpty(assocData.IconId))
                goto Cancel;

            MessageBoxEx.ButtonTextOverrideEnabled = true;
            MessageBoxEx.ButtonText = new MessageBoxButtonText
            {
                Yes = "App",
                No = "Launcher",
                Cancel = Language.GetText(nameof(en_US.Cancel))
            };

            var result = !quiet ? MessageBoxEx.Show(Language.GetText(nameof(en_US.AssociateAppWayQuestion)), Resources.GlobalTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) : DialogResult.Yes;
            switch (result)
            {
                case DialogResult.Yes:
                    assocData.StarterPath = EnvironmentEx.GetVariableWithPath(appData.ExecutablePath, false, false);
                    break;
                case DialogResult.No:
                    assocData.StarterPath = EnvironmentEx.GetVariableWithPath(PathEx.LocalPath, false, false);
                    break;
                default:
                    goto Cancel;
            }

            /*
            if (FileEx.Exists(assocData.StarterPath))
                appData.Settings.FileTypeAssoc = assocData;
            else
                goto Cancel;

            assocData = appData.Settings?.FileTypeAssoc;
            */
            assocData.SystemRegistryAccess?.AssociateFileTypes(false);
            return;

            Cancel:
            if (!quiet)
                MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.OperationCanceledMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal static void Associate(LocalAppData appData, Form owner) =>
            Associate(appData, false, owner);

        internal static void Associate(string appName, bool quiet = false, Form owner = default) =>
            Associate(CacheData.FindInCurrentAppInfo(appName), quiet, owner);

        internal static void Associate(string appName, Form owner) =>
            Associate(CacheData.FindInCurrentAppInfo(appName), false, owner);

        internal static void Restore(LocalAppData appData, bool quiet = false, Form owner = default)
        {
            if (appData == default)
                return;
            if (owner != default)
            {
                owner.TopMost = false;
                owner.Enabled = false;
                TaskBarProgress.SetState(owner.Handle, TaskBarProgressState.Indeterminate);
                _Settings.WriteToFile(true);
            }
            /*
            var assocData = appData.Settings.FileTypeAssoc;
            assocData?.SystemRegistryAccess?.LoadRestorePoint(quiet);
            */
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
            Restore(CacheData.FindInCurrentAppInfo(appName), quiet, owner);

        internal static void Restore(string appName, Form owner) =>
            Restore(CacheData.FindInCurrentAppInfo(appName), false, owner);
    }
}
