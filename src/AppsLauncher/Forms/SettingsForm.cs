namespace AppsLauncher.Forms;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Data;
using LangResources;
using Libraries;
using PortAble;
using Properties;
using SilDev;
using SilDev.Drawing;
using SilDev.Forms;
using SilDev.Ini.Legacy;
using static SilDev.WinApi;

public partial class SettingsForm : Form
{
    public sealed override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    private bool PreventAppSettingsLoader { get; set; }

    private AppsLauncherJson SettingsNew { get; }

    private LocalAppData SelectedAppData { get; set; }

    private DialogResult Result { get; set; }

    public SettingsForm(AppsLauncherJson settings, LocalAppData appData)
    {
        PreventAppSettingsLoader = true;

        SettingsNew = settings;
        SelectedAppData = appData;

        InitializeComponent();

        if (Desktop.AppsUseDarkTheme)
        {
            Desktop.EnableDarkMode(Handle);
            this.ChangeColorMode(false);
        }

        SuspendLayout();

        Icon = Resources.PaLogoSymbol;

        var backColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Back, DefaultBackColor);
        if (!Desktop.AppsUseDarkTheme)
            BackColor = backColor;
        tabCtrlButtonBorderPanel.BackColor = backColor;
        foreach (TabPage tab in tabCtrl.TabPages)
            tab.BackColor = backColor;

        CacheData.SetComponentImageColor(locationBtn, true);
        CacheData.SetComponentImageColor(associateBtn, Color.DarkGoldenrod);
        CacheData.SetComponentImageColor(restoreFileTypesBtn, Color.DarkGreen);
        CacheData.SetComponentImageColor(addToShellBtn, Color.DarkGoldenrod);
        CacheData.SetComponentImageColor(rmFromShellBtn, Color.DarkGoldenrod);

        fileTypes.AutoVerticalScrollBar();
        fileTypes.MaxLength = short.MaxValue;
        if (EnvironmentEx.IsAtLeastWindows(11))
            Desktop.RoundCorners(fileTypesMenu.Handle, true);
        else
        {
            fileTypesMenu.EnableAnimation();
            fileTypesMenu.SetFixedSingle();
        }
        restoreFileTypesBtn.Height = associateBtn.Height;

        previewBg.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Back, DefaultBackColor);
        if (SettingsNew.WindowBackground != default)
        {
            var width = (int)Math.Round(SettingsNew.WindowBackground.Width * .65d) + 1;
            var height = (int)Math.Round(SettingsNew.WindowBackground.Height * .65d) + 1;
            previewBg.BackgroundImage = SettingsNew.WindowBackground.Redraw(width, height);
            previewBg.BackgroundImageLayout = SettingsNew.WindowBackgroundLayout;
        }
        var exeImage = CacheData.GetImage(nameof(Resources.PaLogoClear), Resources.PaLogoClear, default, 16);
        if (exeImage != null)
        {
            previewSmallImgList.Images.Add(exeImage);
            previewSmallImgList.Images.Add(exeImage);
        }
        exeImage = CacheData.GetImage(nameof(Resources.PaLogoClear), Resources.PaLogoClear, default, 32);
        if (exeImage != null)
        {
            previewLargeImgList.Images.Add(exeImage);
            previewLargeImgList.Images.Add(exeImage);
        }
        previewAppList.StateImageList = SettingsNew.ViewStyle == View.LargeIcon ? previewLargeImgList : previewSmallImgList;
        previewAppList.View = SettingsNew.ViewStyle == View.LargeIcon ? View.Tile : View.List;

        var comparer = new AlphaNumericComparer<object>();
        var appNames = CacheData.CurrentAppInfo.Select(x => x.Name).Cast<object>().OrderBy(x => x, comparer).ToArray();
        appsBox.Items.AddRange(appNames);

        appsBox.SelectedItem = SelectedAppData?.Name;
        if (appsBox.Items.Count > 0 && appsBox.SelectedIndex < 0)
            appsBox.SelectedIndex = 0;

        appDirs.AutoVerticalScrollBar();

        LoadSettings();

        ResumeLayout(false);
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {
        this.Dockable();
        NativeHelper.MoveWindowToVisibleScreenArea(Handle);
        PreventAppSettingsLoader = false;
        AppsBox_SelectedIndexChanged(appsBox, EventArgs.Empty);
    }

    private void SettingsForm_Shown(object sender, EventArgs e)
    {
        var timer = new Timer(components)
        {
            Interval = 1,
            Enabled = true
        };
        timer.Tick += (o, _) =>
        {
            if (o is not Timer owner)
                return;
            if (Opacity < 1d)
            {
                Opacity += .1d;
                return;
            }
            owner.Enabled = false;
            TopMost = false;
            Result = DialogResult.No;
            owner.Dispose();
        };
    }

    private void SettingsForm_EnabledChanged(object sender, EventArgs e)
    {
        if (sender is Form { Enabled: true })
            AppsBox_SelectedIndexChanged(appsBox, EventArgs.Empty);
    }

    private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e) =>
        DialogResult = Result;

    private void LoadSettings()
    {
        viewStyle.SelectedIndex = SettingsNew.ViewStyle switch
        {
            View.Tile => 1,
            View.LargeIcon => 2,
            _ => 0
        };

        var decValue = SettingsNew.BlurStrength.ToDecimal();
        blurNum.Value = decValue >= blurNum.Minimum && decValue <= blurNum.Maximum ? decValue : 90m;

        decValue = SettingsNew.OpacityLevel.ToDecimal();
        opacityNum.Value = decValue >= opacityNum.Minimum && decValue <= opacityNum.Maximum ? decValue : .85m;

        defBgCheck.Checked = !File.Exists(CachePaths.CurrentBackgroundImage);

        var intValue = SettingsNew.WindowBackgroundLayout.ToInt32();
        bgLayout.SelectedIndex = intValue.IsBetween(1, bgLayout.Items.Count - 1) ? intValue : 1;

        mainColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Back, SystemColors.Window);
        controlColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Control, SystemColors.Control);
        controlTextColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.ControlText, SystemColors.ControlText);
        btnHoverColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Highlight, SystemColors.Highlight);

        showCaptionCheck.Checked = SettingsNew.ShowCaption;
        showInTaskbarCheck.Checked = SettingsNew.ShowInTaskbar;

        StylePreviewUpdate();

        appDirs.Text = SettingsNew.AppInstallLocations.Join(Environment.NewLine);

        startMenuIntegration.SelectedIndex = SettingsNew.SystemStartMenuIntegration ? 1 : 0;

        intValue = (int)SettingsNew.StartPosition;
        defaultPos.SelectedIndex = intValue > 0 && intValue < defaultPos.Items.Count ? intValue : 0;

        intValue = (int)SettingsNew.UpdateCheck;
        updateCheck.SelectedIndex = intValue > 0 && intValue < updateCheck.Items.Count ? intValue : 0;

        intValue = (int)SettingsNew.UpdateChannel;
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
        if (PreventAppSettingsLoader)
            return;

        var selectedApp = (sender as ComboBox)?.SelectedItem?.ToString();
        SelectedAppData = CacheData.FindInCurrentAppInfo(selectedApp);
        if (SelectedAppData?.Name?.EqualsEx(selectedApp) != true)
            return;

        var appData = SelectedAppData;
        fileTypes.Text = appData.Settings.FileTypes.Join(',');

        var restPointDir = Path.Combine(CorePaths.FileTypeAssocDir, appData.Key);
        restoreFileTypesBtn.Enabled = Directory.Exists(restPointDir) && DirectoryEx.EnumerateFiles(restPointDir, "*.dat", SearchOption.AllDirectories)?.Any() == true;
        restoreFileTypesBtn.Visible = restoreFileTypesBtn.Enabled;

        startArgsFirst.Text = appData.Settings.StartArgsMod?.Item1;
        startArgsLast.Text = appData.Settings.StartArgsMod?.Item2;

        sortArgPathsCheck.Checked = appData.Settings.StartArgsDoSort;
        runAsAdminCheck.Checked = appData.Settings.RunAsAdmin;
        noUpdatesCheck.Checked = appData.Settings.DisableUpdates;
        langConfirmedCheck.Checked = appData.Settings.ArchiveLangConfirmed;
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
                    var appData = CacheData.FindInCurrentAppInfo(selectedItem);
                    if (appData != default)
                    {
                        var iniPath = appData.AppInfoPath;
                        if (!File.Exists(iniPath))
                            iniPath = appData.ConfigPath;
                        if (!File.Exists(iniPath))
                            iniPath = DirectoryEx.EnumerateFiles(appData.InstallDir, "*.ini").LastOrDefault();
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
        var appInfo = CacheData.FindInCurrentAppInfo(appsBox.SelectedItem.ToString());
        if (!appInfo.Name.EqualsEx(appsBox.SelectedItem.ToString()))
            return false;
        var alreadyDefined = new Dictionary<string, List<string>>();
        foreach (var settings in CacheData.CurrentAppSettings)
        {
            if (settings.Key.EqualsEx(appInfo.Key))
                continue;
            if (settings.Value.FileTypes?.Count is null or < 1)
                continue;
            var selectedTypes = fileTypes.Text.Split(',').Select(LocalGetPureExtension).ToList();
            var settingsTypes = settings.Value.FileTypes.Select(LocalGetPureExtension).ToList();
            foreach (var type in selectedTypes.Where(type => settingsTypes.ContainsItem(type)))
            {
                if (!alreadyDefined.ContainsKey(settings.Key))
                {
                    alreadyDefined.Add(settings.Key, new List<string>
                    {
                        type
                    });
                    continue;
                }
                if (!alreadyDefined[settings.Key].ContainsItem(type))
                    alreadyDefined[settings.Key].Add(type);
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
            catch (Exception ex) when (ex.IsCaught())
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
        return MessageBoxEx.Show(this, string.Format(CultureInfo.InvariantCulture, Language.GetText(nameof(en_US.AssociateConflictMsg)), msg), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes;

        static string LocalGetPureExtension(string extension) =>
            extension.TrimStart('*', '.');
    }

    private void AssociateBtn_Click(object sender, EventArgs e)
    {
        if (sender is not Control owner)
            return;
        var isNull = string.IsNullOrWhiteSpace(fileTypes.Text);
        if (!isNull)
            if (fileTypes.Text.Contains(","))
                isNull = fileTypes.Text.Split(',').Where(s => !s.StartsWith(".", StringComparison.Ordinal)).ToArray().Length == 0;
            else
                isNull = fileTypes.Text.StartsWith(".", StringComparison.Ordinal);
        if (isNull)
        {
            MessageBoxEx.Show(this, Language.GetText($"{owner.Name}Msg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (SelectedAppData == default || FileTypesConflict())
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
        if (SelectedAppData == default)
        {
            MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCanceledMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        FileTypeAssoc.Restore(SelectedAppData, true, this);
    }

    private void SetBgBtn_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog();
        dialog.CheckFileExists = true;
        dialog.CheckPathExists = true;
        dialog.Multiselect = false;
        var path = PathEx.Combine(PathEx.LocalDir, "Assets", "bg");
        if (Directory.Exists(path))
            dialog.InitialDirectory = path;
        var imageEncoders = ImageCodecInfo.GetImageEncoders();
        var extensions = new List<string>();
        for (var i = 0; i < imageEncoders.Length; i++)
        {
            extensions.Add(imageEncoders[i].FilenameExtension.ToLowerInvariant());
            var description = imageEncoders[i].CodecName.Substring(8).Replace("Codec", "Files").Trim();
            var pattern = extensions[extensions.Count - 1];
            dialog.Filter = string.Format(CultureInfo.InvariantCulture, @"{0}{1}{2} ({3})|{3}", dialog.Filter, i > 0 ? "|" : string.Empty, description, pattern);
        }
        dialog.Filter = string.Format(CultureInfo.InvariantCulture, @"{0}|Image Files ({1})|{1}", dialog.Filter, extensions.Join(";"));
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
            CacheData.CurrentBackgroundImage = image;
            previewBg.BackgroundImage = image.Redraw((int)Math.Round(image.Width * .65f) + 1, (int)Math.Round(image.Height * .65f) + 1);
            defBgCheck.Checked = false;
            Result = DialogResult.Yes;
            MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationCompletedMsg)), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex.IsCaught())
        {
            Log.Write(ex);
            MessageBoxEx.Show(this, Language.GetText(nameof(en_US.OperationFailedMsg)), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void DefBgCheck_CheckedChanged(object sender, EventArgs e)
    {
        if (sender is not CheckBox owner)
            return;
        try
        {
            if (!owner.Checked)
            {
                var bgImg = File.ReadAllBytes(CachePaths.CurrentBackgroundImage).DeserializeObject<Image>();
                previewBg.BackgroundImage = bgImg.Redraw((int)Math.Round(bgImg.Width * .65f) + 1, (int)Math.Round(bgImg.Height * .65f) + 1);
            }
            else
                previewBg.BackgroundImage = default;
        }
        catch (Exception ex) when (ex.IsCaught())
        {
            if (!owner.Checked)
                owner.Checked = true;
        }
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
        if (sender is not Panel owner)
            return;
        var title = default(string);
        try
        {
            title = Controls.Find(owner.Name + "Label", true).First().Text;
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
        var savedOles = SettingsNew.CustomColors
                                   .Select(ColorEx.ToRgb)
                                   .Select(ColorEx.FromRgb)
                                   .Select(ColorTranslator.ToOle)
                                   .ToArray();

        dialog.CustomColors = savedOles;
        if (dialog.ShowDialog() == DialogResult.Cancel)
            return;

        // Alpha to maximum so that the color is completely opaque.
        if (dialog.Color != owner.BackColor)
        {
            SettingsNew.WindowColorsForceDefault = false;
            owner.BackColor = Color.FromArgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);
        }

        if (dialog.CustomColors is { } newOles)
        {
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
                SettingsNew.CustomColors = filterdOles.Select(ColorTranslator.FromOle).ToList();
        }

        StylePreviewUpdate();
    }

    private void ResetColorsBtn_Click(object sender, EventArgs e)
    {
        SettingsNew.WindowColorsForceDefault = true;
        SettingsNew.ResetColors();

        mainColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Back, SystemColors.Window);
        controlColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Control, SystemColors.Control);
        controlTextColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.ControlText, SystemColors.ControlText);
        btnHoverColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Highlight, SystemColors.Highlight);

        Result = DialogResult.Yes;

        StylePreviewUpdate();
    }

    private void PreviewAppList_Paint(object sender, PaintEventArgs e)
    {
        if (sender is not Panel owner)
            return;
        using var g = e.Graphics;
        g.TranslateTransform((int)(owner.Width / (Math.PI * 2)), owner.Width + 40);
        g.RotateTransform(-70);
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        using var b = new SolidBrush(Color.FromArgb(50, (byte)~owner.BackColor.R, (byte)~owner.BackColor.G, (byte)~owner.BackColor.B));
        using var f = new Font("Comic Sans MS", 24f);
        g.DrawString("Preview", f, b, 0f, 0f);
    }

    private void ControlOption_Changed(object sender, EventArgs e)
    {
        Result = DialogResult.Yes;
        StylePreviewUpdate();
    }

    private void StylePreviewUpdate()
    {
        previewCaption.Visible = showCaptionCheck.Checked;
        previewBg.BackgroundImageLayout = bgLayout.SelectedIndex switch
        {
            0 => ImageLayout.None,
            2 => ImageLayout.Center,
            3 => ImageLayout.Stretch,
            4 => ImageLayout.Zoom,
            _ => ImageLayout.Tile
        };
        previewBg.BackColor = mainColorPanel.BackColor;
        previewMainColor.BackColor = mainColorPanel.BackColor;
        previewAppList.ForeColor = controlTextColorPanel.BackColor;
        previewAppList.BackColor = controlColorPanel.BackColor;
        previewAppList.StateImageList = viewStyle.SelectedIndex > 1 ? previewLargeImgList : previewSmallImgList;
        previewAppList.Scrollable = viewStyle.SelectedIndex < 2;
        previewAppList.View = viewStyle.SelectedIndex > 1 ? View.Tile : View.List;
        previewAppListPanel.BackColor = controlColorPanel.BackColor;
    }

    private void ShellBtns_Click(object sender, EventArgs e)
    {
        if (sender as Button == addToShellBtn)
        {
            SystemIntegration.Apply();
            return;
        }
        SystemIntegration.Remove();
    }

    private void ShellBtns_TextChanged(object sender, EventArgs e)
    {
        if (sender is Button owner)
            owner.TextAlign = owner.Text.Length < 0x16 ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleRight;
    }

    private void SaveBtn_Click(object sender, EventArgs e)
    {
        SaveAppSettings(e);

        SettingsNew.ViewStyle = viewStyle.SelectedIndex switch
        {
            1 => View.Tile,
            2 => View.LargeIcon,
            _ => View.List
        };

        SettingsNew.OpacityLevel = opacityNum.Value.ToDouble();

        SettingsNew.BlurStrength = blurNum.Value.ToInt32();

        if (defBgCheck.Checked)
        {
            if (CacheData.CurrentBackgroundImage != default)
            {
                CacheData.CurrentBackgroundImage = default;
                Result = DialogResult.Yes;
            }
            bgLayout.SelectedIndex = 1;
        }

        SettingsNew.WindowBackgroundLayout = (ImageLayout)bgLayout.SelectedIndex;

        SettingsNew.WindowColors.TrySet(ColorOption.Back, mainColorPanel.BackColor);
        SettingsNew.WindowColors.TrySet(ColorOption.Control, controlColorPanel.BackColor);
        SettingsNew.WindowColors.TrySet(ColorOption.ControlText, controlTextColorPanel.BackColor);
        SettingsNew.WindowColors.TrySet(ColorOption.Highlight, btnHoverColorPanel.BackColor);

        mainColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Back, SystemColors.Window);
        controlColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Control, SystemColors.Control);
        controlTextColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.ControlText, SystemColors.ControlText);
        btnHoverColorPanel.BackColor = SettingsNew.WindowColors.TryGetValue(ColorOption.Highlight, SystemColors.Highlight);

        SettingsNew.ShowCaption = showCaptionCheck.Checked;
        SettingsNew.ShowInTaskbar = showInTaskbarCheck.Checked;

        if (!string.IsNullOrWhiteSpace(appDirs.Text))
        {
            CacheData.UpdateAppDirs(appDirs.Text);
            SettingsNew.AppInstallLocations =
                CacheData.AppDirs.Count > CorePaths.AppDirs.Count
                    ? CacheData.AppDirs.Where(x => !CorePaths.AppDirs.Contains(x))
                               .Select(x => EnvironmentEx.GetVariableWithPath(x))
                               .ToList()
                    : new();

            appDirs.Text = SettingsNew.AppInstallLocations.Join(Environment.NewLine);
        }

        SettingsNew.SystemStartMenuIntegration = startMenuIntegration.SelectedIndex > 0;
        if (!SettingsNew.SystemStartMenuIntegration)
        {
            var lnkDirs = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.SendTo),
                PathEx.Combine(Environment.SpecialFolder.StartMenu, "Programs")
            };
            foreach (var lnk in lnkDirs.Select(d => DirectoryEx.EnumerateFiles(d, "Apps Launcher*.lnk")).SelectMany(s => s.Where(File.Exists)))
                FileEx.TryDelete(lnk);
            var startMenuDir = Path.Combine(lnkDirs.Last(), "Portable Apps");
            DirectoryEx.TryDelete(startMenuDir);
        }

        SettingsNew.StartPosition = (StartPositionOption)defaultPos.SelectedIndex;

        SettingsNew.UpdateCheck = (UpdateCheckOption)updateCheck.SelectedIndex;
        SettingsNew.UpdateChannel = (UpdateChannelOption)updateChannel.SelectedIndex;

        SettingsNew.SaveToFile();

        MessageBoxEx.Show(this, Language.GetText(nameof(en_US.SavedSettings)), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    private void SaveAppSettings(EventArgs e)
    {
        var appData = SelectedAppData;
        if (appData == default)
            return;

        if (string.IsNullOrWhiteSpace(fileTypes.Text))
            appData.Settings.FileTypes = default;
        else
        {
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
                    if (type.StartsWith(".", StringComparison.Ordinal))
                    {
                        while (type.Contains(".."))
                            type = type.Replace("..", ".");
                        if (typesList.ContainsItem(type) || typesList.ContainsItem(type.Substring(1)))
                            continue;
                    }
                    else
                    {
                        if (typesList.ContainsItem(type) || typesList.ContainsItem($".{type}"))
                            continue;
                    }
                    if (type.Length == 1 && type.StartsWith(".", StringComparison.Ordinal))
                        continue;
                    typesList.Add(type);
                }
                if (typesList.Count > 0)
                {
                    var comparer = new AlphaNumericComparer<string>();
                    typesList = typesList.OrderBy(x => x, comparer).ToList();
                    fileTypes.Text = typesList.Join(",");
                    appData.Settings.FileTypes = new Collection<string>(typesList);
                }
            }
            else
                fileTypes.Text = appData.Settings.FileTypes.Join(',');
        }

        if (!string.IsNullOrEmpty(startArgsFirst.Text) &&
            !string.IsNullOrEmpty(startArgsLast.Text))
            appData.Settings.StartArgsMod = new(startArgsFirst.Text, startArgsLast.Text);
        else
            appData.Settings.StartArgsMod = default;

        appData.Settings.StartArgsDoSort = sortArgPathsCheck.Checked;
        appData.Settings.RunAsAdmin = runAsAdminCheck.Checked;
        appData.Settings.DisableUpdates = noUpdatesCheck.Checked;
        appData.Settings.ArchiveLangConfirmed = langConfirmedCheck.Checked;

        appData.Settings.SaveToFile();
    }
}