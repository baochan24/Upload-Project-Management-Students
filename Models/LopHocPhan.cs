namespace QuanLySinhVien.Models
{
    public class LopHocPhan
    {
        public string MaLHP { get; set; }
        public string MaLopHienThi { get; set; }
        public string MaMon { get; set; }
        public string MaGV { get; set; }
        public string MaHocKy { get; set; }
        public string MaPhong { get; set; }
        public int SiSoToiDa { get; set; }
        public int SiSoHienTai { get; set; }
        public string TrangThai { get; set; }
        public byte? Thu { get; set; }
        public byte? TietBatDau { get; set; }
        public byte? TietKetThuc { get; set; }
    }
}
