namespace QuanLyDichVuKhachSan.Models
{
    public enum UserRole
    {
        Admin,         // Chủ khách sạn / Toàn quyền
        Receptionist   // Lễ tân / Giới hạn quyền ghi nhận
    }

    public enum FoodType
    {
        FreshFood,     // Món tươi: Phở, bún... có giới hạn suất ăn trong ngày
        DryFood        // Thực phẩm khô: Mì ly, nước suối, snack, cà phê gói... quản lý tồn kho
    }

    public enum SpaceStatus
    {
        Available,     // Trống / Sẵn sàng
        Rented,        // Đang cho thuê
        Cleaning,      // Đang dọn dẹp
        Maintenance    // Đang sửa chữa / bảo trì
    }

    public enum VehicleType
    {
        Scooter,       // Xe tay ga
        Manual,        // Xe số
        Clutch,        // Xe côn tay
        Car            // Xe ô tô
    }

    public enum VehicleStatus
    {
        Available,     // Sẵn sàng
        Rented,        // Đang cho thuê
        Maintenance    // Bảo trì
    }

    public enum ParkingChargeType
    {
        ByTurn,        // Xe máy vãng lai (5.000 đ/lượt)
        ByHour,        // Ô tô vãng lai (50.000 đ/giờ)
        ByDay,         // Theo ngày
        ResidentFree   // Khách phòng miễn phí
    }

    public enum LaundryServiceType
    {
        WashAndDry,    // Giặt sấy khô thông thường
        DryCleaning,   // Giặt hấp cao cấp (vest, dạ hội)
        Ironing        // Ủi phẳng cao cấp
    }

    public enum LaundryStatus
    {
        PendingDispatch, // Chờ giao tiệm (Chờ giao đối tác)
        Washing,         // Đang giặt
        Washed,          // Giặt xong
        Completed,       // Hoàn tất
        Cancelled        // Đã hủy
    }

    public enum PaymentMethodType
    {
        Cash,           // Tiền mặt (Cash)
        BankTransfer,   // Chuyển khoản (Bank Transfer)
        CreditCard      // Quẹt thẻ (Credit Card)
    }

    public enum BillingType
    {
        DirectPayment,  // Thanh toán trực tiếp (PayNow / Direct Payment) - Thu tiền dứt điểm tại quầy/outlet
        ChargeToRoom    // Ghi nợ vào phòng (RoomCharge / Charge to Room / Post to Folio) - Treo công nợ phòng, trả khi Check-out
    }

    public enum FoodPaymentType
    {
        DirectPayment,                                // Thanh toán trực tiếp (PayNow)
        ChargeToRoom,                                 // Ghi nợ vào phòng (RoomCharge)
        SeparateBill = DirectPayment,                 // Alias tương thích ngược
        AddToRoomBill = ChargeToRoom,                 // Alias tương thích ngược
        RoomBill = ChargeToRoom                       // Alias tương thích ngược
    }
}
