using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class EventHourlySlot : INotifyPropertyChanged
    {
        public int Hour { get; set; }
        public string TimeLabel { get; set; } = "";
        public bool IsOccupied { get; set; }
        public string CustomerDetail { get; set; } = "";

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnPropertyChanged(nameof(StatusText));
                    OnPropertyChanged(nameof(SlotBg));
                    OnPropertyChanged(nameof(SlotFg));
                    OnPropertyChanged(nameof(BorderBrush));
                }
            }
        }

        public string StatusText
        {
            get
            {
                if (IsSelected) return "Đang chọn";
                if (IsOccupied) return "Đang sử dụng";
                return "Trống";
            }
        }

        public string SlotBg
        {
            get
            {
                if (IsSelected) return "#EFF6FF"; // Blue-50
                if (IsOccupied) return "#FEF2F2"; // Red-50
                return "#F0FDF4";                 // Green-50
            }
        }

        public string SlotFg
        {
            get
            {
                if (IsSelected) return "#1D4ED8"; // Blue-700
                if (IsOccupied) return "#DC2626"; // Red-600
                return "#16A34A";                 // Green-600
            }
        }

        public string BorderBrush
        {
            get
            {
                if (IsSelected) return "#3B82F6"; // Blue-500
                if (IsOccupied) return "#FECACA"; // Red-200
                return "#BBF7D0";                 // Green-200
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    public class EventViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;
        public AuthService Auth => AuthService.Instance;

        public bool IsAdmin => Auth.IsAdmin;
        public bool IsReceptionist => Auth.IsReceptionist;

        private readonly DispatcherTimer _realtimeCheckTimer;

        // Quản lý Tab Cột Trái (Tổng Quan vs Thời Khóa Biểu)
        private string _leftPanelTab = "Overview"; // "Overview" hoặc "Timetable"
        public string LeftPanelTab
        {
            get => _leftPanelTab;
            set
            {
                if (SetProperty(ref _leftPanelTab, value))
                {
                    OnPropertyChanged(nameof(IsOverviewTab));
                    OnPropertyChanged(nameof(IsTimetableTab));
                    if (IsTimetableTab)
                    {
                        BuildHourlySchedule();
                    }
                }
            }
        }

        public bool IsOverviewTab => LeftPanelTab == "Overview";
        public bool IsTimetableTab => LeftPanelTab == "Timetable";

        // Quản lý ngày và sảnh xem thời khóa biểu chi tiết (06h - 23h)
        private DateTime _selectedTimetableDate = DateTime.Today;
        public DateTime SelectedTimetableDate
        {
            get => _selectedTimetableDate;
            set
            {
                if (SetProperty(ref _selectedTimetableDate, value))
                {
                    StartDate = value.Date;
                    BuildHourlySchedule();
                    OnPropertyChanged(nameof(TimetableDateDisplay));
                }
            }
        }

        public string TimetableDateDisplay => SelectedTimetableDate.ToString("dd/MM/yyyy (dddd)");

        private EventSpace? _selectedTimetableSpace;
        public EventSpace? SelectedTimetableSpace
        {
            get => _selectedTimetableSpace;
            set
            {
                if (SetProperty(ref _selectedTimetableSpace, value))
                {
                    if (value != null) SelectedSpace = value;
                    BuildHourlySchedule();
                }
            }
        }

        public ObservableCollection<EventHourlySlot> HourlyScheduleSlots { get; } = new();

        private string _selectionStatusMessage = "";
        public string SelectionStatusMessage
        {
            get => _selectionStatusMessage;
            set => SetProperty(ref _selectionStatusMessage, value);
        }

        private bool _isSelectionContinuous = true;
        public bool IsSelectionContinuous
        {
            get => _isSelectionContinuous;
            set => SetProperty(ref _isSelectionContinuous, value);
        }

        // Đặt sảnh mới (Lễ tân)
        private EventSpace? _selectedSpace;
        public EventSpace? SelectedSpace
        {
            get => _selectedSpace;
            set
            {
                if (SetProperty(ref _selectedSpace, value))
                {
                    CalculateEstimatedPrice();
                }
            }
        }

        private string _bookingCustomerName = "";
        public string BookingCustomerName { get => _bookingCustomerName; set => SetProperty(ref _bookingCustomerName, value); }

        private string _bookingPhoneNumber = "";
        public string BookingPhoneNumber { get => _bookingPhoneNumber; set => SetProperty(ref _bookingPhoneNumber, value); }

        private string _bookingRoomNumber = "";
        public string BookingRoomNumber
        {
            get => _bookingRoomNumber;
            set
            {
                if (SetProperty(ref _bookingRoomNumber, value))
                {
                    AutoFillCustomerInfo(value);
                    if (!CanChooseBookingRoomBill && BookingBillingType == BillingType.ChargeToRoom)
                    {
                        BookingBillingType = BillingType.DirectPayment;
                    }
                    OnPropertyChanged(nameof(CanChooseBookingRoomBill));
                    OnPropertyChanged(nameof(IsDirectBookingPayment));
                    OnPropertyChanged(nameof(IsRoomChargeBookingPayment));
                    OnPropertyChanged(nameof(BookingRoomChargeNoticeText));
                }
            }
        }

        private void AutoFillCustomerInfo(string roomNumber)
        {
            string norm = DataService.NormalizeRoomNumber(roomNumber);
            if (!string.IsNullOrEmpty(norm))
            {
                var room = Data.HotelRooms.FirstOrDefault(r => 
                    DataService.NormalizeRoomNumber(r.RoomNumber) == norm ||
                    string.Equals(r.RoomNumber, norm, StringComparison.OrdinalIgnoreCase));
                if (room != null && !string.IsNullOrWhiteSpace(room.CustomerName))
                {
                    BookingCustomerName = room.CustomerName;
                    if (!string.IsNullOrWhiteSpace(room.PhoneNumber)) BookingPhoneNumber = room.PhoneNumber;
                }
                else
                {
                    BookingCustomerName = "";
                    BookingPhoneNumber = "";
                }
            }
            else
            {
                BookingCustomerName = "";
                BookingPhoneNumber = "";
            }
        }

        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set { if (SetProperty(ref _startDate, value)) CalculateEstimatedPrice(); }
        }

        private string _startTimeStr = "08:00";
        public string StartTimeStr
        {
            get => _startTimeStr;
            set { if (SetProperty(ref _startTimeStr, value)) CalculateEstimatedPrice(); }
        }

        private string _endTimeStr = "12:00";
        public string EndTimeStr
        {
            get => _endTimeStr;
            set { if (SetProperty(ref _endTimeStr, value)) CalculateEstimatedPrice(); }
        }

        public string[] AvailableHours { get; } = new string[]
        {
            "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00",
            "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00",
            "20:00", "21:00", "22:00", "23:00"
        };

        private string _bookingNote = "";
        public string BookingNote
        {
            get => _bookingNote;
            set => SetProperty(ref _bookingNote, value);
        }

        private bool _isTimeRangeSelected = true;
        public bool IsTimeRangeSelected
        {
            get => _isTimeRangeSelected;
            set => SetProperty(ref _isTimeRangeSelected, value);
        }

        private double _durationHours = 4;
        public double DurationHours
        {
            get => _durationHours;
            set => SetProperty(ref _durationHours, value);
        }

        public string DurationDisplay => $"{DurationHours:0.#} giờ";

        // GIAI ĐOẠN 1: HÌNH THỨC THANH TOÁN (Cọc trước vs Trả hết)
        private bool _isDepositPayment = true;
        public bool IsDepositPayment
        {
            get => _isDepositPayment;
            set
            {
                if (SetProperty(ref _isDepositPayment, value))
                {
                    if (value)
                    {
                        // Setup tiền cọc mặc định là 30% so với tổng tiền, có thể thay đổi tùy ý
                        DepositAmount = Math.Round(EstimatedTotalAmount * 0.3m / 1000) * 1000;
                        if (DepositAmount > EstimatedTotalAmount) DepositAmount = EstimatedTotalAmount;
                    }
                    else
                    {
                        // Trả hết
                        DepositAmount = EstimatedTotalAmount;
                    }
                    NotifyPaymentCalculationChanged();
                }
            }
        }

        public bool IsFullPayment
        {
            get => !_isDepositPayment;
            set => IsDepositPayment = !value;
        }

        private decimal _depositAmount = 0;
        public decimal DepositAmount
        {
            get => _depositAmount;
            set
            {
                if (SetProperty(ref _depositAmount, value))
                {
                    NotifyPaymentCalculationChanged();
                }
            }
        }

        private decimal _estimatedTotalAmount = 0;
        public decimal EstimatedTotalAmount
        {
            get => _estimatedTotalAmount;
            set
            {
                if (SetProperty(ref _estimatedTotalAmount, value))
                {
                    if (!IsDepositPayment)
                    {
                        _depositAmount = value;
                        OnPropertyChanged(nameof(DepositAmount));
                    }
                    NotifyPaymentCalculationChanged();
                }
            }
        }

        public bool IsDepositInvalid => IsDepositPayment && DepositAmount > EstimatedTotalAmount;
        public string DepositErrorMessage => IsDepositInvalid ? "⚠️ Tiền cọc không được vượt quá Tổng tiền dự kiến!" : "";
        public decimal RemainingEstimatedAmount => Math.Max(0, EstimatedTotalAmount - (IsFullPayment ? EstimatedTotalAmount : DepositAmount));
        public decimal CurrentCollectAmount => IsFullPayment ? EstimatedTotalAmount : DepositAmount;
        public string ConfirmButtonLabel => "XÁC NHẬN ĐẶT SẢNH";

        private string _bookingPaymentMethod = "Tiền mặt";
        public string BookingPaymentMethod
        {
            get => _bookingPaymentMethod;
            set => SetProperty(ref _bookingPaymentMethod, value);
        }

        private BillingType _bookingBillingType = BillingType.DirectPayment;
        public BillingType BookingBillingType
        {
            get => _bookingBillingType;
            set
            {
                if (SetProperty(ref _bookingBillingType, value))
                {
                    OnPropertyChanged(nameof(IsDirectBookingPayment));
                    OnPropertyChanged(nameof(IsRoomChargeBookingPayment));
                    OnPropertyChanged(nameof(BookingRoomChargeNoticeText));
                }
            }
        }

        public bool CanChooseBookingRoomBill => !string.IsNullOrWhiteSpace(DataService.NormalizeRoomNumber(BookingRoomNumber));
        public bool IsDirectBookingPayment => BookingBillingType == BillingType.DirectPayment;
        public bool IsRoomChargeBookingPayment => BookingBillingType == BillingType.ChargeToRoom;
        public string BookingRoomChargeNoticeText => $"💡 Tiền đặt sảnh/cọc ({CurrentCollectAmount:N0} đ) sẽ được ghi nợ vào hóa đơn phòng {BookingRoomNumber}. Khách sẽ thanh toán khi Check-out.";

        private bool _isBookingCash = true;
        public bool IsBookingCash
        {
            get => _isBookingCash;
            set
            {
                if (SetProperty(ref _isBookingCash, value))
                {
                    if (value) _isBookingBankTransfer = false;
                    OnPropertyChanged(nameof(IsBookingBankTransfer));
                    if (value) BookingPaymentMethod = "Tiền mặt";
                }
            }
        }

        private bool _isBookingBankTransfer;
        public bool IsBookingBankTransfer
        {
            get => _isBookingBankTransfer;
            set
            {
                if (SetProperty(ref _isBookingBankTransfer, value))
                {
                    if (value) _isBookingCash = false;
                    OnPropertyChanged(nameof(IsBookingCash));
                    if (value) BookingPaymentMethod = "Chuyển khoản";
                }
            }
        }

        public string CreatorStaffDisplay => Auth.IsAdmin ? "Chủ khách sạn" : Data.GetCurrentDutyStaffName(DateTime.Now);

        private void NotifyPaymentCalculationChanged()
        {
            OnPropertyChanged(nameof(IsDepositInvalid));
            OnPropertyChanged(nameof(DepositErrorMessage));
            OnPropertyChanged(nameof(RemainingEstimatedAmount));
            OnPropertyChanged(nameof(CurrentCollectAmount));
            OnPropertyChanged(nameof(ConfirmButtonLabel));
            OnPropertyChanged(nameof(BookingRoomChargeNoticeText));
        }

        // GIAI ĐOẠN 2: QUYẾT TOÁN ĐƠN ĐẶT SỰ KIỆN (Bỏ nhánh hoàn tiền, chỉ 1 công thức chuẩn)
        public System.Collections.Generic.IEnumerable<EventBooking> SelectableEventBookings => Data.EventBookings;

        public System.Collections.Generic.IEnumerable<EventBooking> PendingEventBookings => 
            Data.EventBookings.Where(b => b.PaymentStatus != "Hoàn tất" && b.PaymentStatus != "Đã hủy");

        private EventBooking? _selectedBookingToComplete;
        public EventBooking? SelectedBookingToComplete
        {
            get => _selectedBookingToComplete;
            set
            {
                if (SetProperty(ref _selectedBookingToComplete, value))
                {
                    if (!CanChargeSettlementToRoom)
                    {
                        SettlementBillingType = BillingType.DirectPayment;
                    }
                    OnPropertyChanged(nameof(SelectedBookingTargetDisplay));
                    OnPropertyChanged(nameof(CanChargeSettlementToRoom));
                    OnPropertyChanged(nameof(IsDirectSettlementPayment));
                    OnPropertyChanged(nameof(IsRoomChargeSettlementPayment));
                    OnPropertyChanged(nameof(RoomChargeNoticeText));
                    NotifySettlementCalculationChanged();
                }
            }
        }

        public string SelectedBookingTargetDisplay
        {
            get
            {
                if (SelectedBookingToComplete == null) return "Chưa chọn đơn sự kiện nào";
                string roomStr = string.IsNullOrWhiteSpace(SelectedBookingToComplete.RoomNumber) ? "Khách vãng lai" : $"Phòng P.{SelectedBookingToComplete.RoomNumber}";
                return $"📍 {SelectedBookingToComplete.BookingCode} · {SelectedBookingToComplete.SpaceName} · Khách: {SelectedBookingToComplete.CustomerName} ({roomStr})";
            }
        }

        private decimal _extraFee = 0;
        public decimal ExtraFee
        {
            get => _extraFee;
            set
            {
                if (SetProperty(ref _extraFee, value))
                {
                    NotifySettlementCalculationChanged();
                }
            }
        }

        private string _damageNote = "";
        public string DamageNote { get => _damageNote; set => SetProperty(ref _damageNote, value); }

        private string _settlementPaymentMethod = "Tiền mặt";
        public string SettlementPaymentMethod { get => _settlementPaymentMethod; set => SetProperty(ref _settlementPaymentMethod, value); }

        // Phương thức ghi nợ / thanh toán khi quyết toán
        private BillingType _settlementBillingType = BillingType.DirectPayment;
        public BillingType SettlementBillingType
        {
            get => _settlementBillingType;
            set
            {
                if (SetProperty(ref _settlementBillingType, value))
                {
                    OnPropertyChanged(nameof(IsDirectSettlementPayment));
                    OnPropertyChanged(nameof(IsRoomChargeSettlementPayment));
                    OnPropertyChanged(nameof(RoomChargeNoticeText));
                }
            }
        }

        public bool CanChargeSettlementToRoom =>
            SelectedBookingToComplete != null &&
            !string.IsNullOrWhiteSpace(DataService.NormalizeRoomNumber(SelectedBookingToComplete.RoomNumber));

        public bool IsDirectSettlementPayment => SettlementBillingType == BillingType.DirectPayment;
        public bool IsRoomChargeSettlementPayment => SettlementBillingType == BillingType.ChargeToRoom;

        private bool _isSettlementCash = true;
        public bool IsSettlementCash
        {
            get => _isSettlementCash;
            set
            {
                if (SetProperty(ref _isSettlementCash, value))
                {
                    if (value) _isSettlementBankTransfer = false;
                    OnPropertyChanged(nameof(IsSettlementBankTransfer));
                }
            }
        }

        private bool _isSettlementBankTransfer = false;
        public bool IsSettlementBankTransfer
        {
            get => _isSettlementBankTransfer;
            set
            {
                if (SetProperty(ref _isSettlementBankTransfer, value))
                {
                    if (value) _isSettlementCash = false;
                    OnPropertyChanged(nameof(IsSettlementCash));
                }
            }
        }

        public string RoomChargeNoticeText => SelectedBookingToComplete != null
            ? $"💡 Chi phí quyết toán ({SettlementRemainingToPay:N0} đ) sẽ được chuyển vào hóa đơn chi tiết phòng {SelectedBookingToComplete.RoomNumber}. Khách sẽ thanh toán khi Check-out."
            : "";

        public bool IsSelectedBookingInUse => 
            SelectedBookingToComplete != null && 
            (SelectedBookingToComplete.OrderStatus == "Đang sử dụng" || 
             SelectedBookingToComplete.PaymentStatus == "Đang sử dụng" ||
             (SelectedBookingToComplete.OrderStatus == "Đã đặt" && SelectedBookingToComplete.StartTime <= DateTime.Now && SelectedBookingToComplete.EndTime >= DateTime.Now && !SelectedBookingToComplete.DaThanhToan));

        public bool CanCollectSettlement => SelectedBookingToComplete != null && IsSelectedBookingInUse;

        public decimal SettlementEstimatedTotal => SelectedBookingToComplete?.TotalEstimatedAmount ?? 0;
        public decimal SettlementGrandTotal => SettlementEstimatedTotal + ExtraFee;
        public decimal SettlementPaidBefore => SelectedBookingToComplete?.DepositAmount ?? 0;
        public decimal SettlementRemainingToPay
        {
            get
            {
                if (SelectedBookingToComplete == null) return 0;
                if (SelectedBookingToComplete.OrderStatus == "Hoàn tất" || 
                    SelectedBookingToComplete.PaymentStatus == "Hoàn tất" || 
                    SelectedBookingToComplete.PaymentStatus == "Đã hoàn tất" ||
                    SelectedBookingToComplete.OrderStatus.StartsWith("Đã hủy") ||
                    SelectedBookingToComplete.PaymentStatus.StartsWith("Đã hủy"))
                {
                    return 0;
                }
                if (!IsSelectedBookingInUse)
                {
                    return 0;
                }
                return Math.Max(0, SettlementGrandTotal - SettlementPaidBefore);
            }
        }

        public string SettlementButtonLabel
        {
            get
            {
                if (SelectedBookingToComplete == null) return "CHỌN ĐƠN SỰ KIỆN";
                if (SelectedBookingToComplete.OrderStatus == "Hoàn tất" || SelectedBookingToComplete.PaymentStatus == "Hoàn tất" || SelectedBookingToComplete.PaymentStatus == "Đã hoàn tất")
                {
                    return "ĐÃ HOÀN TẤT";
                }
                if (SelectedBookingToComplete.OrderStatus.StartsWith("Đã hủy") || SelectedBookingToComplete.PaymentStatus.StartsWith("Đã hủy"))
                {
                    return "ĐƠN ĐÃ HỦY";
                }
                if (SelectedBookingToComplete.StartTime > DateTime.Now && !IsSelectedBookingInUse)
                {
                    return "SỰ KIỆN CHƯA DIỄN RA";
                }
                if (IsSelectedBookingInUse)
                {
                    return "HOÀN TẤT & QUYẾT TOÁN";
                }
                return "HOÀN TẤT";
            }
        }

        private void NotifySettlementCalculationChanged()
        {
            OnPropertyChanged(nameof(IsSelectedBookingInUse));
            OnPropertyChanged(nameof(SettlementEstimatedTotal));
            OnPropertyChanged(nameof(SettlementGrandTotal));
            OnPropertyChanged(nameof(SettlementPaidBefore));
            OnPropertyChanged(nameof(SettlementRemainingToPay));
            OnPropertyChanged(nameof(SettlementButtonLabel));
            OnPropertyChanged(nameof(CanCollectSettlement));
            OnPropertyChanged(nameof(CanChargeSettlementToRoom));
            OnPropertyChanged(nameof(IsDirectSettlementPayment));
            OnPropertyChanged(nameof(IsRoomChargeSettlementPayment));
            OnPropertyChanged(nameof(RoomChargeNoticeText));
        }

        // Commands
        public ICommand BookSpaceCommand { get; }
        public ICommand CompleteBookingCommand { get; }
        public ICommand SelectSpaceCommand { get; }
        public ICommand SetLeftTabCommand { get; }
        public ICommand SelectTimetableSpaceCommand { get; }
        public ICommand PrevDayCommand { get; }
        public ICommand TodayCommand { get; }
        public ICommand NextDayCommand { get; }
        public ICommand ToggleSlotSelectionCommand { get; }
        public ICommand ClearSlotSelectionCommand { get; }
        public ICommand EditEventSpaceCommand { get; }

        // Tìm kiếm khách hàng theo tên / số điện thoại real-time (Debounced 180ms)
        private string _searchBookingQuery = "";
        public string SearchBookingQuery
        {
            get => _searchBookingQuery;
            set
            {
                if (SetProperty(ref _searchBookingQuery, value))
                {
                    Debounce("EventBookingSearch", ApplyBookingFilter, 180);
                }
            }
        }

        // Bộ lọc thời gian: "Today" (mặc định), "All", "Custom"
        private string _timeFilter = "Today";
        public string TimeFilter
        {
            get => _timeFilter;
            set
            {
                if (SetProperty(ref _timeFilter, value))
                {
                    OnPropertyChanged(nameof(IsTodayTimeFilter));
                    OnPropertyChanged(nameof(IsAllTimeFilter));
                    OnPropertyChanged(nameof(IsCustomTimeFilter));
                    ApplyBookingFilter();
                }
            }
        }

        public bool IsTodayTimeFilter
        {
            get => TimeFilter == "Today";
            set { if (value) TimeFilter = "Today"; }
        }

        public bool IsAllTimeFilter
        {
            get => TimeFilter == "All";
            set { if (value) TimeFilter = "All"; }
        }

        public bool IsCustomTimeFilter
        {
            get => TimeFilter == "Custom";
            set { if (value) TimeFilter = "Custom"; }
        }

        private DateTime? _fromDate = DateTime.Today;
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                if (SetProperty(ref _fromDate, value))
                {
                    TimeFilter = "Custom";
                    ApplyBookingFilter();
                }
            }
        }

        private DateTime? _toDate = DateTime.Today.AddDays(1);
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (SetProperty(ref _toDate, value))
                {
                    TimeFilter = "Custom";
                    ApplyBookingFilter();
                }
            }
        }

        // Bộ lọc trạng thái đơn sảnh: "All" (mặc định), "Đã cọc", "Đang sử dụng", "Đã thanh toán full", "Hoàn tất", "Đã hủy"
        private string _statusFilter = "All";
        public string StatusFilter
        {
            get => _statusFilter;
            set
            {
                if (SetProperty(ref _statusFilter, value))
                {
                    OnPropertyChanged(nameof(IsAllStatusFilter));
                    OnPropertyChanged(nameof(IsDepositStatusFilter));
                    OnPropertyChanged(nameof(IsInUseStatusFilter));
                    OnPropertyChanged(nameof(IsFullPaidStatusFilter));
                    OnPropertyChanged(nameof(IsCompletedStatusFilter));
                    OnPropertyChanged(nameof(IsCancelledStatusFilter));
                    ApplyBookingFilter();
                }
            }
        }

        public bool IsAllStatusFilter { get => StatusFilter == "All"; set { if (value) StatusFilter = "All"; } }
        public bool IsDepositStatusFilter { get => StatusFilter == "Đã cọc"; set { if (value) StatusFilter = "Đã cọc"; } }
        public bool IsInUseStatusFilter { get => StatusFilter == "Đang sử dụng"; set { if (value) StatusFilter = "Đang sử dụng"; } }
        public bool IsFullPaidStatusFilter { get => StatusFilter == "Đã thanh toán full"; set { if (value) StatusFilter = "Đã thanh toán full"; } }
        public bool IsCompletedStatusFilter { get => StatusFilter == "Hoàn tất"; set { if (value) StatusFilter = "Hoàn tất"; } }
        public bool IsCancelledStatusFilter { get => StatusFilter == "Đã hủy"; set { if (value) StatusFilter = "Đã hủy"; } }

        public ICommand SetTimeFilterCommand { get; }
        public ICommand SetStatusFilterCommand { get; }

        public ObservableRangeCollection<EventBooking> FilteredEventBookings { get; } = new();

        public void ApplyBookingFilter()
        {
            var list = Data.EventBookings.AsEnumerable();

            // 1. Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(StatusFilter) && StatusFilter != "All")
            {
                if (StatusFilter == "Đã cọc")
                {
                    list = list.Where(b => (b.InitialPaymentType == "Đặt cọc trước" || b.PaymentStatus == "Đã cọc") && b.OrderStatus != "Đang sử dụng" && b.PaymentStatus != "Đang sử dụng" && b.OrderStatus != "Hoàn tất" && b.PaymentStatus != "Hoàn tất" && !b.OrderStatus.StartsWith("Đã hủy"));
                }
                else if (StatusFilter == "Đã thanh toán full" || StatusFilter == "Đã thanh toán hết")
                {
                    list = list.Where(b => (b.InitialPaymentType == "Thanh toán toàn bộ" || b.PaymentStatus == "Đã thanh toán" || b.PaymentStatus == "Đã thanh toán full" || b.PaymentStatus == "Đã thanh toán hết") && b.OrderStatus != "Đang sử dụng" && b.PaymentStatus != "Đang sử dụng" && b.OrderStatus != "Hoàn tất" && b.PaymentStatus != "Hoàn tất" && !b.OrderStatus.StartsWith("Đã hủy"));
                }
                else if (StatusFilter == "Đang sử dụng")
                {
                    list = list.Where(b => b.OrderStatus == "Đang sử dụng" || b.PaymentStatus == "Đang sử dụng");
                }
                else if (StatusFilter == "Hoàn tất")
                {
                    list = list.Where(b => b.OrderStatus == "Hoàn tất" || b.PaymentStatus == "Đã hoàn tất" || b.PaymentStatus == "Hoàn tất");
                }
                else if (StatusFilter == "Đã hủy")
                {
                    list = list.Where(b => b.OrderStatus.StartsWith("Đã hủy") || b.PaymentStatus.StartsWith("Đã hủy"));
                }
                else
                {
                    list = list.Where(b => string.Equals(b.PaymentStatus, StatusFilter, StringComparison.OrdinalIgnoreCase));
                }
            }

            // 2. Lọc theo thời gian (Hôm Nay / Tất Cả / Khoảng Ngày)
            var today = DateTime.Today;
            if (TimeFilter == "Today")
            {
                list = list.Where(b => 
                    b.StartTime.Date == today || 
                    (b.StartTime.Date <= today && b.EndTime.Date >= today)
                );
            }
            else if (TimeFilter == "Custom")
            {
                var start = (FromDate ?? DateTime.Today).Date;
                var end = (ToDate ?? DateTime.Today.AddDays(1)).Date;
                if (start > end)
                {
                    var temp = start;
                    start = end;
                    end = temp;
                }
                list = list.Where(b => 
                    (b.StartTime.Date >= start && b.StartTime.Date <= end) ||
                    (b.EndTime.Date >= start && b.EndTime.Date <= end) ||
                    (b.StartTime.Date <= start && b.EndTime.Date >= end)
                );
            }

            // 3. Tìm kiếm từ khóa Real-time
            if (!string.IsNullOrWhiteSpace(SearchBookingQuery))
            {
                string rawQ = SearchBookingQuery.Trim();
                string cleanQ = TextSearchHelper.ToSearchKey(rawQ);
                list = list.Where(b =>
                    (!string.IsNullOrWhiteSpace(b.PhoneNumber) && b.PhoneNumber.Contains(rawQ)) ||
                    TextSearchHelper.MatchPrepared(b.CustomerName, cleanQ) ||
                    TextSearchHelper.MatchPrepared(b.SpaceName, cleanQ) ||
                    (!string.IsNullOrWhiteSpace(b.RoomNumber) && b.RoomNumber.Contains(rawQ)) ||
                    (!string.IsNullOrWhiteSpace(b.BookingCode) && b.BookingCode.IndexOf(rawQ, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }
            FilteredEventBookings.ReplaceRange(list.ToList());
        }

        public EventViewModel()
        {
            SetTimeFilterCommand = new RelayCommand(p => { if (p is string m) TimeFilter = m; });
            SetStatusFilterCommand = new RelayCommand(p => { if (p is string s) StatusFilter = s; });
            BookSpaceCommand = new RelayCommand(BookSpace);
            CompleteBookingCommand = new RelayCommand(CompleteBooking);
            SelectSpaceCommand = new RelayCommand(p => { if (p is EventSpace sp) SelectedSpace = sp; });
            SetLeftTabCommand = new RelayCommand(p => { if (p is string tab) LeftPanelTab = tab; });
            SelectTimetableSpaceCommand = new RelayCommand(p => { if (p is EventSpace sp) SelectedTimetableSpace = sp; });
            PrevDayCommand = new RelayCommand(_ => SelectedTimetableDate = SelectedTimetableDate.AddDays(-1));
            TodayCommand = new RelayCommand(_ => SelectedTimetableDate = DateTime.Today);
            NextDayCommand = new RelayCommand(_ => SelectedTimetableDate = SelectedTimetableDate.AddDays(1));
            ToggleSlotSelectionCommand = new RelayCommand(ToggleSlotSelection);
            ClearSlotSelectionCommand = new RelayCommand(_ => ClearSlotSelection());
            EditEventSpaceCommand = new RelayCommand(EditEventSpace);

            SelectedSpace = null;
            SelectedTimetableSpace = Data.EventSpaces.FirstOrDefault();
            CalculateEstimatedPrice();
            BuildHourlySchedule();

            ApplyBookingFilter();
            Data.EventBookings.CollectionChanged += (s, e) => ApplyBookingFilter();

            // Tự động chọn đơn đang sử dụng nếu có, hoặc đơn chưa hoàn tất
            SelectedBookingToComplete = Data.EventBookings.FirstOrDefault(b => b.OrderStatus == "Đang sử dụng" || b.PaymentStatus == "Đang sử dụng") ?? PendingEventBookings.FirstOrDefault();

            // Real-time timer kiểm tra tự động hoàn tất sự kiện khi hết giờ và cập nhật lịch biểu
            _realtimeCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };
            _realtimeCheckTimer.Tick += (s, e) => 
            {
                CheckRealtimeEventCompletions();
                if (IsTimetableTab) BuildHourlySchedule();
            };
            _realtimeCheckTimer.Start();
        }

        public void BuildHourlySchedule()
        {
            HourlyScheduleSlots.Clear();
            if (SelectedTimetableSpace == null) return;

            DateTime viewDate = SelectedTimetableDate.Date;
            int spaceId = SelectedTimetableSpace.Id;

            var bookings = Data.EventBookings
                .Where(b => b.SpaceId == spaceId && b.PaymentStatus != "Đã hủy")
                .ToList();

            // Khung giờ chi tiết từng tiếng từ 06:00 đến 23:00 (17 khung giờ)
            for (int hour = 6; hour <= 22; hour++)
            {
                DateTime slotStart = viewDate.AddHours(hour);
                DateTime slotEnd = viewDate.AddHours(hour + 1);

                var booking = bookings.FirstOrDefault(b => b.StartTime < slotEnd && b.EndTime > slotStart);

                string timeLabel = $"{hour:D2}:00 - {hour + 1:D2}:00";
                string detail = "";
                if (booking != null)
                {
                    string roomStr = string.IsNullOrWhiteSpace(booking.RoomNumber) ? "Vãng lai" : $"P.{booking.RoomNumber}";
                    detail = $"{booking.CustomerName} ({roomStr}) | {booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}";
                }

                HourlyScheduleSlots.Add(new EventHourlySlot
                {
                    Hour = hour,
                    TimeLabel = timeLabel,
                    IsOccupied = booking != null,
                    CustomerDetail = detail,
                    IsSelected = false
                });
            }

            ClearSlotSelection();
        }

        public void SelectRange(int startHour, int endHour)
        {
            int minH = Math.Min(startHour, endHour);
            int maxH = Math.Max(startHour, endHour);

            foreach (var slot in HourlyScheduleSlots)
            {
                if (slot.Hour >= minH && slot.Hour <= maxH)
                {
                    slot.IsSelected = !slot.IsOccupied;
                }
                else
                {
                    slot.IsSelected = false;
                }
            }

            SyncSelectionToBookingForm();
        }

        public void ToggleSlot(EventHourlySlot slot, bool isCtrl = false)
        {
            if (slot.IsOccupied)
            {
                MessageBox.Show($"Khung giờ [{slot.TimeLabel}] hiện đang có sự kiện sử dụng ({slot.CustomerDetail})!\nVui lòng chọn các khung giờ còn trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (isCtrl)
            {
                slot.IsSelected = !slot.IsSelected;
            }
            else
            {
                bool prev = slot.IsSelected;
                foreach (var s in HourlyScheduleSlots) s.IsSelected = false;
                slot.IsSelected = !prev;
            }

            SyncSelectionToBookingForm();
        }

        private void ToggleSlotSelection(object? parameter)
        {
            if (parameter is EventHourlySlot slot)
            {
                ToggleSlot(slot, isCtrl: true);
            }
        }

        public void ClearSlotSelection()
        {
            foreach (var slot in HourlyScheduleSlots)
            {
                slot.IsSelected = false;
            }
            SelectionStatusMessage = "";
            IsSelectionContinuous = true;
        }

        private void SyncSelectionToBookingForm()
        {
            var selectedSlots = HourlyScheduleSlots.Where(x => x.IsSelected).OrderBy(x => x.Hour).ToList();

            if (selectedSlots.Count == 0)
            {
                SelectionStatusMessage = "";
                IsSelectionContinuous = true;
                return;
            }

            // Kiểm tra tính liên tục của các khung giờ đã chọn
            bool continuous = true;
            for (int i = 0; i < selectedSlots.Count - 1; i++)
            {
                if (selectedSlots[i + 1].Hour != selectedSlots[i].Hour + 1)
                {
                    continuous = false;
                    break;
                }
            }

            IsSelectionContinuous = continuous;

            if (!continuous)
            {
                SelectionStatusMessage = "⚠️ CẢNH BÁO: Các khung giờ bạn chọn không liên tục! Quy định chỉ tiếp nhận khung giờ liên tục trong 1 lần đặt.";
                return;
            }

            int startHour = selectedSlots.First().Hour;
            int endHour = selectedSlots.Last().Hour + 1;

            StartTimeStr = $"{startHour:D2}:00";
            EndTimeStr = $"{endHour:D2}:00";
            StartDate = SelectedTimetableDate.Date;

            if (SelectedTimetableSpace != null)
            {
                SelectedSpace = SelectedTimetableSpace;
            }

            CalculateEstimatedPrice();

            SelectionStatusMessage = $"✅ Đã chọn khung giờ liên tục: {StartTimeStr} - {EndTimeStr} ({selectedSlots.Count} tiếng) tại [{SelectedTimetableSpace?.Name}] ngày {SelectedTimetableDate:dd/MM/yyyy}";
        }

        private void CheckRealtimeEventCompletions()
        {
            var now = DateTime.Now;
            bool stateChanged = false;
            foreach (var b in Data.EventBookings.Where(x => (x.OrderStatus == "Đã đặt" || x.PaymentStatus == "Đã cọc" || x.PaymentStatus == "Đã thanh toán full" || x.PaymentStatus == "Đã thanh toán") && now >= x.StartTime && now < x.EndTime).ToList())
            {
                b.OrderStatus = "Đang sử dụng";
                b.PaymentStatus = "Đang sử dụng";
                stateChanged = true;
                NotificationService.Instance.AddNotification(
                    "🎪 Sự kiện bắt đầu sử dụng", 
                    $"Sảnh [{b.SpaceName}] đã đến giờ bắt đầu vào lúc {b.StartTime:HH:mm} (Khách: {b.CustomerName}). Đơn đã chuyển sang trạng thái 'Đang sử dụng'.",
                    NotificationType.EventEnding,
                    roomNumber: b.RoomNumber,
                    orderCode: b.BookingCode,
                    serviceCategory: "Sự kiện"
                );
            }
            if (stateChanged)
            {
                ApplyBookingFilter();
                NotifySettlementCalculationChanged();
            }
        }

        private bool TryGetDateTimes(out DateTime start, out DateTime end)
        {
            start = StartDate.Date;
            end = StartDate.Date;

            if (string.IsNullOrWhiteSpace(StartTimeStr) || string.IsNullOrWhiteSpace(EndTimeStr))
            {
                return false;
            }

            if (TimeSpan.TryParse(StartTimeStr.Trim(), out var tStart) &&
                TimeSpan.TryParse(EndTimeStr.Trim(), out var tEnd))
            {
                start = StartDate.Date.Add(tStart);
                end = StartDate.Date.Add(tEnd);

                if (end <= start)
                {
                    end = end.AddDays(1);
                }
                return true;
            }

            return false;
        }

        public void CalculateEstimatedPrice()
        {
            if (SelectedSpace == null || !TryGetDateTimes(out var start, out var end))
            {
                IsTimeRangeSelected = false;
                EstimatedTotalAmount = 0;
                DepositAmount = 0;
                OnPropertyChanged(nameof(DurationDisplay));
                return;
            }

            IsTimeRangeSelected = true;
            TimeSpan diff = end - start;
            DurationHours = Math.Max(0.5, diff.TotalHours);

            // Chỉ tính tiền theo giờ (theo yêu cầu)
            EstimatedTotalAmount = (decimal)DurationHours * SelectedSpace.HourlyRate;

            if (IsDepositPayment)
            {
                // Setup tiền cọc mặc định là 30% so với tổng tiền và tiền cọc có thể thay đổi tùy ý
                DepositAmount = Math.Round(EstimatedTotalAmount * 0.3m / 1000) * 1000;
            }
            else
            {
                DepositAmount = EstimatedTotalAmount;
            }

            OnPropertyChanged(nameof(DurationDisplay));
        }

        private void BookSpace(object? parameter)
        {
            if (SelectedSpace == null)
            {
                MessageBox.Show("Vui lòng chọn khu vực/sảnh sự kiện!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BookingCustomerName))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng hoặc chọn phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BookingCustomerName.Any(char.IsDigit))
            {
                MessageBox.Show("Họ & Tên khách hàng không được chứa chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsDepositInvalid)
            {
                MessageBox.Show("Số tiền cọc không được vượt quá Tổng tiền dự kiến! Vui lòng điều chỉnh lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsSelectionContinuous)
            {
                MessageBox.Show("Khung giờ đặt sảnh phải liên tục không ngắt quãng trong 1 lần tiếp nhận! Vui lòng chọn lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryGetDateTimes(out var start, out var end))
            {
                MessageBox.Show("Vui lòng chọn khung giờ thuê hợp lệ (Từ - Đến)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra trùng lịch đặt
            bool isConflict = Data.EventBookings.Any(b => 
                b.SpaceId == SelectedSpace.Id &&
                b.PaymentStatus != "Đã hủy" &&
                b.StartTime < end && b.EndTime > start);

            if (isConflict)
            {
                MessageBox.Show($"Khu vực [{SelectedSpace.Name}] đã có đơn đặt trùng vào khung giờ {start:HH:mm dd/MM} - {end:HH:mm dd/MM}! Vui lòng chọn khung giờ khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isRoomCharge = BookingBillingType == BillingType.ChargeToRoom && CanChooseBookingRoomBill;
            string method = isRoomCharge ? "Ghi nợ vào phòng" : (IsBookingBankTransfer ? "Chuyển khoản" : "Tiền mặt");
            string code = $"SK-{DateTime.Now:yyyyMMdd}-{Data.EventBookings.Count + 1:D2}";
            string roomNum = DataService.NormalizeRoomNumber(BookingRoomNumber);
            decimal actualCollect = IsFullPayment ? EstimatedTotalAmount : DepositAmount;
            string staff = CreatorStaffDisplay;

            var booking = new EventBooking
            {
                BookingCode = code,
                SpaceId = SelectedSpace.Id,
                SpaceName = SelectedSpace.Name,
                CustomerName = BookingCustomerName.Trim(),
                PhoneNumber = BookingPhoneNumber.Trim(),
                RoomNumber = roomNum,
                StartTime = start,
                EndTime = end,
                DepositAmount = actualCollect,
                TotalEstimatedAmount = EstimatedTotalAmount,
                DamageNote = BookingNote?.Trim() ?? "",
                PaymentMethod = method,
                DaThanhToan = !isRoomCharge && IsFullPayment,
                PaymentStatus = isRoomCharge ? "Ghi nợ vào phòng" : (IsFullPayment ? "Đã thanh toán full" : "Đã cọc"),
                RecordedBy = staff,
                PaidByStaffName = (!isRoomCharge && IsFullPayment) ? staff : "",
                ThoiGianThanhToan = (!isRoomCharge && IsFullPayment) ? DateTime.Now : null
            };

            // Mở hóa đơn cọc / thu tiền
            var win = new Views.InvoiceBillWindow(booking);
            bool? res = win.ShowDialog();

            if (res == true && win.IsConfirmed)
            {
                Data.AddEventBooking(booking);
                BuildHourlySchedule();

                BookingCustomerName = "";
                BookingPhoneNumber = "";
                BookingRoomNumber = "";
                BookingNote = "";
                BookingBillingType = BillingType.DirectPayment;
                ClearSlotSelection();
                OnPropertyChanged(nameof(PendingEventBookings));
                SelectedBookingToComplete = PendingEventBookings.FirstOrDefault();
            }
        }

        private void CompleteBooking(object? parameter)
        {
            if (SelectedBookingToComplete == null || !IsSelectedBookingInUse)
            {
                if (SelectedBookingToComplete != null)
                {
                    if (SelectedBookingToComplete.OrderStatus == "Hoàn tất" || SelectedBookingToComplete.PaymentStatus == "Hoàn tất" || SelectedBookingToComplete.PaymentStatus == "Đã hoàn tất")
                    {
                        MessageBox.Show("Đơn sự kiện này đã được hoàn tất và thanh toán trước đó!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    if (SelectedBookingToComplete.StartTime > DateTime.Now)
                    {
                        MessageBox.Show("Sự kiện này chưa diễn ra! Chỉ có thể quyết toán khi sự kiện đang diễn ra hoặc đã đến giờ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                MessageBox.Show("Vui lòng chọn một đơn sự kiện đang sử dụng để bàn giao & quyết toán!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var targetBooking = SelectedBookingToComplete;
            bool isRoomCharge = SettlementBillingType == BillingType.ChargeToRoom && CanChargeSettlementToRoom;
            string paymentMethod = isRoomCharge ? "Ghi nợ vào phòng" : (IsSettlementBankTransfer ? "Chuyển khoản" : "Tiền mặt");

            decimal prevAdditionalCost = targetBooking.AdditionalCost;
            string prevDamageNote = targetBooking.DamageNote;
            string prevStatus = targetBooking.PaymentStatus;
            string prevOrderStatus = targetBooking.OrderStatus;
            bool prevDaThanhToan = targetBooking.DaThanhToan;
            string prevPayer = targetBooking.PaidByStaffName;
            DateTime? prevThoiGianTT = targetBooking.ThoiGianThanhToan;

            targetBooking.AdditionalCost = ExtraFee;
            targetBooking.DamageNote = DamageNote;
            targetBooking.OrderStatus = "Hoàn tất";
            targetBooking.PaymentStatus = isRoomCharge ? "Ghi nợ vào phòng" : "Hoàn tất";
            targetBooking.DaThanhToan = !isRoomCharge;
            targetBooking.PaidByStaffName = CreatorStaffDisplay;
            targetBooking.ThoiGianThanhToan = DateTime.Now;

            var win = new Views.InvoiceBillWindow(targetBooking);
            bool? res = win.ShowDialog();

            if (res == true && win.IsConfirmed)
            {
                Data.CompleteEventBooking(targetBooking, ExtraFee, DamageNote, isRoomCharge, paymentMethod);
                BuildHourlySchedule();

                ExtraFee = 0;
                DamageNote = "";
                OnPropertyChanged(nameof(PendingEventBookings));
                OnPropertyChanged(nameof(SelectableEventBookings));
                ApplyBookingFilter();
                NotifySettlementCalculationChanged();

                MessageBox.Show(
                    "Thanh toán thành công!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                // Revert
                targetBooking.AdditionalCost = prevAdditionalCost;
                targetBooking.DamageNote = prevDamageNote;
                targetBooking.OrderStatus = prevOrderStatus;
                targetBooking.PaymentStatus = prevStatus;
                targetBooking.DaThanhToan = prevDaThanhToan;
                targetBooking.PaidByStaffName = prevPayer;
                targetBooking.ThoiGianThanhToan = prevThoiGianTT;
            }
        }

        private void EditEventSpace(object? parameter)
        {
            if (parameter is EventSpace sp)
            {
                var dlg = new Views.EditEventSpaceDialogWindow(sp);
                if (dlg.ShowDialog() == true)
                {
                    CalculateEstimatedPrice();
                    if (IsTimetableTab) BuildHourlySchedule();
                }
            }
        }

        public event Action<EventBooking>? RequestScrollBookingIntoView;

        /// <summary>
        /// Điều hướng và chọn đơn đặt sự kiện trong bảng lịch sử theo mã đơn hoặc số phòng
        /// </summary>
        public bool SelectBookingByCode(string? bookingCode, string? roomNumber = null)
        {
            TimeFilter = "All";
            StatusFilter = "All";
            SearchBookingQuery = "";
            ApplyBookingFilter();

            EventBooking? target = null;
            if (!string.IsNullOrWhiteSpace(bookingCode))
            {
                string clean = bookingCode.Trim();
                target = FilteredEventBookings.FirstOrDefault(b => 
                    string.Equals(b.BookingCode, clean, StringComparison.OrdinalIgnoreCase));
            }

            if (target == null && !string.IsNullOrWhiteSpace(roomNumber))
            {
                string norm = DataService.NormalizeRoomNumber(roomNumber);
                target = FilteredEventBookings.FirstOrDefault(b => 
                    !b.DaThanhToan && 
                    DataService.NormalizeRoomNumber(b.RoomNumber) == norm);
            }

            if (target != null)
            {
                SelectedBookingToComplete = target;
                RequestScrollBookingIntoView?.Invoke(target);
                return true;
            }
            return false;
        }
    }
}
