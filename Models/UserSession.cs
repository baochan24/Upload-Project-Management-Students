namespace QuanLySinhVien.Models
{
    public static class UserSession
    {
        public static int UserID { get; set; }
        public static string Username { get; set; }
        public static string RoleName { get; set; }
        public static string MaNguoiDung { get; set; }
        public static string Email { get; set; }

        public static bool IsAdmin => string.Equals(RoleName, "Admin", System.StringComparison.OrdinalIgnoreCase);
        public static bool IsStaff => string.Equals(RoleName, "PhongDT", System.StringComparison.OrdinalIgnoreCase);
        public static bool IsLoggedIn => !string.IsNullOrWhiteSpace(Username);

        public static void Clear()
        {
            UserID = 0;
            Username = null;
            RoleName = null;
            MaNguoiDung = null;
            Email = null;
        }
    }
}
