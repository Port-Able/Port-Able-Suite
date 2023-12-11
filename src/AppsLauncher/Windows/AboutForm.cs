namespace AppsLauncher.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using Libraries;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public sealed partial class AboutForm : Form
    {
        private static readonly object BwLocker = new();
        private readonly ProgressCircle _progressCircle;

        private int? ExitCode { get; set; } = 0;

        public AboutForm()
        {
            InitializeComponent();

            if (Desktop.AppsUseDarkTheme)
                Desktop.EnableDarkMode(Handle);

            SuspendLayout();

            Language.SetControlLang(this);
            Text = Language.GetText(Name);

            Icon = Resources.PaLogoSymbol;

            AddFileInfoLabels();

            var series = spaceChart.Series.FirstOrDefault();
            if (series != null)
            {
                series.LabelForeColor = Color.Transparent;
                series.LabelBackColor = Color.Transparent;
            }
            diskChartUpdater.RunWorkerAsync();

            logoPanel.BackgroundImage = Resources.PatternDiagonal;
            logoPanel.BackColor = Settings.Window.Colors.Base.EnsureLightLight();

            CacheData.SetComponentImageColor(updateBtn);
            updateBtn.Width = TextRenderer.MeasureText(updateBtn.Text, updateBtn.Font).Width + 32;

            _progressCircle = new ProgressCircle
            {
                Anchor = updateBtn.Anchor,
                BackColor = Color.Transparent,
                ForeColor = mainPanel.BackColor,
                InnerRadius = 7,
                Location = new Point(updateBtn.Right + 3, updateBtn.Top + 1),
                OuterRadius = 9,
                RotationSpeed = 80,
                Size = new Size(updateBtn.Height, updateBtn.Height),
                Thickness = 3,
                Visible = false
            };
            mainPanel.Controls.Add(_progressCircle);

            var aboutInfoLabelData = new[]
            {
                new[]
                {
                    "Si13n7 Developments",
                    "http://www.si13n7.com"
                },
                new[]
                {
                    Language.GetText(nameof(aboutInfoLabel) + "LinkLabel1"),
                    "http://paypal.si13n7.com"
                },
                new[]
                {
                    Language.GetText(nameof(aboutInfoLabel) + "LinkLabel2"),
                    "https://support.si13n7.com"
                }
            };
            aboutInfoLabel.ActiveLinkColor = Settings.Window.Colors.Base;
            aboutInfoLabel.BorderStyle = BorderStyle.None;
            aboutInfoLabel.Text = string.Format(CultureInfo.InvariantCulture, Language.GetText(aboutInfoLabel), aboutInfoLabelData.Select(x => x.First()).Cast<object>().ToArray());
            aboutInfoLabel.Links.Clear();
            aboutInfoLabelData.ForEach(x => aboutInfoLabel.LinkText(x.First(), x.Last()));

            copyrightLabel.Text = string.Format(CultureInfo.InvariantCulture, copyrightLabel.Text, DateTime.Now.Year);

            ResumeLayout(false);
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            this.Dockable();
            WinApi.NativeHelper.MoveWindowToVisibleScreenArea(Handle);
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            var timer = new Timer(components)
            {
                Interval = 1,
                Enabled = true
            };
            timer.Tick += (_, _) =>
            {
                if (Opacity < 1d)
                {
                    Opacity += .1d;
                    return;
                }
                timer.Dispose();
                if (TopMost)
                    TopMost = false;
            };
        }

        private void AddFileInfoLabels()
        {
            var verInfoList = new List<FileVersionInfo>();
            var strArray = CorePaths.FullAppsSuitePathMap;
            var verArray = new Version[strArray.Length];
            for (var i = 0; i < strArray.Length; i++)
                foreach (var file in strArray[i])
                    try
                    {
                        var fvi = FileVersionInfo.GetVersionInfo(file);
                        verArray[i] = FileEx.GetVersion(fvi.FileName);
                        verInfoList.Add(fvi);
                    }
                    catch (Exception ex) when (ex.IsCaught())
                    {
                        Log.Write(ex);
                    }

            var bottom = 0;
            foreach (var fvi in verInfoList)
            {
                var description = fvi.FileDescription;
                if (description.StartsWithEx("7z"))
                    description = description.Replace("7z", "7-Zip");
                if (description.StartsWithEx("Json"))
                    description = description.Replace("Json.NET", "Newtonsoft Json.NET");
                var name = new Label
                {
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 12.25f, FontStyle.Bold, GraphicsUnit.Point),
                    ForeColor = Color.PowderBlue,
                    Location = new Point(aboutInfoLabel.Left, bottom == 0 ? 15 : bottom + 10),
                    Text = description
                };
                mainPanel.Controls.Add(name);
                Version reqVer;
                var fna = Path.GetFileName(fvi.FileName);
                if (fna.EqualsEx(strArray.Second().Select(Path.GetFileName).ToArray()))
                    reqVer = verArray.Second();
                else if (fna.EqualsEx("7zG.exe", "7z.dll"))
                    reqVer = verArray.Third();
                else
                    reqVer = verArray.First();
                var curVer = FileEx.GetVersion(fvi.FileName);
                var strVer = curVer.ToString();
                if (!fna.EqualsEx("7zG.exe", "7z.dll"))
                {
                    reqVer = Version.Parse(reqVer.ToString(3));
                    curVer = Version.Parse(curVer.ToString(3));
                }
                if (fna.EqualsEx("Newtonsoft.Json.dll"))
                    reqVer = curVer;
                var version = new Label
                {
                    AutoSize = true,
                    BackColor = name.BackColor,
                    Font = new Font(name.Font.FontFamily, 8.25f, FontStyle.Regular, name.Font.Unit),
                    ForeColor = reqVer == curVer ? Color.PaleGreen : Color.OrangeRed,
                    Location = new Point(name.Left + 3, name.Bottom),
                    Text = strVer
                };
                mainPanel.Controls.Add(version);
                var separator = new Label
                {
                    AutoSize = true,
                    BackColor = name.BackColor,
                    Font = version.Font,
                    ForeColor = copyrightLabel.ForeColor,
                    Location = new Point(version.Right, name.Bottom),
                    Text = Resources.Separator
                };
                mainPanel.Controls.Add(separator);
                var path = new Label
                {
                    AutoSize = true,
                    BackColor = name.BackColor,
                    Font = version.Font,
                    ForeColor = version.ForeColor,
                    Location = new Point(separator.Right, name.Bottom),
                    Text = Path.Combine(SystemIntegration.IsEnabled ? $"%{Settings.EnvironmentVariable}%" : ".",
                                        fvi.FileName.RemoveText(PathEx.LocalDir).TrimStart(Path.DirectorySeparatorChar))
                };
                mainPanel.Controls.Add(path);
                bottom = path.Bottom;
            }

            Height += bottom;
            if (StartPosition == FormStartPosition.CenterScreen)
                Top -= (int)Math.Floor(bottom / 2d);
        }

        private void DiskChartUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is not BackgroundWorker owner)
                return;
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!PathEx.LocalDir.StartsWithEx(drive.Name))
                    continue;
                var free = drive.TotalFreeSpace;
                owner.ReportProgress(0, free);
                owner.ReportProgress(25, drive.TotalSize - free);
                var apps = DirectoryEx.GetSize(PathEx.LocalDir);
                var other = drive.TotalSize - free - apps;
                owner.ReportProgress(0, free);
                owner.ReportProgress(50, apps);
                owner.ReportProgress(100, other);
                break;
            }
        }

        private void DiskChartUpdater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is not long length)
            {
                spaceChart.Visible = false;
                return;
            }
            try
            {
                var series = spaceChart.Series.First();
                switch (e.ProgressPercentage)
                {
                    case 0:
                        series.Points.Clear();
                        series.Points.AddXY($"{length.FormatSize(SizeOption.Round)} Free", length);
                        break;
                    case 25:
                        series.Points.AddXY($"{length.FormatSize(SizeOption.Round)} Used", length);
                        series.Points.Last().Color = Color.Firebrick;
                        break;
                    case 50:
                        series.Points.AddXY($"{length.FormatSize(SizeOption.Round)} Used (Apps)", length);
                        break;
                    case 100:
                        series.Points.AddXY($"{length.FormatSize(SizeOption.Round)} Used (Other)", length);
                        break;
                }
            }
            catch (NullReferenceException ex)
            {
                if (Log.DebugMode > 1)
                    Log.Write(ex);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                Log.Write(ex);
            }
        }

        private void AboutForm_FormClosing(object sender, FormClosingEventArgs e) =>
            e.Cancel = updateChecker.IsBusy;

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (sender is not Button owner)
                return;
            owner.Enabled = false;
            if (!updateChecker.IsBusy)
            {
                _progressCircle.Active = true;
                _progressCircle.Visible = true;
                updateChecker.RunWorkerAsync();
            }
            closeToUpdate.Enabled = true;
        }

        private void UpdateChecker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var process = ProcessEx.Start(CorePaths.AppsSuiteUpdater, false, false))
            {
                if (process?.HasExited == false)
                    process.WaitForExit();
                lock (BwLocker)
                    ExitCode = process?.ExitCode;
            }
            using (var process = ProcessEx.Start(CorePaths.AppsDownloader, ActionGuid.UpdateInstance, false, false))
            {
                if (process?.HasExited == false)
                    process.WaitForExit();
                lock (BwLocker)
                    ExitCode = process?.ExitCode;
            }
        }

        private void UpdateChecker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) =>
            updateBtn.Enabled = true;

        private void CloseToUpdate_Tick(object sender, EventArgs e)
        {
            if (updateChecker.IsBusy)
                return;
            _progressCircle.Active = false;
            _progressCircle.Visible = false;
            closeToUpdate.Enabled = false;
            var message = ExitCode switch
            {
                0 => Language.GetText(nameof(en_US.OperationCompletedMsg)),
                1 => Language.GetText(nameof(en_US.OperationCanceledMsg)),
                _ => Language.GetText(nameof(en_US.NoUpdatesFoundMsg))
            };
            MessageBoxEx.Show(this, message, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AboutInfoLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e?.Link?.LinkData is Uri url)
                Process.Start(url.ToString());
        }
    }
}
