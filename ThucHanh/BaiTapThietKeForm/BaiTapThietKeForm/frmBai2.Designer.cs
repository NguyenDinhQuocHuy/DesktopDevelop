namespace BaiTapThietKeForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbDSHangHoa = new System.Windows.Forms.Label();
            this.lbMatHangKhachMua = new System.Windows.Forms.Label();
            this.lbDanhSachHangHoa = new System.Windows.Forms.ListBox();
            this.btnChonHang = new System.Windows.Forms.Button();
            this.btnTraHang = new System.Windows.Forms.Button();
            this.lbKhachMua = new System.Windows.Forms.ListBox();
            this.btnTinhTien = new System.Windows.Forms.Button();
            this.lbThanhToan = new System.Windows.Forms.Label();
            this.lbSoTien = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnChonHang);
            this.panel1.Controls.Add(this.lbDanhSachHangHoa);
            this.panel1.Controls.Add(this.lbDSHangHoa);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(530, 589);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.btnTinhTien);
            this.panel2.Controls.Add(this.btnTraHang);
            this.panel2.Controls.Add(this.lbSoTien);
            this.panel2.Controls.Add(this.lbThanhToan);
            this.panel2.Controls.Add(this.lbKhachMua);
            this.panel2.Controls.Add(this.lbMatHangKhachMua);
            this.panel2.Location = new System.Drawing.Point(530, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(511, 589);
            this.panel2.TabIndex = 1;
            
            //
            // lbDSHangHoa
            // 
            this.lbDSHangHoa.AutoSize = true;
            this.lbDSHangHoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDSHangHoa.Location = new System.Drawing.Point(91, 20);
            this.lbDSHangHoa.Name = "lbDSHangHoa";
            this.lbDSHangHoa.Size = new System.Drawing.Size(230, 29);
            this.lbDSHangHoa.TabIndex = 0;
            this.lbDSHangHoa.Text = "Danh sách hàng hóa";
            // 
            // lbMatHangKhachMua
            // 
            this.lbMatHangKhachMua.AutoSize = true;
            this.lbMatHangKhachMua.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMatHangKhachMua.Location = new System.Drawing.Point(172, 20);
            this.lbMatHangKhachMua.Name = "lbMatHangKhachMua";
            this.lbMatHangKhachMua.Size = new System.Drawing.Size(280, 29);
            this.lbMatHangKhachMua.TabIndex = 0;
            this.lbMatHangKhachMua.Text = "Các mặt hàng khách mua";
            // 
            // lbDanhSachHangHoa
            // 
            this.lbDanhSachHangHoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDanhSachHangHoa.FormattingEnabled = true;
            this.lbDanhSachHangHoa.ItemHeight = 25;
            this.lbDanhSachHangHoa.Items.AddRange(new object[] {
            "Chuột",
            "Bàn phím",
            "Máy in",
            "USB Kingmax"});
            this.lbDanhSachHangHoa.Location = new System.Drawing.Point(19, 85);
            this.lbDanhSachHangHoa.Name = "lbDanhSachHangHoa";
            this.lbDanhSachHangHoa.Size = new System.Drawing.Size(360, 254);
            this.lbDanhSachHangHoa.TabIndex = 1;
            // 
            // btnChonHang
            // 
            this.btnChonHang.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnChonHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChonHang.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnChonHang.Location = new System.Drawing.Point(385, 203);
            this.btnChonHang.Name = "btnChonHang";
            this.btnChonHang.Size = new System.Drawing.Size(124, 36);
            this.btnChonHang.TabIndex = 2;
            this.btnChonHang.Text = "Chọn hàng ";
            this.btnChonHang.UseVisualStyleBackColor = false;
            this.btnChonHang.Click += new System.EventHandler(this.btnChonHang_Click);
            // 
            // btnTraHang
            // 
            this.btnTraHang.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnTraHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTraHang.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTraHang.Location = new System.Drawing.Point(18, 203);
            this.btnTraHang.Name = "btnTraHang";
            this.btnTraHang.Size = new System.Drawing.Size(124, 36);
            this.btnTraHang.TabIndex = 2;
            this.btnTraHang.Text = "Trả hàng";
            this.btnTraHang.UseVisualStyleBackColor = false;
            // 
            // lbKhachMua
            // 
            this.lbKhachMua.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbKhachMua.FormattingEnabled = true;
            this.lbKhachMua.ItemHeight = 25;
            this.lbKhachMua.Location = new System.Drawing.Point(148, 85);
            this.lbKhachMua.Name = "lbKhachMua";
            this.lbKhachMua.Size = new System.Drawing.Size(360, 254);
            this.lbKhachMua.TabIndex = 1;
            // 
            // btnTinhTien
            // 
            this.btnTinhTien.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnTinhTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTinhTien.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTinhTien.Location = new System.Drawing.Point(256, 372);
            this.btnTinhTien.Name = "btnTinhTien";
            this.btnTinhTien.Size = new System.Drawing.Size(124, 36);
            this.btnTinhTien.TabIndex = 2;
            this.btnTinhTien.Text = "Tính tiền";
            this.btnTinhTien.UseVisualStyleBackColor = false;
            // 
            // lbThanhToan
            // 
            this.lbThanhToan.AutoSize = true;
            this.lbThanhToan.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbThanhToan.Location = new System.Drawing.Point(18, 534);
            this.lbThanhToan.Name = "lbThanhToan";
            this.lbThanhToan.Size = new System.Drawing.Size(237, 29);
            this.lbThanhToan.TabIndex = 0;
            this.lbThanhToan.Text = "Tổng tiền thanh toán:";
            // 
            // lbSoTien
            // 
            this.lbSoTien.AutoSize = true;
            this.lbSoTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSoTien.Location = new System.Drawing.Point(251, 534);
            this.lbSoTien.Name = "lbSoTien";
            this.lbSoTien.Size = new System.Drawing.Size(0, 29);
            this.lbSoTien.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.HighlightText;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(280, 541);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(186, 15);
            this.textBox1.TabIndex = 3;
            // 
            // frmBai2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1053, 613);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmBai2";
            this.Text = "frmBai2";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbDSHangHoa;
        private System.Windows.Forms.Label lbMatHangKhachMua;
        private System.Windows.Forms.ListBox lbDanhSachHangHoa;
        private System.Windows.Forms.Button btnChonHang;
        private System.Windows.Forms.Button btnTraHang;
        private System.Windows.Forms.ListBox lbKhachMua;
        private System.Windows.Forms.Button btnTinhTien;
        private System.Windows.Forms.Label lbThanhToan;
        private System.Windows.Forms.Label lbSoTien;
        private System.Windows.Forms.TextBox textBox1;
    }
}