using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class GiangVienDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var p = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachGiangVien", p);
        }

        public static OperationResult Add(string maGV, string hoTen, string maKhoa,
            string email, string soDienThoai, DateTime? ngaySinh, bool? gioiTinh,
            string hocVi, string hocHam)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_ThemGiangVien", new[]
            {
                new SqlParameter("@MaGV",        SqlDbType.VarChar,   10)  { Value = maGV },
                new SqlParameter("@HoTen",       SqlDbType.NVarChar, 100)  { Value = hoTen },
                new SqlParameter("@MaKhoa",      SqlDbType.VarChar,   10)  { Value = maKhoa },
                new SqlParameter("@Email",       SqlDbType.VarChar,  100)  { Value = (object)email ?? DBNull.Value },
                new SqlParameter("@SoDienThoai", SqlDbType.VarChar,   15)  { Value = (object)soDienThoai ?? DBNull.Value },
                new SqlParameter("@NgaySinh",    SqlDbType.Date)           { Value = (object)ngaySinh ?? DBNull.Value },
                new SqlParameter("@GioiTinh",    SqlDbType.Bit)            { Value = (object)gioiTinh ?? DBNull.Value },
                new SqlParameter("@HocVi",       SqlDbType.NVarChar,  50)  { Value = (object)hocVi ?? DBNull.Value },
                new SqlParameter("@HocHam",      SqlDbType.NVarChar,  50)  { Value = (object)hocHam ?? DBNull.Value },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Update(string maGV, string hoTen, string maKhoa,
            string email, string soDienThoai, DateTime? ngaySinh, bool? gioiTinh,
            string hocVi, string hocHam)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_SuaGiangVien", new[]
            {
                new SqlParameter("@MaGV",        SqlDbType.VarChar,   10)  { Value = maGV },
                new SqlParameter("@HoTen",       SqlDbType.NVarChar, 100)  { Value = hoTen },
                new SqlParameter("@MaKhoa",      SqlDbType.VarChar,   10)  { Value = maKhoa },
                new SqlParameter("@Email",       SqlDbType.VarChar,  100)  { Value = (object)email ?? DBNull.Value },
                new SqlParameter("@SoDienThoai", SqlDbType.VarChar,   15)  { Value = (object)soDienThoai ?? DBNull.Value },
                new SqlParameter("@NgaySinh",    SqlDbType.Date)           { Value = (object)ngaySinh ?? DBNull.Value },
                new SqlParameter("@GioiTinh",    SqlDbType.Bit)            { Value = (object)gioiTinh ?? DBNull.Value },
                new SqlParameter("@HocVi",       SqlDbType.NVarChar,  50)  { Value = (object)hocVi ?? DBNull.Value },
                new SqlParameter("@HocHam",      SqlDbType.NVarChar,  50)  { Value = (object)hocHam ?? DBNull.Value },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Delete(string maGV)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_XoaGiangVien", new[]
            {
                new SqlParameter("@MaGV", SqlDbType.VarChar, 10) { Value = maGV },
                rp
            });
            return CreateResult(rp);
        }

        private static OperationResult CreateResult(SqlParameter rp)
        {
            var code = rp.Value == DBNull.Value ? 0 : (int)rp.Value;
            return code switch
            {
                1  => OperationResult.Ok(),
                -1 => OperationResult.Fail("Mã giảng viên đã tồn tại hoặc không tồn tại."),
                -2 => OperationResult.Fail("Vui lòng nhập đầy đủ thông tin."),
                -3 => OperationResult.Fail("Khoa không tồn tại."),
                -4 => OperationResult.Fail("Email đã được sử dụng bởi giảng viên khác."),
                _  => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
