using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace QuanLyDichVuKhachSan.Models
{
    public class ServiceRevenueSummary
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public int TransactionCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public double Percentage { get; set; }
        public string ColorHex { get; set; } = "#3B82F6";
        public Brush ColorBrush => new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorHex));
    }

    public class PieSliceItem
    {
        public string ServiceName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public double Percentage { get; set; }
        public string ColorHex { get; set; } = "#3B82F6";
        public Brush SliceBrush => new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorHex));
        public Geometry SliceGeometry { get; set; } = Geometry.Empty;
        public string TooltipText => TotalRevenue > 0 
            ? $"{ServiceName}\nDoanh thu: {TotalRevenue:N0} đ ({Percentage:0.#}%)" 
            : "Chưa có doanh thu trong kỳ";
        public string LegendDisplay => $"{ServiceName}: {Percentage:0.#}% ({TotalRevenue:N0} đ)";
    }

    public class FoodStatSummary
    {
        public string FoodName { get; set; } = string.Empty;
        public string FoodType { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int StockLeft { get; set; }
    }

    public class VehicleStatSummary
    {
        public string LicensePlate { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public int RentalCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class EventSpaceStatSummary
    {
        public string SpaceName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public string DamageReport { get; set; } = string.Empty;
    }

    public class LaundryPartnerStatSummary
    {
        public string PartnerName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalWeightKg { get; set; }
        public decimal TotalOrderValue { get; set; }
        public decimal HotelCommission { get; set; }
        public decimal PartnerPayout { get; set; }
    }

    public class PeriodRevenueSummary
    {
        public string PeriodLabel { get; set; } = string.Empty;
        public decimal FoodRevenue { get; set; }
        public decimal EventRevenue { get; set; }
        public decimal VehicleRevenue { get; set; }
        public decimal ParkingRevenue { get; set; }
        public decimal LaundryRevenue { get; set; }
        public decimal TotalRevenue => FoodRevenue + EventRevenue + VehicleRevenue + ParkingRevenue + LaundryRevenue;
    }
}
