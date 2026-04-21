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
            Text   = "Thời khóa biểu";
            Width  = 1100;
            Height = 620;

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

            dgv = new DataGridView
            {
                Dock                = DockStyle.Fill,
                ReadOnly            = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows  = false,
                BackgroundColor     = Color.White
            };

            Controls.Add(panel);
            Controls.Add(dgv);

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
                    return;
                }

                var dt = DangKyBUS.LoadBySinhVien(maSV);
                dgv.DataSource = dt;
                lblInfo.Text = $"Thời khóa biểu – Sinh viên: {maSV}  ({dt.Rows.Count} lớp học phần)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được dữ liệu:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
