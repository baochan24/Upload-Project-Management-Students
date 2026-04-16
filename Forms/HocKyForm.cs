using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class HocKyForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtMaHocKy, txtTenHocKy, txtNamHoc, txtSearch;
        private readonly NumericUpDown nudSoTinToiDa;
        private readonly DateTimePicker dtpNgayBatDau, dtpNgayKetThuc;
        private readonly CheckBox chkNgayBatDau, chkNgayKetThuc;

        public HocKyForm()
        {
            Text = "Quản lý học kỳ";
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

            panel.Controls.Add(new Label { Text = "Mã học kỳ", Top = 10, Left = 10, AutoSize = true });
            txtMaHocKy = new TextBox { Top = 30, Left = 10, Width = 180 };
            panel.Controls.Add(txtMaHocKy);

            panel.Controls.Add(new Label { Text = "Tên học kỳ", Top = 10, Left = 210, AutoSize = true });
            txtTenHocKy = new TextBox { Top = 30, Left = 210, Width = 220 };
            panel.Controls.Add(txtTenHocKy);

            panel.Controls.Add(new Label { Text = "Năm học (VD: 2024-2025)", Top = 80, Left = 10, AutoSize = true });
            txtNamHoc = new TextBox { Top = 100, Left = 10, Width = 180 };
            panel.Controls.Add(txtNamHoc);

            panel.Controls.Add(new Label { Text = "Số tín chỉ tối đa", Top = 80, Left = 210, AutoSize = true });
            nudSoTinToiDa = new NumericUpDown { Top = 100, Left = 210, Width = 100, Minimum = 1, Maximum = 50, Value = 24 };
            panel.Controls.Add(nudSoTinToiDa);

            chkNgayBatDau = new CheckBox { Text = "Ngày bắt đầu", Top = 150, Left = 10, AutoSize = true };
            dtpNgayBatDau = new DateTimePicker { Top = 170, Left = 10, Width = 200, Format = DateTimePickerFormat.Short, Enabled = false };
            chkNgayBatDau.CheckedChanged += (s, e) => dtpNgayBatDau.Enabled = chkNgayBatDau.Checked;
            panel.Controls.Add(chkNgayBatDau);
            panel.Controls.Add(dtpNgayBatDau);

            chkNgayKetThuc = new CheckBox { Text = "Ngày kết thúc", Top = 150, Left = 240, AutoSize = true };
            dtpNgayKetThuc = new DateTimePicker { Top = 170, Left = 240, Width = 200, Format = DateTimePickerFormat.Short, Enabled = false };
            chkNgayKetThuc.CheckedChanged += (s, e) => dtpNgayKetThuc.Enabled = chkNgayKetThuc.Checked;
            panel.Controls.Add(chkNgayKetThuc);
            panel.Controls.Add(dtpNgayKetThuc);

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

            Load += (s, e) => LoadData();
        }

        private void LoadData(string keyword = null)
        {
            try { dgv.DataSource = HocKyBUS.LoadAll(keyword); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = dgv.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return;

            txtMaHocKy.Text  = row["MaHocKy"]?.ToString();
            txtTenHocKy.Text = row["TenHocKy"]?.ToString();
            txtNamHoc.Text   = row["NamHoc"]?.ToString();
            nudSoTinToiDa.Value = row["SoTinToiDa"] != DBNull.Value ? Convert.ToDecimal(row["SoTinToiDa"]) : 24;

            if (row["NgayBatDau"] != DBNull.Value && DateTime.TryParse(row["NgayBatDau"]?.ToString(), out var nbd))
            { chkNgayBatDau.Checked = true; dtpNgayBatDau.Value = nbd; }
            else chkNgayBatDau.Checked = false;

            if (row["NgayKetThuc"] != DBNull.Value && DateTime.TryParse(row["NgayKetThuc"]?.ToString(), out var nkt))
            { chkNgayKetThuc.Checked = true; dtpNgayKetThuc.Value = nkt; }
            else chkNgayKetThuc.Checked = false;
        }

        private object GetNgayBatDau()  => chkNgayBatDau.Checked  ? (object)dtpNgayBatDau.Value.Date  : DBNull.Value;
        private object GetNgayKetThuc() => chkNgayKetThuc.Checked ? (object)dtpNgayKetThuc.Value.Date : DBNull.Value;

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var r = HocKyBUS.Add(txtMaHocKy.Text, txtTenHocKy.Text, txtNamHoc.Text,
                GetNgayBatDau(), GetNgayKetThuc(), (int)nudSoTinToiDa.Value);
            ShowResult(r);
            if (r.Success) LoadData();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var r = HocKyBUS.Update(txtMaHocKy.Text, txtTenHocKy.Text, txtNamHoc.Text,
                GetNgayBatDau(), GetNgayKetThuc(), (int)nudSoTinToiDa.Value);
            ShowResult(r);
            if (r.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa học kỳ này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var r = HocKyBUS.Delete(txtMaHocKy.Text);
            ShowResult(r);
            if (r.Success) LoadData();
        }

        private void ClearForm()
        {
            txtMaHocKy.Clear(); txtTenHocKy.Clear(); txtNamHoc.Clear();
            nudSoTinToiDa.Value = 24;
            chkNgayBatDau.Checked = false;
            chkNgayKetThuc.Checked = false;
        }

        private void ShowResult(OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
