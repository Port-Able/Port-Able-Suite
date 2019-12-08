namespace AppsLauncher.Windows
{
    partial class OpenWithForm
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
            this.appMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.appMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.runCmdLine = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.notifyIconDisabler = new System.ComponentModel.BackgroundWorker();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.addBtn = new System.Windows.Forms.Button();
            this.appsBox = new System.Windows.Forms.ComboBox();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.startBtnPanel = new System.Windows.Forms.Panel();
            this.settingsBtnPanel = new System.Windows.Forms.Panel();
            this.appMenu.SuspendLayout();
            this.startBtnPanel.SuspendLayout();
            this.settingsBtnPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // appMenu
            // 
            this.appMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appMenuItem1,
            this.appMenuItem2,
            this.toolStripSeparator2,
            this.appMenuItem3,
            this.appMenuItem4,
            this.toolStripSeparator3,
            this.appMenuItem7});
            this.appMenu.Name = "addMenu";
            this.appMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.appMenu.Size = new System.Drawing.Size(212, 126);
            this.appMenu.Opening += new System.ComponentModel.CancelEventHandler(this.AppMenuItem_Opening);
            // 
            // appMenuItem1
            // 
            this.appMenuItem1.Name = "appMenuItem1";
            this.appMenuItem1.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem1.Text = "Run";
            this.appMenuItem1.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem2
            // 
            this.appMenuItem2.Name = "appMenuItem2";
            this.appMenuItem2.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem2.Text = "Run as administrator";
            this.appMenuItem2.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(208, 6);
            // 
            // appMenuItem3
            // 
            this.appMenuItem3.Name = "appMenuItem3";
            this.appMenuItem3.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem3.Text = "Open app location";
            this.appMenuItem3.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem4
            // 
            this.appMenuItem4.Name = "appMenuItem4";
            this.appMenuItem4.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem4.Text = "Create a Desktop Shortcut";
            this.appMenuItem4.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(208, 6);
            // 
            // appMenuItem7
            // 
            this.appMenuItem7.Name = "appMenuItem7";
            this.appMenuItem7.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem7.Text = "Delete";
            this.appMenuItem7.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // runCmdLine
            // 
            this.runCmdLine.Interval = 1;
            this.runCmdLine.Tick += new System.EventHandler(this.RunCmdLine_Tick);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Click += new System.EventHandler(this.NotifyIcon_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // notifyIconDisabler
            // 
            this.notifyIconDisabler.WorkerSupportsCancellation = true;
            this.notifyIconDisabler.DoWork += new System.ComponentModel.DoWorkEventHandler(this.NotifyIconDisabler_DoWork);
            this.notifyIconDisabler.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.NotifyIconDisabler_RunWorkerCompleted);
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.searchBox.Location = new System.Drawing.Point(11, 44);
            this.searchBox.Multiline = true;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(282, 21);
            this.searchBox.TabIndex = 2;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            this.searchBox.Enter += new System.EventHandler(this.SearchBox_Enter);
            this.searchBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchBox_KeyPress);
            this.searchBox.Leave += new System.EventHandler(this.SearchBox_Leave);
            // 
            // addBtn
            // 
            this.addBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addBtn.BackColor = System.Drawing.SystemColors.Control;
            this.addBtn.FlatAppearance.BorderSize = 0;
            this.addBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.addBtn.Image = global::AppsLauncher.Properties.Resources.add_13;
            this.addBtn.Location = new System.Drawing.Point(272, 12);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(21, 21);
            this.addBtn.TabIndex = 1;
            this.addBtn.UseVisualStyleBackColor = false;
            this.addBtn.Click += new System.EventHandler(this.AddBtn_Click);
            this.addBtn.MouseEnter += new System.EventHandler(this.AddBtn_MouseEnter);
            this.addBtn.MouseLeave += new System.EventHandler(this.AddBtn_MouseLeave);
            // 
            // appsBox
            // 
            this.appsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.appsBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.appsBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appsBox.FormattingEnabled = true;
            this.appsBox.Location = new System.Drawing.Point(11, 12);
            this.appsBox.Name = "appsBox";
            this.appsBox.Size = new System.Drawing.Size(258, 21);
            this.appsBox.TabIndex = 0;
            this.appsBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AppsBox_KeyPress);
            // 
            // settingsBtn
            // 
            this.settingsBtn.BackColor = System.Drawing.SystemColors.Control;
            this.settingsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsBtn.FlatAppearance.BorderSize = 0;
            this.settingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.settingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsBtn.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.settingsBtn.Location = new System.Drawing.Point(0, 0);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(136, 21);
            this.settingsBtn.TabIndex = 4;
            this.settingsBtn.Text = "Settings";
            this.settingsBtn.UseVisualStyleBackColor = false;
            this.settingsBtn.Click += new System.EventHandler(this.SettingsBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.BackColor = System.Drawing.SystemColors.Control;
            this.startBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.startBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startBtn.FlatAppearance.BorderSize = 0;
            this.startBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.startBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startBtn.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.startBtn.Location = new System.Drawing.Point(0, 0);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(136, 21);
            this.startBtn.TabIndex = 3;
            this.startBtn.Text = "Run";
            this.startBtn.UseVisualStyleBackColor = false;
            this.startBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // startBtnPanel
            // 
            this.startBtnPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startBtnPanel.BackColor = System.Drawing.Color.Transparent;
            this.startBtnPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.startBtnPanel.Controls.Add(this.startBtn);
            this.startBtnPanel.Location = new System.Drawing.Point(11, 78);
            this.startBtnPanel.Name = "startBtnPanel";
            this.startBtnPanel.Size = new System.Drawing.Size(138, 23);
            this.startBtnPanel.TabIndex = 3;
            // 
            // settingsBtnPanel
            // 
            this.settingsBtnPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsBtnPanel.BackColor = System.Drawing.Color.Transparent;
            this.settingsBtnPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.settingsBtnPanel.Controls.Add(this.settingsBtn);
            this.settingsBtnPanel.Location = new System.Drawing.Point(155, 78);
            this.settingsBtnPanel.Name = "settingsBtnPanel";
            this.settingsBtnPanel.Size = new System.Drawing.Size(138, 23);
            this.settingsBtnPanel.TabIndex = 4;
            // 
            // OpenWithForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(304, 117);
            this.Controls.Add(this.settingsBtnPanel);
            this.Controls.Add(this.startBtnPanel);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.addBtn);
            this.Controls.Add(this.appsBox);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(310, 146);
            this.Name = "OpenWithForm";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open with:";
            this.TopMost = true;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.OpenWithForm_HelpButtonClicked);
            this.Activated += new System.EventHandler(this.OpenWithForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpenWithForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OpenWithForm_FormClosed);
            this.Load += new System.EventHandler(this.OpenWithForm_Load);
            this.Shown += new System.EventHandler(this.OpenWithForm_Shown);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OpenWithForm_DragEnter);
            this.appMenu.ResumeLayout(false);
            this.startBtnPanel.ResumeLayout(false);
            this.settingsBtnPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer runCmdLine;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolTip toolTip;
        private System.ComponentModel.BackgroundWorker notifyIconDisabler;
        private System.Windows.Forms.ContextMenuStrip appMenu;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem7;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.ComboBox appsBox;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Panel startBtnPanel;
        private System.Windows.Forms.Panel settingsBtnPanel;
    }
}

