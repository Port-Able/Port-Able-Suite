namespace AppsLauncher.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using Libraries;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public sealed partial class MenuViewForm :
#if !DEBUG
        FormEx.BorderlessResizable
#else
        Form
#endif
    {
        public MenuViewForm()
        {
            InitializeComponent();

            Language.SetControlLang(this);
            Text = Settings.Title;

            BackColor = Settings.Window.Colors.BaseDark;
            if (Settings.Window.BackgroundImage != default(Image))
            {
                BackgroundImage = Settings.Window.BackgroundImage;
                BackgroundImageLayout = Settings.Window.BackgroundImageLayout;
            }
            Icon = Resources.Logo;

            ControlEx.DrawSizeGrip(this, Settings.Window.Colors.Base);
            ControlEx.DrawBorder(this, Settings.Window.Colors.Base);

            appsListViewPanel.BackColor = Settings.Window.Colors.Control;
            appsListViewPanel.ForeColor = Settings.Window.Colors.ControlText;
            appsListView.BackColor = Settings.Window.Colors.Control;
            appsListView.ForeColor = Settings.Window.Colors.ControlText;
            appsListView.SetDoubleBuffer();
            appsListView.SetMouseOverCursor();

            searchBox.BackColor = Settings.Window.Colors.Control;
            searchBox.ForeColor = Settings.Window.Colors.ControlText;
            searchBox.DrawSearchSymbol(Settings.Window.Colors.ControlText);
            SearchBox_Leave(searchBox, EventArgs.Empty);

            title.ForeColor = Settings.Window.Colors.BaseText;
            logoBox.Image = Resources.Logo64px;
            appsCount.ForeColor = Settings.Window.Colors.BaseText;

            aboutBtn.BackgroundImage = CacheData.GetSystemImage(ResourcesEx.IconIndex.Help);
            aboutBtn.BackgroundImage = aboutBtn.BackgroundImage.SwitchGrayScale($"{aboutBtn.Name}BackgroundImage");

            profileBtn.BackgroundImage = CacheData.GetSystemImage(ResourcesEx.IconIndex.UserDir, true);

            downloadBtn.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Network);
            settingsBtn.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.SystemControl);
            foreach (var btn in new[] { downloadBtn, settingsBtn })
            {
                btn.BackColor = Settings.Window.Colors.Button;
                btn.ForeColor = Settings.Window.Colors.ButtonText;
                btn.FlatAppearance.MouseDownBackColor = Settings.Window.Colors.Button;
                btn.FlatAppearance.MouseOverBackColor = Settings.Window.Colors.ButtonHover;
            }

            appMenuItem2.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Uac);
            appMenuItem3.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Directory);
            appMenuItem5.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Pin);
            appMenuItem7.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.RecycleBinEmpty);
            appMenuItem8.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.SystemControl);
            for (var i = 0; i < appMenu.Items.Count; i++)
                appMenu.Items[i].Text = Language.GetText(appMenu.Items[i].Name);
            appMenu.CloseOnMouseLeave(32);
            appMenu.EnableAnimation();
            appMenu.SetFixedSingle();

#if !DEBUG
            SetResizingBorders(TaskBar.GetLocation(Handle));
#endif

            appMenu.ResumeLayout(false);
            appsListViewPanel.ResumeLayout(false);
            ((ISupportInitialize)logoBox).EndInit();
            ((ISupportInitialize)aboutBtn).EndInit();
            ((ISupportInitialize)profileBtn).EndInit();
            downloadBtnPanel.ResumeLayout(false);
            settingsBtnPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private Point CursorLocation { get; set; }

        private bool PreventClosure { get; set; }

        private string SearchText { get; set; }

        private void MenuViewForm_Load(object sender, EventArgs e)
        {
            MinimumSize = Settings.Window.Size.Minimum;
            MaximumSize = Settings.Window.Size.Maximum;
            if (Settings.Window.Size.Width > Settings.Window.Size.Minimum.Width)
                Width = Settings.Window.Size.Width;
            if (Settings.Window.Size.Height > Settings.Window.Size.Minimum.Height)
                Height = Settings.Window.Size.Height;
            MenuViewFormUpdate();
            if (Settings.Window.Animation == WinApi.AnimateWindowFlags.Blend)
                return;
            Opacity = Settings.Window.Opacity;
            WinApi.NativeHelper.AnimateWindow(Handle, Settings.Window.FadeInDuration, Settings.Window.Animation);
        }

        private void MenuViewForm_Shown(object sender, EventArgs e)
        {
            MenuViewForm_Resize(this, EventArgs.Empty);
            if (Opacity <= 0d)
            {
                Opacity = 0d;
                var timer = new Timer(components)
                {
                    Interval = 1,
                    Enabled = true
                };
                timer.Tick += (o, args) =>
                {
                    if (Opacity < Settings.Window.Opacity)
                    {
                        var opacity = Settings.Window.Opacity / (Settings.Window.FadeInDuration / 10d) + Opacity;
                        if (opacity <= Settings.Window.Opacity)
                        {
                            Opacity = opacity;
                            return;
                        }
                    }
                    Opacity = Settings.Window.Opacity;
                    if (WinApi.NativeHelper.GetForegroundWindow() != Handle)
                        WinApi.NativeHelper.SetForegroundWindow(Handle);
                    timer.Dispose();
                };
                return;
            }
            if (WinApi.NativeHelper.GetForegroundWindow() != Handle)
                WinApi.NativeHelper.SetForegroundWindow(Handle);
        }

        private void MenuViewForm_Deactivate(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count > 1 || appMenu.Focus() || PreventClosure)
                return;
            if (!ClientRectangle.Contains(PointToClient(MousePosition)))
                Close();
        }

        private void MenuViewForm_ResizeBegin(object sender, EventArgs e)
        {
            if (!appsListView.Scrollable)
                appsListView.Scrollable = true;
            BackColor = Settings.Window.Colors.Base;
            if (BackgroundImage != default(Image))
                BackgroundImage = default(Image);
            this.SetChildVisibility(false, appsListViewPanel);
        }

        private void MenuViewForm_ResizeEnd(object sender, EventArgs e)
        {
            if (!appsListView.Focus())
                appsListView.Select();
            BackColor = Settings.Window.Colors.BaseDark;
            if (Settings.Window.BackgroundImage != default(Image))
                BackgroundImage = Settings.Window.BackgroundImage;
            this.SetChildVisibility(true, appsListViewPanel);
            Settings.Window.Size.Width = Width;
            Settings.Window.Size.Height = Height;
        }

        private void MenuViewForm_Resize(object sender, EventArgs e)
        {
            if (!Settings.Window.HideHScrollBar)
            {
                Refresh();
                return;
            }
            if (appsListView.Dock != DockStyle.None)
                appsListView.Dock = DockStyle.None;
            var padding = (int)Math.Floor(SystemInformation.HorizontalScrollBarHeight / 3d);
            appsListView.Location = new Point(padding + 3, padding + 2);
            appsListView.Size = appsListViewPanel.Size;
            appsListView.Region = new Region(new RectangleF(0, 0, appsListViewPanel.Width - padding - 3, appsListViewPanel.Height - SystemInformation.HorizontalScrollBarHeight - 2));
            Refresh();
        }

        private void MenuViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PreventClosure = true;
            if (Opacity > 0)
                Opacity = 0;
            if (!Settings.StartMenuIntegration)
                return;
            try
            {
                var appNames = appsListView.Items.OfType<ListViewItem>().Select(x => x.Text).Where(x => !x.EqualsEx("Portable"));
                SystemIntegration.UpdateStartMenuShortcuts(appNames);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void MenuViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.WriteToFile();
            Settings.StartUpdateSearch();
        }

        private void MenuViewFormUpdate(bool setWindowLocation = true)
        {
            appsListView.BeginUpdate();
            try
            {
                appsListView.Items.Clear();
                if (appsListView.SmallImageList != default(ImageList))
                    appsListView.SmallImageList.Images.Clear();
                if (!appsListView.Scrollable)
                    appsListView.Scrollable = true;

                var largeImages = Settings.Window.LargeImages;
                imageList.Images.Clear();
                foreach (var appData in CacheData.CurrentAppInfo)
                {
                    var key = appData.Key;
                    if (string.IsNullOrWhiteSpace(key))
                        continue;

                    var name = appData.Name;
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    Image image;
                    if (CacheData.CurrentImages.ContainsKey(key))
                    {
                        image = CacheData.CurrentImages[key];
                        goto Finalize;
                    }
                    if (CacheData.AppImages.ContainsKey(key))
                    {
                        image = CacheData.AppImages[key];
                        goto UpdateCache;
                    }

                    var exePath = appData.FilePath;
                    if (!File.Exists(exePath))
                        continue;
                    try
                    {
                        var imgPath = Path.ChangeExtension(exePath, ".ico");
                        var indicator = largeImages ? 32 : 16;
                        if (File.Exists(imgPath))
                        {
                            exePath = imgPath;
                            goto FromFile;
                        }
                        imgPath = Path.ChangeExtension(exePath, ".png");
                        if (!File.Exists(imgPath))
                        {
                            var appDir = Path.GetDirectoryName(exePath);
                            var imgDir = Path.Combine(appDir, "App\\AppInfo");
                            if (largeImages)
                            {
                                var sizes = new[] { 32, 40, 48, 64, 96, 128, 256 };
                                foreach(var size in sizes)
                                {
                                    imgPath = Path.Combine(imgDir, $"appicon_{size}.png");
                                    if (File.Exists(imgPath))
                                        break;
                                }
                            }
                            else
                                imgPath = Path.Combine(imgDir, "appicon_16.png");
                        }
                        if (File.Exists(imgPath))
                        {
                            image = Image.FromFile(imgPath);
                            if (image != default(Image))
                            {
                                if (image.Width != image.Height || image.Width != indicator)
                                    image = image.Redraw(indicator, indicator);
                                goto UpdateCache;
                            }
                        }
                        if (exePath.EndsWithEx(".bat", ".cmd", ".jse", ".vbe", ".vbs"))
                        {
                            image = CacheData.GetSystemImage(ResourcesEx.IconIndex.CommandPrompt, largeImages);
                            goto UpdateCache;
                        }
                        FromFile:
                        using (var ico = ResourcesEx.GetIconFromFile(exePath, 0, largeImages))
                        {
                            image = ico?.ToBitmap()?.Redraw(indicator, indicator);
                            if (image != default(Image))
                                goto UpdateCache;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }

                    image = CacheData.GetSystemImage(ResourcesEx.IconIndex.ExeFile, largeImages);
                    if (image == default(Image))
                        continue;

                    UpdateCache:
                    if (!CacheData.CurrentImages.ContainsKey(key))
                        CacheData.CurrentImages.Add(key, image);

                    Finalize:
                    imageList.Images.Add(image);
                    appsListView.Items.Add(name, imageList.Images.Count - 1);
                }

                if (largeImages)
                    imageList.ImageSize = new Size(32, 32);
                appsListView.SmallImageList = imageList;
                CacheData.UpdateCurrentImagesFile();

                if (!setWindowLocation)
                    return;
                var taskbarLocation = TaskBar.GetLocation(Handle);
                if (Settings.Window.DefaultPosition == 0 && taskbarLocation != TaskBarLocation.Hidden)
                {
                    var screen = Screen.PrimaryScreen.WorkingArea;
                    foreach (var scr in Screen.AllScreens)
                    {
                        if (!scr.Bounds.Contains(Cursor.Position))
                            continue;
                        screen = scr.WorkingArea;
                        break;
                    }
                    switch (taskbarLocation)
                    {
                        case TaskBarLocation.Left:
                        case TaskBarLocation.Top:
                            Left = screen.X;
                            Top = screen.Y;
                            break;
                        case TaskBarLocation.Right:
                            Left = screen.Width - Width;
                            Top = screen.Y;
                            break;
                        default:
                            Left = screen.X;
                            Top = screen.Height - Height;
                            break;
                    }
                }
                else
                {
                    Left = Cursor.Position.X - Width / 2;
                    Top = Cursor.Position.Y - Height / 2;
                    if (!WinApi.NativeHelper.WindowIsOutOfScreenArea(Handle, out var newRect))
                        return;
                    Left = newRect.X;
                    Top = newRect.Y;
                }
            }
            finally
            {
                appsListView.EndUpdate();
                appsCount.Text = string.Format(Language.GetText(appsCount), appsListView.Items.Count, appsListView.Items.Count == 1 ? Language.GetText(nameof(en_US.App)) : Language.GetText(nameof(en_US.Apps)));
                if (!appsListView.Focus())
                    appsListView.Select();
            }
        }

        private void AppsListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            if (!(sender is ListView owner) || owner.SelectedItems.Count <= 0)
                return;
            PreventClosure = true;
            if (Opacity > 0)
                Opacity = 0;
            var selectedItem = appsListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem == default(ListViewItem))
                return;
            var appData = CacheData.FindAppData(selectedItem.Text);
            if (appData == default(LocalAppData))
                return;
            appData.StartApplication(true);
        }

        private void AppsListView_MouseEnter(object sender, EventArgs e)
        {
            if (sender is ListView owner && !owner.LabelEdit && !owner.Focus())
                owner.Select();
        }

        private void AppsListView_MouseLeave(object sender, EventArgs e)
        {
            if (sender is ListView owner && !owner.LabelEdit && owner.Focus())
                owner.Parent.Select();
        }

        private void AppsListView_MouseMove(object sender, MouseEventArgs e)
        {
            var owner = sender as ListView;
            if (owner?.LabelEdit == true)
                return;
            var ownerItem = owner.ItemFromPoint();
            if (ownerItem == null || CursorLocation == Cursor.Position)
                return;
            ownerItem.Selected = true;
            CursorLocation = Cursor.Position;
        }

        private void AppsListView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:
                    var isEmpty = searchBox.Text.EqualsEx(Language.GetText(searchBox));
                    if (isEmpty && SearchText.Length > 0 || !isEmpty && searchBox.Text.Length > 0)
                    {
                        if (!searchBox.Focus())
                            searchBox.Select();
                        searchBox.Text = searchBox.Text.Substring(0, searchBox.Text.Length - 1);
                        searchBox.SelectionStart = searchBox.TextLength;
                        searchBox.ScrollToCaret();
                        e.Handled = true;
                    }
                    break;
                case Keys.ControlKey:
                    if (!e.Shift)
                    {
                        if (!searchBox.Focus())
                            searchBox.Select();
                        e.Handled = true;
                    }
                    break;
                case Keys.Delete:
                    AppMenuItem_Click(appMenuItem7, EventArgs.Empty);
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    AppMenuItem_Click(!e.Control && !e.Shift ? appMenuItem1 : appMenuItem2, EventArgs.Empty);
                    e.Handled = true;
                    break;
                case Keys.F2:
                    AppMenuItem_Click(appMenuItem6, EventArgs.Empty);
                    e.Handled = true;
                    break;
                case Keys.Space:
                    if (!searchBox.Focus())
                        searchBox.Select();
                    searchBox.Text += @" ";
                    searchBox.SelectionStart = searchBox.TextLength;
                    searchBox.ScrollToCaret();
                    e.Handled = true;
                    break;
                default:
                    if (char.IsLetterOrDigit((char)e.KeyCode))
                    {
                        if (!searchBox.Focus())
                            searchBox.Select();
                        var key = Enum.GetName(typeof(Keys), e.KeyCode)?.ToLower();
                        searchBox.Text += key?.Last();
                        searchBox.SelectionStart = searchBox.TextLength;
                        searchBox.ScrollToCaret();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void AppsListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void AppsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (!(sender is ListView owner))
                return;
            try
            {
                if (string.IsNullOrWhiteSpace(e.Label))
                    throw new ArgumentNullException(nameof(e.Label));
                var selectedItem = appsListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
                if (selectedItem == default(ListViewItem))
                    throw new ArgumentNullException(nameof(selectedItem));
                var appData = CacheData.FindAppData(selectedItem.Text);
                if (appData == default(LocalAppData))
                    throw new ArgumentNullException(nameof(appData));
                if (appData.Name.Equals(e.Label))
                    throw new ArgumentException();
                if (!File.Exists(appData.ConfigPath))
                    File.Create(appData.ConfigPath).Close();
                Ini.Write("AppInfo", "Name", e.Label, appData.ConfigPath);
                Ini.WriteAll(appData.ConfigPath, true, true);
                CacheData.ResetCurrent();
                MenuViewFormUpdate(false);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            if (owner.LabelEdit)
                owner.LabelEdit = false;
        }

        private void AppMenu_Opening(object sender, CancelEventArgs e) =>
            e.Cancel = appsListView.SelectedItems.Count == 0;

        private void AppMenuItem_Click(object sender, EventArgs e)
        {
            if (appsListView.SelectedItems.Count == 0)
                return;
            var selectedItem = appsListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem == default(ListViewItem))
                return;
            var appData = CacheData.FindAppData(selectedItem.Text);
            if (appData == default(LocalAppData))
                return;
            var owner = sender as ToolStripMenuItem;
            switch (owner?.Name)
            {
                case nameof(appMenuItem1):
                case nameof(appMenuItem2):
                case nameof(appMenuItem3):
                    if (Opacity > 0)
                        Opacity = 0;
                    switch (owner.Name)
                    {
                        case nameof(appMenuItem1):
                        case nameof(appMenuItem2):
                            PreventClosure = true;
                            appData.StartApplication(true, owner.Name.EqualsEx(nameof(appMenuItem2)));
                            break;
                        case nameof(appMenuItem3):
                            PreventClosure = true;
                            appData.OpenLocation(true);
                            break;
                    }
                    break;
                case nameof(appMenuItem4):
                {
                    var linkPath = PathEx.Combine(Environment.SpecialFolder.Desktop, selectedItem.Text);
                    var created = FileEx.CreateShellLink(appData.FilePath, linkPath);
                    MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                    MessageBoxEx.Show(this, Language.GetText(created ? nameof(en_US.appMenuItem4Msg0) : nameof(en_US.appMenuItem4Msg1)), Settings.Title, MessageBoxButtons.OK, created ? MessageBoxIcon.Asterisk : MessageBoxIcon.Warning);
                    break;
                }
                case nameof(appMenuItem5):
                {
                    var pinned = TaskBar.Pin(appData.FilePath);
                    MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                    MessageBoxEx.Show(this, Language.GetText(pinned ? nameof(en_US.appMenuItem4Msg0) : nameof(en_US.appMenuItem4Msg1)), Settings.Title, MessageBoxButtons.OK, pinned ? MessageBoxIcon.Asterisk : MessageBoxIcon.Warning);
                    break;
                }
                case nameof(appMenuItem6):
                    if (appsListView.SelectedItems.Count > 0)
                    {
                        if (!appsListView.LabelEdit)
                            appsListView.LabelEdit = true;
                        selectedItem.BeginEdit();
                    }
                    break;
                case nameof(appMenuItem7):
                    MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                    if (MessageBoxEx.Show(this, string.Format(Language.GetText(nameof(en_US.appMenuItem7Msg)), selectedItem.Text), Settings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                        PreventClosure = true;
                        if (appData.RemoveApplication(this))
                            MenuViewFormUpdate(false);
                        PreventClosure = false;
                    }
                    else
                    {
                        MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                        MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCanceledMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    break;
                case nameof(appMenuItem8):
                    OpenForm(new SettingsForm(appData));
                    break;
            }
            if (MessageBoxEx.CenterMousePointer)
                MessageBoxEx.CenterMousePointer = false;
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            CursorLocation = Cursor.Position;
            if (!(sender is TextBox owner))
                return;
            owner.Font = new Font("Segoe UI", owner.Font.Size);
            owner.ForeColor = Settings.Window.Colors.ControlText;
            owner.Text = SearchText ?? string.Empty;
            appsListView.HideSelection = true;
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
            appsListView.HideSelection = false;
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                    if (!appsListView.Focus())
                        appsListView.Select();
                    SendKeys.SendWait($"{{{Enum.GetName(typeof(Keys), e.KeyCode)?.ToUpper()}}}");
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    AppMenuItem_Click(!e.Control && !e.Shift ? appMenuItem1 : appMenuItem2, EventArgs.Empty);
                    e.Handled = true;
                    break;
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox owner))
                return;
            appsListView.BeginUpdate();
            try
            {
                var itemList = new List<string>();
                foreach (ListViewItem item in appsListView.Items)
                {
                    item.ForeColor = Settings.Window.Colors.ControlText;
                    item.BackColor = Settings.Window.Colors.Control;
                    itemList.Add(item.Text);
                }
                if (string.IsNullOrWhiteSpace(owner.Text) || owner.Font.Italic)
                    return;
                foreach (ListViewItem item in appsListView.Items)
                    if (item.Text.Equals(itemList.SearchItem(owner.Text)))
                    {
                        item.ForeColor = Settings.Window.Colors.HighlightText;
                        item.BackColor = Settings.Window.Colors.Highlight;
                        item.Selected = true;
                        item.Focused = true;
                        item.EnsureVisible();
                        break;
                    }
                CursorLocation = Cursor.Position;
            }
            finally
            {
                appsListView.EndUpdate();
            }
        }

        private void ImageButton_MouseEnterLeave(object sender, EventArgs e)
        {
            var owner1 = sender as Button;
            if (owner1?.BackgroundImage != null)
                owner1.BackgroundImage = owner1.BackgroundImage.SwitchGrayScale($"{owner1.Name}BackgroundImage");
            if (owner1?.Image != null)
                owner1.Image = owner1.Image.SwitchGrayScale($"{owner1.Name}Image");
            if (owner1 != null)
                return;
            var owner2 = sender as PictureBox;
            if (owner2?.BackgroundImage != null)
                owner2.BackgroundImage = owner2.BackgroundImage.SwitchGrayScale($"{owner2.Name}BackgroundImage");
            if (owner2?.Image != null)
                owner2.Image = owner2.Image.SwitchGrayScale($"{owner2.Name}Image");
        }

        private void AboutBtn_Click(object sender, EventArgs e)
        {
            OpenForm(new AboutForm());
            if (WinApi.NativeHelper.GetForegroundWindow() != Handle)
                WinApi.NativeHelper.SetForegroundWindow(Handle);
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            if (!OpenForm(new SettingsForm(default(LocalAppData))))
                return;
            ProcessEx.Start(PathEx.LocalPath, ActionGuid.AllowNewInstance);
            Application.Exit();
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            ProcessEx.Start(CorePaths.SystemExplorer, CorePaths.UserDirs.FirstOrDefault());
            Application.Exit();
        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            Settings.SkipUpdateSearch = true;
            ProcessEx.Start(CorePaths.AppsDownloader);
            Application.Exit();
        }

        private bool OpenForm(Form form)
        {
            PreventClosure = true;
            TopMost = false;
            var result = false;
            try
            {
                using (var dialog = form)
                {
                    dialog.TopMost = true;
                    dialog.Plus();
                    result = dialog.ShowDialog() == DialogResult.Yes;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            PreventClosure = false;
            TopMost = true;
            return result;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData != Keys.LWin && keyData != Keys.RWin)
                return base.ProcessCmdKey(ref msg, keyData);
            Application.Exit();
            return true;
        }
    }
}
