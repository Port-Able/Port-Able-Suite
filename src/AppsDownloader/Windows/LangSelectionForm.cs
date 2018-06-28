namespace AppsDownloader.Windows
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Media;
    using System.Windows.Forms;
    using Libraries;
    using SilDev.Drawing;
    using SilDev.Forms;

    public partial class LangSelectionForm : Form
    {
        private readonly AppData _appData;

        public LangSelectionForm(AppData appData)
        {
            _appData = appData;
            if (_appData == default(AppData))
                throw new ArgumentNullException(nameof(appData));

            InitializeComponent();
            SuspendLayout();

            Text = Language.GetText(Name);

            if (!CacheData.AppImagesLarge.TryGetValue(appData.Key, out var image))
                CacheData.AppImages.TryGetValue(appData.Key, out image);
            if (image != default(Image))
                Icon = image.ToIcon();

            appNameLabel.Text = _appData.Name;

            langBox.Items.AddRange(_appData.Languages.Cast<object>().ToArray());
            langBox.SelectedItem = _appData.Settings.ArchiveLang;
            if (langBox.SelectedIndex < 0)
                langBox.SelectedIndex = 0;

            rememberLangCheck.Checked = _appData.Settings.ArchiveLangConfirmed;

            ResumeLayout(false);
        }

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
            _appData.Settings.ArchiveLang = langBox.GetItemText(langBox.SelectedItem);
            _appData.Settings.ArchiveLangConfirmed = rememberLangCheck.Checked;
            DialogResult = DialogResult.OK;
        }

        private void CancelBtn_Click(object sender, EventArgs e) =>
            DialogResult = DialogResult.Cancel;
    }
}
