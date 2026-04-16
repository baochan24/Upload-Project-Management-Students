using System;

namespace QuanLySinhVien.Models
{
    public class HocPhi
    {
        public int MaHocPhi { get; set; }
        public string MaSV { get; set; }
        public string MaHocKy { get; set; }
        public int SoTinChiDangKy { get; set; }
        public float TongHocPhi { get; set; }
        public float DaDong { get; set; }
        public string TrangThai { get; set; }
        public DateTime? NgayDong { get; set; }
    }
}
