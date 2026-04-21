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

        /// <summary>Lấy thông tin học phí của một sinh viên cụ thể.</summary>
        public static DataSet LoadBySinhVien(string maSV, string maHocKy = null)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                return new DataSet();
            return HocPhiDAL.LoadBySinhVien(maSV.Trim().ToUpperInvariant(),
                                            string.IsNullOrWhiteSpace(maHocKy) ? null : maHocKy.Trim().ToUpperInvariant());
        }
    }
}
