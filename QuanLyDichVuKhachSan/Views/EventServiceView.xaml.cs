using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class EventServiceView : UserControl
    {
        private bool _isDragging = false;
        private int _dragStartHour = -1;

        public EventServiceView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is EventViewModel oldVm)
            {
                oldVm.RequestScrollBookingIntoView -= OnRequestScrollBookingIntoView;
            }
            if (e.NewValue is EventViewModel newVm)
            {
                newVm.RequestScrollBookingIntoView += OnRequestScrollBookingIntoView;
            }
        }

        private void OnRequestScrollBookingIntoView(EventBooking booking)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (GridEventBookings != null && booking != null)
                {
                    GridEventBookings.SelectedItems.Clear();
                    GridEventBookings.SelectedItem = booking;
                    GridEventBookings.ScrollIntoView(booking);
                    GridEventBookings.Focus();
                }
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void Slot_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement elem && elem.DataContext is EventHourlySlot slot)
            {
                bool isCtrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
                if (DataContext is EventViewModel vm)
                {
                    if (isCtrl)
                    {
                        vm.ToggleSlot(slot, isCtrl: true);
                    }
                    else
                    {
                        _isDragging = true;
                        _dragStartHour = slot.Hour;
                        vm.SelectRange(_dragStartHour, slot.Hour);
                    }
                }
            }
        }

        private void Slot_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement elem && elem.DataContext is EventHourlySlot slot)
            {
                if (DataContext is EventViewModel vm && _dragStartHour >= 0)
                {
                    vm.SelectRange(_dragStartHour, slot.Hour);
                }
            }
        }

        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _dragStartHour = -1;
        }

        private void Border_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!Services.AuthService.Instance.IsAdmin)
            {
                e.Handled = true; // Chặn hoàn toàn mở ContextMenu đối với tài khoản Lễ tân
            }
        }

        private void MenuItem_EditSpace_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is Models.EventSpace space)
            {
                var dlg = new EditEventSpaceDialogWindow(space);
                if (dlg.ShowDialog() == true)
                {
                    if (DataContext is EventViewModel vm)
                    {
                        vm.CalculateEstimatedPrice();
                        if (vm.IsTimetableTab) vm.BuildHourlySchedule();
                    }
                }
            }
        }

        private void BtnAddSpace_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddEventSpaceDialogWindow();
            if (dlg.ShowDialog() == true)
            {
                if (DataContext is EventViewModel vm)
                {
                    vm.CalculateEstimatedPrice();
                    if (vm.IsTimetableTab) vm.BuildHourlySchedule();
                }
            }
        }
    }
}
