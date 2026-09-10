namespace QuanLySinhVien
{
    partial class frmSinhVien
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmMoFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmThoat = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmThem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmXoa = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSua = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmFont = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmMauChu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSapXep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTimKiem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupboxTTSV = new System.Windows.Forms.GroupBox();
            this.btnThoat = new System.Windows.Forms.Button();
            this.btnMacDinh = new System.Windows.Forms.Button();
            this.btnSua = new System.Windows.Forms.Button();
            this.btnXoa = new System.Windows.Forms.Button();
            this.btnThem = new System.Windows.Forms.Button();
            this.clbChuyenNganh = new System.Windows.Forms.CheckedListBox();
            this.lblChuyenNganh = new System.Windows.Forms.Label();
            this.rdNu = new System.Windows.Forms.RadioButton();
            this.rdNam = new System.Windows.Forms.RadioButton();
            this.lblGioiTinh = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtHinh = new System.Windows.Forms.TextBox();
            this.lblHinh = new System.Windows.Forms.Label();
            this.pbHinh = new System.Windows.Forms.PictureBox();
            this.cboLop = new System.Windows.Forms.ComboBox();
            this.lblLop = new System.Windows.Forms.Label();
            this.txtDiaChi = new System.Windows.Forms.TextBox();
            this.lblDiaChi = new System.Windows.Forms.Label();
            this.dtpNgaySinh = new System.Windows.Forms.DateTimePicker();
            this.lblNgaySinh = new System.Windows.Forms.Label();
            this.txtHoTen = new System.Windows.Forms.TextBox();
            this.lblHoTen = new System.Windows.Forms.Label();
            this.mtxtMaSo = new System.Windows.Forms.MaskedTextBox();
            this.lblMaSo = new System.Windows.Forms.Label();
            this.groupboxDSSV = new System.Windows.Forms.GroupBox();
            this.lvSinhVien = new System.Windows.Forms.ListView();
            this.chMaSo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chHoTen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chNgaySinh = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDiaChi = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLop = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPhai = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chChuyenNganh = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chHinh = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmContextFont = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmContextMauChu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmContextSapXep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmContextTimKiem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmContextXoa = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssTongSV = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialogHinh = new System.Windows.Forms.OpenFileDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupboxTTSV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHinh)).BeginInit();
            this.groupboxDSSV.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1294, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmMoFile,
            this.tsmThoat});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // tsmMoFile
            // 
            this.tsmMoFile.Name = "tsmMoFile";
            this.tsmMoFile.Size = new System.Drawing.Size(141, 26);
            this.tsmMoFile.Text = "Mở File";
            this.tsmMoFile.Click += new System.EventHandler(this.tsmMoFile_Click);
            // 
            // tsmThoat
            // 
            this.tsmThoat.Name = "tsmThoat";
            this.tsmThoat.Size = new System.Drawing.Size(141, 26);
            this.tsmThoat.Text = "Thoát";
            this.tsmThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmThem,
            this.tsmXoa,
            this.tsmSua,
            this.listViewToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // tsmThem
            // 
            this.tsmThem.Name = "tsmThem";
            this.tsmThem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.T)));
            this.tsmThem.Size = new System.Drawing.Size(208, 26);
            this.tsmThem.Text = "Thêm";
            this.tsmThem.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // tsmXoa
            // 
            this.tsmXoa.Name = "tsmXoa";
            this.tsmXoa.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Y)));
            this.tsmXoa.Size = new System.Drawing.Size(208, 26);
            this.tsmXoa.Text = "Xóa";
            this.tsmXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // tsmSua
            // 
            this.tsmSua.Name = "tsmSua";
            this.tsmSua.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
            this.tsmSua.Size = new System.Drawing.Size(208, 26);
            this.tsmSua.Text = "Sửa";
            this.tsmSua.Click += new System.EventHandler(this.btnSua_Click);
            // 
            // listViewToolStripMenuItem
            // 
            this.listViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmFont,
            this.tsmMauChu,
            this.tsmSapXep,
            this.tsmTimKiem});
            this.listViewToolStripMenuItem.Name = "listViewToolStripMenuItem";
            this.listViewToolStripMenuItem.Size = new System.Drawing.Size(208, 26);
            this.listViewToolStripMenuItem.Text = "ListView";
            // 
            // tsmFont
            // 
            this.tsmFont.Name = "tsmFont";
            this.tsmFont.Size = new System.Drawing.Size(153, 26);
            this.tsmFont.Text = "Font";
            this.tsmFont.Click += new System.EventHandler(this.tsmFont_Click);
            // 
            // tsmMauChu
            // 
            this.tsmMauChu.Name = "tsmMauChu";
            this.tsmMauChu.Size = new System.Drawing.Size(153, 26);
            this.tsmMauChu.Text = "Màu chữ";
            this.tsmMauChu.Click += new System.EventHandler(this.tsmMauChu_Click);
            // 
            // tsmSapXep
            // 
            this.tsmSapXep.Name = "tsmSapXep";
            this.tsmSapXep.Size = new System.Drawing.Size(153, 26);
            this.tsmSapXep.Text = "Sắp xếp";
            this.tsmSapXep.Click += new System.EventHandler(this.tsmSapXep_Click);
            // 
            // tsmTimKiem
            // 
            this.tsmTimKiem.Name = "tsmTimKiem";
            this.tsmTimKiem.Size = new System.Drawing.Size(153, 26);
            this.tsmTimKiem.Text = "Tìm kiếm";
            this.tsmTimKiem.Click += new System.EventHandler(this.tsmTimKiem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupboxTTSV);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupboxDSSV);
            this.splitContainer1.Size = new System.Drawing.Size(1294, 660);
            this.splitContainer1.SplitterDistance = 593;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupboxTTSV
            // 
            this.groupboxTTSV.Controls.Add(this.btnThoat);
            this.groupboxTTSV.Controls.Add(this.btnMacDinh);
            this.groupboxTTSV.Controls.Add(this.btnSua);
            this.groupboxTTSV.Controls.Add(this.btnXoa);
            this.groupboxTTSV.Controls.Add(this.btnThem);
            this.groupboxTTSV.Controls.Add(this.clbChuyenNganh);
            this.groupboxTTSV.Controls.Add(this.lblChuyenNganh);
            this.groupboxTTSV.Controls.Add(this.rdNu);
            this.groupboxTTSV.Controls.Add(this.rdNam);
            this.groupboxTTSV.Controls.Add(this.lblGioiTinh);
            this.groupboxTTSV.Controls.Add(this.btnBrowse);
            this.groupboxTTSV.Controls.Add(this.txtHinh);
            this.groupboxTTSV.Controls.Add(this.lblHinh);
            this.groupboxTTSV.Controls.Add(this.pbHinh);
            this.groupboxTTSV.Controls.Add(this.cboLop);
            this.groupboxTTSV.Controls.Add(this.lblLop);
            this.groupboxTTSV.Controls.Add(this.txtDiaChi);
            this.groupboxTTSV.Controls.Add(this.lblDiaChi);
            this.groupboxTTSV.Controls.Add(this.dtpNgaySinh);
            this.groupboxTTSV.Controls.Add(this.lblNgaySinh);
            this.groupboxTTSV.Controls.Add(this.txtHoTen);
            this.groupboxTTSV.Controls.Add(this.lblHoTen);
            this.groupboxTTSV.Controls.Add(this.mtxtMaSo);
            this.groupboxTTSV.Controls.Add(this.lblMaSo);
            this.groupboxTTSV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupboxTTSV.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupboxTTSV.Location = new System.Drawing.Point(0, 0);
            this.groupboxTTSV.Name = "groupboxTTSV";
            this.groupboxTTSV.Size = new System.Drawing.Size(593, 660);
            this.groupboxTTSV.TabIndex = 0;
            this.groupboxTTSV.TabStop = false;
            this.groupboxTTSV.Text = "Thông tin sinh viên";
            // 
            // btnThoat
            // 
            this.btnThoat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.ForeColor = System.Drawing.Color.MediumBlue;
            this.btnThoat.Location = new System.Drawing.Point(440, 570);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(95, 35);
            this.btnThoat.TabIndex = 23;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnMacDinh
            // 
            this.btnMacDinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMacDinh.ForeColor = System.Drawing.Color.MediumBlue;
            this.btnMacDinh.Location = new System.Drawing.Point(325, 570);
            this.btnMacDinh.Name = "btnMacDinh";
            this.btnMacDinh.Size = new System.Drawing.Size(105, 35);
            this.btnMacDinh.TabIndex = 22;
            this.btnMacDinh.Text = "Mặc Định";
            this.btnMacDinh.UseVisualStyleBackColor = true;
            this.btnMacDinh.Click += new System.EventHandler(this.btnMacDinh_Click);
            // 
            // btnSua
            // 
            this.btnSua.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSua.ForeColor = System.Drawing.Color.MediumBlue;
            this.btnSua.Location = new System.Drawing.Point(220, 570);
            this.btnSua.Name = "btnSua";
            this.btnSua.Size = new System.Drawing.Size(95, 35);
            this.btnSua.TabIndex = 21;
            this.btnSua.Text = "Sửa";
            this.btnSua.UseVisualStyleBackColor = true;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            // 
            // btnXoa
            // 
            this.btnXoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXoa.ForeColor = System.Drawing.Color.MediumBlue;
            this.btnXoa.Location = new System.Drawing.Point(115, 570);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(95, 35);
            this.btnXoa.TabIndex = 20;
            this.btnXoa.Text = "Xóa";
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // btnThem
            // 
            this.btnThem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThem.ForeColor = System.Drawing.Color.MediumBlue;
            this.btnThem.Location = new System.Drawing.Point(10, 570);
            this.btnThem.Name = "btnThem";
            this.btnThem.Size = new System.Drawing.Size(95, 35);
            this.btnThem.TabIndex = 19;
            this.btnThem.Text = "Thêm";
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // clbChuyenNganh
            // 
            this.clbChuyenNganh.CheckOnClick = true;
            this.clbChuyenNganh.FormattingEnabled = true;
            this.clbChuyenNganh.Items.AddRange(new object[] {
            "Mạng truyền thông",
            "Công nghệ phần mềm",
            "Khoa học máy tính",
            "Trí tuệ nhân tạo",
            "Hệ thống thông tin",
            "Tin học ứng dụng"});
            this.clbChuyenNganh.Location = new System.Drawing.Point(150, 395);
            this.clbChuyenNganh.Name = "clbChuyenNganh";
            this.clbChuyenNganh.Size = new System.Drawing.Size(370, 137);
            this.clbChuyenNganh.TabIndex = 18;
            // 
            // lblChuyenNganh
            // 
            this.lblChuyenNganh.AutoSize = true;
            this.lblChuyenNganh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChuyenNganh.Location = new System.Drawing.Point(20, 395);
            this.lblChuyenNganh.Name = "lblChuyenNganh";
            this.lblChuyenNganh.Size = new System.Drawing.Size(117, 18);
            this.lblChuyenNganh.TabIndex = 17;
            this.lblChuyenNganh.Text = "Chuyên Ngành";
            // 
            // rdNu
            // 
            this.rdNu.AutoSize = true;
            this.rdNu.Location = new System.Drawing.Point(240, 350);
            this.rdNu.Name = "rdNu";
            this.rdNu.Size = new System.Drawing.Size(48, 22);
            this.rdNu.TabIndex = 16;
            this.rdNu.Text = "Nữ";
            this.rdNu.UseVisualStyleBackColor = true;
            // 
            // rdNam
            // 
            this.rdNam.AutoSize = true;
            this.rdNam.Checked = true;
            this.rdNam.Location = new System.Drawing.Point(150, 350);
            this.rdNam.Name = "rdNam";
            this.rdNam.Size = new System.Drawing.Size(61, 22);
            this.rdNam.TabIndex = 15;
            this.rdNam.TabStop = true;
            this.rdNam.Text = "Nam";
            this.rdNam.UseVisualStyleBackColor = true;
            // 
            // lblGioiTinh
            // 
            this.lblGioiTinh.AutoSize = true;
            this.lblGioiTinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGioiTinh.Location = new System.Drawing.Point(20, 352);
            this.lblGioiTinh.Name = "lblGioiTinh";
            this.lblGioiTinh.Size = new System.Drawing.Size(76, 18);
            this.lblGioiTinh.TabIndex = 14;
            this.lblGioiTinh.Text = "Giới Tính";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(480, 298);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(40, 28);
            this.btnBrowse.TabIndex = 13;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtHinh
            // 
            this.txtHinh.Location = new System.Drawing.Point(150, 300);
            this.txtHinh.Name = "txtHinh";
            this.txtHinh.ReadOnly = true;
            this.txtHinh.Size = new System.Drawing.Size(320, 24);
            this.txtHinh.TabIndex = 12;
            // 
            // lblHinh
            // 
            this.lblHinh.AutoSize = true;
            this.lblHinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHinh.Location = new System.Drawing.Point(20, 303);
            this.lblHinh.Name = "lblHinh";
            this.lblHinh.Size = new System.Drawing.Size(42, 18);
            this.lblHinh.TabIndex = 11;
            this.lblHinh.Text = "Hình";
            // 
            // pbHinh
            // 
            this.pbHinh.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbHinh.Location = new System.Drawing.Point(20, 30);
            this.pbHinh.Name = "pbHinh";
            this.pbHinh.Size = new System.Drawing.Size(170, 230);
            this.pbHinh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbHinh.TabIndex = 10;
            this.pbHinh.TabStop = false;
            // 
            // cboLop
            // 
            this.cboLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLop.FormattingEnabled = true;
            this.cboLop.Items.AddRange(new object[] {
            "CTK31",
            "CTK32",
            "CTK33",
            "CTK34",
            "CTK32CD",
            "CTK33CD",
            "CTK34CD"});
            this.cboLop.Location = new System.Drawing.Point(295, 215);
            this.cboLop.Name = "cboLop";
            this.cboLop.Size = new System.Drawing.Size(225, 26);
            this.cboLop.TabIndex = 9;
            // 
            // lblLop
            // 
            this.lblLop.AutoSize = true;
            this.lblLop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLop.Location = new System.Drawing.Point(205, 218);
            this.lblLop.Name = "lblLop";
            this.lblLop.Size = new System.Drawing.Size(36, 18);
            this.lblLop.TabIndex = 8;
            this.lblLop.Text = "Lớp";
            // 
            // txtDiaChi
            // 
            this.txtDiaChi.Location = new System.Drawing.Point(295, 170);
            this.txtDiaChi.Name = "txtDiaChi";
            this.txtDiaChi.Size = new System.Drawing.Size(225, 24);
            this.txtDiaChi.TabIndex = 7;
            // 
            // lblDiaChi
            // 
            this.lblDiaChi.AutoSize = true;
            this.lblDiaChi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiaChi.Location = new System.Drawing.Point(205, 173);
            this.lblDiaChi.Name = "lblDiaChi";
            this.lblDiaChi.Size = new System.Drawing.Size(63, 18);
            this.lblDiaChi.TabIndex = 6;
            this.lblDiaChi.Text = "Địa Chỉ";
            // 
            // dtpNgaySinh
            // 
            this.dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
            this.dtpNgaySinh.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgaySinh.Location = new System.Drawing.Point(295, 125);
            this.dtpNgaySinh.Name = "dtpNgaySinh";
            this.dtpNgaySinh.Size = new System.Drawing.Size(225, 24);
            this.dtpNgaySinh.TabIndex = 5;
            // 
            // lblNgaySinh
            // 
            this.lblNgaySinh.AutoSize = true;
            this.lblNgaySinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNgaySinh.Location = new System.Drawing.Point(205, 128);
            this.lblNgaySinh.Name = "lblNgaySinh";
            this.lblNgaySinh.Size = new System.Drawing.Size(84, 18);
            this.lblNgaySinh.TabIndex = 4;
            this.lblNgaySinh.Text = "Ngày Sinh";
            // 
            // txtHoTen
            // 
            this.txtHoTen.Location = new System.Drawing.Point(295, 78);
            this.txtHoTen.Name = "txtHoTen";
            this.txtHoTen.Size = new System.Drawing.Size(225, 24);
            this.txtHoTen.TabIndex = 3;
            // 
            // lblHoTen
            // 
            this.lblHoTen.AutoSize = true;
            this.lblHoTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoTen.Location = new System.Drawing.Point(205, 81);
            this.lblHoTen.Name = "lblHoTen";
            this.lblHoTen.Size = new System.Drawing.Size(63, 18);
            this.lblHoTen.TabIndex = 2;
            this.lblHoTen.Text = "Họ Tên";
            // 
            // mtxtMaSo
            // 
            this.mtxtMaSo.Location = new System.Drawing.Point(295, 33);
            this.mtxtMaSo.Mask = "SV.00000";
            this.mtxtMaSo.Name = "mtxtMaSo";
            this.mtxtMaSo.Size = new System.Drawing.Size(225, 24);
            this.mtxtMaSo.TabIndex = 1;
            // 
            // lblMaSo
            // 
            this.lblMaSo.AutoSize = true;
            this.lblMaSo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaSo.Location = new System.Drawing.Point(205, 36);
            this.lblMaSo.Name = "lblMaSo";
            this.lblMaSo.Size = new System.Drawing.Size(55, 18);
            this.lblMaSo.TabIndex = 0;
            this.lblMaSo.Text = "Mã số";
            // 
            // groupboxDSSV
            // 
            this.groupboxDSSV.Controls.Add(this.lvSinhVien);
            this.groupboxDSSV.Controls.Add(this.statusStrip1);
            this.groupboxDSSV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupboxDSSV.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupboxDSSV.Location = new System.Drawing.Point(0, 0);
            this.groupboxDSSV.Name = "groupboxDSSV";
            this.groupboxDSSV.Size = new System.Drawing.Size(697, 660);
            this.groupboxDSSV.TabIndex = 0;
            this.groupboxDSSV.TabStop = false;
            this.groupboxDSSV.Text = "Danh sách sinh viên";
            // 
            // lvSinhVien
            // 
            this.lvSinhVien.CheckBoxes = true;
            this.lvSinhVien.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chMaSo,
            this.chHoTen,
            this.chNgaySinh,
            this.chDiaChi,
            this.chLop,
            this.chPhai,
            this.chChuyenNganh,
            this.chHinh});
            this.lvSinhVien.ContextMenuStrip = this.contextMenuStrip1;
            this.lvSinhVien.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSinhVien.FullRowSelect = true;
            this.lvSinhVien.GridLines = true;
            this.lvSinhVien.HideSelection = false;
            this.lvSinhVien.Location = new System.Drawing.Point(3, 20);
            this.lvSinhVien.Name = "lvSinhVien";
            this.lvSinhVien.Size = new System.Drawing.Size(691, 611);
            this.lvSinhVien.TabIndex = 0;
            this.lvSinhVien.UseCompatibleStateImageBehavior = false;
            this.lvSinhVien.View = System.Windows.Forms.View.Details;
            this.lvSinhVien.SelectedIndexChanged += new System.EventHandler(this.lvSinhVien_SelectedIndexChanged);
            // 
            // chMaSo
            // 
            this.chMaSo.Text = "Mã số";
            this.chMaSo.Width = 90;
            // 
            // chHoTen
            // 
            this.chHoTen.Text = "Họ tên";
            this.chHoTen.Width = 140;
            // 
            // chNgaySinh
            // 
            this.chNgaySinh.Text = "Ngày Sinh";
            this.chNgaySinh.Width = 95;
            // 
            // chDiaChi
            // 
            this.chDiaChi.Text = "Địa Chỉ";
            this.chDiaChi.Width = 90;
            // 
            // chLop
            // 
            this.chLop.Text = "Lớp";
            this.chLop.Width = 70;
            // 
            // chPhai
            // 
            this.chPhai.Text = "Phái";
            // 
            // chChuyenNganh
            // 
            this.chChuyenNganh.Text = "Chuyên ngành";
            this.chChuyenNganh.Width = 180;
            // 
            // chHinh
            // 
            this.chHinh.Text = "Hình";
            this.chHinh.Width = 100;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmContextFont,
            this.tsmContextMauChu,
            this.tsmContextSapXep,
            this.tsmContextTimKiem,
            this.tsmContextXoa});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(140, 124);
            // 
            // tsmContextFont
            // 
            this.tsmContextFont.Name = "tsmContextFont";
            this.tsmContextFont.Size = new System.Drawing.Size(139, 24);
            this.tsmContextFont.Text = "Font";
            this.tsmContextFont.Click += new System.EventHandler(this.tsmFont_Click);
            // 
            // tsmContextMauChu
            // 
            this.tsmContextMauChu.Name = "tsmContextMauChu";
            this.tsmContextMauChu.Size = new System.Drawing.Size(139, 24);
            this.tsmContextMauChu.Text = "Màu chữ";
            this.tsmContextMauChu.Click += new System.EventHandler(this.tsmMauChu_Click);
            // 
            // tsmContextSapXep
            // 
            this.tsmContextSapXep.Name = "tsmContextSapXep";
            this.tsmContextSapXep.Size = new System.Drawing.Size(139, 24);
            this.tsmContextSapXep.Text = "Sắp xếp";
            this.tsmContextSapXep.Click += new System.EventHandler(this.tsmSapXep_Click);
            // 
            // tsmContextTimKiem
            // 
            this.tsmContextTimKiem.Name = "tsmContextTimKiem";
            this.tsmContextTimKiem.Size = new System.Drawing.Size(139, 24);
            this.tsmContextTimKiem.Text = "Tìm kiếm";
            this.tsmContextTimKiem.Click += new System.EventHandler(this.tsmTimKiem_Click);
            // 
            // tsmContextXoa
            // 
            this.tsmContextXoa.Name = "tsmContextXoa";
            this.tsmContextXoa.Size = new System.Drawing.Size(139, 24);
            this.tsmContextXoa.Text = "Xóa";
            this.tsmContextXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssTongSV});
            this.statusStrip1.Location = new System.Drawing.Point(3, 631);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(691, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssTongSV
            // 
            this.tssTongSV.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssTongSV.Name = "tssTongSV";
            this.tssTongSV.Size = new System.Drawing.Size(131, 20);
            this.tssTongSV.Text = "Tổng Sinh Viên: 0";
            // 
            // openFileDialogHinh
            // 
            this.openFileDialogHinh.FileName = "Hãy Chọn File";
            this.openFileDialogHinh.Filter = "Image File(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All File(*.*)|*.*";
            // 
            // frmSinhVien
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1294, 688);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmSinhVien";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo Sinh viên";
            this.Load += new System.EventHandler(this.frmSinhVien_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupboxTTSV.ResumeLayout(false);
            this.groupboxTTSV.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHinh)).EndInit();
            this.groupboxDSSV.ResumeLayout(false);
            this.groupboxDSSV.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmMoFile;
        private System.Windows.Forms.ToolStripMenuItem tsmThoat;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmThem;
        private System.Windows.Forms.ToolStripMenuItem tsmXoa;
        private System.Windows.Forms.ToolStripMenuItem tsmSua;
        private System.Windows.Forms.ToolStripMenuItem listViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmFont;
        private System.Windows.Forms.ToolStripMenuItem tsmMauChu;
        private System.Windows.Forms.ToolStripMenuItem tsmSapXep;
        private System.Windows.Forms.ToolStripMenuItem tsmTimKiem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupboxTTSV;
        private System.Windows.Forms.GroupBox groupboxDSSV;
        private System.Windows.Forms.MaskedTextBox mtxtMaSo;
        private System.Windows.Forms.Label lblMaSo;
        private System.Windows.Forms.TextBox txtHoTen;
        private System.Windows.Forms.Label lblHoTen;
        private System.Windows.Forms.DateTimePicker dtpNgaySinh;
        private System.Windows.Forms.Label lblNgaySinh;
        private System.Windows.Forms.TextBox txtDiaChi;
        private System.Windows.Forms.Label lblDiaChi;
        private System.Windows.Forms.ComboBox cboLop;
        private System.Windows.Forms.Label lblLop;
        private System.Windows.Forms.PictureBox pbHinh;
        private System.Windows.Forms.TextBox txtHinh;
        private System.Windows.Forms.Label lblHinh;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.RadioButton rdNu;
        private System.Windows.Forms.RadioButton rdNam;
        private System.Windows.Forms.Label lblGioiTinh;
        private System.Windows.Forms.CheckedListBox clbChuyenNganh;
        private System.Windows.Forms.Label lblChuyenNganh;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Button btnMacDinh;
        private System.Windows.Forms.Button btnSua;
        private System.Windows.Forms.Button btnXoa;
        private System.Windows.Forms.Button btnThem;
        private System.Windows.Forms.ListView lvSinhVien;
        private System.Windows.Forms.ColumnHeader chMaSo;
        private System.Windows.Forms.ColumnHeader chHoTen;
        private System.Windows.Forms.ColumnHeader chNgaySinh;
        private System.Windows.Forms.ColumnHeader chDiaChi;
        private System.Windows.Forms.ColumnHeader chLop;
        private System.Windows.Forms.ColumnHeader chPhai;
        private System.Windows.Forms.ColumnHeader chChuyenNganh;
        private System.Windows.Forms.ColumnHeader chHinh;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssTongSV;
        private System.Windows.Forms.OpenFileDialog openFileDialogHinh;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmContextFont;
        private System.Windows.Forms.ToolStripMenuItem tsmContextMauChu;
        private System.Windows.Forms.ToolStripMenuItem tsmContextSapXep;
        private System.Windows.Forms.ToolStripMenuItem tsmContextTimKiem;
        private System.Windows.Forms.ToolStripMenuItem tsmContextXoa;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}
