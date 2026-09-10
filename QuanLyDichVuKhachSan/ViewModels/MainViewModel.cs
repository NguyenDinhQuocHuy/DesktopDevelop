using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;
using QuanLyDichVuKhachSan.Views;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public AuthService Auth => AuthService.Instance;
        public DataService Data => DataService.Instance;

        // ViewModels con
        public RoomMapViewModel RoomMapVM { get; }
        public FoodViewModel FoodVM { get; }
        public EventViewModel EventVM { get; }
        public VehicleViewModel VehicleVM { get; }
        public ParkingViewModel ParkingVM { get; }
        public LaundryViewModel LaundryVM { get; }
        public StaffViewModel StaffVM { get; }
        public StatisticsViewModel StatisticsVM { get; }
        public CustomerViewModel CustomerVM { get; }
        public InventoryViewModel InventoryVM { get; }
        public ServicesConfigViewModel ServicesConfigVM { get; }
        public TransactionHistoryViewModel TransactionsVM { get; }

        private string _currentTab = "RoomMap";
        public string CurrentTab
        {
            get => _currentTab;
            set => SetProperty(ref _currentTab, value);
        }

        public bool IsLoggedIn => Auth.IsLoggedIn;
        public bool IsAdmin => Auth.IsAdmin;
        public bool IsReceptionist => Auth.IsReceptionist;

        // Header role display: Chỉ hiện Admin hoặc Lễ Tân, không hiện họ tên
        public string CurrentUserRoleDisplay => Auth.IsAdmin ? "Admin" : "Lễ Tân";
        public string RoleBadgeBackground => Auth.IsAdmin ? "#D97706" : "#0D9488";
        public string AdminBadgeTooltip => Auth.IsAdmin ? "Click chuột phải để Quản lý tài khoản (Đổi user/pass, bật/tắt)" : "Tài khoản Lễ Tân";

        // Tự động ẩn/hiển thị tab dịch vụ khi Bật/Tắt Active trong Cấu hình dịch vụ (Áp dụng cả Admin và Lễ tân)
        public bool IsFoodServiceVisible => Data.ServicesConfig.FirstOrDefault(x => x.Code == "AN_UONG")?.IsActive ?? true;
        public bool IsEventServiceVisible => Data.ServicesConfig.FirstOrDefault(x => x.Code == "SU_KIEN")?.IsActive ?? true;
        public bool IsVehicleServiceVisible => Data.ServicesConfig.FirstOrDefault(x => x.Code == "THUE_XE")?.IsActive ?? true;
        public bool IsParkingServiceVisible => Data.ServicesConfig.FirstOrDefault(x => x.Code == "DO_XE")?.IsActive ?? true;
        public bool IsLaundryServiceVisible => Data.ServicesConfig.FirstOrDefault(x => x.Code == "GIAT_UI")?.IsActive ?? true;

        // Notifications
        public NotificationService NotificationService => NotificationService.Instance;

        public System.Collections.Generic.IEnumerable<AppNotification> FilteredNotifications
        {
            get
            {
                var list = IsAdmin 
                    ? NotificationService.Notifications 
                    : NotificationService.Notifications.Where(x => x.TargetRole != "Admin");
                return list.OrderBy(x => x.IsRead ? 1 : 0).ThenByDescending(x => x.Timestamp).ToList();
            }
        }

        public int UnreadNotificationCount => FilteredNotifications.Count(x => !x.IsRead);
        public bool HasUnreadNotifications => UnreadNotificationCount > 0;

        private bool _isNotificationPopupOpen;
        public bool IsNotificationPopupOpen
        {
            get => _isNotificationPopupOpen;
            set => SetProperty(ref _isNotificationPopupOpen, value);
        }

        // Commands
        public ICommand SelectTabCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SyncDatabaseCommand { get; }
        public ICommand OpenAccountManagementCommand { get; }
        public ICommand ToggleNotificationPopupCommand { get; }
        public ICommand MarkAllNotificationsReadCommand { get; }
        public ICommand MarkNotificationReadCommand { get; }
        public ICommand OpenNotificationTargetCommand { get; }

        public event Action? RequestLogout;

        public MainViewModel()
        {
            RoomMapVM = new RoomMapViewModel();
            FoodVM = new FoodViewModel();
            EventVM = new EventViewModel();
            VehicleVM = new VehicleViewModel();
            ParkingVM = new ParkingViewModel();
            LaundryVM = new LaundryViewModel();
            StaffVM = new StaffViewModel();
            StatisticsVM = new StatisticsViewModel();
            CustomerVM = new CustomerViewModel();
            InventoryVM = new InventoryViewModel();
            ServicesConfigVM = new ServicesConfigViewModel();
            TransactionsVM = new TransactionHistoryViewModel();

            SelectTabCommand = new RelayCommand(SelectTab);
            LogoutCommand = new RelayCommand(Logout);
            SyncDatabaseCommand = new RelayCommand(async () => await SyncDatabaseAsync());
            OpenAccountManagementCommand = new RelayCommand(OpenAccountManagement);
            OpenNotificationTargetCommand = new RelayCommand(OpenNotificationTarget);

            ToggleNotificationPopupCommand = new RelayCommand(_ =>
            {
                IsNotificationPopupOpen = !IsNotificationPopupOpen;
                if (IsNotificationPopupOpen)
                {
                    OnPropertyChanged(nameof(FilteredNotifications));
                    OnPropertyChanged(nameof(UnreadNotificationCount));
                    OnPropertyChanged(nameof(HasUnreadNotifications));
                }
            });

            MarkNotificationReadCommand = new RelayCommand(p =>
            {
                if (p is AppNotification notif)
                {
                    notif.IsRead = true;
                    OnPropertyChanged(nameof(FilteredNotifications));
                    OnPropertyChanged(nameof(UnreadNotificationCount));
                    OnPropertyChanged(nameof(HasUnreadNotifications));
                }
            });

            MarkAllNotificationsReadCommand = new RelayCommand(_ =>
            {
                NotificationService.MarkAllAsRead();
                OnPropertyChanged(nameof(FilteredNotifications));
                OnPropertyChanged(nameof(UnreadNotificationCount));
                OnPropertyChanged(nameof(HasUnreadNotifications));
            });

            NotificationService.Notifications.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(FilteredNotifications));
                OnPropertyChanged(nameof(UnreadNotificationCount));
                OnPropertyChanged(nameof(HasUnreadNotifications));
            };

            Auth.AuthStateChanged += OnAuthStateChanged;
            HotelServiceConfig.OnActiveChanged += _ => RefreshServiceVisibilities();
            Data.ServicesConfig.CollectionChanged += (s, e) => RefreshServiceVisibilities();
        }

        public void OpenAccountManagement(object? parameter = null)
        {
            if (IsAdmin)
            {
                var win = new AccountManagementWindow();
                win.ShowDialog();
            }
            else
            {
                MessageBox.Show("Chức năng quản lý tài khoản chỉ dành riêng cho Quản trị viên (Admin)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void RefreshServiceVisibilities()
        {
            OnPropertyChanged(nameof(IsFoodServiceVisible));
            OnPropertyChanged(nameof(IsEventServiceVisible));
            OnPropertyChanged(nameof(IsVehicleServiceVisible));
            OnPropertyChanged(nameof(IsParkingServiceVisible));
            OnPropertyChanged(nameof(IsLaundryServiceVisible));

            // Nếu tab hiện tại vừa bị tắt (inactive), tự động chuyển về Sơ đồ phòng
            if ((CurrentTab == "Food" && !IsFoodServiceVisible) ||
                (CurrentTab == "Event" && !IsEventServiceVisible) ||
                (CurrentTab == "Vehicle" && !IsVehicleServiceVisible) ||
                (CurrentTab == "Parking" && !IsParkingServiceVisible) ||
                (CurrentTab == "Laundry" && !IsLaundryServiceVisible))
            {
                CurrentTab = "RoomMap";
            }
        }

        private void SelectTab(object? parameter)
        {
            if (parameter is string tabName)
            {
                // Kiểm tra phân quyền: Lễ tân không được vào các tab Admin
                if (!IsAdmin && (tabName == "Statistics" || tabName == "Staff" || tabName == "Customers" || tabName == "Inventory" || tabName == "ServicesConfig"))
                {
                    MessageBox.Show("Mục này chỉ dành riêng cho quyền Quản trị viên (Admin)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CurrentTab = tabName;

                if (tabName == "RoomMap")
                {
                    RoomMapVM.ApplyFilter();
                    RoomMapVM.LoadServicesForSelectedRoom();
                    RoomMapVM.NotifyAllStateChanges();
                }
                if (tabName == "Customers") CustomerVM.ApplyFilter();
                if (tabName == "Inventory") InventoryVM.RefreshAll();
                if (tabName == "Food") FoodVM.ApplyFoodFilter();
                if (tabName == "Statistics") StatisticsVM.RefreshStatistics();
                if (tabName == "Vehicle") VehicleVM.ApplyVehicleFilter();
                if (tabName == "Transactions") TransactionsVM.Refresh();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private async System.Threading.Tasks.Task SyncDatabaseAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                await Data.SyncDataToDatabaseAsync();
                RoomMapVM.ApplyFilter();
                RoomMapVM.LoadServicesForSelectedRoom();
                RoomMapVM.NotifyAllStateChanges();
                FoodVM.ApplyFoodFilter();
                EventVM.BuildHourlySchedule();
                VehicleVM.ApplyVehicleFilter();
                CustomerVM.ApplyFilter();
                InventoryVM.RefreshAll();
                StatisticsVM.RefreshStatistics();
                TransactionsVM.Refresh();
                RefreshServiceVisibilities();
            }
            finally
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                IsBusy = false;
            }
        }

        public void RefreshAuthState()
        {
            OnAuthStateChanged();
        }

        private void OnAuthStateChanged()
        {
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(IsReceptionist));
            OnPropertyChanged(nameof(CurrentUserRoleDisplay));
            OnPropertyChanged(nameof(RoleBadgeBackground));
            OnPropertyChanged(nameof(AdminBadgeTooltip));
            RefreshServiceVisibilities();
            SetDefaultTab();
            StatisticsVM.RefreshStatistics();
        }

        private void SetDefaultTab()
        {
            CurrentTab = "RoomMap";
        }

        private void Logout(object? parameter)
        {
            Auth.Logout();
            RequestLogout?.Invoke();
        }

        /// <summary>
        /// Xử lý nhấp đúp vào thông báo: tự động đóng popup, đánh dấu đã đọc và chuyển thẳng tới dòng đơn trong bảng lịch sử của tab dịch vụ đó
        /// </summary>
        private void OpenNotificationTarget(object? param)
        {
            if (param is not AppNotification notif) return;

            // 1. Đánh dấu đã đọc và đóng popup thông báo
            notif.IsRead = true;
            IsNotificationPopupOpen = false;
            OnPropertyChanged(nameof(FilteredNotifications));
            OnPropertyChanged(nameof(UnreadNotificationCount));
            OnPropertyChanged(nameof(HasUnreadNotifications));

            // Nếu không phải thông báo liên quan đến quyết toán (ví dụ: đăng nhập, nhập kho, thông báo chung), chỉ đánh dấu đã đọc
            if (!notif.IsSettlementRelated)
            {
                return;
            }

            string? room = notif.GetResolvedRoomNumber();
            string? orderCode = notif.GetResolvedOrderCode();

            // 2. Chuyển thẳng sang TAB DỊCH VỤ tương ứng (danh sách lịch sử) để trỏ vào đơn cần xử lý
            switch (notif.Type)
            {
                case NotificationType.LaundryReturn:
                    SelectTab("Laundry");
                    LaundryVM.SelectOrderByCode(orderCode, room);
                    break;

                case NotificationType.VehicleReturn:
                    SelectTab("Vehicle");
                    VehicleVM.SelectRentalByCode(orderCode, room);
                    break;

                case NotificationType.EventEnding:
                    SelectTab("Event");
                    EventVM.SelectBookingByCode(orderCode, room);
                    break;

                case NotificationType.RoomCheckout:
                    SelectTab("RoomMap");
                    if (!string.IsNullOrWhiteSpace(room))
                    {
                        RoomMapVM.NavigateToRoomAndService(room, orderCode, "Tiền phòng");
                    }
                    break;

                case NotificationType.Transaction:
                    SelectTab("Transactions");
                    break;

                case NotificationType.Inventory:
                    if (IsAdmin) SelectTab("Inventory");
                    break;

                default:
                    // Phân loại thông minh theo mã đơn hoặc danh mục dịch vụ
                    if (!string.IsNullOrWhiteSpace(orderCode))
                    {
                        if (orderCode.StartsWith("DH-GU", StringComparison.OrdinalIgnoreCase))
                        {
                            SelectTab("Laundry");
                            LaundryVM.SelectOrderByCode(orderCode, room);
                        }
                        else if (orderCode.StartsWith("TX", StringComparison.OrdinalIgnoreCase))
                        {
                            SelectTab("Vehicle");
                            VehicleVM.SelectRentalByCode(orderCode, room);
                        }
                        else if (orderCode.StartsWith("SK", StringComparison.OrdinalIgnoreCase))
                        {
                            SelectTab("Event");
                            EventVM.SelectBookingByCode(orderCode, room);
                        }
                        else
                        {
                            SelectTab("RoomMap");
                            if (!string.IsNullOrWhiteSpace(room)) RoomMapVM.NavigateToRoomAndService(room, orderCode, notif.ServiceCategory);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(notif.ServiceCategory))
                    {
                        if (notif.ServiceCategory.IndexOf("giặt", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            SelectTab("Laundry");
                            LaundryVM.SelectOrderByCode(null, room);
                        }
                        else if (notif.ServiceCategory.IndexOf("xe", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            SelectTab("Vehicle");
                            VehicleVM.SelectRentalByCode(null, room);
                        }
                        else if (notif.ServiceCategory.IndexOf("kiện", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            SelectTab("Event");
                            EventVM.SelectBookingByCode(null, room);
                        }
                        else
                        {
                            SelectTab("RoomMap");
                            if (!string.IsNullOrWhiteSpace(room)) RoomMapVM.NavigateToRoomAndService(room, null, notif.ServiceCategory);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(room))
                    {
                        SelectTab("RoomMap");
                        RoomMapVM.NavigateToRoomAndService(room, orderCode, notif.ServiceCategory);
                    }
                    break;
            }
        }
    }
}
