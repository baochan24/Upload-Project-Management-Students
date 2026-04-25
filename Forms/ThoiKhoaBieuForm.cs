using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class ThoiKhoaBieuForm : Form
    {
        private readonly DataGridView dgv;
        private readonly Label lblInfo;

        public ThoiKhoaBieuForm()
        {
            Text = "Thời khóa biểu";
            // Không set Width/Height cứng – để Dock=Fill từ MainForm quyết định

            // ── Header panel ──────────────────────────────────────────────
            var panel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };

            lblInfo = new Label
            {
                Text     = "Danh sách môn học đã đăng ký",
                Top      = 14,
                Left     = 10,
                AutoSize = true,
                Font     = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var btnRefresh = new Button { Text = "Làm mới", Top = 12, Left = 400, Width = 90 };
            btnRefresh.Click += (s, e) => LoadData();

            panel.Controls.AddRange(new Control[] { lblInfo, btnRefresh });

            // ── DataGridView ──────────────────────────────────────────────
            dgv = new DataGridView
            {
                Dock                = DockStyle.Fill,
                ReadOnly            = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows  = false,
                BackgroundColor     = Color.White,
                ColumnHeadersHeight = 36,
                RowTemplate         = { Height = 30 }
            };

            // Thứ tự add: Fill (dgv) trước, Top (panel) sau
            // → WinForms reverse-order: panel(Top, index 1) dock trước → y=0; dgv(Fill, index 0) điền còn lại
            Controls.Add(dgv);    // Fill – index 0
            Controls.Add(panel);  // Top  – index 1 (last → processed first in reverse → docks top)

            Load += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                string maSV = UserSession.MaSV;
                if (string.IsNullOrWhiteSpace(maSV))
                {
                    lblInfo.Text = "Không xác định được mã sinh viên.";
                    dgv.DataSource = null;
                    return;
                }

                var dt = DangKyBUS.LoadBySinhVien(maSV);
                dgv.DataSource = dt;
                ConfigureColumns();
                lblInfo.Text = $"Thời khóa biểu – Sinh viên: {maSV}  ({dt.Rows.Count} lớp học phần)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được dữ liệu:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Đặt header tiếng Việt và ẩn cột kỹ thuật sau khi bind DataSource.</summary>
        private void ConfigureColumns()
        {
            if (dgv.Columns.Count == 0) return;

            // Ẩn các cột kỹ thuật không cần thiết cho sinh viên xem
            HideCol("MaDK");
            HideCol("MaSV");
            HideCol("TenSinhVien");
            HideCol("NgayDangKy");
            HideCol("MaLHP");

            // Đặt tiêu đề tiếng Việt
            RenameCol("MaLopHienThi", "Lớp học phần");
            RenameCol("TenMon",       "Tên môn học");
            RenameCol("SoTinChi",     "Số TC");
            RenameCol("TenHocKy",     "Học kỳ");
            RenameCol("NamHoc",       "Năm học");
            RenameCol("Thu",          "Thứ");
            RenameCol("TietBatDau",   "Tiết BĐ");
            RenameCol("TietKetThuc",  "Tiết KT");
            RenameCol("TrangThai",    "Trạng thái");

            // Đặt chiều rộng cố định cho cột nhỏ
            SetWidth("SoTinChi",   60);
            SetWidth("Thu",        55);
            SetWidth("TietBatDau", 65);
            SetWidth("TietKetThuc",65);
        }

        private void HideCol(string name)
        {
            if (dgv.Columns.Contains(name)) dgv.Columns[name].Visible = false;
        }

        private void RenameCol(string name, string header)
        {
            if (dgv.Columns.Contains(name)) dgv.Columns[name].HeaderText = header;
        }

        private void SetWidth(string name, int width)
        {
            if (dgv.Columns.Contains(name))
            {
                dgv.Columns[name].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns[name].Width        = width;
            }
        }
    }
}
