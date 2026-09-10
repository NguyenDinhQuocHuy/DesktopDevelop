using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace QuanLyNhaThuoc
{
    public static class DataManager
    {
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        private static readonly string CustomersFile = Path.Combine(DataFolder, "customers.json");
        private static readonly string MedicinesFile = Path.Combine(DataFolder, "medicines.json");
        private static readonly string InvoicesFile = Path.Combine(DataFolder, "invoices.json");

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        static DataManager()
        {
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }
        }

        // ===================== KHÁCH HÀNG (CUSTOMERS) =====================
        public static List<Customer> LoadCustomers()
        {
            try
            {
                if (File.Exists(CustomersFile))
                {
                    string json = File.ReadAllText(CustomersFile);
                    var list = JsonSerializer.Deserialize<List<Customer>>(json, Options);
                    if (list != null && list.Count > 0) return list;
                }
            }
            catch { }

            // Mặc định dữ liệu khởi tạo mẫu nếu chưa có tệp
            var defaultCustomers = new List<Customer>
            {
                new Customer("Nguyễn Văn An", "0901234567", 125),
                new Customer("Trần Thị Bình", "0912345678", 87),
                new Customer("Lê Văn Cường", "0923456789", 215),
                new Customer("Phạm Thị Dung", "0934567890", 56),
                new Customer("Hoàng Văn Đức", "0945678901", 178),
                new Customer("Vũ Thị Hoa", "0956789012", 320),
                new Customer("Đặng Văn Hùng", "0967890123", 42),
                new Customer("Bùi Thị Lan", "0978901234", 156),
                new Customer("Ngô Văn Minh", "0989012345", 93),
                new Customer("Đỗ Thị Ngọc", "0990123456", 267)
            };

            SaveCustomers(defaultCustomers);
            return defaultCustomers;
        }

        public static void SaveCustomers(List<Customer> customers)
        {
            try
            {
                string json = JsonSerializer.Serialize(customers, Options);
                File.ReadAllText(CustomersFile); // Test access
                File.WriteAllText(CustomersFile, json);
            }
            catch
            {
                try
                {
                    string json = JsonSerializer.Serialize(customers, Options);
                    File.WriteAllText(CustomersFile, json);
                }
                catch { }
            }
        }

        // ===================== THUỐC (MEDICINES) =====================
        public static List<Medicine> LoadMedicines()
        {
            try
            {
                if (File.Exists(MedicinesFile))
                {
                    string json = File.ReadAllText(MedicinesFile);
                    var list = JsonSerializer.Deserialize<List<Medicine>>(json, Options);
                    if (list != null && list.Count > 0) return list;
                }
            }
            catch { }

            var defaultMedicines = new List<Medicine>
            {
                new Medicine("Paracetamol", 2000),
                new Medicine("Aspirin", 1500),
                new Medicine("Ibuprofen", 3000),
                new Medicine("Amoxicillin", 5000),
                new Medicine("Azithromycin", 8000),
                new Medicine("Cephalexin", 6000),
                new Medicine("Cefuroxime", 10000),
                new Medicine("Cefixime", 12000),
                new Medicine("Metformin", 2500),
                new Medicine("Glimepiride", 4000),
                new Medicine("Amlodipine", 3000),
                new Medicine("Losartan", 3500),
                new Medicine("Omeprazole", 4000),
                new Medicine("Esomeprazole", 6000),
                new Medicine("Pantoprazole", 5500),
                new Medicine("Smecta", 5000),
                new Medicine("Berocca", 7000),
                new Medicine("Vitamin C", 2000),
                new Medicine("Dầu cá Omega 3", 5000),
                new Medicine("Cetirizine", 2500),
                new Medicine("Loratadine", 3000),
                new Medicine("Acetylcysteine", 4000),
                new Medicine("Berberin", 2000)
            };

            SaveMedicines(defaultMedicines);
            return defaultMedicines;
        }

        public static void SaveMedicines(List<Medicine> medicines)
        {
            try
            {
                string json = JsonSerializer.Serialize(medicines, Options);
                File.WriteAllText(MedicinesFile, json);
            }
            catch { }
        }

        // ===================== HÓA ĐƠN / LỊCH SỬ MUA HÀNG (INVOICES) =====================
        public static List<Invoice> LoadInvoices()
        {
            try
            {
                if (File.Exists(InvoicesFile))
                {
                    string json = File.ReadAllText(InvoicesFile);
                    var list = JsonSerializer.Deserialize<List<Invoice>>(json, Options);
                    if (list != null && list.Count > 0) return list;
                }
            }
            catch { }

            var defaultInvoices = new List<Invoice>
            {
                new Invoice(
                    "HD20260810143001",
                    "0901234567",
                    "Nguyễn Văn An",
                    DateTime.Now.AddDays(-7),
                    new List<CartItem> { new CartItem("Paracetamol", 2000, 5), new CartItem("Smecta", 5000, 2), new CartItem("Berocca", 7000, 1) },
                    27000,
                    27
                ),
                new Invoice(
                    "HD20260812091522",
                    "0901234567",
                    "Nguyễn Văn An",
                    DateTime.Now.AddDays(-4),
                    new List<CartItem> { new CartItem("Amoxicillin", 5000, 4), new CartItem("Vitamin C", 2000, 3) },
                    26000,
                    26
                ),
                new Invoice(
                    "HD20260815162045",
                    "0901234567",
                    "Nguyễn Văn An",
                    DateTime.Now.AddDays(-1),
                    new List<CartItem> { new CartItem("Dầu cá Omega 3", 5000, 2), new CartItem("Acetylcysteine", 4000, 2) },
                    18000,
                    18
                ),
                new Invoice(
                    "HD20260811100512",
                    "0912345678",
                    "Trần Thị Bình",
                    DateTime.Now.AddDays(-5),
                    new List<CartItem> { new CartItem("Aspirin", 1500, 10), new CartItem("Pantoprazole", 5500, 2) },
                    26000,
                    26
                ),
                new Invoice(
                    "HD20260814113000",
                    "0923456789",
                    "Lê Văn Cường",
                    DateTime.Now.AddDays(-2),
                    new List<CartItem> { new CartItem("Azithromycin", 8000, 3), new CartItem("Cetirizine", 2500, 4) },
                    34000,
                    34
                ),
                new Invoice(
                    "HD20260808084530",
                    "0956789012",
                    "Vũ Thị Hoa",
                    DateTime.Now.AddDays(-9),
                    new List<CartItem> { new CartItem("Cefixime", 12000, 2), new CartItem("Berberin", 2000, 5) },
                    34000,
                    34
                )
            };

            SaveInvoices(defaultInvoices);
            return defaultInvoices;
        }

        public static void SaveInvoices(List<Invoice> invoices)
        {
            try
            {
                string json = JsonSerializer.Serialize(invoices, Options);
                File.WriteAllText(InvoicesFile, json);
            }
            catch { }
        }
    }
}
