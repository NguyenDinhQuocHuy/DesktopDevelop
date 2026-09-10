using System;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username = "";
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        private bool _isPasswordVisible;
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                if (SetProperty(ref _isPasswordVisible, value))
                {
                    OnPropertyChanged(nameof(EyeIcon));
                }
            }
        }

        public string EyeIcon => IsPasswordVisible ? "👁️" : "🙈";

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoggedIn => AuthService.Instance.IsLoggedIn;

        public ICommand LoginCommand { get; }
        public ICommand QuickLoginAdminCommand { get; }
        public ICommand QuickLoginReceptionistCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }

        public event Action? LoginSucceeded;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
            QuickLoginAdminCommand = new RelayCommand(QuickLoginAdmin);
            QuickLoginReceptionistCommand = new RelayCommand(QuickLoginReceptionist);
            TogglePasswordVisibilityCommand = new RelayCommand(_ => IsPasswordVisible = !IsPasswordVisible);

            AuthService.Instance.AuthStateChanged += () =>
            {
                OnPropertyChanged(nameof(IsLoggedIn));
            };

            LoadRememberedCredentials();
        }

        private void LoadRememberedCredentials()
        {
            var remembered = AuthService.Instance.LoadRememberMe();
            if (remembered.HasValue && !string.IsNullOrWhiteSpace(remembered.Value.Username))
            {
                Username = remembered.Value.Username;
                Password = remembered.Value.Password;
                RememberMe = true;
            }
            else
            {
                Username = "";
                Password = "";
                RememberMe = false;
            }
        }

        private void ExecuteLogin(object? parameter)
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                return;
            }

            if (AuthService.Instance.Login(Username, Password, out string authError))
            {
                SaveOrClearRememberedCredentials();
                ErrorMessage = "";
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = authError;
            }
        }

        private void SaveOrClearRememberedCredentials()
        {
            if (RememberMe && !string.IsNullOrEmpty(Username))
            {
                AuthService.Instance.SaveRememberMe(Username, Password);
            }
            else
            {
                AuthService.Instance.ClearRememberMe();
            }
        }

        private void QuickLoginAdmin(object? parameter)
        {
            Username = "admin";
            Password = "123";
            ExecuteLogin(null);
        }

        private void QuickLoginReceptionist(object? parameter)
        {
            Username = "letan";
            Password = "123";
            ExecuteLogin(null);
        }
    }
}
