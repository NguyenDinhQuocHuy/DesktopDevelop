using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuanLyDichVuKhachSan.Models
{
    public enum RoomStatus
    {
        Available,   // Trống (Xanh lá)
        Occupied,    // Đang ở (Xanh dương)
        Cleaning,    // Dọn dẹp (Vàng cam)
        Maintenance, // Bảo trì (Đỏ)
        Reserved     // Đã đặt trước (Tím)
    }

    public class HotelRoom : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty; // P.101, P.102,...
        public int Floor { get; set; } // 1..5
        public string RoomType { get; set; } = "Phòng Standard"; // Standard, Deluxe, Suite, Family, VIP
        public int Capacity { get; set; } = 2;

        private decimal _pricePerNight = 500000;
        public decimal PricePerNight
        {
            get => _pricePerNight;
            set
            {
                if (_pricePerNight != value)
                {
                    _pricePerNight = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalRoomCost));
                }
            }
        }
        
        private RoomStatus _status = RoomStatus.Available;
        public RoomStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StatusDisplay));
                    OnPropertyChanged(nameof(StatusColorBrush));
                    OnPropertyChanged(nameof(IsOccupied));
                    OnPropertyChanged(nameof(IsReserved));
                    OnPropertyChanged(nameof(CanCheckIn));
                    OnPropertyChanged(nameof(CanCheckOut));
                    OnPropertyChanged(nameof(IsMaintenance));
                    OnPropertyChanged(nameof(IsCleaning));
                    OnPropertyChanged(nameof(CanSetMaintenance));
                    OnPropertyChanged(nameof(CanSetCleaning));
                }
            }
        }

        private string _customerName = string.Empty;
        public string CustomerName
        {
            get => _customerName;
            set
            {
                if (_customerName != value)
                {
                    _customerName = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        private string _identityCard = string.Empty;
        public string IdentityCard
        {
            get => _identityCard;
            set
            {
                if (_identityCard != value)
                {
                    _identityCard = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _parkedVehicleInfo = string.Empty;
        public string ParkedVehicleInfo
        {
            get => _parkedVehicleInfo;
            set
            {
                if (_parkedVehicleInfo != value)
                {
                    _parkedVehicleInfo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasParkedVehicle));
                }
            }
        }
        public bool HasParkedVehicle => !string.IsNullOrEmpty(ParkedVehicleInfo);

        private DateTime? _checkInDate;
        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (_checkInDate != value)
                {
                    _checkInDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StayNights));
                    OnPropertyChanged(nameof(TotalRoomCost));
                }
            }
        }

        private DateTime? _expectedCheckOutDate;
        public DateTime? ExpectedCheckOutDate
        {
            get => _expectedCheckOutDate;
            set
            {
                if (_expectedCheckOutDate != value)
                {
                    _expectedCheckOutDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StayNights));
                    OnPropertyChanged(nameof(TotalRoomCost));
                }
            }
        }

        private string _note = string.Empty;
        public string Note
        {
            get => _note;
            set
            {
                if (_note != value)
                {
                    _note = value;
                    OnPropertyChanged();
                }
            }
        }

        public int StayNights
        {
            get
            {
                if (!CheckInDate.HasValue) return 1;
                DateTime end = ExpectedCheckOutDate ?? DateTime.Now;
                int nights = (int)Math.Ceiling((end - CheckInDate.Value).TotalDays);
                return Math.Max(1, nights);
            }
        }

        public decimal TotalRoomCost => StayNights * PricePerNight;

        public bool IsOccupied => Status == RoomStatus.Occupied;
        public bool IsReserved => Status == RoomStatus.Reserved;
        public bool CanCheckIn => Status == RoomStatus.Available || Status == RoomStatus.Cleaning || Status == RoomStatus.Reserved;
        public bool CanCheckOut => Status == RoomStatus.Occupied;
        public bool IsMaintenance => Status == RoomStatus.Maintenance;
        public bool IsCleaning => Status == RoomStatus.Cleaning;
        public bool CanSetMaintenance => Status != RoomStatus.Occupied && Status != RoomStatus.Maintenance;
        public bool CanSetCleaning => Status != RoomStatus.Cleaning && Status != RoomStatus.Maintenance;

        public string StatusDisplay => Status switch
        {
            RoomStatus.Available => "Trống",
            RoomStatus.Occupied => "Đang ở",
            RoomStatus.Cleaning => "Dọn dẹp",
            RoomStatus.Maintenance => "Bảo trì",
            RoomStatus.Reserved => "Đã đặt trước",
            _ => "Khác"
        };

        public string StatusColorBrush => Status switch
        {
            RoomStatus.Available => "#10B981",    // Green
            RoomStatus.Occupied => "#2563EB",     // Blue
            RoomStatus.Cleaning => "#F59E0B",     // Amber
            RoomStatus.Maintenance => "#EF4444",  // Red
            RoomStatus.Reserved => "#8B5CF6",     // Purple
            _ => "#64748B"
        };

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsNotEditing));
                }
            }
        }
        public bool IsNotEditing => !IsEditing;

        public string CheckInDisplay => CheckInDate.HasValue ? CheckInDate.Value.ToString("dd/MM/yyyy HH:mm") : "";
        public string CheckOutDisplay => ExpectedCheckOutDate.HasValue ? ExpectedCheckOutDate.Value.ToString("dd/MM/yyyy HH:mm") : "";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RoomReservation : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public DateTime ExpectedCheckIn { get; set; } = DateTime.Today;
        public DateTime ExpectedCheckOut { get; set; } = DateTime.Today.AddDays(1);
        public decimal DepositAmount { get; set; }
        public string Status { get; set; } = "Đã đặt cọc"; // Đã đặt cọc, Đã nhận phòng, Đã hủy
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FloorGroup : INotifyPropertyChanged
    {
        public int FloorNumber { get; set; }
        public string FloorName => $"🏢 TẦNG {FloorNumber}";
        public ObservableCollection<HotelRoom> Rooms { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RoomServiceUsageItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string ServiceCategory { get; set; } = string.Empty; // Tiền phòng, Ẩm thực, Thuê xe, Sự kiện, Bãi xe, Giặt ủi
        public string ServiceName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime UsedTime { get; set; } = DateTime.Now;
        
        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PaymentStatusDisplay));
                    OnPropertyChanged(nameof(PaymentStatusColor));
                    OnPropertyChanged(nameof(PaymentStatusBg));
                }
            }
        }

        public string RecordedBy { get; set; } = string.Empty;
        public string PaidBy { get; set; } = string.Empty;
        
        private string _paymentMethod = "Thanh toán trực tiếp";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set
            {
                if (_paymentMethod != value)
                {
                    _paymentMethod = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool _isPaid;
        public bool IsPaid
        {
            get => _isPaid;
            set
            {
                if (_isPaid != value)
                {
                    _isPaid = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PaymentStatusDisplay));
                    OnPropertyChanged(nameof(PaymentStatusColor));
                    OnPropertyChanged(nameof(PaymentStatusBg));
                }
            }
        }

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                if (_isHighlighted != value)
                {
                    _isHighlighted = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PaymentStatusDisplay => Amount < 0 
            ? (IsPaid ? "Đã khấu trừ" : "Hoàn lại khách") 
            : (IsPaid ? "Đã thanh toán" : "Chưa thanh toán");

        public string PaymentStatusColor => Amount < 0 
            ? "#DC2626" 
            : (IsPaid ? "#059669" : "#D97706");

        public string PaymentStatusBg => Amount < 0 
            ? "#FEE2E2" 
            : (IsPaid ? "#DCFCE7" : "#FEF3C7");

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RoomInvoice
    {
        public string InvoiceCode { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int StayNights { get; set; } = 1;
        public decimal PricePerNight { get; set; }
        public decimal RoomCost { get; set; }
        public decimal ServicesCost { get; set; }
        public decimal TotalAmount { get; set; }
        public string IssuedBy { get; set; } = string.Empty;
        public string PaidBy { get; set; } = string.Empty;
        public List<RoomServiceUsageItem> PaidItems { get; set; } = new();
    }
}
