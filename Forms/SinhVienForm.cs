using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.Forms
{
    public class SinhVienForm : Form
    {
        private readonly DataGridView dgvSinhVien;
        private readonly TextBox txtMaSV;
        private readonly TextBox txtHoTen;
        private readonly DateTimePicker dtpNgaySinh;
        private readonly ComboBox cmbGioiTinh;
        private readonly TextBox txtDiaChi;
        private readonly ComboBox cmbMaLopSH;
        private readonly ComboBox cmbTinhTrang;
        private readonly TextBox txtSearch;

        public SinhVienForm()
        {
            Text = "Quản lý sinh viên";
            Width = 980;
            Height = 700;

            dgvSinhVien = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 320,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvSinhVien.SelectionChanged += DgvSinhVien_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 300, Padding = new Padding(10) };

            txtMaSV = CreateLabeledTextBox(panel, "Mã SV", 10);
            txtHoTen = CreateLabeledTextBox(panel, "Họ tên", 80);

            var lblNgaySinh = new Label { Text = "Ngày sinh", Top = 150, Left = 10, AutoSize = true };
            dtpNgaySinh = new DateTimePicker { Top = 170, Left = 10, Width = 220, Format = DateTimePickerFormat.Short };
            panel.Controls.Add(lblNgaySinh);
            panel.Controls.Add(dtpNgaySinh);

            var lblGioiTinh = new Label { Text = "Giới tính", Top = 150, Left = 260, AutoSize = true };
            cmbGioiTinh = new ComboBox { Top = 170, Left = 260, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ" });
            panel.Controls.Add(lblGioiTinh);
            panel.Controls.Add(cmbGioiTinh);

            var lblDiaChi = new Label { Text = "Địa chỉ", Top = 210, Left = 10, AutoSize = true };
            txtDiaChi = new TextBox { Top = 230, Left = 10, Width = 470 };
            panel.Controls.Add(lblDiaChi);
            panel.Controls.Add(txtDiaChi);

            var lblMaLopSH = new Label { Text = "Lớp SH", Top = 210, Left = 500, AutoSize = true };
            cmbMaLopSH = new ComboBox { Top = 230, Left = 500, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(lblMaLopSH);
            panel.Controls.Add(cmbMaLopSH);

            var lblTinhTrang = new Label { Text = "Tình trạng", Top = 260, Left = 500, AutoSize = true };
            cmbTinhTrang = new ComboBox { Top = 280, Left = 500, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTinhTrang.Items.AddRange(new object[] { "Đang học", "Nghỉ học", "Thôi học", "Tốt nghiệp" });
            panel.Controls.Add(lblTinhTrang);
            panel.Controls.Add(cmbTinhTrang);

            var lblSearch = new Label { Text = "Tìm kiếm", Top = 10, Left = 500, AutoSize = true };
            txtSearch = new TextBox { Top = 30, Left = 500, Width = 220 };
            panel.Controls.Add(lblSearch);
            panel.Controls.Add(txtSearch);

            var btnSearch = new Button { Text = "Tìm kiếm", Top = 70, Left = 500, Width = 100 };
            btnSearch.Click += (sender, args) => LoadData(txtSearch.Text.Trim());

            var btnAdd = new Button { Text = "Thêm", Top = 120, Left = 500, Width = 100 };
            btnAdd.Click += BtnAdd_Click;

            var btnUpdate = new Button { Text = "Cập nhật", Top = 170, Left = 500, Width = 100 };
            btnUpdate.Click += BtnUpdate_Click;

            var btnDelete = new Button { Text = "Xóa", Top = 220, Left = 500, Width = 100 };
            btnDelete.Click += BtnDelete_Click;

            var btnRefresh = new Button { Text = "Làm mới", Top = 270, Left = 500, Width = 100 };
            btnRefresh.Click += (sender, args) => LoadData();

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh });
            Controls.Add(panel);
            Controls.Add(dgvSinhVien);

            Load += SinhVienForm_Load;
        }

        private void SinhVienForm_Load(object sender, EventArgs e)
        {
            LoadLookups();
            LoadData();
        }

        private TextBox CreateLabeledTextBox(Control parent, string labelText, int top)
        {
            var label = new Label { Text = labelText, Top = top, Left = 10, AutoSize = true };
            var textBox = new TextBox { Top = top + 20, Left = 10, Width = 220 };
            parent.Controls.Add(label);
            parent.Controls.Add(textBox);
            return textBox;
        }

        private void LoadLookups()
        {
            cmbMaLopSH.DisplayMember = "MaLopSH";
            cmbMaLopSH.ValueMember = "MaLopSH";
            cmbMaLopSH.DataSource = CommonDAL.LoadLookup("sp_LayDanhSachLopSinhHoat");
            if (cmbGioiTinh.Items.Count > 0) cmbGioiTinh.SelectedIndex = 0;
            if (cmbTinhTrang.Items.Count > 0) cmbTinhTrang.SelectedIndex = 0;
        }

        private void LoadData(string keyword = null)
        {
            try
            {
                dgvSinhVien.DataSource = SinhVienBUS.LoadAll(keyword);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvSinhVien_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow == null) return;
            var row = dgvSinhVien.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return;

            txtMaSV.Text = row["MaSV"]?.ToString();
            txtHoTen.Text = row["HoTen"]?.ToString();
            if (DateTime.TryParse(row["NgaySinh"]?.ToString(), out var ngaySinh))
                dtpNgaySinh.Value = ngaySinh;

            cmbGioiTinh.SelectedItem = row["GioiTinh"] != DBNull.Value && Convert.ToBoolean(row["GioiTinh"]) ? "Nam" : "Nữ";
            txtDiaChi.Text = row["DiaChi"]?.ToString();
            cmbMaLopSH.SelectedValue = row["MaLopSH"]?.ToString();
            cmbTinhTrang.SelectedItem = row["TinhTrang"]?.ToString();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var result = SinhVienBUS.Add(txtMaSV.Text, txtHoTen.Text, dtpNgaySinh.Value, cmbGioiTinh.SelectedItem?.ToString() == "Nam", txtDiaChi.Text, cmbMaLopSH.SelectedValue?.ToString(), null);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (result.Success) LoadData();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var result = SinhVienBUS.Update(txtMaSV.Text, txtHoTen.Text, dtpNgaySinh.Value, cmbGioiTinh.SelectedItem?.ToString() == "Nam", txtDiaChi.Text, null);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (result.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var result = SinhVienBUS.Delete(txtMaSV.Text);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (result.Success) LoadData();
        }
    }
}
