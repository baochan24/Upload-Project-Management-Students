using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class DiemForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtSearch, txtMaDK;
        private readonly ComboBox cmbMaLHP;
        private readonly NumericUpDown nudDiemCC, nudDiemGK, nudDiemCK;
        private readonly Label lblThongTin, lblTrangThai;

        public DiemForm()
        {
            Text = "Quản lý điểm số";
            Width = 1100;
            Height = 750;

            dgv = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 380,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 310, Padding = new Padding(10) };

            // --- Tìm theo lớp ---
            panel.Controls.Add(new Label { Text = "Lớp học phần", Top = 10, Left = 10, AutoSize = true });
            cmbMaLHP = new ComboBox { Top = 30, Left = 10, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbMaLHP);

            var btnLoadLop = new Button { Text = "Tải danh sách", Top = 28, Left = 320, Width = 110 };
            btnLoadLop.Click += (s, e) => LoadByLop();
            panel.Controls.Add(btnLoadLop);

            var btnKhoa = new Button { Text = "🔒 Khóa điểm lớp", Top = 28, Left = 445, Width = 130,
                BackColor = System.Drawing.Color.IndianRed, ForeColor = System.Drawing.Color.White };
            btnKhoa.Click += BtnKhoa_Click;
            panel.Controls.Add(btnKhoa);

            lblThongTin = new Label { Text = "", Top = 65, Left = 10, AutoSize = true, ForeColor = System.Drawing.Color.DarkBlue };
            panel.Controls.Add(lblThongTin);

            // --- Tìm kiếm tự do ---
            panel.Controls.Add(new Label { Text = "Tìm kiếm (MaSV / Tên / Môn)", Top = 10, Left = 620, AutoSize = true });
            txtSearch = new TextBox { Top = 30, Left = 620, Width = 200 };
            panel.Controls.Add(txtSearch);
            var btnSearch = new Button { Text = "Tìm kiếm", Top = 28, Left = 828, Width = 90 };
            btnSearch.Click += (s, e) => LoadData(txtSearch.Text.Trim());
            panel.Controls.Add(btnSearch);

            // --- Form nhập điểm ---
            var grpScore = new GroupBox { Text = "Nhập / Cập nhật điểm", Top = 90, Left = 10, Width = 700, Height = 190 };

            grpScore.Controls.Add(new Label { Text = "Mã đăng ký (MaDK)", Top = 20, Left = 10, AutoSize = true });
            txtMaDK = new TextBox { Top = 40, Left = 10, Width = 140, ReadOnly = true, BackColor = System.Drawing.SystemColors.Control };
            grpScore.Controls.Add(txtMaDK);

            lblTrangThai = new Label { Text = "Trạng thái điểm: —", Top = 70, Left = 10, AutoSize = true };
            grpScore.Controls.Add(lblTrangThai);

            grpScore.Controls.Add(new Label { Text = "Điểm chuyên cần (0-10)", Top = 20, Left = 180, AutoSize = true });
            nudDiemCC = new NumericUpDown { Top = 40, Left = 180, Width = 90, Minimum = 0, Maximum = 10, DecimalPlaces = 1, Increment = 0.5m };
            grpScore.Controls.Add(nudDiemCC);

            grpScore.Controls.Add(new Label { Text = "Điểm giữa kỳ (0-10)", Top = 20, Left = 290, AutoSize = true });
            nudDiemGK = new NumericUpDown { Top = 40, Left = 290, Width = 90, Minimum = 0, Maximum = 10, DecimalPlaces = 1, Increment = 0.5m };
            grpScore.Controls.Add(nudDiemGK);

            grpScore.Controls.Add(new Label { Text = "Điểm cuối kỳ (0-10)", Top = 20, Left = 400, AutoSize = true });
            nudDiemCK = new NumericUpDown { Top = 40, Left = 400, Width = 90, Minimum = 0, Maximum = 10, DecimalPlaces = 1, Increment = 0.5m };
            grpScore.Controls.Add(nudDiemCK);

            var btnLuu = new Button { Text = "💾 Lưu điểm", Top = 100, Left = 180, Width = 120, Height = 35,
                BackColor = System.Drawing.Color.SteelBlue, ForeColor = System.Drawing.Color.White };
            btnLuu.Click += BtnLuu_Click;
            grpScore.Controls.Add(btnLuu);

            var btnRefresh = new Button { Text = "Làm mới form", Top = 100, Left = 315, Width = 110, Height = 35 };
            btnRefresh.Click += (s, e) => ClearScoreForm();
            grpScore.Controls.Add(btnRefresh);

            panel.Controls.Add(grpScore);
            Controls.Add(panel);
            Controls.Add(dgv);

            Load += (s, e) => { LoadLopLookup(); LoadData(); };
        }

        private void LoadLopLookup()
        {
            DataTable dt;
            if (UserSession.IsGiangVien && !string.IsNullOrWhiteSpace(UserSession.MaGV))
                dt = LopHocPhanBUS.LoadByGiangVien(UserSession.MaGV);
            else
                dt = CommonDAL.LoadLookup("sp_LayDanhSachLopHocPhan");

            cmbMaLHP.DataSource    = dt;
            cmbMaLHP.DisplayMember = "MaLopHienThi";
            cmbMaLHP.ValueMember   = "MaLHP";

            if (cmbMaLHP.SelectedItem is DataRowView row)
                lblThongTin.Text = $"Môn: {row["TenMon"]}  |  GV: {row["TenGV"]}  |  Phòng: {row["TenPhong"]}";
        }

        private void LoadByLop()
        {
            var maLHP = cmbMaLHP.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(maLHP)) return;

            try
            {
                // Load registrations + scores for this class (sp_LayDanhSachDangKy supports keyword search by MaLHP)
                dgv.DataSource = DangKyBUS.LoadAll(maLHP);
                if (cmbMaLHP.SelectedItem is DataRowView row)
                    lblThongTin.Text = $"Môn: {row["TenMon"]}  |  GV: {row["TenGV"]}  |  Phòng: {row["TenPhong"]}";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LoadData(string keyword = null)
        {
            try { dgv.DataSource = DiemBUS.LoadAll(keyword); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = dgv.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return;

            txtMaDK.Text = row["MaDK"]?.ToString();

            // Read current scores if exist
            nudDiemCC.Value = row.Row.Table.Columns.Contains("DiemChuyenCan") && row["DiemChuyenCan"] != DBNull.Value
                ? Convert.ToDecimal(row["DiemChuyenCan"]) : 0;
            nudDiemGK.Value = row.Row.Table.Columns.Contains("DiemGiuaKy") && row["DiemGiuaKy"] != DBNull.Value
                ? Convert.ToDecimal(row["DiemGiuaKy"]) : 0;
            nudDiemCK.Value = row.Row.Table.Columns.Contains("DiemCuoiKy") && row["DiemCuoiKy"] != DBNull.Value
                ? Convert.ToDecimal(row["DiemCuoiKy"]) : 0;

            var tt = row.Row.Table.Columns.Contains("TrangThaiDiem") ? row["TrangThaiDiem"]?.ToString() : "";
            lblTrangThai.Text = $"Trạng thái điểm: {(string.IsNullOrEmpty(tt) ? "Chưa nhập" : tt)}";
            lblTrangThai.ForeColor = tt == "Đã khóa" ? System.Drawing.Color.Red : System.Drawing.Color.DarkGreen;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtMaDK.Text, out var maDK) || maDK <= 0)
            { MessageBox.Show("Chọn sinh viên từ danh sách trước.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            // Giảng viên chỉ được nhập điểm cho lớp của mình
            if (UserSession.IsGiangVien)
            {
                var maLHP = cmbMaLHP.SelectedValue?.ToString();
                int qr = DiemBUS.KiemTraQuyenSuaDiem(maLHP, UserSession.MaGV);
                if (qr != 1)
                {
                    string msg = qr == -1 ? "Lớp học phần không tồn tại." : "Bạn không có quyền nhập điểm cho lớp này.";
                    MessageBox.Show(msg, "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            float? cc = (float)nudDiemCC.Value;
            float? gk = (float)nudDiemGK.Value;
            float? ck = (float)nudDiemCK.Value;

            var r = DiemBUS.EnterScore(maDK, cc, gk, ck);
            ShowResult(r);
            if (r.Success) LoadByLop();
        }

        private void BtnKhoa_Click(object sender, EventArgs e)
        {
            var maLHP = cmbMaLHP.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(maLHP))
            { MessageBox.Show("Chọn lớp học phần trước khi khóa điểm.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (MessageBox.Show($"Khóa điểm toàn bộ sinh viên lớp {maLHP}?\nSau khi khóa KHÔNG thể sửa điểm!",
                "Xác nhận khóa điểm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var r = DiemBUS.LockClassScores(maLHP);
            ShowResult(r);
            if (r.Success) LoadByLop();
        }

        private void ClearScoreForm()
        {
            txtMaDK.Clear(); nudDiemCC.Value = 0; nudDiemGK.Value = 0; nudDiemCK.Value = 0;
            lblTrangThai.Text = "Trạng thái điểm: —";
            lblTrangThai.ForeColor = System.Drawing.SystemColors.ControlText;
        }

        private void ShowResult(OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
