using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLySinhVien.BUS;

namespace QuanLySinhVien.Forms
{
    public class DangKyForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox      txtMaSV;
        private readonly Label        lblThongTinSV;
        private readonly ComboBox     cmbLopHocPhan;
        private readonly TextBox      txtSearch;

        public DangKyForm()
        {
            Text    = "Quản lý đăng ký học phần";
            Width   = 1080;
            Height  = 700;

            // ── DataGridView ──────────────────────────────────────────────
            dgv = new DataGridView
            {
                Dock                  = DockStyle.Bottom,
                Height                = 420,
                ReadOnly              = true,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                BackgroundColor       = SystemColors.Window
            };

            // ── Panel điều khiển ─────────────────────────────────────────
            var panel = new Panel { Dock = DockStyle.Top, Height = 230, Padding = new Padding(10) };

            // ─ Cột trái: nhập sinh viên ───────────────────────────────
            panel.Controls.Add(new Label { Text = "Mã sinh viên", Top = 10, Left = 10, AutoSize = true });
            txtMaSV = new TextBox { Top = 30, Left = 10, Width = 180 };
            txtMaSV.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) TimSinhVien(); };
            panel.Controls.Add(txtMaSV);

            var btnTimSV = new Button { Text = "Tìm SV", Top = 28, Left = 200, Width = 80, Height = 26 };
            btnTimSV.Click += (s, e) => TimSinhVien();
            panel.Controls.Add(btnTimSV);

            lblThongTinSV = new Label
            {
                Top       = 62,
                Left      = 10,
                Width     = 620,
                Height    = 20,
                AutoSize  = false,
                ForeColor = Color.DarkBlue
            };
            panel.Controls.Add(lblThongTinSV);

            // ─ Lớp học phần ──────────────────────────────────────────
            panel.Controls.Add(new Label { Text = "Lớp học phần", Top = 95, Left = 10, AutoSize = true });
            cmbLopHocPhan = new ComboBox
            {
                Top           = 115,
                Left          = 10,
                Width         = 630,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DropDownWidth = 780
            };
            panel.Controls.Add(cmbLopHocPhan);

            // ─ Cột phải: tìm kiếm + nút ──────────────────────────────
            panel.Controls.Add(new Label { Text = "Tìm kiếm đăng ký", Top = 10, Left = 700, AutoSize = true });
            txtSearch = new TextBox { Top = 30, Left = 700, Width = 240 };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadData(txtSearch.Text.Trim()); };
            panel.Controls.Add(txtSearch);

            var btnSearch  = new Button { Text = "Tìm kiếm",     Top = 58,  Left = 700, Width = 115 };
            var btnRefresh = new Button { Text = "Làm mới",       Top = 58,  Left = 825, Width = 115 };
            var btnDangKy  = new Button
            {
                Text      = "✔ Đăng ký HP",
                Top       = 100, Left = 700, Width = 240, Height = 34,
                BackColor = Color.FromArgb(144, 238, 144),
                Font      = new Font(Font, FontStyle.Bold)
            };
            var btnHuyDK = new Button
            {
                Text      = "✘ Hủy đăng ký",
                Top       = 144, Left = 700, Width = 240, Height = 34,
                BackColor = Color.FromArgb(255, 160, 122),
                Font      = new Font(Font, FontStyle.Bold)
            };

            btnSearch.Click  += (s, e) => LoadData(txtSearch.Text.Trim());
            btnRefresh.Click += (s, e) => { ClearForm(); LoadData(); };
            btnDangKy.Click  += BtnDangKy_Click;
            btnHuyDK.Click   += BtnHuyDK_Click;

            panel.Controls.AddRange(new Control[] { btnSearch, btnRefresh, btnDangKy, btnHuyDK });

            Controls.Add(panel);
            Controls.Add(dgv);

            Load += DangKyForm_Load;
        }

        // ─── Load ────────────────────────────────────────────────────────
        private void DangKyForm_Load(object sender, EventArgs e)
        {
            LoadLopHocPhanCombo();
            LoadData();
        }

        private void LoadLopHocPhanCombo()
        {
            try
            {
                var dt = LopHocPhanBUS.LoadAll();

                // Thêm cột hiển thị thân thiện
                dt.Columns.Add("TenHienThi", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    string thuTiet = "";
                    if (row["Thu"] != DBNull.Value && row["TietBatDau"] != DBNull.Value)
                        thuTiet = $"  Thứ {row["Thu"]} t.{row["TietBatDau"]}-{row["TietKetThuc"]}";

                    string siso = $"{row["SiSoHienTai"]}/{row["SiSoToiDa"]}";
                    string trangThai = row["TrangThai"]?.ToString() ?? "";
                    string dau = trangThai == "Đang mở" ? "●" : "○";

                    row["TenHienThi"] =
                        $"{dau} [{row["MaLopHienThi"]}] {row["TenMon"]}" +
                        $"  |  GV: {row["TenGV"]}" +
                        $"  |  {row["TenHocKy"]}" +
                        $"{thuTiet}  |  SV: {siso}  ({trangThai})";
                }

                cmbLopHocPhan.DataSource    = dt;
                cmbLopHocPhan.DisplayMember = "TenHienThi";
                cmbLopHocPhan.ValueMember   = "MaLHP";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách lớp học phần:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData(string keyword = null)
        {
            try   { dgv.DataSource = DangKyBUS.LoadAll(keyword); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ─── Tìm sinh viên ───────────────────────────────────────────────
        private void TimSinhVien()
        {
            var maSV = txtMaSV.Text.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(maSV)) { lblThongTinSV.Text = ""; return; }

            try
            {
                var dt = SinhVienBUS.LoadAll(keyword: maSV);
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    txtMaSV.Text          = row["MaSV"]?.ToString();
                    lblThongTinSV.ForeColor = Color.DarkBlue;
                    lblThongTinSV.Text    =
                        $"✔  {row["HoTen"]}   |   Lớp: {row["TenLop"]}   |   {row["TinhTrang"]}";
                }
                else
                {
                    lblThongTinSV.ForeColor = Color.Red;
                    lblThongTinSV.Text      = "✘  Không tìm thấy sinh viên với mã này.";
                }
            }
            catch (Exception ex)
            {
                lblThongTinSV.ForeColor = Color.Red;
                lblThongTinSV.Text      = "Lỗi: " + ex.Message;
            }
        }

        // ─── Đăng ký ────────────────────────────────────────────────────
        private void BtnDangKy_Click(object sender, EventArgs e)
        {
            var maSV  = txtMaSV.Text.Trim().ToUpperInvariant();
            var maLHP = cmbLopHocPhan.SelectedValue?.ToString();

            if (string.IsNullOrWhiteSpace(maSV))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên (và bấm Tìm SV để xác nhận).",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(maLHP))
            {
                MessageBox.Show("Vui lòng chọn lớp học phần.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var r = DangKyBUS.Register(maSV, maLHP);
            ShowResult(r);
            if (r.Success)
            {
                LoadData(txtSearch.Text.Trim());
                LoadLopHocPhanCombo();   // cập nhật sĩ số trong combo
            }
        }

        // ─── Hủy đăng ký ────────────────────────────────────────────────
        private void BtnHuyDK_Click(object sender, EventArgs e)
        {
            if (!(dgv.CurrentRow?.DataBoundItem is DataRowView row))
            {
                MessageBox.Show("Vui lòng chọn một dòng đăng ký trong danh sách để hủy.",
                    "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var trangThai = row["TrangThai"]?.ToString();
            if (trangThai == "Đã hủy")
            {
                MessageBox.Show("Đăng ký này đã bị hủy trước đó.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var maSV   = row["MaSV"]?.ToString();
            var maLHP  = row["MaLHP"]?.ToString();
            var tenMon = row["TenMon"]?.ToString();

            if (MessageBox.Show(
                    $"Hủy đăng ký môn \"{tenMon}\" của sinh viên {maSV}?",
                    "Xác nhận hủy đăng ký",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var r = DangKyBUS.Cancel(maSV, maLHP);
            ShowResult(r);
            if (r.Success)
            {
                LoadData(txtSearch.Text.Trim());
                LoadLopHocPhanCombo();
            }
        }

        private void ClearForm()
        {
            txtMaSV.Text       = string.Empty;
            txtSearch.Text     = string.Empty;
            lblThongTinSV.Text = string.Empty;
        }

        private void ShowResult(Models.OperationResult r) =>
            MessageBox.Show(r.Message,
                r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
