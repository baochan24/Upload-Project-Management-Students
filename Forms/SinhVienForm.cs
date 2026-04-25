using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.Forms
{
    public class SinhVienForm : Form
    {
        private readonly Guna2DataGridView dgvSinhVien;
        private readonly TextBox txtMaSV;
        private readonly TextBox txtHoTen;
        private readonly DateTimePicker dtpNgaySinh;
        private readonly ComboBox cmbGioiTinh;
        private readonly TextBox txtDiaChi;
        private readonly ComboBox cmbMaLopSH;
        private readonly ComboBox cmbTinhTrang;
        private readonly TextBox txtSearch;
        private readonly Button btnXemChiTiet;
        private readonly Button btnChuyenLop;

        public SinhVienForm()
        {
            Text = "Quản lý sinh viên";
            // Không set Width/Height cứng – để Dock=Fill từ MainForm quyết định kích thước

            dgvSinhVien = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,   // Fill thay vì Bottom+Height cứng → tránh chồng lấn
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

            // ── Cột nút thứ hai (Left=620) ──────────────────────────────
            btnXemChiTiet = new Button
            {
                Text = "Xem chi tiết",
                Top = 120, Left = 620, Width = 120, Height = 26,
                BackColor = System.Drawing.Color.FromArgb(173, 216, 230)
            };
            btnXemChiTiet.Click += BtnXemChiTiet_Click;

            btnChuyenLop = new Button
            {
                Text = "Chuyển lớp",
                Top = 170, Left = 620, Width = 120, Height = 26,
                BackColor = System.Drawing.Color.FromArgb(255, 228, 181)
            };
            btnChuyenLop.Click += BtnChuyenLop_Click;

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh, btnXemChiTiet, btnChuyenLop });
            // WinForms reverse-order: dgv(Fill, index 0) → fill pass; panel(Top, index 1, LAST) → dock top trước
            Controls.Add(dgvSinhVien);  // Fill – index 0 (fill pass)
            Controls.Add(panel);        // Top  – index 1 (last → processed first → docks to top 300px)

            Load += SinhVienForm_Load;

            // ── Áp style UI – không ảnh hưởng logic ──────────────────────
            ApplyTheme(panel, dgvSinhVien, btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh);
        }

        private void SinhVienForm_Load(object sender, EventArgs e)
        {
            LoadLookups();
            LoadData();
        }

        // ══════════════════════════════════════════════════════════════════
        // UI THEME – không chứa logic nghiệp vụ
        // ══════════════════════════════════════════════════════════════════

        private void ApplyTheme(Panel inputPanel, DataGridView dgv,
                                Button btnSearch, Button btnAdd, Button btnUpdate,
                                Button btnDelete, Button btnRefresh)
        {
            // Form
            BackColor = ThemeConfig.BgMain;
            Font      = ThemeConfig.FontDefault;

            // Panel nhập liệu
            inputPanel.BackColor = Color.White;

            // Tiêu đề form
            var lblTitle = new Label
            {
                Text      = "QUẢN LÝ SINH VIÊN",
                Font      = ThemeConfig.FontBold,
                ForeColor = ThemeConfig.PrimaryMid,
                AutoSize  = true,
                Left      = 10,
                Top       = -2
            };
            // Đường kẻ phân cách dưới tiêu đề
            var separator = new Panel
            {
                Left      = 10,
                Top       = 18,
                Width     = inputPanel.Width - 20,
                Height    = 2,
                BackColor = ThemeConfig.PrimaryLight
            };
            inputPanel.Controls.Add(lblTitle);
            inputPanel.Controls.Add(separator);

            // DataGridView
            UIHelper.StyleDataGridView(dgv);

            // Nút chức năng chính
            UIHelper.StyleButton(btnSearch, ThemeConfig.BtnSearch, ThemeConfig.BtnSearchHover);
            UIHelper.StyleButton(btnAdd,    ThemeConfig.BtnAdd,    ThemeConfig.BtnAddHover);
            UIHelper.StyleButton(btnUpdate, ThemeConfig.BtnEdit,   ThemeConfig.BtnEditHover);
            UIHelper.StyleButton(btnDelete, ThemeConfig.BtnDelete, ThemeConfig.BtnDeleteHover);
            UIHelper.StyleButton(btnRefresh,ThemeConfig.BtnNeutral,ThemeConfig.BtnNeutralHover);

            // Nút đặc biệt (field)
            UIHelper.StyleButton(btnXemChiTiet, ThemeConfig.BtnSearch,  ThemeConfig.BtnSearchHover,
                                 size: new Size(120, 30));
            UIHelper.StyleButton(btnChuyenLop,  ThemeConfig.BtnEdit,    ThemeConfig.BtnEditHover,
                                 size: new Size(120, 30));

            // Style các TextBox và Label trong inputPanel
            foreach (Control c in inputPanel.Controls)
            {
                if (c is TextBox tb)  UIHelper.StyleTextBox(tb);
                if (c is Label  lbl) UIHelper.StyleLabel(lbl);
            }
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
            var maSV = txtMaSV.Text.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(maSV))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần chuyển trạng thái.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                    $"Chuyển sinh viên [{maSV}] sang trạng thái \"Thôi học\"?\n\n" +
                    "Thao tác này không xóa dữ liệu khỏi hệ thống.",
                    "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var result = SinhVienBUS.CapNhatTinhTrang(maSV, "Thôi học");
            MessageBox.Show(result.Message,
                result.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (result.Success) LoadData();
        }

        // ── Xem chi tiết hồ sơ ───────────────────────────────────────────
        private void BtnXemChiTiet_Click(object sender, EventArgs e)
        {
            var maSV = txtMaSV.Text.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(maSV))
            {
                MessageBox.Show("Vui lòng chọn hoặc nhập mã sinh viên.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var form = new ChiTietSinhVienForm(maSV);
                form.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Chuyển lớp ───────────────────────────────────────────────────
        private void BtnChuyenLop_Click(object sender, EventArgs e)
        {
            var maSV = txtMaSV.Text.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(maSV))
            {
                MessageBox.Show("Vui lòng chọn hoặc nhập mã sinh viên.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var form = new ChuyenLopDialog(maSV);
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
