namespace QuanLySinhVien.Models
{
    public static class UserSession
    {
        public static int UserID { get; set; }
        public static int RoleID { get; set; }
        public static string Username { get; set; }
        public static string RoleName { get; set; }
        public static string MaNguoiDung { get; set; }
        public static string Email { get; set; }

        public static bool IsAdmin       => string.Equals(RoleName, "Admin",    System.StringComparison.OrdinalIgnoreCase);
        public static bool IsStaff       => string.Equals(RoleName, "PhongDT",  System.StringComparison.OrdinalIgnoreCase);
        public static bool IsGiangVien   => string.Equals(RoleName, "GiangVien",System.StringComparison.OrdinalIgnoreCase);
        public static bool IsSinhVien    => string.Equals(RoleName, "SinhVien", System.StringComparison.OrdinalIgnoreCase);
        public static bool IsLoggedIn    => !string.IsNullOrWhiteSpace(Username);

        /// <summary>Mã sinh viên – chỉ có giá trị khi IsSinhVien = true.</summary>
        public static string MaSV => IsSinhVien ? MaNguoiDung : null;

        /// <summary>Mã giảng viên – chỉ có giá trị khi IsGiangVien = true.</summary>
        public static string MaGV => IsGiangVien ? MaNguoiDung : null;

        public static void Clear()
        {
            UserID      = 0;
            RoleID      = 0;
            Username    = null;
            RoleName    = null;
            MaNguoiDung = null;
            Email       = null;
        }
    }
}
