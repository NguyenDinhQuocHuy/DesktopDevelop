using System;

namespace QuanLyDichVuKhachSan.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public string RoleDisplayName => Role == UserRole.Admin ? "Admin" : "Lễ Tân";
        public bool CanBeLocked => Role != UserRole.Admin;
    }

    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }

    public class Staff
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public string Status { get; set; } = "Đang làm việc";
    }

    public class Shift
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Note { get; set; } = string.Empty;

        public string TimeDisplay => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }

    public class ShiftAssignment
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public DateTime ShiftDate { get; set; }
        public string Note { get; set; } = string.Empty;
    }

    public class HotelServiceConfig : System.ComponentModel.INotifyPropertyChanged
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty; // Tự túc / Ngoài

        public string Icon => Code?.Trim().ToUpperInvariant() switch
        {
            "AN_UONG" => "🍲",
            "SU_KIEN" => "🏛️",
            "THUE_XE" => "🛵",
            "DO_XE" => "🅿️",
            "GIAT_UI" => "🧺",
            _ => "🛎️"
        };

        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(IsActive)));
                    OnActiveChanged?.Invoke(this);
                }
            }
        }

        public string Description { get; set; } = string.Empty;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        public static event Action<HotelServiceConfig>? OnActiveChanged;
    }
}
