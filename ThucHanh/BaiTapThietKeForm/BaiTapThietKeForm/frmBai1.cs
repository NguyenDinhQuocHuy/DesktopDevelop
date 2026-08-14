using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiTapThietKeForm
{
    public partial class frmBai1 : Form
    {
        public frmBai1()
        {
            InitializeComponent();
        }

        
        private void rbXanh_CheckedChanged(object sender, EventArgs e)
        {
               tbDonGia.Text = "22000";
             
        }

        private void rbDo_CheckedChanged(object sender, EventArgs e)
        {
            tbDonGia.Text = "21000";
        }

        private void rbTrang_CheckedChanged(object sender, EventArgs e)
        {
            tbDonGia.Text = "20000";
        }

        private void btnTinhTien_Click(object sender, EventArgs e)
        {
            int soLuong = int.Parse(nudSoLuong.Text);
            int donGia = int.Parse(tbDonGia.Text);
            int tongTien = soLuong * donGia;
            tbTongTien.Text = tongTien.ToString();
        }

        private void frmBai1_Load(object sender, EventArgs e)
        {

        }
    }
}
