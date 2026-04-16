using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class LopHocPhanBUS
    {
        public static DataTable LoadAll(string keyword = null)
        {
            return LopHocPhanDAL.LoadAll(keyword);
        }

        public static OperationResult Add(string maLHP, string maLopHienThi, string maMon, string maGV, string maHocKy, string maPhong, int siSoToiDa, byte? thu, byte? tietBatDau, byte? tietKetThuc)
        {
            if (string.IsNullOrWhiteSpace(maLHP) || string.IsNullOrWhiteSpace(maLopHienThi) || string.IsNullOrWhiteSpace(maMon) || string.IsNullOrWhiteSpace(maGV) || string.IsNullOrWhiteSpace(maHocKy) || string.IsNullOrWhiteSpace(maPhong))
                return OperationResult.Fail("Các trường mã lớp, mã môn, giảng viên, học kỳ và phòng học là bắt buộc.");
            if (siSoToiDa <= 0)
                return OperationResult.Fail("Sĩ số tối đa phải lớn hơn 0.");

            return LopHocPhanDAL.Add(maLHP.Trim().ToUpperInvariant(), maLopHienThi.Trim(), maMon.Trim().ToUpperInvariant(), maGV.Trim().ToUpperInvariant(), maHocKy.Trim().ToUpperInvariant(), maPhong.Trim().ToUpperInvariant(), siSoToiDa, thu, tietBatDau, tietKetThuc);
        }

        public static OperationResult Update(string maLHP, string maLopHienThi, string maGV, string maPhong, int siSoToiDa, byte? thu, byte? tietBatDau, byte? tietKetThuc, string trangThai)
        {
            if (string.IsNullOrWhiteSpace(maLHP) || string.IsNullOrWhiteSpace(maLopHienThi) || string.IsNullOrWhiteSpace(maGV) || string.IsNullOrWhiteSpace(maPhong))
                return OperationResult.Fail("Các trường mã lớp, mã giảng viên và phòng học là bắt buộc.");
            if (siSoToiDa <= 0)
                return OperationResult.Fail("Sĩ số tối đa phải lớn hơn 0.");

            return LopHocPhanDAL.Update(maLHP.Trim().ToUpperInvariant(), maLopHienThi.Trim(), maGV.Trim().ToUpperInvariant(), maPhong.Trim().ToUpperInvariant(), siSoToiDa, thu, tietBatDau, tietKetThuc, string.IsNullOrWhiteSpace(trangThai) ? null : trangThai.Trim());
        }

        public static OperationResult Delete(string maLHP)
        {
            if (string.IsNullOrWhiteSpace(maLHP))
                return OperationResult.Fail("Mã lớp học phần là bắt buộc.");

            return LopHocPhanDAL.Delete(maLHP.Trim().ToUpperInvariant());
        }
    }
}
