using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class NganhDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var p = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachNganhHoc", p);
        }

        public static OperationResult Add(string maNganh, string tenNganh, string maKhoa)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_ThemNganhHoc", new[]
            {
                new SqlParameter("@MaNganh",  SqlDbType.VarChar,   10)  { Value = maNganh  },
                new SqlParameter("@TenNganh", SqlDbType.NVarChar, 100)  { Value = tenNganh },
                new SqlParameter("@MaKhoa",   SqlDbType.VarChar,   10)  { Value = maKhoa   },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Update(string maNganh, string tenNganh, string maKhoa)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_SuaNganhHoc", new[]
            {
                new SqlParameter("@MaNganh",  SqlDbType.VarChar,   10)  { Value = maNganh  },
                new SqlParameter("@TenNganh", SqlDbType.NVarChar, 100)  { Value = tenNganh },
                new SqlParameter("@MaKhoa",   SqlDbType.VarChar,   10)  { Value = maKhoa   },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Delete(string maNganh)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_XoaNganhHoc", new[]
            {
                new SqlParameter("@MaNganh", SqlDbType.VarChar, 10) { Value = maNganh },
                rp
            });
            return CreateDeleteResult(rp);
        }

        private static OperationResult CreateResult(SqlParameter rp)
        {
            var code = rp.Value == DBNull.Value ? 0 : (int)rp.Value;
            return code switch
            {
                1  => OperationResult.Ok(),
                -1 => OperationResult.Fail("Mã ngành đã tồn tại."),
                -2 => OperationResult.Fail("Vui lòng nhập đầy đủ thông tin."),
                -3 => OperationResult.Fail("Khoa không tồn tại."),
                _  => OperationResult.Fail("Thao tác thất bại.")
            };
        }

        private static OperationResult CreateDeleteResult(SqlParameter rp)
        {
            var code = rp.Value == DBNull.Value ? 0 : (int)rp.Value;
            return code switch
            {
                1  => OperationResult.Ok(),
                -1 => OperationResult.Fail("Không tìm thấy ngành này."),
                -2 => OperationResult.Fail("Không thể xóa: ngành đang có lớp sinh hoạt."),
                _  => OperationResult.Fail("Thao tác thất bại.")
            };
        }
    }
}
