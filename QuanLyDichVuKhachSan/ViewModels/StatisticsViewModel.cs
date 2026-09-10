using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;

        private string _selectedPeriod = "Tất cả thời gian";
        public string SelectedPeriod
        {
            get => _selectedPeriod;
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    RefreshStatistics();
                }
            }
        }

        public ObservableCollection<string> PeriodOptions { get; } = new()
        {
            "Hôm nay",
            "Tháng này",
            "Quý này",
            "Năm nay",
            "Tất cả thời gian"
        };

        // Báo cáo tổng hợp doanh thu
        public ObservableRangeCollection<ServiceRevenueSummary> RevenueSummaries { get; } = new();
        public ObservableCollection<PieSliceItem> PieSlices { get; } = new();
        public ObservableRangeCollection<FoodStatSummary> FoodStats { get; } = new();
        public ObservableRangeCollection<VehicleStatSummary> VehicleStats { get; } = new();
        public ObservableRangeCollection<EventSpaceStatSummary> EventSpaceStats { get; } = new();
        public ObservableRangeCollection<LaundryPartnerStatSummary> LaundryPartnerStats { get; } = new();
        public ObservableRangeCollection<StaffSalaryReportItem> StaffSalaryReports { get; } = new();

        public decimal TotalHotelRevenue => RevenueSummaries.Sum(x => x.TotalRevenue);
        public int TotalTransactions => RevenueSummaries.Sum(x => x.TransactionCount);

        // Các chỉ số Realtime Dashboard Khách sạn
        public int TotalRoomsCount => Data.HotelRooms.Count;
        public int OccupiedRoomsCount => Data.HotelRooms.Count(x => x.Status == RoomStatus.Occupied);
        public int AvailableRoomsCount => Data.HotelRooms.Count(x => x.Status == RoomStatus.Available);
        public int CleaningRoomsCount => Data.HotelRooms.Count(x => x.Status == RoomStatus.Cleaning);
        public int MaintenanceRoomsCount => Data.HotelRooms.Count(x => x.Status == RoomStatus.Maintenance);
        public double OccupancyRate => TotalRoomsCount > 0 ? Math.Round((double)OccupiedRoomsCount / TotalRoomsCount * 100, 1) : 0;

        public int TotalVehiclesCount => Data.Vehicles.Count;
        public int RentedVehiclesCount => Data.Vehicles.Count(v => v.Status == VehicleStatus.Rented);
        public int AvailableVehiclesCount => Data.Vehicles.Count(v => v.Status == VehicleStatus.Available);

        public int ActiveEventsTodayCount => Data.EventBookings.Count(e => e.StartTime.Date == DateTime.Today);
        public int TotalEventSpacesCount => Data.EventSpaces.Count;
        public int ActiveLaundryOrdersCount => Data.LaundryOrders.Count(l => l.Status == LaundryStatus.Washing);
        public int TotalLaundryPartnersCount => Data.LaundryPartners.Count;
        public string ActiveDutyStaffName => Data.GetCurrentDutyStaffName();

        public string PeriodRangeDisplay
        {
            get
            {
                var (start, end) = GetDateRange();
                if (SelectedPeriod == "Tất cả thời gian") return "Toàn bộ lịch sử hoạt động";
                if (SelectedPeriod == "Hôm nay") return $"{start:dd/MM/yyyy}";
                return $"{start:dd/MM/yyyy} - {end:dd/MM/yyyy}";
            }
        }

        public ICommand RefreshCommand { get; }

        public StatisticsViewModel()
        {
            RefreshCommand = new RelayCommand(RefreshStatistics);
            RefreshStatistics();
        }

        private (DateTime Start, DateTime End) GetDateRange()
        {
            DateTime now = DateTime.Now;
            return SelectedPeriod switch
            {
                "Hôm nay" => (now.Date, now.Date.AddDays(1).AddTicks(-1)),
                "Tháng này" => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1)),
                "Quý này" => (new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1), new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1).AddMonths(3).AddTicks(-1)),
                "Năm nay" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31, 23, 59, 59)),
                _ => (DateTime.MinValue, DateTime.MaxValue)
            };
        }

        public void RefreshStatistics()
        {
            var (startDate, endDate) = GetDateRange();
            bool isAllTime = SelectedPeriod == "Tất cả thời gian";

            string[] defaultColors = new[] { "#0284C7", "#7C3AED", "#F59E0B", "#10B981", "#EC4899" };
            int colorIdx = 0;
            var summaries = isAllTime 
                ? Data.GetServiceRevenueSummaries() 
                : Data.GetServiceRevenueSummaries(startDate, endDate);

            foreach (var item in summaries)
            {
                item.ColorHex = defaultColors[colorIdx % defaultColors.Length];
                colorIdx++;
            }
            RevenueSummaries.ReplaceRange(summaries);

            BuildPieChartSlices();

            FoodStats.ReplaceRange(Data.GetFoodStats());
            VehicleStats.ReplaceRange(Data.GetVehicleStats());
            EventSpaceStats.ReplaceRange(Data.GetEventSpaceStats());
            LaundryPartnerStats.ReplaceRange(Data.GetLaundryPartnerStats());

            var salaryList = new List<StaffSalaryReportItem>();
            foreach (var staff in Data.StaffList)
            {
                int shifts = Data.ShiftAssignments.Count(a => a.StaffId == staff.Id && a.ShiftDate >= startDate && a.ShiftDate <= endDate);
                salaryList.Add(new StaffSalaryReportItem
                {
                    StaffCode = staff.Code,
                    StaffName = staff.FullName,
                    Position = staff.Position,
                    BaseSalary = staff.BaseSalary,
                    TotalShifts = shifts,
                    Status = staff.Status
                });
            }
            StaffSalaryReports.ReplaceRange(salaryList);

            OnPropertyChanged(nameof(TotalHotelRevenue));
            OnPropertyChanged(nameof(TotalTransactions));
            OnPropertyChanged(nameof(TotalRoomsCount));
            OnPropertyChanged(nameof(OccupiedRoomsCount));
            OnPropertyChanged(nameof(AvailableRoomsCount));
            OnPropertyChanged(nameof(CleaningRoomsCount));
            OnPropertyChanged(nameof(MaintenanceRoomsCount));
            OnPropertyChanged(nameof(OccupancyRate));
            OnPropertyChanged(nameof(TotalVehiclesCount));
            OnPropertyChanged(nameof(RentedVehiclesCount));
            OnPropertyChanged(nameof(AvailableVehiclesCount));
            OnPropertyChanged(nameof(ActiveEventsTodayCount));
            OnPropertyChanged(nameof(ActiveLaundryOrdersCount));
            OnPropertyChanged(nameof(ActiveDutyStaffName));
            OnPropertyChanged(nameof(PeriodRangeDisplay));
        }

        /// <summary>
        /// Thuật toán tạo hình học các lát cắt biểu đồ tròn (Pie Chart Geometry Algorithm)
        /// </summary>
        private void BuildPieChartSlices()
        {
            PieSlices.Clear();
            decimal total = RevenueSummaries.Sum(x => x.TotalRevenue);

            double centerX = 105;
            double centerY = 105;
            double radius = 95;
            var validItems = RevenueSummaries.Where(x => x.TotalRevenue > 0).ToList();

            if (total <= 0 || validItems.Count == 0)
            {
                var emptyGeom = new EllipseGeometry(new System.Windows.Point(centerX, centerY), radius, radius);

                PieSlices.Add(new PieSliceItem
                {
                    ServiceName = "Chưa có doanh thu trong kỳ",
                    TotalRevenue = 0,
                    Percentage = 0,
                    ColorHex = "#F1F5F9", // Màu xám trắng biểu thị không có doanh thu
                    SliceGeometry = emptyGeom
                });
                return;
            }

            if (validItems.Count == 1)
            {
                var single = validItems[0];
                var fullPie = new EllipseGeometry(new System.Windows.Point(centerX, centerY), radius, radius);

                PieSlices.Add(new PieSliceItem
                {
                    ServiceName = single.ServiceName,
                    TotalRevenue = single.TotalRevenue,
                    Percentage = 100,
                    ColorHex = single.ColorHex,
                    SliceGeometry = fullPie
                });
                return;
            }

            double currentAngle = -90.0; // Bắt đầu từ góc 12 giờ đỉnh trên

            foreach (var item in validItems)
            {
                double sweepAngle = (double)(item.TotalRevenue / total) * 360.0;
                if (sweepAngle < 0.1) continue;

                double startRad = currentAngle * Math.PI / 180.0;
                double endRad = (currentAngle + sweepAngle) * Math.PI / 180.0;

                System.Windows.Point pOuterStart = new(
                    centerX + radius * Math.Cos(startRad),
                    centerY + radius * Math.Sin(startRad));

                System.Windows.Point pOuterEnd = new(
                    centerX + radius * Math.Cos(endRad),
                    centerY + radius * Math.Sin(endRad));

                var figure = new System.Windows.Media.PathFigure
                {
                    StartPoint = new System.Windows.Point(centerX, centerY),
                    IsClosed = true,
                    IsFilled = true
                };

                figure.Segments.Add(new System.Windows.Media.LineSegment(pOuterStart, true));
                figure.Segments.Add(new System.Windows.Media.ArcSegment(
                    pOuterEnd,
                    new System.Windows.Size(radius, radius),
                    0,
                    sweepAngle > 180.0,
                    System.Windows.Media.SweepDirection.Clockwise,
                    true));
                figure.Segments.Add(new System.Windows.Media.LineSegment(new System.Windows.Point(centerX, centerY), true));

                var pathGeom = new System.Windows.Media.PathGeometry();
                pathGeom.Figures.Add(figure);

                PieSlices.Add(new PieSliceItem
                {
                    ServiceName = item.ServiceName,
                    TotalRevenue = item.TotalRevenue,
                    Percentage = Math.Round((double)(item.TotalRevenue / total) * 100, 1),
                    ColorHex = item.ColorHex,
                    SliceGeometry = pathGeom
                });

                currentAngle += sweepAngle;
            }
        }
    }
}
