using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class KhoaDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var p = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachKhoa", p);
        }

        public static OperationResult Add(string maKhoa, string tenKhoa)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_ThemKhoa", new[]
            {
                new SqlParameter("@MaKhoa",  SqlDbType.VarChar,   10)  { Value = maKhoa  },
                new SqlParameter("@TenKhoa", SqlDbType.NVarChar, 100)  { Value = tenKhoa },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Update(string maKhoa, string tenKhoa)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_SuaKhoa", new[]
            {
                new SqlParameter("@MaKhoa",  SqlDbType.VarChar,   10)  { Value = maKhoa  },
                new SqlParameter("@TenKhoa", SqlDbType.NVarChar, 100)  { Value = tenKhoa },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Delete(string maKhoa)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_XoaKhoa", new[]
            {
                new SqlParameter("@MaKhoa", SqlDbType.VarChar, 10) { Value = maKhoa },
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
                -1 => OperationResult.Fail("Mã khoa đã tồn tại hoặc không tồn tại."),
                -2 => OperationResult.Fail("Vui lòng nhập đầy đủ thông tin."),
                -3 => OperationResult.Fail("Không thể xóa khoa đang có ngành."),
                _  => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
