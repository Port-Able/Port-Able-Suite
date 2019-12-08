namespace AppsDownloader.Windows
{
    using System;
    using System.Linq;
    using System.Media;
    using System.Windows.Forms;
    using Libraries;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public partial class LangSelectionForm : Form
    {
        private readonly AppData _appData;

        public LangSelectionForm(AppData appData)
        {
            _appData = appData;
            if (_appData == default)
                throw new ArgumentNullException(nameof(appData));

            InitializeComponent();

            SuspendLayout();

            Text = Language.GetText(Name);

            if (!CacheData.AppImagesLarge.TryGetValue(appData.Key, out var image))
                CacheData.AppImages.TryGetValue(appData.Key, out image);
            if (image != default)
                Icon = image.ToIcon();

            appNameLabel.Text = _appData.Name;

            LangNames = Resources.LangNames;
            var languages = _appData.Languages.ToArray();
            for (var i = 0; i < languages.Length; i++)
            {
                var shortName = languages[i];
                var longName = Ini.Read("LongName", shortName, shortName, LangNames);
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

            ResumeLayout(false);
        }

        private string LangNames { get; }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void LangSelectionForm_Load(object sender, EventArgs e)
        {
            FormEx.Dockable(this);
            Language.SetControlLang(this);
        }

        private void SetArchiveLangForm_Shown(object sender, EventArgs e) =>
            SystemSounds.Asterisk.Play();

        private void OKBtn_Click(object sender, EventArgs e)
        {
            var selected = langBox.GetItemText(langBox.SelectedItem);
            var langKey = Ini.Read("ShortName", selected, selected, LangNames);
            _appData.Settings.ArchiveLang = langKey;
            _appData.Settings.ArchiveLangConfirmed = rememberLangCheck.Checked;
            DialogResult = DialogResult.OK;
        }

        private void CancelBtn_Click(object sender, EventArgs e) =>
            DialogResult = DialogResult.Cancel;
    }
}
