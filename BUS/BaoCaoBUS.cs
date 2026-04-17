using System;
using System.Data;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.BUS
{
    public static class BaoCaoBUS
    {
        // ── BC04: Bảng điểm cá nhân ──────────────────────────────────────
        public static DataTable LayDiemSinhVien(string maSV, string maHocKy = null)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                throw new ArgumentException("Mã sinh viên là bắt buộc.");
            return BaoCaoDAL.LayDiemSinhVien(
                maSV.Trim().ToUpperInvariant(),
                string.IsNullOrWhiteSpace(maHocKy) ? null : maHocKy.Trim()
            );
        }
    }
}
