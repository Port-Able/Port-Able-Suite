namespace AppsLauncher.Libraries
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using SilDev;
    using SilDev.Forms;

    internal static class FileTypeAssoc
    {
        internal static void Associate(LocalAppData appData, bool quiet = false, Form owner = default(Form))
        {
            if (appData?.Settings?.FileTypes?.Any() != true)
            {
                MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.associateBtnMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!Elevation.IsAdministrator)
            {
                if (owner != default(Form))
                {
                    owner.TopMost = false;
                    owner.Enabled = false;
                    TaskBarProgress.SetState(owner.Handle, TaskBarProgressFlags.Indeterminate);
                }
                var bw = new BackgroundWorker();
                bw.DoWork += (sender, args) =>
                {
                    using (var process = ProcessEx.Start(PathEx.LocalPath, $"{ActionGuid.FileTypeAssociation} \"{appData.Key}\"", true, false))
                        if (!process?.HasExited == true)
                            process.WaitForExit();
                };
                bw.RunWorkerCompleted += (sender, args) =>
                {
                    if (owner == default(Form))
                        return;
                    TaskBarProgress.SetState(owner.Handle, TaskBarProgressFlags.NoProgress);
                    if (WinApi.NativeHelper.GetForegroundWindow() != owner.Handle)
                        WinApi.NativeHelper.SetForegroundWindow(owner.Handle);
                    owner.Enabled = true;
                    owner.TopMost = true;
                };
                bw.RunWorkerAsync();
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
                    assocData.IconId = dialog.IconId.ToString();
                    MessageBox.Show(dialog.IconPath + @"," + dialog.IconId);
                    Environment.Exit(0);
                }
            }

            if (!FileEx.Exists(assocData.IconPath) || string.IsNullOrEmpty(assocData.IconId))
                goto Cancel;

            MessageBoxEx.ButtonText.OverrideEnabled = true;
            MessageBoxEx.ButtonText.Yes = "App";
            MessageBoxEx.ButtonText.No = "Launcher";
            MessageBoxEx.ButtonText.Cancel = Language.GetText(nameof(en_US.Cancel));
            var result = MessageBoxEx.Show(Language.GetText(nameof(en_US.associateAppWayQuestion)), Settings.Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
            MessageBoxEx.Show(owner, Language.GetText(nameof(en_US.OperationCanceledMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal static void Associate(LocalAppData appData, Form owner) =>
            Associate(appData, false, owner);

        internal static void Associate(string appName, bool quiet = false, Form owner = default(Form)) =>
            Associate(CacheData.FindAppData(appName), quiet, owner);

        internal static void Associate(string appName, Form owner) =>
            Associate(CacheData.FindAppData(appName), false, owner);

        internal static void Restore(LocalAppData appData, bool quiet = false, Form owner = default(Form))
        {
            if (appData == default(LocalAppData))
                return;
            if (owner != default(Form))
            {
                owner.TopMost = false;
                owner.Enabled = false;
                TaskBarProgress.SetState(owner.Handle, TaskBarProgressFlags.Indeterminate);
                Settings.WriteToFile(true);
            }
            var assocData = appData.Settings.FileTypeAssoc;
            assocData?.SystemRegistryAccess?.LoadRestorePoint(quiet);
            if (owner == default(Form))
                return;
            TaskBarProgress.SetState(owner.Handle, TaskBarProgressFlags.NoProgress);
            if (WinApi.NativeHelper.GetForegroundWindow() != owner.Handle)
                WinApi.NativeHelper.SetForegroundWindow(owner.Handle);
            owner.Enabled = true;
            owner.TopMost = true;
        }

        internal static void Restore(LocalAppData appData, Form owner = default(Form)) =>
            Restore(appData, false, owner);

        internal static void Restore(string appName, bool quiet = false, Form owner = default(Form)) =>
            Restore(CacheData.FindAppData(appName), quiet, owner);

        internal static void Restore(string appName, Form owner) =>
            Restore(CacheData.FindAppData(appName), false, owner);
    }
}
