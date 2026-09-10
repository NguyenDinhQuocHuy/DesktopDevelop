using System;

namespace QuanLyNhaThuoc
{
    public class Medicine
    {
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        
        public Medicine(string name, float price)
        { 
            Name = name;
            Price = price;
        }

        public Medicine() { }
    }
}
