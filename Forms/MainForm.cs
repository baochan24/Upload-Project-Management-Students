using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class MainForm : Form
    {
        private readonly Panel panelContainer;
        private readonly ToolStripStatusLabel statusLabel;

        public MainForm()
        {
            Text = "Quản lý sinh viên";
            WindowState = FormWindowState.Maximized;

            var menuStrip = new MenuStrip();

            // ── Hệ thống ──
            var systemMenu = new ToolStripMenuItem("Hệ thống");
            var logoutItem = new ToolStripMenuItem("Đăng xuất", null, Logout_Click);
            var exitItem   = new ToolStripMenuItem("Thoát",     null, Exit_Click);
            systemMenu.DropDownItems.AddRange(new[] { logoutItem, exitItem });

            // ── Danh mục (Admin / PhongDT) ──
            var dmMenu      = new ToolStripMenuItem("Danh mục");
            var khoaItem    = new ToolStripMenuItem("Khoa",       null, (s, e) => OpenChildForm(new KhoaForm()));
            var nganhItem   = new ToolStripMenuItem("Ngành học",  null, (s, e) => OpenChildForm(new NganhForm()));
            var khoaHocItem = new ToolStripMenuItem("Khóa học",   null, (s, e) => OpenChildForm(new KhoaHocForm()));
            var gvItem      = new ToolStripMenuItem("Giảng viên", null, (s, e) => OpenChildForm(new GiangVienForm()));
            var phongItem   = new ToolStripMenuItem("Phòng học",  null, (s, e) => OpenChildForm(new PhongHocForm()));
            var hocKyItem   = new ToolStripMenuItem("Học kỳ",     null, (s, e) => OpenChildForm(new HocKyForm()));
            var monHocItem  = new ToolStripMenuItem("Môn học",    null, (s, e) => OpenChildForm(new MonHocForm()));
            dmMenu.DropDownItems.AddRange(new ToolStripItem[] { khoaItem, nganhItem, khoaHocItem, gvItem, phongItem, new ToolStripSeparator(), hocKyItem, monHocItem });

            // ── Sinh viên (Admin / PhongDT) ──
            var svMenu    = new ToolStripMenuItem("Sinh viên");
            var svItem    = new ToolStripMenuItem("Sinh viên",     null, (s, e) => OpenChildForm(new SinhVienForm()));
            var lopSHItem = new ToolStripMenuItem("Lớp sinh hoạt", null, (s, e) => OpenChildForm(new LopSinhHoatForm()));
            svMenu.DropDownItems.AddRange(new[] { svItem, lopSHItem });

            // ── Học phần (Admin / PhongDT / GiangVien) ──
            var hpMenu    = new ToolStripMenuItem("Học phần");
            var lopHPItem = new ToolStripMenuItem("Lớp học phần", null, (s, e) => OpenChildForm(new LopHocPhanForm()));
            var dkItem    = new ToolStripMenuItem("Đăng ký HP",   null, (s, e) => OpenChildForm(new DangKyForm()));
            // Sinh viên: chỉ thấy TKB cá nhân
            var tkbItem   = new ToolStripMenuItem("Thời khóa biểu", null, (s, e) => OpenChildForm(new ThoiKhoaBieuForm()));
            hpMenu.DropDownItems.AddRange(new ToolStripItem[] { lopHPItem, dkItem, new ToolStripSeparator(), tkbItem });

            // ── Điểm & Học phí ──
            var diemMenu   = new ToolStripMenuItem("Điểm & Học phí");
            var diemItem   = new ToolStripMenuItem("Quản lý điểm",    null, (s, e) => OpenChildForm(new DiemForm()));
            var hocPhiItem = new ToolStripMenuItem("Học phí",          null, (s, e) => OpenChildForm(new HocPhiForm()));
            var xemHpItem  = new ToolStripMenuItem("Xem học phí",      null, (s, e) => OpenChildForm(new XemHocPhiForm()));
            diemMenu.DropDownItems.AddRange(new ToolStripItem[] { diemItem, hocPhiItem, new ToolStripSeparator(), xemHpItem });

            // ── Báo cáo ──
            var bcMenu  = new ToolStripMenuItem("Báo cáo");
            var bcItem  = new ToolStripMenuItem("Báo cáo & Thống kê", null, (s, e) => OpenChildForm(new BaoCaoForm()));
            var bdItem  = new ToolStripMenuItem("Bảng điểm cá nhân",  null, (s, e) => OpenChildForm(new BangDiemCaNhanForm()));
            bcMenu.DropDownItems.AddRange(new ToolStripItem[] { bcItem, new ToolStripSeparator(), bdItem });

            // ── Quản trị (Admin only) ──
            var adminMenu = new ToolStripMenuItem("Quản trị");
            var qtItem    = new ToolStripMenuItem("Quản lý tài khoản", null, (s, e) => OpenChildForm(new QuanTriForm()));
            adminMenu.DropDownItems.Add(qtItem);

            menuStrip.Items.AddRange(new ToolStripItem[] { systemMenu, dmMenu, svMenu, hpMenu, diemMenu, bcMenu, adminMenu });
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            panelContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke, Padding = new Padding(12) };
            Controls.Add(panelContainer);

            var statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel { Text = "Chào mừng" };
            statusStrip.Items.Add(statusLabel);
            Controls.Add(statusStrip);

            Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            statusLabel.Text = $"Đăng nhập: {UserSession.Username} - Vai trò: {UserSession.RoleName}";
            ApplyRoleVisibility();

            // Mở form mặc định theo vai trò
            if (UserSession.IsSinhVien)
                OpenChildForm(new ThoiKhoaBieuForm());
            else if (UserSession.IsGiangVien)
                OpenChildForm(new DiemForm());
            else
                OpenChildForm(new SinhVienForm());
        }

        /// <summary>Ẩn/hiện menu theo vai trò người dùng.</summary>
        private void ApplyRoleVisibility()
        {
            bool isAdminOrStaff = UserSession.IsAdmin || UserSession.IsStaff;
            bool isGV           = UserSession.IsGiangVien;
            bool isSV           = UserSession.IsSinhVien;

            // Lấy menu items theo index (thứ tự: systemMenu=0, dmMenu=1, svMenu=2, hpMenu=3, diemMenu=4, bcMenu=5, adminMenu=6)
            var items = MainMenuStrip.Items;

            // Danh mục – chỉ Admin/PhongDT
            items[1].Visible = isAdminOrStaff;

            // Sinh viên – chỉ Admin/PhongDT
            items[2].Visible = isAdminOrStaff;

            // Học phần
            if (items[3] is ToolStripMenuItem hpMenu)
            {
                // "Lớp học phần": Admin/PhongDT/GiangVien
                hpMenu.DropDownItems[0].Visible = isAdminOrStaff || isGV;
                // "Đăng ký HP" (quản lý): Admin/PhongDT
                hpMenu.DropDownItems[1].Visible = isAdminOrStaff;
                // separator
                hpMenu.DropDownItems[2].Visible = true;
                // "Thời khóa biểu": tất cả (nhưng hữu ích nhất với SV)
                hpMenu.DropDownItems[3].Visible = true;
            }

            // Điểm & Học phí
            if (items[4] is ToolStripMenuItem diemMenu)
            {
                // "Quản lý điểm": Admin/PhongDT/GiangVien
                diemMenu.DropDownItems[0].Visible = isAdminOrStaff || isGV;
                // "Học phí" (báo cáo toàn trường): Admin/PhongDT
                diemMenu.DropDownItems[1].Visible = isAdminOrStaff;
                // separator
                diemMenu.DropDownItems[2].Visible = true;
                // "Xem học phí" (cá nhân): SinhVien
                diemMenu.DropDownItems[3].Visible = isSV || isAdminOrStaff;
            }

            // Báo cáo
            if (items[5] is ToolStripMenuItem bcMenu)
            {
                // "Báo cáo & Thống kê": Admin/PhongDT
                bcMenu.DropDownItems[0].Visible = isAdminOrStaff;
                // separator
                bcMenu.DropDownItems[1].Visible = true;
                // "Bảng điểm cá nhân": tất cả
                bcMenu.DropDownItems[2].Visible = true;
            }

            // Quản trị – chỉ Admin
            items[6].Visible = UserSession.IsAdmin;
        }

        private void OpenChildForm(Form child)
        {
            panelContainer.Controls.Clear();
            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;
            panelContainer.Controls.Add(child);
            child.Show();
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Models.UserSession.Clear();
                Hide();
                using (var loginForm = new LoginForm())
                {
                    loginForm.ShowDialog();
                }
                Close();
            }
        }

        private void Exit_Click(object sender, EventArgs e) => Close();
    }
}
