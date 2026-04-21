using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class XemHocPhiForm : Form
    {
        private readonly ComboBox     cmbHocKy;
        private readonly DataGridView dgvDetail;
        private readonly DataGridView dgvSummary;
        private readonly Label        lblTitle;

        public XemHocPhiForm()
        {
            Text   = "Xem học phí";
            Width  = 900;
            Height = 680;

            // ── Panel điều khiển ──────────────────────────────────────────
            var panel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(10) };

            panel.Controls.Add(new Label { Text = "Học kỳ (bỏ trống = tất cả)", Top = 8, Left = 10, AutoSize = true });
            cmbHocKy = new ComboBox { Top = 26, Left = 10, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbHocKy);

            var btnLoad = new Button { Text = "Tải học phí", Top = 24, Left = 285, Width = 100 };
            btnLoad.Click += BtnLoad_Click;
            panel.Controls.Add(btnLoad);

            // ── Tiêu đề thông tin SV ─────────────────────────────────────
            lblTitle = new Label
            {
                Dock      = DockStyle.None,
                Top       = 55,
                Left      = 0,
                Height    = 28,
                Width     = 900,
                Padding   = new Padding(10, 5, 0, 0),
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Text      = "",
                BackColor = Color.AliceBlue
            };

            // ── Chi tiết học phần ─────────────────────────────────────────
            var lblDetail = new Label { Text = "Chi tiết học phần:", Dock = DockStyle.None, AutoSize = true };
            dgvDetail = new DataGridView
            {
                Height              = 280,
                Dock                = DockStyle.None,
                ReadOnly            = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows  = false,
                BackgroundColor     = Color.White
            };

            // ── Tổng kết ──────────────────────────────────────────────────
            var lblSummary = new Label { Text = "Tổng kết học phí:", Dock = DockStyle.None, AutoSize = true };
            dgvSummary = new DataGridView
            {
                Height              = 120,
                Dock                = DockStyle.None,
                ReadOnly            = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows  = false,
                BackgroundColor     = Color.White
            };

            // Layout bằng TableLayoutPanel
            var layout = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 1,
                RowCount    = 4,
                Padding     = new Padding(8)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 35));

            layout.Controls.Add(lblDetail,  0, 0);
            layout.Controls.Add(dgvDetail,  0, 1);
            layout.Controls.Add(lblSummary, 0, 2);
            layout.Controls.Add(dgvSummary, 0, 3);

            dgvDetail.Dock  = DockStyle.Fill;
            dgvSummary.Dock = DockStyle.Fill;

            Controls.Add(layout);
            Controls.Add(lblTitle);
            Controls.Add(panel);

            Load += XemHocPhiForm_Load;
        }

        private void XemHocPhiForm_Load(object sender, EventArgs e)
        {
            try
            {
                var dtHocKy = HocKyBUS.LoadAll();
                cmbHocKy.DisplayMember = "TenHocKy";
                cmbHocKy.ValueMember   = "MaHocKy";

                var dtAll  = dtHocKy.Copy();
                var allRow = dtAll.NewRow();
                allRow["MaHocKy"]  = DBNull.Value;
                allRow["TenHocKy"] = "(Tất cả học kỳ)";
                dtAll.Rows.InsertAt(allRow, 0);

                cmbHocKy.DataSource    = dtAll;
                cmbHocKy.SelectedIndex = 0;

                // Tự động tải nếu đây là tài khoản sinh viên
                if (UserSession.IsSinhVien)
                    LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách học kỳ:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e) => LoadData();

        private void LoadData()
        {
            string maSV = UserSession.IsSinhVien ? UserSession.MaSV : null;
            if (string.IsNullOrWhiteSpace(maSV))
            {
                MessageBox.Show("Không xác định được mã sinh viên.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maHocKy = null;
            if (cmbHocKy.SelectedValue != null && !(cmbHocKy.SelectedValue is DBNull))
                maHocKy = cmbHocKy.SelectedValue.ToString();

            try
            {
                var ds = HocPhiBUS.LoadBySinhVien(maSV, maHocKy);

                if (ds.Tables.Count > 0)
                {
                    dgvDetail.DataSource = ds.Tables[0];
                    lblTitle.Text = $"Học phí – Sinh viên: {maSV}  ({ds.Tables[0].Rows.Count} học phần)";
                }

                if (ds.Tables.Count > 1)
                    dgvSummary.DataSource = ds.Tables[1];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được dữ liệu học phí:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
