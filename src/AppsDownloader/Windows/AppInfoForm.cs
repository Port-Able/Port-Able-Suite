namespace AppsDownloader.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Libraries;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public partial class AppInfoForm : Form
    {
        public AppInfoForm(AppData appData)
        {
            if (appData == default)
                throw new ArgumentNullException(nameof(appData));

            InitializeComponent();

            SuspendLayout();

            Text = appData.Name;
            var text = appData.ToString(true);
            if (text == default)
                return;

            if (!CacheData.AppImagesLarge.TryGetValue(appData.Key, out var image))
                CacheData.AppImages.TryGetValue(appData.Key, out image);
            if (image != default)
                Icon = image.ToIcon();

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
                        "49:"
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

            ResumeLayout(false);
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void InfoForm_Load(object sender, EventArgs e) =>
            FormEx.Dockable(this);

        private void InfoForm_Shown(object sender, EventArgs e) =>
            infoBox.Enabled = true;

        private void InfoBox_HideCaret(object sender, EventArgs e)
        {
            if (!(sender is RichTextBox owner) || !owner.Enabled || !owner.Visible)
                return;
            WinApi.NativeHelper.HideCaret(owner.Handle);
        }

        private void InfoBox_HideCaret(object sender, MouseEventArgs e) =>
            InfoBox_HideCaret(sender, EventArgs.Empty);
    }
}
