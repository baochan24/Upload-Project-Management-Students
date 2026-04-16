using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class QuanTriDAL
    {
        public static DataTable LoadAllUsers(string keyword = null)
        {
            var p = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachNguoiDung", p);
        }

        public static OperationResult AddUser(string username, string password, int roleID, string maNguoiDung, string email)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_ThemNguoiDung", new[]
            {
                new SqlParameter("@Username",    SqlDbType.VarChar,   50) { Value = username },
                new SqlParameter("@Password",    SqlDbType.VarChar,  250) { Value = password },
                new SqlParameter("@RoleID",      SqlDbType.Int)           { Value = roleID },
                new SqlParameter("@MaNguoiDung", SqlDbType.VarChar,   20) { Value = (object)maNguoiDung ?? DBNull.Value },
                new SqlParameter("@Email",       SqlDbType.VarChar,  100) { Value = (object)email ?? DBNull.Value },
                rp
            });
            return CreateUserResult(rp);
        }

        public static OperationResult UpdateUser(int userID, string email, string maNguoiDung, int? roleID, bool? status)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_CapNhatNguoiDung", new[]
            {
                new SqlParameter("@UserID",      SqlDbType.Int)           { Value = userID },
                new SqlParameter("@Email",       SqlDbType.VarChar,  100) { Value = (object)email ?? DBNull.Value },
                new SqlParameter("@MaNguoiDung", SqlDbType.VarChar,  100) { Value = (object)maNguoiDung ?? DBNull.Value },
                new SqlParameter("@RoleID",      SqlDbType.Int)           { Value = (object)roleID ?? DBNull.Value },
                new SqlParameter("@Status",      SqlDbType.Bit)           { Value = (object)status ?? DBNull.Value },
                rp
            });
            return CreateUserResult(rp);
        }

        public static OperationResult DeleteUser(int userID)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_XoaNguoiDung", new[]
            {
                new SqlParameter("@UserID", SqlDbType.Int) { Value = userID },
                rp
            });
            return CreateUserResult(rp);
        }

        public static DataTable LoadRoles()
        {
            return DatabaseHelper.ExecuteTextDataTable("SELECT RoleID, RoleName FROM Roles ORDER BY RoleID");
        }

        private static OperationResult CreateUserResult(SqlParameter rp)
        {
            var code = rp.Value == DBNull.Value ? 0 : (int)rp.Value;
            return code switch
            {
                1  => OperationResult.Ok(),
                -1 => OperationResult.Fail("Username đã tồn tại hoặc người dùng không tồn tại."),
                -2 => OperationResult.Fail("Không thể khóa Admin cuối cùng hoặc RoleID không hợp lệ."),
                _  => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
