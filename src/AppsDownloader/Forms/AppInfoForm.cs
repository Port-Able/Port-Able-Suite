namespace AppsDownloader.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using PortAble;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;
    using static SilDev.WinApi;

    public partial class AppInfoForm : Form
    {
        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public AppInfoForm(AppData appData, Image appImage)
        {
            if (appData == default)
                throw new ArgumentNullException(nameof(appData));

            InitializeComponent();

            SuspendLayout();

            Text = appData.Name;
            var text = appData.ToString(true);
            if (text == default)
                return;

            if (appImage != default)
                Icon = appImage.ToIcon();

            if (Desktop.AppsUseDarkTheme)
                this.ChangeColorMode(ControlExColorMode.DarkDarkDark);

            infoBox.AppendText(Environment.NewLine);
            infoBox.AppendText(text);
            infoBox.AppendText(Environment.NewLine);
            infoBox.SelectionStart = 0;

            var colorMap = new Dictionary<Color, string[]>
            {
                {
                    SystemColors.Highlight, new[]
                    {
                        "Key:",
                        "Name:",
                        "Description:",
                        "Category:",
                        "Website:",
                        "DisplayVersion:",
                        "PackageVersion:",
                        "VersionData:",
                        "DefaultLanguage:",
                        "Languages:",
                        "DownloadCollection:",
                        "UpdateCollection:",
                        "DownloadSize:",
                        "InstallSize:",
                        "InstallSize:",
                        "InstallDir:",
                        "Requirements:",
                        "PackageReleaseDate:",
                        "PackageUpdateDate:",
                        "InstallerVersion:",
                        "Advanced:",
                        "ServerKey:",
                        "Item1:",
                        "Item2:",
                        "0:",
                        "1:",
                        "2:",
                        "3:",
                        "4:",
                        "5:",
                        "6:",
                        "7:",
                        "8:",
                        "9:",
                        "10:",
                        "11:",
                        "12:",
                        "13:",
                        "14:",
                        "15:",
                        "16:",
                        "17:",
                        "18:",
                        "19:",
                        "20:",
                        "21:",
                        "22:",
                        "23:",
                        "24:",
                        "25:",
                        "26:",
                        "27:",
                        "28:",
                        "29:",
                        "30:",
                        "31:",
                        "32:",
                        "33:",
                        "34:",
                        "35:",
                        "36:",
                        "37:",
                        "38:",
                        "39:",
                        "40:",
                        "41:",
                        "42:",
                        "43:",
                        "44:",
                        "45:",
                        "46:",
                        "47:",
                        "48:",
                        "49:",
                        "Settings:",
                        "ArchiveLang:",
                        "ArchiveLangCode:",
                        "ArchiveLangConfirmed:",
                        "DisableUpdates:",
                        "DelayUpdates:"
                    }
                },
                {
                    SystemColors.Highlight.InvertRgb(), new[]
                    {
                        "{",
                        "}",
                        ": ",
                        ":\r",
                        ":\n",
                        "'",
                        ","
                    }
                }
            };
            foreach (var color in colorMap)
                foreach (var s in color.Value)
                    infoBox.MarkText(s, color.Key);

            if (Desktop.AppsUseDarkTheme)
            {
                Desktop.EnableDarkMode(Handle);
                Desktop.EnableDarkMode(infoBox.Handle);
            }

            ResumeLayout(false);
        }

        private void InfoForm_Load(object sender, EventArgs e) =>
            this.Dockable();

        private void InfoForm_Shown(object sender, EventArgs e) =>
            infoBox.Enabled = true;

        private void InfoBox_HideCaret(object sender, EventArgs e)
        {
            if (sender is not RichTextBox { Enabled: true, Visible: true } owner)
                return;
            NativeHelper.HideCaret(owner.Handle);
        }

        private void InfoBox_HideCaret(object sender, MouseEventArgs e) =>
            InfoBox_HideCaret(sender, EventArgs.Empty);
    }
}
