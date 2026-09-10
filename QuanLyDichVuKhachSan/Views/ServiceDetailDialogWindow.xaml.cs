using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class ServiceDetailDialogWindow : Window
    {
        public class ServiceDetailRow
        {
            public int Index { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public int Quantity { get; set; } = 1;
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice => Quantity * UnitPrice;
        }

        public ServiceDetailDialogWindow(RoomServiceUsageItem serviceItem, HotelRoom? room = null)
        {
            InitializeComponent();
            LoadFromUsageItem(serviceItem, room);
        }

        public ServiceDetailDialogWindow(FoodOrder order)
        {
            InitializeComponent();
            LoadFoodOrder(order);
        }

        public ServiceDetailDialogWindow(VehicleRental rental)
        {
            InitializeComponent();
            LoadVehicleRental(rental);
        }

        public ServiceDetailDialogWindow(EventBooking booking)
        {
            InitializeComponent();
            LoadEventBooking(booking);
        }

        public ServiceDetailDialogWindow(LaundryOrder order)
        {
            InitializeComponent();
            LoadLaundryOrder(order);
        }

        public ServiceDetailDialogWindow(ParkingRecord record)
        {
            InitializeComponent();
            LoadParkingRecord(record);
        }

        private void SetPaymentBadge(bool isPaid)
        {
            if (isPaid)
            {
                BadgePaymentStatus.Background = (Brush)new BrushConverter().ConvertFrom("#DCFCE7")!;
                BadgePaymentStatus.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#10B981")!;
                TxtPaymentStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#059669")!;
                TxtPaymentStatus.Text = "✓ Đã thanh toán";
            }
            else
            {
                BadgePaymentStatus.Background = (Brush)new BrushConverter().ConvertFrom("#FEF3C7")!;
                BadgePaymentStatus.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#F59E0B")!;
                TxtPaymentStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#D97706")!;
                TxtPaymentStatus.Text = "⏳ Chưa thanh toán (Tính vào tiền phòng)";
            }
        }

        public void LoadFoodOrder(FoodOrder order)
        {
            TxtCategoryTitle.Text = "🍲 CHI TIẾT HÓA ĐƠN ẨM THỰC & MINI-BAR";
            string roomStr = string.IsNullOrWhiteSpace(order.RoomNumber) ? "Khách vãng lai" : $"Phòng P.{DataService.NormalizeRoomNumber(order.RoomNumber)}";
            TxtOrderCodeAndRoom.Text = $"Mã đơn: {order.OrderCode} • {roomStr}";

            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(order.CustomerName) ? order.CustomerName : "Khách hàng";
            TxtBillTime.Text = order.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
            TxtStaffName.Text = !string.IsNullOrWhiteSpace(order.RecordedBy) ? order.RecordedBy : "Lễ tân tiếp nhận";
            TxtPaymentMethod.Text = order.PaymentType == FoodPaymentType.DirectPayment
                ? (!string.IsNullOrWhiteSpace(order.PaymentMethod) ? $"Thanh toán trực tiếp ({order.PaymentMethod})" : "Thanh toán trực tiếp")
                : "Ghi nợ vào phòng";
            TxtNote.Text = !string.IsNullOrWhiteSpace(order.Note) ? order.Note : "Không có ghi chú";
            TxtGrandTotal.Text = $"{order.TotalAmount:N0} đ";

            SetPaymentBadge(order.DaThanhToan);

            var rows = new List<ServiceDetailRow>();
            int idx = 1;
            foreach (var it in order.Items)
            {
                rows.Add(new ServiceDetailRow
                {
                    Index = idx++,
                    ItemName = it.FoodItemName,
                    Quantity = it.Quantity,
                    UnitPrice = it.Price
                });
            }
            GridDetailItems.ItemsSource = rows;
        }

        public void LoadVehicleRental(VehicleRental rental)
        {
            TxtCategoryTitle.Text = "🛵 CHI TIẾT HỢP ĐỒNG THUÊ XE MÁY";
            string roomStr = string.IsNullOrWhiteSpace(rental.RoomNumber) ? "Khách ngoài" : $"Phòng P.{DataService.NormalizeRoomNumber(rental.RoomNumber)}";
            TxtOrderCodeAndRoom.Text = $"Mã thuê: {rental.RentalCode} • {roomStr}";

            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(rental.CustomerName) ? rental.CustomerName : "Khách thuê xe";
            TxtBillTime.Text = rental.RentalDate.ToString("dd/MM/yyyy HH:mm:ss");
            TxtStaffName.Text = !string.IsNullOrWhiteSpace(rental.RecordedBy) ? rental.RecordedBy : "Lễ tân tiếp nhận";
            TxtPaymentMethod.Text = (rental.PaymentStatus == "Ghi nợ vào phòng" || rental.Status == "Ghi nợ vào phòng")
                ? "Ghi nợ vào phòng (Trả khi Check-out)"
                : (rental.DaThanhToan ? "Đã thanh toán" : "Chưa thanh toán");
            TxtNote.Text = $"Xe: {rental.VehicleName} (Biển số: {rental.LicensePlate}) • Cọc: {rental.DepositAmount:N0}đ{(string.IsNullOrWhiteSpace(rental.DamageNote) ? "" : " • Ghi chú: " + rental.DamageNote)}";
            TxtGrandTotal.Text = $"{rental.TotalPayment:N0} đ";

            SetPaymentBadge(rental.DaThanhToan);

            var rows = new List<ServiceDetailRow>();
            int days = Math.Max(1, (int)(rental.ExpectedReturnDate - rental.RentalDate).TotalDays);
            rows.Add(new ServiceDetailRow
            {
                Index = 1,
                ItemName = $"Thuê xe {rental.VehicleName} (Biển số: {rental.LicensePlate})",
                Quantity = days,
                UnitPrice = rental.RentalFee / days
            });

            if (rental.AdditionalCost > 0)
            {
                rows.Add(new ServiceDetailRow
                {
                    Index = 2,
                    ItemName = "Chi phí phát sinh / Phụ thu hư hại",
                    Quantity = 1,
                    UnitPrice = rental.AdditionalCost
                });
            }
            GridDetailItems.ItemsSource = rows;
        }

        public void LoadEventBooking(EventBooking booking)
        {
            TxtCategoryTitle.Text = "🏛️ CHI TIẾT ĐẶT SẢNH & SỰ KIỆN";
            string roomStr = string.IsNullOrWhiteSpace(booking.RoomNumber) ? "Khách ngoài" : $"Phòng P.{DataService.NormalizeRoomNumber(booking.RoomNumber)}";
            TxtOrderCodeAndRoom.Text = $"Mã đặt: {booking.BookingCode} • {roomStr}";

            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(booking.CustomerName) ? booking.CustomerName : "Khách đặt sự kiện";
            TxtBillTime.Text = booking.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
            TxtStaffName.Text = !string.IsNullOrWhiteSpace(booking.RecordedBy) ? booking.RecordedBy : "Lễ tân tiếp nhận";
            TxtPaymentMethod.Text = booking.PaymentStatus == "Ghi nợ vào phòng" 
                ? "Ghi nợ vào phòng (Trả khi Check-out)" 
                : (booking.DaThanhToan ? "Đã thanh toán" : (booking.PaymentStatus ?? "Đã cọc trước"));
            TxtNote.Text = $"Sảnh: {booking.SpaceName} ({booking.StartTime:dd/MM HH:mm} - {booking.EndTime:dd/MM HH:mm}) • Cọc: {booking.DepositAmount:N0}đ{(string.IsNullOrWhiteSpace(booking.DamageNote) ? "" : " • Ghi chú: " + booking.DamageNote)}";
            TxtGrandTotal.Text = $"{booking.FinalTotal:N0} đ";

            SetPaymentBadge(booking.DaThanhToan);

            var rows = new List<ServiceDetailRow>
            {
                new()
                {
                    Index = 1,
                    ItemName = $"Thuê {booking.SpaceName}",
                    Quantity = 1,
                    UnitPrice = booking.TotalEstimatedAmount
                }
            };
            if (booking.AdditionalCost > 0)
            {
                rows.Add(new ServiceDetailRow
                {
                    Index = 2,
                    ItemName = "Chi phí phát sinh dịch vụ sự kiện",
                    Quantity = 1,
                    UnitPrice = booking.AdditionalCost
                });
            }
            GridDetailItems.ItemsSource = rows;
        }

        public void LoadLaundryOrder(LaundryOrder order)
        {
            TxtCategoryTitle.Text = "🧺 CHI TIẾT HÓA ĐƠN GIẶT ỦI";
            string roomStr = string.IsNullOrWhiteSpace(order.RoomNumber) ? "Khách vãng lai" : $"Phòng P.{DataService.NormalizeRoomNumber(order.RoomNumber)}";
            TxtOrderCodeAndRoom.Text = $"Mã đơn: {order.OrderCode} • {roomStr}";

            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(order.CustomerName) ? order.CustomerName : "Khách gửi giặt";
            TxtBillTime.Text = order.ReceivedDate.ToString("dd/MM/yyyy HH:mm:ss");
            TxtStaffName.Text = !string.IsNullOrWhiteSpace(order.RecordedBy) ? order.RecordedBy : "Lễ tân tiếp nhận";
            TxtPaymentMethod.Text = order.DaThanhToan ? "Đã thanh toán" : "Ghi nợ vào phòng";
            TxtNote.Text = $"Đối tác: {order.PartnerName} • Hẹn trả: {order.AppointmentDate:dd/MM/yyyy HH:mm} • Ghi chú: {order.ClothesConditionNote}";
            TxtGrandTotal.Text = $"{order.TotalPrice:N0} đ";

            SetPaymentBadge(order.DaThanhToan);

            var rows = new List<ServiceDetailRow>
            {
                new()
                {
                    Index = 1,
                    ItemName = $"{order.ServiceTypeDisplay} (Đối tác: {order.PartnerName})",
                    Quantity = (int)Math.Ceiling(order.WeightKg),
                    UnitPrice = order.UnitPricePerKg
                }
            };
            GridDetailItems.ItemsSource = rows;
        }

        public void LoadParkingRecord(ParkingRecord record)
        {
            TxtCategoryTitle.Text = "🅿️ CHI TIẾT VÉ GỬI XE BÃI & HẦM";
            string roomStr = string.IsNullOrWhiteSpace(record.RoomNumber) ? "Khách vãng lai" : $"Phòng P.{DataService.NormalizeRoomNumber(record.RoomNumber)}";
            TxtOrderCodeAndRoom.Text = $"Mã vé: {record.TicketCode} • {roomStr}";

            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(record.CustomerName) ? record.CustomerName : "Khách gửi xe";
            TxtBillTime.Text = record.CheckInTime.ToString("dd/MM/yyyy HH:mm:ss");
            TxtStaffName.Text = !string.IsNullOrWhiteSpace(record.RecordedBy) ? record.RecordedBy : "Bảo vệ / Lễ tân";
            TxtPaymentMethod.Text = record.ChargeType == ParkingChargeType.ResidentFree ? "Khách lưu trú (Miễn phí)" : (record.DaThanhToan ? "Đã thanh toán" : "Ghi nợ vào phòng");
            TxtNote.Text = $"Loại xe: {record.VehicleType} (Biển số: {record.LicensePlate}) • Ghi chú: {record.RepairNote}";
            TxtGrandTotal.Text = $"{record.ParkingFee:N0} đ";

            SetPaymentBadge(record.DaThanhToan);

            var rows = new List<ServiceDetailRow>
            {
                new()
                {
                    Index = 1,
                    ItemName = $"Gửi xe {record.VehicleType} ({record.LicensePlate})",
                    Quantity = 1,
                    UnitPrice = record.ParkingFee
                }
            };
            GridDetailItems.ItemsSource = rows;
        }

        private void LoadFromUsageItem(RoomServiceUsageItem item, HotelRoom? room)
        {
            var data = DataService.Instance;
            string roomNum = !string.IsNullOrEmpty(item.RoomNumber) ? item.RoomNumber : (room?.RoomNumber ?? "");

            if (item.ServiceCategory.Contains("Ẩm thực"))
            {
                var order = data.FoodOrders.FirstOrDefault(x => 
                    item.Details.Contains(x.OrderCode) || 
                    (x.TotalAmount == item.Amount && DataService.NormalizeRoomNumber(x.RoomNumber) == DataService.NormalizeRoomNumber(roomNum))
                );
                if (order != null)
                {
                    LoadFoodOrder(order);
                    return;
                }
            }
            else if (item.ServiceCategory.Contains("xe") || item.ServiceCategory.Contains("Thuê xe"))
            {
                var rental = data.VehicleRentals.FirstOrDefault(x => 
                    item.Details.Contains(x.RentalCode) || 
                    DataService.NormalizeRoomNumber(x.RoomNumber) == DataService.NormalizeRoomNumber(roomNum)
                );
                if (rental != null)
                {
                    LoadVehicleRental(rental);
                    return;
                }
            }
            else if (item.ServiceCategory.Contains("kiện") || item.ServiceCategory.Contains("Sự kiện"))
            {
                var booking = data.EventBookings.FirstOrDefault(x => 
                    item.Details.Contains(x.BookingCode) || 
                    DataService.NormalizeRoomNumber(x.RoomNumber) == DataService.NormalizeRoomNumber(roomNum)
                );
                if (booking != null)
                {
                    LoadEventBooking(booking);
                    return;
                }
            }
            else if (item.ServiceCategory.Contains("Giặt") || item.ServiceCategory.Contains("ủi"))
            {
                var laundry = data.LaundryOrders.FirstOrDefault(x => 
                    item.Details.Contains(x.OrderCode) || 
                    DataService.NormalizeRoomNumber(x.RoomNumber) == DataService.NormalizeRoomNumber(roomNum)
                );
                if (laundry != null)
                {
                    LoadLaundryOrder(laundry);
                    return;
                }
            }
            else if (item.ServiceCategory.Contains("Bãi") || item.ServiceCategory.Contains("Đỗ"))
            {
                var park = data.ParkingRecords.FirstOrDefault(x => 
                    item.Details.Contains(x.TicketCode) || 
                    DataService.NormalizeRoomNumber(x.RoomNumber) == DataService.NormalizeRoomNumber(roomNum)
                );
                if (park != null)
                {
                    LoadParkingRecord(park);
                    return;
                }
            }

            // Fallback Generic
            TxtCategoryTitle.Text = $"🛎️ CHI TIẾT DỊCH VỤ: {item.ServiceCategory.ToUpper()}";
            TxtOrderCodeAndRoom.Text = $"{item.ServiceName} • Phòng P.{DataService.NormalizeRoomNumber(roomNum)}";

            TxtCustomerName.Text = !string.IsNullOrEmpty(room?.CustomerName) ? room.CustomerName : "Khách lưu trú";
            TxtBillTime.Text = item.UsedTime.ToString("dd/MM/yyyy HH:mm:ss");
            TxtStaffName.Text = "Lễ tân ca trực";
            TxtPaymentMethod.Text = item.IsPaid ? "Đã thanh toán (Thanh toán trực tiếp)" : "Ghi nợ vào phòng";
            TxtNote.Text = item.Details;
            TxtGrandTotal.Text = $"{item.Amount:N0} đ";

            SetPaymentBadge(item.IsPaid);

            GridDetailItems.ItemsSource = new List<ServiceDetailRow>
            {
                new()
                {
                    Index = 1,
                    ItemName = item.ServiceName,
                    Quantity = 1,
                    UnitPrice = item.Amount
                }
            };
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
