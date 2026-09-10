using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class LaundryServiceView : UserControl
    {
        public LaundryServiceView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is LaundryViewModel oldVm)
            {
                oldVm.RequestScrollOrderIntoView -= OnRequestScrollOrderIntoView;
            }
            if (e.NewValue is LaundryViewModel newVm)
            {
                newVm.RequestScrollOrderIntoView += OnRequestScrollOrderIntoView;
            }
        }

        private void OnRequestScrollOrderIntoView(LaundryOrder order)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (GridLaundryOrders != null && order != null)
                {
                    GridLaundryOrders.SelectedItems.Clear();
                    GridLaundryOrders.SelectedItem = order;
                    GridLaundryOrders.ScrollIntoView(order);
                    GridLaundryOrders.Focus();
                }
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void BtnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddLaundryPartnerDialogWindow();
            dlg.ShowDialog();
        }

        private void PartnerBorder_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!AuthService.Instance.IsAdmin)
            {
                e.Handled = true; // Chỉ tài khoản admin mới có menu chuột phải chỉnh sửa đối tác
            }
        }

        private void MenuItem_EditPartner_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is LaundryPartner partner)
            {
                var dlg = new EditLaundryPartnerDialogWindow(partner);
                dlg.ShowDialog();
            }
        }

        private void PartnerCell_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                ShowBatchOrSingleDispatchContextMenu(fe, fe.DataContext as LaundryOrder);
            }
            e.Handled = true;
        }

        private void DataGridRow_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                // Nếu click chuột phải vào dòng CHƯA được chọn -> chọn riêng dòng đó
                // Nếu dòng click chuột phải ĐÃ ĐƯỢC CHỌN (trong nhóm nhiều dòng đang chọn qua Ctrl/Shift/kéo chuột) -> GIỮ NGUYÊN để thao tác hàng loạt
                if (!row.IsSelected)
                {
                    GridLaundryOrders.SelectedItems.Clear();
                    row.IsSelected = true;
                }
                row.Focus();
            }
        }

        private void DataGridRow_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                ShowBatchOrSingleDispatchContextMenu(row, row.DataContext as LaundryOrder);
            }
            e.Handled = true;
        }

        /// <summary>
        /// Menu chuột phải thông minh hỗ trợ:
        /// 1. BÀN GIAO HÀNG LOẠT: Khi người dùng giữ Ctrl hoặc kéo chuột chọn nhiều đơn chờ giao tiệm,
        ///    chuột phải sẽ hiện danh sách tiệm để bàn giao toàn bộ các đơn đó cùng lúc.
        /// 2. BÀN GIAO ĐƠN LẺ: Click chọn nhanh tiệm đối tác chỉ với 1 cú nhấp chuột.
        /// </summary>
        private void ShowBatchOrSingleDispatchContextMenu(FrameworkElement targetElement, LaundryOrder? clickedOrder)
        {
            var selectedOrders = GridLaundryOrders.SelectedItems
                .OfType<LaundryOrder>()
                .ToList();

            if (!selectedOrders.Any() && clickedOrder != null)
            {
                selectedOrders.Add(clickedOrder);
            }

            var dispatchableOrders = selectedOrders.Where(o => o.CanDispatch).ToList();
            var activePartners = DataService.Instance.LaundryPartners.Where(p => p.IsActive).ToList();

            var contextMenu = new ContextMenu();

            if (dispatchableOrders.Count > 1)
            {
                // ==================== BÀN GIAO HÀNG LOẠT (MULTI-SELECTION) ====================
                var titleItem = new MenuItem
                {
                    Header = $"🚚 BÀN GIAO {dispatchableOrders.Count} ĐƠN CHO ĐỐI TÁC TIỆM GIẶT",
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(3, 105, 161)),
                    IsEnabled = false
                };
                contextMenu.Items.Add(titleItem);

                decimal totalWeight = dispatchableOrders.Sum(o => o.WeightKg);
                decimal totalEstimated = dispatchableOrders.Sum(o => o.TotalPrice);
                var subItem = new MenuItem
                {
                    Header = $"📦 Đã chọn: {dispatchableOrders.Count} đơn · Tổng trọng lượng: {totalWeight:0.#} kg · Tổng thu: {totalEstimated:N0} đ",
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                    IsEnabled = false
                };
                contextMenu.Items.Add(subItem);
                contextMenu.Items.Add(new Separator());

                if (!activePartners.Any())
                {
                    var noPartnerItem = new MenuItem
                    {
                        Header = "⚠️ Không có đối tác giặt ủi nào đang hoạt động!",
                        IsEnabled = false
                    };
                    contextMenu.Items.Add(noPartnerItem);
                }
                else
                {
                    var partnerHeader = new MenuItem
                    {
                        Header = "👉 Chọn tiệm để bàn giao tất cả đơn đã chọn:",
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(15, 23, 42)),
                        IsEnabled = false
                    };
                    contextMenu.Items.Add(partnerHeader);

                    foreach (var partner in activePartners)
                    {
                        var partnerItem = new MenuItem
                        {
                            Header = $"🏢 {partner.Name} (SĐT: {partner.PhoneNumber})",
                            FontWeight = FontWeights.SemiBold,
                            Foreground = new SolidColorBrush(Color.FromRgb(2, 132, 199)),
                            ToolTip = $"Click để bàn giao ngay {dispatchableOrders.Count} đơn đã chọn cho [{partner.Name}]"
                        };

                        partnerItem.Click += (s, ev) =>
                        {
                            int successCount = 0;
                            foreach (var ord in dispatchableOrders)
                            {
                                DataService.Instance.AssignPartnerToLaundryOrder(ord, partner);
                                successCount++;
                            }

                            (DataContext as ViewModels.LaundryViewModel)?.RefreshPendingReturns();
                            (DataContext as ViewModels.LaundryViewModel)?.ApplyLaundryFilter();

                            MessageBox.Show(
                                $"Đã bàn giao thành công {successCount} đơn giặt ủi cho đối tác [{partner.Name}]!\n\n" +
                                $"• Tổng trọng lượng: {totalWeight:0.#} kg\n" +
                                $"• Trạng thái các đơn đã chuyển sang 'Đang giặt'.",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );
                        };

                        contextMenu.Items.Add(partnerItem);
                    }
                }

                // Tùy chọn hủy hàng loạt các đơn chờ giao tiệm
                var cancelableOrders = selectedOrders.Where(o => o.CanCancel).ToList();
                if (cancelableOrders.Any())
                {
                    contextMenu.Items.Add(new Separator());
                    var cancelAllItem = new MenuItem
                    {
                        Header = $"❌ HỦY {cancelableOrders.Count} ĐƠN GIẶT ỦI ĐÃ CHỌN",
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38))
                    };
                    cancelAllItem.Click += (s, ev) =>
                    {
                        var result = MessageBox.Show(
                            $"Bạn có chắc chắn muốn hủy {cancelableOrders.Count} đơn giặt ủi đã chọn không?",
                            "Thông báo",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes && DataContext is ViewModels.LaundryViewModel vm)
                        {
                            foreach (var ord in cancelableOrders)
                            {
                                DataService.Instance.CancelLaundryOrder(ord);
                            }
                            vm.ApplyLaundryFilter();
                            vm.RefreshPendingReturns();
                            MessageBox.Show($"Đã hủy {cancelableOrders.Count} đơn giặt ủi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    };
                    contextMenu.Items.Add(cancelAllItem);
                }
            }
            else
            {
                // ==================== BÀN GIAO 1 ĐƠN LẺ ====================
                var singleOrder = dispatchableOrders.FirstOrDefault() ?? clickedOrder ?? selectedOrders.FirstOrDefault();
                if (singleOrder == null) return;

                var titleItem = new MenuItem
                {
                    Header = $"🧺 ĐƠN GIẶT [{singleOrder.OrderCode}] - {singleOrder.StatusDisplay}",
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                    IsEnabled = false
                };
                contextMenu.Items.Add(titleItem);

                var infoItem = new MenuItem
                {
                    Header = $"Khách: {singleOrder.CustomerName} (P.{singleOrder.RoomNumber}) · {singleOrder.WeightKg} kg · {singleOrder.TotalPrice:N0} đ",
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                    IsEnabled = false
                };
                contextMenu.Items.Add(infoItem);
                contextMenu.Items.Add(new Separator());

                if (singleOrder.CanDispatch)
                {
                    var dispatchHeader = new MenuItem
                    {
                        Header = "🚚 Chọn tiệm bàn giao:",
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(3, 105, 161)),
                        IsEnabled = false
                    };
                    contextMenu.Items.Add(dispatchHeader);

                    foreach (var partner in activePartners)
                    {
                        decimal costPerKg = partner.GetCostPriceForService(singleOrder.ServiceType);
                        decimal totalCost = Math.Round(singleOrder.WeightKg * costPerKg, 0);
                        decimal hotelProfit = singleOrder.TotalPrice - totalCost;

                        var partnerItem = new MenuItem
                        {
                            Header = $"🏢 {partner.Name} (Giá vốn: {costPerKg:N0} đ/kg · KS lời: {hotelProfit:N0} đ)",
                            FontWeight = FontWeights.SemiBold,
                            Foreground = new SolidColorBrush(Color.FromRgb(2, 132, 199)),
                            ToolTip = $"Địa chỉ: {partner.Address}\nSĐT: {partner.PhoneNumber}\n\n👉 Click chuột trái để bàn giao ngay cho [{partner.Name}]"
                        };

                        partnerItem.Click += (s, ev) =>
                        {
                            DataService.Instance.AssignPartnerToLaundryOrder(singleOrder, partner);
                            (DataContext as ViewModels.LaundryViewModel)?.RefreshPendingReturns();
                            (DataContext as ViewModels.LaundryViewModel)?.ApplyLaundryFilter();

                            MessageBox.Show(
                                $"Đã bàn giao đơn hàng {singleOrder.OrderCode} cho đối tác [{partner.Name}] thành công!\n\n" +
                                $"• Trạng thái: Đang giặt\n" +
                                $"• Giá vốn: {costPerKg:N0} đ/kg ({totalCost:N0} đ)\n" +
                                $"• Khách sạn lời: {hotelProfit:N0} đ",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );
                        };

                        contextMenu.Items.Add(partnerItem);
                    }

                    var openDialogItem = new MenuItem
                    {
                        Header = "📋 Mở bảng chi tiết bàn giao đối tác...",
                        FontSize = 11.5
                    };
                    openDialogItem.Click += (s, ev) =>
                    {
                        var dlg = new SelectPartnerDialogWindow(activePartners, singleOrder);
                        if (dlg.ShowDialog() == true && dlg.SelectedPartner != null)
                        {
                            DataService.Instance.AssignPartnerToLaundryOrder(singleOrder, dlg.SelectedPartner);
                            (DataContext as ViewModels.LaundryViewModel)?.RefreshPendingReturns();
                            (DataContext as ViewModels.LaundryViewModel)?.ApplyLaundryFilter();
                            MessageBox.Show($"Đã bàn giao đơn hàng {singleOrder.OrderCode} cho đối tác [{dlg.SelectedPartner.Name}] thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    };
                    contextMenu.Items.Add(openDialogItem);
                }

                if (singleOrder.CanCancel)
                {
                    contextMenu.Items.Add(new Separator());
                    var cancelItem = new MenuItem
                    {
                        Header = "❌ HỦY ĐƠN GIẶT ỦI",
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38)),
                        ToolTip = "Hủy đơn giặt khi còn ở trạng thái Chờ giao tiệm. Tự động xóa khỏi bill phòng hoặc hoàn tiền lại nếu đã thanh toán trực tiếp."
                    };
                    cancelItem.Click += (s, ev) =>
                    {
                        if (DataContext is ViewModels.LaundryViewModel vm)
                        {
                            vm.CancelOrder(singleOrder);
                        }
                    };
                    contextMenu.Items.Add(cancelItem);
                }
            }

            targetElement.ContextMenu = contextMenu;
            contextMenu.IsOpen = true;
        }
    }
}
