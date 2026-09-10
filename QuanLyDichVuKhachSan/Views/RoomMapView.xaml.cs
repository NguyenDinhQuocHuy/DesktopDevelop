using System.Windows;
using System.Windows.Controls;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class RoomMapView : UserControl
    {
        public RoomMapView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is RoomMapViewModel oldVm)
            {
                oldVm.RequestScrollServiceIntoView -= OnRequestScrollServiceIntoView;
            }
            if (e.NewValue is RoomMapViewModel newVm)
            {
                newVm.RequestScrollServiceIntoView += OnRequestScrollServiceIntoView;
            }
        }

        private void OnRequestScrollServiceIntoView(RoomServiceUsageItem item)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (GridRoomServices != null && item != null)
                {
                    GridRoomServices.SelectedItem = item;
                    GridRoomServices.ScrollIntoView(item);
                    GridRoomServices.Focus();
                }
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void RoomButton_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement elem && elem.DataContext is HotelRoom room && DataContext is RoomMapViewModel vm)
            {
                vm.SelectRoomCommand.Execute(room);
            }
        }

        private HotelRoom? GetRoomFromSender(object sender)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.DataContext is HotelRoom room) return room;
                if (menuItem.Parent is ContextMenu cm && cm.PlacementTarget is FrameworkElement elem && elem.DataContext is HotelRoom r) return r;
            }
            return null;
        }

        private void MenuCheckIn_Click(object sender, RoutedEventArgs e)
        {
            var room = GetRoomFromSender(sender);
            if (room != null && room.Status != RoomStatus.Occupied && room.Status != RoomStatus.Maintenance && DataContext is RoomMapViewModel vm)
            {
                vm.CheckInCommand.Execute(room);
            }
        }

        private void MenuCheckOut_Click(object sender, RoutedEventArgs e)
        {
            var room = GetRoomFromSender(sender);
            if (room != null && room.Status == RoomStatus.Occupied && DataContext is RoomMapViewModel vm)
            {
                vm.CheckOutCommand.Execute(room);
            }
        }

        private void MenuExtendStay_Click(object sender, RoutedEventArgs e)
        {
            var room = GetRoomFromSender(sender);
            if (room != null && room.Status == RoomStatus.Occupied && DataContext is RoomMapViewModel vm)
            {
                vm.ExtendStayCommand.Execute(room);
            }
        }

        private void MenuSetCleaning_Click(object sender, RoutedEventArgs e)
        {
            var room = GetRoomFromSender(sender);
            if (room != null && room.Status != RoomStatus.Cleaning && room.Status != RoomStatus.Maintenance && DataContext is RoomMapViewModel vm)
            {
                vm.SetCleaningCommand.Execute(room);
            }
        }

        private void MenuFinishCleaning_Click(object sender, RoutedEventArgs e)
        {
            var room = GetRoomFromSender(sender);
            if (room != null && room.Status == RoomStatus.Cleaning && DataContext is RoomMapViewModel vm)
            {
                vm.FinishCleaningCommand.Execute(room);
            }
        }

        private void MenuSetMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var room = GetRoomFromSender(sender);
            if (room != null && room.Status != RoomStatus.Occupied && room.Status != RoomStatus.Maintenance && DataContext is RoomMapViewModel vm)
            {
                vm.SetMaintenanceCommand.Execute(room);
            }
        }

        private void MenuFinishMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var room = GetRoomFromSender(sender);
            if (room != null && room.Status == RoomStatus.Maintenance && DataContext is RoomMapViewModel vm)
            {
                vm.FinishMaintenanceCommand.Execute(room);
            }
        }

        private void MenuViewServiceDetail_Click(object sender, RoutedEventArgs e)
        {
            var item = GridRoomServices.SelectedItem as RoomServiceUsageItem;
            if (item == null && sender is MenuItem menuItem)
            {
                if (menuItem.CommandParameter is RoomServiceUsageItem paramItem) item = paramItem;
                else if (menuItem.DataContext is RoomServiceUsageItem dcItem) item = dcItem;
                else if (menuItem.Parent is ContextMenu cm && cm.PlacementTarget is FrameworkElement elem && elem.DataContext is RoomServiceUsageItem targetItem) item = targetItem;
            }

            if (item != null && DataContext is RoomMapViewModel vm)
            {
                var win = new InvoiceBillWindow(item, vm.SelectedRoom);
                win.Owner = Window.GetWindow(this);
                win.ShowDialog();
            }
        }

        private void ServiceRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement elem && elem.DataContext is RoomServiceUsageItem item && DataContext is RoomMapViewModel vm)
            {
                var win = new InvoiceBillWindow(item, vm.SelectedRoom);
                win.Owner = Window.GetWindow(this);
                win.ShowDialog();
            }
        }
    }
}
