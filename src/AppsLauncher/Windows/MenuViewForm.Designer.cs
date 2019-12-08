namespace AppsLauncher.Windows
{
    sealed partial class MenuViewForm
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
            this.appMenuItemSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItemSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItemSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.appsListView = new System.Windows.Forms.ListView();
            this.appsListViewPanel = new System.Windows.Forms.Panel();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.title = new System.Windows.Forms.Label();
            this.logoBox = new System.Windows.Forms.PictureBox();
            this.appsCount = new System.Windows.Forms.Label();
            this.aboutBtn = new System.Windows.Forms.PictureBox();
            this.profileBtn = new System.Windows.Forms.PictureBox();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.downloadBtnPanel = new System.Windows.Forms.Panel();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.settingsBtnPanel = new System.Windows.Forms.Panel();
            this.appMenu.SuspendLayout();
            this.appsListViewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aboutBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.profileBtn)).BeginInit();
            this.downloadBtnPanel.SuspendLayout();
            this.settingsBtnPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // appMenu
            // 
            this.appMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appMenuItem1,
            this.appMenuItem2,
            this.appMenuItemSeparator1,
            this.appMenuItem3,
            this.appMenuItem4,
            this.appMenuItemSeparator2,
            this.appMenuItem6,
            this.appMenuItem7,
            this.appMenuItemSeparator3,
            this.appMenuItem8});
            this.appMenu.Name = "addMenu";
            this.appMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.appMenu.ShowItemToolTips = false;
            this.appMenu.Size = new System.Drawing.Size(212, 176);
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
            // appMenuItemSeparator1
            // 
            this.appMenuItemSeparator1.Name = "appMenuItemSeparator1";
            this.appMenuItemSeparator1.Size = new System.Drawing.Size(208, 6);
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
            // appMenuItemSeparator2
            // 
            this.appMenuItemSeparator2.Name = "appMenuItemSeparator2";
            this.appMenuItemSeparator2.Size = new System.Drawing.Size(208, 6);
            // 
            // appMenuItem6
            // 
            this.appMenuItem6.Name = "appMenuItem6";
            this.appMenuItem6.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem6.Text = "Rename";
            this.appMenuItem6.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem7
            // 
            this.appMenuItem7.Name = "appMenuItem7";
            this.appMenuItem7.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem7.Text = "Delete";
            this.appMenuItem7.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItemSeparator3
            // 
            this.appMenuItemSeparator3.Name = "appMenuItemSeparator3";
            this.appMenuItemSeparator3.Size = new System.Drawing.Size(208, 6);
            // 
            // appMenuItem8
            // 
            this.appMenuItem8.Name = "appMenuItem8";
            this.appMenuItem8.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem8.Text = "Options";
            this.appMenuItem8.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // appsListView
            // 
            this.appsListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.appsListView.BackColor = System.Drawing.SystemColors.Window;
            this.appsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.appsListView.ContextMenuStrip = this.appMenu;
            this.appsListView.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.appsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appsListView.Font = new System.Drawing.Font("Calibri", 9F);
            this.appsListView.ForeColor = System.Drawing.SystemColors.WindowText;
            this.appsListView.FullRowSelect = true;
            this.appsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.appsListView.HideSelection = false;
            this.appsListView.LabelWrap = false;
            this.appsListView.Location = new System.Drawing.Point(4, 3);
            this.appsListView.MultiSelect = false;
            this.appsListView.Name = "appsListView";
            this.appsListView.ShowGroups = false;
            this.appsListView.Size = new System.Drawing.Size(192, 275);
            this.appsListView.TabIndex = 0;
            this.appsListView.TileSize = new System.Drawing.Size(128, 30);
            this.appsListView.UseCompatibleStateImageBehavior = false;
            this.appsListView.View = System.Windows.Forms.View.List;
            this.appsListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.AppsListView_AfterLabelEdit);
            this.appsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AppsListView_KeyDown);
            this.appsListView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AppsListView_KeyPress);
            this.appsListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AppsListView_MouseClick);
            this.appsListView.MouseEnter += new System.EventHandler(this.AppsListView_MouseEnter);
            this.appsListView.MouseLeave += new System.EventHandler(this.AppsListView_MouseLeave);
            this.appsListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AppsListView_MouseMove);
            // 
            // appsListViewPanel
            // 
            this.appsListViewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appsListViewPanel.BackColor = System.Drawing.SystemColors.Window;
            this.appsListViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.appsListViewPanel.Controls.Add(this.appsListView);
            this.appsListViewPanel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.appsListViewPanel.Location = new System.Drawing.Point(5, 6);
            this.appsListViewPanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.appsListViewPanel.Name = "appsListViewPanel";
            this.appsListViewPanel.Padding = new System.Windows.Forms.Padding(4, 3, 0, 4);
            this.appsListViewPanel.Size = new System.Drawing.Size(198, 284);
            this.appsListViewPanel.TabIndex = 10;
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.searchBox.Location = new System.Drawing.Point(5, 294);
            this.searchBox.MaximumSize = new System.Drawing.Size(256, 21);
            this.searchBox.MinimumSize = new System.Drawing.Size(192, 21);
            this.searchBox.Multiline = true;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(198, 21);
            this.searchBox.TabIndex = 3;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            this.searchBox.Enter += new System.EventHandler(this.SearchBox_Enter);
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchBox_KeyDown);
            this.searchBox.Leave += new System.EventHandler(this.SearchBox_Leave);
            // 
            // title
            // 
            this.title.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.title.BackColor = System.Drawing.Color.Transparent;
            this.title.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.title.Location = new System.Drawing.Point(209, 11);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(110, 19);
            this.title.TabIndex = 9;
            this.title.Text = "Port-Able";
            this.title.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // logoBox
            // 
            this.logoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logoBox.BackColor = System.Drawing.Color.Transparent;
            this.logoBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.logoBox.Location = new System.Drawing.Point(206, 33);
            this.logoBox.Name = "logoBox";
            this.logoBox.Size = new System.Drawing.Size(116, 64);
            this.logoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoBox.TabIndex = 5;
            this.logoBox.TabStop = false;
            // 
            // appsCount
            // 
            this.appsCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.appsCount.BackColor = System.Drawing.Color.Transparent;
            this.appsCount.Font = new System.Drawing.Font("Calibri", 8.25F);
            this.appsCount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.appsCount.Location = new System.Drawing.Point(206, 100);
            this.appsCount.Name = "appsCount";
            this.appsCount.Size = new System.Drawing.Size(116, 13);
            this.appsCount.TabIndex = 2;
            this.appsCount.Text = "0 apps found!";
            this.appsCount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // aboutBtn
            // 
            this.aboutBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aboutBtn.BackColor = System.Drawing.Color.Transparent;
            this.aboutBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.aboutBtn.Cursor = System.Windows.Forms.Cursors.Help;
            this.aboutBtn.Location = new System.Drawing.Point(321, 3);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(23, 23);
            this.aboutBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.aboutBtn.TabIndex = 6;
            this.aboutBtn.TabStop = false;
            this.aboutBtn.Click += new System.EventHandler(this.AboutBtn_Click);
            this.aboutBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.aboutBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // profileBtn
            // 
            this.profileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.profileBtn.BackColor = System.Drawing.Color.Transparent;
            this.profileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.profileBtn.Location = new System.Drawing.Point(318, 212);
            this.profileBtn.Name = "profileBtn";
            this.profileBtn.Size = new System.Drawing.Size(20, 20);
            this.profileBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.profileBtn.TabIndex = 8;
            this.profileBtn.TabStop = false;
            this.profileBtn.Click += new System.EventHandler(this.ProfileBtn_Click);
            this.profileBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.profileBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // downloadBtn
            // 
            this.downloadBtn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.downloadBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadBtn.FlatAppearance.BorderSize = 0;
            this.downloadBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.downloadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.downloadBtn.Location = new System.Drawing.Point(0, 0);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Size = new System.Drawing.Size(132, 22);
            this.downloadBtn.TabIndex = 2;
            this.downloadBtn.TabStop = false;
            this.downloadBtn.Text = "Get More";
            this.downloadBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.downloadBtn.UseVisualStyleBackColor = false;
            this.downloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            this.downloadBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.downloadBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // downloadBtnPanel
            // 
            this.downloadBtnPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadBtnPanel.BackColor = System.Drawing.Color.Transparent;
            this.downloadBtnPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadBtnPanel.Controls.Add(this.downloadBtn);
            this.downloadBtnPanel.Location = new System.Drawing.Point(207, 238);
            this.downloadBtnPanel.Name = "downloadBtnPanel";
            this.downloadBtnPanel.Size = new System.Drawing.Size(134, 24);
            this.downloadBtnPanel.TabIndex = 2;
            // 
            // settingsBtn
            // 
            this.settingsBtn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.settingsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsBtn.FlatAppearance.BorderSize = 0;
            this.settingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.settingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsBtn.Location = new System.Drawing.Point(0, 0);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(132, 22);
            this.settingsBtn.TabIndex = 3;
            this.settingsBtn.TabStop = false;
            this.settingsBtn.Text = "Setting";
            this.settingsBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.settingsBtn.UseVisualStyleBackColor = false;
            this.settingsBtn.Click += new System.EventHandler(this.SettingsBtn_Click);
            this.settingsBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.settingsBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // settingsBtnPanel
            // 
            this.settingsBtnPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsBtnPanel.BackColor = System.Drawing.Color.Transparent;
            this.settingsBtnPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.settingsBtnPanel.Controls.Add(this.settingsBtn);
            this.settingsBtnPanel.Location = new System.Drawing.Point(207, 266);
            this.settingsBtnPanel.Name = "settingsBtnPanel";
            this.settingsBtnPanel.Size = new System.Drawing.Size(134, 24);
            this.settingsBtnPanel.TabIndex = 3;
            // 
            // MenuViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(346, 320);
            this.Controls.Add(this.appsListViewPanel);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.title);
            this.Controls.Add(this.logoBox);
            this.Controls.Add(this.appsCount);
            this.Controls.Add(this.aboutBtn);
            this.Controls.Add(this.profileBtn);
            this.Controls.Add(this.downloadBtnPanel);
            this.Controls.Add(this.settingsBtnPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(346, 320);
            this.Name = "MenuViewForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Apps Launcher";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.MenuViewForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuViewForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MenuViewForm_FormClosed);
            this.Load += new System.EventHandler(this.MenuViewForm_Load);
            this.Shown += new System.EventHandler(this.MenuViewForm_Shown);
            this.ResizeBegin += new System.EventHandler(this.MenuViewForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MenuViewForm_ResizeEnd);
            this.Resize += new System.EventHandler(this.MenuViewForm_Resize);
            this.appMenu.ResumeLayout(false);
            this.appsListViewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aboutBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.profileBtn)).EndInit();
            this.downloadBtnPanel.ResumeLayout(false);
            this.settingsBtnPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip appMenu;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem2;
        private System.Windows.Forms.ToolStripSeparator appMenuItemSeparator1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem4;
        private System.Windows.Forms.ToolStripSeparator appMenuItemSeparator2;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem7;
        private System.Windows.Forms.ToolStripSeparator appMenuItemSeparator3;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem8;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView appsListView;
        private System.Windows.Forms.Panel appsListViewPanel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.PictureBox logoBox;
        private System.Windows.Forms.Label appsCount;
        private System.Windows.Forms.PictureBox aboutBtn;
        private System.Windows.Forms.PictureBox profileBtn;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.Panel downloadBtnPanel;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.Panel settingsBtnPanel;
    }
}