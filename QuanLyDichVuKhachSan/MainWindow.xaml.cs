using System.Windows;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }
        public LoginViewModel LoginVM { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            LoginVM = new LoginViewModel();

            DataContext = ViewModel;
            LoginControl.DataContext = LoginVM;

            // Khởi tạo ở kích thước nhỏ gọn cho màn hình đăng nhập
            ApplyLoginWindowSize();

            LoginVM.LoginSucceeded += () =>
            {
                ViewModel.RefreshAuthState();
                ApplyWorkWindowSize();
            };

            ViewModel.RequestLogout += () =>
            {
                ApplyLoginWindowSize();
            };
        }

        private void ApplyLoginWindowSize()
        {
            this.WindowState = WindowState.Normal;
            this.ResizeMode = ResizeMode.CanMinimize;
            this.MinWidth = 460;
            this.MinHeight = 460;
            this.Width = 480;
            this.Height = 480;
            CenterWindowOnScreen();
        }

        private void ApplyWorkWindowSize()
        {
            this.MinWidth = 1100;
            this.MinHeight = 700;
            this.ResizeMode = ResizeMode.CanResize;
            this.WindowState = WindowState.Maximized;
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
        }
    }
}