using AppData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModel
{
    public class HoaDonCTViewModel
    {
        public HoaDonChiTiet HoaDonChiTiet { get; set; }
        public List<LichSuHoaDon> LichSuHoaDon { get; set; } 
        public SanPhamChiTiet SanPhamChiTiet { get; set; }
        public KhachHang KhachHang { get; set; }

        public HoaDonCTViewModel()
        {
            HoaDonChiTiet = new HoaDonChiTiet();
            LichSuHoaDon = new List<LichSuHoaDon>(); 
            SanPhamChiTiet = new SanPhamChiTiet();
            KhachHang = new KhachHang();
        }
    }

}
