namespace QuanLyNhaThuoc
{
    partial class FormEditCustomer
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
            lblPhone = new Label();
            txtPhone = new TextBox();
            lblName = new Label();
            txtName = new TextBox();
            lblPoints = new Label();
            nudPoints = new NumericUpDown();
            btnSave = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)nudPoints).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(50, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(265, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "CẬP NHẬT THÔNG TIN KHÁCH HÀNG";
            // 
            // lblPhone
            // 
            lblPhone.AutoSize = true;
            lblPhone.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblPhone.Location = new Point(30, 60);
            lblPhone.Name = "lblPhone";
            lblPhone.Size = new Size(94, 17);
            lblPhone.TabIndex = 1;
            lblPhone.Text = "Số điện thoại:";
            // 
            // txtPhone
            // 
            txtPhone.Font = new Font("Segoe UI", 10F);
            txtPhone.Location = new Point(135, 56);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(200, 25);
            txtPhone.TabIndex = 2;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblName.Location = new Point(30, 105);
            lblName.Name = "lblName";
            lblName.Size = new Size(72, 17);
            lblName.TabIndex = 3;
            lblName.Text = "Họ và Tên:";
            // 
            // txtName
            // 
            txtName.Font = new Font("Segoe UI", 10F);
            txtName.Location = new Point(135, 101);
            txtName.Name = "txtName";
            txtName.Size = new Size(200, 25);
            txtName.TabIndex = 4;
            // 
            // lblPoints
            // 
            lblPoints.AutoSize = true;
            lblPoints.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblPoints.Location = new Point(30, 150);
            lblPoints.Name = "lblPoints";
            lblPoints.Size = new Size(99, 17);
            lblPoints.TabIndex = 5;
            lblPoints.Text = "Điểm tích lũy:";
            // 
            // nudPoints
            // 
            nudPoints.Font = new Font("Segoe UI", 10F);
            nudPoints.Location = new Point(135, 146);
            nudPoints.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudPoints.Name = "nudPoints";
            nudPoints.Size = new Size(200, 25);
            nudPoints.TabIndex = 6;
            // 
            // btnSave
            // 
            btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSave.Location = new Point(50, 195);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(130, 35);
            btnSave.TabIndex = 7;
            btnSave.Text = "Lưu Cập Nhật";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("Segoe UI", 9.5F);
            btnCancel.Location = new Point(195, 195);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(130, 35);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Hủy Bỏ";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // FormEditCustomer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(375, 255);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(nudPoints);
            Controls.Add(lblPoints);
            Controls.Add(txtName);
            Controls.Add(lblName);
            Controls.Add(txtPhone);
            Controls.Add(lblPhone);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormEditCustomer";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cập Nhật Thông Tin Khách Hàng";
            ((System.ComponentModel.ISupportInitialize)nudPoints).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblPhone;
        private TextBox txtPhone;
        private Label lblName;
        private TextBox txtName;
        private Label lblPoints;
        private NumericUpDown nudPoints;
        private Button btnSave;
        private Button btnCancel;
    }
}
