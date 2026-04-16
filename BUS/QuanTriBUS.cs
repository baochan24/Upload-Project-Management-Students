using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class QuanTriBUS
    {
        public static DataTable LoadAllUsers(string keyword = null) => QuanTriDAL.LoadAllUsers(keyword);
        public static DataTable LoadRoles() => QuanTriDAL.LoadRoles();

        public static OperationResult AddUser(string username, string password, int roleID, string maNguoiDung, string email)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return OperationResult.Fail("Tên đăng nhập và mật khẩu là bắt buộc.");
            if (password.Length < 6)
                return OperationResult.Fail("Mật khẩu phải có ít nhất 6 ký tự.");
            return QuanTriDAL.AddUser(username.Trim(), password,
                roleID,
                string.IsNullOrWhiteSpace(maNguoiDung) ? null : maNguoiDung.Trim(),
                string.IsNullOrWhiteSpace(email) ? null : email.Trim());
        }

        public static OperationResult UpdateUser(int userID, string email, string maNguoiDung, int? roleID, bool? status)
        {
            if (userID <= 0)
                return OperationResult.Fail("Người dùng không hợp lệ.");
            return QuanTriDAL.UpdateUser(userID,
                string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                string.IsNullOrWhiteSpace(maNguoiDung) ? null : maNguoiDung.Trim(),
                roleID, status);
        }

        public static OperationResult DeleteUser(int userID)
        {
            if (userID <= 0)
                return OperationResult.Fail("Người dùng không hợp lệ.");
            return QuanTriDAL.DeleteUser(userID);
        }
    }
}
