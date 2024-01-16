namespace AppsLauncher.Forms
{
    using System.ComponentModel;

    partial class SettingsForm
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
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Item 2");
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.restoreFileTypesBtn = new System.Windows.Forms.Button();
            this.fileTypes = new System.Windows.Forms.TextBox();
            this.fileTypesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fileTypesMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileTypesMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileTypesMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.startArgsFirst = new System.Windows.Forms.TextBox();
            this.startArgsLast = new System.Windows.Forms.TextBox();
            this.startArgsDefaultLabel = new System.Windows.Forms.Label();
            this.locationBtn = new System.Windows.Forms.Button();
            this.appDirs = new System.Windows.Forms.TextBox();
            this.langConfirmedCheck = new System.Windows.Forms.CheckBox();
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.sortArgPathsCheck = new System.Windows.Forms.CheckBox();
            this.fileTypesTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.fileTypesButtonFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.associateBtn = new System.Windows.Forms.Button();
            this.startArgsTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.runAsAdminCheck = new System.Windows.Forms.CheckBox();
            this.noUpdatesCheck = new System.Windows.Forms.CheckBox();
            this.appsBox = new System.Windows.Forms.ComboBox();
            this.fileTypesLabel = new System.Windows.Forms.Label();
            this.appsBoxLabel = new System.Windows.Forms.Label();
            this.addArgsLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.previewCaption = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.showInTaskbarCheck = new System.Windows.Forms.CheckBox();
            this.showCaptionCheck = new System.Windows.Forms.CheckBox();
            this.viewStyle = new System.Windows.Forms.ComboBox();
            this.viewStyleLable = new System.Windows.Forms.Label();
            this.bgLayout = new System.Windows.Forms.ComboBox();
            this.bgLayoutLabel = new System.Windows.Forms.Label();
            this.controlColorPanel = new System.Windows.Forms.Panel();
            this.controlColorPanelLabel = new System.Windows.Forms.Label();
            this.resetColorsBtn = new System.Windows.Forms.Button();
            this.controlTextColorPanel = new System.Windows.Forms.Panel();
            this.previewMainColor = new System.Windows.Forms.Panel();
            this.previewBg = new System.Windows.Forms.Panel();
            this.previewAppListPanel = new System.Windows.Forms.Panel();
            this.previewAppList = new System.Windows.Forms.ListView();
            this.controlTextColorPanelLabel = new System.Windows.Forms.Label();
            this.defBgCheck = new System.Windows.Forms.CheckBox();
            this.setBgBtn = new System.Windows.Forms.Button();
            this.blurNumLabel = new System.Windows.Forms.Label();
            this.opacityNum = new System.Windows.Forms.NumericUpDown();
            this.blurNum = new System.Windows.Forms.NumericUpDown();
            this.opacityNumLabel = new System.Windows.Forms.Label();
            this.mainColorPanelLabel = new System.Windows.Forms.Label();
            this.btnHoverColorPanel = new System.Windows.Forms.Panel();
            this.btnHoverColorPanelLabel = new System.Windows.Forms.Label();
            this.mainColorPanel = new System.Windows.Forms.Panel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.updateChannel = new System.Windows.Forms.ComboBox();
            this.topControlTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.buttonFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.rmFromShellBtn = new System.Windows.Forms.Button();
            this.addToShellBtn = new System.Windows.Forms.Button();
            this.updateChannelLabel = new System.Windows.Forms.Label();
            this.defaultPos = new System.Windows.Forms.ComboBox();
            this.defaultPosLabel = new System.Windows.Forms.Label();
            this.startMenuIntegration = new System.Windows.Forms.ComboBox();
            this.startMenuIntegrationLabel = new System.Windows.Forms.Label();
            this.updateCheck = new System.Windows.Forms.ComboBox();
            this.updateCheckLabel = new System.Windows.Forms.Label();
            this.appDirsLabel = new System.Windows.Forms.Label();
            this.previewSmallImgList = new System.Windows.Forms.ImageList(this.components);
            this.previewLargeImgList = new System.Windows.Forms.ImageList(this.components);
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.saveBtn = new System.Windows.Forms.Button();
            this.exitBtn = new System.Windows.Forms.Button();
            this.tabCtrlPanel = new System.Windows.Forms.Panel();
            this.tabCtrlButtonBorderPanel = new System.Windows.Forms.Panel();
            this.fileTypesMenu.SuspendLayout();
            this.tabCtrl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.fileTypesTableLayout.SuspendLayout();
            this.fileTypesButtonFlowLayout.SuspendLayout();
            this.startArgsTableLayout.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.previewCaption.SuspendLayout();
            this.previewMainColor.SuspendLayout();
            this.previewBg.SuspendLayout();
            this.previewAppListPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opacityNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blurNum)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.topControlTableLayout.SuspendLayout();
            this.buttonFlowLayout.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.tabCtrlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 200;
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 200;
            this.toolTip.ReshowDelay = 40;
            this.toolTip.ShowAlways = true;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Notice:";
            // 
            // restoreFileTypesBtn
            // 
            this.restoreFileTypesBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.restoreFileTypesBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.restoreFileTypesBtn.Enabled = false;
            this.restoreFileTypesBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.restoreFileTypesBtn.Image = global::AppsLauncher.Properties.Resources.Undo;
            this.restoreFileTypesBtn.Location = new System.Drawing.Point(132, 10);
            this.restoreFileTypesBtn.Margin = new System.Windows.Forms.Padding(0, 10, 3, 0);
            this.restoreFileTypesBtn.Name = "restoreFileTypesBtn";
            this.restoreFileTypesBtn.Size = new System.Drawing.Size(44, 27);
            this.restoreFileTypesBtn.TabIndex = 0;
            this.toolTip.SetToolTip(this.restoreFileTypesBtn, "Restore file type associations.");
            this.restoreFileTypesBtn.UseVisualStyleBackColor = false;
            this.restoreFileTypesBtn.Visible = false;
            this.restoreFileTypesBtn.Click += new System.EventHandler(this.RestoreFileTypesBtn_Click);
            // 
            // fileTypes
            // 
            this.fileTypes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileTypes.ContextMenuStrip = this.fileTypesMenu;
            this.fileTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTypes.Location = new System.Drawing.Point(0, 0);
            this.fileTypes.Margin = new System.Windows.Forms.Padding(0);
            this.fileTypes.Multiline = true;
            this.fileTypes.Name = "fileTypes";
            this.fileTypes.Size = new System.Drawing.Size(329, 94);
            this.fileTypes.TabIndex = 0;
            this.toolTip.SetToolTip(this.fileTypes, "App associations can be made with file types. When a file is sent to\r\nthe Apps La" +
        "uncher, the first associated app is selected by default.");
            // 
            // fileTypesMenu
            // 
            this.fileTypesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileTypesMenuItem1,
            this.fileTypesMenuItem2,
            this.toolStripSeparator1,
            this.fileTypesMenuItem3});
            this.fileTypesMenu.Name = "fileTypesMenu";
            this.fileTypesMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.fileTypesMenu.ShowImageMargin = false;
            this.fileTypesMenu.Size = new System.Drawing.Size(117, 76);
            // 
            // fileTypesMenuItem1
            // 
            this.fileTypesMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileTypesMenuItem1.Name = "fileTypesMenuItem1";
            this.fileTypesMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.fileTypesMenuItem1.Text = "Copy";
            this.fileTypesMenuItem1.Click += new System.EventHandler(this.FileTypesMenu_Click);
            // 
            // fileTypesMenuItem2
            // 
            this.fileTypesMenuItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileTypesMenuItem2.Name = "fileTypesMenuItem2";
            this.fileTypesMenuItem2.Size = new System.Drawing.Size(116, 22);
            this.fileTypesMenuItem2.Text = "Paste";
            this.fileTypesMenuItem2.Click += new System.EventHandler(this.FileTypesMenu_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(113, 6);
            // 
            // fileTypesMenuItem3
            // 
            this.fileTypesMenuItem3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileTypesMenuItem3.Name = "fileTypesMenuItem3";
            this.fileTypesMenuItem3.Size = new System.Drawing.Size(116, 22);
            this.fileTypesMenuItem3.Text = "Load Default";
            this.fileTypesMenuItem3.Click += new System.EventHandler(this.FileTypesMenu_Click);
            // 
            // startArgsFirst
            // 
            this.startArgsFirst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startArgsFirst.Location = new System.Drawing.Point(0, 3);
            this.startArgsFirst.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.startArgsFirst.Name = "startArgsFirst";
            this.startArgsFirst.Size = new System.Drawing.Size(144, 25);
            this.startArgsFirst.TabIndex = 0;
            this.toolTip.SetToolTip(this.startArgsFirst, "This will be added preceding the current command line arguments.");
            // 
            // startArgsLast
            // 
            this.startArgsLast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startArgsLast.Location = new System.Drawing.Point(185, 3);
            this.startArgsLast.Name = "startArgsLast";
            this.startArgsLast.Size = new System.Drawing.Size(141, 25);
            this.startArgsLast.TabIndex = 2;
            this.toolTip.SetToolTip(this.startArgsLast, "This will be added following the current command line arguments.");
            // 
            // startArgsDefaultLabel
            // 
            this.startArgsDefaultLabel.BackColor = System.Drawing.Color.Transparent;
            this.startArgsDefaultLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startArgsDefaultLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startArgsDefaultLabel.Location = new System.Drawing.Point(150, 0);
            this.startArgsDefaultLabel.Name = "startArgsDefaultLabel";
            this.startArgsDefaultLabel.Size = new System.Drawing.Size(29, 27);
            this.startArgsDefaultLabel.TabIndex = 1;
            this.startArgsDefaultLabel.Text = "%*";
            this.startArgsDefaultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.startArgsDefaultLabel, "The current command-line arguments.");
            // 
            // locationBtn
            // 
            this.locationBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.locationBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.locationBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.locationBtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.locationBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.locationBtn.Image = global::AppsLauncher.Properties.Resources.Folder;
            this.locationBtn.Location = new System.Drawing.Point(413, 10);
            this.locationBtn.Name = "locationBtn";
            this.locationBtn.Size = new System.Drawing.Size(25, 27);
            this.locationBtn.TabIndex = 2;
            this.toolTip.SetToolTip(this.locationBtn, "Open app location.");
            this.locationBtn.UseVisualStyleBackColor = false;
            this.locationBtn.Click += new System.EventHandler(this.LocationBtn_Click);
            // 
            // appDirs
            // 
            this.appDirs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.appDirs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appDirs.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appDirs.Location = new System.Drawing.Point(0, 0);
            this.appDirs.Margin = new System.Windows.Forms.Padding(0);
            this.appDirs.Multiline = true;
            this.appDirs.Name = "appDirs";
            this.appDirs.Size = new System.Drawing.Size(329, 132);
            this.appDirs.TabIndex = 0;
            this.toolTip.SetToolTip(this.appDirs, "Add directories with portable apps:\r\n\r\n- Enter full paths, one per line\r\n- Accept" +
        "s environment variables");
            // 
            // langConfirmedCheck
            // 
            this.langConfirmedCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.langConfirmedCheck.AutoSize = true;
            this.langConfirmedCheck.BackColor = System.Drawing.Color.Transparent;
            this.langConfirmedCheck.Location = new System.Drawing.Point(127, 333);
            this.langConfirmedCheck.Name = "langConfirmedCheck";
            this.langConfirmedCheck.Size = new System.Drawing.Size(315, 21);
            this.langConfirmedCheck.TabIndex = 11;
            this.langConfirmedCheck.Text = "Latest language auto-confirmed on every request";
            this.toolTip.SetToolTip(this.langConfirmedCheck, "The latest language chosen for this app is automatically confirmed with each requ" +
        "est.");
            this.langConfirmedCheck.UseVisualStyleBackColor = false;
            // 
            // tabCtrl
            // 
            this.tabCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrl.Controls.Add(this.tabPage1);
            this.tabCtrl.Controls.Add(this.tabPage2);
            this.tabCtrl.Controls.Add(this.tabPage3);
            this.tabCtrl.ItemSize = new System.Drawing.Size(80, 22);
            this.tabCtrl.Location = new System.Drawing.Point(-4, 1);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            this.tabCtrl.Size = new System.Drawing.Size(508, 400);
            this.tabCtrl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabCtrl.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
            this.tabPage1.Controls.Add(this.langConfirmedCheck);
            this.tabPage1.Controls.Add(this.sortArgPathsCheck);
            this.tabPage1.Controls.Add(this.fileTypesTableLayout);
            this.tabPage1.Controls.Add(this.startArgsTableLayout);
            this.tabPage1.Controls.Add(this.runAsAdminCheck);
            this.tabPage1.Controls.Add(this.noUpdatesCheck);
            this.tabPage1.Controls.Add(this.locationBtn);
            this.tabPage1.Controls.Add(this.appsBox);
            this.tabPage1.Controls.Add(this.fileTypesLabel);
            this.tabPage1.Controls.Add(this.appsBoxLabel);
            this.tabPage1.Controls.Add(this.addArgsLabel);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(500, 370);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "App Options";
            // 
            // sortArgPathsCheck
            // 
            this.sortArgPathsCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sortArgPathsCheck.AutoSize = true;
            this.sortArgPathsCheck.BackColor = System.Drawing.Color.Transparent;
            this.sortArgPathsCheck.Location = new System.Drawing.Point(127, 232);
            this.sortArgPathsCheck.Name = "sortArgPathsCheck";
            this.sortArgPathsCheck.Size = new System.Drawing.Size(336, 21);
            this.sortArgPathsCheck.TabIndex = 7;
            this.sortArgPathsCheck.Text = "Sort paths in command line arguments alphabetically";
            this.sortArgPathsCheck.UseVisualStyleBackColor = false;
            // 
            // fileTypesTableLayout
            // 
            this.fileTypesTableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTypesTableLayout.ColumnCount = 1;
            this.fileTypesTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fileTypesTableLayout.Controls.Add(this.fileTypes, 0, 0);
            this.fileTypesTableLayout.Controls.Add(this.fileTypesButtonFlowLayout, 0, 2);
            this.fileTypesTableLayout.Location = new System.Drawing.Point(128, 48);
            this.fileTypesTableLayout.Name = "fileTypesTableLayout";
            this.fileTypesTableLayout.RowCount = 3;
            this.fileTypesTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fileTypesTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.fileTypesTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.fileTypesTableLayout.Size = new System.Drawing.Size(329, 136);
            this.fileTypesTableLayout.TabIndex = 4;
            // 
            // fileTypesButtonFlowLayout
            // 
            this.fileTypesButtonFlowLayout.Controls.Add(this.associateBtn);
            this.fileTypesButtonFlowLayout.Controls.Add(this.restoreFileTypesBtn);
            this.fileTypesButtonFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTypesButtonFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.fileTypesButtonFlowLayout.Location = new System.Drawing.Point(0, 96);
            this.fileTypesButtonFlowLayout.Margin = new System.Windows.Forms.Padding(0);
            this.fileTypesButtonFlowLayout.Name = "fileTypesButtonFlowLayout";
            this.fileTypesButtonFlowLayout.Size = new System.Drawing.Size(329, 40);
            this.fileTypesButtonFlowLayout.TabIndex = 1;
            // 
            // associateBtn
            // 
            this.associateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.associateBtn.AutoSize = true;
            this.associateBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.associateBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.associateBtn.Image = global::AppsLauncher.Properties.Resources.ShieldExclamation;
            this.associateBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.associateBtn.Location = new System.Drawing.Point(179, 10);
            this.associateBtn.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.associateBtn.Name = "associateBtn";
            this.associateBtn.Size = new System.Drawing.Size(150, 27);
            this.associateBtn.TabIndex = 1;
            this.associateBtn.Text = "Associate File Types";
            this.associateBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.associateBtn.UseVisualStyleBackColor = false;
            this.associateBtn.Click += new System.EventHandler(this.AssociateBtn_Click);
            // 
            // startArgsTableLayout
            // 
            this.startArgsTableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startArgsTableLayout.ColumnCount = 3;
            this.startArgsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.startArgsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.startArgsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.startArgsTableLayout.Controls.Add(this.startArgsFirst, 0, 0);
            this.startArgsTableLayout.Controls.Add(this.startArgsLast, 2, 0);
            this.startArgsTableLayout.Controls.Add(this.startArgsDefaultLabel, 1, 0);
            this.startArgsTableLayout.Location = new System.Drawing.Point(128, 193);
            this.startArgsTableLayout.Name = "startArgsTableLayout";
            this.startArgsTableLayout.RowCount = 1;
            this.startArgsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.startArgsTableLayout.Size = new System.Drawing.Size(329, 27);
            this.startArgsTableLayout.TabIndex = 6;
            // 
            // runAsAdminCheck
            // 
            this.runAsAdminCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.runAsAdminCheck.AutoSize = true;
            this.runAsAdminCheck.BackColor = System.Drawing.Color.Transparent;
            this.runAsAdminCheck.Location = new System.Drawing.Point(127, 275);
            this.runAsAdminCheck.Name = "runAsAdminCheck";
            this.runAsAdminCheck.Size = new System.Drawing.Size(336, 21);
            this.runAsAdminCheck.TabIndex = 9;
            this.runAsAdminCheck.Text = "Run this app with administrator privileges at all times";
            this.runAsAdminCheck.UseVisualStyleBackColor = false;
            // 
            // noUpdatesCheck
            // 
            this.noUpdatesCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.noUpdatesCheck.AutoSize = true;
            this.noUpdatesCheck.BackColor = System.Drawing.Color.Transparent;
            this.noUpdatesCheck.Location = new System.Drawing.Point(127, 304);
            this.noUpdatesCheck.Name = "noUpdatesCheck";
            this.noUpdatesCheck.Size = new System.Drawing.Size(253, 21);
            this.noUpdatesCheck.TabIndex = 10;
            this.noUpdatesCheck.Text = "Do not search for updates for this app";
            this.noUpdatesCheck.UseVisualStyleBackColor = false;
            // 
            // appsBox
            // 
            this.appsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.appsBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appsBox.FormattingEnabled = true;
            this.appsBox.Location = new System.Drawing.Point(128, 11);
            this.appsBox.Name = "appsBox";
            this.appsBox.Size = new System.Drawing.Size(278, 25);
            this.appsBox.TabIndex = 1;
            this.appsBox.SelectedIndexChanged += new System.EventHandler(this.AppsBox_SelectedIndexChanged);
            // 
            // fileTypesLabel
            // 
            this.fileTypesLabel.AutoSize = true;
            this.fileTypesLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileTypesLabel.Location = new System.Drawing.Point(55, 51);
            this.fileTypesLabel.Name = "fileTypesLabel";
            this.fileTypesLabel.Size = new System.Drawing.Size(67, 17);
            this.fileTypesLabel.TabIndex = 3;
            this.fileTypesLabel.Text = "File Types:";
            this.fileTypesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // appsBoxLabel
            // 
            this.appsBoxLabel.AutoSize = true;
            this.appsBoxLabel.BackColor = System.Drawing.Color.Transparent;
            this.appsBoxLabel.Location = new System.Drawing.Point(46, 15);
            this.appsBoxLabel.Name = "appsBoxLabel";
            this.appsBoxLabel.Size = new System.Drawing.Size(76, 17);
            this.appsBoxLabel.TabIndex = 0;
            this.appsBoxLabel.Text = "Application:";
            this.appsBoxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // addArgsLabel
            // 
            this.addArgsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addArgsLabel.AutoSize = true;
            this.addArgsLabel.BackColor = System.Drawing.Color.Transparent;
            this.addArgsLabel.Location = new System.Drawing.Point(20, 199);
            this.addArgsLabel.Name = "addArgsLabel";
            this.addArgsLabel.Size = new System.Drawing.Size(102, 17);
            this.addArgsLabel.TabIndex = 5;
            this.addArgsLabel.Text = "Add Arguments:";
            this.addArgsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addArgsLabel.MouseEnter += new System.EventHandler(this.ToolTipAtMouseEnter);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Window;
            this.tabPage2.Controls.Add(this.previewCaption);
            this.tabPage2.Controls.Add(this.showInTaskbarCheck);
            this.tabPage2.Controls.Add(this.showCaptionCheck);
            this.tabPage2.Controls.Add(this.viewStyle);
            this.tabPage2.Controls.Add(this.viewStyleLable);
            this.tabPage2.Controls.Add(this.bgLayout);
            this.tabPage2.Controls.Add(this.bgLayoutLabel);
            this.tabPage2.Controls.Add(this.controlColorPanel);
            this.tabPage2.Controls.Add(this.controlColorPanelLabel);
            this.tabPage2.Controls.Add(this.resetColorsBtn);
            this.tabPage2.Controls.Add(this.controlTextColorPanel);
            this.tabPage2.Controls.Add(this.previewMainColor);
            this.tabPage2.Controls.Add(this.controlTextColorPanelLabel);
            this.tabPage2.Controls.Add(this.defBgCheck);
            this.tabPage2.Controls.Add(this.setBgBtn);
            this.tabPage2.Controls.Add(this.blurNumLabel);
            this.tabPage2.Controls.Add(this.opacityNum);
            this.tabPage2.Controls.Add(this.blurNum);
            this.tabPage2.Controls.Add(this.opacityNumLabel);
            this.tabPage2.Controls.Add(this.mainColorPanelLabel);
            this.tabPage2.Controls.Add(this.btnHoverColorPanel);
            this.tabPage2.Controls.Add(this.btnHoverColorPanelLabel);
            this.tabPage2.Controls.Add(this.mainColorPanel);
            this.tabPage2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(500, 370);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Style";
            // 
            // previewCaption
            // 
            this.previewCaption.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.previewCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewCaption.Controls.Add(this.panel2);
            this.previewCaption.Location = new System.Drawing.Point(274, 51);
            this.previewCaption.Name = "previewCaption";
            this.previewCaption.Size = new System.Drawing.Size(160, 16);
            this.previewCaption.TabIndex = 55;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(142, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(16, 14);
            this.panel2.TabIndex = 0;
            // 
            // showInTaskbarCheck
            // 
            this.showInTaskbarCheck.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.showInTaskbarCheck.AutoSize = true;
            this.showInTaskbarCheck.BackColor = System.Drawing.Color.Transparent;
            this.showInTaskbarCheck.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showInTaskbarCheck.Location = new System.Drawing.Point(284, 318);
            this.showInTaskbarCheck.Name = "showInTaskbarCheck";
            this.showInTaskbarCheck.Size = new System.Drawing.Size(121, 21);
            this.showInTaskbarCheck.TabIndex = 54;
            this.showInTaskbarCheck.Text = "Show in Taskbar";
            this.showInTaskbarCheck.UseVisualStyleBackColor = false;
            // 
            // showCaptionCheck
            // 
            this.showCaptionCheck.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.showCaptionCheck.AutoSize = true;
            this.showCaptionCheck.BackColor = System.Drawing.Color.Transparent;
            this.showCaptionCheck.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showCaptionCheck.Location = new System.Drawing.Point(284, 291);
            this.showCaptionCheck.Name = "showCaptionCheck";
            this.showCaptionCheck.Size = new System.Drawing.Size(107, 21);
            this.showCaptionCheck.TabIndex = 53;
            this.showCaptionCheck.Text = "Show Caption";
            this.showCaptionCheck.UseVisualStyleBackColor = false;
            this.showCaptionCheck.CheckedChanged += new System.EventHandler(this.ControlOption_Changed);
            // 
            // viewStyle
            // 
            this.viewStyle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.viewStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.viewStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewStyle.FormattingEnabled = true;
            this.viewStyle.Items.AddRange(new object[] {
            "List (small)",
            "Tile (small)",
            "Tile (large)"});
            this.viewStyle.Location = new System.Drawing.Point(152, 18);
            this.viewStyle.Name = "viewStyle";
            this.viewStyle.Size = new System.Drawing.Size(90, 25);
            this.viewStyle.TabIndex = 52;
            this.viewStyle.SelectedIndexChanged += new System.EventHandler(this.ControlOption_Changed);
            // 
            // viewStyleLable
            // 
            this.viewStyleLable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.viewStyleLable.AutoSize = true;
            this.viewStyleLable.BackColor = System.Drawing.Color.Transparent;
            this.viewStyleLable.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewStyleLable.Location = new System.Drawing.Point(75, 22);
            this.viewStyleLable.Name = "viewStyleLable";
            this.viewStyleLable.Size = new System.Drawing.Size(69, 17);
            this.viewStyleLable.TabIndex = 51;
            this.viewStyleLable.Text = "View Style:";
            this.viewStyleLable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bgLayout
            // 
            this.bgLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bgLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bgLayout.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bgLayout.FormattingEnabled = true;
            this.bgLayout.Items.AddRange(new object[] {
            "None",
            "Tile",
            "Center",
            "Stretch",
            "Zoom"});
            this.bgLayout.Location = new System.Drawing.Point(151, 157);
            this.bgLayout.Name = "bgLayout";
            this.bgLayout.Size = new System.Drawing.Size(90, 25);
            this.bgLayout.TabIndex = 8;
            this.bgLayout.SelectedIndexChanged += new System.EventHandler(this.ControlOption_Changed);
            // 
            // bgLayoutLabel
            // 
            this.bgLayoutLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bgLayoutLabel.AutoSize = true;
            this.bgLayoutLabel.BackColor = System.Drawing.Color.Transparent;
            this.bgLayoutLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bgLayoutLabel.Location = new System.Drawing.Point(96, 161);
            this.bgLayoutLabel.Name = "bgLayoutLabel";
            this.bgLayoutLabel.Size = new System.Drawing.Size(49, 17);
            this.bgLayoutLabel.TabIndex = 7;
            this.bgLayoutLabel.Text = "Layout:";
            this.bgLayoutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // controlColorPanel
            // 
            this.controlColorPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.controlColorPanel.BackColor = System.Drawing.SystemColors.Control;
            this.controlColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controlColorPanel.Location = new System.Drawing.Point(225, 256);
            this.controlColorPanel.Name = "controlColorPanel";
            this.controlColorPanel.Size = new System.Drawing.Size(16, 16);
            this.controlColorPanel.TabIndex = 13;
            this.controlColorPanel.TabStop = true;
            this.controlColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.controlColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.controlColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // controlColorPanelLabel
            // 
            this.controlColorPanelLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.controlColorPanelLabel.AutoSize = true;
            this.controlColorPanelLabel.BackColor = System.Drawing.Color.Transparent;
            this.controlColorPanelLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlColorPanelLabel.Location = new System.Drawing.Point(128, 254);
            this.controlColorPanelLabel.Name = "controlColorPanelLabel";
            this.controlColorPanelLabel.Size = new System.Drawing.Size(90, 17);
            this.controlColorPanelLabel.TabIndex = 12;
            this.controlColorPanelLabel.Text = "Control Color:";
            this.controlColorPanelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // resetColorsBtn
            // 
            this.resetColorsBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.resetColorsBtn.AutoSize = true;
            this.resetColorsBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.resetColorsBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resetColorsBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.resetColorsBtn.Location = new System.Drawing.Point(178, 332);
            this.resetColorsBtn.Name = "resetColorsBtn";
            this.resetColorsBtn.Size = new System.Drawing.Size(63, 27);
            this.resetColorsBtn.TabIndex = 22;
            this.resetColorsBtn.Text = "Reset";
            this.resetColorsBtn.UseVisualStyleBackColor = false;
            this.resetColorsBtn.Click += new System.EventHandler(this.ResetColorsBtn_Click);
            // 
            // controlTextColorPanel
            // 
            this.controlTextColorPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.controlTextColorPanel.BackColor = System.Drawing.SystemColors.WindowText;
            this.controlTextColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controlTextColorPanel.Location = new System.Drawing.Point(225, 282);
            this.controlTextColorPanel.Name = "controlTextColorPanel";
            this.controlTextColorPanel.Size = new System.Drawing.Size(16, 16);
            this.controlTextColorPanel.TabIndex = 15;
            this.controlTextColorPanel.TabStop = true;
            this.controlTextColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.controlTextColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.controlTextColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // previewMainColor
            // 
            this.previewMainColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.previewMainColor.BackColor = System.Drawing.SystemColors.Window;
            this.previewMainColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewMainColor.Controls.Add(this.previewBg);
            this.previewMainColor.Location = new System.Drawing.Point(274, 66);
            this.previewMainColor.Name = "previewMainColor";
            this.previewMainColor.Size = new System.Drawing.Size(160, 213);
            this.previewMainColor.TabIndex = 50;
            // 
            // previewBg
            // 
            this.previewBg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewBg.BackColor = System.Drawing.Color.Transparent;
            this.previewBg.Controls.Add(this.previewAppListPanel);
            this.previewBg.Location = new System.Drawing.Point(1, 1);
            this.previewBg.Name = "previewBg";
            this.previewBg.Size = new System.Drawing.Size(156, 209);
            this.previewBg.TabIndex = 50;
            // 
            // previewAppListPanel
            // 
            this.previewAppListPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewAppListPanel.BackColor = System.Drawing.SystemColors.Control;
            this.previewAppListPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewAppListPanel.Controls.Add(this.previewAppList);
            this.previewAppListPanel.Location = new System.Drawing.Point(4, 4);
            this.previewAppListPanel.Name = "previewAppListPanel";
            this.previewAppListPanel.Padding = new System.Windows.Forms.Padding(3);
            this.previewAppListPanel.Size = new System.Drawing.Size(121, 201);
            this.previewAppListPanel.TabIndex = 50;
            this.previewAppListPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewAppList_Paint);
            // 
            // previewAppList
            // 
            this.previewAppList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewAppList.BackColor = System.Drawing.SystemColors.Control;
            this.previewAppList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.previewAppList.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewAppList.ForeColor = System.Drawing.SystemColors.ControlText;
            this.previewAppList.HideSelection = false;
            this.previewAppList.HoverSelection = true;
            listViewItem3.StateImageIndex = 0;
            listViewItem4.StateImageIndex = 0;
            this.previewAppList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.previewAppList.Location = new System.Drawing.Point(3, 3);
            this.previewAppList.Name = "previewAppList";
            this.previewAppList.Scrollable = false;
            this.previewAppList.ShowGroups = false;
            this.previewAppList.Size = new System.Drawing.Size(113, 53);
            this.previewAppList.TabIndex = 50;
            this.previewAppList.TabStop = false;
            this.previewAppList.UseCompatibleStateImageBehavior = false;
            this.previewAppList.View = System.Windows.Forms.View.List;
            // 
            // controlTextColorPanelLabel
            // 
            this.controlTextColorPanelLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.controlTextColorPanelLabel.AutoSize = true;
            this.controlTextColorPanelLabel.BackColor = System.Drawing.Color.Transparent;
            this.controlTextColorPanelLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlTextColorPanelLabel.Location = new System.Drawing.Point(101, 281);
            this.controlTextColorPanelLabel.Name = "controlTextColorPanelLabel";
            this.controlTextColorPanelLabel.Size = new System.Drawing.Size(117, 17);
            this.controlTextColorPanelLabel.TabIndex = 14;
            this.controlTextColorPanelLabel.Text = "Control Text Color:";
            this.controlTextColorPanelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // defBgCheck
            // 
            this.defBgCheck.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.defBgCheck.AutoSize = true;
            this.defBgCheck.BackColor = System.Drawing.Color.Transparent;
            this.defBgCheck.Checked = true;
            this.defBgCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.defBgCheck.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defBgCheck.Location = new System.Drawing.Point(97, 189);
            this.defBgCheck.Name = "defBgCheck";
            this.defBgCheck.Size = new System.Drawing.Size(141, 21);
            this.defBgCheck.TabIndex = 9;
            this.defBgCheck.Text = "Default Background";
            this.defBgCheck.UseVisualStyleBackColor = false;
            this.defBgCheck.CheckedChanged += new System.EventHandler(this.DefBgCheck_CheckedChanged);
            // 
            // setBgBtn
            // 
            this.setBgBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.setBgBtn.AutoSize = true;
            this.setBgBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.setBgBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setBgBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.setBgBtn.Location = new System.Drawing.Point(59, 120);
            this.setBgBtn.Name = "setBgBtn";
            this.setBgBtn.Size = new System.Drawing.Size(183, 27);
            this.setBgBtn.TabIndex = 6;
            this.setBgBtn.Text = "Change Background";
            this.setBgBtn.UseVisualStyleBackColor = false;
            this.setBgBtn.Click += new System.EventHandler(this.SetBgBtn_Click);
            // 
            // blurNumLabel
            // 
            this.blurNumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.blurNumLabel.AutoSize = true;
            this.blurNumLabel.BackColor = System.Drawing.Color.Transparent;
            this.blurNumLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blurNumLabel.Location = new System.Drawing.Point(139, 55);
            this.blurNumLabel.Name = "blurNumLabel";
            this.blurNumLabel.Size = new System.Drawing.Size(33, 17);
            this.blurNumLabel.TabIndex = 2;
            this.blurNumLabel.Text = "Blur:";
            this.blurNumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // opacityNum
            // 
            this.opacityNum.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.opacityNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opacityNum.DecimalPlaces = 2;
            this.opacityNum.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opacityNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.opacityNum.Location = new System.Drawing.Point(177, 85);
            this.opacityNum.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            this.opacityNum.Minimum = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.opacityNum.Name = "opacityNum";
            this.opacityNum.Size = new System.Drawing.Size(64, 25);
            this.opacityNum.TabIndex = 5;
            this.opacityNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.opacityNum.Value = new decimal(new int[] {
            95,
            0,
            0,
            131072});
            // 
            // blurNum
            // 
            this.blurNum.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.blurNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.blurNum.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blurNum.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.blurNum.Location = new System.Drawing.Point(178, 51);
            this.blurNum.Name = "blurNum";
            this.blurNum.Size = new System.Drawing.Size(64, 25);
            this.blurNum.TabIndex = 3;
            this.blurNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.blurNum.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // opacityNumLabel
            // 
            this.opacityNumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.opacityNumLabel.AutoSize = true;
            this.opacityNumLabel.BackColor = System.Drawing.Color.Transparent;
            this.opacityNumLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opacityNumLabel.Location = new System.Drawing.Point(116, 89);
            this.opacityNumLabel.Name = "opacityNumLabel";
            this.opacityNumLabel.Size = new System.Drawing.Size(55, 17);
            this.opacityNumLabel.TabIndex = 4;
            this.opacityNumLabel.Text = "Opacity:";
            this.opacityNumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mainColorPanelLabel
            // 
            this.mainColorPanelLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mainColorPanelLabel.AutoSize = true;
            this.mainColorPanelLabel.BackColor = System.Drawing.Color.Transparent;
            this.mainColorPanelLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainColorPanelLabel.Location = new System.Drawing.Point(142, 229);
            this.mainColorPanelLabel.Name = "mainColorPanelLabel";
            this.mainColorPanelLabel.Size = new System.Drawing.Size(76, 17);
            this.mainColorPanelLabel.TabIndex = 10;
            this.mainColorPanelLabel.Text = "Main Color:";
            this.mainColorPanelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnHoverColorPanel
            // 
            this.btnHoverColorPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHoverColorPanel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnHoverColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btnHoverColorPanel.Location = new System.Drawing.Point(225, 307);
            this.btnHoverColorPanel.Name = "btnHoverColorPanel";
            this.btnHoverColorPanel.Size = new System.Drawing.Size(16, 16);
            this.btnHoverColorPanel.TabIndex = 19;
            this.btnHoverColorPanel.TabStop = true;
            this.btnHoverColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.btnHoverColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.btnHoverColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // btnHoverColorPanelLabel
            // 
            this.btnHoverColorPanelLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHoverColorPanelLabel.AutoSize = true;
            this.btnHoverColorPanelLabel.BackColor = System.Drawing.Color.Transparent;
            this.btnHoverColorPanelLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoverColorPanelLabel.Location = new System.Drawing.Point(95, 306);
            this.btnHoverColorPanelLabel.Name = "btnHoverColorPanelLabel";
            this.btnHoverColorPanelLabel.Size = new System.Drawing.Size(123, 17);
            this.btnHoverColorPanelLabel.TabIndex = 18;
            this.btnHoverColorPanelLabel.Text = "Button Hover Color:";
            this.btnHoverColorPanelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mainColorPanel
            // 
            this.mainColorPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mainColorPanel.BackColor = System.Drawing.SystemColors.Window;
            this.mainColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainColorPanel.Location = new System.Drawing.Point(225, 230);
            this.mainColorPanel.Name = "mainColorPanel";
            this.mainColorPanel.Size = new System.Drawing.Size(16, 16);
            this.mainColorPanel.TabIndex = 11;
            this.mainColorPanel.TabStop = true;
            this.mainColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.mainColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.mainColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Window;
            this.tabPage3.Controls.Add(this.updateChannel);
            this.tabPage3.Controls.Add(this.topControlTableLayout);
            this.tabPage3.Controls.Add(this.updateChannelLabel);
            this.tabPage3.Controls.Add(this.defaultPos);
            this.tabPage3.Controls.Add(this.defaultPosLabel);
            this.tabPage3.Controls.Add(this.startMenuIntegration);
            this.tabPage3.Controls.Add(this.startMenuIntegrationLabel);
            this.tabPage3.Controls.Add(this.updateCheck);
            this.tabPage3.Controls.Add(this.updateCheckLabel);
            this.tabPage3.Controls.Add(this.appDirsLabel);
            this.tabPage3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(500, 398);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Misc";
            // 
            // updateChannel
            // 
            this.updateChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updateChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.updateChannel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateChannel.FormattingEnabled = true;
            this.updateChannel.Items.AddRange(new object[] {
            "Release",
            "Beta"});
            this.updateChannel.Location = new System.Drawing.Point(301, 344);
            this.updateChannel.Name = "updateChannel";
            this.updateChannel.Size = new System.Drawing.Size(160, 25);
            this.updateChannel.TabIndex = 10;
            // 
            // topControlTableLayout
            // 
            this.topControlTableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.topControlTableLayout.ColumnCount = 1;
            this.topControlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topControlTableLayout.Controls.Add(this.appDirs);
            this.topControlTableLayout.Controls.Add(this.buttonFlowLayout, 0, 2);
            this.topControlTableLayout.Location = new System.Drawing.Point(132, 43);
            this.topControlTableLayout.Name = "topControlTableLayout";
            this.topControlTableLayout.RowCount = 3;
            this.topControlTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topControlTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.topControlTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.topControlTableLayout.Size = new System.Drawing.Size(329, 174);
            this.topControlTableLayout.TabIndex = 1;
            // 
            // buttonFlowLayout
            // 
            this.buttonFlowLayout.Controls.Add(this.rmFromShellBtn);
            this.buttonFlowLayout.Controls.Add(this.addToShellBtn);
            this.buttonFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonFlowLayout.Location = new System.Drawing.Point(0, 134);
            this.buttonFlowLayout.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFlowLayout.Name = "buttonFlowLayout";
            this.buttonFlowLayout.Size = new System.Drawing.Size(329, 40);
            this.buttonFlowLayout.TabIndex = 1;
            // 
            // rmFromShellBtn
            // 
            this.rmFromShellBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rmFromShellBtn.AutoSize = true;
            this.rmFromShellBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rmFromShellBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rmFromShellBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rmFromShellBtn.Image = global::AppsLauncher.Properties.Resources.ShieldExclamation;
            this.rmFromShellBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rmFromShellBtn.Location = new System.Drawing.Point(179, 10);
            this.rmFromShellBtn.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.rmFromShellBtn.Name = "rmFromShellBtn";
            this.rmFromShellBtn.Size = new System.Drawing.Size(150, 27);
            this.rmFromShellBtn.TabIndex = 1;
            this.rmFromShellBtn.Text = "Remove from Shell";
            this.rmFromShellBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rmFromShellBtn.UseVisualStyleBackColor = false;
            this.rmFromShellBtn.TextChanged += new System.EventHandler(this.ShellBtns_TextChanged);
            this.rmFromShellBtn.Click += new System.EventHandler(this.ShellBtns_Click);
            // 
            // addToShellBtn
            // 
            this.addToShellBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addToShellBtn.AutoSize = true;
            this.addToShellBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.addToShellBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addToShellBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.addToShellBtn.Image = global::AppsLauncher.Properties.Resources.ShieldExclamation;
            this.addToShellBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addToShellBtn.Location = new System.Drawing.Point(38, 10);
            this.addToShellBtn.Margin = new System.Windows.Forms.Padding(0, 10, 3, 0);
            this.addToShellBtn.Name = "addToShellBtn";
            this.addToShellBtn.Size = new System.Drawing.Size(138, 27);
            this.addToShellBtn.TabIndex = 0;
            this.addToShellBtn.Text = "Integrate to Shell";
            this.addToShellBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addToShellBtn.UseVisualStyleBackColor = false;
            this.addToShellBtn.TextChanged += new System.EventHandler(this.ShellBtns_TextChanged);
            this.addToShellBtn.Click += new System.EventHandler(this.ShellBtns_Click);
            // 
            // updateChannelLabel
            // 
            this.updateChannelLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updateChannelLabel.AutoSize = true;
            this.updateChannelLabel.BackColor = System.Drawing.Color.Transparent;
            this.updateChannelLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateChannelLabel.Location = new System.Drawing.Point(191, 348);
            this.updateChannelLabel.Name = "updateChannelLabel";
            this.updateChannelLabel.Size = new System.Drawing.Size(104, 17);
            this.updateChannelLabel.TabIndex = 9;
            this.updateChannelLabel.Text = "Update Channel:";
            this.updateChannelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // defaultPos
            // 
            this.defaultPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.defaultPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.defaultPos.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defaultPos.FormattingEnabled = true;
            this.defaultPos.Items.AddRange(new object[] {
            "Align to Cursor Position",
            "Align to Start Menu"});
            this.defaultPos.Location = new System.Drawing.Point(301, 266);
            this.defaultPos.Name = "defaultPos";
            this.defaultPos.Size = new System.Drawing.Size(160, 25);
            this.defaultPos.TabIndex = 6;
            // 
            // defaultPosLabel
            // 
            this.defaultPosLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.defaultPosLabel.AutoSize = true;
            this.defaultPosLabel.BackColor = System.Drawing.Color.Transparent;
            this.defaultPosLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defaultPosLabel.Location = new System.Drawing.Point(190, 270);
            this.defaultPosLabel.Name = "defaultPosLabel";
            this.defaultPosLabel.Size = new System.Drawing.Size(105, 17);
            this.defaultPosLabel.TabIndex = 5;
            this.defaultPosLabel.Text = "Default Location:";
            this.defaultPosLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // startMenuIntegration
            // 
            this.startMenuIntegration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startMenuIntegration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startMenuIntegration.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startMenuIntegration.FormattingEnabled = true;
            this.startMenuIntegration.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
            this.startMenuIntegration.Location = new System.Drawing.Point(301, 227);
            this.startMenuIntegration.Name = "startMenuIntegration";
            this.startMenuIntegration.Size = new System.Drawing.Size(160, 25);
            this.startMenuIntegration.TabIndex = 4;
            // 
            // startMenuIntegrationLabel
            // 
            this.startMenuIntegrationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startMenuIntegrationLabel.AutoSize = true;
            this.startMenuIntegrationLabel.BackColor = System.Drawing.Color.Transparent;
            this.startMenuIntegrationLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startMenuIntegrationLabel.Location = new System.Drawing.Point(156, 231);
            this.startMenuIntegrationLabel.Name = "startMenuIntegrationLabel";
            this.startMenuIntegrationLabel.Size = new System.Drawing.Size(142, 17);
            this.startMenuIntegrationLabel.TabIndex = 3;
            this.startMenuIntegrationLabel.Text = "Start Menu integration:";
            this.startMenuIntegrationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // updateCheck
            // 
            this.updateCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updateCheck.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.updateCheck.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateCheck.FormattingEnabled = true;
            this.updateCheck.Items.AddRange(new object[] {
            "Never",
            "Hourly (full)",
            "Hourly (only apps)",
            "Hourly (only apps suite)",
            "Daily (full)",
            "Daily (only apps)",
            "Daily (only apps suite)",
            "Weekly (full)",
            "Weekly (only apps)",
            "Weekly (only apps suite)",
            "Monthly (full)",
            "Monthly (only apps)",
            "Monthly (only apps suite)"});
            this.updateCheck.Location = new System.Drawing.Point(301, 305);
            this.updateCheck.Name = "updateCheck";
            this.updateCheck.Size = new System.Drawing.Size(160, 25);
            this.updateCheck.TabIndex = 8;
            // 
            // updateCheckLabel
            // 
            this.updateCheckLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updateCheckLabel.AutoSize = true;
            this.updateCheckLabel.BackColor = System.Drawing.Color.Transparent;
            this.updateCheckLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateCheckLabel.Location = new System.Drawing.Point(171, 309);
            this.updateCheckLabel.Name = "updateCheckLabel";
            this.updateCheckLabel.Size = new System.Drawing.Size(124, 17);
            this.updateCheckLabel.TabIndex = 7;
            this.updateCheckLabel.Text = "Search for Updates:";
            this.updateCheckLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // appDirsLabel
            // 
            this.appDirsLabel.AutoSize = true;
            this.appDirsLabel.BackColor = System.Drawing.Color.Transparent;
            this.appDirsLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appDirsLabel.Location = new System.Drawing.Point(24, 46);
            this.appDirsLabel.Name = "appDirsLabel";
            this.appDirsLabel.Size = new System.Drawing.Size(102, 17);
            this.appDirsLabel.TabIndex = 0;
            this.appDirsLabel.Text = "App Directories:";
            this.appDirsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // previewSmallImgList
            // 
            this.previewSmallImgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.previewSmallImgList.ImageSize = new System.Drawing.Size(12, 12);
            this.previewSmallImgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // previewLargeImgList
            // 
            this.previewLargeImgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.previewLargeImgList.ImageSize = new System.Drawing.Size(24, 24);
            this.previewLargeImgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.Transparent;
            this.buttonPanel.Controls.Add(this.saveBtn);
            this.buttonPanel.Controls.Add(this.exitBtn);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 396);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(500, 65);
            this.buttonPanel.TabIndex = 11;
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.AutoSize = true;
            this.saveBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.saveBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.saveBtn.Location = new System.Drawing.Point(281, 19);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(90, 28);
            this.saveBtn.TabIndex = 100;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = false;
            this.saveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // exitBtn
            // 
            this.exitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitBtn.AutoSize = true;
            this.exitBtn.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.exitBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.exitBtn.Location = new System.Drawing.Point(385, 19);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(90, 28);
            this.exitBtn.TabIndex = 101;
            this.exitBtn.Text = "Exit";
            this.exitBtn.UseVisualStyleBackColor = false;
            this.exitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // tabCtrlPanel
            // 
            this.tabCtrlPanel.Controls.Add(this.tabCtrl);
            this.tabCtrlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlPanel.Location = new System.Drawing.Point(0, 0);
            this.tabCtrlPanel.Name = "tabCtrlPanel";
            this.tabCtrlPanel.Size = new System.Drawing.Size(500, 396);
            this.tabCtrlPanel.TabIndex = 14;
            // 
            // tabCtrlButtonBorderPanel
            // 
            this.tabCtrlButtonBorderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrlButtonBorderPanel.Location = new System.Drawing.Point(0, 25);
            this.tabCtrlButtonBorderPanel.Name = "tabCtrlButtonBorderPanel";
            this.tabCtrlButtonBorderPanel.Size = new System.Drawing.Size(500, 3);
            this.tabCtrlButtonBorderPanel.TabIndex = 15;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(500, 461);
            this.Controls.Add(this.tabCtrlButtonBorderPanel);
            this.Controls.Add(this.tabCtrlPanel);
            this.Controls.Add(this.buttonPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(510, 490);
            this.Name = "SettingsForm";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.EnabledChanged += new System.EventHandler(this.SettingsForm_EnabledChanged);
            this.fileTypesMenu.ResumeLayout(false);
            this.tabCtrl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.fileTypesTableLayout.ResumeLayout(false);
            this.fileTypesTableLayout.PerformLayout();
            this.fileTypesButtonFlowLayout.ResumeLayout(false);
            this.fileTypesButtonFlowLayout.PerformLayout();
            this.startArgsTableLayout.ResumeLayout(false);
            this.startArgsTableLayout.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.previewCaption.ResumeLayout(false);
            this.previewMainColor.ResumeLayout(false);
            this.previewBg.ResumeLayout(false);
            this.previewAppListPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.opacityNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blurNum)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.topControlTableLayout.ResumeLayout(false);
            this.topControlTableLayout.PerformLayout();
            this.buttonFlowLayout.ResumeLayout(false);
            this.buttonFlowLayout.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.tabCtrlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button associateBtn;
        private System.Windows.Forms.Label fileTypesLabel;
        private System.Windows.Forms.Label appsBoxLabel;
        private System.Windows.Forms.TextBox startArgsLast;
        private System.Windows.Forms.Label addArgsLabel;
        private System.Windows.Forms.TextBox startArgsFirst;
        private System.Windows.Forms.ComboBox updateCheck;
        private System.Windows.Forms.Label updateCheckLabel;
        private System.Windows.Forms.Label appDirsLabel;
        private System.Windows.Forms.ComboBox appsBox;
        private System.Windows.Forms.Label startArgsDefaultLabel;
        private System.Windows.Forms.Button locationBtn;
        private System.Windows.Forms.Button addToShellBtn;
        private System.Windows.Forms.Button rmFromShellBtn;
        private System.Windows.Forms.ComboBox startMenuIntegration;
        private System.Windows.Forms.Label startMenuIntegrationLabel;
        private System.Windows.Forms.ContextMenuStrip fileTypesMenu;
        private System.Windows.Forms.ToolStripMenuItem fileTypesMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fileTypesMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem fileTypesMenuItem3;
        private System.Windows.Forms.CheckBox noUpdatesCheck;
        private System.Windows.Forms.CheckBox runAsAdminCheck;
        private System.Windows.Forms.Button restoreFileTypesBtn;
        private System.Windows.Forms.ComboBox defaultPos;
        private System.Windows.Forms.Label defaultPosLabel;
        private System.Windows.Forms.ImageList previewSmallImgList;
        private System.Windows.Forms.ImageList previewLargeImgList;
        private System.Windows.Forms.ComboBox updateChannel;
        private System.Windows.Forms.Label updateChannelLabel;
        private System.Windows.Forms.TextBox fileTypes;
        private System.Windows.Forms.TextBox appDirs;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TableLayoutPanel startArgsTableLayout;
        private System.Windows.Forms.TableLayoutPanel fileTypesTableLayout;
        private System.Windows.Forms.FlowLayoutPanel fileTypesButtonFlowLayout;
        private System.Windows.Forms.TableLayoutPanel topControlTableLayout;
        private System.Windows.Forms.FlowLayoutPanel buttonFlowLayout;
        private System.Windows.Forms.CheckBox sortArgPathsCheck;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox bgLayout;
        private System.Windows.Forms.Label bgLayoutLabel;
        private System.Windows.Forms.Panel controlColorPanel;
        private System.Windows.Forms.Label controlColorPanelLabel;
        private System.Windows.Forms.Button resetColorsBtn;
        private System.Windows.Forms.Panel controlTextColorPanel;
        private System.Windows.Forms.Panel previewMainColor;
        private System.Windows.Forms.Panel previewBg;
        private System.Windows.Forms.Panel previewAppListPanel;
        private System.Windows.Forms.ListView previewAppList;
        private System.Windows.Forms.Label controlTextColorPanelLabel;
        private System.Windows.Forms.CheckBox defBgCheck;
        private System.Windows.Forms.Button setBgBtn;
        private System.Windows.Forms.Label blurNumLabel;
        private System.Windows.Forms.NumericUpDown opacityNum;
        private System.Windows.Forms.NumericUpDown blurNum;
        private System.Windows.Forms.Label opacityNumLabel;
        private System.Windows.Forms.Label mainColorPanelLabel;
        private System.Windows.Forms.Panel btnHoverColorPanel;
        private System.Windows.Forms.Label btnHoverColorPanelLabel;
        private System.Windows.Forms.Panel mainColorPanel;
        private System.Windows.Forms.Panel tabCtrlPanel;
        private System.Windows.Forms.Panel tabCtrlButtonBorderPanel;
        private System.Windows.Forms.ComboBox viewStyle;
        private System.Windows.Forms.Label viewStyleLable;
        private System.Windows.Forms.CheckBox showInTaskbarCheck;
        private System.Windows.Forms.CheckBox showCaptionCheck;
        private System.Windows.Forms.Panel previewCaption;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox langConfirmedCheck;
    }
}