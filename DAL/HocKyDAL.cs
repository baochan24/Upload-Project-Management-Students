using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;
    

namespace QuanLySinhVien.DAL
{
    public static class HocKyDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var parameter = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachHocKy", parameter);
        }

        public static OperationResult Add(string maHocKy, string tenHocKy, string namHoc, object ngayBatDau, object ngayKetThuc, int soTinToiDa)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaHocKy", SqlDbType.VarChar, 10) { Value = maHocKy },
                new SqlParameter("@TenHocKy", SqlDbType.NVarChar, 50) { Value = tenHocKy },
                new SqlParameter("@NamHoc", SqlDbType.VarChar, 9) { Value = namHoc },
                new SqlParameter("@NgayBatDau", SqlDbType.Date) { Value = (object)ngayBatDau ?? DBNull.Value },
                new SqlParameter("@NgayKetThuc", SqlDbType.Date) { Value = (object)ngayKetThuc ?? DBNull.Value },
                new SqlParameter("@SoTinToiDa", SqlDbType.Int) { Value = soTinToiDa },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemHocKy", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Update(string maHocKy, string tenHocKy, string namHoc, object ngayBatDau, object ngayKetThuc, int soTinToiDa)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaHocKy", SqlDbType.VarChar, 10) { Value = maHocKy },
                new SqlParameter("@TenHocKy", SqlDbType.NVarChar, 50) { Value = tenHocKy },
                new SqlParameter("@NamHoc", SqlDbType.VarChar, 9) { Value = namHoc },
                new SqlParameter("@NgayBatDau", SqlDbType.Date) { Value = (object)ngayBatDau ?? DBNull.Value },
                new SqlParameter("@NgayKetThuc", SqlDbType.Date) { Value = (object)ngayKetThuc ?? DBNull.Value },
                new SqlParameter("@SoTinToiDa", SqlDbType.Int) { Value = soTinToiDa },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_SuaHocKy", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Delete(string maHocKy)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaHocKy", SqlDbType.VarChar, 10) { Value = maHocKy },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaHocKy", parameters);
            return CreateResult(resultParameter);
        }

        private static OperationResult CreateResult(SqlParameter resultParameter)
        {
            var code = resultParameter.Value == DBNull.Value ? 0 : (int)resultParameter.Value;
            return code switch
            {
                1 => OperationResult.Ok(),
                -1 => OperationResult.Fail("Mã học kỳ đã tồn tại hoặc không tồn tại."),
                -2 => OperationResult.Fail("Thông tin học kỳ không hợp lệ."),
                -3 => OperationResult.Fail("Học kỳ không tồn tại."),
                _ => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
