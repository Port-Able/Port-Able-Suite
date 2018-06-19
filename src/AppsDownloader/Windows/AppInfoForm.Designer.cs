namespace AppsDownloader.Windows
{
    partial class AppInfoForm
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
            this.infoBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // infoBox
            // 
            this.infoBox.BackColor = System.Drawing.SystemColors.Window;
            this.infoBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.infoBox.DetectUrls = false;
            this.infoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoBox.Enabled = false;
            this.infoBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.infoBox.Location = new System.Drawing.Point(0, 0);
            this.infoBox.Margin = new System.Windows.Forms.Padding(3, 23, 3, 23);
            this.infoBox.Name = "infoBox";
            this.infoBox.ReadOnly = true;
            this.infoBox.Size = new System.Drawing.Size(915, 647);
            this.infoBox.TabIndex = 0;
            this.infoBox.Text = "";
            this.infoBox.WordWrap = false;
            this.infoBox.Click += new System.EventHandler(this.InfoBox_HideCaret);
            this.infoBox.EnabledChanged += new System.EventHandler(this.InfoBox_HideCaret);
            this.infoBox.SizeChanged += new System.EventHandler(this.InfoBox_HideCaret);
            this.infoBox.VisibleChanged += new System.EventHandler(this.InfoBox_HideCaret);
            this.infoBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.InfoBox_HideCaret);
            // 
            // AppInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(915, 647);
            this.Controls.Add(this.infoBox);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MinimizeBox = false;
            this.Name = "AppInfoForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.InfoForm_Load);
            this.Shown += new System.EventHandler(this.InfoForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox infoBox;
    }
}