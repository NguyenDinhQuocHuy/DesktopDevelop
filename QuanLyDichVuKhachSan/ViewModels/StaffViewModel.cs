using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class WeeklyShiftSlot : BaseViewModel
    {
        public DateTime Date { get; set; }
        public string DayOfWeekName { get; set; } = string.Empty;
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string TimeRange { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        private int _staffId;
        public int StaffId
        {
            get => _staffId;
            set => SetProperty(ref _staffId, value);
        }

        private string _staffName = "Chưa phân công";
        public string StaffName
        {
            get => _staffName;
            set
            {
                if (SetProperty(ref _staffName, value))
                {
                    OnPropertyChanged(nameof(IsAssigned));
                    OnPropertyChanged(nameof(StatusBadgeBg));
                    OnPropertyChanged(nameof(StatusBadgeFg));
                    OnPropertyChanged(nameof(SlotBg));
                    OnPropertyChanged(nameof(CardBorderBrush));
                }
            }
        }

        public bool IsAssigned => !string.IsNullOrWhiteSpace(StaffName) && StaffName != "Chưa phân công";
        public string StatusBadgeBg => IsAssigned ? "#DCFCE7" : "#F1F5F9";
        public string StatusBadgeFg => IsAssigned ? "#15803D" : "#94A3B8";

        public string SlotBg => IsSelected ? "#EFF6FF" : (IsAssigned ? "#F0FDF4" : "#F8FAFC");
        public string CardBorderBrush => IsSelected ? "#2563EB" : (IsAssigned ? "#86EFAC" : "#E2E8F0");
        public double CardBorderThickness => IsSelected ? 2.0 : 1.2;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    OnPropertyChanged(nameof(SlotBg));
                    OnPropertyChanged(nameof(CardBorderBrush));
                    OnPropertyChanged(nameof(CardBorderThickness));
                }
            }
        }
    }

    public class WeeklyDaySchedule : BaseViewModel
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; } = string.Empty;
        public string DateDisplay { get; set; } = string.Empty;
        public bool IsToday { get; set; }
        public bool IsSunday { get; set; }

        public WeeklyShiftSlot MorningShift { get; set; } = new WeeklyShiftSlot();
        public WeeklyShiftSlot AfternoonShift { get; set; } = new WeeklyShiftSlot();
        public WeeklyShiftSlot NightShift { get; set; } = new WeeklyShiftSlot();
    }

    public class StaffViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;
        public AuthService Auth => AuthService.Instance;

        public bool IsAdmin => Auth.IsAdmin;
        public string CurrentDutyStaffDisplay => Data.GetCurrentDutyStaffName();

        // Thêm nhân viên mới
        private string _newStaffCode = "NV004";
        public string NewStaffCode { get => _newStaffCode; set => SetProperty(ref _newStaffCode, value); }

        private string _newStaffName = "";
        public string NewStaffName { get => _newStaffName; set => SetProperty(ref _newStaffName, value); }

        private string _newStaffCccd = "";
        public string NewStaffCccd { get => _newStaffCccd; set => SetProperty(ref _newStaffCccd, value); }

        private string _newStaffPhone = "";
        public string NewStaffPhone { get => _newStaffPhone; set => SetProperty(ref _newStaffPhone, value); }

        private string _newStaffEmail = "";
        public string NewStaffEmail { get => _newStaffEmail; set => SetProperty(ref _newStaffEmail, value); }

        private string _newStaffPosition = "Lễ tân";
        public string NewStaffPosition { get => _newStaffPosition; set => SetProperty(ref _newStaffPosition, value); }

        private decimal _newStaffSalary = 8000000;
        public decimal NewStaffSalary { get => _newStaffSalary; set => SetProperty(ref _newStaffSalary, value); }

        // Selection for assignment
        private Staff? _selectedStaffToAssign;
        public Staff? SelectedStaffToAssign
        {
            get => _selectedStaffToAssign;
            set
            {
                if (SetProperty(ref _selectedStaffToAssign, value))
                {
                    OnPropertyChanged(nameof(SelectedStaffDisplayName));
                    ClearSlotSelection();
                }
            }
        }

        public string SelectedStaffDisplayName => SelectedStaffToAssign != null 
            ? $"👤 Nhân viên đang chọn: {SelectedStaffToAssign.FullName} ({SelectedStaffToAssign.Code})" 
            : "⚠️ Chưa chọn nhân viên (Click chọn nhân viên trong danh sách)";

        // Weekly Schedule Matrix
        private DateTime _currentWeekStart;
        public DateTime CurrentWeekStart
        {
            get => _currentWeekStart;
            set
            {
                if (SetProperty(ref _currentWeekStart, value))
                {
                    OnPropertyChanged(nameof(WeekTitleDisplay));
                    RefreshWeeklySchedule();
                }
            }
        }

        public string WeekTitleDisplay => $"Tuần từ Thứ 2 ({CurrentWeekStart:dd/MM/yyyy}) đến Chủ Nhật ({CurrentWeekStart.AddDays(6):dd/MM/yyyy})";

        public ObservableCollection<WeeklyDaySchedule> WeeklyDays { get; } = new ObservableCollection<WeeklyDaySchedule>();

        // Multi-selection of slots (Holding Ctrl)
        private readonly List<WeeklyShiftSlot> _selectedSlots = new List<WeeklyShiftSlot>();
        public IReadOnlyList<WeeklyShiftSlot> SelectedSlots => _selectedSlots;

        private string _selectedSlotsSummary = "⚠️ Chưa chọn ca trực (Click vào ô ca trực hoặc giữ phím Ctrl để chọn nhiều ca)";
        public string SelectedSlotsSummary
        {
            get => _selectedSlotsSummary;
            set => SetProperty(ref _selectedSlotsSummary, value);
        }

        // Commands
        public ICommand AddStaffCommand { get; }
        public ICommand DeleteStaffCommand { get; }
        public ICommand AssignSelectedSlotCommand { get; }
        public ICommand ClearSlotCommand { get; }
        public ICommand PrevWeekCommand { get; }
        public ICommand CurrentWeekCommand { get; }
        public ICommand NextWeekCommand { get; }

        public StaffViewModel()
        {
            AddStaffCommand = new RelayCommand(AddStaff);
            DeleteStaffCommand = new RelayCommand(DeleteStaff);
            AssignSelectedSlotCommand = new RelayCommand(AssignSelectedSlots);
            ClearSlotCommand = new RelayCommand(_ => ClearSelectedSlots());

            PrevWeekCommand = new RelayCommand(_ => CurrentWeekStart = CurrentWeekStart.AddDays(-7));
            CurrentWeekCommand = new RelayCommand(_ => SetWeekToDate(DateTime.Today));
            NextWeekCommand = new RelayCommand(_ => CurrentWeekStart = CurrentWeekStart.AddDays(7));

            // Khởi tạo tuần hiện tại
            SetWeekToDate(DateTime.Today);

            // Mặc định chọn nhân viên đầu tiên
            SelectedStaffToAssign = Data.StaffList.FirstOrDefault();
            UpdateNextStaffCode();
        }

        private void UpdateNextStaffCode()
        {
            int maxId = Data.StaffList.Count > 0 ? Data.StaffList.Max(x => x.Id) : 0;
            NewStaffCode = $"NV{maxId + 1:D3}";
        }

        private void SetWeekToDate(DateTime date)
        {
            // Lấy ngày Thứ 2 của tuần chứa ngày này
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            CurrentWeekStart = date.Date.AddDays(-1 * diff);
        }

        public void RefreshWeeklySchedule()
        {
            _selectedSlots.Clear();
            UpdateSelectedSlotsSummary();

            WeeklyDays.Clear();
            var dayNames = new[] { "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy", "Chủ Nhật" };

            // Tải lại phân công từ CSDL
            var dbAssignments = DatabaseService.Instance.LoadShiftAssignmentsFromDb(Data.StaffList.ToList(), Data.Shifts.ToList());
            if (dbAssignments != null && dbAssignments.Count > 0)
            {
                Data.ShiftAssignments.Clear();
                foreach (var a in dbAssignments)
                {
                    Data.ShiftAssignments.Add(a);
                }
            }

            for (int i = 0; i < 7; i++)
            {
                DateTime dayDate = CurrentWeekStart.AddDays(i);
                string dayName = dayNames[i];

                var daySchedule = new WeeklyDaySchedule
                {
                    Date = dayDate,
                    DayName = dayName,
                    DateDisplay = dayDate.ToString("dd/MM"),
                    IsToday = dayDate.Date == DateTime.Today,
                    IsSunday = dayDate.DayOfWeek == DayOfWeek.Sunday,
                    MorningShift = CreateSlot(dayDate, dayName, 1, "Ca Sáng", "06:00 - 14:00"),
                    AfternoonShift = CreateSlot(dayDate, dayName, 2, "Ca Chiều", "14:00 - 22:00"),
                    NightShift = CreateSlot(dayDate, dayName, 3, "Ca Đêm", "22:00 - 06:00")
                };

                WeeklyDays.Add(daySchedule);
            }
        }

        private WeeklyShiftSlot CreateSlot(DateTime date, string dayName, int shiftId, string shiftName, string timeRange)
        {
            var assign = Data.ShiftAssignments.FirstOrDefault(x => 
                x.ShiftDate.Date == date.Date && 
                (x.ShiftId == shiftId || (shiftId == 1 && x.ShiftName.Contains("Sáng")) || (shiftId == 2 && x.ShiftName.Contains("Chiều")) || (shiftId == 3 && x.ShiftName.Contains("Đêm"))));

            return new WeeklyShiftSlot
            {
                Date = date,
                DayOfWeekName = dayName,
                ShiftId = shiftId,
                ShiftName = shiftName,
                TimeRange = timeRange,
                StaffId = assign?.StaffId ?? 0,
                StaffName = string.IsNullOrWhiteSpace(assign?.StaffName) ? "Chưa phân công" : assign.StaffName,
                Note = assign?.Note ?? ""
            };
        }

        /// <summary>
        /// Xử lý click chọn slot ca trực (Hỗ trợ giữ Ctrl để chọn/bỏ chọn nhiều ca)
        /// </summary>
        public void HandleSlotClick(WeeklyShiftSlot? slot, bool isCtrlPressed)
        {
            if (slot == null) return;

            if (isCtrlPressed)
            {
                // Giữ Ctrl -> Toggle chọn/bỏ chọn slot này
                slot.IsSelected = !slot.IsSelected;
                if (slot.IsSelected)
                {
                    if (!_selectedSlots.Contains(slot)) _selectedSlots.Add(slot);
                }
                else
                {
                    _selectedSlots.Remove(slot);
                }
            }
            else
            {
                // Click đơn lẻ -> Bỏ chọn tất cả slot khác, chỉ chọn slot này
                foreach (var s in _selectedSlots.ToList())
                {
                    s.IsSelected = false;
                }
                _selectedSlots.Clear();

                slot.IsSelected = true;
                _selectedSlots.Add(slot);
            }

            UpdateSelectedSlotsSummary();
        }

        private void UpdateSelectedSlotsSummary()
        {
            if (_selectedSlots.Count == 0)
            {
                SelectedSlotsSummary = "⚠️ Chưa chọn ca trực (Click vào ô ca trực hoặc giữ phím Ctrl để chọn nhiều ca)";
            }
            else if (_selectedSlots.Count == 1)
            {
                var s = _selectedSlots[0];
                SelectedSlotsSummary = $"📅 Đang chọn 1 ca: {s.DayOfWeekName} ({s.Date:dd/MM}) - {s.ShiftName} ({s.TimeRange})";
            }
            else
            {
                SelectedSlotsSummary = $"📅 Đang chọn {_selectedSlots.Count} ca trực cùng lúc (Giữ Ctrl để chọn thêm hoặc bỏ bớt)";
            }
        }

        public void AssignSelectedSlots(object? parameter)
        {
            if (_selectedSlots.Count == 0)
            {
                MessageBox.Show("Vui lòng click chọn ít nhất một ô ca trực (giữ phím Ctrl để chọn nhiều ca) trong Thời khóa biểu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedStaffToAssign == null)
            {
                MessageBox.Show("Vui lòng click chọn một nhân viên trong Danh sách nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var staff = SelectedStaffToAssign;
            int count = _selectedSlots.Count;

            foreach (var slot in _selectedSlots)
            {
                slot.StaffId = staff.Id;
                slot.StaffName = staff.FullName;

                var existing = Data.ShiftAssignments.FirstOrDefault(x => 
                    x.ShiftDate.Date == slot.Date.Date && 
                    (x.ShiftId == slot.ShiftId || (slot.ShiftId == 1 && x.ShiftName.Contains("Sáng")) || (slot.ShiftId == 2 && x.ShiftName.Contains("Chiều")) || (slot.ShiftId == 3 && x.ShiftName.Contains("Đêm"))));

                if (existing != null)
                {
                    existing.StaffId = staff.Id;
                    existing.StaffName = staff.FullName;
                }
                else
                {
                    var newAssign = new ShiftAssignment
                    {
                        Id = Data.ShiftAssignments.Count > 0 ? Data.ShiftAssignments.Max(x => x.Id) + 1 : 1,
                        StaffId = staff.Id,
                        StaffName = staff.FullName,
                        ShiftId = slot.ShiftId,
                        ShiftName = slot.ShiftName,
                        ShiftDate = slot.Date.Date,
                        Note = "Phân công theo thời khóa biểu tuần"
                    };
                    Data.ShiftAssignments.Insert(0, newAssign);
                }

                // Ghi ngay lập tức vào CSDL SQL Server (Table PhanCongCaTruc)
                string sql = @"
IF EXISTS (SELECT 1 FROM PhanCongCaTruc WHERE CaTrucId = @CaTrucId AND CAST(NgayTruc AS DATE) = @NgayTruc)
BEGIN
    UPDATE PhanCongCaTruc 
    SET NhanVienId = @NhanVienId, GhiChu = @GhiChu
    WHERE CaTrucId = @CaTrucId AND CAST(NgayTruc AS DATE) = @NgayTruc
END
ELSE
BEGIN
    INSERT INTO PhanCongCaTruc (NhanVienId, CaTrucId, NgayTruc, GhiChu)
    VALUES (@NhanVienId, @CaTrucId, @NgayTruc, @GhiChu)
END";

                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    sql,
                    new SqlParameter("@NhanVienId", staff.Id),
                    new SqlParameter("@CaTrucId", slot.ShiftId),
                    new SqlParameter("@NgayTruc", slot.Date.Date),
                    new SqlParameter("@GhiChu", $"Phân công {staff.FullName}")
                );
            }

            // Sau khi phân công xong: Thiết lập lại thời khóa biểu về trạng thái không chọn ca (hủy multi-selection)
            ClearSlotSelection();
        }

        public void ClearSlotSelection()
        {
            foreach (var slot in _selectedSlots.ToList())
            {
                slot.IsSelected = false;
            }
            _selectedSlots.Clear();
            UpdateSelectedSlotsSummary();
        }

        public void ClearSelectedSlots()
        {
            if (_selectedSlots.Count == 0) return;

            foreach (var slot in _selectedSlots.ToList())
            {
                slot.StaffId = 0;
                slot.StaffName = "Chưa phân công";

                var existing = Data.ShiftAssignments.FirstOrDefault(x => 
                    x.ShiftDate.Date == slot.Date.Date && 
                    (x.ShiftId == slot.ShiftId || (slot.ShiftId == 1 && x.ShiftName.Contains("Sáng")) || (slot.ShiftId == 2 && x.ShiftName.Contains("Chiều")) || (slot.ShiftId == 3 && x.ShiftName.Contains("Đêm"))));

                if (existing != null)
                {
                    Data.ShiftAssignments.Remove(existing);
                }

                string sql = "DELETE FROM PhanCongCaTruc WHERE CaTrucId = @CaTrucId AND CAST(NgayTruc AS DATE) = @NgayTruc";
                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    sql,
                    new SqlParameter("@CaTrucId", slot.ShiftId),
                    new SqlParameter("@NgayTruc", slot.Date.Date)
                );
            }

            ClearSlotSelection();
        }

        /// <summary>
        /// Phân công trực tiếp 1 ca cho 1 nhân viên (khi chọn từ menu chuột phải)
        /// </summary>
        public void AssignSlotDirectly(WeeklyShiftSlot? slot, Staff? staff)
        {
            if (slot == null || staff == null) return;

            slot.StaffId = staff.Id;
            slot.StaffName = staff.FullName;

            var existing = Data.ShiftAssignments.FirstOrDefault(x => 
                x.ShiftDate.Date == slot.Date.Date && 
                (x.ShiftId == slot.ShiftId || (slot.ShiftId == 1 && x.ShiftName.Contains("Sáng")) || (slot.ShiftId == 2 && x.ShiftName.Contains("Chiều")) || (slot.ShiftId == 3 && x.ShiftName.Contains("Đêm"))));

            if (existing != null)
            {
                existing.StaffId = staff.Id;
                existing.StaffName = staff.FullName;
            }
            else
            {
                var newAssign = new ShiftAssignment
                {
                    Id = Data.ShiftAssignments.Count > 0 ? Data.ShiftAssignments.Max(x => x.Id) + 1 : 1,
                    StaffId = staff.Id,
                    StaffName = staff.FullName,
                    ShiftId = slot.ShiftId,
                    ShiftName = slot.ShiftName,
                    ShiftDate = slot.Date.Date,
                    Note = "Phân công theo thời khóa biểu tuần"
                };
                Data.ShiftAssignments.Insert(0, newAssign);
            }

            // Ghi ngay lập tức vào CSDL SQL Server (Table PhanCongCaTruc)
            string sql = @"
IF EXISTS (SELECT 1 FROM PhanCongCaTruc WHERE CaTrucId = @CaTrucId AND CAST(NgayTruc AS DATE) = @NgayTruc)
BEGIN
    UPDATE PhanCongCaTruc 
    SET NhanVienId = @NhanVienId, GhiChu = @GhiChu
    WHERE CaTrucId = @CaTrucId AND CAST(NgayTruc AS DATE) = @NgayTruc
END
ELSE
BEGIN
    INSERT INTO PhanCongCaTruc (NhanVienId, CaTrucId, NgayTruc, GhiChu)
    VALUES (@NhanVienId, @CaTrucId, @NgayTruc, @GhiChu)
END";

            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                sql,
                new SqlParameter("@NhanVienId", staff.Id),
                new SqlParameter("@CaTrucId", slot.ShiftId),
                new SqlParameter("@NgayTruc", slot.Date.Date),
                new SqlParameter("@GhiChu", $"Phân công {staff.FullName}")
            );

            UpdateSelectedSlotsSummary();
        }

        /// <summary>
        /// Hủy phân công trực tiếp 1 ca (khi chọn từ menu chuột phải)
        /// </summary>
        public void ClearSlotDirectly(WeeklyShiftSlot? slot)
        {
            if (slot == null) return;

            slot.StaffId = 0;
            slot.StaffName = "Chưa phân công";

            var existing = Data.ShiftAssignments.FirstOrDefault(x => 
                x.ShiftDate.Date == slot.Date.Date && 
                (x.ShiftId == slot.ShiftId || (slot.ShiftId == 1 && x.ShiftName.Contains("Sáng")) || (slot.ShiftId == 2 && x.ShiftName.Contains("Chiều")) || (slot.ShiftId == 3 && x.ShiftName.Contains("Đêm"))));

            if (existing != null)
            {
                Data.ShiftAssignments.Remove(existing);
            }

            string sql = "DELETE FROM PhanCongCaTruc WHERE CaTrucId = @CaTrucId AND CAST(NgayTruc AS DATE) = @NgayTruc";
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                sql,
                new SqlParameter("@CaTrucId", slot.ShiftId),
                new SqlParameter("@NgayTruc", slot.Date.Date)
            );

            UpdateSelectedSlotsSummary();
        }

        private void AddStaff(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(NewStaffName) || string.IsNullOrWhiteSpace(NewStaffCccd) || string.IsNullOrWhiteSpace(NewStaffPhone))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ: Tên, CCCD và Số điện thoại nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewStaffName.Any(char.IsDigit))
            {
                MessageBox.Show("Họ & Tên nhân viên không được chứa chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewStaffPhone.Any(c => !char.IsDigit(c)))
            {
                MessageBox.Show("Số điện thoại chỉ được chứa các chữ số (0-9)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newId = Data.StaffList.Count > 0 ? Data.StaffList.Max(x => x.Id) + 1 : 1;
            var staff = new Staff
            {
                Id = newId,
                Code = string.IsNullOrWhiteSpace(NewStaffCode) ? $"NV{newId:D3}" : NewStaffCode.Trim(),
                FullName = NewStaffName.Trim(),
                IdentityCard = NewStaffCccd.Trim(),
                PhoneNumber = NewStaffPhone.Trim(),
                Email = NewStaffEmail?.Trim() ?? "",
                Position = NewStaffPosition,
                BaseSalary = NewStaffSalary,
                Status = "Đang làm việc"
            };

            Data.StaffList.Add(staff);
            SelectedStaffToAssign = staff;

            NewStaffName = "";
            NewStaffCccd = "";
            NewStaffPhone = "";
            NewStaffEmail = "";
            UpdateNextStaffCode();

            // Đồng bộ xuống CSDL SQL Server (Table NhanVien)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "INSERT INTO NhanVien (MaNV, HoTen, CCCD, SoDienThoai, Email, ChucVu, LuongCoBan, TrangThai) VALUES (@MaNV, @HoTen, @CCCD, @Phone, @Email, @ChucVu, @Luong, @TrangThai)",
                new SqlParameter("@MaNV", staff.Code),
                new SqlParameter("@HoTen", staff.FullName),
                new SqlParameter("@CCCD", staff.IdentityCard),
                new SqlParameter("@Phone", staff.PhoneNumber),
                new SqlParameter("@Email", staff.Email),
                new SqlParameter("@ChucVu", staff.Position),
                new SqlParameter("@Luong", staff.BaseSalary),
                new SqlParameter("@TrangThai", staff.Status)
            );

            MessageBox.Show($"Đã thêm nhân viên [{staff.FullName} - {staff.Code}] thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteStaff(object? parameter)
        {
            if (parameter is Staff staff)
            {
                var res = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhân viên [{staff.FullName}]?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    Data.StaffList.Remove(staff);

                    // Xóa các phân công ca trực của nhân viên này khỏi bộ nhớ RAM
                    var assignsToRemove = Data.ShiftAssignments.Where(a => a.StaffId == staff.Id).ToList();
                    foreach (var a in assignsToRemove)
                    {
                        Data.ShiftAssignments.Remove(a);
                    }

                    if (SelectedStaffToAssign == staff)
                    {
                        SelectedStaffToAssign = Data.StaffList.FirstOrDefault();
                    }

                    // Xóa khỏi CSDL SQL Server (Table PhanCongCaTruc & NhanVien)
                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "DELETE FROM PhanCongCaTruc WHERE NhanVienId = @Id; DELETE FROM NhanVien WHERE Id = @Id OR MaNV = @Code;",
                        new SqlParameter("@Id", staff.Id),
                        new SqlParameter("@Code", staff.Code)
                    );

                    RefreshWeeklySchedule();
                }
            }
        }
    }
}
