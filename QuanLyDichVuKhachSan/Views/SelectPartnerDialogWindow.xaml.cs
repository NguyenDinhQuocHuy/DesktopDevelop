using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyDichVuKhachSan.Models;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class SelectPartnerDialogWindow : Window
    {
        public LaundryPartner? SelectedPartner { get; private set; }
        private readonly LaundryOrder _order;

        public SelectPartnerDialogWindow(List<LaundryPartner> activePartners, LaundryOrder order)
        {
            InitializeComponent();
            _order = order;

            TxtOrderCode.Text = order.OrderCode;
            TxtCustomerInfo.Text = $"{order.CustomerName} (P.{order.RoomNumber})";
            TxtServiceType.Text = order.ServiceTypeDisplay;
            TxtWeightAndPrice.Text = $"{order.WeightKg} Kg · {order.TotalPrice:N0} đ";
            TxtAppointment.Text = $"{order.AppointmentDate:HH:mm dd/MM/yyyy}";

            CmbPartner.ItemsSource = activePartners;
            if (activePartners.Count > 0)
            {
                CmbPartner.SelectedIndex = 0;
            }
        }

        private void CmbPartner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbPartner.SelectedItem is LaundryPartner p)
            {
                BorderPartnerDetail.Visibility = Visibility.Visible;
                TxtPartnerName.Text = p.Name;
                TxtPartnerAddress.Text = $"📍 Địa chỉ: {p.Address}";
                TxtPartnerPhone.Text = $"📞 SĐT: {p.PhoneNumber}";

                decimal costPerKg = p.GetCostPriceForService(_order.ServiceType);
                decimal totalCost = Math.Round(_order.WeightKg * costPerKg, 0);
                decimal hotelProfit = _order.TotalPrice - totalCost;
                TxtPartnerCommission.Text = $"💰 Giá vốn đối tác nhận: {costPerKg:N0} đ/kg ({totalCost:N0} đ) • Khách sạn lời: {hotelProfit:N0} đ";
            }
            else
            {
                BorderPartnerDetail.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (CmbPartner.SelectedItem is not LaundryPartner partner)
            {
                MessageBox.Show("Vui lòng chọn một đối tác giặt ủi để bàn giao!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedPartner = partner;
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
