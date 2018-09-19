namespace Updater.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using LangResources;
    using Libraries;
    using Properties;
    using SilDev;
    using SilDev.Forms;
    using SilDev.Investment;
    using Timer = System.Windows.Forms.Timer;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Icon = Resources.Logo;
            logoBox.Image = Resources.Changelog;
            Language.SetControlLang(this);
            changeLogPanel.ResumeLayout(false);
            ((ISupportInitialize)logoBox).EndInit();
            buttonPanel.ResumeLayout(false);
            statusTableLayoutPanel.ResumeLayout(false);
            statusBarPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private CounterInvestor<int> Counter { get; } = new CounterInvestor<int>();

        private List<string> DownloadMirrors { get; } = new List<string>();

        private string HashInfo { get; set; }

        private string LastFinalStamp { get; set; }

        private string LastStamp { get; set; }

        private NetEx.AsyncTransfer Transferor { get; } = new NetEx.AsyncTransfer();

        private void SetChangeLog(params string[] mirrors)
        {
            if (mirrors?.Any() == true)
            {
                var changes = string.Empty;
                foreach (var mirror in mirrors)
                {
                    var path = PathEx.AltCombine(mirror, "ChangeLog.txt");
                    if (string.IsNullOrWhiteSpace(path))
                        continue;
                    if (!NetEx.FileIsAvailable(path, 60000))
                        continue;
                    changes = NetEx.Transfer.DownloadString(path);
                    if (!string.IsNullOrWhiteSpace(changes))
                        break;
                }
                if (string.IsNullOrWhiteSpace(changes))
                    return;
                changeLog.Font = new Font("Consolas", 8.25f);
                changeLog.Text = TextEx.FormatNewLine(changes);
                var colorMap = new Dictionary<Color, string[]>
                {
                    {
                        Color.PaleGreen, new[]
                        {
                            " PORTABLE APPS SUITE",
                            " UPDATED:",
                            " CHANGES:"
                        }
                    },
                    {
                        Color.SkyBlue, new[]
                        {
                            " Global:",
                            " Apps Launcher:",
                            " Apps Downloader:",
                            " Apps Suite Updater:"
                        }
                    },
                    {
                        Color.Khaki, new[]
                        {
                            "Version History:"
                        }
                    },
                    {
                        Color.Plum, new[]
                        {
                            "{", "}",
                            "(", ")",
                            "|",
                            ".",
                            "-"
                        }
                    },
                    {
                        Color.Tomato, new[]
                        {
                            " * "
                        }
                    },
                    {
                        Color.Black, new[]
                        {
                            new string('_', 84)
                        }
                    }
                };
                foreach (var line in changeLog.Text.Split('\n'))
                {
                    if (line.Length < 1 || !DateTime.TryParseExact(line.Trim(' ', ':'), "d MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
                        continue;
                    changeLog.MarkText(line, Color.Khaki);
                }
                foreach (var color in colorMap)
                    foreach (var s in color.Value)
                        changeLog.MarkText(s, color.Key);
            }
            else
            {
                changeLog.Dock = DockStyle.None;
                changeLog.Size = new Size(changeLogPanel.Width, TextRenderer.MeasureText(changeLog.Text, changeLog.Font).Height);
                changeLog.Location = new Point(0, changeLogPanel.Height / 2 - changeLog.Height - 16);
                changeLog.SelectAll();
                changeLog.SelectionAlignment = HorizontalAlignment.Center;
                changeLog.DeselectAll();
            }
        }

        private void SetUpdateInfo(bool final, params string[] mirrors)
        {
            if (mirrors?.Any() != true)
                return;
            foreach (var mirror in mirrors)
            {
                try
                {
                    var path = PathEx.AltCombine(mirror, "Last.ini");
                    if (!NetEx.FileIsAvailable(path, 60000))
                        throw new PathNotFoundException(path);
                    var data = NetEx.Transfer.DownloadString(path);
                    if (string.IsNullOrWhiteSpace(data))
                        throw new ArgumentNullException(nameof(data));
                    var lastStamp = Ini.ReadOnly("Info", "LastStamp", data);
                    if (string.IsNullOrWhiteSpace(lastStamp))
                        throw new ArgumentNullException(nameof(lastStamp));
                    path = PathEx.AltCombine(mirror, $"{lastStamp}.ini");
                    if (!NetEx.FileIsAvailable(path, 60000))
                        throw new PathNotFoundException(path);
                    data = NetEx.Transfer.DownloadString(path);
                    if (string.IsNullOrWhiteSpace(data))
                        throw new ArgumentNullException(nameof(data));
                    HashInfo = data;
                    if (final)
                        LastFinalStamp = lastStamp;
                    else
                        LastStamp = lastStamp;
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
                if (!string.IsNullOrWhiteSpace(HashInfo))
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Check internet connection
            if (!Network.InternetIsAvailable)
            {
                Environment.ExitCode = 1;
                Application.Exit();
                return;
            }

            // Get update infos from GitHub if enabled
            if (Settings.UpdateChannel == Settings.UpdateChannelOptions.Beta)
            {
                if (!Network.IPv4IsAvalaible && Network.IPv6IsAvalaible)
                {
                    Environment.ExitCode = 1;
                    Application.Exit();
                    return;
                }
                SetUpdateInfo(false, CorePaths.RepoSnapshotsUrl);
            }

            // Get update infos if not already set
            if (string.IsNullOrWhiteSpace(HashInfo))
            {
                var mirrors = new[]
                {
                    // IPv4 + IPv6
                    "http://dl.0.port-a.de",
                    "http://dl.1.port-a.de",

                    // IPv4
                    "http://dl.2.port-a.de"
                };
                if (!Network.IPv4IsAvalaible && Network.IPv6IsAvalaible)
                    mirrors = mirrors.Take(2).ToArray();
                DownloadMirrors.AddRange(mirrors.Select(x => PathEx.AltCombine(x, "Port-Able%20Suite")));
                if (!DownloadMirrors.Any())
                {
                    Environment.ExitCode = 1;
                    Application.Exit();
                    return;
                }
                SetUpdateInfo(true, DownloadMirrors.ToArray());
            }
            if (string.IsNullOrWhiteSpace(HashInfo))
            {
                Environment.ExitCode = 1;
                Application.Exit();
                return;
            }

            // Compare hashes
            var updateAvailable = false;
            try
            {
                foreach (var key in Ini.GetKeys("SHA256", HashInfo))
                {
                    var file = PathEx.Combine(PathEx.LocalDir, $"{key}.exe");
                    if (!File.Exists(file))
                        file = Path.Combine(CorePaths.HomeDir, $"{key}.exe");
                    if (Ini.Read("SHA256", key, HashInfo).EqualsEx(file.EncryptFile(ChecksumAlgorithms.Sha256)))
                        continue;
                    updateAvailable = true;
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                Environment.ExitCode = 1;
                Application.Exit();
                return;
            }

            // Install updates
            if (updateAvailable)
                if (MessageBox.Show(Language.GetText(nameof(en_US.UpdateAvailableMsg)), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SetChangeLog(DownloadMirrors.ToArray());
                    ShowInTaskbar = true;
                    FormEx.Dockable(this);
                    return;
                }

            // Exit the application if no updates were found
            Environment.ExitCode = 2;
            Application.Exit();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!ShowInTaskbar)
                return;
            Refresh();
            var timer = new Timer(components)
            {
                Interval = 1,
                Enabled = true
            };
            timer.Tick += (o, args) =>
            {
                if (Opacity < 1d)
                {
                    Opacity += .05d;
                    return;
                }
                timer.Dispose();
            };
        }

        private void ChangeLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
                WindowState = FormWindowState.Minimized;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (!(sender is Button owner))
                return;

            owner.Enabled = false;
            var downloadPath = default(string);
            if (!string.IsNullOrWhiteSpace(LastStamp))
                try
                {
                    downloadPath = PathEx.AltCombine(CorePaths.RepoSnapshotsUrl, $"{LastStamp}.7z");
                    if (!NetEx.FileIsAvailable(downloadPath, 60000))
                        throw new PathNotFoundException(downloadPath);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    downloadPath = null;
                }

            if (string.IsNullOrWhiteSpace(downloadPath))
                try
                {
                    var exist = false;
                    foreach (var mirror in DownloadMirrors)
                    {
                        downloadPath = PathEx.AltCombine(mirror, $"{LastFinalStamp}.7z");
                        exist = NetEx.FileIsAvailable(downloadPath, 60000);
                        if (exist)
                            break;
                    }
                    if (!exist)
                        throw new PathNotFoundException(downloadPath);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    downloadPath = null;
                }

            if (!string.IsNullOrWhiteSpace(downloadPath))
                try
                {
                    if (!Directory.Exists(CachePaths.UpdateDir))
                        Directory.CreateDirectory(CachePaths.UpdateDir);
                    foreach (var path in CorePaths.FileArchiverFiles)
                    {
                        var file = Path.GetFileName(path);
                        if (string.IsNullOrEmpty(file))
                            continue;
                        File.Copy(path, Path.Combine(CachePaths.UpdateDir, file));
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex, true);
                    return;
                }

            try
            {
                Transferor.DownloadFile(downloadPath, CachePaths.UpdatePath);
                checkDownload.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, true);
            }
        }

        private void CheckDownload_Tick(object sender, EventArgs e)
        {
            if (!(sender is Timer owner))
                return;

            statusLabel.Text = $@"{Transferor.TransferSpeedAd} - {Transferor.DataReceived}";
            statusBar.Value = Transferor.ProgressPercentage;
            if (Transferor.IsBusy)
                return;
            if (Counter.Increase(0) == 10)
                statusBar.JumpToEnd();
            if (Counter.GetValue(0) < 100)
                return;

            owner.Enabled = false;
            try
            {
                var lastStamp = LastFinalStamp;
                if (string.IsNullOrWhiteSpace(lastStamp))
                    lastStamp = LastStamp;
                if (!Ini.Read("MD5", lastStamp, HashInfo).EqualsEx(CachePaths.UpdatePath.EncryptFile()))
                    throw new InvalidOperationException();

                var helper = string.Format(Resources.BatchDummy, ActionGuid.CurrentAction.RemoveChar('{', '}'), CorePaths.HomeDir.TrimEnd('\\'), Guid.NewGuid());
                File.WriteAllText(CachePaths.UpdateHelperPath, helper);

                var regPath = Path.Combine(Settings.RegistryPath, CorePaths.HomeDir.Encrypt(ChecksumAlgorithms.Adler32));
                var elevated = Reg.SubKeyExists(regPath) && Reg.GetSubKeys(regPath)?.Any() == true;
                if (elevated)
                    Reg.RemoveSubKey(regPath);

                ProcessEx.Start(CachePaths.UpdateHelperPath, elevated, ProcessWindowStyle.Hidden);
                Application.Exit();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                MessageBoxEx.Show(this, Language.GetText(nameof(en_US.InstallErrorMsg)), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.ExitCode = 1;
                CancelBtn_Click(cancelBtn, EventArgs.Empty);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Transferor.IsBusy)
                    Transferor.CancelAsync();
                DirectoryEx.Delete(CachePaths.UpdateDir);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            Application.Exit();
        }

        private void ProgressLabel_TextChanged(object sender, EventArgs e)
        {
            try
            {
                statusTableLayoutPanel.ColumnStyles[0].Width = progressLabel.Width + 8;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void VirusTotalBtn_Click(object sender, EventArgs e)
        {
            if (!(sender is Label owner))
                return;
            owner.Enabled = false;
            try
            {
                foreach (var key in Ini.GetKeys("SHA256", HashInfo))
                {
                    Process.Start(string.Format(CorePaths.VirusTotalUrl, Ini.Read("SHA256", key, HashInfo)));
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            owner.Enabled = true;
        }

        private void WebBtn_Click(object sender, EventArgs e) =>
            Process.Start(CorePaths.PortAbleUrl);
    }
}
