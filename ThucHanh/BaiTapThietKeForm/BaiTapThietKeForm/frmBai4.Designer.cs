namespace BaiTapThietKeForm
{
    partial class frmBai4
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
            this.lbxDanhSach = new System.Windows.Forms.ListBox();
            this.lblNhap = new System.Windows.Forms.Label();
            this.lblKetQua = new System.Windows.Forms.Label();
            this.nudNhap = new System.Windows.Forms.NumericUpDown();
            this.btnTimSo = new System.Windows.Forms.Button();
            this.txtKetQua = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudNhap)).BeginInit();
            this.SuspendLayout();
            // 
            // lbxDanhSach
            // 
            this.lbxDanhSach.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbxDanhSach.FormattingEnabled = true;
            this.lbxDanhSach.ItemHeight = 20;
            this.lbxDanhSach.Location = new System.Drawing.Point(34, 50);
            this.lbxDanhSach.Name = "lbxDanhSach";
            this.lbxDanhSach.Size = new System.Drawing.Size(333, 364);
            this.lbxDanhSach.TabIndex = 0;
            // 
            // lblNhap
            // 
            this.lblNhap.AutoSize = true;
            this.lblNhap.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNhap.Location = new System.Drawing.Point(449, 105);
            this.lblNhap.Name = "lblNhap";
            this.lblNhap.Size = new System.Drawing.Size(123, 20);
            this.lblNhap.TabIndex = 1;
            this.lblNhap.Text = "Nhập số cần tìm";
            // 
            // lblKetQua
            // 
            this.lblKetQua.AutoSize = true;
            this.lblKetQua.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKetQua.Location = new System.Drawing.Point(474, 324);
            this.lblKetQua.Name = "lblKetQua";
            this.lblKetQua.Size = new System.Drawing.Size(64, 20);
            this.lblKetQua.TabIndex = 1;
            this.lblKetQua.Text = "Kết quả";
            // 
            // nudNhap
            // 
            this.nudNhap.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudNhap.Location = new System.Drawing.Point(596, 99);
            this.nudNhap.Name = "nudNhap";
            this.nudNhap.Size = new System.Drawing.Size(120, 26);
            this.nudNhap.TabIndex = 2;
            // 
            // btnTimSo
            // 
            this.btnTimSo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTimSo.Location = new System.Drawing.Point(533, 157);
            this.btnTimSo.Name = "btnTimSo";
            this.btnTimSo.Size = new System.Drawing.Size(106, 28);
            this.btnTimSo.TabIndex = 3;
            this.btnTimSo.Text = "Tìm số";
            this.btnTimSo.UseVisualStyleBackColor = true;
            this.btnTimSo.Click += new System.EventHandler(this.btnTimSo_Click);
            // 
            // txtKetQua
            // 
            this.txtKetQua.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKetQua.Location = new System.Drawing.Point(568, 321);
            this.txtKetQua.Name = "txtKetQua";
            this.txtKetQua.Size = new System.Drawing.Size(164, 26);
            this.txtKetQua.TabIndex = 4;
            // 
            // frmBai4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtKetQua);
            this.Controls.Add(this.btnTimSo);
            this.Controls.Add(this.nudNhap);
            this.Controls.Add(this.lblKetQua);
            this.Controls.Add(this.lblNhap);
            this.Controls.Add(this.lbxDanhSach);
            this.Name = "frmBai4";
            this.Text = "frmBai4";
            this.Load += new System.EventHandler(this.frmBai4_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudNhap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxDanhSach;
        private System.Windows.Forms.Label lblNhap;
        private System.Windows.Forms.Label lblKetQua;
        private System.Windows.Forms.NumericUpDown nudNhap;
        private System.Windows.Forms.Button btnTimSo;
        private System.Windows.Forms.TextBox txtKetQua;
    }
}