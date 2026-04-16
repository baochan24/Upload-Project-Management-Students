using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;

namespace QuanLySinhVien.Forms
{
    public class PhongHocForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtMaPhong;
        private readonly TextBox txtTenPhong;
        private readonly TextBox txtSucChua;
        private readonly TextBox txtViTri;
        private readonly TextBox txtSearch;

        public PhongHocForm()
        {
            Text = "Quản lý Phòng học";
            Width = 950;
            Height = 660;

            dgv = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 360,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 260, Padding = new Padding(10) };

            txtMaPhong  = CreateLabeledTextBox(panel, "Mã phòng",  10);
            txtTenPhong = CreateLabeledTextBox(panel, "Tên phòng", 80);
            txtSucChua  = CreateLabeledTextBox(panel, "Sức chứa", 150);
            txtViTri    = CreateLabeledTextBox(panel, "Vị trí",   220);

            var lblSearch = new Label { Text = "Tìm kiếm", Top = 10, Left = 500, AutoSize = true };
            txtSearch = new TextBox { Top = 30, Left = 500, Width = 220 };
            panel.Controls.Add(lblSearch);
            panel.Controls.Add(txtSearch);

            var btnSearch  = new Button { Text = "Tìm kiếm", Top = 60,  Left = 500, Width = 100 };
            var btnAdd     = new Button { Text = "Thêm",     Top = 100, Left = 500, Width = 100 };
            var btnUpdate  = new Button { Text = "Cập nhật", Top = 140, Left = 500, Width = 100 };
            var btnDelete  = new Button { Text = "Xóa",      Top = 180, Left = 500, Width = 100 };
            var btnRefresh = new Button { Text = "Làm mới",  Top = 60,  Left = 620, Width = 100 };

            btnSearch.Click  += (s, e) => LoadData(txtSearch.Text.Trim());
            btnAdd.Click     += BtnAdd_Click;
            btnUpdate.Click  += BtnUpdate_Click;
            btnDelete.Click  += BtnDelete_Click;
            btnRefresh.Click += (s, e) => { ClearForm(); LoadData(); };

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh });
            Controls.Add(panel);
            Controls.Add(dgv);

            Load += (s, e) => LoadData();
        }

        private TextBox CreateLabeledTextBox(Control parent, string label, int top)
        {
            parent.Controls.Add(new Label { Text = label, Top = top, Left = 10, AutoSize = true });
            var tb = new TextBox { Top = top + 20, Left = 10, Width = 250 };
            parent.Controls.Add(tb);
            return tb;
        }

        private void LoadData(string keyword = null)
        {
            try { dgv.DataSource = PhongHocBUS.LoadAll(keyword); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow?.DataBoundItem is DataRowView row)
            {
                txtMaPhong.Text  = row["MaPhong"]?.ToString();
                txtTenPhong.Text = row["TenPhong"]?.ToString();
                txtSucChua.Text  = row["SucChua"]?.ToString();
                txtViTri.Text    = row["ViTri"]?.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var r = PhongHocBUS.Add(txtMaPhong.Text, txtTenPhong.Text, txtSucChua.Text, txtViTri.Text);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var r = PhongHocBUS.Update(txtMaPhong.Text, txtTenPhong.Text, txtSucChua.Text, txtViTri.Text);
            Show(r); if (r.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa phòng học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var r = PhongHocBUS.Delete(txtMaPhong.Text);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void ClearForm() { txtMaPhong.Text = txtTenPhong.Text = txtSucChua.Text = txtViTri.Text = string.Empty; }
        private void Show(Models.OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
