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

        /// <summary>
        /// Kiểm tra giảng viên có quyền nhập/sửa điểm cho lớp học phần không.
        /// Kết quả: 1 = được phép, -1 = lớp không tồn tại, -2 = không phải GV của lớp.
        /// </summary>
        public static int KiemTraQuyenSuaDiem(string maLHP, string maGV)
        {
            if (string.IsNullOrWhiteSpace(maLHP) || string.IsNullOrWhiteSpace(maGV))
                return -2;
            return DiemDAL.KiemTraQuyenSuaDiem(maLHP.Trim().ToUpperInvariant(), maGV.Trim().ToUpperInvariant());
        }

        /// <summary>Lấy bảng điểm cá nhân của sinh viên đang đăng nhập.</summary>
        public static System.Data.DataTable LayDiemCaNhan(string maSV, string maHocKy = null)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                return new System.Data.DataTable();
            return DiemDAL.LayDiemCaNhan(maSV.Trim().ToUpperInvariant(),
                                         string.IsNullOrWhiteSpace(maHocKy) ? null : maHocKy.Trim().ToUpperInvariant());
        }
    }
}
