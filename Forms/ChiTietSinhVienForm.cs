using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLySinhVien.BUS;

namespace QuanLySinhVien.Forms
{
    public class ChiTietSinhVienForm : Form
    {
        public ChiTietSinhVienForm(string maSV)
        {
            Text = $"Hồ sơ sinh viên – {maSV}";
            Width  = 780;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            // ── Tiêu đề ─────────────────────────────────────────────────
            var lblTitle = new Label
            {
                Text      = "THÔNG TIN CÁ NHÂN",
                Top       = 10, Left = 10,
                AutoSize  = true,
                Font      = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            // ── Bảng thông tin cá nhân (2 cột) ──────────────────────────
            var tblInfo = new TableLayoutPanel
            {
                Top        = 32, Left = 10,
                Width      = 740, Height = 220,
                ColumnCount = 4, RowCount = 6,
                BorderStyle = BorderStyle.FixedSingle
            };
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            // Các label kết quả — được gán trong LoadData
            var lblMaSV        = MakeInfoLabel();
            var lblHoTen       = MakeInfoLabel();
            var lblNgaySinh    = MakeInfoLabel();
            var lblGioiTinh    = MakeInfoLabel();
            var lblDiaChi      = MakeInfoLabel();
            var lblTinhTrang   = MakeInfoLabel();
            var lblLop         = MakeInfoLabel();
            var lblNganh       = MakeInfoLabel();
            var lblKhoa        = MakeInfoLabel();
            var lblKhoaHoc     = MakeInfoLabel();
            var lblGVCN        = MakeInfoLabel();
            var lblSdtGVCN     = MakeInfoLabel();

            AddRow(tblInfo, "Mã SV:",       lblMaSV,      "Họ tên:",       lblHoTen);
            AddRow(tblInfo, "Ngày sinh:",   lblNgaySinh,  "Giới tính:",    lblGioiTinh);
            AddRow(tblInfo, "Địa chỉ:",     lblDiaChi,    "Tình trạng:",   lblTinhTrang);
            AddRow(tblInfo, "Lớp SH:",      lblLop,       "Ngành:",        lblNganh);
            AddRow(tblInfo, "Khoa:",        lblKhoa,      "Khóa học:",     lblKhoaHoc);
            AddRow(tblInfo, "GVCN:",        lblGVCN,      "SĐT GVCN:",     lblSdtGVCN);

            // ── Lịch sử chuyển lớp ──────────────────────────────────────
            var lblHistory = new Label
            {
                Text      = "LỊCH SỬ CHUYỂN LỚP",
                Top       = 265, Left = 10,
                AutoSize  = true,
                Font      = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            var dgvHistory = new DataGridView
            {
                Top                  = 288, Left = 10,
                Width                = 740, Height = 240,
                ReadOnly             = true,
                AutoSizeColumnsMode  = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode        = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows   = false,
                BackgroundColor      = SystemColors.Window
            };

            // ── Nút đóng ─────────────────────────────────────────────────
            var btnClose = new Button
            {
                Text   = "Đóng",
                Width  = 80, Height = 28,
                Top    = 540, Left  = 340
            };
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblTitle, tblInfo, lblHistory, dgvHistory, btnClose
            });

            // ── Tải dữ liệu ──────────────────────────────────────────────
            Load += (s, e) =>
            {
                try
                {
                    var sets = SinhVienBUS.LayChiTiet(maSV);

                    // Result set 0: thông tin cá nhân
                    if (sets[0].Rows.Count > 0)
                    {
                        var r = sets[0].Rows[0];
                        lblMaSV.Text      = r["MaSV"]?.ToString();
                        lblHoTen.Text     = r["HoTen"]?.ToString();
                        lblNgaySinh.Text  = r["NgaySinh"] == DBNull.Value
                            ? "(chưa có)"
                            : Convert.ToDateTime(r["NgaySinh"]).ToString("dd/MM/yyyy");
                        lblGioiTinh.Text  = r["GioiTinhHienThi"]?.ToString();
                        lblDiaChi.Text    = r["DiaChi"] == DBNull.Value ? "(chưa có)" : r["DiaChi"]?.ToString();
                        lblTinhTrang.Text = r["TinhTrang"]?.ToString();
                        lblLop.Text       = $"{r["MaLopSH"]} – {r["TenLop"]}";
                        lblNganh.Text     = r["TenNganh"]?.ToString();
                        lblKhoa.Text      = r["TenKhoa"]?.ToString();
                        lblKhoaHoc.Text   = r["TenKhoaHoc"]?.ToString();
                        lblGVCN.Text      = r["TenGVCN"]?.ToString();
                        lblSdtGVCN.Text   = r["SdtGVCN"] == DBNull.Value ? "(chưa có)" : r["SdtGVCN"]?.ToString();
                    }
                    else
                    {
                        lblMaSV.Text = "Không tìm thấy sinh viên.";
                    }

                    // Result set 1: lịch sử chuyển lớp
                    dgvHistory.DataSource = sets[1];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        // ── Helpers ──────────────────────────────────────────────────────
        private static Label MakeInfoLabel() =>
            new Label { AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(3) };

        private static Label MakeHeaderLabel(string text) =>
            new Label
            {
                Text      = text,
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Arial", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 230, 241),
                Padding   = new Padding(3)
            };

        private static void AddRow(TableLayoutPanel tbl, string h1, Label v1, string h2, Label v2)
        {
            tbl.Controls.Add(MakeHeaderLabel(h1));
            tbl.Controls.Add(v1);
            tbl.Controls.Add(MakeHeaderLabel(h2));
            tbl.Controls.Add(v2);
        }
    }
}
