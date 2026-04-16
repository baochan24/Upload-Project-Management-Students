using System;
using System.Drawing;
using System.Windows.Forms;

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
            var systemMenu   = new ToolStripMenuItem("Hệ thống");
            var logoutItem   = new ToolStripMenuItem("Đăng xuất", null, Logout_Click);
            var exitItem     = new ToolStripMenuItem("Thoát",     null, Exit_Click);
            systemMenu.DropDownItems.AddRange(new[] { logoutItem, exitItem });

            // ── Danh mục ──
            var dmMenu      = new ToolStripMenuItem("Danh mục");
            var khoaItem    = new ToolStripMenuItem("Khoa",          null, (s, e) => OpenChildForm(new KhoaForm()));
            var nganhItem   = new ToolStripMenuItem("Ngành học",     null, (s, e) => OpenChildForm(new NganhForm()));
            var khoaHocItem = new ToolStripMenuItem("Khóa học",      null, (s, e) => OpenChildForm(new KhoaHocForm()));
            var gvItem      = new ToolStripMenuItem("Giảng viên",    null, (s, e) => OpenChildForm(new GiangVienForm()));
            var phongItem   = new ToolStripMenuItem("Phòng học",     null, (s, e) => OpenChildForm(new PhongHocForm()));
            var hocKyItem   = new ToolStripMenuItem("Học kỳ",        null, (s, e) => OpenChildForm(new HocKyForm()));
            var monHocItem  = new ToolStripMenuItem("Môn học",       null, (s, e) => OpenChildForm(new MonHocForm()));
            dmMenu.DropDownItems.AddRange(new ToolStripItem[] { khoaItem, nganhItem, khoaHocItem, gvItem, phongItem, new ToolStripSeparator(), hocKyItem, monHocItem });

            // ── Quản lý sinh viên ──
            var svMenu      = new ToolStripMenuItem("Sinh viên");
            var svItem      = new ToolStripMenuItem("Sinh viên",      null, (s, e) => OpenChildForm(new SinhVienForm()));
            var lopSHItem   = new ToolStripMenuItem("Lớp sinh hoạt",  null, (s, e) => OpenChildForm(new LopSinhHoatForm()));
            svMenu.DropDownItems.AddRange(new[] { svItem, lopSHItem });

            // ── Học phần & Đăng ký ──
            var hpMenu      = new ToolStripMenuItem("Học phần");
            var lopHPItem   = new ToolStripMenuItem("Lớp học phần",   null, (s, e) => OpenChildForm(new LopHocPhanForm()));
            var dkItem      = new ToolStripMenuItem("Đăng ký HP",     null, (s, e) => OpenChildForm(new DangKyForm()));
            hpMenu.DropDownItems.AddRange(new[] { lopHPItem, dkItem });

            // ── Điểm & Học phí ──
            var diemMenu    = new ToolStripMenuItem("Điểm & Học phí");
            var diemItem    = new ToolStripMenuItem("Quản lý điểm",   null, (s, e) => OpenChildForm(new DiemForm()));
            var hocPhiItem  = new ToolStripMenuItem("Học phí",        null, (s, e) => OpenChildForm(new HocPhiForm()));
            diemMenu.DropDownItems.AddRange(new[] { diemItem, hocPhiItem });

            // ── Báo cáo ──
            var bcMenu      = new ToolStripMenuItem("Báo cáo");
            var bcItem      = new ToolStripMenuItem("Báo cáo & Thống kê", null, (s, e) => OpenChildForm(new BaoCaoForm()));
            bcMenu.DropDownItems.Add(bcItem);

            // ── Quản trị ──
            var adminMenu   = new ToolStripMenuItem("Quản trị");
            var qtItem      = new ToolStripMenuItem("Quản lý tài khoản", null, (s, e) => OpenChildForm(new QuanTriForm()));
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
            statusLabel.Text = $"Đăng nhập: {Models.UserSession.Username} - Vai trò: {Models.UserSession.RoleName}";
            OpenChildForm(new SinhVienForm());
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
