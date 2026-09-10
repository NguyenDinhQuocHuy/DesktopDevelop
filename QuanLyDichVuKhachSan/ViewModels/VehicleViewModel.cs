using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class VehicleViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;
        public AuthService Auth => AuthService.Instance;

        public bool IsAdmin => Auth.IsAdmin;
        public bool IsReceptionist => Auth.IsReceptionist;

        private readonly DispatcherTimer _vehicleReminderTimer;

        // Bộ lọc trạng thái xe
        private string _statusFilter = "All"; // All (Tất cả), Available (Sẵn sàng), Rented (Đang thuê), Maintenance (Bảo trì)
        public string StatusFilter
        {
            get => _statusFilter;
            set
            {
                if (SetProperty(ref _statusFilter, value))
                {
                    OnPropertyChanged(nameof(IsAllFleetFilter));
                    OnPropertyChanged(nameof(IsAvailableFleetFilter));
                    OnPropertyChanged(nameof(IsRentedFleetFilter));
                    OnPropertyChanged(nameof(IsMaintenanceFleetFilter));
                    ApplyVehicleFilter();
                }
            }
        }

        public bool IsAllFleetFilter { get => StatusFilter == "All"; set { if (value) StatusFilter = "All"; } }
        public bool IsAvailableFleetFilter { get => StatusFilter == "Available"; set { if (value) StatusFilter = "Available"; } }
        public bool IsRentedFleetFilter { get => StatusFilter == "Rented"; set { if (value) StatusFilter = "Rented"; } }
        public bool IsMaintenanceFleetFilter { get => StatusFilter == "Maintenance"; set { if (value) StatusFilter = "Maintenance"; } }

        public ObservableRangeCollection<Vehicle> FilteredVehicles { get; } = new();
        public ObservableRangeCollection<Vehicle> AvailableVehiclesForRent { get; } = new();

        // Đơn thuê xe mới (Lễ tân)
        private Vehicle? _selectedVehicle;
        public Vehicle? SelectedVehicle
        {
            get => _selectedVehicle;
            set
            {
                if (SetProperty(ref _selectedVehicle, value))
                {
                    CalculateRentalFee();
                }
            }
        }

        private string _rentalCustomerName = "";
        public string RentalCustomerName { get => _rentalCustomerName; set => SetProperty(ref _rentalCustomerName, value); }

        private string _rentalIdentityCard = "";
        public string RentalIdentityCard { get => _rentalIdentityCard; set => SetProperty(ref _rentalIdentityCard, value); }

        private string _rentalPhoneNumber = "";
        public string RentalPhoneNumber { get => _rentalPhoneNumber; set => SetProperty(ref _rentalPhoneNumber, value); }

        private string _rentalReceptionNote = "";
        public string RentalReceptionNote { get => _rentalReceptionNote; set => SetProperty(ref _rentalReceptionNote, value); }

        private string _rentalRoomNumber = "";
        public string RentalRoomNumber
        {
            get => _rentalRoomNumber;
            set
            {
                if (SetProperty(ref _rentalRoomNumber, value))
                {
                    AutoFillCustomerInfo(value);
                    if (!CanChooseRentalRoomBill && RentalBillingType == BillingType.ChargeToRoom)
                    {
                        RentalBillingType = BillingType.DirectPayment;
                    }
                    OnPropertyChanged(nameof(CanChooseRentalRoomBill));
                    OnPropertyChanged(nameof(IsDirectRentalPayment));
                    OnPropertyChanged(nameof(IsRoomChargeRentalPayment));
                    OnPropertyChanged(nameof(RentalRoomChargeNoticeText));
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
                    RentalCustomerName = room.CustomerName;
                    if (!string.IsNullOrWhiteSpace(room.PhoneNumber)) RentalPhoneNumber = room.PhoneNumber;
                    if (!string.IsNullOrWhiteSpace(room.IdentityCard)) RentalIdentityCard = room.IdentityCard;
                }
                else
                {
                    RentalCustomerName = "";
                    RentalPhoneNumber = "";
                    RentalIdentityCard = "";
                }
            }
            else
            {
                RentalCustomerName = "";
                RentalPhoneNumber = "";
                RentalIdentityCard = "";
            }
        }

        private DateTime _rentalStartDate = DateTime.Today;
        public DateTime RentalStartDate
        {
            get => _rentalStartDate;
            set { if (SetProperty(ref _rentalStartDate, value)) CalculateRentalFee(); }
        }

        private string _rentalStartTimeStr = "08:00";
        public string RentalStartTimeStr
        {
            get => _rentalStartTimeStr;
            set { if (SetProperty(ref _rentalStartTimeStr, value)) CalculateRentalFee(); }
        }

        private DateTime _rentalEndDate = DateTime.Today.AddDays(1);
        public DateTime RentalEndDate
        {
            get => _rentalEndDate;
            set { if (SetProperty(ref _rentalEndDate, value)) CalculateRentalFee(); }
        }

        private string _rentalEndTimeStr = "12:00"; // Mốc trả xe mặc định là 12h trưa
        public string RentalEndTimeStr
        {
            get => _rentalEndTimeStr;
            set { if (SetProperty(ref _rentalEndTimeStr, value)) CalculateRentalFee(); }
        }

        private decimal _calculatedFee;
        public decimal CalculatedFee
        {
            get => _calculatedFee;
            set
            {
                if (SetProperty(ref _calculatedFee, value))
                {
                    if (IsFullPayment)
                    {
                        _depositAmount = value;
                        OnPropertyChanged(nameof(DepositAmount));
                    }
                    NotifyPaymentCalculationChanged();
                }
            }
        }

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
                        DepositAmount = Math.Round(CalculatedFee * 0.3m / 1000) * 1000;
                        if (DepositAmount > CalculatedFee) DepositAmount = CalculatedFee;
                    }
                    else
                    {
                        DepositAmount = CalculatedFee;
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

        public bool IsDepositInvalid => IsDepositPayment && DepositAmount > CalculatedFee;
        public string DepositErrorMessage => IsDepositInvalid ? "⚠️ Tiền cọc không được vượt quá Tiền thuê tính toán!" : "";
        public decimal RemainingEstimatedFee => Math.Max(0, CalculatedFee - (IsFullPayment ? CalculatedFee : DepositAmount));
        public decimal CurrentCollectAmount => IsFullPayment ? CalculatedFee : DepositAmount;
        public string RentButtonLabel => "XÁC NHẬN CHO THUÊ";

        private string _rentalPaymentMethod = "Tiền mặt";
        public string RentalPaymentMethod
        {
            get => _rentalPaymentMethod;
            set => SetProperty(ref _rentalPaymentMethod, value);
        }

        private BillingType _rentalBillingType = BillingType.DirectPayment;
        public BillingType RentalBillingType
        {
            get => _rentalBillingType;
            set
            {
                if (SetProperty(ref _rentalBillingType, value))
                {
                    OnPropertyChanged(nameof(IsDirectRentalPayment));
                    OnPropertyChanged(nameof(IsRoomChargeRentalPayment));
                    OnPropertyChanged(nameof(RentalRoomChargeNoticeText));
                }
            }
        }

        public bool CanChooseRentalRoomBill => !string.IsNullOrWhiteSpace(DataService.NormalizeRoomNumber(RentalRoomNumber));
        public bool IsDirectRentalPayment => RentalBillingType == BillingType.DirectPayment;
        public bool IsRoomChargeRentalPayment => RentalBillingType == BillingType.ChargeToRoom;
        public string RentalRoomChargeNoticeText => $"💡 Tiền thuê/cọc xe ({CurrentCollectAmount:N0} đ) sẽ được ghi nợ vào hóa đơn phòng {RentalRoomNumber}. Khách sẽ thanh toán khi Check-out.";

        private bool _isRentalCash = true;
        public bool IsRentalCash
        {
            get => _isRentalCash;
            set
            {
                if (SetProperty(ref _isRentalCash, value))
                {
                    if (value) _isRentalBankTransfer = false;
                    OnPropertyChanged(nameof(IsRentalBankTransfer));
                    if (value) RentalPaymentMethod = "Tiền mặt";
                }
            }
        }

        private bool _isRentalBankTransfer;
        public bool IsRentalBankTransfer
        {
            get => _isRentalBankTransfer;
            set
            {
                if (SetProperty(ref _isRentalBankTransfer, value))
                {
                    if (value) _isRentalCash = false;
                    OnPropertyChanged(nameof(IsRentalCash));
                    if (value) RentalPaymentMethod = "Chuyển khoản";
                }
            }
        }

        public string CreatorStaffDisplay => Auth.IsAdmin ? "Chủ khách sạn" : Data.GetCurrentDutyStaffName(DateTime.Now);

        private void NotifyPaymentCalculationChanged()
        {
            OnPropertyChanged(nameof(IsDepositInvalid));
            OnPropertyChanged(nameof(DepositErrorMessage));
            OnPropertyChanged(nameof(RemainingEstimatedFee));
            OnPropertyChanged(nameof(CurrentCollectAmount));
            OnPropertyChanged(nameof(RentButtonLabel));
            OnPropertyChanged(nameof(RentalRoomChargeNoticeText));
        }

        private string _calculatedDaysDisplay = "1 ngày";
        public string CalculatedDaysDisplay
        {
            get => _calculatedDaysDisplay;
            set => SetProperty(ref _calculatedDaysDisplay, value);
        }

        // GIAI ĐOẠN 2: TRẢ XE & QUYẾT TOÁN
        public System.Collections.Generic.IEnumerable<VehicleRental> SelectableVehicleRentals => Data.VehicleRentals;

        public System.Collections.Generic.IEnumerable<VehicleRental> PendingVehicleRentals =>
            Data.VehicleRentals.Where(r => r.Status == "Đang thuê");

        private VehicleRental? _selectedRentalToReturn;
        public VehicleRental? SelectedRentalToReturn
        {
            get => _selectedRentalToReturn;
            set
            {
                if (SetProperty(ref _selectedRentalToReturn, value))
                {
                    if (!CanChargeSettlementToRoom)
                    {
                        SettlementBillingType = BillingType.DirectPayment;
                    }
                    RecalculateActualReturnDetails();
                    OnPropertyChanged(nameof(CanChargeSettlementToRoom));
                    OnPropertyChanged(nameof(IsDirectSettlementPayment));
                    OnPropertyChanged(nameof(IsRoomChargeSettlementPayment));
                    OnPropertyChanged(nameof(RoomChargeNoticeText));
                }
            }
        }

        private DateTime _actualReturnDate = DateTime.Now;
        public DateTime ActualReturnDate
        {
            get => _actualReturnDate;
            set
            {
                if (SetProperty(ref _actualReturnDate, value))
                {
                    RecalculateActualReturnDetails();
                }
            }
        }

        private double _actualDays = 1.0;
        public double ActualDays
        {
            get => _actualDays;
            set => SetProperty(ref _actualDays, value);
        }

        public string ActualDaysDisplay => $"{ActualDays:0.#} ngày";

        private decimal _actualRentalFee = 0;
        public decimal ActualRentalFee
        {
            get => _actualRentalFee;
            set => SetProperty(ref _actualRentalFee, value);
        }

        private decimal _returnExtraCost = 0;
        public decimal ReturnExtraCost
        {
            get => _returnExtraCost;
            set
            {
                if (SetProperty(ref _returnExtraCost, value))
                {
                    NotifySettlementCalculationChanged();
                }
            }
        }

        private string _vehicleCondition = "Nguyên vẹn";
        public string VehicleCondition
        {
            get => _vehicleCondition;
            set => SetProperty(ref _vehicleCondition, value);
        }

        public string[] VehicleConditions { get; } = new string[]
        {
            "Nguyên vẹn", "Trầy xước nhẹ", "Hư hỏng / Cần sửa chữa"
        };

        private string _returnDamageNote = "";
        public string ReturnDamageNote { get => _returnDamageNote; set => SetProperty(ref _returnDamageNote, value); }

        private string _settlementPaymentMethod = "Tiền mặt";
        public string SettlementPaymentMethod { get => _settlementPaymentMethod; set => SetProperty(ref _settlementPaymentMethod, value); }

        // Phương thức ghi nợ / thanh toán khi trả xe
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
            SelectedRentalToReturn != null &&
            !string.IsNullOrWhiteSpace(DataService.NormalizeRoomNumber(SelectedRentalToReturn.RoomNumber));

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

        public string RoomChargeNoticeText => SelectedRentalToReturn != null
            ? $"💡 Chi phí quyết toán ({SettlementRemainingToPay:N0} đ) sẽ được chuyển vào hóa đơn chi tiết phòng {SelectedRentalToReturn.RoomNumber}. Khách sẽ thanh toán khi Check-out."
            : "";

        public bool IsSelectedRentalActive => 
            SelectedRentalToReturn != null && 
            (SelectedRentalToReturn.Status == "Đang thuê" || 
             SelectedRentalToReturn.OrderStatus == "Đang thuê" ||
             (!SelectedRentalToReturn.DaTraXe && SelectedRentalToReturn.Status != "Hoàn tất" && !SelectedRentalToReturn.Status.StartsWith("Đã hủy") && SelectedRentalToReturn.RentalDate <= DateTime.Now));

        public bool CanCollectSettlement => SelectedRentalToReturn != null && IsSelectedRentalActive;

        public decimal SettlementGrandTotal => IsSelectedRentalActive ? (ActualRentalFee + ReturnExtraCost) : (SelectedRentalToReturn?.TotalPayment ?? 0);
        public decimal SettlementPaidBefore => SelectedRentalToReturn?.DepositAmount ?? 0;
        public decimal SettlementRemainingToPay
        {
            get
            {
                if (SelectedRentalToReturn == null) return 0;
                if (SelectedRentalToReturn.Status == "Hoàn tất" || 
                    SelectedRentalToReturn.Status == "Đã hoàn tất" || 
                    SelectedRentalToReturn.OrderStatus == "Hoàn tất" ||
                    SelectedRentalToReturn.Status.StartsWith("Đã hủy") ||
                    SelectedRentalToReturn.OrderStatus.StartsWith("Đã hủy") ||
                    SelectedRentalToReturn.DaTraXe)
                {
                    return 0;
                }
                if (!IsSelectedRentalActive)
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
                if (SelectedRentalToReturn == null) return "CHỌN ĐƠN THUÊ XE";
                if (SelectedRentalToReturn.Status == "Hoàn tất" || SelectedRentalToReturn.Status == "Đã hoàn tất" || SelectedRentalToReturn.OrderStatus == "Hoàn tất" || SelectedRentalToReturn.DaTraXe)
                {
                    return "ĐÃ HOÀN TẤT TRẢ XE";
                }
                if (SelectedRentalToReturn.Status.StartsWith("Đã hủy") || SelectedRentalToReturn.OrderStatus.StartsWith("Đã hủy"))
                {
                    return "HỢP ĐỒNG ĐÃ HỦY";
                }
                if (SelectedRentalToReturn.RentalDate > DateTime.Now && !IsSelectedRentalActive)
                {
                    return "CHƯA ĐẾN GIỜ NHẬN XE";
                }
                if (IsSelectedRentalActive)
                {
                    return "HOÀN TẤT TRẢ XE";
                }
                return "HOÀN TẤT TRẢ XE";
            }
        }

        private void RecalculateActualReturnDetails()
        {
            if (SelectedRentalToReturn == null)
            {
                ActualDays = 1.0;
                ActualRentalFee = 0;
            }
            else
            {
                ActualDays = CalculateVehicleRentalDays(SelectedRentalToReturn.RentalDate, ActualReturnDate);
                var veh = Data.Vehicles.FirstOrDefault(v => v.Id == SelectedRentalToReturn.VehicleId);
                decimal rate = veh?.DailyRate ?? 150000;
                ActualRentalFee = Math.Round((decimal)ActualDays * rate, 0);
            }

            OnPropertyChanged(nameof(ActualDaysDisplay));
            NotifySettlementCalculationChanged();
        }

        private void NotifySettlementCalculationChanged()
        {
            OnPropertyChanged(nameof(IsSelectedRentalActive));
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

        private int _extendDays = 1;
        public int ExtendDays
        {
            get => _extendDays;
            set => SetProperty(ref _extendDays, value);
        }

        // Thêm xe mới (Admin)
        private string _newPlate = "";
        public string NewPlate { get => _newPlate; set => SetProperty(ref _newPlate, value); }

        private string _newVehicleName = "";
        public string NewVehicleName { get => _newVehicleName; set => SetProperty(ref _newVehicleName, value); }

        private VehicleType _newVehicleType = VehicleType.Scooter;
        public VehicleType NewVehicleType { get => _newVehicleType; set => SetProperty(ref _newVehicleType, value); }

        private decimal _newDailyRate = 160000;
        public decimal NewDailyRate { get => _newDailyRate; set => SetProperty(ref _newDailyRate, value); }

        private decimal _newRequiredDeposit = 500000;
        public decimal NewRequiredDeposit { get => _newRequiredDeposit; set => SetProperty(ref _newRequiredDeposit, value); }

        public ICommand RentVehicleCommand { get; }
        public ICommand ReturnVehicleCommand { get; }
        public ICommand ExtendRentalCommand { get; }
        public ICommand AddNewVehicleCommand { get; }
        public ICommand PrintContractCommand { get; }
        public ICommand SelectVehicleCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand EditVehicleCommand { get; }
        public ICommand SetMaintenanceCommand { get; }
        public ICommand SetAvailableCommand { get; }

        // Tìm kiếm khách hàng theo tên / số điện thoại real-time (Debounced 180ms)
        private string _searchRentalQuery = "";
        public string SearchRentalQuery
        {
            get => _searchRentalQuery;
            set
            {
                if (SetProperty(ref _searchRentalQuery, value))
                {
                    Debounce("VehicleRentalSearch", ApplyRentalFilter, 180);
                }
            }
        }

        // Bộ lọc thời gian hợp đồng thuê: "Today" (mặc định), "All", "Custom"
        private string _rentalTimeFilter = "Today";
        public string RentalTimeFilter
        {
            get => _rentalTimeFilter;
            set
            {
                if (SetProperty(ref _rentalTimeFilter, value))
                {
                    OnPropertyChanged(nameof(IsTodayRentalTimeFilter));
                    OnPropertyChanged(nameof(IsAllRentalTimeFilter));
                    OnPropertyChanged(nameof(IsCustomRentalTimeFilter));
                    ApplyRentalFilter();
                }
            }
        }

        public bool IsTodayRentalTimeFilter
        {
            get => RentalTimeFilter == "Today";
            set { if (value) RentalTimeFilter = "Today"; }
        }

        public bool IsAllRentalTimeFilter
        {
            get => RentalTimeFilter == "All";
            set { if (value) RentalTimeFilter = "All"; }
        }

        public bool IsCustomRentalTimeFilter
        {
            get => RentalTimeFilter == "Custom";
            set { if (value) RentalTimeFilter = "Custom"; }
        }

        private DateTime? _rentalFromDate = DateTime.Today;
        public DateTime? RentalFromDate
        {
            get => _rentalFromDate;
            set
            {
                if (SetProperty(ref _rentalFromDate, value))
                {
                    RentalTimeFilter = "Custom";
                    ApplyRentalFilter();
                }
            }
        }

        private DateTime? _rentalToDate = DateTime.Today.AddDays(1);
        public DateTime? RentalToDate
        {
            get => _rentalToDate;
            set
            {
                if (SetProperty(ref _rentalToDate, value))
                {
                    RentalTimeFilter = "Custom";
                    ApplyRentalFilter();
                }
            }
        }

        // Bộ lọc trạng thái hợp đồng thuê xe: "All" (mặc định), "Đang thuê", "Quá hạn", "Đã cọc", "Hoàn tất", "Đã hủy"
        private string _rentalStatusFilter = "All";
        public string RentalStatusFilter
        {
            get => _rentalStatusFilter;
            set
            {
                if (SetProperty(ref _rentalStatusFilter, value))
                {
                    OnPropertyChanged(nameof(IsAllRentalStatusFilter));
                    OnPropertyChanged(nameof(IsDepositedRentalStatusFilter));
                    OnPropertyChanged(nameof(IsFullPaidRentalStatusFilter));
                    OnPropertyChanged(nameof(IsRentingRentalStatusFilter));
                    OnPropertyChanged(nameof(IsCompletedRentalStatusFilter));
                    OnPropertyChanged(nameof(IsCancelledRentalStatusFilter));
                    ApplyRentalFilter();
                }
            }
        }

        public bool IsAllRentalStatusFilter { get => RentalStatusFilter == "All"; set { if (value) RentalStatusFilter = "All"; } }
        public bool IsDepositedRentalStatusFilter { get => RentalStatusFilter == "Đã cọc"; set { if (value) RentalStatusFilter = "Đã cọc"; } }
        public bool IsFullPaidRentalStatusFilter { get => RentalStatusFilter == "Đã thanh toán full" || RentalStatusFilter == "Đã thanh toán hết"; set { if (value) RentalStatusFilter = "Đã thanh toán full"; } }
        public bool IsRentingRentalStatusFilter { get => RentalStatusFilter == "Đang thuê"; set { if (value) RentalStatusFilter = "Đang thuê"; } }
        public bool IsCompletedRentalStatusFilter { get => RentalStatusFilter == "Hoàn tất"; set { if (value) RentalStatusFilter = "Hoàn tất"; } }
        public bool IsCancelledRentalStatusFilter { get => RentalStatusFilter == "Đã hủy"; set { if (value) RentalStatusFilter = "Đã hủy"; } }

        public ICommand SetRentalTimeFilterCommand { get; }
        public ICommand SetRentalStatusFilterCommand { get; }

        public ObservableRangeCollection<VehicleRental> FilteredRentals { get; } = new();

        public void ApplyRentalFilter()
        {
            var list = Data.VehicleRentals.AsEnumerable();

            // 1. Lọc theo trạng thái hợp đồng thuê
            if (!string.IsNullOrWhiteSpace(RentalStatusFilter) && RentalStatusFilter != "All")
            {
                if (RentalStatusFilter == "Đã cọc")
                {
                    list = list.Where(r => (r.InitialPaymentType == "Đặt cọc trước" || r.Status == "Đã cọc") && r.Status != "Đang thuê" && r.OrderStatus != "Đang thuê" && r.Status != "Hoàn tất" && !r.Status.StartsWith("Đã hủy"));
                }
                else if (RentalStatusFilter == "Đã thanh toán full" || RentalStatusFilter == "Đã thanh toán hết")
                {
                    list = list.Where(r => (r.InitialPaymentType == "Thanh toán toàn bộ" || r.Status == "Đã thanh toán full" || r.Status == "Đã thanh toán hết") && r.Status != "Đang thuê" && r.OrderStatus != "Đang thuê" && r.Status != "Hoàn tất" && !r.Status.StartsWith("Đã hủy"));
                }
                else if (RentalStatusFilter == "Đang thuê")
                {
                    list = list.Where(r => r.Status == "Đang thuê" || r.OrderStatus == "Đang thuê");
                }
                else if (RentalStatusFilter == "Hoàn tất")
                {
                    list = list.Where(r => r.Status == "Hoàn tất" || r.Status == "Đã hoàn tất" || r.OrderStatus == "Hoàn tất");
                }
                else if (RentalStatusFilter == "Đã hủy")
                {
                    list = list.Where(r => r.Status.StartsWith("Đã hủy") || r.OrderStatus.StartsWith("Đã hủy"));
                }
                else
                {
                    list = list.Where(r => string.Equals(r.Status, RentalStatusFilter, StringComparison.OrdinalIgnoreCase));
                }
            }

            // 2. Lọc theo thời gian (Hôm Nay / Tất Cả / Khoảng Ngày)
            var today = DateTime.Today;
            if (RentalTimeFilter == "Today")
            {
                list = list.Where(r =>
                    r.RentalDate.Date <= today && (
                        r.RentalDate.Date == today ||
                        r.ExpectedReturnDate.Date == today ||
                        (r.ActualReturnDate.HasValue && r.ActualReturnDate.Value.Date == today) ||
                        r.Status == "Đang thuê" ||
                        r.IsOverdue
                    )
                );
            }
            else if (RentalTimeFilter == "Custom")
            {
                var start = (RentalFromDate ?? DateTime.Today).Date;
                var end = (RentalToDate ?? DateTime.Today.AddDays(1)).Date;
                if (start > end)
                {
                    var temp = start;
                    start = end;
                    end = temp;
                }
                list = list.Where(r =>
                    (r.RentalDate.Date >= start && r.RentalDate.Date <= end) ||
                    (r.ExpectedReturnDate.Date >= start && r.ExpectedReturnDate.Date <= end) ||
                    (r.ActualReturnDate.HasValue && r.ActualReturnDate.Value.Date >= start && r.ActualReturnDate.Value.Date <= end) ||
                    (r.RentalDate.Date <= start && r.ExpectedReturnDate.Date >= end)
                );
            }

            // 3. Tìm kiếm từ khóa Real-time
            if (!string.IsNullOrWhiteSpace(SearchRentalQuery))
            {
                string rawQ = SearchRentalQuery.Trim();
                string cleanQ = TextSearchHelper.ToSearchKey(rawQ);
                list = list.Where(r =>
                    (!string.IsNullOrWhiteSpace(r.PhoneNumber) && r.PhoneNumber.Contains(rawQ)) ||
                    TextSearchHelper.MatchPrepared(r.CustomerName, cleanQ) ||
                    TextSearchHelper.MatchPrepared(r.VehicleName, cleanQ) ||
                    (!string.IsNullOrWhiteSpace(r.RoomNumber) && r.RoomNumber.Contains(rawQ)) ||
                    (!string.IsNullOrWhiteSpace(r.LicensePlate) && r.LicensePlate.IndexOf(rawQ, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrWhiteSpace(r.RentalCode) && r.RentalCode.IndexOf(rawQ, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }
            FilteredRentals.ReplaceRange(list.ToList());
        }

        public VehicleViewModel()
        {
            SetRentalTimeFilterCommand = new RelayCommand(p => { if (p is string m) RentalTimeFilter = m; });
            SetRentalStatusFilterCommand = new RelayCommand(p => { if (p is string s) RentalStatusFilter = s; });
            RentVehicleCommand = new RelayCommand(RentVehicle);
            ReturnVehicleCommand = new RelayCommand(ReturnVehicle);
            ExtendRentalCommand = new RelayCommand(ExtendRental);
            AddNewVehicleCommand = new RelayCommand(AddNewVehicle);
            PrintContractCommand = new RelayCommand(PrintContract);
            SelectVehicleCommand = new RelayCommand(p => { if (p is Vehicle v && v.Status == VehicleStatus.Available) SelectedVehicle = v; });
            FilterCommand = new RelayCommand(p => { if (p is string s) StatusFilter = s; });
            EditVehicleCommand = new RelayCommand(EditVehicle);
            SetMaintenanceCommand = new RelayCommand(SetMaintenance);
            SetAvailableCommand = new RelayCommand(SetAvailable);

            ApplyVehicleFilter();
            ApplyRentalFilter();
            Data.VehicleRentals.CollectionChanged += (s, e) => ApplyRentalFilter();

            SelectedVehicle = AvailableVehiclesForRent.FirstOrDefault();
            SelectedRentalToReturn = Data.VehicleRentals.FirstOrDefault(r => r.Status == "Đang thuê") ?? Data.VehicleRentals.FirstOrDefault();
            CalculateRentalFee();

            // Real-time reminder timer (30 phút trước giờ trả xe)
            _vehicleReminderTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _vehicleReminderTimer.Tick += (s, e) => CheckVehicleDueReminders();
            _vehicleReminderTimer.Start();
        }

        private void SetMaintenance(object? parameter)
        {
            if (parameter is Vehicle v)
            {
                if (v.Status == VehicleStatus.Rented)
                {
                    MessageBox.Show($"Xe [{v.Name} - {v.LicensePlate}] hiện đang cho khách thuê, không thể chuyển sang bảo trì!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                v.Status = VehicleStatus.Maintenance;
                ApplyVehicleFilter();
                if (SelectedVehicle == v) SelectedVehicle = AvailableVehiclesForRent.FirstOrDefault();
                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    "UPDATE XeChoThue SET TrangThai = N'Bảo trì' WHERE Id = @Id OR BienSo = @BienSo",
                    new Microsoft.Data.SqlClient.SqlParameter("@Id", v.Id),
                    new Microsoft.Data.SqlClient.SqlParameter("@BienSo", v.LicensePlate)
                );
                MessageBox.Show($"Đã chuyển xe [{v.Name} - {v.LicensePlate}] sang trạng thái BẢO TRÌ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SetAvailable(object? parameter)
        {
            if (parameter is Vehicle v)
            {
                v.Status = VehicleStatus.Available;
                ApplyVehicleFilter();
                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    "UPDATE XeChoThue SET TrangThai = N'Sẵn sàng' WHERE Id = @Id OR BienSo = @BienSo",
                    new Microsoft.Data.SqlClient.SqlParameter("@Id", v.Id),
                    new Microsoft.Data.SqlClient.SqlParameter("@BienSo", v.LicensePlate)
                );
                MessageBox.Show($"Đã chuyển xe [{v.Name} - {v.LicensePlate}] sang trạng thái SẴN SÀNG phục vụ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CheckVehicleDueReminders()
        {
            NotificationService.Instance.ScanOperationalAlerts();
        }

        private void EditVehicle(object? parameter)
        {
            if (parameter is Vehicle v)
            {
                var dlg = new Views.EditVehicleDialogWindow(v);
                if (dlg.ShowDialog() == true)
                {
                    ApplyVehicleFilter();
                }
            }
        }

        public void ApplyVehicleFilter()
        {
            Data.SyncVehicleFleetStatus();
            var list = Data.Vehicles.AsEnumerable();

            if (StatusFilter == "Available")
            {
                list = list.Where(v => v.Status == VehicleStatus.Available);
            }
            else if (StatusFilter == "Rented")
            {
                list = list.Where(v => v.Status == VehicleStatus.Rented);
            }
            else if (StatusFilter == "Maintenance")
            {
                list = list.Where(v => v.Status == VehicleStatus.Maintenance);
            }

            FilteredVehicles.ReplaceRange(list.ToList());
            AvailableVehiclesForRent.ReplaceRange(Data.Vehicles.Where(v => v.Status == VehicleStatus.Available).ToList());

            OnPropertyChanged(nameof(AvailableVehiclesForRent));
        }

        private bool TryGetRentalDateTimes(out DateTime start, out DateTime end)
        {
            start = RentalStartDate.Date;
            end = RentalEndDate.Date;

            if (!TimeSpan.TryParse(RentalStartTimeStr, out var startTime))
            {
                startTime = new TimeSpan(8, 0, 0);
            }
            if (!TimeSpan.TryParse(RentalEndTimeStr, out var endTime))
            {
                endTime = new TimeSpan(12, 0, 0);
            }

            start = RentalStartDate.Date.Add(startTime);
            end = RentalEndDate.Date.Add(endTime);

            if (end <= start) end = start.AddDays(1);
            return true;
        }

        public static double CalculateVehicleRentalDays(DateTime start, DateTime end)
        {
            // Nếu start và end cùng ngày:
            if (start.Date == end.Date || (end <= start.AddHours(24) && start.Date == end.Date))
            {
                return 1.0; // Khách chỉ thuê trong 1 ngày (sáng đến tối hoặc nửa ngày) đều tính 1 ngày
            }

            // Nếu thuê từ 2 ngày trở đi:
            // Mốc sáng (05:00 - 12:00) -> tính 1 ngày. Mốc chiều tối (12:00 - 24:00) -> tính 0.5 ngày.
            double startDayCost = start.Hour >= 12 ? 0.5 : 1.0;

            // Số ngày trọn vẹn ở giữa
            int fullDaysBetween = Math.Max(0, (end.Date - start.Date).Days - 1);

            // Mốc trả xe: trước hoặc đúng 12:00 trưa -> tính 0.5 ngày; sau 12:00 trưa -> tính 1 ngày
            double endDayCost = (end.Hour < 12 || (end.Hour == 12 && end.Minute == 0)) ? 0.5 : 1.0;

            double totalDays = startDayCost + fullDaysBetween + endDayCost;
            return Math.Max(1.0, totalDays);
        }

        private void CalculateRentalFee()
        {
            if (SelectedVehicle == null)
            {
                CalculatedDaysDisplay = "";
                CalculatedFee = 0;
                DepositAmount = 0;
                return;
            }

            TryGetRentalDateTimes(out var start, out var end);
            double days = CalculateVehicleRentalDays(start, end);

            CalculatedDaysDisplay = $"{days:0.#} ngày";
            CalculatedFee = Math.Round((decimal)days * SelectedVehicle.DailyRate, 0);

            if (IsDepositPayment)
            {
                // Setup tiền cọc mặc định là 30% so với tổng tiền và tiền cọc có thể thay đổi tùy ý
                DepositAmount = Math.Round(CalculatedFee * 0.3m / 1000) * 1000;
            }
            else
            {
                DepositAmount = CalculatedFee;
            }
        }

        private void RentVehicle(object? parameter)
        {
            if (SelectedVehicle == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần cho thuê từ danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedVehicle.Status != VehicleStatus.Available)
            {
                MessageBox.Show($"Xe [{SelectedVehicle.LicensePlate}] hiện đang ở trạng thái '{SelectedVehicle.StatusDisplay}', không thể cho thuê!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(RentalCustomerName) || string.IsNullOrWhiteSpace(RentalIdentityCard) || string.IsNullOrWhiteSpace(RentalPhoneNumber))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên, CCCD và Số điện thoại khách thuê xe!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RentalCustomerName.Any(char.IsDigit))
            {
                MessageBox.Show("Họ & Tên khách hàng không được chứa chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RentalPhoneNumber.Any(c => !char.IsDigit(c)))
            {
                MessageBox.Show("Số điện thoại chỉ được chứa các chữ số (0-9)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsDepositInvalid)
            {
                MessageBox.Show("Tiền cọc không được vượt quá Tổng tiền thuê tính toán! Vui lòng điều chỉnh lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isRoomCharge = RentalBillingType == BillingType.ChargeToRoom && CanChooseRentalRoomBill;
            string method = isRoomCharge ? "Ghi nợ vào phòng" : (IsRentalBankTransfer ? "Chuyển khoản" : "Tiền mặt");
            decimal actualCollect = IsFullPayment ? CalculatedFee : DepositAmount;
            string staff = CreatorStaffDisplay;
            string code = $"TX-{DateTime.Now:yyyyMMdd}-{Data.VehicleRentals.Count + 1:D2}";
            TryGetRentalDateTimes(out var start, out var end);

            var rental = new VehicleRental
            {
                RentalCode = code,
                VehicleId = SelectedVehicle.Id,
                VehicleName = SelectedVehicle.Name,
                LicensePlate = SelectedVehicle.LicensePlate,
                CustomerName = RentalCustomerName.Trim(),
                IdentityCard = RentalIdentityCard.Trim(),
                PhoneNumber = RentalPhoneNumber.Trim(),
                RoomNumber = DataService.NormalizeRoomNumber(RentalRoomNumber),
                RentalDate = start,
                ExpectedReturnDate = end,
                DepositAmount = actualCollect,
                RentalFee = CalculatedFee,
                TotalPayment = CalculatedFee,
                PaymentMethod = method,
                PaymentStatus = isRoomCharge ? "Ghi nợ vào phòng" : (IsFullPayment ? "Đã thanh toán" : "Đã đặt cọc"),
                DaThanhToan = !isRoomCharge && IsFullPayment,
                ReceptionNote = RentalReceptionNote.Trim(),
                Status = "Đang thuê",
                RecordedBy = staff,
                PaidByStaffName = (!isRoomCharge && IsFullPayment) ? staff : "",
                ThoiGianThanhToan = (!isRoomCharge && IsFullPayment) ? DateTime.Now : null
            };

            var win = new Views.InvoiceBillWindow(rental);
            bool? res = win.ShowDialog();

            if (res == true && win.IsConfirmed)
            {
                Data.AddVehicleRental(rental);
                SelectedVehicle.Status = VehicleStatus.Rented;
                ApplyVehicleFilter();

                RentalCustomerName = "";
                RentalIdentityCard = "";
                RentalPhoneNumber = "";
                RentalRoomNumber = "";
                RentalReceptionNote = "";
                RentalBillingType = BillingType.DirectPayment;
                SelectedVehicle = null;
                CalculateRentalFee();

                OnPropertyChanged(nameof(PendingVehicleRentals));
                SelectedRentalToReturn = PendingVehicleRentals.FirstOrDefault();
            }
        }

        private void ExtendRental(object? parameter)
        {
            if (SelectedRentalToReturn == null)
            {
                MessageBox.Show("Vui lòng chọn đơn thuê xe cần gia hạn từ danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ExtendDays <= 0)
            {
                MessageBox.Show("Số ngày gia hạn phải từ 1 ngày trở lên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var vehicle = Data.Vehicles.FirstOrDefault(v => v.Id == SelectedRentalToReturn.VehicleId);
            decimal rate = vehicle?.DailyRate ?? 150000;
            decimal extraFee = ExtendDays * rate;

            SelectedRentalToReturn.ExpectedReturnDate = SelectedRentalToReturn.ExpectedReturnDate.AddDays(ExtendDays);
            SelectedRentalToReturn.RentalFee += extraFee;
            SelectedRentalToReturn.TotalPayment += extraFee;
            SelectedRentalToReturn.Status = "Đang thuê";

            // Đồng bộ xuống CSDL SQL Server (Table DonThueXe)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE DonThueXe SET NgayTraDuKien = @NgayTra, TienThue = @Tien, TongTienThanhToan = @Tong, TrangThai = N'Đang thuê' WHERE MaDon = @Ma",
                new Microsoft.Data.SqlClient.SqlParameter("@NgayTra", SelectedRentalToReturn.ExpectedReturnDate),
                new Microsoft.Data.SqlClient.SqlParameter("@Tien", SelectedRentalToReturn.RentalFee),
                new Microsoft.Data.SqlClient.SqlParameter("@Tong", SelectedRentalToReturn.TotalPayment),
                new Microsoft.Data.SqlClient.SqlParameter("@Ma", SelectedRentalToReturn.RentalCode)
            );

            RecalculateActualReturnDetails();

            MessageBox.Show(
                $"✅ Gia hạn thuê xe thành công!\n\n" +
                $"- Xe: {SelectedRentalToReturn.VehicleName} ({SelectedRentalToReturn.LicensePlate})\n" +
                $"- Khách: {SelectedRentalToReturn.CustomerName}\n" +
                $"- Gia hạn thêm: {ExtendDays} ngày\n" +
                $"- Hạn trả mới: {SelectedRentalToReturn.ExpectedReturnDate:dd/MM/yyyy HH:mm}\n" +
                $"- Phí gia hạn cộng thêm: {extraFee:N0} đ\n" +
                $"- Trạng thái: Đang thuê",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ReturnVehicle(object? parameter)
        {
            if (SelectedRentalToReturn == null || !IsSelectedRentalActive)
            {
                if (SelectedRentalToReturn != null)
                {
                    if (SelectedRentalToReturn.Status == "Hoàn tất" || SelectedRentalToReturn.OrderStatus == "Hoàn tất" || SelectedRentalToReturn.DaTraXe)
                    {
                        MessageBox.Show("Hợp đồng thuê xe này đã được hoàn tất trả xe và quyết toán!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    if (SelectedRentalToReturn.RentalDate > DateTime.Now)
                    {
                        MessageBox.Show("Hợp đồng này chưa đến giờ nhận xe! Không thể thực hiện trả xe.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                MessageBox.Show("Vui lòng chọn một đơn thuê xe đang hoạt động để thực hiện trả xe!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var rental = SelectedRentalToReturn;

            // Kiểm tra không nhận trả xe vào ban đêm (00:00 - 05:00) đối với khách vãng lai
            if (string.IsNullOrWhiteSpace(rental.RoomNumber))
            {
                var curHour = DateTime.Now.Hour;
                if (curHour >= 0 && curHour < 5)
                {
                    MessageBox.Show("Khách sạn không nhận trả xe vào ban đêm (sau 00:00 đến trước 05:00 sáng) đối với khách vãng lai!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            string combinedDamageNote = string.IsNullOrWhiteSpace(ReturnDamageNote) 
                ? VehicleCondition 
                : $"{VehicleCondition} - {ReturnDamageNote.Trim()}";

            bool isRoomCharge = SettlementBillingType == BillingType.ChargeToRoom && CanChargeSettlementToRoom;
            string paymentMethod = isRoomCharge ? "Ghi nợ vào phòng" : (IsSettlementBankTransfer ? "Chuyển khoản" : "Tiền mặt");

            decimal prevFee = rental.RentalFee;
            decimal prevExtra = rental.AdditionalCost;
            string prevDamage = rental.DamageNote;
            string prevStatus = rental.Status;
            string prevOrderStatus = rental.OrderStatus;
            bool prevPaid = rental.DaThanhToan;
            string prevPayer = rental.PaidByStaffName;
            DateTime? prevThoiGianTT = rental.ThoiGianThanhToan;

            rental.ActualReturnDate = ActualReturnDate;
            rental.RentalFee = ActualRentalFee;
            rental.AdditionalCost = ReturnExtraCost;
            rental.DamageNote = combinedDamageNote;
            rental.Status = "Hoàn tất";
            rental.OrderStatus = "Hoàn tất";
            rental.PaymentStatus = isRoomCharge ? "Ghi nợ vào phòng" : "Đã thanh toán";
            rental.DaThanhToan = !isRoomCharge;
            rental.PaidByStaffName = CreatorStaffDisplay;
            rental.ThoiGianThanhToan = DateTime.Now;

            var win = new Views.InvoiceBillWindow(rental);
            bool? res = win.ShowDialog();

            if (res == true && win.IsConfirmed)
            {
                Data.ReturnVehicle(rental, ReturnExtraCost, combinedDamageNote, isRoomCharge, paymentMethod);
                ApplyVehicleFilter();

                ReturnExtraCost = 0;
                ReturnDamageNote = "";
                VehicleCondition = "Nguyên vẹn";

                OnPropertyChanged(nameof(PendingVehicleRentals));
                OnPropertyChanged(nameof(SelectableVehicleRentals));
                ApplyRentalFilter();
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
                rental.RentalFee = prevFee;
                rental.AdditionalCost = prevExtra;
                rental.DamageNote = prevDamage;
                rental.Status = prevStatus;
                rental.OrderStatus = prevOrderStatus;
                rental.DaThanhToan = prevPaid;
                rental.PaidByStaffName = prevPayer;
                rental.ThoiGianThanhToan = prevThoiGianTT;
            }
        }

        private void AddNewVehicle(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(NewPlate) || string.IsNullOrWhiteSpace(NewVehicleName))
            {
                MessageBox.Show("Vui lòng nhập Biển số và Tên xe!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newId = Data.Vehicles.Count > 0 ? Data.Vehicles.Max(x => x.Id) + 1 : 1;
            var vehicle = new Vehicle
            {
                Id = newId,
                LicensePlate = NewPlate.Trim(),
                Name = NewVehicleName.Trim(),
                Type = NewVehicleType,
                DailyRate = NewDailyRate,
                RequiredDeposit = NewRequiredDeposit,
                Status = VehicleStatus.Available,
                ConditionNote = "Xe mới bàn giao",
                ImageUrl = "🛵"
            };

            Data.Vehicles.Add(vehicle);
            ApplyVehicleFilter();

            NewPlate = "";
            NewVehicleName = "";

            // Đồng bộ xuống CSDL SQL Server (Table XeChoThue)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "INSERT INTO XeChoThue (BienSo, TenXe, LoaiXe, GiaThueNgay, TienCocQuyDinh, TrangThai, GhiChuTinhTrang, HinhAnh) " +
                "VALUES (@BienSo, @Ten, @Loai, @Gia, @Coc, N'Sẵn sàng', @GhiChu, @Img)",
                new Microsoft.Data.SqlClient.SqlParameter("@BienSo", vehicle.LicensePlate),
                new Microsoft.Data.SqlClient.SqlParameter("@Ten", vehicle.Name),
                new Microsoft.Data.SqlClient.SqlParameter("@Loai", vehicle.TypeDisplay),
                new Microsoft.Data.SqlClient.SqlParameter("@Gia", vehicle.DailyRate),
                new Microsoft.Data.SqlClient.SqlParameter("@Coc", vehicle.RequiredDeposit),
                new Microsoft.Data.SqlClient.SqlParameter("@GhiChu", vehicle.ConditionNote ?? ""),
                new Microsoft.Data.SqlClient.SqlParameter("@Img", vehicle.ImageUrl)
            );

            MessageBox.Show($"Đã thêm xe mới: [{vehicle.Name} - {vehicle.LicensePlate}]!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PrintContract(object? parameter)
        {
            if (SelectedRentalToReturn == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng cần in!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var win = new Views.InvoiceBillWindow(SelectedRentalToReturn);
            win.ShowDialog();
        }

        public event Action<VehicleRental>? RequestScrollRentalIntoView;

        /// <summary>
        /// Điều hướng và chọn hợp đồng thuê xe trong bảng lịch sử theo mã hợp đồng hoặc số phòng
        /// </summary>
        public bool SelectRentalByCode(string? rentalCode, string? roomNumber = null)
        {
            RentalTimeFilter = "All";
            RentalStatusFilter = "All";
            SearchRentalQuery = "";
            ApplyRentalFilter();

            VehicleRental? target = null;
            if (!string.IsNullOrWhiteSpace(rentalCode))
            {
                string clean = rentalCode.Trim();
                target = FilteredRentals.FirstOrDefault(r => 
                    string.Equals(r.RentalCode, clean, StringComparison.OrdinalIgnoreCase));
            }

            if (target == null && !string.IsNullOrWhiteSpace(roomNumber))
            {
                string norm = DataService.NormalizeRoomNumber(roomNumber);
                target = FilteredRentals.FirstOrDefault(r => 
                    !r.DaThanhToan && 
                    DataService.NormalizeRoomNumber(r.RoomNumber) == norm);
            }

            if (target != null)
            {
                SelectedRentalToReturn = target;
                RequestScrollRentalIntoView?.Invoke(target);
                return true;
            }
            return false;
        }
    }
}
