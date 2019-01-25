namespace AppsLauncher.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using Libraries;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public partial class SettingsForm : Form
    {
        public SettingsForm(LocalAppData appData)
        {
            SelectedAppData = appData;

            InitializeComponent();

            Icon = CacheData.GetSystemIcon(ResourcesEx.IconIndex.SystemControl);

            foreach (TabPage tab in tabCtrl.TabPages)
                tab.BackColor = Settings.Window.Colors.BaseDark;

            locationBtn.BackgroundImage = CacheData.GetSystemImage(ResourcesEx.IconIndex.Directory);
            fileTypes.AutoVerticalScrollBar();
            fileTypes.MaxLength = short.MaxValue;
            fileTypesMenu.EnableAnimation();
            fileTypesMenu.SetFixedSingle();
            associateBtn.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Uac);
            try
            {
                restoreFileTypesBtn.Image = new Bitmap(28, 16);
                using (var g = Graphics.FromImage(restoreFileTypesBtn.Image))
                {
                    g.DrawImage(CacheData.GetSystemImage(ResourcesEx.IconIndex.Uac), 0, 0);
                    g.DrawImage(CacheData.GetSystemImage(ResourcesEx.IconIndex.Undo), 12, 0);
                }
            }
            catch
            {
                restoreFileTypesBtn.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Uac);
                restoreFileTypesBtn.ImageAlign = ContentAlignment.MiddleLeft;
                restoreFileTypesBtn.Text = @"<=";
                if (restoreFileTypesBtn.Image != null)
                    restoreFileTypesBtn.TextAlign = ContentAlignment.MiddleRight;
            }

            previewBg.BackColor = Settings.Window.Colors.BaseDark;
            if (Settings.Window.BackgroundImage != default(Image))
            {
                var width = (int)Math.Round(Settings.Window.BackgroundImage.Width * .65d) + 1;
                var height = (int)Math.Round(Settings.Window.BackgroundImage.Height * .65d) + 1;
                previewBg.BackgroundImage = Settings.Window.BackgroundImage.Redraw(width, height);
                previewBg.BackgroundImageLayout = Settings.Window.BackgroundImageLayout;
                previewLogoBox.BackColor = Color.Transparent;
            }
            previewLogoBox.Image = Resources.Logo128px.Redraw(previewLogoBox.Height, previewLogoBox.Height);
            var exeImage = CacheData.GetSystemImage(ResourcesEx.IconIndex.ExeFile);
            if (exeImage != null)
            {
                previewSmallImgList.Images.Add(exeImage);
                previewSmallImgList.Images.Add(exeImage);
            }
            exeImage = CacheData.GetSystemImage(ResourcesEx.IconIndex.ExeFile, true);
            if (exeImage != null)
            {
                previewLargeImgList.Images.Add(exeImage);
                previewLargeImgList.Images.Add(exeImage);
            }
            previewAppList.StateImageList = Settings.Window.LargeImages ? previewLargeImgList : previewSmallImgList;
            previewAppList.View = Settings.Window.LargeImages ? View.Tile : View.List;

            foreach (var btn in new[] { saveBtn, exitBtn })
            {
                btn.BackColor = Settings.Window.Colors.Button;
                btn.ForeColor = Settings.Window.Colors.ButtonText;
                btn.FlatAppearance.MouseDownBackColor = Settings.Window.Colors.Button;
                btn.FlatAppearance.MouseOverBackColor = Settings.Window.Colors.ButtonHover;
            }

            var comparer = new Comparison.AlphanumericComparer();
            var appNames = CacheData.CurrentAppInfo.Select(x => x.Name).Cast<object>().OrderBy(x => x, comparer).ToArray();
            appsBox.Items.AddRange(appNames);

            appsBox.SelectedItem = SelectedAppData?.Name;
            if (appsBox.SelectedIndex < 0)
                appsBox.SelectedIndex = 0;

            appDirs.AutoVerticalScrollBar();
            addToShellBtn.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Uac);
            rmFromShellBtn.Image = CacheData.GetSystemImage(ResourcesEx.IconIndex.Uac);

            tabCtrl.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            fileTypesTableLayout.ResumeLayout(false);
            fileTypesTableLayout.PerformLayout();
            fileTypesButtonFlowLayout.ResumeLayout(false);
            fileTypesMenu.ResumeLayout(false);
            startArgsTableLayout.ResumeLayout(false);
            startArgsTableLayout.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            previewMainColor.ResumeLayout(false);
            previewBg.ResumeLayout(false);
            ((ISupportInitialize)previewLogoBox).EndInit();
            previewAppListPanel.ResumeLayout(false);
            ((ISupportInitialize)opacityNum).EndInit();
            ((ISupportInitialize)fadeInNum).EndInit();
            tabPage3.ResumeLayout(false);
            topControlTableLayout.ResumeLayout(false);
            topControlTableLayout.PerformLayout();
            buttonFlowLayout.ResumeLayout(false);
            ((ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            exitBtnPanel.ResumeLayout(false);
            saveBtnPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private LocalAppData SelectedAppData { get; set; }

        private DialogResult Result { get; set; }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
            FormEx.Dockable(this);
            WinApi.NativeHelper.MoveWindowToVisibleScreenArea(Handle);
        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            var timer = new Timer(components)
            {
                Interval = 1,
                Enabled = true
            };
            timer.Tick += (o, args) =>
            {
                if (Opacity < 1d)
                {
                    Opacity += .1d;
                    return;
                }
                timer.Dispose();
                if (TopMost)
                    TopMost = false;
                Result = DialogResult.No;
            };
        }

        private void SettingsForm_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is Form owner && owner.Enabled)
                AppsBox_SelectedIndexChanged(appsBox, EventArgs.Empty);
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e) =>
            DialogResult = Result;

        private void LoadSettings()
        {
            if (!setLang.Items.Contains(Settings.Language))
                Settings.Language = "en-US";
            setLang.SelectedItem = Settings.Language;
            Language.UserLang = Settings.Language;

            Language.SetControlLang(this);

            var title = Language.GetText(nameof(en_US.settingsBtn));
            if (!string.IsNullOrWhiteSpace(title))
                Text = title;

            for (var i = 0; i < fileTypesMenu.Items.Count; i++)
                fileTypesMenu.Items[i].Text = Language.GetText(fileTypesMenu.Items[i].Name);

            var decValue = (decimal)Settings.Window.Opacity;
            opacityNum.Value = decValue >= opacityNum.Minimum && decValue <= opacityNum.Maximum ? decValue : .95m;

            var intValue = (int)Settings.Window.FadeInEffect;
            fadeInCombo.SelectedIndex = intValue < fadeInCombo.Items.Count ? intValue : 0;

            intValue = Settings.Window.FadeInDuration;
            fadeInNum.Value = intValue >= fadeInNum.Minimum && intValue <= fadeInNum.Maximum ? intValue : 100;

            defBgCheck.Checked = !File.Exists(CachePaths.CurrentImageBg);
            if (bgLayout.Items.Count > 0)
                bgLayout.Items.Clear();
            for (var i = 0; i < 5; i++)
                bgLayout.Items.Add(Language.GetText($"{bgLayout.Name}Option{i}"));

            intValue = (int)Settings.Window.BackgroundImageLayout;
            bgLayout.SelectedIndex = intValue > 0 && intValue < bgLayout.Items.Count ? intValue : 1;

            mainColorPanel.BackColor = Settings.Window.Colors.Base;
            controlColorPanel.BackColor = Settings.Window.Colors.Control;
            controlTextColorPanel.BackColor = Settings.Window.Colors.ControlText;
            btnColorPanel.BackColor = Settings.Window.Colors.Button;
            btnHoverColorPanel.BackColor = Settings.Window.Colors.ButtonHover;
            btnTextColorPanel.BackColor = Settings.Window.Colors.ButtonText;

            hScrollBarCheck.Checked = Settings.Window.HideHScrollBar;
            showLargeImagesCheck.Checked = Settings.Window.LargeImages;

            StylePreviewUpdate();

            appDirs.Text = Settings.AppDirs?.Where(x => !CorePaths.AppDirs.ContainsEx(x)).Select(x => EnvironmentEx.GetVariablePathFull(x)).Join(Environment.NewLine);

            if (startMenuIntegration.Items.Count > 0)
                startMenuIntegration.Items.Clear();
            for (var i = 0; i < 2; i++)
                startMenuIntegration.Items.Add(Language.GetText($"{startMenuIntegration.Name}Option{i}"));
            startMenuIntegration.SelectedIndex = Settings.StartMenuIntegration ? 1 : 0;

            if (defaultPos.Items.Count > 0)
                defaultPos.Items.Clear();
            for (var i = 0; i < 2; i++)
                defaultPos.Items.Add(Language.GetText($"{defaultPos.Name}Option{i}"));

            intValue = Settings.Window.DefaultPosition;
            defaultPos.SelectedIndex = intValue > 0 && intValue < defaultPos.Items.Count ? intValue : 0;
            if (updateCheck.Items.Count > 0)
                updateCheck.Items.Clear();
            for (var i = 0; i < 10; i++)
                updateCheck.Items.Add(Language.GetText($"{updateCheck.Name}Option{i}"));

            intValue = (int)Settings.UpdateCheck;
            updateCheck.SelectedIndex = intValue > 0 && intValue < updateCheck.Items.Count ? intValue : 0;
            if (updateChannel.Items.Count > 0)
                updateChannel.Items.Clear();
            for (var i = 0; i < 2; i++)
                updateChannel.Items.Add(Language.GetText($"{updateChannel.Name}Option{i}"));

            intValue = (int)Settings.UpdateChannel;
            updateChannel.SelectedIndex = intValue > 0 ? 1 : 0;

            if (!saveBtn.Focused)
                saveBtn.Select();
        }

        private void ToolTipAtMouseEnter(object sender, EventArgs e)
        {
            if (sender is Control owner)
                toolTip.SetToolTip(owner, Language.GetText($"{owner.Name}Tip"));
        }

        private void ExitBtn_Click(object sender, EventArgs e) =>
            Close();

        private void AppsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedApp = (sender as ComboBox)?.SelectedItem?.ToString();
            SelectedAppData = CacheData.FindAppData(selectedApp);
            if (SelectedAppData?.Name?.EqualsEx(selectedApp) != true)
                return;

            fileTypes.Text = SelectedAppData.Settings.FileTypes.Join(',');

            var restPointDir = Path.Combine(CorePaths.RestorePointDir, SelectedAppData.Key);
            restoreFileTypesBtn.Enabled = Directory.Exists(restPointDir) && DirectoryEx.EnumerateFiles(restPointDir, "*.dat", SearchOption.AllDirectories)?.Any() == true;
            restoreFileTypesBtn.Visible = restoreFileTypesBtn.Enabled;

            startArgsFirst.Text = SelectedAppData.Settings.StartArgsFirst;
            startArgsLast.Text = SelectedAppData.Settings.StartArgsLast;

            noConfirmCheck.Checked = SelectedAppData.Settings.NoConfirm;
            runAsAdminCheck.Checked = SelectedAppData.Settings.RunAsAdmin;
            noUpdatesCheck.Checked = SelectedAppData.Settings.NoUpdates;
        }

        private void LocationBtn_Click(object sender, EventArgs e) =>
            SelectedAppData?.OpenLocation();

        private void FileTypesMenu_Click(object sender, EventArgs e)
        {
            switch ((sender as ToolStripMenuItem)?.Name)
            {
                case nameof(fileTypesMenuItem1):
                    if (!string.IsNullOrEmpty(fileTypes.SelectedText))
                        Clipboard.SetText(fileTypes.SelectedText);
                    break;
                case nameof(fileTypesMenuItem2):
                    if (Clipboard.ContainsText())
                        if (string.IsNullOrEmpty(fileTypes.SelectedText))
                        {
                            var start = fileTypes.SelectionStart;
                            fileTypes.Text = fileTypes.Text.Insert(start, Clipboard.GetText());
                            fileTypes.SelectionStart = start + Clipboard.GetText().Length;
                        }
                        else
                            fileTypes.SelectedText = Clipboard.GetText();
                    break;
                case nameof(fileTypesMenuItem3):
                    var selectedItem = appsBox.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(selectedItem))
                    {
                        var appData = CacheData.FindAppData(selectedItem);
                        if (appData != default(LocalAppData))
                        {
                            var iniPath = appData.AppInfoPath;
                            if (!File.Exists(iniPath))
                                iniPath = appData.ConfigPath;
                            if (!File.Exists(iniPath))
                                iniPath = DirectoryEx.EnumerateFiles(appData.FileDir, "*.ini").LastOrDefault();
                            if (File.Exists(iniPath))
                            {
                                var types = Ini.Read("Associations", "FileTypes", iniPath);
                                if (!string.IsNullOrWhiteSpace(types))
                                {
                                    fileTypes.Text = types.RemoveChar(' ');
                                    return;
                                }
                            }
                        }
                    }
                    MessageBoxEx.Show(this, Language.GetText(nameof(en_US.NoDefaultTypesFoundMsg)), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    break;
            }
        }

        private bool FileTypesConflict()
        {
            var appInfo = CacheData.FindAppData(appsBox.SelectedItem.ToString());
            if (!appInfo.Name.EqualsEx(appsBox.SelectedItem.ToString()))
                return false;
            var alreadyDefined = new Dictionary<string, List<string>>();
            foreach (var section in CacheData.CurrentAppSections)
            {
                if (section.EqualsEx(appInfo.Key))
                    continue;
                var types = Ini.Read(section, "FileTypes");
                if (string.IsNullOrWhiteSpace(types))
                    continue;
                var textBoxTypes = fileTypes.Text.RemoveChar('*', '.').Split(',').ToList();
                var configTypes = types.RemoveChar('*', '.').Split(',').ToList();
                foreach (var type in textBoxTypes)
                {
                    if (!configTypes.ContainsEx(type))
                        continue;
                    if (!alreadyDefined.ContainsKey(section))
                    {
                        alreadyDefined.Add(section, new List<string>
                        {
                            type
                        });
                        continue;
                    }
                    if (!alreadyDefined[section].ContainsEx(type))
                        alreadyDefined[section].Add(type);
                }
            }
            if (alreadyDefined.Count <= 0)
                return false;
            var msg = string.Empty;
            var sep = new string('-', 75);
            foreach (var entry in alreadyDefined)
            {
                string appName;
                try
                {
                    appName = CacheData.CurrentAppInfo.First(x => x.Key.EqualsEx(entry.Key)).Name;
                }
                catch
                {
                    Ini.RemoveSection(entry.Key);
                    Ini.WriteAll();
                    continue;
                }
                var types = entry.Value.ToArray().Sort().Join("; ");
                msg = $"{msg}{sep}{Environment.NewLine}{appName}: {types}{Environment.NewLine}";
            }
            if (string.IsNullOrEmpty(msg))
                return false;
            msg += sep;
            return MessageBoxEx.Show(this, string.Format(Language.GetText(nameof(en_US.AssociateConflictMsg)), msg), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes;
        }

        private void AssociateBtn_Click(object sender, EventArgs e)
        {
            if (!(sender is Control owner))
                return;
            var isNull = string.IsNullOrWhiteSpace(fileTypes.Text);
            if (!isNull)
                if (fileTypes.Text.Contains(","))
                    isNull = fileTypes.Text.Split(',').Where(s => !s.StartsWith(".")).ToArray().Length == 0;
                else
                    isNull = fileTypes.Text.StartsWith(".");
            if (isNull)
            {
                MessageBoxEx.Show(this, Language.GetText($"{owner.Name}Msg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (SelectedAppData == default(LocalAppData) || FileTypesConflict())
            {
                MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCanceledMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!fileTypes.Text.EqualsEx(SelectedAppData.Settings.FileTypes.Join(',')))
                SaveBtn_Click(saveBtn, EventArgs.Empty);
            FileTypeAssoc.Associate(SelectedAppData, this);
        }

        private void RestoreFileTypesBtn_Click(object sender, EventArgs e)
        {
            if (SelectedAppData == default(LocalAppData))
            {
                MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCanceledMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            FileTypeAssoc.Restore(SelectedAppData, true, this);
        }

        private void SetBgBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { CheckFileExists = true, CheckPathExists = true, Multiselect = false })
            {
                var path = PathEx.Combine(PathEx.LocalDir, "Assets", "bg");
                if (Directory.Exists(path))
                    dialog.InitialDirectory = path;
                var imageEncoders = ImageCodecInfo.GetImageEncoders();
                var extensions = new List<string>();
                for (var i = 0; i < imageEncoders.Length; i++)
                {
                    extensions.Add(imageEncoders[i].FilenameExtension.ToLower());
                    var description = imageEncoders[i].CodecName.Substring(8).Replace("Codec", "Files").Trim();
                    var pattern = extensions[extensions.Count - 1];
                    dialog.Filter = string.Format("{0}{1}{2} ({3})|{3}", dialog.Filter, i > 0 ? "|" : string.Empty, description, pattern);
                }
                dialog.Filter = string.Format("{0}|Image Files ({1})|{1}", dialog.Filter, extensions.Join(";"));
                dialog.FilterIndex = imageEncoders.Length + 1;
                dialog.ShowDialog();
                if (!File.Exists(dialog.FileName))
                    return;
                try
                {
                    var indicator = Screen.AllScreens.Max(x => Math.Max(x.WorkingArea.Width, x.WorkingArea.Height));
                    var image = Image.FromFile(dialog.FileName).Redraw(SmoothingMode.HighQuality, indicator);
                    if (Math.Max(image.Width, image.Height) > 768)
                        MessageBoxEx.Show(this, Language.GetText(nameof(en_US.BgImageSizeInfoMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CacheData.CurrentImageBg = image;
                    previewBg.BackgroundImage = image.Redraw((int)Math.Round(image.Width * .65f) + 1, (int)Math.Round(image.Height * .65f) + 1);
                    previewLogoBox.BackColor = Color.Transparent;
                    defBgCheck.Checked = false;
                    Result = DialogResult.Yes;
                    MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCompletedMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationFailedMsg)), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DefBgCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox owner))
                return;
            try
            {
                if (!owner.Checked)
                {
                    var bgImg = File.ReadAllBytes(CachePaths.CurrentImageBg).DeserializeObject<Image>();
                    previewBg.BackgroundImage = bgImg.Redraw((int)Math.Round(bgImg.Width * .65f) + 1, (int)Math.Round(bgImg.Height * .65f) + 1);
                }
                else
                    previewBg.BackgroundImage = default(Image);
            }
            catch
            {
                if (!owner.Checked)
                    owner.Checked = true;
            }
        }

        private void BgLayout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Result != DialogResult.Yes)
                Result = DialogResult.Yes;
            StylePreviewUpdate();
        }

        private void ColorPanel_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Panel owner)
                owner.BackColor = Color.FromArgb(128, owner.BackColor.R, owner.BackColor.G, owner.BackColor.B);
        }

        private void ColorPanel_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Panel owner)
                owner.BackColor = Color.FromArgb(owner.BackColor.R, owner.BackColor.G, owner.BackColor.B);
        }

        private void ColorPanel_Click(object sender, EventArgs e)
        {
            if (!(sender is Panel owner))
                return;
            var title = default(string);
            try
            {
                title = Controls.Find(owner.Name + "Label", true).First().Text;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            using (var dialog = new ColorDialogEx(this, title)
            {
                AllowFullOpen = true,
                AnyColor = true,
                SolidColorOnly = true,
                Color = owner.BackColor,
                FullOpen = true
            })
            {
                if (Settings.Window.CustomColors.Any())
                    dialog.CustomColors = Settings.Window.CustomColors;
                if (dialog.ShowDialog() != DialogResult.Cancel)
                {
                    if (dialog.Color != owner.BackColor)
                        owner.BackColor = Color.FromArgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);
                    var colors = dialog.CustomColors ?? Array.Empty<int>();
                    if (colors.SequenceEqual(Settings.Window.CustomColors) == false)
                        Settings.Window.CustomColors = dialog.CustomColors;
                }
            }
            if (Result != DialogResult.Yes)
                Result = DialogResult.Yes;
            StylePreviewUpdate();
        }

        private void ResetColorsBtn_Click(object sender, EventArgs e)
        {
            mainColorPanel.BackColor = Settings.Window.Colors.System;
            previewBg.BackColor = ControlPaint.Dark(Settings.Window.Colors.System, .25f);
            controlColorPanel.BackColor = SystemColors.Window;
            controlTextColorPanel.BackColor = SystemColors.WindowText;
            btnColorPanel.BackColor = SystemColors.ButtonFace;
            btnHoverColorPanel.BackColor = ProfessionalColors.ButtonSelectedHighlight;
            btnTextColorPanel.BackColor = SystemColors.ControlText;
            if (Result != DialogResult.Yes)
                Result = DialogResult.Yes;
            StylePreviewUpdate();
        }

        private void PreviewAppList_Paint(object sender, PaintEventArgs e)
        {
            if (!(sender is Panel owner))
                return;
            using (var g = e.Graphics)
            {
                g.TranslateTransform((int)(owner.Width / (Math.PI * 2)), owner.Width + 40);
                g.RotateTransform(-70);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                using (var b = new SolidBrush(Color.FromArgb(50, (byte)~owner.BackColor.R, (byte)~owner.BackColor.G, (byte)~owner.BackColor.B)))
                    g.DrawString("Preview", new Font("Comic Sans MS", 24f), b, 0f, 0f);
            }
        }

        private void StyleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Result != DialogResult.Yes)
                Result = DialogResult.Yes;
            StylePreviewUpdate();
        }

        private void StylePreviewUpdate()
        {
            previewBg.BackgroundImageLayout = (ImageLayout)bgLayout.SelectedIndex;
            previewMainColor.BackColor = mainColorPanel.BackColor;
            previewAppList.ForeColor = controlTextColorPanel.BackColor;
            previewAppList.BackColor = controlColorPanel.BackColor;
            previewAppList.StateImageList = showLargeImagesCheck.Checked ? previewLargeImgList : previewSmallImgList;
            previewAppList.Scrollable = !showLargeImagesCheck.Checked;
            previewAppList.View = showLargeImagesCheck.Checked ? View.Tile : View.List;
            previewAppListPanel.BackColor = controlColorPanel.BackColor;
            foreach (var b in new[] { previewBtn1, previewBtn2 })
            {
                b.ForeColor = btnTextColorPanel.BackColor;
                b.BackColor = btnColorPanel.BackColor;
                b.FlatAppearance.MouseOverBackColor = btnHoverColorPanel.BackColor;
            }
            previewHScrollBar.Visible = !hScrollBarCheck.Checked;
        }

        private void ShellBtns_Click(object sender, EventArgs e) =>
            SystemIntegration.Enable(sender as Button == addToShellBtn);

        private void ShellBtns_TextChanged(object sender, EventArgs e)
        {
            if (sender is Button owner)
                owner.TextAlign = owner.Text.Length < 0x16 ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleRight;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var appData = CacheData.FindAppData(appsBox.SelectedItem.ToString());
            if (appData != default(LocalAppData))
            {
                if (!string.IsNullOrWhiteSpace(fileTypes.Text))
                    if (e == EventArgs.Empty || !FileTypesConflict())
                    {
                        var typesList = new List<string>();
                        foreach (var item in $"{fileTypes.Text},".Split(','))
                        {
                            if (string.IsNullOrWhiteSpace(item))
                                continue;
                            var type = new string(item.ToCharArray().Where(c => !Path.GetInvalidFileNameChars().Contains(c) && !char.IsWhiteSpace(c)).ToArray());
                            if (string.IsNullOrWhiteSpace(type) || type.Length < 1)
                                continue;
                            if (type.StartsWith("."))
                            {
                                while (type.Contains(".."))
                                    type = type.Replace("..", ".");
                                if (typesList.ContainsEx(type) || typesList.ContainsEx(type.Substring(1)))
                                    continue;
                            }
                            else
                            {
                                if (typesList.ContainsEx(type) || typesList.ContainsEx($".{type}"))
                                    continue;
                            }
                            if (type.Length == 1 && type.StartsWith("."))
                                continue;
                            typesList.Add(type);
                        }
                        if (typesList.Any())
                        {
                            var comparer = new Comparison.AlphanumericComparer();
                            typesList = typesList.OrderBy(x => x, comparer).ToList();
                            fileTypes.Text = typesList.Join(",");
                            appData.Settings.FileTypes = typesList.ToArray();
                        }
                    }
                    else
                        fileTypes.Text = appData.Settings.FileTypes.Join(',');

                appData.Settings.StartArgsFirst = startArgsFirst.Text;
                appData.Settings.StartArgsLast = startArgsLast.Text;

                appData.Settings.NoConfirm = noConfirmCheck.Checked;
                appData.Settings.RunAsAdmin = runAsAdminCheck.Checked;
                appData.Settings.NoUpdates = noUpdatesCheck.Checked;
            }

            if (defBgCheck.Checked)
            {
                if (CacheData.CurrentImageBg != default(Image))
                {
                    CacheData.CurrentImageBg = default(Image);
                    if (Result != DialogResult.Yes)
                        Result = DialogResult.Yes;
                }
                bgLayout.SelectedIndex = 1;
            }

            Settings.Window.FadeInEffect = (Settings.Window.FadeInEffectOptions)fadeInCombo.SelectedIndex;
            Settings.Window.FadeInDuration = (int)fadeInNum.Value;
            Settings.Window.Opacity = (double)opacityNum.Value;

            Settings.Window.BackgroundImageLayout = (ImageLayout)bgLayout.SelectedIndex;

            Settings.Window.Colors.Base = mainColorPanel.BackColor;
            Settings.Window.Colors.Control = controlColorPanel.BackColor;
            Settings.Window.Colors.ControlText = controlTextColorPanel.BackColor;
            Settings.Window.Colors.Button = btnColorPanel.BackColor;
            Settings.Window.Colors.ButtonHover = btnHoverColorPanel.BackColor;
            Settings.Window.Colors.ButtonText = btnTextColorPanel.BackColor;

            Settings.Window.HideHScrollBar = hScrollBarCheck.Checked;
            Settings.Window.LargeImages = showLargeImagesCheck.Checked;

            var dirList = new List<string>();
            if (!string.IsNullOrWhiteSpace(appDirs.Text))
            {
                var tmpDir = appDirs.Text + Environment.NewLine;
                foreach (var item in tmpDir.SplitNewLine())
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;
                    var dir = PathEx.Combine(item);
                    try
                    {
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        dir = EnvironmentEx.GetVariablePathFull(dir);
                        if (!dirList.ContainsEx(dir))
                            dirList.Add(dir);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }
                }
                if (dirList.Count > 0)
                {
                    dirList.Sort();
                    appDirs.Text = dirList.Join(Environment.NewLine);
                }
            }
            Settings.AppDirs = dirList.ToArray();

            Settings.StartMenuIntegration = startMenuIntegration.SelectedIndex > 0;
            if (!Settings.StartMenuIntegration)
                try
                {
                    var shortcutDirs = new[]
                    {
                        Environment.GetFolderPath(Environment.SpecialFolder.SendTo),
                        PathEx.Combine(Environment.SpecialFolder.StartMenu, "Programs")
                    };
                    foreach (var dir in shortcutDirs)
                    {
                        var shortcuts = Directory.GetFiles(dir, "Apps Launcher*.lnk", SearchOption.TopDirectoryOnly);
                        foreach (var shortcut in shortcuts)
                            if (File.Exists(shortcut))
                                File.Delete(shortcut);
                    }
                    var startMenuDir = Path.Combine(shortcutDirs.Last(), "Portable Apps");
                    if (Directory.Exists(startMenuDir))
                        Directory.Delete(startMenuDir, true);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }

            Settings.Window.DefaultPosition = defaultPos.SelectedIndex;

            Settings.UpdateCheck = (Settings.UpdateCheckOptions)updateCheck.SelectedIndex;
            Settings.UpdateChannel = (Settings.UpdateChannelOptions)updateChannel.SelectedIndex;

            var lang = setLang.SelectedItem.ToString();
            if (!Settings.Language.EqualsEx(lang))
            {
                if (Result != DialogResult.Yes)
                    Result = DialogResult.Yes;
                Settings.Language = lang;
                LoadSettings();
            }

            Settings.WriteToFile();

            MessageBoxEx.Show(this, Language.GetText(nameof(en_US.SavedSettings)), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
