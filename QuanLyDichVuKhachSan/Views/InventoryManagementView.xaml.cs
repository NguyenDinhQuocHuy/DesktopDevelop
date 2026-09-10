using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class InventoryManagementView : UserControl
    {
        public InventoryManagementView()
        {
            InitializeComponent();
        }

        private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.DataContext is FoodItem)
            {
                row.IsSelected = true;
                row.Focus();
            }
        }

        private FoodItem? GetTargetFoodItem(object sender)
        {
            if (sender is MenuItem mi)
            {
                if (mi.DataContext is FoodItem item) return item;
                if (mi.Parent is ContextMenu cm && cm.PlacementTarget is FrameworkElement elem && elem.DataContext is FoodItem targetItem)
                    return targetItem;
            }
            if (FindName("CatalogDataGrid") is DataGrid dg && dg.SelectedItem is FoodItem selected)
            {
                return selected;
            }
            return null;
        }

        private void MenuItem_Edit_Click(object sender, RoutedEventArgs e)
        {
            var item = GetTargetFoodItem(sender);
            if (item != null && DataContext is InventoryViewModel vm)
            {
                vm.EditFoodItemCommand.Execute(item);
            }
        }

        private void MenuItem_Trash_Click(object sender, RoutedEventArgs e)
        {
            var item = GetTargetFoodItem(sender);
            if (item != null && DataContext is InventoryViewModel vm)
            {
                vm.TrashFoodItemCommand.Execute(item);
            }
        }

        private void CatalogDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid dg && dg.SelectedItem is FoodItem item && DataContext is InventoryViewModel vm)
            {
                vm.EditFoodItemCommand.Execute(item);
            }
            else if (sender is DataGridRow row && row.DataContext is FoodItem rowItem && DataContext is InventoryViewModel vm2)
            {
                vm2.EditFoodItemCommand.Execute(rowItem);
            }
        }

        private void BtnTrash_Click(object sender, RoutedEventArgs e)
        {
            if (FindName("TrashPopup") is Popup popup)
            {
                popup.IsOpen = !popup.IsOpen;
            }
        }

        private void BtnCloseTrashPopup_Click(object sender, RoutedEventArgs e)
        {
            if (FindName("TrashPopup") is Popup popup)
            {
                popup.IsOpen = false;
            }
        }
    }
}
