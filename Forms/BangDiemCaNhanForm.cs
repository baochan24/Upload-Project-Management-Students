using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class BangDiemCaNhanForm : Form
    {
        private readonly TextBox        txtMaSV;
        private readonly ComboBox       cmbHocKy;
        private readonly DataGridView   dgv;
        private readonly PrintDocument  _printDoc = new PrintDocument();
        private int                     _printPageIndex;

        public BangDiemCaNhanForm()
        {
            Text   = "Bảng điểm cá nhân";
            Width  = 1000;
            Height = 620;

            var panel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };

            // ── Mã SV ────────────────────────────────────────────────────
            panel.Controls.Add(new Label { Text = "Mã sinh viên", Top = 8, Left = 10, AutoSize = true });
            txtMaSV = new TextBox { Top = 26, Left = 10, Width = 140 };

            // Tự điền nếu user đang đăng nhập là sinh viên
            if (string.Equals(UserSession.RoleName, "SinhVien", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(UserSession.MaNguoiDung))
            {
                txtMaSV.Text     = UserSession.MaNguoiDung;
                txtMaSV.ReadOnly = true;
            }

            panel.Controls.Add(txtMaSV);

            // ── Học kỳ ───────────────────────────────────────────────────
            panel.Controls.Add(new Label { Text = "Học kỳ (bỏ trống = tất cả)", Top = 8, Left = 165, AutoSize = true });
            cmbHocKy = new ComboBox
            {
                Top           = 26, Left = 165, Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panel.Controls.Add(cmbHocKy);

            // ── Nút ──────────────────────────────────────────────────────
            var btnLoad = new Button { Text = "Tải điểm", Top = 24, Left = 445, Width = 90 };
            btnLoad.Click += BtnLoad_Click;

            var btnExport = new Button { Text = "Xuất CSV", Top = 24, Left = 545, Width = 90 };
            btnExport.Click += BtnExport_Click;

            var btnPrint = new Button { Text = "In bảng điểm", Top = 24, Left = 645, Width = 110 };
            btnPrint.Click += BtnPrint_Click;

            panel.Controls.AddRange(new Control[] { btnLoad, btnExport, btnPrint });

            // ── DataGridView ──────────────────────────────────────────────
            dgv = new DataGridView
            {
                Dock                 = DockStyle.Fill,
                ReadOnly             = true,
                AutoSizeColumnsMode  = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode        = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows   = false,
                BackgroundColor      = Color.White
            };

            Controls.Add(panel);
            Controls.Add(dgv);

            _printDoc.PrintPage += PrintDoc_PrintPage;
            Load += BangDiemCaNhanForm_Load;
        }

        private void BangDiemCaNhanForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Tải danh sách học kỳ vào ComboBox
                var dtHocKy = HocKyBUS.LoadAll();
                cmbHocKy.DisplayMember = "TenHocKy";
                cmbHocKy.ValueMember   = "MaHocKy";

                // Thêm dòng "Tất cả" đầu tiên
                var dtWithAll = dtHocKy.Copy();
                var allRow    = dtWithAll.NewRow();
                allRow["MaHocKy"]  = DBNull.Value;
                allRow["TenHocKy"] = "(Tất cả học kỳ)";
                dtWithAll.Rows.InsertAt(allRow, 0);

                cmbHocKy.DataSource    = dtWithAll;
                cmbHocKy.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách học kỳ:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            var maSV = txtMaSV.Text.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(maSV))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maHocKy = null;
            if (cmbHocKy.SelectedValue != null && !(cmbHocKy.SelectedValue is DBNull))
                maHocKy = cmbHocKy.SelectedValue.ToString();

            try
            {
                var dt = BaoCaoBUS.LayDiemSinhVien(maSV, maHocKy);
                dgv.DataSource = dt;

                if (dt.Rows.Count == 0)
                    MessageBox.Show("Không tìm thấy dữ liệu điểm cho sinh viên này.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để in.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var preview = new PrintPreviewDialog { Document = _printDoc, Width = 900, Height = 700 })
            {
                _printPageIndex = 0;
                preview.ShowDialog();
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var g      = e.Graphics;
            var margin = e.MarginBounds;
            var fontTitle = new Font("Arial", 13, FontStyle.Bold);
            var fontHead  = new Font("Arial", 9,  FontStyle.Bold);
            var fontCell  = new Font("Arial", 9,  FontStyle.Regular);
            var brush     = Brushes.Black;

            float y = margin.Top;

            // Tiêu đề
            string title = $"BẢNG ĐIỂM CÁ NHÂN - Sinh viên: {txtMaSV.Text.Trim().ToUpperInvariant()}";
            g.DrawString(title, fontTitle, brush, margin.Left, y);
            y += fontTitle.GetHeight(g) + 6;

            // Tính độ rộng cột đều nhau
            int colCount  = dgv.Columns.Count;
            float colWidth = (float)margin.Width / colCount;
            float rowH    = fontHead.GetHeight(g) + 4;

            // Header
            for (int c = 0; c < colCount; c++)
            {
                var rect = new RectangleF(margin.Left + c * colWidth, y, colWidth, rowH);
                g.FillRectangle(Brushes.LightSteelBlue, rect);
                g.DrawRectangle(Pens.Gray, rect.X, rect.Y, rect.Width, rect.Height);
                g.DrawString(dgv.Columns[c].HeaderText, fontHead, brush, rect.X + 2, rect.Y + 2);
            }
            y += rowH;

            // Rows
            while (_printPageIndex < dgv.Rows.Count)
            {
                var row = dgv.Rows[_printPageIndex];
                rowH = fontCell.GetHeight(g) + 4;

                if (y + rowH > margin.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                for (int c = 0; c < colCount; c++)
                {
                    var rect = new RectangleF(margin.Left + c * colWidth, y, colWidth, rowH);
                    g.DrawRectangle(Pens.LightGray, rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(row.Cells[c].Value?.ToString() ?? "", fontCell, brush, rect.X + 2, rect.Y + 2);
                }

                y += rowH;
                _printPageIndex++;
            }

            e.HasMorePages = false;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new SaveFileDialog
            {
                Filter   = "CSV files (*.csv)|*.csv",
                FileName = $"BangDiem_{txtMaSV.Text.Trim()}_{DateTime.Now:yyyyMMdd}.csv"
            })
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        // Header
                        var headers = new System.Collections.Generic.List<string>();
                        foreach (DataGridViewColumn col in dgv.Columns)
                            headers.Add($"\"{col.HeaderText}\"");
                        writer.WriteLine(string.Join(",", headers));

                        // Rows
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            var cells = new System.Collections.Generic.List<string>();
                            foreach (DataGridViewCell cell in row.Cells)
                                cells.Add($"\"{cell.Value?.ToString()?.Replace("\"", "\"\"")}\"");
                            writer.WriteLine(string.Join(",", cells));
                        }
                    }

                    MessageBox.Show($"Đã xuất thành công:\n{dialog.FileName}",
                        "Xuất CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Xuất file thất bại:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
