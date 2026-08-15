using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       
        private void rbNu_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNu.Checked) 
            {
                MessageBox.Show("Bạn đã chọn giới tính Nữ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbNam_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNam.Checked)
            {
                MessageBox.Show("Bạn đã chọn giới tính Nam", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnToMau_Click(object sender, EventArgs e)
        {
            if (rbDo.Checked)
            {
                txtHopMau.BackColor = Color.Red;
            }
            else if (rbXanh.Checked)
            {
                txtHopMau.BackColor = Color.Blue;
            }
        }
    }
}
