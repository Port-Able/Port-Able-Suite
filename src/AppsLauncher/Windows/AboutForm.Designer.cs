namespace AppsLauncher.Windows
{
    partial class AboutForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.logoBox = new System.Windows.Forms.PictureBox();
            this.updateChecker = new System.ComponentModel.BackgroundWorker();
            this.closeToUpdate = new System.Windows.Forms.Timer(this.components);
            this.logoPanel = new System.Windows.Forms.Panel();
            this.spaceChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.leftBorderPanel = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.updateBtnPanel = new System.Windows.Forms.Panel();
            this.updateBtn = new System.Windows.Forms.Button();
            this.aboutInfoLabel = new System.Windows.Forms.LinkLabel();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.diskChartUpdater = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).BeginInit();
            this.logoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spaceChart)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.updateBtnPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // logoBox
            // 
            this.logoBox.BackColor = System.Drawing.Color.Transparent;
            this.logoBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.logoBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.logoBox.Location = new System.Drawing.Point(0, 0);
            this.logoBox.Name = "logoBox";
            this.logoBox.Size = new System.Drawing.Size(163, 145);
            this.logoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.logoBox.TabIndex = 0;
            this.logoBox.TabStop = false;
            // 
            // updateChecker
            // 
            this.updateChecker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.UpdateChecker_DoWork);
            this.updateChecker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UpdateChecker_RunWorkerCompleted);
            // 
            // closeToUpdate
            // 
            this.closeToUpdate.Interval = 1;
            this.closeToUpdate.Tick += new System.EventHandler(this.CloseToUpdate_Tick);
            // 
            // logoPanel
            // 
            this.logoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(64)))));
            this.logoPanel.Controls.Add(this.spaceChart);
            this.logoPanel.Controls.Add(this.logoBox);
            this.logoPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.logoPanel.Location = new System.Drawing.Point(0, 0);
            this.logoPanel.Name = "logoPanel";
            this.logoPanel.Size = new System.Drawing.Size(163, 181);
            this.logoPanel.TabIndex = 19;
            // 
            // spaceChart
            // 
            this.spaceChart.BackColor = System.Drawing.Color.Transparent;
            this.spaceChart.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisY.IsMarginVisible = false;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.BorderColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "chartArea";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 50F;
            chartArea1.Position.Width = 94F;
            chartArea1.Position.X = 3F;
            chartArea1.Position.Y = 25F;
            chartArea1.ShadowColor = System.Drawing.Color.Transparent;
            this.spaceChart.ChartAreas.Add(chartArea1);
            this.spaceChart.Dock = System.Windows.Forms.DockStyle.Bottom;
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.AutoFitMinFontSize = 5;
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend1.ForeColor = System.Drawing.Color.White;
            legend1.IsTextAutoFit = false;
            legend1.ItemColumnSpacing = 0;
            legend1.LegendItemOrder = System.Windows.Forms.DataVisualization.Charting.LegendItemOrder.ReversedSeriesOrder;
            legend1.Name = "chartLegend";
            legend1.ShadowColor = System.Drawing.Color.Transparent;
            legend1.TitleFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spaceChart.Legends.Add(legend1);
            this.spaceChart.Location = new System.Drawing.Point(0, -85);
            this.spaceChart.Margin = new System.Windows.Forms.Padding(0);
            this.spaceChart.Name = "spaceChart";
            this.spaceChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.spaceChart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.ForestGreen,
        System.Drawing.Color.IndianRed,
        System.Drawing.Color.Firebrick};
            series1.BackImageAlignment = System.Windows.Forms.DataVisualization.Charting.ChartImageAlignmentStyle.Center;
            series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            series1.BorderWidth = 2;
            series1.ChartArea = "chartArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.LabelBackColor = System.Drawing.Color.Transparent;
            series1.LabelBorderColor = System.Drawing.Color.Transparent;
            series1.LabelBorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            series1.LabelForeColor = System.Drawing.Color.White;
            series1.Legend = "chartLegend";
            series1.Name = "chartSeries";
            series1.YValuesPerPoint = 6;
            this.spaceChart.Series.Add(series1);
            this.spaceChart.Size = new System.Drawing.Size(163, 266);
            this.spaceChart.TabIndex = 12;
            // 
            // leftBorderPanel
            // 
            this.leftBorderPanel.BackColor = System.Drawing.Color.Black;
            this.leftBorderPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftBorderPanel.Location = new System.Drawing.Point(163, 0);
            this.leftBorderPanel.Name = "leftBorderPanel";
            this.leftBorderPanel.Size = new System.Drawing.Size(1, 181);
            this.leftBorderPanel.TabIndex = 20;
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(64)))));
            this.mainPanel.BackgroundImage = global::AppsLauncher.Properties.Resources.horizontal_pattern;
            this.mainPanel.Controls.Add(this.updateBtnPanel);
            this.mainPanel.Controls.Add(this.aboutInfoLabel);
            this.mainPanel.Controls.Add(this.copyrightLabel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(164, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(412, 181);
            this.mainPanel.TabIndex = 21;
            // 
            // updateBtnPanel
            // 
            this.updateBtnPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateBtnPanel.BackColor = System.Drawing.Color.Transparent;
            this.updateBtnPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.updateBtnPanel.Controls.Add(this.updateBtn);
            this.updateBtnPanel.Location = new System.Drawing.Point(20, 29);
            this.updateBtnPanel.Name = "updateBtnPanel";
            this.updateBtnPanel.Size = new System.Drawing.Size(130, 23);
            this.updateBtnPanel.TabIndex = 23;
            // 
            // updateBtn
            // 
            this.updateBtn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.updateBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateBtn.FlatAppearance.BorderSize = 0;
            this.updateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.updateBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.updateBtn.Location = new System.Drawing.Point(0, 0);
            this.updateBtn.Name = "updateBtn";
            this.updateBtn.Size = new System.Drawing.Size(128, 21);
            this.updateBtn.TabIndex = 23;
            this.updateBtn.Text = "Check for updates";
            this.updateBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.updateBtn.UseVisualStyleBackColor = false;
            this.updateBtn.Click += new System.EventHandler(this.UpdateBtn_Click);
            // 
            // aboutInfoLabel
            // 
            this.aboutInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.aboutInfoLabel.BackColor = System.Drawing.Color.Transparent;
            this.aboutInfoLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.aboutInfoLabel.Font = new System.Drawing.Font("Tahoma", 7.25F);
            this.aboutInfoLabel.ForeColor = System.Drawing.Color.SlateGray;
            this.aboutInfoLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.aboutInfoLabel.LinkColor = System.Drawing.Color.PowderBlue;
            this.aboutInfoLabel.Location = new System.Drawing.Point(14, 64);
            this.aboutInfoLabel.Name = "aboutInfoLabel";
            this.aboutInfoLabel.Size = new System.Drawing.Size(384, 79);
            this.aboutInfoLabel.TabIndex = 26;
            this.aboutInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.aboutInfoLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AboutInfoLabel_LinkClicked);
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.BackColor = System.Drawing.Color.Transparent;
            this.copyrightLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.copyrightLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(85)))));
            this.copyrightLabel.Location = new System.Drawing.Point(0, 159);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(412, 22);
            this.copyrightLabel.TabIndex = 25;
            this.copyrightLabel.Text = "Copyright © Si13n7 Dev. ® {0}";
            this.copyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // diskChartUpdater
            // 
            this.diskChartUpdater.WorkerReportsProgress = true;
            this.diskChartUpdater.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DiskChartUpdater_DoWork);
            this.diskChartUpdater.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.DiskChartUpdater_ProgressChanged);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(576, 181);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.leftBorderPanel);
            this.Controls.Add(this.logoPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Silver;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(592, 220);
            this.Name = "AboutForm";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Portable Apps Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AboutForm_FormClosing);
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.Shown += new System.EventHandler(this.AboutForm_Shown);

        }

        #endregion

        private System.Windows.Forms.PictureBox logoBox;
        private System.ComponentModel.BackgroundWorker updateChecker;
        private System.Windows.Forms.Timer closeToUpdate;
        private System.Windows.Forms.Panel logoPanel;
        private System.Windows.Forms.Label leftBorderPanel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.LinkLabel aboutInfoLabel;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.Button updateBtn;
        private System.Windows.Forms.Panel updateBtnPanel;
        private System.Windows.Forms.DataVisualization.Charting.Chart spaceChart;
        private System.ComponentModel.BackgroundWorker diskChartUpdater;
    }
}