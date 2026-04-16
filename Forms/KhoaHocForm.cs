using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;

namespace QuanLySinhVien.Forms
{
    public class KhoaHocForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtMaKhoaHoc;
        private readonly TextBox txtTenKhoaHoc;
        private readonly TextBox txtSearch;

        public KhoaHocForm()
        {
            Text = "Quản lý Khóa học";
            Width = 900;
            Height = 600;

            dgv = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 340,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 220, Padding = new Padding(10) };

            txtMaKhoaHoc  = CreateLabeledTextBox(panel, "Mã khóa học",  10);
            txtTenKhoaHoc = CreateLabeledTextBox(panel, "Tên khóa học", 80);

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
            try { dgv.DataSource = KhoaHocBUS.LoadAll(keyword); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow?.DataBoundItem is DataRowView row)
            {
                txtMaKhoaHoc.Text  = row["MaKhoaHoc"]?.ToString();
                txtTenKhoaHoc.Text = row["TenKhoaHoc"]?.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var r = KhoaHocBUS.Add(txtMaKhoaHoc.Text, txtTenKhoaHoc.Text);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var r = KhoaHocBUS.Update(txtMaKhoaHoc.Text, txtTenKhoaHoc.Text);
            Show(r); if (r.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa khóa học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var r = KhoaHocBUS.Delete(txtMaKhoaHoc.Text);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void ClearForm() { txtMaKhoaHoc.Text = txtTenKhoaHoc.Text = string.Empty; }
        private void Show(Models.OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
