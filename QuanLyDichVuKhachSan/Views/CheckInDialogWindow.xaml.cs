using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyDichVuKhachSan.Models;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class CheckInDialogWindow : Window
    {
        public HotelRoom Room { get; }
        public bool IsConfirmed { get; private set; }

        public CheckInDialogWindow(HotelRoom room)
        {
            InitializeComponent();
            Room = room;
            TxtRoomDetails.Text = $"Phòng: {room.RoomNumber} - {room.RoomType} (Tầng {room.Floor})";

            // 1. Ngày giờ nhận phòng: Ghi nhận thời gian thực tế hiện tại
            DateTime now = DateTime.Now;
            DpCheckInDate.SelectedDate = now.Date;
            TxtCheckInTime.Text = now.ToString("HH:mm");

            // 2. Ngày giờ trả phòng: Mặc định 12:00 trưa ngày mai (Tiêu chuẩn khách sạn)
            DateTime tomorrow = DateTime.Today.AddDays(1);
            DpCheckOutDate.SelectedDate = tomorrow;
            TxtCheckOutTime.Text = "12:00";

            TxtCustomerName.Focus();
        }

        private void DpCheckInDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tự động gợi ý ngày trả phòng sau ngày nhận phòng 1 ngày nếu ngày trả chưa chọn
            if (DpCheckInDate.SelectedDate.HasValue)
            {
                if (!DpCheckOutDate.SelectedDate.HasValue || DpCheckOutDate.SelectedDate.Value <= DpCheckInDate.SelectedDate.Value)
                {
                    DpCheckOutDate.SelectedDate = DpCheckInDate.SelectedDate.Value.AddDays(1);
                }
            }
        }

        private void DpCheckOutDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            PnlError.Visibility = Visibility.Collapsed;

            string customerName = TxtCustomerName.Text.Trim();
            string phone = TxtPhoneNumber.Text.Trim();
            string cccd = TxtIdentityCard.Text.Trim();
            string note = TxtNote.Text.Trim();

            if (string.IsNullOrEmpty(customerName))
            {
                TxtError.Text = "⚠️ Vui lòng nhập Họ & Tên khách hàng!";
                PnlError.Visibility = Visibility.Visible;
                TxtCustomerName.Focus();
                return;
            }

            if (customerName.Any(char.IsDigit))
            {
                TxtError.Text = "⚠️ Họ & Tên khách hàng không được chứa chữ số!";
                PnlError.Visibility = Visibility.Visible;
                TxtCustomerName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(phone))
            {
                TxtError.Text = "⚠️ Vui lòng nhập Số điện thoại liên hệ!";
                PnlError.Visibility = Visibility.Visible;
                TxtPhoneNumber.Focus();
                return;
            }

            if (phone.Any(c => !char.IsDigit(c)))
            {
                TxtError.Text = "⚠️ Số điện thoại chỉ được chứa các chữ số (0-9)!";
                PnlError.Visibility = Visibility.Visible;
                TxtPhoneNumber.Focus();
                return;
            }

            // 1. Xử lý thời gian Check-in (Mặc định: Thời gian thực tế)
            DateTime inDate = DpCheckInDate.SelectedDate ?? DateTime.Today;
            TimeSpan inTime = DateTime.Now.TimeOfDay;
            if (!string.IsNullOrWhiteSpace(TxtCheckInTime.Text) && TimeSpan.TryParse(TxtCheckInTime.Text.Trim(), out var parsedInTime))
            {
                inTime = parsedInTime;
            }
            DateTime checkInFinal = inDate.Date.Add(inTime);

            // 2. Xử lý thời gian Check-out (Mặc định: 12:00 trưa)
            DateTime outDate = DpCheckOutDate.SelectedDate ?? inDate.AddDays(1);
            TimeSpan outTime = new TimeSpan(12, 0, 0); // Mặc định 12:00 trưa
            if (!string.IsNullOrWhiteSpace(TxtCheckOutTime.Text) && TimeSpan.TryParse(TxtCheckOutTime.Text.Trim(), out var parsedOutTime))
            {
                outTime = parsedOutTime;
            }
            DateTime checkOutFinal = outDate.Date.Add(outTime);

            if (checkOutFinal <= checkInFinal)
            {
                TxtError.Text = "⚠️ Ngày giờ trả phòng phải sau thời gian nhận phòng!";
                PnlError.Visibility = Visibility.Visible;
                return;
            }

            // Cập nhật thông tin vào phòng
            Room.CustomerName = customerName;
            Room.PhoneNumber = phone;
            Room.IdentityCard = cccd;
            Room.Note = string.IsNullOrEmpty(note) ? "Khách nhận phòng trực tiếp" : note;
            Room.CheckInDate = checkInFinal;
            Room.ExpectedCheckOutDate = checkOutFinal;
            Room.Status = RoomStatus.Occupied;

            IsConfirmed = true;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
