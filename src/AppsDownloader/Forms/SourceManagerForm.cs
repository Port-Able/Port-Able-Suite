namespace AppsDownloader
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Media;
    using System.Windows.Forms;
    using PortAble;
    using PortAble.Properties;
    using Properties;
    using SilDev;
    using SilDev.Drawing;
    using SilDev.Forms;

    public partial class SourceManagerForm : Form
    {
        private DialogResult _result = DialogResult.No;

        public SourceManagerForm()
        {
            InitializeComponent();

            if (Desktop.AppsUseDarkTheme)
            {
                this.EnableDarkMode();
                this.ChangeColorMode();
                srcMngDataGridView.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                srcMngDataGridView.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
            }

            SuspendLayout();

            Icon = Resources.PaLogoGreenSymbol;
            cancelBtn.Click += (_, _) => Close();

            ResumeLayout(false);
        }

        private void SourceManagerForm_Load(object sender, EventArgs e)
        {
            if (CacheData.CustomAppSuppliers.Count > 0)
            {
                foreach (var data in CacheData.CustomAppSuppliers)
                {
                    if (string.IsNullOrWhiteSpace(data.Address))
                        continue;
                    var i = srcMngDataGridView.Rows.Add();
                    srcMngDataGridView.Rows[i].Cells[nameof(Address)].Value = data.Address;
                    srcMngDataGridView.Rows[i].Cells[nameof(User)].Value = data.User;
                    srcMngDataGridView.Rows[i].Cells[nameof(Password)].Value = data.Password;
                    srcMngDataGridView.Rows[i].Cells[nameof(UserAgent)].Value = data.UserAgent;
                }
                return;
            }
            MessageBoxEx.Show(this, LangStrings.SourceManagerWarnMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SourceManagerForm_FormClosed(object sender, FormClosedEventArgs e) =>
            DialogResult = _result;

        private void SourceManagerForm_ResizeEnd(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Normal || this.LayoutIsSuspended())
                return;
            SuspendLayout();
            MaximumSize = SizeEx.GetDesktopSize(Location);
            Height = MinimumSize.Height;
            foreach (var row in srcMngDataGridView.Rows.Cast<DataGridViewRow>())
            {
                var rowHeight = row.Height;
                var newFormHeight = Height + rowHeight;
                if (MaximumSize.Height < newFormHeight ||
                    MinimumSize.Height > newFormHeight)
                    break;
                Height = newFormHeight;
            }
            WinApi.NativeHelper.CenterWindow(Handle);
            ResumeLayout(true);
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var suppliers = (IList<CustomAppSupplier>)CacheData.CustomAppSuppliers;
            foreach (var supplier in suppliers)
                supplier.RemoveFile();
            suppliers.Clear();

            foreach (var row in srcMngDataGridView.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow))
            {
                if (row.Cells.Count < 1 ||
                    row.Cells[nameof(Address)].Value is not string address ||
                    !address.StartsWithEx("http://", "https://") ||
                    address.All(c => c != '.'))
                    continue;

                var data = new CustomAppSupplier(address.TrimEnd(' ', '\v', '\r', '\n', '/'),
                                                 row.Cells[nameof(User)].Value as string,
                                                 row.Cells[nameof(Password)].Value as string,
                                                 row.Cells[nameof(UserAgent)].Value as string);

                if (!data.IsValid())
                    MessageBoxEx.Show(LangStrings.CustomAppSupplierInvalidMsg.FormatInvariant(address), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    _result = DialogResult.Yes;

                data.SaveToFile(true);
            }

            var dir = CorePaths.CustomAppSuppliersDir;
            if (DirectoryEx.EnumerateFiles(dir)?.Any() == true)
                foreach (var supplier in DirectoryEx.EnumerateFiles(dir).Select(f => CacheData.LoadDat<CustomAppSupplier>(f)).Where(x => x != default))
                    suppliers.Add(supplier);

            Close();
        }

        private void SrcMngDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView owner || !e.RowIndex.IsBetween(0, owner.Rows.Count - 1))
                return;
            owner.SelectedRows.Clear();
            owner.Rows[e.RowIndex].Selected = true;
        }

        private void SrcMngDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not DataGridView owner)
                return;
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    owner.BeginEdit(true);
                    foreach (var row in owner.SelectedRows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow))
                        owner.Rows.Remove(row);
                    owner.EndEdit();
                    break;
                case Keys.C:
                    SystemSounds.Asterisk.Play();
                    break;
                case Keys.D:
                    if (!e.Control)
                        return;
                    owner.BeginEdit(true);
                    owner.AllowUserToAddRows = false;
                    foreach (var row in owner.SelectedRows.Cast<DataGridViewRow>())
                    {
                        var clone = (DataGridViewRow)row.Clone();
                        if (clone == null)
                            continue;
                        for (var i = 0; i < clone.Cells.Count; i++)
                            clone.Cells[i].Value = row.Cells[i].Value;
                        owner.Rows.Add(clone);
                    }
                    owner.AllowUserToAddRows = true;
                    owner.EndEdit();
                    SystemSounds.Asterisk.Play();
                    break;
                case Keys.V:
                    if (!e.Control || !Clipboard.ContainsText(TextDataFormat.Text))
                        return;
                    owner.BeginEdit(true);
                    owner.AllowUserToAddRows = false;

                    var nrow = new DataGridViewRow();
                    nrow.CreateCells(owner);

                    var str = Clipboard.GetText(TextDataFormat.Text).Trim();
                    try
                    {
                        var cells = str.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);
                        for (var i = 0; i < cells.Length; i++)
                        {
                            if (i > nrow.Cells.Count - 1)
                                break;
                            nrow.Cells[i].Value = cells[i].Trim();
                        }
                    }
                    catch (Exception ex) when (ex.IsCaught())
                    {
                        if (Log.DebugMode > 1)
                            Log.Write(ex);
                        nrow.Cells[0].Value = str;
                    }

                    var selected = owner.SelectedRows;
                    if (selected.Count > 0)
                    {
                        var last = selected.Count - 1;
                        for (var i = 0; i < 4; i++)
                        {
                            var cell = nrow.Cells[i].Value as string;
                            if (string.IsNullOrEmpty(cell))
                                continue;
                            selected[last].Cells[i].Value = cell;
                        }
                    }
                    else
                        owner.Rows.Add(nrow);

                    owner.AllowUserToAddRows = true;
                    owner.EndEdit();
                    SystemSounds.Asterisk.Play();
                    break;
            }
        }

        private void SrcMngDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) =>
            SrcMngDataGridView_RowsChanged(sender, true);

        private void SrcMngDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) =>
            SrcMngDataGridView_RowsChanged(sender, false);

        private void SrcMngDataGridView_RowsChanged(object sender, bool increase)
        {
            if (sender is not DataGridView owner || WindowState != FormWindowState.Normal || this.LayoutIsSuspended())
                return;
            SuspendLayout();
            MaximumSize = SizeEx.GetDesktopSize(Location);
            var firstRow = owner.Rows.Cast<DataGridViewRow>().FirstOrDefault();
            if (firstRow != null)
            {
                var rowHeight = firstRow.Height;
                var newFormHeight = increase ? Height + rowHeight : Height - rowHeight;
                if (newFormHeight.IsBetween(MinimumSize.Height, MaximumSize.Height))
                {
                    Height = newFormHeight;
                    var newFormTop = increase ? Top - rowHeight / 2 : Top + rowHeight / 2;
                    if (newFormTop.IsBetween(0, MaximumSize.Height))
                        Top = newFormTop;
                    ResumeLayout(true);
                }
            }
            ResumeLayout(true);
        }
    }
}
