using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class StaffManagementView : UserControl
    {
        public StaffManagementView()
        {
            InitializeComponent();
        }

        private void ShiftSlot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement elem && elem.DataContext is WeeklyShiftSlot slot)
            {
                bool isCtrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
                if (DataContext is StaffViewModel vm)
                {
                    vm.HandleSlotClick(slot, isCtrl);
                }
            }
        }

        private void ShiftSlot_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (sender is FrameworkElement elem && elem.DataContext is WeeklyShiftSlot slot)
            {
                if (DataContext is StaffViewModel vm)
                {
                    // Đảm bảo chọn slot này khi click chuột phải
                    vm.HandleSlotClick(slot, isCtrlPressed: false);

                    var cm = new ContextMenu();

                    // Tiêu đề ca trực
                    var headerItem = new MenuItem
                    {
                        Header = $"📅 {slot.DayOfWeekName} ({slot.Date:dd/MM}) - {slot.ShiftName} ({slot.TimeRange})",
                        FontWeight = FontWeights.Bold,
                        IsEnabled = false
                    };
                    cm.Items.Add(headerItem);
                    cm.Items.Add(new Separator());

                    // Danh sách nhân viên để chọn nhanh / đổi người trực
                    foreach (var staff in vm.Data.StaffList)
                    {
                        var isCurrent = slot.StaffId == staff.Id;
                        var staffItem = new MenuItem
                        {
                            Header = $"👤 {staff.FullName} ({staff.Code})",
                            FontWeight = isCurrent ? FontWeights.Bold : FontWeights.Normal,
                            Foreground = isCurrent ? new SolidColorBrush(Color.FromRgb(22, 101, 52)) : new SolidColorBrush(Color.FromRgb(30, 41, 59))
                        };

                        if (isCurrent)
                        {
                            staffItem.Icon = new TextBlock
                            {
                                Text = "✔",
                                Foreground = new SolidColorBrush(Color.FromRgb(22, 101, 52)),
                                FontWeight = FontWeights.Bold,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };
                        }

                        var targetStaff = staff;
                        staffItem.Click += (s, ev) =>
                        {
                            vm.AssignSlotDirectly(slot, targetStaff);
                        };
                        cm.Items.Add(staffItem);
                    }

                    cm.Items.Add(new Separator());

                    // Mục hủy phân công ca này
                    if (slot.IsAssigned)
                    {
                        var clearItem = new MenuItem
                        {
                            Header = "❌ Hủy / Xóa phân công ca này",
                            Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38)),
                            FontWeight = FontWeights.SemiBold
                        };
                        clearItem.Click += (s, ev) =>
                        {
                            vm.ClearSlotDirectly(slot);
                        };
                        cm.Items.Add(clearItem);
                    }

                    elem.ContextMenu = cm;
                    cm.IsOpen = true;
                    e.Handled = true;
                }
            }
        }
    }
}
