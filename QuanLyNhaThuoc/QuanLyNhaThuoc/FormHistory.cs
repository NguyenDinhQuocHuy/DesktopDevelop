using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyNhaThuoc
{
    public partial class FormHistory : Form
    {
        public FormHistory(Customer customer, List<Invoice> allInvoices)
        {
            InitializeComponent();

            lblCustomerInfo.Text = $"Khách hàng: {customer.Name} | SĐT: {customer.NumPhone} | Điểm hiện tại: {customer.Point} điểm";

            var customerInvoices = allInvoices
                .Where(x => x.CustomerPhone == customer.NumPhone)
                .OrderByDescending(x => x.PurchaseDate)
                .Select(x => new
                {
                    Mã_HD = x.InvoiceId,
                    Ngày_Mua = x.DisplayDate,
                    Sản_Phẩm = x.ItemsSummary,
                    Tổng_Tiền = x.FormattedTotal,
                    Điểm_Thưởng = $"+{x.PointsEarned}"
                })
                .ToList();

            dgvHistory.DataSource = customerInvoices;

            float totalSpent = allInvoices.Where(x => x.CustomerPhone == customer.NumPhone).Sum(x => x.TotalAmount);
            lblSummaryStats.Text = $"Tổng số hóa đơn: {customerInvoices.Count} | Tổng tiền đã mua: {totalSpent:N0}đ";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
