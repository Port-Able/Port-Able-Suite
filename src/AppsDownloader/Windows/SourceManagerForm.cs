namespace AppsDownloader
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using Libraries;
    using SilDev;

    public partial class SourceManagerForm : Form
    {
        public SourceManagerForm()
        {
            InitializeComponent();

            Language.SetControlLang(this);
            Address.HeaderText = Language.GetText(Address.Name);
            User.HeaderText = Language.GetText(User.Name);
            Password.HeaderText = Language.GetText(Password.Name);

            Icon = CacheData.GetSystemIcon(ResourcesEx.IconIndex.Sharing);

            cancelBtn.Click += (s, e) => Close();

            ((ISupportInitialize)srcMngDataGridView).EndInit();
            ResumeLayout(false);
        }

        private void SourceManagerForm_Load(object sender, EventArgs e)
        {
            foreach (var srv in Shareware.GetAddresses())
            {
                if (string.IsNullOrWhiteSpace(srv))
                    continue;
                var i = srcMngDataGridView.Rows.Add();
                srcMngDataGridView.Rows[i].Cells["Address"].Value = srv;
                var usr = Shareware.GetUser(srv);
                if (!string.IsNullOrWhiteSpace(srv))
                    srcMngDataGridView.Rows[i].Cells["User"].Value = usr;
                var pwd = Shareware.GetPassword(srv);
                if (!string.IsNullOrWhiteSpace(pwd))
                    srcMngDataGridView.Rows[i].Cells["Password"].Value = pwd;
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var srcData = new Dictionary<string, Tuple<string, string>>();
            foreach (var row in srcMngDataGridView.Rows.Cast<DataGridViewRow>())
            {
                if (!(row.Cells["Address"].Value is string srv) || !srv.StartsWithEx("http://", "https://") || srv.Count(c => c == '.') < 1)
                    continue;
                srv = Shareware.Encrypt(srv);
                var usr = row.Cells["User"].Value as string ?? string.Empty;
                if (!string.IsNullOrEmpty(usr))
                    usr = Shareware.Encrypt(usr);
                var pwd = row.Cells["Password"].Value as string ?? string.Empty;
                if (!string.IsNullOrEmpty(pwd))
                    pwd = Shareware.Encrypt(pwd);
                srcData[srv] = Tuple.Create(usr, pwd);
            }
            Shareware.Data = srcData;
            Close();
        }
    }
}
