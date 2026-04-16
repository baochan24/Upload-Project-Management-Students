using System.Data;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.BUS
{
    public static class HocPhiBUS
    {
        public static DataSet LoadReport(string maHocKy)
        {
            return HocPhiDAL.LoadReport(maHocKy);
        }
    }
}
