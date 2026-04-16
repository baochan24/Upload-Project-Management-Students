using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class LopHocPhanDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var parameter = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachLopHocPhan", parameter);
        }

        public static OperationResult Add(string maLHP, string maLopHienThi, string maMon, string maGV, string maHocKy, string maPhong, int siSoToiDa, byte? thu, byte? tietBatDau, byte? tietKetThuc)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaLHP", SqlDbType.VarChar, 20) { Value = maLHP },
                new SqlParameter("@MaLopHienThi", SqlDbType.NVarChar, 20) { Value = maLopHienThi },
                new SqlParameter("@MaMon", SqlDbType.VarChar, 10) { Value = maMon },
                new SqlParameter("@MaGV", SqlDbType.VarChar, 10) { Value = maGV },
                new SqlParameter("@MaHocKy", SqlDbType.VarChar, 10) { Value = maHocKy },
                new SqlParameter("@MaPhong", SqlDbType.VarChar, 10) { Value = maPhong },
                new SqlParameter("@SiSoToiDa", SqlDbType.Int) { Value = siSoToiDa },
                new SqlParameter("@Thu", SqlDbType.TinyInt) { Value = (object)thu ?? DBNull.Value },
                new SqlParameter("@TietBatDau", SqlDbType.TinyInt) { Value = (object)tietBatDau ?? DBNull.Value },
                new SqlParameter("@TietKetThuc", SqlDbType.TinyInt) { Value = (object)tietKetThuc ?? DBNull.Value },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_MoLopHocPhan", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Update(string maLHP, string maLopHienThi, string maGV, string maPhong, int siSoToiDa, byte? thu, byte? tietBatDau, byte? tietKetThuc, string trangThai)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaLHP", SqlDbType.VarChar, 20) { Value = maLHP },
                new SqlParameter("@MaLopHienThi", SqlDbType.NVarChar, 20) { Value = maLopHienThi },
                new SqlParameter("@MaGV", SqlDbType.VarChar, 10) { Value = maGV },
                new SqlParameter("@MaPhong", SqlDbType.VarChar, 10) { Value = maPhong },
                new SqlParameter("@SiSoToiDa", SqlDbType.Int) { Value = siSoToiDa },
                new SqlParameter("@Thu", SqlDbType.TinyInt) { Value = (object)thu ?? DBNull.Value },
                new SqlParameter("@TietBatDau", SqlDbType.TinyInt) { Value = (object)tietBatDau ?? DBNull.Value },
                new SqlParameter("@TietKetThuc", SqlDbType.TinyInt) { Value = (object)tietKetThuc ?? DBNull.Value },
                new SqlParameter("@TrangThai", SqlDbType.NVarChar, 20) { Value = (object)trangThai ?? DBNull.Value },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_SuaLopHocPhan", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Delete(string maLHP)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaLHP", SqlDbType.VarChar, 20) { Value = maLHP },
                resultParameter
            };
            DatabaseHelper.ExecuteNonQuery("sp_XoaLopHocPhan", parameters);
            return CreateResult(resultParameter);
        }

        private static OperationResult CreateResult(SqlParameter resultParameter)
        {
            var code = resultParameter.Value == DBNull.Value ? 0 : (int)resultParameter.Value;
            return code switch
            {
                1 => OperationResult.Ok(),
                -1 => OperationResult.Fail("Lớp học phần không tồn tại hoặc mã đã trùng."),
                -2 => OperationResult.Fail("Không thể giảm sĩ số nhỏ hơn số hiện tại."),
                -3 => OperationResult.Fail("Giảng viên không tồn tại."),
                -4 => OperationResult.Fail("Phòng học không tồn tại."),
                -6 => OperationResult.Fail("Sĩ số tối đa phải lớn hơn 0."),
                -7 => OperationResult.Fail("Trùng lịch phòng học."),
                _ => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
