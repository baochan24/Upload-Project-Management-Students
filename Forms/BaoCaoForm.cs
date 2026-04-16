using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.Forms
{
    public class BaoCaoForm : Form
    {
        public BaoCaoForm()
        {
            Text = "Báo cáo & Thống kê";
            Width = 1100;
            Height = 720;

            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildDashboardTab());
            tabs.TabPages.Add(BuildBaoCaoHocPhiTab());
            tabs.TabPages.Add(BuildTimKiemNangCaoTab());

            Controls.Add(tabs);
        }

        // ──────────────────────────────────────────
        //  TAB 1: DASHBOARD TỔNG HỢP
        // ──────────────────────────────────────────
        private TabPage BuildDashboardTab()
        {
            var page = new TabPage("Dashboard tổng hợp");
            var btnLoad = new Button { Text = "Tải dữ liệu", Top = 8, Left = 8, Width = 120 };
            var lblTitle1 = new Label { Text = "Tổng quan hệ thống", Top = 45, Left = 8, AutoSize = true, Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
            var dgvOverview = new DataGridView { Top = 65, Left = 8, Width = 500, Height = 130, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var lblTitle2 = new Label { Text = "Tình trạng sinh viên", Top = 210, Left = 8, AutoSize = true, Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
            var dgvTinhTrang = new DataGridView { Top = 230, Left = 8, Width = 350, Height = 150, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var lblTitle3 = new Label { Text = "Top 5 môn tỷ lệ đậu cao nhất", Top = 45, Left = 540, AutoSize = true, Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
            var dgvTopMon = new DataGridView { Top = 65, Left = 540, Width = 480, Height = 150, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var lblTitle4 = new Label { Text = "Học phí theo học kỳ", Top = 230, Left = 540, AutoSize = true, Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
            var dgvHocPhi = new DataGridView { Top = 250, Left = 540, Width = 480, Height = 360, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            btnLoad.Click += (s, e) =>
            {
                try
                {
                    var results = BaoCaoDAL.Dashboard();
                    if (results.Length > 0) dgvOverview.DataSource  = results[0];
                    if (results.Length > 1) dgvTinhTrang.DataSource = results[1];
                    if (results.Length > 2) dgvTopMon.DataSource    = results[2];
                    if (results.Length > 3) dgvHocPhi.DataSource    = results[3];
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            page.Controls.AddRange(new Control[] { btnLoad, lblTitle1, dgvOverview, lblTitle2, dgvTinhTrang, lblTitle3, dgvTopMon, lblTitle4, dgvHocPhi });
            return page;
        }

        // ──────────────────────────────────────────
        //  TAB 2: BÁO CÁO HỌC PHÍ
        // ──────────────────────────────────────────
        private TabPage BuildBaoCaoHocPhiTab()
        {
            var page = new TabPage("Báo cáo học phí");

            var lblHK = new Label { Text = "Học kỳ:", Top = 12, Left = 8, AutoSize = true };
            var cmbHocKy = new ComboBox { Top = 8, Left = 60, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            var btnLoad  = new Button { Text = "Xem báo cáo", Top = 6, Left = 280, Width = 120 };

            var lblDetail = new Label { Text = "Chi tiết học phí sinh viên", Top = 50, Left = 8, AutoSize = true, Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
            var dgvDetail = new DataGridView { Top = 70, Left = 8, Width = 1040, Height = 450, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            var lblSum = new Label { Text = "Tổng hợp:", Top = 535, Left = 8, AutoSize = true, Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
            var dgvSum = new DataGridView { Top = 555, Left = 8, Width = 600, Height = 60, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            // Nạp danh sách học kỳ khi tab được hiển thị lần đầu
            bool hocKyLoaded = false;
            page.Enter += (s, e) =>
            {
                if (hocKyLoaded) return;
                hocKyLoaded = true;
                try
                {
                    cmbHocKy.DisplayMember = "MaHocKy";
                    cmbHocKy.ValueMember   = "MaHocKy";
                    cmbHocKy.DataSource    = CommonDAL.LoadLookup("sp_LayDanhSachHocKy");
                }
                catch (Exception ex) { MessageBox.Show("Không tải được danh sách học kỳ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            };

            btnLoad.Click += (s, e) =>
            {
                if (cmbHocKy.SelectedValue == null) { MessageBox.Show("Vui lòng chọn học kỳ.", "Thông báo"); return; }
                try
                {
                    var results = BaoCaoDAL.BaoCaoHocPhi(cmbHocKy.SelectedValue.ToString());
                    if (results.Length > 0) dgvDetail.DataSource = results[0];
                    if (results.Length > 1) dgvSum.DataSource    = results[1];
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            page.Controls.AddRange(new Control[] { lblHK, cmbHocKy, btnLoad, lblDetail, dgvDetail, lblSum, dgvSum });
            return page;
        }

        // ──────────────────────────────────────────
        //  TAB 3: TÌM KIẾM SINH VIÊN NÂNG CAO
        // ──────────────────────────────────────────
        private TabPage BuildTimKiemNangCaoTab()
        {
            var page = new TabPage("Tìm kiếm SV nâng cao");

            var pnlFilter = new Panel { Top = 0, Left = 0, Width = 1080, Height = 160, BorderStyle = BorderStyle.FixedSingle };

            var txtHoTen   = CreateFilter(pnlFilter, "Họ tên SV:",    5,   8);
            var txtMaSV    = CreateFilter(pnlFilter, "Mã SV:",        5, 210);
            var txtMaKhoa  = CreateFilter(pnlFilter, "Mã khoa:",      5, 410);
            var txtMaNganh = CreateFilter(pnlFilter, "Mã ngành:",     5, 610);

            var lblLop = new Label { Text = "Mã lớp SH:", Top = 80, Left = 8, AutoSize = true };
            var txtMaLop = new TextBox { Top = 98, Left = 8, Width = 160 };
            pnlFilter.Controls.Add(lblLop); pnlFilter.Controls.Add(txtMaLop);

            var lblTT = new Label { Text = "Tình trạng:", Top = 80, Left = 210, AutoSize = true };
            var cmbTinhTrang = new ComboBox { Top = 98, Left = 210, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTinhTrang.Items.AddRange(new object[] { "(Tất cả)", "Đang học", "Nghỉ học", "Thôi học", "Tốt nghiệp" });
            cmbTinhTrang.SelectedIndex = 0;
            pnlFilter.Controls.Add(lblTT); pnlFilter.Controls.Add(cmbTinhTrang);

            var lblDiemTu = new Label { Text = "Điểm TB từ:", Top = 80, Left = 410, AutoSize = true };
            var txtDiemTu = new TextBox { Top = 98, Left = 410, Width = 80 };
            var lblDiemDen = new Label { Text = "đến:", Top = 80, Left = 510, AutoSize = true };
            var txtDiemDen = new TextBox { Top = 98, Left = 540, Width = 80 };
            pnlFilter.Controls.Add(lblDiemTu); pnlFilter.Controls.Add(txtDiemTu);
            pnlFilter.Controls.Add(lblDiemDen); pnlFilter.Controls.Add(txtDiemDen);

            var btnSearch = new Button { Text = "Tìm kiếm", Top = 90, Left = 700, Width = 110 };
            var btnClear  = new Button { Text = "Xóa lọc",  Top = 90, Left = 830, Width = 100 };
            pnlFilter.Controls.Add(btnSearch); pnlFilter.Controls.Add(btnClear);

            var dgvResult = new DataGridView { Top = 168, Left = 0, Width = 1080, Height = 490, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };

            btnSearch.Click += (s, e) =>
            {
                double? dTu = double.TryParse(txtDiemTu.Text, out var dt) ? dt : (double?)null;
                double? dDen = double.TryParse(txtDiemDen.Text, out var dd) ? dd : (double?)null;
                string tt = cmbTinhTrang.SelectedIndex == 0 ? null : cmbTinhTrang.SelectedItem.ToString();
                try
                {
                    dgvResult.DataSource = BaoCaoDAL.TimKiemSinhVienNangCao(
                        string.IsNullOrWhiteSpace(txtHoTen.Text) ? null : txtHoTen.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtMaSV.Text) ? null : txtMaSV.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtMaLop.Text) ? null : txtMaLop.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtMaKhoa.Text) ? null : txtMaKhoa.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtMaNganh.Text) ? null : txtMaNganh.Text.Trim(),
                        tt, dTu, dDen, null);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            btnClear.Click += (s, e) =>
            {
                txtHoTen.Text = txtMaSV.Text = txtMaKhoa.Text = txtMaNganh.Text = txtMaLop.Text = txtDiemTu.Text = txtDiemDen.Text = string.Empty;
                cmbTinhTrang.SelectedIndex = 0;
                dgvResult.DataSource = null;
            };

            page.Controls.Add(pnlFilter);
            page.Controls.Add(dgvResult);
            return page;
        }

        private TextBox CreateFilter(Control parent, string label, int top, int left)
        {
            parent.Controls.Add(new Label { Text = label, Top = top, Left = left, AutoSize = true });
            var tb = new TextBox { Top = top + 18, Left = left, Width = 160 };
            parent.Controls.Add(tb);
            return tb;
        }
    }
}
