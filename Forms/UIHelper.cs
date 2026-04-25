using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QuanLySinhVien.Forms
{
    /// <summary>
    /// Các method tĩnh áp style UI – KHÔNG chứa logic nghiệp vụ.
    /// Gọi từ cuối constructor của từng Form (sau khi tạo xong control).
    /// </summary>
    public static class UIHelper
    {
        // ── Guard chống double-apply ───────────────────────────────────────
        private static readonly HashSet<Control> _styled = new HashSet<Control>();

        // ══════════════════════════════════════════════════════════════════
        // Standard Button
        // ══════════════════════════════════════════════════════════════════

        public static void StyleButton(Button btn, Color backColor, Color hoverColor,
                                       Color foreColor = default(Color), Size? size = null)
        {
            if (foreColor == default(Color)) foreColor = Color.White;

            btn.BackColor    = backColor;
            btn.ForeColor    = foreColor;
            btn.FlatStyle    = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize             = 0;
            btn.FlatAppearance.MouseOverBackColor     = hoverColor;
            btn.FlatAppearance.MouseDownBackColor     = ControlPaint.Dark(hoverColor, 0.1f);
            btn.Font         = ThemeConfig.FontBold;
            btn.Cursor       = Cursors.Hand;
            btn.Size         = size ?? ThemeConfig.ButtonSize;
            btn.TextAlign    = ContentAlignment.MiddleCenter;
            btn.UseVisualStyleBackColor = false;

            btn.MouseEnter += (s, e) => ((Button)s).BackColor = hoverColor;
            btn.MouseLeave += (s, e) => ((Button)s).BackColor = backColor;
        }

        // ══════════════════════════════════════════════════════════════════
        // Guna2Button
        // ══════════════════════════════════════════════════════════════════

        public static void StyleGuna2Button(Guna2Button btn, Color fillColor, Color hoverColor,
                                            Color foreColor = default(Color), int radius = 8,
                                            Size? size = null)
        {
            if (foreColor == default(Color)) foreColor = Color.White;

            btn.FillColor    = fillColor;
            btn.ForeColor    = foreColor;
            btn.Font         = ThemeConfig.FontBold;
            btn.BorderRadius = radius;
            btn.Cursor       = Cursors.Hand;
            if (size.HasValue) btn.Size = size.Value;
            btn.HoverState.FillColor = hoverColor;
        }

        // ══════════════════════════════════════════════════════════════════
        // DataGridView (+ Guna2DataGridView)
        // ══════════════════════════════════════════════════════════════════

        public static void StyleDataGridView(DataGridView dgv)
        {
            if (_styled.Contains(dgv)) return;
            _styled.Add(dgv);
            dgv.Disposed += (s, e) => _styled.Remove((DataGridView)s);

            if (dgv is Guna2DataGridView gunaGrid)
            {
                StyleGuna2DataGridView(gunaGrid);
                return;
            }

            // ── Standard DataGridView ──────────────────────────────────────
            dgv.BorderStyle          = BorderStyle.None;
            dgv.GridColor            = Color.FromArgb(220, 225, 230);
            dgv.BackgroundColor      = Color.White;
            dgv.RowHeadersVisible    = false;
            dgv.AllowUserToAddRows   = false;
            dgv.CellBorderStyle      = DataGridViewCellBorderStyle.SingleHorizontal;

            dgv.EnableHeadersVisualStyles                            = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor             = ThemeConfig.DgvHeader;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor             = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font                  = ThemeConfig.FontBold;
            dgv.ColumnHeadersDefaultCellStyle.Alignment             = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Padding               = new Padding(6, 0, 6, 0);
            dgv.ColumnHeadersHeight                                  = 38;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgv.DefaultCellStyle.Font              = ThemeConfig.FontDefault;
            dgv.DefaultCellStyle.ForeColor         = ThemeConfig.TextDark;
            dgv.DefaultCellStyle.BackColor         = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor= ThemeConfig.DgvSelect;
            dgv.DefaultCellStyle.SelectionForeColor= Color.White;
            dgv.DefaultCellStyle.Padding           = new Padding(6, 3, 6, 3);
            dgv.RowTemplate.Height                 = 32;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = ThemeConfig.DgvRowAlt;

            AttachDgvHover(dgv);
        }

        private static void StyleGuna2DataGridView(Guna2DataGridView dgv)
        {
            dgv.BackgroundColor    = Color.White;
            dgv.BorderStyle        = BorderStyle.None;
            dgv.RowHeadersVisible  = false;
            dgv.AllowUserToAddRows = false;
            dgv.CellBorderStyle    = DataGridViewCellBorderStyle.SingleHorizontal;

            // Guna ThemeStyle
            dgv.ThemeStyle.BackColor  = Color.White;
            dgv.ThemeStyle.GridColor  = Color.FromArgb(225, 228, 232);
            dgv.ThemeStyle.ReadOnly   = false;

            dgv.ThemeStyle.HeaderStyle.BackColor  = ThemeConfig.DgvHeader;
            dgv.ThemeStyle.HeaderStyle.ForeColor  = Color.White;
            dgv.ThemeStyle.HeaderStyle.Font       = ThemeConfig.FontBold;
            dgv.ThemeStyle.HeaderStyle.Height     = 38;

            dgv.ThemeStyle.RowsStyle.BackColor          = Color.White;
            dgv.ThemeStyle.RowsStyle.ForeColor          = ThemeConfig.TextDark;
            dgv.ThemeStyle.RowsStyle.Font               = ThemeConfig.FontDefault;
            dgv.ThemeStyle.RowsStyle.Height             = 32;
            dgv.ThemeStyle.RowsStyle.SelectionBackColor = ThemeConfig.DgvSelect;
            dgv.ThemeStyle.RowsStyle.SelectionForeColor = Color.White;

            dgv.ThemeStyle.AlternatingRowsStyle.BackColor          = ThemeConfig.DgvRowAlt;
            dgv.ThemeStyle.AlternatingRowsStyle.ForeColor          = ThemeConfig.TextDark;
            dgv.ThemeStyle.AlternatingRowsStyle.Font               = ThemeConfig.FontDefault;
            dgv.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = ThemeConfig.DgvSelect;
            dgv.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.White;

            // Override default cell style for alignment và padding
            dgv.DefaultCellStyle.Padding             = new Padding(6, 3, 6, 3);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            AttachDgvHover(dgv);
        }

        private static void AttachDgvHover(DataGridView dgv)
        {
            dgv.CellMouseEnter += (s, e) =>
            {
                var g = (DataGridView)s;
                if (e.RowIndex < 0 || e.RowIndex >= g.Rows.Count) return;
                var row = g.Rows[e.RowIndex];
                if (!row.Selected) row.DefaultCellStyle.BackColor = ThemeConfig.DgvHover;
            };
            dgv.CellMouseLeave += (s, e) =>
            {
                var g = (DataGridView)s;
                if (e.RowIndex < 0 || e.RowIndex >= g.Rows.Count) return;
                var row = g.Rows[e.RowIndex];
                if (!row.Selected)
                    row.DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? Color.White : ThemeConfig.DgvRowAlt;
            };
        }

        // ══════════════════════════════════════════════════════════════════
        // MenuStrip
        // ══════════════════════════════════════════════════════════════════

        public static void StyleMenuStrip(MenuStrip menu)
        {
            menu.BackColor  = ThemeConfig.PrimaryDark;
            menu.ForeColor  = Color.White;
            menu.Font       = ThemeConfig.FontBold;
            menu.Padding    = new Padding(4, 2, 0, 2);
            menu.Dock       = DockStyle.Top;
            // Đặt RenderMode = Custom TRƯỚC khi gán Renderer
            // để Windows 11 không override bằng visual styles mặc định.
            menu.RenderMode = ToolStripRenderMode.Professional; // 🔥 bắt buộc
            menu.Renderer   = new DarkMenuRenderer();
            EnableOptimizedRendering(menu);

            foreach (ToolStripItem item in menu.Items)
            {
                item.ForeColor = Color.White;
                item.Font      = ThemeConfig.FontBold;
                if (item is ToolStripMenuItem mi)
                    StyleDropDownChildren(mi);
            }
        }

        private static void StyleDropDownChildren(ToolStripMenuItem parent)
        {
            foreach (ToolStripItem item in parent.DropDownItems)
            {
                item.ForeColor = ThemeConfig.TextDark;
                item.Font      = ThemeConfig.FontDefault;
                if (item is ToolStripMenuItem child)
                    StyleDropDownChildren(child);
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // StatusStrip
        // ══════════════════════════════════════════════════════════════════

        public static void StyleStatusStrip(StatusStrip ss)
        {
            ss.BackColor  = ThemeConfig.PrimaryMid;
            ss.ForeColor  = Color.White;
            ss.Font       = ThemeConfig.FontSmall;
            ss.SizingGrip = false;
            EnableOptimizedRendering(ss);
            foreach (ToolStripItem item in ss.Items)
            {
                item.ForeColor = Color.White;
                item.Font      = ThemeConfig.FontSmall;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // TextBox / Label helpers
        // ══════════════════════════════════════════════════════════════════

        public static void StyleTextBox(TextBox txt)
        {
            txt.Font        = ThemeConfig.FontDefault;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor   = Color.White;
            txt.ForeColor   = ThemeConfig.TextDark;
        }

        public static void StyleLabel(Label lbl, bool bold = false)
        {
            lbl.Font      = bold ? ThemeConfig.FontBold : ThemeConfig.FontDefault;
            lbl.ForeColor = ThemeConfig.TextDark;
        }

        // ══════════════════════════════════════════════════════════════════
        // Auto-theme toàn Form (gọi từ OpenChildForm)
        // ══════════════════════════════════════════════════════════════════

        public static void ApplyFormTheme(Form form)
        {
            if (_styled.Contains(form)) return;
            _styled.Add(form);
            form.Disposed += (s, e) => _styled.Remove((Form)s);

            form.BackColor = ThemeConfig.BgMain;
            form.Font      = ThemeConfig.FontDefault;
            EnableOptimizedRendering(form);

            ApplyThemeRecursive(form.Controls);
        }

        private static void ApplyThemeRecursive(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                switch (c)
                {
                    case DataGridView dgv:
                        StyleDataGridView(dgv);
                        break;

                    case Guna2Button gBtn:
                        // Guna2Button: style via native API (skip FlatStyle)
                        StyleGuna2ButtonByText(gBtn);
                        break;

                    case Button btn:
                        StyleButtonByText(btn);
                        break;

                    case Panel pnl:
                        if (pnl.BackColor == SystemColors.Control || pnl.BackColor == default(Color))
                            pnl.BackColor = Color.White;
                        EnableOptimizedRendering(pnl);
                        ApplyThemeRecursive(pnl.Controls);
                        break;

                    case Label lbl:
                        if (lbl.Font?.Bold == false) lbl.Font = ThemeConfig.FontDefault;
                        break;

                    case TextBox txt:
                        StyleTextBox(txt);
                        break;

                    case GroupBox gb:
                        gb.Font = ThemeConfig.FontBold;
                        ApplyThemeRecursive(gb.Controls);
                        break;

                    case TabControl tab:
                        tab.Font = ThemeConfig.FontDefault;
                        foreach (TabPage page in tab.TabPages)
                            ApplyThemeRecursive(page.Controls);
                        break;

                    default:
                        if (c.HasChildren) ApplyThemeRecursive(c.Controls);
                        break;
                }
            }
        }

        private static void StyleButtonByText(Button btn)
        {
            Color back, hover;
            ResolveButtonColor(btn.Text, out back, out hover);
            StyleButton(btn, back, hover);
        }

        private static void StyleGuna2ButtonByText(Guna2Button btn)
        {
            Color back, hover;
            ResolveButtonColor(btn.Text, out back, out hover);
            btn.FillColor            = back;
            btn.HoverState.FillColor = hover;
            btn.ForeColor            = Color.White;
            btn.Font                 = ThemeConfig.FontBold;
            btn.Cursor               = Cursors.Hand;
        }

        private static void ResolveButtonColor(string text, out Color back, out Color hover)
        {
            var t = (text ?? "").Trim().ToLowerInvariant();

            if (t.Contains("thêm") || t.Contains("them") || t == "add")
            { back = ThemeConfig.BtnAdd;    hover = ThemeConfig.BtnAddHover; }
            else if (t.Contains("sửa") || t.Contains("cập nhật") || t.Contains("lưu")
                  || t == "edit" || t == "save")
            { back = ThemeConfig.BtnEdit;   hover = ThemeConfig.BtnEditHover; }
            else if (t.Contains("xóa") || t.Contains("xoa") || t.Contains("thôi học")
                  || t == "delete" || t == "remove")
            { back = ThemeConfig.BtnDelete; hover = ThemeConfig.BtnDeleteHover; }
            else if (t.Contains("tìm") || t.Contains("tim") || t.Contains("search")
                  || t.Contains("lọc"))
            { back = ThemeConfig.BtnSearch; hover = ThemeConfig.BtnSearchHover; }
            else if (t.Contains("xuất") || t.Contains("excel") || t.Contains("export")
                  || t.Contains("xem") || t.Contains("chi tiết"))
            { back = ThemeConfig.PrimaryLight; hover = ThemeConfig.BtnSearchHover; }
            else if (t.Contains("chuyển") || t.Contains("gán") || t.Contains("đăng ký"))
            { back = ThemeConfig.BtnEdit;   hover = ThemeConfig.BtnEditHover; }
            else if (t.Contains("thoát") || t.Contains("hủy") || t.Contains("đóng")
                  || t == "cancel" || t == "close" || t == "exit")
            { back = ThemeConfig.BtnNeutral; hover = ThemeConfig.BtnNeutralHover; }
            else
            { back = ThemeConfig.BtnNeutral; hover = ThemeConfig.BtnNeutralHover; }
        }

        // ══════════════════════════════════════════════════════════════════
        // Gradient paint helper
        // ══════════════════════════════════════════════════════════════════

        public static void PaintGradient(PaintEventArgs e, Color c1, Color c2,
                                         LinearGradientMode mode = LinearGradientMode.Vertical)
        {
            if (e.ClipRectangle.IsEmpty) return;
            using (var brush = new LinearGradientBrush(e.ClipRectangle, c1, c2, mode))
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
        }

        public static void EnableOptimizedRendering(Control control)
        {
            if (control == null) return;

            var setStyle = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            var updateStyles = typeof(Control).GetMethod("UpdateStyles", BindingFlags.Instance | BindingFlags.NonPublic);

            if (control is Form)
            {
                var styles = ControlStyles.UserPaint |
                             ControlStyles.AllPaintingInWmPaint |
                             ControlStyles.OptimizedDoubleBuffer |
                             ControlStyles.ResizeRedraw;
                setStyle?.Invoke(control, new object[] { styles, true });
                updateStyles?.Invoke(control, null);
                return;
            }

            var prop = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            prop?.SetValue(control, true, null);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Custom renderer cho MenuStrip tối
    // ══════════════════════════════════════════════════════════════════════

    public class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkMenuColorTable()) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var item = e.Item;
            var rect = new Rectangle(Point.Empty, item.Size);
            Color bg;

            if (item.IsOnDropDown)
                bg = item.Selected ? ThemeConfig.DgvHover : Color.White;
            else
                bg = (item.Selected || item.Pressed) ? ThemeConfig.PrimaryLight : ThemeConfig.PrimaryDark;

            using (var brush = new SolidBrush(bg))
                e.Graphics.FillRectangle(brush, rect);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(ThemeConfig.PrimaryDark))
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.IsOnDropDown ? ThemeConfig.TextDark : Color.White;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int midY = e.Item.Height / 2;
            using (var pen = new Pen(Color.FromArgb(220, 220, 220)))
                e.Graphics.DrawLine(pen, 30, midY, e.Item.Width - 4, midY);
        }
    }

    public class DarkMenuColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected              => ThemeConfig.DgvHover;
        public override Color MenuItemBorder                => Color.Transparent;
        public override Color MenuItemSelectedGradientBegin => ThemeConfig.DgvHover;
        public override Color MenuItemSelectedGradientEnd   => ThemeConfig.DgvHover;
        public override Color MenuBorder                    => Color.FromArgb(200, 200, 200);
        public override Color ToolStripDropDownBackground   => Color.White;
        public override Color ImageMarginGradientBegin      => Color.FromArgb(248, 249, 250);
        public override Color ImageMarginGradientMiddle     => Color.FromArgb(248, 249, 250);
        public override Color ImageMarginGradientEnd        => Color.FromArgb(248, 249, 250);
    }
}
