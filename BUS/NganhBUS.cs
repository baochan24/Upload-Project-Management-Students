using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class NganhBUS
    {
        public static DataTable LoadAll(string keyword = null) => NganhDAL.LoadAll(keyword);

        public static OperationResult Add(string maNganh, string tenNganh, string maKhoa)
        {
            if (string.IsNullOrWhiteSpace(maNganh) || string.IsNullOrWhiteSpace(tenNganh) || string.IsNullOrWhiteSpace(maKhoa))
                return OperationResult.Fail("Mã ngành, tên ngành và khoa là bắt buộc.");
            return NganhDAL.Add(maNganh.Trim().ToUpperInvariant(), tenNganh.Trim(), maKhoa.Trim().ToUpperInvariant());
        }

        public static OperationResult Update(string maNganh, string tenNganh, string maKhoa)
        {
            if (string.IsNullOrWhiteSpace(maNganh) || string.IsNullOrWhiteSpace(tenNganh) || string.IsNullOrWhiteSpace(maKhoa))
                return OperationResult.Fail("Mã ngành, tên ngành và khoa là bắt buộc.");
            return NganhDAL.Update(maNganh.Trim().ToUpperInvariant(), tenNganh.Trim(), maKhoa.Trim().ToUpperInvariant());
        }

        public static OperationResult Delete(string maNganh)
        {
            if (string.IsNullOrWhiteSpace(maNganh))
                return OperationResult.Fail("Mã ngành là bắt buộc.");
            return NganhDAL.Delete(maNganh.Trim().ToUpperInvariant());
        }
    }
}
