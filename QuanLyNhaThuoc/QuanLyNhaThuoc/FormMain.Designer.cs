namespace QuanLyNhaThuoc
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            pnlCartBox = new Panel();
            products = new Label();
            proBox = new ComboBox();
            lblQty = new Label();
            nudQuantity = new NumericUpDown();
            addProduct = new Button();
            menuPro = new DataGridView();
            pnlBillingSummary = new Panel();
            label2 = new Label();
            points = new Label();
            label3 = new Label();
            money = new Label();
            calc = new Button();
            pnlCustomerBox = new Panel();
            grpCustInfo = new GroupBox();
            sdt = new Label();
            sdtBox = new TextBox();
            find = new Button();
            name = new Label();
            nameBox = new TextBox();
            lblEditPoint = new Label();
            txtEditPoint = new TextBox();
            btnUpdate = new Button();
            register = new Button();
            grpCustTabs = new GroupBox();
            tabCustomer = new TabControl();
            tabList = new TabPage();
            customer = new DataGridView();
            tabLeader = new TabPage();
            leaderboard = new DataGridView();
            tabRedeem = new TabPage();
            lblChooseReward = new Label();
            cboRewards = new ComboBox();
            lblRedeemNote = new Label();
            btnRedeemReward = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            contextMenuStrip1 = new ContextMenuStrip(components);
            contextMenuStrip2 = new ContextMenuStrip(components);
            contextMenuStrip3 = new ContextMenuStrip(components);
            contextMenuStrip4 = new ContextMenuStrip(components);
            contextMenuStrip5 = new ContextMenuStrip(components);
            pnlHeader.SuspendLayout();
            pnlCartBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudQuantity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)menuPro).BeginInit();
            pnlBillingSummary.SuspendLayout();
            pnlCustomerBox.SuspendLayout();
            grpCustInfo.SuspendLayout();
            grpCustTabs.SuspendLayout();
            tabCustomer.SuspendLayout();
            tabList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)customer).BeginInit();
            tabLeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)leaderboard).BeginInit();
            tabRedeem.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.LightGray;
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1120, 45);
            pnlHeader.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.Black;
            lblHeaderTitle.Location = new Point(20, 10);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(392, 21);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "HỆ THỐNG QUẢN LÝ NHÀ THUỐC & BÁN HÀNG POS";
            // 
            // pnlCartBox
            // 
            pnlCartBox.BackColor = SystemColors.Control;
            pnlCartBox.BorderStyle = BorderStyle.FixedSingle;
            pnlCartBox.Controls.Add(products);
            pnlCartBox.Controls.Add(proBox);
            pnlCartBox.Controls.Add(lblQty);
            pnlCartBox.Controls.Add(nudQuantity);
            pnlCartBox.Controls.Add(addProduct);
            pnlCartBox.Controls.Add(menuPro);
            pnlCartBox.Controls.Add(pnlBillingSummary);
            pnlCartBox.Location = new Point(20, 60);
            pnlCartBox.Name = "pnlCartBox";
            pnlCartBox.Size = new Size(670, 555);
            pnlCartBox.TabIndex = 1;
            // 
            // products
            // 
            products.AutoSize = true;
            products.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            products.Location = new Point(15, 18);
            products.Name = "products";
            products.Size = new Size(83, 17);
            products.TabIndex = 0;
            products.Text = "Chọn thuốc:";
            products.Click += label3_Click;
            // 
            // proBox
            // 
            proBox.Font = new Font("Segoe UI", 10F);
            proBox.FormattingEnabled = true;
            proBox.Location = new Point(100, 14);
            proBox.Name = "proBox";
            proBox.Size = new Size(230, 25);
            proBox.TabIndex = 1;
            proBox.TextChanged += proBox_TextChanged;
            // 
            // lblQty
            // 
            lblQty.AutoSize = true;
            lblQty.Font = new Font("Segoe UI", 9.5F);
            lblQty.Location = new Point(345, 18);
            lblQty.Name = "lblQty";
            lblQty.Size = new Size(64, 17);
            lblQty.TabIndex = 2;
            lblQty.Text = "Số lượng:";
            // 
            // nudQuantity
            // 
            nudQuantity.Font = new Font("Segoe UI", 10F);
            nudQuantity.Location = new Point(415, 15);
            nudQuantity.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudQuantity.Name = "nudQuantity";
            nudQuantity.Size = new Size(75, 25);
            nudQuantity.TabIndex = 3;
            nudQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // addProduct
            // 
            addProduct.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            addProduct.Location = new Point(505, 12);
            addProduct.Name = "addProduct";
            addProduct.Size = new Size(148, 30);
            addProduct.TabIndex = 4;
            addProduct.Text = "Thêm Thuốc";
            addProduct.UseVisualStyleBackColor = true;
            addProduct.Click += addPro_Click;
            // 
            // menuPro
            // 
            menuPro.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            menuPro.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            menuPro.Location = new Point(15, 55);
            menuPro.Name = "menuPro";
            menuPro.ReadOnly = true;
            menuPro.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            menuPro.Size = new Size(638, 370);
            menuPro.TabIndex = 5;
            // 
            // pnlBillingSummary
            // 
            pnlBillingSummary.BackColor = SystemColors.Control;
            pnlBillingSummary.BorderStyle = BorderStyle.FixedSingle;
            pnlBillingSummary.Controls.Add(label2);
            pnlBillingSummary.Controls.Add(points);
            pnlBillingSummary.Controls.Add(label3);
            pnlBillingSummary.Controls.Add(money);
            pnlBillingSummary.Controls.Add(calc);
            pnlBillingSummary.Location = new Point(15, 435);
            pnlBillingSummary.Name = "pnlBillingSummary";
            pnlBillingSummary.Size = new Size(638, 105);
            pnlBillingSummary.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label2.ForeColor = Color.DimGray;
            label2.Location = new Point(15, 18);
            label2.Name = "label2";
            label2.Size = new Size(165, 17);
            label2.TabIndex = 0;
            label2.Text = "Điểm tích lũy nhận được:";
            // 
            // points
            // 
            points.AutoSize = true;
            points.BackColor = Color.Black;
            points.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            points.ForeColor = Color.Lime;
            points.Location = new Point(200, 12);
            points.Name = "points";
            points.Size = new Size(23, 25);
            points.TabIndex = 1;
            points.Text = "0";
            points.Click += label4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label3.Location = new Point(15, 55);
            label3.Name = "label3";
            label3.Size = new Size(141, 20);
            label3.TabIndex = 2;
            label3.Text = "Tổng tiền hóa đơn:";
            // 
            // money
            // 
            money.AutoSize = true;
            money.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            money.ForeColor = Color.Red;
            money.Location = new Point(195, 45);
            money.Name = "money";
            money.Size = new Size(43, 32);
            money.TabIndex = 3;
            money.Text = "0đ";
            money.Click += label4_Click;
            // 
            // calc
            // 
            calc.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            calc.Location = new Point(390, 15);
            calc.Name = "calc";
            calc.Size = new Size(230, 75);
            calc.TabIndex = 4;
            calc.Text = "Tính Tiền";
            calc.UseVisualStyleBackColor = true;
            calc.Click += calc_Click;
            // 
            // pnlCustomerBox
            // 
            pnlCustomerBox.BackColor = SystemColors.Control;
            pnlCustomerBox.BorderStyle = BorderStyle.FixedSingle;
            pnlCustomerBox.Controls.Add(grpCustInfo);
            pnlCustomerBox.Controls.Add(grpCustTabs);
            pnlCustomerBox.Location = new Point(710, 60);
            pnlCustomerBox.Name = "pnlCustomerBox";
            pnlCustomerBox.Size = new Size(390, 555);
            pnlCustomerBox.TabIndex = 2;
            // 
            // grpCustInfo
            // 
            grpCustInfo.Controls.Add(sdt);
            grpCustInfo.Controls.Add(sdtBox);
            grpCustInfo.Controls.Add(find);
            grpCustInfo.Controls.Add(name);
            grpCustInfo.Controls.Add(nameBox);
            grpCustInfo.Controls.Add(lblEditPoint);
            grpCustInfo.Controls.Add(txtEditPoint);
            grpCustInfo.Controls.Add(btnUpdate);
            grpCustInfo.Controls.Add(register);
            grpCustInfo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grpCustInfo.Location = new Point(15, 15);
            grpCustInfo.Name = "grpCustInfo";
            grpCustInfo.Size = new Size(358, 185);
            grpCustInfo.TabIndex = 0;
            grpCustInfo.TabStop = false;
            grpCustInfo.Text = "1. Thông Tin Khách Hàng";
            // 
            // sdt
            // 
            sdt.AutoSize = true;
            sdt.Font = new Font("Segoe UI", 9F);
            sdt.Location = new Point(15, 30);
            sdt.Name = "sdt";
            sdt.Size = new Size(79, 15);
            sdt.TabIndex = 0;
            sdt.Text = "Số điện thoại:";
            // 
            // sdtBox
            // 
            sdtBox.Font = new Font("Segoe UI", 9.5F);
            sdtBox.Location = new Point(115, 26);
            sdtBox.Name = "sdtBox";
            sdtBox.Size = new Size(140, 24);
            sdtBox.TabIndex = 1;
            sdtBox.TextChanged += sdtBox_TextChanged;
            // 
            // find
            // 
            find.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            find.Location = new Point(265, 25);
            find.Name = "find";
            find.Size = new Size(78, 27);
            find.TabIndex = 2;
            find.Text = "Tra cứu";
            find.UseVisualStyleBackColor = true;
            find.Click += find_Click;
            // 
            // name
            // 
            name.AutoSize = true;
            name.Font = new Font("Segoe UI", 9F);
            name.Location = new Point(15, 65);
            name.Name = "name";
            name.Size = new Size(63, 15);
            name.TabIndex = 3;
            name.Text = "Họ và Tên:";
            // 
            // nameBox
            // 
            nameBox.Font = new Font("Segoe UI", 9.5F);
            nameBox.Location = new Point(115, 61);
            nameBox.Name = "nameBox";
            nameBox.Size = new Size(228, 24);
            nameBox.TabIndex = 4;
            nameBox.TextChanged += nameBox_TextChanged;
            // 
            // lblEditPoint
            // 
            lblEditPoint.AutoSize = true;
            lblEditPoint.Font = new Font("Segoe UI", 9F);
            lblEditPoint.Location = new Point(15, 100);
            lblEditPoint.Name = "lblEditPoint";
            lblEditPoint.Size = new Size(86, 15);
            lblEditPoint.TabIndex = 5;
            lblEditPoint.Text = "Điểm Tích Lũy:";
            // 
            // txtEditPoint
            // 
            txtEditPoint.Font = new Font("Segoe UI", 9.5F);
            txtEditPoint.Location = new Point(115, 96);
            txtEditPoint.Name = "txtEditPoint";
            txtEditPoint.Size = new Size(110, 24);
            txtEditPoint.TabIndex = 6;
            // 
            // btnUpdate
            // 
            btnUpdate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnUpdate.Location = new Point(233, 95);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(110, 27);
            btnUpdate.TabIndex = 7;
            btnUpdate.Text = "Cập Nhật Điểm";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // register
            // 
            register.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            register.Location = new Point(15, 135);
            register.Name = "register";
            register.Size = new Size(328, 34);
            register.TabIndex = 8;
            register.Text = "Đăng Ký Khách Mới";
            register.UseVisualStyleBackColor = true;
            register.Click += register_Click;
            // 
            // grpCustTabs
            // 
            grpCustTabs.Controls.Add(tabCustomer);
            grpCustTabs.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grpCustTabs.Location = new Point(15, 210);
            grpCustTabs.Name = "grpCustTabs";
            grpCustTabs.Size = new Size(358, 330);
            grpCustTabs.TabIndex = 1;
            grpCustTabs.TabStop = false;
            grpCustTabs.Text = "2. Quản Lý Khách Hàng & VIP";
            // 
            // tabCustomer
            // 
            tabCustomer.Controls.Add(tabList);
            tabCustomer.Controls.Add(tabLeader);
            tabCustomer.Controls.Add(tabRedeem);
            tabCustomer.Dock = DockStyle.Fill;
            tabCustomer.Font = new Font("Segoe UI", 9F);
            tabCustomer.Location = new Point(3, 20);
            tabCustomer.Name = "tabCustomer";
            tabCustomer.SelectedIndex = 0;
            tabCustomer.Size = new Size(352, 307);
            tabCustomer.TabIndex = 0;
            // 
            // tabList
            // 
            tabList.Controls.Add(customer);
            tabList.Location = new Point(4, 24);
            tabList.Name = "tabList";
            tabList.Padding = new Padding(3);
            tabList.Size = new Size(344, 279);
            tabList.TabIndex = 0;
            tabList.Text = "📋 Danh Sách Khách";
            tabList.UseVisualStyleBackColor = true;
            // 
            // customer
            // 
            customer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            customer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            customer.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            customer.DefaultCellStyle = dataGridViewCellStyle2;
            customer.Dock = DockStyle.Fill;
            customer.Location = new Point(3, 3);
            customer.Name = "customer";
            customer.ReadOnly = true;
            customer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            customer.Size = new Size(338, 273);
            customer.TabIndex = 0;
            customer.CellClick += customer_CellClick;
            // 
            // tabLeader
            // 
            tabLeader.Controls.Add(leaderboard);
            tabLeader.Location = new Point(4, 24);
            tabLeader.Name = "tabLeader";
            tabLeader.Padding = new Padding(3);
            tabLeader.Size = new Size(344, 279);
            tabLeader.TabIndex = 1;
            tabLeader.Text = "🏆 Top VIP";
            tabLeader.UseVisualStyleBackColor = true;
            // 
            // leaderboard
            // 
            leaderboard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            leaderboard.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            leaderboard.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            leaderboard.DefaultCellStyle = dataGridViewCellStyle4;
            leaderboard.Dock = DockStyle.Fill;
            leaderboard.Location = new Point(3, 3);
            leaderboard.Name = "leaderboard";
            leaderboard.ReadOnly = true;
            leaderboard.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            leaderboard.Size = new Size(338, 273);
            leaderboard.TabIndex = 0;
            // 
            // tabRedeem
            // 
            tabRedeem.Controls.Add(lblChooseReward);
            tabRedeem.Controls.Add(cboRewards);
            tabRedeem.Controls.Add(lblRedeemNote);
            tabRedeem.Controls.Add(btnRedeemReward);
            tabRedeem.Location = new Point(4, 24);
            tabRedeem.Name = "tabRedeem";
            tabRedeem.Padding = new Padding(10);
            tabRedeem.Size = new Size(344, 279);
            tabRedeem.TabIndex = 2;
            tabRedeem.Text = "🎁 Đổi Quà & Reset";
            tabRedeem.UseVisualStyleBackColor = true;
            // 
            // lblChooseReward
            // 
            lblChooseReward.AutoSize = true;
            lblChooseReward.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblChooseReward.Location = new Point(10, 15);
            lblChooseReward.Name = "lblChooseReward";
            lblChooseReward.Size = new Size(135, 15);
            lblChooseReward.TabIndex = 0;
            lblChooseReward.Text = "Chọn phần quà quy đổi:";
            // 
            // cboRewards
            // 
            cboRewards.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRewards.Font = new Font("Segoe UI", 9.5F);
            cboRewards.FormattingEnabled = true;
            cboRewards.Location = new Point(10, 35);
            cboRewards.Name = "cboRewards";
            cboRewards.Size = new Size(324, 25);
            cboRewards.TabIndex = 1;
            // 
            // lblRedeemNote
            // 
            lblRedeemNote.AutoSize = true;
            lblRedeemNote.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            lblRedeemNote.ForeColor = Color.DimGray;
            lblRedeemNote.Location = new Point(10, 68);
            lblRedeemNote.Name = "lblRedeemNote";
            lblRedeemNote.Size = new Size(293, 15);
            lblRedeemNote.TabIndex = 2;
            lblRedeemNote.Text = "* Đổi quà cho khách hàng đang chọn (SĐT/Tên ở trên)";
            // 
            // btnRedeemReward
            // 
            btnRedeemReward.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRedeemReward.Location = new Point(10, 92);
            btnRedeemReward.Name = "btnRedeemReward";
            btnRedeemReward.Size = new Size(324, 38);
            btnRedeemReward.TabIndex = 3;
            btnRedeemReward.Text = "🎁 Đổi Quà Ngay";
            btnRedeemReward.UseVisualStyleBackColor = true;
            btnRedeemReward.Click += btnRedeemReward_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // contextMenuStrip2
            // 
            contextMenuStrip2.Name = "contextMenuStrip2";
            contextMenuStrip2.Size = new Size(61, 4);
            // 
            // contextMenuStrip3
            // 
            contextMenuStrip3.Name = "contextMenuStrip3";
            contextMenuStrip3.Size = new Size(61, 4);
            // 
            // contextMenuStrip4
            // 
            contextMenuStrip4.Name = "contextMenuStrip4";
            contextMenuStrip4.Size = new Size(61, 4);
            // 
            // contextMenuStrip5
            // 
            contextMenuStrip5.Name = "contextMenuStrip5";
            contextMenuStrip5.Size = new Size(61, 4);
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1120, 640);
            Controls.Add(pnlCustomerBox);
            Controls.Add(pnlCartBox);
            Controls.Add(pnlHeader);
            Name = "FormMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PHARMACY POS - HỆ THỐNG BÁN HÀNG & TÍCH ĐIỂM NHÀ THUỐC";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlCartBox.ResumeLayout(false);
            pnlCartBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudQuantity).EndInit();
            ((System.ComponentModel.ISupportInitialize)menuPro).EndInit();
            pnlBillingSummary.ResumeLayout(false);
            pnlBillingSummary.PerformLayout();
            pnlCustomerBox.ResumeLayout(false);
            grpCustInfo.ResumeLayout(false);
            grpCustInfo.PerformLayout();
            grpCustTabs.ResumeLayout(false);
            tabCustomer.ResumeLayout(false);
            tabList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)customer).EndInit();
            tabLeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)leaderboard).EndInit();
            tabRedeem.ResumeLayout(false);
            tabRedeem.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Panel pnlCartBox;
        private Label products;
        private ComboBox proBox;
        private Label lblQty;
        private NumericUpDown nudQuantity;
        private Button addProduct;
        private DataGridView menuPro;
        private Panel pnlBillingSummary;
        private Label label2;
        private Label points;
        private Label label3;
        private Label money;
        private Button calc;
        private Panel pnlCustomerBox;
        private GroupBox grpCustInfo;
        private Label sdt;
        private TextBox sdtBox;
        private Button find;
        private Label name;
        private TextBox nameBox;
        private Label lblEditPoint;
        private TextBox txtEditPoint;
        private Button btnUpdate;
        private Button register;
        private GroupBox grpCustTabs;
        private TabControl tabCustomer;
        private TabPage tabList;
        private DataGridView customer;
        private TabPage tabLeader;
        private DataGridView leaderboard;
        private TabPage tabRedeem;
        private Label lblChooseReward;
        private ComboBox cboRewards;
        private Label lblRedeemNote;
        private Button btnRedeemReward;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ContextMenuStrip contextMenuStrip1;
        private ContextMenuStrip contextMenuStrip2;
        private ContextMenuStrip contextMenuStrip3;
        private ContextMenuStrip contextMenuStrip4;
        private ContextMenuStrip contextMenuStrip5;
    }
}
