namespace AppsDownloader.Windows
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Libraries;
    using SilDev;
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

            appListGroupBox.ResumeLayout(false);
            appListGroupBox.PerformLayout();
            groupColorsGroupBox.ResumeLayout(false);
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
    }
}
