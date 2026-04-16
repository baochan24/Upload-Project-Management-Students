using System.Windows.Forms;

namespace QuanLySinhVien.Forms
{
    public class DangKyForm : Form
    {
        public DangKyForm()
        {
            Text = "Quản lý đăng ký";
            Width = 980;
            Height = 700;

            var label = new Label
            {
                Text = "Giao diện quản lý đăng ký đang được phát triển.",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            Controls.Add(label);
        }
    }
}