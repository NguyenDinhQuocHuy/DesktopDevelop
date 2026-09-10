using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace QuanLyDichVuKhachSan.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class FoodItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ItemCode => $"MH{Id:D3}";

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Price)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfitMargin)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfitPercentage)));
                }
            }
        }

        private string _category = "Đồ ăn";
        public string Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Category)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDrink)));
                }
            }
        }

        private string _retailUnit = "Cái";
        public string RetailUnit
        {
            get => _retailUnit;
            set
            {
                if (_retailUnit != value)
                {
                    _retailUnit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RetailUnit)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StockDisplay)));
                }
            }
        }

        private string _defaultImportUnit = "Thùng";
        public string DefaultImportUnit
        {
            get => _defaultImportUnit;
            set
            {
                if (_defaultImportUnit != value)
                {
                    _defaultImportUnit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultImportUnit)));
                }
            }
        }

        private int _defaultConversionRate = 24;
        public int DefaultConversionRate
        {
            get => _defaultConversionRate;
            set
            {
                if (_defaultConversionRate != value)
                {
                    _defaultConversionRate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultConversionRate)));
                }
            }
        }
        public decimal AverageCostPrice { get; set; } = 0;     // GiaVonBinhQuan (tính theo bình quân gia quyền từ trigger)
        public int StockQuantity { get; set; }                 // SoLuongTonKho
        public int LowStockThreshold { get; set; } = 10;       // NguongCanhBaoTon
        public string ImageUrl { get; set; } = string.Empty;
        private string _status = "Đang phục vụ";
        public string Status
        {
            get => (_status == "Còn hàng" || _status == "Đang phục vụ") ? "Đang phục vụ" : "Ngừng kinh doanh";
            set
            {
                string norm = (value == "Còn hàng" || value == "Đang phục vụ") ? "Đang phục vụ" : "Ngừng kinh doanh";
                if (_status != norm)
                {
                    _status = norm;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailable)));
                }
            }
        }
 
        public bool IsActive
        {
            get => Status == "Đang phục vụ" && StockQuantity > 0;
            set
            {
                string newStatus = value ? "Đang phục vụ" : "Ngừng kinh doanh";
                if (Status != newStatus)
                {
                    Status = newStatus;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailable)));
                    // Đồng bộ xuống database
                    _ = Services.DatabaseService.Instance.ExecuteNonQueryAsync(
                        "UPDATE MonAn SET TrangThai = @Status WHERE Id = @Id",
                        new Microsoft.Data.SqlClient.SqlParameter("@Status", Status),
                        new Microsoft.Data.SqlClient.SqlParameter("@Id", Id)
                    );
                }
            }
        }

        private int _selectedQty = 1;
        public int SelectedQty
        {
            get => _selectedQty;
            set
            {
                if (_selectedQty != value)
                {
                    _selectedQty = Math.Max(1, value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedQty)));
                }
            }
        }

        public bool IsLowStock => Status == "Đang phục vụ" && StockQuantity <= LowStockThreshold;
        public bool IsAvailable => StockQuantity > 0 && Status == "Đang phục vụ";
        public string StockDisplay => $"{StockQuantity} {RetailUnit}";
        public decimal ProfitMargin => Price > 0 ? (Price - AverageCostPrice) : 0;
        public double ProfitPercentage => Price > 0 ? (double)((Price - AverageCostPrice) / Price * 100) : 0;

        public bool IsDrink
        {
            get
            {
                string cat = (Category ?? "").ToLowerInvariant();
                if (cat.Contains("uống") || cat.Contains("nước") || cat.Contains("rượu") || cat.Contains("bia") || cat.Contains("giải khát") || cat.Contains("drink") || cat.Contains("beverage"))
                {
                    return true;
                }
                if (cat.Contains("ăn") || cat.Contains("bánh") || cat.Contains("kẹo") || cat.Contains("mứt") || cat.Contains("hạt") || cat.Contains("snack") || cat.Contains("mì") || cat.Contains("đặc sản") || cat.Contains("food"))
                {
                    return false;
                }
                string n = (Name ?? "").ToLowerInvariant();
                return n.Contains("lavie") || n.Contains("khoáng") || n.Contains("suối") ||
                       n.Contains("coca") || n.Contains("pepsi") || n.Contains("nước") ||
                       n.Contains("cà phê") || n.Contains("cafe") || n.Contains("g7") ||
                       n.Contains("trà") || n.Contains("atiso") || n.Contains("tea") ||
                       n.Contains("rượu") || n.Contains("vang") || n.Contains("bia") ||
                       n.Contains("sinh tố") || n.Contains("nước ép") || n.Contains("sữa");
            }
        }

        private bool _isSearchMatched = false;
        public bool IsSearchMatched
        {
            get => _isSearchMatched;
            set
            {
                if (_isSearchMatched != value)
                {
                    _isSearchMatched = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchMatched)));
                }
            }
        }

        public string DisplayIcon
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ImageUrl) && !ImageUrl.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) && !ImageUrl.EndsWith(".png", StringComparison.OrdinalIgnoreCase) && ImageUrl.Length <= 4)
                {
                    return ImageUrl;
                }
                return GetDefaultIcon(Name);
            }
        }

        public static string GetDefaultIcon(string name)
        {
            string n = (name ?? "").ToLower();
            if (n.Contains("lavie") || n.Contains("nước khoáng") || n.Contains("suối")) return "💧";
            if (n.Contains("coca") || n.Contains("nước ngọt") || n.Contains("nước ép") || n.Contains("sinh tố")) return "🥤";
            if (n.Contains("cà phê") || n.Contains("cafe") || n.Contains("g7")) return "☕";
            if (n.Contains("trà") || n.Contains("atiso") || n.Contains("tea")) return "🍵";
            if (n.Contains("snack") || n.Contains("khoai tây") || n.Contains("bánh") || n.Contains("lay")) return "🍿";
            if (n.Contains("rượu") || n.Contains("vang") || n.Contains("bia")) return "🍷";
            if (n.Contains("mứt") || n.Contains("dâu") || n.Contains("trái cây")) return "🍓";
            if (n.Contains("omachi") || n.Contains("mì")) return "🍜";
            return "📦";
        }

        // Thùng rác: Soft-delete
        public DateTime? TrashedDate { get; set; }
        public bool IsTrashed => TrashedDate.HasValue;
        public string TrashedDateDisplay => TrashedDate.HasValue 
            ? $"Xóa lúc: {TrashedDate.Value:dd/MM/yyyy HH:mm} • Hết hạn khôi phục: {TrashedDate.Value.AddMonths(3):dd/MM/yyyy}" 
            : "";

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyStockChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StockQuantity)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AverageCostPrice)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLowStock)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StockDisplay)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RetailUnit)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultImportUnit)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultConversionRate)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfitMargin)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfitPercentage)));
        }
    }

    public class InventoryBatch
    {
        public int Id { get; set; }
        public int ReceiptId { get; set; }
        public string ReceiptCode { get; set; } = string.Empty; // Mã phiếu nhập (VD: PN-20260831-01)
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; } = string.Empty;
        public string RetailUnit { get; set; } = "Cái";
        public string ImportUnit { get; set; } = "Thùng";        // ĐVT lúc nhập: Thùng, Lốc, Hộp, Cái...
        public int ConversionRate { get; set; } = 1;             // Hệ số quy đổi ra đơn vị lẻ (VD: 1 Thùng = 24 Lon)
        public int Quantity { get; set; } = 1;                   // Số lượng theo ĐVT nhập (VD: 5 Thùng)
        public decimal ImportPrice { get; set; }                 // Đơn giá theo ĐVT nhập (VD: 240,000 đ/Thùng)
        public decimal TotalCost => Quantity * ImportPrice;      // Tổng tiền nhập = Số lượng x Đơn giá
        public int TotalRetailUnits => Quantity * ConversionRate;// Tổng số lượng đơn vị bán lẻ cộng vào kho (5 x 24 = 120 lon)
        public string BatchNumber { get; set; } = string.Empty;  // Số lô bao bì
        public DateTime? ExpiryDate { get; set; }                // Hạn sử dụng (FIFO)
        public DateTime ImportDate { get; set; } = DateTime.Now;
        public string ImportedBy { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        public string CalculationSummary => $"{Quantity} {ImportUnit} x {ConversionRate} = {TotalRetailUnits} {RetailUnit}";
        public string ExpiryDisplay => ExpiryDate.HasValue ? ExpiryDate.Value.ToString("dd/MM/yyyy") : "---";
        public bool IsNearExpiry => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.Today.AddDays(7);
    }

    public class WarehouseReceiptDraftItem : INotifyPropertyChanged
    {
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; } = string.Empty;
        public string RetailUnit { get; set; } = "Cái";
        public string ImportUnit { get; set; } = "Thùng";
        public int ConversionRate { get; set; } = 24;
        public int Quantity { get; set; } = 1;
        public decimal ImportPrice { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; } = DateTime.Today.AddMonths(6);
        public string Note { get; set; } = string.Empty;

        public int TotalRetailUnits => Quantity * ConversionRate;
        public decimal TotalCost => Quantity * ImportPrice;
        public decimal CostPerRetailUnit => ConversionRate > 0 ? (ImportPrice / ConversionRate) : 0;
        public string CalculationSummary => $"{Quantity} {ImportUnit} x {ConversionRate} = {TotalRetailUnits} {RetailUnit}";
        public string ExpiryDisplay => ExpiryDate.HasValue ? ExpiryDate.Value.ToString("dd/MM/yyyy") : "---";

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyValuesChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImportPrice)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConversionRate)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalRetailUnits)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalCost)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CostPerRetailUnit)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalculationSummary)));
        }
    }

    public class InventoryProfitReportItem
    {
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public double ProfitMarginPercentage => Revenue > 0 ? (double)(GrossProfit / Revenue * 100) : 0;
    }

    public class FoodOrderItem : INotifyPropertyChanged
    {
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; } = string.Empty;
        public string RetailUnit { get; set; } = "Cái";

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalPrice)));
                }
            }
        }

        public decimal Price { get; set; }
        public decimal CostPrice { get; set; } // GiaVon tại thời điểm bán
        public decimal TotalPrice => Quantity * Price;
        public decimal TotalCost => Quantity * CostPrice;
        public decimal Profit => TotalPrice - TotalCost;

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class FoodOrder
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public int? KhachHangId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public FoodPaymentType PaymentType { get; set; }
        public List<FoodOrderItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public bool DaThanhToan { get; set; }
        public string PaymentMethod { get; set; } = "Tiền mặt";
        public string Note { get; set; } = string.Empty;
        
        // Khóa ngoại nhân viên theo DB
        public int NguoiTaoId { get; set; } = 1;
        public int? NguoiThanhToanId { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; }

        public string RecordedBy { get; set; } = string.Empty;
        public string PaidByStaffName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Chờ xử lý";

        public string PaymentTypeDisplay => PaymentType == FoodPaymentType.DirectPayment 
            ? "Thanh toán trực tiếp (Thu tiền ngay)" 
            : "Ghi nợ vào phòng (Tính khi Check-out)";
    }

    public class EventSpace : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(DisplayIcon));
                }
            }
        }

        private string _spaceType = string.Empty; // Sảnh tiệc, Phòng họp VIP, Sân thượng Sky Lounge
        public string SpaceType
        {
            get => _spaceType;
            set
            {
                if (_spaceType != value)
                {
                    _spaceType = value;
                    OnPropertyChanged(nameof(SpaceType));
                }
            }
        }

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (_capacity != value)
                {
                    _capacity = value;
                    OnPropertyChanged(nameof(Capacity));
                }
            }
        }

        private decimal _hourlyRate;
        public decimal HourlyRate
        {
            get => _hourlyRate;
            set
            {
                if (_hourlyRate != value)
                {
                    _hourlyRate = value;
                    OnPropertyChanged(nameof(HourlyRate));
                }
            }
        }

        private string _equipments = string.Empty;
        public string Equipments
        {
            get => _equipments;
            set
            {
                if (_equipments != value)
                {
                    _equipments = value;
                    OnPropertyChanged(nameof(Equipments));
                }
            }
        }

        private SpaceStatus _status = SpaceStatus.Available;
        public SpaceStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(StatusDisplay));
                    OnPropertyChanged(nameof(StatusBadgeBg));
                    OnPropertyChanged(nameof(StatusBadgeFg));
                    OnPropertyChanged(nameof(IsAvailableForRent));
                }
            }
        }

        public string ImageUrl { get; set; } = string.Empty;

        public string DisplayIcon
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ImageUrl) && !ImageUrl.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) && !ImageUrl.EndsWith(".png", StringComparison.OrdinalIgnoreCase) && ImageUrl.Length <= 4)
                {
                    return ImageUrl;
                }
                string n = (Name ?? "").ToLower();
                if (n.Contains("diamond") || n.Contains("đại tiệc")) return "🏛️";
                if (n.Contains("ruby") || n.Contains("hội nghị")) return "🎙️";
                if (n.Contains("sky") || n.Contains("lounge") || n.Contains("sunset")) return "🌅";
                if (n.Contains("zen") || n.Contains("trà") || n.Contains("garden")) return "🍵";
                return "🏛️";
            }
        }

        public string StatusDisplay => Status switch
        {
            SpaceStatus.Available => "Trống / Sẵn sàng",
            SpaceStatus.Rented => "Đang cho thuê",
            SpaceStatus.Maintenance => "Đang sửa chữa",
            SpaceStatus.Cleaning => "Đang dọn dẹp",
            _ => "Trống / Sẵn sàng"
        };

        public string StatusBadgeBg => Status switch
        {
            SpaceStatus.Available => "#DCFCE7",     // Green bg
            SpaceStatus.Rented => "#FEF3C7",        // Amber bg
            SpaceStatus.Maintenance => "#FEE2E2",   // Red bg
            SpaceStatus.Cleaning => "#FEF9C3",      // Yellow bg
            _ => "#F1F5F9"
        };

        public string StatusBadgeFg => Status switch
        {
            SpaceStatus.Available => "#15803D",     // Green text
            SpaceStatus.Rented => "#B45309",        // Amber text
            SpaceStatus.Maintenance => "#DC2626",   // Red text
            SpaceStatus.Cleaning => "#A16207",      // Yellow text
            _ => "#475569"
        };

        public bool IsAvailableForRent => Status != SpaceStatus.Maintenance;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class EventBooking : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public int Id { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public int SpaceId { get; set; }
        public string SpaceName { get; set; } = string.Empty;
        public int? KhachHangId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        private decimal _depositAmount;
        public decimal DepositAmount 
        { 
            get => _depositAmount; 
            set { _depositAmount = value; OnPropertyChanged(nameof(DepositAmount)); OnPropertyChanged(nameof(RemainingPayment)); } 
        }

        private decimal _totalEstimatedAmount;
        public decimal TotalEstimatedAmount 
        { 
            get => _totalEstimatedAmount; 
            set { _totalEstimatedAmount = value; OnPropertyChanged(nameof(TotalEstimatedAmount)); OnPropertyChanged(nameof(FinalTotal)); OnPropertyChanged(nameof(RemainingPayment)); } 
        }

        private decimal _additionalCost;
        public decimal AdditionalCost 
        { 
            get => _additionalCost; 
            set { _additionalCost = value; OnPropertyChanged(nameof(AdditionalCost)); OnPropertyChanged(nameof(FinalTotal)); OnPropertyChanged(nameof(RemainingPayment)); } 
        }

        private string _damageNote = string.Empty;
        public string DamageNote 
        { 
            get => _damageNote; 
            set { _damageNote = value; OnPropertyChanged(nameof(DamageNote)); } 
        }

        private bool _daThanhToan;
        public bool DaThanhToan 
        { 
            get => _daThanhToan; 
            set { _daThanhToan = value; OnPropertyChanged(nameof(DaThanhToan)); } 
        }

        private string _paymentStatus = "Đã cọc";
        public string PaymentStatus 
        { 
            get => _paymentStatus; 
            set { _paymentStatus = value; OnPropertyChanged(nameof(PaymentStatus)); } 
        }

        private string _paymentMethod = "Tiền mặt";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(nameof(PaymentMethod)); }
        }

        private string _initialPaymentType = "Đặt cọc trước";
        public string InitialPaymentType 
        { 
            get => _initialPaymentType; 
            set { _initialPaymentType = value; OnPropertyChanged(nameof(InitialPaymentType)); } 
        }

        private string _orderStatus = "Đã đặt";
        public string OrderStatus 
        { 
            get => _orderStatus; 
            set { _orderStatus = value; OnPropertyChanged(nameof(OrderStatus)); } 
        }

        public int NguoiTaoId { get; set; } = 1;
        public int? NguoiThanhToanId { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; }

        public string RecordedBy { get; set; } = string.Empty;
        public string PaidByStaffName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal FinalTotal => TotalEstimatedAmount + AdditionalCost;
        public decimal RemainingPayment => Math.Max(0, FinalTotal - DepositAmount);
        public string BookingDisplay => $"{BookingCode} · {SpaceName} · Khách: {CustomerName} {(string.IsNullOrWhiteSpace(RoomNumber) ? "" : "(P." + RoomNumber + ")")}";
    }

    public class Vehicle : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _licensePlate = string.Empty;
        public string LicensePlate
        {
            get => _licensePlate;
            set
            {
                if (_licensePlate != value)
                {
                    _licensePlate = value;
                    OnPropertyChanged(nameof(LicensePlate));
                }
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(DisplayIcon));
                }
            }
        }

        private VehicleType _type;
        public VehicleType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                    OnPropertyChanged(nameof(TypeDisplay));
                    OnPropertyChanged(nameof(DisplayIcon));
                }
            }
        }

        private decimal _dailyRate;
        public decimal DailyRate
        {
            get => _dailyRate;
            set
            {
                if (_dailyRate != value)
                {
                    _dailyRate = value;
                    OnPropertyChanged(nameof(DailyRate));
                }
            }
        }

        private decimal _requiredDeposit;
        public decimal RequiredDeposit
        {
            get => _requiredDeposit;
            set
            {
                if (_requiredDeposit != value)
                {
                    _requiredDeposit = value;
                    OnPropertyChanged(nameof(RequiredDeposit));
                }
            }
        }

        private VehicleStatus _status = VehicleStatus.Available;
        public VehicleStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(StatusDisplay));
                    OnPropertyChanged(nameof(IsAvailable));
                    OnPropertyChanged(nameof(IsMaintenance));
                    OnPropertyChanged(nameof(IsRented));
                }
            }
        }

        private string _conditionNote = string.Empty;
        public string ConditionNote
        {
            get => _conditionNote;
            set
            {
                if (_conditionNote != value)
                {
                    _conditionNote = value;
                    OnPropertyChanged(nameof(ConditionNote));
                }
            }
        }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsAvailable => Status == VehicleStatus.Available;
        public bool IsMaintenance => Status == VehicleStatus.Maintenance;
        public bool IsRented => Status == VehicleStatus.Rented;

        public string DisplayIcon
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ImageUrl) && !ImageUrl.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) && !ImageUrl.EndsWith(".png", StringComparison.OrdinalIgnoreCase) && ImageUrl.Length <= 4)
                {
                    return ImageUrl;
                }
                string n = (Name ?? "").ToLower();
                if (n.Contains("sh")) return "🛵";
                if (n.Contains("air blade") || n.Contains("ab")) return "🛵";
                if (n.Contains("vision")) return "🛵";
                if (n.Contains("lead")) return "🛵";
                if (n.Contains("winner") || n.Contains("exciter")) return "🏍️";
                if (n.Contains("wave") || n.Contains("sirius") || n.Contains("future")) return "🛵";
                if (Type == VehicleType.Car) return "🚗";
                return "🛵";
            }
        }

        public string StatusDisplay => Status switch
        {
            VehicleStatus.Available => "Sẵn sàng",
            VehicleStatus.Rented => "Đang thuê",
            VehicleStatus.Maintenance => "Bảo trì",
            _ => "Khác"
        };

        public string TypeDisplay => Type switch
        {
            VehicleType.Scooter => "Xe tay ga",
            VehicleType.Manual => "Xe số",
            VehicleType.Clutch => "Xe côn tay",
            VehicleType.Car => "Ô tô",
            _ => "Khác"
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class VehicleRental : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public int Id { get; set; }
        public string RentalCode { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public int? KhachHangId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime RentalDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }

        private DateTime? _actualReturnDate;
        public DateTime? ActualReturnDate 
        { 
            get => _actualReturnDate; 
            set { _actualReturnDate = value; OnPropertyChanged(nameof(ActualReturnDate)); OnPropertyChanged(nameof(DaTraXe)); } 
        }

        private decimal _depositAmount;
        public decimal DepositAmount 
        { 
            get => _depositAmount; 
            set { _depositAmount = value; OnPropertyChanged(nameof(DepositAmount)); } 
        }

        private decimal _rentalFee;
        public decimal RentalFee 
        { 
            get => _rentalFee; 
            set { _rentalFee = value; OnPropertyChanged(nameof(RentalFee)); } 
        }

        private decimal _additionalCost;
        public decimal AdditionalCost 
        { 
            get => _additionalCost; 
            set { _additionalCost = value; OnPropertyChanged(nameof(AdditionalCost)); } 
        }

        private decimal _totalPayment;
        public decimal TotalPayment 
        { 
            get => _totalPayment; 
            set { _totalPayment = value; OnPropertyChanged(nameof(TotalPayment)); } 
        }

        private bool _daThanhToan;
        public bool DaThanhToan 
        { 
            get => _daThanhToan; 
            set { _daThanhToan = value; OnPropertyChanged(nameof(DaThanhToan)); } 
        }

        public string ReceptionNote { get; set; } = string.Empty; // Ghi chú khi nhận xe / yêu cầu khách

        private string _damageNote = string.Empty;
        public string DamageNote 
        { 
            get => _damageNote; 
            set { _damageNote = value; OnPropertyChanged(nameof(DamageNote)); } 
        }

        private string _status = "Đang thuê";
        public string Status 
        { 
            get => _status; 
            set { _status = value; OnPropertyChanged(nameof(Status)); OnPropertyChanged(nameof(DaTraXe)); OnPropertyChanged(nameof(IsOverdue)); } 
        }

        private string _paymentMethod = "Tiền mặt";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(nameof(PaymentMethod)); }
        }

        private string _initialPaymentType = "Đặt cọc trước";
        public string InitialPaymentType 
        { 
            get => _initialPaymentType; 
            set { _initialPaymentType = value; OnPropertyChanged(nameof(InitialPaymentType)); } 
        }

        private string _orderStatus = "Đang thuê";
        public string OrderStatus 
        { 
            get => _orderStatus; 
            set { _orderStatus = value; OnPropertyChanged(nameof(OrderStatus)); OnPropertyChanged(nameof(DaTraXe)); } 
        }

        private string _paymentStatus = "Đang thuê";
        public string PaymentStatus
        {
            get => _paymentStatus;
            set { _paymentStatus = value; OnPropertyChanged(nameof(PaymentStatus)); OnPropertyChanged(nameof(DaTraXe)); }
        }

        public int NguoiTaoId { get; set; } = 1;
        public int? NguoiThanhToanId { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; }

        public string RecordedBy { get; set; } = string.Empty;
        public string PaidByStaffName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal RemainingPayment => Math.Max(0, (TotalPayment > 0 ? TotalPayment : (RentalFee + AdditionalCost)) - DepositAmount);
        public bool DaTraXe => ActualReturnDate.HasValue || Status == "Hoàn tất" || OrderStatus == "Hoàn tất" || Status == "Ghi nợ vào phòng" || PaymentStatus == "Ghi nợ vào phòng";
        public bool IsOverdue => Status == "Đang thuê" && DateTime.Now > ExpectedReturnDate;
        public string RentalDisplay => $"{RentalCode} · {VehicleName} ({LicensePlate}) · Khách: {CustomerName} {(string.IsNullOrWhiteSpace(RoomNumber) ? "" : "(P." + RoomNumber + ")")}";
    }

    public class ParkingRecord
    {
        public int Id { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string VehicleType { get; set; } = "Xe máy";
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; } = DateTime.Now;
        public DateTime? CheckOutTime { get; set; }
        public ParkingChargeType ChargeType { get; set; }
        public decimal ParkingFee { get; set; }
        public bool DaThanhToan { get; set; }
        public bool IsPaid { get => DaThanhToan; set => DaThanhToan = value; }
        public string RepairNote { get; set; } = string.Empty; // Ghi chú hư hại / sửa chữa bãi

        public int NguoiTaoId { get; set; } = 1;
        public int? NguoiThanhToanId { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; }

        public string RecordedBy { get; set; } = string.Empty;
        public string PaidByStaffName { get; set; } = string.Empty;

        public string ChargeTypeDisplay
        {
            get
            {
                if (ChargeType == ParkingChargeType.ResidentFree || !string.IsNullOrWhiteSpace(RoomNumber))
                    return "Khách phòng (Miễn phí)";
                if (VehicleType == "Xe máy" || ChargeType == ParkingChargeType.ByTurn)
                    return "Xe máy (5.000 đ/lượt)";
                return "Ô tô (50.000 đ/giờ)";
            }
            set { }
        }

        public string GetDurationDisplay(DateTime? outTime = null)
        {
            var end = outTime ?? CheckOutTime ?? DateTime.Now;
            var span = end > CheckInTime ? (end - CheckInTime) : TimeSpan.Zero;
            int totalHours = (int)span.TotalHours;
            int mins = span.Minutes;
            if (totalHours == 0 && mins == 0) return "Dưới 1 phút";
            if (totalHours == 0) return $"{mins} phút";
            if (mins == 0) return $"{totalHours} giờ";
            return $"{totalHours} giờ {mins} phút";
        }

        public decimal CalculateRealtimeFee(DateTime? outTime = null)
        {
            if (ChargeType == ParkingChargeType.ResidentFree || !string.IsNullOrWhiteSpace(RoomNumber)) return 0;
            if (VehicleType == "Xe máy" || ChargeType == ParkingChargeType.ByTurn) return 5000;
            
            var end = outTime ?? CheckOutTime ?? DateTime.Now;
            var duration = end - CheckInTime;
            double hours = Math.Max(1.0, Math.Ceiling(duration.TotalMinutes / 60.0));
            return (decimal)hours * 50000; // Ô tô 50k/giờ
        }
    }

    public class LaundryPartner : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _address = string.Empty;
        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        private decimal _hotelCommissionRate = 30.0m;
        public decimal HotelCommissionRate
        {
            get => _hotelCommissionRate;
            set
            {
                if (_hotelCommissionRate != value)
                {
                    _hotelCommissionRate = value;
                    OnPropertyChanged(nameof(HotelCommissionRate));
                    OnPropertyChanged(nameof(PartnerRate));
                }
            }
        }

        public decimal PartnerRate => 100.0m - HotelCommissionRate;

        // Bảng giá vốn theo từng dịch vụ mà đối tác báo giá (Bắt buộc nhỏ hơn giá niêm yết của KS)
        private decimal _giaVonGiatSay = 21000m;
        public decimal GiaVonGiatSay
        {
            get => _giaVonGiatSay;
            set
            {
                if (_giaVonGiatSay != value)
                {
                    _giaVonGiatSay = value;
                    OnPropertyChanged(nameof(GiaVonGiatSay));
                    OnPropertyChanged(nameof(PriceSummary));
                }
            }
        }

        private decimal _giaVonGiatHap = 56000m;
        public decimal GiaVonGiatHap
        {
            get => _giaVonGiatHap;
            set
            {
                if (_giaVonGiatHap != value)
                {
                    _giaVonGiatHap = value;
                    OnPropertyChanged(nameof(GiaVonGiatHap));
                    OnPropertyChanged(nameof(PriceSummary));
                }
            }
        }

        private decimal _giaVonUiPhang = 28000m;
        public decimal GiaVonUiPhang
        {
            get => _giaVonUiPhang;
            set
            {
                if (_giaVonUiPhang != value)
                {
                    _giaVonUiPhang = value;
                    OnPropertyChanged(nameof(GiaVonUiPhang));
                    OnPropertyChanged(nameof(PriceSummary));
                }
            }
        }

        public string PriceSummary => $"Sấy: {GiaVonGiatSay:N0}đ • Hấp: {GiaVonGiatHap:N0}đ • Ủi: {GiaVonUiPhang:N0}đ";

        public decimal GetCostPriceForService(LaundryServiceType serviceType)
        {
            return serviceType switch
            {
                LaundryServiceType.WashAndDry => GiaVonGiatSay > 0 ? GiaVonGiatSay : Math.Round(30000m * (PartnerRate / 100m), 0),
                LaundryServiceType.DryCleaning => GiaVonGiatHap > 0 ? GiaVonGiatHap : Math.Round(80000m * (PartnerRate / 100m), 0),
                LaundryServiceType.Ironing => GiaVonUiPhang > 0 ? GiaVonUiPhang : Math.Round(40000m * (PartnerRate / 100m), 0),
                _ => Math.Round(30000m * (PartnerRate / 100m), 0)
            };
        }

        public decimal GetProfitForService(LaundryServiceType serviceType)
        {
            decimal retailPrice = serviceType switch
            {
                LaundryServiceType.WashAndDry => 30000m,
                LaundryServiceType.DryCleaning => 80000m,
                LaundryServiceType.Ironing => 40000m,
                _ => 30000m
            };
            return retailPrice - GetCostPriceForService(serviceType);
        }

        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LaundryOrder : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public int? KhachHangId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;

        private int _partnerId;
        public int PartnerId
        {
            get => _partnerId;
            set
            {
                if (_partnerId != value)
                {
                    _partnerId = value;
                    OnPropertyChanged(nameof(PartnerId));
                    OnPropertyChanged(nameof(HasPartner));
                    OnPropertyChanged(nameof(PartnerDisplay));
                    OnPropertyChanged(nameof(CanDispatch));
                }
            }
        }

        private string _partnerName = string.Empty;
        public string PartnerName
        {
            get => _partnerName;
            set
            {
                if (_partnerName != value)
                {
                    _partnerName = value;
                    OnPropertyChanged(nameof(PartnerName));
                    OnPropertyChanged(nameof(PartnerDisplay));
                    OnPropertyChanged(nameof(HasPartner));
                }
            }
        }

        public bool HasPartner => PartnerId > 0 && !string.IsNullOrWhiteSpace(PartnerName) && !PartnerName.Contains("Chờ");
        public string PartnerDisplay => HasPartner ? PartnerName : "⏳ Chờ giao đối tác";

        public LaundryServiceType ServiceType { get; set; }
        public decimal WeightKg { get; set; }
        public decimal UnitPricePerKg { get; set; }
        public decimal TotalPrice
        {
            get => WeightKg * UnitPricePerKg;
            set { }
        }

        private decimal? _donGiaVonKg;
        public decimal? DonGiaVonKg
        {
            get => _donGiaVonKg;
            set { if (_donGiaVonKg != value) { _donGiaVonKg = value; OnPropertyChanged(nameof(DonGiaVonKg)); } }
        }

        private decimal _hotelEarnings;
        public decimal HotelEarnings
        {
            get => _hotelEarnings;
            set { if (_hotelEarnings != value) { _hotelEarnings = value; OnPropertyChanged(nameof(HotelEarnings)); } }
        }

        private decimal _partnerEarnings;
        public decimal PartnerEarnings
        {
            get => _partnerEarnings;
            set { if (_partnerEarnings != value) { _partnerEarnings = value; OnPropertyChanged(nameof(PartnerEarnings)); } }
        }

        public string ClothesConditionNote { get; set; } = string.Empty;
        public DateTime ReceivedDate { get; set; } = DateTime.Now;

        private DateTime? _ngayGiaoDoiTac;
        public DateTime? NgayGiaoDoiTac
        {
            get => _ngayGiaoDoiTac;
            set { if (_ngayGiaoDoiTac != value) { _ngayGiaoDoiTac = value; OnPropertyChanged(nameof(NgayGiaoDoiTac)); } }
        }

        public DateTime AppointmentDate { get; set; }

        private LaundryStatus _status = LaundryStatus.PendingDispatch;
        public LaundryStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(StatusDisplay));
                    OnPropertyChanged(nameof(CanDispatch));
                    OnPropertyChanged(nameof(CanReturn));
                    OnPropertyChanged(nameof(CanCancel));
                    OnPropertyChanged(nameof(DisplayTitle));
                }
            }
        }

        public bool CanDispatch => (Status == LaundryStatus.PendingDispatch || !HasPartner) && Status != LaundryStatus.Completed && Status != LaundryStatus.Cancelled;
        public bool CanReturn => Status == LaundryStatus.Washing || Status == LaundryStatus.Washed;
        public bool CanCancel => Status == LaundryStatus.PendingDispatch;

        public string PaymentMethod { get; set; } = "Ghi nợ vào phòng"; // Thanh toán trực tiếp / Ghi nợ vào phòng
        public bool DaThanhToan { get; set; }
        public bool IsPaid { get => DaThanhToan; set => DaThanhToan = value; }

        public int NguoiTaoId { get; set; } = 1;
        public int? NguoiThanhToanId { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; }

        public string RecordedBy { get; set; } = string.Empty;
        public string PaidByStaffName { get; set; } = string.Empty;

        // Trả đồ thực tế & chi phí phát sinh
        public DateTime? ActualReturnDate { get; set; }
        public decimal ChiPhiPhatSinh { get; set; }
        public decimal FinalTotal
        {
            get => TotalPrice + ChiPhiPhatSinh;
            set { }
        }

        // Quản lý công nợ đối tác & đền bù
        public bool DaThanhToanChoDoiTac { get; set; }
        public DateTime? NgayThanhToanChoDoiTac { get; set; }
        public bool CoDenBu { get; set; }
        public string? BenChiuTrachNhiem { get; set; } // KhachSan, DoiTac, KhongCo
        public decimal SoTienDenBu { get; set; }
        public string? LyDoDenBu { get; set; }
        public DateTime? NgayGhiNhanDenBu { get; set; }
        public int? NguoiGhiNhanDenBuId { get; set; }

        public string ServiceTypeDisplay
        {
            get => ServiceType switch
            {
                LaundryServiceType.WashAndDry => "Giặt sấy thông thường",
                LaundryServiceType.DryCleaning => "Giặt hấp cao cấp",
                LaundryServiceType.Ironing => "Ủi phẳng",
                _ => "Khác"
            };
            set { }
        }

        public string StatusDisplay
        {
            get => Status switch
            {
                LaundryStatus.PendingDispatch => "Chờ giao tiệm",
                LaundryStatus.Washing => "Đang giặt",
                LaundryStatus.Washed => "Giặt xong",
                LaundryStatus.Completed => "Hoàn tất",
                LaundryStatus.Cancelled => "Đã hủy",
                _ => "Chờ giao tiệm"
            };
            set { }
        }

        public string DisplayTitle
        {
            get => $"{OrderCode} • {CustomerName} • {(string.IsNullOrWhiteSpace(RoomNumber) ? "Khách vãng lai" : "P." + RoomNumber)} • {StatusDisplay}";
            set { }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class StaffSalaryReportItem
    {
        public string StaffCode { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public int TotalShifts { get; set; }
        public decimal ShiftAllowance => TotalShifts * 50000m;
        public decimal EstimatedSalary => BaseSalary + ShiftAllowance;
        public string Status { get; set; } = "Đang làm việc";
    }

    public class TransactionRecord
    {
        public string TransactionCode { get; set; } = string.Empty;
        public string ServiceCategory { get; set; } = string.Empty; // "Tiền phòng & Dịch vụ", "Ẩm thực & Ăn uống", "Sảnh & Sự kiện", "Thuê xe máy", "Bãi gửi xe", "Giặt ủi"
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomDisplay => !string.IsNullOrWhiteSpace(RoomNumber) ? (RoomNumber.StartsWith("Phòng", StringComparison.OrdinalIgnoreCase) ? RoomNumber : $"Phòng {RoomNumber.TrimStart('P', 'p', '0', ' ')}") : "Khách vãng lai";
        public DateTime PaymentTime { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Thu tiền ngay"; // "Thu tiền ngay (Hóa đơn riêng)" hoặc "Tính chung tiền phòng"
        public string CreatedBy { get; set; } = "Lễ tân";
        public string PaidBy { get; set; } = "Lễ tân";
        public string Status { get; set; } = "Đã thanh toán";
        public object? OriginalObject { get; set; }

        public string ServiceBadgeBg => ServiceCategory switch
        {
            "Tiền phòng & Dịch vụ" => "#DBEAFE",
            "Ăn uống" or "Ẩm thực & Ăn uống" => "#FEF3C7",
            "Sảnh & Sự kiện" => "#FCE7F3",
            "Thuê xe máy" => "#E0E7FF",
            "Bãi gửi xe" => "#DCFCE7",
            "Giặt ủi" => "#F3E8FF",
            _ => "#F1F5F9"
        };

        public string ServiceBadgeFg => ServiceCategory switch
        {
            "Tiền phòng & Dịch vụ" => "#1D4ED8",
            "Ăn uống" or "Ẩm thực & Ăn uống" => "#B45309",
            "Sảnh & Sự kiện" => "#BE185D",
            "Thuê xe máy" => "#4338CA",
            "Bãi gửi xe" => "#15803D",
            "Giặt ủi" => "#7E22CE",
            _ => "#475569"
        };
    }
}
