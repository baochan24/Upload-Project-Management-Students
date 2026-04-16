using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;

namespace QuanLySinhVien.Forms
{
    public class LoginForm : Form
    {
        private readonly TextBox txtUsername;
        private readonly TextBox txtPassword;
        private readonly Button btnLogin;
        private readonly Button btnExit;

        public LoginForm()
        {
            Text = "Đăng nhập hệ thống";
            Width = 420;
            Height = 260;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            var lblUsername = new Label { Text = "Tên đăng nhập", Top = 30, Left = 30, Width = 100 };
            txtUsername = new TextBox { Top = 50, Left = 30, Width = 320 };
            var lblPassword = new Label { Text = "Mật khẩu", Top = 90, Left = 30, Width = 100 };
            txtPassword = new TextBox { Top = 110, Left = 30, Width = 320, UseSystemPasswordChar = true };
            btnLogin = new Button { Text = "Đăng nhập", Top = 160, Left = 30, Width = 150 };
            btnExit = new Button { Text = "Thoát", Top = 160, Left = 200, Width = 150 };

            btnLogin.Click += BtnLogin_Click;
            btnExit.Click += (sender, args) => Close();

            Controls.AddRange(new Control[] { lblUsername, txtUsername, lblPassword, txtPassword, btnLogin, btnExit });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var result = AuthBUS.Login(txtUsername.Text.Trim(), txtPassword.Text, out DataRow userInfo);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AuthBUS.SetUserSession(userInfo);
                Hide();
                using (var mainForm = new MainForm())
                {
                    mainForm.ShowDialog();
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
