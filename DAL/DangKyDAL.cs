using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class DangKyDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var parameter = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachDangKy", parameter);
        }

        public static OperationResult Register(string maSV, string maLHP)
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery("sp_DangKyHocPhan",
                    new SqlParameter("@MaSV", SqlDbType.VarChar, 10) { Value = maSV },
                    new SqlParameter("@MaLHP", SqlDbType.VarChar, 20) { Value = maLHP });
                return OperationResult.Ok();
            }
            catch (SqlException ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public static OperationResult Cancel(string maSV, string maLHP)
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery("sp_HuyDangKy",
                    new SqlParameter("@MaSV", SqlDbType.VarChar, 10) { Value = maSV },
                    new SqlParameter("@MaLHP", SqlDbType.VarChar, 20) { Value = maLHP });
                return OperationResult.Ok();
            }
            catch (SqlException ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
    }
}
