using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;

namespace QuanLySinhVien.Forms
{
    public class QuanTriForm : Form
    {
        private readonly DataGridView dgv;
        private readonly TextBox txtUsername;
        private readonly TextBox txtPassword;
        private readonly TextBox txtMaNguoiDung;
        private readonly TextBox txtEmail;
        private readonly ComboBox cmbRole;
        private readonly CheckBox chkStatus;
        private readonly TextBox txtSearch;
        private int _selectedUserID = 0;

        public QuanTriForm()
        {
            Text = "Quản trị Người dùng";
            Width = 1000;
            Height = 680;

            dgv = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 360,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var panel = new Panel { Dock = DockStyle.Top, Height = 280, Padding = new Padding(10) };

            // Cột trái
            txtUsername     = CreateLabeledTextBox(panel, "Tên đăng nhập",  10, 10);
            txtPassword     = CreateLabeledTextBox(panel, "Mật khẩu (mới)", 80, 10);
            txtPassword.PasswordChar = '*';
            txtMaNguoiDung  = CreateLabeledTextBox(panel, "Mã người dùng", 150, 10);

            // Cột giữa
            txtEmail = CreateLabeledTextBox(panel, "Email", 10, 290);

            panel.Controls.Add(new Label { Text = "Vai trò", Top = 80, Left = 290, AutoSize = true });
            cmbRole = new ComboBox { Top = 100, Left = 290, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbRole);

            chkStatus = new CheckBox { Text = "Đang hoạt động", Top = 145, Left = 290, AutoSize = true, Checked = true };
            panel.Controls.Add(chkStatus);

            // Cột phải - tìm kiếm & nút
            var lblSearch = new Label { Text = "Tìm kiếm", Top = 10, Left = 620, AutoSize = true };
            txtSearch = new TextBox { Top = 30, Left = 620, Width = 220 };
            panel.Controls.Add(lblSearch);
            panel.Controls.Add(txtSearch);

            var btnSearch  = new Button { Text = "Tìm kiếm",    Top = 60,  Left = 620, Width = 110 };
            var btnAdd     = new Button { Text = "Thêm mới",    Top = 100, Left = 620, Width = 110 };
            var btnUpdate  = new Button { Text = "Cập nhật",    Top = 140, Left = 620, Width = 110 };
            var btnDelete  = new Button { Text = "Khóa tài khoản", Top = 180, Left = 620, Width = 110 };
            var btnRefresh = new Button { Text = "Làm mới",     Top = 60,  Left = 750, Width = 110 };

            btnSearch.Click  += (s, e) => LoadData(txtSearch.Text.Trim());
            btnAdd.Click     += BtnAdd_Click;
            btnUpdate.Click  += BtnUpdate_Click;
            btnDelete.Click  += BtnDelete_Click;
            btnRefresh.Click += (s, e) => { ClearForm(); LoadData(); };

            panel.Controls.AddRange(new Control[] { btnSearch, btnAdd, btnUpdate, btnDelete, btnRefresh });
            Controls.Add(panel);
            Controls.Add(dgv);

            Load += QuanTriForm_Load;
        }

        private TextBox CreateLabeledTextBox(Control parent, string label, int top, int left)
        {
            parent.Controls.Add(new Label { Text = label, Top = top, Left = left, AutoSize = true });
            var tb = new TextBox { Top = top + 20, Left = left, Width = 220 };
            parent.Controls.Add(tb);
            return tb;
        }

        private void QuanTriForm_Load(object sender, EventArgs e)
        {
            cmbRole.DisplayMember = "RoleName";
            cmbRole.ValueMember   = "RoleID";
            cmbRole.DataSource    = QuanTriBUS.LoadRoles();
            LoadData();
        }

        private void LoadData(string keyword = null)
        {
            try
            {
                dgv.DataSource = QuanTriBUS.LoadAllUsers(keyword);
                _selectedUserID = 0;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow?.DataBoundItem is DataRowView row)
            {
                _selectedUserID     = Convert.ToInt32(row["UserID"]);
                txtUsername.Text    = row["Username"]?.ToString();
                txtMaNguoiDung.Text = row["MaNguoiDung"]?.ToString();
                txtEmail.Text       = row["Email"]?.ToString();
                txtPassword.Text    = string.Empty;
                var roleID = row["RoleName"] != DBNull.Value
                    ? GetRoleIDByName(row["RoleName"].ToString())
                    : null;
                if (roleID != null) cmbRole.SelectedValue = roleID;
                else cmbRole.SelectedIndex = -1;
                chkStatus.Checked   = row["Status"] != DBNull.Value && (bool)row["Status"];
            }
        }

        private object GetRoleIDByName(string roleName)
        {
            if (cmbRole.DataSource is DataTable dt)
                foreach (DataRow r in dt.Rows)
                    if (r["RoleName"].ToString() == roleName)
                        return r["RoleID"];
            return null;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbRole.SelectedValue == null) { MessageBox.Show("Vui lòng chọn vai trò.", "Lỗi"); return; }
            var r = QuanTriBUS.AddUser(txtUsername.Text, txtPassword.Text,
                Convert.ToInt32(cmbRole.SelectedValue), txtMaNguoiDung.Text, txtEmail.Text);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedUserID == 0) { MessageBox.Show("Vui lòng chọn người dùng cần cập nhật.", "Thông báo"); return; }
            int? roleID = cmbRole.SelectedValue != null ? Convert.ToInt32(cmbRole.SelectedValue) : (int?)null;
            var r = QuanTriBUS.UpdateUser(_selectedUserID, txtEmail.Text, txtMaNguoiDung.Text, roleID, chkStatus.Checked);
            Show(r); if (r.Success) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedUserID == 0) { MessageBox.Show("Vui lòng chọn người dùng cần khóa.", "Thông báo"); return; }
            if (MessageBox.Show("Khóa tài khoản này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var r = QuanTriBUS.DeleteUser(_selectedUserID);
            Show(r); if (r.Success) { ClearForm(); LoadData(); }
        }

        private void ClearForm()
        {
            txtUsername.Text = txtPassword.Text = txtMaNguoiDung.Text = txtEmail.Text = string.Empty;
            chkStatus.Checked = true;
            _selectedUserID = 0;
        }

        private void Show(Models.OperationResult r) =>
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
}
