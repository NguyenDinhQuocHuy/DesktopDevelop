USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'QuanLyDichVuKhachSan')
BEGIN
    ALTER DATABASE QuanLyDichVuKhachSan SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyDichVuKhachSan;
END
GO
CREATE DATABASE QuanLyDichVuKhachSan;
GO

USE QuanLyDichVuKhachSan;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- =====================================================================================
-- PHẦN 1: ĐỊNH NGHĨA KIỂU DỮ LIỆU TỰ ĐỊNH NGHĨA (USER-DEFINED TABLE TYPES)
-- =====================================================================================

IF TYPE_ID(N'kieu_ChiTietNhapKho') IS NOT NULL DROP TYPE kieu_ChiTietNhapKho;
GO
CREATE TYPE kieu_ChiTietNhapKho AS TABLE (
    MonAnId     INT NOT NULL,
    DonViNhap   NVARCHAR(50) NOT NULL,
    HeSoQuyDoi  INT NOT NULL,
    SoLuongNhap INT NOT NULL,
    DonGiaNhap  DECIMAL(18,2) NOT NULL,
    SoLo        VARCHAR(50) NULL,
    HanSuDung   DATE NULL,
    GhiChu      NVARCHAR(255) NULL
);
GO


-- =====================================================================================
-- PHẦN 2: TẠO TẤT CẢ CÁC BẢNG (TABLES, CONSTRAINTS & INDEXES)
-- =====================================================================================

-- 2.1 Tài khoản đăng nhập (Admin / Lễ tân)
IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL DROP TABLE TaiKhoan;
CREATE TABLE TaiKhoan (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap     NVARCHAR(50)  NOT NULL UNIQUE,
    MatKhauHash     NVARCHAR(255) NOT NULL,
    HoTen           NVARCHAR(100) NOT NULL,
    VaiTro          NVARCHAR(20)  NOT NULL CHECK (VaiTro IN (N'Admin', N'LeTan')),
    SoDienThoai     NVARCHAR(20)  NULL,
    Email           NVARCHAR(100) NULL,
    TrangThai       BIT NOT NULL DEFAULT 1,
    NgayTao         DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- 2.2 Nhân viên & Ca trực
IF OBJECT_ID('NhanVien', 'U') IS NOT NULL DROP TABLE NhanVien;
CREATE TABLE NhanVien (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    MaNV            NVARCHAR(20)  NOT NULL UNIQUE,
    HoTen           NVARCHAR(100) NOT NULL,
    CCCD            NVARCHAR(20)  NOT NULL UNIQUE,
    SoDienThoai     NVARCHAR(20)  NOT NULL,
    Email           NVARCHAR(100) NULL,
    ChucVu          NVARCHAR(50)  NOT NULL,
    LuongCoBan      DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (LuongCoBan >= 0),
    TrangThai       NVARCHAR(30)  NOT NULL DEFAULT N'Đang làm việc'
                        CHECK (TrangThai IN (N'Đang làm việc', N'Nghỉ phép', N'Đã nghỉ'))
);
GO

IF OBJECT_ID('CaTruc', 'U') IS NOT NULL DROP TABLE CaTruc;
CREATE TABLE CaTruc (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    TenCa       NVARCHAR(50) NOT NULL UNIQUE,
    GioBatDau   TIME NOT NULL,
    GioKetThuc  TIME NOT NULL,
    GhiChu      NVARCHAR(255) NULL
);
GO

IF OBJECT_ID('PhanCongCaTruc', 'U') IS NOT NULL DROP TABLE PhanCongCaTruc;
CREATE TABLE PhanCongCaTruc (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    NhanVienId  INT NOT NULL FOREIGN KEY REFERENCES NhanVien(Id) ON DELETE CASCADE,
    CaTrucId    INT NOT NULL FOREIGN KEY REFERENCES CaTruc(Id),
    NgayTruc    DATE NOT NULL,
    GhiChu      NVARCHAR(255) NULL,
    CONSTRAINT UQ_PhanCong UNIQUE (NhanVienId, CaTrucId, NgayTruc)
);
GO

-- 2.3 Danh mục dịch vụ hệ thống & Cấu hình tham số
IF OBJECT_ID('DichVuHeThong', 'U') IS NOT NULL DROP TABLE DichVuHeThong;
CREATE TABLE DichVuHeThong (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    MaDV            NVARCHAR(30)  NOT NULL UNIQUE,
    TenDV           NVARCHAR(100) NOT NULL,
    LoaiDichVu      NVARCHAR(30)  NOT NULL CHECK (LoaiDichVu IN (N'Tự túc', N'Dịch vụ ngoài')),
    DangHoatDong    BIT NOT NULL DEFAULT 1,
    MoTa            NVARCHAR(255) NULL
);
GO

IF OBJECT_ID('ThamSoHeThong', 'U') IS NOT NULL DROP TABLE ThamSoHeThong;
CREATE TABLE ThamSoHeThong (
    MaThamSo     VARCHAR(50) PRIMARY KEY,
    TenThamSo    NVARCHAR(100) NOT NULL,
    GiaTri       NVARCHAR(100) NOT NULL,
    MoTa         NVARCHAR(255) NULL
);
GO

-- 2.4 Sơ đồ phòng khách sạn
IF OBJECT_ID('PhongKhachSan', 'U') IS NOT NULL DROP TABLE PhongKhachSan;
CREATE TABLE PhongKhachSan (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    SoPhong      NVARCHAR(20) NOT NULL UNIQUE,
    Tang         INT NOT NULL,
    LoaiPhong    NVARCHAR(50) NOT NULL,
    TrangThai    NVARCHAR(30) NOT NULL DEFAULT N'Trống'
                     CHECK (TrangThai IN (N'Trống', N'Đang ở', N'Đang dọn', N'Bảo trì')),
    TenKhach     NVARCHAR(100) NULL,
    SoDienThoai  NVARCHAR(20)  NULL,
    CCCD         NVARCHAR(20)  NULL,
    GhiChu       NVARCHAR(255) NULL,
    NgayNhanPhong DATETIME NULL,
    NgayTraPhong  DATETIME NULL
);
GO

-- 2.5 Hồ sơ khách hàng
IF OBJECT_ID('KhachHang', 'U') IS NOT NULL DROP TABLE KhachHang;
CREATE TABLE KhachHang (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    HoTen           NVARCHAR(100) NOT NULL,
    CCCD            NVARCHAR(20)  NULL,
    SoDienThoai     NVARCHAR(20)  NOT NULL,
    SoPhong         NVARCHAR(20)  NULL,
    GhiChu          NVARCHAR(255) NULL,
    NgayTao         DATETIME NOT NULL DEFAULT GETDATE()
);
GO
CREATE INDEX IX_KhachHang_CCCD ON KhachHang(CCCD);
CREATE INDEX IX_KhachHang_SDT  ON KhachHang(SoDienThoai);
GO

-- 2.6 Nhà cung cấp, Món ăn minibar, Nhập kho
IF OBJECT_ID('NhaCungCap', 'U') IS NOT NULL DROP TABLE NhaCungCap;
CREATE TABLE NhaCungCap (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenNhaCungCap   NVARCHAR(100) NOT NULL UNIQUE,
    NguoiLienHe     NVARCHAR(100) NULL,
    SoDienThoai     NVARCHAR(20)  NOT NULL,
    DiaChi          NVARCHAR(255) NULL,
    Email           NVARCHAR(100) NULL,
    GhiChu          NVARCHAR(255) NULL,
    TrangThai       BIT NOT NULL DEFAULT 1
);
GO

IF OBJECT_ID('MonAn', 'U') IS NOT NULL DROP TABLE MonAn;
CREATE TABLE MonAn (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenMon          NVARCHAR(100) NOT NULL UNIQUE,
    DanhMuc         NVARCHAR(50)  NOT NULL,
    DonViTinh       NVARCHAR(20)  NOT NULL,
    GiaBan          DECIMAL(18,2) NOT NULL CHECK (GiaBan >= 0),
    GiaVonBinhQuan  DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (GiaVonBinhQuan >= 0),
    SoLuongTonKho   INT           NOT NULL DEFAULT 0 CHECK (SoLuongTonKho >= 0),
    MucTonToiThieu  INT           NOT NULL DEFAULT 10,
    HinhAnh         NVARCHAR(500) NULL,
    TrangThai       NVARCHAR(30)  NOT NULL DEFAULT N'Đang phục vụ'
                        CHECK (TrangThai IN (N'Đang phục vụ', N'Còn hàng', N'Hết hàng', N'Ngừng kinh doanh', N'Đã xóa')),
    NhaCungCapId    INT           NULL FOREIGN KEY REFERENCES NhaCungCap(Id),
    GhiChu          NVARCHAR(255) NULL,
    DonViNhapMacDinh NVARCHAR(50) NULL DEFAULT N'Thùng',
    HeSoQuyDoiMacDinh INT         NULL DEFAULT 24
);
GO

IF OBJECT_ID('PhieuNhapKho', 'U') IS NOT NULL DROP TABLE PhieuNhapKho;
CREATE TABLE PhieuNhapKho (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuNhap     NVARCHAR(50)  NOT NULL UNIQUE,
    NhaCungCapId    INT           NOT NULL FOREIGN KEY REFERENCES NhaCungCap(Id),
    NguoiNhapId     INT           NOT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    NgayNhap        DATETIME      NOT NULL DEFAULT GETDATE(),
    TongTien        DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TongTien >= 0),
    GhiChu          NVARCHAR(255) NULL
);
GO

IF OBJECT_ID('ChiTietPhieuNhapKho', 'U') IS NOT NULL DROP TABLE ChiTietPhieuNhapKho;
CREATE TABLE ChiTietPhieuNhapKho (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    PhieuNhapId     INT NOT NULL FOREIGN KEY REFERENCES PhieuNhapKho(Id) ON DELETE CASCADE,
    MonAnId         INT NOT NULL FOREIGN KEY REFERENCES MonAn(Id),
    DonViNhap       NVARCHAR(50) NOT NULL,
    HeSoQuyDoi      INT NOT NULL DEFAULT 1 CHECK (HeSoQuyDoi >= 1),
    SoLuongNhap     INT NOT NULL CHECK (SoLuongNhap > 0),
    SoLuongCoSo     AS (SoLuongNhap * HeSoQuyDoi) PERSISTED,
    DonGiaNhap      DECIMAL(18,2) NOT NULL CHECK (DonGiaNhap >= 0),
    ThanhTien       AS (SoLuongNhap * DonGiaNhap) PERSISTED,
    GiaVonCoSo      AS (DonGiaNhap / HeSoQuyDoi) PERSISTED,
    SoLo            VARCHAR(50) NULL,
    HanSuDung       DATE NULL,
    GhiChu          NVARCHAR(255) NULL
);
GO

-- 2.7 Đơn hàng ẩm thực & minibar (Phương thức ghi nợ / Thời điểm thanh toán PMS chuẩn)
IF OBJECT_ID('DonHangMonAn', 'U') IS NOT NULL DROP TABLE DonHangMonAn;
CREATE TABLE DonHangMonAn (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    MaDon               NVARCHAR(30) NOT NULL UNIQUE,
    KhachHangId         INT NULL FOREIGN KEY REFERENCES KhachHang(Id),
    TenKhach            NVARCHAR(100) NOT NULL,
    SoPhong             NVARCHAR(20) NULL,
    -- Phương thức ghi nợ: Thanh toán trực tiếp (Direct Payment) vs Ghi nợ vào phòng (Charge to Room)
    HinhThucThanhToan   NVARCHAR(50) NOT NULL DEFAULT N'Ghi nợ vào phòng'
                            CHECK (HinhThucThanhToan IN (N'Thanh toán trực tiếp', N'Ghi nợ vào phòng', N'Hóa đơn riêng', N'Tính chung tiền phòng')),
    TongTien            DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TongTien >= 0),
    DaThanhToan         BIT NOT NULL DEFAULT 0,
    GhiChu              NVARCHAR(255) NULL,
    NguoiTaoId          INT NOT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    NguoiThanhToanId    INT NULL     FOREIGN KEY REFERENCES NhanVien(Id),
    ThoiGianThanhToan   DATETIME NULL,
    NgayTao             DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai           NVARCHAR(30) NOT NULL DEFAULT N'Chờ xử lý'
                            CHECK (TrangThai IN (N'Chờ xử lý', N'Đang chuẩn bị', N'Đã giao', N'Đã hủy')),
    CONSTRAINT CK_DonHang_XacNhanTT CHECK (
        (DaThanhToan = 0 AND NguoiThanhToanId IS NULL AND ThoiGianThanhToan IS NULL) OR
        (DaThanhToan = 1 AND NguoiThanhToanId IS NOT NULL AND ThoiGianThanhToan IS NOT NULL)
    )
);
GO

IF OBJECT_ID('ChiTietDonHangMonAn', 'U') IS NOT NULL DROP TABLE ChiTietDonHangMonAn;
CREATE TABLE ChiTietDonHangMonAn (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    DonHangId   INT NOT NULL FOREIGN KEY REFERENCES DonHangMonAn(Id) ON DELETE CASCADE,
    MonAnId     INT NOT NULL FOREIGN KEY REFERENCES MonAn(Id),
    SoLuong     INT NOT NULL CHECK (SoLuong > 0),
    DonGia      DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    ThanhTien   AS (SoLuong * DonGia) PERSISTED,
    GiaVon      DECIMAL(18,2) NOT NULL DEFAULT 0,
    LoiNhuan    AS (SoLuong * DonGia - SoLuong * GiaVon) PERSISTED,
    GhiChu      NVARCHAR(255) NULL
);
GO

-- 2.8 Cho thuê sảnh & Hội nghị sự kiện
IF OBJECT_ID('KhuVucSuKien', 'U') IS NOT NULL DROP TABLE KhuVucSuKien;
CREATE TABLE KhuVucSuKien (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenKhuVuc       NVARCHAR(100) NOT NULL UNIQUE,
    LoaiKhuVuc      NVARCHAR(50)  NOT NULL,
    SucChua         INT           NOT NULL CHECK (SucChua > 0),
    GiaThueTheoGio  DECIMAL(18,2) NOT NULL CHECK (GiaThueTheoGio >= 0),
    TrangThietBi    NVARCHAR(500) NULL,
    -- FIX LOGIC QUAN TRỌNG: TrangThai trước đây có 'Trống/Đang sử dụng/Bảo trì/Đang dọn' —
    -- SAI MÔ HÌNH vì 1 khu vực có thể có NHIỀU đơn đặt trước ở NHIỀU khung giờ khác nhau
    -- trong tương lai (đúng như UI "2. Thời khóa biểu" đã hiển thị theo từng khung giờ).
    -- "Trống"/"Đang sử dụng"/"Đang dọn" là trạng thái phụ thuộc THỜI ĐIỂM truy vấn, không
    -- thể lưu tĩnh 1 giá trị duy nhất cho cả khu vực — lưu tĩnh sẽ luôn bị sai/lệch.
    -- Cột này giờ CHỈ còn phản ánh việc khu vực có được đưa vào khai thác hay không (không
    -- phụ thuộc thời gian). Trạng thái tại 1 thời điểm cụ thể PHẢI tính động qua
    -- vw_TrangThaiKhuVucHienTai (join với DonDatSuKien theo giờ hiện tại).
    TrangThai       NVARCHAR(30)  NOT NULL DEFAULT N'Hoạt động'
                        CHECK (TrangThai IN (N'Hoạt động', N'Ngừng khai thác'))
);
GO

IF OBJECT_ID('DonDatSuKien', 'U') IS NOT NULL DROP TABLE DonDatSuKien;
CREATE TABLE DonDatSuKien (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    MaDon               NVARCHAR(30) NOT NULL UNIQUE,
    KhuVucId            INT NOT NULL FOREIGN KEY REFERENCES KhuVucSuKien(Id),
    KhachHangId         INT NULL FOREIGN KEY REFERENCES KhachHang(Id),
    TenKhach            NVARCHAR(100) NOT NULL,
    SoDienThoai         NVARCHAR(20) NOT NULL,
    SoPhong             NVARCHAR(20) NULL,
    ThoiGianBatDau      DATETIME NOT NULL,
    ThoiGianKetThuc     DATETIME NOT NULL,

    -- FIX (bổ sung theo yêu cầu): thời gian dọn dẹp SAU khi kết thúc trước khi khu vực có thể
    -- nhận khách kế tiếp. NULL = dùng mặc định hệ thống (ThamSoHeThong.EVENT_CLEANING_BUFFER_MINUTES).
    -- Điền số cụ thể (VD 0 hoặc nhỏ hơn mặc định) để RÚT NGẮN riêng cho đơn này, kèm phụ thu.
    ThoiGianDonDepPhut      INT NULL CHECK (ThoiGianDonDepPhut IS NULL OR ThoiGianDonDepPhut >= 0),
    PhuThuRutNganDonDep     DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (PhuThuRutNganDonDep >= 0),
    GhiChuRutNganDonDep     NVARCHAR(255) NULL,

    TienCoc             DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TienCoc >= 0),
    TongTienDuKien      DECIMAL(18,2) NOT NULL CHECK (TongTienDuKien >= 0),
    ChiPhiPhatSinh      DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (ChiPhiPhatSinh >= 0),
    DaThanhToan         BIT NOT NULL DEFAULT 0,
    GhiChuHuHai         NVARCHAR(500) NULL,

    -- FIX LOGIC QUAN TRỌNG NHẤT: bản cũ dùng 1 cột "TrangThaiThanhToan" trộn lẫn 2 khái niệm
    -- hoàn toàn khác nhau (hình thức thu tiền lúc đặt, VÀ vòng đời sử dụng thực tế của đơn),
    -- khiến KHÔNG THỂ biểu diễn "Đang sử dụng" hay "Đã hủy" — nay tách thành 2 cột riêng:

    -- (1) Hình thức thu tiền lúc đặt (cọc trước hay trả đủ luôn)
    HinhThucThanhToanBanDau NVARCHAR(20) NOT NULL DEFAULT N'Đặt cọc trước'
                                CHECK (HinhThucThanhToanBanDau IN (N'Đặt cọc trước', N'Thanh toán toàn bộ')),

    -- (2) Vòng đời sử dụng thực tế — ĐÂY LÀ PHẦN BỊ THIẾU HOÀN TOÀN TRONG BẢN CŨ
    TrangThaiDon        NVARCHAR(30) NOT NULL DEFAULT N'Đã đặt'
                            CHECK (TrangThaiDon IN (
                                N'Đã đặt', N'Đang sử dụng', N'Hoàn tất',
                                N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')),

    -- Thông tin hủy đơn (trả lời đúng yêu cầu: hủy trước 1 ngày mới được hoàn cọc)
    ThoiGianHuy         DATETIME NULL,
    LyDoHuy             NVARCHAR(255) NULL,
    NguoiHuyId          INT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    SoTienHoanCoc       DECIMAL(18,2) NULL,

    NguoiTaoId          INT NOT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    NguoiThanhToanId    INT NULL     FOREIGN KEY REFERENCES NhanVien(Id),
    ThoiGianThanhToan   DATETIME NULL,
    NgayTao             DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_SuKien_ThoiGian CHECK (ThoiGianKetThuc > ThoiGianBatDau),
    CONSTRAINT CK_DonDatSuKien_TienCoc CHECK (TienCoc <= TongTienDuKien),
    CONSTRAINT CK_DonDatSuKien_XacNhanTT CHECK (
        (DaThanhToan = 0 AND NguoiThanhToanId IS NULL AND ThoiGianThanhToan IS NULL) OR
        (DaThanhToan = 1 AND NguoiThanhToanId IS NOT NULL AND ThoiGianThanhToan IS NOT NULL)
    ),
    -- FIX: đơn đã hủy bắt buộc phải có đủ thông tin hủy; đơn chưa hủy thì không được có
    CONSTRAINT CK_DonDatSuKien_Huy CHECK (
        (TrangThaiDon NOT IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
            AND ThoiGianHuy IS NULL AND NguoiHuyId IS NULL AND SoTienHoanCoc IS NULL) OR
        (TrangThaiDon IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
            AND ThoiGianHuy IS NOT NULL AND NguoiHuyId IS NOT NULL AND SoTienHoanCoc IS NOT NULL)
    )
);
GO
CREATE INDEX IX_DonDatSuKien_KhuVuc_ThoiGian ON DonDatSuKien(KhuVucId, ThoiGianBatDau, ThoiGianKetThuc);
GO

-- 2.9 Dịch vụ cho thuê xe máy & ô tô (XeChoThue và DonThueXe / DonDatXeChoThue)
-- Bảng quản lý danh mục xe
IF OBJECT_ID('DonDatXeChoThue', 'V') IS NOT NULL DROP VIEW DonDatXeChoThue;
IF OBJECT_ID('DonThueXe', 'U') IS NOT NULL DROP TABLE DonThueXe;
IF OBJECT_ID('XeChoThue', 'U') IS NOT NULL DROP TABLE XeChoThue;
CREATE TABLE XeChoThue (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    BienSo              NVARCHAR(20) NOT NULL UNIQUE,
    TenXe               NVARCHAR(100) NOT NULL,
    LoaiXe              NVARCHAR(30) NOT NULL,
    GiaThueNgay         DECIMAL(18,2) NOT NULL CHECK (GiaThueNgay >= 0),
    TienCocQuyDinh      DECIMAL(18,2) NOT NULL DEFAULT 500000 CHECK (TienCocQuyDinh >= 0),
    -- FIX LOGIC: Tương tự KhuVucSuKien, TrangThai của xe chỉ lưu trạng thái kỹ thuật/vận hành
    -- ('Hoạt động' / 'Sẵn sàng', 'Bảo trì', 'Ngừng khai thác'). Trạng thái thực tế tại từng thời điểm
    -- (Trống/Sẵn sàng, Đang thuê, Đang vệ sinh kiểm tra...) được tính ĐỘNG qua vw_TrangThaiXeHienTai theo GETDATE().
    TrangThai           NVARCHAR(30) NOT NULL DEFAULT N'Hoạt động'
                            CHECK (TrangThai IN (N'Hoạt động', N'Sẵn sàng', N'Bảo trì', N'Ngừng khai thác')),
    GhiChuTinhTrang     NVARCHAR(255) NULL
);
GO

-- Bảng đơn đặt & thuê xe (mô hình chuẩn hóa tương đương DonDatSuKien)
CREATE TABLE DonThueXe (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,
    MaDon                   NVARCHAR(30) NOT NULL UNIQUE,
    XeId                    INT NOT NULL FOREIGN KEY REFERENCES XeChoThue(Id),
    KhachHangId             INT NULL FOREIGN KEY REFERENCES KhachHang(Id),
    TenKhach                NVARCHAR(100) NOT NULL,
    CCCD                    NVARCHAR(20) NOT NULL,
    SoDienThoai             NVARCHAR(20) NOT NULL,
    SoPhong                 NVARCHAR(20) NULL,
    NgayThue                DATETIME NOT NULL,
    NgayTraDuKien           DATETIME NOT NULL,
    NgayTraThucTe           DATETIME NULL,

    -- Thời gian vệ sinh & kiểm tra xe SAU khi khách trả trước khi bàn giao cho lượt thuê tiếp theo.
    -- NULL = mặc định từ ThamSoHeThong.VEHICLE_CLEANING_BUFFER_MINUTES (30 phút).
    -- Có thể điền số cụ thể (VD 0 phút hoặc ngắn hơn) kèm phụ thu nếu khách nhận xe gấp.
    ThoiGianKiemTraXePhut   INT NULL CHECK (ThoiGianKiemTraXePhut IS NULL OR ThoiGianKiemTraXePhut >= 0),
    PhuThuRutNganBuffer     DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (PhuThuRutNganBuffer >= 0),
    GhiChuRutNganBuffer     NVARCHAR(255) NULL,

    TienCoc                 DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TienCoc >= 0),
    TienThue                DECIMAL(18,2) NOT NULL CHECK (TienThue >= 0),
    ChiPhiPhatSinh          DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (ChiPhiPhatSinh >= 0),
    TongTienThanhToan       DECIMAL(18,2) NOT NULL DEFAULT 0,
    DaThanhToan             BIT NOT NULL DEFAULT 0,
    GhiChuKhiNhanXe         NVARCHAR(500) NULL,
    GhiChuHuHai             NVARCHAR(500) NULL,

    -- (1) Hình thức thanh toán lúc đặt xe
    HinhThucThanhToanBanDau NVARCHAR(20) NOT NULL DEFAULT N'Đặt cọc trước'
                                CHECK (HinhThucThanhToanBanDau IN (N'Đặt cọc trước', N'Thanh toán toàn bộ')),

    -- (2) Vòng đời sử dụng thực tế (Chuẩn hóa giống đặt sự kiện, bên sự kiện là 'Đang sử dụng', xe là 'Đang thuê')
    TrangThaiDon            NVARCHAR(30) NOT NULL DEFAULT N'Đã đặt'
                                CHECK (TrangThaiDon IN (
                                    N'Đã đặt', N'Đang thuê', N'Hoàn tất',
                                    N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')),

    -- Thông tin hủy đơn & hoàn cọc (Quy tắc như sự kiện: hủy trước ngày thuê tính theo ngày lịch được hoàn cọc)
    ThoiGianHuy             DATETIME NULL,
    LyDoHuy                 NVARCHAR(255) NULL,
    NguoiHuyId              INT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    SoTienHoanCoc           DECIMAL(18,2) NULL,

    NguoiTaoId              INT NOT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    NguoiThanhToanId        INT NULL     FOREIGN KEY REFERENCES NhanVien(Id),
    ThoiGianThanhToan       DATETIME NULL,
    NgayTao                 DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_ThueXe_NgayTra CHECK (NgayTraDuKien > NgayThue),
    CONSTRAINT CK_DonThueXe_TienCoc CHECK (TienCoc <= TienThue),
    CONSTRAINT CK_DonThueXe_XacNhanTT CHECK (
        (DaThanhToan = 0 AND NguoiThanhToanId IS NULL AND ThoiGianThanhToan IS NULL) OR
        (DaThanhToan = 1 AND NguoiThanhToanId IS NOT NULL AND ThoiGianThanhToan IS NOT NULL)
    ),
    CONSTRAINT CK_DonThueXe_Huy CHECK (
        (TrangThaiDon NOT IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
            AND ThoiGianHuy IS NULL AND NguoiHuyId IS NULL AND SoTienHoanCoc IS NULL) OR
        (TrangThaiDon IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
            AND ThoiGianHuy IS NOT NULL AND NguoiHuyId IS NOT NULL AND SoTienHoanCoc IS NOT NULL)
    )
);
GO
CREATE INDEX IX_DonThueXe_Xe_ThoiGian ON DonThueXe(XeId, NgayThue, NgayTraDuKien);
GO

-- View DonDatXeChoThue: đảm bảo cả 2 tên gọi DonThueXe và DonDatXeChoThue đều truy vấn đồng nhất
IF OBJECT_ID('DonDatXeChoThue', 'V') IS NOT NULL DROP VIEW DonDatXeChoThue;
GO
CREATE VIEW DonDatXeChoThue AS SELECT * FROM DonThueXe;
GO

-- 2.10 Bãi đỗ xe
IF OBJECT_ID('BaiDoXe', 'U') IS NOT NULL DROP TABLE BaiDoXe;
CREATE TABLE BaiDoXe (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    MaVe                NVARCHAR(30) NOT NULL UNIQUE,
    BienSoXe            NVARCHAR(20) NOT NULL,
    LoaiXe              NVARCHAR(30) NOT NULL CHECK (LoaiXe IN (N'Xe máy', N'Ô tô')),
    TenKhach            NVARCHAR(100) NULL,
    SoDienThoai         NVARCHAR(20) NULL,
    SoPhong             NVARCHAR(20) NULL,
    ThoiGianVao         DATETIME NOT NULL DEFAULT GETDATE(),
    ThoiGianRa          DATETIME NULL,
    HinhThucGui         NVARCHAR(30) NOT NULL DEFAULT N'Theo lượt'
                            CHECK (HinhThucGui IN (N'Theo lượt', N'Theo tiếng', N'Theo giờ', N'Theo ngày', N'Khách phòng (Miễn phí)')),
    PhiGui              DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (PhiGui >= 0),
    DaThanhToan         BIT NOT NULL DEFAULT 0,
    GhiChuSuaChua       NVARCHAR(255) NULL,
    NguoiTaoId          INT NOT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    NguoiThanhToanId    INT NULL     FOREIGN KEY REFERENCES NhanVien(Id),
    ThoiGianThanhToan   DATETIME NULL,
    CONSTRAINT CK_BaiDoXe_XacNhanTT CHECK (
        (DaThanhToan = 0 AND NguoiThanhToanId IS NULL AND ThoiGianThanhToan IS NULL) OR
        (DaThanhToan = 1 AND NguoiThanhToanId IS NOT NULL AND ThoiGianThanhToan IS NOT NULL)
    )
);
GO

-- 2.11 Bảng giá chung loại hình dịch vụ giặt ủi & Đối tác liên kết
IF OBJECT_ID('DonGiatUi', 'U') IS NOT NULL DROP TABLE DonGiatUi;
IF OBJECT_ID('BangGiaDoiTacGiatUi', 'U') IS NOT NULL DROP TABLE BangGiaDoiTacGiatUi;
IF OBJECT_ID('DoiTacGiatUi', 'U') IS NOT NULL DROP TABLE DoiTacGiatUi;
IF OBJECT_ID('LoaiDichVuGiatUi', 'U') IS NOT NULL DROP TABLE LoaiDichVuGiatUi;
GO

CREATE TABLE LoaiDichVuGiatUi (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    MaLoai              NVARCHAR(30)  NOT NULL UNIQUE,
    TenLoai             NVARCHAR(100) NOT NULL UNIQUE,
    DonGiaBanKg         DECIMAL(18,2) NOT NULL CHECK (DonGiaBanKg > 0),
    DonViTinh           NVARCHAR(20)  NOT NULL DEFAULT N'Kg',
    HeSoBoiThuongToiDa  INT           NOT NULL DEFAULT 10 CHECK (HeSoBoiThuongToiDa > 0),
    DangKinhDoanh       BIT           NOT NULL DEFAULT 1
);
GO

CREATE TABLE DoiTacGiatUi (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    TenDoiTac           NVARCHAR(100) NOT NULL UNIQUE,
    DiaChi              NVARCHAR(255) NULL,
    SoDienThoai         NVARCHAR(20)  NOT NULL,
    HanThanhToanCongNo  NVARCHAR(30)  NOT NULL DEFAULT N'Theo tháng'
                            CHECK (HanThanhToanCongNo IN (N'Theo tháng', N'Theo tuần', N'Theo đơn')),
    TyLeKhachSanHuong   DECIMAL(5,2)  NULL DEFAULT 30.00,
    TyLeDoiTacHuong     DECIMAL(5,2)  NULL DEFAULT 70.00,
    TrangThai           BIT           NOT NULL DEFAULT 1
);
GO

-- Bảng giá vốn theo từng đối tác và loại dịch vụ (Mô hình giá vốn thực tế)
CREATE TABLE BangGiaDoiTacGiatUi (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    DoiTacId            INT NOT NULL FOREIGN KEY REFERENCES DoiTacGiatUi(Id) ON DELETE CASCADE,
    LoaiDichVuId        INT NOT NULL FOREIGN KEY REFERENCES LoaiDichVuGiatUi(Id) ON DELETE CASCADE,
    DonGiaVonKg         DECIMAL(18,2) NOT NULL CHECK (DonGiaVonKg >= 0),
    GhiChu              NVARCHAR(255) NULL,
    CONSTRAINT UQ_BangGiaDoiTac UNIQUE (DoiTacId, LoaiDichVuId)
);
GO

CREATE TABLE DonGiatUi (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,
    MaDon                   NVARCHAR(30) NOT NULL UNIQUE,
    KhachHangId             INT NULL FOREIGN KEY REFERENCES KhachHang(Id),
    TenKhach                NVARCHAR(100) NOT NULL,
    SoDienThoai             NVARCHAR(20) NOT NULL,
    SoPhong                 NVARCHAR(20) NULL,
    LoaiDichVu              NVARCHAR(50) NOT NULL,
    KhoiLuongKg             DECIMAL(6,2) NOT NULL CHECK (KhoiLuongKg > 0),
    DonGiaKg                DECIMAL(18,2) NOT NULL CHECK (DonGiaKg >= 0), -- Khóa giá bán lúc nhận đồ
    TongTienThuKhach        DECIMAL(18,2) NOT NULL DEFAULT 0,            -- Giá bán thống nhất = KhoiLuongKg * DonGiaKg
    DoiTacId                INT NULL FOREIGN KEY REFERENCES DoiTacGiatUi(Id), -- CHO PHÉP NULL KHI MỚI NHẬN
    DonGiaVonKg             DECIMAL(18,2) NULL,                          -- Khóa giá vốn khi gán đối tác
    TienDoiTacNhan          DECIMAL(18,2) NOT NULL DEFAULT 0,            -- Tiền trả đối tác = KhoiLuongKg * DonGiaVonKg
    TienKhachSanNhan        DECIMAL(18,2) NOT NULL DEFAULT 0,            -- Lợi nhuận KS = TongTienThuKhach - TienDoiTacNhan
    TinhTrangQuanAoLucNhan  NVARCHAR(255) NULL,
    NgayNhan                DATETIME NOT NULL DEFAULT GETDATE(),
    NgayGiaoDoiTac          DATETIME NULL,                               -- Thời điểm bàn giao đồ cho đối tác
    NgayHenTra              DATETIME NOT NULL,
    NgayTraThucTe           DATETIME NULL,
    ChiPhiPhatSinh          DECIMAL(18,2) NOT NULL DEFAULT 0,
    HinhThucThanhToan       NVARCHAR(50) NOT NULL DEFAULT N'Ghi nợ vào phòng',
    TrangThai               NVARCHAR(30) NOT NULL DEFAULT N'Chờ giao đối tác'
                                CHECK (TrangThai IN (N'Chờ giao đối tác', N'Đang giặt', N'Giặt xong', N'Hoàn tất', N'Đã hủy')),
    DaThanhToan             BIT NOT NULL DEFAULT 0,
    NguoiTaoId              INT NOT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    NguoiThanhToanId        INT NULL     FOREIGN KEY REFERENCES NhanVien(Id),
    ThoiGianThanhToan       DATETIME NULL,
    -- Quản lý công nợ đối tác
    DaThanhToanChoDoiTac    BIT NOT NULL DEFAULT 0,
    NgayThanhToanChoDoiTac  DATETIME NULL,
    -- Quản lý đền bù thiệt hại
    CoDenBu                 BIT NOT NULL DEFAULT 0,
    BenChiuTrachNhiem       NVARCHAR(50) NULL CHECK (BenChiuTrachNhiem IS NULL OR BenChiuTrachNhiem IN (N'KhachSan', N'DoiTac', N'KhongCo')),
    SoTienDenBu             DECIMAL(18,2) NOT NULL DEFAULT 0,
    LyDoDenBu               NVARCHAR(255) NULL,
    NgayGhiNhanDenBu        DATETIME NULL,
    NguoiGhiNhanDenBuId     INT NULL FOREIGN KEY REFERENCES NhanVien(Id),
    CONSTRAINT CK_GiatUi_XacNhanTT CHECK (
        (DaThanhToan = 0 AND NguoiThanhToanId IS NULL AND ThoiGianThanhToan IS NULL) OR
        (DaThanhToan = 1 AND NguoiThanhToanId IS NOT NULL AND ThoiGianThanhToan IS NOT NULL)
    )
);
GO


-- =====================================================================================
-- PHẦN 3: TẠO TẤT CẢ CÁC KHUNG NHÌN (VIEWS)
-- =====================================================================================

-- 3.1 View dịch vụ chưa thanh toán theo phòng (Ghi nợ phòng cần thu lúc Check-out)
IF OBJECT_ID('vw_DichVuChuaThanhToanTheoPhong', 'V') IS NOT NULL DROP VIEW vw_DichVuChuaThanhToanTheoPhong;
GO
CREATE VIEW vw_DichVuChuaThanhToanTheoPhong AS
SELECT SoPhong, N'Ẩm thực' AS LoaiDichVu, MaDon, TongTien AS SoTien, NgayTao AS ThoiGian
FROM DonHangMonAn
WHERE DaThanhToan = 0 AND (HinhThucThanhToan = N'Ghi nợ vào phòng' OR HinhThucThanhToan = N'Tính chung tiền phòng')
UNION ALL
SELECT SoPhong, N'Sự kiện', MaDon, (TongTienDuKien + ChiPhiPhatSinh - TienCoc), NgayTao
FROM DonDatSuKien
-- FIX: loại trừ đơn đã hủy (dù DaThanhToan đã tự set = 1 khi hủy, thêm điều kiện này để
-- phòng vệ 2 lớp, tránh đơn hủy "Không hoàn cọc" bị tính nhầm vào công nợ phòng)
WHERE DaThanhToan = 0 AND SoPhong IS NOT NULL
  AND TrangThaiDon NOT IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
UNION ALL
SELECT SoPhong, N'Thuê xe', MaDon, (TienThue + ChiPhiPhatSinh - TienCoc), NgayTao
FROM DonThueXe
WHERE DaThanhToan = 0 AND SoPhong IS NOT NULL
  AND TrangThaiDon NOT IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
UNION ALL
SELECT SoPhong, N'Bãi đỗ xe', MaVe, PhiGui, ThoiGianVao
FROM BaiDoXe
WHERE DaThanhToan = 0 AND SoPhong IS NOT NULL AND PhiGui > 0
UNION ALL
SELECT SoPhong, N'Giặt ủi', MaDon, TongTienThuKhach, NgayNhan
FROM DonGiatUi
WHERE DaThanhToan = 0 AND SoPhong IS NOT NULL AND TrangThai != N'Đã hủy';
GO

-- 3.2 View thống kê doanh thu theo dịch vụ
IF OBJECT_ID('vw_DoanhThuTheoDichVu', 'V') IS NOT NULL DROP VIEW vw_DoanhThuTheoDichVu;
GO
CREATE VIEW vw_DoanhThuTheoDichVu AS
SELECT N'Ăn Uống' AS TenDichVu, COUNT(Id) AS SoGiaoDich, ISNULL(SUM(TongTien),0) AS DoanhThu, NgayTao AS Ngay
FROM DonHangMonAn WHERE DaThanhToan = 1 GROUP BY NgayTao
UNION ALL
SELECT N'Sảnh & Sự Kiện', COUNT(Id), ISNULL(SUM(TongTienDuKien + ChiPhiPhatSinh),0), NgayTao
FROM DonDatSuKien WHERE DaThanhToan = 1 GROUP BY NgayTao
UNION ALL
SELECT N'Cho Thuê Xe Máy', COUNT(Id), ISNULL(SUM(TienThue + ChiPhiPhatSinh),0), NgayTao
FROM DonThueXe WHERE DaThanhToan = 1 GROUP BY NgayTao
UNION ALL
SELECT N'Bãi & Hầm Đỗ Xe', COUNT(Id), ISNULL(SUM(PhiGui),0), ThoiGianVao
FROM BaiDoXe WHERE DaThanhToan = 1 AND PhiGui > 0 GROUP BY ThoiGianVao
UNION ALL
SELECT N'Giặt Ủi' AS TenDichVu, COUNT(Id) AS SoGiaoDich, ISNULL(SUM(TongTienThuKhach),0) AS DoanhThu, NgayNhan AS Ngay
FROM DonGiatUi WHERE DaThanhToan = 1 AND TrangThai != N'Đã hủy' GROUP BY NgayNhan;
GO

-- 3.x FIX (bổ sung theo yêu cầu): trạng thái Trống/Đang sử dụng/Đang dọn của 1 khu vực TẠI
-- THỜI ĐIỂM HIỆN TẠI — tính ĐỘNG theo giờ hiện tại thay vì lưu tĩnh trong KhuVucSuKien
-- (vì 1 khu vực có nhiều đơn đặt trước ở nhiều khung giờ khác nhau, không thể có 1 trạng
-- thái cố định duy nhất đúng cho mọi lúc).
IF OBJECT_ID('vw_TrangThaiKhuVucHienTai', 'V') IS NOT NULL DROP VIEW vw_TrangThaiKhuVucHienTai;
GO
CREATE VIEW vw_TrangThaiKhuVucHienTai AS
SELECT kv.Id AS KhuVucId, kv.TenKhuVuc, kv.TrangThai AS TrangThaiKhaiThac,
    CASE
        WHEN kv.TrangThai = N'Ngừng khai thác' THEN N'Ngừng khai thác'
        WHEN dangDung.Id IS NOT NULL THEN N'Đang sử dụng'
        WHEN dangDon.Id IS NOT NULL THEN N'Đang dọn'
        ELSE N'Trống'
    END AS TrangThaiHienTai
FROM KhuVucSuKien kv
OUTER APPLY (
    SELECT TOP 1 d.Id FROM DonDatSuKien d
    WHERE d.KhuVucId = kv.Id AND d.TrangThaiDon = N'Đang sử dụng'
      AND GETDATE() BETWEEN d.ThoiGianBatDau AND d.ThoiGianKetThuc
) dangDung
OUTER APPLY (
    SELECT TOP 1 d.Id FROM DonDatSuKien d
    WHERE d.KhuVucId = kv.Id
      AND d.TrangThaiDon IN (N'Đang sử dụng', N'Hoàn tất')
      AND GETDATE() >= d.ThoiGianKetThuc
      AND GETDATE() < DATEADD(MINUTE, ISNULL(d.ThoiGianDonDepPhut,
            ISNULL((SELECT CAST(GiaTri AS INT) FROM ThamSoHeThong WHERE MaThamSo='EVENT_CLEANING_BUFFER_MINUTES'), 60)),
            d.ThoiGianKetThuc)
) dangDon;
GO

-- 3.x FIX (bổ sung theo yêu cầu): cảnh báo các đơn sắp hết giờ thuê (dùng tham số
-- EVENT_ADVANCE_ALERT_MINUTES đã có sẵn nhưng trước đây KHÔNG được view/proc nào dùng tới)
IF OBJECT_ID('vw_SuKienSapHetGio', 'V') IS NOT NULL DROP VIEW vw_SuKienSapHetGio;
GO
CREATE VIEW vw_SuKienSapHetGio AS
SELECT d.Id, d.MaDon, d.KhuVucId, kv.TenKhuVuc, d.TenKhach, d.ThoiGianKetThuc,
       DATEDIFF(MINUTE, GETDATE(), d.ThoiGianKetThuc) AS SoPhutConLai
FROM DonDatSuKien d
JOIN KhuVucSuKien kv ON kv.Id = d.KhuVucId
WHERE d.TrangThaiDon = N'Đang sử dụng'
  AND d.ThoiGianKetThuc BETWEEN GETDATE() AND DATEADD(MINUTE,
        ISNULL((SELECT CAST(GiaTri AS INT) FROM ThamSoHeThong WHERE MaThamSo='EVENT_ADVANCE_ALERT_MINUTES'), 15),
        GETDATE());
GO

-- 3.x Trạng thái Sẵn sàng / Đang thuê / Đang kiểm tra vệ sinh của từng Xe TẠI THỜI ĐIỂM HIỆN TẠI
-- Tính ĐỘNG theo GETDATE() tương tự vw_TrangThaiKhuVucHienTai
IF OBJECT_ID('vw_TrangThaiXeHienTai', 'V') IS NOT NULL DROP VIEW vw_TrangThaiXeHienTai;
GO
CREATE VIEW vw_TrangThaiXeHienTai AS
SELECT x.Id AS XeId, x.BienSo, x.TenXe, x.LoaiXe, x.GiaThueNgay, x.TienCocQuyDinh,
       x.TrangThai AS TrangThaiKhaiThac,
       CASE
           WHEN x.TrangThai IN (N'Bảo trì', N'Ngừng khai thác') THEN x.TrangThai
           WHEN dangThue.Id IS NOT NULL THEN N'Đang thuê'
           WHEN dangKiemTra.Id IS NOT NULL THEN N'Đang vệ sinh / kiểm tra'
           ELSE N'Sẵn sàng'
       END AS TrangThaiHienTai
FROM XeChoThue x
OUTER APPLY (
    SELECT TOP 1 d.Id FROM DonThueXe d
    WHERE d.XeId = x.Id AND d.TrangThaiDon = N'Đang thuê'
      AND GETDATE() >= d.NgayThue AND (d.NgayTraThucTe IS NULL OR d.NgayTraThucTe > GETDATE())
) dangThue
OUTER APPLY (
    SELECT TOP 1 d.Id FROM DonThueXe d
    WHERE d.XeId = x.Id
      AND d.TrangThaiDon IN (N'Đang thuê', N'Hoàn tất')
      AND GETDATE() >= ISNULL(d.NgayTraThucTe, d.NgayTraDuKien)
      AND GETDATE() < DATEADD(MINUTE, ISNULL(d.ThoiGianKiemTraXePhut,
            ISNULL((SELECT CAST(GiaTri AS INT) FROM ThamSoHeThong WHERE MaThamSo='VEHICLE_CLEANING_BUFFER_MINUTES'), 30)),
            ISNULL(d.NgayTraThucTe, d.NgayTraDuKien))
) dangKiemTra;
GO

-- 3.x Cảnh báo các đơn thuê xe sắp đến giờ trả xe (dùng tham số VEHICLE_ADVANCE_ALERT_MINUTES)
-- Tương tự vw_SuKienSapHetGio
IF OBJECT_ID('vw_XeSapHetGioThue', 'V') IS NOT NULL DROP VIEW vw_XeSapHetGioThue;
GO
CREATE VIEW vw_XeSapHetGioThue AS
SELECT d.Id, d.MaDon, d.XeId, x.BienSo, x.TenXe, d.TenKhach, d.SoDienThoai, d.SoPhong, d.NgayTraDuKien,
       DATEDIFF(MINUTE, GETDATE(), d.NgayTraDuKien) AS SoPhutConLai
FROM DonThueXe d
JOIN XeChoThue x ON x.Id = d.XeId
WHERE d.TrangThaiDon = N'Đang thuê'
  AND d.NgayTraThucTe IS NULL
  AND d.NgayTraDuKien BETWEEN GETDATE() AND DATEADD(MINUTE,
        ISNULL((SELECT CAST(GiaTri AS INT) FROM ThamSoHeThong WHERE MaThamSo='VEHICLE_ADVANCE_ALERT_MINUTES'), 30),
        GETDATE());
GO

-- 3.x Cảnh báo các đơn sự kiện quá giờ hẹn kết thúc từ 15 phút trở lên mà chưa hoàn tất
IF OBJECT_ID('vw_SuKienQuaHan', 'V') IS NOT NULL DROP VIEW vw_SuKienQuaHan;
GO
CREATE VIEW vw_SuKienQuaHan AS
SELECT d.Id, d.MaDon, d.KhuVucId, kv.TenKhuVuc, d.TenKhach, d.SoDienThoai, d.SoPhong, d.ThoiGianKetThuc,
       DATEDIFF(MINUTE, d.ThoiGianKetThuc, GETDATE()) AS SoPhutQuaHan
FROM DonDatSuKien d
JOIN KhuVucSuKien kv ON kv.Id = d.KhuVucId
WHERE d.TrangThaiDon = N'Đang sử dụng'
  AND GETDATE() >= DATEADD(MINUTE, 15, d.ThoiGianKetThuc);
GO

-- 3.x Cảnh báo các đơn thuê xe quá giờ hẹn trả từ 30 phút trở lên mà chưa trả xe, quyết toán
IF OBJECT_ID('vw_XeThueQuaHan', 'V') IS NOT NULL DROP VIEW vw_XeThueQuaHan;
GO
CREATE VIEW vw_XeThueQuaHan AS
SELECT d.Id, d.MaDon, d.XeId, x.BienSo, x.TenXe, d.TenKhach, d.SoDienThoai, d.SoPhong, d.NgayTraDuKien,
       DATEDIFF(MINUTE, d.NgayTraDuKien, GETDATE()) AS SoPhutQuaHan
FROM DonThueXe d
JOIN XeChoThue x ON x.Id = d.XeId
WHERE d.TrangThaiDon = N'Đang thuê'
  AND d.NgayTraThucTe IS NULL
  AND GETDATE() >= DATEADD(MINUTE, 30, d.NgayTraDuKien);
GO


-- =====================================================================================
-- PHẦN 4: TẠO TẤT CẢ CÁC TRÌNH KÍCH HOẠT (TRIGGERS)
-- =====================================================================================

-- 4.1 Trigger tự động cộng tồn kho và cập nhật giá vốn bình quân khi nhập kho
IF OBJECT_ID('trg_ChiTietPhieuNhapKho_CongTonKho', 'TR') IS NOT NULL
    DROP TRIGGER trg_ChiTietPhieuNhapKho_CongTonKho;
GO
CREATE TRIGGER trg_ChiTietPhieuNhapKho_CongTonKho
ON ChiTietPhieuNhapKho
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE ma
    SET
        ma.GiaVonBinhQuan = CASE
            WHEN (ma.SoLuongTonKho + ins.TongSoLuongNhapMoi) > 0 THEN
                ((ma.SoLuongTonKho * ma.GiaVonBinhQuan) + ins.TongGiaTriNhapMoi)
                / (ma.SoLuongTonKho + ins.TongSoLuongNhapMoi)
            ELSE ma.GiaVonBinhQuan
        END,
        ma.SoLuongTonKho = ma.SoLuongTonKho + ins.TongSoLuongNhapMoi,
        -- Sửa lỗi: Nếu Admin đã chủ động thiết lập 'Ngừng kinh doanh', nhập thêm kho vẫn giữ nguyên trạng thái này
        ma.TrangThai     = CASE
            WHEN ma.TrangThai = N'Ngừng kinh doanh' THEN N'Ngừng kinh doanh'
            ELSE N'Còn hàng'
        END
    FROM MonAn ma
    JOIN (
        SELECT MonAnId,
               SUM(SoLuongNhap * HeSoQuyDoi) AS TongSoLuongNhapMoi,
               SUM(SoLuongNhap * DonGiaNhap) AS TongGiaTriNhapMoi
        FROM inserted
        GROUP BY MonAnId
    ) ins ON ma.Id = ins.MonAnId;

    UPDATE pnk
    SET TongTien = (
        SELECT ISNULL(SUM(ThanhTien), 0)
        FROM ChiTietPhieuNhapKho
        WHERE PhieuNhapId = pnk.Id
    )
    FROM PhieuNhapKho pnk
    WHERE pnk.Id IN (SELECT DISTINCT PhieuNhapId FROM inserted);
END;
GO

-- 4.2 Trigger tự động tính tiền giặt ủi: khóa giá bán lúc nhận, khóa giá vốn khi gán đối tác (Mô hình giá vốn thực tế)
IF OBJECT_ID('trg_DonGiatUi_TinhTien', 'TR') IS NOT NULL DROP TRIGGER trg_DonGiatUi_TinhTien;
GO
CREATE TRIGGER trg_DonGiatUi_TinhTien
ON DonGiatUi
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    IF UPDATE(KhoiLuongKg) OR UPDATE(DonGiaKg) OR UPDATE(DoiTacId) OR UPDATE(DonGiaVonKg)
    BEGIN
        UPDATE d
        SET 
            TongTienThuKhach = i.KhoiLuongKg * i.DonGiaKg,
            DonGiaVonKg = CASE 
                WHEN i.DoiTacId IS NULL THEN NULL
                WHEN i.DonGiaVonKg IS NOT NULL THEN i.DonGiaVonKg
                ELSE ISNULL(bg.DonGiaVonKg, i.DonGiaKg * (ISNULL(dt.TyLeDoiTacHuong, 70.0) / 100.0))
            END,
            TienDoiTacNhan = CASE 
                WHEN i.DoiTacId IS NULL THEN 0
                ELSE i.KhoiLuongKg * CASE 
                    WHEN i.DonGiaVonKg IS NOT NULL THEN i.DonGiaVonKg
                    ELSE ISNULL(bg.DonGiaVonKg, i.DonGiaKg * (ISNULL(dt.TyLeDoiTacHuong, 70.0) / 100.0))
                END
            END,
            TienKhachSanNhan = (i.KhoiLuongKg * i.DonGiaKg) - CASE 
                WHEN i.DoiTacId IS NULL THEN 0
                ELSE i.KhoiLuongKg * CASE 
                    WHEN i.DonGiaVonKg IS NOT NULL THEN i.DonGiaVonKg
                    ELSE ISNULL(bg.DonGiaVonKg, i.DonGiaKg * (ISNULL(dt.TyLeDoiTacHuong, 70.0) / 100.0))
                END
            END
        FROM DonGiatUi d
        JOIN inserted i ON d.Id = i.Id
        LEFT JOIN DoiTacGiatUi dt ON dt.Id = i.DoiTacId
        LEFT JOIN LoaiDichVuGiatUi ldv ON (ldv.TenLoai = i.LoaiDichVu OR ldv.MaLoai = i.LoaiDichVu)
        LEFT JOIN BangGiaDoiTacGiatUi bg ON (bg.DoiTacId = i.DoiTacId AND bg.LoaiDichVuId = ldv.Id);
    END
END;
GO

-- 4.3 Trigger kiểm tra giá vốn đối tác báo phải nhỏ hơn giá niêm yết của khách sạn (đảm bảo luôn có lợi nhuận)
IF OBJECT_ID('trg_BangGiaDoiTac_KiemTraGiaVon', 'TR') IS NOT NULL DROP TRIGGER trg_BangGiaDoiTac_KiemTraGiaVon;
GO
CREATE TRIGGER trg_BangGiaDoiTac_KiemTraGiaVon
ON BangGiaDoiTacGiatUi
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        JOIN LoaiDichVuGiatUi ldv ON ldv.Id = i.LoaiDichVuId
        WHERE i.DonGiaVonKg >= ldv.DonGiaBanKg
    )
    BEGIN
        RAISERROR(N'Giá vốn của đối tác báo (DonGiaVonKg) phải nhỏ hơn giá niêm yết bán cho khách (DonGiaBanKg) để khách sạn có lợi nhuận!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO


-- 4.x FIX (bổ sung theo yêu cầu): chống 2 đơn đặt sảnh chồng/sát giờ nhau cùng 1 khu vực.
-- Bắt buộc có khoảng đệm dọn dẹp giữa 2 đơn liên tiếp (mặc định lấy từ ThamSoHeThong,
-- có thể RÚT NGẮN riêng từng đơn qua cột ThoiGianDonDepPhut kèm phụ thu).
-- Bỏ qua các đơn đã hủy (không còn chiếm chỗ lịch).
IF OBJECT_ID('trg_DonDatSuKien_ChongTrungLich', 'TR') IS NOT NULL DROP TRIGGER trg_DonDatSuKien_ChongTrungLich;
GO
CREATE TRIGGER trg_DonDatSuKien_ChongTrungLich
ON DonDatSuKien
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT (UPDATE(KhuVucId) OR UPDATE(ThoiGianBatDau) OR UPDATE(ThoiGianKetThuc)
            OR UPDATE(TrangThaiDon) OR UPDATE(ThoiGianDonDepPhut))
        RETURN;  -- không đụng cột ảnh hưởng lịch thì khỏi kiểm tra lại

    DECLARE @BufferMacDinh INT = ISNULL(
        (SELECT CAST(GiaTri AS INT) FROM ThamSoHeThong WHERE MaThamSo = 'EVENT_CLEANING_BUFFER_MINUTES'), 60);

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN DonDatSuKien d
            ON d.KhuVucId = i.KhuVucId
           AND d.Id <> i.Id
           AND d.TrangThaiDon NOT IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
        WHERE i.TrangThaiDon NOT IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
          AND i.ThoiGianBatDau < DATEADD(MINUTE, ISNULL(d.ThoiGianDonDepPhut, @BufferMacDinh), d.ThoiGianKetThuc)
          AND d.ThoiGianBatDau < DATEADD(MINUTE, ISNULL(i.ThoiGianDonDepPhut, @BufferMacDinh), i.ThoiGianKetThuc)
    )
    BEGIN
        RAISERROR(N'Khung giờ bị trùng hoặc chưa đủ thời gian dọn dẹp với 1 đơn khác đã đặt cho cùng khu vực này.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 4.x Chống 2 đơn thuê cùng 1 xe bị chồng/sát giờ nhau.
-- Bắt buộc có khoảng đệm vệ sinh & kiểm tra xe giữa 2 đơn liên tiếp (mặc định lấy từ ThamSoHeThong.VEHICLE_CLEANING_BUFFER_MINUTES).
-- Bỏ qua các đơn đã hủy (không còn giữ chỗ xe).
IF OBJECT_ID('trg_DonThueXe_ChongTrungLich', 'TR') IS NOT NULL DROP TRIGGER trg_DonThueXe_ChongTrungLich;
GO
CREATE TRIGGER trg_DonThueXe_ChongTrungLich
ON DonThueXe
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT (UPDATE(XeId) OR UPDATE(NgayThue) OR UPDATE(NgayTraDuKien)
            OR UPDATE(TrangThaiDon) OR UPDATE(ThoiGianKiemTraXePhut))
        RETURN;

    DECLARE @BufferMacDinh INT = ISNULL(
        (SELECT CAST(GiaTri AS INT) FROM ThamSoHeThong WHERE MaThamSo = 'VEHICLE_CLEANING_BUFFER_MINUTES'), 30);

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN DonThueXe d
            ON d.XeId = i.XeId
           AND d.Id <> i.Id
           AND d.TrangThaiDon NOT IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
        WHERE i.TrangThaiDon NOT IN (N'Đã hủy - Hoàn cọc', N'Đã hủy - Không hoàn cọc')
          AND i.NgayThue < DATEADD(MINUTE, ISNULL(d.ThoiGianKiemTraXePhut, @BufferMacDinh), d.NgayTraDuKien)
          AND d.NgayThue < DATEADD(MINUTE, ISNULL(i.ThoiGianKiemTraXePhut, @BufferMacDinh), i.NgayTraDuKien)
    )
    BEGIN
        RAISERROR(N'Khung thời gian thuê bị trùng hoặc chưa đủ thời gian kiểm tra/vệ sinh với 1 đơn khác đã đặt cho cùng xe này.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO


-- =====================================================================================
-- PHẦN 5: TẠO TẤT CẢ CÁC THỦ TỤC LƯU TRỮ (STORED PROCEDURES)
-- =====================================================================================

-- 5.1 Tính tổng tiền dịch vụ CHƯA thanh toán của 1 phòng (dùng lúc Check-out)
IF OBJECT_ID('sp_TinhTongDichVuChuaThanhToanTheoPhong', 'P') IS NOT NULL
    DROP PROCEDURE sp_TinhTongDichVuChuaThanhToanTheoPhong;
GO
CREATE PROCEDURE sp_TinhTongDichVuChuaThanhToanTheoPhong
    @SoPhong NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM PhongKhachSan WHERE SoPhong = @SoPhong)
    BEGIN
        RAISERROR(N'Không tìm thấy phòng %s trong hệ thống.', 16, 1, @SoPhong);
        RETURN;
    END

    SELECT * FROM vw_DichVuChuaThanhToanTheoPhong
    WHERE SoPhong = @SoPhong
    ORDER BY ThoiGian;

    SELECT
        @SoPhong AS SoPhong,
        COUNT(*) AS SoDichVuChuaThanhToan,
        ISNULL(SUM(SoTien), 0) AS TongTienPhaiThu
    FROM vw_DichVuChuaThanhToanTheoPhong
    WHERE SoPhong = @SoPhong;
END;
GO

-- 5.2 Xác nhận đã thu tiền toàn bộ dịch vụ của 1 phòng (gọi khi Check-out)
IF OBJECT_ID('sp_XacNhanThanhToanDichVuTheoPhong', 'P') IS NOT NULL
    DROP PROCEDURE sp_XacNhanThanhToanDichVuTheoPhong;
GO
CREATE PROCEDURE sp_XacNhanThanhToanDichVuTheoPhong
    @SoPhong NVARCHAR(20),
    @NhanVienThanhToanId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE Id = @NhanVienThanhToanId AND TrangThai = N'Đang làm việc')
    BEGIN
        RAISERROR(N'Nhân viên xác nhận thanh toán không hợp lệ hoặc đã nghỉ việc.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @Now DATETIME = GETDATE();

        UPDATE DonHangMonAn SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE SoPhong = @SoPhong AND DaThanhToan = 0 AND (HinhThucThanhToan = N'Ghi nợ vào phòng' OR HinhThucThanhToan = N'Tính chung tiền phòng');

        UPDATE DonDatSuKien SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE SoPhong = @SoPhong AND DaThanhToan = 0;

        UPDATE DonThueXe SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE SoPhong = @SoPhong AND DaThanhToan = 0;

        UPDATE BaiDoXe SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE SoPhong = @SoPhong AND DaThanhToan = 0;

        UPDATE DonGiatUi SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE SoPhong = @SoPhong AND DaThanhToan = 0 AND TrangThai != N'Đã hủy';

        COMMIT TRANSACTION;

        SELECT N'Đã xác nhận thanh toán toàn bộ dịch vụ của phòng ' + @SoPhong
               + N' bởi nhân viên #' + CAST(@NhanVienThanhToanId AS NVARCHAR(10)) AS KetQua;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 5.3 Xác nhận thanh toán 1 đơn lẻ (Thanh toán trực tiếp tại quầy / Khách vãng lai)
IF OBJECT_ID('sp_XacNhanThanhToanDonLe', 'P') IS NOT NULL DROP PROCEDURE sp_XacNhanThanhToanDonLe;
GO
CREATE PROCEDURE sp_XacNhanThanhToanDonLe
    @LoaiDichVu NVARCHAR(20),
    @MaDon NVARCHAR(30),
    @NhanVienThanhToanId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE Id = @NhanVienThanhToanId AND TrangThai = N'Đang làm việc')
    BEGIN
        RAISERROR(N'Nhân viên xác nhận thanh toán không hợp lệ hoặc đã nghỉ việc.', 16, 1);
        RETURN;
    END

    DECLARE @Now DATETIME = GETDATE();

    IF @LoaiDichVu = 'AnUong'
        UPDATE DonHangMonAn SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE MaDon = @MaDon AND DaThanhToan = 0;
    ELSE IF @LoaiDichVu = 'SuKien'
        UPDATE DonDatSuKien SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE MaDon = @MaDon AND DaThanhToan = 0;
    ELSE IF @LoaiDichVu = 'ThueXe'
        UPDATE DonThueXe SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE MaDon = @MaDon AND DaThanhToan = 0;
    ELSE IF @LoaiDichVu = 'DoXe'
        UPDATE BaiDoXe SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE MaVe = @MaDon AND DaThanhToan = 0;
    ELSE IF @LoaiDichVu = 'GiatUi'
        UPDATE DonGiatUi SET DaThanhToan = 1, NguoiThanhToanId = @NhanVienThanhToanId, ThoiGianThanhToan = @Now
        WHERE MaDon = @MaDon AND DaThanhToan = 0 AND TrangThai != N'Đã hủy';
    ELSE
        RAISERROR(N'Loại dịch vụ không hợp lệ.', 16, 1);

    SELECT @@ROWCOUNT AS SoDongDaCapNhat;
END;
GO

-- 5.4 Trả phòng khách sạn
IF OBJECT_ID('sp_TraPhong', 'P') IS NOT NULL DROP PROCEDURE sp_TraPhong;
GO
CREATE PROCEDURE sp_TraPhong
    @SoPhong NVARCHAR(20),
    @KetQua INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ConNo INT;

    SELECT @ConNo = COUNT(*) FROM vw_DichVuChuaThanhToanTheoPhong WHERE SoPhong = @SoPhong;

    IF @ConNo > 0
    BEGIN
        SET @KetQua = 0;
        RETURN;
    END

    UPDATE PhongKhachSan
    SET TrangThai = N'Đang dọn', TenKhach = NULL, SoDienThoai = NULL, CCCD = NULL,
        GhiChu = NULL, NgayNhanPhong = NULL, NgayTraPhong = NULL
    WHERE SoPhong = @SoPhong;

    SET @KetQua = 1;
END;
GO

-- 5.5 Lập phiếu nhập kho
IF OBJECT_ID('sp_LapPhieuNhapKho', 'P') IS NOT NULL DROP PROCEDURE sp_LapPhieuNhapKho;
GO
CREATE PROCEDURE sp_LapPhieuNhapKho
    @MaPhieuNhap NVARCHAR(50),
    @NhaCungCapId INT,
    @NguoiNhapId INT,
    @GhiChu NVARCHAR(255) = NULL,
    @ChiTiet kieu_ChiTietNhapKho READONLY,
    @PhieuNhapId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM @ChiTiet)
        BEGIN
            RAISERROR(N'Phiếu nhập phải có ít nhất 1 dòng hàng.', 16, 1); RETURN;
        END

        BEGIN TRANSACTION;

        INSERT INTO PhieuNhapKho (MaPhieuNhap, NhaCungCapId, NguoiNhapId, GhiChu)
        VALUES (@MaPhieuNhap, @NhaCungCapId, @NguoiNhapId, @GhiChu);

        SET @PhieuNhapId = SCOPE_IDENTITY();

        INSERT INTO ChiTietPhieuNhapKho
            (PhieuNhapId, MonAnId, DonViNhap, HeSoQuyDoi, SoLuongNhap, DonGiaNhap, SoLo, HanSuDung, GhiChu)
        SELECT @PhieuNhapId, MonAnId, DonViNhap, HeSoQuyDoi, SoLuongNhap, DonGiaNhap, SoLo, HanSuDung, GhiChu
        FROM @ChiTiet;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 5.6 Bán hàng / mini-bar kiểm tra tồn kho
IF OBJECT_ID('sp_DatMonAn', 'P') IS NOT NULL DROP PROCEDURE sp_DatMonAn;
GO
CREATE PROCEDURE sp_DatMonAn
    @MonAnId INT,
    @SoLuong INT,
    @KetQua INT OUTPUT,
    @GiaVonTaiThoiDiemBan DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @TonKho INT;

        SELECT @TonKho = SoLuongTonKho, @GiaVonTaiThoiDiemBan = GiaVonBinhQuan
        FROM MonAn WITH (UPDLOCK, ROWLOCK)
        WHERE Id = @MonAnId;

        IF @TonKho IS NULL OR @TonKho < @SoLuong
        BEGIN
            SET @KetQua = 0; ROLLBACK TRANSACTION; RETURN;
        END

        UPDATE MonAn
        SET SoLuongTonKho = SoLuongTonKho - @SoLuong,
            TrangThai = CASE WHEN SoLuongTonKho - @SoLuong <= 0 THEN N'Hết hàng' ELSE TrangThai END
        WHERE Id = @MonAnId;

        SET @KetQua = 1;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 5.7 Báo cáo lợi nhuận hàng khô / mini-bar
IF OBJECT_ID('sp_BaoCaoLoiNhuanKho', 'P') IS NOT NULL DROP PROCEDURE sp_BaoCaoLoiNhuanKho;
GO
CREATE PROCEDURE sp_BaoCaoLoiNhuanKho
    @TuNgay DATETIME,
    @DenNgay DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ma.Id AS MonAnId, ma.TenMon,
           SUM(ct.SoLuong)                 AS SoLuongDaBan,
           SUM(ct.ThanhTien)               AS DoanhThu,
           SUM(ct.SoLuong * ct.GiaVon)     AS TongGiaVon,
           SUM(ct.LoiNhuan)                AS LoiNhuanGop
    FROM ChiTietDonHangMonAn ct
    JOIN DonHangMonAn dh ON dh.Id = ct.DonHangId
    JOIN MonAn ma        ON ma.Id = ct.MonAnId
    WHERE dh.DaThanhToan = 1 AND dh.NgayTao BETWEEN @TuNgay AND @DenNgay
    GROUP BY ma.Id, ma.TenMon
    ORDER BY LoiNhuanGop DESC;
END;
GO

-- 5.8 Báo cáo doanh thu theo khoảng thời gian
IF OBJECT_ID('sp_BaoCaoDoanhThuTheoKhoangThoiGian', 'P') IS NOT NULL
    DROP PROCEDURE sp_BaoCaoDoanhThuTheoKhoangThoiGian;
GO
CREATE PROCEDURE sp_BaoCaoDoanhThuTheoKhoangThoiGian
    @TuNgay DATETIME,
    @DenNgay DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT N'Ăn Uống' AS NhomDichVu, COUNT(Id) AS SoGiaoDich, ISNULL(SUM(TongTien),0) AS DoanhThu
    FROM DonHangMonAn WHERE DaThanhToan = 1 AND NgayTao BETWEEN @TuNgay AND @DenNgay
    UNION ALL
    SELECT N'Sảnh & Sự Kiện', COUNT(Id), ISNULL(SUM(TongTienDuKien + ChiPhiPhatSinh),0)
    FROM DonDatSuKien WHERE DaThanhToan = 1 AND NgayTao BETWEEN @TuNgay AND @DenNgay
    UNION ALL
    SELECT N'Cho Thuê Xe Máy', COUNT(Id), ISNULL(SUM(TienThue + ChiPhiPhatSinh),0)
    FROM DonThueXe WHERE DaThanhToan = 1 AND NgayTao BETWEEN @TuNgay AND @DenNgay
    UNION ALL
    SELECT N'Bãi & Hầm Đỗ Xe', COUNT(Id), ISNULL(SUM(PhiGui),0)
    FROM BaiDoXe WHERE DaThanhToan = 1 AND ThoiGianVao BETWEEN @TuNgay AND @DenNgay
    UNION ALL
    SELECT N'Giặt Ủi' AS NhomDichVu, COUNT(Id) AS SoGiaoDich, ISNULL(SUM(TongTienThuKhach),0) AS DoanhThu
    FROM DonGiatUi WHERE DaThanhToan = 1 AND TrangThai != N'Đã hủy' AND NgayNhan BETWEEN @TuNgay AND @DenNgay;
END;
GO

-- 5.9 Nhận phòng (Check-in)
IF OBJECT_ID('sp_NhanPhong', 'P') IS NOT NULL DROP PROCEDURE sp_NhanPhong;
GO
CREATE PROCEDURE sp_NhanPhong
    @SoPhong NVARCHAR(20),
    @TenKhach NVARCHAR(100),
    @SoDienThoai NVARCHAR(20),
    @CCCD NVARCHAR(20),
    @NgayTraDuKien DATETIME = NULL,
    @GhiChu NVARCHAR(255) = NULL,
    @KetQua INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TrangThaiHienTai NVARCHAR(30);

    SELECT @TrangThaiHienTai = TrangThai FROM PhongKhachSan WITH (UPDLOCK, ROWLOCK) WHERE SoPhong = @SoPhong;

    IF @TrangThaiHienTai IS NULL
    BEGIN
        SET @KetQua = -1; RETURN;
    END

    IF @TrangThaiHienTai <> N'Trống'
    BEGIN
        SET @KetQua = 0; RETURN;
    END

    UPDATE PhongKhachSan
    SET TrangThai = N'Đang ở', TenKhach = @TenKhach, SoDienThoai = @SoDienThoai,
        CCCD = @CCCD, GhiChu = @GhiChu, NgayNhanPhong = GETDATE(), NgayTraPhong = @NgayTraDuKien
    WHERE SoPhong = @SoPhong;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE (SoDienThoai = @SoDienThoai AND @SoDienThoai <> '') OR (CCCD = @CCCD AND @CCCD <> ''))
        UPDATE KhachHang SET HoTen = @TenKhach, SoPhong = @SoPhong, GhiChu = @GhiChu
        WHERE (SoDienThoai = @SoDienThoai AND @SoDienThoai <> '') OR (CCCD = @CCCD AND @CCCD <> '');
    ELSE
        INSERT INTO KhachHang (HoTen, CCCD, SoDienThoai, SoPhong, GhiChu)
        VALUES (@TenKhach, @CCCD, @SoDienThoai, @SoPhong, @GhiChu);

    SET @KetQua = 1;
END;
GO

-- 5.10 Hoàn tất dọn phòng
IF OBJECT_ID('sp_HoanTatDonPhong', 'P') IS NOT NULL DROP PROCEDURE sp_HoanTatDonPhong;
GO
CREATE PROCEDURE sp_HoanTatDonPhong
    @SoPhong NVARCHAR(20),
    @KetQua INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM PhongKhachSan WHERE SoPhong = @SoPhong AND TrangThai = N'Đang dọn')
    BEGIN
        SET @KetQua = 0; RETURN;
    END

    UPDATE PhongKhachSan SET TrangThai = N'Trống' WHERE SoPhong = @SoPhong;
    SET @KetQua = 1;
END;
GO

-- 5.11 Bàn giao đồ giặt cho đối tác (Tách riêng bước Giao đồ sau khi Lễ tân đã nhận đồ)
IF OBJECT_ID('sp_GiaoDoChoDoiTac', 'P') IS NOT NULL DROP PROCEDURE sp_GiaoDoChoDoiTac;
GO
CREATE PROCEDURE sp_GiaoDoChoDoiTac
    @MaDon NVARCHAR(30),
    @DoiTacId INT,
    @NhanVienGiaoId INT,
    @KetQua INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Kiểm tra đối tác còn hoạt động không
    IF NOT EXISTS (SELECT 1 FROM DoiTacGiatUi WHERE Id = @DoiTacId AND TrangThai = 1)
    BEGIN
        RAISERROR(N'Đối tác giặt ủi không tồn tại hoặc đã ngừng hợp tác.', 16, 1);
        SET @KetQua = 0;
        RETURN;
    END

    -- Kiểm tra đơn giặt ủi
    IF NOT EXISTS (SELECT 1 FROM DonGiatUi WHERE MaDon = @MaDon)
    BEGIN
        RAISERROR(N'Không tìm thấy mã đơn giặt ủi %s.', 16, 1, @MaDon);
        SET @KetQua = 0;
        RETURN;
    END

    UPDATE DonGiatUi
    SET 
        DoiTacId = @DoiTacId,
        NgayGiaoDoiTac = GETDATE(),
        TrangThai = N'Đang giặt'
    WHERE MaDon = @MaDon;

    SET @KetQua = 1;
END;
GO

-- 5.12 Ghi nhận đền bù thiệt hại giặt ủi (Phải xác định rõ bên chịu trách nhiệm, chặn vượt trần hợp đồng)
IF OBJECT_ID('sp_GhiNhanDenBuGiatUi', 'P') IS NOT NULL DROP PROCEDURE sp_GhiNhanDenBuGiatUi;
GO
CREATE PROCEDURE sp_GhiNhanDenBuGiatUi
    @MaDon NVARCHAR(30),
    @BenChiuTrachNhiem NVARCHAR(50), -- N'KhachSan' hoặc N'DoiTac'
    @SoTienDenBu DECIMAL(18,2),
    @LyDoDenBu NVARCHAR(255),
    @NhanVienGhiNhanId INT,
    @KetQua INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @BenChiuTrachNhiem NOT IN (N'KhachSan', N'DoiTac')
    BEGIN
        RAISERROR(N'Bên chịu trách nhiệm phải là KhachSan hoặc DoiTac.', 16, 1);
        SET @KetQua = 0;
        RETURN;
    END

    IF @SoTienDenBu <= 0
    BEGIN
        RAISERROR(N'Số tiền đền bù phải lớn hơn 0.', 16, 1);
        SET @KetQua = 0;
        RETURN;
    END

    DECLARE @TongTienThuKhach DECIMAL(18,2);
    DECLARE @LoaiDichVu NVARCHAR(50);
    DECLARE @HeSoTran INT;

    SELECT 
        @TongTienThuKhach = d.TongTienThuKhach,
        @LoaiDichVu = d.LoaiDichVu,
        @HeSoTran = ISNULL(ldv.HeSoBoiThuongToiDa, 10)
    FROM DonGiatUi d
    LEFT JOIN LoaiDichVuGiatUi ldv ON (ldv.TenLoai = d.LoaiDichVu OR ldv.MaLoai = d.LoaiDichVu)
    WHERE d.MaDon = @MaDon;

    IF @TongTienThuKhach IS NULL
    BEGIN
        RAISERROR(N'Không tìm thấy đơn giặt ủi %s.', 16, 1, @MaDon);
        SET @KetQua = 0;
        RETURN;
    END

    -- Nếu lỗi do đối tác, chặn số tiền đền bù vượt trần hợp đồng (HeSoBoiThuongToiDa x Đơn giá dịch vụ)
    IF @BenChiuTrachNhiem = N'DoiTac'
    BEGIN
        DECLARE @MucTranBoiThuong DECIMAL(18,2) = @TongTienThuKhach * @HeSoTran;
        IF @SoTienDenBu > @MucTranBoiThuong
        BEGIN
            DECLARE @StrMucTran NVARCHAR(50) = CONVERT(NVARCHAR(50), @MucTranBoiThuong);
            RAISERROR(N'Số tiền đền bù vượt mức trần thỏa thuận trong hợp đồng (Tối đa %d lần giá dịch vụ = %s VNĐ).', 16, 1, @HeSoTran, @StrMucTran);
            SET @KetQua = 0;
            RETURN;
        END
    END

    UPDATE DonGiatUi
    SET 
        CoDenBu = 1,
        BenChiuTrachNhiem = @BenChiuTrachNhiem,
        SoTienDenBu = @SoTienDenBu,
        LyDoDenBu = @LyDoDenBu,
        NgayGhiNhanDenBu = GETDATE(),
        NguoiGhiNhanDenBuId = @NhanVienGhiNhanId
    WHERE MaDon = @MaDon;

    SET @KetQua = 1;
END;
GO

-- 5.13 Quyết toán công nợ theo kỳ cho đối tác giặt ủi (Thực hiện định kỳ 1 tháng 1 lần)
IF OBJECT_ID('sp_QuyetToanCongNoDoiTac', 'P') IS NOT NULL DROP PROCEDURE sp_QuyetToanCongNoDoiTac;
GO
CREATE PROCEDURE sp_QuyetToanCongNoDoiTac
    @DoiTacId INT,
    @DenNgay DATETIME = NULL,
    @NhanVienQuyetToanId INT,
    @SoDonQuyetToan INT OUTPUT,
    @TongTienQuyetToan DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @DenNgay IS NULL SET @DenNgay = GETDATE();

    -- Tính tổng số đơn và số tiền cần quyết toán (đã trừ tiền đền bù thiệt hại do đối tác gây ra)
    SELECT 
        @SoDonQuyetToan = COUNT(Id),
        @TongTienQuyetToan = ISNULL(SUM(TienDoiTacNhan - CASE WHEN CoDenBu = 1 AND BenChiuTrachNhiem = N'DoiTac' THEN SoTienDenBu ELSE 0 END), 0)
    FROM DonGiatUi
    WHERE DoiTacId = @DoiTacId
      AND DaThanhToanChoDoiTac = 0
      AND TrangThai IN (N'Giặt xong', N'Hoàn tất')
      AND NgayNhan <= @DenNgay;

    -- Cập nhật cờ đã thanh toán cho đối tác
    UPDATE DonGiatUi
    SET 
        DaThanhToanChoDoiTac = 1,
        NgayThanhToanChoDoiTac = GETDATE()
    WHERE DoiTacId = @DoiTacId
      AND DaThanhToanChoDoiTac = 0
      AND TrangThai IN (N'Giặt xong', N'Hoàn tất')
      AND NgayNhan <= @DenNgay;

    SELECT 
        @DoiTacId AS DoiTacId,
        @SoDonQuyetToan AS SoDonDaQuyetToan,
        @TongTienQuyetToan AS TongTienThanhToanChoDoiTac,
        GETDATE() AS NgayQuyetToan;
END;
GO

-- 5.14 Thêm đối tác giặt ủi mới kèm thiết lập bảng giá vốn dịch vụ (Bắt buộc Giá vốn < Giá niêm yết)
IF OBJECT_ID('sp_ThemDoiTacGiatUi', 'P') IS NOT NULL DROP PROCEDURE sp_ThemDoiTacGiatUi;
GO
CREATE PROCEDURE sp_ThemDoiTacGiatUi
    @TenDoiTac           NVARCHAR(100),
    @DiaChi              NVARCHAR(255) = NULL,
    @SoDienThoai         NVARCHAR(20),
    @HanThanhToanCongNo  NVARCHAR(30) = N'Theo tháng',
    @TyLeKhachSanHuong   DECIMAL(5,2) = 30.00,
    @GiaVonGiatSay       DECIMAL(18,2) = NULL,
    @GiaVonGiatHap       DECIMAL(18,2) = NULL,
    @GiaVonUiPhang       DECIMAL(18,2) = NULL,
    @NewPartnerId        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @TyLeDoiTacHuong DECIMAL(5,2) = 100.00 - @TyLeKhachSanHuong;

        INSERT INTO DoiTacGiatUi (TenDoiTac, DiaChi, SoDienThoai, HanThanhToanCongNo, TyLeKhachSanHuong, TyLeDoiTacHuong, TrangThai)
        VALUES (@TenDoiTac, @DiaChi, @SoDienThoai, @HanThanhToanCongNo, @TyLeKhachSanHuong, @TyLeDoiTacHuong, 1);

        SET @NewPartnerId = SCOPE_IDENTITY();

        -- Lấy giá bán niêm yết của 3 dịch vụ
        DECLARE @GiaBanGiatSay DECIMAL(18,2), @GiaBanGiatHap DECIMAL(18,2), @GiaBanUiPhang DECIMAL(18,2);
        SELECT @GiaBanGiatSay = DonGiaBanKg FROM LoaiDichVuGiatUi WHERE MaLoai = 'GIAT_SAY';
        SELECT @GiaBanGiatHap = DonGiaBanKg FROM LoaiDichVuGiatUi WHERE MaLoai = 'GIAT_HAP';
        SELECT @GiaBanUiPhang = DonGiaBanKg FROM LoaiDichVuGiatUi WHERE MaLoai = 'UI_PHANG';

        -- Mặc định tính giá vốn theo % chiết khấu nếu không truyền vào cụ thể
        IF @GiaVonGiatSay IS NULL OR @GiaVonGiatSay <= 0
            SET @GiaVonGiatSay = ROUND(@GiaBanGiatSay * (@TyLeDoiTacHuong / 100.0), 0);
        IF @GiaVonGiatHap IS NULL OR @GiaVonGiatHap <= 0
            SET @GiaVonGiatHap = ROUND(@GiaBanGiatHap * (@TyLeDoiTacHuong / 100.0), 0);
        IF @GiaVonUiPhang IS NULL OR @GiaVonUiPhang <= 0
            SET @GiaVonUiPhang = ROUND(@GiaBanUiPhang * (@TyLeDoiTacHuong / 100.0), 0);

        -- Đảm bảo giá vốn không bao giờ lớn hơn hoặc bằng giá niêm yết
        IF @GiaVonGiatSay >= @GiaBanGiatSay SET @GiaVonGiatSay = @GiaBanGiatSay - 1000;
        IF @GiaVonGiatHap >= @GiaBanGiatHap SET @GiaVonGiatHap = @GiaBanGiatHap - 1000;
        IF @GiaVonUiPhang >= @GiaBanUiPhang SET @GiaVonUiPhang = @GiaBanUiPhang - 1000;

        DECLARE @IdGiatSay INT, @IdGiatHap INT, @IdUiPhang INT;
        SELECT @IdGiatSay = Id FROM LoaiDichVuGiatUi WHERE MaLoai = 'GIAT_SAY';
        SELECT @IdGiatHap = Id FROM LoaiDichVuGiatUi WHERE MaLoai = 'GIAT_HAP';
        SELECT @IdUiPhang = Id FROM LoaiDichVuGiatUi WHERE MaLoai = 'UI_PHANG';

        INSERT INTO BangGiaDoiTacGiatUi (DoiTacId, LoaiDichVuId, DonGiaVonKg, GhiChu) VALUES
        (@NewPartnerId, @IdGiatSay, @GiaVonGiatSay, N'Giá vốn giặt sấy thỏa thuận'),
        (@NewPartnerId, @IdGiatHap, @GiaVonGiatHap, N'Giá vốn giặt hấp thỏa thuận'),
        (@NewPartnerId, @IdUiPhang, @GiaVonUiPhang, N'Giá vốn ủi phẳng thỏa thuận');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 5.15 Cập nhật bảng giá vốn của đối tác giặt ủi
IF OBJECT_ID('sp_CapNhatBangGiaDoiTac', 'P') IS NOT NULL DROP PROCEDURE sp_CapNhatBangGiaDoiTac;
GO
CREATE PROCEDURE sp_CapNhatBangGiaDoiTac
    @DoiTacId      INT,
    @LoaiDichVuId  INT,
    @DonGiaVonKg   DECIMAL(18,2),
    @KetQua        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DonGiaBanKg DECIMAL(18,2);
    SELECT @DonGiaBanKg = DonGiaBanKg FROM LoaiDichVuGiatUi WHERE Id = @LoaiDichVuId;

    IF @DonGiaBanKg IS NULL
    BEGIN
        RAISERROR(N'Loại hình dịch vụ không tồn tại.', 16, 1);
        SET @KetQua = 0; RETURN;
    END

    IF @DonGiaVonKg >= @DonGiaBanKg
    BEGIN
        DECLARE @StrVon NVARCHAR(30) = CONVERT(NVARCHAR(30), @DonGiaVonKg);
        DECLARE @StrBan NVARCHAR(30) = CONVERT(NVARCHAR(30), @DonGiaBanKg);
        RAISERROR(N'Giá vốn (%s VNĐ) phải nhỏ hơn giá niêm yết bán cho khách (%s VNĐ)!', 16, 1, @StrVon, @StrBan);
        SET @KetQua = -1; RETURN;
    END

    IF EXISTS (SELECT 1 FROM BangGiaDoiTacGiatUi WHERE DoiTacId = @DoiTacId AND LoaiDichVuId = @LoaiDichVuId)
    BEGIN
        UPDATE BangGiaDoiTacGiatUi
        SET DonGiaVonKg = @DonGiaVonKg
        WHERE DoiTacId = @DoiTacId AND LoaiDichVuId = @LoaiDichVuId;
    END
    ELSE
    BEGIN
        INSERT INTO BangGiaDoiTacGiatUi (DoiTacId, LoaiDichVuId, DonGiaVonKg, GhiChu)
        VALUES (@DoiTacId, @LoaiDichVuId, @DonGiaVonKg, N'Cập nhật giá vốn mới');
    END

    SET @KetQua = 1;
END;
GO


-- 5.x FIX (bổ sung theo yêu cầu): tự động chuyển "Đã đặt" -> "Đang sử dụng" khi đến giờ.
-- Gọi định kỳ (SQL Server Agent Job mỗi 1-5 phút) hoặc app tự gọi khi mở màn hình Sảnh & Sự kiện.
-- Không tự động chuyển "Hoàn tất" — đây PHẢI là thao tác thủ công của lễ tân khi khách trả sảnh.
IF OBJECT_ID('sp_KichHoatDangSuDungSuKien', 'P') IS NOT NULL DROP PROCEDURE sp_KichHoatDangSuDungSuKien;
GO
CREATE PROCEDURE sp_KichHoatDangSuDungSuKien
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE DonDatSuKien
    SET TrangThaiDon = N'Đang sử dụng'
    WHERE TrangThaiDon = N'Đã đặt'
      AND GETDATE() >= ThoiGianBatDau
      AND GETDATE() < ThoiGianKetThuc;

    SELECT @@ROWCOUNT AS SoDonDaKichHoat;
END;
GO

-- 5.x Hoàn tất sự kiện (lễ tân bấm khi khách trả sảnh, thường sau cảnh báo "sắp hết giờ")
IF OBJECT_ID('sp_HoanTatSuKien', 'P') IS NOT NULL DROP PROCEDURE sp_HoanTatSuKien;
GO
CREATE PROCEDURE sp_HoanTatSuKien
    @DonId INT,
    @ChiPhiPhatSinh DECIMAL(18,2) = 0,
    @GhiChuHuHai NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM DonDatSuKien WHERE Id = @DonId AND TrangThaiDon = N'Đang sử dụng')
    BEGIN
        RAISERROR(N'Chỉ có thể hoàn tất đơn đang ở trạng thái Đang sử dụng.', 16, 1);
        RETURN;
    END

    UPDATE DonDatSuKien
    SET TrangThaiDon = N'Hoàn tất',
        ChiPhiPhatSinh = @ChiPhiPhatSinh,
        GhiChuHuHai = ISNULL(@GhiChuHuHai, GhiChuHuHai)
    WHERE Id = @DonId;
    -- Việc thu nốt tiền còn lại + phát sinh gọi tiếp sp_XacNhanThanhToanDonLe (@LoaiDichVu='SuKien')
END;
GO

-- 5.x FIX (bổ sung theo yêu cầu): hủy đơn đặt sảnh — chỉ hoàn cọc nếu báo hủy trước NGÀY
-- diễn ra sự kiện ít nhất 1 ngày (tính theo NGÀY LỊCH, không phải chẵn 24 giờ đồng hồ).
-- VD: thuê thứ Ba -> phải báo hủy chậm nhất trong ngày thứ Hai (trước 00:00 thứ Ba).
IF OBJECT_ID('sp_HuySuKien', 'P') IS NOT NULL DROP PROCEDURE sp_HuySuKien;
GO
CREATE PROCEDURE sp_HuySuKien
    @DonId INT,
    @NguoiHuyId INT,
    @LyDoHuy NVARCHAR(255) = NULL,
    @KetQua NVARCHAR(30) OUTPUT,      -- 'HoanCoc' hoặc 'KhongHoanCoc'
    @SoTienHoan DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ThoiGianBatDauVal DATETIME, @TienCocVal DECIMAL(18,2), @TrangThaiHienTai NVARCHAR(30);

    SELECT @ThoiGianBatDauVal = ThoiGianBatDau, @TienCocVal = TienCoc, @TrangThaiHienTai = TrangThaiDon
    FROM DonDatSuKien WHERE Id = @DonId;

    IF @ThoiGianBatDauVal IS NULL
    BEGIN
        RAISERROR(N'Không tìm thấy đơn đặt sảnh.', 16, 1); RETURN;
    END

    IF @TrangThaiHienTai NOT IN (N'Đã đặt', N'Đang sử dụng')
    BEGIN
        RAISERROR(N'Đơn này không ở trạng thái có thể hủy (đã hoàn tất hoặc đã hủy trước đó).', 16, 1);
        RETURN;
    END

    IF CAST(GETDATE() AS DATE) <= DATEADD(DAY, -1, CAST(@ThoiGianBatDauVal AS DATE))
    BEGIN
        SET @KetQua = N'HoanCoc';
        SET @SoTienHoan = @TienCocVal;
        UPDATE DonDatSuKien
        SET TrangThaiDon = N'Đã hủy - Hoàn cọc', ThoiGianHuy = GETDATE(), LyDoHuy = @LyDoHuy,
            NguoiHuyId = @NguoiHuyId, SoTienHoanCoc = @TienCocVal,
            -- Đơn đã hủy không còn khoản nào phải thu/trả qua luồng phòng nữa
            DaThanhToan = 1, NguoiThanhToanId = @NguoiHuyId, ThoiGianThanhToan = GETDATE()
        WHERE Id = @DonId;
    END
    ELSE
    BEGIN
        SET @KetQua = N'KhongHoanCoc';
        SET @SoTienHoan = 0;
        UPDATE DonDatSuKien
        SET TrangThaiDon = N'Đã hủy - Không hoàn cọc', ThoiGianHuy = GETDATE(), LyDoHuy = @LyDoHuy,
            NguoiHuyId = @NguoiHuyId, SoTienHoanCoc = 0,
            DaThanhToan = 1, NguoiThanhToanId = @NguoiHuyId, ThoiGianThanhToan = GETDATE()
        WHERE Id = @DonId;
    END
END;
GO

-- 5.x Tự động chuyển "Đã đặt" -> "Đang thuê" khi đến giờ thuê xe
IF OBJECT_ID('sp_KichHoatDangThueXe', 'P') IS NOT NULL DROP PROCEDURE sp_KichHoatDangThueXe;
GO
CREATE PROCEDURE sp_KichHoatDangThueXe
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE DonThueXe
    SET TrangThaiDon = N'Đang thuê'
    WHERE TrangThaiDon = N'Đã đặt'
      AND GETDATE() >= NgayThue
      AND GETDATE() < NgayTraDuKien;

    SELECT @@ROWCOUNT AS SoDonDaKichHoat;
END;
GO

-- 5.x Hoàn tất đơn thuê xe (lễ tân bấm khi khách trả xe)
IF OBJECT_ID('sp_HoanTatThueXe', 'P') IS NOT NULL DROP PROCEDURE sp_HoanTatThueXe;
GO
CREATE PROCEDURE sp_HoanTatThueXe
    @DonId INT,
    @ChiPhiPhatSinh DECIMAL(18,2) = 0,
    @GhiChuHuHai NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM DonThueXe WHERE Id = @DonId AND TrangThaiDon = N'Đang thuê')
    BEGIN
        RAISERROR(N'Chỉ có thể hoàn tất đơn đang ở trạng thái Đang thuê.', 16, 1);
        RETURN;
    END

    UPDATE DonThueXe
    SET TrangThaiDon = N'Hoàn tất',
        NgayTraThucTe = GETDATE(),
        ChiPhiPhatSinh = @ChiPhiPhatSinh,
        TongTienThanhToan = TienThue + @ChiPhiPhatSinh,
        GhiChuHuHai = ISNULL(@GhiChuHuHai, GhiChuHuHai)
    WHERE Id = @DonId;
END;
GO

-- 5.x Hủy đơn thuê xe — quy tắc hoàn cọc tương tự sự kiện (hủy trước ngày thuê theo ngày lịch được hoàn cọc)
IF OBJECT_ID('sp_HuyThueXe', 'P') IS NOT NULL DROP PROCEDURE sp_HuyThueXe;
GO
CREATE PROCEDURE sp_HuyThueXe
    @DonId INT,
    @NguoiHuyId INT,
    @LyDoHuy NVARCHAR(255) = NULL,
    @KetQua NVARCHAR(30) OUTPUT,      -- 'HoanCoc' hoặc 'KhongHoanCoc'
    @SoTienHoan DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NgayThueVal DATETIME, @TienCocVal DECIMAL(18,2), @TrangThaiHienTai NVARCHAR(30);

    SELECT @NgayThueVal = NgayThue, @TienCocVal = TienCoc, @TrangThaiHienTai = TrangThaiDon
    FROM DonThueXe WHERE Id = @DonId;

    IF @NgayThueVal IS NULL
    BEGIN
        RAISERROR(N'Không tìm thấy đơn thuê xe.', 16, 1); RETURN;
    END

    IF @TrangThaiHienTai NOT IN (N'Đã đặt', N'Đang thuê')
    BEGIN
        RAISERROR(N'Đơn này không ở trạng thái có thể hủy (đã hoàn tất hoặc đã hủy trước đó).', 16, 1);
        RETURN;
    END

    -- So sánh theo ngày lịch: hủy trước ngày bắt đầu thuê ít nhất 1 ngày thì được hoàn cọc
    IF CAST(GETDATE() AS DATE) <= DATEADD(DAY, -1, CAST(@NgayThueVal AS DATE))
    BEGIN
        SET @KetQua = N'HoanCoc';
        SET @SoTienHoan = @TienCocVal;
        UPDATE DonThueXe
        SET TrangThaiDon = N'Đã hủy - Hoàn cọc', ThoiGianHuy = GETDATE(), LyDoHuy = @LyDoHuy,
            NguoiHuyId = @NguoiHuyId, SoTienHoanCoc = @TienCocVal,
            DaThanhToan = 1, NguoiThanhToanId = @NguoiHuyId, ThoiGianThanhToan = GETDATE()
        WHERE Id = @DonId;
    END
    ELSE
    BEGIN
        SET @KetQua = N'KhongHoanCoc';
        SET @SoTienHoan = 0;
        UPDATE DonThueXe
        SET TrangThaiDon = N'Đã hủy - Không hoàn cọc', ThoiGianHuy = GETDATE(), LyDoHuy = @LyDoHuy,
            NguoiHuyId = @NguoiHuyId, SoTienHoanCoc = 0,
            DaThanhToan = 1, NguoiThanhToanId = @NguoiHuyId, ThoiGianThanhToan = GETDATE()
        WHERE Id = @DonId;
    END
END;
GO


-- =====================================================================================
-- PHẦN 6: DỮ LIỆU TĨNH & DANH MỤC HỆ THỐNG (SYSTEM & MASTER DATA)
-- =====================================================================================

-- 6.1 Tài khoản đăng nhập mặc định (Mật khẩu mặc định: 123)
INSERT INTO TaiKhoan (TenDangNhap, MatKhauHash, HoTen, VaiTro, SoDienThoai, Email) VALUES
('admin', 'ubJSwSb+V3dx+o3DsP1XwWiiBuvs6i40PTcsb1ziOsU=', N'Chủ Khách Sạn', N'Admin', '0948198812', 'dalathotel@gmail.com'),
('letan', 'ubJSwSb+V3dx+o3DsP1XwWiiBuvs6i40PTcsb1ziOsU=', N'Lễ Tân Trực Chính', N'LeTan', '0912345678', 'letan@dalathotel.vn');
GO

-- 6.2 Nhân viên
INSERT INTO NhanVien (MaNV, HoTen, CCCD, SoDienThoai, Email, ChucVu, LuongCoBan, TrangThai) VALUES
('NV001', N'Trần Thị Mai',  '001198000123', '0912345678', 'maitran@dalathotel.vn',   N'Lễ tân trưởng', 12000000, N'Đang làm việc'),
('NV002', N'Nguyễn Văn An',  '001199000456', '0987654321', 'annguyen@dalathotel.vn',  N'Nhân viên lễ tân',8500000, N'Đang làm việc'),
('NV003', N'Lê Hoàng Nam',   '001197000789', '0933112233', 'namle@dalathotel.vn',     N'Nhân viên lễ tân',8500000, N'Đang làm việc');
GO

-- 6.3 Ca trực
INSERT INTO CaTruc (TenCa, GioBatDau, GioKetThuc, GhiChu) VALUES
(N'Ca Sáng',  '06:00:00', '14:00:00', N'Tiếp nhận khách check-in sáng, kiểm tra mini-bar'),
(N'Ca Chiều', '14:00:00', '22:00:00', N'Đỉnh điểm check-in/out, bàn giao xe cho thuê'),
(N'Ca Đêm',   '22:00:00', '06:00:00', N'Trực đêm, bảo an bãi xe, chốt ca ngày');
GO

-- 6.4 Phân công ca trực cho 3 nhân viên luân phiên dày đặc hết tháng này (mỗi ngày 3 ca cho 3 người)
INSERT INTO PhanCongCaTruc (NhanVienId, CaTrucId, NgayTruc, GhiChu) VALUES
-- Hôm qua (Ngày -1)
(2, 1, CAST(DATEADD(DAY,-1,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,-1,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,-1,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Hôm nay (Ngày 0 - 05/09)
(1, 1, CAST(GETDATE() AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(GETDATE() AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(GETDATE() AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 1 (06/09)
(2, 1, CAST(DATEADD(DAY,1,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,1,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,1,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Ngày 2 (07/09)
(3, 1, CAST(DATEADD(DAY,2,GETDATE()) AS DATE), N'Lê Hoàng Nam ca sáng'),
(1, 2, CAST(DATEADD(DAY,2,GETDATE()) AS DATE), N'Trần Thị Mai ca chiều'),
(2, 3, CAST(DATEADD(DAY,2,GETDATE()) AS DATE), N'Nguyễn Văn An ca đêm'),

-- Ngày 3 (08/09)
(1, 1, CAST(DATEADD(DAY,3,GETDATE()) AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(DATEADD(DAY,3,GETDATE()) AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(DATEADD(DAY,3,GETDATE()) AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 4 (09/09)
(2, 1, CAST(DATEADD(DAY,4,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,4,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,4,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Ngày 5 (10/09)
(3, 1, CAST(DATEADD(DAY,5,GETDATE()) AS DATE), N'Lê Hoàng Nam ca sáng'),
(1, 2, CAST(DATEADD(DAY,5,GETDATE()) AS DATE), N'Trần Thị Mai ca chiều'),
(2, 3, CAST(DATEADD(DAY,5,GETDATE()) AS DATE), N'Nguyễn Văn An ca đêm'),

-- Ngày 6 (11/09)
(1, 1, CAST(DATEADD(DAY,6,GETDATE()) AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(DATEADD(DAY,6,GETDATE()) AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(DATEADD(DAY,6,GETDATE()) AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 7 (12/09)
(2, 1, CAST(DATEADD(DAY,7,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,7,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,7,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Ngày 8 (13/09)
(3, 1, CAST(DATEADD(DAY,8,GETDATE()) AS DATE), N'Lê Hoàng Nam ca sáng'),
(1, 2, CAST(DATEADD(DAY,8,GETDATE()) AS DATE), N'Trần Thị Mai ca chiều'),
(2, 3, CAST(DATEADD(DAY,8,GETDATE()) AS DATE), N'Nguyễn Văn An ca đêm'),

-- Ngày 9 (14/09)
(1, 1, CAST(DATEADD(DAY,9,GETDATE()) AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(DATEADD(DAY,9,GETDATE()) AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(DATEADD(DAY,9,GETDATE()) AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 10 (15/09)
(2, 1, CAST(DATEADD(DAY,10,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,10,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,10,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Ngày 11 (16/09)
(3, 1, CAST(DATEADD(DAY,11,GETDATE()) AS DATE), N'Lê Hoàng Nam ca sáng'),
(1, 2, CAST(DATEADD(DAY,11,GETDATE()) AS DATE), N'Trần Thị Mai ca chiều'),
(2, 3, CAST(DATEADD(DAY,11,GETDATE()) AS DATE), N'Nguyễn Văn An ca đêm'),

-- Ngày 12 (17/09)
(1, 1, CAST(DATEADD(DAY,12,GETDATE()) AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(DATEADD(DAY,12,GETDATE()) AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(DATEADD(DAY,12,GETDATE()) AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 13 (18/09)
(2, 1, CAST(DATEADD(DAY,13,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,13,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,13,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Ngày 14 (19/09)
(3, 1, CAST(DATEADD(DAY,14,GETDATE()) AS DATE), N'Lê Hoàng Nam ca sáng'),
(1, 2, CAST(DATEADD(DAY,14,GETDATE()) AS DATE), N'Trần Thị Mai ca chiều'),
(2, 3, CAST(DATEADD(DAY,14,GETDATE()) AS DATE), N'Nguyễn Văn An ca đêm'),

-- Ngày 15 (20/09)
(1, 1, CAST(DATEADD(DAY,15,GETDATE()) AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(DATEADD(DAY,15,GETDATE()) AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(DATEADD(DAY,15,GETDATE()) AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 16 (21/09)
(2, 1, CAST(DATEADD(DAY,16,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,16,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,16,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Ngày 17 (22/09)
(3, 1, CAST(DATEADD(DAY,17,GETDATE()) AS DATE), N'Lê Hoàng Nam ca sáng'),
(1, 2, CAST(DATEADD(DAY,17,GETDATE()) AS DATE), N'Trần Thị Mai ca chiều'),
(2, 3, CAST(DATEADD(DAY,17,GETDATE()) AS DATE), N'Nguyễn Văn An ca đêm'),

-- Ngày 18 (23/09)
(1, 1, CAST(DATEADD(DAY,18,GETDATE()) AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(DATEADD(DAY,18,GETDATE()) AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(DATEADD(DAY,18,GETDATE()) AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 19 (24/09)
(2, 1, CAST(DATEADD(DAY,19,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,19,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,19,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Ngày 20 (25/09)
(3, 1, CAST(DATEADD(DAY,20,GETDATE()) AS DATE), N'Lê Hoàng Nam ca sáng'),
(1, 2, CAST(DATEADD(DAY,20,GETDATE()) AS DATE), N'Trần Thị Mai ca chiều'),
(2, 3, CAST(DATEADD(DAY,20,GETDATE()) AS DATE), N'Nguyễn Văn An ca đêm'),

-- Ngày 21 (26/09)
(1, 1, CAST(DATEADD(DAY,21,GETDATE()) AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(DATEADD(DAY,21,GETDATE()) AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(DATEADD(DAY,21,GETDATE()) AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 22 (27/09)
(2, 1, CAST(DATEADD(DAY,22,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,22,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,22,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm'),

-- Ngày 23 (28/09)
(3, 1, CAST(DATEADD(DAY,23,GETDATE()) AS DATE), N'Lê Hoàng Nam ca sáng'),
(1, 2, CAST(DATEADD(DAY,23,GETDATE()) AS DATE), N'Trần Thị Mai ca chiều'),
(2, 3, CAST(DATEADD(DAY,23,GETDATE()) AS DATE), N'Nguyễn Văn An ca đêm'),

-- Ngày 24 (29/09)
(1, 1, CAST(DATEADD(DAY,24,GETDATE()) AS DATE), N'Trần Thị Mai ca sáng'),
(2, 2, CAST(DATEADD(DAY,24,GETDATE()) AS DATE), N'Nguyễn Văn An ca chiều'),
(3, 3, CAST(DATEADD(DAY,24,GETDATE()) AS DATE), N'Lê Hoàng Nam ca đêm'),

-- Ngày 25 (30/09 - Hết tháng này)
(2, 1, CAST(DATEADD(DAY,25,GETDATE()) AS DATE), N'Nguyễn Văn An ca sáng'),
(3, 2, CAST(DATEADD(DAY,25,GETDATE()) AS DATE), N'Lê Hoàng Nam ca chiều'),
(1, 3, CAST(DATEADD(DAY,25,GETDATE()) AS DATE), N'Trần Thị Mai ca đêm');
GO

-- 6.5 Danh mục 5 dịch vụ hệ thống
INSERT INTO DichVuHeThong (MaDV, TenDV, LoaiDichVu, DangHoatDong, MoTa) VALUES
('AN_UONG', N'Ăn Uống',              N'Tự túc',        1, N'Bán lẻ hàng khô, nước giải khát, snack tại phòng & quầy lễ tân'),
('SU_KIEN', N'Sảnh & Sự Kiện',        N'Tự túc',        1, N'Cho thuê sảnh tiệc, phòng họp VIP, sân thượng Sky Lounge theo giờ/ngày'),
('THUE_XE', N'Cho Thuê Xe Máy',      N'Tự túc',        1, N'Cho thuê xe tay ga, xe số kèm mũ bảo hiểm'),
('DO_XE',   N'Bãi & Hầm Đỗ Xe',      N'Tự túc',        1, N'Quản lý trông giữ xe máy, ô tô có thu phí theo lượt/giờ/ngày'),
('GIAT_UI', N'Giặt Ủi',              N'Dịch vụ ngoài', 1, N'Liên kết đối tác giặt sấy, giặt hấp lấy hoa hồng 25-35%');
GO

-- 6.6 Cấu hình tham số hệ thống
INSERT INTO ThamSoHeThong (MaThamSo, TenThamSo, GiaTri, MoTa) VALUES
('CHECKIN_STANDARD_TIME',       '14:00',      N'14:00', N'Giờ nhận phòng tiêu chuẩn'),
('CHECKOUT_STANDARD_TIME',      '12:00',      N'12:00', N'Giờ trả phòng tiêu chuẩn'),
('EVENT_ADVANCE_ALERT_MINUTES', '15',         N'15',    N'Báo trước khi sự kiện sắp hết giờ (phút)'),
-- FIX (bổ sung theo yêu cầu): thời gian dọn dẹp mặc định giữa 2 đơn liên tiếp cùng khu vực
('EVENT_CLEANING_BUFFER_MINUTES','60',        N'60',    N'Thời gian dọn dẹp/kiểm tra tối thiểu giữa 2 đơn liên tiếp cùng 1 khu vực sự kiện (phút)'),
('VEHICLE_ADVANCE_ALERT_MINUTES','30',        N'30',    N'Báo trước khi xe sắp hết giờ thuê (phút)'),
('VEHICLE_CLEANING_BUFFER_MINUTES','30',      N'30',    N'Thời gian vệ sinh/kiểm tra xe tối thiểu giữa 2 lượt thuê liên tiếp cùng 1 xe (phút)');
GO

-- 6.7 Danh mục 20 phòng khách sạn (Tầng 1 đến Tầng 5)
-- 13 phòng đang ở (~2/3 tổng số phòng), thời gian lưu trú đầy đủ 2-3 ngày, 1-2 tuần từ hôm nay
INSERT INTO PhongKhachSan (SoPhong, Tang, LoaiPhong, TrangThai, TenKhach, SoDienThoai, CCCD, GhiChu, NgayNhanPhong, NgayTraPhong) VALUES
('101', 1, N'Phòng đơn', N'Đang ở',   N'Đoàn Minh Trí',     '0908112233', '079198001234', N'Khách ở 2 ngày từ hôm nay, thuê xe SH', GETDATE(), DATEADD(DAY,2,GETDATE())),
('102', 1, N'Phòng đơn', N'Đang ở',   N'Trịnh Hồng Quân',   '0909123456', '079192003344', N'Khách ở 3 ngày từ hôm nay, thuê xe Lead', GETDATE(), DATEADD(DAY,3,GETDATE())),
('103', 1, N'Phòng đôi', N'Đang ở',   N'Vũ Hoàng Nam',      '0912334455', '079193005566', N'Khách ở 2 tuần từ hôm nay, thuê xe Grande', GETDATE(), DATEADD(DAY,14,GETDATE())),
('104', 1, N'Phòng đôi', N'Đang dọn', NULL,                 NULL,         NULL,           N'Khách vừa trả phòng lúc sáng', NULL, NULL),
('201', 2, N'Phòng đơn', N'Đang ở',   N'Hoàng Mai Linh',    '0978112244', '079194007788', N'Khách ở 2 ngày từ hôm nay, thuê xe Future', GETDATE(), DATEADD(DAY,2,GETDATE())),
('202', 2, N'Phòng đôi', N'Đang ở',   N'Lê Tấn Phát',       '0969888999', '079201009876', N'Khách ở 1 tuần từ hôm nay, thuê xe AirBlade', GETDATE(), DATEADD(DAY,7,GETDATE())),
('203', 2, N'Phòng đôi', N'Đang ở',   N'Bùi Thanh Tùng',    '0938445566', '079196001122', N'Khách ở 2 tuần từ hôm nay, thuê xe Winner X', GETDATE(), DATEADD(DAY,14,GETDATE())),
('204', 2, N'Phòng đơn', N'Bảo trì',  NULL,                 NULL,         NULL,           N'Đang sửa vòi sen toilet', NULL, NULL),
('301', 3, N'Phòng đôi', N'Đang ở',   N'Nguyễn Anh Thư',    '0918777666', '079195003456', N'Khách ở 2 tuần từ hôm nay (honeymoon, thuê xe Vision)', GETDATE(), DATEADD(DAY,14,GETDATE())),
('302', 3, N'Phòng đơn', N'Đang ở',   N'Đặng Hải Yến',      '0982334411', '079197003355', N'Khách ở 3 ngày từ hôm nay, thuê xe Vespa', GETDATE(), DATEADD(DAY,3,GETDATE())),
('303', 3, N'Phòng đôi', N'Đang ở',   N'Phan Văn Đức',      '0945667788', '079199004466', N'Khách ở 2 tuần từ hôm nay, thuê xe NVX', GETDATE(), DATEADD(DAY,14,GETDATE())),
('304', 3, N'Phòng đơn', N'Trống',    NULL,                 NULL,         NULL,           NULL, NULL, NULL),
('401', 4, N'Phòng đôi', N'Đang ở',   N'Phạm Quốc Bảo',     '0933444555', '079188005678', N'Khách ở 2 tuần từ hôm nay, gửi xe ô tô 7 chỗ', GETDATE(), DATEADD(DAY,14,GETDATE())),
('402', 4, N'Phòng đơn', N'Đang ở',   N'Trần Thu Hà',       '0919223344', '079189006677', N'Khách ở 3 ngày từ hôm nay, thuê xe Liberty', GETDATE(), DATEADD(DAY,3,GETDATE())),
('403', 4, N'Phòng đôi', N'Đang ở',   N'Đỗ Minh Khang',     '0987556677', '079187008899', N'Khách ở 1 tuần từ hôm nay, thuê xe Exciter', GETDATE(), DATEADD(DAY,7,GETDATE())),
('404', 4, N'Phòng đơn', N'Trống',    NULL,                 NULL,         NULL,           NULL, NULL, NULL),
('501', 5, N'Phòng đôi', N'Đang ở',   N'Nguyễn Hoàng Long', '0903889900', '079186001234', N'Khách ở 2 tuần từ hôm nay, thuê xe PG-1', GETDATE(), DATEADD(DAY,14,GETDATE())),
('502', 5, N'Phòng đơn', N'Đang ở',   N'Lê Thị Ngọc Ánh',   '0979334455', '079185002345', N'Khách ở 1 tuần từ hôm nay, thuê xe Sirius', GETDATE(), DATEADD(DAY,7,GETDATE())),
('503', 5, N'Phòng đôi', N'Đang dọn', NULL,                 NULL,         NULL,           N'Đang thay ga trải giường', NULL, NULL),
('504', 5, N'Phòng đơn', N'Trống',    NULL,                 NULL,         NULL,           NULL, NULL, NULL);
GO

-- 6.8 Nhà cung cấp
INSERT INTO NhaCungCap (TenNhaCungCap, NguoiLienHe, SoDienThoai, DiaChi, Email, GhiChu, TrangThai) VALUES
(N'Công ty CP Hàng tiêu dùng Masan', N'Nguyễn Văn Toàn', '02838221100', N'Tầng 12 Central Plaza, Q.1, TP.HCM', 'contact@masan.vn', N'Cung cấp nước khoáng, nước ngọt, mì ly ăn liền', 1),
(N'Công ty TNHH Nestlé Việt Nam',     N'Phạm Mai Lan',   '02839112233', N'KCN Biên Hòa 2, Đồng Nai',          'order@vn.nestle.com', N'Cung cấp cà phê G7, trà đóng chai, nước tăng lực', 1),
(N'Cơ sở Đặc sản Rượu Vang Đà Lạt',  N'Lê Hữu Thọ',     '02633822456', N'Phường 8, TP. Đà Lạt',             'sales@dalatwine.com', N'Cung cấp rượu vang, mứt dâu, trà Atiso cao cấp', 1);
GO

-- 6.9 Danh mục món ăn & mini-bar
INSERT INTO MonAn (TenMon, DanhMuc, DonViTinh, GiaBan, GiaVonBinhQuan, SoLuongTonKho, MucTonToiThieu, NhaCungCapId, GhiChu) VALUES
(N'Mì tôm ly Modern',               N'Đồ ăn nhẹ',       N'Ly',    20000, 11000, 150, 20, 1, N'Hạn dùng 8 tháng'),
(N'Nước khoáng Lavie 500ml',        N'Nước giải khát',  N'Chai',  15000,  6000, 240, 30, 1, N'Hạn dùng 12 tháng'),
(N'Coca Cola lon 330ml',            N'Nước giải khát',  N'Lon',   20000, 10000, 180, 25, 1, N'Hạn dùng 9 tháng'),
(N'Cà phê hòa tan G7 (hộp 20 gói)', N'Đồ uống pha',     N'Hộp',   45000, 22000,  80, 15, 2, N'Hạn dùng 18 tháng'),
(N'Trà Atiso túi lọc Đà Lạt',       N'Đồ uống pha',     N'Hộp',   60000, 30000,  60, 10, 3, N'Đặc sản Đà Lạt, HSD 12 tháng'),
(N'Snack khoai tây Lay''s',         N'Đồ ăn nhẹ',       N'Gói',   25000, 13000, 120, 20, 1, N'Hạn dùng 6 tháng'),
(N'Rượu vang đỏ Đà Lạt 750ml',      N'Rượu cao cấp',    N'Chai', 220000,120000,  40,  8, 3, N'Hạn dùng 36 tháng'),
(N'Hạt điều rang muối 200g',        N'Đồ ăn nhẹ',       N'Hũ',    85000, 45000,  50, 10, 3, N'Hạn dùng 12 tháng'),
(N'Nước tăng lực Red Bull lon',     N'Nước giải khát',  N'Lon',   25000, 12500,  90, 15, 2, N'Hạn dùng 12 tháng'),
(N'Socola thanh KitKat 4 finger',   N'Bánh kẹo',        N'Thanh', 22000, 11000,  70, 15, 2, N'Hạn dùng 10 tháng'),
(N'Trà xanh C2 hương chanh 455ml',  N'Nước giải khát',  N'Chai',  15000,  7000, 110, 20, 1, N'Hạn dùng 9 tháng'),
(N'Bia Heineken lon 330ml',         N'Đồ uống có cồn',  N'Lon',   35000, 18000, 100, 20, 1, N'Hạn dùng 12 tháng'),
(N'Mứt dâu tây Đà Lạt 250g',        N'Đặc sản',         N'Hũ',    55000, 28000,  45, 10, 3, N'Hạn dùng 6 tháng');
GO

-- 6.10 Sảnh & khu vực sự kiện (Chỉ cho thuê và tính tiền theo giờ)
-- FIX: TrangThai giờ chỉ còn 'Hoạt động'/'Ngừng khai thác' (xem giải thích ở CREATE TABLE)
INSERT INTO KhuVucSuKien (TenKhuVuc, LoaiKhuVuc, SucChua, GiaThueTheoGio, TrangThietBi, TrangThai) VALUES
(N'Sảnh Đại Tiệc Grand Diamond',    N'Sảnh lớn',             250, 1500000, N'Âm thanh Line Array, màn LED P3, ánh sáng Moving Head, 25 bàn tiệc tròn', N'Hoạt động'),
(N'Phòng Hội Nghị VIP Ruby',        N'Phòng họp kín',         40,  500000, N'Bàn hội nghị chữ U, 2 máy chiếu laser 4K, 8 mic không dây',              N'Hoạt động'),
(N'Sky Lounge Sân Thượng Sunset',   N'Sân thượng ngoài trời',120, 1200000, N'Quầy bar cocktail, loa Bose ngoài trời, đèn LED dây vintage',            N'Hoạt động'),
(N'Phòng Trà Zen Garden',           N'Phòng nhỏ trong nhà',   20,  300000, N'Bàn trà gỗ, sân vườn nhỏ, phù hợp họp mặt gia đình',                     N'Hoạt động');
GO

-- 6.11 Danh mục 20 xe máy cho thuê (18 xe Hoạt động/sẵn sàng khai thác, 2 xe Bảo trì)
-- Trạng thái động thực tế (Đang thuê, Đang vệ sinh kiểm tra...) được vw_TrangThaiXeHienTai tính toán theo giờ thực
INSERT INTO XeChoThue (BienSo, TenXe, LoaiXe, GiaThueNgay, TienCocQuyDinh, TrangThai, GhiChuTinhTrang) VALUES
('59-F2 888.68', N'Honda SH 150i ABS 2024',     N'Xe tay ga', 300000, 1000000, N'Hoạt động', N'Xe mới 99%, kèm 2 mũ bảo hiểm cao cấp'),
('59-F2 777.23', N'Honda AirBlade 160 2023',    N'Xe tay ga', 180000,  500000, N'Hoạt động', N'Xe sạch đẹp, máy êm'),
('59-F2 555.45', N'Honda Vision Smartkey 2024', N'Xe tay ga', 150000,  500000, N'Hoạt động', N'Xe sạch đẹp, tiết kiệm xăng'),
('59-F2 333.12', N'Yamaha Exciter 155 VVA',     N'Xe côn tay',170000,  500000, N'Hoạt động', N'Xích sên đã bôi trơn bảo dưỡng'),
('59-F2 111.89', N'Honda Wave Alpha 110',       N'Xe số',     100000,  300000, N'Bảo trì',   N'Đang thay nhớt & bugi định kỳ'),
('59-F2 222.34', N'Honda Lead 125 Smartkey 2024',N'Xe tay ga',160000,  500000, N'Hoạt động', N'Cốp rộng, xe êm ái'),
('59-F2 444.56', N'Yamaha Grande Hybrid 2024', N'Xe tay ga', 170000,  500000, N'Hoạt động', N'Động cơ BlueCore Hybrid cực êm'),
('59-F2 666.78', N'Honda Future 125 Fi',        N'Xe số',     120000,  300000, N'Hoạt động', N'Phun xăng điện tử tiết kiệm nhiên liệu'),
('59-F2 999.01', N'Honda Winner X 150 ABS',     N'Xe côn tay',170000,  500000, N'Hoạt động', N'Phanh ABS an toàn, lốp mới'),
('59-F2 123.77', N'Vespa Primavera 125 ABS',    N'Xe tay ga', 280000, 1000000, N'Hoạt động', N'Phong cách thanh lịch, xe cao cấp'),
('59-F2 234.88', N'Yamaha NVX 155 VVA',         N'Xe tay ga', 190000,  500000, N'Hoạt động', N'Dáng thể thao hầm hố, máy bốc'),
('59-F2 345.99', N'Honda Blade 110',           N'Xe số',     100000,  300000, N'Hoạt động', N'Xe gọn nhẹ, đi đường dốc tốt'),
('59-F2 456.11', N'Piaggio Liberty 125 ABS',    N'Xe tay ga', 220000,  800000, N'Hoạt động', N'Bánh lớn đầm chắc, tôn dáng'),
('59-F2 567.22', N'Yamaha PG-1 115 Phượt',      N'Xe côn tay',150000,  500000, N'Hoạt động', N'Lốp gai địa hình, thích hợp cắm trại'),
('59-F2 678.33', N'Yamaha Sirius Fi 115',       N'Xe số',     100000,  300000, N'Hoạt động', N'Máy bốc bền bỉ, tiết kiệm xăng'),
('59-F2 789.44', N'Honda AirBlade 125 2024',    N'Xe tay ga', 160000,  500000, N'Hoạt động', N'Khóa thông minh Smartkey'),
('59-F2 890.55', N'Suzuki Raider R150 Fi',      N'Xe côn tay',180000,  500000, N'Bảo trì',   N'Đang cân chỉnh nhông sên dĩa'),
('59-F2 901.66', N'Honda Vario 160 ABS',        N'Xe tay ga', 200000,  600000, N'Hoạt động', N'Xe mới sạch đẹp, kèm 2 mũ bảo hiểm'),
('59-F2 012.77', N'Honda Wave RSX Fi 110',      N'Xe số',     110000,  300000, N'Hoạt động', N'Tiết kiệm xăng, máy êm'),
('59-F2 123.88', N'Yamaha Exciter 150 GP',      N'Xe côn tay',160000,  500000, N'Hoạt động', N'Xe nguyên bản, phanh đĩa trước sau');
GO

-- 6.12 Bảng giá bán dịch vụ giặt ủi chung (Khóa giá bán thống nhất áp dụng cho mọi khách)
INSERT INTO LoaiDichVuGiatUi (MaLoai, TenLoai, DonGiaBanKg, DonViTinh, HeSoBoiThuongToiDa, DangKinhDoanh) VALUES
('GIAT_SAY', N'Giặt sấy thông thường', 30000, N'Kg', 10, 1),
('GIAT_HAP', N'Giặt hấp cao cấp',     80000, N'Kg', 10, 1),
('UI_PHANG', N'Ủi phẳng',              40000, N'Kg', 10, 1);
GO

-- 6.13 Đối tác giặt ủi ngoài
INSERT INTO DoiTacGiatUi (TenDoiTac, DiaChi, SoDienThoai, HanThanhToanCongNo, TyLeKhachSanHuong, TyLeDoiTacHuong, TrangThai) VALUES
(N'Tiệm Giặt Sấy Tốc Hành EcoClean',        N'Số 12 Đường Hoa Cúc, P.7, Đà Lạt',    '02633445566', N'Theo tháng', 30.00, 70.00, 1),
(N'Xưởng Giặt Hấp Cao Cấp Royal Laundry',   N'Số 88 Nguyễn Chí Thanh, Đà Lạt',      '02633112233', N'Theo tháng', 35.00, 65.00, 1),
(N'Tiệm Giặt Ủi Nhanh Sương Mai',           N'Số 45 Trần Hưng Đạo, Đà Lạt',         '02633998877', N'Theo tháng', 25.00, 75.00, 0);
GO

-- 6.14 Bảng giá vốn đối tác giặt ủi (Mỗi đối tác x mỗi loại dịch vụ = đơn giá vốn riêng, luôn NHỎ HƠN giá niêm yết)
INSERT INTO BangGiaDoiTacGiatUi (DoiTacId, LoaiDichVuId, DonGiaVonKg, GhiChu) VALUES
-- 1. Tiệm Giặt Sấy Tốc Hành EcoClean (Id = 1): Chuyên giặt sấy dân dụng
(1, 1, 21000, N'Giá vốn giặt sấy 21k/kg (Giá bán 30k -> KS lời 9k/kg ~ 30%)'),
(1, 2, 56000, N'Giá vốn giặt hấp 56k/kg (Giá bán 80k -> KS lời 24k/kg ~ 30%)'),
(1, 3, 28000, N'Giá vốn ủi phẳng 28k/kg (Giá bán 40k -> KS lời 12k/kg ~ 30%)'),

-- 2. Xưởng Giặt Hấp Cao Cấp Royal Laundry (Id = 2): Chuyên đồ vest, đầm lụa, chiết khấu hấp dẫn
(2, 1, 20000, N'Giá vốn giặt sấy 20k/kg (Giá bán 30k -> KS lời 10k/kg ~ 33.3%)'),
(2, 2, 52000, N'Giá vốn giặt hấp 52k/kg (Giá bán 80k -> KS lời 28k/kg ~ 35%)'),
(2, 3, 26000, N'Giá vốn ủi phẳng 26k/kg (Giá bán 40k -> KS lời 14k/kg ~ 35%)'),

-- 3. Tiệm Giặt Ủi Nhanh Sương Mai (Id = 3): Đã ngừng hợp tác (Lưu dữ liệu lịch sử đối soát)
(3, 1, 22500, N'Giá vốn giặt sấy cũ lúc còn hợp tác (22.5k/kg)'),
(3, 2, 60000, N'Giá vốn giặt hấp cũ lúc còn hợp tác (60k/kg)'),
(3, 3, 30000, N'Giá vốn ủi phẳng cũ lúc còn hợp tác (30k/kg)');
GO


-- =====================================================================================
-- PHẦN 7: DỮ LIỆU MẪU GIAO DỊCH PHÁT SINH (TRANSACTIONS & OPERATIONAL SAMPLE DATA)
-- =====================================================================================

-- 7.1 Khách hàng mẫu (14 khách lưu trú phòng + khách vãng lai)
INSERT INTO KhachHang (HoTen, CCCD, SoDienThoai, SoPhong, GhiChu, NgayTao) VALUES
(N'Đoàn Minh Trí',     '079198001234', '0908112233', '101', N'Khách lưu trú phòng 101 (2 ngày, thuê xe SH)', GETDATE()),
(N'Trịnh Hồng Quân',   '079192003344', '0909123456', '102', N'Khách lưu trú phòng 102 (3 ngày, thuê xe Lead)', GETDATE()),
(N'Vũ Hoàng Nam',      '079193005566', '0912334455', '103', N'Khách lưu trú phòng 103 (2 tuần, thuê xe Grande)', GETDATE()),
(N'Hoàng Mai Linh',    '079194007788', '0978112244', '201', N'Khách lưu trú phòng 201 (2 ngày, thuê xe Future)', GETDATE()),
(N'Lê Tấn Phát',       '079201009876', '0969888999', '202', N'Khách lưu trú phòng 202 (1 tuần, thuê xe AirBlade)', GETDATE()),
(N'Bùi Thanh Tùng',    '079196001122', '0938445566', '203', N'Khách lưu trú phòng 203 (2 tuần, thuê xe Winner X)', GETDATE()),
(N'Nguyễn Anh Thư',    '079195003456', '0918777666', '301', N'Khách lưu trú phòng 301 (2 tuần honeymoon, thuê xe Vision)', GETDATE()),
(N'Đặng Hải Yến',      '079197003355', '0982334411', '302', N'Khách lưu trú phòng 302 (3 ngày, thuê xe Vespa)', GETDATE()),
(N'Phan Văn Đức',      '079199004466', '0945667788', '303', N'Khách lưu trú phòng 303 (2 tuần, thuê xe NVX)', GETDATE()),
(N'Phạm Quốc Bảo',     '079188005678', '0933444555', '401', N'Khách lưu trú phòng 401 (2 tuần, gửi ô tô 7 chỗ)', GETDATE()),
(N'Trần Thu Hà',       '079189006677', '0919223344', '402', N'Khách lưu trú phòng 402 (3 ngày, thuê xe Liberty)', GETDATE()),
(N'Đỗ Minh Khang',     '079187008899', '0987556677', '403', N'Khách lưu trú phòng 403 (1 tuần, thuê xe Exciter)', GETDATE()),
(N'Nguyễn Hoàng Long', '079186001234', '0903889900', '501', N'Khách lưu trú phòng 501 (2 tuần, thuê xe PG-1)', GETDATE()),
(N'Lê Thị Ngọc Ánh',   '079185002345', '0979334455', '502', N'Khách lưu trú phòng 502 (1 tuần, thuê xe Sirius)', GETDATE()),
(N'Công ty TechCorp',  '0312999888',   '0903999888', NULL,  N'Khách vãng lai tổ chức sự kiện',   GETDATE()),
(N'Nguyễn Văn Khách',  '079190001111', '0988112233', NULL,  N'Khách vãng lai thuê xe',           GETDATE());
GO

-- 7.2 Phiếu nhập kho mẫu
INSERT INTO PhieuNhapKho (MaPhieuNhap, NhaCungCapId, NguoiNhapId, NgayNhap, GhiChu) VALUES
('PN-20260825-01', 1, 1, DATEADD(DAY,-6,GETDATE()), N'Nhập hàng định kỳ đầu tuần từ Masan'),
('PN-20260827-01', 1, 2, DATEADD(DAY,-4,GETDATE()), N'Nhập bổ sung nước khoáng và Coca Cola'),
('PN-20260829-01', 2, 1, DATEADD(DAY,-2,GETDATE()), N'Nhập cà phê G7 và socola từ Nestlé'),
('PN-20260830-01', 3, 3, DATEADD(DAY,-1,GETDATE()), N'Nhập rượu vang và đặc sản Đà Lạt');
GO

INSERT INTO ChiTietPhieuNhapKho (PhieuNhapId, MonAnId, DonViNhap, HeSoQuyDoi, SoLuongNhap, DonGiaNhap, SoLo, HanSuDung, GhiChu) VALUES
(1, 1, N'Thùng', 30, 5,  330000, N'MO-2608-A', DATEADD(MONTH,8,GETDATE()),  N'1 thùng = 30 gói'),
(1, 6, N'Thùng', 24, 3,  480000, N'LAY-2608-B', DATEADD(MONTH,6,GETDATE()), N'1 thùng = 24 gói'),
(2, 2, N'Thùng', 24, 10, 144000, N'LAV-2608-C', DATEADD(MONTH,12,GETDATE()),N'1 thùng = 24 chai 500ml'),
(2, 3, N'Thùng', 24, 8,  240000, N'COCA-2608-D',DATEADD(MONTH,9,GETDATE()), N'1 thùng = 24 lon'),
(3, 4, N'Hộp',   1,  20, 22000,  N'G7-2608-E',  DATEADD(MONTH,10,GETDATE()),N'Nhập lẻ theo hộp, không quy đổi'),
(4, 7, N'Thùng', 6,  5,  720000, N'VANG-2608-F',DATEADD(DAY,10,GETDATE()), N'CẢNH BÁO: còn 10 ngày là hết hạn, cần ưu tiên bán trước (FIFO)');
GO

-- 7.3 Đơn hàng ẩm thực & minibar mẫu (Ghi nợ phòng vs Thanh toán trực tiếp, đầy đủ các trạng thái)
INSERT INTO DonHangMonAn (MaDon, KhachHangId, TenKhach, SoPhong, HinhThucThanhToan, TongTien, DaThanhToan, GhiChu, NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan, NgayTao, TrangThai) VALUES
('DH-AU-001', 1,  N'Đoàn Minh Trí',     '101', N'Ghi nợ vào phòng',      35000,  0, N'1 Coca + 1 Lavie',                   1, NULL, NULL,                       DATEADD(HOUR,-3,GETDATE()), N'Đã giao'),
('DH-AU-002', 1,  N'Đoàn Minh Trí',     '101', N'Thanh toán trực tiếp',  45000,  1, N'1 Cà phê G7 (Khách trả tiền ngay)',  1, 1,    DATEADD(HOUR,-2,GETDATE()), DATEADD(HOUR,-2,GETDATE()), N'Đã giao'),
('DH-AU-003', 2,  N'Trịnh Hồng Quân',   '102', N'Ghi nợ vào phòng',      60000,  0, N'2 Mì Modern + 1 Coca',               2, NULL, NULL,                       DATEADD(HOUR,-2,GETDATE()), N'Đã giao'),
('DH-AU-004', 3,  N'Vũ Hoàng Nam',      '103', N'Ghi nợ vào phòng',     220000,  0, N'1 Rượu vang đỏ Đà Lạt',              1, NULL, NULL,                       DATEADD(HOUR,-1,GETDATE()), N'Đang chuẩn bị'),
('DH-AU-005', 4,  N'Hoàng Mai Linh',    '201', N'Thanh toán trực tiếp',  85000,  1, N'1 Hạt điều rang muối',               2, 2,    DATEADD(MINUTE,-45,GETDATE()),DATEADD(MINUTE,-45,GETDATE()), N'Đã giao'),
('DH-AU-006', 5,  N'Lê Tấn Phát',       '202', N'Ghi nợ vào phòng',     110000,  0, N'3 Mì tôm ly + 2 Snack khoai tây',    2, NULL, NULL,                       DATEADD(HOUR,-1,GETDATE()), N'Đã giao'),
('DH-AU-007', 6,  N'Bùi Thanh Tùng',    '203', N'Ghi nợ vào phòng',      70000,  0, N'2 Bia Heineken ướp lạnh',            3, NULL, NULL,                       DATEADD(MINUTE,-30,GETDATE()), N'Chờ xử lý'),
('DH-AU-008', 7,  N'Nguyễn Anh Thư',    '301', N'Ghi nợ vào phòng',     275000,  0, N'1 Rượu vang + 1 Mứt dâu tây',        3, NULL, NULL,                       DATEADD(MINUTE,-30,GETDATE()), N'Đã giao'),
('DH-AU-009', 8,  N'Đặng Hải Yến',      '302', N'Ghi nợ vào phòng',      90000,  0, N'2 Coca + 2 Snack khoai tây',         1, NULL, NULL,                       DATEADD(HOUR,-3,GETDATE()), N'Đã giao'),
('DH-AU-010', 9,  N'Phan Văn Đức',      '303', N'Ghi nợ vào phòng',     105000,  0, N'3 Bia Heineken lon',                 2, NULL, NULL,                       DATEADD(HOUR,-2,GETDATE()), N'Đang chuẩn bị'),
('DH-AU-011', 10, N'Phạm Quốc Bảo',     '401', N'Ghi nợ vào phòng',      80000,  0, N'1 Trà Atiso + 1 Coca',               1, NULL, NULL,                       DATEADD(MINUTE,-15,GETDATE()), N'Chờ xử lý'),
('DH-AU-012', 11, N'Trần Thu Hà',       '402', N'Ghi nợ vào phòng',      50000,  0, N'2 Nước tăng lực Red Bull (Khách hủy)',2, NULL, NULL,                     DATEADD(HOUR,-4,GETDATE()), N'Đã hủy'),
('DH-AU-013', 12, N'Đỗ Minh Khang',     '403', N'Ghi nợ vào phòng',      60000,  0, N'1 Red Bull + 1 Mì + 1 Lavie',        2, NULL, NULL,                       DATEADD(HOUR,-4,GETDATE()), N'Đã giao'),
('DH-AU-014', 13, N'Nguyễn Hoàng Long', '501', N'Ghi nợ vào phòng',     220000,  0, N'1 Rượu vang đỏ Đà Lạt',              1, NULL, NULL,                       DATEADD(HOUR,-5,GETDATE()), N'Đã giao'),
('DH-AU-015', 14, N'Lê Thị Ngọc Ánh',   '502', N'Ghi nợ vào phòng',     110000,  0, N'2 Hũ Mứt dâu tây Đà Lạt',            3, NULL, NULL,                       DATEADD(HOUR,-1,GETDATE()), N'Đang chuẩn bị');
GO

INSERT INTO ChiTietDonHangMonAn (DonHangId, MonAnId, SoLuong, DonGia, GiaVon, GhiChu) VALUES
(1, 3, 1, 20000, 10000, N'Coca Cola lon'),
(1, 2, 1, 15000,  6000, N'Nước khoáng Lavie'),
(2, 4, 1, 45000, 22000, N'Cà phê G7 hộp 20 gói'),
(3, 1, 2, 20000, 11000, N'Mì Modern ly'),
(3, 3, 1, 20000, 10000, N'Coca Cola lon'),
(4, 7, 1, 220000,120000,N'Rượu vang đỏ Đà Lạt'),
(5, 8, 1, 85000, 45000, N'Hạt điều rang muối 200g'),
(6, 1, 3, 20000, 11000, N'Mì Modern ly'),
(6, 6, 2, 25000, 13000, N'Snack khoai tây Lay''s'),
(7, 12,2, 35000, 18000, N'Bia Heineken lon'),
(8, 7, 1, 220000,120000,N'Rượu vang đỏ Đà Lạt'),
(8, 13,1, 55000, 28000, N'Mứt dâu tây 250g'),
(9, 3, 2, 20000, 10000, N'Coca Cola lon'),
(9, 6, 2, 25000, 13000, N'Snack khoai tây Lay''s'),
(10,12,3, 35000, 18000, N'Bia Heineken lon'),
(11, 5, 1, 60000, 30000, N'Trà Atiso túi lọc'),
(11, 3, 1, 20000, 10000, N'Coca Cola lon'),
(12, 9, 2, 25000, 12500, N'Nước tăng lực Red Bull'),
(13, 9, 1, 25000, 12500, N'Nước tăng lực Red Bull'),
(13, 1, 1, 20000, 11000, N'Mì Modern ly'),
(13, 2, 1, 15000,  6000, N'Nước khoáng Lavie'),
(14, 7, 1, 220000,120000,N'Rượu vang đỏ Đà Lạt'),
(15, 13,2, 55000, 28000, N'Mứt dâu tây 250g');
GO

-- 7.4 Đơn đặt sảnh & sự kiện mẫu (Đa dạng trạng thái từ tháng 8 đến hết tháng 9: Hoàn tất, Đang sử dụng, Đã đặt, Đã hủy)
INSERT INTO DonDatSuKien (
    MaDon, KhuVucId, KhachHangId, TenKhach, SoDienThoai, SoPhong,
    ThoiGianBatDau, ThoiGianKetThuc, ThoiGianDonDepPhut, PhuThuRutNganDonDep, GhiChuRutNganDonDep,
    TienCoc, TongTienDuKien, ChiPhiPhatSinh, DaThanhToan, GhiChuHuHai,
    HinhThucThanhToanBanDau, TrangThaiDon,
    ThoiGianHuy, LyDoHuy, NguoiHuyId, SoTienHoanCoc,
    NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan, NgayTao
) VALUES
('SK-2026-001', 1, 7, N'Nguyễn Anh Thư', '0918777666', '301', DATEADD(HOUR,8,DATEADD(DAY,-20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,-20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2250000, 7500000, 0, 1, N'Đại tiệc kỷ niệm gia đình, phục vụ chu đáo', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(HOUR,13,DATEADD(DAY,-20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-23,GETDATE())),
('SK-2026-002', 2, 15, N'Công ty TechCorp', '0903999888', NULL, DATEADD(HOUR,13,DATEADD(DAY,-18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,-18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2000000, 2000000, 0, 1, N'Họp HĐQT quý 3 đã xong', N'Thanh toán toàn bộ', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(HOUR,17,DATEADD(DAY,-18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-21,GETDATE())),
('SK-2026-003', 3, 1, N'Đoàn Minh Trí', '0908112233', '101', DATEADD(HOUR,17,DATEADD(DAY,-15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,-15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 1, N'Tiệc sinh nhật Acoustic hoàng hôn', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 2, 2, DATEADD(HOUR,22,DATEADD(DAY,-15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-18,GETDATE())),
('SK-2026-004', 4, 8, N'Đặng Hải Yến', '0982334411', '302', DATEADD(HOUR,8,DATEADD(DAY,-13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,11,DATEADD(DAY,-13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 270000, 900000, 0, 1, N'Thưởng trà sáng', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(HOUR,11,DATEADD(DAY,-13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-15,GETDATE())),
('SK-2026-005', 1, 15, N'Tập đoàn Vingroup', '0909555666', NULL, DATEADD(HOUR,9,DATEADD(DAY,-10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,-10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 3150000, 10500000, 0, 1, N'Khách hoãn hội thảo sang quý 4', N'Đặt cọc trước', N'Đã hủy - Hoàn cọc', DATEADD(DAY,-13,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), N'Khách thông báo hủy trước 3 ngày, hoàn cọc 100% theo quy định', 1, 3150000, 1, 1, DATEADD(DAY,-13,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), DATEADD(DAY,-16,GETDATE())),
('SK-2026-006', 2, 6, N'Bùi Thanh Tùng', '0938445566', '203', DATEADD(HOUR,8,DATEADD(DAY,-8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,-8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), 30, 250000, N'Dọn dẹp cấp tốc 30 phút phục vụ đoàn đại biểu', 750000, 2500000, 0, 1, N'Hội thảo khoa học công nghệ', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 2, 2, DATEADD(HOUR,13,DATEADD(DAY,-8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-10,GETDATE())),
('SK-2026-007', 4, 15, N'CLB Thư Pháp Trà Đạo', '0914223355', NULL, DATEADD(HOUR,14,DATEADD(DAY,-6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,-6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1200000, 0, 1, N'Khách báo hủy đột xuất trước giờ bắt đầu', N'Đặt cọc trước', N'Đã hủy - Không hoàn cọc', DATEADD(HOUR,11,DATEADD(DAY,-6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), N'Khách báo hủy đột xuất trước giờ bắt đầu 3 tiếng, không hoàn cọc theo quy chế', 2, 0, 2, 2, DATEADD(HOUR,11,DATEADD(DAY,-6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-8,GETDATE())),
('SK-2026-008', 1, 15, N'Tập đoàn Hoa Sen Tri Ân', '0907123987', NULL, DATEADD(HOUR,17,DATEADD(DAY,-4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,23,DATEADD(DAY,-4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2700000, 9000000, 500000, 1, N'Khách làm vỡ 2 ly rượu pha lê cao cấp, đã bồi thường 500.000đ', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(HOUR,23,DATEADD(DAY,-4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-6,GETDATE())),
('SK-2026-009', 3, 15, N'Live Chill Rooftop', '0916334455', NULL, DATEADD(HOUR,16,DATEADD(DAY,-3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,-3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 1, N'Live Chill Rooftop chào mừng lễ', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(HOUR,22,DATEADD(DAY,-3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-5,GETDATE())),
('SK-2026-010', 2, 5, N'Lê Tấn Phát', '0969888999', '202', DATEADD(HOUR,8,DATEADD(DAY,-2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,12,DATEADD(DAY,-2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2000000, 2000000, 0, 1, N'Họp chiến lược kinh doanh tháng 9', N'Thanh toán toàn bộ', N'Hoàn tất', NULL, NULL, NULL, NULL, 2, 2, DATEADD(HOUR,12,DATEADD(DAY,-2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-4,GETDATE())),
('SK-2026-011', 4, 10, N'Phạm Quốc Bảo', '0933444555', '401', DATEADD(HOUR,8,DATEADD(DAY,-1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,11,DATEADD(DAY,-1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 270000, 900000, 0, 1, N'Giao lưu trà đạo nghệ thuật', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(HOUR,11,DATEADD(DAY,-1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-3,GETDATE())),
('SK-2026-012', 1, 15, N'Ngân hàng Vietcombank', '0912888999', NULL, DATEADD(HOUR,14,DATEADD(DAY,-1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,-1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 1, N'Tiệc tri ân đối tác đầu tháng', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(HOUR,18,DATEADD(DAY,-1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY,-3,GETDATE())),
('SK-2026-013', 4, 2, N'Trịnh Hồng Quân', '0909123456', '102', DATEADD(HOUR,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), DATEADD(HOUR,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), NULL, 0, NULL, 270000, 900000, 0, 1, N'Thưởng trà sáng và đàm đạo', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(HOUR,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), DATEADD(DAY,-1,GETDATE())),
('SK-2026-014', 1, 15, N'FPT Software Đà Lạt', '0908112345', NULL, DATEADD(HOUR,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), DATEADD(HOUR,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), NULL, 0, NULL, 2250000, 7500000, 0, 0, N'Hội thảo AI & Điện toán đám mây - Đang diễn ra', N'Đặt cọc trước', N'Đang sử dụng', NULL, NULL, NULL, NULL, 1, NULL, NULL, DATEADD(DAY,-2,GETDATE())),
('SK-2026-015', 2, 3, N'Vũ Hoàng Nam', '0912334455', '103', DATEADD(HOUR,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), DATEADD(HOUR,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), NULL, 0, NULL, 600000, 1500000, 0, 0, N'Họp ban cố vấn tài chính (13:00 - 15:00)', N'Đặt cọc trước', N'Đang sử dụng', NULL, NULL, NULL, NULL, 2, NULL, NULL, DATEADD(DAY,-1,GETDATE())),
('SK-2026-016', 3, 15, N'CLB Doanh Nhân Trẻ', '0902334455', NULL, DATEADD(HOUR,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), DATEADD(HOUR,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Tiệc cocktail Acoustic ngắm hoàng hôn tối nay', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-017', 4, 11, N'Trần Thu Hà', '0919223344', '402', DATEADD(HOUR,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), DATEADD(HOUR,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), NULL, 0, NULL, 900000, 900000, 0, 0, N'Đàm đạo trà thất tối nay (Khách trả trước 100%)', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 2, NULL, NULL, GETDATE()),
('SK-2026-018', 4, 12, N'Đỗ Minh Khang', '0987556677', '403', DATEADD(HOUR,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), DATEADD(HOUR,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME)), NULL, 0, NULL, 300000, 900000, 0, 1, N'Khách hủy sát giờ trưa nay', N'Đặt cọc trước', N'Đã hủy - Không hoàn cọc', DATEADD(HOUR,-1,GETDATE()), N'Khách báo bận đột xuất trước giờ họp 1 tiếng, không hoàn cọc theo quy chế', 2, 0, 2, 2, DATEADD(HOUR,-1,GETDATE()), DATEADD(DAY,-1,GETDATE())),
('SK-2026-019', 1, 15, N'Tập đoàn Vingroup', '0909555666', NULL, DATEADD(HOUR,8,DATEADD(DAY,1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4050000, 13500000, 0, 0, N'Hội thảo khách hàng toàn quốc', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-020', 2, 5, N'Lê Tấn Phát', '0969888999', '202', DATEADD(HOUR,13,DATEADD(DAY,1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Ký kết hợp đồng đối tác', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-021', 3, 15, N'Ngân hàng Vietcombank', '0912888999', NULL, DATEADD(HOUR,16,DATEADD(DAY,1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Gala Dinner tri ân khách VIP', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-022', 4, 10, N'Phạm Quốc Bảo', '0933444555', '401', DATEADD(HOUR,14,DATEADD(DAY,1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,1,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 900000, 900000, 0, 0, N'Giao lưu trà đạo nghệ thuật', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-023', 1, 15, N'Tiệc cưới Hoàng Gia', '0988777111', NULL, DATEADD(HOUR,9,DATEADD(DAY,2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 3150000, 10500000, 0, 0, N'Tiệc cưới trọn gói cao cấp', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-024', 2, 6, N'Bùi Thanh Tùng', '0938445566', '203', DATEADD(HOUR,8,DATEADD(DAY,2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Họp chi nhánh công ty', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-025', 3, 15, N'CLB Doanh Nhân Trẻ', '0902334455', NULL, DATEADD(HOUR,17,DATEADD(DAY,2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,23,DATEADD(DAY,2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Cocktail Networking Night', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-026', 4, 2, N'Trịnh Hồng Quân', '0909123456', '102', DATEADD(HOUR,9,DATEADD(DAY,2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,15,DATEADD(DAY,2,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 540000, 1800000, 0, 0, N'Họp mặt trà đạo', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-027', 1, 15, N'Tiệc cưới Trọng Tấn', '0988777222', NULL, DATEADD(HOUR,10,DATEADD(DAY,3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2700000, 9000000, 0, 0, N'Tiệc cưới phong cách Đà Lạt', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-028', 2, 3, N'Vũ Hoàng Nam', '0912334455', '103', DATEADD(HOUR,14,DATEADD(DAY,3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,19,DATEADD(DAY,3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), 30, 300000, N'Phụ thu dọn dẹp cấp tốc 30 phút phục vụ đoàn đại biểu', 750000, 2500000, 0, 0, N'Hội thảo nhóm chuyên gia', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-029', 3, 15, N'Sunset Acoustic Night', '0903112244', NULL, DATEADD(HOUR,16,DATEADD(DAY,3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Đêm nhạc acoustic ngắm hoàng hôn', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-030', 4, 15, N'CLB Thư Pháp Trà Đạo', '0914223355', NULL, DATEADD(HOUR,8,DATEADD(DAY,3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,3,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1500000, 1500000, 0, 0, N'Giao lưu thư pháp cuối tuần', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-031', 1, 15, N'FPT Software Đà Lạt', '0908112345', NULL, DATEADD(HOUR,8,DATEADD(DAY,4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4500000, 15000000, 0, 0, N'Tech Summit & Triển lãm AI', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-032', 2, 4, N'Hoàng Mai Linh', '0978112244', '201', DATEADD(HOUR,8,DATEADD(DAY,4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,12,DATEADD(DAY,4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 600000, 2000000, 0, 0, N'Phỏng vấn nhân sự dự án', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-033', 3, 13, N'Nguyễn Hoàng Long', '0903889900', '501', DATEADD(HOUR,17,DATEADD(DAY,4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Tiệc mừng thọ gia đình', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-034', 4, 12, N'Đỗ Minh Khang', '0987556677', '403', DATEADD(HOUR,13,DATEADD(DAY,4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,4,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1500000, 0, 0, N'Họp mặt nhóm bạn học', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-035', 1, 15, N'Gala Tri Ân Dược Hậu Giang', '0913998877', NULL, DATEADD(HOUR,16,DATEADD(DAY,5,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,5,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2700000, 9000000, 0, 1, N'Khách chuyển địa điểm tổ chức ra Hà Nội', N'Đặt cọc trước', N'Đã hủy - Hoàn cọc', GETDATE(), N'Khách báo hủy trước 5 ngày, hoàn 100% tiền cọc', 1, 2700000, 1, 1, GETDATE(), GETDATE()),
('SK-2026-036', 2, 15, N'Hội thảo Y Khoa Lâm Đồng', '0914556677', NULL, DATEADD(HOUR,8,DATEADD(DAY,5,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,5,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1350000, 4500000, 0, 0, N'Hội thảo cập nhật kiến thức y khoa', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-037', 3, 15, N'Rooftop DJ Night', '0908778899', NULL, DATEADD(HOUR,18,DATEADD(DAY,5,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,23,DATEADD(DAY,5,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Đêm nhạc EDM ngoài trời', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-038', 4, 14, N'Lê Thị Ngọc Ánh', '0979334455', '502', DATEADD(HOUR,9,DATEADD(DAY,5,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,14,DATEADD(DAY,5,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1500000, 1500000, 0, 0, N'Tiệc trà sinh nhật thân mật', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-039', 1, 15, N'Diễn đàn Đổi mới Sáng tạo', '0903221199', NULL, DATEADD(HOUR,8,DATEADD(DAY,6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 3600000, 12000000, 0, 0, N'Hội nghị xúc tiến khởi nghiệp xanh', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-040', 2, 9, N'Phan Văn Đức', '0945667788', '303', DATEADD(HOUR,13,DATEADD(DAY,6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Họp bàn dự án kinh doanh', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-041', 3, 15, N'Live Chill Rooftop', '0916334455', NULL, DATEADD(HOUR,16,DATEADD(DAY,6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Đêm ngắm sao và thưởng thức đồ uống', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-042', 4, 11, N'Trần Thu Hà', '0919223344', '402', DATEADD(HOUR,14,DATEADD(DAY,6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,6,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 360000, 1200000, 0, 0, N'Trà chiều cùng gia đình', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-043', 1, 15, N'Hội nghị Xúc tiến Du lịch 2026', '0914667788', NULL, DATEADD(HOUR,8,DATEADD(DAY,7,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,7,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4050000, 13500000, 0, 0, N'Hội nghị du lịch các tỉnh Tây Nguyên', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-044', 2, 15, N'Hiệp hội Du lịch Lâm Đồng', '0914667788', NULL, DATEADD(HOUR,9,DATEADD(DAY,7,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,7,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1050000, 3500000, 0, 0, N'Tập huấn tiêu chuẩn nghiệp vụ lễ tân', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-045', 3, 15, N'Tiệc Sinh Nhật VIP', '0908990011', NULL, DATEADD(HOUR,17,DATEADD(DAY,7,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,7,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Tiệc sinh nhật riêng tư trên sân thượng', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-046', 4, 2, N'Trịnh Hồng Quân', '0909123456', '102', DATEADD(HOUR,8,DATEADD(DAY,7,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,12,DATEADD(DAY,7,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1200000, 1200000, 0, 0, N'Thưởng trà đàm đạo buổi sáng', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-047', 1, 15, N'Xây dựng Hòa Bình Gala', '0907123456', NULL, DATEADD(HOUR,17,DATEADD(DAY,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), 30, 500000, N'Dọn dẹp nhanh 30 phút để set up đại tiệc', 2250000, 7500000, 0, 0, N'Đại tiệc tổng kết công trình', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-048', 2, 15, N'Đại hội Cổ đông BĐS', '0918223344', NULL, DATEADD(HOUR,8,DATEADD(DAY,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Đại hội cổ đông thường niên', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-049', 3, 15, N'Barbecue Sunset Chill', '0903445566', NULL, DATEADD(HOUR,16,DATEADD(DAY,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Tiệc nướng BBQ ngắm thung lũng', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-050', 4, 15, N'Trà thiền ngắm sương', '0915667788', NULL, DATEADD(HOUR,13,DATEADD(DAY,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,8,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1500000, 0, 0, N'Buổi trà thiền thư giãn cuối tuần', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-051', 1, 15, N'Tiệc cưới Thanh Bình & Mai Hương', '0988112233', NULL, DATEADD(HOUR,9,DATEADD(DAY,9,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,9,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 3150000, 10500000, 0, 0, N'Tiệc cưới sang trọng', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-052', 2, 15, N'Workshop Nhiếp ảnh', '0909334455', NULL, DATEADD(HOUR,13,DATEADD(DAY,9,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,9,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Chia sẻ kỹ thuật chụp ảnh phong cảnh', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-053', 3, 15, N'Weekend Sky Party', '0912445566', NULL, DATEADD(HOUR,17,DATEADD(DAY,9,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,23,DATEADD(DAY,9,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Tiệc cocktail đón hoàng hôn', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-054', 4, 1, N'Đoàn Minh Trí', '0908112233', '101', DATEADD(HOUR,9,DATEADD(DAY,9,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,14,DATEADD(DAY,9,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1500000, 0, 0, N'Trà đạo gặp gỡ bạn bè', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-055', 1, 15, N'Hội thảo Nông sản Sạch', '0908332211', NULL, DATEADD(HOUR,10,DATEADD(DAY,10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2700000, 9000000, 0, 0, N'Xúc tiến tiêu thụ rau hoa Đà Lạt', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-056', 2, 7, N'Nguyễn Anh Thư', '0918777666', '301', DATEADD(HOUR,8,DATEADD(DAY,10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,12,DATEADD(DAY,10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 600000, 2000000, 0, 0, N'Họp ban cố vấn', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-057', 3, 15, N'Sky Club Members Night', '0903556677', NULL, DATEADD(HOUR,16,DATEADD(DAY,10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Đêm giao lưu thành viên', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-058', 4, 8, N'Đặng Hải Yến', '0982334411', '302', DATEADD(HOUR,14,DATEADD(DAY,10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,19,DATEADD(DAY,10,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1500000, 0, 0, N'Trà chiều phong vị cao nguyên', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-059', 1, 15, N'Tập đoàn Hoa Sen Tri Ân', '0907123987', NULL, DATEADD(HOUR,8,DATEADD(DAY,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4050000, 13500000, 0, 0, N'Đại tiệc tri ân hệ thống phân phối', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-060', 2, 15, N'Hội thảo Pháp lý Doanh nghiệp', '0914889900', NULL, DATEADD(HOUR,9,DATEADD(DAY,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,15,DATEADD(DAY,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 900000, 3000000, 0, 0, N'Cập nhật luật doanh nghiệp mới', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-061', 3, 6, N'Bùi Thanh Tùng', '0938445566', '203', DATEADD(HOUR,17,DATEADD(DAY,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Tiệc kỷ niệm ngày cưới gia đình', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-062', 4, 15, N'Trà thất bạn hữu', '0917223344', NULL, DATEADD(HOUR,8,DATEADD(DAY,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,11,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1500000, 0, 0, N'Giao lưu trà đạo nghệ nhân', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-063', 1, 15, N'Viettel Telecom Hội nghị', '0986112233', NULL, DATEADD(HOUR,8,DATEADD(DAY,12,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,12,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4500000, 15000000, 0, 0, N'Hội nghị triển khai kế hoạch kinh doanh', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-064', 2, 12, N'Đỗ Minh Khang', '0987556677', '403', DATEADD(HOUR,13,DATEADD(DAY,12,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,12,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Họp dự án phần mềm', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-065', 3, 15, N'Sunset Chill Out', '0908445566', NULL, DATEADD(HOUR,18,DATEADD(DAY,12,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,23,DATEADD(DAY,12,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 1, N'Khách thay đổi kế hoạch gia đình', N'Đặt cọc trước', N'Đã hủy - Hoàn cọc', GETDATE(), N'Khách báo hủy trước 1 tuần, hoàn 100% tiền cọc', 1, 1800000, 1, 1, GETDATE(), GETDATE()),
('SK-2026-066', 4, 15, N'Trà chiều thư giãn', '0913556677', NULL, DATEADD(HOUR,14,DATEADD(DAY,12,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,12,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 360000, 1200000, 0, 0, N'Trà chiều ấm cúng', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-067', 1, 15, N'Công ty TechCorp Demo AI', '0903999888', NULL, DATEADD(HOUR,8,DATEADD(DAY,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4050000, 13500000, 0, 0, N'Hội thảo ra mắt nền tảng AI Khách sạn', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-068', 2, 5, N'Lê Tấn Phát', '0969888999', '202', DATEADD(HOUR,13,DATEADD(DAY,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Họp chiến lược kinh doanh', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-069', 3, 15, N'Sunset Music Club', '0908119922', NULL, DATEADD(HOUR,16,DATEADD(DAY,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Đêm nhạc acoustic cuối tuần', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-070', 4, 2, N'Trịnh Hồng Quân', '0909123456', '102', DATEADD(HOUR,8,DATEADD(DAY,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,12,DATEADD(DAY,13,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1200000, 1200000, 0, 0, N'Đàm đạo trà sáng bạn bè', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-071', 1, 15, N'Tiệc cưới Minh Quang & Hà My', '0988223344', NULL, DATEADD(HOUR,9,DATEADD(DAY,14,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,14,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 3150000, 10500000, 0, 0, N'Tiệc cưới cao cấp', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-072', 2, 3, N'Vũ Hoàng Nam', '0912334455', '103', DATEADD(HOUR,8,DATEADD(DAY,14,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,14,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Họp nhóm tư vấn tài chính', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-073', 3, 15, N'DJ Sunset Party Night', '0909445566', NULL, DATEADD(HOUR,17,DATEADD(DAY,14,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,23,DATEADD(DAY,14,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Tiệc DJ ngắm cảnh toàn thành phố', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-074', 4, 4, N'Hoàng Mai Linh', '0978112244', '201', DATEADD(HOUR,13,DATEADD(DAY,14,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,14,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 360000, 1200000, 0, 0, N'Trà chiều gia đình', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-075', 1, 15, N'Hội nghị Công nghệ VinFast', '0915998877', NULL, DATEADD(HOUR,8,DATEADD(DAY,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4500000, 15000000, 0, 0, N'Hội nghị chiến lược phân phối xe điện', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-076', 2, 6, N'Bùi Thanh Tùng', '0938445566', '203', DATEADD(HOUR,14,DATEADD(DAY,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,19,DATEADD(DAY,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Họp ban quản lý dự án', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-077', 3, 15, N'Acoustic Night & Wine', '0903332211', NULL, DATEADD(HOUR,16,DATEADD(DAY,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Đêm nhạc acoustic thưởng rượu vang', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-078', 4, 7, N'Nguyễn Anh Thư', '0918777666', '301', DATEADD(HOUR,8,DATEADD(DAY,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,15,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), 30, 200000, N'Yêu cầu dọn sảnh sớm 30 phút để đón khách', 450000, 1500000, 0, 0, N'Thưởng trà tĩnh tâm', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-079', 1, 15, N'Diễn đàn Bất động sản 2026', '0914112233', NULL, DATEADD(HOUR,9,DATEADD(DAY,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 3600000, 12000000, 0, 0, N'Tọa đàm bất động sản nghỉ dưỡng', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-080', 2, 8, N'Đặng Hải Yến', '0982334411', '302', DATEADD(HOUR,8,DATEADD(DAY,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,12,DATEADD(DAY,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 600000, 2000000, 0, 0, N'Hội thảo kỹ năng giao tiếp', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-081', 3, 9, N'Phan Văn Đức', '0945667788', '303', DATEADD(HOUR,17,DATEADD(DAY,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Tiệc sinh nhật ấm cúng trên cao', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-082', 4, 10, N'Phạm Quốc Bảo', '0933444555', '401', DATEADD(HOUR,14,DATEADD(DAY,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,16,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 360000, 1200000, 0, 0, N'Trà chiều kết nối đối tác', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-083', 1, 15, N'Gala Tổng kết Quý 3 Vinatex', '0908556677', NULL, DATEADD(HOUR,16,DATEADD(DAY,17,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,17,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2700000, 9000000, 0, 0, N'Gala Dinner tổng kết hoạt động quý 3', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-084', 2, 15, N'Huấn luyện Quản lý Cấp cao', '0916778899', NULL, DATEADD(HOUR,8,DATEADD(DAY,17,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,17,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1350000, 4500000, 0, 0, N'Khóa đào tạo lãnh đạo chuyển dịch số', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-085', 3, 15, N'Chill Out Rooftop Sunset', '0903882233', NULL, DATEADD(HOUR,18,DATEADD(DAY,17,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,23,DATEADD(DAY,17,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Đêm ngắm hoàng hôn và nghe nhạc', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-086', 4, 11, N'Trần Thu Hà', '0919223344', '402', DATEADD(HOUR,9,DATEADD(DAY,17,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,14,DATEADD(DAY,17,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1500000, 1500000, 0, 0, N'Thưởng trà sương sớm', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-087', 1, 15, N'Hội nghị Khách hàng Petrolimex', '0909887766', NULL, DATEADD(HOUR,8,DATEADD(DAY,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 3600000, 12000000, 0, 0, N'Hội nghị đối tác phân phối dầu nhờn', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-088', 2, 12, N'Đỗ Minh Khang', '0987556677', '403', DATEADD(HOUR,13,DATEADD(DAY,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Họp đội ngũ lập trình viên', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-089', 3, 13, N'Nguyễn Hoàng Long', '0903889900', '501', DATEADD(HOUR,16,DATEADD(DAY,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Tiệc tri ân nhân viên xuất sắc', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-090', 4, 14, N'Lê Thị Ngọc Ánh', '0979334455', '502', DATEADD(HOUR,14,DATEADD(DAY,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,18,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 360000, 1200000, 0, 0, N'Trà chiều cùng người thân', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-091', 1, 15, N'Hội thảo Phát triển Bền vững', '0914228899', NULL, DATEADD(HOUR,8,DATEADD(DAY,19,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,19,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4050000, 13500000, 0, 0, N'Hội thảo công nghiệp xanh Tây Nguyên', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-092', 2, 15, N'Họp Ban Giám Đốc BIDV', '0918334455', NULL, DATEADD(HOUR,9,DATEADD(DAY,19,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,19,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1050000, 3500000, 0, 0, N'Họp giao ban chi nhánh Lâm Đồng', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-093', 3, 1, N'Đoàn Minh Trí', '0908112233', '101', DATEADD(HOUR,17,DATEADD(DAY,19,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,19,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Tiệc mừng công đoàn công tác', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-094', 4, 15, N'Trà đàm nghệ nhân xứ trà', '0915446688', NULL, DATEADD(HOUR,8,DATEADD(DAY,19,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,12,DATEADD(DAY,19,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 360000, 1200000, 0, 0, N'Gặp gỡ nghệ nhân chế tác trà Oolong', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-095', 1, 15, N'Đại tiệc Tri ân Vietjet Air', '0908123789', NULL, DATEADD(HOUR,17,DATEADD(DAY,20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2250000, 7500000, 0, 0, N'Dạ tiệc tri ân phòng vé khu vực', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-096', 2, 15, N'Hội thảo Chuyển đổi số', '0916889900', NULL, DATEADD(HOUR,8,DATEADD(DAY,20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Ứng dụng ERP trong quản trị khách sạn', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-097', 3, 15, N'Sunset Cocktail VIP', '0903114477', NULL, DATEADD(HOUR,16,DATEADD(DAY,20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Tiệc cocktail đón hoàng hôn cuối tuần', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-098', 4, 15, N'Giao lưu trà Việt', '0917335577', NULL, DATEADD(HOUR,13,DATEADD(DAY,20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,20,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1500000, 0, 0, N'Buổi nếm thử trà shan tuyết', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-099', 1, 15, N'Tiệc cưới Văn Hùng & Thu Thảo', '0988665544', NULL, DATEADD(HOUR,9,DATEADD(DAY,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 3150000, 10500000, 0, 0, N'Tiệc cưới phong cách Châu Âu', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-100', 2, 15, N'Hội thảo Đầu tư Khởi nghiệp', '0909558833', NULL, DATEADD(HOUR,13,DATEADD(DAY,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,18,DATEADD(DAY,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Gặp gỡ các quỹ đầu tư mạo hiểm', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-101', 3, 15, N'Weekend Acoustic Sky Club', '0914992211', NULL, DATEADD(HOUR,17,DATEADD(DAY,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,23,DATEADD(DAY,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Đêm nhạc acoustic lãng mạn', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-102', 4, 15, N'Trà chiều thiền định', '0918113355', NULL, DATEADD(HOUR,9,DATEADD(DAY,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,14,DATEADD(DAY,21,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1500000, 1500000, 0, 0, N'Trà thiền ngắm hoa cẩm tú cầu', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-103', 1, 15, N'Ngày hội Du lịch Đà Lạt', '0908771122', NULL, DATEADD(HOUR,10,DATEADD(DAY,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,16,DATEADD(DAY,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2700000, 9000000, 0, 0, N'Họp báo xúc tiến mùa lễ hội hoa', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-104', 2, 15, N'Họp Hội đồng Khoa học', '0912447788', NULL, DATEADD(HOUR,8,DATEADD(DAY,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,12,DATEADD(DAY,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 600000, 2000000, 0, 0, N'Nghiệm thu đề tài cấp tỉnh', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-105', 3, 15, N'Sky Lounge Networking', '0903668899', NULL, DATEADD(HOUR,16,DATEADD(DAY,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2160000, 7200000, 0, 0, N'Giao lưu cộng đồng doanh nhân trẻ', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-106', 4, 15, N'Buổi đàm đạo thư pháp', '0915224466', NULL, DATEADD(HOUR,14,DATEADD(DAY,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,19,DATEADD(DAY,22,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1500000, 0, 0, N'Trình diễn nghệ thuật viết chữ', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-107', 1, 15, N'Đại hội Thường niên Agribank', '0909337711', NULL, DATEADD(HOUR,8,DATEADD(DAY,23,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,17,DATEADD(DAY,23,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 4050000, 13500000, 0, 0, N'Hội nghị sơ kết thi đua khu vực', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-108', 2, 15, N'Tập huấn An toàn thông tin', '0916113355', NULL, DATEADD(HOUR,9,DATEADD(DAY,23,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,15,DATEADD(DAY,23,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 900000, 3000000, 0, 0, N'Tập huấn an ninh mạng cơ quan nhà nước', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-109', 3, 15, N'Sunset Live Band Night', '0908225588', NULL, DATEADD(HOUR,17,DATEADD(DAY,23,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,23,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Đêm nhạc pop ballad nhẹ nhàng', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-110', 4, 15, N'Trà sớm thung lũng sương', '0917882244', NULL, DATEADD(HOUR,8,DATEADD(DAY,23,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,23,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 450000, 1500000, 0, 0, N'Trà sớm ngắm đồi thông reo', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-111', 1, 15, N'Gala Tổng Kết Tháng 9 & Chào Tháng 10', '0908889911', NULL, DATEADD(HOUR,16,DATEADD(DAY,24,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,24,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 2700000, 9000000, 0, 0, N'Đại tiệc liên hoan cuối tháng', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-112', 2, 15, N'Họp Tổng Kết Kinh Doanh Tháng 9', '0912993344', NULL, DATEADD(HOUR,8,DATEADD(DAY,24,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,24,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 750000, 2500000, 0, 0, N'Tổng kết chỉ tiêu quý 3', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-113', 3, 15, N'Sky Party Chốt Sổ Tháng 9', '0903558822', NULL, DATEADD(HOUR,17,DATEADD(DAY,24,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,22,DATEADD(DAY,24,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1800000, 6000000, 0, 0, N'Tiệc chúc mừng vượt KPI tháng 9', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('SK-2026-114', 4, 15, N'Trà chiều tri ân tháng 9', '0916228844', NULL, DATEADD(HOUR,9,DATEADD(DAY,24,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(HOUR,13,DATEADD(DAY,24,CAST(CAST(GETDATE() AS DATE) AS DATETIME))), NULL, 0, NULL, 1200000, 1200000, 0, 0, N'Gặp mặt thân mật cuối tháng', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE());
GO
-- 7.5 Đơn thuê xe mẫu (Thuê xe từ tháng 8 đến nay, đầy đủ các trạng thái: Hoàn tất, Đang thuê, Quá hạn, Đã đặt, Đã hủy)
INSERT INTO DonThueXe (
    MaDon, XeId, KhachHangId, TenKhach, CCCD, SoDienThoai, SoPhong,
    NgayThue, NgayTraDuKien, NgayTraThucTe,
    ThoiGianKiemTraXePhut, PhuThuRutNganBuffer, GhiChuRutNganBuffer,
    TienCoc, TienThue, ChiPhiPhatSinh, TongTienThanhToan, DaThanhToan,
    GhiChuKhiNhanXe, GhiChuHuHai,
    HinhThucThanhToanBanDau, TrangThaiDon,
    ThoiGianHuy, LyDoHuy, NguoiHuyId, SoTienHoanCoc,
    NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan, NgayTao
) VALUES
('TX-2026-001', 5, 16, N'Nguyễn Văn Khách', '079190001111', '0988112233', NULL, DATEADD(DAY,-15,GETDATE()), DATEADD(DAY,-12,GETDATE()), DATEADD(DAY,-12,GETDATE()), NULL, 0, NULL, 100000, 300000, 0, 300000, 1, N'Khách thuê Wave 3 ngày ngắm đồi thông', N'Xe sạch đẹp, trả đúng hạn', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(DAY,-12,GETDATE()), DATEADD(DAY,-15,GETDATE())),
('TX-2026-002', 17, 16, N'Lê Hoàng Long', '079191002222', '0912445566', NULL, DATEADD(DAY,-12,GETDATE()), DATEADD(DAY,-9,GETDATE()), DATEADD(DAY,-9,GETDATE()), NULL, 0, NULL, 200000, 540000, 0, 540000, 1, N'Khách thuê Raider đi phượt', N'Xe hoạt động tốt', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 2, 2, DATEADD(DAY,-9,GETDATE()), DATEADD(DAY,-12,GETDATE())),
('TX-2026-003', 18, 16, N'Phạm Thu Thảo', '079192003333', '0938119900', NULL, DATEADD(DAY,-9,GETDATE()), DATEADD(DAY,-6,GETDATE()), DATEADD(DAY,-6,GETDATE()), NULL, 0, NULL, 300000, 600000, 0, 600000, 1, N'Khách thuê Vario 3 ngày', N'Xe hoàn hảo, đã rửa sạch', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(DAY,-6,GETDATE()), DATEADD(DAY,-9,GETDATE())),
('TX-2026-004', 19, 16, N'Ngô Gia Huy', '079193004444', '0945223344', NULL, DATEADD(DAY,-7,GETDATE()), DATEADD(DAY,-4,GETDATE()), DATEADD(DAY,-4,GETDATE()), NULL, 0, NULL, 200000, 330000, 0, 330000, 1, N'Khách thuê Wave RSX đi công tác', N'Đổ đầy bình xăng khi trả', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 2, 2, DATEADD(DAY,-4,GETDATE()), DATEADD(DAY,-7,GETDATE())),
('TX-2026-005', 20, 16, N'Đặng Quốc Tuấn', '079194005555', '0979887766', NULL, DATEADD(DAY,-5,GETDATE()), DATEADD(DAY,-2,GETDATE()), DATEADD(DAY,-2,GETDATE()), NULL, 0, NULL, 300000, 480000, 0, 480000, 1, N'Khách thuê Exciter GP', N'Xe tốt, bảo dưỡng định kỳ', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 1, 1, DATEADD(DAY,-2,GETDATE()), DATEADD(DAY,-5,GETDATE())),
('TX-2026-006', 5, 16, N'Trần Minh Trí', '079195006666', '0908776655', NULL, DATEADD(DAY,-3,GETDATE()), DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,-1,GETDATE()), NULL, 0, NULL, 100000, 200000, 0, 200000, 1, N'Khách thuê Wave 2 ngày', N'Đã hoàn tất thanh toán', N'Đặt cọc trước', N'Hoàn tất', NULL, NULL, NULL, NULL, 2, 2, DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,-3,GETDATE())),
('TX-2026-007', 17, 16, N'Võ Hoàng Khang', '079196007777', '0918332211', NULL, DATEADD(DAY,-2,GETDATE()), DATEADD(DAY,1,GETDATE()), NULL, NULL, 0, NULL, 200000, 540000, 0, 540000, 1, N'Khách đổi lịch bay, hủy thuê xe trước ngày nhận', N'Đã hoàn lại cọc cho khách', N'Đặt cọc trước', N'Đã hủy - Hoàn cọc', DATEADD(DAY,-2,GETDATE()), N'Khách báo hủy trước ngày nhận xe, hoàn 100% cọc', 1, 200000, 1, 1, DATEADD(DAY,-2,GETDATE()), DATEADD(DAY,-2,GETDATE())),
('TX-2026-008', 1, 1, N'Đoàn Minh Trí', '079198001234', '0908112233', '101', DATEADD(HOUR,-2,GETDATE()), DATEADD(DAY,2,GETDATE()), NULL, NULL, 0, NULL, 300000, 600000, 0, 600000, 0, N'Khách thuê SH kèm 2 nón bảo hiểm cao cấp', N'Đang thuê 2 ngày', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 1, NULL, NULL, DATEADD(HOUR,-2,GETDATE())),
('TX-2026-009', 8, 4, N'Hoàng Mai Linh', '079194007788', '0978112244', '201', DATEADD(HOUR,-3,GETDATE()), DATEADD(DAY,2,GETDATE()), NULL, NULL, 0, NULL, 120000, 240000, 0, 240000, 0, N'Khách thuê Future leo dốc đồi chè', N'Đang thuê 2 ngày', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 2, NULL, NULL, DATEADD(HOUR,-3,GETDATE())),
('TX-2026-010', 6, 2, N'Trịnh Hồng Quân', '079192003344', '0909123456', '102', DATEADD(HOUR,-4,GETDATE()), DATEADD(DAY,3,GETDATE()), NULL, NULL, 0, NULL, 240000, 480000, 0, 480000, 0, N'Khách thuê Lead đi chợ mua đặc sản', N'Đang thuê 3 ngày', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 2, NULL, NULL, DATEADD(HOUR,-4,GETDATE())),
('TX-2026-011', 10, 8, N'Đặng Hải Yến', '079197003355', '0982334411', '302', DATEADD(HOUR,-5,GETDATE()), DATEADD(DAY,3,GETDATE()), NULL, NULL, 0, NULL, 420000, 840000, 0, 840000, 0, N'Khách thuê Vespa chụp ảnh check-in', N'Đang thuê 3 ngày', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 1, NULL, NULL, DATEADD(HOUR,-5,GETDATE())),
('TX-2026-012', 13, 11, N'Trần Thu Hà', '079189006677', '0919223344', '402', DATEADD(HOUR,-2,GETDATE()), DATEADD(DAY,3,GETDATE()), NULL, NULL, 0, NULL, 330000, 660000, 0, 660000, 0, N'Khách thuê Liberty dạo phố cổ', N'Đang thuê 3 ngày', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 2, NULL, NULL, DATEADD(HOUR,-2,GETDATE())),
('TX-2026-013', 2, 5, N'Lê Tấn Phát', '079201009876', '0969888999', '202', DATEADD(HOUR,-3,GETDATE()), DATEADD(DAY,7,GETDATE()), NULL, NULL, 0, NULL, 378000, 1260000, 0, 1260000, 0, N'Khách lấy 2 nón bảo hiểm', N'Đang thuê 1 tuần', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 2, NULL, NULL, DATEADD(HOUR,-3,GETDATE())),
('TX-2026-014', 4, 12, N'Đỗ Minh Khang', '079187008899', '0987556677', '403', DATEADD(HOUR,-4,GETDATE()), DATEADD(DAY,7,GETDATE()), NULL, NULL, 0, NULL, 357000, 1190000, 0, 1190000, 0, N'Khách thuê Exciter đi phượt đèo', N'Đang thuê 1 tuần', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 2, NULL, NULL, DATEADD(HOUR,-4,GETDATE())),
('TX-2026-015', 15, 14, N'Lê Thị Ngọc Ánh', '079185002345', '0979334455', '502', DATEADD(HOUR,-2,GETDATE()), DATEADD(DAY,7,GETDATE()), NULL, NULL, 0, NULL, 210000, 700000, 0, 700000, 0, N'Khách thuê Sirius ngắm hoàng hôn', N'Đang thuê 1 tuần', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 2, NULL, NULL, DATEADD(HOUR,-2,GETDATE())),
('TX-2026-016', 7, 3, N'Vũ Hoàng Nam', '079193005566', '0912334455', '103', DATEADD(HOUR,-6,GETDATE()), DATEADD(DAY,14,GETDATE()), NULL, NULL, 0, NULL, 714000, 2380000, 0, 2380000, 0, N'Khách thuê Grande đi dạo hồ Tuyền Lâm', N'Đang thuê 2 tuần', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 1, NULL, NULL, DATEADD(HOUR,-6,GETDATE())),
('TX-2026-017', 9, 6, N'Bùi Thanh Tùng', '079196001122', '0938445566', '203', DATEADD(HOUR,-5,GETDATE()), DATEADD(DAY,14,GETDATE()), NULL, NULL, 0, NULL, 714000, 2380000, 0, 2380000, 0, N'Khách thuê Winner X săn mây Cầu Đất', N'Đang thuê 2 tuần', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 1, NULL, NULL, DATEADD(HOUR,-5,GETDATE())),
('TX-2026-018', 3, 7, N'Nguyễn Anh Thư', '079195003456', '0918777666', '301', DATEADD(HOUR,-4,GETDATE()), DATEADD(DAY,14,GETDATE()), NULL, NULL, 0, NULL, 630000, 2100000, 0, 2100000, 0, N'Khách thuê xe Vision dạo phố', N'Đang thuê 2 tuần', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 1, NULL, NULL, DATEADD(HOUR,-4,GETDATE())),
('TX-2026-019', 16, 16, N'Nguyễn Văn Khách', '079190001111', '0988112233', NULL, DATEADD(DAY,-3,GETDATE()), DATEADD(MINUTE,-35,GETDATE()), NULL, NULL, 0, NULL, 200000, 480000, 0, 480000, 0, N'Khách thuê 3 ngày quá hạn chưa trả', N'CẢNH BÁO: Quá hạn 35 phút', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 1, NULL, NULL, DATEADD(DAY,-3,GETDATE())),
('TX-2026-020', 14, 16, N'Hoàng Trọng Khôi', '079190002222', '0908771122', NULL, DATEADD(HOUR,-5,GETDATE()), DATEADD(MINUTE,10,GETDATE()), NULL, NULL, 0, NULL, 150000, 300000, 0, 300000, 0, N'Khách thuê xe dạo phố buổi trưa', N'CẢNH BÁO: Sắp hết hạn trong 10 phút', N'Đặt cọc trước', N'Đang thuê', NULL, NULL, NULL, NULL, 2, NULL, NULL, DATEADD(HOUR,-5,GETDATE())),
('TX-2026-021', 18, 16, N'Trần Văn Bình', '079197008888', '0903112233', NULL, DATEADD(DAY,2,GETDATE()), DATEADD(DAY,5,GETDATE()), NULL, NULL, 0, NULL, 300000, 600000, 0, 600000, 0, N'Khách đặt trước Vario 160 cho cuối tuần', N'Đã đặt lịch trước', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('TX-2026-022', 19, 16, N'Nguyễn Thị Mai', '079198009999', '0915443322', NULL, DATEADD(DAY,3,GETDATE()), DATEADD(DAY,6,GETDATE()), NULL, NULL, 0, NULL, 200000, 330000, 0, 330000, 0, N'Khách đặt trước Wave RSX đi dã ngoại', N'Đã đặt lịch trước', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 2, NULL, NULL, GETDATE()),
('TX-2026-023', 20, 16, N'Phan Thanh Hùng', '079199001122', '0937665544', NULL, DATEADD(DAY,4,GETDATE()), DATEADD(DAY,8,GETDATE()), NULL, NULL, 0, NULL, 300000, 640000, 0, 640000, 0, N'Khách đặt trước Exciter 150 đi phượt', N'Đã đặt lịch trước', N'Đặt cọc trước', N'Đã đặt', NULL, NULL, NULL, NULL, 1, NULL, NULL, GETDATE()),
('TX-2026-024', 11, 9, N'Phan Văn Đức', '079199004466', '0945667788', '303', DATEADD(DAY,5,GETDATE()), DATEADD(DAY,9,GETDATE()), NULL, NULL, 0, NULL, 1200000, 1200000, 0, 1200000, 1, N'Khách đặt trước chuyến công tác tuần tới (Thanh toán 100%)', N'Đã thanh toán đủ', N'Thanh toán toàn bộ', N'Đã đặt', NULL, NULL, NULL, NULL, 1, 1, GETDATE(), GETDATE()),
('TX-2026-025', 12, 16, N'Lâm Quốc Anh', '079199005566', '0901223344', NULL, DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,1,GETDATE()), NULL, NULL, 0, NULL, 250000, 500000, 0, 500000, 1, N'Khách báo hủy đột xuất trước giờ nhận xe', N'Không hoàn cọc theo quy chế', N'Đặt cọc trước', N'Đã hủy - Không hoàn cọc', DATEADD(DAY,-1,GETDATE()), N'Khách báo hủy sát giờ nhận xe (trong ngày), không hoàn cọc', 1, 0, 1, 1, DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,-1,GETDATE()));
GO
-- 7.6 Bãi đỗ xe mẫu (Chỉ ghi nhận khi xe đã vào bãi, không có đặt trước; đa dạng từ tháng trước đến hôm nay)
INSERT INTO BaiDoXe (MaVe, BienSoXe, LoaiXe, TenKhach, SoDienThoai, SoPhong, ThoiGianVao, ThoiGianRa, HinhThucGui, PhiGui, DaThanhToan, GhiChuSuaChua, NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan) VALUES
('VE-DX-001', '51G-888.99', N'Ô tô',          N'Lê Tấn Phát',       '0969888999', '202', DATEADD(DAY,-15,GETDATE()), DATEADD(DAY,-12,GETDATE()), N'Khách phòng (Miễn phí)', 0,     1, NULL, 1, 1, DATEADD(DAY,-12,GETDATE())),
('VE-DX-002', '59-S2 123.45', N'Xe máy',        N'Khách vãng lai A',  '0908112233', NULL,  DATEADD(DAY,-10,GETDATE()), DATEADD(HOUR,-235,GETDATE()),N'Theo lượt',              5000,  1, NULL, 2, 2, DATEADD(HOUR,-235,GETDATE())),
('VE-DX-003', '49A-333.22', N'Ô tô',          N'Nguyễn Anh Thư',    '0918777666', '301', DATEADD(DAY,-8,GETDATE()),  DATEADD(DAY,-5,GETDATE()),  N'Khách phòng (Miễn phí)', 0,     1, NULL, 1, 1, DATEADD(DAY,-5,GETDATE())),
('VE-DX-004', '60B-777.88', N'Ô tô',          N'Công ty Du Lịch Đà Lạt','0914556677',NULL,DATEADD(DAY,-5,GETDATE()),  DATEADD(DAY,-4,GETDATE()),  N'Theo tiếng',             150000,1, NULL, 3, 3, DATEADD(DAY,-4,GETDATE())),
('VE-DX-005', '51H-999.11', N'Ô tô',          N'Đặng Hải Yến',      '0982334411', '302', DATEADD(DAY,-3,GETDATE()),  DATEADD(DAY,-1,GETDATE()),  N'Khách phòng (Miễn phí)', 0,     1, NULL, 2, 2, DATEADD(DAY,-1,GETDATE())),
('VE-DX-006', '59-B1 456.78', N'Xe máy',        N'Trần Thu Hà',       '0919223344', '402', DATEADD(DAY,-2,GETDATE()),  DATEADD(DAY,-1,GETDATE()),  N'Khách phòng (Miễn phí)', 0,     1, NULL, 1, 1, DATEADD(DAY,-1,GETDATE())),
('VE-DX-007', '51G-123.45', N'Ô tô',          N'Phạm Quốc Bảo',     '0933444555', '401', DATEADD(HOUR,-3,GETDATE()), NULL,                       N'Khách phòng (Miễn phí)', 0,     1, NULL, 2, 2, DATEADD(HOUR,-3,GETDATE())),
('VE-DX-008', '49A-567.89', N'Ô tô',          N'Vũ Hoàng Nam',       '0912334455', '103', DATEADD(DAY,-1,GETDATE()),  NULL,                       N'Khách phòng (Miễn phí)', 0,     1, NULL, 1, 1, DATEADD(DAY,-1,GETDATE())),
('VE-DX-009', '29B-888.99', N'Ô tô',          N'Nguyễn Hoàng Long',  '0903889900', '501', DATEADD(HOUR,-5,GETDATE()), NULL,                       N'Khách phòng (Miễn phí)', 0,     1, NULL, 3, 3, DATEADD(HOUR,-5,GETDATE())),
('VE-DX-010', '59-S3 444.11', N'Xe máy',        N'Lê Minh Tuấn',      '0912123456', NULL,  DATEADD(HOUR,-2,GETDATE()), NULL,                       N'Theo lượt',              5000,  1, NULL, 1, 1, DATEADD(HOUR,-2,GETDATE())),
('VE-DX-011', '60A-998.22', N'Ô tô',          N'Hoàng Trọng Khôi',  '0908771122', NULL,  DATEADD(HOUR,-2,GETDATE()), NULL,                       N'Theo tiếng',             100000,0, NULL, 2, NULL, NULL),
('VE-DX-012', '59-A1 999.88', N'Xe máy',        N'Khách vãng lai B',  '0900111222', NULL,  DATEADD(HOUR,-3,GETDATE()), DATEADD(HOUR,-1,GETDATE()), N'Theo lượt',              5000,  1, N'Đèn khu vực B hơi yếu, đã báo bảo trì', 2, 2, DATEADD(HOUR,-1,GETDATE())),
('VE-DX-013', '51H-234.56', N'Ô tô',          N'Trần Văn Nam',      '0938112299', NULL,  DATEADD(HOUR,-6,GETDATE()), DATEADD(HOUR,-2,GETDATE()), N'Theo tiếng',             200000,1, NULL, 1, 1, DATEADD(HOUR,-2,GETDATE()));
GO
-- 7.7 Đơn giặt ủi mẫu (Quá khứ chỉ có Hoàn tất; Hôm nay đa dạng Chờ giao, Đang giặt, Giặt xong, Hoàn tất, Đã hủy; KHÔNG có nhận đồ tương lai)
INSERT INTO DonGiatUi (MaDon, KhachHangId, TenKhach, SoDienThoai, SoPhong, DoiTacId, LoaiDichVu, KhoiLuongKg, DonGiaKg, TongTienThuKhach, TienKhachSanNhan, TienDoiTacNhan, TinhTrangQuanAoLucNhan, NgayNhan, NgayGiaoDoiTac, NgayHenTra, NgayTraThucTe, HinhThucThanhToan, TrangThai, DaThanhToan, NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan, CoDenBu, BenChiuTrachNhiem, SoTienDenBu, LyDoDenBu, NgayGhiNhanDenBu, NguoiGhiNhanDenBuId) VALUES
('DH-GU-001', 1, N'Đoàn Minh Trí',     '0908112233', '101', 1,    N'Giặt sấy thông thường', 4.0, 30000, 120000, 36000,  84000,  N'Đồ du lịch đợt 1',            DATEADD(DAY,-15,GETDATE()),DATEADD(DAY,-15,GETDATE()),DATEADD(DAY,-14,GETDATE()),DATEADD(DAY,-14,GETDATE()),N'Thanh toán trực tiếp', N'Hoàn tất', 1, 1, 1, DATEADD(DAY,-14,GETDATE()), 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-002', 3, N'Vũ Hoàng Nam',      '0912334455', '103', 2,    N'Giặt hấp cao cấp',     2.5, 80000, 200000, 70000, 130000,  N'Áo măng tô dạ hội',           DATEADD(DAY,-12,GETDATE()),DATEADD(DAY,-12,GETDATE()),DATEADD(DAY,-11,GETDATE()),DATEADD(DAY,-11,GETDATE()),N'Ghi nợ vào phòng',   N'Hoàn tất', 1, 2, 2, DATEADD(DAY,-11,GETDATE()), 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-003', 7, N'Nguyễn Anh Thư',    '0918777666', '301', 1,    N'Ủi phẳng',              3.0, 40000, 120000, 36000,  84000,  N'6 váy lụa nhẹ',               DATEADD(DAY,-10,GETDATE()),DATEADD(DAY,-10,GETDATE()),DATEADD(DAY,-9,GETDATE()), DATEADD(DAY,-9,GETDATE()), N'Thanh toán trực tiếp', N'Hoàn tất', 1, 1, 1, DATEADD(DAY,-9,GETDATE()),  0, NULL, 0, NULL, NULL, NULL),
('DH-GU-004', 5, N'Lê Tấn Phát',       '0969888999', '202', 1,    N'Giặt sấy thông thường', 5.0, 30000, 150000, 45000, 105000,  N'Quần áo thể thao (Có đền bù)',DATEADD(DAY,-8,GETDATE()), DATEADD(DAY,-8,GETDATE()), DATEADD(DAY,-7,GETDATE()), DATEADD(DAY,-7,GETDATE()), N'Ghi nợ vào phòng',   N'Hoàn tất', 1, 3, 3, DATEADD(DAY,-7,GETDATE()),  1, N'DoiTac', 100000, N'Tiệm giặt làm phai cúc áo, đã bồi thường 100k', DATEADD(DAY,-7,GETDATE()), 1),
('DH-GU-005', 4, N'Hoàng Mai Linh',    '0978112244', '201', NULL, N'Giặt sấy thông thường', 2.0, 30000,  60000,  60000,  0,      N'Khách đổi ý nhận lại tự giặt',DATEADD(DAY,-6,GETDATE()), NULL,                      DATEADD(DAY,-5,GETDATE()), NULL,                      N'Thanh toán trực tiếp', N'Đã hủy',   0, 1, NULL, NULL,                     0, NULL, 0, NULL, NULL, NULL),
('DH-GU-006', 9, N'Phan Văn Đức',      '0945667788', '303', 2,    N'Giặt hấp cao cấp',     3.0, 80000, 240000, 84000, 156000,  N'2 bộ vest dự lễ 2/9',         DATEADD(DAY,-4,GETDATE()), DATEADD(DAY,-4,GETDATE()), DATEADD(DAY,-3,GETDATE()), DATEADD(DAY,-3,GETDATE()), N'Ghi nợ vào phòng',   N'Hoàn tất', 1, 2, 2, DATEADD(DAY,-3,GETDATE()), 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-007', 6, N'Bùi Thanh Tùng',    '0938445566', '203', 1,    N'Giặt sấy thông thường', 4.5, 30000, 135000, 40500,  94500,  N'Đồ cotton gia đình',          DATEADD(DAY,-2,GETDATE()), DATEADD(DAY,-2,GETDATE()), DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,-1,GETDATE()), N'Thanh toán trực tiếp', N'Hoàn tất', 1, 1, 1, DATEADD(DAY,-1,GETDATE()), 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-008', 8, N'Đặng Hải Yến',      '0982334411', '302', 2,    N'Giặt hấp cao cấp',     2.0, 80000, 160000, 56000, 104000,  N'Đầm lụa cao cấp',             DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,-1,GETDATE()), DATEADD(HOUR,-4,GETDATE()),DATEADD(HOUR,-4,GETDATE()),N'Ghi nợ vào phòng',   N'Hoàn tất', 1, 2, 2, DATEADD(HOUR,-4,GETDATE()),0, NULL, 0, NULL, NULL, NULL),
('DH-GU-009', 11,N'Trần Thu Hà',       '0919223344', '402', 1,    N'Ủi phẳng',              2.5, 40000, 100000, 30000,  70000,  N'5 áo sơ mi văn phòng',        DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,-1,GETDATE()), DATEADD(HOUR,-3,GETDATE()),DATEADD(HOUR,-3,GETDATE()),N'Thanh toán trực tiếp', N'Hoàn tất', 1, 1, 1, DATEADD(HOUR,-3,GETDATE()),0, NULL, 0, NULL, NULL, NULL),
('DH-GU-010', 12,N'Đỗ Minh Khang',     '0987556677', '403', 1,    N'Giặt sấy thông thường', 3.5, 30000, 105000, 31500,  73500,  N'Đồ phượt xe máy',             DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,-1,GETDATE()), DATEADD(HOUR,-2,GETDATE()),DATEADD(HOUR,-2,GETDATE()),N'Ghi nợ vào phòng',   N'Hoàn tất', 1, 2, 2, DATEADD(HOUR,-2,GETDATE()),0, NULL, 0, NULL, NULL, NULL),
('DH-GU-011', 1, N'Đoàn Minh Trí',     '0908112233', '101', NULL, N'Giặt hấp cao cấp',     2.0, 80000, 160000, 160000, 0,      N'Áo sơ mi lụa + quần tây (Mới nhận)',  DATEADD(HOUR,-2,GETDATE()), NULL,                      DATEADD(HOUR,6,GETDATE()),  NULL,                      N'Ghi nợ vào phòng',   N'Chờ giao đối tác', 0, 1, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-012', 5, N'Lê Tấn Phát',       '0969888999', '202', NULL, N'Giặt sấy thông thường', 3.5, 30000, 105000, 105000, 0,      N'Đồ thể thao đi phượt (Chờ gom tiệm)', DATEADD(HOUR,-1,GETDATE()), NULL,                      DATEADD(HOUR,5,GETDATE()),  NULL,                      N'Thanh toán trực tiếp', N'Chờ giao đối tác', 1, 2, 2, DATEADD(HOUR,-1,GETDATE()), 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-013', 10,N'Phạm Quốc Bảo',     '0933444555', '401', NULL, N'Giặt hấp cao cấp',     2.5, 80000, 200000, 200000, 0,      N'Bộ vest Dior cao cấp (Chờ giao)',    DATEADD(MINUTE,-45,GETDATE()),NULL,                    DATEADD(DAY,1,GETDATE()),   NULL,                      N'Ghi nợ vào phòng',   N'Chờ giao đối tác', 0, 1, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-014', 14,N'Lê Thị Ngọc Ánh',   '0979334455', '502', NULL, N'Giặt sấy thông thường', 3.0, 30000,  90000,  90000,  0,      N'Đồ len mỏng + khăn quàng (Chờ giao)', DATEADD(MINUTE,-20,GETDATE()),NULL,                    DATEADD(HOUR,6,GETDATE()),  NULL,                      N'Ghi nợ vào phòng',   N'Chờ giao đối tác', 0, 2, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-015', 2, N'Trịnh Hồng Quân',   '0909123456', '102', 1,    N'Ủi phẳng',              2.5, 40000, 100000, 30000,  70000,  N'5 áo sơ mi công sở',                  DATEADD(HOUR,-3,GETDATE()), DATEADD(HOUR,-2,GETDATE()), DATEADD(HOUR,4,GETDATE()),  NULL,                      N'Ghi nợ vào phòng',   N'Đang giặt',        0, 2, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-016', 3, N'Vũ Hoàng Nam',      '0912334455', '103', 2,    N'Giặt hấp cao cấp',     2.5, 80000, 200000, 70000, 130000,  N'Áo măng tô dạ',                       DATEADD(HOUR,-3,GETDATE()), DATEADD(HOUR,-2,GETDATE()), DATEADD(DAY,1,GETDATE()),   NULL,                      N'Ghi nợ vào phòng',   N'Đang giặt',        0, 3, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-017', 4, N'Hoàng Mai Linh',    '0978112244', '201', 1,    N'Giặt sấy thông thường', 3.0, 30000,  90000, 27000,  63000,  N'Váy hoa mùa hè',                      DATEADD(HOUR,-2,GETDATE()), DATEADD(HOUR,-1,GETDATE()), DATEADD(HOUR,5,GETDATE()),  NULL,                      N'Ghi nợ vào phòng',   N'Đang giặt',        0, 1, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-018', 7, N'Nguyễn Anh Thư',    '0918777666', '301', 2,    N'Giặt hấp cao cấp',     2.0, 80000, 160000, 56000, 104000,  N'Váy dạ hội trắng honeymoon',          DATEADD(HOUR,-2,GETDATE()), DATEADD(HOUR,-1,GETDATE()), DATEADD(DAY,1,GETDATE()),   NULL,                      N'Ghi nợ vào phòng',   N'Đang giặt',        0, 2, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-019', 6, N'Bùi Thanh Tùng',    '0938445566', '203', 1,    N'Giặt sấy thông thường', 4.0, 30000, 120000, 36000,  84000,  N'Quần áo ngủ + áo khoác gió',          DATEADD(HOUR,-4,GETDATE()), DATEADD(HOUR,-3,GETDATE()), DATEADD(HOUR,2,GETDATE()),  DATEADD(MINUTE,-30,GETDATE()),N'Thanh toán trực tiếp', N'Giặt xong',     1, 1, 1, DATEADD(HOUR,-4,GETDATE()), 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-020', 8, N'Đặng Hải Yến',      '0982334411', '302', 2,    N'Giặt hấp cao cấp',     1.5, 80000, 120000, 42000,  78000,  N'Đầm lụa tơ tằm',                      DATEADD(HOUR,-4,GETDATE()), DATEADD(HOUR,-3,GETDATE()), DATEADD(HOUR,2,GETDATE()),  DATEADD(MINUTE,-20,GETDATE()),N'Ghi nợ vào phòng',   N'Giặt xong',     0, 2, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-021', 12,N'Đỗ Minh Khang',     '0987556677', '403', 1,    N'Giặt sấy thông thường', 3.0, 30000,  90000, 27000,  63000,  N'Quần áo công sở',                     DATEADD(HOUR,-5,GETDATE()), DATEADD(HOUR,-4,GETDATE()), DATEADD(HOUR,1,GETDATE()),  DATEADD(MINUTE,-15,GETDATE()),N'Ghi nợ vào phòng',   N'Giặt xong',     0, 1, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-022', 9, N'Phan Văn Đức',      '0945667788', '303', 1,    N'Giặt sấy thông thường', 3.0, 30000,  90000, 27000,  63000,  N'Quần áo du lịch gia đình',            DATEADD(HOUR,-6,GETDATE()), DATEADD(HOUR,-5,GETDATE()), DATEADD(HOUR,-1,GETDATE()), DATEADD(HOUR,-1,GETDATE()),  N'Thanh toán trực tiếp', N'Hoàn tất',      1, 1, 1, DATEADD(HOUR,-1,GETDATE()), 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-023', 13,N'Nguyễn Hoàng Long', '0903889900', '501', 1,    N'Ủi phẳng',              2.0, 40000,  80000, 24000,  56000,  N'Áo sơ mi họp sáng',                   DATEADD(HOUR,-6,GETDATE()), DATEADD(HOUR,-5,GETDATE()), DATEADD(HOUR,-2,GETDATE()), DATEADD(HOUR,-2,GETDATE()),  N'Ghi nợ vào phòng',   N'Hoàn tất',      1, 2, 2, DATEADD(HOUR,-2,GETDATE()), 0, NULL, 0, NULL, NULL, NULL),
('DH-GU-024', 11,N'Trần Thu Hà',       '0919223344', '402', NULL, N'Giặt sấy thông thường', 2.0, 30000,  60000,  60000,  0,      N'Khách báo hủy lấy áo mặc gấp đi ăn trưa',DATEADD(HOUR,-2,GETDATE()),NULL,                   DATEADD(HOUR,4,GETDATE()),  NULL,                      N'Ghi nợ vào phòng',   N'Đã hủy',        0, 1, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL);
GO
