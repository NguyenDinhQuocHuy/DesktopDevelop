namespace BaiTapWindowsForm
{
    partial class frmBai1
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
            this.lblThongTinHH = new System.Windows.Forms.Label();
            this.lblThongBao = new System.Windows.Forms.Label();
            this.tbxThongBao = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblThongTinHH
            // 
            this.lblThongTinHH.AutoSize = true;
            this.lblThongTinHH.Location = new System.Drawing.Point(72, 61);
            this.lblThongTinHH.Name = "lblThongTinHH";
            this.lblThongTinHH.Size = new System.Drawing.Size(121, 16);
            this.lblThongTinHH.TabIndex = 0;
            this.lblThongTinHH.Text = "Thông tin hàng hóa";
            // 
            // lblThongBao
            // 
            this.lblThongBao.AutoSize = true;
            this.lblThongBao.Location = new System.Drawing.Point(72, 103);
            this.lblThongBao.Name = "lblThongBao";
            this.lblThongBao.Size = new System.Drawing.Size(0, 16);
            this.lblThongBao.TabIndex = 0;
            // 
            // tbxThongBao
            // 
            this.tbxThongBao.Location = new System.Drawing.Point(75, 97);
            this.tbxThongBao.Name = "tbxThongBao";
            this.tbxThongBao.ReadOnly = true;
            this.tbxThongBao.Size = new System.Drawing.Size(314, 22);
            this.tbxThongBao.TabIndex = 1;
            // 
            // frmBai1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tbxThongBao);
            this.Controls.Add(this.lblThongBao);
            this.Controls.Add(this.lblThongTinHH);
            this.Name = "frmBai1";
            this.Text = "Bài 1";
            this.Load += new System.EventHandler(this.frmBai1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblThongTinHH;
        private System.Windows.Forms.Label lblThongBao;
        private System.Windows.Forms.TextBox tbxThongBao;
    }
}