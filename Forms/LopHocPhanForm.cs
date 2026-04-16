using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.Forms
{
    public class LopHocPhanForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtMaLHP;
        private readonly TextBox txtMaLopHienThi;
        private readonly ComboBox cmbMaMon;
        private readonly ComboBox cmbMaGV;
        private readonly ComboBox cmbMaHocKy;
        private readonly ComboBox cmbMaPhong;
        private readonly TextBox txtSiSoToiDa;
        private readonly ComboBox cmbThu;
        private readonly TextBox txtTietBatDau;
        private readonly TextBox txtTietKetThuc;
        private readonly ComboBox cmbTrangThai;
        private readonly TextBox txtSearch;
        private string _selectedMaLHP = null;

        public LopHocPhanForm()
        {
            Text = "Quản lý lớp học phần";
            Width = 1100;
            Height = 720;

            dgv = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 370,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 320, Padding = new Padding(10) };

            // Cột 1 (left=8)
            txtMaLHP         = CreateLabeledTextBox(panel, "Mã LHP *",          10,  8);
            txtMaLopHienThi  = CreateLabeledTextBox(panel, "Mã lớp hiển thị *", 70,  8);
            txtSiSoToiDa     = CreateLabeledTextBox(panel, "Sĩ số tối đa *",   130,  8);

            // Cột 2 (left=220)
            panel.Controls.Add(new Label { Text = "Môn học *", Top = 10, Left = 220, AutoSize = true });
            cmbMaMon = new ComboBox { Top = 28, Left = 220, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbMaMon);

            panel.Controls.Add(new Label { Text = "Giảng viên *", Top = 70, Left = 220, AutoSize = true });
            cmbMaGV = new ComboBox { Top = 88, Left = 220, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbMaGV);

            panel.Controls.Add(new Label { Text = "Học kỳ *", Top = 130, Left = 220, AutoSize = true });
            cmbMaHocKy = new ComboBox { Top = 148, Left = 220, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbMaHocKy);

            panel.Controls.Add(new Label { Text = "Phòng học *", Top = 190, Left = 220, AutoSize = true });
            cmbMaPhong = new ComboBox { Top = 208, Left = 220, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbMaPhong);

            // Cột 3 (left=450)
            panel.Controls.Add(new Label { Text = "Thứ", Top = 10, Left = 450, AutoSize = true });
            cmbThu = new ComboBox { Top = 28, Left = 450, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbThu.Items.Add("(Không)");
            for (int t = 2; t <= 7; t++) cmbThu.Items.Add("Thứ " + t);
            cmbThu.Items.Add("Chủ nhật");
            cmbThu.SelectedIndex = 0;
            panel.Controls.Add(cmbThu);

            txtTietBatDau  = CreateLabeledTextBox(panel, "Tiết bắt đầu",  70, 450, 120);
            txtTietKetThuc = CreateLabeledTextBox(panel, "Tiết kết thúc", 130, 450, 120);

            panel.Controls.Add(new Label { Text = "Trạng thái", Top = 190, Left = 450, AutoSize = true });
            cmbTrangThai = new ComboBox { Top = 208, Left = 450, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTrangThai.Items.AddRange(new object[] { "Đang mở", "Đã đóng", "Đã kết thúc" });
            cmbTrangThai.SelectedIndex = 0;
            panel.Controls.Add(cmbTrangThai);

            // Cột phải - tìm kiếm & nút (left=650)
            panel.Controls.Add(new Label { Text = "Tìm kiếm", Top = 10, Left = 680, AutoSize = true });
            txtSearch = new TextBox { Top = 28, Left = 680, Width = 220 };
            panel.Controls.Add(txtSearch);

            var btnSearch  = new Button { Text = "Tìm kiếm",  Top = 60,  Left = 680, Width = 110 };
            var btnAdd     = new Button { Text = "Thêm mới",  Top = 100, Left = 680, Width = 110 };
            var btnUpdate  = new Button { Text = "Cập nhật",  Top = 140, Left = 680, Width = 110 };
            var btnDelete  = new Button { Text = "Xóa",       Top = 180, Left = 680, Width = 110 };
            var btnRefresh = new Button { Text = "Làm mới",   Top = 60,  Left = 810, Width = 110 };

            btnSearch.Click  += (s, e) => LoadData(txtSearch.Text.Trim());
            btnAdd.Click     += BtnAdd_Click;
            btnUpdate.Click  += BtnUpdate_Click;
            btnDelete.Click  += BtnDelete_Click;
            btnRefresh.Click += (s, e) => { ClearForm(); LoadData(); };

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh });
            Controls.Add(panel);
            Controls.Add(dgv);

            Load += LopHocPhanForm_Load;
        }

        private TextBox CreateLabeledTextBox(Control parent, string label, int top, int left, int width = 170)
        {
            parent.Controls.Add(new Label { Text = label, Top = top, Left = left, AutoSize = true });
            var tb = new TextBox { Top = top + 18, Left = left, Width = width };
            parent.Controls.Add(tb);
            return tb;
        }

        private void LopHocPhanForm_Load(object sender, EventArgs e)
        {
            try
            {
                var mon = CommonDAL.LoadLookup("sp_LayDanhSachMonHoc");
                cmbMaMon.DisplayMember = "TenMon";
                cmbMaMon.ValueMember   = "MaMon";
                cmbMaMon.DataSource    = mon;
            }
            catch { }

            try
            {
                var gv = CommonDAL.LoadLookup("sp_LayDanhSachGiangVien");
                cmbMaGV.DisplayMember = "HoTen";
                cmbMaGV.ValueMember   = "MaGV";
                cmbMaGV.DataSource    = gv;
            }
            catch { }

            try
            {
                var hk = CommonDAL.LoadLookup("sp_LayDanhSachHocKy");
                cmbMaHocKy.DisplayMember = "MaHocKy";
                cmbMaHocKy.ValueMember   = "MaHocKy";
                cmbMaHocKy.DataSource    = hk;
            }
            catch { }

            try
            {
                var phong = CommonDAL.LoadLookup("sp_LayDanhSachPhongHoc");
                cmbMaPhong.DisplayMember = "TenPhong";
                cmbMaPhong.ValueMember   = "MaPhong";
                cmbMaPhong.DataSource    = phong;
            }
            catch { }

            LoadData();
        }

        private void LoadData(string keyword = null)
        {
            try
            {
                dgv.DataSource = LopHocPhanBUS.LoadAll(keyword);
                _selectedMaLHP = null;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow?.DataBoundItem is DataRowView row)
            {
                _selectedMaLHP          = row["MaLHP"]?.ToString();
                txtMaLHP.Text           = _selectedMaLHP;
                txtMaLopHienThi.Text    = row["MaLopHienThi"]?.ToString();
                txtSiSoToiDa.Text       = row["SiSoToiDa"]?.ToString();
                txtTietBatDau.Text      = row["TietBatDau"]?.ToString();
                txtTietKetThuc.Text     = row["TietKetThuc"]?.ToString();

                if (cmbMaMon.DataSource != null)
                    SetComboValue(cmbMaMon, row["MaMon"]);
                if (cmbMaGV.DataSource != null)
                    SetComboValue(cmbMaGV, row["MaGV"]);
                if (cmbMaHocKy.DataSource != null)
                    SetComboValue(cmbMaHocKy, row["MaHocKy"]);
                if (cmbMaPhong.DataSource != null)
                    SetComboValue(cmbMaPhong, row["MaPhong"]);

                // Thu: DB value 2-8, index 0=(Không), 1=Thứ2...6=Thứ7, 7=CN
                if (row["Thu"] != DBNull.Value)
                {
                    int thu = Convert.ToInt32(row["Thu"]);
                    cmbThu.SelectedIndex = thu == 8 ? 7 : thu - 1; // 2→1, 3→2... 7→6, 8(CN)→7
                }
                else cmbThu.SelectedIndex = 0;

                string tt = row["TrangThai"]?.ToString();
                int idx = cmbTrangThai.Items.IndexOf(tt);
                cmbTrangThai.SelectedIndex = idx >= 0 ? idx : 0;
            }
        }

        private void SetComboValue(ComboBox cmb, object value)
        {
            try { cmb.SelectedValue = value; }
            catch { cmb.SelectedIndex = -1; }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!TryGetThu(out byte? thu)) return;
            if (!TryGetTiet(txtTietBatDau, out byte? tietBD)) return;
            if (!TryGetTiet(txtTietKetThuc, out byte? tietKT)) return;
            if (!int.TryParse(txtSiSoToiDa.Text, out int siSo))
            { MessageBox.Show("Sĩ số tối đa phải là số nguyên.", "Lỗi"); return; }

            var r = LopHocPhanBUS.Add(
                txtMaLHP.Text, txtMaLopHienThi.Text,
                cmbMaMon.SelectedValue?.ToString(), cmbMaGV.SelectedValue?.ToString(),
                cmbMaHocKy.SelectedValue?.ToString(), cmbMaPhong.SelectedValue?.ToString(),
                siSo, thu, tietBD, tietKT);
            ShowResult(r);
            if (r.Success) { ClearForm(); LoadData(); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedMaLHP == null) { MessageBox.Show("Vui lòng chọn lớp học phần cần cập nhật.", "Thông báo"); return; }
            if (!TryGetThu(out byte? thu)) return;
            if (!TryGetTiet(txtTietBatDau, out byte? tietBD)) return;
            if (!TryGetTiet(txtTietKetThuc, out byte? tietKT)) return;
            if (!int.TryParse(txtSiSoToiDa.Text, out int siSo))
            { MessageBox.Show("Sĩ số tối đa phải là số nguyên.", "Lỗi"); return; }

            string tt = cmbTrangThai.SelectedItem?.ToString();
            var r = LopHocPhanBUS.Update(
                _selectedMaLHP, txtMaLopHienThi.Text,
                cmbMaGV.SelectedValue?.ToString(), cmbMaPhong.SelectedValue?.ToString(),
                siSo, thu, tietBD, tietKT, tt);
            ShowResult(r);
            if (r.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedMaLHP == null) { MessageBox.Show("Vui lòng chọn lớp học phần cần xóa.", "Thông báo"); return; }
            if (MessageBox.Show($"Xóa lớp học phần '{_selectedMaLHP}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var r = LopHocPhanBUS.Delete(_selectedMaLHP);
            ShowResult(r);
            if (r.Success) { ClearForm(); LoadData(); }
        }

        private bool TryGetThu(out byte? thu)
        {
            thu = null;
            int idx = cmbThu.SelectedIndex;
            if (idx <= 0) return true;
            // index 1→Thứ2(2), 2→Thứ3(3)... 6→Thứ7(7), 7→CN(8)
            thu = (byte)(idx == 7 ? 8 : idx + 1);
            return true;
        }

        private bool TryGetTiet(TextBox txt, out byte? tiet)
        {
            tiet = null;
            if (string.IsNullOrWhiteSpace(txt.Text)) return true;
            if (byte.TryParse(txt.Text, out byte v)) { tiet = v; return true; }
            MessageBox.Show($"Giá trị tiết '{txt.Text}' không hợp lệ (0-255).", "Lỗi");
            return false;
        }

        private void ClearForm()
        {
            txtMaLHP.Text = txtMaLopHienThi.Text = txtSiSoToiDa.Text = txtTietBatDau.Text = txtTietKetThuc.Text = string.Empty;
            cmbThu.SelectedIndex = 0;
            cmbTrangThai.SelectedIndex = 0;
            if (cmbMaMon.Items.Count > 0) cmbMaMon.SelectedIndex = 0;
            if (cmbMaGV.Items.Count > 0) cmbMaGV.SelectedIndex = 0;
            if (cmbMaHocKy.Items.Count > 0) cmbMaHocKy.SelectedIndex = 0;
            if (cmbMaPhong.Items.Count > 0) cmbMaPhong.SelectedIndex = 0;
            _selectedMaLHP = null;
        }

        private void ShowResult(Models.OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
