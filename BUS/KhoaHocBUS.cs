using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class KhoaHocBUS
    {
        public static DataTable LoadAll(string keyword = null) => KhoaHocDAL.LoadAll(keyword);

        public static OperationResult Add(string maKhoaHoc, string tenKhoaHoc)
        {
            if (string.IsNullOrWhiteSpace(maKhoaHoc) || string.IsNullOrWhiteSpace(tenKhoaHoc))
                return OperationResult.Fail("Mã khóa học và tên khóa học là bắt buộc.");
            return KhoaHocDAL.Add(maKhoaHoc.Trim().ToUpperInvariant(), tenKhoaHoc.Trim());
        }

        public static OperationResult Update(string maKhoaHoc, string tenKhoaHoc)
        {
            if (string.IsNullOrWhiteSpace(maKhoaHoc) || string.IsNullOrWhiteSpace(tenKhoaHoc))
                return OperationResult.Fail("Mã khóa học và tên khóa học là bắt buộc.");
            return KhoaHocDAL.Update(maKhoaHoc.Trim().ToUpperInvariant(), tenKhoaHoc.Trim());
        }

        public static OperationResult Delete(string maKhoaHoc)
        {
            if (string.IsNullOrWhiteSpace(maKhoaHoc))
                return OperationResult.Fail("Mã khóa học là bắt buộc.");
            return KhoaHocDAL.Delete(maKhoaHoc.Trim().ToUpperInvariant());
        }
    }
}
