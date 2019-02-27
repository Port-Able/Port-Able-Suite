namespace AppsDownloader.Windows
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using Libraries;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            Language.SetControlLang(this);
            foreach (var label in groupColorsGroupBox.Controls.OfType<Label>())
            {
                if (string.IsNullOrEmpty(label.Text) || label.Text.EndsWith(":"))
                    continue;
                label.Text += ':';
            }

            Icon = CacheData.GetSystemIcon(ResourcesEx.IconIndex.SystemControl);

            transferPathBox.Text = EnvironmentEx.GetVariablePathFull(Settings.TransferDir, false, false);
            transferPathBtn.BackgroundImage = CacheData.GetSystemImage(ResourcesEx.IconIndex.Directory);
            transferPathUndoBtn.BackgroundImage = CacheData.GetSystemImage(ResourcesEx.IconIndex.Undo);
            if (Settings.TransferDir.EqualsEx(CorePaths.TransferDir))
            {
                transferPathUndoBtn.Enabled = false;
                transferPathUndoBtn.BackgroundImage = transferPathUndoBtn.BackgroundImage.SwitchGrayScale(transferPathUndoBtn);
            }

            appListGroupBox.ResumeLayout(false);
            appListGroupBox.PerformLayout();
            groupColorsGroupBox.ResumeLayout(false);
            ((ISupportInitialize)logoBox).EndInit();
            transferGroupBox.ResumeLayout(false);
            transferGroupBox.PerformLayout();
            advancedGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        private DialogResult Result { get; set; } = DialogResult.No;

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            highlightInstalledCheck.Checked = Settings.Window.HighlightInstalled;
            showLargeImagesCheck.Checked = Settings.Window.LargeImages;
            showGroupsCheck.Checked = Settings.Window.ShowGroups;
            showColorsCheck.Checked = Settings.Window.ShowGroupColors;

            group1ColorPanel.BackColor = Settings.Window.Colors.GroupColor1;
            group2ColorPanel.BackColor = Settings.Window.Colors.GroupColor2;
            group3ColorPanel.BackColor = Settings.Window.Colors.GroupColor3;
            group4ColorPanel.BackColor = Settings.Window.Colors.GroupColor4;
            group5ColorPanel.BackColor = Settings.Window.Colors.GroupColor5;
            group6ColorPanel.BackColor = Settings.Window.Colors.GroupColor6;
            group7ColorPanel.BackColor = Settings.Window.Colors.GroupColor7;
            group8ColorPanel.BackColor = Settings.Window.Colors.GroupColor8;
            group9ColorPanel.BackColor = Settings.Window.Colors.GroupColor9;
            group11ColorPanel.BackColor = Settings.Window.Colors.GroupColor11;
            group12ColorPanel.BackColor = Settings.Window.Colors.GroupColor12;

            FormEx.Dockable(this);
            WinApi.NativeHelper.MoveWindowToVisibleScreenArea(Handle);
        }

        private void SettingsForm_Shown(object sender, EventArgs e) =>
            Opacity = 1d;

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.Window.HighlightInstalled != highlightInstalledCheck.Checked)
                Settings.Window.HighlightInstalled = highlightInstalledCheck.Checked;

            if (Settings.Window.LargeImages != showLargeImagesCheck.Checked)
                Settings.Window.LargeImages = showLargeImagesCheck.Checked;

            if (Settings.Window.ShowGroups != showGroupsCheck.Checked)
                Settings.Window.ShowGroups = showGroupsCheck.Checked;

            if (Settings.Window.ShowGroupColors != showColorsCheck.Checked)
                Settings.Window.ShowGroupColors = showColorsCheck.Checked;

            if (Settings.Window.Colors.GroupColor1 != group1ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor1 = group1ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor2 != group2ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor2 = group2ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor3 != group3ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor3 = group3ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor4 != group4ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor4 = group4ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor5 != group5ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor5 = group5ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor6 != group6ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor6 = group6ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor7 != group7ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor7 = group7ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor8 != group8ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor8 = group8ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor9 != group9ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor9 = group9ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor11 != group11ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor11 = group11ColorPanel.BackColor;

            if (Settings.Window.Colors.GroupColor12 != group12ColorPanel.BackColor)
                Settings.Window.Colors.GroupColor12 = group12ColorPanel.BackColor;
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e) =>
            DialogResult = Result;

        private void CheckBox_Click(object sender, EventArgs e)
        {
            if (Result != DialogResult.Yes)
                Result = DialogResult.Yes;
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
                title = Controls.Find($"listViewGroup{new string(owner.Name.Where(char.IsDigit).ToArray())}", true).First().Text;
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
        }

        private void ResetColorsBtn_Click(object sender, EventArgs e)
        {
            group1ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor1));
            group2ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor2));
            group3ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor3));
            group4ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor4));
            group5ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor5));
            group6ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor6));
            group7ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor7));
            group8ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor8));
            group9ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor9));
            group11ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor11));
            group12ColorPanel.BackColor = Settings.Window.Colors.GetDefColor(nameof(Settings.Window.Colors.GroupColor12));
            if (Result != DialogResult.Yes)
                Result = DialogResult.Yes;
        }

        private void TransferPathBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = Path.GetTempPath();
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCanceledMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                var transferDir = dialog.SelectedPath;
                if (transferDir.EqualsEx(Settings.TransferDir))
                {
                    MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationFailedMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var currentDrive = PathEx.LocalPath.ToUpper().First();
                var transferDrive = transferDir.ToUpper().First();
                if (currentDrive.Equals(transferDrive))
                {
                    MessageBoxEx.Show(this, Language.GetText(nameof(en_US.TransferDirMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var currentDir = Settings.TransferDir;
                Settings.TransferDir = transferDir;
                var dirChanged = !Settings.TransferDir.EqualsEx(currentDir);
                if (dirChanged)
                {
                    transferPathBox.Text = EnvironmentEx.GetVariablePathFull(Settings.TransferDir, false, false);
                    if (!transferPathUndoBtn.Enabled)
                    {
                        transferPathUndoBtn.Enabled = true;
                        transferPathUndoBtn.BackgroundImage = transferPathUndoBtn.BackgroundImage.SwitchGrayScale(transferPathUndoBtn);
                    }
                }
                MessageBoxEx.Show(this, Language.GetText(dirChanged ? nameof(en_US.OperationCompletedMsg) : nameof(en_US.OperationFailedMsg)), Settings.Title, MessageBoxButtons.OK, dirChanged ? MessageBoxIcon.Asterisk : MessageBoxIcon.Warning);
            }
        }

        private void TransferPathUndoBtn_Click(object sender, EventArgs e)
        {
            Settings.TransferDir = CorePaths.TransferDir;
            transferPathBox.Text = EnvironmentEx.GetVariablePathFull(Settings.TransferDir, false, false);
            if (transferPathUndoBtn.Enabled)
            {
                transferPathUndoBtn.Enabled = false;
                transferPathUndoBtn.BackgroundImage = transferPathUndoBtn.BackgroundImage.SwitchGrayScale(transferPathUndoBtn);
            }
            MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCompletedMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void OpenSrcManBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (Form dialog = new SourceManagerForm())
                {
                    dialog.TopMost = true;
                    dialog.Plus();
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
