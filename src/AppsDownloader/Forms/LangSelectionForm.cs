namespace AppsDownloader.Forms
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Media;
    using System.Windows.Forms;
    using PortAble;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public partial class LangSelectionForm : Form
    {
        private readonly AppData _appData;

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public LangSelectionForm(AppData appData, Image appImage)
        {
            _appData = appData;
            if (_appData == default)
                throw new ArgumentNullException(nameof(appData));

            InitializeComponent();

            SuspendLayout();

            if (appImage != default)
                Icon = appImage.ToIcon();

            appNameLabel.Text = _appData.Name;

            var languages = _appData.Languages.ToArray();
            for (var i = 0; i < languages.Length; i++)
            {
                var shortName = languages[i];
                var longName = GetLongName(shortName);
                if (longName.EqualsEx(shortName))
                    continue;
                languages[i] = longName;
            }
            languages = languages.Sort();

            langBox.Items.AddRange(languages.Cast<object>().ToArray());
            langBox.SelectedItem = _appData.Settings.ArchiveLang;
            if (langBox.SelectedIndex < 0)
                langBox.SelectedIndex = 0;

            rememberLangCheck.Checked = _appData.Settings.ArchiveLangConfirmed;

            if (Desktop.AppsUseDarkTheme)
            {
                Desktop.EnableDarkMode(Handle);
                Desktop.EnableDarkMode(rememberLangCheck.Handle);
                Desktop.EnableDarkMode(okBtn.Handle);
                Desktop.EnableDarkMode(cancelBtn.Handle);
                langBox.ChangeColorMode();
            }

            ResumeLayout(false);
        }

        private static string GetShortName(string longName) =>
            longName switch
            {
                "*Default" => "Default",
                "*English" => "English",
                "*Multilingual" => "Multilingual",
                "Chinese (Simplified)" => "SimpChinese",
                "Chinese (Traditional)" => "TradChinese",
                "English (British)" => "EnglishGB",
                "Portuguese (Brazilian)" => "PortugueseBR",
                "Portuguese (Portugal)" => "Portuguese",
                "Serbian (Latin)" => "SerbianLatin",
                "Spanish (International)" => "SpanishInternational",
                "Spanish (Spain)" => "Spanish",
                _ => longName
            };

        private static string GetLongName(string shortName) =>
            shortName switch
            {
                "Default" => "*Default",
                "English" => "*English",
                "Multilingual" => "*Multilingual",
                "SimpChinese" => "Chinese (Simplified)",
                "TradChinese" => "Chinese (Traditional)",
                "EnglishGB" => "English (British)",
                "PortugueseBR" => "Portuguese (Brazilian)",
                "Portuguese" => "Portuguese (Portugal)",
                "SerbianLatin" => "Serbian (Latin)",
                "SpanishInternational" => "Spanish (International)",
                "Spanish" => "Spanish (Spain)",
                _ => shortName
            };

        private static int GetCode(string lang) =>
            lang switch
            {
                "Multilingual" => 1033,
                "Afrikaans" => 1078,
                "Albanian" => 1052,
                "Arabic" => 1025,
                "Armenian" => 1067,
                "Basque" => 1069,
                "Belarusian" => 1059,
                "Bulgarian" => 1026,
                "Catalan" => 1027,
                "Croatian" => 1050,
                "Czech" => 1029,
                "Danish" => 1030,
                "Dutch" => 1043,
                "English" => 1033,
                "EnglishGB" => 2057,
                "Estonian" => 1061,
                "Farsi" => 1065,
                "Filipino" => 1124,
                "Finnish" => 1035,
                "French" => 1036,
                "Galician" => 1110,
                "German" => 1031,
                "Greek" => 1032,
                "Hebrew" => 1037,
                "Hungarian" => 1038,
                "Indonesian" => 1057,
                "Irish" => 6153,
                "Italian" => 1040,
                "Japanese" => 1041,
                "Korean" => 1042,
                "Latvian" => 1062,
                "Lithuanian" => 1063,
                "Luxembourgish" => 1134,
                "Macedonian" => 1071,
                "Malay" => 1086,
                "Norwegian" => 1044,
                "Polish" => 1045,
                "Portuguese" => 2070,
                "PortugueseBR" => 1046,
                "Romanian" => 1048,
                "Russian" => 1049,
                "Serbian" => 3098,
                "SerbianLatin" => 2074,
                "SimpChinese" => 2052,
                "Slovak" => 1051,
                "Slovenian" => 1060,
                "Spanish" => 3082,
                "SpanishInternational" => 2070,
                "Sundanese" => 0,
                "Swedish" => 1053,
                "Thai" => 1054,
                "TradChinese" => 1028,
                "Turkish" => 1055,
                "Ukrainian" => 1058,
                "Vietnamese" => 1066,
                _ => 1033
            };

        private void LangSelectionForm_Load(object sender, EventArgs e) =>
            this.Dockable();

        private void SetArchiveLangForm_Shown(object sender, EventArgs e) =>
            SystemSounds.Asterisk.Play();

        private void OKBtn_Click(object sender, EventArgs e)
        {
            var selected = langBox.GetItemText(langBox.SelectedItem);
            var shortName = GetShortName(selected);
            _appData.Settings.ArchiveLang = shortName;
            _appData.Settings.ArchiveLangCode = GetCode(shortName);
            _appData.Settings.ArchiveLangConfirmed = rememberLangCheck.Checked;
            _appData.Settings.SaveToFile();
            DialogResult = DialogResult.OK;
        }

        private void CancelBtn_Click(object sender, EventArgs e) =>
            DialogResult = DialogResult.Cancel;
    }
}
