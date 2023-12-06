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
            this.aboutBtn = new System.Windows.Forms.PictureBox();
            this.profileBtn = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.settingsBtn = new System.Windows.Forms.PictureBox();
            this.downloadBtn = new System.Windows.Forms.PictureBox();
            this.appMenu.SuspendLayout();
            this.appsListViewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aboutBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.profileBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.downloadBtn)).BeginInit();
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
            this.appMenu.Size = new System.Drawing.Size(212, 198);
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
            this.appMenuItem2.Image = global::AppsLauncher.Properties.Resources.ShieldExclamation;
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
            this.appMenuItem3.Image = global::AppsLauncher.Properties.Resources.Folder;
            this.appMenuItem3.Name = "appMenuItem3";
            this.appMenuItem3.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem3.Text = "Open app location";
            this.appMenuItem3.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem4
            // 
            this.appMenuItem4.Image = global::AppsLauncher.Properties.Resources.AppDesktop;
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
            this.appMenuItem6.Image = global::AppsLauncher.Properties.Resources.AppRename;
            this.appMenuItem6.Name = "appMenuItem6";
            this.appMenuItem6.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem6.Text = "Rename";
            this.appMenuItem6.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem7
            // 
            this.appMenuItem7.Image = global::AppsLauncher.Properties.Resources.AppDelete;
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
            this.appMenuItem8.Image = global::AppsLauncher.Properties.Resources.Settings;
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
            this.appsListView.MinimumSize = new System.Drawing.Size(192, 275);
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
            this.appsListViewPanel.MinimumSize = new System.Drawing.Size(198, 284);
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
            this.searchBox.MinimumSize = new System.Drawing.Size(198, 21);
            this.searchBox.Multiline = true;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(198, 21);
            this.searchBox.TabIndex = 3;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            this.searchBox.Enter += new System.EventHandler(this.SearchBox_Enter);
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchBox_KeyDown);
            this.searchBox.Leave += new System.EventHandler(this.SearchBox_Leave);
            // 
            // aboutBtn
            // 
            this.aboutBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aboutBtn.BackColor = System.Drawing.Color.Transparent;
            this.aboutBtn.BackgroundImage = global::AppsLauncher.Properties.Resources.Interrogation;
            this.aboutBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.aboutBtn.Cursor = System.Windows.Forms.Cursors.Help;
            this.aboutBtn.Location = new System.Drawing.Point(227, 10);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(16, 16);
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
            this.profileBtn.BackgroundImage = global::AppsLauncher.Properties.Resources.User;
            this.profileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.profileBtn.Location = new System.Drawing.Point(211, 169);
            this.profileBtn.Name = "profileBtn";
            this.profileBtn.Size = new System.Drawing.Size(32, 32);
            this.profileBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.profileBtn.TabIndex = 8;
            this.profileBtn.TabStop = false;
            this.profileBtn.Click += new System.EventHandler(this.ProfileBtn_Click);
            this.profileBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.profileBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // settingsBtn
            // 
            this.settingsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsBtn.BackColor = System.Drawing.Color.Transparent;
            this.settingsBtn.BackgroundImage = global::AppsLauncher.Properties.Resources.Settings;
            this.settingsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.settingsBtn.Location = new System.Drawing.Point(211, 253);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(32, 32);
            this.settingsBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.settingsBtn.TabIndex = 12;
            this.settingsBtn.TabStop = false;
            this.settingsBtn.Click += new System.EventHandler(this.SettingsBtn_Click);
            this.settingsBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.settingsBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // downloadBtn
            // 
            this.downloadBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadBtn.BackColor = System.Drawing.Color.Transparent;
            this.downloadBtn.BackgroundImage = global::AppsLauncher.Properties.Resources.AppsAdd;
            this.downloadBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.downloadBtn.Location = new System.Drawing.Point(212, 212);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Size = new System.Drawing.Size(30, 30);
            this.downloadBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.downloadBtn.TabIndex = 11;
            this.downloadBtn.TabStop = false;
            this.downloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            this.downloadBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.downloadBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // MenuViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(252, 320);
            this.Controls.Add(this.settingsBtn);
            this.Controls.Add(this.downloadBtn);
            this.Controls.Add(this.appsListViewPanel);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.aboutBtn);
            this.Controls.Add(this.profileBtn);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(252, 320);
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
            ((System.ComponentModel.ISupportInitialize)(this.aboutBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.profileBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.downloadBtn)).EndInit();
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
        private System.Windows.Forms.PictureBox aboutBtn;
        private System.Windows.Forms.PictureBox profileBtn;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox downloadBtn;
        private System.Windows.Forms.PictureBox settingsBtn;
    }
}