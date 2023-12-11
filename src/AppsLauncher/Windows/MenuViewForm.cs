namespace AppsLauncher.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Data;
    using LangResources;
    using Libraries;
    using Microsoft.Win32;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;
    using SilDev.Legacy;
    using static SilDev.WinApi;

    public sealed partial class MenuViewForm : Form
    {
        private bool _isMouseEnter;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                if (!SettingsNew.ShowCaption)
                    cp.Style &= ~0xc00000;
                return cp;
            }
        }

        private Point CursorLocation { get; set; }

        private bool PreventResizeEvents { get; set; }

        private bool PreventClosure { get; set; }

        private string SearchText { get; set; }

        private AppsLauncherSettings SettingsNew { get; } = new();

        public MenuViewForm()
        {
            InitializeComponent();

            if (Desktop.AppsUseDarkTheme)
            {
                Desktop.EnableDarkMode(Handle);
                appMenu.ChangeColorMode();
            }

            SuspendLayout();

            Icon = Resources.PaLogoSymbol;

            BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Back, DefaultBackColor);
            if (SettingsNew.WindowBackground != default)
            {
                BackgroundImage = SettingsNew.WindowBackground;
                BackgroundImageLayout = SettingsNew.WindowBackgroundLayout;
            }
            else
                BackgroundImageLayout = ImageLayout.Stretch;

            if (!EnvironmentEx.IsAtLeastWindows(11))
                ControlEx.DrawBorder(this, BackColor.IsDarkDark() ? BackColor.EnsureLightLight() : BackColor.EnsureDarkDark());

            appsListViewPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Control, DefaultBackColor);
            appsListViewPanel.ForeColor = SettingsNew.WindowColors.TryGetValue(ColorOption.ControlText, DefaultForeColor);
            appsListView.BackColor = appsListViewPanel.BackColor;
            appsListView.ForeColor = appsListViewPanel.ForeColor;
            appsListView.SetMouseOverCursor();

            searchBox.BackColor = appsListViewPanel.BackColor;
            searchBox.ForeColor = appsListViewPanel.ForeColor;
            searchBox.DrawSearchSymbol(appsListViewPanel.ForeColor);
            SearchBox_Leave(searchBox, EventArgs.Empty);

            CacheData.SetComponentImageColor(aboutBtn);
            CacheData.SetComponentImageColor(profileBtn);
            CacheData.SetComponentImageColor(downloadBtn);
            CacheData.SetComponentImageColor(settingsBtn);

            foreach (var item in appMenu.Items.Cast<ToolStripItem>())
                item.Text = Language.GetText(item.Name);
            CacheData.SetComponentImageColor(appMenuItem2, Color.DarkGoldenrod);
            CacheData.SetComponentImageColor(appMenuItem3, true);
            CacheData.SetComponentImageColor(appMenuItem4);
            CacheData.SetComponentImageColor(appMenuItem6);
            CacheData.SetComponentImageColor(appMenuItem7, Color.Firebrick);
            CacheData.SetComponentImageColor(appMenuItem8);

            appMenu.CloseOnMouseLeave(32);
            if (EnvironmentEx.IsAtLeastWindows(11))
                Desktop.RoundCorners(appMenu.Handle, true);
            else
            {
                appMenu.EnableAnimation();
                appMenu.SetFixedSingle();
            }

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            ResumeLayout(false);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData != Keys.LWin && keyData != Keys.RWin)
                return base.ProcessCmdKey(ref msg, keyData);
            Application.Exit();
            return true;
        }

        private void MenuViewForm_Load(object sender, EventArgs e)
        {
            Width = SettingsNew.WindowSize.Width;
            Height = SettingsNew.WindowSize.Height;
            MinimumSize = MinimumSize.ScaleDimensions();
            MaximumSize = SizeEx.GetDesktopSize(Location);
            MenuViewFormPositionUpdate();
            AppsListViewUpdate();
        }

        private void MenuViewForm_Shown(object sender, EventArgs e)
        {
            BackgroundImage ??= SettingsNew.GetDesktopBehind(this);
            MenuViewForm_Resize(this, EventArgs.Empty);
            if (Opacity <= 0d)
            {
                Opacity = 0d;
                var timer = new Timer(components)
                {
                    Interval = 1,
                    Enabled = true
                };
                timer.Tick += LocalFadeInTick;
                return;
            }
            NativeHelper.SetForegroundWindow(Handle);
            return;

            void LocalFadeInTick(object s, EventArgs _)
            {
                if (s is not Timer owner)
                    return;
                if (Opacity < SettingsNew.OpacityLevel)
                {
                    var opacity = SettingsNew.OpacityLevel / 3d + Opacity;
                    if (opacity < (SettingsNew.BlurStrength.IsBetween(1, 99) ? 1d : SettingsNew.OpacityLevel))
                    {
                        Opacity = opacity;
                        return;
                    }
                }
                owner.Enabled = false;
                Opacity = SettingsNew.BlurStrength.IsBetween(1, 99) ? 1f : SettingsNew.OpacityLevel;
                NativeHelper.SetForegroundWindow(Handle);
                owner.Dispose();
            }
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
            if (PreventResizeEvents)
                return;

            if (!appsListView.Scrollable)
                appsListView.Scrollable = true;

            BackgroundImage?.Dispose();
            BackgroundImage = default;

            this.SetChildVisibility(false, appsListViewPanel);
        }

        private void MenuViewForm_ResizeEnd(object sender, EventArgs e)
        {
            if (PreventResizeEvents)
                return;

            if (!appsListView.Focus())
                appsListView.Select();

            if (SettingsNew.WindowBackground == default && (BackgroundImage ??= SettingsNew.GetDesktopBehind(this)) != default)
                BackgroundImageLayout = ImageLayout.Stretch;
            else
                BackgroundImage = SettingsNew.WindowBackground;

            this.SetChildVisibility(true, appsListViewPanel);

            SettingsNew.WindowSize = new(Width, Height);
            MenuViewFormPositionUpdate();
        }

        private void MenuViewForm_Resize(object sender, EventArgs e)
        {
            if (PreventResizeEvents)
                return;
            if (SettingsNew.ShowHorScrollBar)
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
            SettingsNew.SaveToFile();
            if (!SettingsNew.SystemStartMenuIntegration)
                return;
            try
            {
                var appNames = appsListView.Items.OfType<ListViewItem>().Select(x => x.Text).Where(x => !x.EqualsEx("Portable"));
                SystemIntegration.UpdateStartMenuShortcuts(appNames);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
        }

        private void MenuViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Settings.StartUpdateSearch();
        }

        private void MenuViewFormPositionUpdate()
        {
            PreventResizeEvents = true;
            var taskbarAlignment = TaskBar.GetAlignment();
            var taskbarLocation = TaskBar.GetLocation(Handle);
            if (SettingsNew.StartPosition == StartPositionOption.StartMenu && taskbarLocation != TaskBarLocation.Hidden)
            {
                var screen = Screen.PrimaryScreen.WorkingArea;
                foreach (var scr in Screen.AllScreens.Where(s => s.Bounds.Contains(Cursor.Position)))
                {
                    screen = scr.WorkingArea;
                    break;
                }
                if (taskbarAlignment == TaskBarAlignment.Center)
                    Left = screen.Width / 2 - Width / 2;
                switch (taskbarLocation)
                {
                    case TaskBarLocation.Left:
                    case TaskBarLocation.Top:
                        if (taskbarAlignment == TaskBarAlignment.Left)
                            Left = screen.X;
                        Top = screen.Y;
                        break;
                    case TaskBarLocation.Right:
                        if (taskbarAlignment == TaskBarAlignment.Left)
                            Left = screen.Width - Width;
                        Top = screen.Y;
                        break;
                    default:
                        if (taskbarAlignment == TaskBarAlignment.Left)
                            Left = screen.X;
                        Top = screen.Height - Height;
                        break;
                }
            }
            else
            {
                Left = Cursor.Position.X - Width / 2;
                Top = Cursor.Position.Y - Height / 2;
            }
            if (NativeHelper.WindowIsOutOfScreenArea(Handle, out var newRect))
            {
                Left = newRect.X;
                Top = newRect.Y;
            }
            PreventResizeEvents = false;
        }

        private void AppsListViewUpdate()
        {
            appsListView.BeginUpdate();
            try
            {
                appsListView.Items.Clear();
                appsListView.LargeImageList?.Images.Clear();
                appsListView.SmallImageList?.Images.Clear();
                if (!appsListView.Scrollable)
                    appsListView.Scrollable = true;

                var largeImages = SettingsNew.ShowLargeImages;
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
                        image = SettingsNew.ShowLargeImages ? CacheData.AppImagesLarge[key] : CacheData.AppImages[key];
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
                                foreach (var size in sizes)
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
                            if (image != default)
                            {
                                if (image.Width != image.Height || image.Width != indicator)
                                    image = image.Redraw(indicator, indicator);
                                goto UpdateCache;
                            }
                        }
                        if (exePath.EndsWithEx(".bat", ".cmd", ".jse", ".vbe", ".vbs"))
                        {
                            image = CacheData.GetImage(Resources.Terminal, nameof(Resources.Terminal), Settings.Window.Colors.BaseLight, largeImages ? 32 : 16);
                            goto UpdateCache;
                        }
                        FromFile:

                        // ReSharper disable once ConvertToUsingDeclaration
                        using (var ico = ResourcesEx.GetIconFromFile(exePath, 0, largeImages))
                        {
                            image = ico?.ToBitmap()?.Redraw(indicator, indicator);
                            if (image != default)
                                goto UpdateCache;
                        }
                    }
                    catch (Exception ex) when (ex.IsCaught())
                    {
                        Log.Write(ex);
                    }

                    image = CacheData.GetImage(Resources.Application, nameof(Resources.Application), Settings.Window.Colors.BaseLight, largeImages ? 32 : 16);
                    if (image == default)
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
                switch (appsListView.View)
                {
                    case View.LargeIcon:
                    case View.Tile:
                        appsListView.LargeImageList = imageList;
                        break;
                    default:
                        appsListView.SmallImageList = imageList;
                        break;
                }
                CacheData.UpdateCurrentImagesFile();
            }
            finally
            {
                appsListView.EndUpdate();
                Text = string.Format(CultureInfo.InvariantCulture, Language.GetText(Name), appsListView.Items.Count, appsListView.Items.Count == 1 ? Language.GetText(nameof(en_US.App)) : Language.GetText(nameof(en_US.Apps)));
                if (!appsListView.Focus())
                    appsListView.Select();
            }
        }

        private void AppsListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            if (sender is not ListView owner || owner.SelectedItems.Count <= 0)
                return;
            PreventClosure = true;
            if (Opacity > 0)
                Opacity = 0;
            var selectedItem = appsListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem == default)
                return;
            var appData = CacheData.FindAppData(selectedItem.Text);
            if (appData == default)
                return;
            appData.StartApplication(true);
        }

        private void AppsListView_MouseEnter(object sender, EventArgs _)
        {
            if (sender is ListView { LabelEdit: false } owner && !owner.Focus())
                owner.Select();
        }

        private void AppsListView_MouseLeave(object sender, EventArgs _)
        {
            if (sender is ListView { LabelEdit: false } owner && owner.Focus())
                owner.Parent.Select();
        }

        private void AppsListView_MouseMove(object sender, MouseEventArgs _)
        {
            if (sender is not ListView { LabelEdit: false } owner)
                return;
            var ownerItem = owner.ItemFromPoint();
            if (ownerItem == null || CursorLocation == Cursor.Position)
                return;
            ownerItem.Selected = true;
            CursorLocation = Cursor.Position;
        }

        private void AppsListView_KeyDown(object _, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:
                    var isEmpty = searchBox.Text.EqualsEx(Language.GetText(searchBox));
                    if ((isEmpty && SearchText.Length > 0) || (!isEmpty && searchBox.Text.Length > 0))
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
                        var key = Enum.GetName(typeof(Keys), e.KeyCode)?.ToLowerInvariant();
                        searchBox.Text += key?.Last();
                        searchBox.SelectionStart = searchBox.TextLength;
                        searchBox.ScrollToCaret();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void AppsListView_KeyPress(object _, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void AppsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (sender is not ListView owner)
                return;
            try
            {
                if (string.IsNullOrWhiteSpace(e.Label))
                    throw new ArgumentNullException(nameof(e.Label));
                var selectedItem = appsListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
                if (selectedItem == default(ListViewItem))
                    throw new ArgumentNullException(nameof(selectedItem));
                var appData = CacheData.FindAppData(selectedItem.Text);
                if (appData == default)
                    throw new ArgumentNullException(nameof(appData));
                if (appData.Name.Equals(e.Label, StringComparison.Ordinal))
                    throw new ArgumentException();
                if (!File.Exists(appData.ConfigPath))
                    File.Create(appData.ConfigPath).Close();
                Ini.Write("AppInfo", "Name", e.Label, appData.ConfigPath);
                Ini.WriteAll(appData.ConfigPath, true, true);
                CacheData.ResetCurrent();
                AppsListViewUpdate();
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            if (owner.LabelEdit)
                owner.LabelEdit = false;
        }

        private void AppMenuItem_Click(object sender, EventArgs _)
        {
            if (appsListView.SelectedItems.Count == 0)
                return;
            var selectedItem = appsListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem == default)
                return;
            var appData = CacheData.FindAppData(selectedItem.Text);
            if (appData == default)
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
                    MessageBoxEx.Show(this, Language.GetText(created ? nameof(en_US.appMenuItem4Msg0) : nameof(en_US.appMenuItem4Msg1)), Resources.GlobalTitle, MessageBoxButtons.OK, created ? MessageBoxIcon.Asterisk : MessageBoxIcon.Warning);
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
                    if (MessageBoxEx.Show(this, string.Format(CultureInfo.InvariantCulture, Language.GetText(nameof(en_US.appMenuItem7Msg)), selectedItem.Text), Resources.GlobalTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                        PreventClosure = true;
                        if (appData.RemoveApplication(this))
                            AppsListViewUpdate();
                        PreventClosure = false;
                    }
                    else
                    {
                        MessageBoxEx.CenterMousePointer = !ClientRectangle.Contains(PointToClient(MousePosition));
                        MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCanceledMsg)), Resources.GlobalTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    break;
                case nameof(appMenuItem8):
                    OpenForm(new SettingsForm(appData));
                    break;
            }
            if (MessageBoxEx.CenterMousePointer)
                MessageBoxEx.CenterMousePointer = false;
        }

        private void SearchBox_Enter(object sender, EventArgs _)
        {
            CursorLocation = Cursor.Position;
            if (sender is not TextBox owner)
                return;
            owner.Font = new Font("Segoe UI", owner.Font.Size);
            owner.ForeColor = appsListViewPanel.ForeColor;
            owner.Text = SearchText ?? string.Empty;
            appsListView.HideSelection = true;
        }

        private void SearchBox_Leave(object sender, EventArgs _)
        {
            if (sender is not TextBox owner)
                return;
            var c = appsListViewPanel.ForeColor;
            owner.Font = new Font("Comic Sans MS", owner.Font.Size, FontStyle.Italic);
            owner.ForeColor = Color.FromArgb(c.A, c.R / 2, c.G / 2, c.B / 2);
            SearchText = owner.Text;
            owner.Text = Language.GetText(owner);
            appsListView.HideSelection = false;
        }

        private void SearchBox_KeyDown(object _, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                    if (!appsListView.Focus())
                        appsListView.Select();
                    SendKeys.SendWait($"{{{Enum.GetName(typeof(Keys), e.KeyCode)?.ToUpperInvariant()}}}");
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    AppMenuItem_Click(!e.Control && !e.Shift ? appMenuItem1 : appMenuItem2, EventArgs.Empty);
                    e.Handled = true;
                    break;
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs _)
        {
            if (sender is not TextBox owner)
                return;
            appsListView.BeginUpdate();
            try
            {
                var itemList = new List<string>();
                foreach (ListViewItem item in appsListView.Items)
                {
                    item.ForeColor = appsListViewPanel.ForeColor;
                    item.BackColor = appsListViewPanel.BackColor;
                    itemList.Add(item.Text);
                }
                if (string.IsNullOrWhiteSpace(owner.Text) || owner.Font.Italic)
                    return;
                var highlight = SettingsNew.WindowColors.TryGetValue(ColorOption.Highlight, SystemColors.Highlight);
                var highlightText = SettingsNew.WindowColors.TryGetValue(ColorOption.HighlightText, SystemColors.HighlightText);
                foreach (var item in appsListView.Items.Cast<ListViewItem>().Where(i => i.Text.Equals(itemList.SearchItem(owner.Text), StringComparison.Ordinal)))
                {
                    item.BackColor = highlight;
                    item.ForeColor = highlightText;
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
            _isMouseEnter = !_isMouseEnter;
            switch (sender)
            {
                case Button owner:
                {
                    if (owner.BackgroundImage != null)
                        owner.BackgroundImage = owner.BackgroundImage.SwitchGrayScale($"{owner.Name}BackgroundImage");
                    if (owner.Image != null)
                        owner.Image = owner.Image.SwitchGrayScale($"{owner.Name}Image");
                    break;
                }
                case PictureBox owner:
                {
                    if (_isMouseEnter)
                        toolTip.SetToolTip(owner, Language.GetText($"{owner.Name}Tip"));
                    if (owner.BackgroundImage != null)
                        owner.BackgroundImage = owner.BackgroundImage.SwitchGrayScale($"{owner.Name}BackgroundImage");
                    if (owner.Image != null)
                        owner.Image = owner.Image.SwitchGrayScale($"{owner.Name}Image");
                    break;
                }
            }
        }

        private void AboutBtn_Click(object sender, EventArgs e)
        {
            OpenForm(new AboutForm());
            if (NativeHelper.GetForegroundWindow() != Handle)
                NativeHelper.SetForegroundWindow(Handle);
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            if (!OpenForm(new SettingsForm(default)))
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
            //Settings.SkipUpdateSearch = true;
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
                using var dialog = form;
                dialog.TopMost = TopMost;
                dialog.Plus();
                result = dialog.ShowDialog(this) == DialogResult.Yes;
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            PreventClosure = false;
            TopMost = true;
            return result;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 1)
            {
                Application.Restart();
                return;
            }
            MaximumSize = SizeEx.GetDesktopSize(Location);
            var timer = new Timer(components)
            {
                Interval = 1,
                Enabled = true
            };
            timer.Tick += LocalCheckRestartTick;
            return;

            void LocalCheckRestartTick(object s, EventArgs _)
            {
                if (s is not Timer owner || Application.OpenForms.Count > 1)
                    return;
                owner.Enabled = false;
                Application.Restart();
                owner.Dispose();
            }
        }
    }
}
