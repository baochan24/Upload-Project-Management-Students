using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class LopSinhHoatDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var parameter = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachLopSinhHoat", parameter);
        }

        public static OperationResult Add(string maLopSH, string tenLop, string maNganh, string maKhoaHoc, string maGVCN)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaLopSH", SqlDbType.VarChar, 15) { Value = maLopSH },
                new SqlParameter("@TenLop", SqlDbType.NVarChar, 50) { Value = tenLop },
                new SqlParameter("@MaNganh", SqlDbType.VarChar, 10) { Value = maNganh },
                new SqlParameter("@MaKhoaHoc", SqlDbType.VarChar, 10) { Value = maKhoaHoc },
                new SqlParameter("@MaGVCN", SqlDbType.VarChar, 10) { Value = (object)maGVCN ?? DBNull.Value },
                resultParameter
            };
            DatabaseHelper.ExecuteNonQuery("sp_ThemLopSinhHoat", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Update(string maLopSH, string tenLop, string maNganh, string maKhoaHoc, string maGVCN)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaLopSH", SqlDbType.VarChar, 15) { Value = maLopSH },
                new SqlParameter("@TenLop", SqlDbType.NVarChar, 50) { Value = tenLop },
                new SqlParameter("@MaNganh", SqlDbType.VarChar, 10) { Value = maNganh },
                new SqlParameter("@MaKhoaHoc", SqlDbType.VarChar, 10) { Value = maKhoaHoc },
                new SqlParameter("@MaGVCN", SqlDbType.VarChar, 10) { Value = (object)maGVCN ?? DBNull.Value },
                resultParameter
            };
            DatabaseHelper.ExecuteNonQuery("sp_SuaLopSinhHoat", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Delete(string maLopSH)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaLopSH", SqlDbType.VarChar, 15) { Value = maLopSH },
                resultParameter
            };
            DatabaseHelper.ExecuteNonQuery("sp_XoaLopSinhHoat", parameters);
            return CreateResult(resultParameter);
        }

        private static OperationResult CreateResult(SqlParameter resultParameter)
        {
            var code = resultParameter.Value == DBNull.Value ? 0 : (int)resultParameter.Value;
            return code switch
            {
                1 => OperationResult.Ok(),
                -1 => OperationResult.Fail("Mã lớp sinh hoạt đã tồn tại hoặc không tồn tại."),
                -2 => OperationResult.Fail("Vui lòng nhập đầy đủ thông tin bắt buộc."),
                -3 => OperationResult.Fail("Ngành học không tồn tại."),
                -4 => OperationResult.Fail("Khóa học không tồn tại."),
                -5 => OperationResult.Fail("Giảng viên chủ nhiệm không tồn tại."),
                -6 => OperationResult.Fail("Thao tác thất bại."),
                _ => OperationResult.Fail("Thao tác thất bại.")
            };
        }
        }
    }

