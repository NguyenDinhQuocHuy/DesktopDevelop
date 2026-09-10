using System;
using System.IO;
using System.Text;
using QuanLyDichVuKhachSan.Models;

namespace QuanLyDichVuKhachSan.Services
{
    public static class ExportService
    {
        public static string GenerateFoodBill(FoodOrder order)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=================================================");
            sb.AppendLine("         KHÁCH SẠN NGHỈ DƯỠNG DALAT HOTEL");
            sb.AppendLine("      HÓA ĐƠN DỊCH VỤ ẨM THỰC & MINI-BAR");
            sb.AppendLine("=================================================");
            sb.AppendLine($"Mã đơn hàng : {order.OrderCode}");
            sb.AppendLine($"Thời gian   : {order.CreatedAt:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Khách hàng  : {order.CustomerName} (Phòng: {order.RoomNumber})");
            sb.AppendLine($"Lễ tân lập  : {order.RecordedBy}");
            sb.AppendLine($"Hình thức   : {order.PaymentTypeDisplay}");
            sb.AppendLine("-------------------------------------------------");
            sb.AppendLine(string.Format("{0,-25} {1,5} {2,15}", "Tên món/sản phẩm", "SL", "Thành tiền"));
            sb.AppendLine("-------------------------------------------------");
            foreach (var item in order.Items)
            {
                sb.AppendLine(string.Format("{0,-25} {1,5} {2,15:N0} đ", item.FoodItemName.Length > 24 ? item.FoodItemName[..24] : item.FoodItemName, item.Quantity, item.TotalPrice));
            }
            sb.AppendLine("-------------------------------------------------");
            sb.AppendLine($"TỔNG CỘNG: {order.TotalAmount:N0} VNĐ");
            sb.AppendLine($"Ghi chú: {order.Note}");
            sb.AppendLine("=================================================");
            sb.AppendLine("       CẢM ƠN QUÝ KHÁCH & HẸN GẶP LẠI!");
            return sb.ToString();
        }

        public static string GenerateLaundryReceipt(LaundryOrder order)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=================================================");
            sb.AppendLine("         KHÁCH SẠN NGHỈ DƯỠNG DALAT HOTEL");
            sb.AppendLine("       PHIẾU TIẾP NHẬN & HẸN LẤY GIẶT ỦI");
            sb.AppendLine("=================================================");
            sb.AppendLine($"Mã phiếu     : {order.OrderCode}");
            sb.AppendLine($"Ngày tiếp nhận: {order.ReceivedDate:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Khách hàng   : {order.CustomerName} - SĐT: {order.PhoneNumber}");
            sb.AppendLine($"Số phòng     : {order.RoomNumber}");
            sb.AppendLine($"Đối tác giặt : {order.PartnerName}");
            sb.AppendLine($"Loại dịch vụ : {order.ServiceTypeDisplay}");
            sb.AppendLine($"Khối lượng   : {order.WeightKg} Kg x {order.UnitPricePerKg:N0} đ/Kg");
            sb.AppendLine($"Tổng tiền    : {order.TotalPrice:N0} VNĐ ({(order.IsPaid ? "Đã thanh toán" : "Chưa thanh toán")})");
            sb.AppendLine($"Tình trạng đồ: {order.ClothesConditionNote}");
            sb.AppendLine("-------------------------------------------------");
            sb.AppendLine($"HẸN TRẢ ĐỒ   : {order.AppointmentDate:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Lễ tân nhận  : {order.RecordedBy}");
            sb.AppendLine("=================================================");
            sb.AppendLine(" (Vui lòng mang theo phiếu này khi nhận đồ)");
            return sb.ToString();
        }

        public static string GenerateBikeRentalContract(VehicleRental rental)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=================================================");
            sb.AppendLine("         KHÁCH SẠN NGHỈ DƯỠNG DALAT HOTEL");
            sb.AppendLine("           HỢP ĐỒNG CHO THUÊ XE MÁY");
            sb.AppendLine("=================================================");
            sb.AppendLine($"Mã hợp đồng  : {rental.RentalCode}");
            sb.AppendLine($"Khách thuê   : {rental.CustomerName}");
            sb.AppendLine($"Số CCCD      : {rental.IdentityCard} - SĐT: {rental.PhoneNumber}");
            sb.AppendLine($"Số phòng     : {rental.RoomNumber}");
            sb.AppendLine($"Xe thuê      : {rental.VehicleName} (Biển số: {rental.LicensePlate})");
            sb.AppendLine($"Thời gian    : Từ {rental.RentalDate:dd/MM/yyyy} Đến {rental.ExpectedReturnDate:dd/MM/yyyy}");
            sb.AppendLine($"Tiền cọc     : {rental.DepositAmount:N0} VNĐ");
            sb.AppendLine($"Tiền thuê    : {rental.RentalFee:N0} VNĐ");
            sb.AppendLine($"Chi phí phát sinh: {rental.AdditionalCost:N0} VNĐ");
            sb.AppendLine($"Tổng thanh toán  : {rental.TotalPayment:N0} VNĐ");
            sb.AppendLine($"Trạng thái   : {rental.Status}");
            sb.AppendLine("-------------------------------------------------");
            sb.AppendLine("Lưu ý: Quý khách vui lòng tuân thủ luật GTĐB &");
            sb.AppendLine("bảo quản xe cẩn thận trong suốt thời gian thuê.");
            sb.AppendLine("=================================================");
            return sb.ToString();
        }

        public static string GenerateRoomInvoiceText(RoomInvoice invoice)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=================================================");
            sb.AppendLine("         KHÁCH SẠN NGHỈ DƯỠNG DALAT HOTEL");
            sb.AppendLine("         HÓA ĐƠN THANH TOÁN QUYẾT TOÁN");
            sb.AppendLine("=================================================");
            sb.AppendLine($"Mã hóa đơn   : {invoice.InvoiceCode}");
            sb.AppendLine($"Thời gian    : {invoice.InvoiceDate:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Khách hàng   : {invoice.CustomerName}");
            sb.AppendLine($"Số phòng     : {invoice.RoomNumber}");
            sb.AppendLine($"SĐT / CCCD   : {invoice.PhoneNumber} / {invoice.IdentityCard}");
            sb.AppendLine($"Thu ngân     : {invoice.IssuedBy}");
            sb.AppendLine("-------------------------------------------------");
            sb.AppendLine($"Tiền phòng   : {invoice.StayNights} đêm x {invoice.PricePerNight:N0} đ = {invoice.RoomCost:N0} VNĐ");
            sb.AppendLine($"Tiền dịch vụ : {invoice.ServicesCost:N0} VNĐ");
            sb.AppendLine("-------------------------------------------------");
            sb.AppendLine($"TỔNG CỘNG THANH TOÁN: {invoice.TotalAmount:N0} VNĐ");
            sb.AppendLine("=================================================");
            sb.AppendLine("       CẢM ƠN QUÝ KHÁCH & HẸN GẶP LẠI!");
            return sb.ToString();
        }
    }
}
