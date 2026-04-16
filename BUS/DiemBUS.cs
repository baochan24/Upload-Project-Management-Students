using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class DiemBUS
    {
        public static DataTable LoadAll(string keyword = null)
        {
            return DiemDAL.LoadAll(keyword);
        }

        public static OperationResult EnterScore(int maDK, float? diemChuyenCan, float? diemGiuaKy, float? diemCuoiKy)
        {
            if (maDK <= 0)
                return OperationResult.Fail("Mã đăng ký là bắt buộc.");

            return DiemDAL.EnterScore(maDK, diemChuyenCan, diemGiuaKy, diemCuoiKy, UserSession.Username ?? "System");
        }

        public static OperationResult UpdateScore(int maDiem, string loaiDiem, float diemMoi)
        {
            if (maDiem <= 0 || string.IsNullOrWhiteSpace(loaiDiem))
                return OperationResult.Fail("Mã điểm và loại điểm là bắt buộc.");
            if (diemMoi < 0 || diemMoi > 10)
                return OperationResult.Fail("Điểm phải nằm trong khoảng 0 đến 10.");

            return DiemDAL.UpdateScore(maDiem, loaiDiem.Trim().ToLowerInvariant(), diemMoi, UserSession.Username ?? "System");
        }

        public static OperationResult LockClassScores(string maLHP)
        {
            if (string.IsNullOrWhiteSpace(maLHP))
                return OperationResult.Fail("Mã lớp học phần là bắt buộc.");

            return DiemDAL.LockClassScores(maLHP.Trim().ToUpperInvariant(), UserSession.Username ?? "System");
        }
    }
}
