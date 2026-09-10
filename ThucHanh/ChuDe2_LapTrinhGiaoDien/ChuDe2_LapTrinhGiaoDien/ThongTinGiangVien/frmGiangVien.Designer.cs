namespace ThongTinGiangVien
{
    partial class frmGiangVien
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGiangVien));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMaSo = new System.Windows.Forms.Label();
            this.cboMaSo = new System.Windows.Forms.ComboBox();
            this.lblGioiTinh = new System.Windows.Forms.Label();
            this.rdNam = new System.Windows.Forms.RadioButton();
            this.rdNu = new System.Windows.Forms.RadioButton();
            this.lblHoTen = new System.Windows.Forms.Label();
            this.txtHoTen = new System.Windows.Forms.TextBox();
            this.lblSoDT = new System.Windows.Forms.Label();
            this.mtxtSoDT = new System.Windows.Forms.MaskedTextBox();
            this.lblNgaySinh = new System.Windows.Forms.Label();
            this.dtpNgaySinh = new System.Windows.Forms.DateTimePicker();
            this.lblMail = new System.Windows.Forms.Label();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.lblNgoaiNgu = new System.Windows.Forms.Label();
            this.chklbNgoaiNgu = new System.Windows.Forms.CheckedListBox();
            this.lblDSMH = new System.Windows.Forms.Label();
            this.lblMHGVD = new System.Windows.Forms.Label();
            this.lbDanhSachHP = new System.Windows.Forms.ListBox();
            this.lbHocPhanDay = new System.Windows.Forms.ListBox();
            this.btnChon = new System.Windows.Forms.Button();
            this.btnXoa = new System.Windows.Forms.Button();
            this.btnThongBao = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblWebsite = new System.Windows.Forms.Label();
            this.linklbLienHe = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.MediumOrchid;
            this.lblTitle.Location = new System.Drawing.Point(85, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(517, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "THÔNG TIN GIẢNG VIÊN KHOA CNTT";
            // 
            // lblMaSo
            // 
            this.lblMaSo.AutoSize = true;
            this.lblMaSo.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaSo.Location = new System.Drawing.Point(50, 68);
            this.lblMaSo.Name = "lblMaSo";
            this.lblMaSo.Size = new System.Drawing.Size(57, 19);
            this.lblMaSo.TabIndex = 1;
            this.lblMaSo.Text = "Mã Số";
            // 
            // cboMaSo
            // 
            this.cboMaSo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaSo.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMaSo.FormattingEnabled = true;
            this.cboMaSo.Items.AddRange(new object[] {
            "001",
            "002",
            "003",
            "004"});
            this.cboMaSo.Location = new System.Drawing.Point(145, 65);
            this.cboMaSo.Name = "cboMaSo";
            this.cboMaSo.Size = new System.Drawing.Size(130, 27);
            this.cboMaSo.TabIndex = 2;
            // 
            // lblGioiTinh
            // 
            this.lblGioiTinh.AutoSize = true;
            this.lblGioiTinh.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGioiTinh.Location = new System.Drawing.Point(365, 68);
            this.lblGioiTinh.Name = "lblGioiTinh";
            this.lblGioiTinh.Size = new System.Drawing.Size(82, 19);
            this.lblGioiTinh.TabIndex = 3;
            this.lblGioiTinh.Text = "Giới Tính";
            // 
            // rdNam
            // 
            this.rdNam.AutoSize = true;
            this.rdNam.Checked = true;
            this.rdNam.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdNam.Location = new System.Drawing.Point(460, 66);
            this.rdNam.Name = "rdNam";
            this.rdNam.Size = new System.Drawing.Size(62, 23);
            this.rdNam.TabIndex = 4;
            this.rdNam.TabStop = true;
            this.rdNam.Text = "Nam";
            this.rdNam.UseVisualStyleBackColor = true;
            // 
            // rdNu
            // 
            this.rdNu.AutoSize = true;
            this.rdNu.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdNu.Location = new System.Drawing.Point(530, 66);
            this.rdNu.Name = "rdNu";
            this.rdNu.Size = new System.Drawing.Size(51, 23);
            this.rdNu.TabIndex = 5;
            this.rdNu.Text = "Nữ";
            this.rdNu.UseVisualStyleBackColor = true;
            // 
            // lblHoTen
            // 
            this.lblHoTen.AutoSize = true;
            this.lblHoTen.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoTen.Location = new System.Drawing.Point(50, 108);
            this.lblHoTen.Name = "lblHoTen";
            this.lblHoTen.Size = new System.Drawing.Size(62, 19);
            this.lblHoTen.TabIndex = 6;
            this.lblHoTen.Text = "Họ Tên";
            // 
            // txtHoTen
            // 
            this.txtHoTen.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHoTen.Location = new System.Drawing.Point(145, 105);
            this.txtHoTen.Name = "txtHoTen";
            this.txtHoTen.Size = new System.Drawing.Size(195, 27);
            this.txtHoTen.TabIndex = 7;
            // 
            // lblSoDT
            // 
            this.lblSoDT.AutoSize = true;
            this.lblSoDT.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoDT.Location = new System.Drawing.Point(365, 108);
            this.lblSoDT.Name = "lblSoDT";
            this.lblSoDT.Size = new System.Drawing.Size(55, 19);
            this.lblSoDT.TabIndex = 8;
            this.lblSoDT.Text = "Số ĐT";
            // 
            // mtxtSoDT
            // 
            this.mtxtSoDT.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtxtSoDT.Location = new System.Drawing.Point(460, 105);
            this.mtxtSoDT.Mask = "(\\0633).000.000";
            this.mtxtSoDT.Name = "mtxtSoDT";
            this.mtxtSoDT.Size = new System.Drawing.Size(145, 27);
            this.mtxtSoDT.TabIndex = 9;
            // 
            // lblNgaySinh
            // 
            this.lblNgaySinh.AutoSize = true;
            this.lblNgaySinh.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNgaySinh.Location = new System.Drawing.Point(50, 148);
            this.lblNgaySinh.Name = "lblNgaySinh";
            this.lblNgaySinh.Size = new System.Drawing.Size(81, 19);
            this.lblNgaySinh.TabIndex = 10;
            this.lblNgaySinh.Text = "Ngày Sinh";
            // 
            // dtpNgaySinh
            // 
            this.dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
            this.dtpNgaySinh.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpNgaySinh.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgaySinh.Location = new System.Drawing.Point(145, 145);
            this.dtpNgaySinh.Name = "dtpNgaySinh";
            this.dtpNgaySinh.Size = new System.Drawing.Size(175, 27);
            this.dtpNgaySinh.TabIndex = 11;
            // 
            // lblMail
            // 
            this.lblMail.AutoSize = true;
            this.lblMail.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMail.Location = new System.Drawing.Point(365, 148);
            this.lblMail.Name = "lblMail";
            this.lblMail.Size = new System.Drawing.Size(95, 19);
            this.lblMail.TabIndex = 12;
            this.lblMail.Text = "Địa chỉ mail";
            // 
            // txtMail
            // 
            this.txtMail.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMail.Location = new System.Drawing.Point(460, 145);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(205, 27);
            this.txtMail.TabIndex = 13;
            // 
            // lblNgoaiNgu
            // 
            this.lblNgoaiNgu.AutoSize = true;
            this.lblNgoaiNgu.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNgoaiNgu.Location = new System.Drawing.Point(50, 188);
            this.lblNgoaiNgu.Name = "lblNgoaiNgu";
            this.lblNgoaiNgu.Size = new System.Drawing.Size(86, 19);
            this.lblNgoaiNgu.TabIndex = 14;
            this.lblNgoaiNgu.Text = "Ngoại Ngữ";
            // 
            // chklbNgoaiNgu
            // 
            this.chklbNgoaiNgu.CheckOnClick = true;
            this.chklbNgoaiNgu.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chklbNgoaiNgu.FormattingEnabled = true;
            this.chklbNgoaiNgu.Items.AddRange(new object[] {
            "Tiếng Anh",
            "Tiếng Pháp",
            "Tiếng Nhật",
            "Tiếng Nga"});
            this.chklbNgoaiNgu.Location = new System.Drawing.Point(145, 185);
            this.chklbNgoaiNgu.Name = "chklbNgoaiNgu";
            this.chklbNgoaiNgu.Size = new System.Drawing.Size(140, 92);
            this.chklbNgoaiNgu.TabIndex = 15;
            // 
            // lblDSMH
            // 
            this.lblDSMH.AutoSize = true;
            this.lblDSMH.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDSMH.Location = new System.Drawing.Point(141, 285);
            this.lblDSMH.Name = "lblDSMH";
            this.lblDSMH.Size = new System.Drawing.Size(149, 19);
            this.lblDSMH.TabIndex = 16;
            this.lblDSMH.Text = "Danh sách môn học";
            // 
            // lblMHGVD
            // 
            this.lblMHGVD.AutoSize = true;
            this.lblMHGVD.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMHGVD.Location = new System.Drawing.Point(416, 285);
            this.lblMHGVD.Name = "lblMHGVD";
            this.lblMHGVD.Size = new System.Drawing.Size(167, 19);
            this.lblMHGVD.TabIndex = 17;
            this.lblMHGVD.Text = "Môn học giáo viên dạy";
            // 
            // lbDanhSachHP
            // 
            this.lbDanhSachHP.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDanhSachHP.FormattingEnabled = true;
            this.lbDanhSachHP.ItemHeight = 19;
            this.lbDanhSachHP.Items.AddRange(new object[] {
            "Tin học cơ sở",
            "Lập trình cấu trúc C/C++",
            "Cơ sở dữ liệu",
            "Tiếng Anh B1",
            "Tiếng Anh B2",
            "Lập trình hướng đối tượng",
            "Mạng máy tính",
            "Công nghệ phần mềm",
            "Phân tích TKHDT"});
            this.lbDanhSachHP.Location = new System.Drawing.Point(145, 308);
            this.lbDanhSachHP.Name = "lbDanhSachHP";
            this.lbDanhSachHP.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbDanhSachHP.Size = new System.Drawing.Size(195, 175);
            this.lbDanhSachHP.TabIndex = 18;
            // 
            // lbHocPhanDay
            // 
            this.lbHocPhanDay.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHocPhanDay.FormattingEnabled = true;
            this.lbHocPhanDay.ItemHeight = 19;
            this.lbHocPhanDay.Location = new System.Drawing.Point(420, 308);
            this.lbHocPhanDay.Name = "lbHocPhanDay";
            this.lbHocPhanDay.Size = new System.Drawing.Size(195, 175);
            this.lbHocPhanDay.TabIndex = 19;
            // 
            // btnChon
            // 
            this.btnChon.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChon.Location = new System.Drawing.Point(355, 335);
            this.btnChon.Name = "btnChon";
            this.btnChon.Size = new System.Drawing.Size(46, 35);
            this.btnChon.TabIndex = 20;
            this.btnChon.Text = ">>";
            this.btnChon.UseVisualStyleBackColor = true;
            this.btnChon.Click += new System.EventHandler(this.btnChon_Click);
            // 
            // btnXoa
            // 
            this.btnXoa.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXoa.Location = new System.Drawing.Point(355, 380);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(46, 35);
            this.btnXoa.TabIndex = 21;
            this.btnXoa.Text = "<<";
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // btnThongBao
            // 
            this.btnThongBao.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThongBao.ForeColor = System.Drawing.Color.Blue;
            this.btnThongBao.Location = new System.Drawing.Point(115, 495);
            this.btnThongBao.Name = "btnThongBao";
            this.btnThongBao.Size = new System.Drawing.Size(125, 36);
            this.btnThongBao.TabIndex = 22;
            this.btnThongBao.Text = "Thông báo";
            this.btnThongBao.UseVisualStyleBackColor = true;
            this.btnThongBao.Click += new System.EventHandler(this.btnThongBao_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(265, 495);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 36);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.Blue;
            this.btnExit.Location = new System.Drawing.Point(405, 495);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 36);
            this.btnExit.TabIndex = 24;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblWebsite
            // 
            this.lblWebsite.AutoSize = true;
            this.lblWebsite.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWebsite.Location = new System.Drawing.Point(500, 504);
            this.lblWebsite.Name = "lblWebsite";
            this.lblWebsite.Size = new System.Drawing.Size(65, 19);
            this.lblWebsite.TabIndex = 25;
            this.lblWebsite.Text = "Website";
            // 
            // linklbLienHe
            // 
            this.linklbLienHe.AutoSize = true;
            this.linklbLienHe.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linklbLienHe.Location = new System.Drawing.Point(570, 504);
            this.linklbLienHe.Name = "linklbLienHe";
            this.linklbLienHe.Size = new System.Drawing.Size(60, 19);
            this.linklbLienHe.TabIndex = 26;
            this.linklbLienHe.TabStop = true;
            this.linklbLienHe.Text = "Liên hệ";
            this.linklbLienHe.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linklbLienHe_LinkClicked);
            // 
            // frmGiangVien
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(685, 560);
            this.Controls.Add(this.linklbLienHe);
            this.Controls.Add(this.lblWebsite);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnThongBao);
            this.Controls.Add(this.btnXoa);
            this.Controls.Add(this.btnChon);
            this.Controls.Add(this.lbHocPhanDay);
            this.Controls.Add(this.lbDanhSachHP);
            this.Controls.Add(this.lblMHGVD);
            this.Controls.Add(this.lblDSMH);
            this.Controls.Add(this.chklbNgoaiNgu);
            this.Controls.Add(this.lblNgoaiNgu);
            this.Controls.Add(this.txtMail);
            this.Controls.Add(this.lblMail);
            this.Controls.Add(this.dtpNgaySinh);
            this.Controls.Add(this.lblNgaySinh);
            this.Controls.Add(this.mtxtSoDT);
            this.Controls.Add(this.lblSoDT);
            this.Controls.Add(this.txtHoTen);
            this.Controls.Add(this.lblHoTen);
            this.Controls.Add(this.rdNu);
            this.Controls.Add(this.rdNam);
            this.Controls.Add(this.lblGioiTinh);
            this.Controls.Add(this.cboMaSo);
            this.Controls.Add(this.lblMaSo);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmGiangVien";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Giảng viên";
            this.Load += new System.EventHandler(this.frmGiangVien_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMaSo;
        private System.Windows.Forms.ComboBox cboMaSo;
        private System.Windows.Forms.Label lblGioiTinh;
        private System.Windows.Forms.RadioButton rdNam;
        private System.Windows.Forms.RadioButton rdNu;
        private System.Windows.Forms.Label lblHoTen;
        private System.Windows.Forms.TextBox txtHoTen;
        private System.Windows.Forms.Label lblSoDT;
        private System.Windows.Forms.MaskedTextBox mtxtSoDT;
        private System.Windows.Forms.Label lblNgaySinh;
        private System.Windows.Forms.DateTimePicker dtpNgaySinh;
        private System.Windows.Forms.Label lblMail;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.Label lblNgoaiNgu;
        private System.Windows.Forms.CheckedListBox chklbNgoaiNgu;
        private System.Windows.Forms.Label lblDSMH;
        private System.Windows.Forms.Label lblMHGVD;
        private System.Windows.Forms.ListBox lbDanhSachHP;
        private System.Windows.Forms.ListBox lbHocPhanDay;
        private System.Windows.Forms.Button btnChon;
        private System.Windows.Forms.Button btnXoa;
        private System.Windows.Forms.Button btnThongBao;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblWebsite;
        private System.Windows.Forms.LinkLabel linklbLienHe;
    }
}
