using System;

namespace QuanLySinhVien.Models
{
    public class Diem
    {
        public int MaDiem { get; set; }
        public int MaDK { get; set; }
        public float? DiemChuyenCan { get; set; }
        public float? DiemGiuaKy { get; set; }
        public float? DiemCuoiKy { get; set; }
        public float? DiemTongKet { get; set; }
        public string XepLoai { get; set; }
        public string TrangThaiDiem { get; set; }
        public DateTime? NgayXacNhan { get; set; }
        public string NguoiNhap { get; set; }
        public DateTime NgayNhap { get; set; }
    }
}
