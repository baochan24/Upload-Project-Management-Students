using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLySinhVien.BUS;

namespace QuanLySinhVien.Forms
{
    public class frmLogin : Form
    {
        private readonly GradientHeaderPanel pnlHeader;
        private readonly RoundedBodyPanel pnlBody;
        private readonly RoundedInputPanel pnlUsername;
        private readonly RoundedInputPanel pnlPassword;
        private readonly GradientButton btnLogin;
        private readonly Label btnClose;
        private readonly Label lblHeading;
        private readonly Label lblSubtext;
        private readonly Label lblUsername;
        private readonly Label lblPassword;
        private readonly TextBox txtUsername;
        private readonly TextBox txtPassword;
        private readonly TableLayoutPanel pnlBodyContent;
        private bool _dragging;
        private Point _dragCursorPoint;
        private Point _dragFormPoint;

        public frmLogin()
        {
            Text = "Đăng nhập";
            AutoSize = false;
            Size = new Size(460, 560);
            MinimumSize = new Size(460, 560);
            MaximumSize = new Size(460, 560);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            DoubleBuffered = true;

            pnlHeader = new GradientHeaderPanel
            {
                Dock = DockStyle.Top,
                Height = 130
            };

            btnClose = new Label
            {
                Text = "×",
                AutoSize = false,
                Size = new Size(28, 28),
                Location = new Point(pnlHeader.Width - 36, 8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 14f, FontStyle.Regular, GraphicsUnit.Point)
            };
            btnClose.Click += (s, e) => Close();

            pnlBody = new RoundedBodyPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                Padding = new Padding(30, 20, 30, 24)
            };

            lblHeading = new Label
            {
                Text = "Đăng nhập",
                AutoSize = true,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(37, 41, 56),
                Margin = new Padding(0, 0, 0, 8)
            };

            lblSubtext = new Label
            {
                Text = "Đăng nhập để truy cập hệ thống quản lý sinh viên và tiếp tục làm việc với dữ liệu của bạn.",
                AutoSize = true,
                Dock = DockStyle.Top,
                MaximumSize = new Size(400, 0),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(138, 145, 160),
                Margin = new Padding(0, 0, 0, 16)
            };

            lblUsername = CreateSectionLabel("TÊN ĐĂNG NHẬP");
            txtUsername = CreateInputTextBox("Nhập tên đăng nhập", false);
            pnlUsername = new RoundedInputPanel(InputIcon.User)
            {
                Dock = DockStyle.Top,
                Height = 42,
                Margin = new Padding(0, 0, 0, 16),
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            pnlUsername.SetInnerTextBox(txtUsername);

            lblPassword = CreateSectionLabel("MẬT KHẨU");
            txtPassword = CreateInputTextBox("Nhập mật khẩu", true);
            pnlPassword = new RoundedInputPanel(InputIcon.Lock)
            {
                Dock = DockStyle.Top,
                Height = 42,
                Margin = new Padding(0, 0, 0, 28),
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            pnlPassword.SetInnerTextBox(txtPassword);

            btnLogin = new GradientButton
            {
                Text = "ĐĂNG NHẬP",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 46,
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            pnlBodyContent = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Dock = DockStyle.Top,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            pnlBodyContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBodyContent.Controls.Add(lblHeading, 0, 0);
            pnlBodyContent.Controls.Add(lblSubtext, 0, 1);
            pnlBodyContent.Controls.Add(lblUsername, 0, 2);
            pnlBodyContent.Controls.Add(pnlUsername, 0, 3);
            pnlBodyContent.Controls.Add(lblPassword, 0, 4);
            pnlBodyContent.Controls.Add(pnlPassword, 0, 5);
            pnlBodyContent.Controls.Add(btnLogin, 0, 6);

            pnlHeader.Controls.Add(btnClose);
            pnlBody.Controls.Add(pnlBodyContent);

            Controls.Add(pnlBody);
            Controls.Add(pnlHeader);

            Load += FrmLogin_Load;
            Shown += (s, e) => txtUsername.Focus();
            Resize += FrmLogin_Resize;

            txtUsername.KeyDown += TxtUsername_KeyDown;
            txtPassword.KeyDown += TxtPassword_KeyDown;

            HookDrag(this);
            HookDrag(pnlHeader);
            HookDrag(pnlBody);
            HookDrag(lblHeading);
            HookDrag(lblSubtext);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int csDropShadow = 0x00020000;
                var createParams = base.CreateParams;
                createParams.ClassStyle |= csDropShadow;
                return createParams;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < 10; i++)
            {
                var alpha = Math.Max(6, 32 - (i * 3));
                using (var path = CreateRoundedPath(new Rectangle(6 + i, 6 + i, Width - 12 - (i * 2), Height - 12 - (i * 2)), 26))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 34, 28, 69)))
                {
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(alpha, 34, 28, 69), 1f), path);
                }
            }
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            UpdateLayoutMetrics();
            ApplyFormRegion();
        }

        private void FrmLogin_Resize(object sender, EventArgs e)
        {
            UpdateLayoutMetrics();
            ApplyFormRegion();
        }

        private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            txtPassword.Focus();
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            BtnLogin_Click(sender, EventArgs.Empty);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var username = GetRealText(txtUsername);
                var password = GetRealText(txtPassword);
                var result = AuthBUS.Login(username.Trim(), password, out DataRow userInfo);

                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AuthBUS.SetUserSession(userInfo);
                Hide();

                using (var mainForm = new MainForm())
                {
                    mainForm.ShowDialog();
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFormRegion()
        {
            using (var path = CreateRoundedPath(new Rectangle(0, 0, Width, Height), 28))
            {
                Region = new Region(path);
            }
        }

        private void UpdateLayoutMetrics()
        {
            pnlHeader.Width = ClientSize.Width;
            pnlBody.Width = ClientSize.Width;
            pnlBodyContent.Width = ClientSize.Width - pnlBody.Padding.Left - pnlBody.Padding.Right;

            lblSubtext.MaximumSize = new Size(pnlBodyContent.Width, 0);
            lblHeading.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            lblSubtext.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            lblUsername.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            lblPassword.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            pnlUsername.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            pnlPassword.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            btnLogin.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            txtUsername.Width = pnlUsername.Width - 58;
            txtPassword.Width = pnlPassword.Width - 58;

            pnlBody.Height = pnlBody.PreferredSize.Height;
            ClientSize = new Size(460, pnlHeader.Height + pnlBody.Height);
        }

        private static Label CreateSectionLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(146, 151, 166),
                Margin = new Padding(0, 0, 0, 8)
            };
        }

        private TextBox CreateInputTextBox(string placeholder, bool isPassword)
        {
            var textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(157, 163, 177),
                BackColor = Color.White,
                Location = new Point(48, 12),
                Width = 320,
                Tag = placeholder
            };

            textBox.Text = placeholder;
            textBox.Enter += TextBox_Enter;
            textBox.Leave += TextBox_Leave;

            if (isPassword)
            {
                textBox.Tag = placeholder + "|password";
            }

            return textBox;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var placeholder = GetPlaceholder(textBox);

            if (textBox.Text == placeholder && IsPlaceholderState(textBox))
            {
                textBox.Text = string.Empty;
                textBox.ForeColor = Color.FromArgb(45, 49, 66);
                if (IsPasswordTextBox(textBox))
                {
                    textBox.PasswordChar = '●';
                }
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                return;
            }

            textBox.PasswordChar = '\0';
            textBox.Text = GetPlaceholder(textBox);
            textBox.ForeColor = Color.FromArgb(157, 163, 177);
        }

        private static bool IsPlaceholderState(TextBox textBox)
        {
            return textBox.ForeColor == Color.FromArgb(157, 163, 177);
        }

        private static bool IsPasswordTextBox(TextBox textBox)
        {
            return Convert.ToString(textBox.Tag).EndsWith("|password", StringComparison.Ordinal);
        }

        private static string GetPlaceholder(TextBox textBox)
        {
            var rawTag = Convert.ToString(textBox.Tag);
            var splitIndex = rawTag.IndexOf('|');
            return splitIndex >= 0 ? rawTag.Substring(0, splitIndex) : rawTag;
        }

        private static string GetRealText(TextBox textBox)
        {
            return IsPlaceholderState(textBox) ? string.Empty : textBox.Text;
        }

        private void HookDrag(Control control)
        {
            control.MouseDown += Drag_MouseDown;
            control.MouseMove += Drag_MouseMove;
            control.MouseUp += Drag_MouseUp;
        }

        private void Drag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            _dragging = true;
            _dragCursorPoint = Cursor.Position;
            _dragFormPoint = Location;
        }

        private void Drag_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging)
            {
                return;
            }

            var difference = new Point(Cursor.Position.X - _dragCursorPoint.X, Cursor.Position.Y - _dragCursorPoint.Y);
            Location = new Point(_dragFormPoint.X + difference.X, _dragFormPoint.Y + difference.Y);
        }

        private void Drag_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private static GraphicsPath CreateTopRoundedPath(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddLine(bounds.Right, bounds.Bottom, bounds.X, bounds.Bottom);
            path.CloseFigure();

            return path;
        }

        private static GraphicsPath CreateBottomRoundedPath(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var path = new GraphicsPath();

            path.StartFigure();
            path.AddLine(bounds.X, bounds.Y, bounds.Right, bounds.Y);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private enum InputIcon
        {
            User,
            Lock
        }

        private sealed class GradientHeaderPanel : Panel
        {
            public GradientHeaderPanel()
            {
                DoubleBuffered = true;
                Resize += (s, e) => ApplyRegion();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var path = CreateTopRoundedPath(new Rectangle(0, 0, Width, Height), 28))
                using (var brush = new LinearGradientBrush(e.ClipRectangle, Color.FromArgb(108, 99, 255), Color.FromArgb(155, 89, 182), LinearGradientMode.Horizontal))
                {
                    var previousClip = e.Graphics.Clip;
                    e.Graphics.SetClip(path);
                    e.Graphics.FillRectangle(brush, e.ClipRectangle);
                    e.Graphics.Clip = previousClip;
                }

                DrawCapIcon(e.Graphics);

                using (var titleFont = new Font("Segoe UI", 16f, FontStyle.Bold, GraphicsUnit.Point))
                using (var subtitleFont = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point))
                using (var titleBrush = new SolidBrush(Color.White))
                using (var subtitleBrush = new SolidBrush(Color.FromArgb(240, 240, 248)))
                {
                    var centerFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    e.Graphics.DrawString("HỆ THỐNG QUẢN LÝ", titleFont, titleBrush, new RectangleF(0, 56, Width, 26), centerFormat);
                    e.Graphics.DrawString("SINH VIÊN", subtitleFont, subtitleBrush, new RectangleF(0, 84, Width, 20), centerFormat);
                }
            }

            private void ApplyRegion()
            {
                if (Width <= 0 || Height <= 0)
                {
                    return;
                }

                using (var path = CreateTopRoundedPath(new Rectangle(0, 0, Width, Height), 28))
                {
                    Region = new Region(path);
                }
            }

            private static void DrawCapIcon(Graphics graphics)
            {
                var iconBounds = new Rectangle((graphics.VisibleClipBounds.Width >= 60 ? ((int)graphics.VisibleClipBounds.Width - 60) / 2 : 0), 18, 60, 32);
                using (var pen = new Pen(Color.White, 2.2f))
                using (var brush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                {
                    var capPoints = new[]
                    {
                        new Point(iconBounds.Left + (iconBounds.Width / 2), iconBounds.Top),
                        new Point(iconBounds.Right, iconBounds.Top + 12),
                        new Point(iconBounds.Left + (iconBounds.Width / 2), iconBounds.Top + 24),
                        new Point(iconBounds.Left, iconBounds.Top + 12)
                    };

                    graphics.FillPolygon(brush, capPoints);
                    graphics.DrawPolygon(pen, capPoints);
                    graphics.DrawLine(pen, iconBounds.Left + 12, iconBounds.Top + 16, iconBounds.Right - 12, iconBounds.Top + 16);
                    graphics.DrawArc(pen, iconBounds.Left + 15, iconBounds.Top + 20, 30, 14, 0, 180);
                    graphics.DrawLine(pen, iconBounds.Right - 10, iconBounds.Top + 12, iconBounds.Right - 10, iconBounds.Top + 24);
                    graphics.FillEllipse(Brushes.White, iconBounds.Right - 12, iconBounds.Top + 23, 5, 5);
                }
            }
        }

        private sealed class RoundedBodyPanel : Panel
        {
            public RoundedBodyPanel()
            {
                DoubleBuffered = true;
                Resize += (s, e) => ApplyRegion();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var path = CreateBottomRoundedPath(new Rectangle(0, 0, Width, Height), 28))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }

            private void ApplyRegion()
            {
                if (Width <= 0 || Height <= 0)
                {
                    return;
                }

                using (var path = CreateBottomRoundedPath(new Rectangle(0, 0, Width, Height), 28))
                {
                    Region = new Region(path);
                }
            }
        }

        private sealed class RoundedInputPanel : Panel
        {
            private readonly InputIcon _iconType;

            public RoundedInputPanel(InputIcon iconType)
            {
                _iconType = iconType;
                DoubleBuffered = true;
                BackColor = Color.White;
                Resize += (s, e) => ApplyRegion();
            }

            public void SetInnerTextBox(TextBox textBox)
            {
                Controls.Add(textBox);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var path = CreateRoundedPath(new Rectangle(0, 0, Width - 1, Height - 1), 18))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(Color.FromArgb(221, 225, 235), 1f))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }

                if (_iconType == InputIcon.User)
                {
                    DrawUserIcon(e.Graphics);
                }
                else
                {
                    DrawLockIcon(e.Graphics);
                }
            }

            private void ApplyRegion()
            {
                if (Width <= 0 || Height <= 0)
                {
                    return;
                }

                using (var path = CreateRoundedPath(new Rectangle(0, 0, Width, Height), 18))
                {
                    Region = new Region(path);
                }
            }

            private static void DrawUserIcon(Graphics graphics)
            {
                using (var pen = new Pen(Color.FromArgb(140, 147, 163), 1.6f))
                {
                    graphics.DrawEllipse(pen, 16, 10, 10, 10);
                    graphics.DrawArc(pen, 11, 20, 20, 10, 200, 140);
                }
            }

            private static void DrawLockIcon(Graphics graphics)
            {
                using (var pen = new Pen(Color.FromArgb(140, 147, 163), 1.6f))
                using (var brush = new SolidBrush(Color.FromArgb(140, 147, 163)))
                {
                    graphics.DrawArc(pen, 14, 8, 14, 12, 200, 140);
                    graphics.DrawRectangle(pen, 13, 18, 16, 11);
                    graphics.FillEllipse(brush, 19, 21, 3, 3);
                }
            }
        }

        private sealed class GradientButton : Button
        {
            public GradientButton()
            {
                DoubleBuffered = true;
                FlatAppearance.BorderSize = 0;
                FlatStyle = FlatStyle.Flat;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var backgroundRect = new Rectangle(0, 0, Width - 1, Height - 1);

                using (var path = CreateRoundedPath(backgroundRect, 18))
                using (var brush = new LinearGradientBrush(backgroundRect, Color.FromArgb(108, 99, 255), Color.FromArgb(155, 89, 182), LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillPath(brush, path);
                }

                using (var textBrush = new SolidBrush(ForeColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    e.Graphics.DrawString(Text, Font, textBrush, backgroundRect, format);
                }
            }
        }
    }

    public class LoginForm : frmLogin
    {
    }
}
