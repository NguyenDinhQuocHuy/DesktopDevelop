namespace QuanLySinhVien
{
    partial class frmTuyChon
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
            this.gbTuyChon = new System.Windows.Forms.GroupBox();
            this.rdNgaySinh = new System.Windows.Forms.RadioButton();
            this.rdHoTen = new System.Windows.Forms.RadioButton();
            this.rdMaSV = new System.Windows.Forms.RadioButton();
            this.lblNhapThongTin = new System.Windows.Forms.Label();
            this.txtThongTin = new System.Windows.Forms.TextBox();
            this.btnTim = new System.Windows.Forms.Button();
            this.btnSapXep = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.gbTuyChon.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTuyChon
            // 
            this.gbTuyChon.Controls.Add(this.rdNgaySinh);
            this.gbTuyChon.Controls.Add(this.rdHoTen);
            this.gbTuyChon.Controls.Add(this.rdMaSV);
            this.gbTuyChon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbTuyChon.Location = new System.Drawing.Point(30, 20);
            this.gbTuyChon.Name = "gbTuyChon";
            this.gbTuyChon.Size = new System.Drawing.Size(460, 65);
            this.gbTuyChon.TabIndex = 0;
            this.gbTuyChon.TabStop = false;
            // 
            // rdNgaySinh
            // 
            this.rdNgaySinh.AutoSize = true;
            this.rdNgaySinh.Location = new System.Drawing.Point(320, 25);
            this.rdNgaySinh.Name = "rdNgaySinh";
            this.rdNgaySinh.Size = new System.Drawing.Size(106, 24);
            this.rdNgaySinh.TabIndex = 2;
            this.rdNgaySinh.Text = "Ngày Sinh";
            this.rdNgaySinh.UseVisualStyleBackColor = true;
            this.rdNgaySinh.CheckedChanged += new System.EventHandler(this.rdTuyChon_Click);
            // 
            // rdHoTen
            // 
            this.rdHoTen.AutoSize = true;
            this.rdHoTen.Location = new System.Drawing.Point(180, 25);
            this.rdHoTen.Name = "rdHoTen";
            this.rdHoTen.Size = new System.Drawing.Size(82, 24);
            this.rdHoTen.TabIndex = 1;
            this.rdHoTen.Text = "Họ Tên";
            this.rdHoTen.UseVisualStyleBackColor = true;
            this.rdHoTen.CheckedChanged += new System.EventHandler(this.rdTuyChon_Click);
            // 
            // rdMaSV
            // 
            this.rdMaSV.AutoSize = true;
            this.rdMaSV.Checked = true;
            this.rdMaSV.Location = new System.Drawing.Point(30, 25);
            this.rdMaSV.Name = "rdMaSV";
            this.rdMaSV.Size = new System.Drawing.Size(80, 24);
            this.rdMaSV.TabIndex = 0;
            this.rdMaSV.TabStop = true;
            this.rdMaSV.Text = "Mã SV";
            this.rdMaSV.UseVisualStyleBackColor = true;
            this.rdMaSV.CheckedChanged += new System.EventHandler(this.rdTuyChon_Click);
            // 
            // lblNhapThongTin
            // 
            this.lblNhapThongTin.AutoSize = true;
            this.lblNhapThongTin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNhapThongTin.Location = new System.Drawing.Point(26, 110);
            this.lblNhapThongTin.Name = "lblNhapThongTin";
            this.lblNhapThongTin.Size = new System.Drawing.Size(126, 20);
            this.lblNhapThongTin.TabIndex = 1;
            this.lblNhapThongTin.Text = "Nhập Thông tin:";
            // 
            // txtThongTin
            // 
            this.txtThongTin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtThongTin.Location = new System.Drawing.Point(170, 107);
            this.txtThongTin.Name = "txtThongTin";
            this.txtThongTin.Size = new System.Drawing.Size(210, 26);
            this.txtThongTin.TabIndex = 2;
            // 
            // btnTim
            // 
            this.btnTim.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTim.Location = new System.Drawing.Point(395, 103);
            this.btnTim.Name = "btnTim";
            this.btnTim.Size = new System.Drawing.Size(95, 34);
            this.btnTim.TabIndex = 3;
            this.btnTim.Text = "Tìm";
            this.btnTim.UseVisualStyleBackColor = true;
            this.btnTim.Click += new System.EventHandler(this.btnTim_Click);
            // 
            // btnSapXep
            // 
            this.btnSapXep.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSapXep.Location = new System.Drawing.Point(90, 160);
            this.btnSapXep.Name = "btnSapXep";
            this.btnSapXep.Size = new System.Drawing.Size(130, 38);
            this.btnSapXep.TabIndex = 4;
            this.btnSapXep.Text = "Sắp Xếp";
            this.btnSapXep.UseVisualStyleBackColor = true;
            this.btnSapXep.Click += new System.EventHandler(this.btnSapXep_Click);
            // 
            // btnThoat
            // 
            this.btnThoat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.Location = new System.Drawing.Point(290, 160);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(130, 38);
            this.btnThoat.TabIndex = 5;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // frmTuyChon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 220);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.btnSapXep);
            this.Controls.Add(this.btnTim);
            this.Controls.Add(this.txtThongTin);
            this.Controls.Add(this.lblNhapThongTin);
            this.Controls.Add(this.gbTuyChon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTuyChon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tùy chọn";
            this.gbTuyChon.ResumeLayout(false);
            this.gbTuyChon.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTuyChon;
        private System.Windows.Forms.RadioButton rdNgaySinh;
        private System.Windows.Forms.RadioButton rdHoTen;
        private System.Windows.Forms.RadioButton rdMaSV;
        private System.Windows.Forms.Label lblNhapThongTin;
        private System.Windows.Forms.TextBox txtThongTin;
        private System.Windows.Forms.Button btnTim;
        private System.Windows.Forms.Button btnSapXep;
        private System.Windows.Forms.Button btnThoat;
    }
}
