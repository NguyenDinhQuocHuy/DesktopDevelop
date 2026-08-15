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
    public partial class frmBai3 : Form
    {
        List<string> list = new List<string>();
        public frmBai3()
        {
            InitializeComponent();
        }

        private void btnThemTu_Click(object sender, EventArgs e)
        {
            var tuMoi = tbTuMoi.Text;
            var nghiaTu = tbNghiaTu.Text;
            lbxDSTu.Items.Add(tuMoi);
            list.Add(nghiaTu);

            tbTuMoi.Focus();
            tbTuMoi.Text = "";
            tbNghiaTu.Text = "";
        }

        private void lbxDSTu_SelectedIndexChanged(object sender, EventArgs e)
        {
            var stt = lbxDSTu.SelectedIndex;
            tbNghiaCuaTu.Text = list[stt];
        }

        
    }
}
