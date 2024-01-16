namespace AppsDownloader.Forms;

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

        ResumeLayout(false);

        if (!Desktop.AppsUseDarkTheme)
            return;
        Desktop.EnableDarkMode(Handle);
        Desktop.EnableDarkMode(rememberLangCheck.Handle);
        Desktop.EnableDarkMode(okBtn.Handle);
        Desktop.EnableDarkMode(cancelBtn.Handle);
        this.ChangeColorMode(false);
    }

    private static string GetShortName(string longName) =>
        longName switch
        {
            "*Default" => "Default",
            "*English" => "English",
            "*Multilingual" => "Multilingual",
            "Chinese (Simplified, PRC)" => "SimpChinese",
            "Chinese (Traditional)" => "TradChinese",
            "English (United Kingdom)" => "EnglishGB",
            "Portuguese (Brazil)" => "PortugueseBR",
            "Portuguese (Portugal)" => "Portuguese",
            "Serbian (Latin)" => "SerbianLatin",
            "Spanish (International)" => "SpanishInternational",
            "Spanish (Spain)" => "Spanish",
            "Persian" => "Farsi",
            _ => longName
        };

    private static string GetLongName(string shortName) =>
        shortName switch
        {
            "Default" => "*Default",
            "English" => "*English",
            "Multilingual" => "*Multilingual",
            "SpanishInternational" => "Spanish (International)",
            _ => CultureConfig.GetCultureInfo(shortName).DisplayName,
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
        _appData.Settings.ArchiveLangConfirmed = rememberLangCheck.Checked;
        _appData.Settings.SaveToFile();
        DialogResult = DialogResult.OK;
    }

    private void CancelBtn_Click(object sender, EventArgs e) =>
        DialogResult = DialogResult.Cancel;
}