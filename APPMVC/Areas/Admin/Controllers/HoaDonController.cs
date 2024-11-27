using AppData.Model;
using AppData.ViewModel;
using APPMVC.IService;
using APPMVC.Service;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using static AppData.ViewModel.HoaDonChiTietViewModel;

namespace APPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoaDonController : Controller
    {
       
        private readonly ISanPhamChiTietService _sanPhamCTService;
        private readonly ISanPhamService _sanPhamService;
        private readonly IKhachHangService _khachHangService;
        private readonly IHoaDonChiTietService _hoaDonChiTietService;
        private readonly IHoaDonService _hoaDonService;
        private readonly INhanVienService _nhanVienService;
        private readonly ILichSuHoaDonService _lichSuHoaDonService;
        private readonly IMauSacService _mauSacService;
        private readonly IKichCoService _kichCoService;
        private readonly ISanPhamChiTietMauSacService _sanPhamChiTietMauSacService;
        private readonly ISanPhamChiTietKichCoService _sanPhamChiTietKichCoService;
        private readonly IHinhAnhService _hinhAnhService;
        private readonly ILichSuThanhToanService _lichSuThanhToanService;

        public HoaDonController(ISanPhamChiTietService sanPhamChiTietService, 
            ISanPhamService sanPhamService, 
            IKhachHangService khachHangService, 
            IHoaDonChiTietService hoaDonChiTietService , 
            IHoaDonService hoaDonService , 
            INhanVienService nhanVienService, 
            ILichSuHoaDonService lichSuHoaDonService, 
            IMauSacService mauSacService,
            IKichCoService kichCoService,
            ISanPhamChiTietMauSacService sanPhamChiTietMauSacService,
            ISanPhamChiTietKichCoService sanPhamChiTietKichCoService,
            IHinhAnhService hinhAnhService,
            ILichSuThanhToanService lichSuThanhToanService)
        {
            _sanPhamCTService = sanPhamChiTietService;
            _sanPhamService = sanPhamService;
            _khachHangService = khachHangService ;
            _hoaDonChiTietService = hoaDonChiTietService;
            _hoaDonService = hoaDonService;
            _nhanVienService = nhanVienService;
            _lichSuHoaDonService = lichSuHoaDonService;
            _mauSacService = mauSacService;
            _kichCoService = kichCoService;
            _sanPhamChiTietMauSacService = sanPhamChiTietMauSacService;
            _sanPhamChiTietKichCoService = sanPhamChiTietKichCoService;
            _hinhAnhService = hinhAnhService;
            _lichSuThanhToanService = lichSuThanhToanService;
        }
        [HttpGet]
        public async Task<ActionResult> Index(int page = 1, string status = null)
        {
            page = page < 1 ? 1 : page;
            const int pageSize = 5;

            var hoaDons = await _hoaDonService.GetAllAsync() ?? new List<HoaDon>();

            var filteredHoaDons = hoaDons
                .Where(h => h.TrangThai != "Tạo đơn hàng")
                .ToList();

            if (!string.IsNullOrEmpty(status))
            {
                filteredHoaDons = filteredHoaDons.Where(h => h.TrangThai == status).ToList();
            }

            var distinctStatuses = hoaDons
                .Select(h => h.TrangThai)
                .Where(s => s != "Tạo đơn hàng")
                .Distinct()
                .ToList();

            var hoaDonViewModels = new List<AppData.ViewModel.HoaDonViewModell>();

            foreach (var hoaDon in filteredHoaDons)
            {
                var hoaDonChiTiets = await _hoaDonChiTietService.GetByIdHoaDonAsync(hoaDon.IdHoaDon);
                var totalQuantity = hoaDonChiTiets.Sum(d => d.SoLuong);

                hoaDonViewModels.Add(new AppData.ViewModel.HoaDonViewModell
                {
                    HoaDon = hoaDon,
                    TotalQuantity = totalQuantity,
                    HoaDonChiTiets = hoaDonChiTiets
                });
            }

            filteredHoaDons = filteredHoaDons.OrderByDescending(h => h.NgayTao).ToList();

            var pagedHoaDons = hoaDonViewModels.ToPagedList(page, pageSize);

            ViewBag.CurrentStatus = status;
            ViewBag.TotalProductQuantity = hoaDonViewModels.Sum(vm => vm.TotalQuantity);
            ViewBag.Statuses = distinctStatuses;

            return View(pagedHoaDons);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(Guid id)
        {
            var hoaDonChiTietList = await _hoaDonChiTietService.GetByIdHoaDonAsync(id);
            if (hoaDonChiTietList == null || !hoaDonChiTietList.Any())
            {
                return NotFound($"Hóa đơn chi tiết với ID {id} không tồn tại.");
            }

            var hoaDon = await _hoaDonService.GetByIdAsync(hoaDonChiTietList.First().IdHoaDon);
            if (hoaDon == null)
            {
                return NotFound($"Hóa đơn với ID {hoaDon.IdHoaDon} không tồn tại.");
            }

            var lichSuHoaDons = await _lichSuHoaDonService.GetByIdHoaDonAsync(id);
            //var lichSuThanhToans = await _lichSuThanhToanService.GetByIdHoaDonAsync(id);
            var khachhang = await _khachHangService.GetIdKhachHang(hoaDon.IdKhachHang);

            var sanPhamChiTiets = new List<HoaDonChiTietViewModel.SanPhamChiTietViewModel>();
            foreach (var hoaDonChiTiet in hoaDonChiTietList)
            {
                var sanPhamCT = await _sanPhamCTService.GetSanPhamChiTietById(hoaDonChiTiet.IdSanPhamChiTiet);
                var sanPham = await _sanPhamCTService.GetSanPhamByIdSanPhamChiTietAsync(hoaDonChiTiet.IdSanPhamChiTiet);

                if (sanPhamCT != null)
                {

                    var mauSacList = await _sanPhamChiTietMauSacService.GetMauSacIdsBySanPhamChiTietId(sanPhamCT.IdSanPhamChiTiet);
                    var mauSacTenList = mauSacList.Select(ms => ms.TenMauSac).ToList();

                    var kichCoList = await _sanPhamChiTietKichCoService.GetKichCoIdsBySanPhamChiTietId(sanPhamCT.IdSanPhamChiTiet);
                    var kichCoTenList = kichCoList.Select(kc => kc.TenKichCo).ToList();

                    var hinhAnhs = await _hinhAnhService.GetHinhAnhsBySanPhamChiTietId(sanPhamCT.IdSanPhamChiTiet);


                    sanPhamChiTiets.Add(new HoaDonChiTietViewModel.SanPhamChiTietViewModel
                    {
                        IdSanPhamChiTiet = sanPhamCT.IdSanPhamChiTiet,
                        Quantity = hoaDonChiTiet.SoLuong,
                        Price = sanPhamCT.Gia,
                        ProductName = sanPham.TenSanPham,
                        MauSac = mauSacTenList,
                        KichCo = kichCoTenList,
                        HinhAnhs = hinhAnhs 
                    });
                }
            }

            var viewModel = new HoaDonChiTietViewModel
            {
                DonGia = hoaDon.TongTienDonHang,
                GiamGia = hoaDon.TienGiam,
                PhiVanChuyen = Convert.ToDouble(hoaDon.TienShip),
                TongTien = hoaDon.TongTienDonHang + Convert.ToDouble(hoaDon.TienShip),

                HoaDon = new HoaDonViewModel
                {
                    IdHoaDon = hoaDon.IdHoaDon,
                    MaDon = hoaDon.MaDon,
                    KhachHang = khachhang.HoTen,
                    TrangThai = hoaDon.TrangThai,
                    SoDienThoaiNguoiNhan = hoaDon.SoDienThoaiNguoiNhan,
                    LoaiHoaDon = hoaDon.LoaiHoaDon,
                    DiaChiGiaoHang = hoaDon.DiaChiGiaoHang
                },

                LichSuHoaDons = lichSuHoaDons.Select(lichSu => new HoaDonChiTietViewModel.LichSuHoaDonViewModel
                {
                    IdLichSuHoaDon = lichSu.IdLichSuHoaDon,
                    ThaoTac = lichSu.ThaoTac,
                    NgayTao = lichSu.NgayTao,
                    NguoiThaoTac = lichSu.NguoiThaoTac,
                    TrangThai = lichSu.TrangThai,
                    IdHoaDon = lichSu.IdHoaDon
                }).ToList(),

                SanPhamChiTiets = sanPhamChiTiets,

                //LichSuThanhToans = lichSuThanhToans.Select(payment => new LichSuThanhToanViewModel
                //{
                //    IdLichSuThanhToan = payment.IdLichSuThanhToan,
                //    SoTien = payment.SoTien,
                //    NgayThanhToan = payment.NgayTao,
                //    LoaiGiaoDich = payment.LoaiGiaoDich,
                //    HinhThucThanhToan = payment.Pttt,
                //    TrangThai = payment.TrangThai,
                //    IdHoaDon = payment.IdHoaDon
                //}).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HoaDonChiTietViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var hoaDonChiTiet = new HoaDonChiTiet
                    {
                        IdHoaDonChiTiet = viewModel.IdHoaDonChiTiet,
                        DonGia = viewModel.DonGia,
                        SoLuong = viewModel.SoLuong,
                        TongTien = viewModel.TongTien,
                        KichHoat = viewModel.KichHoat,
                        IdHoaDon = viewModel.IdHoaDon
                    };

                    //await _hoaDonChiTietService.UpdateAsync(hoaDonChiTiet);
                    TempData["Success"] = "Cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi: {ex.Message}";
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInvoiceStatus(Guid id)
        {
            var hoaDon = await _hoaDonService.GetByIdAsync(id);
            if (hoaDon == null)
            {
                return NotFound($"Hóa đơn với ID {id} không tồn tại.");
            }

            switch (hoaDon.TrangThai)
            {
                case "Chờ Xác Nhận":
                    hoaDon.TrangThai = "Chờ Giao Hàng";
                    await _hoaDonService.UpdateAsync(hoaDon);

                    await _lichSuHoaDonService.AddAsync(new LichSuHoaDon
                    {
                        IdHoaDon = hoaDon.IdHoaDon,
                        ThaoTac = "Chờ Giao Hàng",
                        NgayTao = DateTime.Now,
                        NguoiThaoTac = "Nhân Viên",
                        TrangThai = hoaDon.TrangThai
                    });
                    break;

                case "Chờ Giao Hàng":
                    hoaDon.TrangThai = "Đang Vận Chuyển";
                    await _hoaDonService.UpdateAsync(hoaDon);

                    await _lichSuHoaDonService.AddAsync(new LichSuHoaDon
                    {
                        IdHoaDon = hoaDon.IdHoaDon,
                        ThaoTac = "Đang Vận Chuyển",
                        NgayTao = DateTime.Now,
                        NguoiThaoTac = "Nhân Viên",
                        TrangThai = hoaDon.TrangThai
                    });
                    break;

                case "Đang Vận Chuyển":
                    hoaDon.TrangThai = "Đã Giao Hàng";
                    await _hoaDonService.UpdateAsync(hoaDon);

                    await _lichSuHoaDonService.AddAsync(new LichSuHoaDon
                    {
                        IdHoaDon = hoaDon.IdHoaDon,
                        ThaoTac = "Đã Giao Hàng",
                        NgayTao = DateTime.Now,
                        NguoiThaoTac = "Nhân Viên",
                        TrangThai = hoaDon.TrangThai
                    });
                    break;

                case "Đã Giao Hàng":
                    hoaDon.TrangThai = "Hoàn Thành"; 
                    await _hoaDonService.UpdateAsync(hoaDon);

                    await _lichSuHoaDonService.AddAsync(new LichSuHoaDon
                    {
                        IdHoaDon = hoaDon.IdHoaDon,
                        ThaoTac = "Hoàn Thành",
                        NgayTao = DateTime.Now,
                        NguoiThaoTac = "Nhân Viên",
                        TrangThai = hoaDon.TrangThai
                    });
                    break;
            }

            return RedirectToAction("Edit", new { id });
        }

    }
}
