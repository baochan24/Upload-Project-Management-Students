using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.Forms
{
    public class GiangVienForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtMaGV;
        private readonly TextBox txtHoTen;
        private readonly ComboBox cmbMaKhoa;
        private readonly TextBox txtEmail;
        private readonly TextBox txtSoDienThoai;
        private readonly DateTimePicker dtpNgaySinh;
        private readonly CheckBox chkCoNgaySinh;
        private readonly ComboBox cmbGioiTinh;
        private readonly TextBox txtHocVi;
        private readonly TextBox txtHocHam;
        private readonly TextBox txtSearch;

        public GiangVienForm()
        {
            Text = "Quản lý Giảng viên";
            Width = 1050;
            Height = 720;

            dgv = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 340,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 340, Padding = new Padding(10) };

            // Cột trái
            txtMaGV   = CreateLabeledTextBox(panel, "Mã giảng viên", 10, 10);
            txtHoTen  = CreateLabeledTextBox(panel, "Họ tên",        80, 10);

            panel.Controls.Add(new Label { Text = "Khoa", Top = 150, Left = 10, AutoSize = true });
            cmbMaKhoa = new ComboBox { Top = 170, Left = 10, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbMaKhoa);

            panel.Controls.Add(new Label { Text = "Giới tính", Top = 210, Left = 10, AutoSize = true });
            cmbGioiTinh = new ComboBox { Top = 230, Left = 10, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGioiTinh.Items.AddRange(new object[] { "(Chưa cập nhật)", "Nam", "Nữ" });
            cmbGioiTinh.SelectedIndex = 0;
            panel.Controls.Add(cmbGioiTinh);

            panel.Controls.Add(new Label { Text = "Ngày sinh", Top = 275, Left = 10, AutoSize = true });
            chkCoNgaySinh = new CheckBox { Text = "Có ngày sinh", Top = 278, Left = 100, AutoSize = true };
            dtpNgaySinh   = new DateTimePicker { Top = 295, Left = 10, Width = 220, Format = DateTimePickerFormat.Short, Enabled = false };
            chkCoNgaySinh.CheckedChanged += (s, e) => dtpNgaySinh.Enabled = chkCoNgaySinh.Checked;
            panel.Controls.Add(chkCoNgaySinh);
            panel.Controls.Add(dtpNgaySinh);

            // Cột giữa
            txtEmail        = CreateLabeledTextBox(panel, "Email",       10, 280);
            txtSoDienThoai  = CreateLabeledTextBox(panel, "Số điện thoại", 80, 280);
            txtHocVi        = CreateLabeledTextBox(panel, "Học vị",     150, 280);
            txtHocHam       = CreateLabeledTextBox(panel, "Học hàm",    220, 280);

            // Cột phải - tìm kiếm + nút
            var lblSearch = new Label { Text = "Tìm kiếm", Top = 10, Left = 620, AutoSize = true };
            txtSearch = new TextBox { Top = 30, Left = 620, Width = 220 };
            panel.Controls.Add(lblSearch);
            panel.Controls.Add(txtSearch);

            var btnSearch  = new Button { Text = "Tìm kiếm", Top = 60,  Left = 620, Width = 100 };
            var btnAdd     = new Button { Text = "Thêm",     Top = 100, Left = 620, Width = 100 };
            var btnUpdate  = new Button { Text = "Cập nhật", Top = 140, Left = 620, Width = 100 };
            var btnDelete  = new Button { Text = "Xóa",      Top = 180, Left = 620, Width = 100 };
            var btnRefresh = new Button { Text = "Làm mới",  Top = 60,  Left = 740, Width = 100 };

            btnSearch.Click  += (s, e) => LoadData(txtSearch.Text.Trim());
            btnAdd.Click     += BtnAdd_Click;
            btnUpdate.Click  += BtnUpdate_Click;
            btnDelete.Click  += BtnDelete_Click;
            btnRefresh.Click += (s, e) => { ClearForm(); LoadData(); };

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh });
            Controls.Add(panel);
            Controls.Add(dgv);

            Load += GiangVienForm_Load;
        }

        private TextBox CreateLabeledTextBox(Control parent, string label, int top, int left)
        {
            parent.Controls.Add(new Label { Text = label, Top = top, Left = left, AutoSize = true });
            var tb = new TextBox { Top = top + 20, Left = left, Width = 220 };
            parent.Controls.Add(tb);
            return tb;
        }

        private void GiangVienForm_Load(object sender, EventArgs e)
        {
            cmbMaKhoa.DisplayMember = "TenKhoa";
            cmbMaKhoa.ValueMember   = "MaKhoa";
            cmbMaKhoa.DataSource    = CommonDAL.LoadLookup("sp_LayDanhSachKhoa");
            LoadData();
        }

        private void LoadData(string keyword = null)
        {
            try { dgv.DataSource = GiangVienBUS.LoadAll(keyword); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow?.DataBoundItem is DataRowView row)
            {
                txtMaGV.Text            = row["MaGV"]?.ToString();
                txtHoTen.Text           = row["HoTen"]?.ToString();
                cmbMaKhoa.SelectedValue = row["MaKhoa"]?.ToString();
                txtEmail.Text           = row["Email"]  == DBNull.Value ? string.Empty : row["Email"]?.ToString();
                txtSoDienThoai.Text     = row["SoDienThoai"] == DBNull.Value ? string.Empty : row["SoDienThoai"]?.ToString();
                // HocVi / HocHam: SP trả về NULL thuần → textbox để rỗng khi chưa nhập
                txtHocVi.Text           = row["HocVi"]  == DBNull.Value ? string.Empty : row["HocVi"]?.ToString();
                txtHocHam.Text          = row["HocHam"] == DBNull.Value ? string.Empty : row["HocHam"]?.ToString();

                // GioiTinh: SP trả về cột BIT (true/false/NULL)
                var gt = row["GioiTinh"];
                if (gt == DBNull.Value || gt == null)
                    cmbGioiTinh.SelectedIndex = 0;
                else
                    cmbGioiTinh.SelectedIndex = Convert.ToBoolean(gt) ? 1 : 2;

                var ns = row["NgaySinh"];
                if (ns != DBNull.Value && ns != null)
                {
                    chkCoNgaySinh.Checked = true;
                    dtpNgaySinh.Value     = Convert.ToDateTime(ns);
                }
                else
                {
                    chkCoNgaySinh.Checked = false;
                }
            }
        }

        private (DateTime? ns, bool? gt) GetOptionalFields()
        {
            DateTime? ngaySinh = chkCoNgaySinh.Checked ? dtpNgaySinh.Value : (DateTime?)null;
            bool? gioiTinh = cmbGioiTinh.SelectedIndex == 0 ? (bool?)null :
                             cmbGioiTinh.SelectedIndex == 1;
            return (ngaySinh, gioiTinh);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var (ns, gt) = GetOptionalFields();
            var r = GiangVienBUS.Add(txtMaGV.Text, txtHoTen.Text, cmbMaKhoa.SelectedValue?.ToString(),
                txtEmail.Text, txtSoDienThoai.Text, ns, gt, txtHocVi.Text, txtHocHam.Text);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var (ns, gt) = GetOptionalFields();
            var r = GiangVienBUS.Update(txtMaGV.Text, txtHoTen.Text, cmbMaKhoa.SelectedValue?.ToString(),
                txtEmail.Text, txtSoDienThoai.Text, ns, gt, txtHocVi.Text, txtHocHam.Text);
            Show(r); if (r.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa giảng viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var r = GiangVienBUS.Delete(txtMaGV.Text);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void ClearForm()
        {
            txtMaGV.Text = txtHoTen.Text = txtEmail.Text = txtSoDienThoai.Text = txtHocVi.Text = txtHocHam.Text = string.Empty;
            cmbGioiTinh.SelectedIndex = 0;
            chkCoNgaySinh.Checked = false;
        }

        private void Show(Models.OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
