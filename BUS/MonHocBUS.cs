using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class MonHocBUS
    {
        public static DataTable LoadAll(string keyword = null)
        {
            return MonHocDAL.LoadAll(keyword);
        }

        public static OperationResult Add(string maMon, string tenMon, int soTinChi, string monTienQuyet)
        {
            if (string.IsNullOrWhiteSpace(maMon) || string.IsNullOrWhiteSpace(tenMon) || soTinChi < 1)
                return OperationResult.Fail("Mã môn, tên môn và số tín chỉ hợp lệ là bắt buộc.");

            return MonHocDAL.Add(maMon.Trim().ToUpperInvariant(), tenMon.Trim(), soTinChi, string.IsNullOrWhiteSpace(monTienQuyet) ? null : monTienQuyet.Trim().ToUpperInvariant());
        }

        public static OperationResult Update(string maMon, string tenMon, int soTinChi, string monTienQuyet)
        {
            if (string.IsNullOrWhiteSpace(maMon) || string.IsNullOrWhiteSpace(tenMon) || soTinChi < 1)
                return OperationResult.Fail("Mã môn, tên môn và số tín chỉ hợp lệ là bắt buộc.");

            return MonHocDAL.Update(maMon.Trim().ToUpperInvariant(), tenMon.Trim(), soTinChi, string.IsNullOrWhiteSpace(monTienQuyet) ? null : monTienQuyet.Trim().ToUpperInvariant());
        }

        public static OperationResult Delete(string maMon)
        {
            if (string.IsNullOrWhiteSpace(maMon))
                return OperationResult.Fail("Mã môn học là bắt buộc.");

            return MonHocDAL.Delete(maMon.Trim().ToUpperInvariant());
        }
    }
}
