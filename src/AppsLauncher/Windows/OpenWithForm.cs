namespace AppsLauncher.Windows
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using LangResources;
    using Libraries;
    using Microsoft.Win32;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;
    using FormsTimer = System.Windows.Forms.Timer;

    public sealed partial class OpenWithForm : Form
    {
        private static readonly object Locker = new object();

        public OpenWithForm()
        {
            InitializeComponent();

            SuspendLayout();

            Language.SetControlLang(this);
            Text = Language.GetText(Name);

            BackColor = Settings.Window.Colors.BaseDark;
            Icon = Resources.Logo;
            notifyIcon.Icon = CacheData.GetSystemIcon(ImageResourceSymbol.Asterisk, true);

            searchBox.BackColor = Settings.Window.Colors.Control;
            searchBox.ForeColor = Settings.Window.Colors.ControlText;
            searchBox.DrawSearchSymbol(Settings.Window.Colors.ControlText);

            startBtn.Split(Settings.Window.Colors.ButtonText);
            foreach (var btn in new[] { startBtn, settingsBtn })
            {
                btn.BackColor = Settings.Window.Colors.Button;
                btn.ForeColor = Settings.Window.Colors.ButtonText;
                btn.FlatAppearance.MouseDownBackColor = Settings.Window.Colors.Button;
                btn.FlatAppearance.MouseOverBackColor = Settings.Window.Colors.ButtonHover;
            }

            appMenu.EnableAnimation(ContextMenuStripExAnimation.SlideVerPositive, 100);
            appMenu.SetFixedSingle();
            appMenuItem2.Image = CacheData.GetSystemImage(ImageResourceSymbol.Uac);
            appMenuItem3.Image = CacheData.GetSystemImage(ImageResourceSymbol.Directory);
            appMenuItem7.Image = CacheData.GetSystemImage(ImageResourceSymbol.RecycleBinEmpty);

            ResumeLayout(false);

            if (!searchBox.Focused)
                searchBox.Select();

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private bool IsStarted { get; set; }

        private string SearchText { get; set; }

        private void OpenWithForm_Load(object sender, EventArgs e)
        {
            var notifyBox = NotifyBoxEx.Show(Language.GetText(nameof(en_US.FileSystemAccessMsg)), Resources.GlobalTitle, NotifyBoxStartPosition.Center, 0u, false);
            Arguments.DefineAppName();
            notifyBox.Close();

            if (WinApi.NativeHelper.GetForegroundWindow() != Handle)
                WinApi.NativeHelper.SetForegroundWindow(Handle);

            FormEx.Dockable(this);
            AppsBoxUpdate(false);
        }

        private void OpenWithForm_Shown(object sender, EventArgs e)
        {
            Reg.Write(Settings.RegistryPath, "Handle", Handle);
            if (!string.IsNullOrWhiteSpace(Arguments.AppName))
            {
                runCmdLine.Enabled = true;
                return;
            }
            Opacity = 1f;
        }

        private void OpenWithForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Reg.RemoveSubKey(Settings.RegistryPath);
            if (!Settings.StartMenuIntegration)
                return;
            var appNames = appsBox.Items.Cast<string>();
            SystemIntegration.UpdateStartMenuShortcuts(appNames);
        }

        private void OpenWithForm_FormClosed(object sender, FormClosedEventArgs e) =>
            Settings.WriteToFile();

        private void OpenWithForm_DragEnter(object sender, DragEventArgs e)
        {
            if (!DragFileName(out var items, e))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            var added = false;
            foreach (var item in items)
            {
                if (!(item is string str) || string.IsNullOrEmpty(str))
                    continue;
                str = str.RemoveChar('\"');
                if (Arguments.ValidPaths.Contains(str))
                    continue;
                added = true;
                Arguments.ValidPaths.Add(str);
            }
            if (added)
            {
                Arguments.DefineAppName();
                ShowBalloonTip(Text, Language.GetText(nameof(en_US.cmdLineUpdated)));
                foreach (var appInfo in CacheData.CurrentAppInfo)
                    if (Arguments.AppName.EqualsEx(appInfo.Key))
                    {
                        appsBox.SelectedItem = appInfo.Name;
                        Arguments.AppName = string.Empty;
                        break;
                    }
            }
            e.Effect = DragDropEffects.Copy;
        }

        private static bool DragFileName(out Array files, DragEventArgs e)
        {
            files = null;
            if ((e.AllowedEffect & DragDropEffects.Copy) != DragDropEffects.Copy)
                return false;
            var data = e.Data.GetData("FileDrop") as Array;
            if (!(data?.Length > 0) || !(data.GetValue(0) is string))
                return false;
            files = data;
            return true;
        }

        private void OpenWithForm_Activated(object sender, EventArgs e)
        {
            if (!IsStarted)
                IsStarted = true;
            else
                AppsBoxUpdate(true);
        }

        private void OpenWithForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            TopMost = false;
            try
            {
                using (var dialog = new AboutForm())
                {
                    dialog.TopMost = true;
                    dialog.Plus();
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            TopMost = true;
            e.Cancel = true;
        }

        private void RunCmdLine_Tick(object sender, EventArgs e)
        {
            lock (Locker)
                try
                {
                    var curTitle = Language.GetText($"{Name}Title");
                    var curInstance = Process.GetCurrentProcess();
                    var allInstances = ProcessEx.GetInstances(PathEx.LocalPath).ToList();
                    try
                    {
                        if (allInstances.Any(p => p.Handle != curInstance.Handle && p.MainWindowTitle.EqualsEx(curTitle)))
                            return;
                        foreach (var appData in CacheData.CurrentAppInfo)
                        {
                            if (!Arguments.AppName.EqualsEx(appData.Key))
                                continue;
                            appsBox.SelectedItem = appData.Name;
                            if (!Arguments.AppNameConflict && appsBox.SelectedIndex > 0 && appData.Settings.NoConfirm)
                            {
                                runCmdLine.Enabled = false;
                                appData.StartApplication(true);
                                return;
                            }
                            break;
                        }
                        runCmdLine.Enabled = false;
                        Opacity = 1f;
                    }
                    catch (InvalidOperationException ex) when (ex.IsCaught())
                    {
                        if (Log.DebugMode > 1)
                            Log.Write(ex);
                    }
                    catch (Exception ex) when (ex.IsCaught())
                    {
                        Log.Write(ex);
                    }
                    finally
                    {
                        curInstance.Dispose();
                        foreach (var p in allInstances)
                            p?.Dispose();
                    }
                }
                catch (Win32Exception ex) when (ex.IsCaught())
                {
                    if (Log.DebugMode > 1)
                        Log.Write(ex);
                }
                catch (Exception ex) when (ex.IsCaught())
                {
                    Log.Write(ex);
                }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            if (notifyIconDisabler.IsBusy)
                notifyIconDisabler.CancelAsync();
            if (notifyIcon.Visible)
                notifyIcon.Visible = false;
        }

        private void NotifyIconDisabler_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!(sender is BackgroundWorker bw))
                return;
            for (var i = 0; i < 3000; i++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Thread.Sleep(1);
            }
        }

        private void NotifyIconDisabler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) =>
            notifyIcon.Visible = false;

        private void ShowBalloonTip(string title, string tip)
        {
            if (!notifyIcon.Visible)
                notifyIcon.Visible = true;
            if (!notifyIconDisabler.IsBusy)
                notifyIconDisabler.RunWorkerAsync();
            notifyIcon.ShowBalloonTip(1800, title, tip, ToolTipIcon.Info);
        }

        private void AppsBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 0xd)
                return;
            var appName = appsBox.SelectedItem.ToString();
            CacheData.FindAppData(appName)?.StartApplication(true);
        }

        private void AppsBoxUpdate(bool force)
        {
            if (force)
                CacheData.CurrentAppInfo = default;
            if (CacheData.CurrentAppInfo?.Any() != true)
                return;

            var selectedItem = string.Empty;
            if (appsBox.SelectedIndex >= 0)
                selectedItem = appsBox.SelectedItem.ToString();

            appsBox.Items.Clear();
            appsBox.Items.AddRange(CacheData.CurrentAppInfo.Select(x => x.Name).Cast<object>().ToArray());

            if (appsBox.SelectedIndex < 0 && !string.IsNullOrWhiteSpace(Arguments.AppName))
            {
                var appName = CacheData.FindAppData(Arguments.AppName).Key;
                if (string.IsNullOrWhiteSpace(appName) || !appsBox.Items.Contains(appName))
                    appName = CacheData.FindAppData(Arguments.AppName).Name;
                if (!string.IsNullOrWhiteSpace(appName) && appsBox.Items.Contains(appName))
                    appsBox.SelectedItem = appName;
            }

            if (appsBox.SelectedIndex < 0 && !string.IsNullOrWhiteSpace(Settings.LastItem) && appsBox.Items.Contains(Settings.LastItem))
                appsBox.SelectedItem = Settings.LastItem;

            if (!string.IsNullOrWhiteSpace(selectedItem))
                appsBox.SelectedItem = selectedItem;
            if (appsBox.SelectedIndex < 0)
                appsBox.SelectedIndex = 0;

            if (!Settings.StartMenuIntegration)
                return;
            var appNames = appsBox.Items.Cast<object>().Select(item => item.ToString());
            SystemIntegration.UpdateStartMenuShortcuts(appNames);
        }

        private void AppMenuItem_Opening(object sender, CancelEventArgs e)
        {
            var owner = sender as ContextMenuStrip;
            for (var i = 0; i < owner?.Items.Count; i++)
            {
                var text = Language.GetText(owner.Items[i].Name);
                owner.Items[i].Text = !string.IsNullOrWhiteSpace(text) ? text : owner.Items[i].Text;
            }
        }

        private void AppMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = appsBox.SelectedItem.ToString();
            if (string.IsNullOrEmpty(selectedItem))
                return;
            var appData = CacheData.FindAppData(selectedItem);
            if (appData == default)
                return;
            var owner = sender as ToolStripMenuItem;
            switch (owner?.Name)
            {
                case nameof(appMenuItem1):
                    appData.StartApplication(true);
                    break;
                case nameof(appMenuItem2):
                    appData.StartApplication(true, true);
                    break;
                case nameof(appMenuItem3):
                    appData.OpenLocation();
                    break;
                case nameof(appMenuItem4):
                    var linkPath = PathEx.Combine(Environment.SpecialFolder.Desktop, selectedItem);
                    var created = FileEx.CreateShellLink(appData.FilePath, linkPath);
                    MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                    MessageBoxEx.Show(this, Language.GetText(created ? nameof(en_US.appMenuItem4Msg0) : nameof(en_US.appMenuItem4Msg1)), Resources.GlobalTitle, MessageBoxButtons.OK, created ? MessageBoxIcon.Asterisk : MessageBoxIcon.Warning);
                    break;
                case nameof(appMenuItem7):
                    if (MessageBoxEx.Show(this, string.Format(CultureInfo.InvariantCulture, Language.GetText(nameof(en_US.appMenuItem7Msg)), selectedItem), Resources.GlobalTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                        if (appData.RemoveApplication(this))
                        {
                            appsBox.Items.RemoveAt(appsBox.SelectedIndex);
                            if (appsBox.SelectedIndex < 0)
                                appsBox.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                        MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCanceledMsg)), Resources.GlobalTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    break;
            }
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            if (!(sender is TextBox owner))
                return;
            owner.Font = new Font("Segoe UI", owner.Font.Size);
            owner.ForeColor = Settings.Window.Colors.ControlText;
            owner.Text = SearchText ?? string.Empty;
        }

        private void SearchBox_Leave(object sender, EventArgs e)
        {
            if (!(sender is TextBox owner))
                return;
            var c = Settings.Window.Colors.ControlText;
            owner.Font = new Font("Comic Sans MS", owner.Font.Size, FontStyle.Italic);
            owner.ForeColor = Color.FromArgb(c.A, c.R / 2, c.G / 2, c.B / 2);
            SearchText = owner.Text;
            owner.Text = Language.GetText(owner);
        }

        private void SearchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0xd)
            {
                var appName = appsBox.SelectedItem.ToString();
                CacheData.FindAppData(appName)?.StartApplication(true);
                return;
            }
            (sender as TextBox)?.Refresh();
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var owner = sender as TextBox;
            if (string.IsNullOrWhiteSpace(owner?.Text))
                return;
            var itemList = appsBox.Items.Cast<object>().Select(item => item.ToString()).ToList();
            foreach (var item in appsBox.Items)
                if (item.ToString().Equals(itemList.SearchItem(owner.Text), StringComparison.Ordinal))
                {
                    appsBox.SelectedItem = item;
                    break;
                }
        }

        private void AddBtn_Click(object sender, EventArgs e) =>
            ProcessEx.Start(CorePaths.AppsDownloader);

        private void AddBtn_MouseEnter(object sender, EventArgs e)
        {
            if (!(sender is Button owner))
                return;
            owner.Image = owner.Image.SwitchGrayScale($"{owner.Name}BackgroundImage");
            toolTip.SetToolTip(owner, Language.GetText($"{owner.Name}Tip"));
        }

        private void AddBtn_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button owner)
                owner.Image = owner.Image.SwitchGrayScale($"{owner.Name}BackgroundImage");
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (!(sender is Button owner) || owner.SplitClickHandler(appMenu))
                return;
            var appName = appsBox.SelectedItem.ToString();
            CacheData.FindAppData(appName)?.StartApplication(true);
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            TopMost = false;
            try
            {
                var appData = CacheData.FindAppData(appsBox.SelectedItem.ToString());
                using (var dialog = new SettingsForm(appData))
                {
                    dialog.TopMost = true;
                    dialog.Plus();
                    dialog.ShowDialog();
                    Language.SetControlLang(this);
                    Text = Language.GetText(Name);
                    AppsBoxUpdate(true);
                }
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            TopMost = true;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 1)
            {
                Application.Restart();
                return;
            }
            var timer = new FormsTimer(components)
            {
                Interval = 1,
                Enabled = true
            };
            timer.Tick += (o, args) =>
            {
                if (!(o is FormsTimer owner) || Application.OpenForms.Count > 1)
                    return;
                owner.Enabled = false;
                Application.Restart();
                owner.Dispose();
            };
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)WinApi.WindowMenuFlags.WmCopyData:
                    var dStruct = (WinApi.CopyData)Marshal.PtrToStructure(m.LParam, typeof(WinApi.CopyData));
                    var strData = Marshal.PtrToStringUni(dStruct.lpData);
                    if (!string.IsNullOrWhiteSpace(strData) && !Arguments.ValidPaths.ContainsEx(strData))
                    {
                        if (WinApi.NativeHelper.GetForegroundWindow() != Handle)
                            WinApi.NativeHelper.SetForegroundWindow(Handle);
                        Arguments.ValidPaths.Add(strData.Trim('"'));
                        Arguments.DefineAppName();
                        ShowBalloonTip(Text, Language.GetText(nameof(en_US.cmdLineUpdated)));
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
