namespace TinhTienHocTrungTam
{
    partial class Demo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Demo));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMaHocVien = new System.Windows.Forms.Label();
            this.lblGioiTinh = new System.Windows.Forms.Label();
            this.lblHoTen = new System.Windows.Forms.Label();
            this.cboMaHV = new System.Windows.Forms.ComboBox();
            this.rdNam = new System.Windows.Forms.RadioButton();
            this.rdNu = new System.Windows.Forms.RadioButton();
            this.txtHoTen = new System.Windows.Forms.TextBox();
            this.lblNgayDangKy = new System.Windows.Forms.Label();
            this.dtpNgayDangKy = new System.Windows.Forms.DateTimePicker();
            this.chkTinHocA = new System.Windows.Forms.CheckBox();
            this.chkTinHocB = new System.Windows.Forms.CheckBox();
            this.chkTiengAnhA = new System.Windows.Forms.CheckBox();
            this.chkTiengAnhB = new System.Windows.Forms.CheckBox();
            this.lblTienTHA = new System.Windows.Forms.Label();
            this.lblTienTHB = new System.Windows.Forms.Label();
            this.lblTienTAA = new System.Windows.Forms.Label();
            this.lblTienTAB = new System.Windows.Forms.Label();
            this.lblTongTien = new System.Windows.Forms.Label();
            this.txtTongTien = new System.Windows.Forms.TextBox();
            this.btnTinhTien = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(196, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(412, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TÍNH TIỀN HỌC TRUNG TÂM";
            // 
            // lblMaHocVien
            // 
            this.lblMaHocVien.AutoSize = true;
            this.lblMaHocVien.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaHocVien.Location = new System.Drawing.Point(33, 93);
            this.lblMaHocVien.Name = "lblMaHocVien";
            this.lblMaHocVien.Size = new System.Drawing.Size(129, 25);
            this.lblMaHocVien.TabIndex = 1;
            this.lblMaHocVien.Text = "Mã học viên";
            // 
            // lblGioiTinh
            // 
            this.lblGioiTinh.AutoSize = true;
            this.lblGioiTinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGioiTinh.Location = new System.Drawing.Point(504, 93);
            this.lblGioiTinh.Name = "lblGioiTinh";
            this.lblGioiTinh.Size = new System.Drawing.Size(91, 25);
            this.lblGioiTinh.TabIndex = 1;
            this.lblGioiTinh.Text = "Giới tính";
            // 
            // lblHoTen
            // 
            this.lblHoTen.AutoSize = true;
            this.lblHoTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoTen.Location = new System.Drawing.Point(33, 166);
            this.lblHoTen.Name = "lblHoTen";
            this.lblHoTen.Size = new System.Drawing.Size(75, 25);
            this.lblHoTen.TabIndex = 1;
            this.lblHoTen.Text = "Họ tên";
            // 
            // cboMaHV
            // 
            this.cboMaHV.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMaHV.FormattingEnabled = true;
            this.cboMaHV.Items.AddRange(new object[] {
            "001",
            "002",
            "003",
            "004",
            "005",
            "006",
            "007",
            "008",
            "009",
            "010"});
            this.cboMaHV.Location = new System.Drawing.Point(202, 85);
            this.cboMaHV.Name = "cboMaHV";
            this.cboMaHV.Size = new System.Drawing.Size(254, 33);
            this.cboMaHV.TabIndex = 0;
            // 
            // rdNam
            // 
            this.rdNam.AutoSize = true;
            this.rdNam.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdNam.Location = new System.Drawing.Point(621, 91);
            this.rdNam.Name = "rdNam";
            this.rdNam.Size = new System.Drawing.Size(74, 29);
            this.rdNam.TabIndex = 1;
            this.rdNam.TabStop = true;
            this.rdNam.Text = "Nam";
            this.rdNam.UseVisualStyleBackColor = true;
            // 
            // rdNu
            // 
            this.rdNu.AutoSize = true;
            this.rdNu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdNu.Location = new System.Drawing.Point(717, 91);
            this.rdNu.Name = "rdNu";
            this.rdNu.Size = new System.Drawing.Size(58, 29);
            this.rdNu.TabIndex = 2;
            this.rdNu.TabStop = true;
            this.rdNu.Text = "Nữ";
            this.rdNu.UseVisualStyleBackColor = true;
            // 
            // txtHoTen
            // 
            this.txtHoTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHoTen.Location = new System.Drawing.Point(202, 161);
            this.txtHoTen.Name = "txtHoTen";
            this.txtHoTen.Size = new System.Drawing.Size(573, 30);
            this.txtHoTen.TabIndex = 3;
            // 
            // lblNgayDangKy
            // 
            this.lblNgayDangKy.AutoSize = true;
            this.lblNgayDangKy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNgayDangKy.Location = new System.Drawing.Point(33, 233);
            this.lblNgayDangKy.Name = "lblNgayDangKy";
            this.lblNgayDangKy.Size = new System.Drawing.Size(144, 25);
            this.lblNgayDangKy.TabIndex = 1;
            this.lblNgayDangKy.Text = "Ngày đăng ký";
            // 
            // dtpNgayDangKy
            // 
            this.dtpNgayDangKy.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpNgayDangKy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpNgayDangKy.Location = new System.Drawing.Point(202, 236);
            this.dtpNgayDangKy.Name = "dtpNgayDangKy";
            this.dtpNgayDangKy.Size = new System.Drawing.Size(393, 30);
            this.dtpNgayDangKy.TabIndex = 4;
            // 
            // chkTinHocA
            // 
            this.chkTinHocA.AutoSize = true;
            this.chkTinHocA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTinHocA.Location = new System.Drawing.Point(202, 306);
            this.chkTinHocA.Name = "chkTinHocA";
            this.chkTinHocA.Size = new System.Drawing.Size(118, 29);
            this.chkTinHocA.TabIndex = 5;
            this.chkTinHocA.Text = "Tin học A";
            this.chkTinHocA.UseVisualStyleBackColor = true;
            // 
            // chkTinHocB
            // 
            this.chkTinHocB.AutoSize = true;
            this.chkTinHocB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTinHocB.Location = new System.Drawing.Point(202, 341);
            this.chkTinHocB.Name = "chkTinHocB";
            this.chkTinHocB.Size = new System.Drawing.Size(117, 29);
            this.chkTinHocB.TabIndex = 6;
            this.chkTinHocB.Text = "Tin học B";
            this.chkTinHocB.UseVisualStyleBackColor = true;
            // 
            // chkTiengAnhA
            // 
            this.chkTiengAnhA.AutoSize = true;
            this.chkTiengAnhA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTiengAnhA.Location = new System.Drawing.Point(202, 376);
            this.chkTiengAnhA.Name = "chkTiengAnhA";
            this.chkTiengAnhA.Size = new System.Drawing.Size(144, 29);
            this.chkTiengAnhA.TabIndex = 7;
            this.chkTiengAnhA.Text = "Tiếng Anh A";
            this.chkTiengAnhA.UseVisualStyleBackColor = true;
            // 
            // chkTiengAnhB
            // 
            this.chkTiengAnhB.AutoSize = true;
            this.chkTiengAnhB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTiengAnhB.Location = new System.Drawing.Point(202, 411);
            this.chkTiengAnhB.Name = "chkTiengAnhB";
            this.chkTiengAnhB.Size = new System.Drawing.Size(143, 29);
            this.chkTiengAnhB.TabIndex = 8;
            this.chkTiengAnhB.Text = "Tiếng Anh B";
            this.chkTiengAnhB.UseVisualStyleBackColor = true;
            // 
            // lblTienTHA
            // 
            this.lblTienTHA.AutoSize = true;
            this.lblTienTHA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTienTHA.Location = new System.Drawing.Point(463, 306);
            this.lblTienTHA.Name = "lblTienTHA";
            this.lblTienTHA.Size = new System.Drawing.Size(132, 25);
            this.lblTienTHA.TabIndex = 7;
            this.lblTienTHA.Text = "300.000 đồng";
            // 
            // lblTienTHB
            // 
            this.lblTienTHB.AutoSize = true;
            this.lblTienTHB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTienTHB.Location = new System.Drawing.Point(463, 341);
            this.lblTienTHB.Name = "lblTienTHB";
            this.lblTienTHB.Size = new System.Drawing.Size(132, 25);
            this.lblTienTHB.TabIndex = 7;
            this.lblTienTHB.Text = "500.000 đồng";
            // 
            // lblTienTAA
            // 
            this.lblTienTAA.AutoSize = true;
            this.lblTienTAA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTienTAA.Location = new System.Drawing.Point(463, 376);
            this.lblTienTAA.Name = "lblTienTAA";
            this.lblTienTAA.Size = new System.Drawing.Size(132, 25);
            this.lblTienTAA.TabIndex = 7;
            this.lblTienTAA.Text = "400.000 đồng";
            // 
            // lblTienTAB
            // 
            this.lblTienTAB.AutoSize = true;
            this.lblTienTAB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTienTAB.Location = new System.Drawing.Point(463, 411);
            this.lblTienTAB.Name = "lblTienTAB";
            this.lblTienTAB.Size = new System.Drawing.Size(132, 25);
            this.lblTienTAB.TabIndex = 7;
            this.lblTienTAB.Text = "600.000 đồng";
            // 
            // lblTongTien
            // 
            this.lblTongTien.AutoSize = true;
            this.lblTongTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongTien.Location = new System.Drawing.Point(197, 467);
            this.lblTongTien.Name = "lblTongTien";
            this.lblTongTien.Size = new System.Drawing.Size(103, 25);
            this.lblTongTien.TabIndex = 1;
            this.lblTongTien.Text = "Tổng tiền";
            // 
            // txtTongTien
            // 
            this.txtTongTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTongTien.Location = new System.Drawing.Point(334, 464);
            this.txtTongTien.Name = "txtTongTien";
            this.txtTongTien.ReadOnly = true;
            this.txtTongTien.Size = new System.Drawing.Size(261, 30);
            this.txtTongTien.TabIndex = 9;
            // 
            // btnTinhTien
            // 
            this.btnTinhTien.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnTinhTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTinhTien.Location = new System.Drawing.Point(38, 527);
            this.btnTinhTien.Name = "btnTinhTien";
            this.btnTinhTien.Size = new System.Drawing.Size(200, 68);
            this.btnTinhTien.TabIndex = 8;
            this.btnTinhTien.Text = "TÍNH TIỀN";
            this.btnTinhTien.UseVisualStyleBackColor = false;
            this.btnTinhTien.Click += new System.EventHandler(this.btnTinhTien_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(266, 527);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(279, 68);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(588, 527);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(199, 68);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 622);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTinhTien);
            this.Controls.Add(this.lblTienTAB);
            this.Controls.Add(this.lblTienTAA);
            this.Controls.Add(this.lblTienTHB);
            this.Controls.Add(this.lblTienTHA);
            this.Controls.Add(this.chkTiengAnhB);
            this.Controls.Add(this.chkTiengAnhA);
            this.Controls.Add(this.chkTinHocB);
            this.Controls.Add(this.chkTinHocA);
            this.Controls.Add(this.dtpNgayDangKy);
            this.Controls.Add(this.txtTongTien);
            this.Controls.Add(this.txtHoTen);
            this.Controls.Add(this.rdNu);
            this.Controls.Add(this.rdNam);
            this.Controls.Add(this.cboMaHV);
            this.Controls.Add(this.lblGioiTinh);
            this.Controls.Add(this.lblTongTien);
            this.Controls.Add(this.lblNgayDangKy);
            this.Controls.Add(this.lblHoTen);
            this.Controls.Add(this.lblMaHocVien);
            this.Controls.Add(this.lblTitle);
            this.Name = "Demo";
            this.Text = "Demo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMaHocVien;
        private System.Windows.Forms.Label lblGioiTinh;
        private System.Windows.Forms.Label lblHoTen;
        private System.Windows.Forms.ComboBox cboMaHV;
        private System.Windows.Forms.RadioButton rdNam;
        private System.Windows.Forms.RadioButton rdNu;
        private System.Windows.Forms.TextBox txtHoTen;
        private System.Windows.Forms.Label lblNgayDangKy;
        private System.Windows.Forms.DateTimePicker dtpNgayDangKy;
        private System.Windows.Forms.CheckBox chkTinHocA;
        private System.Windows.Forms.CheckBox chkTinHocB;
        private System.Windows.Forms.CheckBox chkTiengAnhA;
        private System.Windows.Forms.CheckBox chkTiengAnhB;
        private System.Windows.Forms.Label lblTienTHA;
        private System.Windows.Forms.Label lblTienTHB;
        private System.Windows.Forms.Label lblTienTAA;
        private System.Windows.Forms.Label lblTienTAB;
        private System.Windows.Forms.Label lblTongTien;
        private System.Windows.Forms.TextBox txtTongTien;
        private System.Windows.Forms.Button btnTinhTien;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
    }
}

