using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class EditEventSpaceDialogWindow : Window
    {
        private readonly EventSpace _space;

        public EditEventSpaceDialogWindow(EventSpace space)
        {
            InitializeComponent();
            _space = space;
            LoadSpaceData();
        }

        private void LoadSpaceData()
        {
            TxtName.Text = _space.Name;
            TxtSpaceType.Text = _space.SpaceType;
            TxtCapacity.Text = _space.Capacity.ToString();
            TxtHourlyRate.Text = _space.HourlyRate.ToString("N0");
            TxtEquipments.Text = _space.Equipments;

            foreach (ComboBoxItem item in CboStatus.Items)
            {
                if (item.Tag is SpaceStatus st && st == _space.Status)
                {
                    CboStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sảnh / khu vực sự kiện!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtCapacity.Text.Trim(), out var capacity) || capacity <= 0)
            {
                MessageBox.Show("Sức chứa phải là số nguyên dương!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string rateStr = TxtHourlyRate.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(rateStr, out var rate) || rate < 0)
            {
                MessageBox.Show("Giá thuê theo giờ không hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _space.Name = TxtName.Text.Trim();
            _space.SpaceType = TxtSpaceType.Text.Trim();
            _space.Capacity = capacity;
            _space.HourlyRate = rate;
            _space.Equipments = TxtEquipments.Text.Trim();

            if (CboStatus.SelectedItem is ComboBoxItem cboItem && cboItem.Tag is SpaceStatus newStatus)
            {
                _space.Status = newStatus;
            }

            // Đồng bộ xuống CSDL SQL Server (Table KhuVucSuKien)
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE KhuVucSuKien SET TenKhuVuc = @Ten, LoaiKhuVuc = @Loai, SucChua = @SucChua, GiaThueTheoGio = @GiaGio, TrangThietBi = @Equip, TrangThai = @Status WHERE Id = @Id",
                new SqlParameter("@Ten", _space.Name),
                new SqlParameter("@Loai", _space.SpaceType),
                new SqlParameter("@SucChua", _space.Capacity),
                new SqlParameter("@GiaGio", _space.HourlyRate),
                new SqlParameter("@Equip", _space.Equipments),
                new SqlParameter("@Status", _space.StatusDisplay),
                new SqlParameter("@Id", _space.Id)
            );

            MessageBox.Show($"Đã lưu cập nhật thông tin khu vực [{_space.Name}] thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoadSpaceData();
        }
    }
}
