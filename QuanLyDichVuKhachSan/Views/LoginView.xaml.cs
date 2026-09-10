using System.Windows;
using System.Windows.Controls;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            Loaded += LoginView_Loaded;
            DataContextChanged += LoginView_DataContextChanged;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            SyncPasswordBox();
        }

        private void LoginView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SyncPasswordBox();
        }

        private void SyncPasswordBox()
        {
            if (DataContext is LoginViewModel vm && !string.IsNullOrEmpty(vm.Password))
            {
                if (TxtPassword.Password != vm.Password)
                {
                    TxtPassword.Password = vm.Password;
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                if (vm.Password != pb.Password)
                {
                    vm.Password = pb.Password;
                }
            }
        }

        private void OnInputKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (DataContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
                {
                    vm.LoginCommand.Execute(null);
                }
            }
        }

        private void QuickLoginAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                TxtPassword.Password = "123";
                vm.QuickLoginAdminCommand.Execute(null);
            }
        }

        private void QuickLoginReceptionist_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                TxtPassword.Password = "123";
                vm.QuickLoginReceptionistCommand.Execute(null);
            }
        }
    }
}
