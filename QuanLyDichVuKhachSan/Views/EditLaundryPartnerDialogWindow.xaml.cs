using System;
using System.Windows;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class EditLaundryPartnerDialogWindow : Window
    {
        private readonly LaundryPartner _partner;

        public EditLaundryPartnerDialogWindow(LaundryPartner partner)
        {
            InitializeComponent();
            _partner = partner;
            LoadPartnerData();
        }

        private void LoadPartnerData()
        {
            TxtCode.Text = $"DT{_partner.Id:D2}";
            TxtName.Text = _partner.Name;
            TxtAddress.Text = _partner.Address;
            TxtPhoneNumber.Text = _partner.PhoneNumber;
            TxtCommissionRate.Text = _partner.HotelCommissionRate.ToString("0.##");
            TxtCostSay.Text = _partner.GiaVonGiatSay.ToString("0");
            TxtCostHap.Text = _partner.GiaVonGiatHap.ToString("0");
            TxtCostUi.Text = _partner.GiaVonUiPhang.ToString("0");
            UpdateProfitLabels();
            CboStatus.SelectedIndex = _partner.IsActive ? 0 : 1;
        }

        private void TxtCost_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
            // Hủy bỏ toàn bộ thay đổi, khôi phục lại dữ liệu ban đầu
            LoadPartnerData();
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

            try
            {
                string sql = "UPDATE DoiTacGiatUi SET " +
                             "TenDoiTac = @Ten, " +
                             "DiaChi = @DiaChi, " +
                             "SoDienThoai = @Phone, " +
                             "TyLeKhachSanHuong = @Commission, " +
                             "TrangThai = @Status " +
                             "WHERE Id = @Id;";

                using var conn = new SqlConnection(DatabaseService.Instance.ConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ten", name);
                cmd.Parameters.AddWithValue("@DiaChi", address);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Commission", commission);
                cmd.Parameters.AddWithValue("@Status", isActive ? 1 : 0);
                cmd.Parameters.AddWithValue("@Id", _partner.Id);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SQL Update Partner Error] {ex.Message}");
            }

            // Lưu bảng giá vốn đối tác báo xuống BangGiaDoiTacGiatUi
            await DatabaseService.Instance.SavePartnerCostPricesAsync(_partner.Id, costSay, costHap, costUi);

            // Cập nhật thông tin đối tác trong bộ nhớ
            _partner.Name = name;
            _partner.Address = address;
            _partner.PhoneNumber = phone;
            _partner.HotelCommissionRate = commission;
            _partner.GiaVonGiatSay = costSay;
            _partner.GiaVonGiatHap = costHap;
            _partner.GiaVonUiPhang = costUi;
            _partner.IsActive = isActive;

            MessageBox.Show($"Đã lưu thay đổi thông tin đối tác giặt ủi [{_partner.Name}] kèm bảng giá vốn mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}
