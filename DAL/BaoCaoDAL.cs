using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLySinhVien.DAL
{
    public static class BaoCaoDAL
    {
        public static DataTable[] Dashboard()
        {
            return ExecuteMultipleResultSets("sp_DashboardTongHop", 4);
        }

        public static DataTable[] BaoCaoHocPhi(string maHocKy)
        {
            var p = new SqlParameter("@MaHocKy", SqlDbType.VarChar, 10) { Value = maHocKy };
            return ExecuteMultipleResultSets("sp_BaoCaoHocPhi", 2, p);
        }

        public static DataTable TimKiemSinhVienNangCao(string hoTen, string maSV, string maLopSH,
            string maKhoa, string maNganh, string tinhTrang, double? diemTu, double? diemDen, string hocKy)
        {
            return DatabaseHelper.ExecuteDataTable("sp_TimKiemSinhVienNangCao",
                new SqlParameter("@HoTen",     SqlDbType.NVarChar, 100) { Value = (object)hoTen ?? DBNull.Value },
                new SqlParameter("@MaSV",      SqlDbType.VarChar,   10) { Value = (object)maSV ?? DBNull.Value },
                new SqlParameter("@MaLopSH",   SqlDbType.VarChar,   15) { Value = (object)maLopSH ?? DBNull.Value },
                new SqlParameter("@MaKhoa",    SqlDbType.VarChar,   10) { Value = (object)maKhoa ?? DBNull.Value },
                new SqlParameter("@MaNganh",   SqlDbType.VarChar,   10) { Value = (object)maNganh ?? DBNull.Value },
                new SqlParameter("@TinhTrang", SqlDbType.NVarChar,  20) { Value = (object)tinhTrang ?? DBNull.Value },
                new SqlParameter("@DiemTu",    SqlDbType.Float)         { Value = (object)diemTu ?? DBNull.Value },
                new SqlParameter("@DiemDen",   SqlDbType.Float)         { Value = (object)diemDen ?? DBNull.Value },
                new SqlParameter("@HocKy",     SqlDbType.VarChar,   10) { Value = (object)hocKy ?? DBNull.Value }
            );
        }

        // ── BC04: Bảng điểm cá nhân ──────────────────────────────────────
        public static DataTable LayDiemSinhVien(string maSV, string maHocKy = null)
        {
            return DatabaseHelper.ExecuteDataTable("sp_LayDiemSinhVien",
                new SqlParameter("@MaSV",    SqlDbType.VarChar, 10) { Value = maSV },
                new SqlParameter("@MaHocKy", SqlDbType.VarChar, 10) { Value = (object)maHocKy ?? DBNull.Value }
            );
        }

        private static DataTable[] ExecuteMultipleResultSets(string sp, int count, params SqlParameter[] parameters)
        {
            var results = new DataTable[count];
            for (int i = 0; i < count; i++) results[i] = new DataTable();

            var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["QuanLySinhVien"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(connStr))
                throw new InvalidOperationException("Connection string 'QuanLySinhVien' chưa được cấu hình.");
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            using (var cmd = new System.Data.SqlClient.SqlCommand(sp, conn) { CommandType = CommandType.StoredProcedure })
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    for (int i = 0; i < count; i++)
                    {
                        results[i].Load(reader);
                        if (i < count - 1 && !reader.NextResult()) break;
                    }
                }
            }
            return results;
        }
    }
}
