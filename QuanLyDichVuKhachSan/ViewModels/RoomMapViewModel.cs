using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;
using QuanLyDichVuKhachSan.Views;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class RoomMapViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;

        // Danh sách toàn bộ 25 phòng (5 tầng x 5 phòng)
        public ObservableCollection<HotelRoom> AllRooms => Data.HotelRooms;

        private ObservableRangeCollection<HotelRoom> _filteredRooms = new();
        public ObservableRangeCollection<HotelRoom> FilteredRooms
        {
            get => _filteredRooms;
            set => SetProperty(ref _filteredRooms, value);
        }

        // Phòng đang được chọn để xem chi tiết dịch vụ
        private HotelRoom? _selectedRoom;
        public HotelRoom? SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                if (SetProperty(ref _selectedRoom, value))
                {
                    LoadServicesForSelectedRoom();
                }
            }
        }

        // Danh sách dịch vụ phòng đã chọn đã sử dụng
        public ObservableRangeCollection<RoomServiceUsageItem> CurrentRoomServices { get; } = new();

        private RoomServiceUsageItem? _selectedRoomService;
        public RoomServiceUsageItem? SelectedRoomService
        {
            get => _selectedRoomService;
            set => SetProperty(ref _selectedRoomService, value);
        }

        public event Action<RoomServiceUsageItem>? RequestScrollServiceIntoView;

        // Bộ lọc trạng thái phòng
        private string _selectedFilter = "Tất cả";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (SetProperty(ref _selectedFilter, value))
                {
                    ApplyFilter();
                }
            }
        }

        // Thống kê nhanh
        public int TotalRoomsCount => AllRooms.Count;
        public int OccupiedRoomsCount => AllRooms.Count(x => x.Status == RoomStatus.Occupied);
        public int AvailableRoomsCount => AllRooms.Count(x => x.Status == RoomStatus.Available);
        public int CleaningRoomsCount => AllRooms.Count(x => x.Status == RoomStatus.Cleaning);
        public int MaintenanceRoomsCount => AllRooms.Count(x => x.Status == RoomStatus.Maintenance);

        // Tổng tiền dịch vụ của phòng đang chọn
        public decimal TotalUnpaidAmount => CurrentRoomServices.Where(x => !x.IsPaid).Sum(x => x.Amount);
        public decimal TotalPaidAmount => CurrentRoomServices.Where(x => x.IsPaid).Sum(x => x.Amount);
        public decimal TotalServiceAmount => CurrentRoomServices.Sum(x => x.Amount);
        public bool HasUnpaidServices => TotalUnpaidAmount > 0;

        // Danh sách các tầng (Mỗi tầng 5 phòng theo hàng ngang)
        public ObservableCollection<FloorGroup> FloorGroups { get; } = new();

        // Commands
        public ICommand SelectRoomCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand PayAndExportBillCommand { get; }
        public ICommand RefreshServicesCommand { get; }
        public ICommand ViewServiceDetailCommand { get; }

        // Context Menu Right-Click Commands
        public ICommand CheckInCommand { get; }
        public ICommand CheckOutCommand { get; }
        public ICommand ExtendStayCommand { get; }
        public ICommand SetCleaningCommand { get; }
        public ICommand FinishCleaningCommand { get; }
        public ICommand SetMaintenanceCommand { get; }
        public ICommand FinishMaintenanceCommand { get; }

        // In-Place Guest Editing Commands
        public ICommand StartEditGuestInfoCommand { get; }
        public ICommand SaveGuestInfoCommand { get; }
        public ICommand CancelEditGuestInfoCommand { get; }

        // Backup khi bắt đầu chỉnh sửa để phục hồi nếu Hủy
        private string _backupName = "";
        private string _backupPhone = "";
        private string _backupCccd = "";
        private string _backupNote = "";
        private DateTime? _backupCheckIn;
        private DateTime? _backupCheckOut;

        public RoomMapViewModel()
        {
            SelectRoomCommand = new RelayCommand(param =>
            {
                if (param is HotelRoom room)
                {
                    if (SelectedRoom != null && SelectedRoom != room && SelectedRoom.IsEditing)
                    {
                        SelectedRoom.IsEditing = false;
                    }
                    SelectedRoom = room;
                }
            });

            FilterCommand = new RelayCommand(param =>
            {
                if (param is string filter && !string.IsNullOrEmpty(filter))
                {
                    SelectedFilter = filter;
                }
            });

            PayAndExportBillCommand = new RelayCommand(PayAndExportBill);
            RefreshServicesCommand = new RelayCommand(LoadServicesForSelectedRoom);

            ViewServiceDetailCommand = new RelayCommand(param =>
            {
                if (param is RoomServiceUsageItem item)
                {
                    var win = new InvoiceBillWindow(item, SelectedRoom);
                    win.ShowDialog();
                }
            });

            // In-Place Guest Editing
            StartEditGuestInfoCommand = new RelayCommand(_ =>
            {
                if (SelectedRoom != null && SelectedRoom.IsOccupied)
                {
                    _backupName = SelectedRoom.CustomerName;
                    _backupPhone = SelectedRoom.PhoneNumber;
                    _backupCccd = SelectedRoom.IdentityCard;
                    _backupNote = SelectedRoom.Note;
                    _backupCheckIn = SelectedRoom.CheckInDate;
                    _backupCheckOut = SelectedRoom.ExpectedCheckOutDate;

                    SelectedRoom.IsEditing = true;
                }
            });

            SaveGuestInfoCommand = new RelayCommand(_ =>
            {
                if (SelectedRoom != null && SelectedRoom.IsEditing)
                {
                    string name = SelectedRoom.CustomerName?.Trim() ?? "";
                    string phone = SelectedRoom.PhoneNumber?.Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        MessageBox.Show("Họ & Tên khách hàng không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(phone))
                    {
                        MessageBox.Show("Số điện thoại không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Lưu vào DB SQL Server bất đồng bộ (Không chặn UI)
                    _ = DatabaseService.Instance.UpdateRoomGuestInfoAsync(SelectedRoom);

                    // Cập nhật bảng KhachHang
                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "IF EXISTS (SELECT 1 FROM KhachHang WHERE SoPhong = @Phong) " +
                        "UPDATE KhachHang SET HoTen = @Ten, SoDienThoai = @Phone, CCCD = @CCCD, GhiChu = @Note WHERE SoPhong = @Phong " +
                        "ELSE INSERT INTO KhachHang (HoTen, CCCD, SoDienThoai, SoPhong, GhiChu) VALUES (@Ten, @CCCD, @Phone, @Phong, @Note)",
                        new Microsoft.Data.SqlClient.SqlParameter("@Phong", SelectedRoom.RoomNumber),
                        new Microsoft.Data.SqlClient.SqlParameter("@Ten", name),
                        new Microsoft.Data.SqlClient.SqlParameter("@Phone", phone),
                        new Microsoft.Data.SqlClient.SqlParameter("@CCCD", SelectedRoom.IdentityCard ?? ""),
                        new Microsoft.Data.SqlClient.SqlParameter("@Note", SelectedRoom.Note ?? "")
                    );

                    SelectedRoom.IsEditing = false;
                    NotifyAllStateChanges();

                    try
                    {
                        NotificationService.Instance.AddNotification(
                            "✏️ Cập nhật thông tin khách",
                            $"Thông tin khách phòng {SelectedRoom.RoomNumber} ({name} - {phone}) đã được cập nhật thành công.",
                            NotificationType.SystemAlert,
                            "All"
                        );
                    }
                    catch { }

                    MessageBox.Show($"Đã lưu cập nhật thông tin khách phòng {SelectedRoom.RoomNumber} thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            CancelEditGuestInfoCommand = new RelayCommand(_ =>
            {
                if (SelectedRoom != null && SelectedRoom.IsEditing)
                {
                    SelectedRoom.CustomerName = _backupName;
                    SelectedRoom.PhoneNumber = _backupPhone;
                    SelectedRoom.IdentityCard = _backupCccd;
                    SelectedRoom.Note = _backupNote;
                    SelectedRoom.CheckInDate = _backupCheckIn;
                    SelectedRoom.ExpectedCheckOutDate = _backupCheckOut;

                    SelectedRoom.IsEditing = false;
                }
            });

            // Gia hạn thời gian ở
            ExtendStayCommand = new RelayCommand(param =>
            {
                if (param is HotelRoom room)
                {
                    if (room.Status != RoomStatus.Occupied)
                    {
                        MessageBox.Show($"Phòng {room.RoomNumber} hiện đang '{room.StatusDisplay}', không thể gia hạn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var dlg = new Views.ExtendStayDialogWindow(room);
                    if (dlg.ShowDialog() == true && dlg.IsConfirmed)
                    {
                        room.ExpectedCheckOutDate = dlg.SelectedNewCheckOut;
                        _ = DatabaseService.Instance.UpdateRoomGuestInfoAsync(room);

                        SelectedRoom = room;
                        NotifyAllStateChanges();

                        try
                        {
                            NotificationService.Instance.AddNotification(
                                "⏰ Gia hạn thời gian ở",
                                $"Phòng {room.RoomNumber} (Khách: {room.CustomerName}) đã gia hạn thời gian trả phòng đến {room.CheckOutDisplay}.",
                                NotificationType.RoomCheckout,
                                "All"
                            );
                        }
                        catch { }

                        MessageBox.Show($"Đã gia hạn thành công phòng {room.RoomNumber} đến {room.CheckOutDisplay}!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            });

            // Right-Click ContextMenu Commands (Cập nhật Realtime Tức Thì)
            CheckInCommand = new RelayCommand(param =>
            {
                if (param is HotelRoom room)
                {
                    if (room.Status == RoomStatus.Occupied)
                    {
                        MessageBox.Show($"Phòng {room.RoomNumber} hiện đang có khách lưu trú ({room.CustomerName})!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (room.Status == RoomStatus.Maintenance)
                    {
                        MessageBox.Show($"Phòng {room.RoomNumber} đang trong quá trình bảo trì, vui lòng hoàn tất bảo trì trước khi nhận phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Mở bảng đăng ký khách nhận phòng
                    var checkInDialog = new Views.CheckInDialogWindow(room);
                    if (checkInDialog.ShowDialog() == true && checkInDialog.IsConfirmed)
                    {
                        // 1. Cập nhật vào danh sách Khách hàng trong RAM Cache
                        int newCustId = Data.Customers.Count > 0 ? Data.Customers.Max(x => x.Id) + 1 : 1;
                        Data.AddCustomerToCache(new Customer
                        {
                            Id = newCustId,
                            FullName = room.CustomerName,
                            PhoneNumber = room.PhoneNumber,
                            IdentityCard = room.IdentityCard,
                            RoomNumber = room.RoomNumber,
                            Note = room.Note
                        });

                        SelectedRoom = room;
                        ApplyFilter();
                        LoadServicesForSelectedRoom();
                        NotifyAllStateChanges();

                        // 2. Đồng bộ trực tiếp xuống CSDL SQL Server (Table PhongKhachSan) bất đồng bộ
                        _ = DatabaseService.Instance.UpdateRoomGuestInfoAsync(room);

                        // 3. Lưu thông tin khách vào bảng KhachHang trong SQL Server
                        _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                            "IF EXISTS (SELECT 1 FROM KhachHang WHERE (SoDienThoai = @Phone AND @Phone <> '') OR (CCCD = @CCCD AND @CCCD <> '')) " +
                            "UPDATE KhachHang SET HoTen = @Ten, SoPhong = @Phong, GhiChu = @Note WHERE (SoDienThoai = @Phone AND @Phone <> '') OR (CCCD = @CCCD AND @CCCD <> '') " +
                            "ELSE INSERT INTO KhachHang (HoTen, CCCD, SoDienThoai, SoPhong, GhiChu) " +
                            "VALUES (@Ten, @CCCD, @Phone, @Phong, @Note)",
                            new Microsoft.Data.SqlClient.SqlParameter("@Phong", room.RoomNumber),
                            new Microsoft.Data.SqlClient.SqlParameter("@Ten", room.CustomerName ?? ""),
                            new Microsoft.Data.SqlClient.SqlParameter("@Phone", room.PhoneNumber ?? ""),
                            new Microsoft.Data.SqlClient.SqlParameter("@CCCD", room.IdentityCard ?? ""),
                            new Microsoft.Data.SqlClient.SqlParameter("@Note", room.Note ?? "")
                        );

                        try
                        {
                            NotificationService.Instance.AddNotification(
                                "🔑 Khách nhận phòng thành công",
                                $"Phòng {room.RoomNumber} ({room.CustomerName} - SĐT: {room.PhoneNumber}) đã nhận phòng lúc {room.CheckInDisplay}. Dự kiến trả phòng lúc {room.CheckOutDisplay}.",
                                NotificationType.RoomCheckout,
                                "All"
                            );
                        }
                        catch { }
                    }
                }
            });

            CheckOutCommand = new RelayCommand(param =>
            {
                if (param is HotelRoom room)
                {
                    if (room.Status != RoomStatus.Occupied)
                    {
                        MessageBox.Show($"Phòng {room.RoomNumber} hiện đang '{room.StatusDisplay}', không có khách lưu trú để trả phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // 1. Kiểm tra các khoản dịch vụ chưa thanh toán -> Mở trực tiếp bảng Hóa Đơn (Bill) không cần hiện bảng hỏi
                    var unpaid = Data.GetServicesForRoom(room.RoomNumber).Where(x => !x.IsPaid).ToList();
                    if (unpaid.Count > 0)
                    {
                        SelectedRoom = room;
                        LoadServicesForSelectedRoom();
                        NotifyAllStateChanges();

                        // Mở thẳng bảng Hóa Đơn / Bill thanh toán luôn
                        PayAndExportBill();
                        return;
                    }

                    // 2. Khi đã thanh toán hết 100% dịch vụ (hoặc tổng dịch vụ chưa thanh toán = 0) -> Cho phép trả phòng và chuyển sang dọn dẹp
                    room.Status = RoomStatus.Cleaning;
                    room.CustomerName = "";
                    room.PhoneNumber = "";
                    room.IdentityCard = "";
                    room.CheckInDate = null;
                    room.Note = "Khách đã trả phòng & quyết toán xong - Chờ dọn phòng";
                    SelectedRoom = room;
                    ApplyFilter();
                    LoadServicesForSelectedRoom();
                    NotifyAllStateChanges();

                    // Đồng bộ ngầm trạng thái xuống CSDL SQL Server (Non-blocking)
                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "UPDATE PhongKhachSan SET TrangThai = 'Cleaning', TenKhach = '', SoDienThoai = '', CCCD = '', GhiChu = @Note WHERE SoPhong = @Phong",
                        new Microsoft.Data.SqlClient.SqlParameter("@Phong", room.RoomNumber),
                        new Microsoft.Data.SqlClient.SqlParameter("@Note", room.Note)
                    );
                }
            });

            SetCleaningCommand = new RelayCommand(param =>
            {
                if (param is HotelRoom room)
                {
                    room.Status = RoomStatus.Cleaning;
                    room.Note = "Đang dọn dẹp và thay mới đồ dùng";
                    SelectedRoom = room;
                    ApplyFilter();
                    NotifyAllStateChanges();

                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "UPDATE PhongKhachSan SET TrangThai = 'Cleaning', GhiChu = @Note WHERE SoPhong = @Phong",
                        new Microsoft.Data.SqlClient.SqlParameter("@Phong", room.RoomNumber),
                        new Microsoft.Data.SqlClient.SqlParameter("@Note", room.Note)
                    );
                }
            });

            FinishCleaningCommand = new RelayCommand(param =>
            {
                if (param is HotelRoom room)
                {
                    room.Status = RoomStatus.Available;
                    room.CustomerName = "";
                    room.PhoneNumber = "";
                    room.IdentityCard = "";
                    room.CheckInDate = null;
                    room.Note = "Phòng đã dọn dẹp sạch sẽ, sẵn sàng đón khách";
                    SelectedRoom = room;
                    ApplyFilter();
                    LoadServicesForSelectedRoom();
                    NotifyAllStateChanges();

                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "UPDATE PhongKhachSan SET TrangThai = 'Available', TenKhach = '', SoDienThoai = '', CCCD = '', GhiChu = @Note WHERE SoPhong = @Phong",
                        new Microsoft.Data.SqlClient.SqlParameter("@Phong", room.RoomNumber),
                        new Microsoft.Data.SqlClient.SqlParameter("@Note", room.Note)
                    );
                }
            });

            SetMaintenanceCommand = new RelayCommand(param =>
            {
                if (param is HotelRoom room)
                {
                    room.Status = RoomStatus.Maintenance;
                    room.Note = "Đang bảo trì thiết bị kỹ thuật";
                    SelectedRoom = room;
                    ApplyFilter();
                    NotifyAllStateChanges();

                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "UPDATE PhongKhachSan SET TrangThai = 'Maintenance', GhiChu = @Note WHERE SoPhong = @Phong",
                        new Microsoft.Data.SqlClient.SqlParameter("@Phong", room.RoomNumber),
                        new Microsoft.Data.SqlClient.SqlParameter("@Note", room.Note)
                    );
                }
            });

            FinishMaintenanceCommand = new RelayCommand(param =>
            {
                if (param is HotelRoom room)
                {
                    room.Status = RoomStatus.Available;
                    room.Note = "Đã bảo trì hoàn tất, sẵn sàng đón khách";
                    SelectedRoom = room;
                    ApplyFilter();
                    LoadServicesForSelectedRoom();
                    NotifyAllStateChanges();

                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "UPDATE PhongKhachSan SET TrangThai = 'Available', GhiChu = @Note WHERE SoPhong = @Phong",
                        new Microsoft.Data.SqlClient.SqlParameter("@Phong", room.RoomNumber),
                        new Microsoft.Data.SqlClient.SqlParameter("@Note", room.Note)
                    );
                }
            });

            Data.RoomServiceUsages.CollectionChanged += (s, e) =>
            {
                LoadServicesForSelectedRoom();
                NotifyAllStateChanges();
            };

            Data.ParkingRecords.CollectionChanged += (s, e) =>
            {
                ApplyFilter();
                LoadServicesForSelectedRoom();
                NotifyAllStateChanges();
            };

            ApplyFilter();

            // Mặc định chọn phòng P.302 (đang có nhiều dịch vụ) để người dùng thấy ngay
            SelectedRoom = AllRooms.FirstOrDefault(x => x.RoomNumber == "P.302") ?? AllRooms.FirstOrDefault();
        }

        public void NotifyAllStateChanges()
        {
            OnPropertyChanged(nameof(SelectedRoom));
            OnPropertyChanged(nameof(TotalRoomsCount));
            OnPropertyChanged(nameof(OccupiedRoomsCount));
            OnPropertyChanged(nameof(AvailableRoomsCount));
            OnPropertyChanged(nameof(CleaningRoomsCount));
            OnPropertyChanged(nameof(MaintenanceRoomsCount));
            OnPropertyChanged(nameof(TotalUnpaidAmount));
            OnPropertyChanged(nameof(TotalPaidAmount));
            OnPropertyChanged(nameof(TotalServiceAmount));
            OnPropertyChanged(nameof(HasUnpaidServices));
        }

        public void ApplyFilter()
        {
            FilteredRooms.Clear();
            FloorGroups.Clear();

            // Đảm bảo mỗi phòng là duy nhất theo mã phòng (RoomNumber)
            IEnumerable<HotelRoom> query = AllRooms.GroupBy(x => x.RoomNumber).Select(g => g.First());

            if (SelectedFilter == "Đang ở")
                query = query.Where(x => x.Status == RoomStatus.Occupied);
            else if (SelectedFilter == "Trống")
                query = query.Where(x => x.Status == RoomStatus.Available);
            else if (SelectedFilter == "Dọn dẹp")
                query = query.Where(x => x.Status == RoomStatus.Cleaning);
            else if (SelectedFilter == "Bảo trì")
                query = query.Where(x => x.Status == RoomStatus.Maintenance);

            var filteredList = query.ToList();
            FilteredRooms.ReplaceRange(filteredList);

            // Nhóm theo 5 tầng (Tầng 1 -> Tầng 5)
            for (int f = 1; f <= 5; f++)
            {
                var floorRooms = filteredList.Where(x => x.Floor == f).OrderBy(x => x.RoomNumber).ToList();
                if (floorRooms.Count > 0)
                {
                    var group = new FloorGroup { FloorNumber = f };
                    foreach (var r in floorRooms)
                    {
                        var parkedList = Data.ParkingRecords.Where(p => 
                            DataService.NormalizeRoomNumber(p.RoomNumber) == DataService.NormalizeRoomNumber(r.RoomNumber) && 
                            p.CheckOutTime == null).ToList();
                        r.ParkedVehicleInfo = parkedList.Count > 0
                            ? string.Join("; ", parkedList.Select(p => $"{p.VehicleType} (BS: {p.LicensePlate})"))
                            : "";
                        group.Rooms.Add(r);
                    }
                    FloorGroups.Add(group);
                }
            }

            OnPropertyChanged(nameof(TotalRoomsCount));
            OnPropertyChanged(nameof(OccupiedRoomsCount));
            OnPropertyChanged(nameof(AvailableRoomsCount));
            OnPropertyChanged(nameof(CleaningRoomsCount));
            OnPropertyChanged(nameof(MaintenanceRoomsCount));
        }

        public void LoadServicesForSelectedRoom()
        {
            CurrentRoomServices.Clear();
            if (SelectedRoom != null)
            {
                var parkedList = Data.ParkingRecords.Where(p => 
                    DataService.NormalizeRoomNumber(p.RoomNumber) == DataService.NormalizeRoomNumber(SelectedRoom.RoomNumber) && 
                    p.CheckOutTime == null).ToList();
                SelectedRoom.ParkedVehicleInfo = parkedList.Count > 0
                    ? string.Join("; ", parkedList.Select(p => $"{p.VehicleType} | Biển số: {p.LicensePlate} (Vào bãi: {p.CheckInTime:HH:mm dd/MM})"))
                    : "";

                var services = Data.GetServicesForRoom(SelectedRoom.RoomNumber);
                CurrentRoomServices.ReplaceRange(services);
            }
            else
            {
                CurrentRoomServices.ReplaceRange(null);
            }

            OnPropertyChanged(nameof(TotalUnpaidAmount));
            OnPropertyChanged(nameof(TotalPaidAmount));
            OnPropertyChanged(nameof(TotalServiceAmount));
            OnPropertyChanged(nameof(HasUnpaidServices));
        }

        private void PayAndExportBill()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng để thanh toán dịch vụ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!HasUnpaidServices)
            {
                MessageBox.Show($"Phòng {SelectedRoom.RoomNumber} hiện không có khoản dịch vụ nào chưa thanh toán!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 1. Lấy danh sách dịch vụ chưa thanh toán để lập bản xem trước hóa đơn
            var unpaidItems = CurrentRoomServices.Where(x => !x.IsPaid).ToList();
            decimal unpaidServicesCost = unpaidItems.Sum(x => x.Amount);

            // 2. Khởi tạo đối tượng Hóa đơn dịch vụ (RoomInvoice)
            var invoice = new RoomInvoice
            {
                InvoiceCode = $"HD-KS-{DateTime.Now:yyyyMMdd}-{SelectedRoom.RoomNumber.Replace(".", "")}",
                RoomNumber = $"{SelectedRoom.RoomNumber} ({SelectedRoom.RoomType})",
                CustomerName = string.IsNullOrEmpty(SelectedRoom.CustomerName) ? "Khách lưu trú" : SelectedRoom.CustomerName,
                PhoneNumber = SelectedRoom.PhoneNumber,
                IdentityCard = SelectedRoom.IdentityCard,
                InvoiceDate = DateTime.Now,
                CheckInDate = SelectedRoom.CheckInDate,
                CheckOutDate = SelectedRoom.ExpectedCheckOutDate ?? DateTime.Now,
                StayNights = SelectedRoom.StayNights,
                PricePerNight = SelectedRoom.PricePerNight,
                RoomCost = 0,
                ServicesCost = unpaidServicesCost,
                TotalAmount = unpaidServicesCost,
                IssuedBy = Data.GetCurrentDutyStaffName(),
                PaidItems = unpaidItems
            };

            // 3. Mở trực tiếp cửa sổ Hóa đơn / Bill
            var billWindow = new InvoiceBillWindow(invoice);
            bool? dialogResult = billWindow.ShowDialog();

            // 4. CHỈ LƯU VÀO DATABASE VÀ ĐỔI TRẠNG THÁI KHI NGƯỜI DÙNG BẤM "ĐÓNG & HOÀN TẤT"
            if (dialogResult == true || billWindow.IsConfirmed)
            {
                var payResult = Data.PayRoomServices(SelectedRoom.RoomNumber);
                LoadServicesForSelectedRoom();
                NotifyAllStateChanges();
            }
            else
            {
                // Nếu bấm nút HỦY hoặc dấu X: Giữ nguyên trạng thái chưa thanh toán
                LoadServicesForSelectedRoom();
                NotifyAllStateChanges();
            }
        }

        /// <summary>
        /// Điều hướng trực tiếp đến phòng và dòng dịch vụ cụ thể (highlight + scroll)
        /// </summary>
        public bool NavigateToRoomAndService(string roomNumber, string? orderCode = null, string? serviceCategory = null)
        {
            string norm = DataService.NormalizeRoomNumber(roomNumber);
            if (string.IsNullOrEmpty(norm)) return false;

            // Đảm bảo bộ lọc phòng hiển thị phòng này (chuyển sang 'Tất cả')
            if (SelectedFilter != "Tất cả")
            {
                SelectedFilter = "Tất cả";
            }

            var targetRoom = AllRooms.FirstOrDefault(r => 
                DataService.NormalizeRoomNumber(r.RoomNumber) == norm ||
                string.Equals(r.RoomNumber, norm, StringComparison.OrdinalIgnoreCase));

            if (targetRoom == null) return false;

            // Chọn phòng -> Nạp lại dịch vụ
            SelectedRoom = targetRoom;
            LoadServicesForSelectedRoom();

            // Xóa highlight cũ của tất cả dịch vụ
            foreach (var s in CurrentRoomServices)
            {
                s.IsHighlighted = false;
            }

            RoomServiceUsageItem? targetService = null;

            // 1. Tìm theo mã đơn hàng (orderCode) nếu có
            if (!string.IsNullOrWhiteSpace(orderCode))
            {
                string cleanCode = orderCode.Trim();
                targetService = CurrentRoomServices.FirstOrDefault(s => 
                    (!string.IsNullOrEmpty(s.Details) && s.Details.IndexOf(cleanCode, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.ServiceName) && s.ServiceName.IndexOf(cleanCode, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            // 2. Nếu chưa thấy và có serviceCategory, ưu tiên dòng chưa thanh toán thuộc category đó
            if (targetService == null && !string.IsNullOrWhiteSpace(serviceCategory))
            {
                targetService = CurrentRoomServices.FirstOrDefault(s => 
                    !s.IsPaid && 
                    !string.IsNullOrEmpty(s.ServiceCategory) && 
                    s.ServiceCategory.IndexOf(serviceCategory, StringComparison.OrdinalIgnoreCase) >= 0);

                if (targetService == null)
                {
                    targetService = CurrentRoomServices.FirstOrDefault(s => 
                        !string.IsNullOrEmpty(s.ServiceCategory) && 
                        s.ServiceCategory.IndexOf(serviceCategory, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }

            // 3. Nếu vẫn chưa thấy, ưu tiên dòng dịch vụ chưa thanh toán đầu tiên
            if (targetService == null)
            {
                targetService = CurrentRoomServices.FirstOrDefault(s => !s.IsPaid);
            }

            // 4. Fallback dòng đầu tiên
            targetService ??= CurrentRoomServices.FirstOrDefault();

            if (targetService != null)
            {
                targetService.IsHighlighted = true;
                SelectedRoomService = targetService;
                RequestScrollServiceIntoView?.Invoke(targetService);
            }

            return true;
        }
    }
}
