using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class AddVehicleDialogWindow : Window
    {
        public AddVehicleDialogWindow()
        {
            InitializeComponent();
            ResetForm();
        }

        private void ResetForm()
        {
            int nextIndex = DataService.Instance.Vehicles.Count + 1;
            string autoCode = $"XE{nextIndex:D3}";
            string suggestedPlate = $"59-F2 999.{nextIndex:D2}";
            while (DataService.Instance.Vehicles.Any(x => x.LicensePlate == suggestedPlate))
            {
                nextIndex++;
                autoCode = $"XE{nextIndex:D3}";
                suggestedPlate = $"59-F2 999.{nextIndex:D2}";
            }

            TxtCode.Text = autoCode;
            TxtLicensePlate.Text = suggestedPlate;
            TxtName.Text = "Honda AirBlade 160 2024";
            CboType.SelectedIndex = 0;
            TxtDailyRate.Text = "180,000";
            TxtRequiredDeposit.Text = "54,000";
            CboStatus.SelectedIndex = 0;
            TxtConditionNote.Text = "";
        }

        private void TxtDailyRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            string rateStr = TxtDailyRate.Text.Replace(",", "").Replace(".", "").Trim();
            if (decimal.TryParse(rateStr, out var rate) && rate > 0)
            {
                decimal deposit = Math.Round(rate * 0.3m / 1000m) * 1000m;
                TxtRequiredDeposit.Text = deposit.ToString("N0");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string plate = TxtLicensePlate.Text.Trim();
            string name = TxtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(plate) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ biển số xe và tên xe!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DataService.Instance.Vehicles.Any(x => string.Equals(x.LicensePlate, plate, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"Biển số xe [{plate}] đã tồn tại trong hệ thống! Vui lòng chọn biển số khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string rateStr = TxtDailyRate.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(rateStr, out var rate) || rate <= 0)
            {
                MessageBox.Show("Giá thuê theo ngày phải lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string depositStr = TxtRequiredDeposit.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(depositStr, out var deposit) || deposit < 0)
            {
                deposit = Math.Round(rate * 0.3m / 1000m) * 1000m;
            }

            VehicleType vType = VehicleType.Scooter;
            if (CboType.SelectedItem is ComboBoxItem typeItem && typeItem.Tag is VehicleType vt)
            {
                vType = vt;
            }

            VehicleStatus vStatus = VehicleStatus.Available;
            if (CboStatus.SelectedItem is ComboBoxItem stItem && stItem.Tag is VehicleStatus vs)
            {
                vStatus = vs;
            }

            string note = TxtConditionNote.Text.Trim();

            int newId = 0;
            try
            {
                string sql = "INSERT INTO XeChoThue (BienSo, TenXe, LoaiXe, GiaThueNgay, TienCocQuyDinh, TrangThai, GhiChuTinhTrang) " +
                             "OUTPUT INSERTED.Id " +
                             "VALUES (@BienSo, @Ten, @Loai, @Gia, @Coc, @Status, @Note);";

                using var conn = new SqlConnection(DatabaseService.Instance.ConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BienSo", plate);
                cmd.Parameters.AddWithValue("@Ten", name);
                cmd.Parameters.AddWithValue("@Loai", vType == VehicleType.Scooter ? "Xe tay ga" : vType == VehicleType.Manual ? "Xe số" : "Xe côn tay");
                cmd.Parameters.AddWithValue("@Gia", rate);
                cmd.Parameters.AddWithValue("@Coc", deposit);
                cmd.Parameters.AddWithValue("@Status", vStatus == VehicleStatus.Available ? "Sẵn sàng" : vStatus == VehicleStatus.Rented ? "Đang cho thuê" : "Bảo trì");
                cmd.Parameters.AddWithValue("@Note", note);

                var res = await cmd.ExecuteScalarAsync();
                if (res != null && int.TryParse(res.ToString(), out var id))
                {
                    newId = id;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SQL Insert Vehicle Error] {ex.Message}");
            }

            if (newId <= 0)
            {
                newId = DataService.Instance.Vehicles.Count > 0 
                    ? DataService.Instance.Vehicles.Max(x => x.Id) + 1 
                    : 1;
            }

            var newVehicle = new Vehicle
            {
                Id = newId,
                LicensePlate = plate,
                Name = name,
                Type = vType,
                DailyRate = rate,
                RequiredDeposit = deposit,
                Status = vStatus,
                ConditionNote = note
            };

            DataService.Instance.AddVehicleToCache(newVehicle);

            MessageBox.Show($"Đã thêm mới xe [{newVehicle.Name} - {newVehicle.LicensePlate}] (Mã: {TxtCode.Text}) thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}
