using System;
using System.Windows;
using System.Windows.Controls;
using QuanLyDichVuKhachSan.Models;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class ExtendStayDialogWindow : Window
    {
        public HotelRoom Room { get; }
        public DateTime SelectedNewCheckOut { get; private set; }
        public bool IsConfirmed { get; private set; }

        public ExtendStayDialogWindow(HotelRoom room)
        {
            InitializeComponent();
            Room = room;

            TxtRoomInfo.Text = $"Phòng {room.RoomNumber} ({room.RoomType}) - Khách: {room.CustomerName}";
            DateTime currentCheckOut = room.ExpectedCheckOutDate ?? DateTime.Now.AddDays(1);
            TxtCurrentCheckOut.Text = currentCheckOut.ToString("HH:mm - dd/MM/yyyy");

            for (int h = 0; h < 24; h++)
            {
                CmbNewCheckOutHour.Items.Add($"{h:D2}:00");
                CmbNewCheckOutHour.Items.Add($"{h:D2}:30");
            }

            // Mặc định chọn ngày mai và 12:00
            DateTime defaultNext = currentCheckOut > DateTime.Now ? currentCheckOut.AddDays(1) : DateTime.Today.AddDays(1);
            DpNewCheckOutDate.SelectedDate = defaultNext.Date;
            CmbNewCheckOutHour.SelectedItem = "12:00";
        }

        private void BtnPlus1Day_Click(object sender, RoutedEventArgs e) => AddDaysToDate(1);
        private void BtnPlus2Days_Click(object sender, RoutedEventArgs e) => AddDaysToDate(2);
        private void BtnPlus3Days_Click(object sender, RoutedEventArgs e) => AddDaysToDate(3);
        private void BtnPlus1Week_Click(object sender, RoutedEventArgs e) => AddDaysToDate(7);

        private void AddDaysToDate(int days)
        {
            DateTime baseDate = DpNewCheckOutDate.SelectedDate ?? DateTime.Today;
            DpNewCheckOutDate.SelectedDate = baseDate.AddDays(days);
        }

        private void DpNewCheckOutDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!DpNewCheckOutDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày trả phòng mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime date = DpNewCheckOutDate.SelectedDate.Value.Date;
            TimeSpan time = TimeSpan.FromHours(12);

            if (CmbNewCheckOutHour.SelectedItem is string timeStr && TimeSpan.TryParse(timeStr, out var parsed))
            {
                time = parsed;
            }

            DateTime newCheckOut = date.Add(time);

            if (Room.CheckInDate.HasValue && newCheckOut <= Room.CheckInDate.Value)
            {
                MessageBox.Show("Ngày giờ trả phòng mới phải sau thời gian nhận phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedNewCheckOut = newCheckOut;
            IsConfirmed = true;
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
