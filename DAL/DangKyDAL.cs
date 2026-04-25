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

        /// <summary>Lấy danh sách đăng ký (lịch học) của một sinh viên cụ thể.</summary>
        public static DataTable LoadBySinhVien(string maSV)
        {
            // Thử gọi SP phiên bản mới (có @MaSV – cần chạy DataSetFinal.sql phần "I.4")
            try
            {
                return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachDangKy",
                    new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = DBNull.Value },
                    new SqlParameter("@MaSV",    SqlDbType.VarChar,  10)  { Value = maSV });
            }
            catch (SqlException ex) when (ex.Message.Contains("@MaSV"))
            {
                // Fallback: SP chưa được cập nhật → gọi không có @MaSV, filter trong bộ nhớ
                var dt = DatabaseHelper.ExecuteDataTable("sp_LayDanhSachDangKy",
                    new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = DBNull.Value });
                var result = dt.Clone();
                foreach (DataRow row in dt.Rows)
                    if (string.Equals(row["MaSV"]?.ToString(), maSV, StringComparison.OrdinalIgnoreCase))
                        result.ImportRow(row);
                return result;
            }
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
