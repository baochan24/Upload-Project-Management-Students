using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.Forms
{
    public class LopSinhHoatForm : Form
    {
        private readonly DataGridView dgvLopSH;
        private readonly TextBox txtMaLopSH;
        private readonly TextBox txtTenLop;
        private readonly ComboBox cmbMaNganh;
        private readonly ComboBox cmbMaKhoaHoc;
        private readonly ComboBox cmbMaGVCN;
        private readonly TextBox txtSearch;

        public LopSinhHoatForm()
        {
            Text = "Quản lý lớp sinh hoạt";
            Width = 980;
            Height = 700;

            dgvLopSH = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 320,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvLopSH.SelectionChanged += DgvLopSH_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 300, Padding = new Padding(10) };

            txtMaLopSH = CreateLabeledTextBox(panel, "Mã lớp SH", 10);
            txtTenLop = CreateLabeledTextBox(panel, "Tên lớp", 80);

            var lblMaNganh = new Label { Text = "Mã ngành", Top = 150, Left = 10, AutoSize = true };
            cmbMaNganh = new ComboBox { Top = 170, Left = 10, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(lblMaNganh);
            panel.Controls.Add(cmbMaNganh);

            var lblMaKhoaHoc = new Label { Text = "Mã khóa học", Top = 150, Left = 260, AutoSize = true };
            cmbMaKhoaHoc = new ComboBox { Top = 170, Left = 260, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(lblMaKhoaHoc);
            panel.Controls.Add(cmbMaKhoaHoc);

            var lblMaGVCN = new Label { Text = "Mã GVCN", Top = 210, Left = 10, AutoSize = true };
            cmbMaGVCN = new ComboBox { Top = 230, Left = 10, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(lblMaGVCN);
            panel.Controls.Add(cmbMaGVCN);

            var lblSearch = new Label { Text = "Tìm kiếm", Top = 10, Left = 500, AutoSize = true };
            txtSearch = new TextBox { Top = 30, Left = 500, Width = 220 };
            panel.Controls.Add(lblSearch);
            panel.Controls.Add(txtSearch);

            var btnSearch = new Button { Text = "Tìm kiếm", Top = 70, Left = 500, Width = 100 };
            btnSearch.Click += (sender, args) => LoadData(txtSearch.Text.Trim());

            var btnAdd = new Button { Text = "Thêm", Top = 120, Left = 500, Width = 100 };
            btnAdd.Click += BtnAdd_Click;

            var btnUpdate = new Button { Text = "Cập nhật", Top = 170, Left = 500, Width = 100 };
            btnUpdate.Click += BtnUpdate_Click;

            var btnDelete = new Button { Text = "Xóa", Top = 220, Left = 500, Width = 100 };
            btnDelete.Click += BtnDelete_Click;

            var btnRefresh = new Button { Text = "Làm mới", Top = 270, Left = 500, Width = 100 };
            btnRefresh.Click += (sender, args) => LoadData();

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh });
            Controls.Add(panel);
            Controls.Add(dgvLopSH);

            Load += LopSinhHoatForm_Load;
        }

        private void LopSinhHoatForm_Load(object sender, EventArgs e)
        {
            LoadLookups();
            LoadData();
        }

        private TextBox CreateLabeledTextBox(Control parent, string labelText, int top)
        {
            var label = new Label { Text = labelText, Top = top, Left = 10, AutoSize = true };
            var textBox = new TextBox { Top = top + 20, Left = 10, Width = 220 };
            parent.Controls.Add(label);
            parent.Controls.Add(textBox);
            return textBox;
        }

        private void LoadLookups()
        {
            cmbMaNganh.DisplayMember = "MaNganh";
            cmbMaNganh.ValueMember = "MaNganh";
            cmbMaNganh.DataSource = CommonDAL.LoadLookup("sp_LayDanhSachNganhHoc");

            cmbMaKhoaHoc.DisplayMember = "MaKhoaHoc";
            cmbMaKhoaHoc.ValueMember = "MaKhoaHoc";
            cmbMaKhoaHoc.DataSource = CommonDAL.LoadLookup("sp_LayDanhSachKhoaHoc");

            cmbMaGVCN.DisplayMember = "MaGV";
            cmbMaGVCN.ValueMember = "MaGV";
            cmbMaGVCN.DataSource = CommonDAL.LoadLookup("sp_LayDanhSachGiangVien");
        }

        private void LoadData(string keyword = null)
        {
            try
            {
                dgvLopSH.DataSource = LopSinhHoatBUS.LoadAll(keyword);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvLopSH_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLopSH.CurrentRow == null) return;
            var row = dgvLopSH.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return;

            txtMaLopSH.Text = row["MaLopSH"]?.ToString();
            txtTenLop.Text = row["TenLop"]?.ToString();
            cmbMaNganh.SelectedValue = row["MaNganh"]?.ToString();
            cmbMaKhoaHoc.SelectedValue = row["MaKhoaHoc"]?.ToString();
            cmbMaGVCN.SelectedValue = row["MaGVCN"]?.ToString();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var result = LopSinhHoatBUS.Add(txtMaLopSH.Text, txtTenLop.Text, cmbMaNganh.SelectedValue?.ToString(), cmbMaKhoaHoc.SelectedValue?.ToString(), cmbMaGVCN.SelectedValue?.ToString());
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (result.Success) LoadData();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var result = LopSinhHoatBUS.Update(txtMaLopSH.Text, txtTenLop.Text, cmbMaNganh.SelectedValue?.ToString(), cmbMaKhoaHoc.SelectedValue?.ToString(), cmbMaGVCN.SelectedValue?.ToString());
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (result.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa lớp sinh hoạt này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var result = LopSinhHoatBUS.Delete(txtMaLopSH.Text);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (result.Success) LoadData();
        }
    }
}
