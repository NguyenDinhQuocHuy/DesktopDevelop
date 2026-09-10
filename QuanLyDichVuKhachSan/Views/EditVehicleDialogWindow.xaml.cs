using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class EditVehicleDialogWindow : Window
    {
        private readonly Vehicle _vehicle;

        public EditVehicleDialogWindow(Vehicle vehicle)
        {
            InitializeComponent();
            _vehicle = vehicle;
            LoadVehicleData();
        }

        private void LoadVehicleData()
        {
            TxtName.Text = _vehicle.Name;
            TxtLicensePlate.Text = _vehicle.LicensePlate;
            TxtDailyRate.Text = _vehicle.DailyRate.ToString("N0");
            TxtConditionNote.Text = _vehicle.ConditionNote;

            foreach (ComboBoxItem item in CboType.Items)
            {
                if (item.Tag is VehicleType vt && vt == _vehicle.Type)
                {
                    CboType.SelectedItem = item;
                    break;
                }
            }
            if (CboType.SelectedItem == null && CboType.Items.Count > 0) CboType.SelectedIndex = 0;

            foreach (ComboBoxItem item in CboStatus.Items)
            {
                if (item.Tag is VehicleStatus st && st == _vehicle.Status)
                {
                    CboStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtLicensePlate.Text))
            {
                MessageBox.Show("Vui lòng nhập tên xe và biển số xe!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string rateStr = TxtDailyRate.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(rateStr, out var rate) || rate <= 0)
            {
                MessageBox.Show("Giá thuê theo ngày phải lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _vehicle.Name = TxtName.Text.Trim();
            _vehicle.LicensePlate = TxtLicensePlate.Text.Trim();
            _vehicle.DailyRate = rate;
            _vehicle.RequiredDeposit = Math.Round(rate * 0.3m / 1000m) * 1000m;
            _vehicle.ConditionNote = TxtConditionNote.Text.Trim();

            if (CboType.SelectedItem is ComboBoxItem typeItem && typeItem.Tag is VehicleType newType)
            {
                _vehicle.Type = newType;
            }

            if (CboStatus.SelectedItem is ComboBoxItem cboItem && cboItem.Tag is VehicleStatus newStatus)
            {
                _vehicle.Status = newStatus;
            }

            // Đồng bộ xuống CSDL SQL Server (Table XeChoThue)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE XeChoThue SET TenXe = @Ten, BienSo = @BienSo, LoaiXe = @Loai, GiaThueNgay = @Gia, TienCocQuyDinh = @Coc, GhiChuTinhTrang = @Note, TrangThai = @Status WHERE Id = @Id",
                new SqlParameter("@Ten", _vehicle.Name),
                new SqlParameter("@BienSo", _vehicle.LicensePlate),
                new SqlParameter("@Loai", _vehicle.TypeDisplay),
                new SqlParameter("@Gia", _vehicle.DailyRate),
                new SqlParameter("@Coc", _vehicle.RequiredDeposit),
                new SqlParameter("@Note", _vehicle.ConditionNote),
                new SqlParameter("@Status", _vehicle.StatusDisplay),
                new SqlParameter("@Id", _vehicle.Id)
            );

            MessageBox.Show($"Đã lưu cập nhật thông tin xe [{_vehicle.Name} - {_vehicle.LicensePlate}] thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoadVehicleData();
        }
    }
}
