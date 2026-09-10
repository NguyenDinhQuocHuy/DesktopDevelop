using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using QuanLyDichVuKhachSan.Models;
using QuanLyDichVuKhachSan.Services;

namespace QuanLyDichVuKhachSan.Views
{
    public partial class InvoiceBillWindow : Window
    {
        public class InvoiceDisplayItem
        {
            public string ServiceCategory { get; set; } = string.Empty;
            public string ServiceName { get; set; } = string.Empty;
            public DateTime UsedTime { get; set; } = DateTime.Now;
            public decimal Amount { get; set; }
        }

        private bool _isViewOnly = false;

        public void SetViewOnlyMode(bool isViewOnly = true)
        {
            _isViewOnly = isViewOnly;
            if (BtnPrint != null) BtnPrint.Visibility = isViewOnly ? Visibility.Collapsed : Visibility.Visible;
            if (BtnCancel != null) BtnCancel.Visibility = isViewOnly ? Visibility.Collapsed : Visibility.Visible;
            if (BtnConfirm != null)
            {
                BtnConfirm.Content = isViewOnly ? "✖ Đóng" : "✅ Đóng & Hoàn Tất";
                BtnConfirm.Width = isViewOnly ? 110 : 160;
                BtnConfirm.Background = isViewOnly 
                    ? new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#475569")) 
                    : new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#16A34A"));
            }
        }

        public InvoiceBillWindow(RoomInvoice invoice, bool isViewOnly = false)
        {
            InitializeComponent();
            LoadRoomInvoiceData(invoice);
            if (isViewOnly) SetViewOnlyMode(true);
        }

        public InvoiceBillWindow(TransactionRecord trans)
        {
            InitializeComponent();
            LoadTransactionRecordData(trans);
            SetViewOnlyMode(true); // Khi xem từ chi tiết giao dịch: chỉ hiện nội dung, ẩn in và hủy
        }

        public InvoiceBillWindow(RoomServiceUsageItem serviceItem, HotelRoom? room = null, bool isViewOnly = false)
        {
            InitializeComponent();
            LoadServiceItemData(serviceItem, room);
            if (isViewOnly) SetViewOnlyMode(true);
        }

        public InvoiceBillWindow(FoodOrder order, bool isViewOnly = false)
        {
            InitializeComponent();
            LoadFoodOrderData(order);
            if (isViewOnly) SetViewOnlyMode(true);
        }

        public InvoiceBillWindow(VehicleRental rental, bool isViewOnly = false)
        {
            InitializeComponent();
            LoadVehicleRentalData(rental);
            if (isViewOnly) SetViewOnlyMode(true);
        }

        public InvoiceBillWindow(EventBooking booking, bool isViewOnly = false)
        {
            InitializeComponent();
            LoadEventBookingData(booking);
            if (isViewOnly) SetViewOnlyMode(true);
        }

        public InvoiceBillWindow(LaundryOrder order, bool isViewOnly = false)
        {
            InitializeComponent();
            LoadLaundryOrderData(order);
            if (isViewOnly) SetViewOnlyMode(true);
        }

        public InvoiceBillWindow(ParkingRecord record, bool isViewOnly = false)
        {
            InitializeComponent();
            LoadParkingRecordData(record);
            if (isViewOnly) SetViewOnlyMode(true);
        }

        private void LoadTransactionRecordData(TransactionRecord trans)
        {
            if (trans == null) return;

            TxtInvoiceCode.Text = trans.TransactionCode;
            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(trans.CustomerName) ? trans.CustomerName : "Khách hàng";
            TxtPhoneAndCccd.Text = !string.IsNullOrWhiteSpace(trans.PhoneNumber) ? trans.PhoneNumber : trans.ServiceCategory;
            TxtRoomNumber.Text = trans.RoomDisplay;
            TxtInvoiceDate.Text = trans.PaymentTime.ToString("dd/MM/yyyy HH:mm");
            TxtCreatorStaff.Text = !string.IsNullOrWhiteSpace(trans.CreatedBy) ? trans.CreatedBy : "Chủ khách sạn";
            TxtCashierStaff.Text = !string.IsNullOrWhiteSpace(trans.PaidBy) ? trans.PaidBy : "Chủ khách sạn";
            TxtGrandTotal.Text = $"{trans.Amount:N0} đ";

            if (trans.OriginalObject is RoomInvoice roomInv && roomInv.PaidItems.Count > 0)
            {
                DgInvoiceItems.ItemsSource = roomInv.PaidItems.Select(x => new InvoiceDisplayItem
                {
                    ServiceCategory = x.ServiceCategory,
                    ServiceName = x.ServiceName,
                    UsedTime = x.UsedTime,
                    Amount = x.Amount
                }).ToList();
            }
            else if (trans.OriginalObject is FoodOrder foodOrder && foodOrder.Items.Count > 0)
            {
                DgInvoiceItems.ItemsSource = foodOrder.Items.Select(it => new InvoiceDisplayItem
                {
                    ServiceCategory = "Ẩm thực",
                    ServiceName = $"{it.Quantity}x {it.FoodItemName} ({it.Price:N0} đ/suất)",
                    UsedTime = foodOrder.CreatedAt,
                    Amount = it.TotalPrice
                }).ToList();
            }
            else if (trans.OriginalObject is VehicleRental vr)
            {
                int days = Math.Max(1, (int)(vr.ExpectedReturnDate - vr.RentalDate).TotalDays);
                var items = new List<InvoiceDisplayItem>
                {
                    new()
                    {
                        ServiceCategory = "Thuê xe máy",
                        ServiceName = $"Thuê xe {vr.VehicleName} (BS: {vr.LicensePlate}) - {days} ngày",
                        UsedTime = vr.RentalDate,
                        Amount = vr.RentalFee
                    }
                };
                if (vr.AdditionalCost > 0)
                {
                    items.Add(new InvoiceDisplayItem
                    {
                        ServiceCategory = "Thuê xe máy",
                        ServiceName = "Chi phí phát sinh / Phụ thu",
                        UsedTime = vr.ActualReturnDate ?? vr.RentalDate,
                        Amount = vr.AdditionalCost
                    });
                }
                DgInvoiceItems.ItemsSource = items;
            }
            else if (trans.OriginalObject is ParkingRecord pr)
            {
                DgInvoiceItems.ItemsSource = new List<InvoiceDisplayItem>
                {
                    new()
                    {
                        ServiceCategory = "Bãi đỗ xe",
                        ServiceName = $"Gửi xe {pr.VehicleType} (Biển số: {pr.LicensePlate})",
                        UsedTime = pr.CheckInTime,
                        Amount = pr.ParkingFee
                    }
                };
            }
            else if (trans.OriginalObject is LaundryOrder lo)
            {
                DgInvoiceItems.ItemsSource = new List<InvoiceDisplayItem>
                {
                    new()
                    {
                        ServiceCategory = "Giặt ủi",
                        ServiceName = $"{lo.ServiceTypeDisplay} ({lo.WeightKg} kg • {lo.PartnerName})",
                        UsedTime = lo.ReceivedDate,
                        Amount = lo.TotalPrice
                    }
                };
            }
            else if (trans.OriginalObject is EventBooking eb)
            {
                DgInvoiceItems.ItemsSource = new List<InvoiceDisplayItem>
                {
                    new()
                    {
                        ServiceCategory = "Sự kiện",
                        ServiceName = $"Thuê {eb.SpaceName} ({eb.StartTime:dd/MM HH:mm} - {eb.EndTime:dd/MM HH:mm})",
                        UsedTime = eb.StartTime,
                        Amount = eb.PaymentStatus == "Hoàn tất" ? eb.FinalTotal : eb.DepositAmount
                    }
                };
            }
            else
            {
                DgInvoiceItems.ItemsSource = new List<InvoiceDisplayItem>
                {
                    new()
                    {
                        ServiceCategory = trans.ServiceCategory,
                        ServiceName = $"{trans.ServiceCategory} - {trans.PaymentMethod}",
                        UsedTime = trans.PaymentTime,
                        Amount = trans.Amount
                    }
                };
            }
        }

        private void LoadRoomInvoiceData(RoomInvoice invoice)
        {
            if (invoice == null) return;

            string creator = !string.IsNullOrWhiteSpace(invoice.IssuedBy) && invoice.IssuedBy != "Lễ Tân Trực Chính" 
                ? invoice.IssuedBy 
                : "Chủ khách sạn";
            string cashier = !string.IsNullOrWhiteSpace(invoice.PaidBy)
                ? invoice.PaidBy
                : (!string.IsNullOrWhiteSpace(invoice.IssuedBy) ? invoice.IssuedBy : "Chủ khách sạn");

            TxtInvoiceCode.Text = invoice.InvoiceCode;
            TxtCustomerName.Text = string.IsNullOrEmpty(invoice.CustomerName) ? "Khách lưu trú" : invoice.CustomerName;
            TxtPhoneAndCccd.Text = $"{invoice.PhoneNumber} | CCCD: {invoice.IdentityCard}";
            TxtRoomNumber.Text = invoice.RoomNumber;
            TxtInvoiceDate.Text = invoice.InvoiceDate.ToString("dd/MM/yyyy HH:mm");
            TxtCreatorStaff.Text = creator;
            TxtCashierStaff.Text = cashier;

            // Chỉ tính tổng tiền các dịch vụ đã sử dụng
            TxtGrandTotal.Text = $"{invoice.ServicesCost:N0} đ";

            var items = invoice.PaidItems.Select(x => new InvoiceDisplayItem
            {
                ServiceCategory = x.ServiceCategory,
                ServiceName = x.ServiceName,
                UsedTime = x.UsedTime,
                Amount = x.Amount
            }).ToList();

            DgInvoiceItems.ItemsSource = items;
        }

        private void LoadServiceItemData(RoomServiceUsageItem item, HotelRoom? room)
        {
            string roomNum = !string.IsNullOrEmpty(item.RoomNumber) ? item.RoomNumber : (room?.RoomNumber ?? "");
            string guestName = !string.IsNullOrEmpty(room?.CustomerName) ? room.CustomerName : "Khách lưu trú";
            string phoneCccd = !string.IsNullOrEmpty(room?.PhoneNumber) ? $"{room.PhoneNumber} | CCCD: {room.IdentityCard}" : "Thông tin lưu trú";
            
            string creator = !string.IsNullOrWhiteSpace(item.RecordedBy)
                ? item.RecordedBy
                : (AuthService.Instance.IsAdmin ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(item.UsedTime));

            string cashier = item.IsPaid 
                ? (!string.IsNullOrWhiteSpace(item.PaidBy) ? item.PaidBy : (!string.IsNullOrWhiteSpace(item.RecordedBy) ? item.RecordedBy : (AuthService.Instance.IsAdmin ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(item.UsedTime))))
                : "";

            TxtInvoiceCode.Text = $"HD-DV-{item.Id:D4}";
            TxtCustomerName.Text = guestName;
            TxtPhoneAndCccd.Text = phoneCccd;
            TxtRoomNumber.Text = $"{roomNum} ({(room?.RoomType ?? "Standard")})";
            TxtInvoiceDate.Text = item.UsedTime.ToString("dd/MM/yyyy HH:mm");
            TxtCreatorStaff.Text = creator;
            TxtCashierStaff.Text = cashier;
            TxtGrandTotal.Text = $"{item.Amount:N0} đ";

            DgInvoiceItems.ItemsSource = new List<InvoiceDisplayItem>
            {
                new()
                {
                    ServiceCategory = item.ServiceCategory,
                    ServiceName = item.ServiceName,
                    UsedTime = item.UsedTime,
                    Amount = item.Amount
                }
            };
        }

        private void LoadFoodOrderData(FoodOrder order)
        {
            string roomStr = string.IsNullOrWhiteSpace(order.RoomNumber) ? "Khách vãng lai" : $"Phòng {DataService.NormalizeRoomNumber(order.RoomNumber)}";
            string creator = !string.IsNullOrWhiteSpace(order.RecordedBy) && order.RecordedBy != "Lễ Tân Trực Chính"
                ? order.RecordedBy 
                : (order.NguoiTaoId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(order.CreatedAt));

            // Nếu tính chung vào tiền phòng và chưa thanh toán -> Người thu tiền: để trống
            string cashier = (order.PaymentType == FoodPaymentType.RoomBill || order.PaymentType == FoodPaymentType.AddToRoomBill) && !order.DaThanhToan
                ? ""
                : (!string.IsNullOrWhiteSpace(order.PaidByStaffName)
                    ? order.PaidByStaffName 
                    : (!string.IsNullOrWhiteSpace(order.RecordedBy) ? order.RecordedBy : (order.NguoiThanhToanId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(order.ThoiGianThanhToan ?? order.CreatedAt))));

            TxtInvoiceCode.Text = order.OrderCode;
            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(order.CustomerName) ? order.CustomerName : "Khách hàng";
            TxtPhoneAndCccd.Text = "Hóa đơn dịch vụ ẩm thực";
            TxtRoomNumber.Text = roomStr;
            TxtInvoiceDate.Text = order.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            TxtCreatorStaff.Text = creator;
            TxtCashierStaff.Text = cashier;
            TxtGrandTotal.Text = $"{order.TotalAmount:N0} đ";

            var items = new List<InvoiceDisplayItem>();
            if (order.Items.Count > 0)
            {
                foreach (var it in order.Items)
                {
                    items.Add(new InvoiceDisplayItem
                    {
                        ServiceCategory = "Ẩm thực",
                        ServiceName = $"{it.Quantity}x {it.FoodItemName} ({it.Price:N0} đ/suất)",
                        UsedTime = order.CreatedAt,
                        Amount = it.TotalPrice
                    });
                }
            }
            else
            {
                items.Add(new InvoiceDisplayItem
                {
                    ServiceCategory = "Ẩm thực",
                    ServiceName = "Đơn gọi món ẩm thực & mini-bar",
                    UsedTime = order.CreatedAt,
                    Amount = order.TotalAmount
                });
            }

            DgInvoiceItems.ItemsSource = items;
        }

        private void LoadVehicleRentalData(VehicleRental rental)
        {
            string roomStr = string.IsNullOrWhiteSpace(rental.RoomNumber) ? "Khách ngoài" : $"Phòng {DataService.NormalizeRoomNumber(rental.RoomNumber)}";
            string creator = !string.IsNullOrWhiteSpace(rental.RecordedBy) && rental.RecordedBy != "Lễ Tân Trực Chính"
                ? rental.RecordedBy 
                : (rental.NguoiTaoId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(rental.RentalDate));

            string cashier = !rental.DaThanhToan && !string.IsNullOrWhiteSpace(rental.RoomNumber)
                ? ""
                : (!string.IsNullOrWhiteSpace(rental.PaidByStaffName)
                    ? rental.PaidByStaffName
                    : (!string.IsNullOrWhiteSpace(rental.RecordedBy) ? rental.RecordedBy : (rental.NguoiThanhToanId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(rental.ThoiGianThanhToan ?? rental.ActualReturnDate ?? rental.RentalDate))));

            TxtInvoiceCode.Text = rental.RentalCode;
            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(rental.CustomerName) ? rental.CustomerName : "Khách thuê xe";
            TxtPhoneAndCccd.Text = $"{rental.PhoneNumber} | CCCD: {rental.IdentityCard}";
            TxtRoomNumber.Text = roomStr;
            TxtInvoiceDate.Text = rental.RentalDate.ToString("dd/MM/yyyy HH:mm");
            TxtCreatorStaff.Text = creator;
            TxtCashierStaff.Text = (!rental.DaThanhToan && (rental.PaymentStatus == "Ghi nợ vào phòng" || !string.IsNullOrWhiteSpace(rental.RoomNumber)))
                ? "Ghi nợ phòng (Chờ thanh toán khi Check-out)"
                : cashier;

            int days = Math.Max(1, (int)(rental.ExpectedReturnDate - rental.RentalDate).TotalDays);
            var items = new List<InvoiceDisplayItem>
            {
                new()
                {
                    ServiceCategory = "Thuê xe máy",
                    ServiceName = $"Thuê xe {rental.VehicleName} (BS: {rental.LicensePlate}) - {days} ngày",
                    UsedTime = rental.RentalDate,
                    Amount = rental.RentalFee
                }
            };

            if (rental.AdditionalCost > 0)
            {
                items.Add(new InvoiceDisplayItem
                {
                    ServiceCategory = "Thuê xe máy",
                    ServiceName = "Chi phí phát sinh / Phụ thu",
                    UsedTime = rental.ActualReturnDate ?? DateTime.Now,
                    Amount = rental.AdditionalCost
                });
            }

            if (rental.Status == "Hoàn tất" || rental.OrderStatus == "Hoàn tất" || rental.PaymentStatus == "Ghi nợ vào phòng" || rental.DaTraXe)
            {
                if (rental.DepositAmount > 0)
                {
                    items.Add(new InvoiceDisplayItem
                    {
                        ServiceCategory = "Thuê xe máy",
                        ServiceName = "Đã trừ tiền cọc thu trước",
                        UsedTime = rental.RentalDate,
                        Amount = -rental.DepositAmount
                    });
                }
                decimal remaining = Math.Max(0, (rental.RentalFee + rental.AdditionalCost) - rental.DepositAmount);
                TxtGrandTotal.Text = $"{remaining:N0} đ";
            }
            else
            {
                TxtGrandTotal.Text = $"{(rental.DaThanhToan ? rental.TotalPayment : rental.DepositAmount):N0} đ";
            }

            DgInvoiceItems.ItemsSource = items;
        }

        private void LoadEventBookingData(EventBooking booking)
        {
            string roomStr = string.IsNullOrWhiteSpace(booking.RoomNumber) ? "Khách ngoài" : $"Phòng {DataService.NormalizeRoomNumber(booking.RoomNumber)}";
            string creator = !string.IsNullOrWhiteSpace(booking.RecordedBy) && booking.RecordedBy != "Lễ Tân Trực Chính"
                ? booking.RecordedBy 
                : (booking.NguoiTaoId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(booking.CreatedAt));

            string cashier = (booking.PaymentStatus == "Ghi nợ vào phòng" || (!booking.DaThanhToan && !string.IsNullOrWhiteSpace(booking.RoomNumber)))
                ? "Ghi nợ phòng (Chờ thanh toán khi Check-out)"
                : (!string.IsNullOrWhiteSpace(booking.PaidByStaffName)
                    ? booking.PaidByStaffName
                    : (!string.IsNullOrWhiteSpace(booking.RecordedBy) ? booking.RecordedBy : (booking.NguoiThanhToanId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(booking.ThoiGianThanhToan ?? booking.EndTime))));

            TxtInvoiceCode.Text = booking.BookingCode;
            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(booking.CustomerName) ? booking.CustomerName : "Khách đặt sự kiện";
            TxtPhoneAndCccd.Text = $"{booking.PhoneNumber}";
            TxtRoomNumber.Text = roomStr;
            TxtInvoiceDate.Text = booking.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            TxtCreatorStaff.Text = creator;
            TxtCashierStaff.Text = cashier;

            var items = new List<InvoiceDisplayItem>
            {
                new()
                {
                    ServiceCategory = "Sự kiện",
                    ServiceName = $"Thuê {booking.SpaceName} ({booking.StartTime:dd/MM HH:mm} - {booking.EndTime:dd/MM HH:mm})",
                    UsedTime = booking.StartTime,
                    Amount = booking.TotalEstimatedAmount
                }
            };

            if (booking.AdditionalCost > 0)
            {
                items.Add(new InvoiceDisplayItem
                {
                    ServiceCategory = "Sự kiện",
                    ServiceName = "Chi phí phát sinh / Phụ thu sự kiện",
                    UsedTime = booking.EndTime,
                    Amount = booking.AdditionalCost
                });
            }

            if (booking.PaymentStatus == "Hoàn tất" || booking.PaymentStatus == "Ghi nợ vào phòng" || booking.OrderStatus == "Hoàn tất")
            {
                // Khi hoàn tất thanh toán quyết toán: Số tiền phải trả = Số tiền tổng – Tiền đã cọc
                if (booking.DepositAmount > 0)
                {
                    items.Add(new InvoiceDisplayItem
                    {
                        ServiceCategory = "Sự kiện",
                        ServiceName = "Đã trừ tiền cọc thu trước",
                        UsedTime = booking.CreatedAt,
                        Amount = -booking.DepositAmount
                    });
                }
                decimal remainingToPay = Math.Max(0, (booking.TotalEstimatedAmount + booking.AdditionalCost) - booking.DepositAmount);
                TxtGrandTotal.Text = $"{remainingToPay:N0} đ";
            }
            else
            {
                // Lúc tiếp nhận đặt sảnh: Thu tiền cọc
                TxtGrandTotal.Text = $"{booking.DepositAmount:N0} đ";
            }

            DgInvoiceItems.ItemsSource = items;
        }

        private void LoadLaundryOrderData(LaundryOrder order)
        {
            string roomStr = string.IsNullOrWhiteSpace(order.RoomNumber) ? "Khách vãng lai" : $"Phòng {DataService.NormalizeRoomNumber(order.RoomNumber)}";
            string creator = !string.IsNullOrWhiteSpace(order.RecordedBy) && order.RecordedBy != "Lễ Tân Trực Chính"
                ? order.RecordedBy 
                : (order.NguoiTaoId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(order.ReceivedDate));

            string cashier = !order.DaThanhToan && !string.IsNullOrWhiteSpace(order.RoomNumber)
                ? ""
                : (!string.IsNullOrWhiteSpace(order.PaidByStaffName)
                    ? order.PaidByStaffName
                    : (!string.IsNullOrWhiteSpace(order.RecordedBy) ? order.RecordedBy : (order.NguoiThanhToanId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(order.ThoiGianThanhToan ?? order.ReceivedDate))));

            TxtInvoiceCode.Text = order.OrderCode;
            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(order.CustomerName) ? order.CustomerName : "Khách gửi giặt";
            TxtPhoneAndCccd.Text = $"{order.PhoneNumber}";
            TxtRoomNumber.Text = roomStr;
            TxtInvoiceDate.Text = order.ReceivedDate.ToString("dd/MM/yyyy HH:mm");
            TxtCreatorStaff.Text = creator;
            TxtCashierStaff.Text = cashier;
            TxtGrandTotal.Text = $"{order.TotalPrice:N0} đ";

            DgInvoiceItems.ItemsSource = new List<InvoiceDisplayItem>
            {
                new()
                {
                    ServiceCategory = "Giặt ủi",
                    ServiceName = $"{order.ServiceTypeDisplay} ({order.WeightKg} kg • Đối tác: {order.PartnerName})",
                    UsedTime = order.ReceivedDate,
                    Amount = order.TotalPrice
                }
            };
        }

        private void LoadParkingRecordData(ParkingRecord record)
        {
            string roomStr = string.IsNullOrWhiteSpace(record.RoomNumber) ? "Khách vãng lai" : $"Phòng {DataService.NormalizeRoomNumber(record.RoomNumber)}";
            string creator = !string.IsNullOrWhiteSpace(record.RecordedBy) && record.RecordedBy != "Lễ Tân Trực Chính"
                ? record.RecordedBy 
                : (record.NguoiTaoId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(record.CheckInTime));

            string cashier = record.ChargeType == ParkingChargeType.ResidentFree
                ? "Miễn phí (Khách phòng)"
                : (record.DaThanhToan 
                    ? (!string.IsNullOrWhiteSpace(record.PaidByStaffName)
                        ? record.PaidByStaffName
                        : (!string.IsNullOrWhiteSpace(record.RecordedBy) ? record.RecordedBy : (record.NguoiThanhToanId == 0 ? "Chủ khách sạn" : DataService.Instance.GetCurrentDutyStaffName(record.ThoiGianThanhToan ?? record.CheckInTime))))
                    : "");

            TxtInvoiceCode.Text = record.TicketCode;
            TxtCustomerName.Text = !string.IsNullOrWhiteSpace(record.CustomerName) ? record.CustomerName : "Khách gửi xe";
            TxtPhoneAndCccd.Text = $"{record.PhoneNumber}";
            TxtRoomNumber.Text = roomStr;
            TxtInvoiceDate.Text = record.CheckInTime.ToString("dd/MM/yyyy HH:mm");
            TxtCreatorStaff.Text = creator;
            TxtCashierStaff.Text = cashier;
            TxtGrandTotal.Text = $"{record.ParkingFee:N0} đ";

            DgInvoiceItems.ItemsSource = new List<InvoiceDisplayItem>
            {
                new()
                {
                    ServiceCategory = "Bãi đỗ xe",
                    ServiceName = $"Gửi xe {record.VehicleType} (Biển số: {record.LicensePlate})",
                    UsedTime = record.CheckInTime,
                    Amount = record.ParkingFee
                }
            };
        }

        public bool IsConfirmed { get; private set; } = false;

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDlg = new System.Windows.Controls.PrintDialog();
                if (printDlg.ShowDialog() == true)
                {
                    printDlg.PrintVisual(this, $"HoaDonDichVu_{TxtInvoiceCode.Text}");
                    MessageBox.Show("Lệnh in hóa đơn dịch vụ đã được gửi tới máy in thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể gửi lệnh in: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isViewOnly)
            {
                IsConfirmed = false;
                DialogResult = true;
                Close();
                return;
            }

            IsConfirmed = true;
            DialogResult = true;
            Close();
        }
    }
}
