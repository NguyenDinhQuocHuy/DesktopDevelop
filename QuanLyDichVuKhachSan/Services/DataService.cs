using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Models;

namespace QuanLyDichVuKhachSan.Services
{
    /// <summary>
    /// =========================================================================================================================
    /// LỚP QUẢN LÝ DỮ LIỆU VÀ BỘ NHỚ ĐỆM HIỆU NĂNG CAO (IN-MEMORY CACHE & DATA SERVICE)
    /// ĐỒNG BỘ 100% VỚI CSDL SQL SERVER CHÍNH THỨC TRONG Database/QuanLyDichVuKhachSan.sql
    /// =========================================================================================================================
    /// </summary>
    public class DataService
    {
        private static DataService? _instance;
        public static DataService Instance => _instance ??= new DataService();

        // ------------------------------------------------------------------------------------------------------
        // 1. DANH SÁCH BỘ NHỚ ĐỆM CHO UI BINDING
        // ------------------------------------------------------------------------------------------------------
        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<Staff> StaffList { get; } = new();
        public ObservableCollection<Shift> Shifts { get; } = new();
        public ObservableCollection<ShiftAssignment> ShiftAssignments { get; } = new();
        public ObservableCollection<Customer> Customers { get; } = new();
        public ObservableCollection<HotelServiceConfig> ServicesConfig { get; } = new();

        // Sơ đồ phòng & Đặt phòng
        public ObservableCollection<HotelRoom> HotelRooms { get; } = new();
        public ObservableCollection<RoomServiceUsageItem> RoomServiceUsages { get; } = new();
        public ObservableCollection<RoomInvoice> RoomInvoices { get; } = new();

        // Dịch vụ Ẩm thực & Kho
        public ObservableCollection<FoodItem> FoodItems { get; } = new();
        public ObservableCollection<FoodItem> TrashedFoodItems { get; } = new();
        public ObservableCollection<Supplier> Suppliers { get; } = new();
        public ObservableCollection<InventoryBatch> InventoryBatches { get; } = new();
        public ObservableCollection<FoodOrder> FoodOrders { get; } = new();

        // Dịch vụ Sự kiện
        public ObservableCollection<EventSpace> EventSpaces { get; } = new();
        public ObservableCollection<EventBooking> EventBookings { get; } = new();

        // Dịch vụ Thuê xe máy
        public ObservableCollection<Vehicle> Vehicles { get; } = new();
        public ObservableCollection<VehicleRental> VehicleRentals { get; } = new();

        // Dịch vụ Bãi đỗ xe
        public ObservableCollection<ParkingRecord> ParkingRecords { get; } = new();

        // Dịch vụ Giặt ủi ngoài
        public ObservableCollection<LaundryPartner> LaundryPartners { get; } = new();
        public ObservableCollection<LaundryOrder> LaundryOrders { get; } = new();

        // Danh sách số phòng hỗ trợ chọn hoặc gõ tự do (Chỉ các phòng có khách đang ở)
        public ObservableCollection<string> AvailableRoomNumbers { get; } = new() { "" };

        public void RefreshAvailableRoomNumbers()
        {
            AvailableRoomNumbers.Clear();
            AvailableRoomNumbers.Add("");
            var occupiedRooms = HotelRooms
                .Where(r => r.Status == RoomStatus.Occupied && !string.IsNullOrWhiteSpace(r.CustomerName))
                .OrderBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber);

            foreach (var r in occupiedRooms)
            {
                string norm = NormalizeRoomNumber(r.RoomNumber);
                if (!AvailableRoomNumbers.Contains(norm))
                {
                    AvailableRoomNumbers.Add(norm);
                }
            }
        }

        public static string NormalizeRoomNumber(string? roomNumber)
        {
            if (string.IsNullOrWhiteSpace(roomNumber)) return "";
            string clean = roomNumber.Trim().ToUpper();
            if (clean.Contains("KHÔNG CHỌN") || clean.Contains("KHÁCH NGOÀI") || clean == "NONE" || clean == "NULL") return "";
            if (clean.StartsWith("P.")) return clean.Substring(2);
            if (clean.StartsWith("P") && clean.Length > 1 && char.IsDigit(clean[1])) return clean.Substring(1);
            return clean;
        }

        // ------------------------------------------------------------------------------------------------------
        // 2. BẢNG BĂM CHỈ MỤC TỐC ĐỘ CAO O(1)
        // ------------------------------------------------------------------------------------------------------
        private readonly Dictionary<int, FoodItem> _foodItemCache = new();
        private readonly Dictionary<int, Vehicle> _vehicleCache = new();
        private readonly Dictionary<int, EventSpace> _eventSpaceCache = new();
        private readonly Dictionary<int, Customer> _customerCache = new();
        private readonly Dictionary<int, Staff> _staffCache = new();
        private readonly Dictionary<int, LaundryPartner> _laundryPartnerCache = new();
        private readonly Dictionary<int, User> _userCache = new();

        public DataService()
        {
            InitializeAndSeedCache();
        }

        /// <summary>
        /// Khởi tạo và nạp toàn bộ dữ liệu trực tiếp từ CSDL SQL Server
        /// </summary>
        private void InitializeAndSeedCache()
        {
            // 1. Tải tài khoản người dùng
            Users.Clear();
            _userCache.Clear();
            var dbUsers = DatabaseService.Instance.LoadUsersFromDb();
            if (dbUsers != null && dbUsers.Count > 0)
            {
                foreach (var u in dbUsers) AddUserToCache(u);
            }
            else
            {
                AddUserToCache(new User { Id = 1, Username = "admin", Password = "123", FullName = "Chủ Khách Sạn", Role = UserRole.Admin, PhoneNumber = "0948198812", Email = "dalathotel@gmail.com" });
                AddUserToCache(new User { Id = 2, Username = "letan", Password = "123", FullName = "Lễ Tân Trực Chính", Role = UserRole.Receptionist, PhoneNumber = "0912345678", Email = "letan@dalathotel.vn" });
            }

            // 2. Tải phòng khách sạn
            HotelRooms.Clear();
            var dbRooms = DatabaseService.Instance.LoadRoomsFromDb();
            if (dbRooms != null && dbRooms.Count > 0)
            {
                foreach (var r in dbRooms) HotelRooms.Add(r);
            }
            RefreshAvailableRoomNumbers();

            // 3. Tải nhân viên
            StaffList.Clear();
            _staffCache.Clear();
            var dbStaff = DatabaseService.Instance.LoadStaffFromDb();
            if (dbStaff != null && dbStaff.Count > 0)
            {
                foreach (var s in dbStaff) AddStaffToCache(s);
            }

            // 4. Ca trực & Phân công
            Shifts.Clear();
            var dbShifts = DatabaseService.Instance.LoadShiftsFromDb();
            if (dbShifts != null && dbShifts.Count > 0)
            {
                foreach (var sh in dbShifts) Shifts.Add(sh);
            }

            ShiftAssignments.Clear();
            var dbAssignments = DatabaseService.Instance.LoadShiftAssignmentsFromDb(StaffList.ToList(), Shifts.ToList());
            if (dbAssignments != null && dbAssignments.Count > 0)
            {
                foreach (var assign in dbAssignments) ShiftAssignments.Add(assign);
            }

            // 5. Cấu hình dịch vụ hệ thống
            ServicesConfig.Clear();
            var dbServices = DatabaseService.Instance.LoadServicesConfigFromDb();
            if (dbServices != null && dbServices.Count > 0)
            {
                foreach (var sc in dbServices) ServicesConfig.Add(sc);
            }
            else
            {
                ServicesConfig.Add(new HotelServiceConfig { Code = "AN_UONG", Name = "Ăn Uống", ServiceType = "Tự túc", IsActive = true, Description = "Bán lẻ hàng khô, nước giải khát, snack tại phòng & quầy lễ tân" });
                ServicesConfig.Add(new HotelServiceConfig { Code = "SU_KIEN", Name = "Sảnh & Sự Kiện", ServiceType = "Tự túc", IsActive = true, Description = "Cho thuê sảnh tiệc, phòng họp VIP, sân thượng Sky Lounge theo giờ/ngày" });
                ServicesConfig.Add(new HotelServiceConfig { Code = "THUE_XE", Name = "Cho Thuê Xe Máy", ServiceType = "Tự túc", IsActive = true, Description = "Cho thuê xe tay ga, xe số kèm mũ bảo hiểm" });
                ServicesConfig.Add(new HotelServiceConfig { Code = "DO_XE", Name = "Bãi & Hầm Đỗ Xe", ServiceType = "Tự túc", IsActive = true, Description = "Quản lý trông giữ xe máy, ô tô có thu phí theo lượt/giờ/ngày" });
                ServicesConfig.Add(new HotelServiceConfig { Code = "GIAT_UI", Name = "Giặt Ủi", ServiceType = "Dịch vụ ngoài", IsActive = true, Description = "Liên kết đối tác giặt sấy, giặt hấp lấy hoa hồng 25-35%" });
            }

            // 6. Khách hàng
            Customers.Clear();
            _customerCache.Clear();
            var dbCustomers = DatabaseService.Instance.LoadCustomersFromDb();
            if (dbCustomers != null && dbCustomers.Count > 0)
            {
                foreach (var c in dbCustomers) AddCustomerToCache(c);
            }

            // 7. Món ăn & Nhập kho
            FoodItems.Clear();
            TrashedFoodItems.Clear();
            _foodItemCache.Clear();
            var dbFoods = DatabaseService.Instance.LoadFoodItemsFromDb();
            if (dbFoods != null && dbFoods.Count > 0)
            {
                foreach (var f in dbFoods)
                {
                    if (f.IsTrashed)
                    {
                        TrashedFoodItems.Add(f);
                    }
                    else
                    {
                        AddFoodItemToCache(f);
                    }
                }
            }

            Suppliers.Clear();
            var dbSuppliers = DatabaseService.Instance.LoadSuppliersFromDb();
            if (dbSuppliers != null && dbSuppliers.Count > 0)
            {
                foreach (var s in dbSuppliers) Suppliers.Add(s);
            }

            InventoryBatches.Clear();
            var dbBatches = DatabaseService.Instance.LoadInventoryBatchesFromDb(FoodItems.ToList());
            if (dbBatches != null && dbBatches.Count > 0)
            {
                foreach (var b in dbBatches) InventoryBatches.Add(b);
            }

            FoodOrders.Clear();
            var dbFoodOrders = DatabaseService.Instance.LoadFoodOrdersFromDb(FoodItems.ToList(), StaffList.ToList());
            if (dbFoodOrders != null && dbFoodOrders.Count > 0)
            {
                foreach (var fo in dbFoodOrders) FoodOrders.Add(fo);
            }

            // 8. Sảnh sự kiện
            EventSpaces.Clear();
            _eventSpaceCache.Clear();
            var dbSpaces = DatabaseService.Instance.LoadEventSpacesFromDb();
            if (dbSpaces != null && dbSpaces.Count > 0)
            {
                foreach (var sp in dbSpaces) AddEventSpaceToCache(sp);
            }

            EventBookings.Clear();
            var dbEventBookings = DatabaseService.Instance.LoadEventBookingsFromDb(EventSpaces.ToList(), StaffList.ToList());
            if (dbEventBookings != null && dbEventBookings.Count > 0)
            {
                foreach (var eb in dbEventBookings) EventBookings.Add(eb);
            }

            // 9. Xe máy & Thuê xe
            Vehicles.Clear();
            _vehicleCache.Clear();
            var dbVehicles = DatabaseService.Instance.LoadVehiclesFromDb();
            if (dbVehicles != null && dbVehicles.Count > 0)
            {
                foreach (var v in dbVehicles) AddVehicleToCache(v);
            }

            VehicleRentals.Clear();
            var dbRentals = DatabaseService.Instance.LoadVehicleRentalsFromDb(Vehicles.ToList(), StaffList.ToList());
            if (dbRentals != null && dbRentals.Count > 0)
            {
                foreach (var vr in dbRentals) VehicleRentals.Add(vr);
            }
            SyncVehicleFleetStatus();

            // 10. Bãi đỗ xe
            ParkingRecords.Clear();
            var dbParking = DatabaseService.Instance.LoadParkingRecordsFromDb(StaffList.ToList());
            if (dbParking != null && dbParking.Count > 0)
            {
                foreach (var pr in dbParking) ParkingRecords.Add(pr);
            }

            // 11. Đối tác Giặt ủi & Đơn giặt
            LaundryPartners.Clear();
            _laundryPartnerCache.Clear();
            var dbPartners = DatabaseService.Instance.LoadLaundryPartnersFromDb();
            if (dbPartners != null && dbPartners.Count > 0)
            {
                foreach (var lp in dbPartners) AddLaundryPartnerToCache(lp);
            }

            LaundryOrders.Clear();
            var dbLaundryOrders = DatabaseService.Instance.LoadLaundryOrdersFromDb(LaundryPartners.ToList(), StaffList.ToList());
            if (dbLaundryOrders != null && dbLaundryOrders.Count > 0)
            {
                foreach (var lo in dbLaundryOrders) LaundryOrders.Add(lo);
            }

            // 12. Hóa đơn thanh toán phòng mẫu
            RoomInvoices.Clear();
            RoomInvoices.Add(new RoomInvoice
            {
                InvoiceCode = "HD-KS-20260830-101",
                RoomNumber = "101 (Phòng đơn)",
                CustomerName = "Nguyễn Văn Hùng",
                PhoneNumber = "0912345678",
                IdentityCard = "001200001234",
                InvoiceDate = DateTime.Now.AddDays(-1),
                CheckInDate = DateTime.Now.AddDays(-3),
                CheckOutDate = DateTime.Now.AddDays(-1),
                StayNights = 2,
                PricePerNight = 500000,
                RoomCost = 1000000,
                ServicesCost = 160000,
                TotalAmount = 1160000,
                IssuedBy = "Trần Thị Thu Hà",
                PaidItems = new List<RoomServiceUsageItem>
                {
                    new() { ServiceCategory = "Ẩm thực", ServiceName = "2x Mì Ly Omachi Bò Hầm", UsedTime = DateTime.Now.AddDays(-2), Amount = 40000, IsPaid = true },
                    new() { ServiceCategory = "Thuê xe", ServiceName = "Thuê xe máy Honda Wave Alpha", UsedTime = DateTime.Now.AddDays(-2), Amount = 120000, IsPaid = true }
                }
            });
            RoomInvoices.Add(new RoomInvoice
            {
                InvoiceCode = "HD-KS-20260829-202",
                RoomNumber = "202 (Deluxe City View)",
                CustomerName = "Trần Đình Trọng",
                PhoneNumber = "0987654321",
                IdentityCard = "001200005678",
                InvoiceDate = DateTime.Now.AddDays(-2),
                CheckInDate = DateTime.Now.AddDays(-4),
                CheckOutDate = DateTime.Now.AddDays(-2),
                StayNights = 2,
                PricePerNight = 800000,
                RoomCost = 1600000,
                ServicesCost = 250000,
                TotalAmount = 1850000,
                IssuedBy = "Lê Hoàng Nam",
                PaidItems = new List<RoomServiceUsageItem>
                {
                    new() { ServiceCategory = "Giặt ủi", ServiceName = "Giặt sấy khô thông thường 5kg", UsedTime = DateTime.Now.AddDays(-3), Amount = 150000, IsPaid = true },
                    new() { ServiceCategory = "Ẩm thực", ServiceName = "2x Nước cam ép Teppy", UsedTime = DateTime.Now.AddDays(-3), Amount = 100000, IsPaid = true }
                }
            });

            // 13. Nạp danh sách dịch vụ phòng từ các đơn hàng thực tế
            RebuildRoomServiceUsages();
        }

        public void RebuildRoomServiceUsages()
        {
            RoomServiceUsages.Clear();
            int usageId = 1;

            // 1. Đơn ăn uống (tất cả các đơn có gắn số phòng, dù tính chung vào phòng hay thanh toán riêng)
            foreach (var fo in FoodOrders.Where(x => !string.IsNullOrEmpty(x.RoomNumber)))
            {
                var itemNames = string.Join(", ", fo.Items.Select(i => $"{i.Quantity}x {i.FoodItemName}"));
                RoomServiceUsages.Add(new RoomServiceUsageItem
                {
                    Id = usageId++,
                    RoomNumber = NormalizeRoomNumber(fo.RoomNumber),
                    ServiceCategory = "Ẩm thực",
                    ServiceName = string.IsNullOrEmpty(itemNames) ? $"Đơn món ăn ({fo.OrderCode})" : itemNames,
                    Details = $"Mã {fo.OrderCode}" + (!string.IsNullOrEmpty(fo.Note) ? $" ({fo.Note})" : ""),
                    UsedTime = fo.CreatedAt,
                    Amount = fo.TotalAmount,
                    IsPaid = fo.DaThanhToan
                });
            }

            // 2. Đơn sự kiện gắn số phòng
            foreach (var eb in EventBookings.Where(x => !string.IsNullOrEmpty(x.RoomNumber)))
            {
                decimal eventAmount = eb.FinalTotal > eb.DepositAmount ? (eb.FinalTotal - eb.DepositAmount) : eb.FinalTotal;
                RoomServiceUsages.Add(new RoomServiceUsageItem
                {
                    Id = usageId++,
                    RoomNumber = NormalizeRoomNumber(eb.RoomNumber),
                    ServiceCategory = "Sự kiện",
                    ServiceName = $"Thuê {eb.SpaceName}",
                    Details = $"Mã {eb.BookingCode}",
                    UsedTime = eb.StartTime,
                    Amount = eventAmount,
                    IsPaid = eb.DaThanhToan
                });
            }

            // 3. Đơn thuê xe gắn số phòng
            foreach (var vr in VehicleRentals.Where(x => !string.IsNullOrEmpty(x.RoomNumber)))
            {
                decimal vehicleCost = vr.RentalFee + vr.AdditionalCost;
                RoomServiceUsages.Add(new RoomServiceUsageItem
                {
                    Id = usageId++,
                    RoomNumber = NormalizeRoomNumber(vr.RoomNumber),
                    ServiceCategory = "Thuê xe máy",
                    ServiceName = $"{vr.VehicleName} (BS: {vr.LicensePlate})",
                    Details = $"Mã {vr.RentalCode}",
                    UsedTime = vr.RentalDate,
                    Amount = vehicleCost,
                    IsPaid = vr.DaThanhToan
                });
            }

            // 4. Bãi xe gắn số phòng (Chỉ ghi nhận dịch vụ nếu có thu phí > 0 đ; Khách phòng đỗ xe miễn phí 0đ thì không hiện thành dòng dịch vụ tính tiền)
            foreach (var pr in ParkingRecords.Where(x => !string.IsNullOrEmpty(x.RoomNumber)))
            {
                if (pr.ParkingFee <= 0 || pr.ChargeType == ParkingChargeType.ResidentFree)
                    continue;

                RoomServiceUsages.Add(new RoomServiceUsageItem
                {
                    Id = usageId++,
                    RoomNumber = NormalizeRoomNumber(pr.RoomNumber),
                    ServiceCategory = "Bãi đỗ xe",
                    ServiceName = $"Gửi xe {pr.VehicleType} ({pr.LicensePlate})",
                    Details = $"Mã vé {pr.TicketCode}",
                    UsedTime = pr.CheckInTime,
                    Amount = pr.ParkingFee,
                    IsPaid = pr.DaThanhToan
                });
            }

            // 5. Giặt ủi gắn số phòng
            foreach (var lo in LaundryOrders.Where(x => !string.IsNullOrEmpty(x.RoomNumber)))
            {
                if (lo.Status == LaundryStatus.Cancelled)
                {
                    // Nếu đơn bị hủy và khách đã thanh toán trực tiếp lúc nhận đồ:
                    // Thể hiện dòng thanh toán ban đầu (Đã thanh toán) và dòng hoàn tiền âm (Chờ khấu trừ)
                    if (lo.DaThanhToan)
                    {
                        RoomServiceUsages.Add(new RoomServiceUsageItem
                        {
                            Id = usageId++,
                            RoomNumber = NormalizeRoomNumber(lo.RoomNumber),
                            ServiceCategory = "Giặt ủi",
                            ServiceName = $"{lo.ServiceTypeDisplay} ({lo.WeightKg} kg)",
                            Details = $"Mã {lo.OrderCode} ({lo.PaymentMethod})",
                            UsedTime = lo.ReceivedDate,
                            Amount = lo.TotalPrice,
                            IsPaid = true
                        });

                        RoomServiceUsages.Add(new RoomServiceUsageItem
                        {
                            Id = usageId++,
                            RoomNumber = NormalizeRoomNumber(lo.RoomNumber),
                            ServiceCategory = "Giặt ủi",
                            ServiceName = $"Hoàn tiền đơn giặt {lo.OrderCode}",
                            Details = $"Khách hủy đơn {lo.OrderCode} lúc chờ giao tiệm (Hoàn lại tiền đã thu)",
                            UsedTime = lo.ReceivedDate.AddMinutes(5),
                            Amount = -lo.TotalPrice,
                            IsPaid = false,
                            RecordedBy = !string.IsNullOrWhiteSpace(lo.RecordedBy) ? lo.RecordedBy : "Lễ tân"
                        });
                    }
                    // Nếu đơn ghi nợ vào phòng (chưa thanh toán) mà bị hủy: Bỏ qua hoàn toàn, không tính vào bill phòng!
                    continue;
                }

                RoomServiceUsages.Add(new RoomServiceUsageItem
                {
                    Id = usageId++,
                    RoomNumber = NormalizeRoomNumber(lo.RoomNumber),
                    ServiceCategory = "Giặt ủi",
                    ServiceName = $"{lo.ServiceTypeDisplay} ({lo.WeightKg} kg)",
                    Details = $"Mã {lo.OrderCode}",
                    UsedTime = lo.ReceivedDate,
                    Amount = lo.TotalPrice,
                    IsPaid = lo.DaThanhToan
                });
            }
        }

        /// <summary>
        /// Đồng bộ trạng thái của đội xe (Vehicles) dựa trên danh sách hợp đồng thuê xe hiện tại.
        /// Xe nào có hợp đồng trạng thái "Đang thuê" (hoặc đang trong thời gian thuê) thì chuyển sang VehicleStatus.Rented.
        /// Xe nào đang bảo trì thì giữ nguyên VehicleStatus.Maintenance.
        /// Còn lại chuyển về VehicleStatus.Available.
        /// </summary>
        public void SyncVehicleFleetStatus()
        {
            if (Vehicles == null || Vehicles.Count == 0) return;

            var activeRentalVehicleIds = VehicleRentals?
                .Where(r => r.Status == "Đang thuê" || r.OrderStatus == "Đang thuê" ||
                            (!r.DaTraXe && r.Status != "Hoàn tất" && (r.Status == null || !r.Status.StartsWith("Đã hủy")) && r.RentalDate <= DateTime.Now && r.ExpectedReturnDate >= DateTime.Now))
                .Select(r => r.VehicleId)
                .ToHashSet() ?? new HashSet<int>();

            foreach (var v in Vehicles)
            {
                if (v.Status == VehicleStatus.Maintenance)
                {
                    continue;
                }

                if (activeRentalVehicleIds.Contains(v.Id))
                {
                    v.Status = VehicleStatus.Rented;
                }
                else
                {
                    v.Status = VehicleStatus.Available;
                }
            }
        }


        // ------------------------------------------------------------------------------------------------------
        // 3. TRA CỨU HASH TABLE O(1)
        // ------------------------------------------------------------------------------------------------------

        private void AddUserToCache(User user)
        {
            Users.Add(user);
            _userCache[user.Id] = user;
        }

        private void AddStaffToCache(Staff staff)
        {
            StaffList.Add(staff);
            _staffCache[staff.Id] = staff;
        }

        public void AddCustomerToCache(Customer customer)
        {
            Customers.Add(customer);
            _customerCache[customer.Id] = customer;
        }

        public void AddFoodItemToCache(FoodItem item)
        {
            FoodItems.Add(item);
            _foodItemCache[item.Id] = item;
        }

        public void AddVehicleToCache(Vehicle v)
        {
            Vehicles.Add(v);
            _vehicleCache[v.Id] = v;
        }

        public void AddEventSpaceToCache(EventSpace s)
        {
            EventSpaces.Add(s);
            _eventSpaceCache[s.Id] = s;
        }

        public void AddLaundryPartnerToCache(LaundryPartner p)
        {
            LaundryPartners.Add(p);
            _laundryPartnerCache[p.Id] = p;
        }

        public FoodItem? GetFoodItemById(int id) => _foodItemCache.TryGetValue(id, out var item) ? item : null;
        public Vehicle? GetVehicleById(int id) => _vehicleCache.TryGetValue(id, out var v) ? v : null;
        public EventSpace? GetEventSpaceById(int id) => _eventSpaceCache.TryGetValue(id, out var s) ? s : null;
        public LaundryPartner? GetLaundryPartnerById(int id) => _laundryPartnerCache.TryGetValue(id, out var p) ? p : null;

        // ------------------------------------------------------------------------------------------------------
        // 4. THUẬT TOÁN KIỂM TRA GIAO THOA THỜI GIAN SỰ KIỆN (INTERVAL INTERSECTION ALGORITHM - O(K))
        // ------------------------------------------------------------------------------------------------------
        public bool CheckEventBookingOverlap(int spaceId, DateTime start, DateTime end, int excludeBookingId = 0)
        {
            return EventBookings.Any(b =>
                b.SpaceId == spaceId &&
                b.Id != excludeBookingId &&
                b.StartTime < end &&
                b.EndTime > start);
        }

        // ------------------------------------------------------------------------------------------------------
        // 5. THUẬT TOÁN TÌM NHÂN VIÊN TRỰC CA HIỆN TẠI (TIME-SLOT MATCHING ALGORITHM - O(S))
        // ------------------------------------------------------------------------------------------------------
        public int GetCurrentDutyStaffId()
        {
            try
            {
                DateTime now = DateTime.Now;
                TimeSpan nowTime = now.TimeOfDay;

                var currentShift = Shifts.FirstOrDefault(s =>
                {
                    if (s.StartTime <= s.EndTime)
                        return nowTime >= s.StartTime && nowTime < s.EndTime;
                    else
                        return nowTime >= s.StartTime || nowTime < s.EndTime;
                });

                if (currentShift != null)
                {
                    var assignment = ShiftAssignments.FirstOrDefault(a => 
                        a.ShiftDate.Date == now.Date && a.ShiftId == currentShift.Id);

                    if (assignment != null && assignment.StaffId > 0)
                    {
                        return assignment.StaffId;
                    }
                }

                var firstStaff = StaffList.FirstOrDefault(x => x.Status == "Đang làm việc");
                if (firstStaff != null) return firstStaff.Id;
            }
            catch { }
            return 1;
        }

        public string GetCurrentDutyStaffName(DateTime? atTime = null)
        {
            try
            {
                // Nếu tài khoản đang đăng nhập là Admin -> Ghi nhận là "Chủ khách sạn"
                if (AuthService.Instance.IsAdmin)
                {
                    return "Chủ khách sạn";
                }

                // Nếu là tài khoản Lễ tân -> Lấy tên nhân viên theo ca trực được phân công tại thời điểm đó
                DateTime targetTime = atTime ?? DateTime.Now;
                TimeSpan timeOfDay = targetTime.TimeOfDay;

                var currentShift = Shifts.FirstOrDefault(s =>
                {
                    if (s.StartTime <= s.EndTime)
                        return timeOfDay >= s.StartTime && timeOfDay < s.EndTime;
                    else
                        return timeOfDay >= s.StartTime || timeOfDay < s.EndTime;
                });

                if (currentShift != null)
                {
                    var assignment = ShiftAssignments.FirstOrDefault(a => 
                        a.ShiftDate.Date == targetTime.Date && a.ShiftId == currentShift.Id);

                    if (assignment != null && assignment.StaffId > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(assignment.StaffName) && assignment.StaffName != "Chưa phân công")
                        {
                            return assignment.StaffName;
                        }
                        var staff = StaffList.FirstOrDefault(x => x.Id == assignment.StaffId);
                        if (staff != null) return staff.FullName;
                    }
                }

                // Nếu thời điểm đó chưa phân ca cụ thể, lấy nhân viên lễ tân đang làm việc trong danh sách
                var activeStaff = StaffList.FirstOrDefault(x => x.Status == "Đang làm việc");
                if (activeStaff != null) return activeStaff.FullName;
                if (StaffList.Count > 0) return StaffList[0].FullName;
            }
            catch { }
            return "Phạm Minh Tuấn";
        }

        public List<TransactionRecord> GetAllTransactions()
        {
            var list = new List<TransactionRecord>();

            // 1. Hóa đơn thanh toán phòng & dịch vụ kèm phòng
            foreach (var inv in RoomInvoices)
            {
                list.Add(new TransactionRecord
                {
                    TransactionCode = inv.InvoiceCode,
                    ServiceCategory = "Tiền phòng & Dịch vụ",
                    CustomerName = inv.CustomerName,
                    PhoneNumber = inv.PhoneNumber,
                    RoomNumber = inv.RoomNumber,
                    PaymentTime = inv.InvoiceDate,
                    Amount = inv.TotalAmount,
                    PaymentMethod = "Thanh toán phòng (Check-out)",
                    CreatedBy = !string.IsNullOrWhiteSpace(inv.IssuedBy) ? inv.IssuedBy : "Chủ khách sạn",
                    PaidBy = !string.IsNullOrWhiteSpace(inv.PaidBy) ? inv.PaidBy : (!string.IsNullOrWhiteSpace(inv.IssuedBy) ? inv.IssuedBy : "Chủ khách sạn"),
                    Status = "Đã thanh toán",
                    OriginalObject = inv
                });
            }

            // 2. Dịch vụ Ăn uống / Mini-bar
            foreach (var fo in FoodOrders)
            {
                string creator = !string.IsNullOrWhiteSpace(fo.RecordedBy) ? fo.RecordedBy : (fo.NguoiTaoId == 0 ? "Chủ khách sạn" : "Phạm Minh Tuấn");
                string payer = !string.IsNullOrWhiteSpace(fo.PaidByStaffName) ? fo.PaidByStaffName : (!string.IsNullOrWhiteSpace(fo.RecordedBy) ? fo.RecordedBy : (fo.NguoiThanhToanId == 0 ? "Chủ khách sạn" : "Phạm Minh Tuấn"));
                list.Add(new TransactionRecord
                {
                    TransactionCode = fo.OrderCode,
                    ServiceCategory = "Ăn uống",
                    CustomerName = fo.CustomerName,
                    PhoneNumber = "",
                    RoomNumber = fo.RoomNumber,
                    PaymentTime = fo.CreatedAt,
                    Amount = fo.TotalAmount,
                    PaymentMethod = fo.PaymentType == FoodPaymentType.DirectPayment 
                        ? (!string.IsNullOrWhiteSpace(fo.PaymentMethod) ? $"Thanh toán trực tiếp ({fo.PaymentMethod})" : "Thanh toán trực tiếp") 
                        : "Ghi nợ vào phòng",
                    CreatedBy = creator,
                    PaidBy = payer,
                    Status = "Đã thanh toán",
                    OriginalObject = fo
                });
            }

            // 3. Sảnh & Sự kiện
            foreach (var eb in EventBookings)
            {
                decimal amt = (eb.PaymentStatus == "Hoàn tất" || eb.PaymentStatus == "Ghi nợ vào phòng") ? eb.FinalTotal : eb.DepositAmount;
                string creator = !string.IsNullOrWhiteSpace(eb.RecordedBy) ? eb.RecordedBy : (eb.NguoiTaoId == 0 ? "Chủ khách sạn" : "Trần Thị Mai");
                string payer = !string.IsNullOrWhiteSpace(eb.PaidByStaffName) ? eb.PaidByStaffName : (!string.IsNullOrWhiteSpace(eb.RecordedBy) ? eb.RecordedBy : (eb.NguoiThanhToanId == 0 ? "Chủ khách sạn" : "Trần Thị Mai"));
                list.Add(new TransactionRecord
                {
                    TransactionCode = eb.BookingCode,
                    ServiceCategory = "Sảnh & Sự kiện",
                    CustomerName = eb.CustomerName,
                    PhoneNumber = eb.PhoneNumber,
                    RoomNumber = eb.RoomNumber,
                    PaymentTime = eb.EndTime,
                    Amount = amt,
                    PaymentMethod = eb.PaymentStatus == "Hoàn tất" 
                        ? "Thanh toán trực tiếp (Quyết toán)" 
                        : (eb.PaymentStatus == "Ghi nợ vào phòng" ? "Ghi nợ vào phòng" : "Đặt cọc trước"),
                    CreatedBy = creator,
                    PaidBy = payer,
                    Status = eb.PaymentStatus,
                    OriginalObject = eb
                });
            }

            // 4. Thuê xe máy
            foreach (var vr in VehicleRentals)
            {
                string creator = !string.IsNullOrWhiteSpace(vr.RecordedBy) ? vr.RecordedBy : (vr.NguoiTaoId == 0 ? "Chủ khách sạn" : "Nguyễn Văn An");
                string payer = !string.IsNullOrWhiteSpace(vr.PaidByStaffName) ? vr.PaidByStaffName : (!string.IsNullOrWhiteSpace(vr.RecordedBy) ? vr.RecordedBy : (vr.NguoiThanhToanId == 0 ? "Chủ khách sạn" : "Nguyễn Văn An"));
                list.Add(new TransactionRecord
                {
                    TransactionCode = vr.RentalCode,
                    ServiceCategory = "Thuê xe máy",
                    CustomerName = vr.CustomerName,
                    PhoneNumber = vr.PhoneNumber,
                    RoomNumber = vr.RoomNumber,
                    PaymentTime = vr.ActualReturnDate ?? vr.RentalDate,
                    Amount = vr.TotalPayment > 0 ? vr.TotalPayment : vr.RentalFee,
                    PaymentMethod = (vr.PaymentStatus == "Ghi nợ vào phòng" || vr.Status == "Ghi nợ vào phòng")
                        ? "Ghi nợ vào phòng"
                        : (string.IsNullOrWhiteSpace(vr.RoomNumber) ? "Thanh toán trực tiếp" : (vr.DaThanhToan ? "Thanh toán trực tiếp" : "Ghi nợ vào phòng")),
                    CreatedBy = creator,
                    PaidBy = payer,
                    Status = vr.Status == "Đã trả" ? "Đã thanh toán" : vr.Status,
                    OriginalObject = vr
                });
            }

            // 5. Bãi gửi xe (Chỉ ghi nhận khách vãng lai đậu đỗ xe có thu phí > 0 đ; Bỏ qua xe khách phòng miễn phí 0đ)
            foreach (var pr in ParkingRecords)
            {
                if (pr.ParkingFee <= 0 || pr.ChargeType == ParkingChargeType.ResidentFree || !string.IsNullOrWhiteSpace(pr.RoomNumber))
                    continue;

                string creator = !string.IsNullOrWhiteSpace(pr.RecordedBy) ? pr.RecordedBy : (pr.NguoiTaoId == 0 ? "Chủ khách sạn" : "Lê Hoàng Nam");
                string payer = !string.IsNullOrWhiteSpace(pr.PaidByStaffName) ? pr.PaidByStaffName : (!string.IsNullOrWhiteSpace(pr.RecordedBy) ? pr.RecordedBy : (pr.NguoiThanhToanId == 0 ? "Chủ khách sạn" : "Lê Hoàng Nam"));
                list.Add(new TransactionRecord
                {
                    TransactionCode = pr.TicketCode,
                    ServiceCategory = "Bãi gửi xe",
                    CustomerName = pr.CustomerName,
                    PhoneNumber = pr.PhoneNumber,
                    RoomNumber = pr.RoomNumber,
                    PaymentTime = pr.CheckOutTime ?? pr.CheckInTime,
                    Amount = pr.ParkingFee,
                    PaymentMethod = "Thanh toán trực tiếp",
                    CreatedBy = creator,
                    PaidBy = payer,
                    Status = "Đã thanh toán",
                    OriginalObject = pr
                });
            }

            // 6. Giặt ủi
            foreach (var lo in LaundryOrders)
            {
                string creator = !string.IsNullOrWhiteSpace(lo.RecordedBy) ? lo.RecordedBy : (lo.NguoiTaoId == 0 ? "Chủ khách sạn" : "Trần Thị Mai");
                string payer = !string.IsNullOrWhiteSpace(lo.PaidByStaffName) ? lo.PaidByStaffName : (!string.IsNullOrWhiteSpace(lo.RecordedBy) ? lo.RecordedBy : (lo.NguoiThanhToanId == 0 ? "Chủ khách sạn" : "Trần Thị Mai"));
                list.Add(new TransactionRecord
                {
                    TransactionCode = lo.OrderCode,
                    ServiceCategory = "Giặt ủi",
                    CustomerName = lo.CustomerName,
                    PhoneNumber = lo.PhoneNumber,
                    RoomNumber = lo.RoomNumber,
                    PaymentTime = lo.ReceivedDate,
                    Amount = lo.TotalPrice,
                    PaymentMethod = string.IsNullOrWhiteSpace(lo.RoomNumber) ? "Thanh toán trực tiếp" : "Ghi nợ vào phòng",
                    CreatedBy = creator,
                    PaidBy = payer,
                    Status = lo.Status == LaundryStatus.Cancelled ? "Đã hủy" : (lo.DaThanhToan ? "Đã thanh toán" : "Chưa thanh toán"),
                    OriginalObject = lo
                });
            }

            return list.OrderByDescending(x => x.PaymentTime).ToList();
        }

        // ------------------------------------------------------------------------------------------------------
        // 6. THỐNG KÊ TỔNG HỢP SIÊU TỐC TRÊN BỘ NHỚ ĐỆM RAM (IN-MEMORY STREAMING AGGREGATION)
        // ------------------------------------------------------------------------------------------------------

        public List<ServiceRevenueSummary> GetServiceRevenueSummaries(DateTime? start = null, DateTime? end = null)
        {
            var foodList = FoodOrders.Where(x => x.DaThanhToan);
            var eventList = EventBookings.Where(x => x.DaThanhToan);
            var vehicleList = VehicleRentals.Where(x => x.DaThanhToan);
            var parkingList = ParkingRecords.Where(x => x.DaThanhToan);
            var laundryList = LaundryOrders.Where(x => x.DaThanhToan && x.Status != LaundryStatus.Cancelled);

            if (start.HasValue && end.HasValue)
            {
                foodList = foodList.Where(x => x.CreatedAt >= start.Value && x.CreatedAt <= end.Value);
                eventList = eventList.Where(x => x.StartTime >= start.Value && x.StartTime <= end.Value);
                vehicleList = vehicleList.Where(x => x.RentalDate >= start.Value && x.RentalDate <= end.Value);
                parkingList = parkingList.Where(x => x.CheckInTime >= start.Value && x.CheckInTime <= end.Value);
                laundryList = laundryList.Where(x => x.ReceivedDate >= start.Value && x.ReceivedDate <= end.Value);
            }

            decimal foodRev = foodList.Sum(x => x.TotalAmount);
            decimal eventRev = eventList.Sum(x => x.FinalTotal);
            decimal vehicleRev = vehicleList.Sum(x => x.RentalFee + x.AdditionalCost);
            decimal parkingRev = parkingList.Sum(x => x.ParkingFee);
            decimal laundryRev = laundryList.Sum(x => x.HotelEarnings);

            decimal totalAll = foodRev + eventRev + vehicleRev + parkingRev + laundryRev;
            if (totalAll == 0) totalAll = 1;

            return new List<ServiceRevenueSummary>
            {
                new() { ServiceName = "Ăn Uống", ServiceType = "Tự túc", TransactionCount = foodList.Count(), TotalRevenue = foodRev, Percentage = Math.Round((double)(foodRev / totalAll * 100), 1) },
                new() { ServiceName = "Sảnh & Sự Kiện", ServiceType = "Tự túc", TransactionCount = eventList.Count(), TotalRevenue = eventRev, Percentage = Math.Round((double)(eventRev / totalAll * 100), 1) },
                new() { ServiceName = "Cho Thuê Xe Máy", ServiceType = "Tự túc", TransactionCount = vehicleList.Count(), TotalRevenue = vehicleRev, Percentage = Math.Round((double)(vehicleRev / totalAll * 100), 1) },
                new() { ServiceName = "Bãi & Hầm Đỗ Xe", ServiceType = "Tự túc", TransactionCount = parkingList.Count(), TotalRevenue = parkingRev, Percentage = Math.Round((double)(parkingRev / totalAll * 100), 1) },
                new() { ServiceName = "Giặt Ủi", ServiceType = "Dịch vụ ngoài", TransactionCount = laundryList.Count(), TotalRevenue = laundryRev, Percentage = Math.Round((double)(laundryRev / totalAll * 100), 1) },
            };
        }

        public List<FoodStatSummary> GetFoodStats()
        {
            var list = new List<FoodStatSummary>();
            var soldCounts = FoodOrders
                .SelectMany(o => o.Items)
                .GroupBy(i => i.FoodItemId)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

            foreach (var item in FoodItems)
            {
                soldCounts.TryGetValue(item.Id, out int totalSold);
                list.Add(new FoodStatSummary
                {
                    FoodName = item.Name,
                    FoodType = "Hàng khô (Mini-bar)",
                    TotalQuantitySold = totalSold,
                    TotalRevenue = totalSold * item.Price,
                    StockLeft = item.StockQuantity
                });
            }
            return list.OrderByDescending(x => x.TotalQuantitySold).ToList();
        }

        public List<VehicleStatSummary> GetVehicleStats()
        {
            var list = new List<VehicleStatSummary>();
            foreach (var v in Vehicles)
            {
                int rentalCount = 0;
                decimal totalRev = 0;
                foreach (var r in VehicleRentals.Where(x => x.VehicleId == v.Id && x.DaThanhToan))
                {
                    rentalCount++;
                    totalRev += (r.RentalFee + r.AdditionalCost);
                }

                list.Add(new VehicleStatSummary
                {
                    LicensePlate = v.LicensePlate,
                    VehicleName = v.Name,
                    VehicleType = v.TypeDisplay,
                    RentalCount = rentalCount,
                    TotalRevenue = totalRev,
                    Status = v.StatusDisplay
                });
            }
            return list.OrderByDescending(x => x.RentalCount).ToList();
        }

        public List<EventSpaceStatSummary> GetEventSpaceStats()
        {
            var list = new List<EventSpaceStatSummary>();
            foreach (var sp in EventSpaces)
            {
                int count = 0;
                decimal totalRev = 0;
                var damageNotes = new List<string>();

                foreach (var b in EventBookings.Where(x => x.SpaceId == sp.Id))
                {
                    count++;
                    if (b.DaThanhToan) totalRev += b.FinalTotal;
                    if (!string.IsNullOrWhiteSpace(b.DamageNote))
                    {
                        damageNotes.Add(b.DamageNote);
                    }
                }

                list.Add(new EventSpaceStatSummary
                {
                    SpaceName = sp.Name,
                    BookingCount = count,
                    TotalRevenue = totalRev,
                    DamageReport = damageNotes.Count > 0 ? string.Join("; ", damageNotes) : "Không có hư hại ghi nhận"
                });
            }
            return list.OrderByDescending(x => x.TotalRevenue).ToList();
        }

        public List<LaundryPartnerStatSummary> GetLaundryPartnerStats()
        {
            var list = new List<LaundryPartnerStatSummary>();
            foreach (var p in LaundryPartners)
            {
                int orderCount = 0;
                decimal totalWeight = 0;
                decimal totalVal = 0;
                decimal hotelEarn = 0;
                decimal partnerEarn = 0;

                foreach (var o in LaundryOrders.Where(x => x.PartnerId == p.Id))
                {
                    orderCount++;
                    totalWeight += o.WeightKg;
                    totalVal += o.TotalPrice;
                    if (o.DaThanhToan)
                    {
                        hotelEarn += o.HotelEarnings;
                        partnerEarn += o.PartnerEarnings;
                    }
                }

                list.Add(new LaundryPartnerStatSummary
                {
                    PartnerName = p.Name,
                    OrderCount = orderCount,
                    TotalWeightKg = totalWeight,
                    TotalOrderValue = totalVal,
                    HotelCommission = hotelEarn,
                    PartnerPayout = partnerEarn
                });
            }
            return list.OrderByDescending(x => x.TotalOrderValue).ToList();
        }

        // ------------------------------------------------------------------------------------------------------
        // 7. CÁC THAO TÁC THÊM GIAO DỊCH VÀ ĐỒNG BỘ NỀN REALTIME XUỐNG SQL SERVER
        // ------------------------------------------------------------------------------------------------------

        public void AddFoodOrder(FoodOrder order)
        {
            order.Id = FoodOrders.Count > 0 ? FoodOrders.Max(x => x.Id) + 1 : 1;
            order.OrderCode = $"DH-AU-{order.Id:D3}";
            order.NguoiTaoId = GetCurrentDutyStaffId();
            order.RecordedBy = GetCurrentDutyStaffName();
            order.RoomNumber = NormalizeRoomNumber(order.RoomNumber);

            if (order.PaymentType == FoodPaymentType.SeparateBill)
            {
                order.DaThanhToan = true;
                order.NguoiThanhToanId = order.NguoiTaoId;
                order.ThoiGianThanhToan = DateTime.Now;
                order.Status = "Đã giao";
            }
            else
            {
                order.DaThanhToan = false;
                order.NguoiThanhToanId = null;
                order.ThoiGianThanhToan = null;
                order.Status = "Chờ xử lý";
            }

            FoodOrders.Insert(0, order);

            // Cập nhật tồn kho
            // Trừ tồn kho và lấy giá vốn bình quân tại thời điểm bán
            foreach (var item in order.Items)
            {
                if (_foodItemCache.TryGetValue(item.FoodItemId, out var food))
                {
                    item.CostPrice = food.AverageCostPrice;
                    food.StockQuantity = Math.Max(0, food.StockQuantity - item.Quantity);
                    food.NotifyStockChanged();
                }

                // Gọi sp_DatMonAn trên SQL Server để kiểm tra, trừ kho và lấy giá vốn chuẩn
                try
                {
                    var (success, cost) = DatabaseService.Instance.DatMonAn(item.FoodItemId, item.Quantity);
                    if (cost > 0)
                    {
                        item.CostPrice = cost;
                    }
                }
                catch { }
            }

            string payTypeStr = (order.PaymentType == FoodPaymentType.ChargeToRoom || order.PaymentType == FoodPaymentType.AddToRoomBill) 
                ? "Ghi nợ vào phòng" 
                : "Thanh toán trực tiếp";
            
            string insertSql = @"
                INSERT INTO DonHangMonAn (MaDon, KhachHangId, TenKhach, SoPhong, HinhThucThanhToan, TongTien, DaThanhToan, GhiChu, NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan, NgayTao, TrangThai)
                VALUES (@Ma, @KhachId, @Ten, @Phong, @HTTT, @Tien, @DaTT, @Note, @NguoiTao, @NguoiTT, @ThoiGianTT, @Ngay, @TrangThai);
                DECLARE @NewDonHangId INT = SCOPE_IDENTITY();";

            var paramList = new List<SqlParameter>
            {
                new("@Ma", order.OrderCode),
                new("@KhachId", (object?)order.KhachHangId ?? DBNull.Value),
                new("@Ten", order.CustomerName),
                new("@Phong", string.IsNullOrWhiteSpace(order.RoomNumber) ? DBNull.Value : (object)order.RoomNumber),
                new("@HTTT", payTypeStr),
                new("@Tien", order.TotalAmount),
                new("@DaTT", order.DaThanhToan ? 1 : 0),
                new("@Note", order.Note ?? ""),
                new("@NguoiTao", order.NguoiTaoId),
                new("@NguoiTT", (object?)order.NguoiThanhToanId ?? DBNull.Value),
                new("@ThoiGianTT", (object?)order.ThoiGianThanhToan ?? DBNull.Value),
                new("@Ngay", order.CreatedAt),
                new("@TrangThai", order.Status)
            };

            for (int i = 0; i < order.Items.Count; i++)
            {
                var it = order.Items[i];
                insertSql += $@"
                INSERT INTO ChiTietDonHangMonAn (DonHangId, MonAnId, SoLuong, DonGia, GiaVon)
                VALUES (@NewDonHangId, @FoodId_{i}, @Qty_{i}, @Price_{i}, @Cost_{i});";

                paramList.Add(new SqlParameter($"@FoodId_{i}", it.FoodItemId));
                paramList.Add(new SqlParameter($"@Qty_{i}", it.Quantity));
                paramList.Add(new SqlParameter($"@Price_{i}", it.Price));
                paramList.Add(new SqlParameter($"@Cost_{i}", it.CostPrice));
            }

            _ = DatabaseService.Instance.ExecuteNonQueryAsync(insertSql, paramList.ToArray());

            // Đồng bộ sang sơ đồ phòng nếu có chọn phòng
            if (!string.IsNullOrEmpty(order.RoomNumber))
            {
                var itemNames = string.Join(", ", order.Items.Select(i => $"{i.Quantity}x {i.FoodItemName}"));
                RoomServiceUsages.Insert(0, new RoomServiceUsageItem
                {
                    Id = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1,
                    RoomNumber = order.RoomNumber,
                    ServiceCategory = "Ẩm thực",
                    ServiceName = string.IsNullOrEmpty(itemNames) ? "Đơn Ẩm thực & Mini-bar" : itemNames,
                    Details = $"Mã {order.OrderCode}" + (!string.IsNullOrEmpty(order.Note) ? $" ({order.Note})" : ""),
                    UsedTime = order.CreatedAt,
                    Amount = order.TotalAmount,
                    IsPaid = order.DaThanhToan
                });
            }
        }

        public void AddInventoryBatch(InventoryBatch batch)
        {
            batch.Id = InventoryBatches.Count > 0 ? InventoryBatches.Max(x => x.Id) + 1 : 1;
            if (string.IsNullOrWhiteSpace(batch.ReceiptCode))
            {
                batch.ReceiptCode = $"PN-{DateTime.Now:yyyyMMdd}-{batch.Id:D2}";
            }

            InventoryBatches.Insert(0, batch);

            if (_foodItemCache.TryGetValue(batch.FoodItemId, out var food))
            {
                int retailUnitsToAdd = batch.TotalRetailUnits > 0 ? batch.TotalRetailUnits : batch.Quantity * Math.Max(1, batch.ConversionRate);
                food.StockQuantity += retailUnitsToAdd;
                food.NotifyStockChanged();

                try
                {
                    NotificationService.Instance.NotifyInventoryImport(food.Name, retailUnitsToAdd, batch.TotalCost, batch.ImportedBy ?? "Chủ Khách Sạn");
                }
                catch { }

                // Đồng bộ cập nhật vào CSDL theo cấu trúc mới: PhieuNhapKho và ChiTietPhieuNhapKho
                int staffId = GetCurrentDutyStaffId();
                if (staffId <= 0) staffId = 1;

                string syncSql = @"
                DECLARE @NccId INT, @PnId INT;
                SELECT TOP 1 @NccId = Id FROM NhaCungCap WHERE TenNhaCungCap = @NhaCungCap;
                IF @NccId IS NULL
                BEGIN
                    INSERT INTO NhaCungCap (TenNhaCungCap, TrangThai) VALUES (@NhaCungCap, 1);
                    SET @NccId = SCOPE_IDENTITY();
                END

                INSERT INTO PhieuNhapKho (MaPhieuNhap, NhaCungCapId, NguoiNhapId, NgayNhap, GhiChu)
                VALUES (@MaPhieu, @NccId, @StaffId, @Date, @GhiChu);
                SET @PnId = SCOPE_IDENTITY();

                INSERT INTO ChiTietPhieuNhapKho (PhieuNhapId, MonAnId, DonViNhap, HeSoQuyDoi, SoLuongNhap, DonGiaNhap, SoLo, HanSuDung, GhiChu)
                VALUES (@PnId, @FoodId, @DonVi, @HeSo, @Qty, @Price, @SoLo, @Hsd, @GhiChu);";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@MaPhieu", batch.ReceiptCode),
                    new SqlParameter("@NhaCungCap", string.IsNullOrWhiteSpace(batch.Supplier) ? "NPP Uy Tín" : batch.Supplier.Trim()),
                    new SqlParameter("@StaffId", staffId),
                    new SqlParameter("@Date", batch.ImportDate),
                    new SqlParameter("@GhiChu", batch.Note ?? ""),
                    new SqlParameter("@FoodId", batch.FoodItemId),
                    new SqlParameter("@DonVi", string.IsNullOrWhiteSpace(batch.ImportUnit) ? "Thùng" : batch.ImportUnit),
                    new SqlParameter("@HeSo", batch.ConversionRate > 0 ? batch.ConversionRate : 1),
                    new SqlParameter("@Qty", batch.Quantity > 0 ? batch.Quantity : 1),
                    new SqlParameter("@Price", batch.ImportPrice),
                    new SqlParameter("@SoLo", string.IsNullOrWhiteSpace(batch.BatchNumber) ? (object)DBNull.Value : batch.BatchNumber),
                    new SqlParameter("@Hsd", batch.ExpiryDate.HasValue ? (object)batch.ExpiryDate.Value : DBNull.Value)
                };

                _ = DatabaseService.Instance.ExecuteNonQueryAsync(syncSql, parameters.ToArray());
            }
        }

        public void AddEventBooking(EventBooking booking)
        {
            booking.Id = EventBookings.Count > 0 ? EventBookings.Max(x => x.Id) + 1 : 1;
            booking.BookingCode = $"SK-2026-{booking.Id:D3}";
            booking.RoomNumber = NormalizeRoomNumber(booking.RoomNumber);
            booking.NguoiTaoId = GetCurrentDutyStaffId();
            booking.RecordedBy = GetCurrentDutyStaffName();

            if (booking.PaymentStatus == "Đã thanh toán full" || booking.PaymentStatus == "Hoàn tất")
            {
                booking.DaThanhToan = true;
                booking.NguoiThanhToanId = booking.NguoiTaoId;
                booking.ThoiGianThanhToan = DateTime.Now;
            }
            else
            {
                booking.DaThanhToan = false;
                booking.NguoiThanhToanId = null;
                booking.ThoiGianThanhToan = null;
            }

            EventBookings.Insert(0, booking);

            string initialPay = !string.IsNullOrEmpty(booking.InitialPaymentType)
                ? booking.InitialPaymentType
                : (booking.PaymentStatus == "Đã thanh toán full" || booking.DepositAmount >= booking.TotalEstimatedAmount ? "Thanh toán toàn bộ" : "Đặt cọc trước");

            string orderStatus = !string.IsNullOrEmpty(booking.OrderStatus)
                ? booking.OrderStatus
                : (booking.PaymentStatus == "Đang sử dụng" ? "Đang sử dụng" :
                   booking.PaymentStatus == "Hoàn tất" ? "Hoàn tất" :
                   booking.PaymentStatus == "Đã hủy" ? "Đã hủy - Không hoàn cọc" : "Đã đặt");

            // Đồng bộ xuống CSDL SQL Server (Table DonDatSuKien)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "INSERT INTO DonDatSuKien (MaDon, KhuVucId, KhachHangId, TenKhach, SoDienThoai, SoPhong, ThoiGianBatDau, ThoiGianKetThuc, TienCoc, TongTienDuKien, ChiPhiPhatSinh, GhiChuHuHai, DaThanhToan, HinhThucThanhToanBanDau, TrangThaiDon, NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan, NgayTao) " +
                "VALUES (@Ma, @KhuVucId, @KhachId, @TenKhach, @SDT, @SoPhong, @Start, @End, @Coc, @TongTien, @PhatSinh, @GhiChu, @DaTT, @HinhThucTT, @TrangThaiDon, @NguoiTao, @NguoiTT, @ThoiGianTT, @NgayTao)",
                new SqlParameter("@Ma", booking.BookingCode),
                new SqlParameter("@KhuVucId", booking.SpaceId),
                new SqlParameter("@KhachId", (object?)booking.KhachHangId ?? DBNull.Value),
                new SqlParameter("@TenKhach", booking.CustomerName),
                new SqlParameter("@SDT", booking.PhoneNumber),
                new SqlParameter("@SoPhong", string.IsNullOrWhiteSpace(booking.RoomNumber) ? DBNull.Value : (object)booking.RoomNumber),
                new SqlParameter("@Start", booking.StartTime),
                new SqlParameter("@End", booking.EndTime),
                new SqlParameter("@Coc", booking.DepositAmount),
                new SqlParameter("@TongTien", booking.TotalEstimatedAmount),
                new SqlParameter("@PhatSinh", booking.AdditionalCost),
                new SqlParameter("@GhiChu", booking.DamageNote ?? ""),
                new SqlParameter("@DaTT", booking.DaThanhToan ? 1 : 0),
                new SqlParameter("@HinhThucTT", initialPay),
                new SqlParameter("@TrangThaiDon", orderStatus),
                new SqlParameter("@NguoiTao", booking.NguoiTaoId),
                new SqlParameter("@NguoiTT", (object?)booking.NguoiThanhToanId ?? DBNull.Value),
                new SqlParameter("@ThoiGianTT", (object?)booking.ThoiGianThanhToan ?? DBNull.Value),
                new SqlParameter("@NgayTao", booking.CreatedAt)
            );

            // Đồng bộ sang sơ đồ phòng
            if (!string.IsNullOrEmpty(booking.RoomNumber))
            {
                RoomServiceUsages.Insert(0, new RoomServiceUsageItem
                {
                    Id = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1,
                    RoomNumber = booking.RoomNumber,
                    ServiceCategory = "Sự kiện",
                    ServiceName = $"Thuê {booking.SpaceName}",
                    Details = $"Mã {booking.BookingCode} (Đã cọc {booking.DepositAmount:N0}đ)",
                    UsedTime = booking.StartTime,
                    Amount = booking.FinalTotal - booking.DepositAmount,
                    IsPaid = booking.DaThanhToan
                });
            }
        }

        public void AddVehicleRental(VehicleRental rental)
        {
            rental.Id = VehicleRentals.Count > 0 ? VehicleRentals.Max(x => x.Id) + 1 : 1;
            rental.RentalCode = $"TX-2026-{rental.Id:D3}";
            rental.RoomNumber = NormalizeRoomNumber(rental.RoomNumber);
            rental.NguoiTaoId = GetCurrentDutyStaffId();
            rental.RecordedBy = GetCurrentDutyStaffName();
            rental.DaThanhToan = false;
            rental.NguoiThanhToanId = null;
            rental.ThoiGianThanhToan = null;

            VehicleRentals.Insert(0, rental);

            if (_vehicleCache.TryGetValue(rental.VehicleId, out var v))
            {
                v.Status = VehicleStatus.Rented;
            }

            string initialPay = !string.IsNullOrEmpty(rental.InitialPaymentType)
                ? rental.InitialPaymentType
                : (rental.DepositAmount >= rental.RentalFee ? "Thanh toán toàn bộ" : "Đặt cọc trước");

            string orderStatus = !string.IsNullOrEmpty(rental.OrderStatus)
                ? rental.OrderStatus
                : (rental.Status == "Hoàn tất" ? "Hoàn tất" :
                   rental.Status != null && rental.Status.StartsWith("Đã hủy") ? "Đã hủy - Không hoàn cọc" :
                   rental.Status == "Đã đặt" ? "Đã đặt" : "Đang thuê");

            // Đồng bộ xuống CSDL SQL Server (Table DonThueXe & XeChoThue)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "INSERT INTO DonThueXe (MaDon, XeId, KhachHangId, TenKhach, CCCD, SoDienThoai, SoPhong, NgayThue, NgayTraDuKien, TienCoc, TienThue, ChiPhiPhatSinh, TongTienThanhToan, DaThanhToan, GhiChuKhiNhanXe, GhiChuHuHai, HinhThucThanhToanBanDau, TrangThaiDon, NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan, NgayTao) " +
                "VALUES (@Ma, @XeId, @KhachId, @Ten, @CCCD, @SDT, @Phong, @NgayThue, @NgayTra, @Coc, @TienThue, @PhatSinh, @TongTien, @DaTT, @GhiChuNhan, @GhiChu, @HinhThucTT, @TrangThaiDon, @NguoiTao, @NguoiTT, @ThoiGianTT, @NgayTao);",
                new SqlParameter("@Ma", rental.RentalCode),
                new SqlParameter("@XeId", rental.VehicleId),
                new SqlParameter("@KhachId", (object?)rental.KhachHangId ?? DBNull.Value),
                new SqlParameter("@Ten", rental.CustomerName),
                new SqlParameter("@CCCD", rental.IdentityCard ?? ""),
                new SqlParameter("@SDT", rental.PhoneNumber ?? ""),
                new SqlParameter("@Phong", string.IsNullOrWhiteSpace(rental.RoomNumber) ? DBNull.Value : (object)rental.RoomNumber),
                new SqlParameter("@NgayThue", rental.RentalDate),
                new SqlParameter("@NgayTra", rental.ExpectedReturnDate),
                new SqlParameter("@Coc", rental.DepositAmount),
                new SqlParameter("@TienThue", rental.RentalFee),
                new SqlParameter("@PhatSinh", rental.AdditionalCost),
                new SqlParameter("@TongTien", rental.TotalPayment),
                new SqlParameter("@DaTT", rental.DaThanhToan ? 1 : 0),
                new SqlParameter("@GhiChuNhan", rental.ReceptionNote ?? ""),
                new SqlParameter("@GhiChu", rental.DamageNote ?? ""),
                new SqlParameter("@HinhThucTT", initialPay),
                new SqlParameter("@TrangThaiDon", orderStatus),
                new SqlParameter("@NguoiTao", rental.NguoiTaoId),
                new SqlParameter("@NguoiTT", (rental.DaThanhToan && rental.NguoiThanhToanId.HasValue) ? (object)rental.NguoiThanhToanId.Value : (rental.DaThanhToan ? (object)rental.NguoiTaoId : DBNull.Value)),
                new SqlParameter("@ThoiGianTT", rental.DaThanhToan ? (object)(rental.ThoiGianThanhToan ?? rental.CreatedAt) : DBNull.Value),
                new SqlParameter("@NgayTao", rental.CreatedAt)
            );

            if (!string.IsNullOrEmpty(rental.RoomNumber))
            {
                int rentalDays = Math.Max(1, (int)(rental.ExpectedReturnDate - rental.RentalDate).TotalDays);
                RoomServiceUsages.Insert(0, new RoomServiceUsageItem
                {
                    Id = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1,
                    RoomNumber = rental.RoomNumber,
                    ServiceCategory = "Thuê xe máy",
                    ServiceName = $"{rental.VehicleName} ({rentalDays} ngày)",
                    Details = $"Mã {rental.RentalCode} (BS: {rental.LicensePlate})",
                    UsedTime = rental.RentalDate,
                    Amount = rental.RentalFee + rental.AdditionalCost - rental.DepositAmount,
                    IsPaid = rental.DaThanhToan
                });
            }
        }

        public void CompleteEventBooking(EventBooking booking, decimal additionalCost, string damageNote, bool isRoomCharge = false, string paymentMethod = "Tiền mặt")
        {
            booking.AdditionalCost = additionalCost;
            booking.DamageNote = damageNote;
            booking.OrderStatus = "Hoàn tất";
            booking.PaymentStatus = isRoomCharge ? "Ghi nợ vào phòng" : "Hoàn tất";
            booking.DaThanhToan = !isRoomCharge;
            if (isRoomCharge)
            {
                booking.NguoiThanhToanId = null;
                booking.PaidByStaffName = string.Empty;
                booking.ThoiGianThanhToan = null;
            }
            else
            {
                booking.NguoiThanhToanId = GetCurrentDutyStaffId();
                booking.PaidByStaffName = GetCurrentDutyStaffName();
                booking.ThoiGianThanhToan = DateTime.Now;
            }

            string hinhThucTT = isRoomCharge ? "Ghi nợ vào phòng" : (string.IsNullOrWhiteSpace(paymentMethod) ? "Thanh toán trực tiếp" : paymentMethod);
            string ghiChuLuuDb = booking.DamageNote ?? "";
            if (isRoomCharge && !ghiChuLuuDb.Contains("Ghi nợ vào phòng"))
            {
                ghiChuLuuDb = string.IsNullOrWhiteSpace(ghiChuLuuDb) ? "[Ghi nợ vào phòng]" : $"[Ghi nợ vào phòng] {ghiChuLuuDb}";
            }

            // Cập nhật CSDL ngay lập tức (không trì hoãn)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE DonDatSuKien SET ChiPhiPhatSinh = @ChiPhiPhatSinh, DaThanhToan = @DaThanhToan, TrangThaiDon = N'Hoàn tất', GhiChuHuHai = @GhiChuHuHai, NguoiThanhToanId = @NguoiThanhToanId, ThoiGianThanhToan = @ThoiGianThanhToan WHERE MaDon = @MaDon;",
                new SqlParameter("@ChiPhiPhatSinh", booking.AdditionalCost),
                new SqlParameter("@DaThanhToan", booking.DaThanhToan ? 1 : 0),
                new SqlParameter("@GhiChuHuHai", ghiChuLuuDb),
                new SqlParameter("@NguoiThanhToanId", (object?)booking.NguoiThanhToanId ?? DBNull.Value),
                new SqlParameter("@ThoiGianThanhToan", (object?)booking.ThoiGianThanhToan ?? DBNull.Value),
                new SqlParameter("@MaDon", booking.BookingCode)
            );

            // Đồng bộ sang sơ đồ phòng (RoomServiceUsages)
            if (!string.IsNullOrEmpty(booking.RoomNumber))
            {
                string normRoom = NormalizeRoomNumber(booking.RoomNumber);
                decimal remainingCost = Math.Max(0, booking.FinalTotal - booking.DepositAmount);
                var usage = RoomServiceUsages.FirstOrDefault(x => x.Details.Contains(booking.BookingCode));
                if (usage != null)
                {
                    usage.Amount = isRoomCharge ? remainingCost : booking.FinalTotal;
                    usage.IsPaid = !isRoomCharge;
                    usage.PaymentMethod = hinhThucTT;
                }
                else
                {
                    RoomServiceUsages.Add(new RoomServiceUsageItem
                    {
                        Id = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1,
                        RoomNumber = normRoom,
                        ServiceCategory = "Sự kiện",
                        ServiceName = $"Thuê {booking.SpaceName}",
                        Details = $"Mã {booking.BookingCode}",
                        UsedTime = booking.StartTime,
                        Amount = isRoomCharge ? remainingCost : booking.FinalTotal,
                        IsPaid = !isRoomCharge,
                        PaymentMethod = hinhThucTT
                    });
                }
            }
        }

        public void ReturnVehicle(VehicleRental rental, decimal additionalCost, string damageNote, bool isRoomCharge = false, string paymentMethod = "Tiền mặt")
        {
            rental.ActualReturnDate = DateTime.Now;
            rental.AdditionalCost = additionalCost;
            rental.DamageNote = damageNote;
            rental.TotalPayment = rental.RentalFee + additionalCost;
            rental.Status = isRoomCharge ? "Ghi nợ vào phòng" : "Hoàn tất";
            rental.OrderStatus = "Hoàn tất";
            rental.PaymentStatus = isRoomCharge ? "Ghi nợ vào phòng" : "Hoàn tất";
            rental.DaThanhToan = !isRoomCharge;
            if (isRoomCharge)
            {
                rental.NguoiThanhToanId = null;
                rental.PaidByStaffName = string.Empty;
                rental.ThoiGianThanhToan = null;
            }
            else
            {
                rental.NguoiThanhToanId = GetCurrentDutyStaffId();
                rental.PaidByStaffName = GetCurrentDutyStaffName();
                rental.ThoiGianThanhToan = DateTime.Now;
            }

            if (_vehicleCache.TryGetValue(rental.VehicleId, out var v))
            {
                v.Status = VehicleStatus.Available;
            }
            else
            {
                var veh = Vehicles.FirstOrDefault(x => x.Id == rental.VehicleId || (!string.IsNullOrEmpty(rental.LicensePlate) && x.LicensePlate == rental.LicensePlate));
                if (veh != null)
                {
                    veh.Status = VehicleStatus.Available;
                    v = veh;
                }
            }

            string hinhThucTT = isRoomCharge ? "Ghi nợ vào phòng" : (string.IsNullOrWhiteSpace(paymentMethod) ? "Thanh toán trực tiếp" : paymentMethod);
            string ghiChuLuuDb = rental.DamageNote ?? "";
            if (isRoomCharge && !ghiChuLuuDb.Contains("Ghi nợ vào phòng"))
            {
                ghiChuLuuDb = string.IsNullOrWhiteSpace(ghiChuLuuDb) ? "[Ghi nợ vào phòng]" : $"[Ghi nợ vào phòng] {ghiChuLuuDb}";
            }

            // Đồng bộ cập nhật DonThueXe & XeChoThue ngay lập tức
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE DonThueXe SET NgayTraThucTe = @NgayTra, ChiPhiPhatSinh = @PhatSinh, TongTienThanhToan = @TongTien, DaThanhToan = @DaTT, NguoiThanhToanId = @NguoiTT, ThoiGianThanhToan = @ThoiGianTT, GhiChuHuHai = @GhiChu, TrangThaiDon = N'Hoàn tất' WHERE MaDon = @Ma; UPDATE XeChoThue SET TrangThai = N'Sẵn sàng' WHERE Id = @XeId;",
                new SqlParameter("@NgayTra", rental.ActualReturnDate ?? DateTime.Now),
                new SqlParameter("@PhatSinh", rental.AdditionalCost),
                new SqlParameter("@TongTien", rental.TotalPayment),
                new SqlParameter("@DaTT", rental.DaThanhToan ? 1 : 0),
                new SqlParameter("@NguoiTT", (object?)rental.NguoiThanhToanId ?? DBNull.Value),
                new SqlParameter("@ThoiGianTT", (object?)rental.ThoiGianThanhToan ?? DBNull.Value),
                new SqlParameter("@GhiChu", ghiChuLuuDb),
                new SqlParameter("@Ma", rental.RentalCode),
                new SqlParameter("@XeId", rental.VehicleId)
            );

            if (!string.IsNullOrEmpty(rental.RoomNumber))
            {
                string normRoom = NormalizeRoomNumber(rental.RoomNumber);
                decimal remainingCost = Math.Max(0, rental.TotalPayment - rental.DepositAmount);
                var usage = RoomServiceUsages.FirstOrDefault(x => x.Details.Contains(rental.RentalCode));
                if (usage != null)
                {
                    usage.Amount = isRoomCharge ? remainingCost : rental.TotalPayment;
                    usage.IsPaid = !isRoomCharge;
                    usage.PaymentMethod = hinhThucTT;
                }
                else
                {
                    RoomServiceUsages.Add(new RoomServiceUsageItem
                    {
                        Id = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1,
                        RoomNumber = normRoom,
                        ServiceCategory = "Thuê xe máy",
                        ServiceName = $"{rental.VehicleName} (BS: {rental.LicensePlate})",
                        Details = $"Mã {rental.RentalCode}",
                        UsedTime = rental.RentalDate,
                        Amount = isRoomCharge ? remainingCost : rental.TotalPayment,
                        IsPaid = !isRoomCharge,
                        PaymentMethod = hinhThucTT
                    });
                }
            }
        }

        public void AddParkingRecord(ParkingRecord record)
        {
            record.Id = ParkingRecords.Count > 0 ? ParkingRecords.Max(x => x.Id) + 1 : 1;
            record.TicketCode = $"VE-DX-{record.Id:D3}";
            record.RoomNumber = NormalizeRoomNumber(record.RoomNumber);
            record.NguoiTaoId = GetCurrentDutyStaffId();
            record.RecordedBy = GetCurrentDutyStaffName();

            if (record.ChargeType == ParkingChargeType.ResidentFree)
            {
                record.ParkingFee = 0;
                record.DaThanhToan = true;
                record.NguoiThanhToanId = record.NguoiTaoId;
                record.ThoiGianThanhToan = DateTime.Now;
            }

            ParkingRecords.Insert(0, record);

            string vType = record.VehicleType == "Ô tô" ? "Ô tô" : "Xe máy";
            record.VehicleType = vType;
            string hinhThucStr = (!string.IsNullOrWhiteSpace(record.RoomNumber) || record.ChargeType == ParkingChargeType.ResidentFree)
                ? "Khách phòng (Miễn phí)"
                : (vType == "Xe máy" ? "Theo lượt" : "Theo tiếng");

            // Đồng bộ xuống CSDL SQL Server (Table BaiDoXe)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "INSERT INTO BaiDoXe (MaVe, BienSoXe, LoaiXe, TenKhach, SoDienThoai, SoPhong, ThoiGianVao, HinhThucGui, PhiGui, DaThanhToan, GhiChuSuaChua, NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan) " +
                "VALUES (@Ma, @BienSo, @Loai, @Ten, @SDT, @Phong, @Vao, @HinhThuc, @Phi, @DaTT, @GhiChu, @NguoiTao, @NguoiTT, @ThoiGianTT)",
                new SqlParameter("@Ma", record.TicketCode),
                new SqlParameter("@BienSo", record.LicensePlate),
                new SqlParameter("@Loai", vType),
                new SqlParameter("@Ten", record.CustomerName ?? ""),
                new SqlParameter("@SDT", record.PhoneNumber ?? ""),
                new SqlParameter("@Phong", string.IsNullOrWhiteSpace(record.RoomNumber) ? DBNull.Value : (object)record.RoomNumber),
                new SqlParameter("@Vao", record.CheckInTime),
                new SqlParameter("@HinhThuc", hinhThucStr),
                new SqlParameter("@Phi", record.ParkingFee),
                new SqlParameter("@DaTT", record.DaThanhToan ? 1 : 0),
                new SqlParameter("@GhiChu", record.RepairNote ?? ""),
                new SqlParameter("@NguoiTao", record.NguoiTaoId),
                new SqlParameter("@NguoiTT", (object?)record.NguoiThanhToanId ?? DBNull.Value),
                new SqlParameter("@ThoiGianTT", (object?)record.ThoiGianThanhToan ?? DBNull.Value)
            );

            if (!string.IsNullOrEmpty(record.RoomNumber) && record.ParkingFee > 0 && record.ChargeType != ParkingChargeType.ResidentFree)
            {
                RoomServiceUsages.Insert(0, new RoomServiceUsageItem
                {
                    Id = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1,
                    RoomNumber = record.RoomNumber,
                    ServiceCategory = "Bãi đỗ xe",
                    ServiceName = $"Đỗ xe {record.VehicleType}",
                    Details = $"Mã vé {record.TicketCode} (BS: {record.LicensePlate})",
                    UsedTime = record.CheckInTime,
                    Amount = record.ParkingFee,
                    IsPaid = record.DaThanhToan
                });
            }
        }

        public void AddLaundryOrder(LaundryOrder order, LaundryPartner? partner = null)
        {
            order.Id = LaundryOrders.Count > 0 ? LaundryOrders.Max(x => x.Id) + 1 : 1;
            order.OrderCode = $"DH-GU-{order.Id:D3}";
            order.RoomNumber = NormalizeRoomNumber(order.RoomNumber);
            order.NguoiTaoId = GetCurrentDutyStaffId();
            order.RecordedBy = GetCurrentDutyStaffName();

            if (partner != null && partner.Id > 0)
            {
                order.PartnerId = partner.Id;
                order.PartnerName = partner.Name;
                decimal cost = partner.GetCostPriceForService(order.ServiceType);
                order.DonGiaVonKg = cost;
                order.PartnerEarnings = Math.Round(order.WeightKg * cost, 0);
                order.HotelEarnings = order.TotalPrice - order.PartnerEarnings;
                order.Status = LaundryStatus.Washing;
            }
            else if (order.PartnerId > 0 && _laundryPartnerCache.TryGetValue(order.PartnerId, out var cachedPartner))
            {
                order.PartnerName = cachedPartner.Name;
                decimal cost = cachedPartner.GetCostPriceForService(order.ServiceType);
                order.DonGiaVonKg = cost;
                order.PartnerEarnings = Math.Round(order.WeightKg * cost, 0);
                order.HotelEarnings = order.TotalPrice - order.PartnerEarnings;
                if (order.Status == LaundryStatus.PendingDispatch) order.Status = LaundryStatus.Washing;
            }
            else
            {
                order.PartnerId = 0;
                order.PartnerName = "Chờ giao đối tác";
                order.DonGiaVonKg = null;
                order.HotelEarnings = order.TotalPrice;
                order.PartnerEarnings = 0;
                order.Status = LaundryStatus.PendingDispatch;
            }

            LaundryOrders.Insert(0, order);

            // Đồng bộ xuống CSDL SQL Server (Table DonGiatUi)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "INSERT INTO DonGiatUi (MaDon, KhachHangId, TenKhach, SoDienThoai, SoPhong, DoiTacId, LoaiDichVu, KhoiLuongKg, DonGiaKg, TongTienThuKhach, DonGiaVonKg, TienKhachSanNhan, TienDoiTacNhan, TinhTrangQuanAoLucNhan, NgayNhan, NgayHenTra, HinhThucThanhToan, TrangThai, DaThanhToan, NguoiTaoId, NguoiThanhToanId, ThoiGianThanhToan) " +
                "VALUES (@Ma, @KhachId, @Ten, @SDT, @Phong, @DoiTacId, @Loai, @Kg, @DonGia, @Tong, @DonGiaVon, @TienKS, @TienDT, @GhiChu, @Nhan, @HenTra, @HinhThucTT, @TrangThai, @DaTT, @NguoiTao, @NguoiTT, @ThoiGianTT)",
                new SqlParameter("@Ma", order.OrderCode),
                new SqlParameter("@KhachId", (object?)order.KhachHangId ?? DBNull.Value),
                new SqlParameter("@Ten", order.CustomerName),
                new SqlParameter("@SDT", order.PhoneNumber ?? ""),
                new SqlParameter("@Phong", string.IsNullOrWhiteSpace(order.RoomNumber) ? DBNull.Value : (object)order.RoomNumber),
                new SqlParameter("@DoiTacId", order.PartnerId > 0 ? (object)order.PartnerId : DBNull.Value),
                new SqlParameter("@Loai", order.ServiceTypeDisplay),
                new SqlParameter("@Kg", order.WeightKg),
                new SqlParameter("@DonGia", order.UnitPricePerKg),
                new SqlParameter("@Tong", order.TotalPrice),
                new SqlParameter("@DonGiaVon", (object?)order.DonGiaVonKg ?? DBNull.Value),
                new SqlParameter("@TienKS", order.HotelEarnings),
                new SqlParameter("@TienDT", order.PartnerEarnings),
                new SqlParameter("@GhiChu", order.ClothesConditionNote ?? ""),
                new SqlParameter("@Nhan", order.ReceivedDate),
                new SqlParameter("@HenTra", order.AppointmentDate),
                new SqlParameter("@HinhThucTT", order.PaymentMethod),
                new SqlParameter("@TrangThai", order.StatusDisplay),
                new SqlParameter("@DaTT", order.DaThanhToan ? 1 : 0),
                new SqlParameter("@NguoiTao", order.NguoiTaoId),
                new SqlParameter("@NguoiTT", (object?)order.NguoiThanhToanId ?? DBNull.Value),
                new SqlParameter("@ThoiGianTT", (object?)order.ThoiGianThanhToan ?? DBNull.Value)
            );

            if (!string.IsNullOrEmpty(order.RoomNumber))
            {
                RoomServiceUsages.Insert(0, new RoomServiceUsageItem
                {
                    Id = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1,
                    RoomNumber = order.RoomNumber,
                    ServiceCategory = "Giặt ủi",
                    ServiceName = $"{order.ServiceTypeDisplay} ({order.WeightKg} kg)",
                    Details = $"Mã {order.OrderCode} ({order.PaymentMethod})",
                    UsedTime = order.ReceivedDate,
                    Amount = order.TotalPrice,
                    IsPaid = order.DaThanhToan
                });
            }
        }

        public void AssignPartnerToLaundryOrder(LaundryOrder order, LaundryPartner partner)
        {
            if (order == null || partner == null) return;

            order.PartnerId = partner.Id;
            order.PartnerName = partner.Name;
            order.Status = LaundryStatus.Washing;
            order.NgayGiaoDoiTac = DateTime.Now;

            decimal cost = partner.GetCostPriceForService(order.ServiceType);
            order.DonGiaVonKg = cost;
            order.PartnerEarnings = Math.Round(order.WeightKg * cost, 0);
            order.HotelEarnings = order.TotalPrice - order.PartnerEarnings;

            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE DonGiatUi SET DoiTacId = @DoiTacId, DonGiaVonKg = @DonGiaVon, TienDoiTacNhan = @TienDT, TienKhachSanNhan = @TienKS, NgayGiaoDoiTac = GETDATE(), TrangThai = N'Đang giặt' WHERE MaDon = @MaDon OR (Id = @Id AND @Id > 0)",
                new SqlParameter("@DoiTacId", partner.Id),
                new SqlParameter("@DonGiaVon", cost),
                new SqlParameter("@TienDT", order.PartnerEarnings),
                new SqlParameter("@TienKS", order.HotelEarnings),
                new SqlParameter("@MaDon", order.OrderCode),
                new SqlParameter("@Id", order.Id)
            );
        }

        public void CompleteLaundryOrderReturn(LaundryOrder order, decimal extraCost, bool coDenBu, string? benChiuTrachNhiem, decimal soTienDenBu, string? lyDoDenBu)
        {
            if (order == null) return;

            order.Status = LaundryStatus.Completed;
            order.ActualReturnDate = DateTime.Now;
            order.ChiPhiPhatSinh = extraCost;

            if (coDenBu && soTienDenBu > 0)
            {
                order.CoDenBu = true;
                order.BenChiuTrachNhiem = !string.IsNullOrWhiteSpace(benChiuTrachNhiem) ? benChiuTrachNhiem : "DoiTac";
                order.SoTienDenBu = soTienDenBu;
                order.LyDoDenBu = lyDoDenBu ?? "Bồi thường hư hại/thất lạc đồ giặt";
                order.NgayGhiNhanDenBu = DateTime.Now;
                order.NguoiGhiNhanDenBuId = GetCurrentDutyStaffId();

                // Đẩy tiền đền bù sang bill phòng khách sạn (giảm trừ tiền phòng khi check-out)
                if (!string.IsNullOrEmpty(order.RoomNumber))
                {
                    RoomServiceUsages.Insert(0, new RoomServiceUsageItem
                    {
                        Id = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1,
                        RoomNumber = NormalizeRoomNumber(order.RoomNumber),
                        ServiceCategory = "Đền bù đồ giặt",
                        ServiceName = $"Bồi thường đồ giặt ({order.OrderCode})",
                        Details = $"Quy trách nhiệm: {(order.BenChiuTrachNhiem == "DoiTac" ? "Đối tác giặt" : "Khách sạn")} - {order.LyDoDenBu}",
                        UsedTime = DateTime.Now,
                        Amount = -soTienDenBu, // Số tiền âm để giảm trừ hóa đơn phòng
                        IsPaid = false
                    });
                }
            }

            // Đồng bộ xuống CSDL SQL Server
            int staffId = GetCurrentDutyStaffId();
            _ = DatabaseService.Instance.CompleteLaundryOrderReturnAsync(
                order.Id,
                order.OrderCode,
                extraCost,
                order.CoDenBu,
                order.BenChiuTrachNhiem,
                order.SoTienDenBu,
                order.LyDoDenBu,
                staffId
            );
        }

        public bool CancelLaundryOrder(LaundryOrder order)
        {
            if (order == null || order.Status != LaundryStatus.PendingDispatch) return false;

            order.Status = LaundryStatus.Cancelled;

            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE DonGiatUi SET TrangThai = N'Đã hủy' WHERE MaDon = @MaDon OR (Id = @Id AND @Id > 0)",
                new SqlParameter("@MaDon", order.OrderCode),
                new SqlParameter("@Id", order.Id)
            );

            if (!string.IsNullOrEmpty(order.RoomNumber))
            {
                string norm = NormalizeRoomNumber(order.RoomNumber);
                if (!order.DaThanhToan)
                {
                    // Xóa dòng ghi nợ dịch vụ giặt ủi của đơn này khỏi danh sách dịch vụ phòng
                    var existingUsage = RoomServiceUsages.FirstOrDefault(x =>
                        NormalizeRoomNumber(x.RoomNumber).Equals(norm, StringComparison.OrdinalIgnoreCase) &&
                        x.ServiceCategory == "Giặt ủi" &&
                        (x.Details.Contains(order.OrderCode) || x.ServiceName.Contains(order.OrderCode)) &&
                        !x.IsPaid);
                    if (existingUsage != null)
                    {
                        RoomServiceUsages.Remove(existingUsage);
                    }
                }
                else
                {
                    // Nếu khách đã thanh toán trực tiếp lúc nhận đồ:
                    // Thêm một dòng hoàn tiền âm (-TotalPrice) vào dịch vụ phòng để giảm trừ vào tổng thanh toán phòng (IsPaid = false)
                    int newId = RoomServiceUsages.Count > 0 ? RoomServiceUsages.Max(x => x.Id) + 1 : 1;
                    RoomServiceUsages.Insert(0, new RoomServiceUsageItem
                    {
                        Id = newId,
                        RoomNumber = norm,
                        ServiceCategory = "Giặt ủi",
                        ServiceName = $"Hoàn tiền đơn giặt {order.OrderCode}",
                        Details = $"Khách hủy đơn {order.OrderCode} lúc chờ giao tiệm (Hoàn lại tiền đã thu)",
                        UsedTime = DateTime.Now,
                        Amount = -order.TotalPrice,
                        IsPaid = false,
                        RecordedBy = GetCurrentDutyStaffName()
                    });
                }
            }

            NotificationService.Instance.AddNotification("Hủy Đơn Giặt Ủi", $"Đã hủy đơn giặt [{order.OrderCode}] phòng [{order.RoomNumber}] thành công.", NotificationType.SystemAlert);
            return true;
        }

        // ==================== CÁC PHƯƠNG THỨC XỬ LÝ DỊCH VỤ THEO PHÒNG ====================

        public List<RoomServiceUsageItem> GetServicesForRoom(string roomNumber)
        {
            string norm = NormalizeRoomNumber(roomNumber);
            if (string.IsNullOrEmpty(norm)) return new List<RoomServiceUsageItem>();

            return RoomServiceUsages
                .Where(x => NormalizeRoomNumber(x.RoomNumber).Equals(norm, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.UsedTime)
                .ToList();
        }

        public (List<RoomServiceUsageItem> PaidItems, decimal RoomCost, decimal ServicesCost, decimal TotalAmount) PayRoomServices(string roomNumber)
        {
            string norm = NormalizeRoomNumber(roomNumber);
            if (string.IsNullOrEmpty(norm)) return (new List<RoomServiceUsageItem>(), 0, 0, 0);

            var unpaidList = RoomServiceUsages
                .Where(x => string.Equals(x.RoomNumber, norm, StringComparison.OrdinalIgnoreCase) && !x.IsPaid)
                .ToList();

            decimal servicesCost = 0;
            foreach (var item in unpaidList)
            {
                item.IsPaid = true;
                servicesCost += item.Amount;
            }

            decimal roomCost = 0;
            decimal grandTotal = servicesCost;
            int staffId = GetCurrentDutyStaffId();

            // Đồng bộ trạng thái sang các bảng đơn hàng
            foreach (var food in FoodOrders.Where(x => string.Equals(x.RoomNumber, norm, StringComparison.OrdinalIgnoreCase)))
            {
                food.DaThanhToan = true;
                food.NguoiThanhToanId = staffId;
                food.ThoiGianThanhToan = DateTime.Now;
                food.Status = "Đã giao";
            }
            foreach (var sk in EventBookings.Where(x => string.Equals(x.RoomNumber, norm, StringComparison.OrdinalIgnoreCase)))
            {
                sk.DaThanhToan = true;
                sk.NguoiThanhToanId = staffId;
                sk.ThoiGianThanhToan = DateTime.Now;
                sk.PaymentStatus = "Hoàn tất";
            }
            foreach (var tx in VehicleRentals.Where(x => string.Equals(x.RoomNumber, norm, StringComparison.OrdinalIgnoreCase)))
            {
                tx.DaThanhToan = true;
                tx.NguoiThanhToanId = staffId;
                tx.ThoiGianThanhToan = DateTime.Now;
            }
            foreach (var laundry in LaundryOrders.Where(x => string.Equals(x.RoomNumber, norm, StringComparison.OrdinalIgnoreCase)))
            {
                laundry.DaThanhToan = true;
                laundry.NguoiThanhToanId = staffId;
                laundry.ThoiGianThanhToan = DateTime.Now;
            }
            foreach (var parking in ParkingRecords.Where(x => string.Equals(x.RoomNumber, norm, StringComparison.OrdinalIgnoreCase)))
            {
                parking.DaThanhToan = true;
                parking.NguoiThanhToanId = staffId;
                parking.ThoiGianThanhToan = DateTime.Now;
            }

            // Tạo hóa đơn lưu trữ vào lịch sử giao dịch
            var room = HotelRooms.FirstOrDefault(x => string.Equals(x.RoomNumber, norm, StringComparison.OrdinalIgnoreCase));
            var newInv = new RoomInvoice
            {
                InvoiceCode = $"HD-KS-{DateTime.Now:yyyyMMdd}-{norm}",
                RoomNumber = room != null ? $"{room.RoomNumber} ({room.RoomType})" : norm,
                CustomerName = room?.CustomerName ?? "Khách lưu trú",
                PhoneNumber = room?.PhoneNumber ?? "",
                IdentityCard = room?.IdentityCard ?? "",
                InvoiceDate = DateTime.Now,
                CheckInDate = room?.CheckInDate ?? DateTime.Now,
                CheckOutDate = room?.ExpectedCheckOutDate ?? DateTime.Now,
                StayNights = room?.StayNights ?? 1,
                PricePerNight = room?.PricePerNight ?? 0,
                RoomCost = roomCost,
                ServicesCost = servicesCost,
                TotalAmount = grandTotal,
                IssuedBy = GetCurrentDutyStaffName(),
                PaidItems = unpaidList
            };
            RoomInvoices.Insert(0, newInv);

            // Gọi Stored Procedure chính thức trong CSDL SQL Server
            _ = DatabaseService.Instance.ConfirmRoomPaymentAsync(norm, staffId);

            return (unpaidList, roomCost, servicesCost, grandTotal);
        }

        /// <summary>
        /// Đồng bộ toàn diện dữ liệu xuống CSDL SQL Server
        /// </summary>
        public async Task<bool> SyncDataToDatabaseAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (DatabaseService.Instance.TestConnection())
                    {
                        // 1. Đồng bộ trạng thái toàn bộ phòng khách sạn
                        foreach (var r in HotelRooms)
                        {
                            DatabaseService.Instance.ExecuteNonQueryAsync(
                                "IF EXISTS (SELECT 1 FROM PhongKhachSan WHERE SoPhong = @Phong) " +
                                "UPDATE PhongKhachSan SET TrangThai = @Status, TenKhach = @Ten, SoDienThoai = @Phone, CCCD = @CCCD, NgayNhanPhong = @In, GhiChu = @Note WHERE SoPhong = @Phong " +
                                "ELSE INSERT INTO PhongKhachSan (SoPhong, Tang, LoaiPhong, TrangThai, TenKhach, SoDienThoai, CCCD, NgayNhanPhong, GhiChu) " +
                                "VALUES (@Phong, @Tang, @Loai, @Status, @Ten, @Phone, @CCCD, @In, @Note)",
                                new SqlParameter("@Phong", r.RoomNumber),
                                new SqlParameter("@Tang", r.Floor),
                                new SqlParameter("@Loai", r.RoomType),
                                new SqlParameter("@Status", r.Status.ToString()),
                                new SqlParameter("@Ten", r.CustomerName ?? ""),
                                new SqlParameter("@Phone", r.PhoneNumber ?? ""),
                                new SqlParameter("@CCCD", r.IdentityCard ?? ""),
                                new SqlParameter("@In", (object?)r.CheckInDate ?? DBNull.Value),
                                new SqlParameter("@Note", r.Note ?? "")
                            );
                        }

                        // 2. Tải lại toàn bộ dữ liệu mới nhất từ database
                        ReloadAllFromDatabase();
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        public void ReloadAllFromDatabase()
        {
            try
            {
                var dbStaff = DatabaseService.Instance.LoadStaffFromDb();
                var dbShifts = DatabaseService.Instance.LoadShiftsFromDb();
                var dbAssignments = DatabaseService.Instance.LoadShiftAssignmentsFromDb(dbStaff, dbShifts);
                var dbRooms = DatabaseService.Instance.LoadRoomsFromDb();
                var dbCustomers = DatabaseService.Instance.LoadCustomersFromDb();
                var dbServices = DatabaseService.Instance.LoadServicesConfigFromDb();
                var dbFoods = DatabaseService.Instance.LoadFoodItemsFromDb();
                var dbBatches = DatabaseService.Instance.LoadInventoryBatchesFromDb(dbFoods);
                var dbFoodOrders = DatabaseService.Instance.LoadFoodOrdersFromDb(dbFoods, dbStaff);
                var dbSpaces = DatabaseService.Instance.LoadEventSpacesFromDb();
                var dbEventBookings = DatabaseService.Instance.LoadEventBookingsFromDb(dbSpaces, dbStaff);
                var dbVehicles = DatabaseService.Instance.LoadVehiclesFromDb();
                var dbRentals = DatabaseService.Instance.LoadVehicleRentalsFromDb(dbVehicles, dbStaff);
                var dbParking = DatabaseService.Instance.LoadParkingRecordsFromDb(dbStaff);
                var dbPartners = DatabaseService.Instance.LoadLaundryPartnersFromDb();
                var dbLaundry = DatabaseService.Instance.LoadLaundryOrdersFromDb(dbPartners, dbStaff);

                App.Current?.Dispatcher?.Invoke(() =>
                {
                    if (dbStaff != null && dbStaff.Count > 0)
                    {
                        StaffList.Clear();
                        _staffCache.Clear();
                        foreach (var s in dbStaff) AddStaffToCache(s);
                    }
                    if (dbShifts != null && dbShifts.Count > 0)
                    {
                        Shifts.Clear();
                        foreach (var sh in dbShifts) Shifts.Add(sh);
                    }
                    if (dbAssignments != null && dbAssignments.Count > 0)
                    {
                        ShiftAssignments.Clear();
                        foreach (var assign in dbAssignments) ShiftAssignments.Add(assign);
                    }
                    if (dbRooms != null && dbRooms.Count > 0)
                    {
                        HotelRooms.Clear();
                        foreach (var r in dbRooms) HotelRooms.Add(r);
                    }
                    if (dbCustomers != null && dbCustomers.Count > 0)
                    {
                        Customers.Clear();
                        _customerCache.Clear();
                        foreach (var c in dbCustomers) AddCustomerToCache(c);
                    }
                    if (dbServices != null && dbServices.Count > 0)
                    {
                        ServicesConfig.Clear();
                        foreach (var sc in dbServices) ServicesConfig.Add(sc);
                    }
                    if (dbFoods != null && dbFoods.Count > 0)
                    {
                        FoodItems.Clear();
                        _foodItemCache.Clear();
                        foreach (var f in dbFoods) AddFoodItemToCache(f);
                    }
                    if (dbBatches != null && dbBatches.Count > 0)
                    {
                        InventoryBatches.Clear();
                        foreach (var b in dbBatches) InventoryBatches.Add(b);
                    }
                    if (dbFoodOrders != null && dbFoodOrders.Count > 0)
                    {
                        FoodOrders.Clear();
                        foreach (var fo in dbFoodOrders) FoodOrders.Add(fo);
                    }
                    if (dbSpaces != null && dbSpaces.Count > 0)
                    {
                        EventSpaces.Clear();
                        _eventSpaceCache.Clear();
                        foreach (var sp in dbSpaces) AddEventSpaceToCache(sp);
                    }
                    if (dbEventBookings != null && dbEventBookings.Count > 0)
                    {
                        EventBookings.Clear();
                        foreach (var eb in dbEventBookings) EventBookings.Add(eb);
                    }
                    if (dbVehicles != null && dbVehicles.Count > 0)
                    {
                        Vehicles.Clear();
                        _vehicleCache.Clear();
                        foreach (var v in dbVehicles) AddVehicleToCache(v);
                    }
                    if (dbRentals != null && dbRentals.Count > 0)
                    {
                        VehicleRentals.Clear();
                        foreach (var vr in dbRentals) VehicleRentals.Add(vr);
                    }
                    SyncVehicleFleetStatus();
                    if (dbParking != null && dbParking.Count > 0)
                    {
                        ParkingRecords.Clear();
                        foreach (var pr in dbParking) ParkingRecords.Add(pr);
                    }
                    if (dbPartners != null && dbPartners.Count > 0)
                    {
                        LaundryPartners.Clear();
                        _laundryPartnerCache.Clear();
                        foreach (var lp in dbPartners) AddLaundryPartnerToCache(lp);
                    }
                    if (dbLaundry != null && dbLaundry.Count > 0)
                    {
                        LaundryOrders.Clear();
                        foreach (var lo in dbLaundry) LaundryOrders.Add(lo);
                    }

                    RebuildRoomServiceUsages();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ReloadAllFromDatabase Warning] {ex.Message}");
            }
        }
    }
}
