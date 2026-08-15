namespace BaiTapThietKeForm
{
    partial class frmBai3
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
            this.lbTuMoi = new System.Windows.Forms.Label();
            this.lbNghiaTu = new System.Windows.Forms.Label();
            this.tbTuMoi = new System.Windows.Forms.TextBox();
            this.tbNghiaTu = new System.Windows.Forms.TextBox();
            this.btnThemTu = new System.Windows.Forms.Button();
            this.lbDanhSach = new System.Windows.Forms.Label();
            this.lbxDSTu = new System.Windows.Forms.ListBox();
            this.lbNghiaCuaTu = new System.Windows.Forms.Label();
            this.tbNghiaCuaTu = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbTuMoi
            // 
            this.lbTuMoi.AutoSize = true;
            this.lbTuMoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTuMoi.Location = new System.Drawing.Point(42, 53);
            this.lbTuMoi.Name = "lbTuMoi";
            this.lbTuMoi.Size = new System.Drawing.Size(56, 20);
            this.lbTuMoi.TabIndex = 0;
            this.lbTuMoi.Text = "Từ mới";
            // 
            // lbNghiaTu
            // 
            this.lbNghiaTu.AutoSize = true;
            this.lbNghiaTu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNghiaTu.Location = new System.Drawing.Point(42, 88);
            this.lbNghiaTu.Name = "lbNghiaTu";
            this.lbNghiaTu.Size = new System.Drawing.Size(98, 20);
            this.lbNghiaTu.TabIndex = 0;
            this.lbNghiaTu.Text = "Nghĩa của từ";
            // 
            // tbTuMoi
            // 
            this.tbTuMoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTuMoi.Location = new System.Drawing.Point(169, 47);
            this.tbTuMoi.Name = "tbTuMoi";
            this.tbTuMoi.Size = new System.Drawing.Size(161, 26);
            this.tbTuMoi.TabIndex = 0;
            // 
            // tbNghiaTu
            // 
            this.tbNghiaTu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNghiaTu.Location = new System.Drawing.Point(169, 82);
            this.tbNghiaTu.Name = "tbNghiaTu";
            this.tbNghiaTu.Size = new System.Drawing.Size(161, 26);
            this.tbNghiaTu.TabIndex = 1;
            // 
            // btnThemTu
            // 
            this.btnThemTu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnThemTu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThemTu.Location = new System.Drawing.Point(169, 129);
            this.btnThemTu.Name = "btnThemTu";
            this.btnThemTu.Size = new System.Drawing.Size(110, 27);
            this.btnThemTu.TabIndex = 2;
            this.btnThemTu.Text = "Thêm từ mới";
            this.btnThemTu.UseVisualStyleBackColor = false;
            this.btnThemTu.Click += new System.EventHandler(this.btnThemTu_Click);
            // 
            // lbDanhSach
            // 
            this.lbDanhSach.AutoSize = true;
            this.lbDanhSach.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDanhSach.Location = new System.Drawing.Point(123, 175);
            this.lbDanhSach.Name = "lbDanhSach";
            this.lbDanhSach.Size = new System.Drawing.Size(133, 20);
            this.lbDanhSach.TabIndex = 0;
            this.lbDanhSach.Text = "Danh sách từ mới";
            // 
            // lbxDSTu
            // 
            this.lbxDSTu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbxDSTu.FormattingEnabled = true;
            this.lbxDSTu.ItemHeight = 20;
            this.lbxDSTu.Location = new System.Drawing.Point(46, 219);
            this.lbxDSTu.Name = "lbxDSTu";
            this.lbxDSTu.Size = new System.Drawing.Size(284, 184);
            this.lbxDSTu.TabIndex = 3;
            this.lbxDSTu.SelectedIndexChanged += new System.EventHandler(this.lbxDSTu_SelectedIndexChanged);
            // 
            // lbNghiaCuaTu
            // 
            this.lbNghiaCuaTu.AutoSize = true;
            this.lbNghiaCuaTu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNghiaCuaTu.Location = new System.Drawing.Point(456, 175);
            this.lbNghiaCuaTu.Name = "lbNghiaCuaTu";
            this.lbNghiaCuaTu.Size = new System.Drawing.Size(98, 20);
            this.lbNghiaCuaTu.TabIndex = 0;
            this.lbNghiaCuaTu.Text = "Nghĩa của từ";
            // 
            // tbNghiaCuaTu
            // 
            this.tbNghiaCuaTu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNghiaCuaTu.Location = new System.Drawing.Point(416, 219);
            this.tbNghiaCuaTu.Multiline = true;
            this.tbNghiaCuaTu.Name = "tbNghiaCuaTu";
            this.tbNghiaCuaTu.Size = new System.Drawing.Size(184, 184);
            this.tbNghiaCuaTu.TabIndex = 1;
            // 
            // frmBai3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 430);
            this.Controls.Add(this.lbxDSTu);
            this.Controls.Add(this.btnThemTu);
            this.Controls.Add(this.tbNghiaTu);
            this.Controls.Add(this.tbNghiaCuaTu);
            this.Controls.Add(this.tbTuMoi);
            this.Controls.Add(this.lbNghiaCuaTu);
            this.Controls.Add(this.lbDanhSach);
            this.Controls.Add(this.lbNghiaTu);
            this.Controls.Add(this.lbTuMoi);
            this.Name = "frmBai3";
            this.Text = "frmBai3";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTuMoi;
        private System.Windows.Forms.Label lbNghiaTu;
        private System.Windows.Forms.TextBox tbTuMoi;
        private System.Windows.Forms.TextBox tbNghiaTu;
        private System.Windows.Forms.Button btnThemTu;
        private System.Windows.Forms.Label lbDanhSach;
        private System.Windows.Forms.ListBox lbxDSTu;
        private System.Windows.Forms.Label lbNghiaCuaTu;
        private System.Windows.Forms.TextBox tbNghiaCuaTu;
    }
}