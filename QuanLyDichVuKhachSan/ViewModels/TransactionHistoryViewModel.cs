using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;
using QuanLyDichVuKhachSan.Views;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class TransactionHistoryViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;
        public AuthService Auth => AuthService.Instance;

        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    Debounce("TransactionSearch", ApplyFilter, 180);
                }
            }
        }

        private string _selectedCategoryFilter = "Tất cả";
        public string SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set
            {
                if (SetProperty(ref _selectedCategoryFilter, value))
                {
                    ApplyFilter();
                }
            }
        }

        public List<string> CategoryFilters { get; } = new()
        {
            "Tất cả",
            "Tiền phòng & Dịch vụ",
            "Ăn uống",
            "Sảnh & Sự kiện",
            "Thuê xe máy",
            "Bãi gửi xe",
            "Giặt ủi"
        };

        private DateTime _fromDate = DateTime.Today.AddDays(-7);
        public DateTime FromDate
        {
            get => _fromDate;
            set
            {
                if (SetProperty(ref _fromDate, value))
                {
                    ApplyFilter();
                }
            }
        }

        private DateTime _toDate = DateTime.Today;
        public DateTime ToDate
        {
            get => _toDate;
            set
            {
                if (SetProperty(ref _toDate, value))
                {
                    ApplyFilter();
                }
            }
        }

        private TransactionRecord? _selectedTransaction;
        public TransactionRecord? SelectedTransaction
        {
            get => _selectedTransaction;
            set => SetProperty(ref _selectedTransaction, value);
        }

        public ObservableRangeCollection<TransactionRecord> FilteredTransactions { get; } = new();

        public int TotalCount => FilteredTransactions.Count;
        public decimal TotalAmountSum => FilteredTransactions.Sum(x => x.Amount);
        public string CurrentDutyStaffDisplay => Auth.IsAdmin ? "Chủ khách sạn (Admin)" : Data.GetCurrentDutyStaffName();

        public ICommand RefreshCommand { get; }
        public ICommand OpenDetailCommand { get; }
        public ICommand ResetDateCommand { get; }

        public TransactionHistoryViewModel()
        {
            RefreshCommand = new RelayCommand(_ => Refresh());
            OpenDetailCommand = new RelayCommand(p => OpenDetail(p));
            ResetDateCommand = new RelayCommand(_ =>
            {
                FromDate = DateTime.Today.AddDays(-30);
                ToDate = DateTime.Today;
            });

            Refresh();
        }

        public void Refresh()
        {
            ApplyFilter();
        }

        public void ApplyFilter()
        {
            var all = Data.GetAllTransactions();
            string rawQ = (SearchQuery ?? "").Trim();
            string cleanQ = TextSearchHelper.ToSearchKey(rawQ);

            DateTime start = FromDate.Date;
            DateTime end = ToDate.Date.AddDays(1).AddTicks(-1);

            var filteredList = new List<TransactionRecord>();

            foreach (var t in all)
            {
                // Lọc ngày
                if (t.PaymentTime < start || t.PaymentTime > end)
                {
                    continue;
                }

                // Lọc loại dịch vụ
                if (SelectedCategoryFilter != "Tất cả" && !t.ServiceCategory.Contains(SelectedCategoryFilter))
                {
                    continue;
                }

                // Lọc từ khóa tìm kiếm siêu tốc với TextSearchHelper
                if (!string.IsNullOrWhiteSpace(cleanQ))
                {
                    bool matchCode = (!string.IsNullOrWhiteSpace(t.TransactionCode) && t.TransactionCode.IndexOf(rawQ, StringComparison.OrdinalIgnoreCase) >= 0);
                    bool matchCustomer = TextSearchHelper.MatchPrepared(t.CustomerName, cleanQ);
                    bool matchRoom = TextSearchHelper.MatchPrepared(t.RoomNumber, cleanQ);
                    bool matchStaff = TextSearchHelper.MatchPrepared(t.PaidBy, cleanQ) || TextSearchHelper.MatchPrepared(t.CreatedBy, cleanQ);

                    if (!matchCode && !matchCustomer && !matchRoom && !matchStaff)
                    {
                        continue;
                    }
                }

                filteredList.Add(t);
            }

            FilteredTransactions.ReplaceRange(filteredList);

            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(TotalAmountSum));
        }

        private void OpenDetail(object? parameter)
        {
            var target = parameter as TransactionRecord ?? SelectedTransaction;
            if (target == null)
            {
                MessageBox.Show("Vui lòng chọn một giao dịch để xem chi tiết hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new InvoiceBillWindow(target);
            win.ShowDialog();
        }
    }
}
