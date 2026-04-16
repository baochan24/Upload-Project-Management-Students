using System;
using System.Windows.Forms;
using QuanLySinhVien.Forms;

namespace QuanLySinhVien
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
