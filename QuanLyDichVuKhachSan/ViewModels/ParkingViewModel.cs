using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class ParkingViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;
        public AuthService Auth => AuthService.Instance;

        public bool IsAdmin => Auth.IsAdmin;
        public bool IsReceptionist => Auth.IsReceptionist;

        // Tìm kiếm khách hàng theo tên / số điện thoại real-time (Debounced 180ms)
        private string _searchParkingQuery = "";
        public string SearchParkingQuery
        {
            get => _searchParkingQuery;
            set
            {
                if (SetProperty(ref _searchParkingQuery, value))
                {
                    Debounce("ParkingSearch", ApplyParkingFilter, 180);
                }
            }
        }

        public ObservableRangeCollection<ParkingRecord> FilteredParkingRecords { get; } = new();

        public void ApplyParkingFilter()
        {
            var list = Data.ParkingRecords.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchParkingQuery))
            {
                string rawQ = SearchParkingQuery.Trim();
                string cleanQ = TextSearchHelper.ToSearchKey(rawQ);
                list = list.Where(p =>
                    (!string.IsNullOrWhiteSpace(p.PhoneNumber) && p.PhoneNumber.Contains(rawQ)) ||
                    TextSearchHelper.MatchPrepared(p.CustomerName, cleanQ) ||
                    (!string.IsNullOrWhiteSpace(p.LicensePlate) && p.LicensePlate.IndexOf(rawQ, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrWhiteSpace(p.TicketCode) && p.TicketCode.IndexOf(rawQ, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrWhiteSpace(p.RoomNumber) && p.RoomNumber.IndexOf(rawQ, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }
            FilteredParkingRecords.ReplaceRange(list.ToList());
        }

        // Ghi nhận xe vào bãi (Lễ tân)
        private string _licensePlate = "";
        public string LicensePlate { get => _licensePlate; set => SetProperty(ref _licensePlate, value); }

        private string _vehicleType = "Xe máy";
        public string VehicleType
        {
            get => _vehicleType;
            set
            {
                string norm = value == "Ô tô" ? "Ô tô" : "Xe máy";
                if (SetProperty(ref _vehicleType, norm))
                {
                    if (string.IsNullOrWhiteSpace(RoomNumber))
                    {
                        ChargeType = norm == "Ô tô" ? ParkingChargeType.ByHour : ParkingChargeType.ByTurn;
                    }
                    AutoSetFee();
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
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        ChargeType = ParkingChargeType.ResidentFree;
                    }
                    else
                    {
                        ChargeType = VehicleType.Contains("Ô tô") ? ParkingChargeType.ByHour : ParkingChargeType.ByTurn;
                    }
                    AutoSetFee();
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

        private ParkingChargeType _chargeType = ParkingChargeType.ByTurn;
        public ParkingChargeType ChargeType
        {
            get => _chargeType;
            set
            {
                if (SetProperty(ref _chargeType, value))
                {
                    AutoSetFee();
                }
            }
        }

        private decimal _parkingFee = 5000;
        public decimal ParkingFee { get => _parkingFee; set => SetProperty(ref _parkingFee, value); }

        private string _repairNote = "";
        public string RepairNote { get => _repairNote; set => SetProperty(ref _repairNote, value); }

        // Ngày giờ vào thực tế
        private string _checkInTimeStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        public string CheckInTimeStr { get => _checkInTimeStr; set => SetProperty(ref _checkInTimeStr, value); }

        // Quản lý Quyết toán xe ra
        private ParkingRecord? _selectedRecord;
        public ParkingRecord? SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                if (SetProperty(ref _selectedRecord, value))
                {
                    CalculateReturnFee();
                    OnPropertyChanged(nameof(HasSelectedRecord));
                }
            }
        }
        public bool HasSelectedRecord => SelectedRecord != null;

        private DateTime _actualOutTime = DateTime.Now;
        public DateTime ActualOutTime
        {
            get => _actualOutTime;
            set
            {
                if (SetProperty(ref _actualOutTime, value))
                {
                    CalculateReturnFee();
                }
            }
        }

        private double _calculatedHours = 1;
        public double CalculatedHours { get => _calculatedHours; set => SetProperty(ref _calculatedHours, value); }

        private string _calculatedDurationDisplay = "";
        public string CalculatedDurationDisplay { get => _calculatedDurationDisplay; set => SetProperty(ref _calculatedDurationDisplay, value); }

        private decimal _calculatedAmount = 0;
        public decimal CalculatedAmount { get => _calculatedAmount; set => SetProperty(ref _calculatedAmount, value); }

        public ICommand CheckInCommand { get; }
        public ICommand PayAndCheckOutCommand { get; }

        public ParkingViewModel()
        {
            CheckInCommand = new RelayCommand(CheckIn);
            PayAndCheckOutCommand = new RelayCommand(PayAndCheckOut);
            AutoSetFee();
            ApplyParkingFilter();
            Data.ParkingRecords.CollectionChanged += (s, e) => ApplyParkingFilter();
        }

        private void AutoSetFee()
        {
            if (ChargeType == ParkingChargeType.ResidentFree)
            {
                ParkingFee = 0;
            }
            else if (ChargeType == ParkingChargeType.ByTurn)
            {
                ParkingFee = 5000; // Xe máy 5k/lượt
            }
            else if (ChargeType == ParkingChargeType.ByHour)
            {
                ParkingFee = 50000; // Ô tô 50k/giờ
            }
        }

        private void CalculateReturnFee()
        {
            if (SelectedRecord == null)
            {
                CalculatedHours = 0;
                CalculatedAmount = 0;
                CalculatedDurationDisplay = "";
                return;
            }

            var span = ActualOutTime > SelectedRecord.CheckInTime ? (ActualOutTime - SelectedRecord.CheckInTime) : TimeSpan.Zero;
            int totalHours = (int)span.TotalHours;
            int mins = span.Minutes;
            if (totalHours == 0 && mins == 0) CalculatedDurationDisplay = "Dưới 1 phút";
            else if (totalHours == 0) CalculatedDurationDisplay = $"{mins} phút";
            else if (mins == 0) CalculatedDurationDisplay = $"{totalHours} giờ";
            else CalculatedDurationDisplay = $"{totalHours} giờ {mins} phút";

            if (SelectedRecord.ChargeType == ParkingChargeType.ResidentFree || !string.IsNullOrWhiteSpace(SelectedRecord.RoomNumber))
            {
                CalculatedHours = Math.Max(1, Math.Ceiling(span.TotalMinutes / 60.0));
                CalculatedAmount = 0;
            }
            else if (SelectedRecord.VehicleType == "Xe máy" || SelectedRecord.ChargeType == ParkingChargeType.ByTurn)
            {
                CalculatedHours = 1;
                CalculatedAmount = 5000;
            }
            else
            {
                // Ô tô tính theo tiếng (50k/giờ)
                double hours = Math.Max(1.0, Math.Ceiling(span.TotalMinutes / 60.0));
                CalculatedHours = hours;
                CalculatedAmount = (decimal)hours * 50000;
            }
        }

        private void CheckIn(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(LicensePlate))
            {
                MessageBox.Show("Vui lòng nhập biển số xe!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            var record = new ParkingRecord
            {
                LicensePlate = LicensePlate.Trim(),
                VehicleType = VehicleType,
                CustomerName = CustomerName?.Trim() ?? "Khách vãng lai",
                PhoneNumber = PhoneNumber?.Trim() ?? "",
                RoomNumber = RoomNumber?.Trim() ?? "",
                CheckInTime = DateTime.Now,
                ChargeType = ChargeType,
                ParkingFee = ParkingFee,
                IsPaid = ChargeType == ParkingChargeType.ResidentFree || ChargeType == ParkingChargeType.ByTurn,
                RepairNote = RepairNote?.Trim() ?? "",
                RecordedBy = Auth.IsAdmin ? "Chủ khách sạn" : Data.GetCurrentDutyStaffName(DateTime.Now)
            };

            // Chỉ xuất bill thu tiền nếu có phí phát sinh (> 0đ)
            if (record.ParkingFee > 0)
            {
                var win = new Views.InvoiceBillWindow(record);
                bool? res = win.ShowDialog();
                if (res != true || !win.IsConfirmed)
                {
                    // Hủy bỏ hoặc bấm X: Không lưu xuống CSDL
                    return;
                }
            }
            else
            {
                MessageBox.Show($"Đã ghi nhận xe phòng [{record.RoomNumber}] (BS: {record.LicensePlate}) vào bãi đỗ xe thành công (Miễn phí)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Data.AddParkingRecord(record);
            LicensePlate = "";
            CustomerName = "";
            PhoneNumber = "";
            RoomNumber = "";
            RepairNote = "";
            CheckInTimeStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        private void PayAndCheckOut(object? parameter)
        {
            if (SelectedRecord == null)
            {
                MessageBox.Show("Vui lòng click chọn xe cần quyết toán trong bảng danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var record = SelectedRecord;
            DateTime outTime = ActualOutTime;
            decimal fee = CalculatedAmount;

            var win = new Views.InvoiceBillWindow(record);
            bool? res = win.ShowDialog();
            if (res == true && win.IsConfirmed)
            {
                record.CheckOutTime = outTime;
                record.ParkingFee = fee;
                record.DaThanhToan = true;
                record.NguoiThanhToanId = Auth.CurrentUser?.Id ?? 1;
                record.ThoiGianThanhToan = DateTime.Now;

                // Đồng bộ xuống CSDL SQL Server (Table BaiDoXe)
                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    "UPDATE BaiDoXe SET ThoiGianRa = @Out, PhiGui = @Phi, DaThanhToan = 1, NguoiThanhToanId = @NguoiTT, ThoiGianThanhToan = @TgTT WHERE Id = @Id OR MaVe = @Ma",
                    new Microsoft.Data.SqlClient.SqlParameter("@Out", record.CheckOutTime),
                    new Microsoft.Data.SqlClient.SqlParameter("@Phi", record.ParkingFee),
                    new Microsoft.Data.SqlClient.SqlParameter("@NguoiTT", record.NguoiThanhToanId),
                    new Microsoft.Data.SqlClient.SqlParameter("@TgTT", record.ThoiGianThanhToan),
                    new Microsoft.Data.SqlClient.SqlParameter("@Id", record.Id),
                    new Microsoft.Data.SqlClient.SqlParameter("@Ma", record.TicketCode)
                );

                SelectedRecord = null;
            }
        }
    }
}
