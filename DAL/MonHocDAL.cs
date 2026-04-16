using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class MonHocDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var parameter = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachMonHoc", parameter);
        }

        public static OperationResult Add(string maMon, string tenMon, int soTinChi, string monTienQuyet)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaMon", SqlDbType.VarChar, 10) { Value = maMon },
                new SqlParameter("@TenMon", SqlDbType.NVarChar, 100) { Value = tenMon },
                new SqlParameter("@SoTinChi", SqlDbType.Int) { Value = soTinChi },
                new SqlParameter("@MonTienQuyet", SqlDbType.VarChar, 10) { Value = (object)monTienQuyet ?? DBNull.Value },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemMonHoc", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Update(string maMon, string tenMon, int soTinChi, string monTienQuyet)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaMon", SqlDbType.VarChar, 10) { Value = maMon },
                new SqlParameter("@TenMon", SqlDbType.NVarChar, 100) { Value = tenMon },
                new SqlParameter("@SoTinChi", SqlDbType.Int) { Value = soTinChi },
                new SqlParameter("@MonTienQuyet", SqlDbType.VarChar, 10) { Value = (object)monTienQuyet ?? DBNull.Value },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_SuaMonHoc", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Delete(string maMon)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaMon", SqlDbType.VarChar, 10) { Value = maMon },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaMonHoc", parameters);
            return CreateResult(resultParameter);
        }

        private static OperationResult CreateResult(SqlParameter resultParameter)
        {
            var code = resultParameter.Value == DBNull.Value ? 0 : (int)resultParameter.Value;
            return code switch
            {
                1 => OperationResult.Ok(),
                -1 => OperationResult.Fail("Mã môn học đã tồn tại hoặc không tồn tại."),
                -2 => OperationResult.Fail("Thông tin không hợp lệ."),
                -3 => OperationResult.Fail("Môn tiên quyết không tồn tại."),
                _ => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
