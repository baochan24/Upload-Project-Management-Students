using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class KhoaHocDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var p = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachKhoaHoc", p);
        }

        public static OperationResult Add(string maKhoaHoc, string tenKhoaHoc)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_ThemKhoaHoc", new[]
            {
                new SqlParameter("@MaKhoaHoc",  SqlDbType.VarChar,   10)  { Value = maKhoaHoc  },
                new SqlParameter("@TenKhoaHoc", SqlDbType.NVarChar, 100)  { Value = tenKhoaHoc },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Update(string maKhoaHoc, string tenKhoaHoc)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_SuaKhoaHoc", new[]
            {
                new SqlParameter("@MaKhoaHoc",  SqlDbType.VarChar,   10)  { Value = maKhoaHoc  },
                new SqlParameter("@TenKhoaHoc", SqlDbType.NVarChar, 100)  { Value = tenKhoaHoc },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Delete(string maKhoaHoc)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_XoaKhoaHoc", new[]
            {
                new SqlParameter("@MaKhoaHoc", SqlDbType.VarChar, 10) { Value = maKhoaHoc },
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
                -1 => OperationResult.Fail("Mã khóa học đã tồn tại hoặc không tồn tại."),
                -2 => OperationResult.Fail("Vui lòng nhập đầy đủ thông tin."),
                -3 => OperationResult.Fail("Không thể xóa khóa học đang có lớp sinh hoạt."),
                _  => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
