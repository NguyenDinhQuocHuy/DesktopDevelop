using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QuanLyDichVuKhachSan.Helpers;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        public DataService Data => DataService.Instance;
        public AuthService Auth => AuthService.Instance;

        public bool IsAdmin => Auth.IsAdmin;

        // Quản lý Tab điều hướng
        private string _activeTab = "Receipt"; // "Receipt", "Catalog", "History", "ProfitReport"
        public string ActiveTab
        {
            get => _activeTab;
            set => SetProperty(ref _activeTab, value);
        }

        // =======================================================================
        // A. QUẢN LÝ DANH MỤC MẶT HÀNG (CATALOG) - Debounced 180ms
        // =======================================================================
        private string _catalogSearchText = "";
        public string CatalogSearchText
        {
            get => _catalogSearchText;
            set
            {
                if (SetProperty(ref _catalogSearchText, value))
                {
                    Debounce("CatalogSearch", ApplyCatalogFilter, 180);
                }
            }
        }

        public ObservableRangeCollection<FoodItem> FilteredCatalogItems { get; } = new();

        // Form thêm mặt hàng mới
        private string _newItemName = "";
        public string NewItemName
        {
            get => _newItemName;
            set => SetProperty(ref _newItemName, value);
        }

        private decimal _newItemPrice = 25000;
        public decimal NewItemPrice
        {
            get => _newItemPrice;
            set => SetProperty(ref _newItemPrice, value);
        }

        private string _newItemRetailUnit = "Lon";
        public string NewItemRetailUnit
        {
            get => _newItemRetailUnit;
            set => SetProperty(ref _newItemRetailUnit, value);
        }

        private string _newItemDefaultImportUnit = "Thùng";
        public string NewItemDefaultImportUnit
        {
            get => _newItemDefaultImportUnit;
            set => SetProperty(ref _newItemDefaultImportUnit, value);
        }

        private int _newItemDefaultConversionRate = 24;
        public int NewItemDefaultConversionRate
        {
            get => _newItemDefaultConversionRate;
            set => SetProperty(ref _newItemDefaultConversionRate, Math.Max(1, value));
        }

        private string _newItemCategory = "Đồ ăn";
        public string NewItemCategory
        {
            get => _newItemCategory;
            set => SetProperty(ref _newItemCategory, value);
        }

        public List<string> AvailableCategories { get; } = new() { "Đồ ăn", "Thức uống" };

        private int _newItemLowStockThreshold = 10;
        public int NewItemLowStockThreshold
        {
            get => _newItemLowStockThreshold;
            set => SetProperty(ref _newItemLowStockThreshold, Math.Max(0, value));
        }

        // =======================================================================
        // B. LẬP PHIẾU NHẬP KHO (MULTI-ITEM RECEIPT)
        // =======================================================================
        private string _receiptCode = "";
        public string ReceiptCode
        {
            get => _receiptCode;
            set => SetProperty(ref _receiptCode, value);
        }

        public string GenerateUniqueReceiptCode()
        {
            string datePrefix = $"PN-{DateTime.Now:yyyyMMdd}-";
            int maxIndex = 0;
            foreach (var b in Data.InventoryBatches)
            {
                if (!string.IsNullOrWhiteSpace(b.ReceiptCode) && b.ReceiptCode.StartsWith(datePrefix))
                {
                    string suffix = b.ReceiptCode.Substring(datePrefix.Length);
                    if (int.TryParse(suffix, out int idx))
                    {
                        if (idx > maxIndex) maxIndex = idx;
                    }
                }
            }
            return $"{datePrefix}{maxIndex + 1:D2}";
        }

        private Supplier? _selectedSupplier;
        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);
        }

        private string _receiptNote = "";
        public string ReceiptNote
        {
            get => _receiptNote;
            set => SetProperty(ref _receiptNote, value);
        }

        // Dòng hàng đang soạn
        private FoodItem? _selectedItemToImport;
        public FoodItem? SelectedItemToImport
        {
            get => _selectedItemToImport;
            set
            {
                if (SetProperty(ref _selectedItemToImport, value))
                {
                    AutoFillItemReference(value);
                    OnPropertyChanged(nameof(ReferenceSpecAndPrice));
                }
            }
        }

        public string ReferenceSpecAndPrice
        {
            get
            {
                if (SelectedItemToImport == null) return "";
                string n = SelectedItemToImport.Name.ToLowerInvariant();
                if (n.Contains("vang") || n.Contains("rượu"))
                    return "💡 Rượu Vang: Thùng 6 chai 750ml (~480k-540k/thùng, ~85k/chai). Bán lẻ: 100k-120k/chai";
                if (n.Contains("dâu") || n.Contains("mứt"))
                    return "💡 Mứt Dâu: Lốc 6 hũ (~225k-240k/lốc, ~37.5k/hũ). Bán lẻ: 50k-60k/hũ";
                if (n.Contains("lay") || n.Contains("snack") || n.Contains("khoai tây"))
                    return "💡 Snack Lay's: Thùng 40 gói (~350k-380k/thùng, ~9k/gói). Bán lẻ: 12k-15k/gói";
                if (n.Contains("g7") || n.Contains("cà phê") || n.Contains("cafe"))
                    return "💡 Cà Phê G7: Thùng 24 hộp (~1.100k-1.150k/thùng, ~46k/hộp). Bán lẻ: 55k-60k/hộp";
                if (n.Contains("atiso") || n.Contains("trà"))
                    return "💡 Trà Atiso: Thùng 50 hộp (~1.600k/thùng, ~32k/hộp). Bán lẻ: 40k-50k/hộp";
                if (n.Contains("omachi") || n.Contains("mì"))
                    return "💡 Mì Ly Omachi: Thùng 24 ly (~330k-350k/thùng, ~14k/ly). Bán lẻ: 17k-18k/ly";
                if (n.Contains("coca") || n.Contains("nước ngọt"))
                    return "💡 Coca Cola: Thùng 24 lon (~215k-225k/thùng, ~9k/lon). Bán lẻ: 12k/lon";
                if (n.Contains("lavie") || n.Contains("nước khoáng") || n.Contains("suối"))
                    return "💡 Nước Suối Lavie: Thùng 24 chai (~90k-95k/thùng, ~3.8k/chai). Bán lẻ: 6k-7k/chai";
                return $"💡 ĐVT Nhập Mặc Định: {SelectedItemToImport.DefaultImportUnit}, Quy đổi: 1 {SelectedItemToImport.DefaultImportUnit} = {SelectedItemToImport.DefaultConversionRate} {SelectedItemToImport.RetailUnit}";
            }
        }

        private void AutoFillItemReference(FoodItem? item)
        {
            if (item == null) return;
            ImportUnit = !string.IsNullOrWhiteSpace(item.DefaultImportUnit) ? item.DefaultImportUnit : "Thùng";
            ConversionRate = item.DefaultConversionRate > 0 ? item.DefaultConversionRate : 24;

            string n = item.Name.ToLowerInvariant();
            if (n.Contains("vang") || n.Contains("rượu")) { ImportPrice = 510000; }
            else if (n.Contains("dâu") || n.Contains("mứt")) { ImportPrice = 225000; }
            else if (n.Contains("lay") || n.Contains("snack") || n.Contains("khoai tây")) { ImportPrice = 360000; }
            else if (n.Contains("g7") || n.Contains("cà phê") || n.Contains("cafe")) { ImportPrice = 1120000; }
            else if (n.Contains("atiso") || n.Contains("trà")) { ImportPrice = 1600000; }
            else if (n.Contains("omachi") || n.Contains("mì")) { ImportPrice = 340000; }
            else if (n.Contains("coca") || n.Contains("nước ngọt")) { ImportPrice = 220000; }
            else if (n.Contains("lavie") || n.Contains("nước khoáng") || n.Contains("suối")) { ImportPrice = 92000; }
            else { ImportPrice = item.Price * ConversionRate * 0.7m; }
            IsUnitConversionEditable = false;
        }

        private bool _isUnitConversionEditable = false;
        public bool IsUnitConversionEditable
        {
            get => _isUnitConversionEditable;
            set => SetProperty(ref _isUnitConversionEditable, value);
        }

        private string _importUnit = "Thùng";
        public string ImportUnit
        {
            get => _importUnit;
            set
            {
                if (SetProperty(ref _importUnit, value))
                {
                    OnPropertyChanged(nameof(TotalRetailUnitsDisplay));
                    OnPropertyChanged(nameof(CostPerRetailUnitDisplay));
                }
            }
        }

        private int _conversionRate = 24;
        public int ConversionRate
        {
            get => _conversionRate;
            set
            {
                if (SetProperty(ref _conversionRate, Math.Max(1, value)))
                {
                    OnPropertyChanged(nameof(TotalRetailUnits));
                    OnPropertyChanged(nameof(TotalRetailUnitsDisplay));
                    OnPropertyChanged(nameof(CostPerRetailUnitDisplay));
                }
            }
        }

        private int _importQuantity = 5;
        public int ImportQuantity
        {
            get => _importQuantity;
            set
            {
                if (SetProperty(ref _importQuantity, Math.Max(1, value)))
                {
                    OnPropertyChanged(nameof(TotalRetailUnits));
                    OnPropertyChanged(nameof(TotalRetailUnitsDisplay));
                    OnPropertyChanged(nameof(TotalImportCost));
                    OnPropertyChanged(nameof(TotalImportCostDisplay));
                }
            }
        }

        private decimal _importPrice = 240000;
        public decimal ImportPrice
        {
            get => _importPrice;
            set
            {
                if (SetProperty(ref _importPrice, Math.Max(0, value)))
                {
                    OnPropertyChanged(nameof(TotalImportCost));
                    OnPropertyChanged(nameof(TotalImportCostDisplay));
                    OnPropertyChanged(nameof(CostPerRetailUnitDisplay));
                }
            }
        }

        private string _batchNumber = $"LO-{DateTime.Now:MMdd}-A";
        public string BatchNumber
        {
            get => _batchNumber;
            set => SetProperty(ref _batchNumber, value);
        }

        private DateTime? _expiryDate = DateTime.Today.AddMonths(6);
        public DateTime? ExpiryDate
        {
            get => _expiryDate;
            set => SetProperty(ref _expiryDate, value);
        }

        private string _lineNote = "";
        public string LineNote
        {
            get => _lineNote;
            set => SetProperty(ref _lineNote, value);
        }

        public int TotalRetailUnits => ImportQuantity * ConversionRate;
        public string TotalRetailUnitsDisplay => $"{ImportQuantity} {ImportUnit} x {ConversionRate} = {TotalRetailUnits} {SelectedItemToImport?.RetailUnit ?? "đơn vị lẻ"}";
        public decimal TotalImportCost => ImportQuantity * ImportPrice;
        public string TotalImportCostDisplay => $"{TotalImportCost:N0} đ";
        public string CostPerRetailUnitDisplay
        {
            get
            {
                if (ConversionRate <= 0) return "0 đ/đơn vị";
                decimal cost = ImportPrice / ConversionRate;
                return $"{cost:N0} đ / {SelectedItemToImport?.RetailUnit ?? "đơn vị lẻ"}";
            }
        }

        // Danh sách các dòng hàng đang lập trong phiếu nhập
        public ObservableCollection<WarehouseReceiptDraftItem> DraftReceiptItems { get; } = new();

        public int DraftItemsCount => DraftReceiptItems.Count;
        public int DraftTotalRetailUnits => DraftReceiptItems.Sum(x => x.TotalRetailUnits);
        public decimal DraftTotalCost => DraftReceiptItems.Sum(x => x.TotalCost);

        // =======================================================================
        // C. BÁO CÁO LỢI NHUẬN BÁN HÀNG KHO (PROFIT REPORT)
        // =======================================================================
        private DateTime _reportFromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public DateTime ReportFromDate
        {
            get => _reportFromDate;
            set
            {
                if (SetProperty(ref _reportFromDate, value))
                {
                    _ = LoadProfitReportAsync();
                }
            }
        }

        private DateTime _reportToDate = DateTime.Today;
        public DateTime ReportToDate
        {
            get => _reportToDate;
            set
            {
                if (SetProperty(ref _reportToDate, value))
                {
                    _ = LoadProfitReportAsync();
                }
            }
        }

        public ObservableCollection<InventoryProfitReportItem> ProfitReportItems { get; } = new();

        public decimal TotalReportRevenue => ProfitReportItems.Sum(x => x.Revenue);
        public decimal TotalReportCost => ProfitReportItems.Sum(x => x.TotalCost);
        public decimal TotalReportGrossProfit => ProfitReportItems.Sum(x => x.GrossProfit);
        public double AverageReportProfitMargin => TotalReportRevenue > 0 ? (double)(TotalReportGrossProfit / TotalReportRevenue * 100) : 0;

        // KPI Thống kê
        public int TotalDryItemsCount => Data.FoodItems.Count;
        public int TotalStockUnits => Data.FoodItems.Sum(x => x.StockQuantity);
        public decimal TotalStockValue => Data.FoodItems.Sum(x => x.StockQuantity * x.Price);
        public int NearExpiryBatchesCount => Data.InventoryBatches.Count(x => x.IsNearExpiry);
        public int LowStockItemsCount => Data.FoodItems.Count(x => x.IsLowStock);
        public string AutoGeneratedNewItemCode => $"MH{((Data.FoodItems.Count > 0 ? Data.FoodItems.Max(x => x.Id) : 0) + 1):D3}";

        // Commands
        public ICommand SetTabCommand { get; }
        public ICommand AddDraftItemCommand { get; }
        public ICommand RemoveDraftItemCommand { get; }
        public ICommand CommitReceiptCommand { get; }
        public ICommand AddNewFoodItemCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LoadReportCommand { get; }
        public ICommand ToggleUnitConversionEditCommand { get; }
        public ICommand EditFoodItemCommand { get; }
        public ICommand TrashFoodItemCommand { get; }
        public ICommand RestoreFoodItemCommand { get; }
        public ICommand PermanentDeleteFoodItemCommand { get; }

        // Thùng rác (Liên kết trực tiếp với DataService để lưu trữ bền vững)
        public ObservableCollection<FoodItem> TrashedItems => Data.TrashedFoodItems;
        public int TrashedItemsCount => TrashedItems.Count;
        public bool HasTrashedItems => TrashedItems.Count > 0;

        public InventoryViewModel()
        {
            SetTabCommand = new RelayCommand(p => { if (p is string tab) ActiveTab = tab; });
            AddDraftItemCommand = new RelayCommand(_ => AddDraftItem());
            RemoveDraftItemCommand = new RelayCommand(RemoveDraftItem);
            CommitReceiptCommand = new RelayCommand(_ => CommitReceipt());
            AddNewFoodItemCommand = new RelayCommand(_ => AddNewFoodItem());
            RefreshCommand = new RelayCommand(_ => RefreshAll());
            LoadReportCommand = new RelayCommand(_ => _ = LoadProfitReportAsync());
            ToggleUnitConversionEditCommand = new RelayCommand(_ => IsUnitConversionEditable = !IsUnitConversionEditable);
            EditFoodItemCommand = new RelayCommand(p =>
            {
                if (p is FoodItem item)
                {
                    var dlg = new Views.EditFoodItemDialogWindow(item);
                    if (dlg.ShowDialog() == true)
                    {
                        RefreshAll();
                        if (SelectedItemToImport?.Id == item.Id)
                        {
                            AutoFillItemReference(item);
                        }
                    }
                }
            });
            TrashFoodItemCommand = new RelayCommand(p => TrashFoodItem(p));
            RestoreFoodItemCommand = new RelayCommand(p => RestoreFoodItem(p));
            PermanentDeleteFoodItemCommand = new RelayCommand(p => PermanentDeleteFoodItem(p));

            RefreshAll();
            ReceiptCode = GenerateUniqueReceiptCode();

            if (Data.Suppliers.Count > 0)
            {
                SelectedSupplier = Data.Suppliers.First();
            }
            SelectedItemToImport = Data.FoodItems.FirstOrDefault();
        }

        private void TrashFoodItem(object? parameter)
        {
            if (parameter is not FoodItem item) return;
            var result = MessageBox.Show(
                $"Bạn có muốn chuyển mặt hàng [{item.ItemCode}] {item.Name} vào Thùng rác?\n\n" +
                $"• Mặt hàng sẽ bị ẩn khỏi danh mục kho và thực đơn.\n" +
                $"• Có thể khôi phục trong vòng 3 tháng.\n" +
                $"• Sau 3 tháng sẽ tự động xóa vĩnh viễn.",
                "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            item.TrashedDate = DateTime.Now;
            item.Status = "Ngừng kinh doanh";
            Data.FoodItems.Remove(item);
            TrashedItems.Add(item);

            // Đánh dấu trong DB
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE MonAn SET TrangThai = N'Đã xóa' WHERE Id = @Id",
                new Microsoft.Data.SqlClient.SqlParameter("@Id", item.Id)
            );

            NotifyTrashChanged();
            RefreshAll();
        }

        private void RestoreFoodItem(object? parameter)
        {
            if (parameter is not FoodItem item) return;
            item.TrashedDate = null;
            item.Status = "Đang phục vụ";
            TrashedItems.Remove(item);
            Data.FoodItems.Add(item);

            // Khôi phục trong DB
            _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                "UPDATE MonAn SET TrangThai = N'Đang phục vụ' WHERE Id = @Id",
                new Microsoft.Data.SqlClient.SqlParameter("@Id", item.Id)
            );

            NotifyTrashChanged();
            RefreshAll();

            MessageBox.Show($"✅ Đã khôi phục mặt hàng [{item.ItemCode}] {item.Name} về danh mục kho!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void PermanentDeleteFoodItem(object? parameter)
        {
            if (parameter is not FoodItem item) return;
            var result = MessageBox.Show(
                $"⚠️ XÓA VĨNH VIỄN mặt hàng [{item.ItemCode}] {item.Name}?\n\n" +
                $"Hành động này KHÔNG THỂ hoàn tác!",
                "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            TrashedItems.Remove(item);

            // Xóa vĩnh viễn trong DB
            await DatabaseService.Instance.ExecuteNonQueryAsync(
                "DELETE FROM MonAn WHERE Id = @Id",
                new Microsoft.Data.SqlClient.SqlParameter("@Id", item.Id)
            );

            NotifyTrashChanged();
            RefreshAll();
        }

        private void NotifyTrashChanged()
        {
            OnPropertyChanged(nameof(TrashedItems));
            OnPropertyChanged(nameof(TrashedItemsCount));
            OnPropertyChanged(nameof(HasTrashedItems));
        }

        public void RefreshAll()
        {
            // Auto-clean: Xóa vĩnh viễn các mặt hàng trong thùng rác quá 3 tháng
            var expiredTrash = TrashedItems.Where(x => x.TrashedDate.HasValue && x.TrashedDate.Value.AddMonths(3) < DateTime.Now).ToList();
            foreach (var expired in expiredTrash)
            {
                TrashedItems.Remove(expired);
                _ = DatabaseService.Instance.ExecuteNonQueryAsync(
                    "DELETE FROM MonAn WHERE Id = @Id",
                    new Microsoft.Data.SqlClient.SqlParameter("@Id", expired.Id)
                );
            }
            if (expiredTrash.Count > 0) NotifyTrashChanged();

            ApplyCatalogFilter();
            OnPropertyChanged(nameof(TotalDryItemsCount));
            OnPropertyChanged(nameof(TotalStockUnits));
            OnPropertyChanged(nameof(TotalStockValue));
            OnPropertyChanged(nameof(NearExpiryBatchesCount));
            OnPropertyChanged(nameof(LowStockItemsCount));
        }

        public void ApplyCatalogFilter()
        {
            var q = Data.FoodItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(CatalogSearchText))
            {
                string raw = CatalogSearchText.Trim();
                string clean = TextSearchHelper.ToSearchKey(raw);
                q = q.Where(x => TextSearchHelper.MatchPrepared(x.Name, clean) || TextSearchHelper.MatchPrepared(x.RetailUnit, clean));
            }

            var result = q.OrderBy(x => x.IsLowStock ? 0 : 1).ThenBy(x => x.Id).ToList();
            FilteredCatalogItems.ReplaceRange(result);
        }

        private void AddDraftItem()
        {
            if (SelectedItemToImport == null)
            {
                MessageBox.Show("Vui lòng chọn mặt hàng cần nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ImportQuantity <= 0 || ConversionRate <= 0)
            {
                MessageBox.Show("Số lượng nhập và hệ số quy đổi phải lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ExpiryDate.HasValue && ExpiryDate.Value.Date <= DateTime.Today)
            {
                MessageBox.Show("Hạn sử dụng của lô hàng phải sau ngày hôm nay (từ ngày mai trở đi)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var draftItem = new WarehouseReceiptDraftItem
            {
                FoodItemId = SelectedItemToImport.Id,
                FoodItemName = SelectedItemToImport.Name,
                RetailUnit = SelectedItemToImport.RetailUnit,
                ImportUnit = ImportUnit.Trim(),
                ConversionRate = ConversionRate,
                Quantity = ImportQuantity,
                ImportPrice = ImportPrice,
                BatchNumber = string.IsNullOrWhiteSpace(BatchNumber) ? $"LO-{DateTime.Now:MMdd}-{(char)('A' + (DraftReceiptItems.Count % 26))}" : BatchNumber.Trim(),
                ExpiryDate = ExpiryDate,
                Note = LineNote.Trim()
            };

            DraftReceiptItems.Add(draftItem);
            OnPropertyChanged(nameof(DraftItemsCount));
            OnPropertyChanged(nameof(DraftTotalRetailUnits));
            OnPropertyChanged(nameof(DraftTotalCost));

            // Chuẩn bị cho dòng hàng tiếp theo
            BatchNumber = $"LO-{DateTime.Now:MMdd}-{(char)('A' + (DraftReceiptItems.Count % 26))}";
            LineNote = "";
        }

        private void RemoveDraftItem(object? parameter)
        {
            if (parameter is WarehouseReceiptDraftItem item)
            {
                DraftReceiptItems.Remove(item);
                OnPropertyChanged(nameof(DraftItemsCount));
                OnPropertyChanged(nameof(DraftTotalRetailUnits));
                OnPropertyChanged(nameof(DraftTotalCost));
            }
        }

        private async void CommitReceipt()
        {
            if (SelectedSupplier == null)
            {
                MessageBox.Show("Vui lòng chọn Nhà Cung Cấp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DraftReceiptItems.Count == 0)
            {
                MessageBox.Show("Phiếu nhập chưa có dòng hàng nào! Vui lòng bấm '+ Thêm Dòng Hàng Vào Phiếu' trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int staffId = Data.GetCurrentDutyStaffId();
            if (staffId <= 0) staffId = 1;

            string code = string.IsNullOrWhiteSpace(ReceiptCode) ? $"PN-{DateTime.Now:yyyyMMdd}-{Data.InventoryBatches.Count + 1:D2}" : ReceiptCode.Trim();

            // Gọi Stored Procedure: sp_LapPhieuNhapKho
            int receiptId = await DatabaseService.Instance.LapPhieuNhapKhoAsync(code, SelectedSupplier.Id, staffId, ReceiptNote, DraftReceiptItems.ToList());

            if (receiptId > 0)
            {
                // Thêm vào lịch sử hiển thị
                foreach (var draft in DraftReceiptItems)
                {
                    var batch = new InventoryBatch
                    {
                        ReceiptId = receiptId,
                        ReceiptCode = code,
                        FoodItemId = draft.FoodItemId,
                        FoodItemName = draft.FoodItemName,
                        RetailUnit = draft.RetailUnit,
                        ImportUnit = draft.ImportUnit,
                        ConversionRate = draft.ConversionRate,
                        Quantity = draft.Quantity,
                        ImportPrice = draft.ImportPrice,
                        BatchNumber = draft.BatchNumber,
                        ExpiryDate = draft.ExpiryDate,
                        ImportDate = DateTime.Now,
                        ImportedBy = Auth.CurrentUser?.FullName ?? "Admin",
                        Supplier = SelectedSupplier.Name,
                        Note = draft.Note
                    };
                    Data.InventoryBatches.Insert(0, batch);

                    var food = Data.FoodItems.FirstOrDefault(x => x.Id == draft.FoodItemId);
                    if (food != null)
                    {
                        // Tính lại giá vốn bình quân gia quyền & tồn kho
                        int tonCu = food.StockQuantity;
                        decimal giaVonCu = food.AverageCostPrice;
                        int soLeNhap = draft.TotalRetailUnits;
                        decimal giaVonMoiLe = draft.CostPerRetailUnit;

                        decimal tongTienCu = tonCu * giaVonCu;
                        decimal tongTienMoi = soLeNhap * giaVonMoiLe;
                        int tongTonMoi = tonCu + soLeNhap;

                        food.AverageCostPrice = tongTonMoi > 0 ? (tongTienCu + tongTienMoi) / tongTonMoi : giaVonMoiLe;
                        food.StockQuantity = tongTonMoi;
                        food.Status = "Đang phục vụ";
                        food.NotifyStockChanged();
                    }
                }

                MessageBox.Show(
                    $"✅ ĐÃ LẬP PHIẾU NHẬP KHO THÀNH CÔNG!\n\n" +
                    $"• Mã phiếu: {code}\n" +
                    $"• Nhà cung cấp: {SelectedSupplier.Name}\n" +
                    $"• Tổng số mặt hàng: {DraftReceiptItems.Count} loại\n" +
                    $"• Tổng tồn lẻ cộng thêm: +{DraftTotalRetailUnits} đơn vị\n" +
                    $"• Tổng tiền hóa đơn: {DraftTotalCost:N0} đ",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                DraftReceiptItems.Clear();
                OnPropertyChanged(nameof(DraftItemsCount));
                OnPropertyChanged(nameof(DraftTotalRetailUnits));
                OnPropertyChanged(nameof(DraftTotalCost));

                ReceiptCode = GenerateUniqueReceiptCode();
                ReceiptNote = "";
                RefreshAll();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi lưu phiếu nhập kho xuống cơ sở dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddNewFoodItem()
        {
            if (string.IsNullOrWhiteSpace(NewItemName))
            {
                MessageBox.Show("Vui lòng nhập tên mặt hàng mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewItemDefaultImportUnit) || NewItemDefaultConversionRate <= 0)
            {
                MessageBox.Show("Vui lòng nhập ĐVT nhập mặc định và Hệ số quy đổi lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = new FoodItem
            {
                Name = NewItemName.Trim(),
                Category = string.IsNullOrWhiteSpace(NewItemCategory) ? "Đồ ăn" : NewItemCategory.Trim(),
                Price = NewItemPrice,
                RetailUnit = string.IsNullOrWhiteSpace(NewItemRetailUnit) ? "Cái" : NewItemRetailUnit.Trim(),
                DefaultImportUnit = NewItemDefaultImportUnit.Trim(),
                DefaultConversionRate = NewItemDefaultConversionRate,
                AverageCostPrice = 0,
                StockQuantity = 0, // Tồn ban đầu luôn = 0
                LowStockThreshold = NewItemLowStockThreshold,
                Status = "Đang phục vụ"
            };

            int newId = await DatabaseService.Instance.AddNewFoodItemAsync(item);
            if (newId > 0)
            {
                item.Id = newId;
                Data.FoodItems.Add(item);
                RefreshAll();
                SelectedItemToImport = item;

                MessageBox.Show(
                    $"✅ Đã thêm mặt hàng mới: [{item.ItemCode}] {item.Name}!\n" +
                    $"• Mã mặt hàng tự sinh: {item.ItemCode}\n" +
                    $"• Phân loại: {item.Category} | Đơn vị bán lẻ: {item.RetailUnit}\n" +
                    $"• Giá bán lẻ: {item.Price:N0} đ\n" +
                    $"• ĐVT nhập mặc định: 1 {item.DefaultImportUnit} = {item.DefaultConversionRate} {item.RetailUnit}\n" +
                    $"• Trạng thái phục vụ: Còn hàng (Đang phục vụ)\n" +
                    $"• Tồn kho khởi tạo = 0 (Để nhập hàng, hãy lập Phiếu Nhập Kho ở tab bên)",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                NewItemName = "";
            }
            else
            {
                MessageBox.Show("Không thể thêm mặt hàng vào cơ sở dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task LoadProfitReportAsync()
        {
            var reports = await DatabaseService.Instance.GetInventoryProfitReportAsync(ReportFromDate, ReportToDate);
            ProfitReportItems.Clear();
            foreach (var r in reports)
            {
                ProfitReportItems.Add(r);
            }

            OnPropertyChanged(nameof(TotalReportRevenue));
            OnPropertyChanged(nameof(TotalReportCost));
            OnPropertyChanged(nameof(TotalReportGrossProfit));
            OnPropertyChanged(nameof(AverageReportProfitMargin));
        }
    }
}
