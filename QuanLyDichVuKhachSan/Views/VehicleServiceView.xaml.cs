using System.Windows;
using System.Windows.Controls;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class VehicleServiceView : UserControl
    {
        public VehicleServiceView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is VehicleViewModel oldVm)
            {
                oldVm.RequestScrollRentalIntoView -= OnRequestScrollRentalIntoView;
            }
            if (e.NewValue is VehicleViewModel newVm)
            {
                newVm.RequestScrollRentalIntoView += OnRequestScrollRentalIntoView;
            }
        }

        private void OnRequestScrollRentalIntoView(VehicleRental rental)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (GridVehicleRentals != null && rental != null)
                {
                    GridVehicleRentals.SelectedItems.Clear();
                    GridVehicleRentals.SelectedItem = rental;
                    GridVehicleRentals.ScrollIntoView(rental);
                    GridVehicleRentals.Focus();
                }
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void VehicleBorder_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!Services.AuthService.Instance.IsAdmin)
            {
                e.Handled = true; // Chặn hoàn toàn mở ContextMenu trên xe đối với tài khoản Lễ tân
            }
        }

        private void MenuItem_SetMaintenance_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is Vehicle v)
            {
                if (DataContext is VehicleViewModel vm)
                {
                    vm.SetMaintenanceCommand?.Execute(v);
                }
            }
        }

        private void MenuItem_SetAvailable_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is Vehicle v)
            {
                if (DataContext is VehicleViewModel vm)
                {
                    vm.SetAvailableCommand?.Execute(v);
                }
            }
        }

        private void MenuItem_EditVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is Vehicle v)
            {
                var dlg = new EditVehicleDialogWindow(v);
                if (dlg.ShowDialog() == true)
                {
                    if (DataContext is VehicleViewModel vm)
                    {
                        vm.ApplyVehicleFilter();
                    }
                }
            }
        }

        private void BtnAddVehicle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddVehicleDialogWindow();
            if (dlg.ShowDialog() == true)
            {
                if (DataContext is VehicleViewModel vm)
                {
                    vm.ApplyVehicleFilter();
                }
            }
        }
    }
}
