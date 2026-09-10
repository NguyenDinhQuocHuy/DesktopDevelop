using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class ServicesConfigView : UserControl
    {
        private User? _selectedUser;

        public ServicesConfigView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        public void LoadUsers()
        {
            var dbUsers = DatabaseService.Instance.LoadUsersFromDb();
            if (dbUsers != null && dbUsers.Count > 0)
            {
                DataService.Instance.Users.Clear();
                foreach (var u in dbUsers)
                {
                    DataService.Instance.Users.Add(u);
                }
            }

            DgUsers.ItemsSource = null;
            DgUsers.ItemsSource = DataService.Instance.Users;
        }

        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgUsers.SelectedItem is User user)
            {
                _selectedUser = user;
                TxtUsername.Text = user.Username;
                TxtPassword.Text = user.Password;
                TxtFullName.Text = user.FullName;
                ChkIsActive.IsChecked = user.IsActive;

                if (user.Role == UserRole.Admin)
                {
                    CboRole.SelectedIndex = 0;
                }
                else
                {
                    CboRole.SelectedIndex = 1;
                }

                TxtFormTitle.Text = $"✏️ CẬP NHẬT TÀI KHOẢN: [{user.Username}]";
                BtnSave.Content = "💾 Cập Nhật";
            }
        }

        private void BtnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is User user)
            {
                if (user.Role == UserRole.Admin && user.IsActive)
                {
                    MessageBox.Show("Không thể khóa tài khoản Admin duy nhất của hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                user.IsActive = !user.IsActive;

                string sql = @"
IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL
BEGIN
    UPDATE TaiKhoan SET TrangThai = @BitStatus WHERE Id = @Id OR TenDangNhap = @User
END
ELSE IF OBJECT_ID('NguoiDung', 'U') IS NOT NULL
BEGIN
    UPDATE NguoiDung SET TrangThai = @StatusStr WHERE NguoiDungID = @Id OR Id = @Id OR TenDangNhap = @User
END";

                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    sql,
                    new SqlParameter("@BitStatus", user.IsActive ? 1 : 0),
                    new SqlParameter("@StatusStr", user.IsActive ? "HoatDong" : "Khoa"),
                    new SqlParameter("@Id", user.Id),
                    new SqlParameter("@User", user.Username)
                );

                DgUsers.Items.Refresh();
                if (_selectedUser == user)
                {
                    ChkIsActive.IsChecked = user.IsActive;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Text.Trim();
            string fullName = TxtFullName.Text.Trim();
            bool isActive = ChkIsActive.IsChecked ?? true;

            var roleItem = CboRole.SelectedItem as ComboBoxItem;
            UserRole role = (roleItem?.Tag?.ToString() == "Admin") ? UserRole.Admin : UserRole.Receptionist;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedUser != null)
            {
                // Kiểm tra trùng username với user khác
                if (DataService.Instance.Users.Any(u => u.Id != _selectedUser.Id && u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"Tên đăng nhập [{username}] đã được sử dụng bởi tài khoản khác!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_selectedUser.Role != UserRole.Admin && role == UserRole.Admin)
                {
                    if (DataService.Instance.Users.Any(u => u.Role == UserRole.Admin && u.Id != _selectedUser.Id))
                    {
                        MessageBox.Show("Hệ thống chỉ cho phép có DUY NHẤT 1 tài khoản Admin!\n\nKhông thể chuyển thêm tài khoản này sang quyền Admin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else if (_selectedUser.Role == UserRole.Admin && role != UserRole.Admin)
                {
                    int adminCount = DataService.Instance.Users.Count(u => u.Role == UserRole.Admin);
                    if (adminCount <= 1)
                    {
                        MessageBox.Show("Hệ thống bắt buộc phải có ít nhất 1 tài khoản Admin, không thể hạ quyền tài khoản Admin duy nhất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                string oldUsername = _selectedUser.Username;
                _selectedUser.Username = username;
                _selectedUser.Password = password;
                _selectedUser.FullName = string.IsNullOrWhiteSpace(fullName) ? (role == UserRole.Admin ? "Chủ Khách Sạn" : "Lễ tân") : fullName;
                _selectedUser.Role = role;
                _selectedUser.IsActive = isActive;

                string updateSql = @"
IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL
BEGIN
    UPDATE TaiKhoan 
    SET TenDangNhap = @User, MatKhauHash = @PassHash, HoTen = @Ten, VaiTro = @RoleStr, TrangThai = @BitStatus
    WHERE Id = @Id OR TenDangNhap = @User OR TenDangNhap = @OldUser
END
ELSE IF OBJECT_ID('NguoiDung', 'U') IS NOT NULL
BEGIN
    UPDATE NguoiDung 
    SET TenDangNhap = @User, MatKhau = @Pass, HoTen = @Ten, VaiTro = @RoleStr, TrangThai = @StatusStr
    WHERE NguoiDungID = @Id OR Id = @Id OR TenDangNhap = @User OR TenDangNhap = @OldUser
END";

                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    updateSql,
                    new SqlParameter("@User", _selectedUser.Username),
                    new SqlParameter("@OldUser", oldUsername),
                    new SqlParameter("@Pass", _selectedUser.Password),
                    new SqlParameter("@PassHash", SecurityHelper.HashPassword(_selectedUser.Password)),
                    new SqlParameter("@Ten", _selectedUser.FullName),
                    new SqlParameter("@RoleStr", _selectedUser.Role == UserRole.Admin ? "Admin" : "LeTan"),
                    new SqlParameter("@BitStatus", _selectedUser.IsActive ? 1 : 0),
                    new SqlParameter("@StatusStr", _selectedUser.IsActive ? "HoatDong" : "Khoa"),
                    new SqlParameter("@Id", _selectedUser.Id)
                );

                DgUsers.Items.Refresh();
                MessageBox.Show($"Đã cập nhật tài khoản [{_selectedUser.Username}] và lưu vào CSDL thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (role == UserRole.Admin)
                {
                    if (DataService.Instance.Users.Any(u => u.Role == UserRole.Admin))
                    {
                        MessageBox.Show("Hệ thống chỉ cho phép có DUY NHẤT 1 tài khoản Admin!\n\nBạn chỉ có thể tạo thêm tài khoản mới với vai trò Lễ Tân.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (DataService.Instance.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"Tên đăng nhập [{username}] đã tồn tại trong hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int newId = DataService.Instance.Users.Count > 0 ? DataService.Instance.Users.Max(u => u.Id) + 1 : 1;
                var newUser = new User
                {
                    Id = newId,
                    Username = username,
                    Password = password,
                    FullName = string.IsNullOrWhiteSpace(fullName) ? (role == UserRole.Admin ? "Chủ Khách Sạn" : "Lễ tân") : fullName,
                    Role = role,
                    IsActive = isActive
                };

                DataService.Instance.Users.Add(newUser);

                string insertSql = @"
IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL
BEGIN
    INSERT INTO TaiKhoan (TenDangNhap, MatKhauHash, HoTen, VaiTro, TrangThai, NgayTao)
    VALUES (@User, @PassHash, @Ten, @RoleStr, @BitStatus, GETDATE())
END
ELSE IF OBJECT_ID('NguoiDung', 'U') IS NOT NULL
BEGIN
    INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, VaiTro, TrangThai)
    VALUES (@User, @Pass, @Ten, @RoleStr, @StatusStr)
END";

                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    insertSql,
                    new SqlParameter("@User", newUser.Username),
                    new SqlParameter("@Pass", newUser.Password),
                    new SqlParameter("@PassHash", SecurityHelper.HashPassword(newUser.Password)),
                    new SqlParameter("@Ten", newUser.FullName),
                    new SqlParameter("@RoleStr", newUser.Role == UserRole.Admin ? "Admin" : "LeTan"),
                    new SqlParameter("@BitStatus", newUser.IsActive ? 1 : 0),
                    new SqlParameter("@StatusStr", newUser.IsActive ? "HoatDong" : "Khoa")
                );

                DgUsers.Items.Refresh();
                MessageBox.Show($"Đã tạo mới tài khoản [{newUser.Username}] và lưu vào CSDL thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            BtnReset_Click(sender, e);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            _selectedUser = null;
            DgUsers.SelectedItem = null;
            TxtUsername.Text = "";
            TxtPassword.Text = "";
            TxtFullName.Text = "";
            CboRole.SelectedIndex = 1;
            ChkIsActive.IsChecked = true;
            TxtFormTitle.Text = "➕ THÊM / CẬP NHẬT TÀI KHOẢN";
            BtnSave.Content = "💾 Thêm Tài Khoản";
        }
    }
}
