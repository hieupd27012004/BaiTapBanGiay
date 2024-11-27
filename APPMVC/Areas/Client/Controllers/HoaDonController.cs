using AppData.ViewModel;
using APPMVC.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APPMVC.Areas.Client.Controllers
{
    [Area("Client")]
    public class HoaDonController : Controller
    {
        public IHoaDonService _hoaDonService;
        public IHoaDonChiTietService _hoaDonChiTietService;
        public ILichSuHoaDonService _lichSuHoaDonService;
        public ISanPhamChiTietService _sanPhamChiTietService;
        public IKhachHangService _khachHangService;
        public HoaDonController(IHoaDonService hoaDonService, IHoaDonChiTietService hoaDonChiTietService , ILichSuHoaDonService lichSuHoaDonService, ISanPhamChiTietService sanPhamChiTietService, IKhachHangService khachHangService) 
        {
            _hoaDonService = hoaDonService;
            _hoaDonChiTietService = hoaDonChiTietService;
            _lichSuHoaDonService = lichSuHoaDonService;
            _sanPhamChiTietService = sanPhamChiTietService;
            _khachHangService = khachHangService;
        }
        public async Task<IActionResult> Index()
        {
            var hoaDons = await _hoaDonService.GetAllAsync();
            return View(hoaDons);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            // Lấy thông tin hóa đơn chi tiết dựa trên ID
            var hoaDonChiTiet = await _hoaDonService.GetByIdAsync(id);
            if (hoaDonChiTiet == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy hóa đơn
            }

            // Lấy lịch sử hóa đơn dựa trên IdHoaDon
            var lichSuHoaDonList = await _lichSuHoaDonService.GetByIdHoaDonAsync(hoaDonChiTiet.IdHoaDon);

            // Lấy thông tin sản phẩm chi tiết dựa trên IdSanPham
            //var sanPhamChiTiet = await _sanPhamChiTietService.GetSanPhamChiTietById(hoaDonChiTiet.IdSanPham);

            //// Lấy thông tin khách hàng dựa trên IdKhachHang
            //var khachHang = await _khachHangService.GetByIdAsync(hoaDonChiTiet.IdKhachHang);

            // Tạo ViewModel và gán giá trị
            var viewModel = new HoaDonCTViewModel
            {
                //HoaDonChiTiet = hoaDonChiTiet,
                LichSuHoaDon = lichSuHoaDonList,
                //SanPhamChiTiet = sanPhamChiTiet,
                //KhachHang = khachHang
            };

            return View(viewModel); // Trả về View với ViewModel
        }
    }
}
