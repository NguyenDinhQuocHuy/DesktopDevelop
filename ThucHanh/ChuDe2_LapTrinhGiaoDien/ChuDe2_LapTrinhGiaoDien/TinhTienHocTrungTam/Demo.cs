using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinhTienHocTrungTam
{
    public partial class Demo : Form
    {
        public Demo()
        {
            InitializeComponent();
        }

        private void btnTinhTien_Click(object sender, EventArgs e)
        {
            int s = 0;
            if (chkTinHocA.Checked) s += int.Parse(lblTienTHA.Text.Split('.')[0]);
            if (chkTinHocB.Checked) s += int.Parse(lblTienTHB.Text.Split('.')[0]);
            if (chkTiengAnhA.Checked) s += int.Parse(lblTienTAA.Text.Split('.')[0]);
            if (chkTiengAnhB.Checked) s += int.Parse(lblTienTAB.Text.Split('.')[0]);

            this.txtTongTien.Text = s + ".000 đồng";
        }

        private void Reset()
        {
            this.cboMaHV.Text = string.Empty;
            this.txtHoTen.Text = string.Empty;
            this.dtpNgayDangKy.Value = DateTime.Now;
            this.rdNam.Checked = true;
            this.chkTiengAnhA.Checked = false;
            this.chkTiengAnhB.Checked = false;
            this.chkTinHocA.Checked = false;
            this.chkTinHocB.Checked = false;
            this.txtTongTien.Text = string.Empty;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Reset();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
