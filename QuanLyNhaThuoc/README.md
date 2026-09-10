<div align="center">

# 🏥 PHARMACARE PRO — HỆ THỐNG QUẢN LÝ NHÀ THUỐC & KHÁCH HÀNG THÂN THIẾT

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C# 12](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![WinForms](https://img.shields.io/badge/Platform-Windows_Desktop-0078D4?style=for-the-badge&logo=windows&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)
[![Build Status](https://img.shields.io/badge/Build-Passing-2ea44f?style=for-the-badge&logo=github-actions&logoColor=white)]()
[![License](https://img.shields.io/badge/License-MIT-orange?style=for-the-badge)](LICENSE)

<p align="center">
  <b>Hệ thống Quản lý Bán lẻ Nhà thuốc & Chương trình Tích điểm Gamified thế hệ mới</b><br>
  Tối ưu hóa quy trình bán hàng (POS), tự động hóa tìm kiếm đơn thuốc & chăm sóc khách hàng thân thiết.
</p>

---

[📖 Tính Năng](#-tính-năng-đột-phá) •
[🏗️ Kiến Trúc](#-kiến-trúc-hệ-thống) •
[⚙️ Luồng Xử Lý](#️-luồng-xử-lý--thuật-toán) •
[🚀 Cài Đặt](#-hướng-dẫn-cài-đặt) •
[🔮 Lộ Trình](#-lộ-trình-phát-triển)

</div>

<br>

---

## 💡 Tổng Quan Dự Án (Project Overview)

**QuanLyNhaThuoc (PharmaCare Pro)** là giải pháp phần mềm quản lý điểm bán hàng (POS) chuyên biệt cho các cơ sở kinh doanh dược phẩm, nhà thuốc tư nhân và chuỗi bán lẻ thuốc tây. Được xây dựng trên nền tảng **.NET 8.0 Desktop**, ứng dụng kết hợp giữa **tốc độ xử lý siêu nhanh** và **trải nghiệm người dùng thân thiện**, giải quyết triệt để các bài toán:

* ⚡ **Thanh toán nhanh chóng**: Giảm thiểu thời gian chờ đợi tại quầy thuốc với cơ chế tự động tính tiền và tự động đề xuất thuốc thông minh.
* 🎁 **Tích điểm & Giữ chân khách hàng**: Tích hợp chương trình Loyalty tự động chuyển đổi giá trị hóa đơn sang điểm thưởng (`1.000 VNĐ = 1 Điểm`).
* 🏆 **Bảng xếp hạng VIP (Gamification)**: Thúc đẩy mua hàng lặp lại thông qua hệ thống xếp hạng thành viên thân thiết thời gian thực.
* 🛡️ **Chuẩn hóa dữ liệu đầu vào**: Xác thực dữ liệu nghiêm ngặt (Số điện thoại chuẩn Việt Nam, Họ tên hợp lệ) bằng biểu thức chính quy (Regex Engine).

---

## 🚀 Tính Năng Đột Phá (Core Features)

| Biểu Tượng | Tính Năng | Mô Tả Chi Tiết |
| :---: | :--- | :--- |
| 💊 | **Tra Cứu Thuốc Real-Time** | Tìm kiếm thuốc theo dạng Auto-Suggest (Gợi ý ngay khi gõ), hỗ trợ danh mục dược phẩm phong phú (Kháng sinh, Thần kinh, Vitamin, Hạ sốt...). |
| 🛒 | **Giỏ Hàng & Thanh Toán Siêu Tốc** | Thêm thuốc vào giỏ hàng linh hoạt, tự động tổng hợp tổng tiền hóa đơn (`N0 Format`) và quy đổi điểm tích lũy ngay tức thì. |
| 🔍 | **Truy Xuất Khách Hàng Thông Minh** | Tìm kiếm khách hàng đa tiêu chí theo SĐT hoặc Họ tên với độ chính xác cao. |
| 📝 | **Đăng Ký Thành Viên Tự Động** | Khi thanh toán cho khách hàng mới, hệ thống tự động phát hiện và gợi ý tạo tài khoản thành viên chỉ với 1 cú click. |
| 🔒 | **Xác Thực Dữ Liệu An Toàn** | Kiểm soát cú pháp SĐT chuẩn Việt Nam (`^0\d{9}$`), lọc ký tự đặc biệt trong Họ tên (`^[\p{L}\s]+$`), chống trùng lặp dữ liệu. |
| 🏆 | **Bảng Xếp Hạng Khách Hàng (VIP Leaderboard)** | Tự động sắp xếp và hiển thị TOP khách hàng thân thiết theo số điểm tích lũy (`OrderByDescending`). |

---

## 🏗️ Kiến Trúc Hệ Thống (Architecture)

Ứng dụng được thiết kế theo hướng **Modularity & Clean Logic Pattern**, tách biệt rõ ràng giữa mô hình dữ liệu (Data Models), giao diện người dùng (UI Event Handlers), và bộ máy xử lý (In-Memory Processing Engine).

```mermaid
flowchart TD
    subgraph UI_Layer ["📱 Giao Diện Người Dùng (Presentation Layer)"]
        F1["Form1 (WinForms View)"]
        MB["MessageBox & Notification Dialogs"]
    end

    subgraph Controller_Layer ["⚙️ Bộ Máy Xử Lý Trung Tâm (Core Engine)"]
        POS["POS & Checkout Manager"]
        CRM["Customer & Loyalty Manager"]
        INV["Inventory Search Engine"]
    end

    subgraph Validation_Layer ["🛡️ Kiểm Soát Dữ Liệu (Validation Engine)"]
        REG1["Phone Format Regex Validator"]
        REG2["Name Standard Regex Validator"]
    end

    subgraph Data_Layer ["💾 Mô Hình Dữ Liệu (Domain Models & State)"]
        M1["Medicine (Name, Price)"]
        C1["Customer (Name, NumPhone, Point)"]
        ST["In-Memory State Manager (LINQ Engine)"]
    end

    F1 --> POS
    F1 --> CRM
    F1 --> INV
    
    CRM --> REG1
    CRM --> REG2
    
    POS --> ST
    CRM --> ST
    INV --> ST
    
    ST --> M1
    ST --> C1

    classDef ui fill:#0078D4,stroke:#004578,stroke-width:2px,color:#fff;
    classDef logic fill:#239120,stroke:#107C41,stroke-width:2px,color:#fff;
    classDef model fill:#512BD4,stroke:#3B1FA7,stroke-width:2px,color:#fff;
    classDef validation fill:#D83B01,stroke:#A80000,stroke-width:2px,color:#fff;

    class F1,MB ui;
    class POS,CRM,INV logic;
    class REG1,REG2 validation;
    class M1,C1,ST model;
```

---

## ⚙️ Luồng Xử Lý & Thuật Toán (Core Logic & Workflows)

### 1. Luồng Thanh Toán & Tích Điểm (POS & Loyalty Sequence)

```mermaid
sequenceDiagram
    autonumber
    actor NV as Thu Ngân (Cashier)
    participant UI as Form1 UI
    participant Logic as Business Logic Engine
    participant DB as In-Memory State

    NV->>UI: Chọn thuốc & Nhập SĐT/Họ tên
    UI->>Logic: Thêm thuốc vào danh sách mua (`buyings`)
    Logic->>UI: Cập nhật tổng tiền & Điểm quy đổi dự kiến
    NV->>UI: Nhấn "Thanh toán" (`calc_Click`)
    Logic->>Logic: Kiểm tra điều kiện đầu vào (Hợp lệ / Không rỗng)
    Logic->>DB: Tìm kiếm SĐT trong danh sách Khách hàng
    alt Khách hàng đã tồn tại
        DB-->>Logic: Trả về Customer Object
        Logic->>Logic: Cộng điểm (`Point += TotalMoney / 1000`)
        Logic->>UI: Thông báo thanh toán thành công & Điểm mới
    else Khách hàng chưa có trong hệ thống
        DB-->>Logic: null
        Logic->>UI: Hiển thị Dialog gợi ý Đăng ký mới?
        opt Đồng ý đăng ký
            UI->>Logic: Khởi tạo `Customer` mới với điểm tích lũy
            Logic->>DB: Thêm vào `customers` List
        end
    end
    Logic->>UI: Cập nhật DataGridView & Bảng xếp hạng (`LoadLeaderboard`)
    Logic->>UI: Xóa thông tin đơn hàng hiện tại (`ClearBill`)
```

### 2. Thuật Toán Tích Điểm & Định Dạng Tiền Tệ
* **Công thức tích điểm**: 
  $$\text{Points} = \left\lfloor \frac{\text{Tổng tiền hóa đơn (VNĐ)}}{1000} \right\rfloor$$
* **Biểu thức chính quy (Regex Validation)**:
  - **Số điện thoại**: `^0\d{9}$` (Bắt buộc bắt đầu bằng 0, độ dài chính xác 10 chữ số).
  - **Họ tên**: `^[\p{L}\s]+$` (Chấp nhận tất cả ký tự tiếng Việt có dấu và khoảng trắng, chặn tuyệt đối ký tự đặc biệt & số).

---

## 📂 Cấu Trúc Thư Mục Project (Project Structure)

```text
QuanLyNhaThuoc/
├── 📄 QuanLyNhaThuoc.sln            # Visual Studio Solution File
├── 📁 QuanLyNhaThuoc/                # Core Project Source Directory
│   ├── 📄 Program.cs                # Main Application Entry Point
│   ├── 📄 Form1.cs                  # Main Form Business Logic & Event Handlers
│   ├── 📄 Form1.Designer.cs         # Auto-generated UI Layout Components
│   ├── 📄 Form1.resx                # GUI Form Resource File
│   ├── 📄 Customer.cs               # Customer Domain Model (Name, Phone, Point)
│   ├── 📄 Medicine.cs               # Medicine Domain Model (Name, Price)
│   └── 📄 QuanLyNhaThuoc.csproj     # .NET 8.0 C# Project Configuration
└── 📄 README.md                     # Comprehensive Technical Documentation
```

---

## 💻 Hướng Dẫn Cài Đặt (Getting Started)

### Yêu Cầu Tiền Đề (Prerequisites)
* **Hệ điều hành**: Windows 10 / Windows 11 (64-bit).
* **SDK**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) trở lên.
* **IDE Khuyên dùng**: Visual Studio 2022 (v17.8+) hoặc JetBrains Rider 2023.3+ (với Workload **.NET Desktop Development**).

### Các Bước Cài Đặt & Chạy Ứng Dụng

1. **Clone repository về máy local**:
   ```bash
   git clone https://github.com/your-username/QuanLyNhaThuoc.git
   cd QuanLyNhaThuoc
   ```

2. **Khôi phục các gói phụ thuộc (Restore dependencies)**:
   ```bash
   dotnet restore
   ```

3. **Biên dịch & Phát triển (Build Project)**:
   ```bash
   dotnet build --configuration Release
   ```

4. **Khởi chạy ứng dụng (Run App)**:
   ```bash
   dotnet run --project QuanLyNhaThuoc/QuanLyNhaThuoc.csproj
   ```

---

## 🔮 Lộ Trình Phát Triển (Roadmap)

- [x] **Giai đoạn 1**: Hoàn thiện UI/UX chuẩn POS, quản lý khách hàng & cơ chế tích điểm căn bản.
- [ ] **Giai đoạn 2**: Tích hợp cơ sở dữ liệu quan hệ **SQL Server / PostgreSQL** với **Entity Framework Core 8**.
- [ ] **Giai đoạn 3**: Tích hợp in hóa đơn nhiệt **POS Thermal Printer (80mm)** qua cổng USB/LAN.
- [ ] **Giai đoạn 4**: Tích hợp cổng thanh toán mã **VietQR (Napas247)** tự động tạo mã QR chuyển khoản theo số tiền hóa đơn.
- [ ] **Giai đoạn 5**: Quản lý hạn sử dụng (Expiry Date) & cảnh báo lô thuốc sắp hết hạn (FEFO/FIFO).
<div align="center">

⭐ **If you find this project useful, please consider giving it a star!** ⭐

Made with ❤️ & C# .NET 8.0 by **PTUDD Team**

</div>
