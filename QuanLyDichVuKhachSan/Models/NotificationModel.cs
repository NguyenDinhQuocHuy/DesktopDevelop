using System;

namespace QuanLyDichVuKhachSan.Models
{
    public enum NotificationType
    {
        Login,          // Đăng nhập hệ thống (Admin)
        Inventory,      // Nhập kho hàng hóa (Admin)
        Transaction,    // Giao dịch thanh toán (Admin)
        RoomCheckout,   // Cảnh báo khách sắp trả phòng (Admin + Lễ tân)
        LaundryReturn,  // Cảnh báo giờ hẹn trả đồ giặt ủi (Admin + Lễ tân)
        VehicleReturn,  // Cảnh báo xe thuê sắp đến hạn trả (Admin + Lễ tân)
        EventEnding,    // Cảnh báo sự kiện sắp diễn ra / kết thúc (Admin + Lễ tân)
        SystemAlert     // Cảnh báo hệ thống
    }

    public class AppNotification : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propName));

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string TargetRole { get; set; } = "All"; // "Admin", "Staff", "All"
        public DateTime Timestamp { get; set; } = DateTime.Now;

        private bool _isRead;
        public bool IsRead
        {
            get => _isRead;
            set
            {
                if (_isRead != value)
                {
                    _isRead = value;
                    OnPropertyChanged(nameof(IsRead));
                    OnPropertyChanged(nameof(IsUnread));
                    OnPropertyChanged(nameof(ShowDoubleClickPrompt));
                    OnPropertyChanged(nameof(ToolTipText));
                }
            }
        }

        public bool IsUnread => !IsRead;

        /// <summary>
        /// Xác định xem thông báo có liên quan đến nghiệp vụ thanh toán / quyết toán hay không.
        /// Các thông báo hệ thống đăng nhập, kiểm kho... không có dòng chữ nhấp đúp để quyết toán.
        /// </summary>
        public bool IsSettlementRelated
        {
            get
            {
                // 1. Loại trừ các thông báo hệ thống đăng nhập, kiểm kho, hệ thống chung
                if (Type == NotificationType.Login ||
                    Type == NotificationType.Inventory ||
                    Type == NotificationType.SystemAlert)
                {
                    return false;
                }

                // 2. Loại trừ các thông báo đã hoàn tất thanh toán hoặc thông báo kết quả
                if (Type == NotificationType.Transaction ||
                    Title.Contains("hoàn tất", StringComparison.OrdinalIgnoreCase) ||
                    Title.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                // 3. Các loại thông báo sự kiện liên quan đến hạn trả phòng, trả đồ, trả xe, kết thúc sảnh quyết toán
                if (Type == NotificationType.RoomCheckout ||
                    Type == NotificationType.LaundryReturn ||
                    Type == NotificationType.VehicleReturn ||
                    Type == NotificationType.EventEnding)
                {
                    // Loại trừ thông báo bắt đầu sự kiện / thuê xe
                    if (Title.Contains("bắt đầu", StringComparison.OrdinalIgnoreCase) ||
                        Title.Contains("sắp diễn ra", StringComparison.OrdinalIgnoreCase) ||
                        Title.Contains("sắp đến giờ giao", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Chỉ hiển thị dòng chữ nhỏ "👉 Nhấp đúp để chuyển tới dòng thanh toán" khi là thông báo chưa đọc và liên quan đến quyết toán
        /// </summary>
        public bool ShowDoubleClickPrompt => IsUnread && IsSettlementRelated;

        public string ToolTipText => IsSettlementRelated
            ? "💡 Nhấp đúp chuột để chuyển ngay đến dịch vụ cần thanh toán / xử lý (Chuột phải để đánh dấu Đã đọc)"
            : "Chuột phải để đánh dấu Đã đọc";

        // Điều hướng nghiệp vụ đến dòng cần thanh toán / xử lý
        public string? RoomNumber { get; set; }
        public string? OrderCode { get; set; }
        public string? ServiceCategory { get; set; }

        public string Icon => Type switch
        {
            NotificationType.Login => "🔑",
            NotificationType.Inventory => "📦",
            NotificationType.Transaction => "💳",
            NotificationType.RoomCheckout => "⏰",
            NotificationType.LaundryReturn => "🧺",
            NotificationType.VehicleReturn => "🛵",
            NotificationType.EventEnding => "🎪",
            _ => "🔔"
        };

        public string ColorHex => Type switch
        {
            NotificationType.Login => "#0284C7",        // Sky Blue
            NotificationType.Inventory => "#D97706",    // Amber
            NotificationType.Transaction => "#059669",  // Emerald Green
            NotificationType.RoomCheckout => "#DC2626", // Red Alert
            NotificationType.LaundryReturn => "#7C3AED",// Purple
            NotificationType.VehicleReturn => "#F97316",// Orange
            NotificationType.EventEnding => "#0D9488",  // Teal
            _ => "#64748B"
        };

        public string TimeDisplay => Timestamp.ToString("HH:mm:ss dd/MM");

        public string RelativeTime
        {
            get
            {
                var diff = DateTime.Now - Timestamp;
                if (diff.TotalSeconds < 60) return "Vừa xong";
                if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} phút trước";
                if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} giờ trước";
                return Timestamp.ToString("dd/MM HH:mm");
            }
        }

        /// <summary>
        /// Tự động phân giải số phòng: ưu tiên thuộc tính RoomNumber, nếu rỗng thì regex từ Message/Title
        /// </summary>
        public string? GetResolvedRoomNumber()
        {
            if (!string.IsNullOrWhiteSpace(RoomNumber))
                return RoomNumber.Trim();

            string fullText = $"{Title} {Message}";
            var match = System.Text.RegularExpressions.Regex.Match(
                fullText, 
                @"(?:phòng|Phòng|P\.)\s*[:#]?\s*([0-9]{2,4}[A-Za-z]?)", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match.Success && match.Groups.Count > 1)
                return match.Groups[1].Value.Trim();

            return null;
        }

        /// <summary>
        /// Tự động phân giải mã đơn hàng / giao dịch: ưu tiên OrderCode, nếu rỗng regex mã DH-GU, TX, SK, HD
        /// </summary>
        public string? GetResolvedOrderCode()
        {
            if (!string.IsNullOrWhiteSpace(OrderCode))
                return OrderCode.Trim();

            string fullText = $"{Title} {Message}";
            var match = System.Text.RegularExpressions.Regex.Match(
                fullText,
                @"\b((?:DH-GU|TX|SK|HD-KS|FO|VE)-[A-Za-z0-9-]+)\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match.Success && match.Groups.Count > 1)
                return match.Groups[1].Value.Trim();

            return null;
        }
    }
}
