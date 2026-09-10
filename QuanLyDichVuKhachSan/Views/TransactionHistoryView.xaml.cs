using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.ViewModels;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class TransactionHistoryView : UserControl
    {
        public TransactionHistoryView()
        {
            InitializeComponent();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is TransactionRecord record)
            {
                if (DataContext is TransactionHistoryViewModel vm)
                {
                    vm.OpenDetailCommand.Execute(record);
                }
            }
        }

        private void DataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject dep)
            {
                while (dep != null && dep is not DataGridRow)
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                if (dep is DataGridRow row && row.Item is TransactionRecord record)
                {
                    row.IsSelected = true;
                    if (DataContext is TransactionHistoryViewModel vm)
                    {
                        vm.SelectedTransaction = record;
                    }
                }
            }
        }

        private void MenuItem_OpenDetail_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TransactionHistoryViewModel vm && vm.SelectedTransaction != null)
            {
                vm.OpenDetailCommand.Execute(vm.SelectedTransaction);
            }
        }
    }
}
