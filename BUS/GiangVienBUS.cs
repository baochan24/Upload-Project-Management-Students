using System;
using System.Data;
using QuanLySinhVien.DAL;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.BUS
{
    public static class GiangVienBUS
    {
        public static DataTable LoadAll(string keyword = null) => GiangVienDAL.LoadAll(keyword);

        public static OperationResult Add(string maGV, string hoTen, string maKhoa,
            string email, string soDienThoai, DateTime? ngaySinh, bool? gioiTinh,
            string hocVi, string hocHam)
        {
            if (string.IsNullOrWhiteSpace(maGV) || string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(maKhoa))
                return OperationResult.Fail("Mã giảng viên, họ tên và khoa là bắt buộc.");
            return GiangVienDAL.Add(maGV.Trim().ToUpperInvariant(), hoTen.Trim(), maKhoa.Trim().ToUpperInvariant(),
                string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai.Trim(),
                ngaySinh, gioiTinh,
                string.IsNullOrWhiteSpace(hocVi) ? null : hocVi.Trim(),
                string.IsNullOrWhiteSpace(hocHam) ? null : hocHam.Trim());
        }

        public static OperationResult Update(string maGV, string hoTen, string maKhoa,
            string email, string soDienThoai, DateTime? ngaySinh, bool? gioiTinh,
            string hocVi, string hocHam)
        {
            if (string.IsNullOrWhiteSpace(maGV) || string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(maKhoa))
                return OperationResult.Fail("Mã giảng viên, họ tên và khoa là bắt buộc.");
            return GiangVienDAL.Update(maGV.Trim().ToUpperInvariant(), hoTen.Trim(), maKhoa.Trim().ToUpperInvariant(),
                string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai.Trim(),
                ngaySinh, gioiTinh,
                string.IsNullOrWhiteSpace(hocVi) ? null : hocVi.Trim(),
                string.IsNullOrWhiteSpace(hocHam) ? null : hocHam.Trim());
        }

        public static OperationResult Delete(string maGV)
        {
            if (string.IsNullOrWhiteSpace(maGV))
                return OperationResult.Fail("Mã giảng viên là bắt buộc.");
            return GiangVienDAL.Delete(maGV.Trim().ToUpperInvariant());
        }
    }
}
