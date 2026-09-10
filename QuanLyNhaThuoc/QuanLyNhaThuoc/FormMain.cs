using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
namespace QuanLyNhaThuoc
{
    public partial class FormMain : Form
    {

        List<Medicine> medicines = new List<Medicine>();
        List<Customer> customers = new List<Customer>();
        List<CartItem> buyings = new List<CartItem>();
        List<RewardItem> rewards = new List<RewardItem>();
        List<Invoice> invoices = new List<Invoice>();


        public FormMain()
        {
            InitializeComponent();

            find.Enabled = false;
            register.Enabled = false;
            addProduct.Enabled = false;
            calc.Enabled = false;

            invoices = DataManager.LoadInvoices();
            LoadCustomers();
            LoadLeaderboard();
            LoadMedicines();
            LoadRewards();

            menuPro.DataSource = buyings;
            customer.DataSource = customers;

            proBox.DropDownStyle = ComboBoxStyle.DropDown;
            proBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            proBox.AutoCompleteSource = AutoCompleteSource.CustomSource;

            proBox.KeyDown += proBox_KeyDown;
            nudQuantity.KeyDown += nudQuantity_KeyDown;

            SetupContextMenu();
        }

        private void ShowAllCustomers()
        {
            customer.DataSource = null;
            customer.DataSource = customers;
        }

        private void sdtBox_TextChanged(object sender, EventArgs e)
        {
            find.Enabled = !string.IsNullOrWhiteSpace(sdtBox.Text);
            CheckInput_Register();
            CheckInput_Calc();

            if (string.IsNullOrWhiteSpace(sdtBox.Text))
            {
                ShowAllCustomers();
            }

        }
        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            CheckInput_Register();
            find.Enabled = !string.IsNullOrWhiteSpace(nameBox.Text);
            CheckInput_Calc();
            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                ShowAllCustomers();
            }

        }
        private void CheckInput_Register()
        {
            register.Enabled =
                !string.IsNullOrWhiteSpace(nameBox.Text) &&
                !string.IsNullOrWhiteSpace(sdtBox.Text);
        }

        private void CheckInput_Product()
        {
            try
            {
                string text = (proBox.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    addProduct.Enabled = false;
                    return;
                }

                addProduct.Enabled = medicines.Any(x => x.Name.Equals(text, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                addProduct.Enabled = false;
            }
        }

        private void CheckInput_Calc()
        {
            calc.Enabled = buyings.Count > 0;
        }

        private void proBox_TextChanged(object sender, EventArgs e)
        {
            CheckInput_Product();
        }

        private void proBox_TextUpdate(object sender, EventArgs e)
        {
            CheckInput_Product();
        }

        private void proBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Chặn tiếng kêu bíp hệ thống WinForms
                if (addProduct.Enabled)
                {
                    addPro_Click(sender, e);
                }
                else
                {
                    nudQuantity.Focus();
                }
            }
        }

        private void nudQuantity_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Chặn tiếng kêu bíp hệ thống WinForms
                if (addProduct.Enabled)
                {
                    addPro_Click(sender, e);
                }
            }
        }

        private void UpdateTotal()
        {
            float totalMoney = buyings.Sum(x => x.TotalPrice);

            money.Text = $"{totalMoney:N0}đ";

            int totalPoint = (int)(totalMoney / 1000);

            points.Text = totalPoint.ToString();
        }

        private void addPro_Click(object? sender, EventArgs e)
        {
            string name = proBox.Text.Trim();
            int qty = (int)nudQuantity.Value;

            Medicine? medicine = medicines.FirstOrDefault(
                x => x.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase)
            );

            if (medicine == null)
            {
                MessageBox.Show("Vui lòng chọn một thuốc hợp lệ!");
                return;
            }

            CartItem? existingItem = buyings.FirstOrDefault(x => x.Name.Equals(medicine.Name, StringComparison.OrdinalIgnoreCase));
            if (existingItem != null)
            {
                existingItem.Quantity += qty;
            }
            else
            {
                buyings.Add(new CartItem(medicine.Name, medicine.Price, qty));
            }

            menuPro.DataSource = null;
            menuPro.DataSource = buyings;

            UpdateTotal();
            CheckInput_Calc();

            proBox.Text = "";
            nudQuantity.Value = 1;
            proBox.Focus();
        }
        private void find_Click(object sender, EventArgs e)
        {
            string sdt = sdtBox.Text.Trim();
            string name = nameBox.Text.Trim();

            var result = customers.Where(x =>
                (string.IsNullOrWhiteSpace(sdt) ||
                 x.NumPhone.Contains(sdt)) &&

                (string.IsNullOrWhiteSpace(name) ||
                 x.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            customer.DataSource = null;
            customer.DataSource = result;
        }

        private void LoadLeaderboard()
        {
            var ranking = customers.OrderByDescending(x => x.Point).ToList();
            leaderboard.DataSource = null;
            leaderboard.DataSource = ranking;
        }

        private int GetCurrentPoint()
        {
            float totalMoney = buyings.Sum(x => x.TotalPrice);

            return (int)(totalMoney / 1000);
        }


        private void calc_Click(object sender, EventArgs e)
        {
            string sdt = sdtBox.Text.Trim();
            string name = nameBox.Text.Trim();

            // Kiểm tra thông tin nhập vào
            if (string.IsNullOrWhiteSpace(sdt) ||
                string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ SĐT và Họ tên!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // Kiểm tra có thuốc đang mua hay chưa
            if (buyings.Count == 0)
            {
                MessageBox.Show(
                    "Chưa có thuốc nào trong danh sách mua!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // Tính điểm của hóa đơn hiện tại
            int newPoint = GetCurrentPoint();

            // Tìm khách hàng theo SĐT
            Customer? customerFound = customers.FirstOrDefault(
                x => x.NumPhone == sdt
            );

            // =========================
            // TRƯỜNG HỢP KHÁCH ĐÃ CÓ
            // =========================
            if (customerFound != null)
            {
                customerFound.Point += newPoint;

                float totalMoney = buyings.Sum(x => x.TotalPrice);
                string invId = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Invoice inv = new Invoice(invId, customerFound.NumPhone, customerFound.Name, DateTime.Now, new List<CartItem>(buyings), totalMoney, newPoint);
                invoices.Add(inv);

                DataManager.SaveInvoices(invoices);
                DataManager.SaveCustomers(customers);

                MessageBox.Show(
                    $"Khách hàng: {customerFound.Name}\n" +
                    $"SĐT: {customerFound.NumPhone}\n\n" +
                    $"➕ Điểm tích lũy thêm: +{newPoint}\n" +
                    $"🌟 Tổng điểm hiện tại: {customerFound.Point}\n\n" +
                    $"📜 Đã lưu hóa đơn {invId} vào Lịch sử mua hàng!",
                    "Thanh toán thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                ShowAllCustomers();
                LoadLeaderboard();
                ClearBill();
                return;
            }

            // =========================
            // KHÁCH CHƯA CÓ
            // =========================
            DialogResult result = MessageBox.Show(
                $"Không tìm thấy khách hàng trong hệ thống.\n\n" +
                $"SĐT: {sdt}\n" +
                $"Họ tên: {name}\n\n" +
                $"Bạn có muốn đăng ký khách hàng mới không?",
                "Khách hàng mới",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Customer newCustomer = new Customer(
                    name,
                    sdt,
                    newPoint
                );

                customers.Add(newCustomer);

                float totalMoney = buyings.Sum(x => x.TotalPrice);
                string invId = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Invoice inv = new Invoice(invId, newCustomer.NumPhone, newCustomer.Name, DateTime.Now, new List<CartItem>(buyings), totalMoney, newPoint);
                invoices.Add(inv);

                DataManager.SaveInvoices(invoices);
                DataManager.SaveCustomers(customers);

                ShowAllCustomers();
                LoadLeaderboard();

                MessageBox.Show(
                    $"Đã đăng ký khách hàng mới!\n\n" +
                    $"Họ tên: {name}\n" +
                    $"SĐT: {sdt}\n" +
                    $"Điểm tích lũy: {newPoint}\n\n" +
                    $"📜 Đã lưu hóa đơn {invId} vào Lịch sử mua hàng!",
                    "Đăng ký thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            ClearBill();
        }

        private void ClearBill()
        {
            buyings.Clear();

            menuPro.DataSource = null;
            menuPro.DataSource = buyings;

            money.Text = "0đ";
            points.Text = "0";

            proBox.Text = "";
            sdtBox.Clear();
            nameBox.Clear();
            txtEditPoint.Clear();
            CheckInput_Calc();
        }

        private void register_Click(object sender, EventArgs e)
        {
            string sdt = sdtBox.Text.Trim();
            string hoTen = nameBox.Text.Trim();

            // Kiểm tra số điện thoại
            if (!Regex.IsMatch(sdt, @"^0\d{9}$"))
            {
                MessageBox.Show(
                    "Số điện thoại phải có đúng 10 chữ số và đúng cú pháp!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                sdtBox.Focus();
                return;
            }

            // Kiểm tra họ tên
            if (!Regex.IsMatch(hoTen, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show(
                    "Họ và tên chỉ được chứa chữ cái và khoảng trắng!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                nameBox.Focus();
                return;
            }

            // Kiểm tra số điện thoại đã tồn tại
            if (customers.Any(x => x.NumPhone == sdt))
            {
                MessageBox.Show(
                    "Số điện thoại này đã được đăng ký!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                sdtBox.Focus();
                return;
            }

            // Tạo khách hàng mới
            Customer customerNew = new Customer
            {
                Name = hoTen,
                NumPhone = sdt,
                Point = 0
            };

            customers.Add(customerNew);
            DataManager.SaveCustomers(customers);

            // Cập nhật DataGridView khách hàng
            customer.DataSource = null;
            customer.DataSource = customers;

            // Cập nhật bảng xếp hạng
            LoadLeaderboard();

            MessageBox.Show(
                "Đăng ký khách hàng thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            // Xóa dữ liệu sau khi đăng ký
            sdtBox.Clear();
            nameBox.Clear();
        }

        private void customer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= customer.Rows.Count) return;

            DataGridViewRow row = customer.Rows[e.RowIndex];
            if (row.DataBoundItem is Customer cust)
            {
                nameBox.Text = cust.Name;
                sdtBox.Text = cust.NumPhone;
                txtEditPoint.Text = cust.Point.ToString();
            }
        }

        private void OpenEditCustomerDialog()
        {
            string sdt = sdtBox.Text.Trim();
            Customer? targetCust = customers.FirstOrDefault(x => x.NumPhone == sdt);

            if (targetCust == null && customer.SelectedRows.Count > 0)
            {
                targetCust = customer.SelectedRows[0].DataBoundItem as Customer;
            }

            if (targetCust == null && leaderboard.SelectedRows.Count > 0)
            {
                targetCust = leaderboard.SelectedRows[0].DataBoundItem as Customer;
            }

            if (targetCust == null)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng trong danh sách hoặc nhập SĐT để chỉnh sửa thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormEditCustomer editForm = new FormEditCustomer(targetCust, customers);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                sdtBox.Text = targetCust.NumPhone;
                nameBox.Text = targetCust.Name;
                txtEditPoint.Text = targetCust.Point.ToString();

                DataManager.SaveCustomers(customers);
                ShowAllCustomers();
                LoadLeaderboard();

                MessageBox.Show($"Cập nhật toàn bộ thông tin thành công cho khách hàng: {targetCust.Name}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OpenCustomerHistory()
        {
            string sdt = sdtBox.Text.Trim();
            Customer? targetCust = customers.FirstOrDefault(x => x.NumPhone == sdt);

            if (targetCust == null && customer.SelectedRows.Count > 0)
            {
                targetCust = customer.SelectedRows[0].DataBoundItem as Customer;
            }

            if (targetCust == null && leaderboard.SelectedRows.Count > 0)
            {
                targetCust = leaderboard.SelectedRows[0].DataBoundItem as Customer;
            }

            if (targetCust == null)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng trong danh sách hoặc nhập SĐT để xem lịch sử mua hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormHistory historyForm = new FormHistory(targetCust, invoices);
            historyForm.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            OpenEditCustomerDialog();
        }

        // =========================================================================
        // 🖱️ THÀNH PHẦN MENU CHUỘT PHẢI (CONTEXT MENU STRIP) CHO DATAGRIDVIEW
        // =========================================================================
        private ContextMenuStrip cmsCustomer = new ContextMenuStrip();
        private ContextMenuStrip cmsCart = new ContextMenuStrip();

        private void SetupContextMenu()
        {
            // --- Context Menu cho Bảng Khách Hàng / Bảng Xếp Hạng ---
            ToolStripMenuItem tsmiHistory = new ToolStripMenuItem("📜 Xem lịch sử mua hàng của khách hàng này");
            ToolStripMenuItem tsmiEdit = new ToolStripMenuItem("✏️ Sửa toàn bộ thông tin khách hàng (Tên, SĐT, Điểm)");
            ToolStripMenuItem tsmiSelect = new ToolStripMenuItem("💳 Chọn khách hàng này để thanh toán");
            ToolStripMenuItem tsmiDelete = new ToolStripMenuItem("🗑️ Xóa khách hàng này");

            tsmiHistory.Click += (s, e) => OpenCustomerHistory();
            tsmiEdit.Click += (s, e) => OpenEditCustomerDialog();
            tsmiSelect.Click += (s, e) => CheckInput_Calc();
            tsmiDelete.Click += TsmiDelete_Click;

            cmsCustomer.Items.Add(tsmiHistory);
            cmsCustomer.Items.Add(tsmiEdit);
            cmsCustomer.Items.Add(tsmiSelect);
            cmsCustomer.Items.Add(new ToolStripSeparator());
            cmsCustomer.Items.Add(tsmiDelete);

            customer.ContextMenuStrip = cmsCustomer;
            leaderboard.ContextMenuStrip = cmsCustomer;

            customer.CellMouseDown += Customer_CellMouseDown;
            leaderboard.CellMouseDown += Leaderboard_CellMouseDown;

            // --- Context Menu cho Bảng Giỏ Hàng Thuốc ---
            ToolStripMenuItem tsmiRemoveItem = new ToolStripMenuItem("🗑️ Xóa thuốc này khỏi giỏ hàng");
            ToolStripMenuItem tsmiClearAll = new ToolStripMenuItem("🧹 Xóa toàn bộ giỏ hàng");

            tsmiRemoveItem.Click += TsmiRemoveItem_Click;
            tsmiClearAll.Click += (s, e) => ClearBill();

            cmsCart.Items.Add(tsmiRemoveItem);
            cmsCart.Items.Add(tsmiClearAll);

            menuPro.ContextMenuStrip = cmsCart;
            menuPro.CellMouseDown += MenuPro_CellMouseDown;
        }

        private void Customer_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.RowIndex < customer.Rows.Count)
            {
                customer.ClearSelection();
                customer.Rows[e.RowIndex].Selected = true;

                if (customer.Rows[e.RowIndex].DataBoundItem is Customer cust)
                {
                    nameBox.Text = cust.Name;
                    sdtBox.Text = cust.NumPhone;
                    txtEditPoint.Text = cust.Point.ToString();
                }
            }
        }

        private void Leaderboard_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.RowIndex < leaderboard.Rows.Count)
            {
                leaderboard.ClearSelection();
                leaderboard.Rows[e.RowIndex].Selected = true;

                if (leaderboard.Rows[e.RowIndex].DataBoundItem is Customer cust)
                {
                    nameBox.Text = cust.Name;
                    sdtBox.Text = cust.NumPhone;
                    txtEditPoint.Text = cust.Point.ToString();
                }
            }
        }

        private void MenuPro_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.RowIndex < menuPro.Rows.Count)
            {
                menuPro.ClearSelection();
                menuPro.Rows[e.RowIndex].Selected = true;
            }
        }

        private void TsmiDelete_Click(object? sender, EventArgs e)
        {
            string sdt = sdtBox.Text.Trim();
            string name = nameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Vui lòng chọn khách hàng muốn xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialog = MessageBox.Show($"Bạn có chắc chắn muốn xóa khách hàng {name} (SĐT: {sdt}) khỏi hệ thống?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                customers.RemoveAll(x => x.NumPhone == sdt);
                ShowAllCustomers();
                LoadLeaderboard();

                sdtBox.Clear();
                nameBox.Clear();
                txtEditPoint.Clear();

                MessageBox.Show($"Đã xóa thành công khách hàng {name}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TsmiRemoveItem_Click(object? sender, EventArgs e)
        {
            if (menuPro.SelectedRows.Count > 0)
            {
                int index = menuPro.SelectedRows[0].Index;
                if (index >= 0 && index < buyings.Count)
                {
                    buyings.RemoveAt(index);
                    menuPro.DataSource = null;
                    menuPro.DataSource = buyings;
                    UpdateTotal();
                    CheckInput_Calc();
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }




        private void LoadMedicines()
        {
            string data = @"
            Paracetamol|2000
            Aspirin|1500
            Ibuprofen|3000
            Amoxicillin|5000
            Azithromycin|8000
            Cephalexin|6000
            Cefuroxime|10000
            Cefixime|12000
            Metformin|2500
            Glimepiride|4000
            Amlodipine|3000
            Losartan|3500
            Enalapril|2500
            Omeprazole|4000
            Esomeprazole|6000
            Lansoprazole|5000
            Pantoprazole|5500
            Domperidone|3000
            Smecta|5000
            Berocca|7000
            Vitamin C|2000
            Vitamin B1|1500
            Vitamin B6|1500
            Vitamin B12|2000
            Calcium|3000
            Magnesium|2500
            Kẽm|2000
            Sắt|3000
            Dầu cá Omega 3|5000
            Cetirizine|2500
            Loratadine|3000
            Fexofenadine|4000
            Chlorpheniramine|1500
            Salbutamol|2500
            Dextromethorphan|3000
            Guaifenesin|2500
            Acetylcysteine|4000
            Bromhexine|2500
            Ambroxol|3000
            Povidone Iodine|5000
            Oresol|3000
            Berberin|2000
            Than hoạt tính|2500
            Men tiêu hóa|3000
            Loperamide|2500
            Bisacodyl|2000
            Magnesi hydroxide|3000
            Diclofenac|3500
            Naproxen|4000
            Ketoprofen|4500
            ";

            string[] lines = data.Trim().Split('\n');

            foreach (string line in lines)
            {
                string[] parts = line.Trim().Split('|');

                if (parts.Length == 2 &&
                    float.TryParse(parts[1], out float price))
                {
                    medicines.Add(new Medicine(parts[0], price));
                }
            }

            proBox.Items.Clear();
            AutoCompleteStringCollection autoSource = new AutoCompleteStringCollection();
            foreach (var m in medicines)
            {
                proBox.Items.Add(m.Name);
                autoSource.Add(m.Name);
            }
            proBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            proBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            proBox.AutoCompleteCustomSource = autoSource;
        }
        private void LoadCustomers()
        {
            customers.Add(new Customer("Nguyễn Văn An", "0901234567", 125));
            customers.Add(new Customer("Trần Thị Bình", "0912345678", 87));
            customers.Add(new Customer("Lê Văn Cường", "0923456789", 215));
            customers.Add(new Customer("Phạm Thị Dung", "0934567890", 56));
            customers.Add(new Customer("Hoàng Văn Đức", "0945678901", 178));
            customers.Add(new Customer("Vũ Thị Hoa", "0956789012", 320));
            customers.Add(new Customer("Đặng Văn Hùng", "0967890123", 42));
            customers.Add(new Customer("Bùi Thị Lan", "0978901234", 156));
            customers.Add(new Customer("Ngô Văn Minh", "0989012345", 93));
            customers.Add(new Customer("Đỗ Thị Ngọc", "0990123456", 267));
        }

        private void LoadRewards()
        {
            rewards.Clear();
            rewards.Add(new RewardItem("Khẩu trang y tế 4 lớp (50 cái)", 50));
            rewards.Add(new RewardItem("Chai Xịt khuẩn Hand Sanitizer 100ml", 80));
            rewards.Add(new RewardItem("Hộp Băng cá nhân Urgo (100 miếng)", 100));
            rewards.Add(new RewardItem("Voucher Giảm 50.000đ Tiền Thuốc", 150));
            rewards.Add(new RewardItem("Máy Đo Huyết Áp Điện Tử Omron", 500));
            rewards.Add(new RewardItem("Bộ Thể Thao / Áo Phông VIP", 1000));

            cboRewards.DataSource = null;
            cboRewards.DataSource = rewards;
        }

        private void btnRedeemReward_Click(object sender, EventArgs e)
        {
            string sdt = sdtBox.Text.Trim();
            string name = nameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Vui lòng chọn hoặc nhập SĐT khách hàng muốn đổi quà!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sdtBox.Focus();
                return;
            }

            Customer? cust = customers.FirstOrDefault(x => x.NumPhone == sdt);
            if (cust == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cboRewards.SelectedItem is RewardItem selectedReward)
            {
                if (cust.Point >= selectedReward.PointsRequired)
                {
                    cust.Point -= selectedReward.PointsRequired;
                    txtEditPoint.Text = cust.Point.ToString();

                    DataManager.SaveCustomers(customers);
                    ShowAllCustomers();
                    LoadLeaderboard();

                    MessageBox.Show(
                        $"🎁 ĐỔI QUÀ THÀNH CÔNG!\n\n" +
                        $"Khách hàng: {cust.Name} (SĐT: {cust.NumPhone})\n" +
                        $"Phần quà nhận: {selectedReward.Name}\n" +
                        $"Trừ điểm tích lũy: -{selectedReward.PointsRequired} điểm\n" +
                        $"🌟 Số điểm còn lại: {cust.Point} điểm",
                        "Đổi Quà Thành Công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    int missingPoints = selectedReward.PointsRequired - cust.Point;
                    MessageBox.Show(
                        $"Khách hàng {cust.Name} không đủ điểm để đổi quà này!\n\n" +
                        $"• Điểm hiện tại: {cust.Point} điểm\n" +
                        $"• Yêu cầu: {selectedReward.PointsRequired} điểm\n" +
                        $"• Còn thiếu: {missingPoints} điểm",
                        "Không Đủ Điểm",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        private void btnResetNewYear_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "🎆 XÁC NHẬN RESET ĐIỂM NĂM MỚI\n\n" +
                "Bạn có chắc chắn muốn RESET TOÀN BỘ ĐIỂM TÍCH LŨY của TẤT CẢ KHÁCH HÀNG về 0 để bắt đầu Niên Độ Năm Mới không?\n\n" +
                "⚠️ Thao tác này sẽ đặt điểm của tất cả khách hàng về 0!",
                "Xác Nhận Reset Năm Mới",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                foreach (var c in customers)
                {
                    c.Point = 0;
                }

                txtEditPoint.Clear();
                DataManager.SaveCustomers(customers);
                ShowAllCustomers();
                LoadLeaderboard();

                MessageBox.Show(
                    "🎆 RESET ĐIỂM NĂM MỚI THÀNH CÔNG!\n\n" +
                    "Toàn bộ điểm tích lũy của tất cả khách hàng đã được đặt về 0 để bắt đầu niên độ năm mới.",
                    "Thông Báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}
