using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class HocKyBUS
    {
        public static DataTable LoadAll(string keyword = null)
        {
            return HocKyDAL.LoadAll(keyword);
        }

        public static OperationResult Add(string maHocKy, string tenHocKy, string namHoc, object ngayBatDau, object ngayKetThuc, int soTinToiDa)
        {
            if (string.IsNullOrWhiteSpace(maHocKy) || string.IsNullOrWhiteSpace(tenHocKy) || string.IsNullOrWhiteSpace(namHoc) || soTinToiDa < 1)
                return OperationResult.Fail("Mã học kỳ, tên học kỳ, năm học và sĩ số tối đa là bắt buộc.");

            return HocKyDAL.Add(maHocKy.Trim().ToUpperInvariant(), tenHocKy.Trim(), namHoc.Trim(), ngayBatDau, ngayKetThuc, soTinToiDa);
        }

        public static OperationResult Update(string maHocKy, string tenHocKy, string namHoc, object ngayBatDau, object ngayKetThuc, int soTinToiDa)
        {
            if (string.IsNullOrWhiteSpace(maHocKy) || string.IsNullOrWhiteSpace(tenHocKy) || string.IsNullOrWhiteSpace(namHoc) || soTinToiDa < 1)
                return OperationResult.Fail("Mã học kỳ, tên học kỳ, năm học và sĩ số tối đa là bắt buộc.");

            return HocKyDAL.Update(maHocKy.Trim().ToUpperInvariant(), tenHocKy.Trim(), namHoc.Trim(), ngayBatDau, ngayKetThuc, soTinToiDa);
        }

        public static OperationResult Delete(string maHocKy)
        {
            if (string.IsNullOrWhiteSpace(maHocKy))
                return OperationResult.Fail("Mã học kỳ là bắt buộc.");

            return HocKyDAL.Delete(maHocKy.Trim().ToUpperInvariant());
        }
    }
}
