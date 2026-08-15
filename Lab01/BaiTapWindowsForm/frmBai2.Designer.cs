namespace BaiTapWindowsForm
{
    partial class frmBai2
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
            this.components = new System.ComponentModel.Container();
            this.lblTenHang = new System.Windows.Forms.Label();
            this.lblDonGia = new System.Windows.Forms.Label();
            this.tbxDonGia = new System.Windows.Forms.TextBox();
            this.btnTinhTien = new System.Windows.Forms.Button();
            this.grbxHinhThucTT = new System.Windows.Forms.GroupBox();
            this.rbTienMat = new System.Windows.Forms.RadioButton();
            this.rbChuyenKhoan = new System.Windows.Forms.RadioButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cbbTenHang = new System.Windows.Forms.ComboBox();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.nudSoLuong = new System.Windows.Forms.NumericUpDown();
            this.lblThanhToan = new System.Windows.Forms.Label();
            this.grbxHinhThucTT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoLuong)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTenHang
            // 
            this.lblTenHang.AutoSize = true;
            this.lblTenHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenHang.Location = new System.Drawing.Point(25, 44);
            this.lblTenHang.Name = "lblTenHang";
            this.lblTenHang.Size = new System.Drawing.Size(80, 20);
            this.lblTenHang.TabIndex = 0;
            this.lblTenHang.Text = "Tên hàng:";
            // 
            // lblDonGia
            // 
            this.lblDonGia.AutoSize = true;
            this.lblDonGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDonGia.Location = new System.Drawing.Point(25, 83);
            this.lblDonGia.Name = "lblDonGia";
            this.lblDonGia.Size = new System.Drawing.Size(68, 20);
            this.lblDonGia.TabIndex = 0;
            this.lblDonGia.Text = "Đơn giá:";
            // 
            // tbxDonGia
            // 
            this.tbxDonGia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxDonGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxDonGia.Location = new System.Drawing.Point(221, 77);
            this.tbxDonGia.Name = "tbxDonGia";
            this.tbxDonGia.Size = new System.Drawing.Size(139, 26);
            this.tbxDonGia.TabIndex = 1;
            // 
            // btnTinhTien
            // 
            this.btnTinhTien.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnTinhTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTinhTien.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTinhTien.Location = new System.Drawing.Point(221, 253);
            this.btnTinhTien.Name = "btnTinhTien";
            this.btnTinhTien.Size = new System.Drawing.Size(135, 36);
            this.btnTinhTien.TabIndex = 2;
            this.btnTinhTien.Text = "Tính tiền";
            this.btnTinhTien.UseVisualStyleBackColor = false;
            this.btnTinhTien.Click += new System.EventHandler(this.btnTinhTien_Click);
            // 
            // grbxHinhThucTT
            // 
            this.grbxHinhThucTT.Controls.Add(this.rbTienMat);
            this.grbxHinhThucTT.Controls.Add(this.rbChuyenKhoan);
            this.grbxHinhThucTT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbxHinhThucTT.Location = new System.Drawing.Point(221, 160);
            this.grbxHinhThucTT.Name = "grbxHinhThucTT";
            this.grbxHinhThucTT.Size = new System.Drawing.Size(200, 87);
            this.grbxHinhThucTT.TabIndex = 3;
            this.grbxHinhThucTT.TabStop = false;
            this.grbxHinhThucTT.Text = "Hình thức thanh toán";
            // 
            // rbTienMat
            // 
            this.rbTienMat.AutoSize = true;
            this.rbTienMat.Location = new System.Drawing.Point(6, 55);
            this.rbTienMat.Name = "rbTienMat";
            this.rbTienMat.Size = new System.Drawing.Size(88, 24);
            this.rbTienMat.TabIndex = 0;
            this.rbTienMat.TabStop = true;
            this.rbTienMat.Text = "Tiền mặt";
            this.rbTienMat.UseVisualStyleBackColor = true;
            // 
            // rbChuyenKhoan
            // 
            this.rbChuyenKhoan.AutoSize = true;
            this.rbChuyenKhoan.Location = new System.Drawing.Point(6, 25);
            this.rbChuyenKhoan.Name = "rbChuyenKhoan";
            this.rbChuyenKhoan.Size = new System.Drawing.Size(129, 24);
            this.rbChuyenKhoan.TabIndex = 0;
            this.rbChuyenKhoan.TabStop = true;
            this.rbChuyenKhoan.Text = "Chuyển khoản";
            this.rbChuyenKhoan.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // cbbTenHang
            // 
            this.cbbTenHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbTenHang.FormattingEnabled = true;
            this.cbbTenHang.Items.AddRange(new object[] {
            "Chuột",
            "Máy in",
            "Bàn phím"});
            this.cbbTenHang.Location = new System.Drawing.Point(221, 36);
            this.cbbTenHang.Name = "cbbTenHang";
            this.cbbTenHang.Size = new System.Drawing.Size(139, 28);
            this.cbbTenHang.TabIndex = 0;
            this.cbbTenHang.SelectedIndexChanged += new System.EventHandler(this.cbbTenHang_SelectedIndexChanged);
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoLuong.Location = new System.Drawing.Point(25, 121);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(76, 20);
            this.lblSoLuong.TabIndex = 0;
            this.lblSoLuong.Text = "Số lượng:";
            // 
            // nudSoLuong
            // 
            this.nudSoLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudSoLuong.Location = new System.Drawing.Point(221, 115);
            this.nudSoLuong.Name = "nudSoLuong";
            this.nudSoLuong.Size = new System.Drawing.Size(139, 26);
            this.nudSoLuong.TabIndex = 2;
            // 
            // lblThanhToan
            // 
            this.lblThanhToan.AutoSize = true;
            this.lblThanhToan.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThanhToan.Location = new System.Drawing.Point(36, 377);
            this.lblThanhToan.Name = "lblThanhToan";
            this.lblThanhToan.Size = new System.Drawing.Size(171, 24);
            this.lblThanhToan.TabIndex = 9;
            this.lblThanhToan.Text = "Số tiền thanh toán: ";
            // 
            // frmBai2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblThanhToan);
            this.Controls.Add(this.nudSoLuong);
            this.Controls.Add(this.cbbTenHang);
            this.Controls.Add(this.grbxHinhThucTT);
            this.Controls.Add(this.btnTinhTien);
            this.Controls.Add(this.tbxDonGia);
            this.Controls.Add(this.lblSoLuong);
            this.Controls.Add(this.lblDonGia);
            this.Controls.Add(this.lblTenHang);
            this.Name = "frmBai2";
            this.Text = "Xếp loại";
            this.grbxHinhThucTT.ResumeLayout(false);
            this.grbxHinhThucTT.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoLuong)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTenHang;
        private System.Windows.Forms.Label lblDonGia;
        private System.Windows.Forms.TextBox tbxDonGia;
        private System.Windows.Forms.Button btnTinhTien;
        private System.Windows.Forms.GroupBox grbxHinhThucTT;
        private System.Windows.Forms.RadioButton rbTienMat;
        private System.Windows.Forms.RadioButton rbChuyenKhoan;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ComboBox cbbTenHang;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.NumericUpDown nudSoLuong;
        private System.Windows.Forms.Label lblThanhToan;
    }
}