using System.Data;
using System.Data.SqlClient;

namespace QuanLySinhVien.DAL
{
    public static class AuthDAL
    {
        public static DataTable Login(string username, string passwordHash, out int resultCode)
        {
            var parameters = new[]
            {
                new SqlParameter("@Username", SqlDbType.VarChar, 50) { Value = username },
                new SqlParameter("@PasswordHash", SqlDbType.VarChar, 255) { Value = passwordHash },
                new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            var table = DatabaseHelper.ExecuteDataTable("sp_Login", parameters);
            resultCode = (int)parameters[2].Value;
            return table;
        }
    }
}
