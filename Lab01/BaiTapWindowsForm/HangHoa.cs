using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiTapWindowsForm
{
    internal class HangHoa
    {
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string DVT { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }

        public HangHoa()
        {
            SoLuong = 0;
            DonGia = 0;
        }

        public HangHoa(string maHang, string tenHang, string dvt, int soLuong, int donGia)
        {
            MaHang = maHang;
            TenHang = tenHang;
            DVT = dvt;
            SoLuong = soLuong;
            DonGia = donGia;
        }

        public string HienThi()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}", MaHang, TenHang, DVT, SoLuong, DonGia);
        }
    }
}
