using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;
namespace QuanLySinhVien.DAL
{
    public static class SinhVienDAL
    {
        public static DataTable Search(string keyword = null, string maLopSH = null, string maKhoa = null, string maNganh = null, string tinhTrang = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value },
                new SqlParameter("@MaLopSH", SqlDbType.VarChar, 15) { Value = (object)maLopSH ?? DBNull.Value },
                new SqlParameter("@MaKhoa", SqlDbType.VarChar, 10) { Value = (object)maKhoa ?? DBNull.Value },
                new SqlParameter("@MaNganh", SqlDbType.VarChar, 10) { Value = (object)maNganh ?? DBNull.Value },
                new SqlParameter("@TinhTrang", SqlDbType.NVarChar, 20) { Value = (object)tinhTrang ?? DBNull.Value }
            };

            return DatabaseHelper.ExecuteDataTable("sp_TimKiemSinhVien", parameters);
        }

        public static OperationResult Add(string maSV, string hoTen, object ngaySinh, bool? gioiTinh, string diaChi, string maLopSH, string anhDaiDien)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaSV", SqlDbType.VarChar, 10) { Value = maSV },
                new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = hoTen },
                new SqlParameter("@NgaySinh", SqlDbType.Date) { Value = (object)ngaySinh ?? DBNull.Value },
                new SqlParameter("@GioiTinh", SqlDbType.Bit) { Value = gioiTinh.HasValue ? (object)gioiTinh.Value : DBNull.Value },
                new SqlParameter("@DiaChi", SqlDbType.NVarChar, 200) { Value = (object)diaChi ?? DBNull.Value },
                new SqlParameter("@MaLopSH", SqlDbType.VarChar, 15) { Value = maLopSH },
                new SqlParameter("@AnhDaiDien", SqlDbType.NVarChar, 255) { Value = (object)anhDaiDien ?? DBNull.Value },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemSinhVien", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Update(string maSV, string hoTen, object ngaySinh, bool? gioiTinh, string diaChi, string anhDaiDien)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaSV", SqlDbType.VarChar, 10) { Value = maSV },
                new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = hoTen },
                new SqlParameter("@NgaySinh", SqlDbType.Date) { Value = (object)ngaySinh ?? DBNull.Value },
                new SqlParameter("@GioiTinh", SqlDbType.Bit) { Value = gioiTinh.HasValue ? (object)gioiTinh.Value : DBNull.Value },
                new SqlParameter("@DiaChi", SqlDbType.NVarChar, 200) { Value = (object)diaChi ?? DBNull.Value },
                new SqlParameter("@AnhDaiDien", SqlDbType.NVarChar, 255) { Value = (object)anhDaiDien ?? DBNull.Value },
                resultParameter
            };

            DatabaseHelper.ExecuteNonQuery("sp_SuaThongTinSinhVien", parameters);
            return CreateResult(resultParameter);
        }

        public static OperationResult Delete(string maSV)
        {
            var resultParameter = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@MaSV", SqlDbType.VarChar, 10) { Value = maSV },
                resultParameter
            };
            DatabaseHelper.ExecuteNonQuery("sp_XoaSinhVien", parameters);
            return CreateResult(resultParameter);
        }

        private static OperationResult CreateResult(SqlParameter resultParameter)
        {
            var code = resultParameter.Value == DBNull.Value ? 0 : (int)resultParameter.Value;
            if (code == 1)
                return OperationResult.Ok();
            return code switch
            {
                -1 => OperationResult.Fail("Bản ghi không tồn tại hoặc đã tồn tại."),
                -2 => OperationResult.Fail("Vui lòng điền đầy đủ thông tin bắt buộc."),
                -3 => OperationResult.Fail("Mã lớp sinh hoạt không tồn tại."),
                _ => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
