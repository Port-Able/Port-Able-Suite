namespace AppsDownloader.Windows
{
    partial class LangSelectionForm
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
            this.langBox = new System.Windows.Forms.ComboBox();
            this.rememberLangCheck = new System.Windows.Forms.CheckBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.appNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // langBox
            // 
            this.langBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.langBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.langBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.langBox.FormattingEnabled = true;
            this.langBox.Location = new System.Drawing.Point(13, 37);
            this.langBox.Name = "langBox";
            this.langBox.Size = new System.Drawing.Size(223, 23);
            this.langBox.TabIndex = 0;
            // 
            // rememberLangCheck
            // 
            this.rememberLangCheck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rememberLangCheck.BackColor = System.Drawing.Color.Transparent;
            this.rememberLangCheck.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rememberLangCheck.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rememberLangCheck.Location = new System.Drawing.Point(15, 68);
            this.rememberLangCheck.Name = "rememberLangCheck";
            this.rememberLangCheck.Size = new System.Drawing.Size(221, 17);
            this.rememberLangCheck.TabIndex = 1;
            this.rememberLangCheck.Text = "Do not ask again for this application";
            this.rememberLangCheck.UseVisualStyleBackColor = false;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(20, 93);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(99, 23);
            this.okBtn.TabIndex = 2;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(129, 93);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(99, 23);
            this.cancelBtn.TabIndex = 3;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // appNameLabel
            // 
            this.appNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.appNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.appNameLabel.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Bold);
            this.appNameLabel.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.appNameLabel.Location = new System.Drawing.Point(0, 0);
            this.appNameLabel.Name = "appNameLabel";
            this.appNameLabel.Size = new System.Drawing.Size(248, 28);
            this.appNameLabel.TabIndex = 4;
            this.appNameLabel.Text = "Application Name";
            this.appNameLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // LangSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Gray;
            this.BackgroundImage = global::AppsDownloader.Properties.Resources.diagonal_pattern;
            this.ClientSize = new System.Drawing.Size(248, 136);
            this.Controls.Add(this.appNameLabel);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.rememberLangCheck);
            this.Controls.Add(this.langBox);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(264, 175);
            this.Name = "LangSelectionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Language Selection:";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LangSelectionForm_Load);
            this.Shown += new System.EventHandler(this.SetArchiveLangForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox langBox;
        private System.Windows.Forms.CheckBox rememberLangCheck;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label appNameLabel;
    }
}