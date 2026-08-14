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
    public partial class frmBai2 : Form
    {
        public frmBai2()
        {
            InitializeComponent();
        }

        

        private void btnChonHang_Click(object sender, EventArgs e)
        {
            var item = lbDanhSachHangHoa.SelectedItems;
            lbKhachMua.Items.Add(item);
        }
    }
}
