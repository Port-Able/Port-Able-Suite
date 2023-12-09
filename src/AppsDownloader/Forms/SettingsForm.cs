namespace AppsDownloader.Forms
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Data;
    using PortAble;
    using PortAble.Properties;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public partial class SettingsForm : Form
    {
        private readonly AppsDownloaderSettings _settings;
        private DialogResult _result = DialogResult.No;

        public sealed override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        public SettingsForm(AppsDownloaderSettings settings)
        {
            InitializeComponent();

            SuspendLayout();

            Icon = Resources.PaLogoGreenSymbol;

            CacheData.SetComponentImageColor(transferPathBtn);
            CacheData.SetComponentImageColor(transferPathUndoBtn);

            if (Desktop.AppsUseDarkTheme)
            {
                logoBox.BackgroundImage = logoBox.BackgroundImage.InvertColors().HueRotate(180);
                BackColor = BackColor.EnsureDarkDarkDark();
                appListGroupBox.ForeColor = appListGroupBox.ForeColor.EnsureLightLightLight();
                groupColorsGroupBox.ForeColor = groupColorsGroupBox.ForeColor.EnsureLightLightLight();
                transferGroupBox.ForeColor = transferGroupBox.ForeColor.EnsureLightLightLight();
                advancedGroupBox.ForeColor = advancedGroupBox.ForeColor.EnsureLightLightLight();
                Desktop.EnableDarkMode(Handle);
            }

            _settings = settings ?? new AppsDownloaderSettings();

            highlightInstalledCheck.Checked = _settings.HighlightInstalled;
            showLargeImagesCheck.Checked = _settings.ShowLargeImages;
            showGroupsCheck.Checked = _settings.ShowGroups;
            showColorsCheck.Checked = _settings.ShowGroupColors;

            SetGroupColors();

            transferPathPanel.Text = _settings.TransferDir;
            if (PathEx.Combine(_settings.TransferDir).EqualsEx(CorePaths.TransferDir))
            {
                transferPathUndoBtn.Enabled = false;
                transferPathUndoBtn.BackgroundImage = transferPathUndoBtn.BackgroundImage.SwitchGrayScale(transferPathUndoBtn);
            }

            ResumeLayout(false);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.Dockable();
            WinApi.NativeHelper.MoveWindowToVisibleScreenArea(Handle);
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settings.HighlightInstalled = highlightInstalledCheck.Checked;
            _settings.ShowLargeImages = showLargeImagesCheck.Checked;
            _settings.ShowGroups = showGroupsCheck.Checked;
            _settings.ShowGroupColors = showColorsCheck.Checked;
            SetGroupColorFromPanel("listViewGroup1", group1ColorPanel);
            SetGroupColorFromPanel("listViewGroup2", group2ColorPanel);
            SetGroupColorFromPanel("listViewGroup3", group3ColorPanel);
            SetGroupColorFromPanel("listViewGroup4", group4ColorPanel);
            SetGroupColorFromPanel("listViewGroup5", group5ColorPanel);
            SetGroupColorFromPanel("listViewGroup6", group6ColorPanel);
            SetGroupColorFromPanel("listViewGroup7", group7ColorPanel);
            SetGroupColorFromPanel("listViewGroup8", group8ColorPanel);
            SetGroupColorFromPanel("listViewGroup9", group9ColorPanel);
            SetGroupColorFromPanel("listViewGroup10", group10ColorPanel);
            SetGroupColorFromPanel("listViewGroup11", group11ColorPanel);
            _settings.SaveToFile();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e) =>
            DialogResult = _result;

        private void CheckBox_Click(object sender, EventArgs e) =>
            _result = DialogResult.Yes;

        private void ColorPanel_MouseEnter(object sender, EventArgs _)
        {
            if (sender is Control owner)
                owner.BackColor = Color.FromArgb(128, owner.BackColor.R, owner.BackColor.G, owner.BackColor.B);
        }

        private void ColorPanel_MouseLeave(object sender, EventArgs _)
        {
            if (sender is Control owner)
                owner.BackColor = Color.FromArgb(owner.BackColor.R, owner.BackColor.G, owner.BackColor.B);
        }

        private void ColorPanel_Click(object sender, EventArgs _)
        {
            if (sender is not Panel owner)
                return;

            _result = DialogResult.Yes;

            var title = default(string);
            try
            {
                title = Controls.Find($"listViewGroup{new string(owner.Name.Where(char.IsDigit).ToArray())}", true).First().Text;
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }

            using var dialog = new ColorDialogEx(this, title);
            dialog.AllowFullOpen = true;
            dialog.AnyColor = true;
            dialog.SolidColorOnly = true;
            dialog.Color = owner.BackColor;
            dialog.FullOpen = true;

            // In case `SystemColors` are used and in order not to
            // lose them, we convert them to 32-bit RGB values.
            var savedOles = _settings.CustomColors
                                     .Select(ColorEx.ToRgb)
                                     .Select(ColorEx.FromRgb)
                                     .Select(ColorTranslator.ToOle)
                                     .ToArray();

            dialog.CustomColors = savedOles;
            if (dialog.ShowDialog() == DialogResult.Cancel)
                return;

            // Alpha to maximum so that the color is completely opaque.
            if (dialog.Color != owner.BackColor)
                owner.BackColor = Color.FromArgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);

            if (dialog.CustomColors is not { } newOles)
                return;

            // Remove padding of default values before saving.
            var filterdOles = newOles.ToList();
            for (var i = filterdOles.Count - 1; i >= 0; i--)
            {
                if (filterdOles[i] == 0xffffff)
                {
                    filterdOles.RemoveAt(i);
                    continue;
                }
                break;
            }

            if (!savedOles.SequenceEqual(newOles))
                _settings.CustomColors = filterdOles.Select(ColorTranslator.FromOle).ToList();
        }

        private void ResetColorsBtn_Click(object sender, EventArgs _)
        {
            if (sender is not Button)
                return;
            _result = DialogResult.Yes;
            _settings.SetDefaultColors();
            SetGroupColors();
        }

        private void TransferPathBtn_Click(object sender, EventArgs _)
        {
            if (sender is not Button)
                return;
            using var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Path.GetTempPath();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                MessageBoxEx.Show(this, LangStrings.OperationCanceledMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            var transferDir = dialog.SelectedPath;
            var transferEnv = EnvironmentEx.GetVariableWithPath(transferDir);
            if (transferEnv.EqualsEx(_settings.TransferDir))
            {
                MessageBoxEx.Show(this, LangStrings.OperationCanceledMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var currentDrive = PathEx.LocalPath.ToUpperInvariant().First();
            var transferDrive = transferDir.ToUpperInvariant().First();
            if (currentDrive.Equals(transferDrive))
            {
                MessageBoxEx.Show(this, LangStrings.TransferDirMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Elevation.WritableLocation(transferDir))
            {
                MessageBoxEx.Show(this, LangStrings.NoWritePermissionsMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _settings.TransferDir = transferEnv;
            transferPathPanel.Text = _settings.TransferDir;
            if (!transferPathUndoBtn.Enabled)
            {
                transferPathUndoBtn.Enabled = true;
                transferPathUndoBtn.BackgroundImage = transferPathUndoBtn.BackgroundImage.SwitchGrayScale(transferPathUndoBtn);
            }
            MessageBoxEx.Show(this, LangStrings.OperationCompletedMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void TransferPathUndoBtn_Click(object sender, EventArgs _)
        {
            if (sender is not Button)
                return;
            _settings.TransferDir = EnvironmentEx.GetVariableWithPath(CorePaths.TransferDir);
            transferPathPanel.Text = _settings.TransferDir;
            if (transferPathUndoBtn.Enabled)
            {
                transferPathUndoBtn.Enabled = false;
                transferPathUndoBtn.BackgroundImage = transferPathUndoBtn.BackgroundImage.SwitchGrayScale(transferPathUndoBtn);
            }
            MessageBoxEx.Show(this, LangStrings.OperationCompletedMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void OpenSrcManBtn_Click(object sender, EventArgs _)
        {
            if (sender is not Button)
                return;

            var result = DialogResult.None;
            try
            {
                using Form dialog = new SourceManagerForm();
                dialog.TopMost = true;
                dialog.Plus();
                result = dialog.ShowDialog(this);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
            if (result != DialogResult.Yes)
                return;
            MessageBoxEx.Show(LangStrings.CustomAppSupplierUpdatedMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void SetGroupColorFromPanel(string key, Control panel) => 
            _settings.GroupColors.TrySet(key, panel.BackColor);

        private void RestorePanelGroupColor(Control panel, string key) => 
            panel.BackColor = _settings.GroupColors.TryGetValue(key, panel.BackColor);

        private void SetGroupColors()
        {
            RestorePanelGroupColor(group1ColorPanel, "listViewGroup1");
            RestorePanelGroupColor(group2ColorPanel, "listViewGroup2");
            RestorePanelGroupColor(group3ColorPanel, "listViewGroup3");
            RestorePanelGroupColor(group4ColorPanel, "listViewGroup4");
            RestorePanelGroupColor(group5ColorPanel, "listViewGroup5");
            RestorePanelGroupColor(group6ColorPanel, "listViewGroup6");
            RestorePanelGroupColor(group7ColorPanel, "listViewGroup7");
            RestorePanelGroupColor(group8ColorPanel, "listViewGroup8");
            RestorePanelGroupColor(group9ColorPanel, "listViewGroup9");
            RestorePanelGroupColor(group10ColorPanel, "listViewGroup10");
            RestorePanelGroupColor(group11ColorPanel, "listViewGroup11");
        }
    }
}
