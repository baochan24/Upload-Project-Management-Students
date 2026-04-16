using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class DiemDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var parameter = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachDiem", parameter);
        }

        public static OperationResult EnterScore(int maDK, float? diemtiet, float? diemgiuaky, float? diemcuoiky, string nguoiNhap)
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery("sp_NhapDiem",
                    new SqlParameter("@MaDK", SqlDbType.Int) { Value = maDK },
                    new SqlParameter("@DiemChuyenCan", SqlDbType.Float) { Value = (object)diemtiet ?? DBNull.Value },
                    new SqlParameter("@DiemGiuaKy", SqlDbType.Float) { Value = (object)diemgiuaky ?? DBNull.Value },
                    new SqlParameter("@DiemCuoiKy", SqlDbType.Float) { Value = (object)diemcuoiky ?? DBNull.Value },
                    new SqlParameter("@NguoiNhap", SqlDbType.NVarChar, 100) { Value = nguoiNhap });
                return OperationResult.Ok();
            }
            catch (SqlException ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public static OperationResult UpdateScore(int maDiem, string loaiDiem, float diemMoi, string nguoiSua)
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery("sp_SuaDiem",
                    new SqlParameter("@MaDiem", SqlDbType.Int) { Value = maDiem },
                    new SqlParameter("@LoaiDiem", SqlDbType.NVarChar, 10) { Value = loaiDiem },
                    new SqlParameter("@DiemMoi", SqlDbType.Float) { Value = diemMoi },
                    new SqlParameter("@NguoiSua", SqlDbType.NVarChar, 100) { Value = nguoiSua });
                return OperationResult.Ok();
            }
            catch (SqlException ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public static OperationResult LockClassScores(string maLHP, string nguoiXacNhan)
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery("sp_XacNhanDiem",
                    new SqlParameter("@MaLHP", SqlDbType.VarChar, 20) { Value = maLHP },
                    new SqlParameter("@NguoiXacNhan", SqlDbType.NVarChar, 100) { Value = (object)nguoiXacNhan ?? DBNull.Value });
                return OperationResult.Ok("Đã khóa điểm cho lớp học phần.");
            }
            catch (SqlException ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
    }
}
