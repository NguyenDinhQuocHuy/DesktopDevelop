using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;

namespace QuanLyDichVuKhachSan.Services
{
    public class AuthService : INotifyPropertyChanged
    {
        private static AuthService? _instance;
        public static AuthService Instance => _instance ??= new AuthService();

        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;
        public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
        public bool IsReceptionist => CurrentUser?.Role == UserRole.Receptionist;

        public event Action? AuthStateChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        private static string RememberFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "remember.dat");

        private void NotifyAuthChanged()
        {
            AuthStateChanged?.Invoke();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUser)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoggedIn)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAdmin)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReceptionist)));
        }

        public bool Login(string username, string password, out string errorMessage)
        {
            errorMessage = "";
            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "⚠️ Vui lòng nhập Tên đăng nhập!";
                return false;
            }

            string cleanUsername = username.Trim();
            string cleanPassword = password != null ? password.Trim() : "";

            // 1. Ưu tiên kiểm tra từ CSDL SQL Server (Table TaiKhoan / NguoiDung)
            var dbUsers = DatabaseService.Instance.LoadUsersFromDb();
            var user = dbUsers.FirstOrDefault(u => 
                string.Equals(u.Username, cleanUsername, StringComparison.OrdinalIgnoreCase));

            // 2. Nếu không tìm thấy trong DB, kiểm tra trong DataService cache
            if (user == null)
            {
                user = DataService.Instance.Users.FirstOrDefault(u => 
                    string.Equals(u.Username, cleanUsername, StringComparison.OrdinalIgnoreCase));
            }

            if (user == null)
            {
                errorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                return false;
            }

            if (!user.IsActive)
            {
                errorMessage = "Tài khoản này đã bị khóa (Vô hiệu hóa)";
                return false;
            }

            // 3. Xác thực mật khẩu qua SecurityHelper (Hỗ trợ SHA-256 + Salt & Plaintext tương thích ngược)
            bool isValidPassword = SecurityHelper.VerifyPassword(cleanPassword, user.Password);

            if (isValidPassword)
            {
                CurrentUser = user;

                // Tự động nâng cấp hash mật khẩu trong CSDL nếu trước đó là plaintext hoặc placeholder
                string properHash = SecurityHelper.HashPassword(cleanPassword);
                if (user.Password != properHash)
                {
                    user.Password = properHash;
                    _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                        "IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL " +
                        "UPDATE TaiKhoan SET MatKhauHash = @Hash WHERE Id = @Id OR TenDangNhap = @User",
                        new Microsoft.Data.SqlClient.SqlParameter("@Hash", properHash),
                        new Microsoft.Data.SqlClient.SqlParameter("@Id", user.Id),
                        new Microsoft.Data.SqlClient.SqlParameter("@User", user.Username)
                    );
                }

                NotifyAuthChanged();
                try
                {
                    NotificationService.Instance.NotifyLogin(user.Username, user.RoleDisplayName, user.FullName);
                    NotificationService.Instance.ScanOperationalAlerts();
                }
                catch { }
                return true;
            }

            errorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
            return false;
        }

        public bool Login(string username, string password)
        {
            return Login(username, password, out _);
        }

        public void Logout()
        {
            CurrentUser = null;
            NotifyAuthChanged();
        }

        /// <summary>
        /// Lưu thông tin Remember-Me bảo mật với Windows DPAPI
        /// </summary>
        public void SaveRememberMe(string username, string password)
        {
            try
            {
                string raw = $"{username}|{password}";
                string encrypted = SecurityHelper.EncryptString(raw);
                File.WriteAllText(RememberFilePath, encrypted);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SaveRememberMe Error] {ex.Message}");
            }
        }

        /// <summary>
        /// Đọc thông tin Remember-Me đã giải mã
        /// </summary>
        public (string Username, string Password)? LoadRememberMe()
        {
            try
            {
                if (File.Exists(RememberFilePath))
                {
                    string encrypted = File.ReadAllText(RememberFilePath);
                    string raw = SecurityHelper.DecryptString(encrypted);
                    if (!string.IsNullOrEmpty(raw) && raw.Contains('|'))
                    {
                        var parts = raw.Split('|');
                        if (parts.Length >= 2)
                        {
                            return (parts[0], parts[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadRememberMe Error] {ex.Message}");
            }
            return null;
        }

        public void ClearRememberMe()
        {
            try
            {
                if (File.Exists(RememberFilePath))
                {
                    File.Delete(RememberFilePath);
                }
            }
            catch { }
        }
    }
}
