namespace AppsDownloader.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Data;
    using PortAble;
    using PortAble.Properties;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;
    using SilDev.Investment;
    using SilDev.Network;
    using static SilDev.WinApi;

    public sealed partial class MainForm : Form
    {
        private static Task _transferTask;
        private static readonly object DownloadHandler = new();
        private static readonly object DownloadStarter = new();
        private static readonly object SearchHandler = new();
        private readonly CounterInvestor<int> _counter = new();
        private readonly Dictionary<string, (Image, Image)> _imageSwitcherCache = new();
        private readonly NotifyBox _notifyBox;
        private readonly Dictionary<int, int> _reqCounter = new();
        private readonly List<AppData> _transferFails = new();
        private readonly Dictionary<ListViewItem, AppTransferor> _transferManager = new();
        private readonly Stopwatch _transferStopwatch = new();
        private ListView _appsListBackupClone;
        private bool _autoRetry, _noAppsListItemCheckEvent;
        private KeyValuePair<ListViewItem, AppTransferor> _currentTransfer;

        public AppsDownloaderSettings Settings { get; } = new();

        public MainForm(NotifyBox notifyBox = default)
        {
            InitializeComponent();

            SuspendLayout();

            Icon = Resources.PaLogoGreenSymbol;

            appsList.Height = ClientSize.Height - buttonAreaPanel.Height - buttonAreaBorder.Height;
            appsList.ListViewItemSorter = new ControlAlphaNumericComparer();
            appsList.SetDoubleBuffer();

            CacheData.SetComponentImageColor(appMenuItem1);
            _imageSwitcherCache.Add(nameof(appMenuItem1), (appMenuItem1.Image, Resources.Uncheck.RecolorPixels(Color.Black, SystemColors.Highlight.InvertRgb())));

            CacheData.SetComponentImageColor(appMenuItem2);
            _imageSwitcherCache.Add(nameof(appMenuItem2), (appMenuItem2.Image, Resources.Uncheck.RecolorPixels(Color.Black, SystemColors.Highlight.InvertRgb())));

            CacheData.SetComponentImageColor(appMenuItem3);
            _imageSwitcherCache.Add(nameof(appMenuItem3), (appMenuItem3.Image, Resources.Uncheck.RecolorPixels(Color.Black, SystemColors.Highlight.InvertRgb())));

            CacheData.SetComponentImageColor(appMenuItem4, Color.DarkGoldenrod);

            CacheData.SetComponentImageColor(appMenuItem5, Color.Firebrick);
            CacheData.SetComponentImageColor(appMenuItem6, Color.ForestGreen);

            if (EnvironmentEx.IsAtLeastWindows(11))
                Desktop.RoundCorners(appMenu.Handle, true);
            else
            {
                appMenu.EnableAnimation();
                appMenu.SetFixedSingle();
            }

            CacheData.SetComponentImageColor(settingsBtn, Color.LightBlue);
            if (Desktop.AppsUseDarkTheme)
            {
                settingsBtn.BackgroundImage = settingsBtn.BackgroundImage.SwitchGrayScale(settingsBtn);
                Desktop.EnableDarkMode(Handle);
                Desktop.EnableDarkMode(startBtn.Handle);
                Desktop.EnableDarkMode(cancelBtn.Handle);
                appMenu.ChangeColorMode();
                searchBox.ChangeColorMode(ControlExColorMode.DarkDark, ControlExColorMode.LightLightLight);
            }
            searchBox.DrawSearchSymbol(searchBox.ForeColor);

            ResumeLayout(false);

            if (!appsList.Focus())
                appsList.Select();

            if (ActionGuid.IsUpdateInstance)
                return;
            notifyBox?.Close();
            _notifyBox = new NotifyBox();
            _notifyBox.Show(LangStrings.DatabaseAccessMsg, AssemblyInfo.Title, NotifyBoxStartPosition.Center);
        }

        protected override void WndProc(ref Message m)
        {
            var previous = (int)WindowState;
            base.WndProc(ref m);
            var current = (int)WindowState;
            if (previous == 1 || current == 1 || previous == current)
                return;
            AppsListResizeColumns();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Width = Settings.WindowSize.Width;
            Height = Settings.WindowSize.Height;
            MinimumSize = MinimumSize.ScaleDimensions();
            MaximumSize = SizeEx.GetDesktopSize(Location);
            NativeHelper.CenterWindow(Handle);
            if (Settings.WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Maximized;
            this.Dockable();

            if (!NetEx.IPv4IsAvalaible)
            {
                if (!NetEx.IPv6IsAvalaible)
                {
                    if (!ActionGuid.IsUpdateInstance)
                        MessageBoxEx.Show(LangStrings.InternetIsNotAvailableMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ApplicationExit(1);
                    return;
                }
                MessageBoxEx.Show(LangStrings.InternetProtocolWarningMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!ActionGuid.IsUpdateInstance && !AppSupplierMirrors.Pa.Any())
            {
                MessageBoxEx.Show(LangStrings.NoServerAvailableMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ApplicationExit(1);
                return;
            }

            try
            {
                if (!CacheData.AppImages.Any())
                    throw new InvalidOperationException(LangStrings.NoSmallAppImagesFound);

                if (!CacheData.AppImagesLarge.Any())
                    throw new InvalidOperationException(LangStrings.NoLargeAppImagesFound);

                if (!CacheData.AppInfo.Any())
                    throw new InvalidOperationException(LangStrings.NoAppDataFound);

                if (ActionGuid.IsUpdateInstance)
                {
                    var appUpdates = AppSupply.FindOutdatedApps();
                    if (!appUpdates.Any())
                        throw new WarningException(LangStrings.NoUpdatesFound);

                    AppsListUpdate(appUpdates);
                    if (appsList.Items.Count == 0)
                        throw new InvalidOperationException(LangStrings.NoAppsFound);

                    var asterisk = (appsList.Items.Count == 1 ? LangStrings.AppUpdateAvailableMsg : LangStrings.AppUpdatesAvailableMsg).FormatInvariant(appsList.Items.Count);
                    if (MessageBoxEx.Show(asterisk, AssemblyInfo.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
                        throw new WarningException(LangStrings.UpdateCanceled);

                    foreach (var item in appsList.Items.Cast<ListViewItem>())
                        item.Checked = true;
                }
                else
                {
                    if (CacheData.AppInfo.Any())
                        AppsListUpdate();

                    if (appsList.Items.Count == 0)
                        throw new InvalidOperationException(LangStrings.NoAppsFound);
                }
            }
            catch (WarningException ex)
            {
                Log.Write(ex.Message);
                ApplicationExit(2);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
                if (!ActionGuid.IsUpdateInstance)
                    MessageBoxEx.Show(LangStrings.NoServerAvailableMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ApplicationExit(1);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            _notifyBox?.Close();
            TopMost = false;
            Refresh();
            var timer = new Timer(components)
            {
                Interval = 1,
                Enabled = true
            };
            timer.Tick += (_, _) =>
            {
                if (Opacity < 1d)
                {
                    AppsListResizeColumns();
                    Opacity += .05d;
                    return;
                }
                timer.Dispose();
            };
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            appsList.BeginUpdate();
            appsList.Visible = false;
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            appsList.EndUpdate();
            appsList.Visible = true;
            AppsListResizeColumns();
        }

        private void MainForm_SystemColorsChanged(object sender, EventArgs e) =>
            AppsListShowColors();

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_transferManager.Any() && MessageBoxEx.Show(this, LangStrings.AreYouSureMsg, AssemblyInfo.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            _appsListBackupClone?.Dispose();

            if (appsList.Enabled)
            {
                Settings.WindowState = WindowState;
                Settings.WindowSize = new(Width, Height);
                Settings.SaveToFile();
            }

            if (downloadHandler.Enabled)
                downloadHandler.Enabled = false;
            if (downloadStarter.Enabled)
                downloadStarter.Enabled = false;

            foreach (var appTransferor in _transferManager.Values)
                appTransferor.Transfer.CancelAsync();

            if (DirectoryEx.EnumerateFiles(Settings.TransferDir)?.Any() == true)
                CmdExec.WaitThenDelete(Settings.TransferDir);
        }

        private void AppsList_Enter(object sender, EventArgs e) =>
            AppsListShowColors(false);

        private void AppsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (sender is not ListView owner || e.CurrentValue == e.NewValue || owner.Items.Count < e.Index)
                return;

            var current = owner.Items[e.Index];
            var appData = CacheData.AppInfo.FirstOrDefault(x => x.Key == current.Name);
            if (appData == null || appData.Requirements.Any() != true)
                return;

            var installedApps = AppSupply.FindInstalledApps();
            foreach (var req in appData.Requirements.Where(s => !installedApps.Any(x => x.Requirements.Contains(s))))
            {
                foreach (var item in appsList.Items.Cast<ListViewItem>().Where(i => i.Name == req && installedApps.All(a => a.Key != i.Name)))
                {
                    _reqCounter.TryAdd(item.Index, 0);

                    if (e.NewValue == CheckState.Checked)
                        _reqCounter[item.Index]++;
                    else
                        _reqCounter[item.Index]--;

                    item.Checked = _reqCounter[item.Index] > 0;
                    break;
                }
            }
        }

        private void AppsList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_noAppsListItemCheckEvent ||
                sender is not ListView owner ||
                downloadStarter.Enabled ||
                downloadHandler.Enabled ||
                _transferManager.Values.Any(x => x.Transfer.IsBusy))
                return;
            startBtn.Enabled = owner.CheckedItems.Count > 0;
        }

        private void AppMenu_Opening(object sender, CancelEventArgs e)
        {
            if (appsList.SelectedItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            var isChecked = appsList.SelectedItems.Cast<ListViewItem>().FirstOrDefault()?.Checked ?? false;
            if (isChecked)
            {
                appMenuItem1.Text = appMenuItem1.Text.Replace(LangStrings.Check, LangStrings.Uncheck);
                appMenuItem1.Image = _imageSwitcherCache[nameof(appMenuItem1)].Item2;

                appMenuItem2.Text = appMenuItem2.Text.Replace(LangStrings.Check, LangStrings.Uncheck);
                appMenuItem2.Image = _imageSwitcherCache[nameof(appMenuItem2)].Item2;

                appMenuItem3.Text = appMenuItem3.Text.Replace(LangStrings.Check, LangStrings.Uncheck);
                appMenuItem3.Image = _imageSwitcherCache[nameof(appMenuItem3)].Item2;
            }
            else
            {
                appMenuItem1.Text = appMenuItem1.Text.Replace(LangStrings.Uncheck, LangStrings.Check);
                appMenuItem1.Image = _imageSwitcherCache[nameof(appMenuItem1)].Item1;

                appMenuItem2.Text = appMenuItem2.Text.Replace(LangStrings.Uncheck, LangStrings.Check);
                appMenuItem2.Image = _imageSwitcherCache[nameof(appMenuItem2)].Item1;

                appMenuItem3.Text = appMenuItem3.Text.Replace(LangStrings.Uncheck, LangStrings.Check);
                appMenuItem3.Image = _imageSwitcherCache[nameof(appMenuItem3)].Item1;
            }
        }

        private void AppMenuItem_Click(object sender, EventArgs e)
        {
            if (appsList.SelectedItems.Count == 0)
                return;
            var selectedItem = appsList.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem == default(ListViewItem))
                return;
            var owner = sender as ToolStripMenuItem;
            var appInfo = AppSupply.FindInstalledApps();
            switch (owner?.Name)
            {
                case nameof(appMenuItem1):
                    selectedItem.Checked = !selectedItem.Checked;
                    break;
                case nameof(appMenuItem2):
                case nameof(appMenuItem3):
                    var advancedGroup = appsList.Groups.Cast<ListViewGroup>().Last();
                    var isChecked = !selectedItem.Checked;
                    var isCategory = owner.Name == nameof(appMenuItem2);
                    var isInstalled = appInfo.Any(x => x.Key.EqualsEx(selectedItem.Name));
                    var isAdvanced = selectedItem.Group == advancedGroup;
                    _noAppsListItemCheckEvent = !isCategory;
                    appsList.BeginUpdate();
                    appsList.Items.Cast<ListViewItem>().ForEach(item =>
                    {
                        switch (isCategory)
                        {
                            case true when selectedItem.Group != item.Group:
                            case false when isAdvanced && item.Group == advancedGroup:
                                return;
                        }
                        if (item.Checked == isChecked || (!isInstalled && appInfo.Any(x => x.Key.EqualsEx(item.Name))))
                            return;
                        item.Checked = isChecked;
                    });
                    appsList.EndUpdate();
                    _noAppsListItemCheckEvent = false;
                    AppsList_ItemChecked(appsList, null);
                    break;
                case nameof(appMenuItem4):
                {
                    var appData = CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(selectedItem.Name));
                    var website = appData?.Website;
                    if (website?.StartsWithEx("https://", "http://") != true)
                        return;
                    Process.Start(website);
                    break;
                }
                case nameof(appMenuItem5):
                {
                    var appData = CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(selectedItem.Name));
                    var language = DefineInstallerLanguage(appData, selectedItem);
                    var fileHash = appData?.DownloadCollection.ContainsKey(language) switch
                    {
                        true => appData.DownloadCollection[language].FirstOrDefault()?.Item2,
                        _ => appData?.DownloadCollection.First().Value.First().Item2
                    };
                    if (fileHash == null)
                        return;
                    Process.Start($"https://www.virustotal.com/gui/file/{fileHash}");
                    break;
                }
                case nameof(appMenuItem6):
                {
                    var appData = CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(selectedItem.Name));
                    using var dialog = new AppInfoForm(appData, GetListViewItemImage(selectedItem));
                    dialog.TopMost = TopMost;
                    dialog.Plus();
                    dialog.ShowDialog(this);
                    break;
                }
            }
        }

        private void SettingBtn_MouseEnterLeave(object sender, EventArgs e)
        {
            if (sender is not Panel owner)
                return;
            owner.BackgroundImage = owner.BackgroundImage.SwitchGrayScale(owner);
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            var result = DialogResult.None;
            try
            {
                using Form dialog = new SettingsForm(Settings);
                dialog.TopMost = TopMost;
                dialog.Plus();
                result = dialog.ShowDialog(this);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            if (NativeHelper.GetForegroundWindow() != Handle)
                NativeHelper.SetForegroundWindow(Handle);
            if (result != DialogResult.Yes)
                return;
            appsList.SuspendLayout();
            appsList.ShowGroups = Settings.ShowGroups;
            AppsListShowColors();
            appsList.SmallImageList = Settings.ShowLargeImages ? largeImageList : smallImageList;
            appsList.ResumeLayout(true);
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            appsList.SelectedItems.Clear();
            var owner = sender as TextBox;
            if (string.IsNullOrEmpty(owner?.Text))
                return;
            var tmp = owner.Text;
            owner.Text = string.Empty;
            owner.Text = tmp;
        }

        private void SearchBox_KeyPress(object sender, KeyPressEventArgs e) =>
            e.Handled = e.KeyChar == (char)Keys.Return;

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (_appsListBackupClone != default)
            {
                appsList.BeginUpdate();
                appsList.ShowGroups = Settings.ShowGroups;
                try
                {
                    using var backupList = _appsListBackupClone;
                    for (var i = 0; i < appsList.Items.Count; i++)
                    {
                        var item = appsList.Items[i];
                        if (backupList.Items.Count < i)
                            break;
                        var clone = backupList.Items[i];
                        foreach (var group in appsList.Groups.Cast<ListViewGroup>().Where(g => clone.Group.Name.Equals(g.Name, StringComparison.Ordinal)))
                        {
                            if (!clone.Group.Name.Equals(item.Group.Name, StringComparison.Ordinal))
                                group.Items.Add(item);
                            break;
                        }
                    }
                }
                finally
                {
                    _appsListBackupClone = default;
                    appsList.Sort();
                    appsList.EndUpdate();
                    AppsListShowColors(false);
                    appsList.Groups.Cast<ListViewGroup>().First(g => g.Items.Count != 0)
                            .Items.Cast<ListViewItem>().First()?.EnsureVisible();
                }
            }

            var owner = sender as TextBox;
            if (string.IsNullOrEmpty(owner?.Text))
                return;
            if (_appsListBackupClone == default)
            {
                _appsListBackupClone = new();
                foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                    _appsListBackupClone.Groups.Add(new ListViewGroup
                    {
                        Header = group.Header,
                        Name = group.Name
                    });
                _appsListBackupClone.Items.Clear();
                foreach (var item in appsList.Items.Cast<ListViewItem>())
                    _appsListBackupClone.Items.Add((ListViewItem)item.Clone());
            }

            var first = default(ListViewItem);
            appsList.SetDoubleBuffer(false);
            appsList.BeginUpdate();
            try
            {
                foreach (var item in from item in appsList.Items.Cast<ListViewItem>()
                                     let desc = item.SubItems[1]
                                     where desc.Text.ContainsEx(owner.Text) ||
                                           item.Text.ContainsEx(owner.Text)
                                     select item)
                {
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>().Where(g => g.Name.EqualsEx("listViewGroup0") &&
                                                                                           !g.Items.Contains(item)))
                    {
                        appsList.ShowGroups = true;
                        first ??= item;
                        group.Items.Add(item);
                    }
                }
            }
            finally
            {
                appsList.EndUpdate();
                appsList.SetDoubleBuffer();
                first?.EnsureVisible();
            }

            AppsListShowColors();
            _counter.Reset(0);
            if (!searchResultBlinker.Enabled)
                searchResultBlinker.Enabled = true;
        }

        private void SearchResultBlinker_Tick(object sender, EventArgs e)
        {
            lock (SearchHandler)
            {
                if (sender is not Timer owner)
                    return;
                if (owner.Enabled && _counter.GetValue(0) >= 5)
                    owner.Enabled = false;
                appsList.SetDoubleBuffer(false);
                try
                {
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>().Where(g => g.Name.Equals("listViewGroup0", StringComparison.Ordinal)))
                    {
                        if (group.Items.Count <= 0)
                        {
                            owner.Enabled = false;
                            continue;
                        }
                        foreach (var item in appsList.Items.Cast<ListViewItem>().Where(i => i.Group.Name.Equals(group.Name, StringComparison.Ordinal)))
                        {
                            if (!owner.Enabled || item.BackColor != SystemColors.Highlight)
                            {
                                item.BackColor = SystemColors.Highlight;
                                continue;
                            }
                            item.BackColor = appsList.BackColor;
                        }
                    }
                    owner.Interval = owner.Interval >= 200 ? 100 : 200;
                }
                finally
                {
                    appsList.SetDoubleBuffer();
                }
                if (owner.Enabled)
                    _counter.Increase(0);
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (sender is not Button owner)
                return;

            var transferIsBusy = _transferManager.Values.Any(x => x.Transfer.IsBusy);
            if (!owner.Enabled || appsList.Items.Count == 0 || transferIsBusy)
                return;

            SuspendLayout();

            _noAppsListItemCheckEvent = true;
            owner.Enabled = false;
            owner.Visible = false;
            cancelBtn.Visible = false;
            searchBox.Parent.Visible = false;
            settingsBtn.Visible = false;
            searchBox.Text = string.Empty;

            appsList.SetDoubleBuffer(false);
            appsList.BeginUpdate();
            foreach (var item in appsList.Items.Cast<ListViewItem>().Where(i => !i.Checked))
                item.Remove();
            appsList.EndUpdate();
            appsList.SetDoubleBuffer();

            foreach (var filePath in AppSupply.FindAppInstaller())
                FileEx.TryDelete(filePath);

            appsList.HideSelection = true;
            appsList.Enabled = false;
            appsList.Sort();

            appMenuItem1.Enabled = false;
            appMenuItem2.Enabled = false;
            appMenuItem3.Enabled = false;

            settingsBtn.Enabled = false;
            searchBox.Enabled = false;
            cancelBtn.Enabled = false;

            Settings.ShowGroups = false;
            Settings.ShowGroupColors = false;
            Settings.HighlightInstalled = false;
            appsList.ShowGroups = false;
            AppsListShowColors();

            _transferManager.Clear();
            var totalDownloadSize = 0L;
            var totalInstallSize = 0L;
            foreach (var item in appsList.CheckedItems.Cast<ListViewItem>())
            {
                var appData = CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(item.Name));
                if (appData == default)
                    continue;

                _ = DefineInstallerLanguage(appData, item, true);
                _transferManager.Add(item, new AppTransferor(appData));

                unchecked
                {
                    totalDownloadSize += appData.DownloadSize;
                    totalInstallSize += appData.InstallSize;
                }
            }

            (char, long, long)[] spaceData;
            if (Settings.TransferDir.StartsWithEx(CorePaths.HomeDir))
                spaceData = new[]
                {
                    (CorePaths.HomeDir.First(), unchecked(totalDownloadSize + totalInstallSize), DirectoryEx.GetFreeSpace(CorePaths.HomeDir))
                };
            else
                spaceData = new[]
                {
                    (Settings.TransferDir.First(), totalDownloadSize, DirectoryEx.GetFreeSpace(Settings.TransferDir)),
                    (CorePaths.HomeDir.First(), totalInstallSize, DirectoryEx.GetFreeSpace(CorePaths.HomeDir))
                };
            foreach (var (item1, item2, item3) in spaceData)
            {
                if (item2 < item3)
                    continue;
                var warning = LangStrings.NotEnoughSpaceMsg.FormatInvariant(item1, (item2 - item3).FormatSize());
                switch (MessageBoxEx.Show(this, warning, AssemblyInfo.Title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                {
                    case DialogResult.Abort:
                        _transferManager.Clear();
                        ApplicationExit();
                        return;
                    case DialogResult.Retry:
                        owner.Enabled = true;

                        owner.Visible = true;
                        cancelBtn.Visible = true;
                        searchBox.Parent.Visible = true;
                        settingsBtn.Visible = true;

                        appsList.HideSelection = false;
                        appsList.Enabled = true;
                        appMenuItem1.Enabled = true;
                        appMenuItem2.Enabled = true;
                        appMenuItem3.Enabled = true;
                        cancelBtn.Enabled = true;
                        return;
                }
            }

            ResumeLayout();

            downloadStarter.Enabled = !owner.Enabled;
        }

        private void CancelBtn_Click(object sender, EventArgs e) =>
            ApplicationExit();

        private void DownloadStarter_Tick(object sender, EventArgs e)
        {
            if (sender is not Timer owner)
                return;
            lock (DownloadStarter)
            {
                owner.Enabled = false;

                if (_transferManager.Any(x => x.Value.Transfer.IsBusy))
                    return;

                try
                {
                    _currentTransfer = _transferManager.First(x =>
                    {
                        if (_transferFails.Contains(x.Value.AppData))
                            return false;
                        return x.Key.Checked || x.Value.Transfer.HasCanceled;
                    });
                }
                catch (Exception ex) when (ex.IsCaught())
                {
                    Log.Write(ex);
                    ApplicationExit(1);
                    return;
                }

                appStatus.Text = _currentTransfer.Key.Text;
                Text = $@"{LangStrings.FileCountStatus.FormatInvariant(_transferManager.Keys.Count(x => !x.Checked), _transferManager.Keys.Count)} - {appStatus.Text}";

                if (!_transferStopwatch.IsRunning)
                    _transferStopwatch.Start();

                var appTransferor = _currentTransfer.Value;
                _transferTask?.Dispose();
                _transferTask = Task.Run(() => appTransferor.StartDownload());

                appsList.EnsureVisible(_currentTransfer.Key.Index);
                downloadHandler.Enabled = !owner.Enabled;
            }
        }

        private void DownloadHandler_Tick(object sender, EventArgs e)
        {
            if (sender is not Timer owner)
                return;
            lock (DownloadHandler)
            {
                var appTransferor = _currentTransfer.Value;
                DownloadProgressUpdate(appTransferor.Transfer.ProgressPercentage);

                if (buttonAreaBorder.Visible)
                {
                    SuspendLayout();

                    buttonAreaBorder.Visible = false;
                    buttonAreaPanel.Visible = false;

                    statusAreaPanel.Visible = true;
                    statusAreaBorder.Visible = true;

                    appsList.Height = ClientSize.Height - statusAreaPanel.Height - statusAreaBorder.Height;

                    statusAreaLeftPanel.SetDoubleBuffer();
                    statusAreaRightPanel.SetDoubleBuffer();

                    ResumeLayout(true);
                }

                Icon = CacheData.GetAppIcon(appTransferor.AppData.Key, Resources.PaLogoGreenSymbol);

                statusAreaLeftPanel.SuspendLayout();
                fileStatus.Text = string.IsNullOrEmpty(appTransferor.Transfer.FileName) switch
                {
                    false => appTransferor.Transfer.FileName,
                    _ => LangStrings.InitStatusText
                };
                fileStatus.Text = fileStatus.Text.Trim(fileStatus.Font, fileStatus.Width);
                urlStatus.Text = string.IsNullOrEmpty(appTransferor.DownloadHost) switch
                {
                    false => appTransferor.DownloadHost,
                    _ => LangStrings.InitStatusText
                };
                urlStatus.Text = urlStatus.Text.Trim(urlStatus.Font, urlStatus.Width);
                statusAreaLeftPanel.ResumeLayout();

                statusAreaRightPanel.SuspendLayout();
                downloadReceived.Text = appTransferor.Transfer.DataReceived;
                if (downloadReceived.Text.EqualsEx("0.00 bytes / 0.00 bytes"))
                    downloadReceived.Text = LangStrings.InitStatusText;
                downloadSpeed.Text = appTransferor.Transfer.TransferSpeedAd;
                if (downloadSpeed.Text.EqualsEx("0.00 bit/s"))
                    downloadSpeed.Text = LangStrings.InitStatusText;
                timeStatus.Text = _transferStopwatch.Elapsed.ToString("mm\\:ss\\.fff", CultureInfo.InvariantCulture);
                statusAreaRightPanel.ResumeLayout();

                if (_transferTask?.Status == TaskStatus.Running || appTransferor.Transfer.IsBusy)
                {
                    _counter.Reset(1);
                    return;
                }
                if (_counter.Increase(1) < (int)Math.Floor(1000d / owner.Interval))
                    return;

                owner.Enabled = false;
                if (!appTransferor.DownloadStarted)
                {
                    if (!_autoRetry)
                        _autoRetry = appTransferor.AutoRetry;
                    _transferFails.Add(appTransferor.AppData);
                }

                if (appsList.CheckedItems.Count > 0)
                {
                    var listViewItem = _currentTransfer.Key;
                    _noAppsListItemCheckEvent = false;
                    listViewItem.Checked = false;
                    _noAppsListItemCheckEvent = true;
                    if (appsList.CheckedItems.Count > 0)
                    {
                        downloadStarter.Enabled = true;
                        return;
                    }
                }

                var windowState = WindowState;
                WindowState = FormWindowState.Minimized;

                Text = AssemblyInfo.Title;
                _transferStopwatch.Stop();
                TaskBarProgress.SetState(Handle, TaskBarProgressState.Indeterminate);

                Icon = Resources.InstallSymbol;
                var installed = new List<ListViewItem>();
                installed.AddRange(_transferManager.Where(x => x.Value.StartInstall()).Select(x => x.Key));
                if (installed.Any())
                    foreach (var key in installed)
                        _transferManager.Remove(key);
                if (_transferManager.Any())
                    _transferFails.AddRange(_transferManager.Values.Select(x => x.AppData));

                _transferManager.Clear();
                _currentTransfer = default;

                Icon = Resources.PaLogoGreenSymbol;
                TopMost = true;
                WindowState = windowState;

                if (_transferFails.Any())
                {
                    TaskBarProgress.SetState(Handle, TaskBarProgressState.Error);
                    var fails = _transferFails.Select(x => x.Name).Distinct().ToArray();
                    var warning = (fails.Length == 1 ? LangStrings.AppDownloadErrorMsg : LangStrings.AppsDownloadErrorMsg).FormatInvariant(fails.Join(Environment.NewLine));
                    switch (_autoRetry ? DialogResult.Retry : MessageBoxEx.Show(this, warning, AssemblyInfo.Title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                    {
                        case DialogResult.Retry:
                            _autoRetry = false;

                            foreach (var appData in _transferFails.Distinct())
                            {
                                var item = appsList.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Name.EqualsEx(appData.Key));
                                if (item == default(ListViewItem))
                                    continue;
                                item.Checked = true;
                            }
                            _transferFails.Clear();

                            SuspendLayout();

                            TopMost = false;
                            appsList.Enabled = true;
                            appsList.HideSelection = false;

                            downloadSpeed.Text = string.Empty;
                            DownloadProgressUpdate(0);
                            downloadReceived.Text = string.Empty;

                            settingsBtn.Enabled = true;
                            searchBox.Enabled = true;

                            startBtn.Enabled = true;
                            cancelBtn.Enabled = true;

                            startBtn.Visible = true;
                            cancelBtn.Visible = true;
                            searchBox.Parent.Visible = true;
                            settingsBtn.Visible = true;

                            statusAreaBorder.Visible = false;
                            statusAreaPanel.Visible = false;

                            buttonAreaBorder.Visible = true;
                            buttonAreaPanel.Visible = true;
                            appsList.Height = ClientSize.Height - buttonAreaPanel.Height - buttonAreaBorder.Height;

                            statusAreaLeftPanel.SetDoubleBuffer(false);
                            statusAreaRightPanel.SetDoubleBuffer(false);

                            ResumeLayout();

                            StartBtn_Click(startBtn, EventArgs.Empty);
                            return;
                        default:
                            if (ActionGuid.IsUpdateInstance)
                                foreach (var appData in _transferFails)
                                {
                                    appData.Settings.DisableUpdates = true;
                                    appData.Settings.DelayUpdates = DateTime.Now;
                                }
                            break;
                    }
                }

                TaskBarProgress.SetValue(Handle, 100, 100);
                var information = ActionGuid.IsUpdateInstance switch
                {
                    true => installed.Count switch
                    {
                        1 => LangStrings.AppUpdatedMsg,
                        _ => LangStrings.AppsUpdatedMsg
                    },
                    _ => installed.Count switch
                    {
                        1 => LangStrings.AppDownloadedMsg,
                        _ => LangStrings.AppsDownloadedMsg
                    }
                };
                MessageBoxEx.Show(this, information, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ApplicationExit();
            }
        }

        private void UrlStatus_DoubleClick(object sender, EventArgs e)
        {
            if (appsList.Enabled)
                return;
            var appTransferor = _currentTransfer.Value;
            if (!appTransferor.Transfer.IsBusy)
                return;
            appTransferor.Transfer.CancelAsync();
            SystemSounds.Hand.Play();
        }

        private static void ApplicationExit(int exitCode = 0)
        {
            if (exitCode > 0)
            {
                Environment.ExitCode = exitCode;
                Environment.Exit(Environment.ExitCode);
            }
            Application.Exit();
        }

        private static Image GetListViewItemImage(ListViewItem item)
        {
            if (item?.ImageList?.Images is not { } images)
                return default;
            var index = item.ImageIndex;
            return index.IsBetween(0, images.Count - 1) ? images[index] : default;
        }

        private void AppsListResizeColumns()
        {
            if (appsList.Columns.Count < 5)
                return;
            var staticColumnsWidth = SystemInformation.VerticalScrollBarWidth + 2;
            for (var i = 3; i < appsList.Columns.Count; i++)
                staticColumnsWidth += appsList.Columns[i].Width;
            var dynamicColumnsWidth = 0;
            while (dynamicColumnsWidth < appsList.Width - staticColumnsWidth)
                dynamicColumnsWidth++;
            for (var i = 0; i < 3; i++)
                appsList.Columns[i].Width = (int)Math.Ceiling(dynamicColumnsWidth / 100d * i switch
                {
                    0 => 35d,
                    1 => 50d,
                    _ => 15d
                });
        }

        private void AppsListShowColors(bool searchResultColor = true)
        {
            if (searchResultBlinker.Enabled)
                searchResultBlinker.Enabled = false;
            var appInfo = new List<AppData>();
            if (Settings.HighlightInstalled)
                appInfo = AppSupply.FindInstalledApps();
            appsList.SetDoubleBuffer(false);
            appsList.BeginUpdate();
            try
            {
                var lightGreen = Color.FromArgb(0xc0, 0xff, 0xc0);
                var darkGreen = Color.FromArgb(0x20, 0x40, 0x20);
                var seed = DateTime.Now.GetHashCode();
                foreach (var item in appsList.Items.Cast<ListViewItem>())
                {
                    var groupName = item.Group.Name;
                    if (searchResultColor && groupName.Equals("listViewGroup0", StringComparison.Ordinal))
                    {
                        item.BackColor = SystemColors.Highlight;
                        item.ForeColor = SystemColors.HighlightText;
                        continue;
                    }
                    if (Settings.HighlightInstalled && appInfo.Any(x => x.Key.EqualsEx(item.Name)))
                    {
                        item.Font = new Font(appsList.Font, FontStyle.Italic);
                        item.BackColor = !Settings.ShowGroupColors && appsList.BackColor.IsDarkDark() ? darkGreen : lightGreen;
                        item.ForeColor = !Settings.ShowGroupColors && appsList.BackColor.IsDarkDark() ? lightGreen : darkGreen;
                        continue;
                    }
                    item.Font = appsList.Font;
                    if (!Settings.ShowGroupColors)
                    {
                        item.BackColor = appsList.BackColor;
                        item.ForeColor = appsList.ForeColor;
                        continue;
                    }
                    switch (groupName)
                    {
                        case "listViewGroup0":
                            item.BackColor = appsList.BackColor;
                            item.ForeColor = appsList.ForeColor;
                            break;
                        default:
                            if (Settings.GroupColors.TryGetValue(groupName, out var color))
                            {
                                item.BackColor = color;
                                item.ForeColor = color.IsDark() ? Color.White : Color.Black;
                                break;
                            }

                            // Create random group color.
                            var range = Settings.GroupColors.Values.ToList();
                            range.Add(lightGreen);
                            range.Add(darkGreen);
                            do
                                color = ColorEx.GetRandomColor(seed);
                            while (color.IsDark() || color.IsLightLight() || range.Any(x => x.IsInRange(color, 20)));

                            item.BackColor = color;
                            item.ForeColor = color.IsDark() ? Color.White : Color.Black;

                            Settings.GroupColors.Add(groupName, color);
                            break;
                    }

                    if (item.BackColor == appsList.BackColor)
                        continue;

                    if (Settings.HighlightInstalled && appInfo.Any(x => x.Key.EqualsEx(item.Name)))
                        item.BackColor = ControlPaint.LightLight(item.BackColor);
                }
            }
            finally
            {
                appsList.EndUpdate();
                appsList.SetDoubleBuffer();
            }
        }

        private void AppsListUpdate(List<AppData> appInfo = default)
        {
            var index = 0;
            var appImages = CacheData.AppImages.ToDictionary(x => x.Key, x => x.Value);
            var appImagesLarge = CacheData.AppImagesLarge.ToDictionary(x => x.Key, x => x.Value);

            // Get app images of custom app suppliers.
            if (CacheData.CustomAppSuppliers?.Any() == true)
            {
                var appImagesNames = new[]
                {
                    Path.GetFileName(CorePaths.AppImages),
                    Path.GetFileName(CorePaths.AppImagesLarge)
                };
                foreach (var data in CacheData.CustomAppSuppliers)
                {
                    var address = data.Address;
                    var user = data.User;
                    var password = data.Password;
                    var userAgent = data.UserAgent;
                    for (var i = 0; i < appImagesNames.Length; i++)
                    {
                        var name = appImagesNames[i];
                        var url = PathEx.AltCombine(default(char[]), address, name);
                        if (Log.DebugMode > 0)
                            Log.Write($"Custom: Looking for '{url}'.");
                        if (!NetEx.FileIsAvailable(url, user, password, 60000, userAgent))
                            continue;
                        var customAppImages = WebTransfer.DownloadData(url, user, password, 60000, userAgent)?.DeserializeObject<Dictionary<string, Image>>();
                        if (customAppImages == null)
                            continue;
                        foreach (var pair in customAppImages)
                            (i == 0 ? appImages : appImagesLarge).TryAdd(pair.Key, pair.Value);
                    }
                }
            }

            appsList.BeginUpdate();
            appsList.Items.Clear();
            Image smallDef = default,
                  largeDef = default,
                  smallDepracted = default,
                  largeDepracted = default;
            var filter = new[] { "Depracted", "Discontinued", "Legacy" };
            foreach (var appData in appInfo ?? CacheData.AppInfo)
            {
                var url = appData.DownloadCollection.First().Value.First().Item1;
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                var src = LangStrings.HostNotAvailable;
                if (url.StartsWith("{", StringComparison.InvariantCulture) && url.EndsWith("}", StringComparison.InvariantCulture))
                {
                    var searchData = Json.Deserialize<Dictionary<string, string>>(url);
                    if (searchData?.ContainsKey("source") != true)
                        continue;
                    url = searchData["source"];
                }
                if (url.StartsWithEx("http"))
                    if (url.ContainsEx(AppSupplierHosts.Pac) && url.ContainsEx("/redirect/") && url.ContainsEx("&d=sfpa"))
                        src = AppSupplierHosts.Sf;
                    else
                    {
                        src = NetEx.GetShortHost(url);
                        if (string.IsNullOrEmpty(src))
                            continue;
                    }
                else
                {
                    if (appData.Supplier != null)
                        src = NetEx.GetFullHost(appData.Supplier.Address);
                }

                var item = new ListViewItem(appData.Name)
                {
                    Name = appData.Key
                };
                item.SubItems.Add(appData.Description);
                item.SubItems.Add(appData.DisplayVersion);
                item.SubItems.Add(appData.DownloadSize.FormatSize(SizeOption.Trim));
                item.SubItems.Add(appData.InstallSize.FormatSize(SizeOption.Trim));
                item.SubItems.Add(src);
                item.ImageIndex = index;

                if (appData.Key.ContainsEx(filter) ||
                    appData.Name.ContainsEx(filter) ||
                    appData.Description.ContainsEx(filter) ||
                    appData.DisplayVersion.ContainsEx(filter))
                {
                    smallDepracted ??= Resources.FishBones.RecolorPixels(Color.Black, Color.DarkGray).Redraw(16);
                    smallImageList.Images.Add(appData.Key, smallDepracted);

                    largeDepracted ??= Resources.FishBones.RecolorPixels(Color.Black, Color.DarkGray).Redraw(32);
                    largeImageList.Images.Add(appData.Key, largeDepracted);
                }
                else
                {
                    if (appImages?.TryGetValue(appData.Key, out var image) == true)
                        if (image != null)
                            smallImageList.Images.Add(appData.Key, image);

                    if (appImagesLarge?.TryGetValue(appData.Key, out var img) == true)
                        if (img != null)
                            largeImageList.Images.Add(appData.Key, img);
                }

                var smallImageAdded = smallImageList.Images.ContainsKey(appData.Key);
                var largeImageAdded = largeImageList.Images.ContainsKey(appData.Key);
                if (!smallImageAdded || !largeImageAdded)
                {
                    appData.Advanced = true;
                    if (!smallImageAdded)
                    {
                        if (Log.DebugMode > 0)
                            Log.Write($"Cache: Could not find target '{CacheFiles.AppImages}:{appData.Key}'.");
                        smallDef ??= Resources.ImageSlash.RecolorPixels(Color.Black, Color.DarkGray).Redraw(16);
                        smallImageList.Images.Add(appData.Key, smallDef);
                    }
                    if (!largeImageAdded)
                    {
                        if (Log.DebugMode > 0) 
                            Log.Write($"Cache: Could not find target '{CacheFiles.AppImagesLarge}:{appData.Key}'.");
                        largeDef ??= Resources.ImageSlash.RecolorPixels(Color.Black, Color.DarkGray);
                        largeImageList.Images.Add(appData.Key, largeDef);
                    }
                }

                if (appData.Supplier != null)
                {
                    var groupFound = false;

                    Grouping:
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                    {
                        var enName = group.Header;
                        if (!enName.EqualsEx(appData.Category))
                            continue;
                        groupFound = true;
                        appsList.Items.Add(item).Group = group;
                        break;
                    }

                    if (!groupFound)
                    {
                        var newGroupName = appData.Category;
                        var newGroup = new ListViewGroup(newGroupName, HorizontalAlignment.Left)
                        {
                            Header = newGroupName,
                            Name = newGroupName
                        };
                        appsList.Groups.Add(newGroup);
                        groupFound = true;
                        goto Grouping;
                    }
                }
                else
                {
                    var groups = appsList.Groups.Cast<ListViewGroup>().ToArray();
                    var advanced = groups.Last().Header;
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                    {
                        var groupName = group.Header;
                        if ((appData.Advanced || !groupName.EqualsEx(appData.Category)) && !groupName.EqualsEx(advanced))
                            continue;
                        appsList.Items.Add(item).Group = group;
                        break;
                    }
                }

                index++;
            }

            if (Log.DebugMode > 0)
                Log.Write($"Interface: {appsList.Items.Count} {(appsList.Items.Count == 1 ? "App" : "Apps")} has been added!");

            appsList.ShowGroups = Settings.ShowGroups;
            appsList.SmallImageList = Settings.ShowLargeImages ? largeImageList : smallImageList;
            appsList.EndUpdate();
            AppsListShowColors();
        }

        private string DefineInstallerLanguage(AppData appData, ListViewItem item, bool needed = false)
        {
            var selected = appData?.DefaultLanguage ?? string.Empty;
            if (!(appData?.DownloadCollection.Count > 1) || appData.Settings.ArchiveLangConfirmed)
                return selected;
            try
            {
                DialogResult result;
                do
                {
                    using Form dialog = new LangSelectionForm(appData, GetListViewItemImage(item));
                    TopMost = false;
                    result = dialog.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        selected = appData.Settings.ArchiveLang;
                        break;
                    }
                    TopMost = true;
                    result = MessageBoxEx.Show(this, LangStrings.AreYouSureMsg, AssemblyInfo.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        continue;
                    if (!needed)
                        return selected;
                    _transferManager.Clear();
                    ApplicationExit();
                    return selected;
                }
                while (result != DialogResult.OK);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            return selected;
        }

        private void DownloadProgressUpdate(int value)
        {
            var color = PanelFakeProgressBar.SetProgress(downloadProgress, value);
            appStatus.ForeColor = color;
            fileStatus.ForeColor = color;
            urlStatus.ForeColor = color;
            downloadReceived.ForeColor = color;
            downloadSpeed.ForeColor = color;
            timeStatus.ForeColor = color;
        }
    }
}
