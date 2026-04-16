using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class MonHocForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtMaMon, txtTenMon, txtSearch;
        private readonly NumericUpDown nudSoTinChi;
        private readonly ComboBox cmbMonTienQuyet;

        public MonHocForm()
        {
            Text = "Quản lý môn học";
            Width = 980;
            Height = 700;

            dgv = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 350,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 280, Padding = new Padding(10) };

            panel.Controls.Add(new Label { Text = "Mã môn", Top = 10, Left = 10, AutoSize = true });
            txtMaMon = new TextBox { Top = 30, Left = 10, Width = 200 };
            panel.Controls.Add(txtMaMon);

            panel.Controls.Add(new Label { Text = "Tên môn", Top = 10, Left = 230, AutoSize = true });
            txtTenMon = new TextBox { Top = 30, Left = 230, Width = 220 };
            panel.Controls.Add(txtTenMon);

            panel.Controls.Add(new Label { Text = "Số tín chỉ", Top = 80, Left = 10, AutoSize = true });
            nudSoTinChi = new NumericUpDown { Top = 100, Left = 10, Width = 100, Minimum = 1, Maximum = 10, Value = 3 };
            panel.Controls.Add(nudSoTinChi);

            panel.Controls.Add(new Label { Text = "Môn tiên quyết", Top = 80, Left = 130, AutoSize = true });
            cmbMonTienQuyet = new ComboBox { Top = 100, Left = 130, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbMonTienQuyet);

            panel.Controls.Add(new Label { Text = "Tìm kiếm", Top = 10, Left = 500, AutoSize = true });
            txtSearch = new TextBox { Top = 30, Left = 500, Width = 220 };
            panel.Controls.Add(txtSearch);

            var btnSearch  = new Button { Text = "Tìm kiếm", Top = 70,  Left = 500, Width = 105 };
            var btnAdd     = new Button { Text = "Thêm",     Top = 120, Left = 500, Width = 105 };
            var btnUpdate  = new Button { Text = "Cập nhật", Top = 170, Left = 500, Width = 105 };
            var btnDelete  = new Button { Text = "Xóa",      Top = 220, Left = 500, Width = 105 };
            var btnRefresh = new Button { Text = "Làm mới",  Top = 220, Left = 620, Width = 105 };

            btnSearch.Click  += (s, e) => LoadData(txtSearch.Text.Trim());
            btnAdd.Click     += BtnAdd_Click;
            btnUpdate.Click  += BtnUpdate_Click;
            btnDelete.Click  += BtnDelete_Click;
            btnRefresh.Click += (s, e) => { ClearForm(); LoadData(); };

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh });
            Controls.Add(panel);
            Controls.Add(dgv);

            Load += (s, e) => { LoadLookup(); LoadData(); };
        }

        private void LoadLookup()
        {
            var dt = CommonDAL.LoadLookup("sp_LayDanhSachMonHoc");
            var blank = dt.NewRow();
            blank["MaMon"]  = DBNull.Value;
            blank["TenMon"] = "(Không có)";
            dt.Rows.InsertAt(blank, 0);
            cmbMonTienQuyet.DataSource    = dt;
            cmbMonTienQuyet.DisplayMember = "TenMon";
            cmbMonTienQuyet.ValueMember   = "MaMon";
        }

        private void LoadData(string keyword = null)
        {
            try { dgv.DataSource = MonHocBUS.LoadAll(keyword); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = dgv.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return;

            txtMaMon.Text  = row["MaMon"]?.ToString();
            txtTenMon.Text = row["TenMon"]?.ToString();
            nudSoTinChi.Value = row["SoTinChi"] != DBNull.Value ? Convert.ToDecimal(row["SoTinChi"]) : 3;

            var mtq = row["MonTienQuyet"]?.ToString();
            if (string.IsNullOrEmpty(mtq))
                cmbMonTienQuyet.SelectedIndex = 0;
            else
                cmbMonTienQuyet.SelectedValue = mtq;
        }

        private string GetMonTienQuyet()
        {
            var v = cmbMonTienQuyet.SelectedValue;
            return (v == null || v == DBNull.Value || string.IsNullOrWhiteSpace(v.ToString())) ? null : v.ToString();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var r = MonHocBUS.Add(txtMaMon.Text, txtTenMon.Text, (int)nudSoTinChi.Value, GetMonTienQuyet());
            ShowResult(r);
            if (r.Success) { LoadLookup(); LoadData(); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var r = MonHocBUS.Update(txtMaMon.Text, txtTenMon.Text, (int)nudSoTinChi.Value, GetMonTienQuyet());
            ShowResult(r);
            if (r.Success) { LoadLookup(); LoadData(); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa môn học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var r = MonHocBUS.Delete(txtMaMon.Text);
            ShowResult(r);
            if (r.Success) { LoadLookup(); LoadData(); }
        }

        private void ClearForm()
        {
            txtMaMon.Clear(); txtTenMon.Clear(); nudSoTinChi.Value = 3;
            if (cmbMonTienQuyet.Items.Count > 0) cmbMonTienQuyet.SelectedIndex = 0;
        }

        private void ShowResult(OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
