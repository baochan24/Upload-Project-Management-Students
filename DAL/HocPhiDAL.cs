using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuanLySinhVien.DAL
{
    public static class HocPhiDAL
    {
        public static DataSet LoadReport(string maHocKy)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["QuanLySinhVien"]?.ConnectionString))
            using (var command = new SqlCommand("sp_BaoCaoHocPhi", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.Add(new SqlParameter("@MaHocKy", SqlDbType.VarChar, 10) { Value = maHocKy ?? (object)DBNull.Value });
                var adapter = new SqlDataAdapter(command);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                return dataSet;
            }
        }
    }
}
