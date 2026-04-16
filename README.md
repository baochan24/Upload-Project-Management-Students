 HỆ THỐNG QUẢN LÝ SINH VIÊN
Đồ án môn học: Xây dựng hệ thống quản lý sinh viên với C# Winform và SQL Server

📌 GIỚI THIỆU
Hệ thống Quản lý Sinh viên là một ứng dụng quản lý giáo dục toàn diện, được xây dựng nhằm hỗ trợ các cơ sở đào tạo quản lý sinh viên, môn học, học phần, điểm số, học phí và các hoạt động học vụ khác.

Hệ thống được phát triển với mục tiêu:

Đơn giản, dễ sử dụng với giao diện Winform thân thiện

Đầy đủ chức năng đáp ứng 45+ nghiệp vụ quản lý đào tạo

An toàn dữ liệu với phân quyền chi tiết và ghi nhật ký hoạt động

👥 ACTORS CỦA HỆ THỐNG
Actor	Vai trò	Quyền hạn chính
Admin	Quản trị hệ thống	Toàn quyền, phân quyền, quản lý người dùng
Phòng Đào tạo	Quản lý học vụ	Quản lý danh mục, mở lớp, báo cáo
Giảng viên	Giảng dạy	Nhập điểm, xem lớp dạy, in bảng điểm
Sinh viên	Người học	Đăng ký học phần, xem điểm, TKB
🚀 TÍNH NĂNG CHÍNH
1. Quản trị hệ thống
Đăng nhập / Đăng xuất / Đổi mật khẩu

Phân quyền chi tiết (4 vai trò: Admin, Phòng ĐT, Giảng viên, Sinh viên)

Quản lý người dùng (thêm, sửa, xóa, khóa tài khoản)

2. Quản lý danh mục
Quản lý Khoa, Ngành học, Khóa học

Quản lý Lớp sinh hoạt

Quản lý Môn học (mã môn, tên, tín chỉ, môn tiên quyết)

Quản lý Học kỳ (ngày bắt đầu/kết thúc, giới hạn tín chỉ)

Quản lý Giảng viên (hồ sơ, học vị, học hàm)

Quản lý Phòng học (sức chứa, vị trí)

3. Quản lý sinh viên
Thêm, sửa, xóa sinh viên (xóa mềm)

Xem danh sách, tìm kiếm theo MSSV/họ tên/lớp

Xem chi tiết hồ sơ (có ảnh đại diện)

Chuyển lớp/chuyển ngành (có lưu lịch sử)

Import/Export Excel

4. Quản lý học phần & đăng ký
Mở lớp học phần (phân công giảng viên, phòng học, sĩ số)

Đăng ký học phần (kiểm tra sĩ số, trùng lịch, giới hạn tín chỉ)

Xem thời khóa biểu cá nhân

Hủy đăng ký học phần

Quản lý đăng ký (Admin hỗ trợ sinh viên)

5. Quản lý điểm
Nhập điểm (chuyên cần, giữa kỳ, cuối kỳ)

Tự động tính điểm tổng kết và xếp loại (theo tỷ lệ 10%-30%-60%)

Xem bảng điểm cá nhân / theo lớp

Sửa điểm (có lưu lịch sử thay đổi)

Xác nhận / khóa điểm (không cho sửa sau khi khóa)

6. Báo cáo & thống kê
Thống kê số lượng sinh viên theo khoa, lớp, khóa

Thống kê kết quả học tập (tỷ lệ Giỏi/Khá/TB/Yếu)

Xuất Excel danh sách sinh viên, bảng điểm

In bảng điểm cá nhân

Báo cáo học phí (tổng thu, còn nợ)

Dashboard tổng quan với biểu đồ trực quan

🛠 CÔNG NGHỆ SỬ DỤNG
Thành phần	Công nghệ
Ngôn ngữ lập trình	C# .NET Framework 4.7.2 / .NET 6+
Giao diện	Windows Forms (Winform)
Cơ sở dữ liệu	Microsoft SQL Server 2019+
Kết nối DB	ADO.NET (SqlClient)
Xuất Excel	EPPlus / Microsoft.Office.Interop.Excel
Báo cáo	Crystal Reports / RDLC (tùy chọn)
Version control	Git + GitHub
📁 CẤU TRÚC PROJECT
text
QuanLySinhVien.sln
│
├── QuanLySinhVien.DTO/          # Đối tượng dữ liệu (Models)
│   ├── UserDTO.cs
│   ├── SinhVienDTO.cs
│   └── ...
│
├── QuanLySinhVien.DAL/          # Data Access Layer
│   ├── DatabaseHelper.cs
│   ├── UserDAL.cs
│   ├── SinhVienDAL.cs
│   └── ...
│
├── QuanLySinhVien.BLL/          # Business Logic Layer
│   ├── UserBLL.cs
│   ├── SinhVienBLL.cs
│   └── ...
│
├── QuanLySinhVien.UI/           # Windows Forms (Giao diện)
│   ├── Forms/
│   │   ├── frmLogin.cs
│   │   ├── frmMain.cs
│   │   ├── frmQuanLySinhVien.cs
│   │   ├── frmDangKyHocPhan.cs
│   │   ├── frmNhapDiem.cs
│   │   └── ...
│   └── Program.cs
│
├── QuanLySinhVien.Utils/        # Tiện ích chung
│   ├── ExcelHelper.cs
│   ├── EncryptionHelper.cs
│   └── ...
│
└── Database/                    # Scripts SQL
    ├── CreateDatabase.sql
    ├── StoredProcedures.sql
    ├── Functions.sql
    ├── Triggers.sql
    └── SampleData.sql
🗄️ CẤU TRÚC CƠ SỞ DỮ LIỆU
Hệ thống sử dụng 18 bảng với các quan hệ khóa ngoại đầy đủ:

STT	Bảng	Mô tả
1	Roles	Vai trò người dùng
2	Users	Tài khoản đăng nhập
3	PhanQuyen	Phân quyền chi tiết
4	Khoa	Khoa/viện đào tạo
5	Nganh	Ngành học
6	KhoaHoc	Khóa học (K2024, K2025...)
7	LopSinhHoat	Lớp sinh hoạt
8	MonHoc	Môn học
9	HocKy	Học kỳ, năm học
10	PhongHoc	Phòng học
11	GiangVien	Giảng viên
12	SinhVien	Sinh viên
13	LichSuChuyenLop	Lịch sử chuyển lớp
14	LopHocPhan	Lớp học phần (mở lớp)
15	DangKy	Đăng ký học phần
16	Diem	Kết quả điểm
17	LichSuSuaDiem	Lịch sử sửa điểm
18	HocPhi	Học phí
Sơ đồ quan hệ chính
text
Khoa → Nganh → LopSinhHoat → SinhVien → DangKy → Diem
                           ↘ LichSuChuyenLop
MonHoc → LopHocPhan → DangKy
GiangVien → LopHocPhan, LopSinhHoat
HocKy → LopHocPhan, HocPhi
PhongHoc → LopHocPhan
⚙️ CÀI ĐẶT VÀ CHẠY ỨNG DỤNG
Yêu cầu hệ thống
Windows 10/11 hoặc Windows Server

.NET Framework 4.7.2 trở lên

SQL Server 2019 trở lên (hoặc SQL Server Express)

Visual Studio 2019/2022 (Community bản miễn phí)

Hướng dẫn cài đặt
Bước 1: Clone project từ GitHub
bash
git clone https://github.com/ten_cua_ban/QuanLySinhVien_Winform.git
cd QuanLySinhVien_Winform
Bước 2: Tạo database SQL Server
Mở SQL Server Management Studio (SSMS)

Chạy script Database/CreateDatabase.sql để tạo database và các bảng

Chạy script Database/StoredProcedures.sql, Functions.sql, Triggers.sql

(Tùy chọn) Chạy Database/SampleData.sql để thêm dữ liệu mẫu

Bước 3: Cấu hình kết nối trong ứng dụng
Mở file app.config hoặc App.config trong project UI, sửa chuỗi kết nối:

xml
<connectionStrings>
    <add name="QuanLySinhVien" 
         connectionString="Data Source=.;Initial Catalog=QuanLySinhVien;Integrated Security=True;TrustServerCertificate=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
Bước 4: Build và chạy
Mở file QuanLySinhVien.sln bằng Visual Studio

Build solution (Ctrl + Shift + B)

Chạy ứng dụng (F5)

Bước 5: Đăng nhập với tài khoản mặc định
Vai trò	Username	Password
Admin	admin	admin123
Phòng ĐT	phongdt	123456
Giảng viên	GV001	123456
Sinh viên	SV001	123456
📖 HƯỚNG DẪN SỬ DỤNG CƠ BẢN
Đăng nhập và phân quyền
Mỗi người dùng đăng nhập bằng tài khoản riêng

Hệ thống tự động hiển thị menu phù hợp với vai trò

Admin có thể quản lý người dùng và phân quyền trong mục "Quản trị"

Quản lý sinh viên
Vào menu Sinh viên → Danh sách sinh viên

Nhấn Thêm mới để nhập hồ sơ sinh viên

Chọn ảnh đại diện (tùy chọn)

Lưu lại → Hệ thống tự động tạo tài khoản đăng nhập cho sinh viên

Đăng ký học phần (Sinh viên)
Đăng nhập với tài khoản sinh viên

Vào menu Học phần → Đăng ký học phần

Chọn học kỳ, xem danh sách lớp đang mở

Chọn lớp muốn đăng ký → hệ thống kiểm tra sĩ số, trùng lịch, tín chỉ

Xác nhận đăng ký

Nhập điểm (Giảng viên)
Đăng nhập với tài khoản giảng viên

Vào menu Điểm → Nhập điểm

Chọn lớp học phần mình dạy

Nhập điểm chuyên cần, giữa kỳ, cuối kỳ cho từng sinh viên

Hệ thống tự tính điểm tổng kết và xếp loại

Sau khi nhập xong, chọn Xác nhận điểm để khóa điểm (không thể sửa sau đó)

Xem báo cáo thống kê
Vào menu Báo cáo → Thống kê sinh viên hoặc Thống kê kết quả học tập

Chọn tiêu chí lọc (khoa, lớp, học kỳ...)

Nhấn Xuất Excel để lưu báo cáo ra file

🧪 KIỂM THỬ VỚI DỮ LIỆU MẪU
Sau khi chạy script SampleData.sql, bạn sẽ có:

2 Khoa: CNTT, Kinh tế

2 Ngành: CNPM, MMT

1 Khóa học: K2024

2 Lớp sinh hoạt: CNTT1, CNTT2

2 Giảng viên

5 Sinh viên

3 Môn học (có môn tiên quyết)

2 Học kỳ

3 Lớp học phần

Một số đăng ký và điểm mẫu

Bạn có thể sử dụng dữ liệu này để test toàn bộ chức năng.

🔧 XỬ LÝ LỖI THƯỜNG GẶP
Lỗi	Nguyên nhân	Cách khắc phục
Could not find stored procedure 'xxx'	Chưa tạo stored procedure trong database	Chạy script StoredProcedures.sql
Cannot open database "QuanLySinhVien"	Sai tên database hoặc chưa tạo	Kiểm tra chuỗi kết nối, tạo database trước
Login failed for user	Sai tài khoản/mật khẩu SQL Server	Dùng xác thực Windows hoặc kiểm tra user/pass
Lỗi khi Import Excel	Chưa cài đặt EPPlus hoặc file Excel sai định dạng	Cài EPPlus qua NuGet, đảm bảo file đúng mẫu
Lỗi khi xuất Excel	Office chưa được cài hoặc dùng Interop	Nên dùng EPPlus (không cần Office)
👨‍💻 NHÓM PHÁT TRIỂN
Thành viên	Vai trò	Phụ trách
Thành viên 1	Database Architect	Thiết kế database, stored procedures, functions, triggers
Thành viên 2	Backend Developer	Quản trị hệ thống, quản lý danh mục
Thành viên 3	Backend Developer	Quản lý sinh viên, mở lớp học phần
Thành viên 4	Backend Developer	Đăng ký học phần, nhập điểm
Thành viên 5	Frontend + Integration	Báo cáo, xuất Excel, dashboard
📚 TÀI LIỆU THAM KHẢO
Microsoft SQL Server Documentation

Windows Forms Documentation

EPPlus - Excel Export for .NET

Git & GitHub Guide

📄 GIẤY PHÉP
Dự án được phát triển cho mục đích học tập và đồ án môn học. Vui lòng không sử dụng cho mục đích thương mại khi chưa có sự đồng ý của nhóm phát triển.

🌟 KẾT LUẬN
Hệ thống Quản lý Sinh viên là một giải pháp toàn diện, đáp ứng hầu hết các nhu cầu quản lý đào tạo của một cơ sở giáo dục. Với kiến trúc 3 lớp rõ ràng, cơ sở dữ liệu chuẩn hóa và đầy đủ các stored procedure, function, trigger, hệ thống đảm bảo hiệu suất, bảo mật và dễ dàng mở rộng.

Mọi đóng góp và báo cáo lỗi xin gửi về qua GitHub Issues.
