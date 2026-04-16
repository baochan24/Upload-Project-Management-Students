using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class AuthBUS
    {
        public static OperationResult Login(string username, string password, out DataRow userInfo)
        {
            userInfo = null;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return OperationResult.Fail("Tên đăng nhập và mật khẩu không được để trống.");

            var passwordHash = HashHelper.ComputeSha256Hash(password).ToLowerInvariant();
            int resultCode;
            var table = AuthDAL.Login(username.Trim(), passwordHash, out resultCode);

            if (resultCode != 1)
                return OperationResult.Fail("Tên đăng nhập hoặc mật khẩu không đúng.");

            if (table.Rows.Count == 0)
                return OperationResult.Fail("Không tìm thấy thông tin người dùng.");

            userInfo = table.Rows[0];
            return OperationResult.Ok("Đăng nhập thành công.");
        }

        public static void SetUserSession(DataRow row)
        {
            if (row == null)
                return;

            UserSession.UserID = row.Field<int>("UserID");
            UserSession.Username = row.Field<string>("Username");
            UserSession.RoleName = row.Field<string>("RoleName");
            UserSession.MaNguoiDung = row.Field<string>("MaNguoiDung");
            UserSession.Email = row.Field<string>("Email");
        }
    }
}
