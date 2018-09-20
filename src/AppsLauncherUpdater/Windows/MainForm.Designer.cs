namespace Updater.Windows
{
    partial class MainForm
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
            this.changeLogPanel = new System.Windows.Forms.Panel();
            this.changeLog = new System.Windows.Forms.RichTextBox();
            this.logoBox = new System.Windows.Forms.PictureBox();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.statusTableLayoutPanelBorder = new System.Windows.Forms.Panel();
            this.statusTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.progressLabel = new System.Windows.Forms.Label();
            this.statusBarPanel = new System.Windows.Forms.Panel();
            this.statusBar = new System.Windows.Forms.ProgressBar();
            this.virusTotalBtn = new System.Windows.Forms.Label();
            this.webBtn = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.updateBtn = new System.Windows.Forms.Button();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkDownload = new System.Windows.Forms.Timer(this.components);
            this.buttonPanelBorder = new System.Windows.Forms.Panel();
            this.changeLogPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.statusTableLayoutPanel.SuspendLayout();
            this.statusBarPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // changeLogPanel
            // 
            this.changeLogPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.changeLogPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(64)))));
            this.changeLogPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.changeLogPanel.Controls.Add(this.changeLog);
            this.changeLogPanel.Font = new System.Drawing.Font("Calibri", 9F);
            this.changeLogPanel.ForeColor = System.Drawing.Color.White;
            this.changeLogPanel.Location = new System.Drawing.Point(122, 11);
            this.changeLogPanel.Name = "changeLogPanel";
            this.changeLogPanel.Size = new System.Drawing.Size(526, 496);
            this.changeLogPanel.TabIndex = 0;
            // 
            // changeLog
            // 
            this.changeLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(64)))));
            this.changeLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.changeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.changeLog.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changeLog.ForeColor = System.Drawing.Color.White;
            this.changeLog.Location = new System.Drawing.Point(0, 0);
            this.changeLog.Name = "changeLog";
            this.changeLog.ReadOnly = true;
            this.changeLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.changeLog.Size = new System.Drawing.Size(524, 494);
            this.changeLog.TabIndex = 0;
            this.changeLog.TabStop = false;
            this.changeLog.Text = "Commits:\nhttps://github.com/Port-Able/Port-Able-Suite/commits/master";
            this.changeLog.WordWrap = false;
            this.changeLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.ChangeLog_LinkClicked);
            this.changeLog.Click += new System.EventHandler(this.ChangeLog_HideCaret);
            this.changeLog.EnabledChanged += new System.EventHandler(this.ChangeLog_HideCaret);
            this.changeLog.SizeChanged += new System.EventHandler(this.ChangeLog_HideCaret);
            this.changeLog.VisibleChanged += new System.EventHandler(this.ChangeLog_HideCaret);
            this.changeLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChangeLog_HideCaret);
            // 
            // logoBox
            // 
            this.logoBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.logoBox.BackColor = System.Drawing.Color.Transparent;
            this.logoBox.Location = new System.Drawing.Point(12, 11);
            this.logoBox.Name = "logoBox";
            this.logoBox.Size = new System.Drawing.Size(100, 496);
            this.logoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoBox.TabIndex = 2;
            this.logoBox.TabStop = false;
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.buttonPanel.BackgroundImage = global::Updater.Properties.Resources.diagonal_pattern;
            this.buttonPanel.Controls.Add(this.statusTableLayoutPanelBorder);
            this.buttonPanel.Controls.Add(this.statusTableLayoutPanel);
            this.buttonPanel.Controls.Add(this.cancelBtn);
            this.buttonPanel.Controls.Add(this.updateBtn);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 523);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(660, 80);
            this.buttonPanel.TabIndex = 3;
            // 
            // statusTableLayoutPanelBorder
            // 
            this.statusTableLayoutPanelBorder.BackColor = System.Drawing.Color.Black;
            this.statusTableLayoutPanelBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusTableLayoutPanelBorder.Location = new System.Drawing.Point(0, 58);
            this.statusTableLayoutPanelBorder.Name = "statusTableLayoutPanelBorder";
            this.statusTableLayoutPanelBorder.Size = new System.Drawing.Size(660, 1);
            this.statusTableLayoutPanelBorder.TabIndex = 3;
            // 
            // statusTableLayoutPanel
            // 
            this.statusTableLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.statusTableLayoutPanel.ColumnCount = 5;
            this.statusTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.statusTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.statusTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.statusTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.statusTableLayoutPanel.Controls.Add(this.progressLabel, 0, 0);
            this.statusTableLayoutPanel.Controls.Add(this.statusBarPanel, 1, 0);
            this.statusTableLayoutPanel.Controls.Add(this.virusTotalBtn, 3, 0);
            this.statusTableLayoutPanel.Controls.Add(this.webBtn, 4, 0);
            this.statusTableLayoutPanel.Controls.Add(this.statusLabel, 2, 0);
            this.statusTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusTableLayoutPanel.Location = new System.Drawing.Point(0, 59);
            this.statusTableLayoutPanel.Name = "statusTableLayoutPanel";
            this.statusTableLayoutPanel.RowCount = 1;
            this.statusTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusTableLayoutPanel.Size = new System.Drawing.Size(660, 21);
            this.statusTableLayoutPanel.TabIndex = 2;
            // 
            // progressLabel
            // 
            this.progressLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.progressLabel.Location = new System.Drawing.Point(3, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(54, 21);
            this.progressLabel.TabIndex = 1;
            this.progressLabel.Text = "Progress:";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.progressLabel.TextChanged += new System.EventHandler(this.ProgressLabel_TextChanged);
            // 
            // statusBarPanel
            // 
            this.statusBarPanel.Controls.Add(this.statusBar);
            this.statusBarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusBarPanel.Location = new System.Drawing.Point(63, 3);
            this.statusBarPanel.Name = "statusBarPanel";
            this.statusBarPanel.Size = new System.Drawing.Size(134, 15);
            this.statusBarPanel.TabIndex = 2;
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 5);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(133, 8);
            this.statusBar.TabIndex = 2;
            // 
            // virusTotalBtn
            // 
            this.virusTotalBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.virusTotalBtn.ForeColor = System.Drawing.Color.Gray;
            this.virusTotalBtn.Location = new System.Drawing.Point(476, 0);
            this.virusTotalBtn.Name = "virusTotalBtn";
            this.virusTotalBtn.Size = new System.Drawing.Size(99, 21);
            this.virusTotalBtn.TabIndex = 3;
            this.virusTotalBtn.Text = "www.virustotal.com";
            this.virusTotalBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.virusTotalBtn.Click += new System.EventHandler(this.VirusTotalBtn_Click);
            // 
            // webBtn
            // 
            this.webBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBtn.ForeColor = System.Drawing.Color.Gray;
            this.webBtn.Location = new System.Drawing.Point(581, 0);
            this.webBtn.Name = "webBtn";
            this.webBtn.Size = new System.Drawing.Size(76, 21);
            this.webBtn.TabIndex = 4;
            this.webBtn.Text = "www.port-a.de";
            this.webBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.webBtn.Click += new System.EventHandler(this.WebBtn_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.statusLabel.Location = new System.Drawing.Point(203, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(267, 21);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "Waiting for confirmation...";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.BackColor = System.Drawing.SystemColors.Control;
            this.cancelBtn.FlatAppearance.BorderSize = 0;
            this.cancelBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelBtn.Location = new System.Drawing.Point(551, 17);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 1;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = false;
            this.cancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // updateBtn
            // 
            this.updateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateBtn.BackColor = System.Drawing.SystemColors.Control;
            this.updateBtn.FlatAppearance.BorderSize = 0;
            this.updateBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SteelBlue;
            this.updateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateBtn.Location = new System.Drawing.Point(455, 17);
            this.updateBtn.Name = "updateBtn";
            this.updateBtn.Size = new System.Drawing.Size(75, 23);
            this.updateBtn.TabIndex = 0;
            this.updateBtn.Text = "Update";
            this.updateBtn.UseVisualStyleBackColor = false;
            this.updateBtn.Click += new System.EventHandler(this.UpdateBtn_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(55, 17);
            this.toolStripStatusLabel1.Text = "Progress:";
            // 
            // checkDownload
            // 
            this.checkDownload.Interval = 10;
            this.checkDownload.Tick += new System.EventHandler(this.CheckDownload_Tick);
            // 
            // buttonPanelBorder
            // 
            this.buttonPanelBorder.BackColor = System.Drawing.Color.Black;
            this.buttonPanelBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanelBorder.Location = new System.Drawing.Point(0, 522);
            this.buttonPanelBorder.Name = "buttonPanelBorder";
            this.buttonPanelBorder.Size = new System.Drawing.Size(660, 1);
            this.buttonPanelBorder.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(660, 603);
            this.Controls.Add(this.buttonPanelBorder);
            this.Controls.Add(this.changeLogPanel);
            this.Controls.Add(this.logoBox);
            this.Controls.Add(this.buttonPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(666, 632);
            this.Name = "MainForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Port-Able Suite Updater";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);

        }

        #endregion
        private System.Windows.Forms.PictureBox logoBox;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button updateBtn;
        private System.Windows.Forms.TableLayoutPanel statusTableLayoutPanel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Panel statusTableLayoutPanelBorder;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.ProgressBar statusBar;
        private System.Windows.Forms.Panel statusBarPanel;
        private System.Windows.Forms.Label virusTotalBtn;
        private System.Windows.Forms.Label webBtn;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Timer checkDownload;
        private System.Windows.Forms.RichTextBox changeLog;
        private System.Windows.Forms.Panel changeLogPanel;
        private System.Windows.Forms.Panel buttonPanelBorder;
    }
}

