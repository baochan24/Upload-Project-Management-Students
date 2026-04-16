using System;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.DAL
{
    public static class PhongHocDAL
    {
        public static DataTable LoadAll(string keyword = null)
        {
            var p = new SqlParameter("@Keyword", SqlDbType.NVarChar, 100) { Value = (object)keyword ?? DBNull.Value };
            return DatabaseHelper.ExecuteDataTable("sp_LayDanhSachPhongHoc", p);
        }

        public static OperationResult Add(string maPhong, string tenPhong, int sucChua, string viTri)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_ThemPhongHoc", new[]
            {
                new SqlParameter("@MaPhong",  SqlDbType.VarChar,   10)  { Value = maPhong },
                new SqlParameter("@TenPhong", SqlDbType.NVarChar,  50)  { Value = tenPhong },
                new SqlParameter("@SucChua",  SqlDbType.Int)            { Value = sucChua },
                new SqlParameter("@ViTri",    SqlDbType.NVarChar, 100)  { Value = (object)viTri ?? DBNull.Value },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Update(string maPhong, string tenPhong, int sucChua, string viTri)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_SuaPhongHoc", new[]
            {
                new SqlParameter("@MaPhong",  SqlDbType.VarChar,   10)  { Value = maPhong },
                new SqlParameter("@TenPhong", SqlDbType.NVarChar,  50)  { Value = tenPhong },
                new SqlParameter("@SucChua",  SqlDbType.Int)            { Value = sucChua },
                new SqlParameter("@ViTri",    SqlDbType.NVarChar, 100)  { Value = (object)viTri ?? DBNull.Value },
                rp
            });
            return CreateResult(rp);
        }

        public static OperationResult Delete(string maPhong)
        {
            var rp = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DatabaseHelper.ExecuteNonQuery("sp_XoaPhongHoc", new[]
            {
                new SqlParameter("@MaPhong", SqlDbType.VarChar, 10) { Value = maPhong },
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
                -1 => OperationResult.Fail("Mã phòng đã tồn tại hoặc không tồn tại."),
                -2 => OperationResult.Fail("Vui lòng nhập đầy đủ thông tin."),
                -3 => OperationResult.Fail("Sức chứa phải từ 1 đến 1000."),
                -4 => OperationResult.Fail("Không thể giảm sức chứa xuống thấp hơn sĩ số hiện tại."),
                _  => OperationResult.Fail("Không thể xóa phòng đang được sử dụng hoặc thao tác thất bại.")
            };
        }
    }
}
