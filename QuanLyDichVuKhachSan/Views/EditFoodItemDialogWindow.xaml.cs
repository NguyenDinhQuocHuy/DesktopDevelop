using System;
using System.Windows;
using System.Windows.Controls;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class EditFoodItemDialogWindow : Window
    {
        private readonly FoodItem _foodItem;

        public EditFoodItemDialogWindow(FoodItem foodItem)
        {
            InitializeComponent();
            _foodItem = foodItem ?? throw new ArgumentNullException(nameof(foodItem));

            TxtName.Text = _foodItem.Name;
            TxtRetailUnit.Text = _foodItem.RetailUnit;
            TxtPrice.Text = _foodItem.Price.ToString("N0");
            TxtDefaultImportUnit.Text = string.IsNullOrWhiteSpace(_foodItem.DefaultImportUnit) ? "Thùng" : _foodItem.DefaultImportUnit;
            TxtDefaultConversionRate.Text = (_foodItem.DefaultConversionRate > 0 ? _foodItem.DefaultConversionRate : 24).ToString();

            // Set Category
            if (_foodItem.Category == "Thức uống" || _foodItem.IsDrink)
            {
                CmbCategory.SelectedIndex = 1;
            }
            else
            {
                CmbCategory.SelectedIndex = 0;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên mặt hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtRetailUnit.Text))
            {
                MessageBox.Show("Vui lòng nhập đơn vị bán lẻ (Lon, Chai, Gói...)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string defaultImportUnit = string.IsNullOrWhiteSpace(TxtDefaultImportUnit.Text) ? "Thùng" : TxtDefaultImportUnit.Text.Trim();

            if (!int.TryParse(TxtDefaultConversionRate.Text.Trim(), out int conversionRate) || conversionRate <= 0)
            {
                MessageBox.Show("Hệ số quy đổi lẻ phải là số nguyên lớn hơn 0 (Ví dụ: 24, 12, 6)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string cleanPrice = (TxtPrice.Text ?? "").Replace(",", "").Replace(".", "").Replace(" ", "").Replace("đ", "");
            if (!decimal.TryParse(cleanPrice, out decimal price) || price < 0)
            {
                MessageBox.Show("Đơn giá bán lẻ không hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string category = (CmbCategory.SelectedIndex == 1) ? "Thức uống" : "Đồ ăn";
            string retailUnit = TxtRetailUnit.Text.Trim();

            _foodItem.Name = TxtName.Text.Trim();
            _foodItem.Category = category;
            _foodItem.RetailUnit = retailUnit;
            _foodItem.Price = price;
            _foodItem.DefaultImportUnit = defaultImportUnit;
            _foodItem.DefaultConversionRate = conversionRate;

            // Đồng bộ xuống CSDL SQL Server (Table MonAn)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE MonAn SET TenMon = @Ten, DanhMuc = @Loai, DonViTinh = @DVT, GiaBan = @Gia, DonViNhapMacDinh = @DonViNhap, HeSoQuyDoiMacDinh = @HeSoQuyDoi WHERE Id = @Id",
                new Microsoft.Data.SqlClient.SqlParameter("@Ten", _foodItem.Name),
                new Microsoft.Data.SqlClient.SqlParameter("@Loai", _foodItem.Category),
                new Microsoft.Data.SqlClient.SqlParameter("@DVT", _foodItem.RetailUnit),
                new Microsoft.Data.SqlClient.SqlParameter("@Gia", _foodItem.Price),
                new Microsoft.Data.SqlClient.SqlParameter("@DonViNhap", _foodItem.DefaultImportUnit),
                new Microsoft.Data.SqlClient.SqlParameter("@HeSoQuyDoi", _foodItem.DefaultConversionRate),
                new Microsoft.Data.SqlClient.SqlParameter("@Id", _foodItem.Id)
            );

            _foodItem.NotifyStockChanged();

            DialogResult = true;
            Close();
        }
    }
}
