using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyNhaThuoc
{
    public class Invoice
    {
        public string InvoiceId { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public float TotalAmount { get; set; }
        public int PointsEarned { get; set; }

        public Invoice() { }

        public Invoice(string invoiceId, string customerPhone, string customerName, DateTime purchaseDate, List<CartItem> items, float totalAmount, int pointsEarned)
        {
            InvoiceId = invoiceId;
            CustomerPhone = customerPhone;
            CustomerName = customerName;
            PurchaseDate = purchaseDate;
            Items = items;
            TotalAmount = totalAmount;
            PointsEarned = pointsEarned;
        }

        public string DisplayDate => PurchaseDate.ToString("dd/MM/yyyy HH:mm:ss");
        public string FormattedTotal => $"{TotalAmount:N0}đ";
        public string ItemsSummary => string.Join(", ", Items.Select(x => $"{x.Name} (x{x.Quantity})"));
    }
}
