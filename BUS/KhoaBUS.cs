using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class KhoaBUS
    {
        public static DataTable LoadAll(string keyword = null) => KhoaDAL.LoadAll(keyword);

        public static OperationResult Add(string maKhoa, string tenKhoa)
        {
            if (string.IsNullOrWhiteSpace(maKhoa) || string.IsNullOrWhiteSpace(tenKhoa))
                return OperationResult.Fail("Mã khoa và tên khoa là bắt buộc.");
            return KhoaDAL.Add(maKhoa.Trim().ToUpperInvariant(), tenKhoa.Trim());
        }

        public static OperationResult Update(string maKhoa, string tenKhoa)
        {
            if (string.IsNullOrWhiteSpace(maKhoa) || string.IsNullOrWhiteSpace(tenKhoa))
                return OperationResult.Fail("Mã khoa và tên khoa là bắt buộc.");
            return KhoaDAL.Update(maKhoa.Trim().ToUpperInvariant(), tenKhoa.Trim());
        }

        public static OperationResult Delete(string maKhoa)
        {
            if (string.IsNullOrWhiteSpace(maKhoa))
                return OperationResult.Fail("Mã khoa là bắt buộc.");
            return KhoaDAL.Delete(maKhoa.Trim().ToUpperInvariant());
        }
    }
}
