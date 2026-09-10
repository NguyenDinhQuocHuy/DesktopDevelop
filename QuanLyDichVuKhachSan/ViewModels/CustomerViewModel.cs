using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class CustomerDisplayItem
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public string LatestRoomNumber { get; set; } = string.Empty;
        public string RoomNumberDisplay => !string.IsNullOrWhiteSpace(LatestRoomNumber) ? LatestRoomNumber : "---";
        public string CustomerTypeDisplay => !string.IsNullOrWhiteSpace(LatestRoomNumber) ? "Khách lưu trú" : "Khách vãng lai";
        public string Note { get; set; } = string.Empty;
        public bool IsResident => !string.IsNullOrWhiteSpace(LatestRoomNumber);
        public DateTime? LastActivityDate { get; set; }
    }

    public class CustomerViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;

        // Bộ lọc Thời gian: "Today" (mặc định), "All", "Custom"
        private string _timeFilter = "Today";
        public string TimeFilter
        {
            get => _timeFilter;
            set
            {
                if (SetProperty(ref _timeFilter, value))
                {
                    ApplyFilter();
                }
            }
        }

        private DateTime? _fromDate = DateTime.Today.AddDays(-7);
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                if (SetProperty(ref _fromDate, value))
                {
                    if (TimeFilter == "Custom") ApplyFilter();
                }
            }
        }

        private DateTime? _toDate = DateTime.Today;
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (SetProperty(ref _toDate, value))
                {
                    if (TimeFilter == "Custom") ApplyFilter();
                }
            }
        }

        // Bộ lọc Phân loại: "All" (mặc định), "Resident", "WalkIn"
        private string _typeFilter = "All";
        public string TypeFilter
        {
            get => _typeFilter;
            set
            {
                if (SetProperty(ref _typeFilter, value))
                {
                    ApplyFilter();
                }
            }
        }

        // Mục sắp xếp / lọc theo Họ tên vs Phòng (Mặc định lọc theo Họ tên khi mở tab)
        private string _sortBy = "Name"; // "Name" hoặc "Room"
        public string SortBy
        {
            get => _sortBy;
            set
            {
                if (SetProperty(ref _sortBy, value))
                {
                    ApplyFilter();
                }
            }
        }

        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    Debounce("CustomerSearch", ApplyFilter, 180);
                }
            }
        }

        public ObservableRangeCollection<CustomerDisplayItem> FilteredCustomers { get; } = new();

        public int TotalFilteredCount => FilteredCustomers.Count;
        public int ResidentCount => FilteredCustomers.Count(c => c.IsResident);
        public int WalkInCount => FilteredCustomers.Count(c => !c.IsResident);

        public ICommand SetTimeFilterCommand { get; }
        public ICommand SetTypeFilterCommand { get; }
        public ICommand SetSortByCommand { get; }
        public ICommand RefreshCommand { get; }

        public CustomerViewModel()
        {
            SetTimeFilterCommand = new RelayCommand(p => { if (p is string mode) TimeFilter = mode; });
            SetTypeFilterCommand = new RelayCommand(p => { if (p is string type) TypeFilter = type; });
            SetSortByCommand = new RelayCommand(p => { if (p is string sort) SortBy = sort; });
            RefreshCommand = new RelayCommand(_ => ApplyFilter());

            ApplyFilter();
        }

        public void ApplyFilter()
        {
            var dict = new Dictionary<string, CustomerDisplayItem>(StringComparer.OrdinalIgnoreCase);

            int idCounter = 1;

            // 1. Khách hàng từ bảng KhachHang (CSDL)
            foreach (var c in Data.Customers)
            {
                if (string.IsNullOrWhiteSpace(c.FullName)) continue;
                string key = c.FullName.Trim();
                dict[key] = new CustomerDisplayItem
                {
                    Id = c.Id > 0 ? c.Id : idCounter++,
                    FullName = c.FullName.Trim(),
                    PhoneNumber = c.PhoneNumber ?? "",
                    IdentityCard = c.IdentityCard ?? "",
                    LatestRoomNumber = DataService.NormalizeRoomNumber(c.RoomNumber),
                    Note = c.Note ?? "",
                    LastActivityDate = DateTime.Today
                };
            }

            // 2. Khách đang lưu trú tại phòng khách sạn
            foreach (var r in Data.HotelRooms.Where(r => r.Status == RoomStatus.Occupied && !string.IsNullOrWhiteSpace(r.CustomerName)))
            {
                string key = r.CustomerName.Trim();
                if (!dict.TryGetValue(key, out var item))
                {
                    item = new CustomerDisplayItem
                    {
                        Id = idCounter++,
                        FullName = key,
                        PhoneNumber = r.PhoneNumber ?? "",
                        IdentityCard = r.IdentityCard ?? "",
                        LatestRoomNumber = DataService.NormalizeRoomNumber(r.RoomNumber),
                        Note = r.Note ?? "Đang lưu trú tại phòng",
                        LastActivityDate = r.CheckInDate ?? DateTime.Today
                    };
                    dict[key] = item;
                }
                else
                {
                    item.LatestRoomNumber = DataService.NormalizeRoomNumber(r.RoomNumber);
                    if (!string.IsNullOrWhiteSpace(r.PhoneNumber)) item.PhoneNumber = r.PhoneNumber;
                    if (!string.IsNullOrWhiteSpace(r.IdentityCard)) item.IdentityCard = r.IdentityCard;
                    item.LastActivityDate = r.CheckInDate ?? DateTime.Today;
                }
            }

            // 3. Khách từ các đơn dịch vụ: Ẩm thực, Thuê xe, Sự kiện, Gửi xe, Giặt ủi
            void MergeServiceCustomer(string? name, string? phone, string? cccd, string? room, string? note, DateTime activityDate)
            {
                if (string.IsNullOrWhiteSpace(name) || name == "Khách vãng lai" || name == "Khách ngoài") return;
                string key = name.Trim();
                string normRoom = DataService.NormalizeRoomNumber(room);

                if (!dict.TryGetValue(key, out var item))
                {
                    item = new CustomerDisplayItem
                    {
                        Id = idCounter++,
                        FullName = key,
                        PhoneNumber = phone ?? "",
                        IdentityCard = cccd ?? "",
                        LatestRoomNumber = normRoom,
                        Note = note ?? "",
                        LastActivityDate = activityDate
                    };
                    dict[key] = item;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(item.LatestRoomNumber) && !string.IsNullOrWhiteSpace(normRoom))
                    {
                        item.LatestRoomNumber = normRoom;
                    }
                    if (string.IsNullOrWhiteSpace(item.PhoneNumber) && !string.IsNullOrWhiteSpace(phone)) item.PhoneNumber = phone;
                    if (string.IsNullOrWhiteSpace(item.IdentityCard) && !string.IsNullOrWhiteSpace(cccd)) item.IdentityCard = cccd;
                    if (activityDate > (item.LastActivityDate ?? DateTime.MinValue)) item.LastActivityDate = activityDate;
                }
            }

            foreach (var fo in Data.FoodOrders) MergeServiceCustomer(fo.CustomerName, "", "", fo.RoomNumber, "Sử dụng ẩm thực / mini-bar", fo.CreatedAt);
            foreach (var vr in Data.VehicleRentals) MergeServiceCustomer(vr.CustomerName, vr.PhoneNumber, vr.IdentityCard, vr.RoomNumber, "Thuê xe máy", vr.RentalDate);
            foreach (var eb in Data.EventBookings) MergeServiceCustomer(eb.CustomerName, eb.PhoneNumber, "", eb.RoomNumber, "Đặt sảnh sự kiện", eb.StartTime);
            foreach (var pr in Data.ParkingRecords) MergeServiceCustomer(pr.CustomerName, pr.PhoneNumber, "", pr.RoomNumber, "Gửi bãi đỗ xe", pr.CheckInTime);
            foreach (var lo in Data.LaundryOrders) MergeServiceCustomer(lo.CustomerName, lo.PhoneNumber, "", lo.RoomNumber, "Sử dụng giặt ủi", lo.ReceivedDate);

            // Bắt đầu lọc
            var query = dict.Values.AsEnumerable();

            // Lọc theo Thời gian: Hôm nay vs Tất cả vs Khoảng ngày (FromDate -> ToDate)
            if (TimeFilter == "Today")
            {
                var today = DateTime.Today;
                var activeResidentNames = Data.HotelRooms
                    .Where(r => r.Status == RoomStatus.Occupied && !string.IsNullOrWhiteSpace(r.CustomerName))
                    .Select(r => r.CustomerName.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                query = query.Where(c => 
                    activeResidentNames.Contains(c.FullName) ||
                    (c.LastActivityDate.HasValue && c.LastActivityDate.Value.Date == today)
                );
            }
            else if (TimeFilter == "Custom")
            {
                var start = FromDate?.Date ?? DateTime.MinValue;
                var end = (ToDate?.Date ?? DateTime.MaxValue).AddDays(1).AddTicks(-1);

                query = query.Where(c => 
                    c.LastActivityDate.HasValue &&
                    c.LastActivityDate.Value >= start &&
                    c.LastActivityDate.Value <= end
                );
            }

            // Lọc theo Loại khách: Tất cả vs Khách lưu trú vs Khách vãng lai
            if (TypeFilter == "Resident")
            {
                query = query.Where(c => c.IsResident);
            }
            else if (TypeFilter == "WalkIn")
            {
                query = query.Where(c => !c.IsResident);
            }

            // Tìm kiếm theo từ khóa (Tên, Số phòng, SĐT, CCCD...) siêu tốc với TextSearchHelper
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                string rawQ = SearchQuery.Trim();
                string cleanQ = TextSearchHelper.ToSearchKey(rawQ);
                query = query.Where(c =>
                    TextSearchHelper.MatchPrepared(c.FullName, cleanQ) ||
                    TextSearchHelper.MatchPrepared(c.LatestRoomNumber, cleanQ) ||
                    (!string.IsNullOrWhiteSpace(c.PhoneNumber) && c.PhoneNumber.Contains(rawQ)) ||
                    (!string.IsNullOrWhiteSpace(c.IdentityCard) && c.IdentityCard.Contains(rawQ)) ||
                    TextSearchHelper.MatchPrepared(c.Note, cleanQ));
            }

            // Sắp xếp / Lọc theo Họ tên (mặc định) vs Số phòng
            List<CustomerDisplayItem> sortedList;
            if (SortBy == "Room")
            {
                // Ưu tiên khách có phòng trước, xếp phòng tăng dần
                sortedList = query
                    .OrderByDescending(c => c.IsResident)
                    .ThenBy(c => c.LatestRoomNumber)
                    .ThenBy(c => c.FullName)
                    .ToList();
            }
            else
            {
                // Mặc định: Lọc & sắp xếp theo Họ và Tên A-Z
                sortedList = query
                    .OrderBy(c => c.FullName)
                    .ToList();
            }

            FilteredCustomers.ReplaceRange(sortedList);

            OnPropertyChanged(nameof(TotalFilteredCount));
            OnPropertyChanged(nameof(ResidentCount));
            OnPropertyChanged(nameof(WalkInCount));
        }
    }
}
