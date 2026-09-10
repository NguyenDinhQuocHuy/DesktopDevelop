using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class AddEventSpaceDialogWindow : Window
    {
        public AddEventSpaceDialogWindow()
        {
            InitializeComponent();
            ResetForm();
        }

        private void ResetForm()
        {
            int nextIndex = DataService.Instance.EventSpaces.Count + 1;
            string autoCode = $"KV{nextIndex:D2}";
            while (DataService.Instance.EventSpaces.Any(x => x.Name.Contains(autoCode)))
            {
                nextIndex++;
                autoCode = $"KV{nextIndex:D2}";
            }

            TxtCode.Text = autoCode;
            TxtName.Text = $"Sảnh Sự Kiện {autoCode}";
            CboSpaceType.Text = "Sảnh lớn";
            TxtCapacity.Text = "60";
            TxtHourlyRate.Text = "600,000";
            CboStatus.SelectedIndex = 0;
            TxtEquipments.Text = "";
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sảnh / khu vực sự kiện!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string rateStr = TxtHourlyRate.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(rateStr, out var rate) || rate <= 0)
            {
                MessageBox.Show("Giá thuê theo giờ phải lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtCapacity.Text.Trim(), out var capacity) || capacity <= 0)
            {
                MessageBox.Show("Sức chứa phải là số nguyên lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = TxtName.Text.Trim();
            string spaceType = string.IsNullOrWhiteSpace(CboSpaceType.Text) ? "Sảnh lớn" : CboSpaceType.Text.Trim();
            string equipments = TxtEquipments.Text.Trim();
            SpaceStatus status = SpaceStatus.Available;
            if (CboStatus.SelectedItem is ComboBoxItem cboItem && cboItem.Tag is SpaceStatus st)
            {
                status = st;
            }

            string statusStr = status switch
            {
                SpaceStatus.Rented => "Đang sử dụng",
                SpaceStatus.Maintenance => "Bảo trì",
                SpaceStatus.Cleaning => "Đang dọn",
                _ => "Trống"
            };

            int newId = 0;
            try
            {
                string sql = "INSERT INTO KhuVucSuKien (TenKhuVuc, LoaiKhuVuc, SucChua, GiaThueTheoGio, TrangThietBi, TrangThai) " +
                             "OUTPUT INSERTED.Id " +
                             "VALUES (@Ten, @Loai, @SucChua, @GiaGio, @Equip, @Status);";

                using var conn = new SqlConnection(DatabaseService.Instance.ConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ten", name);
                cmd.Parameters.AddWithValue("@Loai", spaceType);
                cmd.Parameters.AddWithValue("@SucChua", capacity);
                cmd.Parameters.AddWithValue("@GiaGio", rate);
                cmd.Parameters.AddWithValue("@Equip", equipments);
                cmd.Parameters.AddWithValue("@Status", statusStr);

                var res = await cmd.ExecuteScalarAsync();
                if (res != null && int.TryParse(res.ToString(), out var id))
                {
                    newId = id;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SQL Insert Space Error] {ex.Message}");
            }

            if (newId <= 0)
            {
                newId = DataService.Instance.EventSpaces.Count > 0 
                    ? DataService.Instance.EventSpaces.Max(x => x.Id) + 1 
                    : 1;
            }

            var newSpace = new EventSpace
            {
                Id = newId,
                Name = name,
                SpaceType = spaceType,
                Capacity = capacity,
                HourlyRate = rate,
                Equipments = equipments,
                Status = status
            };

            DataService.Instance.AddEventSpaceToCache(newSpace);

            MessageBox.Show($"Đã thêm mới khu vực [{newSpace.Name}] (Mã: {TxtCode.Text}) thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}
