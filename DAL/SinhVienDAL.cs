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

        // ── SV03: Soft delete – đổi TinhTrang ────────────────────────────
        public static OperationResult CapNhatTinhTrang(string maSV, string tinhTrangMoi)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_CapNhatTinhTrangSinhVien", new[]
            {
                new SqlParameter("@MaSV",         SqlDbType.VarChar,   10) { Value = maSV },
                new SqlParameter("@TinhTrangMoi",  SqlDbType.NVarChar,  20) { Value = tinhTrangMoi },
                rp
            });
            var code = rp.Value == DBNull.Value ? 0 : (int)rp.Value;
            return code switch
            {
                1  => OperationResult.Ok("Cập nhật tình trạng sinh viên thành công."),
                -1 => OperationResult.Fail("Không tìm thấy sinh viên."),
                -2 => OperationResult.Fail("Giá trị tình trạng không hợp lệ."),
                _  => OperationResult.Fail("Thao tác thất bại.")
            };
        }

        // ── SV06: Xem chi tiết hồ sơ (2 result sets) ─────────────────────
        public static DataTable[] LayChiTiet(string maSV)
        {
            var p = new SqlParameter("@MaSV", SqlDbType.VarChar, 10) { Value = maSV };
            return DatabaseHelper.ExecuteMultipleResultSets("sp_LayChiTietSinhVien", 2, p);
        }

        // ── SV07: Chuyển lớp có log ───────────────────────────────────────
        public static OperationResult ChuyenLop(string maSV, string maLopMoi, string lyDo, string nguoiDuyet)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_ChuyenLop_CoLog", new[]
            {
                new SqlParameter("@MaSV",       SqlDbType.VarChar,   10)  { Value = maSV },
                new SqlParameter("@MaLopMoi",   SqlDbType.VarChar,   15)  { Value = maLopMoi },
                new SqlParameter("@LyDo",       SqlDbType.NVarChar, 200)  { Value = (object)lyDo ?? DBNull.Value },
                new SqlParameter("@NguoiDuyet", SqlDbType.NVarChar, 100)  { Value = (object)nguoiDuyet ?? DBNull.Value },
                rp
            });
            var code = rp.Value == DBNull.Value ? 0 : (int)rp.Value;
            return code switch
            {
                1  => OperationResult.Ok("Chuyển lớp thành công."),
                -1 => OperationResult.Fail("Sinh viên không tồn tại hoặc không đang học."),
                -2 => OperationResult.Fail("Lớp sinh hoạt mới không tồn tại."),
                _  => OperationResult.Fail("Thao tác thất bại.")
            };
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
