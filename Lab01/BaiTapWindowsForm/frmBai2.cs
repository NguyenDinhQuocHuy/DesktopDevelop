using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiTapWindowsForm
{
    public partial class frmBai2 : Form
    {
        public frmBai2()
        {
            InitializeComponent();
        }

        private void cbbTenHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbbTenHang.Text)
            {
                case "Chuột":
                    tbxDonGia.Text = "100000";
                    break;
                case "Máy in":
                    tbxDonGia.Text = "2000000";
                    break;
                case "Bàn phím":
                    tbxDonGia.Text = "150000";
                    break;          
                    
            }
        }

        private void btnTinhTien_Click(object sender, EventArgs e)
        {
            int donGia = int.Parse(tbxDonGia.Text);
            int soLuong = int.Parse(nudSoLuong.Text);
            int thanhTien = donGia * soLuong;
            if (rbChuyenKhoan.Checked)
            {
                thanhTien = (int)(thanhTien * 0.95);
            } 
            
            lblThanhToan.Text = "Số tiền thanh toán: " + thanhTien.ToString();
        }
    }
}
