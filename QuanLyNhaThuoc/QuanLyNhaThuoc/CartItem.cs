using System;

namespace QuanLyNhaThuoc
{
    public class CartItem
    {
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public int Quantity { get; set; } = 1;
        public float TotalPrice => Price * Quantity;

        public CartItem(string name, float price, int quantity = 1)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public CartItem() { }
    }

    public class RewardItem
    {
        public string Name { get; set; } = string.Empty;
        public int PointsRequired { get; set; }

        public RewardItem(string name, int pointsRequired)
        {
            Name = name;
            PointsRequired = pointsRequired;
        }

        public override string ToString()
        {
            return $"{Name} - {PointsRequired} điểm";
        }
    }
}
