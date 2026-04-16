USE master;
GO
DROP DATABASE IF EXISTS QuanLySinhVien;
GO
CREATE DATABASE QuanLySinhVien;
GO
USE QuanLySinhVien;
GO

-- ... phần còn lại giữ nguyên

-- =====================================================
-- PHẦN 1: QUẢN TRỊ HỆ THỐNG
-- =====================================================

-- 1. Roles
CREATE TABLE Roles (
    RoleID   INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL  -- 'Admin', 'PhongDT', 'GiangVien', 'SinhVien'
);

-- 2. Users
CREATE TABLE Users (
    UserID       INT IDENTITY PRIMARY KEY,
    Username     VARCHAR(50)  UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleID       INT          NOT NULL,
    MaNguoiDung  VARCHAR(20)  NULL,
    Email        VARCHAR(100) NULL,
    Status       BIT          DEFAULT 1,
    CreatedAt    DATETIME     DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- 3.Phan Quyen  (Phân quyền chi tiết theo module)
CREATE TABLE PhanQuyen (
    PermissionID INT IDENTITY PRIMARY KEY,
    RoleID       INT          NOT NULL,
    ModuleName   NVARCHAR(50) NOT NULL,
    CanView      BIT DEFAULT 0,
    CanAdd       BIT DEFAULT 0,
    CanEdit      BIT DEFAULT 0,
    CanDelete    BIT DEFAULT 0,
    CanApprove   BIT DEFAULT 0,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- =====================================================
-- PHẦN 2: QUẢN LÝ DANH MỤC
-- =====================================================

-- 4. Khoa
CREATE TABLE Khoa (
    MaKhoa  VARCHAR(10)   PRIMARY KEY,
    TenKhoa NVARCHAR(100) NOT NULL
);

-- 5. Nganh
CREATE TABLE Nganh (
    MaNganh  VARCHAR(10)   PRIMARY KEY,
    TenNganh NVARCHAR(100) NOT NULL,
    MaKhoa   VARCHAR(10)   NOT NULL,
    FOREIGN KEY (MaKhoa) REFERENCES Khoa(MaKhoa)
);

-- 6. KhoaHoc
CREATE TABLE KhoaHoc (
    MaKhoaHoc  VARCHAR(10)  PRIMARY KEY,
    TenKhoaHoc NVARCHAR(50) NOT NULL
);

-- 7. MonHoc
CREATE TABLE MonHoc (
    MaMon         VARCHAR(10)   PRIMARY KEY,
    TenMon        NVARCHAR(100) NOT NULL,
    SoTinChi      INT           NOT NULL,
    MonTienQuyet  VARCHAR(10)   NULL,  -- Self-reference: môn tiên quyết
    FOREIGN KEY (MonTienQuyet) REFERENCES MonHoc(MaMon)
);

-- 8. HocKy

CREATE TABLE HocKy (
    MaHocKy     VARCHAR(10)  PRIMARY KEY,
    TenHocKy    NVARCHAR(50) NOT NULL,
    NamHoc      VARCHAR(9)   NOT NULL,
    NgayBatDau  DATE, -- ghi date theo form 2026-03-22
    NgayKetThuc DATE, -- ghi tuong tu 
    SoTinToiDa  INT  DEFAULT 24  
);

-- 9. PhongHoc
CREATE TABLE PhongHoc (
    MaPhong  VARCHAR(10)   PRIMARY KEY,
    TenPhong NVARCHAR(50)  NOT NULL,
    SucChua  INT           NOT NULL,
    ViTri    NVARCHAR(100) NULL  
);


-- PHẦN 3: GIẢNG VIÊN & SINH VIÊN


-- 10. GiangVien
CREATE TABLE GiangVien (
    MaGV         VARCHAR(10)   PRIMARY KEY,
    HoTen        NVARCHAR(100) NOT NULL,
    MaKhoa       VARCHAR(10)   NOT NULL,
    Email        VARCHAR(100)  NULL,
    SoDienThoai  VARCHAR(15)   NULL,  
    NgaySinh     DATE          NULL,  
    GioiTinh     BIT           NULL,  
    HocVi        NVARCHAR(50)  NULL,  
    HocHam       NVARCHAR(50)  NULL,  
    FOREIGN KEY (MaKhoa) REFERENCES Khoa(MaKhoa)
);

-- 11. LopSinhHoat
CREATE TABLE LopSinhHoat (
    MaLopSH    VARCHAR(15)  PRIMARY KEY,
    TenLop     NVARCHAR(50) NOT NULL,
    MaNganh    VARCHAR(10)  NOT NULL,
    MaKhoaHoc  VARCHAR(10)  NOT NULL,
    MaGVCN     VARCHAR(10)  NULL,  -- Giáo viên chủ nhiệm
    FOREIGN KEY (MaNganh)   REFERENCES Nganh(MaNganh),
    FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc),
    FOREIGN KEY (MaGVCN)    REFERENCES GiangVien(MaGV)
);

-- 12. SinhVien

CREATE TABLE SinhVien (
    MaSV         VARCHAR(10)   PRIMARY KEY,
    HoTen        NVARCHAR(100) NOT NULL,
    NgaySinh     DATE,
    GioiTinh     BIT,
    DiaChi       NVARCHAR(200),
    MaLopSH      VARCHAR(15)   NOT NULL,
    TinhTrang    NVARCHAR(20)  DEFAULT N'Đang học',  -- 'Đang học', 'Nghỉ học', 'Thôi học', 'Tốt nghiệp'
    AnhDaiDien   NVARCHAR(255) NULL,                
    FOREIGN KEY (MaLopSH) REFERENCES LopSinhHoat(MaLopSH)
);

-- 13. LichSuChuyenLop -- bang nay o chuc nang sap xep o chuc nang Chuyen Nganh lop Theo quan ly sinh vien 
CREATE TABLE LichSuChuyenLop (
    MaLS       INT IDENTITY  PRIMARY KEY,
    MaSV       VARCHAR(10)   NOT NULL,
    LopCu      VARCHAR(15)   NOT NULL,
    LopMoi     VARCHAR(15)   NOT NULL,
    LyDo       NVARCHAR(200) NULL,
    NguoiDuyet NVARCHAR(100) NULL,
    NgayChyen  DATETIME      DEFAULT GETDATE(),
    FOREIGN KEY (MaSV)   REFERENCES SinhVien(MaSV),
    FOREIGN KEY (LopCu)  REFERENCES LopSinhHoat(MaLopSH),
    FOREIGN KEY (LopMoi) REFERENCES LopSinhHoat(MaLopSH)
);


-- PHẦN 4: QUẢN LÝ HỌC PHẦN

-- 14. LopHocPhan
CREATE TABLE LopHocPhan (
    MaLHP          VARCHAR(20)   PRIMARY KEY,
    MaLopHienThi   NVARCHAR(20)  NOT NULL,
    MaMon          VARCHAR(10)   NOT NULL,
    MaGV           VARCHAR(10)   NOT NULL,
    MaHocKy        VARCHAR(10)   NOT NULL,
    MaPhong        VARCHAR(10)   NOT NULL,
    SiSoToiDa      INT           DEFAULT 50,
    SiSoHienTai    INT           DEFAULT 0,
    TrangThai      NVARCHAR(20)  DEFAULT N'Đang mở',  -- 'Đang mở', 'Đã đóng', 'Đã hủy'
    Thu            TINYINT       NULL,  -- thu 2=Thứ 2, 3=Thứ 3, ..., 7=Thứ 7 (HP09)
    TietBatDau     TINYINT       NULL,  --  Tiết bắt đầu (1-12) (HP09)
    TietKetThuc    TINYINT       NULL,  -- Tiết kết thúc (1-12) (HP09)
    FOREIGN KEY (MaMon)    REFERENCES MonHoc(MaMon),
    FOREIGN KEY (MaGV)     REFERENCES GiangVien(MaGV),
    FOREIGN KEY (MaHocKy)  REFERENCES HocKy(MaHocKy),
    FOREIGN KEY (MaPhong)  REFERENCES PhongHoc(MaPhong)
);

-- 15. DangKy -- chuc nang nay la phan dang ki hoc phan + voi bang lop hoc phan 
CREATE TABLE DangKy (
    MaDK       INT IDENTITY PRIMARY KEY,
    MaSV       VARCHAR(10)  NOT NULL,
    MaLHP      VARCHAR(20)  NOT NULL,
    NgayDangKy DATETIME     DEFAULT GETDATE(),
    TrangThai  NVARCHAR(20) DEFAULT N'Đã đăng ký',  -- 'Đã đăng ký', 'Đã hủy'
    CONSTRAINT UC_DangKy UNIQUE (MaSV, MaLHP),
    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaLHP) REFERENCES LopHocPhan(MaLHP)
);

-- PHẦN 5: QUẢN LÝ ĐIỂM
-- 16. Diem
CREATE TABLE Diem (
    MaDiem         INT IDENTITY  PRIMARY KEY,
    MaDK           INT           NOT NULL,
    DiemChuyenCan  FLOAT         NULL,
    DiemGiuaKy     FLOAT         NULL,
    DiemCuoiKy     FLOAT         NULL,
    DiemTongKet    FLOAT         NULL,
    XepLoai        NVARCHAR(20)  NULL,  -- 'Xuất sắc', 'Giỏi', 'Khá', 'Trung bình', 'Yếu', 'Kém'
    TrangThaiDiem  NVARCHAR(20)  DEFAULT N'Đang nhập',  -- [BỔ SUNG] 'Đang nhập', 'Đã xác nhận', 'Đã khóa'
    NgayXacNhan    DATETIME      NULL,   --Ngày GV xác nhận khóa điểm (DI08)
    NguoiNhap      NVARCHAR(50)  NULL,
    NgayNhap       DATETIME      DEFAULT GETDATE(),
    FOREIGN KEY (MaDK) REFERENCES DangKy(MaDK)
);

-- 17. LichSuSuaDiem (Ghi lịch sử mỗi lần sửa điểm - DI06)
CREATE TABLE LichSuSuaDiem (
    MaLS      INT IDENTITY  PRIMARY KEY,
    MaDiem    INT           NOT NULL,
    LoaiDiem  NVARCHAR(30)  NULL,   -- 'DiemChuyenCan', 'DiemGiuaKy', 'DiemCuoiKy'
    DiemCu    FLOAT         NULL,
    DiemMoi   FLOAT         NULL,
    NguoiSua  NVARCHAR(100) NOT NULL,
    ThoiGian  DATETIME      DEFAULT GETDATE(),
    FOREIGN KEY (MaDiem) REFERENCES Diem(MaDiem)
);


-- PHẦN 6: HỌC PHÍ
-- 18. HocPhi -- chuc nang mo rong 
CREATE TABLE HocPhi (
    MaHocPhi       INT IDENTITY PRIMARY KEY,
    MaSV           VARCHAR(10)  NOT NULL,
    MaHocKy        VARCHAR(10)  NOT NULL,
    SoTinChiDangKy INT          NOT NULL,
    TongHocPhi     FLOAT        NOT NULL,
    DaDong         FLOAT        DEFAULT 0,
    TrangThai      NVARCHAR(20) DEFAULT N'Chưa đóng',  -- 'Chưa đóng', 'Đã đóng', 'Đóng một phần'
    NgayDong       DATETIME     NULL,
    FOREIGN KEY (MaSV)    REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaHocKy) REFERENCES HocKy(MaHocKy)
);

go
--login 
Create Procedure sp_Login 
@Username varchar(50),
@PasswordHash varchar(250),
@resultCode Int OUTPUT -- procedure write vao day , C# doc lai
AS
Begin 
  set nocount on ;

  -- khai bao bien tam de chua thong tin user tim duoc
  declare @UserID int 
  declare @RoleID int
  declare @MaNguoiDung varchar(20)
  declare @Email varchar(20) 

  select
  @UserID = UserID,
  @RoleID = RoleID,
  @MaNguoiDung = MaNguoiDung,
  @Email = Email

  From Users
  Where Username = @Username
  and PasswordHash = @PasswordHash
  and status = 1 


  If @@ROWCOUNT = 0 
  Begin 
   set @resultCode = 0 
   return -- thoat khoi precedure
  end 

  select 
  @UserID as UserID ,
  @RoleID as RoleID,
  @MaNguoiDung as MaNguoiDung,
  @Email as Email,
  (Select RoleName from Roles where RoleID = @RoleID) as RoleName
  
  set @resultCode = 1 -- = 1 tuc la thanh cong
end 
go 

-- change password
Create procedure sp_ChangePassword
  @UserID int ,
  @MKCuHash varchar(255) ,
  @MkMoiHash VarChar(250),
  @ResultCode INT OUTPUT
as 
begin 
  set NOCOUNT on ; -- dong nay de du lieu C# tranh nham lan du lieu

  if not exists (
    select 1 from Users
    Where UserID = @UserID
      and PasswordHash = @MKCuHash
      and status = 1 -- mk dung 
  )
  begin 
     set @ResultCode = -1 
     return 
  end 
 
  if @MKCuHash = @MkMoiHash
  Begin 
   set @ResultCode = 0 -- khong nhan ket qua false 
   return 
  end 

  Begin Transaction
  Begin try

  update Users 
  set PasswordHash = @MkMoiHash
  Where UserID = @UserID
    and PasswordHash = @MKCuHash -- Tim mk cu de thay doi du lieu 
  
  if @@ROWCOUNT = 0
  BEGIN 
  RollBack Transaction 
  set @ResultCode = -1 
  return 
  end

  commit Transaction
  set @ResultCode = 1 
end try 
begin catch 
   if XACT_STATE() <>0 
      ROLLBACK TRANSACTION
   declare @ErrMsg NVARCHAR(500) = ERROR_MESSAGE()
   raiserror(@ErrMsg , 16,1)
end catch 
end

go

--Phan Quyen
create Procedure sp_SetPermission
  @RoleID int ,
  @Module NVARCHAR(50),
  @CanView BIT,
  @CanAdd BIT,
  @CanEdit BIT,
  @CanDelete BIT,
  @CanApprove BIT 
as 
begin 
 set nocount on ;

 --kiem tra role co ton tai k
 If not exists (select 1 from Roles where RoleID = @RoleID)
 Begin
   Raiserror (N'Role khong ton tai.',16,1)
   return 
 end 

 merge PhanQuyen as target
 
 using (
 select @RoleID as RoleID, @Module as ModuleName
 ) as source 
 on(
 Target.RoleID = Source.RoleID
 and Target.ModuleName = Source.ModuleName
 )

 When matched then 
  update set 
   CanView = @CanView,
   CanAdd = @CanAdd,
   CanEdit = @CanEdit,
   CanDelete = @CanDelete,
   CanApprove = @CanApprove

   When not matched then 
    Insert (RoleID,ModuleName,CanView ,CanAdd,CanEdit,CanDelete,CanApprove)
    Values (@RoleID,@Module,@CanView ,@CanAdd,@CanEdit,@CanDelete,@CanApprove);
 end

 go

Create function fn_KiemTraQuyen(
   @RoleID int ,
   @Module NVARCHAR(50),
   @LoaiQuyen NVARCHAR(20)
 )
Returns bit -- tra ve don vi bit : =1 co quyen , = 0 khong co quyen truy cap
as 
begin 
   declare @KetQua BIT = 0 
   select @KetQua = 
     case @LoaiQuyen 
        when 'View' then CanView
        when 'Add' then CanAdd
        when 'Edit' then CanEdit
        when 'Delete' then CanDelete
        when 'Approve' then CanApprove
        Else 0 
    end 
from PhanQuyen
Where RoleID = @RoleID
 and ModuleName = @Module
return @KetQua
end

go

create procedure sp_GetPermissionsByRole
  @RoleID int 
as 
begin 
  set nocount on ;

  select 
   p.PermissionID,
   r.RoleName,
   p.ModuleName,
   p.CanView,
   p.CanAdd,
   p.CanEdit,
   p.CanDelete,
   p.CanApprove
  from PhanQuyen p
  join Roles r on p.RoleID = r.RoleID
  Where p.RoleID = @RoleID
  order by p.ModuleName -- sap xep theo ten module
end

go
-- Quan Ly nguoi dung 
create procedure sp_LayDanhSachNguoiDung
  @Searchkeyword NVARCHAR(100) = NULL

AS 
begin 
    set nocount on ;
    
    select 
    u.UserID,
    u.UserName,
    u.MaNguoiDung,
    u.Email,
    u.Status,
    r.RoleName,
    Case u.Status 
       when 1 Then 'Onlie'
       when 0 then 'Offline'
    end as TrangThaiHienThi
from Users u 
join Roles r on u.RoleID = r.RoleID
where(
  @Searchkeyword Is Null
  or u.Username Like '%' + @Searchkeyword + '%' --khi searchkeyword = null -> dieu kien Like '%' -> lay tat ca
  or u.MaNguoiDung Like '%' + @Searchkeyword + '%'
  or u.Email Like '%' + @Searchkeyword + '%'
)
Order by r.RoleName, u.Username -- sap xxep theo role truoc , roi theo ten dang nhap

end
go

create procedure sp_ThemNguoiDung
 @Username varchar(50),
 @Password varchar(250),
 @RoleID Int,
 @MaNguoiDung Varchar(20) = Null,
 @Email varchar(100) = Null ,
 @ResultCode int OUTPUT
 -- NEU output =1 thanh cong 
 -- =-1 username da ton tai
 -- -2 roleid khong hop le 
 as
 begin
    set nocount on ;
    -- kiem tra roleid da ton tai chua
    if not exists ( select 1 from Roles where RoleID = @RoleID)
    begin
      set @ResultCode = -2 
      return 
    end 
    --kiem tra username da ton tai chua 
    if exists (select 1 from Users where Username = @Username)
    begin
       set @ResultCode = -1
       return
    end


declare @PasswordHash varchar(255)
set @PasswordHash = LOWER (
    convert (varchar(250),hashbytes('SHA2_256',@Password), 2 )
  )
  Begin Transaction 
  begin try 
     
     Insert into Users(Username,PasswordHash,RoleID,MaNguoiDung,Email,Status)
     values(@Username,@PasswordHash,@RoleID,@MaNguoiDung,@Email,1)

     --SCOPE_IDENTITY() : lay userID vua dc insert tra ve de C# biet userId cua user vua tao
     Select SCOPE_IDENTITY() AS NewUserID

     commit transaction
     set @ResultCode = 1 
end try
begin catch
    if XACT_STATE() <>0 
       ROLLBACK TRANSACTION
    declare @ErrMsg NVARCHAR (500) = ERROR_MESSAGE()
    Raiserror(@ErrMsg,16,1)
end catch
end
go 


create procedure sp_CapNhatNguoiDung 
  @UserID int ,
  @Email varchar(100) = Null ,
  @MaNguoiDung Varchar(100) = Null , 
  @RoleID int = Null,
  @Status BIT = null,
  @ResultCode int output 

as
begin 
   set nocount on ;
     --kiem tra UserID ton tai hay khong 
     if not exists ( select 1 from Users where UserID = @UserID)
     BEGIN 
        SET @ResultCode = -1 
        return 
     end 

     if @Status = 0 --Neu status = 0 thi` dang muon khoa tai khoan nay
     begin
               --dem so Admin con dang hoat dong trong he thong
         declare @SoAdminConLai int 
         select @SoAdminConLai = COUNT(*) 
         from  Users u
         join roles r on u.RoleID = r.RoleID
         where r.RoleName = 'Admin'
          and u.Status = 1 
          and u.UserID <> @UserID -- tru so User dang bi khoa


          If @SoAdminConLai = 0 
          begin 
               set @ResultCode = -2 -- khong cho khoa Admin cuoi cung 
               return
            end 
        end 
-- Ki thuat COALESCE  : tra ve gia tri dau tien khong null 
-- Ky thuat nay cho phep chi cap nhat nhung cot duoc truyen vao ma` khong can viet nhieu procedure cho tung truong hop
-- noi ngan gon chi truyen gia tri moi vao caot duoc chi dinh khong dung vao cot khac

Update Users 
set
  Email =COALESCE (@Email, Email),
  MaNguoiDung= COALESCE (@MaNguoiDung, MaNguoiDung),
  RoleID = COALESCE(@RoleID,RoleID),
  Status = COALESCE (@Status ,Status)
Where UserID =@UserID

Set @ResultCode = 1 
end 

go
-- Xoa nguoi dung 

Create procedure sp_XoaNguoiDUng
  @UserID int,
  @ResultCode int output 
as 
begin 
  set nocount on ;

  -- khong cho xoa chinh minh
  if @UserID = (
      Select UserID from Users
      where UserID = @UserID
       and UserID = @UserID
       )
       --kiem tra day co phai Admin cuoi khong
       Begin 
         declare @SoAdminConLai INT 
         select @SoAdminConLai = COUNT(*)
         from Users u
         join Roles r on u.RoleID = r.RoleID
         Where r.RoleName = 'Admin'
          and u.Status = 1 
          and u.UserID<> @UserID

        if @SoAdminConLai = 0 
        begin
            set @ResultCode = -2 
            return 
        end 
    end 
    
    update Users
    set Status = 0 
    Where UserID = @UserID

    if @@ROWCOUNT = 0 
    begin 
       set @ResultCode = -1 -- khong tim thay User
       Return 
    end 

    set @ResultCode = 1 
end 
go        

--============================--
-- Cac chuc nang quan ly 
--QUAN LY KHOA 

create procedure sp_LayDanhSachKhoa
  @Keyword NVARCHAR(100) = NULL
  -- truyen tham so de tim kiem

as 
begin 
   set nocount on;

   select 
     k.MaKhoa,
     k.TenKhoa,
     (
       select Count(*) 
       from Nganh n
       where n.MaKhoa = k.MaKhoa
      ) as SoNganh,
      
      (
        select count(*) 
        from SinhVien sv
        join LopSinhHoat lsh on sv.MaLopSH = lsh.MaLopSH
        join Nganh n on lsh.MaNganh = n.MaNganh
        where n..MaKhoa = k.MaKhoa
          and sv.TinhTrang =N'Dang Hoc'
      ) as SoSinhVien

    from Khoa k
    where (
        @Keyword is null 
        or k.MaKhoa like '&' + @Keyword + '%'
        or k.TenKhoa like '&' + @Keyword + '%'    
    )

   order by k.MaKhoa
end
go



-- Them Khoa 
create procedure sp_ThemKhoa
  @MaKhoa Varchar(10),
  @TenKhoa NVARCHAR(100),
  @ResultCode int output 
  -- bien result code hien thi 
  -- 1= thanh cong
  -- -1 = MaKhoa khong ton tai 
  -- -2 = du lieu khong hop le 

as
begin 
     set nocount on ;

     -- kiem tra du lieu vdau vao 
     -- LTRIM / RTRIM : cat khoang trang 2 dau chuoi 
     -- Len : dem so ki tu 
     -- su dung UPPER vi khi input du lieu van chuan hoa chu tranh nham lan du lieu khi ra output 
     if LEN(LTRIM(RTRIM(@MaKhoa))) = 0
     or LEN(LTRIM(RTRIM(@TenKhoa))) = 0
     begin 
         set @ResultCode = -2 -- du lieu trong
         return 
     end 
     set @MaKhoa = UPPER(LTRIM(RTRIM(@MaKhoa)))
     set @TenKhoa = LTRIM(RTRIM(@TenKhoa))

     --kiem tra MaKhoa da ton tai chua 
     if exists (select 1 from khoa where MaKhoa = @MaKhoa)
     begin 
         set @ResultCode = -1 
         return 
     end 


     insert into Khoa(MaKhoa,TenKhoa) 
     values (@MaKhoa,@TenKhoa)

     set @ResultCode = 1 
end 
go
    

-- Cap Nhat ten khoa 
create procedure sp_SuaKhoa 
    @MaKhoa varchar(10),
    @TenKhoa NVARCHAR(100),
    @ResultCode int output 
as 
begin 
   set nocount on ;

  if not exists (select 1 from Khoa where MaKhoa = @MaKhoa )
  begin 
      set @ResultCode = -1 -- khong tim thay 
  end 

  -- kiem tra ten khoa co trong khong 
  if Len(LTRIM(RTRIM (@TenKhoa))) = 0 
  begin 
     set @ResultCode = -2 -- ten rong 
     return 
   end 

   -- luu y khong sua code cho nay (MaKhoa)
   UPDATE Khoa 
   set TenKhoa = LTRIM (RTRIM(@TenKhoa))
   where MaKhoa =@MaKhoa


   --kiem tra co update dc khong
   if @@ROWCOUNT = 0
   begin 
      set @ResultCode = -1 
      RETURN 
    END 


    set @ResultCode = 1 
end 
go 


-- xoa Khoa 

create procedure sp_XoaKhoa 
 @MaKhoa varchar(10),
 @ResultCode int output 
 as begin 
 set nocount on ;
 -- 1 = xoa thanh cong 
 -- -1 = xoa khong thanh cong 
 -- -2 = Nganh nay con` hoc , khong dc phep xoa 

 if not exists (select 1 from Khoa where MaKhoa = @MaKhoa)
 begin   
   set @ResultCode  = -1 
   return 
 end 


 -- bien kiem tra xac nhan lai li do xoa de tranh xoa nham lan
 declare @SoNganh int 
 select @SoNganh = COUNT(*)
 FROM Nganh 
 where MaKhoa = @MaKhoa

 if @SoNganh > 0 
 begin 
    set @ResultCode = -2 -- con nganh khong duoc xoa 
    return
 end 

 -- xoa du lieu 
 Delete from Khoa
 where MaKhoa = @MaKhoa
 set @ResultCode = 1
end 
go


--===================-
--Quan Ly Nganh Hoc

create procedure sp_LayDanhSachNganhHoc
  @Keyword NVARCHAR(100) = NULL
  -- truyen tham so de tim kiem

as 
begin 
   set nocount on;

   select 
     ng.MaNganh,
     ng.TenNganh,
     ng.MaKhoa,
     k.TenKhoa,
     (
       select Count(*) 
       from LopSinhHoat lsh
       where lsh.MaNganh = ng.MaNganh
      ) as SoLop,
      
      (
        select count(*) 
        from SinhVien sv
        join LopSinhHoat lsh on sv.MaLopSH = lsh.MaLopSH
        join Nganh n on lsh.MaNganh = n.MaNganh
        where lsh.MaNganh = ng.MaNganh
          and sv.TinhTrang =N'Dang Hoc'
      ) as SoSinhVien

    from Nganh ng
    join Khoa k on ng.MaKhoa = k.MaKhoa
    where (
        @Keyword is null 
        or ng.MaKhoa like '%' + @Keyword + '%'
        or ng.TenNganh like '%' + @Keyword + '%'
        or k.MaKhoa like '%' + @Keyword + '%'
    )

   order by k.MaKhoa , ng.MaNganh
end
go



-- Them Nganh
create procedure sp_ThemNganhHoc
  @MaNganh Varchar(10),
  @TenNganh NVARCHAR(100),
  @MaKhoa varchar(10),
  @ResultCode int output 
  -- bien result code hien thi 
  -- 1= thanh cong
  -- -1 = MaKhoa khong ton tai 
  -- -2 = du lieu khong hop le 
  -- -3 = MaKhoa khong ton tai

as
begin 
     set nocount on ;

     if len(LTRIM (RTRIM(@MaNganh))) = 0 
     or Len(LTRIM(RTRIM(@TenNganh))) = 0 
     or LEN(LTRIM(RTRIM(@MaKhoa))) = 0 
      
     begin 
         set @ResultCode = -2 -- du lieu trong
         return 
     end 

     --chuan hoa du lieu tránh sự nhầm lẫn
     set @MaNganh = UPPER(LTRIM(RTRIM(@MaNganh)))
     set @TenNganh = LTRIM(RTRIM(@TenNganh))
     set @MaKhoa = UPPER(LTRIM(RTRIM(@MaKhoa)))
     --kiem tra MaKhoa da ton tai chua 
     if NOT exists (select 1 from khoa where MaKhoa = @MaKhoa)
     begin 
         set @ResultCode = -3 -- khoa khong ton tai 
         return 
     end 
     

     if exists (select 1 from Nganh where MaNganh = @MaNganh)
     begin 
       set @ResultCode  = -1 
       RETURN
    END

     insert into Nganh(MaNganh,TenNganh,MaKhoa) 
     values (@MaNganh,@TenNganh,@MaKhoa)

     set @ResultCode = 1 
end 
go
    

-- Cap Nhat ten Nganh
create procedure sp_SuaNganhHoc
    @MaNganh varchar(10),
    @TenNganh NVARCHAR(100),
    @MaKhoa VarChar(10),
    @ResultCode int output 
as 
begin 
   set nocount on ;

  if not exists (select 1 from Nganh where MaNganh = @MaNganh )
  begin 
      set @ResultCode = -1 -- khong tim thay 
      return
  end 

  -- kiem tra ten nganh va makhoa co trong khong 
  if Len(LTRIM(RTRIM (@TenNganh))) = 0
  or Len(LTRIM(RTRIM (@MaKhoa))) = 0
  begin 
     set @ResultCode = -2 -- ten rong 
     return 
   end 
   
   set @MaKhoa = upper (LTRIM(RTRIM(@MaKhoa)))
   if not exists ( select 1 from Khoa Khoa where MaKhoa = @MaKhoa)
   begin 
      set @ResultCode = -3 
      return
    end 

   -- luu y khong sua code cho nay (MaNganh)
   UPDATE Nganh
   set TenNganh = LTRIM (RTRIM(@TenNganh)),MaKhoa = @MaKhoa
   where MaNganh =@MaNganh


   --kiem tra co update dc khong
   if @@ROWCOUNT = 0
   begin 
      set @ResultCode = -1 
      RETURN 
    END 


    set @ResultCode = 1 
end 
go 


-- xoa nganh 

create procedure sp_XoaNganhHoc
 @MaNganh varchar(10),
 @ResultCode int output 
 as begin 
 set nocount on ;
 -- 1 = xoa thanh cong 
 -- -1 = xoa khong thanh cong 
 -- -2 = Con` lop sinh hoat thuoc nganh nay

 if not exists (select 1 from Nganh where MaNganh = @MaNganh)
 begin   
   set @ResultCode  = -1 
   return 
 end 


 -- bien kiem tra xac nhan lai li do xoa de tranh xoa nham lan
 declare @SoLop int 
 select @SoLop = COUNT(*)
 FROM LopSinhHoat 
 where MaNganh = @MaNganh

 if @SoLop > 0 
 begin 
    set @ResultCode = -2 -- con nganh khong duoc xoa 
    return
 end 

 -- xoa du lieu 
 Delete from Nganh
 where MaNganh = @MaNganh
 set @ResultCode = 1
end 
go

--===================--
--Quan Ly Khóa Hoc

create procedure sp_LayDanhSachKhoaHoc
  @Keyword NVARCHAR(100) = NULL
  -- truyen tham so de tim kiem

as 
begin 
   set nocount on;

   select 
     Kh.MaKhoaHoc,
     kh.TenKhoaHoc,
     
     (
       select Count(*) 
       from LopSinhHoat lsh
       where lsh.MaKhoaHoc = kh.MaKhoaHoc
      ) as SoLop,
      
      (
        select count(*) 
        from SinhVien sv
        join LopSinhHoat lsh on sv.MaLopSH = lsh.MaLopSH
        where lsh.MaKhoaHoc = kh.MaKhoaHoc
          and sv.TinhTrang =N'Dang Hoc'
      ) as SoSinhVien

    from KhoaHoc kh
    
    where (
        @Keyword is null 
        or kh.MaKhoaHoc like '%' + @Keyword + '%'
        or kh.TenKhoaHoc like '%' + @Keyword + '%'

    )

   order by kh.MaKhoaHoc
end
go



-- Them Khoa Hoc 
create procedure sp_ThemKhoaHoc
  @MaKhoaHoc Varchar(10),
  @TenKhoaHoc NVARCHAR(100),
  @ResultCode int output 
  -- bien result code hien thi 
  -- 1= thanh cong
  -- -1 = MaKhoaHoc da ton tai 
  -- -2 = du lieu khong hop le 
  

as
begin 
     set nocount on ;

     if len(LTRIM (RTRIM(@MaKhoaHoc))) = 0 
     or Len(LTRIM(RTRIM(@TenKhoaHoc))) = 0 
     begin 
         set @ResultCode = -2 -- du lieu trong
         return 
     end 

     --chuan hoa du lieu tránh sự nhầm lẫn
     set @MaKhoaHoc = UPPER(LTRIM(RTRIM(@MaKhoaHoc)))
     set @TenKhoaHoc = LTRIM(RTRIM(@TenKhoaHoc))
  
     --kiem tra MaKhoa da ton tai chua 
     if exists (select 1 from KhoaHoc where @MaKhoaHoc = @MaKhoaHoc)
     begin 
         set @ResultCode = -1 -- khoa khong ton tai 
         return 
     end 
    

     insert into KhoaHoc(MaKhoaHoc,TenKhoaHoc) 
     values (@MaKhoaHoc,@TenKhoaHoc)

     set @ResultCode = 1 
end 
go
    

-- Cap Nhat ten Nganh
create procedure sp_SuaKhoaHoc
    @MaKhoaHoc varchar(10),
    @TenKhoaHoc NVARCHAR(100),
    @ResultCode int output 
as 
begin 
   set nocount on ;

  if not exists (select 1 from KhoaHoc where @MaKhoaHoc = @MaKhoaHoc )
  begin 
      set @ResultCode = -1 -- khong tim thay 
      return
  end 

  -- kiem tra ten KhoaHoc moi khong rong
  if Len(LTRIM(RTRIM (@TenKhoaHoc))) = 0
  begin 
     set @ResultCode = -2 -- ten rong 
     return 
   end 
   
  

   -- luu y khong sua code cho nay (MaNganh)
   UPDATE KhoaHoc
   set TenKhoaHoc = LTRIM (RTRIM(@TenKhoaHoc))
   where MaKhoaHoc =@MaKhoaHoc


   --kiem tra co update dc khong
   if @@ROWCOUNT = 0
   begin 
      set @ResultCode = -1 
      RETURN 
    END 


    set @ResultCode = 1 
end 
go 


-- xoa nganh 

create procedure sp_XoaKhoaHoc
 @MaKhoaHoc varchar(10),
 @ResultCode int output 
 as begin 
 set nocount on ;
 -- 1 = xoa thanh cong 
 -- -1 = xoa khong thanh cong 
 -- -2 = Con` lop sinh hoat thuoc khoa nay

 if not exists (select 1 from KhoaHoc where MaKhoaHoc = @MaKhoaHoc)
 begin   
   set @ResultCode  = -1 
   return 
 end 


 -- bien kiem tra xac nhan lai li do xoa de tranh xoa nham lan
 declare @SoLop int 
 select @SoLop = COUNT(*)
 FROM LopSinhHoat 
 where MaKhoaHoc = @MaKhoaHoc

 if @SoLop > 0 
 begin 
    set @ResultCode = -2 -- con nganh khong duoc xoa 
    return
 end 

 -- xoa du lieu 
 Delete from KhoaHoc
 where MaKhoaHoc = @MaKhoaHoc
 set @ResultCode = 1
end 
go






--===================--
--Quan Ly Lop Sinh Hoat

create procedure sp_LayDanhLopSinhHoat
  @Keyword NVARCHAR(100) = NULL
  -- truyen tham so de tim kiem

as 
begin 
   set nocount on;

   select 
     lsh.MaLopSH,
     lsh.TenLop,
     lsh.MaNganh,
     n.TenNganh,
     lsh.MaKhoaHoc,
     Kh.TenKhoaHoc,
     lsh.MaGVCN,

     isnull(gv.HoTen,'Chua co') as TenGVCN,
     
     (
       select Count(*) 
       from SinhVien sv
       where sv.MaLopSH = lsh.MaLopSH
         and sv.TinhTrang = N'Dang Hoc'
      ) as SoSinhVien
      
    
    from LopSinhHoat lsh
    join Nganh n ON lsh.MaNganh = n.MaNganh

    join KhoaHoc kh ON lsh.MaKhoaHoc = kh.MaKhoaHoc

    left join GiangVien gv on lsh.MaGVCN = gv.MaGV
    
    
    where (
        @Keyword is null 
        or lsh.MaLopSH like '%' + @Keyword + '%'
        or lsh.TenLop like '%' + @Keyword + '%'
        or n.TenNganh like '%' + @Keyword + '%'
        or kh.TenKhoaHoc like '%' + @Keyword + '%'
        or gv.HoTen like '%' + @Keyword + '%'

    )

   order by kh.MaKhoaHoc , n.MaNganh,lsh.MaLopSH
end
go



-- Them Khoa Hoc 
create procedure sp_ThemLopSinhHoat
  @MaLopSH Varchar(10),
  @TenLop NVARCHAR(100),
  @MaNganh varchar(10),
  @MaKhoaHoc varchar(10),
  @MaGVCN varchar(10) = null,
  -- Null gvcn tuy` chon , khong bat buoc
  @ResultCode int output 
  -- bien result code hien thi 
  -- 1= thanh cong
  -- -1 = MaLopSH da ton tai 
  -- -2 = du lieu rong
  -- -3 = MaNganh khong ton tai 
  -- -4 = MaKhoaHoc khong ton tai
  -- -5= MaGVCN khong ton tai ( khi co truyen vao)
  

as
begin 
     set nocount on ;

     if len(LTRIM (RTRIM(@MaLopSH))) = 0 
     or Len(LTRIM(RTRIM(@TenLop))) = 0
     or Len(LTRIM(RTRIM(@MaNganh))) = 0
     or Len(LTRIM(RTRIM(@MaKhoaHoc))) = 0
     begin 
         set @ResultCode = -2 -- du lieu trong
         return 
     end 

     --chuan hoa du lieu tránh sự nhầm lẫn
     set @MaLopSH = UPPER(LTRIM(RTRIM(@MaLopSH)))
     set @TenLop = LTRIM(RTRIM(@TenLop))
     set @MaNganh = UPPER(LTRIM(RTRIM(@MaNganh)))
     set @MaKhoaHoc = LTRIM(RTRIM(@MaKhoaHoc))
     
     --MaGVCN : chi chuan hoa khi kong co NULL
     if @MaGVCN is not null
         SET @MAGVCN = UPPER(LTRIM(RTRIM(@MaGVCN)))
     
     
     --kiem tra MaLopSH da ton tai chua 
     if exists (select 1 from LopSinhHoat where @MaLopSH = @MaLopSH)
     begin 
         set @ResultCode = -1 -- khoa khong ton tai 
         return 
     end 
    
    --kiem tra FK 1 : MaNganh ton tai khong
    if not exists (select 1 from Nganh where MaNganh = @MaNganh)
    begin 
       set @ResultCode = -3 
       return 
    end 

    --kiem tra FK2 : MaKhoaHoc ton tai khong

    if not exists ( select 1 from KhoaHoc where MaKhoaHoc = @MaKhoaHoc)
    begin 
       set @ResultCode = -4 
       return
    end 
     
     --kiem tra FK3 : MaGVCN co ton tai khong 
     -- chi kiem tra khi co truyen vao

     if @MaGVCN is not NULL 
     and not exists ( select 1 from GiangVien where MaGV = @MaGVCN)
     BEGIN 
       set @ResultCode = -5 
       RETURN 
    end 






     insert into LopSinhHoat(MaLopSH,TenLop,MaNganh,MaKhoaHoc,MaGVCN)
     values (@MaLopSH,@TenLop,@MaNganh,@MaKhoaHoc,@MaGVCN)

     set @ResultCode = 1 
end 
go
    

-- Cap Nhat ten Nganh
create procedure sp_SuaLopSinhHoat
    @MaLopSH varchar(15),
    @TenLop NVARCHAR(50),
    @MaNganh varchar(10),
    @MaKhoaHoc varchar(10),
    @MaGVCN VARCHAR(10)=NULL,
    @ResultCode int output 
as 
begin 
   set nocount on ;

  if not exists (select 1 from LopSinhHoat where MaLopSH = @MaLopSH )
  begin 
      set @ResultCode = -1 -- khong tim thay 
      return
  end 

  -- kiem tra ten du lieu bat buoc khong rong
  if Len(LTRIM(RTRIM (@TenLop))) = 0
  or LEN(LTRIM(RTRIM(@MaNganh))) = 0
  or Len(LTRIM(RTRIM(@MaKhoaHoc))) = 0 
  begin 
     set @ResultCode = -2 -- ten rong 
     return 
   end 
   
  -- chuan hoa ten du lieu 
  set @TenLop = LTRIM(RTRIM(@TenLop))
  set @MaNganh = upper(LTRIM(RTRIM(@MaNganh)))
  set @MaKhoaHoc = UPPER (LTRIM(RTRIM(@MaKhoaHoc)))
  if @MaGVCN is not null
   set @MAGVCN = UPPER(LTRIM(RTRIM(@MaGVCN)))




   if not exists (select 1 from Nganh where  MaNganh = @MaNganh)
   begin 
      set @ResultCode = -3 
      return 
    end 


    if not exists (select 1 from KhoaHoc where  MaKhoaHoc = @MaKhoaHoc)
   begin 
      set @ResultCode = -4
      return 
    end 


    if @MaGVCN is not NULL (select 1 from GiangVien where  MaGV = @MaGVCN)
   begin 
      set @ResultCode = -5 
      return 
    end 
  
  
  -- luu y khong sua code cho nay 
   UPDATE LopSinhHoat
   set TenLop = @TenLop,
       MaNganh = @MaNganh,
       MaKhoaHoc = @MaKhoaHoc,
       MaGVCN = @MaGVCN -- NEU MAGVCN = NULL dc chao nhan
   where MaLopSH = @MaLopSH



   --kiem tra co update dc khong
   if @@ROWCOUNT = 0
   begin 
      set @ResultCode = -1 
      RETURN 
    END 


    set @ResultCode = 1 
end 
go 


-- xoa nganh 

create procedure sp_XoaLopSinhHoat
 @MaLopSH varchar(10),
 @ResultCode int output 
 as begin 
 set nocount on ;
 -- 1 = xoa thanh cong 
 -- -1 = xoa khong thanh cong 
 -- -2 = Con` sinh vien sinh hoat thuoc lop nay

 if not exists (select 1 from LopSinhHoat where MaLopSH = @MaLopSH)
 begin   
   set @ResultCode  = -1 
   return 
 end 


 -- bien kiem tra xac nhan lai li do xoa de tranh xoa nham lan
 declare @SoSV int 
 select @SoSV = COUNT(*)
 FROM SinhVien
 where MaLopSH = @MaLopSH

 if @SoSV > 0 
 begin 
    set @ResultCode = -2 
    return
 end 

 -- xoa du lieu 
 Delete from LopSinhHoat
 where MaKhoaHoc = @MaLopSH
 set @ResultCode = 1
end 
go



--===================--
--Quan ly Mon Hoc
create procedure sp_LayDanhMonHoc
  @Keyword NVARCHAR(100) = NULL
  -- truyen tham so de tim kiem

as 
begin 
   set nocount on;

   select 
     m.MaMon,
     m.TenMon,
     m.SoTinChi,
     m.MonTienQuyet,
     ISNULL (mq.TenMon,N'Khong co') as TenMonTienQuyet,
    
    
    -- neu mon tien quyet la null thi hien thi 'khong co'
     (
       select Count(*) 
       from MonHoc mx
       where mx.MonTienQuyet = m.MaMon
      ) as soMonPhuThuoc,
      
      -- dem so lop hoc phan dang day mon nay`
      (
        select count(*) 
        from LopHocPhan lhp
        where lhp.MaMon = m.MaMon
      ) as SoLopHocPhan

    from MonHoc m 
    left join MonHoc mq ON m.MonTienQuyet = mq.MaMon

    where (
        @Keyword is null 
        OR m.MaMon   LIKE '%' + @Keyword + '%'
        OR m.TenMon  LIKE '%' + @Keyword + '%'
        OR mq.TenMon LIKE '%' + @Keyword + '%'  
    )

   order by m.MaMon
end
go



-- Them Khoa 
create procedure sp_ThemMonHoc
  @MaMon Varchar(10),
  @TenMon NVARCHAR(100),
  @SoTinChi int ,
  @MonTienQuyet Varchar(10) = NULL,
  @ResultCode int output 
  -- bien result code hien thi 
  -- 1= thanh cong
  -- -1 = MaKhoa khong ton tai 
  -- -2 = du lieu khong hop le 
  -- -3 = MonTienQuyet khong ton tai 
  -- -4 = mon tu lam tien quyet cho chinh minh

as
begin 
     set nocount on ;

     -- kiem tra du lieu vdau vao 
     -- LTRIM / RTRIM : cat khoang trang 2 dau chuoi 
     -- Len : dem so ki tu 
     -- su dung UPPER vi khi input du lieu van chuan hoa chu tranh nham lan du lieu khi ra output 
     if LEN(LTRIM(RTRIM(@MaMon))) = 0
     or LEN(LTRIM(RTRIM(@TenMon))) = 0
     begin 
         set @ResultCode = -2 -- du lieu trong
         return 
     end 
     --kiem tra So tin chi hop le vi so tin chi phai la duong
     if @SoTinChi <= 0 or @SoTinChi > 10 
     begin 
        set @ResultCode = -2
        Return 
    end

     set @MaMon = UPPER(LTRIM(RTRIM(@MaMon)))
     set @TenMon = LTRIM(RTRIM(@TenMon))
     if @MonTienQuyet IS NOT NULL 
      set @MonTienQuyet = UPPER(LTRIM(RTRIM(@MonTienQuyet)))

     --kiem tra MaKhoa da ton tai chua 
     if exists (select 1 from MonHoc where MaMon = @MaMon)
     begin 
         set @ResultCode = -1 
         return 
     end 

     if @MonTienQuyet is not Null
     and @MonTienQuyet = @MaMon
     begin 
      set @ResultCode = -4 
      return 
      end
     
     if @MonTienQuyet is not Null
     and not exists ( select 1 from  MonHoc where MaMon = @MonTienQuyet)
     begin 
        set @ResultCode = -3
    end
     
     insert into MonHoc(MaMon,TenMon,SoTinChi,MonTienQuyet) 
     values (@MaMon,@TenMon,@SoTinChi,@MonTienQuyet)

     set @ResultCode = 1 
end 
go
    

-- Cap Nhat ten khoa 
create procedure sp_SuaMonHoc 
    @MaMon varchar(10),
    @TenMon NVARCHAR(100),
    @SoTinChi int,
    @MonTienQuyet VARCHAR(10)= NULL,
    @ResultCode int output 
as 
begin 
   set nocount on ;

  if not exists (select 1 from MonHoc where MaMon = @MaMon )
  begin 
      set @ResultCode = -1 -- khong tim thay 
  end 

  -- kiem tra ten khoa co trong khong 
  if Len(LTRIM(RTRIM (@TenMon))) = 0 
  begin 
     set @ResultCode = -2 -- ten rong 
     return 
   end 

   if @SoTinChi <= 0 OR @SoTinChi > 10
   begin 
    set @ResultCode = -2
    return
   end 

   -- chuan hoa du lieu 
   set @TenMon = LTRIM(RTRIM(@TenMon))
   if @MonTienQuyet is not Null 
      set @MonTienQuyet = upper(Ltrim(Rtrim(@MonTienQuyet)))
   --kiem tra tu tham chieu chinh minh
   if @MonTienQuyet IS NOT NULL
   and @MonTienQuyet = @MaMon
   begin 
      set @ResultCode = -4
      return 
   end


  --kiem tra vong lap tien quyet de tranh nham lan dieu kien 
  if @MonTienQuyet is not NULL
  BEGIN 
    declare @Current varchar(10) = @MonTienQuyet
    declare @Dem int = 0

    while @Current is not null and @Dem < 20 -- dat gioi han tranh vong lap lap nhieu lan
    begin 
       if @Current = @MaMon
       begin 
           set @ResultCode = -5 --phat hien vong lap
           return
        end

  -- di den mon tien quyet tiep theo
    select @Current = MonTienQuyet
    from MonHoc
    where MaMon = @Current 

    set @Dem = @Dem + 1 
   end 
end 
    -- kiem tra self FK
    if @MonTienQuyet Is not Null 
    and not exists (select 1 from MonHoc where MaMon = @MonTienQuyet)
    begin
       set @ResultCode = -3
       return
    end

   -- luu y khong sua code cho nay vi la FK
   UPDATE MonHoc
   set TenMon = @TenMon,
       SoTinChi = @SoTinChi,
       MonTienQuyet = @MonTienQuyet
   where MaMon = @MaMon


   --kiem tra co update dc khong
   if @@ROWCOUNT = 0
   begin 
      set @ResultCode = -1 
      RETURN 
    END 


    set @ResultCode = 1 
end 
go 


-- xoa Khoa 

create procedure sp_XoaMonHoc
 @MaMon varchar(10),
 @ResultCode int output 
  -- 1 = xoa thanh cong 
 -- -1 = khong tim thay 
 -- -2 = co mon khac dung mon nay lam mon tien quyet
 -- -3 = co LopHocPhan Dang day mon nay


 as begin 
 set nocount on ;


 if not exists (select 1 from MonHoc where MaMon = @MaMon)
 begin   
   set @ResultCode  = -1 
   return 
 end 


 -- bien kiem tra xac nhan lai li do xoa de tranh xoa nham lan
 declare @SoMonPhuThuoc int 
 select @SoMonPhuThuoc = COUNT(*)
 FROM MonHoc
 where MonTienQuyet = @MaMon

 if @SoMonPhuThuoc > 0 
 begin 
    set @ResultCode = -2 
    return
 end 

 -- KIEM TRA LAN 2 co lopHocPhan nao dang day mon nay khong
 declare @SoLop int
 select @SoLop = count(*)
 from LopHocPhan
 where MaMon = @MaMon
 
 
 if @SoLop >0
 begin 
    SET @ResultCode = -3
    return
 end 

 
 -- xoa du lieu 
 Delete from MonHoc
 where MaMon = @MaMon
 set @ResultCode = 1
end 
go

--===================--
--Quan Ly Hoc Ki
--Lay danh sach hoc ky
create procedure sp_LayDanhHocKy
  @Keyword NVARCHAR(100) = NULL
  -- truyen tham so de tim kiem

as 
begin 
   set nocount on;

   select 
    hk.MaHocKy,
    hk.TenHocKy,
    hk.NamHoc,
    hk.NgayBatDau,
    hk.NgayKetThuc,
    hk.SoTinToiDa,
   

   case 
       when hk.NgayBatDau is not null
       and hk.NgayKetThuc is not null
       then datediff(day,hk.NgayBatDau,hk.NgayKetThuc)
       else Null
    end as SoNgay ,

    case 
        when hk.NgayBatDau is null or hk.NgayKetThuc is null
             then N'Chua xac dinh'
        when getdate() < hk.NgayBatDau
             then N'Chua Bat Dau'
        when GetDate() between hk.NgayBatDau and hk.NgayKetThuc
             then N'Dang dien ra'
         else N'Da Ket Thuc'
    end as TrangThai,
    
    
      -- dem so lop hoc phan dang day mon nay`
      (
        select count(*) 
        from LopHocPhan lhp
        where lhp.MaHocKy = hk.MaHocKy
      ) as SoLopHocPhan

    from HocKy hk
   

    where (
        @Keyword is null 
        OR hk.MaHocKy   LIKE '%' + @Keyword + '%'
        OR hk.TenHocKy  LIKE '%' + @Keyword + '%'
        OR hk.NamHoc LIKE '%' + @Keyword + '%'  
    )

   order by hk.NamHoc desc , hk.MaHocKy desc 
end
go



-- Them Khoa 
create procedure sp_ThemHocKy
  @MaHocKy Varchar(10),
  @TenHocKy NVARCHAR(100),
  @NamHoc varchar(10) ,
  @NgayBatDau DATE = NULL,
  @NgayKetThuc DATE = NULL,
  @SoTinToiDa int = 24,
  @ResultCode int output 
  -- bien result code hien thi 
  -- 1= thanh cong
  -- -1 = MaHocKy khong ton tai 
  -- -2 = du lieu khong hop le 
  -- -3 = Ngay Bat Dau >= NgayKetThuc
  -- -4 = SoTinToiDa Khong hop le

as
begin 
     set nocount on ;

     -- kiem tra du lieu vdau vao 
     -- LTRIM / RTRIM : cat khoang trang 2 dau chuoi 
     -- Len : dem so ki tu 
     -- su dung UPPER vi khi input du lieu van chuan hoa chu tranh nham lan du lieu khi ra output 
     if LEN(LTRIM(RTRIM(@MaHocKy))) = 0
     or LEN(LTRIM(RTRIM(@TenHocKy))) = 0
     or LEN(LTRIM(RTRIM(@NamHoc))) = 0
     begin 
         set @ResultCode = -2 -- du lieu trong
         return 
     end 

     --kiem tra dinh dang Nam Hoc phai la 'YYYY-YYYY'

     if Len(@NamHoc) <> 9
     or SUBSTRING(@NamHoc ,5,1) <>'-'
     begin
         set @ResultCode = -2 
         return 
    end 




     --kiem tra So tin chi hop le vi so tin chi phai la duong
     if @SoTinToiDa <= 0 or @SoTinToiDa > 50 
     begin 
        set @ResultCode = -4
        Return 
    end

     set @MaHocKy = UPPER(LTRIM(RTRIM(@MaHocKy)))
     set @TenHocKy = LTRIM(RTRIM(@TenHocKy))
     set @NamHoc = LTRIM(RTRIM(@NamHoc))

     --kiem tra MaHocKy da ton tai chua 
     if exists (select 1 from HocKy where MaHocKy = @MaHocKy)
     begin 
         set @ResultCode = -1 
         return 
     end 

     if @NgayBatDau is not Null
     and @NgayKetThuc IS NOT NULL
     and @NgayBatDau >= @NgayKetThuc
     begin 
      set @ResultCode = -3
      return 
      end
     
  
     
     insert into HocKy(MaHocKy,TenHocKy,NamHoc,NgayBatDau,NgayKetThuc,SoTinToiDa) 
     values (@MaHocKy,@TenHocKy,@NamHoc,@NgayBatDau,@NgayKetTHuc,@SoTinToiDa)

     set @ResultCode = 1 
end 
go
    

-- Cap Nhat Hoc Ky
create procedure sp_SuaHocKy 
    @MaHocKy varchar(10),
    @TenHocKy NVARCHAR(100),
    @NamHoc Varchar(10),
    @NgayBatDau date = null ,
    @NgayKetThuc date = null,
    @SoTinChiToiDa int  = 24,
    @ResultCode int output 
as 
begin 
   set nocount on ;

  if not exists (select 1 from HocKy where MaHocKy = @MaHocKy )
  begin 
      set @ResultCode = -1 -- khong tim thay 
  end 

  -- kiem tra ten khoa co trong khong 
  if Len(LTRIM(RTRIM (@TenHocKy))) = 0 
  or Len(LTRIM(RTRIM (@NamHoc))) = 0 
  begin 
     set @ResultCode = -2 -- ten rong 
     return 
   end 

   --kiem tra dinh dang cua Nam Hoc
   IF len(@NamHoc) <> 9 
   or substring (@NamHoc, 5 ,1 ) <> '-'
   begin 
      set @ResultCode = -2 
      return 
    end




   if @SoTinChiToiDa <= 0 OR @SoTinChiToiDa > 10
   begin 
    set @ResultCode = -4
    return
   end 

   -- chuan hoa du lieu 
   set @TenHocKy = LTRIM(RTRIM(@TenHocKy))
   set @NamHoc =LTRIM(rTRIM(@NamHoc))

   --kiem tra ngay 
   if @NgayBatDau IS NOT NULL
   AND @NgayBatDau IS NOT NULL
   AND @NgayBatDau >= @NgayKetThuc

   
   begin 
      set @ResultCode = -3
      return 
   end



   IF EXISTS (
   
   select 1 from HocKy
   where MaHocKy = @MaHocKy
   and NgayBatDau is not null
   and NgayKetThuc is not null
   and getdate() between NgayBatDau and NgayKetThuc
   
   )
   and(@NgayBatDau <>( select NgayBatDau from HocKy where MaHocKy = @MaHocKy )
    or @NgayKetThuc <> (select @NgayKetThuc from HocKy where MaHocKy =@MaHocKy))
    begin 
      set @ResultCode = -5
      return 
    end


 
   -- luu y khong sua code cho nay vi la FK
   UPDATE HocKy
   set TenHocKy = @TenHocKy,
       NamHoc = @NamHoc,
       NgayBatDau = @NgayBatDau,
       NgayKetThuc = @NgayKetThuc,
       SoTinToiDa = @SoTinChiToiDa
     where MaHocKy = @MaHocKy


   --kiem tra co update dc khong
   if @@ROWCOUNT = 0
   begin 
      set @ResultCode = -1 
      RETURN 
    END 


    set @ResultCode = 1 
end 
go 


-- xoa 

create procedure sp_XoaHocKy
 @MaHocKy varchar(10),
 @ResultCode int output 
  -- 1 = xoa thanh cong 
 -- -1 = khong tim thay 
 -- -2 = con` LopHocPhan thuoc hoc ky nay
 -- -3 = co hoc ky dang dien ra 


 as begin 
 set nocount on ;


 if not exists (select 1 from HocKy where MaHocKy = @MaHocKy)
 begin   
   set @ResultCode  = -1 
   return 
 end 

 -- khong cho xoa hoc ky dang dien ra 
 if exists 
 (
    select 1 from HocKy
    where MaHocKy = @MaHocKy
    and NgayBatDau is not null
    AND NgayKetThuc is not null
    and getdate() between NgayBatDau AND NgayKetThuc
 
 
 )
 begin 
   set @ResultCode = -3 
   RETURN 
end 



 -- bien kiem tra xac nhan lai li do xoa de tranh xoa nham lan
 declare @SoLop int 
 select @SoLop = COUNT(*)
 FROM LopHocPhan
 where MaHocKy = @MaHocKy

 if @SoLop > 0 
 begin 
    set @ResultCode = -2 
    return
 end 

 
 -- xoa du lieu 
 Delete from HocKy
 where MaHocKy = @MaHocKy
 set @ResultCode = 1
end 
go





--===================--
--Quan Ly Giang Vien

--Lay danh sach Giang Vien 
create procedure sp_LayDanhCuaGiangVien
  @Keyword NVARCHAR(100) = NULL
  -- truyen tham so de tim kiem

as 
begin 
   set nocount on;

   select 
   gv.MaGV,
   gv.HoTen,
   gv.MaKhoa,
   k.TenKhoa,
   gv.Email,
   gv.SoDienThoai,
   gv.NgaySinh,
   
   --Tinh tuoi tu NgaySinh
   case 
       when gv.NgaySinh is not null
       then DATEDIFF(year , gv.NgaySinh,GetDate())
       else NULL
      
    end as SoNgay ,

    case gv.GioiTinh 
      when 1 then N'NAM'
      when 2 then N'NU'
      else N'Chua Cap Nhat'
    end as GioiTinhHienThi,
    isnull (gv.HocVI , N'Chua Cap Nhat') as HocVi,
    isnull (gv.HocHam ,N'Chua cap nhat') as HocHam,

    --dem so lop hoc phan Gvien dang day/da day
    (
    select count(*)
    from LopHocPhan lhp
    where lhp.MaGV = gv.MaGV
    )AS SoLopDay,
     -- kiem tra GV da co Tkhoan chua 

    case 
      when exists (
           select 1 from Users u
           where u.MaNguoiDung = gv.MaGV
      )
      Then N'Co tai khoan'
      else N'Chua co tai khoan'
    end as TrangThaiTaiKhoan
   from GiangVien gv
   join Khoa k on gv.MaKhoa = k.MaKhoa
    
  
   

    where (
        @Keyword is null 
        OR gv.MaGV   LIKE '%' + @Keyword + '%'
        OR gv.HoTen  LIKE '%' + @Keyword + '%'
        OR k.TenKhoa LIKE '%' + @Keyword + '%'
        OR gv.Email   LIKE '%' + @Keyword + '%'
        OR gv.SoDienThoai  LIKE '%' + @Keyword + '%'
        OR gv.HocVi LIKE '%' + @Keyword + '%'
    )

   order by k.MaKhoa , gv.HoTen 
end
go



-- Them GV + Tao tai khoan
create procedure sp_ThemGiangVien
  @MaGV Varchar(10),
  @HoTen NVARCHAR(100),
  @MaKhoa varchar(10) ,
  @Email Varchar(100) = NULL,
  @SoDienThoai Varchar(15) = NULL,
  @NgaySinh DATE = NULL,
  @GioiTinh BIT = NULL,
  @HocVi NVARCHAR(50)= NULL,
  @HocHam NVARCHAR(50) = NULL,
  @ResultCode int output 
 
  -- 1= thanh cong
  -- -1 = Magv da  ton tai 
  -- -2 = du lieu khong hop le 
  -- -3 = MaKhoa khong ton tai
  -- -4 = Email da duoc dung voi GV khac

as
begin 
     set nocount on ;

     -- kiem tra du lieu vdau vao 
     -- LTRIM / RTRIM : cat khoang trang 2 dau chuoi 
     -- Len : dem so ki tu 
     -- su dung UPPER vi khi input du lieu van chuan hoa chu tranh nham lan du lieu khi ra output 
     if LEN(LTRIM(RTRIM(@MaGV))) = 0
     or LEN(LTRIM(RTRIM(@HoTen))) = 0
     or LEN(LTRIM(RTRIM(@MaKhoa))) = 0
     begin 
         set @ResultCode = -2 -- du lieu trong
         return 
     end 
     
     if exists (select 1 from GiangVien where MaGV = @MaGV)
     BEGIN 
        SET @ResultCode = -1 
        return 
    end 


    if not exists (select 1 from Khoa where MaKhoa = @MaKhoa)
    begin 
       set @ResultCode = -3
       return 
    end 

    --kiem tra Email co trung hay khong 
    if @Email is not null
    and Exists (select 1 from GiangVien where Email = @Email)
    begin 
      set @ResultCode = -4
      return 
    end 
    

    begin transaction 

    begin try 
          
          insert into GiangVien
            (MaGV,HoTen,MaKhoa,Email,SoDienThoai,NgaySinh,GioiTinh,HocVi,HocHam)
          values 
            (@MaGV,@HoTen,@MaKhoa,@Email,@SoDienThoai,@NgaySinh,@GioiTinh,@HocVi,@HocHam)



    --Hash mat khau mac dinh = MaGV
    declare @PwdHash varchar(255)
    set @PwdHash = LOWER (convert(varchar(255),hashbytes('SHA_256',@MaGV),2))

    insert into Users
    (Username,PasswordHash,RoleID,MaNguoiDung,Email,Status)
    Values
    (@MaGV,@PwdHash,3,@MaGV,@Email,1)

    commit transaction
    set @ResultCode =1 
    end Try
    begin catch

       if XACT_STATE() <> 0
          ROLLBACK TRANSACTION
            DECLARE @ErrMsg NVARCHAR(500) = ERROR_MESSAGE()
            RAISERROR(@ErrMsg,16,1)
    end catch
end
go
           

    

-- Cap Nhat Hoc Ky
create procedure sp_CapNhatHoSo
    @MaGV        VARCHAR(10),
    @HoTen       NVARCHAR(100),
    @MaKhoa      VARCHAR(10),
    @Email       VARCHAR(100) = NULL,
    @SoDienThoai VARCHAR(15)  = NULL,
    @NgaySinh    DATE         = NULL,
    @GioiTinh    BIT          = NULL,
    @HocVi       NVARCHAR(50) = NULL,
    @HocHam      NVARCHAR(50) = NULL,
    @ResultCode  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

  
    IF NOT EXISTS (SELECT 1 FROM GiangVien WHERE MaGV = @MaGV)
    BEGIN
        SET @ResultCode = -1
        RETURN
    END

    IF LEN(LTRIM(RTRIM(@HoTen)))  = 0
    OR LEN(LTRIM(RTRIM(@MaKhoa))) = 0
    BEGIN
        SET @ResultCode = -2
        RETURN
    END

    
    SET @HoTen  = LTRIM(RTRIM(@HoTen))
    SET @MaKhoa = UPPER(LTRIM(RTRIM(@MaKhoa)))
    IF @Email IS NOT NULL
        SET @Email = LTRIM(RTRIM(@Email))


    IF NOT EXISTS (SELECT 1 FROM Khoa WHERE MaKhoa = @MaKhoa)
    BEGIN
        SET @ResultCode = -3
        RETURN
    END

    -- Kiểm tra Email trùng với GV KHÁC
    -- Dùng <> @MaGV để loại chính GV đang sửa ra
    -- (GV được giữ email cũ của mình)
    IF @Email IS NOT NULL
    AND EXISTS (
        SELECT 1 FROM GiangVien
        WHERE Email = @Email
          AND MaGV <> @MaGV  -- loại chính mình ra
    )
    BEGIN
        SET @ResultCode = -4
        RETURN
    END

    -- Khi sửa GV → cập nhật Email trong Users luôn
    -- để 2 bảng không bị lệch nhau
    BEGIN TRANSACTION
    BEGIN TRY

        -- Cập nhật hồ sơ GV
        UPDATE GiangVien
        SET HoTen       = @HoTen,
            MaKhoa      = @MaKhoa,
            Email       = @Email,
            SoDienThoai = @SoDienThoai,
            NgaySinh    = @NgaySinh,
            GioiTinh    = @GioiTinh,
            HocVi       = @HocVi,
            HocHam      = @HocHam
        WHERE MaGV = @MaGV

        -- Cập nhật Email trong Users để nhất quán
        UPDATE Users
        SET Email = @Email
        WHERE MaNguoiDung = @MaGV

        COMMIT TRANSACTION
        SET @ResultCode = 1

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION
        DECLARE @ErrMsg NVARCHAR(500) = ERROR_MESSAGE()
        RAISERROR(@ErrMsg, 16, 1)
    END CATCH
END
GO

-- xoa 

create procedure sp_XoaGiangVien
 @MaGV varchar(10),
 @ResultCode int output 
  -- 1 = xoa thanh cong 
 -- -1 = khong tim thay GV 
 -- -2 = GV con` day LopHocPhan thuoc hoc ky nay



 as begin 
 set nocount on ;


 if not exists (select 1 from GiangVien where MaGV = @MaGV)
 begin   
   set @ResultCode  = -1 
   return 
 end 
 
 declare @SoLop int 
 select @SoLop = COUNT(*)
 FROM LopHocPhan 
 where MaGV = @MaGV

 if @SoLop > 0
 begin 
    set @ResultCode = -2 
    return 
end 

  begin transaction
  begin try
     delete from GiangVien 
     where MaGV = @MaGV 

     update Users
     set Status = 0 
     where MaNguoiDung = @MaGV

     commit Transaction 
     set @ResultCode = 1

 end try 
 begin catch 
    if XACT_STATE() <> 0
          ROLLBACK TRANSACTION
         DECLARE @ErrMsg Nvarchar(500) = ERROR_MESSAGE()
         RAISERROR (@ErrMsg,16,1)
    end catch
end
go




--===================--
--Quan Ly Phong Hoc

CREATE PROCEDURE sp_GetAllPhongHoc
    @Keyword NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.MaPhong,
        p.TenPhong,
        p.SucChua,
        ISNULL(p.ViTri, N'Chưa cập nhật') AS ViTri,
        -- ISNULL: ViTri NULL → hiển thị 'Chưa cập nhật'
        -- thay vì ô trống khó phân biệt

        -- Đếm số lớp học phần đang dùng phòng này
        (
            SELECT COUNT(*)
            FROM LopHocPhan lhp
            WHERE lhp.MaPhong = p.MaPhong
        ) AS SoLopSuDung,

        -- Tính tỷ lệ sử dụng phòng
        -- CAST: chuyển INT → FLOAT để tính phần trăm
        -- Phòng nào hay được dùng → Admin biết cần mở thêm phòng
        CASE
            WHEN (SELECT COUNT(*) FROM LopHocPhan WHERE MaPhong = p.MaPhong) = 0
            THEN N'Chưa sử dụng'
            ELSE CAST(
                (SELECT COUNT(*) FROM LopHocPhan WHERE MaPhong = p.MaPhong)
                AS NVARCHAR(10)
            ) + N' lớp đang dùng'
        END AS TinhTrangSuDung

    FROM PhongHoc p
    WHERE (
        @Keyword IS NULL
        OR p.MaPhong  LIKE '%' + @Keyword + '%'
        OR p.TenPhong LIKE '%' + @Keyword + '%'
        OR p.ViTri    LIKE '%' + @Keyword + '%'
        -- Cho tìm theo vị trí: 'Tòa A' → ra tất cả phòng tòa A
    )
    ORDER BY p.MaPhong
END
GO


CREATE PROCEDURE sp_ThemPhongHoc
    @MaPhong  VARCHAR(10),
    @TenPhong NVARCHAR(50),
    @SucChua  INT,
    @ViTri    NVARCHAR(100) = NULL,
    @ResultCode INT OUTPUT
    --  1 = thành công
    -- -1 = MaPhong đã tồn tại
    -- -2 = dữ liệu rỗng
    -- -3 = SucChua không hợp lệ
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra dữ liệu bắt buộc rỗng
    IF LEN(LTRIM(RTRIM(@MaPhong)))  = 0
    OR LEN(LTRIM(RTRIM(@TenPhong))) = 0
    BEGIN
        SET @ResultCode = -2
        RETURN
    END

    -- Kiểm tra SucChua hợp lệ
    -- Phòng học thực tế thường từ 1 đến 500 chỗ
    IF @SucChua <= 0 OR @SucChua > 500
    BEGIN
        SET @ResultCode = -3
        RETURN
    END

    -- Chuẩn hóa dữ liệu
    SET @MaPhong  = UPPER(LTRIM(RTRIM(@MaPhong)))
    SET @TenPhong = LTRIM(RTRIM(@TenPhong))
    IF @ViTri IS NOT NULL
        SET @ViTri = LTRIM(RTRIM(@ViTri))

    -- Kiểm tra trùng MaPhong
    IF EXISTS (SELECT 1 FROM PhongHoc WHERE MaPhong = @MaPhong)
    BEGIN
        SET @ResultCode = -1
        RETURN
    END

    -- DM08 không có FK → INSERT thẳng, không cần kiểm tra thêm
    INSERT INTO PhongHoc (MaPhong, TenPhong, SucChua, ViTri)
    VALUES (@MaPhong, @TenPhong, @SucChua, @ViTri)

    SET @ResultCode = 1
END
GO


CREATE PROCEDURE sp_ThemPhongHocMoi
    @MaPhong  VARCHAR(10),
    @TenPhong NVARCHAR(50),
    @SucChua  INT,
    @ViTri    NVARCHAR(100) = NULL,
    @ResultCode INT OUTPUT
    --  1 = thành công
    -- -1 = MaPhong đã tồn tại
    -- -2 = dữ liệu rỗng
    -- -3 = SucChua không hợp lệ
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra dữ liệu bắt buộc rỗng
    IF LEN(LTRIM(RTRIM(@MaPhong)))  = 0
    OR LEN(LTRIM(RTRIM(@TenPhong))) = 0
    BEGIN
        SET @ResultCode = -2
        RETURN
    END

    -- Kiểm tra SucChua hợp lệ
    -- Phòng học phải chứa được ít nhất 1 người
    -- Thực tế không có phòng nào > 1000 chỗ
    IF @SucChua <= 0 OR @SucChua > 1000
    BEGIN
        SET @ResultCode = -3
        RETURN
    END

    -- Chuẩn hóa dữ liệu
    SET @MaPhong  = UPPER(LTRIM(RTRIM(@MaPhong)))
    SET @TenPhong = LTRIM(RTRIM(@TenPhong))
    IF @ViTri IS NOT NULL
        SET @ViTri = LTRIM(RTRIM(@ViTri))

    -- Kiểm tra trùng MaPhong
    IF EXISTS (SELECT 1 FROM PhongHoc WHERE MaPhong = @MaPhong)
    BEGIN
        SET @ResultCode = -1
        RETURN
    END

    -- DM08 không có FK → INSERT thẳng, không cần transaction
    INSERT INTO PhongHoc (MaPhong, TenPhong, SucChua, ViTri)
    VALUES (@MaPhong, @TenPhong, @SucChua, @ViTri)

    SET @ResultCode = 1
END
GO


CREATE PROCEDURE sp_SuaPhongHoc
    @MaPhong  VARCHAR(10),
    @TenPhong NVARCHAR(50),
    @SucChua  INT,
    @ViTri    NVARCHAR(100) = NULL,
    @ResultCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra Phòng tồn tại không
    IF NOT EXISTS (SELECT 1 FROM PhongHoc WHERE MaPhong = @MaPhong)
    BEGIN
        SET @ResultCode = -1
        RETURN
    END

    -- Kiểm tra dữ liệu
    IF LEN(LTRIM(RTRIM(@TenPhong))) = 0
    BEGIN
        SET @ResultCode = -2
        RETURN
    END

    IF @SucChua <= 0 OR @SucChua > 1000
    BEGIN
        SET @ResultCode = -3
        RETURN
    END

    -- Chuẩn hóa
    SET @TenPhong = LTRIM(RTRIM(@TenPhong))
    IF @ViTri IS NOT NULL
        SET @ViTri = LTRIM(RTRIM(@ViTri))

    -- Kiểm tra thêm: không cho giảm SucChua xuống thấp hơn
    -- số SV đang đăng ký lớp trong phòng này
    -- Tránh tình trạng phòng 30 chỗ nhưng đang có 40 SV đăng ký
    DECLARE @SiSoHienTai INT
    SELECT @SiSoHienTai = ISNULL(MAX(lhp.SiSoHienTai), 0)
    FROM LopHocPhan lhp
    WHERE lhp.MaPhong   = @MaPhong
      AND lhp.TrangThai = N'Đang mở'

    IF @SucChua < @SiSoHienTai
    BEGIN
        -- -4: sức chứa mới nhỏ hơn sĩ số đang học trong phòng
        SET @ResultCode = -4
        RETURN
    END

    -- Không cho sửa MaPhong vì là PK
    UPDATE PhongHoc
    SET TenPhong = @TenPhong,
        SucChua  = @SucChua,
        ViTri    = @ViTri
    WHERE MaPhong = @MaPhong

    IF @@ROWCOUNT = 0
    BEGIN
        SET @ResultCode = -1
        RETURN
    END

    SET @ResultCode = 1
END
GO


CREATE PROCEDURE sp_XoaPhongHoc
    @MaPhong    VARCHAR(10),
    @ResultCode INT OUTPUT
    --  1 = xóa thành công
    -- -1 = không tìm thấy
    -- -2 = còn LopHocPhan đang dùng phòng này
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra Phòng tồn tại không
    IF NOT EXISTS (SELECT 1 FROM PhongHoc WHERE MaPhong = @MaPhong)
    BEGIN
        SET @ResultCode = -1
        RETURN
    END

    -- Kiểm tra Referential Integrity
    -- Không phân biệt TrangThai — kể cả lớp đã đóng
    -- vẫn đang tham chiếu đến phòng này trong lịch sử
    DECLARE @SoLop INT
    SELECT @SoLop = COUNT(*)
    FROM LopHocPhan
    WHERE MaPhong = @MaPhong

    IF @SoLop > 0
    BEGIN
        SET @ResultCode = -2
        RETURN
    END

    -- Xóa vật lý — PhongHoc không có dữ liệu lịch sử cần giữ
    DELETE FROM PhongHoc
    WHERE MaPhong = @MaPhong

    SET @ResultCode = 1
END
GO

--Quan Ly Sinh Vien 
--SV01: Thêm sinh viên 
CREATE PROCEDURE sp_ThemSinhVien
    @MaSV VARCHAR(10),          -- mã sinh viên (PK)
    @HoTen NVARCHAR(100),       -- họ tên
    @NgaySinh DATE,             -- ngày sinh
    @GioiTinh BIT,              -- 1: Nam, 0: Nữ
    @MaLopSH VARCHAR(15)        -- lớp sinh hoạt (FK)
AS
BEGIN
    BEGIN TRAN   -- Atomicity: gộp nhiều thao tác

    INSERT INTO SinhVien(MaSV, HoTen, NgaySinh, GioiTinh, MaLopSH)
    VALUES (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @MaLopSH)  
    -- thêm sinh viên

    IF @@ERROR <> 0
        ROLLBACK   -- lỗi → rollback toàn bộ
    ELSE
        COMMIT     -- thành công → lưu vĩnh viễn (Durability)
END;
go
-- Sửa thông tin sinh viên 
CREATE PROCEDURE sp_SuaThongTinSinhVien
    @MaSV VARCHAR(10),          -- sinh viên cần sửa
    @HoTen NVARCHAR(100),       -- họ tên mới
    @NgaySinh DATE,             
    @GioiTinh BIT               
AS
BEGIN
    BEGIN TRAN   -- đảm bảo Atomicity

    UPDATE SinhVien
    SET HoTen = @HoTen,
        NgaySinh = @NgaySinh,
        GioiTinh = @GioiTinh
    WHERE MaSV = @MaSV  

    IF @@ERROR <> 0
        ROLLBACK
    ELSE
        COMMIT
END;
go
--Xoá sinh viên 
CREATE PROCEDURE sp_XoaSinhVien
    @MaSV VARCHAR(10)   -- sinh viên cần xóa
AS
BEGIN
    BEGIN TRAN

    DELETE FROM SinhVien
    WHERE MaSV = @MaSV   -- xóa theo khóa chính

    IF @@ERROR <> 0
        ROLLBACK
    ELSE
        COMMIT
END; 

--Xem danh sach sinh viên  
--SELECT * 
--FROM vw_SinhVienDayDu;   -- view đã join đầy đủ

--Tìm kiếm sinh viên 
--SELECT * 
--FROM fn_SearchSinhVien(@Keyword);  -- keyword

--Xem chi tiết hồ sơ 
--SELECT *
--FROM vw_SinhVienDayDu
--WHERE MaSV = @MaSV;   -- lọc theo MSSV
go
-- Chuyển lớp / ngành 
CREATE PROCEDURE sp_ChuyenLop
    @MaSV VARCHAR(10),        -- sinh viên
    @MaLopMoi VARCHAR(15)     -- lớp mới
AS
BEGIN
    BEGIN TRAN   -- Atomicity

    DECLARE @LopCu VARCHAR(15)

    SELECT @LopCu = MaLopSH 
    FROM SinhVien 
    WHERE MaSV = @MaSV

    -- cập nhật lớp
    UPDATE SinhVien
    SET MaLopSH = @MaLopMoi
    WHERE MaSV = @MaSV
      AND TinhTrang = N'Đang học'   -- đảm bảo Consistency

    -- ghi lịch sử (audit)
    INSERT INTO LichSuChuyenLop(MaSV, LopCu, LopMoi)
    VALUES (@MaSV, @LopCu, @MaLopMoi)

    IF @@ERROR <> 0
        ROLLBACK   -- lỗi 1 bước → rollback hết
    ELSE
        COMMIT     -- đảm bảo Durability
END;
go
--Quan Ly Dang Ki Hoc Phan 
CREATE VIEW vw_LopHocPhanDayDu AS
SELECT 
    lhp.MaLHP, 
    m.TenMon, 
    m.SoTinChi, 
    gv.HoTen AS TenGV, 
    p.TenPhong, 
    p.ViTri, 
    hk.TenHocKy, 
    hk.NamHoc,
    lhp.Thu,
    lhp.TietBatDau,
    lhp.TietKetThuc,
    lhp.SiSoToiDa,
    lhp.SiSoHienTai
FROM LopHocPhan lhp, MonHoc m, GiangVien gv, PhongHoc p, HocKy hk
WHERE lhp.MaMon = m.MaMon 
  AND lhp.MaGV = gv.MaGV 
  AND lhp.MaPhong = p.MaPhong 
  AND lhp.MaHocKy = hk.MaHocKy;
GO

CREATE VIEW vw_DangKyCuaSV AS
SELECT 
    dk.MaSV, 
    dk.MaLHP, 
    lhp.Thu, 
    lhp.TietBatDau, 
    lhp.TietKetThuc, 
    m.TenMon, 
    m.SoTinChi
FROM DangKy dk, LopHocPhan lhp, MonHoc m
WHERE dk.MaLHP = lhp.MaLHP 
  AND lhp.MaMon = m.MaMon
  AND dk.TrangThai = N'Đã đăng ký';
GO

CREATE FUNCTION fn_TongTinChiDangKy (@maSV VARCHAR(10), @maHocKy VARCHAR(10))
RETURNS INT
AS 
BEGIN
    DECLARE @tongTinChi INT;
    
    SELECT @tongTinChi = ISNULL(SUM(m.SoTinChi), 0)
    FROM DangKy dk, LopHocPhan lhp, MonHoc m
    WHERE dk.MaLHP = lhp.MaLHP 
      AND lhp.MaMon = m.MaMon
      AND dk.MaSV = @maSV 
      AND lhp.MaHocKy = @maHocKy 
      AND dk.TrangThai = N'Đã đăng ký';
      
    RETURN @tongTinChi;
END;
GO

CREATE FUNCTION fn_KiemTraTrungLich (@maSV VARCHAR(10), @maLHPMoi VARCHAR(20))
RETURNS BIT
AS 
BEGIN
    DECLARE @isTrung BIT = 0;
    DECLARE @thu TINYINT, @tietBD TINYINT, @tietKT TINYINT, @maHocKy VARCHAR(10);

    SELECT @thu = Thu, @tietBD = TietBatDau, @tietKT = TietKetThuc, @maHocKy = MaHocKy
    FROM LopHocPhan 
    WHERE MaLHP = @maLHPMoi;

    IF EXISTS (
        SELECT 1 
        FROM DangKy dk, LopHocPhan lhp
        WHERE dk.MaLHP = lhp.MaLHP
          AND dk.MaSV = @maSV 
          AND lhp.MaHocKy = @maHocKy
          AND dk.TrangThai = N'Đã đăng ký'
          AND lhp.Thu = @thu
          AND lhp.TietBatDau <= @tietKT 
          AND lhp.TietKetThuc >= @tietBD
    )
    BEGIN
        SET @isTrung = 1;
    END

    RETURN @isTrung;
END;
GO

CREATE TRIGGER trg_KhongChoVuotSiSo
ON DangKy
INSTEAD OF INSERT
AS 
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM inserted i, LopHocPhan lhp
        WHERE i.MaLHP = lhp.MaLHP 
          AND lhp.SiSoHienTai >= lhp.SiSoToiDa
    )
    BEGIN
        RAISERROR(N'Lớp học phần đã đạt sĩ số tối đa, không thể đăng ký!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    INSERT INTO DangKy(MaSV, MaLHP, TrangThai)
    SELECT MaSV, MaLHP, TrangThai FROM inserted;
END;
GO

CREATE TRIGGER trg_CapNhatSiSo
ON DangKy
AFTER INSERT, UPDATE
AS 
BEGIN
    UPDATE LopHocPhan
    SET SiSoHienTai = (
        SELECT COUNT(*) 
        FROM DangKy 
        WHERE MaLHP = LopHocPhan.MaLHP AND TrangThai = N'Đã đăng ký'
    )
    WHERE MaLHP IN (
        SELECT MaLHP FROM inserted
        UNION
        SELECT MaLHP FROM deleted
    );
END;
GO

CREATE PROCEDURE sp_DangKyHocPhan
    @maSV VARCHAR(10),
    @maLHP VARCHAR(20)
AS 
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; 

    BEGIN TRY
        BEGIN TRAN;

        IF dbo.fn_KiemTraTrungLich(@maSV, @maLHP) = 1
        BEGIN
            RAISERROR(N'Bạn bị trùng lịch học với một môn khác đã đăng ký!', 16, 1);
        END

        DECLARE @maHocKy VARCHAR(10), @soTinToiDa INT, @soTinLHP INT;
        
        SELECT @maHocKy = MaHocKy FROM LopHocPhan WHERE MaLHP = @maLHP;
        SELECT @soTinToiDa = SoTinToiDa FROM HocKy WHERE MaHocKy = @maHocKy;
        
        SELECT @soTinLHP = m.SoTinChi 
        FROM MonHoc m, LopHocPhan lhp 
        WHERE m.MaMon = lhp.MaMon 
          AND lhp.MaLHP = @maLHP;

        IF (dbo.fn_TongTinChiDangKy(@maSV, @maHocKy) + @soTinLHP) > @soTinToiDa
        BEGIN
            RAISERROR(N'Vượt quá số tín chỉ tối đa được phép trong học kỳ!', 16, 1);
        END

        INSERT INTO DangKy (MaSV, MaLHP, TrangThai)
        VALUES (@maSV, @maLHP, N'Đã đăng ký');

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO

CREATE PROCEDURE sp_HuyDangKy
    @maSV VARCHAR(10),
    @maLHP VARCHAR(20)
AS 
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRY
        BEGIN TRAN;

        IF EXISTS (
            SELECT 1 FROM DangKy WITH (UPDLOCK) 
            WHERE MaSV = @maSV AND MaLHP = @maLHP AND TrangThai = N'Đã đăng ký'
        )
        BEGIN
            UPDATE DangKy 
            SET TrangThai = N'Đã hủy' 
            WHERE MaSV = @maSV AND MaLHP = @maLHP;
        END
        ELSE 
        BEGIN
            RAISERROR(N'Học phần này chưa được đăng ký hoặc đã bị hủy trước đó!', 16, 1);
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO

--Quan Ly diem so va xep loai 

-- TẠO CHỈ MỤC (INDEX)
CREATE INDEX IX_DangKy_MaLHP ON DangKy(MaLHP);
CREATE INDEX IX_DangKy_MaSV ON DangKy(MaSV);
CREATE INDEX IX_Diem_MaDK ON Diem(MaDK);
CREATE INDEX IX_Diem_TrangThai ON Diem(TrangThaiDiem);
CREATE INDEX IX_LichSuSuaDiem_MaDiem ON LichSuSuaDiem(MaDiem);
GO

-- TẠO FUNCTION
CREATE FUNCTION fn_DiemTongKet (
    @cc FLOAT,
    @gk FLOAT,
    @ck FLOAT
)
RETURNS FLOAT
AS
BEGIN
    IF @cc IS NULL OR @gk IS NULL OR @ck IS NULL
        RETURN NULL;
    RETURN ROUND(@cc * 0.1 + @gk * 0.3 + @ck * 0.6, 2);
END;
GO

ALTER FUNCTION fn_XepLoai (
    @diemTK FLOAT
)
RETURNS NVARCHAR(20)
AS
BEGIN
    IF @diemTK IS NULL
        RETURN NULL;
    RETURN CASE
        WHEN @diemTK >= 8.5 THEN N'Giỏi'
        WHEN @diemTK >= 8.0 THEN N'Khá giỏi'
        WHEN @diemTK >= 7.0 THEN N'Khá'
        WHEN @diemTK >= 6.5 THEN N'Trung bình khá'
        WHEN @diemTK >= 5.5 THEN N'Trung bình'
        WHEN @diemTK >= 5.0 THEN N'Trung bình yếu'
        WHEN @diemTK >= 4.0 THEN N'Yếu'
        WHEN @diemTK < 4.0 THEN N'Kém'
    END;
END;
GO

-- TẠO TRIGGER
ALTER TRIGGER trg_TinhDiemTongKet
ON Diem
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE d
    SET 
        DiemTongKet = dbo.fn_DiemTongKet(i.DiemChuyenCan, i.DiemGiuaKy, i.DiemCuoiKy),
        XepLoai = dbo.fn_XepLoai(dbo.fn_DiemTongKet(i.DiemChuyenCan, i.DiemGiuaKy, i.DiemCuoiKy))
    FROM Diem d
    INNER JOIN inserted i ON d.MaDiem = i.MaDiem;
END;
GO

CREATE TRIGGER trg_KhoaDiem
ON Diem
INSTEAD OF UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted WHERE TrangThaiDiem = N'Đã khóa')
    BEGIN
        RAISERROR(N'Không thể sửa điểm vì điểm đã bị khóa!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    UPDATE d
    SET 
        DiemChuyenCan = i.DiemChuyenCan,
        DiemGiuaKy = i.DiemGiuaKy,
        DiemCuoiKy = i.DiemCuoiKy,
        TrangThaiDiem = i.TrangThaiDiem,
        NgayXacNhan = i.NgayXacNhan
    FROM Diem d
    INNER JOIN inserted i ON d.MaDiem = i.MaDiem;
END;
GO

-- TẠO VIEW
CREATE VIEW vw_DiemSinhVien
AS
SELECT 
    sv.MaSV,
    sv.HoTen,
    mh.TenMon,
    mh.SoTinChi,
    d.DiemChuyenCan,
    d.DiemGiuaKy,
    d.DiemCuoiKy,
    d.DiemTongKet,
    d.XepLoai,
    d.TrangThaiDiem,
    lhp.MaLHP,
    lhp.TenLHP
FROM Diem d
JOIN DangKy dk ON d.MaDK = dk.MaDK
JOIN SinhVien sv ON dk.MaSV = sv.MaSV
JOIN LopHocPhan lhp ON dk.MaLHP = lhp.MaLHP
JOIN MonHoc mh ON lhp.MaMon = mh.MaMon;
GO

CREATE VIEW vw_BangDiemLop
AS
SELECT 
    lhp.MaLHP,
    lhp.TenLHP,
    mh.TenMon,
    mh.SoTinChi,
    sv.MaSV,
    sv.HoTen,
    d.DiemChuyenCan,
    d.DiemGiuaKy,
    d.DiemCuoiKy,
    d.DiemTongKet,
    d.XepLoai,
    d.TrangThaiDiem
FROM LopHocPhan lhp
JOIN MonHoc mh ON lhp.MaMon = mh.MaMon
JOIN DangKy dk ON lhp.MaLHP = dk.MaLHP
JOIN SinhVien sv ON dk.MaSV = sv.MaSV
JOIN Diem d ON dk.MaDK = d.MaDK;
GO

-- TẠO STORED PROCEDURE
CREATE PROCEDURE sp_NhapDiem
    @MaDK INT,
    @DiemChuyenCan FLOAT = NULL,
    @DiemGiuaKy FLOAT = NULL,
    @DiemCuoiKy FLOAT = NULL,
    @NguoiNhap NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM DangKy WHERE MaDK = @MaDK)
        BEGIN
            RAISERROR(N'Mã đăng ký không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM Diem WHERE MaDK = @MaDK)
        BEGIN
            UPDATE Diem
            SET 
                DiemChuyenCan = ISNULL(@DiemChuyenCan, DiemChuyenCan),
                DiemGiuaKy = ISNULL(@DiemGiuaKy, DiemGiuaKy),
                DiemCuoiKy = ISNULL(@DiemCuoiKy, DiemCuoiKy)
            WHERE MaDK = @MaDK;
        END
        ELSE
        BEGIN
            INSERT INTO Diem (MaDK, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TrangThaiDiem)
            VALUES (@MaDK, @DiemChuyenCan, @DiemGiuaKy, @DiemCuoiKy, N'Đang nhập');
        END

        COMMIT TRANSACTION;
        PRINT N'Nhập điểm thành công!';
    END TRY
    BEGIN CATCH
        ROLLBACK;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO

create PROCEDURE sp_SuaDiem
    @MaDiem INT,
    @LoaiDiem NVARCHAR(10),
    @DiemMoi FLOAT,
    @NguoiSua NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Kiểm tra MaDiem tồn tại
        IF NOT EXISTS (SELECT 1 FROM Diem WHERE MaDiem = @MaDiem)
        BEGIN
            RAISERROR(N'MaDiem %d không tồn tại trong bảng Diem!', 16, 1, @MaDiem);
            ROLLBACK;
            RETURN;
        END

        DECLARE @TrangThai NVARCHAR(20);
        SELECT @TrangThai = TrangThaiDiem FROM Diem WHERE MaDiem = @MaDiem;
        IF @TrangThai = N'Đã khóa'
        BEGIN
            RAISERROR(N'Điểm đã bị khóa, không thể sửa!', 16, 1);
            ROLLBACK;
            RETURN;
        END

        DECLARE @DiemCu FLOAT;
        SELECT @DiemCu = CASE @LoaiDiem
            WHEN 'cc' THEN DiemChuyenCan
            WHEN 'gk' THEN DiemGiuaKy
            WHEN 'ck' THEN DiemCuoiKy
            ELSE NULL
        END
        FROM Diem WHERE MaDiem = @MaDiem;

        IF @DiemCu IS NULL AND @LoaiDiem NOT IN ('cc','gk','ck')
        BEGIN
            RAISERROR(N'Loại điểm không hợp lệ!', 16, 1);
            ROLLBACK;
            RETURN;
        END

        -- Cập nhật điểm
        UPDATE Diem
        SET 
            DiemChuyenCan = CASE WHEN @LoaiDiem = 'cc' THEN @DiemMoi ELSE DiemChuyenCan END,
            DiemGiuaKy = CASE WHEN @LoaiDiem = 'gk' THEN @DiemMoi ELSE DiemGiuaKy END,
            DiemCuoiKy = CASE WHEN @LoaiDiem = 'ck' THEN @DiemMoi ELSE DiemCuoiKy END
        WHERE MaDiem = @MaDiem;

        -- Ghi lịch sử (chỉ khi UPDATE thành công)
        INSERT INTO LichSuSuaDiem (MaDiem, LoaiDiem, DiemCu, DiemMoi, NguoiSua)
        VALUES (@MaDiem, @LoaiDiem, @DiemCu, @DiemMoi, @NguoiSua);

        COMMIT TRANSACTION;
        PRINT N'Sửa điểm thành công!';
    END TRY
    BEGIN CATCH
        ROLLBACK;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO

CREATE PROCEDURE sp_XacNhanDiem
    @MaLHP NVARCHAR(20),
    @MaGV NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM LopHocPhan WHERE MaLHP = @MaLHP)
        BEGIN
            RAISERROR(N'Lớp học phần không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END

        IF EXISTS (
            SELECT 1
            FROM Diem d
            JOIN DangKy dk ON d.MaDK = dk.MaDK
            WHERE dk.MaLHP = @MaLHP
              AND (d.DiemChuyenCan IS NULL OR d.DiemGiuaKy IS NULL OR d.DiemCuoiKy IS NULL)
        )
        BEGIN
            RAISERROR(N'Không thể khóa điểm vì còn sinh viên thiếu điểm thành phần!', 16, 1);
            ROLLBACK;
            RETURN;
        END

        UPDATE d
        SET TrangThaiDiem = N'Đã khóa',
            NgayXacNhan = GETDATE()
        FROM Diem d
        JOIN DangKy dk ON d.MaDK = dk.MaDK
        WHERE dk.MaLHP = @MaLHP;

        COMMIT TRANSACTION;
        PRINT N'Đã khóa điểm cho lớp ' + @MaLHP;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO

PRINT N'Đã tạo xong toàn bộ đối tượng trong database QLDiem.';

--Bao cao va thong ke

-- =====================================================
-- VIEW 1: vw_ThongKeSinhVienTheoKhoa
-- =====================================================
USE QuanLySinhVien;
GO

CREATE OR ALTER VIEW vw_ThongKeSinhVienTheoKhoa
AS
SELECT 
    k.MaKhoa,
    k.TenKhoa,
    n.MaNganh,
    n.TenNganh,
    kh.MaKhoaHoc,
    kh.TenKhoaHoc,
    COUNT(sv.MaSV) AS SoLuongSinhVien,
    SUM(CASE WHEN sv.TinhTrang = N'Đang học' THEN 1 ELSE 0 END) AS SoLuongDangHoc,
    SUM(CASE WHEN sv.TinhTrang = N'Tốt nghiệp' THEN 1 ELSE 0 END) AS SoLuongTotNghiep,
    SUM(CASE WHEN sv.TinhTrang = N'Nghỉ học' THEN 1 ELSE 0 END) AS SoLuongNghiHoc,
    SUM(CASE WHEN sv.TinhTrang = N'Thôi học' THEN 1 ELSE 0 END) AS SoLuongThoiHoc
FROM SinhVien sv
JOIN LopSinhHoat l ON sv.MaLopSH = l.MaLopSH
JOIN Nganh n ON l.MaNganh = n.MaNganh
JOIN Khoa k ON n.MaKhoa = k.MaKhoa
JOIN KhoaHoc kh ON l.MaKhoaHoc = kh.MaKhoaHoc
GROUP BY k.MaKhoa, k.TenKhoa, n.MaNganh, n.TenNganh, kh.MaKhoaHoc, kh.TenKhoaHoc;
GO

-- =====================================================
-- VIEW 2: vw_KetQuaHocTapTheoKy
-- =====================================================
CREATE OR ALTER VIEW vw_KetQuaHocTapTheoKy
AS
SELECT 
    hk.MaHocKy,
    hk.TenHocKy,
    hk.NamHoc,
    d.XepLoai,
    COUNT(*) AS SoLuong,
    AVG(d.DiemTongKet) AS DiemTrungBinh
FROM Diem d
JOIN DangKy dk ON d.MaDK = dk.MaDK
JOIN LopHocPhan lhp ON dk.MaLHP = lhp.MaLHP
JOIN HocKy hk ON lhp.MaHocKy = hk.MaHocKy
WHERE d.XepLoai IS NOT NULL 
  AND d.TrangThaiDiem = N'Đã khóa'
GROUP BY hk.MaHocKy, hk.TenHocKy, hk.NamHoc, d.XepLoai;
GO

-- =====================================================
-- VIEW 3: vw_ThongKeSVTheoLop
-- =====================================================
CREATE OR ALTER VIEW vw_ThongKeSVTheoLop
AS
SELECT 
    l.MaLopSH,
    l.TenLop,
    n.TenNganh,
    k.TenKhoa,
    kh.TenKhoaHoc,
    COUNT(sv.MaSV) AS SiSo,
    SUM(CASE WHEN sv.TinhTrang = N'Đang học' THEN 1 ELSE 0 END) AS SoLuongDangHoc,
    SUM(CASE WHEN sv.GioiTinh = 1 THEN 1 ELSE 0 END) AS SoLuongNam,
    SUM(CASE WHEN sv.GioiTinh = 0 THEN 1 ELSE 0 END) AS SoLuongNu
FROM LopSinhHoat l
LEFT JOIN SinhVien sv ON l.MaLopSH = sv.MaLopSH
JOIN Nganh n ON l.MaNganh = n.MaNganh
JOIN Khoa k ON n.MaKhoa = k.MaKhoa
JOIN KhoaHoc kh ON l.MaKhoaHoc = kh.MaKhoaHoc
GROUP BY l.MaLopSH, l.TenLop, n.TenNganh, k.TenKhoa, kh.TenKhoaHoc;
GO

-- =====================================================
-- VIEW 4: vw_DiemTrungBinhSinhVien
-- =====================================================
CREATE OR ALTER VIEW vw_DiemTrungBinhSinhVien
AS
SELECT 
    sv.MaSV,
    sv.HoTen,
    l.TenLop,
    n.TenNganh,
    AVG(d.DiemTongKet) AS DiemTrungBinh,
    MAX(CASE WHEN d.XepLoai IN (N'Xuất sắc', N'Giỏi') THEN 1 ELSE 0 END) AS DaDatGioi
FROM SinhVien sv
JOIN LopSinhHoat l ON sv.MaLopSH = l.MaLopSH
JOIN Nganh n ON l.MaNganh = n.MaNganh
LEFT JOIN DangKy dk ON sv.MaSV = dk.MaSV
LEFT JOIN Diem d ON dk.MaDK = d.MaDK AND d.TrangThaiDiem = N'Đã khóa'
GROUP BY sv.MaSV, sv.HoTen, l.TenLop, n.TenNganh;
GO

-- Kiểm tra view
SELECT * FROM vw_KetQuaHocTapTheoKy ORDER BY NamHoc, TenHocKy, XepLoai
GO
-- =====================================================
-- FUNCTION 1: fn_ThongKeSVTheoKhoa
-- =====================================================
CREATE OR ALTER FUNCTION fn_ThongKeSVTheoKhoa (@maKhoa VARCHAR(10))
RETURNS TABLE
AS
RETURN
(
    SELECT 
        n.MaNganh,
        n.TenNganh,
        COUNT(sv.MaSV) AS TongSoSV,
        SUM(CASE WHEN sv.TinhTrang = N'Đang học' THEN 1 ELSE 0 END) AS DangHoc,
        SUM(CASE WHEN sv.TinhTrang = N'Tốt nghiệp' THEN 1 ELSE 0 END) AS TotNghiep,
        SUM(CASE WHEN sv.TinhTrang IN (N'Nghỉ học', N'Thôi học') THEN 1 ELSE 0 END) AS NgungHoc
    FROM Nganh n
    LEFT JOIN LopSinhHoat l ON n.MaNganh = l.MaNganh
    LEFT JOIN SinhVien sv ON l.MaLopSH = sv.MaLopSH
    WHERE n.MaKhoa = @maKhoa
    GROUP BY n.MaNganh, n.TenNganh
);
GO

-- =====================================================
-- FUNCTION 2: fn_TyLeXepLoai
-- =====================================================
CREATE OR ALTER FUNCTION fn_TyLeXepLoai (@maHocKy VARCHAR(10))
RETURNS TABLE
AS
RETURN
(
    WITH ThongKe AS (
        SELECT 
            XepLoai,
            COUNT(*) AS SoLuong
        FROM Diem d
        JOIN DangKy dk ON d.MaDK = dk.MaDK
        JOIN LopHocPhan lhp ON dk.MaLHP = lhp.MaLHP
        WHERE lhp.MaHocKy = @maHocKy
          AND d.XepLoai IS NOT NULL
          AND d.TrangThaiDiem = N'Đã khóa'
        GROUP BY XepLoai
    )
    SELECT 
        XepLoai,
        SoLuong,
        CAST(SoLuong * 100.0 / SUM(SoLuong) OVER() AS DECIMAL(5,2)) AS TyLePhanTram
    FROM ThongKe
);
GO

-- Kiểm tra function
SELECT * FROM fn_ThongKeSVTheoKhoa('CNTT')
GO
-- =====================================================
-- TRIGGER 1: trg_CapNhatHocPhi (ĐÃ SỬA HOÀN TOÀN)
-- =====================================================
CREATE OR ALTER TRIGGER trg_CapNhatHocPhi
ON DangKy
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DonGiaTinChi FLOAT = 350000;
    
    -- Cập nhật học phí cho các bản ghi đã tồn tại
    UPDATE hp
    SET 
        hp.SoTinChiDangKy = ISNULL((
            SELECT SUM(m.SoTinChi)
            FROM DangKy dk2
            INNER JOIN LopHocPhan lhp2 ON dk2.MaLHP = lhp2.MaLHP
            INNER JOIN MonHoc m ON lhp2.MaMon = m.MaMon
            WHERE dk2.MaSV = hp.MaSV 
              AND lhp2.MaHocKy = hp.MaHocKy
              AND dk2.TrangThai = N'Đã đăng ký'
        ), 0),
        hp.TongHocPhi = ISNULL((
            SELECT SUM(m.SoTinChi) * @DonGiaTinChi
            FROM DangKy dk2
            INNER JOIN LopHocPhan lhp2 ON dk2.MaLHP = lhp2.MaLHP
            INNER JOIN MonHoc m ON lhp2.MaMon = m.MaMon
            WHERE dk2.MaSV = hp.MaSV 
              AND lhp2.MaHocKy = hp.MaHocKy
              AND dk2.TrangThai = N'Đã đăng ký'
        ), 0)
    FROM HocPhi hp
    INNER JOIN inserted i ON hp.MaSV = i.MaSV
    INNER JOIN LopHocPhan lhp ON i.MaLHP = lhp.MaLHP
    WHERE hp.MaHocKy = lhp.MaHocKy;
    
    -- Thêm mới học phí cho những sinh viên chưa có trong học kỳ này
    -- SỬ DỤNG NOT EXISTS THAY VÌ IS NULL ĐỂ TRÁNH LỖI CỘT
    INSERT INTO HocPhi (MaSV, MaHocKy, SoTinChiDangKy, TongHocPhi, DaDong, TrangThai)
    SELECT 
        i.MaSV,
        lhp.MaHocKy,
        ISNULL((
            SELECT SUM(m.SoTinChi)
            FROM MonHoc m
            WHERE m.MaMon = lhp.MaMon
        ), 0),
        ISNULL((
            SELECT SUM(m.SoTinChi) * @DonGiaTinChi
            FROM MonHoc m
            WHERE m.MaMon = lhp.MaMon
        ), 0),
        0,
        N'Chưa đóng'
    FROM inserted i
    INNER JOIN LopHocPhan lhp ON i.MaLHP = lhp.MaLHP
    WHERE NOT EXISTS (
        SELECT 1 
        FROM HocPhi hp 
        WHERE hp.MaSV = i.MaSV 
          AND hp.MaHocKy = lhp.MaHocKy
    );
END
GO

PRINT N'Đã tạo trigger trg_CapNhatHocPhi';
GO

-- =====================================================
-- TRIGGER 2: trg_GiamHocPhiKhiHuy 
-- =====================================================
CREATE OR ALTER TRIGGER trg_GiamHocPhiKhiHuy
ON DangKy
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DonGiaTinChi FLOAT = 350000;
    
    IF UPDATE(TrangThai)
    BEGIN
        UPDATE hp
        SET 
            hp.SoTinChiDangKy = ISNULL((
                SELECT SUM(m.SoTinChi)
                FROM DangKy dk2
                INNER JOIN LopHocPhan lhp2 ON dk2.MaLHP = lhp2.MaLHP
                INNER JOIN MonHoc m ON lhp2.MaMon = m.MaMon
                WHERE dk2.MaSV = hp.MaSV 
                  AND lhp2.MaHocKy = hp.MaHocKy
                  AND dk2.TrangThai = N'Đã đăng ký'
            ), 0),
            hp.TongHocPhi = ISNULL((
                SELECT SUM(m.SoTinChi) * @DonGiaTinChi
                FROM DangKy dk2
                INNER JOIN LopHocPhan lhp2 ON dk2.MaLHP = lhp2.MaLHP
                INNER JOIN MonHoc m ON lhp2.MaMon = m.MaMon
                WHERE dk2.MaSV = hp.MaSV 
                  AND lhp2.MaHocKy = hp.MaHocKy
                  AND dk2.TrangThai = N'Đã đăng ký'
            ), 0),
            hp.TrangThai = CASE 
                WHEN hp.DaDong >= ISNULL((
                    SELECT SUM(m.SoTinChi) * @DonGiaTinChi
                    FROM DangKy dk2
                    INNER JOIN LopHocPhan lhp2 ON dk2.MaLHP = lhp2.MaLHP
                    INNER JOIN MonHoc m ON lhp2.MaMon = m.MaMon
                    WHERE dk2.MaSV = hp.MaSV 
                      AND lhp2.MaHocKy = hp.MaHocKy
                      AND dk2.TrangThai = N'Đã đăng ký'
                ), 0) THEN N'Đã đóng'
                WHEN hp.DaDong > 0 THEN N'Đóng một phần'
                ELSE N'Chưa đóng'
            END
        FROM HocPhi hp
        INNER JOIN deleted d ON hp.MaSV = d.MaSV
        INNER JOIN LopHocPhan lhp ON d.MaLHP = lhp.MaLHP
        WHERE hp.MaHocKy = lhp.MaHocKy
          AND d.TrangThai = N'Đã đăng ký'
          AND EXISTS (
              SELECT 1 
              FROM inserted i 
              WHERE i.MaDK = d.MaDK 
                AND i.TrangThai = N'Đã hủy'
          );
    END
END
GO

PRINT N'Đã tạo trigger trg_GiamHocPhiKhiHuy';
GO

-- Kiểm tra bảng HocPhi có tồn tại không
IF OBJECT_ID('HocPhi', 'U') IS NULL
BEGIN
    PRINT N'Đang tạo bảng HocPhi...';
    
    CREATE TABLE HocPhi (
        MaHocPhi       INT IDENTITY PRIMARY KEY,
        MaSV           VARCHAR(10)  NOT NULL,
        MaHocKy        VARCHAR(10)  NOT NULL,
        SoTinChiDangKy INT          NOT NULL DEFAULT 0,
        TongHocPhi     FLOAT        NOT NULL DEFAULT 0,
        DaDong         FLOAT        DEFAULT 0,
        TrangThai      NVARCHAR(20) DEFAULT N'Chưa đóng',
        NgayDong       DATETIME     NULL,
        FOREIGN KEY (MaSV)    REFERENCES SinhVien(MaSV),
        FOREIGN KEY (MaHocKy) REFERENCES HocKy(MaHocKy)
    );
    
    PRINT N'Đã tạo bảng HocPhi thành công!';
END
ELSE
BEGIN
    PRINT N'Bảng HocPhi đã tồn tại, kiểm tra cấu trúc...';
    
    -- Kiểm tra cấu trúc bảng
    SELECT COLUMN_NAME, DATA_TYPE 
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HocPhi'
    ORDER BY ORDINAL_POSITION;
END
GO

-- PROCEDURE: sp_ThongKeTongQuan
-- Mục đích: Lấy 4 số liệu tổng quan cho Dashboard
-- Sử dụng cho: BC08 - Dashboard tổng quan
-- =====================================================
CREATE OR ALTER PROCEDURE sp_ThongKeTongQuan
    @TongSinhVien INT OUTPUT,
    @TongGiangVien INT OUTPUT,
    @LopHocPhanDangMo INT OUTPUT,
    @TongMonHoc INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Tổng số sinh viên đang học
    SELECT @TongSinhVien = COUNT(*)
    FROM SinhVien
    WHERE TinhTrang = N'Đang học';
    
    -- Tổng số giảng viên
    SELECT @TongGiangVien = COUNT(*)
    FROM GiangVien;
    
    -- Số lớp học phần đang mở trong học kỳ hiện tại
    SELECT @LopHocPhanDangMo = COUNT(*)
    FROM LopHocPhan
    WHERE TrangThai = N'Đang mở';
    
    -- Tổng số môn học
    SELECT @TongMonHoc = COUNT(*)
    FROM MonHoc;
    
    -- Trả về kết quả dạng bảng (phục vụ hiển thị)
    SELECT 
        @TongSinhVien AS TongSinhVien,
        @TongGiangVien AS TongGiangVien,
        @LopHocPhanDangMo AS LopHocPhanDangMo,
        @TongMonHoc AS TongMonHoc;
END
GO

-- Kiểm tra procedure
DECLARE @sv INT, @gv INT, @lhp INT, @mh INT
EXEC sp_ThongKeTongQuan @sv OUTPUT, @gv OUTPUT, @lhp OUTPUT, @mh OUTPUT
SELECT @sv AS TongSinhVien, @gv AS TongGiangVien, @lhp AS LopHocPhanDangMo, @mh AS TongMonHoc


-- =====================================================
-- PROCEDURE: sp_BaoCaoHocPhi
-- Tham số: @maHocKy - Mã học kỳ cần báo cáo
-- Mục đích: Báo cáo chi tiết học phí theo học kỳ
-- Sử dụng cho: BC06 - Báo cáo học phí, xuất Excel
-- =====================================================
GO
CREATE OR ALTER PROCEDURE sp_BaoCaoHocPhi
    @MaHocKy VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        sv.MaSV,
        sv.HoTen,
        l.TenLop,
        n.TenNganh,
        k.TenKhoa,
        hp.SoTinChiDangKy,
        hp.TongHocPhi,
        hp.DaDong,
        (hp.TongHocPhi - hp.DaDong) AS ConLai,
        hp.TrangThai,
        CASE 
            WHEN hp.DaDong >= hp.TongHocPhi THEN N'Hoàn thành'
            WHEN hp.DaDong > 0 THEN N'Còn nợ'
            ELSE N'Chưa đóng'
        END AS TinhTrangThanhToan,
        hp.NgayDong
    FROM HocPhi hp
    JOIN SinhVien sv ON hp.MaSV = sv.MaSV
    JOIN LopSinhHoat l ON sv.MaLopSH = l.MaLopSH
    JOIN Nganh n ON l.MaNganh = n.MaNganh
    JOIN Khoa k ON n.MaKhoa = k.MaKhoa
    WHERE hp.MaHocKy = @MaHocKy
    ORDER BY k.TenKhoa, n.TenNganh, sv.HoTen;
    
    -- Tổng hợp chung
    SELECT 
        COUNT(*) AS TongSinhVien,
        SUM(hp.TongHocPhi) AS TongHocPhi,
        SUM(hp.DaDong) AS TongDaThu,
        SUM(hp.TongHocPhi - hp.DaDong) AS TongConNo,
        CAST(SUM(hp.DaDong) * 100.0 / NULLIF(SUM(hp.TongHocPhi), 0) AS DECIMAL(5,2)) AS TyLeThu
    FROM HocPhi hp
    WHERE hp.MaHocKy = @MaHocKy;
END
GO

-- Kiểm tra procedure
EXEC sp_BaoCaoHocPhi 'HK1-2023'
go
-- =====================================================
-- Thống kê số lượng sinh viên theo từng lớp sinh hoạt
-- Sử dụng cho: BC01 - Thống kê chi tiết
-- =====================================================
CREATE OR ALTER VIEW vw_ThongKeSVTheoLop
AS
SELECT 
    l.MaLopSH,
    l.TenLop,
    n.TenNganh,
    k.TenKhoa,
    kh.TenKhoaHoc,
    COUNT(sv.MaSV) AS SiSo,
    SUM(CASE WHEN sv.TinhTrang = N'Đang học' THEN 1 ELSE 0 END) AS SoLuongDangHoc,
    SUM(CASE WHEN sv.GioiTinh = 1 THEN 1 ELSE 0 END) AS SoLuongNam,
    SUM(CASE WHEN sv.GioiTinh = 0 THEN 1 ELSE 0 END) AS SoLuongNu
FROM LopSinhHoat l
LEFT JOIN SinhVien sv ON l.MaLopSH = sv.MaLopSH
JOIN Nganh n ON l.MaNganh = n.MaNganh
JOIN Khoa k ON n.MaKhoa = k.MaKhoa
JOIN KhoaHoc kh ON l.MaKhoaHoc = kh.MaKhoaHoc
GROUP BY l.MaLopSH, l.TenLop, n.TenNganh, k.TenKhoa, kh.TenKhoaHoc
GO


-- =====================================================
-- View thống kê điểm trung bình của từng sinh viên
-- Sử dụng cho: BC02 - Thống kê kết quả học tập
-- =====================================================
CREATE OR ALTER VIEW vw_DiemTrungBinhSinhVien
AS
SELECT 
    sv.MaSV,
    sv.HoTen,
    l.TenLop,
    n.TenNganh,
    AVG(d.DiemTongKet) AS DiemTrungBinh,
    MAX(CASE WHEN d.XepLoai IN (N'Xuất sắc', N'Giỏi') THEN 1 ELSE 0 END) AS DaDatGioi
FROM SinhVien sv
JOIN LopSinhHoat l ON sv.MaLopSH = l.MaLopSH
JOIN Nganh n ON l.MaNganh = n.MaNganh
LEFT JOIN DangKy dk ON sv.MaSV = dk.MaSV
LEFT JOIN Diem d ON dk.MaDK = d.MaDK AND d.TrangThaiDiem = N'Đã khóa'
GROUP BY sv.MaSV, sv.HoTen, l.TenLop, n.TenNganh
GO
------
------
USE QuanLySinhVien;
GO

USE QuanLySinhVien;
GO

-- Kiểm tra và thêm cột DiaChi nếu chưa có
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'SinhVien' AND COLUMN_NAME = 'DiaChi')
BEGIN
    ALTER TABLE SinhVien ADD DiaChi NVARCHAR(200) NULL;
    PRINT N'Đã thêm cột DiaChi vào bảng SinhVien';
END
ELSE
    PRINT N'Cột DiaChi đã tồn tại';
GO

-- =====================================================
-- PROCEDURE: sp_TimKiemSinhVienNangCao
-- Mục đích: Tìm kiếm sinh viên với nhiều tiêu chí lọc
-- Sử dụng cho: BC03 - Tìm kiếm nâng cao
-- =====================================================
CREATE OR ALTER PROCEDURE sp_TimKiemSinhVienNangCao
    @HoTen NVARCHAR(100) = NULL,
    @MaSV VARCHAR(10) = NULL,
    @MaLopSH VARCHAR(15) = NULL,
    @MaKhoa VARCHAR(10) = NULL,
    @MaNganh VARCHAR(10) = NULL,
    @TinhTrang NVARCHAR(20) = NULL,
    @DiemTu FLOAT = NULL,
    @DiemDen FLOAT = NULL,
    @HocKy VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT
        sv.MaSV,
        sv.HoTen,
        sv.NgaySinh,
        sv.GioiTinh,
        sv.DiaChi,
        l.TenLop,
        n.TenNganh,
        k.TenKhoa,
        sv.TinhTrang,
        AVG(d.DiemTongKet) OVER (PARTITION BY sv.MaSV) AS DiemTrungBinh
    FROM SinhVien sv
    JOIN LopSinhHoat l ON sv.MaLopSH = l.MaLopSH
    JOIN Nganh n ON l.MaNganh = n.MaNganh
    JOIN Khoa k ON n.MaKhoa = k.MaKhoa
    LEFT JOIN DangKy dk ON sv.MaSV = dk.MaSV
    LEFT JOIN Diem d ON dk.MaDK = d.MaDK AND d.TrangThaiDiem = N'Đã khóa'
    LEFT JOIN LopHocPhan lhp ON dk.MaLHP = lhp.MaLHP
    WHERE (@HoTen IS NULL OR sv.HoTen LIKE N'%' + @HoTen + '%')
      AND (@MaSV IS NULL OR sv.MaSV LIKE '%' + @MaSV + '%')
      AND (@MaLopSH IS NULL OR sv.MaLopSH = @MaLopSH)
      AND (@MaKhoa IS NULL OR n.MaKhoa = @MaKhoa)
      AND (@MaNganh IS NULL OR l.MaNganh = @MaNganh)
      AND (@TinhTrang IS NULL OR sv.TinhTrang = @TinhTrang)
      AND (@DiemTu IS NULL OR d.DiemTongKet >= @DiemTu)
      AND (@DiemDen IS NULL OR d.DiemTongKet <= @DiemDen)
      AND (@HocKy IS NULL OR lhp.MaHocKy = @HocKy)
    ORDER BY sv.HoTen;
END
GO

-- Kiểm tra tìm kiếm nâng cao
EXEC sp_TimKiemSinhVienNangCao @HoTen = N'Nguyễn', @DiemTu = 7.0, @DiemDen = 9.0


-- =====================================================
-- Dashboard tổng hợp đầy đủ (BC08)
-- =====================================================
go
CREATE OR ALTER PROCEDURE sp_DashboardTongHop
AS
BEGIN
    SET NOCOUNT ON;
    
    -- 1. Thống kê số lượng tổng quan
    SELECT 'TongSinhVien' AS ChiTieu, COUNT(*) AS SoLuong FROM SinhVien WHERE TinhTrang = N'Đang học'
    UNION ALL
    SELECT 'TongGiangVien', COUNT(*) FROM GiangVien
    UNION ALL
    SELECT 'LopHocPhanDangMo', COUNT(*) FROM LopHocPhan WHERE TrangThai = N'Đang mở'
    UNION ALL
    SELECT 'TongMonHoc', COUNT(*) FROM MonHoc;
    
    -- 2. Thống kê sinh viên theo tình trạng
    SELECT TinhTrang, COUNT(*) AS SoLuong 
    FROM SinhVien 
    GROUP BY TinhTrang;
    
    -- 3. Top 5 môn học có tỷ lệ đậu cao nhất
    SELECT TOP 5 
        m.TenMon,
        COUNT(CASE WHEN d.DiemTongKet >= 5 THEN 1 END) * 100.0 / COUNT(*) AS TyLeDau
    FROM MonHoc m
    JOIN LopHocPhan lhp ON m.MaMon = lhp.MaMon
    JOIN DangKy dk ON lhp.MaLHP = dk.MaLHP
    JOIN Diem d ON dk.MaDK = d.MaDK
    WHERE d.TrangThaiDiem = N'Đã khóa'
    GROUP BY m.TenMon
    ORDER BY TyLeDau DESC;
    
    -- 4. Thống kê học phí theo học kỳ
    SELECT 
        hk.TenHocKy,
        hk.NamHoc,
        SUM(hp.TongHocPhi) AS TongHocPhi,
        SUM(hp.DaDong) AS DaThu,
        SUM(hp.TongHocPhi - hp.DaDong) AS ConNo
    FROM HocPhi hp
    JOIN HocKy hk ON hp.MaHocKy = hk.MaHocKy
    GROUP BY hk.TenHocKy, hk.NamHoc
    ORDER BY hk.NamHoc DESC, hk.TenHocKy;
END
GO

-- Kiểm tra dashboard
EXEC sp_DashboardTongHop

-- =====================================================
-- Kiểm tra tất cả các đối tượng của Thành viên 5
-- =====================================================

-- 1. Kiểm tra Views
SELECT name, type_desc FROM sys.views 
WHERE name IN ('vw_ThongKeSinhVienTheoKhoa', 'vw_KetQuaHocTapTheoKy', 'vw_ThongKeSVTheoLop', 'vw_DiemTrungBinhSinhVien');

-- 2. Kiểm tra Triggers
SELECT name, type_desc, parent_class_desc FROM sys.triggers 
WHERE name IN ('trg_CapNhatHocPhi', 'trg_GiamHocPhiKhiHuy');

-- 3. Kiểm tra Functions
SELECT name, type_desc FROM sys.objects 
WHERE name IN ('fn_ThongKeSVTheoKhoa', 'fn_TyLeXepLoai') AND type IN ('IF', 'FN');

-- 4. Kiểm tra Procedures
SELECT name, type_desc FROM sys.procedures 
WHERE name IN ('sp_ThongKeTongQuan', 'sp_BaoCaoHocPhi', 'sp_TimKiemSinhVienNangCao', 'sp_DashboardTongHop');