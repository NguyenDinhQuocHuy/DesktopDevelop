# HƯỚNG DẪN CHI TIẾT TỪNG BƯỚC (STEP-BY-STEP) DỰ ÁN BÀI KIỂM TRA SỐ 2
**Môn:** Phát triển ứng dụng Desktop  
**Chủ đề:** Quản lý nhà cung cấp (Ôn tập tổng hợp Lab 2 -> Lab 5)  
**Tác giả:** 2411849 - Nguyễn Đình Quốc Huy  

---

## MỤC LỤC
1. [Tổng quan Kiến trúc Dự án](#1-tổng-quan-kiến-trúc-dự-án)
2. [Bước 1: Thiết lập CSDL SQL Server & Stored Procedures](#bước-1-thiết-lập-csdl-sql-server--stored-procedures)
3. [Bước 2: Xây dựng Lớp Đối tượng NhaCungCap.cs](#bước-2-xây-dựng-lớp-đối-tượng-nhacungcapcs)
4. [Bước 3: Thiết kế Interface Đa hình IDocGhiFile.cs](#bước-3-thiết-kế-interface-đa-hình-idocghifilecs)
5. [Bước 4: Hiện thực 4 lớp Đọc/Ghi File (TXT, JSON, XML, Excel)](#bước-4-hiện-thực-4-lớp-đọcghi-file-txt-json-xml-excel)
6. [Bước 5: Lớp Quản lý Danh sách trên RAM QuanLyNhaCungCap.cs](#bước-5-lớp-quản-lý-danh-sách-trên-ram-quanlynhacungcapcs)
7. [Bước 6: Thiết lập Giao diện Windows Forms (Designer)](#bước-6-thiết-lập-giao-diện-windows-forms-designer)
8. [Bước 7: Code xử lý Form frmQuanLyNhaCungCap.cs (Chuẩn Lab 5)](#bước-7-code-xử-lý-form-frmquanlynhacungcapcs-chuẩn-lab-5)
9. [Bước 8: Kết nối Form 1 sang Form 2 (Hóa đơn / Bán hàng)](#bước-8-kết-nối-form-1-sang-form-2-hóa-đơn--bán-hàng)
10. [Bí kíp & Mẹo vàng khi đi thi thực hành](#10-bí-kíp--mẹo-vàng-khi-đi-thi-thực-hành)

---

## 1. TỔNG QUAN KIẾN TRÚC DỰ ÁN
Dự án áp dụng mô hình 3 lớp tách biệt, cơ chế **Disconnected Data Access (Truy xuất ngắt kết nối)**:
- **Tầng CSDL (Database)**: SQL Server lưu trữ bảng `NhaCungCap`.
- **Tầng Dữ liệu bộ nhớ (RAM)**: `QuanLyNhaCungCap` quản lý `List<NhaCungCap>`. Mọi thao tác tìm kiếm, lọc, hiển thị đều xử lý trên RAM bằng LINQ.
- **Tầng Giao diện (GUI)**: `DataGridView` chỉ làm việc với `List<NhaCungCap>` thông qua hàm dùng chung `HienThiGridView(ds)`.
- **Đa hình đọc/ghi file**: Thông qua `IDocGhiFile`, chương trình dễ dàng đọc/xuất 4 định dạng (TXT, JSON, XML, CSV/Excel) mà không làm thay đổi logic giao diện.

---

## BƯỚC 1: THIẾT LẬP CSDL SQL SERVER & STORED PROCEDURES

### 1.1. Cấu trúc bảng `NhaCungCap`
- `MaNCC` : `nvarchar(20)`, Khóa chính (Primary Key).
- `TenNCC` : `nvarchar(100)`, Not Null.
- `DiaChi` : `nvarchar(200)`, Cho phép Null.
- `SoDT` : `varchar(15)`, Cho phép Null.
- `MoTa` : `nvarchar(255)`, Cho phép Null.

### 1.2. Mã SQL tạo bảng và Stored Procedures (Chuẩn Lab 5)
Lưu vào file `QuanLyNhaCungCap.sql`:

```sql
USE [QuanLyNhaCungCap]
GO

-- 1. BẢNG NHÀ CUNG CẤP
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NhaCungCap]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NhaCungCap](
    [MaNCC] [nvarchar](20) NOT NULL PRIMARY KEY,
    [TenNCC] [nvarchar](100) NOT NULL,
    [DiaChi] [nvarchar](200) NULL,
    [SoDT] [varchar](15) NULL,
    [MoTa] [nvarchar](255) NULL
);
END
GO

-- 2. DỮ LIỆU MẪU
IF NOT EXISTS (SELECT * FROM NhaCungCap WHERE MaNCC = 'NCC01')
BEGIN
    INSERT INTO NhaCungCap (MaNCC, TenNCC, DiaChi, SoDT, MoTa) VALUES 
    (N'NCC01', N'Công ty Cổ phần Sữa Việt Nam Vinamilk', N'10 Tân Trào, Q.7, TP.HCM', '02854155555', N'Sữa tươi, Sữa chua'),
    (N'NCC02', N'Tập đoàn Masan Consumer', N'23 Lê Duẩn, Q.1, TP.HCM', '02862563862', N'Nước mắm, mì tôm, tương ớt'),
    (N'NCC03', N'Công ty CP Sữa TH True Milk', N'Nghĩa Sơn, Nghĩa Đàn, Nghệ An', '1800545440', N'Sữa, bơ, phô mai'),
    (N'NCC04', N'Công ty Cổ phần Acecook Việt Nam', N'Lô II-3, KCN Tân Bình, TP.HCM', '02838154064', N'Mì Hảo Hảo, phở Đệ Nhất, miến'),
    (N'NCC05', N'Công ty TNHH NGK Coca-Cola VN', N'Xa Lộ Hà Nội, Linh Trung, TP.HCM', '1900555584', N'Nước ngọt có ga');
END
GO

-- 3. PROCEDURE: LẤY TẤT CẢ
IF OBJECT_ID('LayTatCaNhaCC', 'P') IS NOT NULL DROP PROCEDURE LayTatCaNhaCC;
GO
CREATE PROCEDURE LayTatCaNhaCC
AS
BEGIN
    SELECT MaNCC, TenNCC, DiaChi, SoDT, MoTa FROM NhaCungCap;
END
GO

-- 4. PROCEDURE: THÊM MỚI
IF OBJECT_ID('ThemNhaCC', 'P') IS NOT NULL DROP PROCEDURE ThemNhaCC;
GO
CREATE PROCEDURE ThemNhaCC
    @MaNCC NVARCHAR(20),
    @TenNCC NVARCHAR(100),
    @DiaChi NVARCHAR(200),
    @SoDT VARCHAR(15),
    @MoTa NVARCHAR(255)
AS
BEGIN
    INSERT INTO NhaCungCap(MaNCC, TenNCC, DiaChi, SoDT, MoTa)
    VALUES (@MaNCC, @TenNCC, @DiaChi, @SoDT, @MoTa);
END
GO

-- 5. PROCEDURE: CẬP NHẬT
IF OBJECT_ID('CapNhatNhaCC', 'P') IS NOT NULL DROP PROCEDURE CapNhatNhaCC;
GO
CREATE PROCEDURE CapNhatNhaCC
    @MaNCC NVARCHAR(20),
    @TenNCC NVARCHAR(100),
    @DiaChi NVARCHAR(200),
    @SoDT VARCHAR(15),
    @MoTa NVARCHAR(255)
AS
BEGIN
    UPDATE NhaCungCap
    SET TenNCC = @TenNCC,
        DiaChi = @DiaChi,
        SoDT = @SoDT,
        MoTa = @MoTa
    WHERE MaNCC = @MaNCC;
END
GO

-- 6. PROCEDURE: XÓA
IF OBJECT_ID('XoaNhaCC', 'P') IS NOT NULL DROP PROCEDURE XoaNhaCC;
GO
CREATE PROCEDURE XoaNhaCC
    @MaNCC NVARCHAR(20)
AS
BEGIN
    DELETE FROM NhaCungCap WHERE MaNCC = @MaNCC;
END
GO
```

---

## BƯỚC 2: XÂY DỰNG LỚP ĐỐI TƯỢNG `NhaCungCap.cs`
Lớp này tự đảm nhiệm việc đóng gói dữ liệu và bóc tách dòng:
- Constructor bóc tách đa năng: mặc định phân cách `'|'` (cho file TXT), truyền `','` (cho file CSV/Excel).
- Hàm `ToString()`: xuất dòng TXT phân cách `'|'`.
- Hàm `ToExcelString()`: xuất dòng CSV phân cách `','`.

```csharp
using System;

namespace _2411849_NguyenDinhQuocHuy_BaiKiemTraSo2
{
    public class NhaCungCap
    {
        public string MaNCC { get; set; }
        public string TenNCC { get; set; }
        public string DiaChi { get; set; }
        public string SoDT { get; set; }
        public string MoTa { get; set; }

        public NhaCungCap() { }

        public NhaCungCap(string ma, string ten, string diaChi, string soDT, string moTa)
        {
            this.MaNCC = ma;
            this.TenNCC = ten;
            this.DiaChi = diaChi;
            this.SoDT = soDT;
            this.MoTa = moTa;
        }

        // Constructor bóc tách từ chuỗi: phanCach mặc định là '|', truyền ',' khi đọc CSV/Excel
        public NhaCungCap(string line, char phanCach = '|')
        {
            string[] parts = line.Split(phanCach);
            if (parts.Length >= 5)
            {
                this.MaNCC = parts[0].Trim();
                this.TenNCC = parts[1].Trim();
                this.DiaChi = parts[2].Trim();
                this.SoDT = parts[3].Trim();
                this.MoTa = parts[4].Trim();
            }
        }

        public override string ToString()
        {
            return $"{MaNCC}|{TenNCC}|{DiaChi}|{SoDT}|{MoTa}";
        }

        public string ToExcelString()
        {
            return $"{MaNCC},{TenNCC},{DiaChi},{SoDT},{MoTa}";
        }
    }
}
```

---

## BƯỚC 3: THIẾT KẾ INTERFACE ĐA HÌNH `IDocGhiFile.cs`
Interface định nghĩa chuẩn hành vi đọc/ghi tập tin:

```csharp
using System.Collections.Generic;

namespace _2411849_NguyenDinhQuocHuy_BaiKiemTraSo2
{
    public interface IDocGhiFile
    {
        List<NhaCungCap> DocFile(string duongDan);
        void GhiFile(string duongDan, List<NhaCungCap> danhSach);
    }
}
```

---

## BƯỚC 4: HIỆN THỰC 4 LỚP ĐỌC/GHI FILE (TXT, JSON, XML, EXCEL)

### 4.1. `DocGhiTXT.cs` (Dùng StreamReader & StreamWriter)
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _2411849_NguyenDinhQuocHuy_BaiKiemTraSo2
{
    internal class DocGhiTXT : IDocGhiFile
    {
        public List<NhaCungCap> DocFile(string duongDan)
        {
            if (!File.Exists(duongDan)) return null;
            try
            {
                List<NhaCungCap> ds = new List<NhaCungCap>();
                using (StreamReader sr = new StreamReader(duongDan, Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        ds.Add(new NhaCungCap(line)); // Tách mặc định theo dấu |
                    }
                }
                return ds;
            }
            catch { return null; }
        }

        public void GhiFile(string duongDan, List<NhaCungCap> danhSach)
        {
            using (StreamWriter sw = new StreamWriter(duongDan, false, Encoding.UTF8))
            {
                foreach (var ncc in danhSach)
                    sw.WriteLine(ncc.ToString());
            }
        }
    }
}
```

### 4.2. `DocGhiJSON.cs` (Dùng JavaScriptSerializer có sẵn trong `System.Web.Extensions`)
> *Lưu ý*: Chuột phải **References** trong Solution Explorer -> **Add Reference...** -> Tích chọn **`System.Web.Extensions`**.

```csharp
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace _2411849_NguyenDinhQuocHuy_BaiKiemTraSo2
{
    internal class DocGhiJSON : IDocGhiFile
    {
        public List<NhaCungCap> DocFile(string duongDan)
        {
            if (!File.Exists(duongDan)) return null;
            try
            {
                string json = File.ReadAllText(duongDan, Encoding.UTF8);
                return new JavaScriptSerializer().Deserialize<List<NhaCungCap>>(json);
            }
            catch { return null; }
        }

        public void GhiFile(string duongDan, List<NhaCungCap> danhSach)
        {
            string json = new JavaScriptSerializer().Serialize(danhSach);
            File.WriteAllText(duongDan, json, Encoding.UTF8);
        }
    }
}
```

### 4.3. `DocGhiXML.cs` (Dùng XmlSerializer chuẩn .NET)
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace _2411849_NguyenDinhQuocHuy_BaiKiemTraSo2
{
    internal class DocGhiXML : IDocGhiFile
    {
        public List<NhaCungCap> DocFile(string duongDan)
        {
            if (!File.Exists(duongDan)) return null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<NhaCungCap>));
                using (StreamReader sr = new StreamReader(duongDan))
                {
                    return (List<NhaCungCap>)xs.Deserialize(sr);
                }
            }
            catch { return null; }
        }

        public void GhiFile(string duongDan, List<NhaCungCap> danhSach)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<NhaCungCap>));
            using (StreamWriter sw = new StreamWriter(duongDan))
            {
                xs.Serialize(sw, danhSach);
            }
        }
    }
}
```

### 4.4. `DocGhiExcel.cs` (Định dạng CSV UTF-8 mở trực tiếp bằng Excel)
```csharp
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _2411849_NguyenDinhQuocHuy_BaiKiemTraSo2
{
    internal class DocGhiExcel : IDocGhiFile
    {
        public List<NhaCungCap> DocFile(string duongDan)
        {
            if (!File.Exists(duongDan)) return null;
            try
            {
                List<NhaCungCap> ds = new List<NhaCungCap>();
                using (StreamReader sr = new StreamReader(duongDan, Encoding.UTF8))
                {
                    sr.ReadLine(); // Bỏ qua 1 dòng tiêu đề cột
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        ds.Add(new NhaCungCap(line, ',')); // Bóc tách theo dấu phẩy
                    }
                }
                return ds;
            }
            catch { return null; }
        }

        public void GhiFile(string duongDan, List<NhaCungCap> danhSach)
        {
            // UTF-8 có BOM để Excel mở không bị lỗi font chữ Tiếng Việt
            using (StreamWriter sw = new StreamWriter(duongDan, false, Encoding.UTF8))
            {
                sw.WriteLine("Mã NCC,Tên nhà cung cấp,Địa chỉ,Số điện thoại,Mô tả");
                foreach (var ncc in danhSach)
                    sw.WriteLine(ncc.ToExcelString());
            }
        }
    }
}
```

---

## BƯỚC 5: LỚP QUẢN LÝ DANH SÁCH TRÊN RAM `QuanLyNhaCungCap.cs`
Bóc tách bài toán nghiệp vụ, áp dụng LINQ gọn gàng:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2411849_NguyenDinhQuocHuy_BaiKiemTraSo2
{
    public class QuanLyNhaCungCap
    {
        public List<NhaCungCap> DanhSach { get; set; }

        public QuanLyNhaCungCap()
        {
            DanhSach = new List<NhaCungCap>();
        }

        // Hàm gốc tìm theo mã (dùng FirstOrDefault chống phân biệt hoa/thường)
        public NhaCungCap TimTheoMa(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma)) return null;
            return DanhSach.FirstOrDefault(x => x.MaNCC != null && x.MaNCC.Equals(ma.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        // Tái sử dụng TimTheoMa
        public bool KiemTraTonTai(string ma) => TimTheoMa(ma) != null;

        // Thêm vào RAM
        public void ThemNCC(NhaCungCap ncc) => DanhSach.Add(ncc);

        // Cập nhật trên RAM
        public bool CapNhatNCC(NhaCungCap ncc)
        {
            var item = TimTheoMa(ncc.MaNCC);
            if (item == null) return false;

            item.TenNCC = ncc.TenNCC;
            item.DiaChi = ncc.DiaChi;
            item.SoDT = ncc.SoDT;
            item.MoTa = ncc.MoTa;
            return true;
        }

        // Xóa trên RAM
        public bool XoaNCC(string ma)
        {
            var item = TimTheoMa(ma);
            return item != null && DanhSach.Remove(item);
        }

        // Tìm kiếm theo tên (LINQ Where)
        public List<NhaCungCap> TimKiemTheoTen(string key)
        {
            return DanhSach.Where(x => !string.IsNullOrEmpty(x.TenNCC) && x.TenNCC.ToLower().Contains(key)).ToList();
        }

        // Tìm kiếm theo số ĐT (LINQ Where)
        public List<NhaCungCap> TimKiemTheoSDT(string key)
        {
            return DanhSach.Where(x => !string.IsNullOrEmpty(x.SoDT) && x.SoDT.Contains(key)).ToList();
        }

        // Đa hình đọc file: Chỉ ghi đè khi đọc thành công (không null)
        public bool DocTuFile(string duongDan, IDocGhiFile boDocGhi)
        {
            var dsMoi = boDocGhi.DocFile(duongDan);
            if (dsMoi != null)
            {
                DanhSach = dsMoi;
                return true;
            }
            return false;
        }

        // Đa hình ghi file
        public void GhiRaFile(string duongDan, IDocGhiFile boDocGhi)
        {
            boDocGhi.GhiFile(duongDan, DanhSach);
        }
    }
}
```

---

## BƯỚC 6: THIẾT LẬP GIAO DIỆN WINDOWS FORMS (DESIGNER)

### 6.1. Danh sách Control và Tên biến (`Name`)
- `txtMaNCC`: Nhập mã nhà cung cấp.
- `txtTenNCC`: Nhập tên nhà cung cấp.
- `txtDiaChi`: Nhập địa chỉ.
- `txtSoDT`: Nhập số điện thoại.
- `txtMoTa`: Nhập mô tả (`Multiline = True`).
- `btnMacDinh`: Nút Mặc định (xóa trắng form).
- `btnLuu`: Nút Lưu (Thêm/Sửa).
- `txtTimKiem`: Ô nhập từ khóa tìm kiếm.
- `rdTheoTen`: Radio button tìm theo tên (`Checked = True`).
- `rdTheoSDT`: Radio button tìm theo số ĐT.
- `btnXuatJSON`: Nút Xuất JSON ở góc dưới.
- `menuStrip1`:
  - `msDocFile` -> `msiDocTXT`, `msiDocJSON`, `msiDocXML`, `msiDocExcel`.
  - `msGhiFile` -> `msiGhiTXT`, `msiGhiJSON`, `msiGhiXML`, `msiGhiExcel`.

### 6.2. Thuộc tính quan trọng của DataGridView (`dgvNhaCungCap`)
1. **`AutoGenerateColumns = False`**: Luôn đặt trong `Form_Load`.
2. **`SelectionMode = FullRowSelect`**: Bấm vào ô bất kỳ là chọn nguyên cả hàng.
3. **`MultiSelect = False`**: Chỉ cho chọn 1 dòng tại 1 thời điểm.
4. **`ReadOnly = True`**: Khóa bảng, không cho sửa trực tiếp trên lưới.
5. **Cấu hình 5 cột (Edit Columns)**:
   - Cột 1: `Name` = `colNhaCC`, `HeaderText` = `Nhà cung cấp`, `DataPropertyName` = `MaNCC`
   - Cột 2: `Name` = `colTenNCC`, `HeaderText` = `Tên nhà cung cấp`, `DataPropertyName` = `TenNCC`
   - Cột 3: `Name` = `colDiaChi`, `HeaderText` = `Địa chỉ`, `DataPropertyName` = `DiaChi`
   - Cột 4: `Name` = `colSoDT`, `HeaderText` = `Số điện thoại`, `DataPropertyName` = `SoDT`
   - Cột 5: `Name` = `colMoTa`, `HeaderText` = `Mô tả`, `DataPropertyName` = `MoTa`

---

## BƯỚC 7: CODE XỬ LÝ FORM `frmQuanLyNhaCungCap.cs` (CHUẨN LAB 5)

```csharp
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace _2411849_NguyenDinhQuocHuy_BaiKiemTraSo2
{
    public partial class frmQuanLyNhaCungCap : Form
    {
        // 1. Chuỗi kết nối Database chuẩn
        private string chuoiKetNoi = @"server=.\SQLEXPRESS01; database=QuanLyNhaCungCap; Integrated Security=True; TrustServerCertificate=True";

        // 2. Đối tượng nghiệp vụ quản lý danh sách trên RAM
        private QuanLyNhaCungCap qlNCC = new QuanLyNhaCungCap();

        // 3. Biến truyền giá trị sang Form khác khi chọn xong (chỉ lấy Tên)
        public string TenDuocChon { get; private set; }

        public frmQuanLyNhaCungCap()
        {
            InitializeComponent();
        }

        private void frmQuanLyNhaCungCap_Load(object sender, EventArgs e)
        {
            dgvNhaCungCap.AutoGenerateColumns = false;
            rdTheoTen.Checked = true;
            NapDuLieuTuDatabase();
        }

        // ==================== CÁC HÀM TRUY VẤN CSDL (CHUẨN LAB 5 TỐI ƯU) ====================

        // Đọc danh sách từ SQL Server
        private void NapDuLieuTuDatabase()
        {
            qlNCC.DanhSach.Clear();
            try
            {
                SqlConnection conn = new SqlConnection(chuoiKetNoi);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "EXECUTE LayTatCaNhaCC";

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NhaCungCap ncc = new NhaCungCap(
                        reader["MaNCC"].ToString(),
                        reader["TenNCC"].ToString(),
                        reader["DiaChi"].ToString(),
                        reader["SoDT"].ToString(),
                        reader["MoTa"].ToString()
                    );
                    qlNCC.ThemNCC(ncc);
                }
                reader.Close();
                conn.Close();
                conn.Dispose();

                HienThiGridView(qlNCC.DanhSach);
            }
            catch (SqlException ex) { MessageBox.Show("SQL Error: " + ex.Message, "Lỗi SQL"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Lỗi"); }
        }

        // HÀM DÙNG CHUNG CỐT LÕI: Thực thi procedure Thêm/Sửa (Chuẩn Lab 5)
        private bool ThucThiThuTucNhaCC(string tenProcedure, NhaCungCap ncc)
        {
            try
            {
                SqlConnection conn = new SqlConnection(chuoiKetNoi);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"EXECUTE {tenProcedure} @MaNCC, @TenNCC, @DiaChi, @SoDT, @MoTa";

                cmd.Parameters.AddWithValue("@MaNCC", ncc.MaNCC ?? "");
                cmd.Parameters.AddWithValue("@TenNCC", ncc.TenNCC ?? "");
                cmd.Parameters.AddWithValue("@DiaChi", string.IsNullOrEmpty(ncc.DiaChi) ? (object)DBNull.Value : ncc.DiaChi);
                cmd.Parameters.AddWithValue("@SoDT", string.IsNullOrEmpty(ncc.SoDT) ? (object)DBNull.Value : ncc.SoDT);
                cmd.Parameters.AddWithValue("@MoTa", string.IsNullOrEmpty(ncc.MoTa) ? (object)DBNull.Value : ncc.MoTa);

                conn.Open();
                int soDong = cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
                return soDong > 0;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL Error: " + ex.Message, "Lỗi SQL");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Lỗi");
                return false;
            }
        }

        private bool ThemNhaCCVaoDatabase(NhaCungCap ncc) => ThucThiThuTucNhaCC("ThemNhaCC", ncc);
        private bool CapNhatNhaCCVaoDatabase(NhaCungCap ncc) => ThucThiThuTucNhaCC("CapNhatNhaCC", ncc);

        private bool XoaNhaCCKhoiDatabase(string maNCC)
        {
            try
            {
                SqlConnection conn = new SqlConnection(chuoiKetNoi);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "EXECUTE XoaNhaCC @MaNCC";
                cmd.Parameters.AddWithValue("@MaNCC", maNCC);

                conn.Open();
                int soDong = cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
                return soDong > 0;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL Error: " + ex.Message, "Lỗi SQL");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Lỗi");
                return false;
            }
        }

        // ==================== CÁC HÀM TIỆN ÍCH GIAO DIỆN ====================

        private bool KiemTraTenHopLe(string ten) => !string.IsNullOrWhiteSpace(ten);
        private bool KiemTraMaHopLe(string ma) => !string.IsNullOrWhiteSpace(ma);

        private bool KiemTraDauVao()
        {
            if (!KiemTraTenHopLe(txtTenNCC.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNCC.Focus();
                return false;
            }
            return true;
        }

        private NhaCungCap LayThongTinTuGiaoDien()
        {
            return new NhaCungCap(
                txtMaNCC.Text.Trim(),
                txtTenNCC.Text.Trim(),
                txtDiaChi.Text.Trim(),
                txtSoDT.Text.Trim(),
                txtMoTa.Text.Trim()
            );
        }

        // Hàm hiển thị danh sách lên DataGridView (dùng chung cho mọi nguồn)
        private void HienThiGridView(List<NhaCungCap> ds)
        {
            dgvNhaCungCap.DataSource = null;
            dgvNhaCungCap.DataSource = ds.ToList();
        }

        private void XoaTrangForm()
        {
            txtMaNCC.Clear();
            txtTenNCC.Clear();
            txtDiaChi.Clear();
            txtSoDT.Clear();
            txtMoTa.Clear();
            txtMaNCC.ReadOnly = false;
            txtMaNCC.Focus();
        }

        // ==================== SỰ KIỆN NÚT VÀ BẢNG ====================

        // Câu 5: Nút Mặc định
        private void btnMacDinh_Click(object sender, EventArgs e)
        {
            XoaTrangForm();
        }

        // Câu 6: Click dòng trên DataGridView đổ dữ liệu lên TextBox
        private void dgvNhaCungCap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvNhaCungCap.Rows.Count) return;

            var dong = dgvNhaCungCap.Rows[e.RowIndex];
            txtMaNCC.Text = dong.Cells["colNhaCC"].Value?.ToString();
            txtTenNCC.Text = dong.Cells["colTenNCC"].Value?.ToString();
            txtDiaChi.Text = dong.Cells["colDiaChi"].Value?.ToString();
            txtSoDT.Text = dong.Cells["colSoDT"].Value?.ToString();
            txtMoTa.Text = dong.Cells["colMoTa"].Value?.ToString();

            txtMaNCC.ReadOnly = true; // Khóa mã lại không cho sửa
        }

        // Câu 7: Nút Lưu (Thêm mới hoặc Cập nhật)
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!KiemTraDauVao()) return;

            if (txtMaNCC.ReadOnly && !string.IsNullOrWhiteSpace(txtMaNCC.Text))
            {
                ThucHienCapNhat();
            }
            else
            {
                ThucHienThemMoi();
            }
        }

        private void ThucHienCapNhat()
        {
            NhaCungCap ncc = LayThongTinTuGiaoDien();
            try
            {
                CapNhatNhaCCVaoDatabase(ncc);
                qlNCC.CapNhatNCC(ncc);
                HienThiGridView(qlNCC.DanhSach);
                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi cập nhật: " + ex.Message); }
        }

        private void ThucHienThemMoi()
        {
            if (!KiemTraMaHopLe(txtMaNCC.Text))
            {
                MessageBox.Show("Vui lòng nhập mã nhà cung cấp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNCC.Focus();
                return;
            }

            string ma = txtMaNCC.Text.Trim();
            if (qlNCC.KiemTraTonTai(ma))
            {
                MessageBox.Show("Mã nhà cung cấp đã tồn tại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNCC.Focus();
                return;
            }

            NhaCungCap ncc = LayThongTinTuGiaoDien();
            try
            {
                ThemNhaCCVaoDatabase(ncc);
                qlNCC.ThemNCC(ncc);
                HienThiGridView(qlNCC.DanhSach);
                MessageBox.Show("Thêm mới nhà cung cấp thành công!", "Thông báo");
                XoaTrangForm();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi thêm mới: " + ex.Message); }
        }

        // Tìm kiếm tức thời bằng LINQ
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(tuKhoa))
            {
                HienThiGridView(qlNCC.DanhSach);
                return;
            }

            var ketQua = rdTheoTen.Checked ? qlNCC.TimKiemTheoTen(tuKhoa) : qlNCC.TimKiemTheoSDT(tuKhoa);
            HienThiGridView(ketQua);
        }

        // Nút Xuất JSON
        private void btnXuatJSON_Click(object sender, EventArgs e)
        {
            ThucThiGhiFile(new DocGhiJSON(), "NhaCungCap.json", "Tập tin JSON (*.json)|*.json");
        }

        // ==================== CÁC SỰ KIỆN MENU ĐỌC / GHI FILE ====================

        private void msiDocTXT_Click(object sender, EventArgs e) => ThucThiDocFile(new DocGhiTXT(), "Tập tin Text (*.txt)|*.txt");
        private void msiDocJSON_Click(object sender, EventArgs e) => ThucThiDocFile(new DocGhiJSON(), "Tập tin JSON (*.json)|*.json");
        private void msiDocXML_Click(object sender, EventArgs e) => ThucThiDocFile(new DocGhiXML(), "Tập tin XML (*.xml)|*.xml");
        private void msiDocExcel_Click(object sender, EventArgs e) => ThucThiDocFile(new DocGhiExcel(), "Tập tin CSV (*.csv)|*.csv");

        private void msiGhiTXT_Click(object sender, EventArgs e) => ThucThiGhiFile(new DocGhiTXT(), "NhaCungCap.txt", "Tập tin Text (*.txt)|*.txt");
        private void msiGhiJSON_Click(object sender, EventArgs e) => ThucThiGhiFile(new DocGhiJSON(), "NhaCungCap.json", "Tập tin JSON (*.json)|*.json");
        private void msiGhiXML_Click(object sender, EventArgs e) => ThucThiGhiFile(new DocGhiXML(), "NhaCungCap.xml", "Tập tin XML (*.xml)|*.xml");
        private void msiGhiExcel_Click(object sender, EventArgs e) => ThucThiGhiFile(new DocGhiExcel(), "NhaCungCap.csv", "Tập tin CSV (*.csv)|*.csv");

        private void ThucThiDocFile(IDocGhiFile boDocGhi, string boLoc)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = boLoc, Title = "Chọn tập tin dữ liệu" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (qlNCC.DocTuFile(ofd.FileName, boDocGhi))
                    {
                        HienThiGridView(qlNCC.DanhSach);
                        MessageBox.Show("Đọc dữ liệu từ tập tin thành công!", "Thông báo");
                    }
                    else
                    {
                        MessageBox.Show("Tập tin không hợp lệ hoặc bị lỗi! Dữ liệu cũ được giữ nguyên.", "Cảnh báo");
                    }
                }
            }
        }

        private void ThucThiGhiFile(IDocGhiFile boDocGhi, string tenMacDinh, string boLoc)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { FileName = tenMacDinh, Filter = boLoc, Title = "Chọn nơi lưu tập tin" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    qlNCC.GhiRaFile(sfd.FileName, boDocGhi);
                    MessageBox.Show("Lưu tập tin thành công!", "Thông báo");
                }
            }
        }

        // ==================== CÂU 2: NHẤP ĐÚP CHỌN ĐỂ TRẢ VỀ CHO FORM KHÁC ====================

        private void dgvNhaCungCap_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvNhaCungCap.Rows.Count) return;

            // Chỉ lấy đúng giá trị cần truyền (Tên NCC)
            TenDuocChon = dgvNhaCungCap.Rows[e.RowIndex].Cells["colTenNCC"].Value?.ToString();

            this.DialogResult = DialogResult.OK; // Đánh dấu đã chọn thành công
            this.Close();                         // Đóng form
        }
    }
}
```

---

## BƯỚC 8: KẾT NỐI FORM 1 SANG FORM 2 (HÓA ĐƠN / BÁN HÀNG)

Theo yêu cầu ở **Câu 2 (Trang 2 đề thi)**:
- Trên Form 2 (`frmQuanLyBanHang` / `frmHoaDon`), ô hiển thị Khách hàng/Nhà cung cấp dùng `TextBox` và đặt `ReadOnly = True`.
- Cạnh ô đó có nút `+` (hoặc nút `...`). Khi người dùng bấm nút `+`, mở Form 1 lên bằng `ShowDialog()`. Khi người dùng nháy đúp chọn 1 nhà cung cấp, Form 1 đóng lại và trả về tên để điền vào ô Text:

```csharp
// Xử lý sự kiện click nút "+" trên Form 2:
private void btnThemNCC_Click(object sender, EventArgs e)
{
    using (frmQuanLyNhaCungCap frm = new frmQuanLyNhaCungCap())
    {
        // Mở Form con dạng Dialog
        if (frm.ShowDialog() == DialogResult.OK)
        {
            // Lấy đúng giá trị cần truyền và điền vào ô Text
            txtKhachHang.Text = frm.TenDuocChon;
        }
    }
}
```

---

## 10. BÍ KÍP & MẸO VÀNG KHI ĐI THI THỰC HÀNH

1. **Chuỗi kết nối SQL Server**:
   - Luôn nhớ đặt ký tự `@` phía trước: `@"server=...; database=...; Integrated Security=True; TrustServerCertificate=True"`.
   - Lấy tên Server: Mở SSMS lên copy từ ô **Server name**.
2. **Tránh lỗi cột bị nhân đôi trên DataGridView**:
   - Luôn gõ: `dgvNhaCungCap.AutoGenerateColumns = false;` trong `Form_Load`.
   - Thuộc tính `DataPropertyName` của từng cột trong Designer phải gõ chính xác 100% khớp với thuộc tính trong lớp C# (`MaNCC`, `TenNCC`, `DiaChi`, `SoDT`, `MoTa`).
3. **An toàn khi lấy dữ liệu từ DataGridView**:
   - Luôn kiểm tra `if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;` để không bị văng lỗi khi người dùng click vào thanh tiêu đề cột.
   - Luôn thêm dấu `?.` khi lấy text: `dong.Cells["colTenNCC"].Value?.ToString()`.
4. **Xử lý giá trị NULL trong SQL Server**:
   - Trong SQL, chuỗi rỗng của C# không phải là `null`. Nếu cột cho phép null, luôn dùng:
     `string.IsNullOrEmpty(ncc.DiaChi) ? (object)DBNull.Value : ncc.DiaChi`.
5. **Đọc/Ghi file không cần mạng**:
   - File JSON: Dùng `JavaScriptSerializer` trong reference `System.Web.Extensions`.
   - File XML: Dùng `XmlSerializer`.
   - File Excel: Dùng `StreamWriter` xuất định dạng CSV có UTF-8 (Excel mở lên trực tiếp thành các cột, không bao giờ bị lỗi thiếu thư viện Office COM).
