using System;

namespace QuanLySinhVien.Models
{
    public class HocKy
    {
        public string MaHocKy { get; set; }
        public string TenHocKy { get; set; }
        public string NamHoc { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int SoTinToiDa { get; set; }
    }
}
