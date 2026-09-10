using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLySinhVien
{
    public enum TuyChon
    {
        MaSV,
        HoTen,
        NgaySinh
    }

    public partial class frmTuyChon : Form
    {
        private QuanLySinhVien qlsv;
        private frmSinhVien mainForm;

        public TuyChon Kieu { get; set; }
        public string ChuoiTim { get; set; }

        public frmTuyChon(QuanLySinhVien qlsv, frmSinhVien mainForm)
        {
            InitializeComponent();
            this.qlsv = qlsv;
            this.mainForm = mainForm;
            Kieu = TuyChon.MaSV;
        }

        private void rdTuyChon_Click(object sender, EventArgs e)
        {
            if (rdMaSV.Checked)
                Kieu = TuyChon.MaSV;
            else if (rdHoTen.Checked)
                Kieu = TuyChon.HoTen;
            else if (rdNgaySinh.Checked)
                Kieu = TuyChon.NgaySinh;
        }

        private void btnSapXep_Click(object sender, EventArgs e)
        {
            if (rdMaSV.Checked)
            {
                qlsv.SapXep((a, b) => string.Compare(((SinhVien)a).MaSo, ((SinhVien)b).MaSo, StringComparison.OrdinalIgnoreCase));
            }
            else if (rdHoTen.Checked)
            {
                qlsv.SapXep((a, b) => string.Compare(((SinhVien)a).HoTen, ((SinhVien)b).HoTen, StringComparison.OrdinalIgnoreCase));
            }
            else if (rdNgaySinh.Checked)
            {
                qlsv.SapXep((a, b) => DateTime.Compare(((SinhVien)a).NgaySinh, ((SinhVien)b).NgaySinh));
            }

            mainForm.LoadListView();
            this.Close();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            ChuoiTim = txtThongTin.Text.Trim();
            if (string.IsNullOrEmpty(ChuoiTim))
            {
                MessageBox.Show("Hãy nhập thông tin tìm!", "Lỗi nhập thông tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<SinhVien> dsTimThay = new List<SinhVien>();

            if (rdMaSV.Checked)
            {
                dsTimThay = qlsv.TimDanhSach(ChuoiTim, (a, b) =>
                {
                    string ma = (string)a;
                    SinhVien sv = (SinhVien)b;
                    return sv.MaSo != null && sv.MaSo.IndexOf(ma, StringComparison.OrdinalIgnoreCase) >= 0 ? 0 : 1;
                });
            }
            else if (rdHoTen.Checked)
            {
                dsTimThay = qlsv.TimDanhSach(ChuoiTim, (a, b) =>
                {
                    string ten = (string)a;
                    SinhVien sv = (SinhVien)b;
                    return sv.HoTen != null && sv.HoTen.IndexOf(ten, StringComparison.OrdinalIgnoreCase) >= 0 ? 0 : 1;
                });
            }
            else if (rdNgaySinh.Checked)
            {
                dsTimThay = qlsv.TimDanhSach(ChuoiTim, (a, b) =>
                {
                    string ns = (string)a;
                    SinhVien sv = (SinhVien)b;
                    string strDate = sv.NgaySinh.ToString("dd/MM/yyyy");
                    string strDate2 = sv.NgaySinh.ToString("M/d/yyyy");
                    return (strDate.Contains(ns) || strDate2.Contains(ns) || sv.NgaySinh.Year.ToString().Contains(ns)) ? 0 : 1;
                });
            }

            MessageBox.Show($"Số sinh viên tìm Thấy:{dsTimThay.Count}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (dsTimThay.Count > 0)
            {
                mainForm.HienThiDanhSachTuyChon(dsTimThay);
                this.Close();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
