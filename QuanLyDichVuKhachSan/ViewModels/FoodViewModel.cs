using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class FoodViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;
        public AuthService Auth => AuthService.Instance;

        public bool IsAdmin => Auth.IsAdmin;
        public bool IsReceptionist => Auth.IsReceptionist;

        // Quản lý tìm kiếm mặt hàng realtime (Debounced 180ms)
        private string _searchFoodQuery = "";
        public string SearchFoodQuery
        {
            get => _searchFoodQuery;
            set
            {
                if (SetProperty(ref _searchFoodQuery, value))
                {
                    Debounce("FoodSearch", ApplyFoodFilter, 180);
                }
            }
        }

        public ObservableRangeCollection<FoodItem> FilteredFoodItems { get; } = new();
        public ObservableRangeCollection<FoodItem> FilteredFoods { get; } = new();
        public ObservableRangeCollection<FoodItem> FilteredDrinks { get; } = new();

        public int FoodCount => FilteredFoods.Count;
        public int DrinkCount => FilteredDrinks.Count;

        // Quản lý Order mới (Lễ tân / Mini-bar)
        private string _orderCustomerName = "";
        public string OrderCustomerName
        {
            get => _orderCustomerName;
            set => SetProperty(ref _orderCustomerName, value);
        }

        private string _orderRoomNumber = "";
        public string OrderRoomNumber
        {
            get => _orderRoomNumber;
            set
            {
                if (SetProperty(ref _orderRoomNumber, value))
                {
                    AutoFillCustomerInfo(value);
                    OnPropertyChanged(nameof(CanChooseRoomBill));
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        OrderPaymentType = FoodPaymentType.RoomBill;
                    }
                    else
                    {
                        OrderPaymentType = FoodPaymentType.SeparateBill;
                    }
                }
            }
        }

        public bool CanChooseRoomBill => !string.IsNullOrWhiteSpace(OrderRoomNumber);

        private void AutoFillCustomerInfo(string roomNumber)
        {
            string norm = DataService.NormalizeRoomNumber(roomNumber);
            if (!string.IsNullOrEmpty(norm))
            {
                var room = Data.HotelRooms.FirstOrDefault(r => 
                    DataService.NormalizeRoomNumber(r.RoomNumber) == norm ||
                    string.Equals(r.RoomNumber, norm, StringComparison.OrdinalIgnoreCase));
                if (room != null && !string.IsNullOrWhiteSpace(room.CustomerName))
                {
                    OrderCustomerName = room.CustomerName;
                }
                else
                {
                    OrderCustomerName = "";
                }
            }
            else
            {
                OrderCustomerName = "";
            }
        }

        private FoodPaymentType _orderPaymentType = FoodPaymentType.DirectPayment;
        public FoodPaymentType OrderPaymentType
        {
            get => _orderPaymentType;
            set
            {
                if (SetProperty(ref _orderPaymentType, value))
                {
                    OnPropertyChanged(nameof(IsDirectPayment));
                }
            }
        }

        public bool IsDirectPayment => OrderPaymentType == FoodPaymentType.DirectPayment;

        private string _paymentMethod = "Tiền mặt";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set
            {
                if (SetProperty(ref _paymentMethod, value))
                {
                    OnPropertyChanged(nameof(IsCashPayment));
                    OnPropertyChanged(nameof(IsBankTransferPayment));
                }
            }
        }

        public bool IsCashPayment
        {
            get => PaymentMethod == "Tiền mặt";
            set { if (value) PaymentMethod = "Tiền mặt"; }
        }

        public bool IsBankTransferPayment
        {
            get => PaymentMethod == "Chuyển khoản";
            set { if (value) PaymentMethod = "Chuyển khoản"; }
        }

        private string _orderNote = "";
        public string OrderNote
        {
            get => _orderNote;
            set => SetProperty(ref _orderNote, value);
        }

        public ObservableCollection<FoodOrderItem> CurrentCartItems { get; } = new();

        public decimal CartTotal => CurrentCartItems.Sum(x => x.TotalPrice);

        // Commands
        public ICommand AddToCartCommand { get; }
        public ICommand QuickAddSingleCommand { get; }
        public ICommand IncreaseQtyCommand { get; }
        public ICommand DecreaseQtyCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand PrintBillCommand { get; }

        public FoodViewModel()
        {
            AddToCartCommand = new RelayCommand(AddToCart);
            QuickAddSingleCommand = new RelayCommand(QuickAddSingle);
            IncreaseQtyCommand = new RelayCommand(p => { if (p is FoodItem food) food.SelectedQty++; });
            DecreaseQtyCommand = new RelayCommand(p => { if (p is FoodItem food && food.SelectedQty > 1) food.SelectedQty--; });
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            PlaceOrderCommand = new RelayCommand(PlaceOrder);
            PrintBillCommand = new RelayCommand(PrintBill);

            ApplyFoodFilter();

            Data.FoodItems.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (FoodItem food in e.NewItems)
                    {
                        AttachFoodItemListeners(food);
                    }
                }
                ApplyFoodFilter();
            };

            foreach (var food in Data.FoodItems)
            {
                AttachFoodItemListeners(food);
            }
        }

        private void AttachFoodItemListeners(FoodItem food)
        {
            food.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(FoodItem.IsActive) || 
                    e.PropertyName == nameof(FoodItem.Status) || 
                    e.PropertyName == nameof(FoodItem.Name) ||
                    e.PropertyName == nameof(FoodItem.Price) ||
                    e.PropertyName == nameof(FoodItem.StockQuantity) ||
                    e.PropertyName == nameof(FoodItem.Category))
                {
                    ApplyFoodFilter();
                }
            };
        }

        public void ApplyFoodFilter()
        {
            string rawQ = (SearchFoodQuery ?? "").Trim();
            string cleanQ = TextSearchHelper.ToSearchKey(rawQ);

            // Lấy tất cả các món đang phục vụ
            var activeItems = Data.FoodItems.Where(x => x.IsActive).ToList();

            foreach (var item in activeItems)
            {
                bool matched = !string.IsNullOrWhiteSpace(cleanQ) && TextSearchHelper.MatchPrepared(item.Name, cleanQ);
                item.IsSearchMatched = matched;
            }

            // 1. Phân loại ĐỒ ĂN: Nếu có tra cứu, món khớp được đẩy lên đầu
            var foods = activeItems
                .Where(x => !x.IsDrink)
                .OrderByDescending(x => x.IsSearchMatched)
                .ThenBy(x => x.Id)
                .ToList();

            // 2. Phân loại THỨC UỐNG: Nếu có tra cứu, thức uống khớp được đẩy lên đầu
            var drinks = activeItems
                .Where(x => x.IsDrink)
                .OrderByDescending(x => x.IsSearchMatched)
                .ThenBy(x => x.Id)
                .ToList();

            var allItems = new List<FoodItem>(foods.Count + drinks.Count);
            allItems.AddRange(foods);
            allItems.AddRange(drinks);

            FilteredFoods.ReplaceRange(foods);
            FilteredDrinks.ReplaceRange(drinks);
            FilteredFoodItems.ReplaceRange(allItems);

            OnPropertyChanged(nameof(FoodCount));
            OnPropertyChanged(nameof(DrinkCount));
        }

        private void QuickAddSingle(object? parameter)
        {
            if (parameter is FoodItem food)
            {
                AddFoodWithQuantity(food, 1);
            }
        }

        private void AddToCart(object? parameter)
        {
            if (parameter is FoodItem food)
            {
                int qty = Math.Max(1, food.SelectedQty);
                AddFoodWithQuantity(food, qty);
                food.SelectedQty = 1;
            }
        }

        private void AddFoodWithQuantity(FoodItem food, int qty)
        {
            if (!food.IsAvailable)
            {
                MessageBox.Show($"Mặt hàng [{food.Name}] hiện không sẵn sàng phục vụ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = CurrentCartItems.FirstOrDefault(x => x.FoodItemId == food.Id);
            int currentInCart = existing != null ? existing.Quantity : 0;
            int requestedTotal = currentInCart + qty;

            if (requestedTotal > food.StockQuantity)
            {
                MessageBox.Show($"Không thể thêm {qty} {food.RetailUnit}. Tồn kho chỉ còn {food.StockQuantity} {food.RetailUnit}!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (existing != null)
            {
                existing.Quantity += qty;
            }
            else
            {
                var newItem = new FoodOrderItem
                {
                    FoodItemId = food.Id,
                    FoodItemName = food.Name,
                    RetailUnit = food.RetailUnit,
                    Price = food.Price,
                    CostPrice = food.AverageCostPrice,
                    Quantity = qty
                };
                newItem.PropertyChanged += (s, e) => OnPropertyChanged(nameof(CartTotal));
                CurrentCartItems.Add(newItem);
            }
            OnPropertyChanged(nameof(CartTotal));
        }

        private void RemoveFromCart(object? parameter)
        {
            if (parameter is FoodOrderItem item)
            {
                CurrentCartItems.Remove(item);
                OnPropertyChanged(nameof(CartTotal));
            }
        }

        private void PlaceOrder(object? parameter)
        {
            if (CurrentCartItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 mặt hàng vào giỏ hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(OrderCustomerName) && OrderCustomerName.Any(char.IsDigit))
            {
                MessageBox.Show("Họ & Tên khách hàng không được chứa chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string custName = string.IsNullOrWhiteSpace(OrderCustomerName) ? "Khách vãng lai" : OrderCustomerName.Trim();
            string roomNum = DataService.NormalizeRoomNumber(OrderRoomNumber);

            var order = new FoodOrder
            {
                CustomerName = custName,
                RoomNumber = roomNum,
                PaymentType = OrderPaymentType,
                PaymentMethod = IsDirectPayment ? PaymentMethod : "Ghi nợ vào phòng",
                TotalAmount = CartTotal,
                Note = OrderNote?.Trim() ?? "",
                RecordedBy = Auth.IsAdmin ? "Chủ khách sạn" : Data.GetCurrentDutyStaffName(DateTime.Now)
            };

            foreach (var item in CurrentCartItems)
            {
                order.Items.Add(new FoodOrderItem
                {
                    FoodItemId = item.FoodItemId,
                    FoodItemName = item.FoodItemName,
                    RetailUnit = item.RetailUnit,
                    Price = item.Price,
                    CostPrice = item.CostPrice,
                    Quantity = item.Quantity
                });
            }

            // Hiện hóa đơn trước
            var win = new Views.InvoiceBillWindow(order);
            bool? res = win.ShowDialog();

            // Chỉ khi bấm "Đóng & Hoàn Tất" (res == true && IsConfirmed == true) mới lưu xuống DB và xóa giỏ hàng
            if (res == true && win.IsConfirmed)
            {
                Data.AddFoodOrder(order);
                CurrentCartItems.Clear();
                OrderCustomerName = "";
                OrderRoomNumber = "";
                OrderNote = "";
                OnPropertyChanged(nameof(CartTotal));
                ApplyFoodFilter();
            }
            // Nếu bấm Hủy Bỏ hoặc dấu X: Giữ nguyên giỏ hàng để tiếp tục chỉnh sửa
        }

        private void PrintBill(object? parameter)
        {
            if (parameter is FoodOrder order)
            {
                var win = new Views.InvoiceBillWindow(order);
                win.ShowDialog();
            }
        }
    }
}
