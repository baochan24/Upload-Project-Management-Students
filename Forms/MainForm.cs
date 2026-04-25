using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class MainForm : Form
    {
        private readonly Panel panelContainer;
        private readonly ToolStripStatusLabel statusLabel;
        private readonly Panel _topBanner;
        private readonly MenuStrip _menuStrip;
        private readonly StatusStrip _statusStrip;
        private System.Windows.Forms.Timer _clockTimer;

        public MainForm()
        {
            Text = "Quản lý sinh viên";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);
            BackColor = ThemeConfig.BgMain;
            Font = ThemeConfig.FontDefault;

            UIHelper.EnableOptimizedRendering(this);
            SuspendLayout();

            _menuStrip = BuildMenuStrip();
            _topBanner = BuildTopBanner();

            _statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel { Text = "Chào mừng" };
            _statusStrip.Items.Add(statusLabel);

            panelContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeConfig.BgMain,
                Padding = new Padding(12)
            };
            UIHelper.EnableOptimizedRendering(panelContainer);

            Controls.Add(panelContainer);
            Controls.Add(_statusStrip);
            Controls.Add(_topBanner);
            Controls.Add(_menuStrip);

            Load += MainForm_Load;
            ApplyTheme();

            ResumeLayout(false);
        }

        private MenuStrip BuildMenuStrip()
        {
            var menuStrip = new MenuStrip
            {
                Dock = DockStyle.Top
            };

            var systemMenu = new ToolStripMenuItem("Hệ thống");
            var logoutItem = new ToolStripMenuItem("Đăng xuất", null, Logout_Click);
            var exitItem = new ToolStripMenuItem("Thoát", null, Exit_Click);
            systemMenu.DropDownItems.AddRange(new[] { logoutItem, exitItem });

            var dmMenu = new ToolStripMenuItem("Danh mục");
            var khoaItem = new ToolStripMenuItem("Khoa", null, (s, e) => OpenChildForm(new KhoaForm()));
            var nganhItem = new ToolStripMenuItem("Ngành học", null, (s, e) => OpenChildForm(new NganhForm()));
            var khoaHocItem = new ToolStripMenuItem("Khóa học", null, (s, e) => OpenChildForm(new KhoaHocForm()));
            var gvItem = new ToolStripMenuItem("Giảng viên", null, (s, e) => OpenChildForm(new GiangVienForm()));
            var phongItem = new ToolStripMenuItem("Phòng học", null, (s, e) => OpenChildForm(new PhongHocForm()));
            var hocKyItem = new ToolStripMenuItem("Học kỳ", null, (s, e) => OpenChildForm(new HocKyForm()));
            var monHocItem = new ToolStripMenuItem("Môn học", null, (s, e) => OpenChildForm(new MonHocForm()));
            dmMenu.DropDownItems.AddRange(new ToolStripItem[] { khoaItem, nganhItem, khoaHocItem, gvItem, phongItem, new ToolStripSeparator(), hocKyItem, monHocItem });

            var svMenu = new ToolStripMenuItem("Sinh viên");
            var svItem = new ToolStripMenuItem("Sinh viên", null, (s, e) => OpenChildForm(new SinhVienForm()));
            var lopSHItem = new ToolStripMenuItem("Lớp sinh hoạt", null, (s, e) => OpenChildForm(new LopSinhHoatForm()));
            svMenu.DropDownItems.AddRange(new[] { svItem, lopSHItem });

            var hpMenu = new ToolStripMenuItem("Học phần");
            var lopHPItem = new ToolStripMenuItem("Lớp học phần", null, (s, e) => OpenChildForm(new LopHocPhanForm()));
            var dkItem = new ToolStripMenuItem("Đăng ký HP", null, (s, e) => OpenChildForm(new DangKyForm()));
            var tkbItem = new ToolStripMenuItem("Thời khóa biểu", null, (s, e) => OpenChildForm(new ThoiKhoaBieuForm()));
            hpMenu.DropDownItems.AddRange(new ToolStripItem[] { lopHPItem, dkItem, new ToolStripSeparator(), tkbItem });

            var diemMenu = new ToolStripMenuItem("Điểm & Học phí");
            var diemItem = new ToolStripMenuItem("Quản lý điểm", null, (s, e) => OpenChildForm(new DiemForm()));
            var hocPhiItem = new ToolStripMenuItem("Học phí", null, (s, e) => OpenChildForm(new HocPhiForm()));
            var xemHpItem = new ToolStripMenuItem("Xem học phí", null, (s, e) => OpenChildForm(new XemHocPhiForm()));
            diemMenu.DropDownItems.AddRange(new ToolStripItem[] { diemItem, hocPhiItem, new ToolStripSeparator(), xemHpItem });

            var bcMenu = new ToolStripMenuItem("Báo cáo");
            var bcItem = new ToolStripMenuItem("Báo cáo & Thống kê", null, (s, e) => OpenChildForm(new BaoCaoForm()));
            var bdItem = new ToolStripMenuItem("Bảng điểm cá nhân", null, (s, e) => OpenChildForm(new BangDiemCaNhanForm()));
            bcMenu.DropDownItems.AddRange(new ToolStripItem[] { bcItem, new ToolStripSeparator(), bdItem });

            var adminMenu = new ToolStripMenuItem("Quản trị");
            var qtItem = new ToolStripMenuItem("Quản lý tài khoản", null, (s, e) => OpenChildForm(new QuanTriForm()));
            adminMenu.DropDownItems.Add(qtItem);

            menuStrip.Items.AddRange(new ToolStripItem[] { systemMenu, dmMenu, svMenu, hpMenu, diemMenu, bcMenu, adminMenu });
            MainMenuStrip = menuStrip;
            return menuStrip;
        }

        private Panel BuildTopBanner()
        {
            var banner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                Padding = new Padding(18, 0, 18, 0)
            };
            UIHelper.EnableOptimizedRendering(banner);
            banner.Paint += (s, e) => UIHelper.PaintGradient(e, ThemeConfig.PrimaryDark, ThemeConfig.PrimaryMid, LinearGradientMode.Horizontal);

            var lblAppName = new Label
            {
                Dock = DockStyle.Fill,
                Text = "QUẢN LÝ SINH VIÊN",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblTagline = new Label
            {
                Dock = DockStyle.Right,
                Width = 280,
                Text = "Hệ thống quản lý đào tạo",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(220, 232, 255),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };

            banner.Controls.Add(lblAppName);
            banner.Controls.Add(lblTagline);
            return banner;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            statusLabel.Text = $"Chào  {UserSession.Username}   |   Vai trò: {UserSession.RoleName}";
            ApplyRoleVisibility();

            if (UserSession.IsSinhVien)
                OpenChildForm(new ThoiKhoaBieuForm());
            else if (UserSession.IsGiangVien)
                OpenChildForm(new DiemForm());
            else
                OpenChildForm(new SinhVienForm());
        }

        private void ApplyRoleVisibility()
        {
            bool isAdminOrStaff = UserSession.IsAdmin || UserSession.IsStaff;
            bool isGV = UserSession.IsGiangVien;
            bool isSV = UserSession.IsSinhVien;

            var items = MainMenuStrip.Items;

            items[1].Visible = isAdminOrStaff;
            items[2].Visible = isAdminOrStaff;

            if (items[3] is ToolStripMenuItem hpMenu)
            {
                hpMenu.DropDownItems[0].Visible = isAdminOrStaff || isGV;
                hpMenu.DropDownItems[1].Visible = isAdminOrStaff;
                hpMenu.DropDownItems[2].Visible = true;
                hpMenu.DropDownItems[3].Visible = true;
            }

            if (items[4] is ToolStripMenuItem diemMenu)
            {
                diemMenu.DropDownItems[0].Visible = isAdminOrStaff || isGV;
                diemMenu.DropDownItems[1].Visible = isAdminOrStaff;
                diemMenu.DropDownItems[2].Visible = true;
                diemMenu.DropDownItems[3].Visible = isSV || isAdminOrStaff;
            }

            if (items[5] is ToolStripMenuItem bcMenu)
            {
                bcMenu.DropDownItems[0].Visible = isAdminOrStaff;
                bcMenu.DropDownItems[1].Visible = true;
                bcMenu.DropDownItems[2].Visible = true;
            }

            items[6].Visible = UserSession.IsAdmin;
        }

        private void OpenChildForm(Form child)
        {
            panelContainer.SuspendLayout();
            panelContainer.Controls.Clear();

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;
            child.Margin = Padding.Empty;

            UIHelper.ApplyFormTheme(child);
            panelContainer.Controls.Add(child);
            child.Show();

            panelContainer.ResumeLayout(true);
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

        private void ApplyTheme()
        {
            BackColor = ThemeConfig.BgMain;
            Font = ThemeConfig.FontDefault;

            UIHelper.StyleMenuStrip(MainMenuStrip);

            panelContainer.BackColor = ThemeConfig.BgMain;
            panelContainer.Padding = new Padding(8);

            UIHelper.StyleStatusStrip(_statusStrip);

            var spacer = new ToolStripStatusLabel { Spring = true };
            var lblOnline = new ToolStripStatusLabel
            {
                Text = "●  Trực tuyến",
                ForeColor = Color.FromArgb(100, 230, 140),
                Font = ThemeConfig.FontSmall
            };
            var clockLabel = new ToolStripStatusLabel
            {
                Text = DateTime.Now.ToString("HH:mm:ss  |  dd/MM/yyyy"),
                ForeColor = Color.FromArgb(200, 220, 255),
                Font = ThemeConfig.FontSmall,
                Alignment = ToolStripItemAlignment.Right
            };

            if (_statusStrip.Items.Count < 4)
            {
                _statusStrip.Items.Add(spacer);
                _statusStrip.Items.Add(lblOnline);
                _statusStrip.Items.Add(new ToolStripSeparator());
                _statusStrip.Items.Add(clockLabel);
            }

            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) => clockLabel.Text = DateTime.Now.ToString("HH:mm:ss  |  dd/MM/yyyy");
            _clockTimer.Start();

            FormClosed += (s, e) => _clockTimer?.Dispose();
        }
    }
}
