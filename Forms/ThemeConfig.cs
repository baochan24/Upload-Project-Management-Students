using System.Drawing;

namespace QuanLySinhVien.Forms
{
    /// <summary>Bảng màu và font tập trung – chỉnh một chỗ, áp dụng toàn app.</summary>
    public static class ThemeConfig
    {
        // ── Màu chủ đạo ───────────────────────────────────────────────────
        public static readonly Color PrimaryDark    = Color.FromArgb( 44,  62,  80);  // #2C3E50
        public static readonly Color PrimaryMid     = Color.FromArgb( 52,  73,  94);  // #34495E
        public static readonly Color PrimaryLight   = Color.FromArgb( 52, 152, 219);  // #3498DB

        // ── Màu gradient Login (#667eea → #764ba2) ────────────────────────
        public static readonly Color LoginGrad1     = Color.FromArgb(102, 126, 234);  // #667eea
        public static readonly Color LoginGrad2     = Color.FromArgb(118,  75, 162);  // #764ba2

        // ── Màu nút chức năng ─────────────────────────────────────────────
        public static readonly Color BtnAdd         = Color.FromArgb( 39, 174,  96);
        public static readonly Color BtnAddHover    = Color.FromArgb( 30, 139,  76);
        public static readonly Color BtnEdit        = Color.FromArgb(243, 156,  18);
        public static readonly Color BtnEditHover   = Color.FromArgb(194, 124,  14);
        public static readonly Color BtnDelete      = Color.FromArgb(231,  76,  60);
        public static readonly Color BtnDeleteHover = Color.FromArgb(185,  61,  48);
        public static readonly Color BtnSearch      = Color.FromArgb( 52, 152, 219);
        public static readonly Color BtnSearchHover = Color.FromArgb( 41, 128, 185);
        public static readonly Color BtnNeutral     = Color.FromArgb(127, 140, 141);
        public static readonly Color BtnNeutralHover= Color.FromArgb(100, 112, 112);

        // ── DataGridView ──────────────────────────────────────────────────
        public static readonly Color DgvHeader      = Color.FromArgb( 44,  62,  80);
        public static readonly Color DgvRowAlt      = Color.FromArgb(245, 248, 251);
        public static readonly Color DgvHover       = Color.FromArgb(214, 234, 248);
        public static readonly Color DgvSelect      = Color.FromArgb( 52, 152, 219);

        // ── Nền ───────────────────────────────────────────────────────────
        public static readonly Color BgMain         = Color.FromArgb(242, 245, 248);
        public static readonly Color BgPanel        = Color.White;

        // ── Chữ ──────────────────────────────────────────────────────────
        public static readonly Color TextWhite      = Color.White;
        public static readonly Color TextDark       = Color.FromArgb( 44,  62,  80);
        public static readonly Color TextMuted      = Color.FromArgb(127, 140, 141);
        public static readonly Color TextAccent     = Color.FromArgb(102, 126, 234);

        // ── Font ──────────────────────────────────────────────────────────
        public static readonly Font FontDefault  = new Font("Segoe UI",  9.5f, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font FontBold     = new Font("Segoe UI",  9.5f, FontStyle.Bold,    GraphicsUnit.Point);
        public static readonly Font FontTitle    = new Font("Segoe UI", 15f,   FontStyle.Bold,    GraphicsUnit.Point);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 10f,   FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font FontSmall    = new Font("Segoe UI",  8.5f, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font FontCaption  = new Font("Segoe UI",  7.5f, FontStyle.Bold,    GraphicsUnit.Point);

        // ── Kích thước nút ────────────────────────────────────────────────
        public static readonly Size ButtonSize     = new Size(110, 32);
        public static readonly Size ButtonSizeWide = new Size(140, 32);
    }
}
