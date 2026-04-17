using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.Forms
{
    public class NganhForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtMaNganh;
        private readonly TextBox txtTenNganh;
        private readonly ComboBox cmbMaKhoa;
        private readonly TextBox txtSearch;

        public NganhForm()
        {
            Text = "Quản lý Ngành học";
            Width = 900;
            Height = 640;

            dgv = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 350,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 250, Padding = new Padding(10) };

            txtMaNganh  = CreateLabeledTextBox(panel, "Mã ngành",  10);
            txtTenNganh = CreateLabeledTextBox(panel, "Tên ngành", 80);

            panel.Controls.Add(new Label { Text = "Khoa", Top = 150, Left = 10, AutoSize = true });
            cmbMaKhoa = new ComboBox { Top = 170, Left = 10, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbMaKhoa);

            var lblSearch = new Label { Text = "Tìm kiếm", Top = 10, Left = 500, AutoSize = true };
            txtSearch = new TextBox { Top = 30, Left = 500, Width = 220 };
            panel.Controls.Add(lblSearch);
            panel.Controls.Add(txtSearch);

            var btnSearch  = new Button { Text = "Tìm kiếm", Top = 60,  Left = 500, Width = 100 };
            var btnAdd     = new Button { Text = "Thêm",     Top = 100, Left = 500, Width = 100 };
            var btnUpdate  = new Button { Text = "Cập nhật", Top = 140, Left = 500, Width = 100 };
            var btnDelete  = new Button { Text = "Xóa",      Top = 180, Left = 500, Width = 100 };
            var btnRefresh = new Button { Text = "Làm mới",  Top = 60,  Left = 620, Width = 100 };

            btnSearch.Click  += (s, e) => LoadData(txtSearch.Text.Trim());
            btnAdd.Click     += BtnAdd_Click;
            btnUpdate.Click  += BtnUpdate_Click;
            btnDelete.Click  += BtnDelete_Click;
            btnRefresh.Click += (s, e) => { ClearForm(); LoadData(); };

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh });
            Controls.Add(panel);
            Controls.Add(dgv);

            Load += NganhForm_Load;
        }

        private void NganhForm_Load(object sender, EventArgs e)
        {
            cmbMaKhoa.DisplayMember = "TenKhoa";
            cmbMaKhoa.ValueMember   = "MaKhoa";
            cmbMaKhoa.DataSource    = CommonDAL.LoadLookup("sp_LayDanhSachKhoa");
            LoadData();
        }

        private TextBox CreateLabeledTextBox(Control parent, string label, int top)
        {
            parent.Controls.Add(new Label { Text = label, Top = top, Left = 10, AutoSize = true });
            var tb = new TextBox { Top = top + 20, Left = 10, Width = 250 };
            parent.Controls.Add(tb);
            return tb;
        }

        private void LoadData(string keyword = null)
        {
            try { dgv.DataSource = NganhBUS.LoadAll(keyword); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow?.DataBoundItem is DataRowView row)
            {
                txtMaNganh.Text          = row["MaNganh"]?.ToString();
                txtTenNganh.Text         = row["TenNganh"]?.ToString();
                cmbMaKhoa.SelectedValue  = row["MaKhoa"]?.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var r = NganhBUS.Add(txtMaNganh.Text, txtTenNganh.Text, cmbMaKhoa.SelectedValue?.ToString());
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var r = NganhBUS.Update(txtMaNganh.Text, txtTenNganh.Text, cmbMaKhoa.SelectedValue?.ToString());
            Show(r); if (r.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa ngành này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var r = NganhBUS.Delete(txtMaNganh.Text);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void ClearForm() { txtMaNganh.Text = txtTenNganh.Text = string.Empty; }
        private void Show(Models.OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
