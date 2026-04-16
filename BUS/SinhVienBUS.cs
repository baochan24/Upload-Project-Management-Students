using System;
using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class SinhVienBUS
    {
        public static DataTable LoadAll(string keyword = null, string maLopSH = null, string maKhoa = null, string maNganh = null, string tinhTrang = null)
        {
            return SinhVienDAL.Search(keyword, maLopSH, maKhoa, maNganh, tinhTrang);
        }

        public static OperationResult Add(string maSV, string hoTen, DateTime? ngaySinh, bool? gioiTinh, string diaChi, string maLopSH, string anhDaiDien)
        {
            if (string.IsNullOrWhiteSpace(maSV) || string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(maLopSH))
                return OperationResult.Fail("Mã SV, họ tên và lớp sinh hoạt là bắt buộc.");

            if (maSV.Length > 10)
                return OperationResult.Fail("Mã sinh viên tối đa 10 ký tự.");

            return SinhVienDAL.Add(maSV.Trim().ToUpperInvariant(), hoTen.Trim(), ngaySinh, gioiTinh, diaChi?.Trim(), maLopSH.Trim(), anhDaiDien?.Trim());
        }

        public static OperationResult Update(string maSV, string hoTen, DateTime? ngaySinh, bool? gioiTinh, string diaChi, string anhDaiDien)
        {
            if (string.IsNullOrWhiteSpace(maSV) || string.IsNullOrWhiteSpace(hoTen))
                return OperationResult.Fail("Mã SV và họ tên là bắt buộc.");

            return SinhVienDAL.Update(maSV.Trim().ToUpperInvariant(), hoTen.Trim(), ngaySinh, gioiTinh, diaChi?.Trim(), anhDaiDien?.Trim());
        }

        public static OperationResult Delete(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                return OperationResult.Fail("Mã sinh viên là bắt buộc.");

            return SinhVienDAL.Delete(maSV.Trim().ToUpperInvariant());
        }
    }
}
