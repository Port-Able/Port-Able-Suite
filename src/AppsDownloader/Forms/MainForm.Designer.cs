namespace AppsDownloader.Forms
{
    sealed partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("S E A R C H", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Accessibility", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Education", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Development", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Office", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Internet", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Graphics and Pictures", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Music and Video", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("Security", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("Utilities", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("Games", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup12 = new System.Windows.Forms.ListViewGroup("Advanced", System.Windows.Forms.HorizontalAlignment.Left);
            this.appStatus = new System.Windows.Forms.Label();
            this.downloadReceivedLabel = new System.Windows.Forms.Label();
            this.urlStatus = new System.Windows.Forms.Label();
            this.appStatusLabel = new System.Windows.Forms.Label();
            this.fileStatusLabel = new System.Windows.Forms.Label();
            this.settingsBtn = new System.Windows.Forms.Panel();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.buttonAreaBorder = new System.Windows.Forms.Panel();
            this.urlStatusLabel = new System.Windows.Forms.Label();
            this.timeStatusLabel = new System.Windows.Forms.Label();
            this.buttonAreaPanel = new System.Windows.Forms.Panel();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.timeStatus = new System.Windows.Forms.Label();
            this.statusAreaLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.statusAreaRightPanel = new System.Windows.Forms.TableLayoutPanel();
            this.downloadReceived = new System.Windows.Forms.Label();
            this.downloadSpeed = new System.Windows.Forms.Label();
            this.downloadSpeedLabel = new System.Windows.Forms.Label();
            this.statusAreaLeftPanel = new System.Windows.Forms.TableLayoutPanel();
            this.fileStatus = new System.Windows.Forms.Label();
            this.statusAreaBorder = new System.Windows.Forms.Panel();
            this.downloadProgress = new System.Windows.Forms.Panel();
            this.downloadHandler = new System.Windows.Forms.Timer(this.components);
            this.downloadStarter = new System.Windows.Forms.Timer(this.components);
            this.statusAreaPanel = new System.Windows.Forms.Panel();
            this.appsList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.appMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.appMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItemSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.smallImageList = new System.Windows.Forms.ImageList(this.components);
            this.searchResultBlinker = new System.Windows.Forms.Timer(this.components);
            this.largeImageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonAreaPanel.SuspendLayout();
            this.statusAreaLayoutPanel.SuspendLayout();
            this.statusAreaRightPanel.SuspendLayout();
            this.statusAreaLeftPanel.SuspendLayout();
            this.statusAreaPanel.SuspendLayout();
            this.appMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // appStatus
            // 
            this.appStatus.BackColor = System.Drawing.Color.Transparent;
            this.appStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appStatus.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.appStatus.Location = new System.Drawing.Point(128, 0);
            this.appStatus.Name = "appStatus";
            this.appStatus.Size = new System.Drawing.Size(295, 20);
            this.appStatus.TabIndex = 0;
            this.appStatus.Text = "Example";
            this.appStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // downloadReceivedLabel
            // 
            this.downloadReceivedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadReceivedLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.downloadReceivedLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.downloadReceivedLabel.Location = new System.Drawing.Point(3, 0);
            this.downloadReceivedLabel.Name = "downloadReceivedLabel";
            this.downloadReceivedLabel.Size = new System.Drawing.Size(119, 20);
            this.downloadReceivedLabel.TabIndex = 0;
            this.downloadReceivedLabel.Text = "Downloaded:";
            this.downloadReceivedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // urlStatus
            // 
            this.urlStatus.BackColor = System.Drawing.Color.Transparent;
            this.urlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlStatus.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.urlStatus.Location = new System.Drawing.Point(128, 40);
            this.urlStatus.Name = "urlStatus";
            this.urlStatus.Size = new System.Drawing.Size(295, 24);
            this.urlStatus.TabIndex = 0;
            this.urlStatus.Text = "example.com";
            this.urlStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.urlStatus.DoubleClick += new System.EventHandler(this.UrlStatus_DoubleClick);
            // 
            // appStatusLabel
            // 
            this.appStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.appStatusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.appStatusLabel.Location = new System.Drawing.Point(3, 0);
            this.appStatusLabel.Name = "appStatusLabel";
            this.appStatusLabel.Size = new System.Drawing.Size(119, 20);
            this.appStatusLabel.TabIndex = 0;
            this.appStatusLabel.Text = "Application:";
            this.appStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileStatusLabel
            // 
            this.fileStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.fileStatusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.fileStatusLabel.Location = new System.Drawing.Point(3, 20);
            this.fileStatusLabel.Name = "fileStatusLabel";
            this.fileStatusLabel.Size = new System.Drawing.Size(119, 20);
            this.fileStatusLabel.TabIndex = 0;
            this.fileStatusLabel.Text = "File:";
            this.fileStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // settingsBtn
            // 
            this.settingsBtn.BackColor = System.Drawing.Color.Transparent;
            this.settingsBtn.BackgroundImage = global::AppsDownloader.Properties.Resources.Settings;
            this.settingsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.settingsBtn.Location = new System.Drawing.Point(10, 10);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(32, 32);
            this.settingsBtn.TabIndex = 6;
            this.settingsBtn.Click += new System.EventHandler(this.SettingsBtn_Click);
            this.settingsBtn.MouseEnter += new System.EventHandler(this.SettingBtn_MouseEnterLeave);
            this.settingsBtn.MouseLeave += new System.EventHandler(this.SettingBtn_MouseEnterLeave);
            // 
            // searchBox
            // 
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.Location = new System.Drawing.Point(104, 15);
            this.searchBox.Multiline = true;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(218, 22);
            this.searchBox.TabIndex = 5;
            this.searchBox.WordWrap = false;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            this.searchBox.Enter += new System.EventHandler(this.SearchBox_Enter);
            this.searchBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchBox_KeyPress);
            // 
            // buttonAreaBorder
            // 
            this.buttonAreaBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.buttonAreaBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonAreaBorder.Location = new System.Drawing.Point(0, 629);
            this.buttonAreaBorder.Name = "buttonAreaBorder";
            this.buttonAreaBorder.Size = new System.Drawing.Size(864, 1);
            this.buttonAreaBorder.TabIndex = 0;
            // 
            // urlStatusLabel
            // 
            this.urlStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.urlStatusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.urlStatusLabel.Location = new System.Drawing.Point(3, 40);
            this.urlStatusLabel.Name = "urlStatusLabel";
            this.urlStatusLabel.Size = new System.Drawing.Size(119, 24);
            this.urlStatusLabel.TabIndex = 0;
            this.urlStatusLabel.Text = "Source:";
            this.urlStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timeStatusLabel
            // 
            this.timeStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.timeStatusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.timeStatusLabel.Location = new System.Drawing.Point(3, 40);
            this.timeStatusLabel.Name = "timeStatusLabel";
            this.timeStatusLabel.Size = new System.Drawing.Size(119, 24);
            this.timeStatusLabel.TabIndex = 0;
            this.timeStatusLabel.Text = "Time Elapsed:";
            this.timeStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonAreaPanel
            // 
            this.buttonAreaPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(46)))));
            this.buttonAreaPanel.Controls.Add(this.settingsBtn);
            this.buttonAreaPanel.Controls.Add(this.cancelBtn);
            this.buttonAreaPanel.Controls.Add(this.searchBox);
            this.buttonAreaPanel.Controls.Add(this.startBtn);
            this.buttonAreaPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonAreaPanel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.buttonAreaPanel.Location = new System.Drawing.Point(0, 630);
            this.buttonAreaPanel.Name = "buttonAreaPanel";
            this.buttonAreaPanel.Size = new System.Drawing.Size(864, 52);
            this.buttonAreaPanel.TabIndex = 0;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.BackColor = System.Drawing.SystemColors.Control;
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cancelBtn.Location = new System.Drawing.Point(764, 12);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 28);
            this.cancelBtn.TabIndex = 101;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = false;
            this.cancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startBtn.BackColor = System.Drawing.SystemColors.Control;
            this.startBtn.Enabled = false;
            this.startBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.startBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.startBtn.Location = new System.Drawing.Point(668, 12);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 28);
            this.startBtn.TabIndex = 100;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = false;
            this.startBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // timeStatus
            // 
            this.timeStatus.BackColor = System.Drawing.Color.Transparent;
            this.timeStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeStatus.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.timeStatus.Location = new System.Drawing.Point(128, 40);
            this.timeStatus.Name = "timeStatus";
            this.timeStatus.Size = new System.Drawing.Size(295, 24);
            this.timeStatus.TabIndex = 0;
            this.timeStatus.Text = "00:00.000";
            this.timeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusAreaLayoutPanel
            // 
            this.statusAreaLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.statusAreaLayoutPanel.ColumnCount = 2;
            this.statusAreaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.statusAreaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.statusAreaLayoutPanel.Controls.Add(this.statusAreaRightPanel, 1, 0);
            this.statusAreaLayoutPanel.Controls.Add(this.statusAreaLeftPanel, 0, 0);
            this.statusAreaLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusAreaLayoutPanel.Location = new System.Drawing.Point(0, 8);
            this.statusAreaLayoutPanel.Name = "statusAreaLayoutPanel";
            this.statusAreaLayoutPanel.RowCount = 1;
            this.statusAreaLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.statusAreaLayoutPanel.Size = new System.Drawing.Size(864, 70);
            this.statusAreaLayoutPanel.TabIndex = 0;
            // 
            // statusAreaRightPanel
            // 
            this.statusAreaRightPanel.BackColor = System.Drawing.Color.Transparent;
            this.statusAreaRightPanel.ColumnCount = 2;
            this.statusAreaRightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.statusAreaRightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusAreaRightPanel.Controls.Add(this.timeStatus, 1, 2);
            this.statusAreaRightPanel.Controls.Add(this.timeStatusLabel, 0, 2);
            this.statusAreaRightPanel.Controls.Add(this.downloadReceived, 1, 0);
            this.statusAreaRightPanel.Controls.Add(this.downloadSpeed, 1, 1);
            this.statusAreaRightPanel.Controls.Add(this.downloadReceivedLabel, 0, 0);
            this.statusAreaRightPanel.Controls.Add(this.downloadSpeedLabel, 0, 1);
            this.statusAreaRightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusAreaRightPanel.Location = new System.Drawing.Point(435, 3);
            this.statusAreaRightPanel.Name = "statusAreaRightPanel";
            this.statusAreaRightPanel.RowCount = 3;
            this.statusAreaRightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaRightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaRightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaRightPanel.Size = new System.Drawing.Size(426, 64);
            this.statusAreaRightPanel.TabIndex = 0;
            // 
            // downloadReceived
            // 
            this.downloadReceived.BackColor = System.Drawing.Color.Transparent;
            this.downloadReceived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadReceived.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.downloadReceived.Location = new System.Drawing.Point(128, 0);
            this.downloadReceived.Name = "downloadReceived";
            this.downloadReceived.Size = new System.Drawing.Size(295, 20);
            this.downloadReceived.TabIndex = 0;
            this.downloadReceived.Text = "0.00 bytes / 0.00 bytes";
            this.downloadReceived.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // downloadSpeed
            // 
            this.downloadSpeed.BackColor = System.Drawing.Color.Transparent;
            this.downloadSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadSpeed.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.downloadSpeed.Location = new System.Drawing.Point(128, 20);
            this.downloadSpeed.Name = "downloadSpeed";
            this.downloadSpeed.Size = new System.Drawing.Size(295, 20);
            this.downloadSpeed.TabIndex = 0;
            this.downloadSpeed.Text = "0.00 bit/s";
            this.downloadSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // downloadSpeedLabel
            // 
            this.downloadSpeedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadSpeedLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.downloadSpeedLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.downloadSpeedLabel.Location = new System.Drawing.Point(3, 20);
            this.downloadSpeedLabel.Name = "downloadSpeedLabel";
            this.downloadSpeedLabel.Size = new System.Drawing.Size(119, 20);
            this.downloadSpeedLabel.TabIndex = 0;
            this.downloadSpeedLabel.Text = "Speed:";
            this.downloadSpeedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusAreaLeftPanel
            // 
            this.statusAreaLeftPanel.BackColor = System.Drawing.Color.Transparent;
            this.statusAreaLeftPanel.ColumnCount = 2;
            this.statusAreaLeftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.statusAreaLeftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusAreaLeftPanel.Controls.Add(this.urlStatusLabel, 0, 2);
            this.statusAreaLeftPanel.Controls.Add(this.urlStatus, 0, 2);
            this.statusAreaLeftPanel.Controls.Add(this.appStatus, 1, 0);
            this.statusAreaLeftPanel.Controls.Add(this.fileStatus, 1, 1);
            this.statusAreaLeftPanel.Controls.Add(this.appStatusLabel, 0, 0);
            this.statusAreaLeftPanel.Controls.Add(this.fileStatusLabel, 0, 1);
            this.statusAreaLeftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusAreaLeftPanel.Location = new System.Drawing.Point(3, 3);
            this.statusAreaLeftPanel.Name = "statusAreaLeftPanel";
            this.statusAreaLeftPanel.RowCount = 3;
            this.statusAreaLeftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaLeftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaLeftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaLeftPanel.Size = new System.Drawing.Size(426, 64);
            this.statusAreaLeftPanel.TabIndex = 0;
            // 
            // fileStatus
            // 
            this.fileStatus.BackColor = System.Drawing.Color.Transparent;
            this.fileStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileStatus.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.fileStatus.Location = new System.Drawing.Point(128, 20);
            this.fileStatus.Name = "fileStatus";
            this.fileStatus.Size = new System.Drawing.Size(295, 20);
            this.fileStatus.TabIndex = 0;
            this.fileStatus.Text = "example.7z";
            this.fileStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusAreaBorder
            // 
            this.statusAreaBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.statusAreaBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusAreaBorder.Location = new System.Drawing.Point(0, 682);
            this.statusAreaBorder.Name = "statusAreaBorder";
            this.statusAreaBorder.Size = new System.Drawing.Size(864, 1);
            this.statusAreaBorder.TabIndex = 0;
            // 
            // downloadProgress
            // 
            this.downloadProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadProgress.Location = new System.Drawing.Point(-2, -1);
            this.downloadProgress.Name = "downloadProgress";
            this.downloadProgress.Size = new System.Drawing.Size(868, 10);
            this.downloadProgress.TabIndex = 0;
            // 
            // downloadHandler
            // 
            this.downloadHandler.Interval = 10;
            this.downloadHandler.Tick += new System.EventHandler(this.DownloadHandler_Tick);
            // 
            // downloadStarter
            // 
            this.downloadStarter.Tick += new System.EventHandler(this.DownloadStarter_Tick);
            // 
            // statusAreaPanel
            // 
            this.statusAreaPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(46)))));
            this.statusAreaPanel.Controls.Add(this.downloadProgress);
            this.statusAreaPanel.Controls.Add(this.statusAreaLayoutPanel);
            this.statusAreaPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusAreaPanel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.statusAreaPanel.Location = new System.Drawing.Point(0, 683);
            this.statusAreaPanel.Name = "statusAreaPanel";
            this.statusAreaPanel.Size = new System.Drawing.Size(864, 78);
            this.statusAreaPanel.TabIndex = 0;
            this.statusAreaPanel.Visible = false;
            // 
            // appsList
            // 
            this.appsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.appsList.CheckBoxes = true;
            this.appsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.appsList.ContextMenuStrip = this.appMenu;
            this.appsList.FullRowSelect = true;
            listViewGroup1.Header = "S E A R C H";
            listViewGroup1.Name = "listViewGroup0";
            listViewGroup2.Header = "Accessibility";
            listViewGroup2.Name = "listViewGroup1";
            listViewGroup3.Header = "Education";
            listViewGroup3.Name = "listViewGroup2";
            listViewGroup4.Header = "Development";
            listViewGroup4.Name = "listViewGroup3";
            listViewGroup5.Header = "Office";
            listViewGroup5.Name = "listViewGroup4";
            listViewGroup6.Header = "Internet";
            listViewGroup6.Name = "listViewGroup5";
            listViewGroup7.Header = "Graphics and Pictures";
            listViewGroup7.Name = "listViewGroup6";
            listViewGroup8.Header = "Music and Video";
            listViewGroup8.Name = "listViewGroup7";
            listViewGroup9.Header = "Security";
            listViewGroup9.Name = "listViewGroup8";
            listViewGroup10.Header = "Utilities";
            listViewGroup10.Name = "listViewGroup9";
            listViewGroup11.Header = "Games";
            listViewGroup11.Name = "listViewGroup10";
            listViewGroup12.Header = "Advanced";
            listViewGroup12.Name = "listViewGroup11";
            this.appsList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8,
            listViewGroup9,
            listViewGroup10,
            listViewGroup11,
            listViewGroup12});
            this.appsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.appsList.HideSelection = false;
            this.appsList.LabelWrap = false;
            this.appsList.Location = new System.Drawing.Point(0, 0);
            this.appsList.MultiSelect = false;
            this.appsList.Name = "appsList";
            this.appsList.Size = new System.Drawing.Size(864, 761);
            this.appsList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.appsList.TabIndex = 0;
            this.appsList.TabStop = false;
            this.appsList.UseCompatibleStateImageBehavior = false;
            this.appsList.View = System.Windows.Forms.View.Details;
            this.appsList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.AppsList_ItemCheck);
            this.appsList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.AppsList_ItemChecked);
            this.appsList.Enter += new System.EventHandler(this.AppsList_Enter);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Application";
            this.columnHeader1.Width = 214;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Description";
            this.columnHeader2.Width = 234;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Version";
            this.columnHeader3.Width = 86;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Download Size";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Installed Size";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Source";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 120;
            // 
            // appMenu
            // 
            this.appMenu.BackColor = System.Drawing.SystemColors.Control;
            this.appMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appMenuItem1,
            this.appMenuItem2,
            this.appMenuItem3,
            this.appMenuItemSeparator1,
            this.appMenuItem4,
            this.toolStripSeparator1,
            this.appMenuItem5,
            this.appMenuItem6});
            this.appMenu.Name = "addMenu";
            this.appMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.appMenu.ShowItemToolTips = false;
            this.appMenu.Size = new System.Drawing.Size(256, 148);
            this.appMenu.Opening += new System.ComponentModel.CancelEventHandler(this.AppMenu_Opening);
            // 
            // appMenuItem1
            // 
            this.appMenuItem1.Image = global::AppsDownloader.Properties.Resources.Check;
            this.appMenuItem1.Name = "appMenuItem1";
            this.appMenuItem1.Size = new System.Drawing.Size(255, 22);
            this.appMenuItem1.Text = "Check";
            this.appMenuItem1.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem2
            // 
            this.appMenuItem2.Image = global::AppsDownloader.Properties.Resources.CheckAll;
            this.appMenuItem2.Name = "appMenuItem2";
            this.appMenuItem2.Size = new System.Drawing.Size(255, 22);
            this.appMenuItem2.Text = "Check all in &category";
            this.appMenuItem2.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem3
            // 
            this.appMenuItem3.Image = global::AppsDownloader.Properties.Resources.CheckAll;
            this.appMenuItem3.Name = "appMenuItem3";
            this.appMenuItem3.Size = new System.Drawing.Size(255, 22);
            this.appMenuItem3.Text = "Check &all";
            this.appMenuItem3.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItemSeparator1
            // 
            this.appMenuItemSeparator1.Name = "appMenuItemSeparator1";
            this.appMenuItemSeparator1.Size = new System.Drawing.Size(252, 6);
            // 
            // appMenuItem4
            // 
            this.appMenuItem4.Image = global::AppsDownloader.Properties.Resources.World;
            this.appMenuItem4.Name = "appMenuItem4";
            this.appMenuItem4.Size = new System.Drawing.Size(255, 22);
            this.appMenuItem4.Text = "Visit the &website";
            this.appMenuItem4.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(252, 6);
            // 
            // appMenuItem5
            // 
            this.appMenuItem5.Image = global::AppsDownloader.Properties.Resources.Bacterium;
            this.appMenuItem5.Name = "appMenuItem5";
            this.appMenuItem5.Size = new System.Drawing.Size(255, 22);
            this.appMenuItem5.Text = "Search for previous anti&virus scans";
            this.appMenuItem5.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem6
            // 
            this.appMenuItem6.Image = global::AppsDownloader.Properties.Resources.Info;
            this.appMenuItem6.Name = "appMenuItem6";
            this.appMenuItem6.Size = new System.Drawing.Size(255, 22);
            this.appMenuItem6.Text = "Show advanced &information";
            this.appMenuItem6.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // smallImageList
            // 
            this.smallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.smallImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.smallImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // searchResultBlinker
            // 
            this.searchResultBlinker.Interval = 300;
            this.searchResultBlinker.Tick += new System.EventHandler(this.SearchResultBlinker_Tick);
            // 
            // largeImageList
            // 
            this.largeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.largeImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.largeImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(864, 761);
            this.Controls.Add(this.buttonAreaBorder);
            this.Controls.Add(this.buttonAreaPanel);
            this.Controls.Add(this.statusAreaBorder);
            this.Controls.Add(this.statusAreaPanel);
            this.Controls.Add(this.appsList);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MinimumSize = new System.Drawing.Size(760, 125);
            this.Name = "MainForm";
            this.Opacity = 0D;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Apps Downloader";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.SystemColorsChanged += new System.EventHandler(this.MainForm_SystemColorsChanged);
            this.buttonAreaPanel.ResumeLayout(false);
            this.buttonAreaPanel.PerformLayout();
            this.statusAreaLayoutPanel.ResumeLayout(false);
            this.statusAreaRightPanel.ResumeLayout(false);
            this.statusAreaLeftPanel.ResumeLayout(false);
            this.statusAreaPanel.ResumeLayout(false);
            this.appMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label appStatus;
        private System.Windows.Forms.Label downloadReceivedLabel;
        private System.Windows.Forms.Label urlStatus;
        private System.Windows.Forms.Label appStatusLabel;
        private System.Windows.Forms.Label fileStatusLabel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Panel buttonAreaBorder;
        private System.Windows.Forms.Label urlStatusLabel;
        private System.Windows.Forms.Label timeStatusLabel;
        private System.Windows.Forms.Panel buttonAreaPanel;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Label timeStatus;
        private System.Windows.Forms.TableLayoutPanel statusAreaLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel statusAreaRightPanel;
        private System.Windows.Forms.Label downloadReceived;
        private System.Windows.Forms.Label downloadSpeed;
        private System.Windows.Forms.Label downloadSpeedLabel;
        private System.Windows.Forms.TableLayoutPanel statusAreaLeftPanel;
        private System.Windows.Forms.Label fileStatus;
        private System.Windows.Forms.Panel statusAreaBorder;
        private System.Windows.Forms.Panel downloadProgress;
        private System.Windows.Forms.Timer downloadHandler;
        private System.Windows.Forms.Timer downloadStarter;
        private System.Windows.Forms.Panel statusAreaPanel;
        private System.Windows.Forms.ListView appsList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ContextMenuStrip appMenu;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem3;
        private System.Windows.Forms.ToolStripSeparator appMenuItemSeparator1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem4;
        private System.Windows.Forms.ImageList smallImageList;
        private System.Windows.Forms.Timer searchResultBlinker;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem6;
        private System.Windows.Forms.ImageList largeImageList;
        private System.Windows.Forms.Panel settingsBtn;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem5;
    }
}

