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
    public partial class frmBai4 : Form
    {
        public frmBai4()
        {
            InitializeComponent();
        }

        private void frmBai4_Load(object sender, EventArgs e)
        {
            Random random = new Random();
            for (int i = 0; i < 10; i++) 
            {           
                lbxDanhSach.Items.Add(random.Next(1, 101));
            }
        }

        private void btnTimSo_Click(object sender, EventArgs e)
        {
            var input = nudNhap.Text.Trim();
            foreach (var item in lbxDanhSach.Items)
            {
                if (item.ToString() == input)
                {
                    txtKetQua.Text = "Số tìm thấy!";
                    return;
                }
            }
            txtKetQua.Text = "Số không tìm thấy!";
        }
       
    }
}
