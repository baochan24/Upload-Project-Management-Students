using System;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Forms
{
    public class ChuyenLopDialog : Form
    {
        private readonly string _maSV;
        private readonly ComboBox cmbLopMoi;
        private readonly TextBox  txtLyDo;
        private readonly TextBox  txtNguoiDuyet;

        public ChuyenLopDialog(string maSV)
        {
            _maSV = maSV;
            Text            = $"Chuyển lớp – {maSV}";
            Width           = 420;
            Height          = 280;
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;

            int lbl = 12, tb = 30, gap = 55;

            // ── Lớp mới ────────────────────────────────────────────────
            Controls.Add(new Label { Text = "Lớp sinh hoạt mới *", Top = lbl, Left = 12, AutoSize = true });
            cmbLopMoi = new ComboBox
            {
                Top           = tb,  Left = 12, Width = 370,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            Controls.Add(cmbLopMoi);

            // ── Lý do ──────────────────────────────────────────────────
            Controls.Add(new Label { Text = "Lý do chuyển lớp", Top = lbl + gap, Left = 12, AutoSize = true });
            txtLyDo = new TextBox { Top = tb + gap, Left = 12, Width = 370 };
            Controls.Add(txtLyDo);

            // ── Người duyệt ────────────────────────────────────────────
            Controls.Add(new Label { Text = "Người duyệt", Top = lbl + gap * 2, Left = 12, AutoSize = true });
            txtNguoiDuyet = new TextBox
            {
                Top  = tb + gap * 2, Left = 12, Width = 370,
                Text = UserSession.Username ?? string.Empty
            };
            Controls.Add(txtNguoiDuyet);

            // ── Nút ────────────────────────────────────────────────────
            var btnOk = new Button
            {
                Text         = "Xác nhận",
                DialogResult = DialogResult.None,
                Width        = 100, Height = 28,
                Top          = tb + gap * 3, Left = 160
            };
            var btnCancel = new Button
            {
                Text         = "Hủy",
                DialogResult = DialogResult.Cancel,
                Width        = 80,  Height = 28,
                Top          = tb + gap * 3, Left = 278
            };
            btnOk.Click     += BtnOk_Click;
            Controls.AddRange(new Control[] { btnOk, btnCancel });

            Load += ChuyenLopDialog_Load;
        }

        private void ChuyenLopDialog_Load(object sender, EventArgs e)
        {
            try
            {
                cmbLopMoi.DisplayMember = "MaLopSH";
                cmbLopMoi.ValueMember   = "MaLopSH";
                cmbLopMoi.DataSource    = CommonDAL.LoadLookup("sp_LayDanhSachLopSinhHoat");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách lớp:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            var maLopMoi    = cmbLopMoi.SelectedValue?.ToString();
            var lyDo        = txtLyDo.Text.Trim();
            var nguoiDuyet  = txtNguoiDuyet.Text.Trim();

            if (string.IsNullOrWhiteSpace(maLopMoi))
            {
                MessageBox.Show("Vui lòng chọn lớp mới.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var r = SinhVienBUS.ChuyenLop(
                _maSV,
                maLopMoi,
                string.IsNullOrWhiteSpace(lyDo)       ? null : lyDo,
                string.IsNullOrWhiteSpace(nguoiDuyet) ? null : nguoiDuyet
            );

            MessageBox.Show(r.Message,
                r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (r.Success)
                DialogResult = DialogResult.OK;
        }
    }
}
