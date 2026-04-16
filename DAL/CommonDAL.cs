using System.Data;
using System.Data.SqlClient;

namespace QuanLySinhVien.DAL
{
    public static class CommonDAL
    {
        public static DataTable LoadLookup(string storedProcedure, params SqlParameter[] parameters)
        {
            return DatabaseHelper.ExecuteDataTable(storedProcedure, parameters);
        }
    }
}
