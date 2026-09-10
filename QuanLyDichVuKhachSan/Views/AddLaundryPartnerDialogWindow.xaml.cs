using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class AddLaundryPartnerDialogWindow : Window
    {
        public AddLaundryPartnerDialogWindow()
        {
            InitializeComponent();
            ResetForm();
        }

        private void ResetForm()
        {
            int nextIndex = DataService.Instance.LaundryPartners.Count + 1;
            string autoCode = $"DT{nextIndex:D2}";
            while (DataService.Instance.LaundryPartners.Any(x => x.Name.Contains(autoCode)))
            {
                nextIndex++;
                autoCode = $"DT{nextIndex:D2}";
            }

            TxtCode.Text = autoCode;
            TxtName.Text = $"Tiệm Giặt Sấy {autoCode}";
            TxtAddress.Text = "120 Phan Đình Phùng, Phường 2, TP. Đà Lạt";
            TxtPhoneNumber.Text = $"0909{nextIndex:D3}888";
            TxtCommissionRate.Text = "30";
            TxtCostSay.Text = "21000";
            TxtCostHap.Text = "56000";
            TxtCostUi.Text = "28000";
            UpdateProfitLabels();
            CboStatus.SelectedIndex = 0;
        }

        private void TxtCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProfitLabels();
        }

        private void UpdateProfitLabels()
        {
            if (LblProfitSay == null || LblProfitHap == null || LblProfitUi == null) return;

            if (decimal.TryParse(TxtCostSay?.Text, out var say))
            {
                decimal profit = 30000m - say;
                LblProfitSay.Text = profit > 0 ? $"Lời: {profit:N0}đ/kg" : "⚠️ Lỗ vốn!";
                LblProfitSay.Foreground = profit > 0 ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            }
            if (decimal.TryParse(TxtCostHap?.Text, out var hap))
            {
                decimal profit = 80000m - hap;
                LblProfitHap.Text = profit > 0 ? $"Lời: {profit:N0}đ/kg" : "⚠️ Lỗ vốn!";
                LblProfitHap.Foreground = profit > 0 ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            }
            if (decimal.TryParse(TxtCostUi?.Text, out var ui))
            {
                decimal profit = 40000m - ui;
                LblProfitUi.Text = profit > 0 ? $"Lời: {profit:N0}đ/kg" : "⚠️ Lỗ vốn!";
                LblProfitUi.Foreground = profit > 0 ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtName.Text.Trim();
            string address = TxtAddress.Text.Trim();
            string phone = TxtPhoneNumber.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Vui lòng nhập tên đối tác và số điện thoại liên hệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string commStr = TxtCommissionRate.Text.Replace("%", "").Trim();
            if (!decimal.TryParse(commStr, out var commission) || commission < 0 || commission > 100)
            {
                MessageBox.Show("Tỷ lệ hoa hồng khách sạn hưởng phải từ 0% đến 100%!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra giá vốn từng dịch vụ
            if (!decimal.TryParse(TxtCostSay.Text.Trim(), out var costSay) || costSay <= 0 || costSay >= 30000)
            {
                MessageBox.Show("⚠️ Giá vốn Giặt sấy phải lớn hơn 0 và nhỏ hơn giá niêm yết bán cho khách (30.000 đ/kg) để khách sạn có lãi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtCostHap.Text.Trim(), out var costHap) || costHap <= 0 || costHap >= 80000)
            {
                MessageBox.Show("⚠️ Giá vốn Giặt hấp phải lớn hơn 0 và nhỏ hơn giá niêm yết bán cho khách (80.000 đ/kg) để khách sạn có lãi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtCostUi.Text.Trim(), out var costUi) || costUi <= 0 || costUi >= 40000)
            {
                MessageBox.Show("⚠️ Giá vốn Ủi phẳng phải lớn hơn 0 và nhỏ hơn giá niêm yết bán cho khách (40.000 đ/kg) để khách sạn có lãi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isActive = CboStatus.SelectedIndex == 0;

            int newId = 0;
            try
            {
                string sql = "INSERT INTO DoiTacGiatUi (TenDoiTac, DiaChi, SoDienThoai, TyLeKhachSanHuong, TrangThai) " +
                             "OUTPUT INSERTED.Id " +
                             "VALUES (@Ten, @DiaChi, @Phone, @Commission, @Status);";

                using var conn = new SqlConnection(DatabaseService.Instance.ConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ten", name);
                cmd.Parameters.AddWithValue("@DiaChi", address);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Commission", commission);
                cmd.Parameters.AddWithValue("@Status", isActive ? 1 : 0);

                var res = await cmd.ExecuteScalarAsync();
                if (res != null && int.TryParse(res.ToString(), out var id))
                {
                    newId = id;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SQL Insert Partner Error] {ex.Message}");
            }

            if (newId <= 0)
            {
                newId = DataService.Instance.LaundryPartners.Count > 0 
                    ? DataService.Instance.LaundryPartners.Max(x => x.Id) + 1 
                    : 1;
            }

            // Lưu bảng giá vốn đối tác báo xuống BangGiaDoiTacGiatUi
            await DatabaseService.Instance.SavePartnerCostPricesAsync(newId, costSay, costHap, costUi);

            var newPartner = new LaundryPartner
            {
                Id = newId,
                Name = name,
                Address = address,
                PhoneNumber = phone,
                HotelCommissionRate = commission,
                GiaVonGiatSay = costSay,
                GiaVonGiatHap = costHap,
                GiaVonUiPhang = costUi,
                IsActive = isActive
            };

            DataService.Instance.AddLaundryPartnerToCache(newPartner);

            MessageBox.Show($"Đã thêm mới đối tác giặt ủi [{newPartner.Name}] (Mã: {TxtCode.Text}) kèm bảng giá vốn (Sấy: {costSay:N0}đ, Hấp: {costHap:N0}đ, Ủi: {costUi:N0}đ) thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}
