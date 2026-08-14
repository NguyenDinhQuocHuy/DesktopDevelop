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
            this.btnChonHang = new System.Windows.Forms.Button();
            this.lbDanhSachHangHoa = new System.Windows.Forms.ListBox();
            this.lbDSHangHoa = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbTongTien = new System.Windows.Forms.TextBox();
            this.btnTinhTien = new System.Windows.Forms.Button();
            this.btnTraHang = new System.Windows.Forms.Button();
            this.lbSoTien = new System.Windows.Forms.Label();
            this.lbThanhToan = new System.Windows.Forms.Label();
            this.lbKhachMua = new System.Windows.Forms.ListBox();
            this.lbMatHangKhachMua = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnChonHang);
            this.panel1.Controls.Add(this.lbDanhSachHangHoa);
            this.panel1.Controls.Add(this.lbDSHangHoa);
            this.panel1.Location = new System.Drawing.Point(9, 10);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(398, 479);
            this.panel1.TabIndex = 0;
            // 
            // btnChonHang
            // 
            this.btnChonHang.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnChonHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChonHang.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnChonHang.Location = new System.Drawing.Point(289, 165);
            this.btnChonHang.Margin = new System.Windows.Forms.Padding(2);
            this.btnChonHang.Name = "btnChonHang";
            this.btnChonHang.Size = new System.Drawing.Size(93, 29);
            this.btnChonHang.TabIndex = 2;
            this.btnChonHang.Text = "Chọn hàng ";
            this.btnChonHang.UseVisualStyleBackColor = false;
            this.btnChonHang.Click += new System.EventHandler(this.btnChonHang_Click);
            // 
            // lbDanhSachHangHoa
            // 
            this.lbDanhSachHangHoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDanhSachHangHoa.FormattingEnabled = true;
            this.lbDanhSachHangHoa.ItemHeight = 20;
            this.lbDanhSachHangHoa.Items.AddRange(new object[] {
            "Chuột",
            "Bàn phím",
            "Máy in",
            "USB Kingmax"});
            this.lbDanhSachHangHoa.Location = new System.Drawing.Point(14, 69);
            this.lbDanhSachHangHoa.Margin = new System.Windows.Forms.Padding(2);
            this.lbDanhSachHangHoa.Name = "lbDanhSachHangHoa";
            this.lbDanhSachHangHoa.Size = new System.Drawing.Size(271, 204);
            this.lbDanhSachHangHoa.TabIndex = 1;
            // 
            // lbDSHangHoa
            // 
            this.lbDSHangHoa.AutoSize = true;
            this.lbDSHangHoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDSHangHoa.Location = new System.Drawing.Point(68, 16);
            this.lbDSHangHoa.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDSHangHoa.Name = "lbDSHangHoa";
            this.lbDSHangHoa.Size = new System.Drawing.Size(185, 24);
            this.lbDSHangHoa.TabIndex = 0;
            this.lbDSHangHoa.Text = "Danh sách hàng hóa";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tbTongTien);
            this.panel2.Controls.Add(this.btnTinhTien);
            this.panel2.Controls.Add(this.btnTraHang);
            this.panel2.Controls.Add(this.lbSoTien);
            this.panel2.Controls.Add(this.lbThanhToan);
            this.panel2.Controls.Add(this.lbKhachMua);
            this.panel2.Controls.Add(this.lbMatHangKhachMua);
            this.panel2.Location = new System.Drawing.Point(398, 10);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(383, 479);
            this.panel2.TabIndex = 1;
            // 
            // tbTongTien
            // 
            this.tbTongTien.BackColor = System.Drawing.SystemColors.HighlightText;
            this.tbTongTien.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbTongTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTongTien.Location = new System.Drawing.Point(210, 440);
            this.tbTongTien.Margin = new System.Windows.Forms.Padding(2);
            this.tbTongTien.Name = "tbTongTien";
            this.tbTongTien.ReadOnly = true;
            this.tbTongTien.Size = new System.Drawing.Size(140, 22);
            this.tbTongTien.TabIndex = 3;
            // 
            // btnTinhTien
            // 
            this.btnTinhTien.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnTinhTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTinhTien.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTinhTien.Location = new System.Drawing.Point(192, 302);
            this.btnTinhTien.Margin = new System.Windows.Forms.Padding(2);
            this.btnTinhTien.Name = "btnTinhTien";
            this.btnTinhTien.Size = new System.Drawing.Size(93, 29);
            this.btnTinhTien.TabIndex = 2;
            this.btnTinhTien.Text = "Tính tiền";
            this.btnTinhTien.UseVisualStyleBackColor = false;
            this.btnTinhTien.Click += new System.EventHandler(this.btnTinhTien_Click);
            // 
            // btnTraHang
            // 
            this.btnTraHang.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnTraHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTraHang.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTraHang.Location = new System.Drawing.Point(14, 165);
            this.btnTraHang.Margin = new System.Windows.Forms.Padding(2);
            this.btnTraHang.Name = "btnTraHang";
            this.btnTraHang.Size = new System.Drawing.Size(93, 29);
            this.btnTraHang.TabIndex = 2;
            this.btnTraHang.Text = "Trả hàng";
            this.btnTraHang.UseVisualStyleBackColor = false;
            this.btnTraHang.Click += new System.EventHandler(this.btnTraHang_Click);
            // 
            // lbSoTien
            // 
            this.lbSoTien.AutoSize = true;
            this.lbSoTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSoTien.Location = new System.Drawing.Point(188, 434);
            this.lbSoTien.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbSoTien.Name = "lbSoTien";
            this.lbSoTien.Size = new System.Drawing.Size(0, 24);
            this.lbSoTien.TabIndex = 0;
            // 
            // lbThanhToan
            // 
            this.lbThanhToan.AutoSize = true;
            this.lbThanhToan.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbThanhToan.Location = new System.Drawing.Point(14, 434);
            this.lbThanhToan.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbThanhToan.Name = "lbThanhToan";
            this.lbThanhToan.Size = new System.Drawing.Size(188, 24);
            this.lbThanhToan.TabIndex = 0;
            this.lbThanhToan.Text = "Tổng tiền thanh toán:";
            // 
            // lbKhachMua
            // 
            this.lbKhachMua.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbKhachMua.FormattingEnabled = true;
            this.lbKhachMua.ItemHeight = 20;
            this.lbKhachMua.Location = new System.Drawing.Point(111, 69);
            this.lbKhachMua.Margin = new System.Windows.Forms.Padding(2);
            this.lbKhachMua.Name = "lbKhachMua";
            this.lbKhachMua.Size = new System.Drawing.Size(271, 204);
            this.lbKhachMua.TabIndex = 1;
            // 
            // lbMatHangKhachMua
            // 
            this.lbMatHangKhachMua.AutoSize = true;
            this.lbMatHangKhachMua.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMatHangKhachMua.Location = new System.Drawing.Point(129, 16);
            this.lbMatHangKhachMua.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbMatHangKhachMua.Name = "lbMatHangKhachMua";
            this.lbMatHangKhachMua.Size = new System.Drawing.Size(224, 24);
            this.lbMatHangKhachMua.TabIndex = 0;
            this.lbMatHangKhachMua.Text = "Các mặt hàng khách mua";
            // 
            // frmBai2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(790, 498);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
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
        private System.Windows.Forms.TextBox tbTongTien;
    }
}