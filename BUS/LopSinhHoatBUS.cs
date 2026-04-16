using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class LopSinhHoatBUS
    {
        public static DataTable LoadAll(string keyword = null)
        {
            return LopSinhHoatDAL.LoadAll(keyword);
        }

        public static OperationResult Add(string maLopSH, string tenLop, string maNganh, string maKhoaHoc, string maGVCN)
        {
            if (string.IsNullOrWhiteSpace(maLopSH) || string.IsNullOrWhiteSpace(tenLop) || string.IsNullOrWhiteSpace(maNganh) || string.IsNullOrWhiteSpace(maKhoaHoc))
                return OperationResult.Fail("Mã lớp, tên lớp, ngành và khóa học là bắt buộc.");

            return LopSinhHoatDAL.Add(maLopSH.Trim().ToUpperInvariant(), tenLop.Trim(), maNganh.Trim().ToUpperInvariant(), maKhoaHoc.Trim().ToUpperInvariant(), string.IsNullOrWhiteSpace(maGVCN) ? null : maGVCN.Trim().ToUpperInvariant());
        }

        public static OperationResult Update(string maLopSH, string tenLop, string maNganh, string maKhoaHoc, string maGVCN)
        {
            if (string.IsNullOrWhiteSpace(maLopSH) || string.IsNullOrWhiteSpace(tenLop) || string.IsNullOrWhiteSpace(maNganh) || string.IsNullOrWhiteSpace(maKhoaHoc))
                return OperationResult.Fail("Mã lớp, tên lớp, ngành và khóa học là bắt buộc.");

            return LopSinhHoatDAL.Update(maLopSH.Trim().ToUpperInvariant(), tenLop.Trim(), maNganh.Trim().ToUpperInvariant(), maKhoaHoc.Trim().ToUpperInvariant(), string.IsNullOrWhiteSpace(maGVCN) ? null : maGVCN.Trim().ToUpperInvariant());
        }

        public static OperationResult Delete(string maLopSH)
        {
            if (string.IsNullOrWhiteSpace(maLopSH))
                return OperationResult.Fail("Mã lớp sinh hoạt là bắt buộc.");

            return LopSinhHoatDAL.Delete(maLopSH.Trim().ToUpperInvariant());
        }
    }
}
