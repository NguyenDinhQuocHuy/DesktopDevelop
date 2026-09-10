using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class LaundryViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;
        public AuthService Auth => AuthService.Instance;

        public bool IsAdmin => Auth.IsAdmin;
        public bool IsReceptionist => Auth.IsReceptionist;

        // Đơn giặt mới (Lễ tân)
        private bool _isDeferredPartner = true;
        public bool IsDeferredPartner
        {
            get => _isDeferredPartner;
            set
            {
                if (SetProperty(ref _isDeferredPartner, value))
                {
                    if (value) SelectedPartner = null;
                    else if (SelectedPartner == null) SelectedPartner = Data.LaundryPartners.FirstOrDefault(p => p.IsActive);
                    OnPropertyChanged(nameof(CanSelectPartner));
                }
            }
        }
        public bool CanSelectPartner => !IsDeferredPartner;

        private LaundryPartner? _selectedPartner;
        public LaundryPartner? SelectedPartner
        {
            get => _selectedPartner;
            set
            {
                if (SetProperty(ref _selectedPartner, value))
                {
                    if (value != null && _isDeferredPartner)
                    {
                        _isDeferredPartner = false;
                        OnPropertyChanged(nameof(IsDeferredPartner));
                    }
                    OnPropertyChanged(nameof(CanSelectPartner));
                }
            }
        }

        private LaundryServiceType _selectedServiceType = LaundryServiceType.WashAndDry;
        public LaundryServiceType SelectedServiceType
        {
            get => _selectedServiceType;
            set
            {
                if (SetProperty(ref _selectedServiceType, value))
                {
                    AutoSetPricePerKg();
                }
            }
        }

        private string _customerName = "";
        public string CustomerName { get => _customerName; set => SetProperty(ref _customerName, value); }

        private string _phoneNumber = "";
        public string PhoneNumber { get => _phoneNumber; set => SetProperty(ref _phoneNumber, value); }

        private string _roomNumber = "";
        public string RoomNumber
        {
            get => _roomNumber;
            set
            {
                if (SetProperty(ref _roomNumber, value))
                {
                    AutoFillCustomerInfo(value);
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
                    CustomerName = room.CustomerName;
                    if (!string.IsNullOrWhiteSpace(room.PhoneNumber)) PhoneNumber = room.PhoneNumber;
                }
                else
                {
                    CustomerName = "";
                    PhoneNumber = "";
                }
            }
            else
            {
                CustomerName = "";
                PhoneNumber = "";
            }
        }

        private decimal? _weightKg = null;
        public decimal? WeightKg
        {
            get => _weightKg;
            set { if (SetProperty(ref _weightKg, value)) CalculateTotal(); }
        }

        private string _weightInputStr = "";
        public string WeightInputStr
        {
            get => _weightInputStr;
            set
            {
                if (SetProperty(ref _weightInputStr, value))
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        WeightKg = null;
                    }
                    else if (decimal.TryParse(value.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val))
                    {
                        WeightKg = Math.Max(0, Math.Round(val, 2));
                    }
                    else
                    {
                        WeightKg = null;
                    }
                }
            }
        }

        private decimal _unitPricePerKg = 30000;
        public decimal UnitPricePerKg
        {
            get => _unitPricePerKg;
            set { if (SetProperty(ref _unitPricePerKg, value)) CalculateTotal(); }
        }

        private decimal _totalPrice;
        public decimal TotalPrice { get => _totalPrice; set => SetProperty(ref _totalPrice, value); }

        private string _clothesConditionNote = "";
        public string ClothesConditionNote { get => _clothesConditionNote; set => SetProperty(ref _clothesConditionNote, value); }

        private DateTime _appointmentDate = DateTime.Today;
        public DateTime AppointmentDate { get => _appointmentDate; set => SetProperty(ref _appointmentDate, value); }

        private string _appointmentTimeStr = "00:00";
        public string AppointmentTimeStr { get => _appointmentTimeStr; set => SetProperty(ref _appointmentTimeStr, value); }

        // Phương thức thanh toán: Ghi nợ vào phòng vs Thanh toán trực tiếp
        private string _paymentMethod = "Ghi nợ vào phòng";
        public string PaymentMethod { get => _paymentMethod; set => SetProperty(ref _paymentMethod, value); }

        public bool IsDirectPayment
        {
            get => PaymentMethod == "Thanh toán trực tiếp";
            set
            {
                if (value) PaymentMethod = "Thanh toán trực tiếp";
                OnPropertyChanged(nameof(IsDirectPayment));
                OnPropertyChanged(nameof(IsRoomChargePayment));
            }
        }

        public bool IsRoomChargePayment
        {
            get => PaymentMethod == "Ghi nợ vào phòng";
            set
            {
                if (value) PaymentMethod = "Ghi nợ vào phòng";
                OnPropertyChanged(nameof(IsDirectPayment));
                OnPropertyChanged(nameof(IsRoomChargePayment));
            }
        }

        // Hình thức thu tiền (khi thanh toán trực tiếp): Tiền mặt vs Chuyển khoản
        private string _paymentCollectionMethod = "Tiền mặt";
        public string PaymentCollectionMethod
        {
            get => _paymentCollectionMethod;
            set
            {
                if (SetProperty(ref _paymentCollectionMethod, value))
                {
                    OnPropertyChanged(nameof(IsCashPayment));
                    OnPropertyChanged(nameof(IsTransferPayment));
                }
            }
        }

        public bool IsCashPayment
        {
            get => PaymentCollectionMethod == "Tiền mặt";
            set { if (value) PaymentCollectionMethod = "Tiền mặt"; }
        }

        public bool IsTransferPayment
        {
            get => PaymentCollectionMethod == "Chuyển khoản";
            set { if (value) PaymentCollectionMethod = "Chuyển khoản"; }
        }

        // ==================== GIAI ĐOẠN 2: TRẢ ĐỒ & QUYẾT TOÁN ====================
        public System.Collections.ObjectModel.ObservableCollection<LaundryOrder> PendingReturns { get; } = new();

        // Bộ lọc thời gian đơn giặt ủi: "Today" (mặc định), "All", "Custom"
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
                    ApplyLaundryFilter();
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
                    ApplyLaundryFilter();
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
                    ApplyLaundryFilter();
                }
            }
        }

        // Bộ lọc trạng thái đơn giặt: "All" (mặc định), "PendingDispatch", "Washing", "Washed", "Completed", "Cancelled"
        private string _statusFilter = "All";
        public string StatusFilter
        {
            get => _statusFilter;
            set
            {
                if (SetProperty(ref _statusFilter, value))
                {
                    OnPropertyChanged(nameof(IsAllStatusFilter));
                    OnPropertyChanged(nameof(IsPendingDispatchStatusFilter));
                    OnPropertyChanged(nameof(IsWashingStatusFilter));
                    OnPropertyChanged(nameof(IsWashedStatusFilter));
                    OnPropertyChanged(nameof(IsCompletedStatusFilter));
                    OnPropertyChanged(nameof(IsCancelledStatusFilter));
                    ApplyLaundryFilter();
                }
            }
        }

        public bool IsAllStatusFilter { get => StatusFilter == "All"; set { if (value) StatusFilter = "All"; } }
        public bool IsPendingDispatchStatusFilter { get => StatusFilter == "PendingDispatch"; set { if (value) StatusFilter = "PendingDispatch"; } }
        public bool IsWashingStatusFilter { get => StatusFilter == "Washing"; set { if (value) StatusFilter = "Washing"; } }
        public bool IsWashedStatusFilter { get => StatusFilter == "Washed"; set { if (value) StatusFilter = "Washed"; } }
        public bool IsCompletedStatusFilter { get => StatusFilter == "Completed"; set { if (value) StatusFilter = "Completed"; } }
        public bool IsCancelledStatusFilter { get => StatusFilter == "Cancelled"; set { if (value) StatusFilter = "Cancelled"; } }

        public ICommand SetTimeFilterCommand { get; }
        public ICommand SetStatusFilterCommand { get; }

        // Tìm kiếm khách hàng theo tên / số điện thoại real-time (Debounced 180ms)
        private string _searchLaundryQuery = "";
        public string SearchLaundryQuery
        {
            get => _searchLaundryQuery;
            set
            {
                if (SetProperty(ref _searchLaundryQuery, value))
                {
                    Debounce("LaundryOrderSearch", ApplyLaundryFilter, 180);
                }
            }
        }

        public ObservableRangeCollection<LaundryOrder> FilteredLaundryOrders { get; } = new();

        public void ApplyLaundryFilter()
        {
            var list = Data.LaundryOrders.AsEnumerable();

            // 1. Lọc theo trạng thái đơn giặt
            if (!string.IsNullOrWhiteSpace(StatusFilter) && StatusFilter != "All")
            {
                if (Enum.TryParse<LaundryStatus>(StatusFilter, out var parsedStatus))
                {
                    list = list.Where(o => o.Status == parsedStatus);
                }
            }

            // 2. Lọc theo thời gian (Hôm Nay / Tất Cả / Khoảng Ngày)
            var today = DateTime.Today;
            if (TimeFilter == "Today")
            {
                list = list.Where(o =>
                    o.ReceivedDate.Date <= today && (
                        o.ReceivedDate.Date == today ||
                        o.AppointmentDate.Date == today ||
                        (o.ActualReturnDate.HasValue && o.ActualReturnDate.Value.Date == today) ||
                        (o.AppointmentDate.Date >= today && o.Status != LaundryStatus.Completed && o.Status != LaundryStatus.Cancelled)
                    )
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
                list = list.Where(o =>
                    (o.ReceivedDate.Date >= start && o.ReceivedDate.Date <= end) ||
                    (o.AppointmentDate.Date >= start && o.AppointmentDate.Date <= end) ||
                    (o.ActualReturnDate.HasValue && o.ActualReturnDate.Value.Date >= start && o.ActualReturnDate.Value.Date <= end) ||
                    (o.ReceivedDate.Date <= start && o.AppointmentDate.Date >= end)
                );
            }

            // 3. Tìm kiếm từ khóa Real-time
            if (!string.IsNullOrWhiteSpace(SearchLaundryQuery))
            {
                string rawQ = SearchLaundryQuery.Trim();
                string cleanQ = TextSearchHelper.ToSearchKey(rawQ);
                list = list.Where(o =>
                    (!string.IsNullOrWhiteSpace(o.PhoneNumber) && o.PhoneNumber.Contains(rawQ)) ||
                    TextSearchHelper.MatchPrepared(o.CustomerName, cleanQ) ||
                    (!string.IsNullOrWhiteSpace(o.RoomNumber) && o.RoomNumber.Contains(rawQ)) ||
                    TextSearchHelper.MatchPrepared(o.PartnerName, cleanQ) ||
                    (!string.IsNullOrWhiteSpace(o.OrderCode) && o.OrderCode.IndexOf(rawQ, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }
            FilteredLaundryOrders.ReplaceRange(list.ToList());
        }

        private LaundryOrder? _selectedOrderToReturn;
        public LaundryOrder? SelectedOrderToReturn
        {
            get => _selectedOrderToReturn;
            set
            {
                if (SetProperty(ref _selectedOrderToReturn, value))
                {
                    OnPropertyChanged(nameof(HasSelectedOrderToReturn));
                    OnPropertyChanged(nameof(ReturnGrandTotal));
                    OnPropertyChanged(nameof(SettlementButtonLabel));
                }
            }
        }
        public bool HasSelectedOrderToReturn => SelectedOrderToReturn != null;

        private decimal _returnExtraCost = 0;
        public decimal ReturnExtraCost
        {
            get => _returnExtraCost;
            set
            {
                if (SetProperty(ref _returnExtraCost, value))
                {
                    OnPropertyChanged(nameof(ReturnGrandTotal));
                }
            }
        }

        public decimal ReturnGrandTotal => (SelectedOrderToReturn?.TotalPrice ?? 0m) + ReturnExtraCost;

        // Quản lý đền bù thiệt hại
        private bool _hasCompensation = false;
        public bool HasCompensation
        {
            get => _hasCompensation;
            set
            {
                if (SetProperty(ref _hasCompensation, value))
                {
                    OnPropertyChanged(nameof(ShowCompensationFields));
                }
            }
        }
        public bool ShowCompensationFields => HasCompensation;

        private string _compensationPayer = "DoiTac"; // DoiTac, KhachSan, KhongCo
        public string CompensationPayer
        {
            get => _compensationPayer;
            set
            {
                if (SetProperty(ref _compensationPayer, value))
                {
                    OnPropertyChanged(nameof(IsDoiTacPayer));
                    OnPropertyChanged(nameof(IsKhachSanPayer));
                }
            }
        }
        public bool IsDoiTacPayer
        {
            get => CompensationPayer == "DoiTac";
            set { if (value) CompensationPayer = "DoiTac"; }
        }
        public bool IsKhachSanPayer
        {
            get => CompensationPayer == "KhachSan";
            set { if (value) CompensationPayer = "KhachSan"; }
        }

        private decimal _compensationAmount = 0;
        public decimal CompensationAmount
        {
            get => _compensationAmount;
            set => SetProperty(ref _compensationAmount, value);
        }

        private string _compensationReason = "";
        public string CompensationReason
        {
            get => _compensationReason;
            set => SetProperty(ref _compensationReason, value);
        }

        public string SettlementButtonLabel => "HOÀN TẤT TRẢ ĐỒ";

        // Quản lý Đối tác (Admin)
        private string _newPartnerName = "";
        public string NewPartnerName { get => _newPartnerName; set => SetProperty(ref _newPartnerName, value); }

        private string _newPartnerAddress = "";
        public string NewPartnerAddress { get => _newPartnerAddress; set => SetProperty(ref _newPartnerAddress, value); }

        private string _newPartnerPhone = "";
        public string NewPartnerPhone { get => _newPartnerPhone; set => SetProperty(ref _newPartnerPhone, value); }

        private decimal _newHotelCommission = 30.0m;
        public decimal NewHotelCommission { get => _newHotelCommission; set => SetProperty(ref _newHotelCommission, value); }

        private readonly System.Windows.Threading.DispatcherTimer _laundryTimer;

        public ICommand CreateOrderCommand { get; }
        public ICommand AddPartnerCommand { get; }
        public ICommand PrintSlipCommand { get; }
        public ICommand DispatchOrderCommand { get; }
        public ICommand IncreaseWeightCommand { get; }
        public ICommand DecreaseWeightCommand { get; }
        public ICommand CompleteReturnCommand { get; }
        public ICommand CancelOrderCommand { get; }

        public LaundryViewModel()
        {
            SetTimeFilterCommand = new RelayCommand(p => { if (p is string m) TimeFilter = m; });
            SetStatusFilterCommand = new RelayCommand(p => { if (p is string s) StatusFilter = s; });
            CreateOrderCommand = new RelayCommand(CreateOrder);
            AddPartnerCommand = new RelayCommand(AddPartner);
            PrintSlipCommand = new RelayCommand(PrintSlip);
            DispatchOrderCommand = new RelayCommand(DispatchOrder);
            CompleteReturnCommand = new RelayCommand(CompleteReturn);
            CancelOrderCommand = new RelayCommand(p => { if (p is LaundryOrder o) CancelOrder(o); });

            IncreaseWeightCommand = new RelayCommand(_ =>
            {
                decimal current = WeightKg ?? 0m;
                decimal next = Math.Floor(current) + 1.0m;
                WeightKg = next;
                WeightInputStr = next.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
            });

            DecreaseWeightCommand = new RelayCommand(_ =>
            {
                decimal current = WeightKg ?? 1.0m;
                decimal next = Math.Ceiling(current) - 1.0m;
                if (next < 1.0m) next = 1.0m;
                WeightKg = next;
                WeightInputStr = next.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
            });

            SelectedPartner = null; // Mặc định có thể để trống khi nhận đồ, giao sau
            AutoSetPricePerKg();

            Data.LaundryOrders.CollectionChanged += (s, e) =>
            {
                RefreshPendingReturns();
                ApplyLaundryFilter();
            };
            RefreshPendingReturns();
            ApplyLaundryFilter();

            // Background timer kiểm tra hẹn trả đồ (báo trước 15 phút, tự chuyển Giặt xong khi đến giờ)
            _laundryTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _laundryTimer.Tick += (s, e) => CheckLaundryAppointments();
            _laundryTimer.Start();
        }

        public void RefreshPendingReturns()
        {
            var currentSelectedId = SelectedOrderToReturn?.Id;
            PendingReturns.Clear();
            foreach (var order in Data.LaundryOrders.Where(x => x.Status == LaundryStatus.Washing || x.Status == LaundryStatus.Washed))
            {
                PendingReturns.Add(order);
            }

            if (currentSelectedId.HasValue)
            {
                SelectedOrderToReturn = PendingReturns.FirstOrDefault(x => x.Id == currentSelectedId.Value);
            }
            if (SelectedOrderToReturn == null && PendingReturns.Count > 0)
            {
                SelectedOrderToReturn = PendingReturns[0];
            }
            ApplyLaundryFilter();
        }

        private void CheckLaundryAppointments()
        {
            var now = DateTime.Now;
            bool hasChanged = false;
            foreach (var order in Data.LaundryOrders.Where(o => o.Status == LaundryStatus.Washing).ToList())
            {
                var diff = order.AppointmentDate - now;
                // Trước 15 phút hoặc đã qua giờ hẹn trả: thông báo và chuyển sang 'Giặt xong'
                if (diff.TotalMinutes <= 15)
                {
                    order.Status = LaundryStatus.Washed;
                    hasChanged = true;
                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "UPDATE DonGiatUi SET TrangThai = N'Giặt xong' WHERE Id = @Id OR MaDon = @Ma",
                        new Microsoft.Data.SqlClient.SqlParameter("@Id", order.Id),
                        new Microsoft.Data.SqlClient.SqlParameter("@Ma", order.OrderCode)
                    );
                    NotificationService.Instance.AddNotification(
                        "Đến Giờ Trả Đồ Giặt Ủi",
                        $"Đơn [{order.OrderCode}] của khách phòng [{order.RoomNumber}] ({order.CustomerName}) đã đến giờ trả đồ cho khách! Trạng thái chuyển sang: GIẶT XONG.",
                        NotificationType.LaundryReturn
                    );
                }
            }

            if (hasChanged)
            {
                RefreshPendingReturns();
            }
        }

        private void AutoSetPricePerKg()
        {
            UnitPricePerKg = SelectedServiceType switch
            {
                LaundryServiceType.WashAndDry => 30000,
                LaundryServiceType.DryCleaning => 80000,
                LaundryServiceType.Ironing => 40000,
                _ => 30000
            };
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            TotalPrice = (WeightKg ?? 0m) * UnitPricePerKg;
        }

        private void CreateOrder(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(RoomNumber))
            {
                MessageBox.Show("⚠️ Dịch vụ Giặt ủi chỉ nhận phục vụ khách lưu trú trong phòng khách sạn (không nhận khách vãng lai)!\n\nVui lòng chọn số phòng của khách.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string normRoom = DataService.NormalizeRoomNumber(RoomNumber);
            var checkRoom = Data.HotelRooms.FirstOrDefault(r => string.Equals(r.RoomNumber, normRoom, StringComparison.OrdinalIgnoreCase));
            if (checkRoom == null || checkRoom.Status != RoomStatus.Occupied)
            {
                MessageBox.Show($"⚠️ Phòng [{normRoom}] hiện không có khách đang lưu trú! Dịch vụ giặt ủi chỉ nhận khách đang ở phòng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (WeightKg == null || WeightKg <= 0)
            {
                MessageBox.Show("Khối lượng đồ giặt không được để trống và phải lớn hơn 0 kg!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(CustomerName) && CustomerName.Any(char.IsDigit))
            {
                MessageBox.Show("Họ & Tên khách hàng không được chứa chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(PhoneNumber) && PhoneNumber.Any(c => !char.IsDigit(c)))
            {
                MessageBox.Show("Số điện thoại chỉ được chứa các chữ số (0-9)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string custName = string.IsNullOrWhiteSpace(CustomerName) ? checkRoom.CustomerName : CustomerName.Trim();
            string phone = string.IsNullOrWhiteSpace(PhoneNumber) ? checkRoom.PhoneNumber : PhoneNumber.Trim();

            DateTime fullAppointment = AppointmentDate.Date;
            if (TimeSpan.TryParse(AppointmentTimeStr, out var timeSpan))
            {
                fullAppointment = fullAppointment.Add(timeSpan);
            }
            else
            {
                fullAppointment = fullAppointment.AddHours(17);
            }

            bool daTT = PaymentMethod == "Thanh toán trực tiếp";
            string staff = Auth.IsAdmin ? "Chủ khách sạn" : Data.GetCurrentDutyStaffName(DateTime.Now);
            int staffId = Data.GetCurrentDutyStaffId();

            int partnerId = SelectedPartner?.Id ?? 0;
            string partnerName = SelectedPartner?.Name ?? "Chờ giao tiệm";
            LaundryStatus status = SelectedPartner != null ? LaundryStatus.Washing : LaundryStatus.PendingDispatch;

            string finalPayMethod = daTT 
                ? $"Thanh toán trực tiếp ({PaymentCollectionMethod})" 
                : "Ghi nợ vào phòng";

            var order = new LaundryOrder
            {
                CustomerName = custName,
                PhoneNumber = phone,
                RoomNumber = normRoom,
                PartnerId = partnerId,
                PartnerName = partnerName,
                ServiceType = SelectedServiceType,
                WeightKg = WeightKg.Value,
                UnitPricePerKg = UnitPricePerKg,
                ClothesConditionNote = ClothesConditionNote?.Trim() ?? "",
                ReceivedDate = DateTime.Now,
                AppointmentDate = fullAppointment,
                PaymentMethod = finalPayMethod,
                Status = status,
                DaThanhToan = daTT,
                NguoiTaoId = staffId,
                NguoiThanhToanId = daTT ? staffId : null,
                ThoiGianThanhToan = daTT ? DateTime.Now : null,
                PaidByStaffName = daTT ? staff : "",
                RecordedBy = staff
            };

            var win = new Views.InvoiceBillWindow(order);
            bool? res = win.ShowDialog();

            if (res == true && win.IsConfirmed)
            {
                Data.AddLaundryOrder(order, SelectedPartner);
                CustomerName = "";
                PhoneNumber = "";
                RoomNumber = "";
                ClothesConditionNote = "";
                WeightKg = null;
                WeightInputStr = "";
                AppointmentDate = DateTime.Today;
                AppointmentTimeStr = "00:00";
                RefreshPendingReturns();

                string statusMsg = daTT 
                    ? "Đơn đã được thanh toán ngay và ghi nhận hoàn tất thu tiền!" 
                    : "Đơn đã ghi nợ vào tiền phòng và sẽ thanh toán khi khách trả phòng!";
                MessageBox.Show($"Đã tạo đơn tiếp nhận giặt ủi [{order.OrderCode}] thành công!\n{statusMsg}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CompleteReturn(object? parameter)
        {
            if (SelectedOrderToReturn == null)
            {
                MessageBox.Show("Vui lòng chọn đơn giặt ủi cần trả đồ từ danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (HasCompensation)
            {
                if (CompensationAmount <= 0)
                {
                    MessageBox.Show("Vui lòng nhập số tiền đền bù thiệt hại hợp lệ (> 0 đ)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(CompensationReason))
                {
                    MessageBox.Show("Vui lòng ghi rõ lý do/tình trạng đền bù thiệt hại cho khách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var order = SelectedOrderToReturn;
            string payerName = CompensationPayer == "DoiTac" ? $"Đối tác [{order.PartnerName}]" : "Khách sạn";

            var confirmResult = MessageBox.Show(
                $"Xác nhận hoàn tất trả đồ cho đơn [{order.OrderCode}] - Khách: {order.CustomerName} (P.{order.RoomNumber})?\n\n" +
                $"- Chi phí phát sinh: {ReturnExtraCost:N0} đ\n" +
                (HasCompensation ? $"- Đền bù thiệt hại: {CompensationAmount:N0} đ (Bên chịu: {payerName})\n  (Số tiền đền bù sẽ được khấu trừ vào hóa đơn phòng và trừ công nợ đối tác)\n" : "- Không có đền bù thiệt hại\n"),
                "Thông báo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirmResult != MessageBoxResult.Yes) return;

            Data.CompleteLaundryOrderReturn(
                order, 
                ReturnExtraCost, 
                HasCompensation, 
                CompensationPayer, 
                CompensationAmount, 
                CompensationReason
            );

            NotificationService.Instance.AddNotification(
                "Hoàn Tất Trả Đồ Giặt Ủi",
                $"Đã hoàn tất trả đồ cho đơn [{order.OrderCode}] phòng [{order.RoomNumber}].",
                NotificationType.LaundryReturn
            );

            // Reset form trả đồ
            ReturnExtraCost = 0;
            HasCompensation = false;
            CompensationAmount = 0;
            CompensationReason = "";
            RefreshPendingReturns();
        }

        private void DispatchOrder(object? parameter)
        {
            if (parameter is not LaundryOrder order) return;

            if (order.Status == LaundryStatus.Completed)
            {
                MessageBox.Show("Đơn hàng này đã hoàn tất giặt xong!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var activePartners = Data.LaundryPartners.Where(p => p.IsActive).ToList();
            if (!activePartners.Any())
            {
                MessageBox.Show("Hiện không có đối tác giặt ủi nào đang hoạt động để bàn giao!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new Views.SelectPartnerDialogWindow(activePartners, order);
            if (dialog.ShowDialog() == true && dialog.SelectedPartner != null)
            {
                Data.AssignPartnerToLaundryOrder(order, dialog.SelectedPartner);
                RefreshPendingReturns();
                MessageBox.Show($"Đã bàn giao đơn hàng {order.OrderCode} cho đối tác [{dialog.SelectedPartner.Name}] thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddPartner(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(NewPartnerName) || string.IsNullOrWhiteSpace(NewPartnerPhone))
            {
                MessageBox.Show("Vui lòng nhập tên đối tác và số điện thoại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newId = Data.LaundryPartners.Count > 0 ? Data.LaundryPartners.Max(x => x.Id) + 1 : 1;
            decimal partnerRate = 100.0m - NewHotelCommission;
            decimal costSay = Math.Round(30000m * (partnerRate / 100m), 0);
            decimal costHap = Math.Round(80000m * (partnerRate / 100m), 0);
            decimal costUi = Math.Round(40000m * (partnerRate / 100m), 0);

            var partner = new LaundryPartner
            {
                Id = newId,
                Name = NewPartnerName.Trim(),
                Address = NewPartnerAddress?.Trim() ?? "Địa chỉ đang cập nhật",
                PhoneNumber = NewPartnerPhone.Trim(),
                HotelCommissionRate = NewHotelCommission,
                GiaVonGiatSay = costSay,
                GiaVonGiatHap = costHap,
                GiaVonUiPhang = costUi
            };

            Data.LaundryPartners.Add(partner);
            NewPartnerName = "";
            NewPartnerAddress = "";
            NewPartnerPhone = "";

            // Đồng bộ xuống CSDL SQL Server (Table DoiTacGiatUi & BangGiaDoiTacGiatUi)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "INSERT INTO DoiTacGiatUi (TenDoiTac, DiaChi, SoDienThoai, TyLeKhachSanHuong, TyLeDoiTacHuong, TrangThai) " +
                "VALUES (@Ten, @DiaChi, @Phone, @TyLeKS, @TyLeDT, 1)",
                new Microsoft.Data.SqlClient.SqlParameter("@Ten", partner.Name),
                new Microsoft.Data.SqlClient.SqlParameter("@DiaChi", partner.Address),
                new Microsoft.Data.SqlClient.SqlParameter("@Phone", partner.PhoneNumber),
                new Microsoft.Data.SqlClient.SqlParameter("@TyLeKS", partner.HotelCommissionRate),
                new Microsoft.Data.SqlClient.SqlParameter("@TyLeDT", partner.PartnerRate)
            );
            _ = DatabaseService.Instance.SavePartnerCostPricesAsync(newId, costSay, costHap, costUi);

            MessageBox.Show($"Đã thêm đối tác giặt ủi mới: [{partner.Name}] kèm bảng giá vốn (Sấy: {costSay:N0}đ, Hấp: {costHap:N0}đ, Ủi: {costUi:N0}đ)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PrintSlip(object? parameter)
        {
            if (parameter is LaundryOrder order)
            {
                var win = new Views.InvoiceBillWindow(order);
                win.ShowDialog();
            }
        }

        public void CancelOrder(LaundryOrder? order)
        {
            if (order == null) return;

            if (order.Status != LaundryStatus.PendingDispatch)
            {
                MessageBox.Show($"Chỉ có thể hủy đơn giặt khi đang ở trạng thái 'Chờ giao tiệm'!\nĐơn [{order.OrderCode}] hiện đang ở trạng thái: {order.StatusDisplay}.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string refundDetail = order.DaThanhToan
                ? $"• Khách đã thanh toán trực tiếp: {order.TotalPrice:N0} đ\n  -> Hệ thống sẽ ghi nhận một khoản HOÀN TIỀN {order.TotalPrice:N0} đ vào dịch vụ phòng để giảm trừ hóa đơn phòng cho khách."
                : $"• Khách ghi nợ vào phòng: {order.TotalPrice:N0} đ\n  -> Hệ thống sẽ XÓA khoản tiền giặt ủi này khỏi hóa đơn dịch vụ phòng.";

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn HỦY đơn giặt ủi sau đây không?\n\n" +
                $"• Mã đơn: {order.OrderCode}\n" +
                $"• Khách hàng: {order.CustomerName} (Phòng: {order.RoomNumber})\n" +
                $"• Dịch vụ: {order.ServiceTypeDisplay} - {order.WeightKg} kg\n" +
                $"• Tổng tiền: {order.TotalPrice:N0} đ ({order.PaymentMethod})\n\n" +
                $"{refundDetail}\n\n" +
                "Sau khi hủy, trạng thái đơn sẽ chuyển sang 'Đã hủy' và không thể khôi phục.",
                "Thông báo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            bool ok = Data.CancelLaundryOrder(order);
            if (ok)
            {
                ApplyLaundryFilter();
                RefreshPendingReturns();
                MessageBox.Show($"Đã hủy đơn giặt ủi [{order.OrderCode}] thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public event Action<LaundryOrder>? RequestScrollOrderIntoView;

        /// <summary>
        /// Điều hướng và chọn đơn giặt ủi trong bảng lịch sử theo mã đơn hoặc số phòng
        /// </summary>
        public bool SelectOrderByCode(string? orderCode, string? roomNumber = null)
        {
            TimeFilter = "All";
            StatusFilter = "All";
            SearchLaundryQuery = "";
            ApplyLaundryFilter();

            LaundryOrder? target = null;
            if (!string.IsNullOrWhiteSpace(orderCode))
            {
                string clean = orderCode.Trim();
                target = FilteredLaundryOrders.FirstOrDefault(o => 
                    string.Equals(o.OrderCode, clean, StringComparison.OrdinalIgnoreCase));
            }

            if (target == null && !string.IsNullOrWhiteSpace(roomNumber))
            {
                string norm = DataService.NormalizeRoomNumber(roomNumber);
                target = FilteredLaundryOrders.FirstOrDefault(o => 
                    !o.DaThanhToan && 
                    DataService.NormalizeRoomNumber(o.RoomNumber) == norm);
            }

            if (target != null)
            {
                SelectedOrderToReturn = target;
                RequestScrollOrderIntoView?.Invoke(target);
                return true;
            }
            return false;
        }
    }
}
