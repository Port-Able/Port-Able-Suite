namespace Updater
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using LangResources;
    using Properties;
    using SilDev;
    using SilDev.Forms;
    using SilDev.Investment;
    using Timer = System.Windows.Forms.Timer;

    public partial class MainForm : Form
    {
        private string _hashInfo, _lastFinalStamp, _lastStamp;
        private bool _ipv4, _ipv6;

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

        private static List<string> DownloadMirrors { get; } = new List<string>();

        private static string HomeDir { get; } = PathEx.Combine(PathEx.LocalDir, "..");

        private static Guid UpdateGuid { get; } = Guid.NewGuid();

        private static string UpdateDir { get; } = PathEx.Combine(Path.GetTempPath(), $"Port-Able-{{{UpdateGuid}}}");

        private NetEx.AsyncTransfer Transferor { get; } = new NetEx.AsyncTransfer();

        private string UpdatePath { get; } = Path.Combine(UpdateDir, "Update.7z");

        private CounterInvestor<int> Counter { get; } = new CounterInvestor<int>();

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Check internet connection
            if (!(_ipv4 = NetEx.InternetIsAvailable()) && !(_ipv6 = NetEx.InternetIsAvailable(true)))
            {
                Environment.ExitCode = 1;
                Application.Exit();
                return;
            }

            // Get update infos from GitHub if enabled
            if (Ini.Read("Launcher", "UpdateChannel", 0) > 0)
            {
                if (!_ipv4 && _ipv6)
                {
                    Environment.ExitCode = 1;
                    Application.Exit();
                    return;
                }
                try
                {
                    var path = PathEx.AltCombine(Resources.GitProfileUri, Resources.GitSnapshotsPath, "Last.ini");
                    if (!NetEx.FileIsAvailable(path, 60000))
                        throw new PathNotFoundException(path);
                    var data = NetEx.Transfer.DownloadString(path);
                    if (string.IsNullOrWhiteSpace(data))
                        throw new ArgumentNullException(nameof(data));
                    _lastStamp = Ini.ReadOnly("Info", "LastStamp", data);
                    if (string.IsNullOrWhiteSpace(_lastStamp))
                        throw new ArgumentNullException(_lastStamp);
                    path = PathEx.AltCombine(Resources.GitProfileUri, Resources.GitSnapshotsPath, $"{_lastStamp}.ini");
                    if (!NetEx.FileIsAvailable(path, 60000))
                        throw new PathNotFoundException(path);
                    data = NetEx.Transfer.DownloadString(path);
                    if (string.IsNullOrWhiteSpace(data))
                        throw new ArgumentNullException(nameof(data));
                    _hashInfo = data;
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }

            // Get update infos if not already set
            if (string.IsNullOrWhiteSpace(_hashInfo))
            {
                // Get available download mirrors
                var dnsInfo = string.Empty;
                for (var i = 0; i < 6; i++)
                {
                    try
                    {
                        var path = string.Format(Resources.DnsUri, i);
                        if (!NetEx.FileIsAvailable(path, 20000))
                            throw new PathNotFoundException(path);
                        var data = NetEx.Transfer.DownloadString(path);
                        if (string.IsNullOrWhiteSpace(data))
                            throw new ArgumentNullException(nameof(data));
                        dnsInfo = data;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }
                    if (string.IsNullOrWhiteSpace(dnsInfo) && i < 5)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    break;
                }
                if (!string.IsNullOrWhiteSpace(dnsInfo))
                    foreach (var section in Ini.GetSections(dnsInfo))
                    {
                        var addr = Ini.Read(section, _ipv4 ? "addr" : "ipv6", dnsInfo);
                        if (string.IsNullOrEmpty(addr))
                            continue;
                        var domain = Ini.Read(section, "domain", dnsInfo);
                        if (string.IsNullOrEmpty(domain))
                            continue;
                        var ssl = Ini.ReadOnly(section, "ssl", false, dnsInfo);
                        domain = PathEx.AltCombine(ssl ? "https:" : "http:", domain);
                        if (!DownloadMirrors.ContainsEx(domain))
                            DownloadMirrors.Add(domain);
                    }
                if (DownloadMirrors.Count == 0)
                {
                    Environment.ExitCode = 1;
                    Application.Exit();
                    return;
                }

                // Get file hashes
                foreach (var mirror in DownloadMirrors)
                {
                    try
                    {
                        var path = PathEx.AltCombine(mirror, Resources.ReleasePath, "Last.ini");
                        if (!NetEx.FileIsAvailable(path, 60000))
                            throw new PathNotFoundException(path);
                        var data = NetEx.Transfer.DownloadString(path);
                        if (string.IsNullOrWhiteSpace(data))
                            throw new ArgumentNullException(nameof(data));
                        _lastFinalStamp = Ini.ReadOnly("Info", "LastStamp", data);
                        if (string.IsNullOrWhiteSpace(_lastFinalStamp))
                            throw new ArgumentNullException(nameof(_lastFinalStamp));
                        path = PathEx.AltCombine(mirror, Resources.ReleasePath, $"{_lastFinalStamp}.ini");
                        if (!NetEx.FileIsAvailable(path, 60000))
                            throw new PathNotFoundException(path);
                        data = NetEx.Transfer.DownloadString(path);
                        if (string.IsNullOrWhiteSpace(data))
                            throw new ArgumentNullException(nameof(data));
                        _hashInfo = data;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }
                    if (!string.IsNullOrWhiteSpace(_hashInfo))
                        break;
                }
            }
            if (string.IsNullOrWhiteSpace(_hashInfo))
            {
                Environment.ExitCode = 1;
                Application.Exit();
                return;
            }

            // Compare hashes
            var updateAvailable = false;
            try
            {
                foreach (var key in Ini.GetKeys("SHA256", _hashInfo))
                {
                    var file = Path.Combine(HomeDir, $"{key}.exe");
                    if (!File.Exists(file))
                        file = PathEx.Combine(PathEx.LocalDir, $"{key}.exe");
                    if (Ini.Read("SHA256", key, _hashInfo).EqualsEx(file.EncryptFile(ChecksumAlgorithms.Sha256)))
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
                    // Update changelog
                    if (DownloadMirrors.Count > 0)
                    {
                        var changes = string.Empty;
                        foreach (var mirror in DownloadMirrors)
                        {
                            var path = PathEx.AltCombine(mirror, Resources.ReleasePath, "ChangeLog.txt");
                            if (string.IsNullOrWhiteSpace(path))
                                continue;
                            if (!NetEx.FileIsAvailable(path, 60000))
                                continue;
                            changes = NetEx.Transfer.DownloadString(path);
                            if (!string.IsNullOrWhiteSpace(changes))
                                break;
                        }
                        if (!string.IsNullOrWhiteSpace(changes))
                        {
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
            string downloadPath = null;
            if (!string.IsNullOrWhiteSpace(_lastStamp))
                try
                {
                    downloadPath = PathEx.AltCombine(Resources.GitProfileUri, Resources.GitSnapshotsPath, $"{_lastStamp}.7z");
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
                        downloadPath = PathEx.AltCombine(mirror, Resources.ReleasePath, $"{_lastFinalStamp}.7z");
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
                    if (UpdatePath.ContainsEx(HomeDir))
                        throw new NotSupportedException();
                    var updDir = Path.GetDirectoryName(UpdateDir);
                    if (!string.IsNullOrEmpty(updDir))
                        foreach (var dir in Directory.GetDirectories(updDir, "Port-Able-{*}", SearchOption.TopDirectoryOnly))
                            Directory.Delete(dir, true);
                    if (!Directory.Exists(UpdateDir))
                        Directory.CreateDirectory(UpdateDir);
                    foreach (var file in new[]
                    {
                        "7z.dll",
                        "7zG.exe"
                    })
                    {
                        var path = PathEx.Combine(PathEx.LocalDir, "Helper\\7z");
                        if (Environment.Is64BitOperatingSystem)
                            path = Path.Combine(path, "x64");
                        path = Path.Combine(path, file);
                        File.Copy(path, Path.Combine(UpdateDir, file));
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex, true);
                    return;
                }
            try
            {
                Transferor.DownloadFile(downloadPath, UpdatePath);
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
            string helperPath = null;
            try
            {
                helperPath = Path.GetDirectoryName(UpdatePath);
                if (string.IsNullOrEmpty(helperPath))
                    return;
                helperPath = Path.Combine(helperPath, "UpdateHelper.bat");
                var helper = string.Format(Resources.BatchDummy, UpdateGuid, HomeDir.TrimEnd('\\'), Guid.NewGuid());
                File.WriteAllText(helperPath, helper);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            try
            {
                if (string.IsNullOrEmpty(helperPath))
                    throw new ArgumentNullException(nameof(helperPath));
                var lastStamp = _lastFinalStamp;
                if (string.IsNullOrWhiteSpace(lastStamp))
                    lastStamp = _lastStamp;
                if (!Ini.Read("MD5", lastStamp, _hashInfo).EqualsEx(UpdatePath.EncryptFile()))
                    throw new InvalidOperationException();
                ProcessEx.Start(helperPath, true, ProcessWindowStyle.Hidden);
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
                DirectoryEx.Delete(UpdateDir);
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
                foreach (var key in Ini.GetKeys("SHA256", _hashInfo))
                {
                    Process.Start(string.Format(Resources.VirusTotalUri, Ini.Read("SHA256", key, _hashInfo)));
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
            Process.Start(Resources.DevUri);
    }
}
