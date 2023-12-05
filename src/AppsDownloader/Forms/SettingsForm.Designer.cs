namespace AppsDownloader.Forms
{
    using SilDev;
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
            this.highlightInstalledCheck = new System.Windows.Forms.CheckBox();
            this.showColorsCheck = new System.Windows.Forms.CheckBox();
            this.showGroupsCheck = new System.Windows.Forms.CheckBox();
            this.appListGroupBox = new System.Windows.Forms.GroupBox();
            this.showLargeImagesCheck = new System.Windows.Forms.CheckBox();
            this.groupColorsGroupBox = new System.Windows.Forms.GroupBox();
            this.listViewGroup10 = new System.Windows.Forms.Label();
            this.group10ColorPanel = new System.Windows.Forms.Panel();
            this.listViewGroup1 = new System.Windows.Forms.Label();
            this.group1ColorPanel = new System.Windows.Forms.Panel();
            this.listViewGroup11 = new System.Windows.Forms.Label();
            this.group11ColorPanel = new System.Windows.Forms.Panel();
            this.listViewGroup9 = new System.Windows.Forms.Label();
            this.listViewGroup8 = new System.Windows.Forms.Label();
            this.group9ColorPanel = new System.Windows.Forms.Panel();
            this.listViewGroup7 = new System.Windows.Forms.Label();
            this.group8ColorPanel = new System.Windows.Forms.Panel();
            this.listViewGroup6 = new System.Windows.Forms.Label();
            this.group7ColorPanel = new System.Windows.Forms.Panel();
            this.listViewGroup5 = new System.Windows.Forms.Label();
            this.group6ColorPanel = new System.Windows.Forms.Panel();
            this.listViewGroup4 = new System.Windows.Forms.Label();
            this.group5ColorPanel = new System.Windows.Forms.Panel();
            this.group4ColorPanel = new System.Windows.Forms.Panel();
            this.listViewGroup3 = new System.Windows.Forms.Label();
            this.group3ColorPanel = new System.Windows.Forms.Panel();
            this.resetColorsBtn = new System.Windows.Forms.Button();
            this.listViewGroup2 = new System.Windows.Forms.Label();
            this.group2ColorPanel = new System.Windows.Forms.Panel();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.transferGroupBox = new System.Windows.Forms.GroupBox();
            this.transferPathPanel = new System.Windows.Forms.Label();
            this.transferPathUndoBtn = new System.Windows.Forms.Button();
            this.transferPathBtn = new System.Windows.Forms.Button();
            this.logoBox = new System.Windows.Forms.PictureBox();
            this.openSrcManBtn = new System.Windows.Forms.Button();
            this.advancedGroupBox = new System.Windows.Forms.GroupBox();
            this.appListGroupBox.SuspendLayout();
            this.groupColorsGroupBox.SuspendLayout();
            this.transferGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).BeginInit();
            this.advancedGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // highlightInstalledCheck
            // 
            this.highlightInstalledCheck.AutoSize = true;
            this.highlightInstalledCheck.Checked = true;
            this.highlightInstalledCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.highlightInstalledCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.highlightInstalledCheck.Location = new System.Drawing.Point(21, 34);
            this.highlightInstalledCheck.Name = "highlightInstalledCheck";
            this.highlightInstalledCheck.Size = new System.Drawing.Size(127, 19);
            this.highlightInstalledCheck.TabIndex = 1;
            this.highlightInstalledCheck.Text = "Highlight Installed";
            this.highlightInstalledCheck.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // showColorsCheck
            // 
            this.showColorsCheck.AutoSize = true;
            this.showColorsCheck.Checked = true;
            this.showColorsCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showColorsCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showColorsCheck.Location = new System.Drawing.Point(21, 115);
            this.showColorsCheck.Name = "showColorsCheck";
            this.showColorsCheck.Size = new System.Drawing.Size(132, 19);
            this.showColorsCheck.TabIndex = 4;
            this.showColorsCheck.Text = "Show Group Colors";
            this.showColorsCheck.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // showGroupsCheck
            // 
            this.showGroupsCheck.AutoSize = true;
            this.showGroupsCheck.Checked = true;
            this.showGroupsCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGroupsCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showGroupsCheck.Location = new System.Drawing.Point(21, 88);
            this.showGroupsCheck.Name = "showGroupsCheck";
            this.showGroupsCheck.Size = new System.Drawing.Size(100, 19);
            this.showGroupsCheck.TabIndex = 3;
            this.showGroupsCheck.Text = "Show Groups";
            this.showGroupsCheck.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // appListGroupBox
            // 
            this.appListGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.appListGroupBox.Controls.Add(this.showLargeImagesCheck);
            this.appListGroupBox.Controls.Add(this.showGroupsCheck);
            this.appListGroupBox.Controls.Add(this.highlightInstalledCheck);
            this.appListGroupBox.Controls.Add(this.showColorsCheck);
            this.appListGroupBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.appListGroupBox.Location = new System.Drawing.Point(11, 12);
            this.appListGroupBox.Name = "appListGroupBox";
            this.appListGroupBox.Size = new System.Drawing.Size(220, 157);
            this.appListGroupBox.TabIndex = 0;
            this.appListGroupBox.TabStop = false;
            this.appListGroupBox.Text = "Application List";
            // 
            // showLargeImagesCheck
            // 
            this.showLargeImagesCheck.AutoSize = true;
            this.showLargeImagesCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showLargeImagesCheck.Location = new System.Drawing.Point(21, 61);
            this.showLargeImagesCheck.Name = "showLargeImagesCheck";
            this.showLargeImagesCheck.Size = new System.Drawing.Size(134, 19);
            this.showLargeImagesCheck.TabIndex = 2;
            this.showLargeImagesCheck.Text = "Show Large Images";
            this.showLargeImagesCheck.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // groupColorsGroupBox
            // 
            this.groupColorsGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup10);
            this.groupColorsGroupBox.Controls.Add(this.group10ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup1);
            this.groupColorsGroupBox.Controls.Add(this.group1ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup11);
            this.groupColorsGroupBox.Controls.Add(this.group11ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup9);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup8);
            this.groupColorsGroupBox.Controls.Add(this.group9ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup7);
            this.groupColorsGroupBox.Controls.Add(this.group8ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup6);
            this.groupColorsGroupBox.Controls.Add(this.group7ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup5);
            this.groupColorsGroupBox.Controls.Add(this.group6ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup4);
            this.groupColorsGroupBox.Controls.Add(this.group5ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.group4ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup3);
            this.groupColorsGroupBox.Controls.Add(this.group3ColorPanel);
            this.groupColorsGroupBox.Controls.Add(this.resetColorsBtn);
            this.groupColorsGroupBox.Controls.Add(this.listViewGroup2);
            this.groupColorsGroupBox.Controls.Add(this.group2ColorPanel);
            this.groupColorsGroupBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.groupColorsGroupBox.Location = new System.Drawing.Point(237, 12);
            this.groupColorsGroupBox.Name = "groupColorsGroupBox";
            this.groupColorsGroupBox.Size = new System.Drawing.Size(220, 336);
            this.groupColorsGroupBox.TabIndex = 11;
            this.groupColorsGroupBox.TabStop = false;
            this.groupColorsGroupBox.Text = "Group Colors";
            // 
            // listViewGroup10
            // 
            this.listViewGroup10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup10.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup10.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup10.Location = new System.Drawing.Point(8, 232);
            this.listViewGroup10.Name = "listViewGroup10";
            this.listViewGroup10.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup10.TabIndex = 31;
            this.listViewGroup10.Text = "Games:";
            this.listViewGroup10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group10ColorPanel
            // 
            this.group10ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group10ColorPanel.BackColor = System.Drawing.SystemColors.Window;
            this.group10ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group10ColorPanel.Location = new System.Drawing.Point(184, 231);
            this.group10ColorPanel.Name = "group10ColorPanel";
            this.group10ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group10ColorPanel.TabIndex = 32;
            this.group10ColorPanel.TabStop = true;
            this.group10ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            // 
            // listViewGroup1
            // 
            this.listViewGroup1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup1.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup1.Location = new System.Drawing.Point(8, 34);
            this.listViewGroup1.Name = "listViewGroup1";
            this.listViewGroup1.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup1.TabIndex = 12;
            this.listViewGroup1.Text = "Accessibility:";
            this.listViewGroup1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group1ColorPanel
            // 
            this.group1ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group1ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(153)))));
            this.group1ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group1ColorPanel.Location = new System.Drawing.Point(184, 33);
            this.group1ColorPanel.Name = "group1ColorPanel";
            this.group1ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group1ColorPanel.TabIndex = 13;
            this.group1ColorPanel.TabStop = true;
            this.group1ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group1ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group1ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // listViewGroup11
            // 
            this.listViewGroup11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup11.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup11.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup11.Location = new System.Drawing.Point(8, 254);
            this.listViewGroup11.Name = "listViewGroup11";
            this.listViewGroup11.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup11.TabIndex = 33;
            this.listViewGroup11.Text = "*Advanced:";
            this.listViewGroup11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group11ColorPanel
            // 
            this.group11ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group11ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.group11ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group11ColorPanel.Location = new System.Drawing.Point(184, 253);
            this.group11ColorPanel.Name = "group11ColorPanel";
            this.group11ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group11ColorPanel.TabIndex = 34;
            this.group11ColorPanel.TabStop = true;
            this.group11ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group11ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group11ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // listViewGroup9
            // 
            this.listViewGroup9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup9.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup9.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup9.Location = new System.Drawing.Point(8, 210);
            this.listViewGroup9.Name = "listViewGroup9";
            this.listViewGroup9.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup9.TabIndex = 29;
            this.listViewGroup9.Text = "Utilities:";
            this.listViewGroup9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // listViewGroup8
            // 
            this.listViewGroup8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup8.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup8.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup8.Location = new System.Drawing.Point(8, 188);
            this.listViewGroup8.Name = "listViewGroup8";
            this.listViewGroup8.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup8.TabIndex = 27;
            this.listViewGroup8.Text = "Security:";
            this.listViewGroup8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group9ColorPanel
            // 
            this.group9ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group9ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.group9ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group9ColorPanel.Location = new System.Drawing.Point(184, 209);
            this.group9ColorPanel.Name = "group9ColorPanel";
            this.group9ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group9ColorPanel.TabIndex = 30;
            this.group9ColorPanel.TabStop = true;
            this.group9ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group9ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group9ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // listViewGroup7
            // 
            this.listViewGroup7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup7.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup7.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup7.Location = new System.Drawing.Point(8, 166);
            this.listViewGroup7.Name = "listViewGroup7";
            this.listViewGroup7.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup7.TabIndex = 25;
            this.listViewGroup7.Text = "Music and Video:";
            this.listViewGroup7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group8ColorPanel
            // 
            this.group8ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group8ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(255)))), ((int)(((byte)(153)))));
            this.group8ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group8ColorPanel.Location = new System.Drawing.Point(184, 187);
            this.group8ColorPanel.Name = "group8ColorPanel";
            this.group8ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group8ColorPanel.TabIndex = 28;
            this.group8ColorPanel.TabStop = true;
            this.group8ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group8ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group8ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // listViewGroup6
            // 
            this.listViewGroup6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup6.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup6.Location = new System.Drawing.Point(8, 144);
            this.listViewGroup6.Name = "listViewGroup6";
            this.listViewGroup6.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup6.TabIndex = 22;
            this.listViewGroup6.Text = "Graphics and Pictures:";
            this.listViewGroup6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group7ColorPanel
            // 
            this.group7ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group7ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.group7ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group7ColorPanel.Location = new System.Drawing.Point(184, 165);
            this.group7ColorPanel.Name = "group7ColorPanel";
            this.group7ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group7ColorPanel.TabIndex = 26;
            this.group7ColorPanel.TabStop = true;
            this.group7ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group7ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group7ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // listViewGroup5
            // 
            this.listViewGroup5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup5.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup5.Location = new System.Drawing.Point(8, 122);
            this.listViewGroup5.Name = "listViewGroup5";
            this.listViewGroup5.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup5.TabIndex = 20;
            this.listViewGroup5.Text = "Internet:";
            this.listViewGroup5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group6ColorPanel
            // 
            this.group6ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group6ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.group6ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group6ColorPanel.Location = new System.Drawing.Point(184, 143);
            this.group6ColorPanel.Name = "group6ColorPanel";
            this.group6ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group6ColorPanel.TabIndex = 23;
            this.group6ColorPanel.TabStop = true;
            this.group6ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group6ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group6ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // listViewGroup4
            // 
            this.listViewGroup4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup4.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup4.Location = new System.Drawing.Point(8, 100);
            this.listViewGroup4.Name = "listViewGroup4";
            this.listViewGroup4.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup4.TabIndex = 18;
            this.listViewGroup4.Text = "Office:";
            this.listViewGroup4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group5ColorPanel
            // 
            this.group5ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group5ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(217)))), ((int)(((byte)(206)))));
            this.group5ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group5ColorPanel.Location = new System.Drawing.Point(184, 121);
            this.group5ColorPanel.Name = "group5ColorPanel";
            this.group5ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group5ColorPanel.TabIndex = 21;
            this.group5ColorPanel.TabStop = true;
            this.group5ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group5ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group5ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // group4ColorPanel
            // 
            this.group4ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group4ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(233)))), ((int)(((byte)(236)))));
            this.group4ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group4ColorPanel.Location = new System.Drawing.Point(184, 99);
            this.group4ColorPanel.Name = "group4ColorPanel";
            this.group4ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group4ColorPanel.TabIndex = 19;
            this.group4ColorPanel.TabStop = true;
            this.group4ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group4ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group4ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // listViewGroup3
            // 
            this.listViewGroup3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup3.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup3.Location = new System.Drawing.Point(8, 78);
            this.listViewGroup3.Name = "listViewGroup3";
            this.listViewGroup3.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup3.TabIndex = 16;
            this.listViewGroup3.Text = "Development:";
            this.listViewGroup3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group3ColorPanel
            // 
            this.group3ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group3ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(223)))));
            this.group3ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group3ColorPanel.Location = new System.Drawing.Point(184, 77);
            this.group3ColorPanel.Name = "group3ColorPanel";
            this.group3ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group3ColorPanel.TabIndex = 17;
            this.group3ColorPanel.TabStop = true;
            this.group3ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group3ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group3ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // resetColorsBtn
            // 
            this.resetColorsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetColorsBtn.BackColor = System.Drawing.SystemColors.Control;
            this.resetColorsBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.resetColorsBtn.Location = new System.Drawing.Point(125, 283);
            this.resetColorsBtn.Name = "resetColorsBtn";
            this.resetColorsBtn.Size = new System.Drawing.Size(75, 30);
            this.resetColorsBtn.TabIndex = 12;
            this.resetColorsBtn.Text = "Reset";
            this.resetColorsBtn.UseVisualStyleBackColor = false;
            this.resetColorsBtn.Click += new System.EventHandler(this.ResetColorsBtn_Click);
            // 
            // listViewGroup2
            // 
            this.listViewGroup2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup2.BackColor = System.Drawing.Color.Transparent;
            this.listViewGroup2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewGroup2.Location = new System.Drawing.Point(8, 56);
            this.listViewGroup2.Name = "listViewGroup2";
            this.listViewGroup2.Size = new System.Drawing.Size(170, 13);
            this.listViewGroup2.TabIndex = 14;
            this.listViewGroup2.Text = "Education:";
            this.listViewGroup2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // group2ColorPanel
            // 
            this.group2ColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.group2ColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(204)))));
            this.group2ColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.group2ColorPanel.Location = new System.Drawing.Point(184, 55);
            this.group2ColorPanel.Name = "group2ColorPanel";
            this.group2ColorPanel.Size = new System.Drawing.Size(16, 16);
            this.group2ColorPanel.TabIndex = 15;
            this.group2ColorPanel.TabStop = true;
            this.group2ColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
            this.group2ColorPanel.MouseEnter += new System.EventHandler(this.ColorPanel_MouseEnter);
            this.group2ColorPanel.MouseLeave += new System.EventHandler(this.ColorPanel_MouseLeave);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 150);
            // 
            // transferGroupBox
            // 
            this.transferGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transferGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.transferGroupBox.Controls.Add(this.transferPathPanel);
            this.transferGroupBox.Controls.Add(this.transferPathUndoBtn);
            this.transferGroupBox.Controls.Add(this.transferPathBtn);
            this.transferGroupBox.Location = new System.Drawing.Point(11, 355);
            this.transferGroupBox.Name = "transferGroupBox";
            this.transferGroupBox.Size = new System.Drawing.Size(446, 70);
            this.transferGroupBox.TabIndex = 5;
            this.transferGroupBox.TabStop = false;
            this.transferGroupBox.Text = "Transfer Path";
            // 
            // transferPathPanel
            // 
            this.transferPathPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transferPathPanel.BackColor = System.Drawing.SystemColors.Control;
            this.transferPathPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.transferPathPanel.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.transferPathPanel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transferPathPanel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.transferPathPanel.Location = new System.Drawing.Point(23, 26);
            this.transferPathPanel.Name = "transferPathPanel";
            this.transferPathPanel.Size = new System.Drawing.Size(326, 28);
            this.transferPathPanel.TabIndex = 6;
            this.transferPathPanel.Text = "%AppsSuiteDir%\\Documents\\Cache\\Transfer";
            this.transferPathPanel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // transferPathUndoBtn
            // 
            this.transferPathUndoBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.transferPathUndoBtn.BackColor = System.Drawing.SystemColors.Control;
            this.transferPathUndoBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.transferPathUndoBtn.Image = global::AppsDownloader.Properties.Resources.Undo;
            this.transferPathUndoBtn.Location = new System.Drawing.Point(394, 24);
            this.transferPathUndoBtn.Name = "transferPathUndoBtn";
            this.transferPathUndoBtn.Size = new System.Drawing.Size(32, 32);
            this.transferPathUndoBtn.TabIndex = 8;
            this.transferPathUndoBtn.UseVisualStyleBackColor = false;
            this.transferPathUndoBtn.Click += new System.EventHandler(this.TransferPathUndoBtn_Click);
            // 
            // transferPathBtn
            // 
            this.transferPathBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.transferPathBtn.BackColor = System.Drawing.SystemColors.Control;
            this.transferPathBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.transferPathBtn.Image = global::AppsDownloader.Properties.Resources.Folder;
            this.transferPathBtn.Location = new System.Drawing.Point(357, 24);
            this.transferPathBtn.Name = "transferPathBtn";
            this.transferPathBtn.Size = new System.Drawing.Size(32, 32);
            this.transferPathBtn.TabIndex = 7;
            this.transferPathBtn.UseVisualStyleBackColor = false;
            this.transferPathBtn.Click += new System.EventHandler(this.TransferPathBtn_Click);
            // 
            // logoBox
            // 
            this.logoBox.BackColor = System.Drawing.Color.Transparent;
            this.logoBox.BackgroundImage = global::AppsDownloader.Properties.Resources.PaLogoNegative;
            this.logoBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.logoBox.Location = new System.Drawing.Point(21, 186);
            this.logoBox.Name = "logoBox";
            this.logoBox.Size = new System.Drawing.Size(196, 161);
            this.logoBox.TabIndex = 12;
            this.logoBox.TabStop = false;
            // 
            // openSrcManBtn
            // 
            this.openSrcManBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openSrcManBtn.BackColor = System.Drawing.SystemColors.Control;
            this.openSrcManBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.openSrcManBtn.Location = new System.Drawing.Point(20, 29);
            this.openSrcManBtn.Name = "openSrcManBtn";
            this.openSrcManBtn.Size = new System.Drawing.Size(405, 30);
            this.openSrcManBtn.TabIndex = 10;
            this.openSrcManBtn.Text = "Open the Apps Source Manager";
            this.openSrcManBtn.UseVisualStyleBackColor = false;
            this.openSrcManBtn.Click += new System.EventHandler(this.OpenSrcManBtn_Click);
            // 
            // advancedGroupBox
            // 
            this.advancedGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.advancedGroupBox.Controls.Add(this.openSrcManBtn);
            this.advancedGroupBox.Location = new System.Drawing.Point(12, 431);
            this.advancedGroupBox.Name = "advancedGroupBox";
            this.advancedGroupBox.Size = new System.Drawing.Size(446, 81);
            this.advancedGroupBox.TabIndex = 9;
            this.advancedGroupBox.TabStop = false;
            this.advancedGroupBox.Text = "*Custom";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(469, 525);
            this.Controls.Add(this.advancedGroupBox);
            this.Controls.Add(this.logoBox);
            this.Controls.Add(this.transferGroupBox);
            this.Controls.Add(this.groupColorsGroupBox);
            this.Controls.Add(this.appListGroupBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(485, 564);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.appListGroupBox.ResumeLayout(false);
            this.appListGroupBox.PerformLayout();
            this.groupColorsGroupBox.ResumeLayout(false);
            this.transferGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).EndInit();
            this.advancedGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox highlightInstalledCheck;
        private System.Windows.Forms.CheckBox showColorsCheck;
        private System.Windows.Forms.CheckBox showGroupsCheck;
        private System.Windows.Forms.GroupBox appListGroupBox;
        private System.Windows.Forms.CheckBox showLargeImagesCheck;
        private System.Windows.Forms.GroupBox groupColorsGroupBox;
        private System.Windows.Forms.Button resetColorsBtn;
        private System.Windows.Forms.Label listViewGroup2;
        private System.Windows.Forms.Panel group2ColorPanel;
        private System.Windows.Forms.Label listViewGroup3;
        private System.Windows.Forms.Panel group3ColorPanel;
        private System.Windows.Forms.Label listViewGroup9;
        private System.Windows.Forms.Label listViewGroup8;
        private System.Windows.Forms.Panel group9ColorPanel;
        private System.Windows.Forms.Label listViewGroup7;
        private System.Windows.Forms.Panel group8ColorPanel;
        private System.Windows.Forms.Label listViewGroup6;
        private System.Windows.Forms.Panel group7ColorPanel;
        private System.Windows.Forms.Label listViewGroup5;
        private System.Windows.Forms.Panel group6ColorPanel;
        private System.Windows.Forms.Label listViewGroup4;
        private System.Windows.Forms.Panel group5ColorPanel;
        private System.Windows.Forms.Panel group4ColorPanel;
        private System.Windows.Forms.Label listViewGroup11;
        private System.Windows.Forms.Panel group11ColorPanel;
        private System.Windows.Forms.Label listViewGroup1;
        private System.Windows.Forms.Panel group1ColorPanel;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.GroupBox transferGroupBox;
        private System.Windows.Forms.Button transferPathBtn;
        private System.Windows.Forms.PictureBox logoBox;
        private System.Windows.Forms.Button transferPathUndoBtn;
        private System.Windows.Forms.Button openSrcManBtn;
        private System.Windows.Forms.GroupBox advancedGroupBox;
        private System.Windows.Forms.Label listViewGroup10;
        private System.Windows.Forms.Panel group10ColorPanel;
        private System.Windows.Forms.Label transferPathPanel;
    }
}