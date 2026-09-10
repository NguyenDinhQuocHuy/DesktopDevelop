using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySinhVien
{
    public delegate int SoSanh(object sv1, object sv2);

    public class QuanLySinhVien
    {
        public List<SinhVien> dsSinhVien;

        public QuanLySinhVien()
        {
            dsSinhVien = new List<SinhVien>();
        }

        public SinhVien this[int index]
        {
            get { return this.dsSinhVien[index]; }
            set { dsSinhVien[index] = value; }
        }

        public void Them(SinhVien sv)
        {
            this.dsSinhVien.Add(sv);
        }

        public SinhVien Tim(object obj, SoSanh ss)
        {
            SinhVien svresult = null;
            foreach (SinhVien sv in dsSinhVien)
            {
                if (ss(obj, sv) == 0)
                {
                    svresult = sv;
                    break;
                }
            }
            return svresult;
        }

        public List<SinhVien> TimDanhSach(object obj, SoSanh ss)
        {
            List<SinhVien> listResult = new List<SinhVien>();
            foreach (SinhVien sv in dsSinhVien)
            {
                if (ss(obj, sv) == 0)
                {
                    listResult.Add(sv);
                }
            }
            return listResult;
        }

        public bool Sua(SinhVien svsua, object obj, SoSanh ss)
        {
            int i, count;
            bool kq = false;
            count = this.dsSinhVien.Count - 1;
            for (i = 0; i <= count; i++)
            {
                if (ss(obj, this[i]) == 0)
                {
                    this[i] = svsua;
                    kq = true;
                    break;
                }
            }
            return kq;
        }

        public void Xoa(object obj, SoSanh ss)
        {
            int i = dsSinhVien.Count - 1;
            for (; i >= 0; i--)
            {
                if (ss(obj, this[i]) == 0)
                    this.dsSinhVien.RemoveAt(i);
            }
        }

        public void DocTuFile(string filename)
        {
            if (!File.Exists(filename)) return;

            string t;
            string[] s;
            SinhVien sv;
            using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read), Encoding.UTF8))
            {
                while ((t = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(t)) continue;
                    s = t.Split('\t');
                    if (s.Length < 8) continue;
                    sv = new SinhVien();
                    sv.MaSo = s[0].Trim();
                    sv.HoTen = s[1].Trim();
                    DateTime d;
                    if (DateTime.TryParse(s[2].Trim(), out d))
                        sv.NgaySinh = d;
                    else
                        sv.NgaySinh = DateTime.Now;
                    sv.DiaChi = s[3].Trim();
                    sv.Lop = s[4].Trim();
                    sv.Hinh = s[5].Trim();
                    sv.GioiTinh = false;
                    if (s[6].Trim() == "1")
                        sv.GioiTinh = true;
                    string[] cn = s[7].Split(',');
                    foreach (string c in cn)
                    {
                        if (!string.IsNullOrWhiteSpace(c))
                            sv.ChuyenNganh.Add(c.Trim());
                    }
                    this.Them(sv);
                }
            }
        }

        public void GhiRaFile(string filename)
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                foreach (SinhVien sv in dsSinhVien)
                {
                    string gt = sv.GioiTinh ? "1" : "0";
                    string cn = string.Join(",", sv.ChuyenNganh);
                    sw.WriteLine($"{sv.MaSo}\t{sv.HoTen}\t{sv.NgaySinh:MM/dd/yyyy}\t{sv.DiaChi}\t{sv.Lop}\t{sv.Hinh}\t{gt}\t{cn}");
                }
            }
        }

        public void SapXep(SoSanh ss)
        {
            dsSinhVien.Sort((a, b) => ss(a, b));
        }
    }
}
