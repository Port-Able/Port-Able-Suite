namespace AppsDownloader.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using LangResources;
    using Libraries;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;
    using SilDev.Investment;

    public sealed partial class MainForm : Form
    {
        private static readonly object DownloadStarter = new object(),
                                       DownloadHandler = new object(),
                                       SearchHandler = new object();

        public MainForm(NotifyBox notifyBox = default)
        {
            InitializeComponent();

            Language.SetControlLang(this);
            Text = Settings.Title;
            for (var i = 0; i < appsList.Columns.Count; i++)
                appsList.Columns[i].Text = Language.GetText($"columnHeader{i + 1}");
            for (var i = 0; i < appsList.Groups.Count; i++)
                appsList.Groups[i].Header = Language.GetText(appsList.Groups[i].Name);
            for (var i = 0; i < appMenu.Items.Count; i++)
                appMenu.Items[i].Text = Language.GetText(appMenu.Items[i].Name);
            var statusLabels = new[]
            {
                appStatusLabel,
                fileStatusLabel,
                urlStatusLabel,
                downloadReceivedLabel,
                downloadSpeedLabel,
                timeStatusLabel
            };
            foreach (var label in statusLabels)
                label.Text = Language.GetText(label.Name);

            Icon = Resources.Logo;

            appsList.ListViewItemSorter = new ListViewEx.AlphanumericComparer();
            appsList.SetDoubleBuffer();

            GroupColors = new Dictionary<string, Color>
            {
                {
                    "listViewGroup1",
                    Settings.Window.Colors.GroupColor1
                },
                {
                    "listViewGroup2",
                    Settings.Window.Colors.GroupColor2
                },
                {
                    "listViewGroup3",
                    Settings.Window.Colors.GroupColor3
                },
                {
                    "listViewGroup4",
                    Settings.Window.Colors.GroupColor4
                },
                {
                    "listViewGroup5",
                    Settings.Window.Colors.GroupColor5
                },
                {
                    "listViewGroup6",
                    Settings.Window.Colors.GroupColor6
                },
                {
                    "listViewGroup7",
                    Settings.Window.Colors.GroupColor7
                },
                {
                    "listViewGroup8",
                    Settings.Window.Colors.GroupColor8
                },
                {
                    "listViewGroup9",
                    Settings.Window.Colors.GroupColor9
                },
                {
                    "listViewGroup11",
                    Settings.Window.Colors.GroupColor11
                },
                {
                    "listViewGroup12",
                    Settings.Window.Colors.GroupColor12
                }
            };

            appMenuItem3.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Network);
            appMenuItem4.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Asterisk);
            appMenu.EnableAnimation();
            appMenu.SetFixedSingle();

            statusAreaLeftPanel.SetDoubleBuffer();
            statusAreaRightPanel.SetDoubleBuffer();

            settingsBtn.BackgroundImage = CacheData.GetSystemImage(ResourcesEx.IconIndex.SystemControl, true);
            searchBox.DrawSearchSymbol(searchBox.ForeColor);

            settingsAreaPanel.ResumeLayout(false);
            settingsAreaPanel.PerformLayout();
            buttonAreaPanel.ResumeLayout(false);
            statusAreaLayoutPanel.ResumeLayout(false);
            statusAreaLeftPanel.ResumeLayout(false);
            statusAreaRightPanel.ResumeLayout(false);
            statusAreaPanel.ResumeLayout(false);
            appMenu.ResumeLayout(false);
            ResumeLayout(false);

            if (!appsList.Focus())
                appsList.Select();

            if (ActionGuid.IsUpdateInstance)
                return;
            notifyBox?.Close();
            NotifyBox = new NotifyBox();
            NotifyBox.Show(Language.GetText(nameof(en_US.DatabaseAccessMsg)), Settings.Title, NotifyBoxStartPosition.Center);
        }

        private bool AutoRetry { get; set; }

        private ListView AppsListClone { get; } = new ListView();

        private Dictionary<string, Color> GroupColors { get; }

        private CounterInvestor<int> Counter { get; } = new CounterInvestor<int>();

        private NotifyBox NotifyBox { get; }

        private static Task TransferTask { get; set; }

        private Dictionary<ListViewItem, AppTransferor> TransferManager { get; } = new Dictionary<ListViewItem, AppTransferor>();

        private KeyValuePair<ListViewItem, AppTransferor> CurrentTransfer { get; set; }

        private Stopwatch TransferStopwatch { get; } = new Stopwatch();

        private List<AppData> TransferFails { get; } = new List<AppData>();

        private void MainForm_Load(object sender, EventArgs e)
        {
            MinimumSize = Settings.Window.Size.Minimum;
            MaximumSize = Settings.Window.Size.Maximum;
            if (Settings.Window.Size.Width > Settings.Window.Size.Minimum.Width)
                Width = Settings.Window.Size.Width;
            if (Settings.Window.Size.Height > Settings.Window.Size.Minimum.Height)
                Height = Settings.Window.Size.Height;
            WinApi.NativeHelper.CenterWindow(Handle);
            if (Settings.Window.State == FormWindowState.Maximized)
                WindowState = FormWindowState.Maximized;

            FormEx.Dockable(this);

            if (!Network.InternetIsAvailable)
            {
                if (!ActionGuid.IsUpdateInstance)
                    MessageBoxEx.Show(Language.GetText(nameof(en_US.InternetIsNotAvailableMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ApplicationExit(1);
                return;
            }

            if (!ActionGuid.IsUpdateInstance && !AppSupply.GetMirrors(AppSuppliers.Internal).Any())
            {
                MessageBoxEx.Show(Language.GetText(nameof(en_US.NoServerAvailableMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ApplicationExit(1);
                return;
            }

            try
            {
                if (!CacheData.AppImages.Any())
                    throw new InvalidOperationException("No small app image found.");

                if (!CacheData.AppImagesLarge.Any())
                    throw new InvalidOperationException("No large app image found.");

                if (!CacheData.AppInfo.Any())
                    throw new InvalidOperationException("No app data found.");

                if (ActionGuid.IsUpdateInstance)
                {
                    var appUpdates = AppSupply.FindOutdatedApps();
                    if (!appUpdates.Any())
                        throw new WarningException("No updates available.");

                    AppsListUpdate(appUpdates);
                    if (appsList.Items.Count == 0)
                        throw new InvalidOperationException("No apps available.");

                    var asterisk = string.Format(Language.GetText(appsList.Items.Count == 1 ? nameof(en_US.AppUpdateAvailableMsg) : nameof(en_US.AppUpdatesAvailableMsg)), appsList.Items.Count);
                    if (MessageBoxEx.Show(asterisk, Settings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
                        throw new WarningException("Update canceled.");

                    foreach (var item in appsList.Items.Cast<ListViewItem>())
                        item.Checked = true;
                }
                else
                {
                    if (CacheData.AppInfo.Any())
                        AppsListUpdate();

                    if (appsList.Items.Count == 0)
                        throw new InvalidOperationException("No apps available.");
                }
            }
            catch (WarningException ex)
            {
                Log.Write(ex.Message);
                ApplicationExit(2);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                if (!ActionGuid.IsUpdateInstance)
                    MessageBoxEx.Show(Language.GetText(nameof(en_US.NoServerAvailableMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ApplicationExit(1);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            NotifyBox?.Close();
            TopMost = false;
            Refresh();
            var timer = new Timer(components)
            {
                Interval = 1,
                Enabled = true
            };
            timer.Tick += (o, args) =>
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
            if (TransferManager.Any() && MessageBoxEx.Show(this, Language.GetText(nameof(en_US.AreYouSureMsg)), Settings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            Settings.Window.State = WindowState;
            Settings.Window.Size.Width = Width;
            Settings.Window.Size.Height = Height;

            if (downloadHandler.Enabled)
                downloadHandler.Enabled = false;
            if (downloadStarter.Enabled)
                downloadStarter.Enabled = false;

            foreach (var appTransferor in TransferManager.Values)
                appTransferor.Transfer.CancelAsync();

            if (DirectoryEx.EnumerateFiles(Settings.TransferDir)?.Any() == true)
                ProcessEx.SendHelper.WaitThenDelete(Settings.TransferDir);
        }

        private void AppsList_Enter(object sender, EventArgs e) =>
            AppsListShowColors(false);

        private void AppsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var appData = CacheData.AppInfo?.FirstOrDefault(x => x.Key.Equals(appsList.Items[e.Index].Name));
            if (appData?.Requirements?.Any() != true)
                return;
            var checkedNames = appsList.CheckedItems.Cast<ListViewItem>().Where(x => !x.Name.EqualsEx(appData.Key)).Select(x => x.Name).ToArray();
            var requirements = appData.Requirements;
            if (checkedNames.Any())
            {
                var equalReqsChecked = CacheData.AppInfo.Where(x => !requirements.Contains(x.Key) && requirements.SequenceEqual(x.Requirements) && checkedNames.Contains(x.Key));
                if (equalReqsChecked.Any())
                    return;
            }
            var installedApps = AppSupply.FindInstalledApps();
            foreach (var requirement in requirements)
            {
                if (installedApps.Any(x => x.Requirements.Contains(requirement)))
                    continue;
                foreach (var item in appsList.Items.Cast<ListViewItem>())
                {
                    if (!item.Name.Equals(requirement))
                        continue;
                    item.Checked = e.NewValue == CheckState.Checked;
                    break;
                }
            }
        }

        private void AppsList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!downloadStarter.Enabled && !downloadHandler.Enabled && !TransferManager.Values.Any(x => x.Transfer.IsBusy))
                startBtn.Enabled = appsList.CheckedItems.Count > 0;
        }

        private void AppsListReset()
        {
            if (AppsListClone.Items.Count != appsList.Items.Count)
            {
                AppsListShowColors(false);
                appsList.Sort();
                return;
            }
            for (var i = 0; i < appsList.Items.Count; i++)
            {
                var item = appsList.Items[i];
                var clone = AppsListClone.Items[i];
                foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                    if (clone.Group.Name.Equals(group.Name))
                    {
                        if (!clone.Group.Name.Equals(item.Group.Name))
                            group.Items.Add(item);
                        break;
                    }
            }
            AppsListShowColors(false);
            appsList.Sort();
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
                appsList.Columns[i].Width = (int)Math.Ceiling(dynamicColumnsWidth / 100d * (i == 0 ? 35d : i == 1 ? 50d : 15d));
        }

        private void AppsListShowColors(bool searchResultColor = true)
        {
            if (searchResultBlinker.Enabled)
                searchResultBlinker.Enabled = false;
            var appInfo = new List<AppData>();
            if (Settings.Window.HighlightInstalled)
                appInfo = AppSupply.FindInstalledApps();
            appsList.SetDoubleBuffer(false);
            appsList.BeginUpdate();
            try
            {
                var lightGreen = Color.FromArgb(0xc0, 0xff, 0xc0);
                var darkGreen = Color.FromArgb(0x20, 0x40, 0x20);
                foreach (var item in appsList.Items.Cast<ListViewItem>())
                {
                    var groupName = item.Group.Name;
                    if (searchResultColor && groupName.Equals(nameof(en_US.listViewGroup0)))
                    {
                        item.BackColor = SystemColors.Highlight;
                        item.ForeColor = SystemColors.HighlightText;
                        continue;
                    }
                    if (Settings.Window.HighlightInstalled && appInfo.Any(x => x.Key.EqualsEx(item.Name)))
                    {
                        item.Font = new Font(appsList.Font, FontStyle.Italic);
                        item.BackColor = !Settings.Window.ShowGroupColors && appsList.BackColor.IsDarkDark() ? darkGreen : lightGreen;
                        item.ForeColor = !Settings.Window.ShowGroupColors && appsList.BackColor.IsDarkDark() ? lightGreen : darkGreen;
                        continue;
                    }
                    item.Font = appsList.Font;
                    if (!Settings.Window.ShowGroupColors)
                    {
                        item.BackColor = appsList.BackColor;
                        item.ForeColor = appsList.ForeColor;
                        continue;
                    }
                    switch (groupName)
                    {
                        case "listViewGroup0":
                        case "listViewGroup10":
                            item.BackColor = appsList.BackColor;
                            item.ForeColor = appsList.ForeColor;
                            break;
                        default:
                            if (GroupColors.TryGetValue(groupName, out var color) ||
                                GroupColors.TryGetValue("listViewGroup12", out color))
                            {
                                item.BackColor = color;
                                item.ForeColor = color.IsDarkDark() ? Color.White : Color.Black;
                                break;
                            }
                            color = ColorEx.GetRandomColor(0).EnsureDark();
                            item.BackColor = color;
                            GroupColors.Add(groupName, color);
                            break;
                    }

                    if (item.BackColor == appsList.BackColor)
                        continue;

                    if (Settings.Window.HighlightInstalled && appInfo.Any(x => x.Key.EqualsEx(item.Name)))
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
            var appImages = CacheData.AppImages ?? new Dictionary<string, Image>();
            var appImagesLarge = CacheData.AppImagesLarge ?? new Dictionary<string, Image>();
            if (Shareware.Enabled)
            {
                var names = new[]
                {
                    Path.GetFileName(CorePaths.AppImages),
                    Path.GetFileName(CorePaths.AppImagesLarge)
                };
                foreach (var srv in Shareware.GetAddresses())
                {
                    var usr = Shareware.GetUser(srv);
                    var pwd = Shareware.GetPassword(srv);
                    for (var i = 0; i < names.Length; i++)
                    {
                        var name = names[i];
                        var url = PathEx.AltCombine(srv, name);
                        if (Log.DebugMode > 0)
                            Log.Write($"Shareware: Looking for '{{{Shareware.FindAddressKey(srv).Encode()}}}/{name}'.");
                        if (!NetEx.FileIsAvailable(url, usr, pwd, 60000))
                            continue;
                        var swAppImages = NetEx.Transfer.DownloadData(url, usr, pwd)?.DeserializeObject<Dictionary<string, Image>>();
                        if (swAppImages == null)
                            continue;
                        foreach (var pair in swAppImages)
                        {
                            if ((i == 0 ? appImages : appImagesLarge).ContainsKey(pair.Key))
                                continue;
                            (i == 0 ? appImages : appImagesLarge).Add(pair.Key, pair.Value);
                        }
                    }
                }
            }

            appsList.BeginUpdate();
            appsList.Items.Clear();
            foreach (var appData in appInfo ?? CacheData.AppInfo)
            {
                if (!Shareware.Enabled && appData.ServerKey != null)
                    continue;

                var url = appData.DownloadCollection.First().Value.First().Item1;
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                var src = Language.GetText(en_US.HostNotAvailable);
                if (url.StartsWithEx("http"))
                    if (url.ContainsEx(AppSupplierHosts.PortableApps) && url.ContainsEx("/redirect/"))
                        src = AppSupplierHosts.SourceForge;
                    else
                    {
                        src = NetEx.GetShortHost(url);
                        if (string.IsNullOrEmpty(src))
                            continue;
                    }
                else
                {
                    if (appData.ServerKey != null)
                        foreach (var srv in Shareware.GetAddresses())
                        {
                            if (Shareware.FindAddressKey(srv) != appData.ServerKey.Encode(BinaryToTextEncodings.Base85))
                                continue;
                            src = NetEx.GetFullHost(srv);
                            break;
                        }
                }

                var item = new ListViewItem(appData.Name)
                {
                    Name = appData.Key
                };
                item.SubItems.Add(appData.Description);
                item.SubItems.Add(appData.DisplayVersion);
                item.SubItems.Add(appData.DownloadSize.FormatSize(SizeOptions.Trim));
                item.SubItems.Add(appData.InstallSize.FormatSize(SizeOptions.Trim));
                item.SubItems.Add(src);
                item.ImageIndex = index;

                if (appImages?.ContainsKey(appData.Key) == true)
                {
                    var img = appImages[appData.Key];
                    if (img != null)
                        smallImageList.Images.Add(appData.Key, img);
                }

                if (appImagesLarge?.ContainsKey(appData.Key) == true)
                {
                    var img = appImagesLarge[appData.Key];
                    if (img != null)
                        largeImageList.Images.Add(appData.Key, img);
                }

                var smallImageAdded = smallImageList.Images.ContainsKey(appData.Key);
                var largeImageAdded = largeImageList.Images.ContainsKey(appData.Key);
                if (!smallImageAdded || !largeImageAdded)
                {
                    if (Log.DebugMode == 0)
                        continue;
                    appData.Advanced = true;
                    if (!smallImageAdded)
                    {
                        Log.Write($"Cache: Could not find target '{CachePaths.AppImages}:{appData.Key}'.");
                        smallImageList.Images.Add(appData.Key, CacheData.GetSystemImage(ResourcesEx.IconIndex.Stop));
                    }
                    if (!largeImageAdded)
                    {
                        Log.Write($"Cache: Could not find target '{CachePaths.AppImagesLarge}:{appData.Key}'.");
                        largeImageList.Images.Add(appData.Key, CacheData.GetSystemImage(ResourcesEx.IconIndex.Stop, true));
                    }
                }

                if (appData.ServerKey != null)
                {
                    var groupFound = false;

                    Grouping:
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                    {
                        var enName = Language.GetText(nameof(en_US), group.Name);
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
                    var advanced = Language.GetText(nameof(en_US), groups.Last().Name);
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                    {
                        var groupName = Language.GetText(nameof(en_US), group.Name);
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

            appsList.ShowGroups = Settings.Window.ShowGroups;
            appsList.SmallImageList = Settings.Window.LargeImages ? largeImageList : smallImageList;
            appsList.EndUpdate();
            AppsListShowColors();

            AppsListClone.Groups.Clear();
            foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                AppsListClone.Groups.Add(new ListViewGroup
                {
                    Header = group.Header,
                    Name = group.Name
                });
            AppsListClone.Items.Clear();
            foreach (var item in appsList.Items.Cast<ListViewItem>())
                AppsListClone.Items.Add((ListViewItem)item.Clone());
        }

        private void AppMenu_Opening(object sender, CancelEventArgs e)
        {
            if (appsList.SelectedItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            var isChecked = appsList.SelectedItems.Cast<ListViewItem>().FirstOrDefault()?.Checked ?? false;
            appMenuItem1.Text = isChecked ? Language.GetText(nameof(appMenuItem1) + "u") : Language.GetText(nameof(appMenuItem1));
            appMenuItem2.Text = isChecked ? Language.GetText(nameof(appMenuItem2) + "u") : Language.GetText(nameof(appMenuItem2));
        }

        private void AppMenuItem_Click(object sender, EventArgs e)
        {
            if (appsList.SelectedItems.Count == 0)
                return;
            var selectedItem = appsList.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem == default(ListViewItem))
                return;
            var owner = sender as ToolStripMenuItem;
            switch (owner?.Name)
            {
                case nameof(appMenuItem1):
                    selectedItem.Checked = !selectedItem.Checked;
                    break;
                case nameof(appMenuItem2):
                    var isChecked = !selectedItem.Checked;
                    for (var i = 0; i < appsList.Items.Count; i++)
                    {
                        if (Settings.Window.ShowGroups && selectedItem.Group != appsList.Items[i].Group)
                            continue;
                        appsList.Items[i].Checked = isChecked;
                    }
                    break;
                case nameof(appMenuItem3):
                {
                    var appData = CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(selectedItem.Name));
                    var website = appData?.Website;
                    if (website?.StartsWithEx("https://", "http://") != true)
                        return;
                    Process.Start(website);
                    break;
                }
                case nameof(appMenuItem4):
                {
                    var appData = CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(selectedItem.Name));
                    using (var dialog = new AppInfoForm(appData))
                        dialog.ShowDialog();
                    break;
                }
            }
        }

        private void SettingBtn_MouseEnterLeave(object sender, EventArgs e)
        {
            if (!(sender is Panel owner))
                return;
            owner.BackgroundImage = owner.BackgroundImage.SwitchGrayScale(owner);
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            var result = DialogResult.None;
            try
            {
                using (Form dialog = new SettingsForm())
                {
                    dialog.TopMost = true;
                    dialog.Plus();
                    result = dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            if (result != DialogResult.Yes)
                return;
            appsList.SuspendLayout();
            GroupColors["listViewGroup1"] = Settings.Window.Colors.GroupColor1;
            GroupColors["listViewGroup2"] = Settings.Window.Colors.GroupColor2;
            GroupColors["listViewGroup3"] = Settings.Window.Colors.GroupColor3;
            GroupColors["listViewGroup4"] = Settings.Window.Colors.GroupColor4;
            GroupColors["listViewGroup5"] = Settings.Window.Colors.GroupColor5;
            GroupColors["listViewGroup6"] = Settings.Window.Colors.GroupColor6;
            GroupColors["listViewGroup7"] = Settings.Window.Colors.GroupColor7;
            GroupColors["listViewGroup8"] = Settings.Window.Colors.GroupColor8;
            GroupColors["listViewGroup9"] = Settings.Window.Colors.GroupColor9;
            GroupColors["listViewGroup11"] = Settings.Window.Colors.GroupColor11;
            GroupColors["listViewGroup12"] = Settings.Window.Colors.GroupColor12;
            appsList.ShowGroups = Settings.Window.ShowGroups;
            AppsListShowColors();
            appsList.SmallImageList = Settings.Window.LargeImages ? largeImageList : smallImageList;
            appsList.ResumeLayout(true);
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            var owner = sender as TextBox;
            if (string.IsNullOrWhiteSpace(owner?.Text))
                return;
            var tmp = owner.Text;
            owner.Text = string.Empty;
            owner.Text = tmp;
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            SearchReset();
            var owner = sender as TextBox;
            if (string.IsNullOrWhiteSpace(owner?.Text))
                return;
            appsList.SetDoubleBuffer(false);
            try
            {
                foreach (var item in appsList.Items.Cast<ListViewItem>())
                {
                    var description = item.SubItems[1];
                    if (description.Text.ContainsEx(owner.Text))
                    {
                        foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                            if (group.Name.EqualsEx(nameof(en_US.listViewGroup0)))
                            {
                                if (!group.Items.Contains(item))
                                    group.Items.Add(item);
                                if (!item.Selected)
                                    item.EnsureVisible();
                            }
                        continue;
                    }
                    if (!item.Text.ContainsEx(owner.Text))
                        continue;
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                        if (group.Name.EqualsEx(nameof(en_US.listViewGroup0)))
                        {
                            if (!group.Items.Contains(item))
                                group.Items.Add(item);
                            if (!item.Selected)
                                item.EnsureVisible();
                        }
                }
            }
            finally
            {
                appsList.SetDoubleBuffer();
            }
            AppsListShowColors();
            Counter.Reset(0);
            if (!searchResultBlinker.Enabled)
                searchResultBlinker.Enabled = true;
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var separators = Environment.NewLine.ToArray();
            if (!searchBox.Text.ContainsEx(separators))
                return;
            var text = searchBox.Text;
            searchBox.Text = text.RemoveChar(separators);
            searchBox.SelectionStart = searchBox.TextLength;
            searchBox.ScrollToCaret();
        }

        private void SearchResultBlinker_Tick(object sender, EventArgs e)
        {
            lock (SearchHandler)
            {
                if (!(sender is Timer owner))
                    return;
                if (owner.Enabled && Counter.GetValue(0) >= 5)
                    owner.Enabled = false;
                appsList.SetDoubleBuffer(false);
                try
                {
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                    {
                        if (!group.Name.Equals(nameof(en_US.listViewGroup0)))
                            continue;
                        if (group.Items.Count > 0)
                            foreach (var item in appsList.Items.Cast<ListViewItem>())
                            {
                                if (!item.Group.Name.Equals(group.Name))
                                    continue;
                                if (!owner.Enabled || item.BackColor != SystemColors.Highlight)
                                {
                                    item.BackColor = SystemColors.Highlight;
                                    continue;
                                }
                                item.BackColor = appsList.BackColor;
                            }
                        else
                            owner.Enabled = false;
                    }
                    owner.Interval = owner.Interval >= 200 ? 100 : 200;
                }
                finally
                {
                    appsList.SetDoubleBuffer();
                }
                if (owner.Enabled)
                    Counter.Increase(0);
            }
        }

        private void SearchReset()
        {
            if (!Settings.Window.ShowGroups)
                Settings.Window.ShowGroups = true;
            AppsListReset();
            AppsListShowColors(false);
            appsList.Sort();
            foreach (var group in appsList.Groups.Cast<ListViewGroup>())
            {
                if (group.Items.Count == 0)
                    continue;
                foreach (var item in group.Items.Cast<ListViewItem>())
                {
                    item.EnsureVisible();
                    return;
                }
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (!(sender is Button owner))
                return;

            var transferIsBusy = TransferManager.Values.Any(x => x.Transfer.IsBusy);
            if (!owner.Enabled || appsList.Items.Count == 0 || transferIsBusy)
                return;

            SuspendLayout();

            owner.Enabled = false;
            owner.Visible = owner.Enabled;
            cancelBtn.Visible = owner.Enabled;
            searchBox.Parent.Visible = owner.Enabled;
            settingsBtn.Visible = owner.Enabled;
            searchBox.Text = string.Empty;

            appsList.BeginUpdate();
            foreach (var item in appsList.Items.Cast<ListViewItem>())
            {
                if (item.Checked)
                    continue;
                item.Remove();
            }
            appsList.EndUpdate();

            foreach (var filePath in AppSupply.FindAppInstaller())
                FileEx.TryDelete(filePath);

            appsList.HideSelection = !owner.Enabled;
            appsList.Enabled = owner.Enabled;
            appsList.Sort();

            appMenu.Enabled = owner.Enabled;
            settingsBtn.Enabled = owner.Enabled;
            searchBox.Enabled = owner.Enabled;
            cancelBtn.Enabled = owner.Enabled;

            Settings.SkipWriteValue = true;
            Settings.Window.ShowGroups = owner.Enabled;
            appsList.ShowGroups = Settings.Window.ShowGroups;
            Settings.Window.HighlightInstalled = owner.Enabled;
            Settings.Window.ShowGroupColors = owner.Enabled;
            AppsListShowColors();

            TransferManager.Clear();
            var totalDownloadSize = 0L;
            var totalInstallSize = 0L;
            foreach (var item in appsList.CheckedItems.Cast<ListViewItem>())
            {
                var appData = CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(item.Name));
                if (appData == default)
                    continue;

                if (appData.DownloadCollection.Count > 1 && !appData.Settings.ArchiveLangConfirmed)
                    try
                    {
                        var result = DialogResult.None;
                        while (result != DialogResult.OK)
                            using (Form dialog = new LangSelectionForm(appData))
                            {
                                TopMost = false;
                                result = dialog.ShowDialog();
                                if (result == DialogResult.OK)
                                    break;
                                TopMost = true;
                                if (MessageBoxEx.Show(this, Language.GetText(nameof(en_US.AreYouSureMsg)), Settings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                                    continue;
                                TransferManager.Clear();
                                ApplicationExit();
                                return;
                            }
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }

                TransferManager.Add(item, new AppTransferor(appData));

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
                var warning = string.Format(Language.GetText(nameof(en_US.NotEnoughSpaceMsg)), item1, (item2 - item3).FormatSize());
                switch (MessageBoxEx.Show(this, warning, Settings.Title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                {
                    case DialogResult.Abort:
                        TransferManager.Clear();
                        ApplicationExit();
                        return;
                    case DialogResult.Retry:
                        owner.Enabled = !owner.Enabled;

                        owner.Visible = owner.Enabled;
                        cancelBtn.Visible = owner.Enabled;
                        searchBox.Parent.Visible = owner.Enabled;
                        settingsBtn.Visible = owner.Enabled;

                        appsList.HideSelection = !owner.Enabled;
                        appsList.Enabled = owner.Enabled;
                        appMenu.Enabled = owner.Enabled;
                        cancelBtn.Enabled = owner.Enabled;
                        return;
                }
            }

            buttonAreaBorder.Visible = owner.Enabled;
            buttonAreaPanel.Visible = owner.Enabled;
            settingsAreaBorder.Visible = owner.Enabled;
            settingsAreaPanel.Visible = owner.Enabled;
            ResumeLayout();

            Icon = CacheData.GetSystemIcon(ResourcesEx.IconIndex.Network, true);
            downloadStarter.Enabled = !owner.Enabled;
        }

        private void CancelBtn_Click(object sender, EventArgs e) =>
            ApplicationExit();

        private void DownloadStarter_Tick(object sender, EventArgs e)
        {
            if (!(sender is Timer owner))
                return;
            lock (DownloadStarter)
            {
                owner.Enabled = false;

                if (TransferManager.Any(x => x.Value.Transfer.IsBusy))
                    return;

                try
                {
                    CurrentTransfer = TransferManager.First(x => !TransferFails.Contains(x.Value.AppData) && (x.Key.Checked || x.Value.Transfer.HasCanceled));
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    ApplicationExit(1);
                    return;
                }

                var listViewItem = CurrentTransfer.Key;
                appStatus.Text = listViewItem.Text;
                urlStatus.Text = $@"{listViewItem.SubItems[listViewItem.SubItems.Count - 1].Text} ";
                Text = $@"{string.Format(Language.GetText(nameof(en_US.titleStatus)), TransferManager.Keys.Count(x => !x.Checked), TransferManager.Keys.Count)} - {appStatus.Text}";

                if (!TransferStopwatch.IsRunning)
                    TransferStopwatch.Start();

                var appTransferor = CurrentTransfer.Value;
                TransferTask?.Dispose();
                TransferTask = Task.Run(() => appTransferor.StartDownload());

                downloadHandler.Enabled = !owner.Enabled;
            }
        }

        private void DownloadHandler_Tick(object sender, EventArgs e)
        {
            if (!(sender is Timer owner))
                return;
            lock (DownloadHandler)
            {
                var appTransferor = CurrentTransfer.Value;
                DownloadProgressUpdate(appTransferor.Transfer.ProgressPercentage);

                if (!statusAreaBorder.Visible || !statusAreaPanel.Visible)
                {
                    SuspendLayout();
                    statusAreaBorder.Visible = true;
                    statusAreaPanel.Visible = true;
                    ResumeLayout();
                }

                statusAreaLeftPanel.SuspendLayout();
                fileStatus.Text = appTransferor.Transfer.FileName;
                if (string.IsNullOrEmpty(fileStatus.Text))
                    fileStatus.Text = Language.GetText(nameof(en_US.InitStatusText));
                fileStatus.Text = fileStatus.Text.Trim(fileStatus.Font, fileStatus.Width);
                statusAreaLeftPanel.ResumeLayout();

                statusAreaRightPanel.SuspendLayout();
                downloadReceived.Text = appTransferor.Transfer.DataReceived;
                if (downloadReceived.Text.EqualsEx("0.00 bytes / 0.00 bytes"))
                    downloadReceived.Text = Language.GetText(nameof(en_US.InitStatusText));
                downloadSpeed.Text = appTransferor.Transfer.TransferSpeedAd;
                if (downloadSpeed.Text.EqualsEx("0.00 bit/s"))
                    downloadSpeed.Text = Language.GetText(nameof(en_US.InitStatusText));
                timeStatus.Text = TransferStopwatch.Elapsed.ToString("mm\\:ss\\.fff");
                statusAreaRightPanel.ResumeLayout();

                if (TransferTask?.Status == TaskStatus.Running || appTransferor.Transfer.IsBusy)
                {
                    Counter.Reset(1);
                    return;
                }
                if (Counter.Increase(1) < (int)Math.Floor(1000d / owner.Interval))
                    return;

                owner.Enabled = false;
                if (!appTransferor.DownloadStarted)
                {
                    if (!AutoRetry)
                        AutoRetry = appTransferor.AutoRetry;
                    TransferFails.Add(appTransferor.AppData);
                }
                if (appsList.CheckedItems.Count > 0)
                {
                    var listViewItem = CurrentTransfer.Key;
                    listViewItem.Checked = false;
                    if (appsList.CheckedItems.Count > 0)
                    {
                        downloadStarter.Enabled = !owner.Enabled;
                        return;
                    }
                }

                var windowState = WindowState;
                WindowState = FormWindowState.Minimized;

                Text = Settings.Title;
                TransferStopwatch.Stop();
                TaskBarProgress.SetState(Handle, TaskBarProgressFlags.Indeterminate);

                Icon = CacheData.GetSystemIcon(ResourcesEx.IconIndex.Install, true);
                var installed = new List<ListViewItem>();
                installed.AddRange(TransferManager.Where(x => x.Value.StartInstall()).Select(x => x.Key));
                if (installed.Any())
                    foreach (var key in installed)
                        TransferManager.Remove(key);
                if (TransferManager.Any())
                    TransferFails.AddRange(TransferManager.Values.Select(x => x.AppData));

                TransferManager.Clear();
                CurrentTransfer = default;

                Icon = Resources.Logo;
                TopMost = true;
                WindowState = windowState;

                if (TransferFails.Any())
                {
                    TaskBarProgress.SetState(Handle, TaskBarProgressFlags.Error);
                    var fails = TransferFails.Select(x => x.Name).Distinct().ToArray();
                    var warning = string.Format(Language.GetText(fails.Length == 1 ? nameof(en_US.AppDownloadErrorMsg) : nameof(en_US.AppsDownloadErrorMsg)), fails.Join(Environment.NewLine));
                    switch (AutoRetry ? DialogResult.Retry : MessageBoxEx.Show(this, warning, Settings.Title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                    {
                        case DialogResult.Retry:
                            AutoRetry = false;

                            foreach (var appData in TransferFails.Distinct())
                            {
                                var item = appsList.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Name.EqualsEx(appData.Key));
                                if (item == default(ListViewItem))
                                    continue;
                                item.Checked = true;
                            }
                            TransferFails.Clear();

                            SuspendLayout();

                            TopMost = false;
                            appsList.Enabled = true;
                            appsList.HideSelection = !appsList.Enabled;

                            downloadSpeed.Text = string.Empty;
                            DownloadProgressUpdate(0);
                            downloadReceived.Text = string.Empty;

                            settingsBtn.Enabled = appsList.Enabled;
                            searchBox.Enabled = appsList.Enabled;

                            startBtn.Enabled = appsList.Enabled;
                            cancelBtn.Enabled = appsList.Enabled;

                            startBtn.Visible = appsList.Enabled;
                            cancelBtn.Visible = appsList.Enabled;
                            searchBox.Parent.Visible = appsList.Enabled;
                            settingsBtn.Visible = appsList.Enabled;

                            statusAreaBorder.Visible = !appsList.Enabled;
                            statusAreaPanel.Visible = !appsList.Enabled;

                            buttonAreaBorder.Visible = appsList.Enabled;
                            buttonAreaPanel.Visible = appsList.Enabled;
                            settingsAreaBorder.Visible = appsList.Enabled;
                            settingsAreaPanel.Visible = appsList.Enabled;

                            ResumeLayout();

                            StartBtn_Click(startBtn, EventArgs.Empty);
                            return;
                        default:
                            if (ActionGuid.IsUpdateInstance)
                                foreach (var appData in TransferFails)
                                {
                                    appData.Settings.NoUpdates = true;
                                    appData.Settings.NoUpdatesTime = DateTime.Now;
                                }
                            break;
                    }
                }

                TaskBarProgress.SetValue(Handle, 100, 100);
                var information = Language.GetText(ActionGuid.IsUpdateInstance ? installed.Count == 1 ? nameof(en_US.AppUpdatedMsg) : nameof(en_US.AppsUpdatedMsg) : installed.Count == 1 ? nameof(en_US.AppDownloadedMsg) : nameof(en_US.AppsDownloadedMsg));
                MessageBoxEx.Show(this, information, Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ApplicationExit();
            }
        }

        private void DownloadProgressUpdate(int value)
        {
            var color = PanelEx.FakeProgressBar.Update(downloadProgress, value);
            appStatus.ForeColor = color;
            fileStatus.ForeColor = color;
            urlStatus.ForeColor = color;
            downloadReceived.ForeColor = color;
            downloadSpeed.ForeColor = color;
            timeStatus.ForeColor = color;
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

        protected override void WndProc(ref Message m)
        {
            var previous = (int)WindowState;
            base.WndProc(ref m);
            var current = (int)WindowState;
            if (previous == 1 || current == 1 || previous == current)
                return;
            AppsListResizeColumns();
        }
    }
}
