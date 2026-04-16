using System;
using System.Data;
using System.Windows.Forms;
using QuanLySinhVien.BUS;
using QuanLySinhVien.DAL;

namespace QuanLySinhVien.Forms
{
    public class HocPhiForm : Form
    {
        private readonly DataGridView dgvDetail, dgvSummary;
        private readonly ComboBox cmbMaHocKy;
        private readonly Label lblTongHop;

        public HocPhiForm()
        {
            Text = "Báo cáo học phí";
            Width = 1100;
            Height = 750;

            // Top panel: filter
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(10) };
            panelTop.Controls.Add(new Label { Text = "Học kỳ", Top = 10, Left = 10, AutoSize = true });
            cmbMaHocKy = new ComboBox { Top = 28, Left = 10, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            panelTop.Controls.Add(cmbMaHocKy);

            var btnLoad = new Button { Text = "Xem báo cáo", Top = 26, Left = 285, Width = 120, Height = 30 };
            btnLoad.Click += BtnLoad_Click;
            panelTop.Controls.Add(btnLoad);

            lblTongHop = new Label { Text = "", Top = 33, Left = 425, AutoSize = true, ForeColor = System.Drawing.Color.DarkBlue };
            panelTop.Controls.Add(lblTongHop);

            // Summary panel (bottom)
            var panelSummary = new Panel { Dock = DockStyle.Bottom, Height = 80 };
            dgvSummary = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            panelSummary.Controls.Add(dgvSummary);

            var lblSum = new Label { Dock = DockStyle.Top, Text = "Tổng hợp học phí:", Height = 22,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) };
            panelSummary.Controls.Add(lblSum);

            // Detail grid
            var panelDetail = new Panel { Dock = DockStyle.Fill };
            dgvDetail = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            var lblDetail = new Label { Dock = DockStyle.Top, Text = "Chi tiết học phí sinh viên:", Height = 22,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) };
            panelDetail.Controls.Add(dgvDetail);
            panelDetail.Controls.Add(lblDetail);

            Controls.Add(panelDetail);
            Controls.Add(panelSummary);
            Controls.Add(panelTop);

            Load += (s, e) => LoadHocKyLookup();
        }

        private void LoadHocKyLookup()
        {
            var dt = CommonDAL.LoadLookup("sp_LayDanhSachHocKy");
            cmbMaHocKy.DataSource    = dt;
            cmbMaHocKy.DisplayMember = "TenHocKy";
            cmbMaHocKy.ValueMember   = "MaHocKy";
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            var maHocKy = cmbMaHocKy.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(maHocKy))
            { MessageBox.Show("Vui lòng chọn học kỳ.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                var ds = HocPhiBUS.LoadReport(maHocKy);

                dgvDetail.DataSource  = ds.Tables.Count > 0 ? ds.Tables[0] : null;
                dgvSummary.DataSource = ds.Tables.Count > 1 ? ds.Tables[1] : null;

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    var row = ds.Tables[1].Rows[0];
                    lblTongHop.Text =
                        $"Tổng SV: {row["TongSinhVien"]}  |  " +
                        $"Tổng HP: {row["TongHocPhi"]:N0}đ  |  " +
                        $"Đã thu: {row["TongDaThu"]:N0}đ  |  " +
                        $"Còn nợ: {row["TongConNo"]:N0}đ  |  " +
                        $"Tỷ lệ thu: {row["TyLeThu"]}%";
                }
                else lblTongHop.Text = "Không có dữ liệu học phí cho học kỳ này.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
