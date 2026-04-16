using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class PhongHocBUS
    {
        public static DataTable LoadAll(string keyword = null) => PhongHocDAL.LoadAll(keyword);

        public static OperationResult Add(string maPhong, string tenPhong, string sucChuaStr, string viTri)
        {
            if (string.IsNullOrWhiteSpace(maPhong) || string.IsNullOrWhiteSpace(tenPhong))
                return OperationResult.Fail("Mã phòng và tên phòng là bắt buộc.");
            if (!int.TryParse(sucChuaStr, out int sucChua) || sucChua <= 0)
                return OperationResult.Fail("Sức chứa phải là số nguyên dương.");
            return PhongHocDAL.Add(maPhong.Trim().ToUpperInvariant(), tenPhong.Trim(), sucChua,
                string.IsNullOrWhiteSpace(viTri) ? null : viTri.Trim());
        }

        public static OperationResult Update(string maPhong, string tenPhong, string sucChuaStr, string viTri)
        {
            if (string.IsNullOrWhiteSpace(maPhong) || string.IsNullOrWhiteSpace(tenPhong))
                return OperationResult.Fail("Mã phòng và tên phòng là bắt buộc.");
            if (!int.TryParse(sucChuaStr, out int sucChua) || sucChua <= 0)
                return OperationResult.Fail("Sức chứa phải là số nguyên dương.");
            return PhongHocDAL.Update(maPhong.Trim().ToUpperInvariant(), tenPhong.Trim(), sucChua,
                string.IsNullOrWhiteSpace(viTri) ? null : viTri.Trim());
        }

        public static OperationResult Delete(string maPhong)
        {
            if (string.IsNullOrWhiteSpace(maPhong))
                return OperationResult.Fail("Mã phòng là bắt buộc.");
            return PhongHocDAL.Delete(maPhong.Trim().ToUpperInvariant());
        }
    }
}
