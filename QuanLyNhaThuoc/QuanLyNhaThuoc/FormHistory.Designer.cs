namespace QuanLyNhaThuoc
{
    partial class FormHistory
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblCustomerInfo = new Label();
            dgvHistory = new DataGridView();
            btnClose = new Button();
            lblSummaryStats = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(297, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📜 LỊCH SỬ MUA HÀNG KHÁCH HÀNG";
            // 
            // lblCustomerInfo
            // 
            lblCustomerInfo.AutoSize = true;
            lblCustomerInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCustomerInfo.ForeColor = Color.DarkBlue;
            lblCustomerInfo.Location = new Point(20, 45);
            lblCustomerInfo.Name = "lblCustomerInfo";
            lblCustomerInfo.Size = new Size(181, 19);
            lblCustomerInfo.TabIndex = 1;
            lblCustomerInfo.Text = "Khách hàng: Nguyễn Văn A";
            // 
            // dgvHistory
            // 
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistory.Location = new Point(20, 75);
            dgvHistory.Name = "dgvHistory";
            dgvHistory.ReadOnly = true;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.Size = new Size(740, 360);
            dgvHistory.TabIndex = 2;
            // 
            // btnClose
            // 
            btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnClose.Location = new Point(640, 445);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(120, 35);
            btnClose.TabIndex = 3;
            btnClose.Text = "Đóng";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblSummaryStats
            // 
            lblSummaryStats.AutoSize = true;
            lblSummaryStats.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblSummaryStats.Location = new Point(20, 453);
            lblSummaryStats.Name = "lblSummaryStats";
            lblSummaryStats.Size = new Size(244, 17);
            lblSummaryStats.TabIndex = 4;
            lblSummaryStats.Text = "Tổng số hóa đơn: 0 | Tổng tiền: 0đ";
            // 
            // FormHistory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(780, 495);
            Controls.Add(lblSummaryStats);
            Controls.Add(btnClose);
            Controls.Add(dgvHistory);
            Controls.Add(lblCustomerInfo);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormHistory";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Lịch Sử Mua Hàng Khách Hàng";
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblCustomerInfo;
        private DataGridView dgvHistory;
        private Button btnClose;
        private Label lblSummaryStats;
    }
}
