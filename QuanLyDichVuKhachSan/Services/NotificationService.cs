using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using QuanLyDichVuKhachSan.Models;

namespace QuanLyDichVuKhachSan.Services
{
    public class NotificationService
    {
        private static NotificationService? _instance;
        public static NotificationService Instance => _instance ??= new NotificationService();

        public ObservableCollection<AppNotification> Notifications { get; } = new();

        private readonly DispatcherTimer _scanTimer;
        private int _nextId = 1;
        private readonly HashSet<string> _notifiedKeys = new(StringComparer.OrdinalIgnoreCase);

        private NotificationService()
        {
            // Quét kiểm tra cảnh báo vận hành tự động mỗi 30 giây
            _scanTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _scanTimer.Tick += (s, e) => ScanOperationalAlerts();
            _scanTimer.Start();
        }

        public void AddNotification(
            string title, 
            string message, 
            NotificationType type, 
            string targetRole = "All",
            string? roomNumber = null,
            string? orderCode = null,
            string? serviceCategory = null)
        {
            App.Current?.Dispatcher.Invoke(() =>
            {
                var notif = new AppNotification
                {
                    Id = _nextId++,
                    Title = title,
                    Message = message,
                    Type = type,
                    TargetRole = targetRole,
                    Timestamp = DateTime.Now,
                    IsRead = false,
                    RoomNumber = roomNumber,
                    OrderCode = orderCode,
                    ServiceCategory = serviceCategory
                };

                Notifications.Insert(0, notif);

                // Giới hạn 200 thông báo gần nhất
                while (Notifications.Count > 200)
                {
                    Notifications.RemoveAt(Notifications.Count - 1);
                }
            });
        }

        public void NotifyLogin(string username, string role, string fullName)
        {
            AddNotification(
                "🔑 Đăng nhập hệ thống",
                $"Tài khoản {username} ({fullName} - {role}) vừa đăng nhập thành công vào lúc {DateTime.Now:HH:mm:ss}.",
                NotificationType.Login,
                "Admin"
            );
        }

        public void NotifyInventoryImport(string itemName, int quantity, decimal total, string importedBy)
        {
            AddNotification(
                "📦 Nhập kho thực phẩm/hàng hóa",
                $"{importedBy} vừa nhập kho +{quantity} suất '{itemName}' (Tổng: {total:N0} đ).",
                NotificationType.Inventory,
                "Admin"
            );
        }

        public void NotifyTransaction(string serviceName, string code, decimal amount, string roomOrCust)
        {
            AddNotification(
                "💳 Giao dịch thanh toán hoàn tất",
                $"Đơn {code} [{serviceName}] - {roomOrCust} đã thanh toán {amount:N0} đ.",
                NotificationType.Transaction,
                "Admin",
                orderCode: code,
                serviceCategory: serviceName
            );
        }

        /// <summary>
        /// Tự động quét dữ liệu phòng, xe, giặt ủi, sự kiện để tạo cảnh báo thời gian thực
        /// </summary>
        public void ScanOperationalAlerts()
        {
            try
            {
                var data = DataService.Instance;
                DateTime now = DateTime.Now;

                // 1. Cảnh báo khách phòng sắp tới giờ hoặc quá giờ trả phòng
                foreach (var r in data.HotelRooms.Where(x => x.Status == RoomStatus.Occupied && x.ExpectedCheckOutDate.HasValue))
                {
                    DateTime checkOut = r.ExpectedCheckOutDate!.Value;
                    string keyOverdue = $"ROOM_OVERDUE_{r.RoomNumber}_{checkOut:yyyyMMddHHmm}";
                    string keySoon = $"ROOM_SOON_{r.RoomNumber}_{checkOut:yyyyMMddHHmm}";

                    if (checkOut <= now && !_notifiedKeys.Contains(keyOverdue))
                    {
                        _notifiedKeys.Add(keyOverdue);
                        AddNotification(
                            "⏰ Quá giờ trả phòng!",
                            $"Phòng {r.RoomNumber} (Khách: {r.CustomerName} - SĐT: {r.PhoneNumber}) đã quá giờ hẹn trả phòng lúc {checkOut:HH:mm dd/MM}. Vui lòng liên hệ hỗ trợ hoặc làm thủ tục gia hạn.",
                            NotificationType.RoomCheckout,
                            "All",
                            roomNumber: r.RoomNumber,
                            serviceCategory: "Tiền phòng"
                        );
                    }
                    else if (checkOut > now && checkOut <= now.AddMinutes(15) && !_notifiedKeys.Contains(keySoon))
                    {
                        _notifiedKeys.Add(keySoon);
                        int remainingMin = Math.Max(1, (int)Math.Ceiling((checkOut - now).TotalMinutes));
                        AddNotification(
                            "⏰ Sắp tới giờ trả phòng (15 phút)",
                            $"Phòng {r.RoomNumber} (Khách: {r.CustomerName} - SĐT: {r.PhoneNumber}) sắp đến giờ trả phòng lúc {checkOut:HH:mm} (còn {remainingMin} phút).",
                            NotificationType.RoomCheckout,
                            "All",
                            roomNumber: r.RoomNumber,
                            serviceCategory: "Tiền phòng"
                        );
                    }
                }

                // 2. Cảnh báo giặt ủi sắp tới giờ hẹn trả quần áo (trước 30 phút hoặc quá hạn)
                foreach (var l in data.LaundryOrders.Where(x => !x.DaThanhToan && x.Status != LaundryStatus.Completed && x.Status != LaundryStatus.Cancelled))
                {
                    DateTime returnTime = l.AppointmentDate;
                    string keyLaundryOverdue = $"LAUNDRY_OVERDUE_{l.OrderCode}_{returnTime:yyyyMMddHHmm}";
                    string keyLaundrySoon = $"LAUNDRY_SOON_{l.OrderCode}_{returnTime:yyyyMMddHHmm}";

                    if (returnTime <= now && !_notifiedKeys.Contains(keyLaundryOverdue))
                    {
                        _notifiedKeys.Add(keyLaundryOverdue);
                        AddNotification(
                            "⏰ Quá giờ hẹn trả đồ giặt!",
                            $"Đơn giặt ủi {l.OrderCode} (Phòng: {l.RoomNumber} - Khách: {l.CustomerName}) đã quá giờ hẹn trả lúc {returnTime:HH:mm dd/MM}. Vui lòng kiểm tra và hoàn tất trả đồ cho khách.",
                            NotificationType.LaundryReturn,
                            "All",
                            roomNumber: l.RoomNumber,
                            orderCode: l.OrderCode,
                            serviceCategory: "Giặt ủi"
                        );
                    }
                    else if (returnTime > now && returnTime <= now.AddHours(2) && !_notifiedKeys.Contains(keyLaundrySoon))
                    {
                        _notifiedKeys.Add(keyLaundrySoon);
                        int remainingMin = Math.Max(1, (int)Math.Ceiling((returnTime - now).TotalMinutes));
                        AddNotification(
                            $"🧺 Sắp đến giờ hẹn trả đồ giặt (còn {remainingMin} phút)",
                            $"Đơn giặt ủi {l.OrderCode} (Phòng: {l.RoomNumber} - Khách: {l.CustomerName}) sắp tới giờ hẹn trả lúc {returnTime:HH:mm dd/MM} (còn {remainingMin} phút).",
                            NotificationType.LaundryReturn,
                            "All",
                            roomNumber: l.RoomNumber,
                            orderCode: l.OrderCode,
                            serviceCategory: "Giặt ủi"
                        );
                    }
                }

                // Tự động kích hoạt trạng thái "Đang sử dụng" / "Đang thuê" trong SQL Server
                _ = DatabaseService.Instance.ExecuteNonQueryAsync("EXEC sp_KichHoatDangSuDungSuKien; EXEC sp_KichHoatDangThueXe;");

                // 3. Cảnh báo xe thuê sắp bắt đầu (15-30 phút), tự động chuyển Đang thuê, sắp tới hạn trả (trước 15 phút) hoặc quá hạn 30 phút mà chưa trả/quyết toán
                foreach (var v in data.VehicleRentals.Where(x => !x.DaThanhToan && x.ActualReturnDate == null && x.Status != "Đã hủy" && x.Status != "Hoàn tất"))
                {
                    DateTime returnTime = v.ExpectedReturnDate;
                    DateTime startTime = v.RentalDate;
                    string roomStr = string.IsNullOrWhiteSpace(v.RoomNumber) ? "" : $" - Phòng {v.RoomNumber}";

                    // 3a. Thông báo xe thuê sắp bắt đầu (trước 15 - 30 phút)
                    string keyVehicleStartSoon = $"VEHICLE_START_SOON_{v.RentalCode}_{startTime:yyyyMMddHHmm}";
                    if (startTime > now && startTime <= now.AddMinutes(30) && !_notifiedKeys.Contains(keyVehicleStartSoon))
                    {
                        _notifiedKeys.Add(keyVehicleStartSoon);
                        int remainingStartMin = Math.Max(1, (int)Math.Ceiling((startTime - now).TotalMinutes));
                        AddNotification(
                            $"🛵 Xe thuê sắp đến giờ giao (còn {remainingStartMin} phút)",
                            $"Đơn thuê xe {v.RentalCode} ({v.VehicleName}{roomStr} - Khách: {v.CustomerName}) sắp tới giờ nhận xe lúc {startTime:HH:mm dd/MM} (còn {remainingStartMin} phút). Vui lòng chuẩn bị xe.",
                            NotificationType.VehicleReturn,
                            "All",
                            roomNumber: v.RoomNumber,
                            orderCode: v.RentalCode,
                            serviceCategory: "Thuê xe máy"
                        );
                    }

                    // 3b. Tự động chuyển sang "Đang thuê" khi đến thời gian bắt đầu
                    if (now >= startTime && now < returnTime && v.Status != "Đang thuê")
                    {
                        v.Status = "Đang thuê";
                        v.OrderStatus = "Đang thuê";
                        data.SyncVehicleFleetStatus();
                        _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                            "UPDATE DonThueXe SET TrangThaiDon = N'Đang thuê' WHERE MaDon = @Ma AND TrangThaiDon = N'Đã đặt';",
                            new Microsoft.Data.SqlClient.SqlParameter("@Ma", v.RentalCode));

                        string keyStart = $"VEHICLE_STARTED_{v.RentalCode}_{startTime:yyyyMMddHHmm}";
                        if (!_notifiedKeys.Contains(keyStart))
                        {
                            _notifiedKeys.Add(keyStart);
                            AddNotification(
                                "🛵 Bắt đầu thời gian thuê xe",
                                $"Đơn thuê xe {v.RentalCode} ({v.VehicleName}{roomStr} - Khách: {v.CustomerName}) đã đến giờ thuê ({startTime:HH:mm}). Hệ thống chuyển trạng thái sang 'Đang thuê'.",
                                NotificationType.VehicleReturn,
                                "All",
                                roomNumber: v.RoomNumber,
                                orderCode: v.RentalCode,
                                serviceCategory: "Thuê xe máy"
                            );
                        }
                    }

                    // 3c. Quá giờ hẹn trả xe 30 phút mà vẫn chưa thực hiện trả xe, quyết toán
                    string keyVehicleOverdue = $"VEHICLE_OVERDUE_{v.RentalCode}_{returnTime:yyyyMMddHHmm}";
                    string keyVehicleSoon = $"VEHICLE_SOON_{v.RentalCode}_{returnTime:yyyyMMddHHmm}";
                    if (now >= returnTime.AddMinutes(30) && !_notifiedKeys.Contains(keyVehicleOverdue))
                    {
                        _notifiedKeys.Add(keyVehicleOverdue);
                        int overdueMin = (int)Math.Max(30, (now - returnTime).TotalMinutes);
                        AddNotification(
                            "⏰ Quá hạn trả xe thuê (quá 30 phút)!",
                            $"Đơn thuê xe {v.RentalCode} ({v.VehicleName}{roomStr} - Khách: {v.CustomerName}) đã quá giờ hẹn trả {overdueMin} phút (hẹn lúc {returnTime:HH:mm dd/MM}) mà chưa thực hiện trả xe, quyết toán. Vui lòng liên hệ khách!",
                            NotificationType.VehicleReturn,
                            "All",
                            roomNumber: v.RoomNumber,
                            orderCode: v.RentalCode,
                            serviceCategory: "Thuê xe máy"
                        );
                    }
                    // 3d. Còn 15 phút nữa là đến giờ trả xe
                    else if (returnTime > now && returnTime <= now.AddMinutes(15) && !_notifiedKeys.Contains(keyVehicleSoon))
                    {
                        _notifiedKeys.Add(keyVehicleSoon);
                        int remainingMin = Math.Max(1, (int)Math.Ceiling((returnTime - now).TotalMinutes));
                        AddNotification(
                            $"🛵 Sắp đến hạn trả xe (còn {remainingMin} phút)",
                            $"Đơn thuê xe {v.RentalCode} ({v.VehicleName}{roomStr} - Khách: {v.CustomerName}) còn {remainingMin} phút nữa đến giờ hẹn trả lúc {returnTime:HH:mm dd/MM}.",
                            NotificationType.VehicleReturn,
                            "All",
                            roomNumber: v.RoomNumber,
                            orderCode: v.RentalCode,
                            serviceCategory: "Thuê xe máy"
                        );
                    }
                }

                // 4. Cảnh báo sảnh sự kiện sắp diễn ra (15-30 phút), tự động chuyển Đang sử dụng, sắp kết thúc (trước 15 phút) hoặc quá giờ 15-30 phút mà chưa trả sảnh/quyết toán
                foreach (var eb in data.EventBookings.Where(x => !x.DaThanhToan && x.OrderStatus != "Hoàn tất" && x.OrderStatus != "Đã hủy" && x.PaymentStatus != "Hoàn tất" && x.PaymentStatus != "Đã hủy"))
                {
                    DateTime endTime = eb.EndTime;
                    DateTime startTime = eb.StartTime;
                    string roomInfo = string.IsNullOrWhiteSpace(eb.RoomNumber) ? "" : $" (Phòng: {eb.RoomNumber})";

                    // 4a. Thông báo sự kiện sắp diễn ra (trước 15 - 30 phút)
                    string keyEventStartSoon = $"EVENT_START_SOON_{eb.BookingCode}_{startTime:yyyyMMddHHmm}";
                    if (startTime > now && startTime <= now.AddMinutes(30) && !_notifiedKeys.Contains(keyEventStartSoon))
                    {
                        _notifiedKeys.Add(keyEventStartSoon);
                        int remainingStartMin = Math.Max(1, (int)Math.Ceiling((startTime - now).TotalMinutes));
                        AddNotification(
                            $"🎪 Sự kiện sắp diễn ra (còn {remainingStartMin} phút)",
                            $"Đơn sự kiện {eb.BookingCode} [{eb.SpaceName}]{roomInfo} - Khách: {eb.CustomerName} sắp diễn ra lúc {startTime:HH:mm dd/MM} (còn {remainingStartMin} phút). Vui lòng chuẩn bị sảnh và thiết bị đón khách.",
                            NotificationType.EventEnding,
                            "All",
                            roomNumber: eb.RoomNumber,
                            orderCode: eb.BookingCode,
                            serviceCategory: "Sự kiện"
                        );
                    }

                    // 4b. Tự động chuyển sang "Đang sử dụng" khi đến thời gian bắt đầu
                    if (now >= startTime && now < endTime && eb.OrderStatus != "Đang sử dụng")
                    {
                        eb.OrderStatus = "Đang sử dụng";
                        eb.PaymentStatus = "Đang sử dụng";
                        _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                            "UPDATE DonDatSuKien SET TrangThaiDon = N'Đang sử dụng' WHERE MaDon = @Ma AND TrangThaiDon = N'Đã đặt';",
                            new Microsoft.Data.SqlClient.SqlParameter("@Ma", eb.BookingCode));

                        string keyStart = $"EVENT_STARTED_{eb.BookingCode}_{startTime:yyyyMMddHHmm}";
                        if (!_notifiedKeys.Contains(keyStart))
                        {
                            _notifiedKeys.Add(keyStart);
                            AddNotification(
                                "🎪 Sự kiện bắt đầu diễn ra",
                                $"Đơn sự kiện {eb.BookingCode} [{eb.SpaceName}]{roomInfo} - Khách: {eb.CustomerName} đã đến giờ bắt đầu ({startTime:HH:mm}). Hệ thống chuyển trạng thái sang 'Đang sử dụng'.",
                                NotificationType.EventEnding,
                                "All",
                                roomNumber: eb.RoomNumber,
                                orderCode: eb.BookingCode,
                                serviceCategory: "Sự kiện"
                            );
                        }
                    }

                    // 4c. Quá giờ hẹn trả 15-30 phút rồi mà vẫn chưa thực hiện trả sảnh / sự kiện
                    string keyEventOverdue = $"EVENT_OVERDUE_{eb.BookingCode}_{endTime:yyyyMMddHHmm}";
                    string keyEventSoon = $"EVENT_SOON_{eb.BookingCode}_{endTime:yyyyMMddHHmm}";
                    if (now >= endTime.AddMinutes(15) && !_notifiedKeys.Contains(keyEventOverdue))
                    {
                        _notifiedKeys.Add(keyEventOverdue);
                        int overdueMin = (int)Math.Max(15, (now - endTime).TotalMinutes);
                        AddNotification(
                            "⏰ Quá hạn sử dụng sảnh / sự kiện!",
                            $"Đơn sự kiện {eb.BookingCode} [{eb.SpaceName}]{roomInfo} - Khách: {eb.CustomerName} đã quá giờ kết thúc {overdueMin} phút (kết thúc lúc {endTime:HH:mm dd/MM}) mà chưa thực hiện trả sảnh, quyết toán. Vui lòng kiểm tra và quyết toán!",
                            NotificationType.EventEnding,
                            "All",
                            roomNumber: eb.RoomNumber,
                            orderCode: eb.BookingCode,
                            serviceCategory: "Sự kiện"
                        );
                    }
                    // 4d. Còn 15 phút nữa đến giờ kết thúc
                    else if (endTime > now && endTime <= now.AddMinutes(15) && !_notifiedKeys.Contains(keyEventSoon))
                    {
                        _notifiedKeys.Add(keyEventSoon);
                        int remainingMin = Math.Max(1, (int)Math.Ceiling((endTime - now).TotalMinutes));
                        AddNotification(
                            $"🎪 Sự kiện sắp kết thúc (còn {remainingMin} phút)",
                            $"Đơn sự kiện {eb.BookingCode} [{eb.SpaceName}]{roomInfo} - Khách: {eb.CustomerName} còn {remainingMin} phút nữa đến giờ kết thúc lúc {endTime:HH:mm}. Vui lòng chuẩn bị kiểm tra sảnh và quyết toán.",
                            NotificationType.EventEnding,
                            "All",
                            roomNumber: eb.RoomNumber,
                            orderCode: eb.BookingCode,
                            serviceCategory: "Sự kiện"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Scan Alert Error] {ex.Message}");
            }
        }

        public void MarkAsRead(AppNotification notif)
        {
            notif.IsRead = true;
        }

        public void MarkAllAsRead()
        {
            if (App.Current?.Dispatcher != null && !App.Current.Dispatcher.CheckAccess())
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var n in Notifications)
                    {
                        n.IsRead = true;
                    }
                });
            }
            else
            {
                foreach (var n in Notifications)
                {
                    n.IsRead = true;
                }
            }
        }
    }
}
