using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class DangKyBUS
    {
        public static DataTable LoadAll(string keyword = null)
        {
            return DangKyDAL.LoadAll(keyword);
        }

        /// <summary>Lấy lịch học / danh sách đăng ký của một sinh viên.</summary>
        public static DataTable LoadBySinhVien(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                return new DataTable();
            return DangKyDAL.LoadBySinhVien(maSV.Trim().ToUpperInvariant());
        }

        public static OperationResult Register(string maSV, string maLHP)
        {
            if (string.IsNullOrWhiteSpace(maSV) || string.IsNullOrWhiteSpace(maLHP))
                return OperationResult.Fail("Sinh viên và lớp học phần phải được chọn.");

            return DangKyDAL.Register(maSV.Trim().ToUpperInvariant(), maLHP.Trim().ToUpperInvariant());
        }

        public static OperationResult Cancel(string maSV, string maLHP)
        {
            if (string.IsNullOrWhiteSpace(maSV) || string.IsNullOrWhiteSpace(maLHP))
                return OperationResult.Fail("Sinh viên và lớp học phần phải được chọn.");

            return DangKyDAL.Cancel(maSV.Trim().ToUpperInvariant(), maLHP.Trim().ToUpperInvariant());
        }
    }
}
