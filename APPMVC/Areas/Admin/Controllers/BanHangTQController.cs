using AppData.Model;
using AppData.Model.Vnpay;
using AppData.ViewModel;
using APPMVC.IService;
using APPMVC.Libraries;
using APPMVC.Service.Vnpay;
using DinkToPdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using static AppData.ViewModel.HoaDonChiTietViewModel;
using SanPhamChiTietViewModel = AppData.ViewModel.HoaDonChiTietViewModel.SanPhamChiTietViewModel;

namespace APPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BanHangTQController : Controller
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
        private readonly IVnPayService _vnPayServie;
        private readonly IConfiguration _configuration;

        public BanHangTQController(
            ISanPhamChiTietService sanPhamChiTietService,
            ISanPhamService sanPhamService,
            IKhachHangService khachHangService,
            IHoaDonChiTietService hoaDonChiTietService,
            IHoaDonService hoaDonService,
            INhanVienService nhanVienService,
            ILichSuHoaDonService lichSuHoaDonService,
            IMauSacService mauSacService,
            IKichCoService kichCoService,
            ISanPhamChiTietMauSacService sanPhamChiTietMauSacService,
            ISanPhamChiTietKichCoService sanPhamChiTietKichCoService,
            IHinhAnhService hinhAnhService,
            IVnPayService vnPayServie,
            IConfiguration configuration
            )
        {
            _sanPhamCTService = sanPhamChiTietService;
            _sanPhamService = sanPhamService;
            _khachHangService = khachHangService;
            _hoaDonChiTietService = hoaDonChiTietService;
            _hoaDonService = hoaDonService;
            _nhanVienService = nhanVienService;
            _lichSuHoaDonService = lichSuHoaDonService;
            _mauSacService = mauSacService;
            _kichCoService = kichCoService;
            _sanPhamChiTietMauSacService = sanPhamChiTietMauSacService;
            _sanPhamChiTietKichCoService = sanPhamChiTietKichCoService;
            _hinhAnhService = hinhAnhService;
            _vnPayServie = vnPayServie;
            _configuration = configuration;
        }

        public async Task<ActionResult> Index()
        {
            var hoaDons = await _hoaDonService.GetAllAsync();

            if (hoaDons == null || !hoaDons.Any())
            {
                return View(new List<HoaDonChiTietViewModel>());
            }

            var filteredOrders = hoaDons
                .Where(hd => hd.TrangThai == "Tạo đơn hàng")
                .Select(hd => new HoaDonChiTietViewModel
                {
                    IdHoaDon = hd.IdHoaDon,
                    HoaDon = new HoaDonChiTietViewModel.HoaDonViewModel
                    {
                        MaDon = hd.MaDon
                    }
                })
                .Take(5)
                .ToList();

            return View(filteredOrders);
        }
        [HttpPost]
        public async Task<IActionResult> TaoHD()
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            var NVIdString = HttpContext.Session.GetString("IdNhanVien");

            if (string.IsNullOrEmpty(NVIdString) || !Guid.TryParse(NVIdString, out Guid NVID))
            {
                return Unauthorized(new { message = "Nhân viên không tồn tại trong phiên làm việc." });
            }

            var currentOrders = await _hoaDonService.GetAllAsync();
            var countPendingOrders = currentOrders.Count(hd => hd.TrangThai == "Tạo đơn hàng");

            if (countPendingOrders >= 5)
            {
                ModelState.AddModelError("", "Không thể tạo thêm đơn hàng. Đã đạt giới hạn tối đa 5 đơn hàng với trạng thái 'Tạo đơn hàng'.");
                return View("Index");
            }

            var order = new HoaDon
            {
                IdHoaDon = Guid.NewGuid(),
                MaDon = await GenerateOrderNumberAsync(),
                NguoiNhan = null,
                SoDienThoaiNguoiNhan = null,
                DiaChiGiaoHang = null,
                LoaiHoaDon = "Tại Quầy",
                TienShip = 0,
                TongTienDonHang = 0,
                TongTienHoaDon = 0,
                NgayTao = DateTime.Now,
                NguoiTao = "Nhân Viên",
                KichHoat = 1,
                TrangThai = "Tạo đơn hàng",
                IdKhachHang = null,
                IdNhanVien = NVID
            };

            try
            {
                await _hoaDonService.AddAsync(order);

                var lichSu = new LichSuHoaDon
                {
                    IdLichSuHoaDon = Guid.NewGuid(),
                    ThaoTac = order.TrangThai,
                    NgayTao = DateTime.Now,
                    NguoiThaoTac = "Nhân Viên",
                    TrangThai = "1",
                    IdHoaDon = order.IdHoaDon,
                };

                await _lichSuHoaDonService.AddAsync(lichSu);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi xử lý đơn hàng. Vui lòng thử lại.");
                Console.WriteLine(ex.Message);
            }

            return View("Index");
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var lastOrderNumber = await GetLastOrderNumberAsync();
            int newOrderNumber = lastOrderNumber + 1;
            string newOrderNumberString = $"HD{newOrderNumber}";

            while (await IsOrderNumberExists(newOrderNumberString))
            {
                newOrderNumber++;
                newOrderNumberString = $"HD{newOrderNumber}";
            }

            return newOrderNumberString;
        }

        private async Task<int> GetLastOrderNumberAsync()
        {
            var allOrders = await _hoaDonService.GetAllAsync();
            if (allOrders == null || !allOrders.Any())
            {
                return 0;
            }

            var orderNumbers = new List<int>();
            foreach (var order in allOrders)
            {
                if (order.MaDon.Length > 2 &&
                    int.TryParse(order.MaDon.Substring(2), out int orderNumber))
                {
                    orderNumbers.Add(orderNumber);
                }
            }

            return orderNumbers.Any() ? orderNumbers.Max() : 0;
        }
        private async Task<bool> IsOrderNumberExists(string orderNumber)
        {
            var existingOrder = await _hoaDonService.GetByOrderNumberAsync(orderNumber);
            return existingOrder != null;
        }

        [HttpGet]
        public async Task<ActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("ID không hợp lệ.");
            }

            var hoaDon = await _hoaDonService.GetByIdAsync(id);
            if (hoaDon == null)
            {
                return NotFound($"Hóa đơn với ID {id} không tồn tại.");
            }

            var hoaDonChiTietList = await _hoaDonChiTietService.GetByIdHoaDonAsync(id) ?? new List<HoaDonChiTiet>();

            var sanPhamChiTiets = new List<HoaDonChiTietViewModel.SanPhamChiTietViewModel>();

            foreach (var hoaDonChiTiet in hoaDonChiTietList)
            {
                var sanPhamCT = await _sanPhamCTService.GetSanPhamChiTietById(hoaDonChiTiet.IdSanPhamChiTiet);
                if (sanPhamCT != null)
                {
                    var sanPham = await _sanPhamCTService.GetSanPhamByIdSanPhamChiTietAsync(hoaDonChiTiet.IdSanPhamChiTiet);

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
                        ProductName = sanPham?.TenSanPham,
                        MauSac = mauSacTenList,
                        KichCo = kichCoTenList,
                        HinhAnhs = hinhAnhs,
                        IdHoaDonChiTiet = hoaDonChiTiet.IdHoaDonChiTiet
                    });
                }
            }

            double tongTienHang = hoaDonChiTietList.Sum(hoaDonChiTiet => hoaDonChiTiet.SoLuong * hoaDonChiTiet.DonGia);

            var viewModel = new HoaDonChiTietViewModel
            {
                DonGia = tongTienHang,
                GiamGia = hoaDon.TienGiam,
                TongTien = tongTienHang,
                HoaDon = new HoaDonChiTietViewModel.HoaDonViewModel
                {
                    IdHoaDon = hoaDon.IdHoaDon,
                    MaDon = hoaDon.MaDon,
                    TrangThai = hoaDon.TrangThai,
                    SoDienThoaiNguoiNhan = hoaDon.SoDienThoaiNguoiNhan,
                    LoaiHoaDon = hoaDon.LoaiHoaDon,
                    DiaChiGiaoHang = hoaDon.DiaChiGiaoHang
                },
                SanPhamChiTiets = sanPhamChiTiets 
            };

            return PartialView("Edit", viewModel);
        }

        public async Task<ActionResult> GetSanPhamChiTietList(Guid idHoaDon)
        {
            try
            {
                var sanPhamChiTietList = await _sanPhamCTService.GetSanPhamChiTiets();

                var sanPhamChiTietViewModels = new List<SanPhamChiTietViewModel>();
                if (sanPhamChiTietList != null && sanPhamChiTietList.Any())
                {
                    var tasks = sanPhamChiTietList.Select(async sanPhamCT =>
                    {
                        var sanPham = await _sanPhamCTService.GetSanPhamByIdSanPhamChiTietAsync(sanPhamCT.IdSanPhamChiTiet);
                        var mauSacList = await _sanPhamChiTietMauSacService.GetMauSacIdsBySanPhamChiTietId(sanPhamCT.IdSanPhamChiTiet);
                        var mauSacTenList = mauSacList.Select(ms => ms.TenMauSac).ToList();

                        var kichCoList = await _sanPhamChiTietKichCoService.GetKichCoIdsBySanPhamChiTietId(sanPhamCT.IdSanPhamChiTiet);
                        var kichCoTenList = kichCoList.Select(kc => kc.TenKichCo).ToList();

                        var hinhAnhs = await _hinhAnhService.GetHinhAnhsBySanPhamChiTietId(sanPhamCT.IdSanPhamChiTiet);

                        sanPhamChiTietViewModels.Add(new SanPhamChiTietViewModel
                        {
                            IdSanPhamChiTiet = sanPhamCT.IdSanPhamChiTiet,
                            Quantity = sanPhamCT.SoLuong,
                            Price = sanPhamCT.Gia,
                            ProductName = sanPham?.TenSanPham,
                            MauSac = mauSacTenList,
                            KichCo = kichCoTenList,
                            HinhAnhs = hinhAnhs
                        });
                    });

                    await Task.WhenAll(tasks);
                }

                return PartialView("~/Areas/Admin/Views/BanHangTQ/ListSanPhamChiTiet.cshtml", (sanPhamChiTietViewModels, idHoaDon));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSanPhamChiTietList: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách sản phẩm chi tiết." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSanPhamChiTiet(Guid IdSanPhamChiTiet, Guid IdHoaDon)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sanPhamChiTiet = await _sanPhamCTService.GetSanPhamChiTietById(IdSanPhamChiTiet);
                if (sanPhamChiTiet == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                int requestedQuantity = 1;
                if (requestedQuantity <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0." });
                }

                if (requestedQuantity > sanPhamChiTiet.SoLuong)
                {
                    return Json(new { success = false, message = "Số lượng yêu cầu vượt quá số lượng có sẵn." });
                }

                var hoaDonChiTiet = new HoaDonChiTiet
                {
                    IdHoaDonChiTiet = Guid.NewGuid(),
                    IdHoaDon = IdHoaDon,
                    IdSanPhamChiTiet = sanPhamChiTiet.IdSanPhamChiTiet,
                    DonGia = sanPhamChiTiet.Gia,
                    SoLuong = requestedQuantity,
                    TongTien = sanPhamChiTiet.Gia * requestedQuantity,
                    KichHoat = 1
                };

                await _hoaDonChiTietService.AddAsync(new List<HoaDonChiTiet> { hoaDonChiTiet });

                sanPhamChiTiet.SoLuong -= requestedQuantity;

                await _sanPhamCTService.Update(sanPhamChiTiet);

                var updatedProductList = await GetSanPhamChiTietList(IdHoaDon);
                return Json(new { success = true, message = "Thêm sản phẩm thành công.", html = updatedProductList });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateSanPhamChiTiet: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm sản phẩm vào hóa đơn." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInvoiceDetails([FromBody] HoaDonChiTietViewModel model)
        {
            if (model == null || model.IdHoaDon == Guid.Empty)
            {
                return Json(new { success = false, message = "Dữ liệu hóa đơn không hợp lệ." });
            }


            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, errors });
            }

            var hoaDonChiTietList = await _hoaDonChiTietService.GetByIdHoaDonAsync(model.IdHoaDon);

            foreach (var item in hoaDonChiTietList)
            {
                var updatedItem = model.SanPhamChiTiets.FirstOrDefault(x => x.IdSanPhamChiTiet == item.IdSanPhamChiTiet);
                if (updatedItem != null)
                {
                    var originalQuantity = item.SoLuong;
                    item.SoLuong = updatedItem.Quantity;

                    var sanPhamCT = await _sanPhamCTService.GetSanPhamChiTietById(item.IdSanPhamChiTiet);
                    if (sanPhamCT == null)
                    {
                        return Json(new { success = false, message = "Chi tiết sản phẩm không tồn tại." });
                    }

                    double quantityChange = item.SoLuong - originalQuantity;
                    double availableStock = sanPhamCT.SoLuong + originalQuantity;

                    if (quantityChange > 0)
                    {
                        if (item.SoLuong > availableStock)
                        {
                            return Json(new { success = false, message = "Số lượng yêu cầu vượt quá số lượng có sẵn." });
                        }
                        sanPhamCT.SoLuong -= quantityChange; // Deduct from stock
                    }
                    else if (quantityChange < 0) // Decreasing quantity
                    {
                        sanPhamCT.SoLuong += Math.Abs(quantityChange); // Add back to stock
                    }

                    // Calculate total amount
                    item.TongTien = item.SoLuong * sanPhamCT.Gia;

                    // Update product stock
                    await _sanPhamCTService.Update(sanPhamCT);
                }
            }

            // Update the invoice line items
            await _hoaDonChiTietService.UpdateAsync(hoaDonChiTietList);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanThanhToan(Guid idHoaDon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var NVIdString = HttpContext.Session.GetString("IdNhanVien");
            if (string.IsNullOrEmpty(NVIdString) || !Guid.TryParse(NVIdString, out Guid NVID))
            {
                return Unauthorized(new { message = "Nhân viên không tồn tại trong phiên làm việc." });
            }

            var hoaDonChiTietList = await _hoaDonChiTietService.GetByIdHoaDonAsync(idHoaDon);
            if (hoaDonChiTietList == null)
            {
                return NotFound(new { message = "Không tìm thấy chi tiết hóa đơn." });
            }

            double tongTienHang = 0;
            foreach (var hoaDonChiTiet in hoaDonChiTietList)
            {
                var sanPhamCT = await _sanPhamCTService.GetSanPhamChiTietById(hoaDonChiTiet.IdSanPhamChiTiet);
                if (sanPhamCT != null)
                {
                    double thanhTien = hoaDonChiTiet.SoLuong * hoaDonChiTiet.DonGia;
                    tongTienHang += thanhTien;
                }
            }

            var hoaDon = await _hoaDonService.GetByIdAsync(idHoaDon);
            if (hoaDon == null)
            {
                return NotFound(new { message = "Hóa đơn không tồn tại." });
            }

            if (hoaDon.IdKhachHang == null || hoaDon.IdKhachHang == Guid.Empty)
            {
                var newCustomer = new KhachHang
                {
                    IdKhachHang = Guid.NewGuid(),
                    HoTen = "Khách Lẻ",
                    AuthProvider = "1",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    NguoiTao = "Nhân Viên",
                    NguoiCapNhat = "Nhân Viên",
                    KichHoat = 1,
                };

                try
                {
                    await _khachHangService.AddKhachHang(newCustomer);
                    hoaDon.IdKhachHang = newCustomer.IdKhachHang;
                    hoaDon.NguoiNhan = newCustomer.HoTen;
                }
                catch (Exception ex)
                {
                    // Log lỗi khi tạo khách hàng
                    return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo khách hàng." });
                }
            }

            hoaDon.TienGiam = 0;
            hoaDon.TongTienDonHang = tongTienHang;
            hoaDon.TongTienHoaDon = tongTienHang;
            hoaDon.TrangThai = "Hoàn Thành";

            try
            {
                await _hoaDonService.UpdateAsync(hoaDon);

                var lichSu = new LichSuHoaDon
                {
                    IdLichSuHoaDon = Guid.NewGuid(),
                    ThaoTac = hoaDon.TrangThai,
                    NgayTao = DateTime.Now,
                    NguoiThaoTac = "Nhân Viên",
                    TrangThai = "1",
                    IdHoaDon = hoaDon.IdHoaDon,
                };

                await _lichSuHoaDonService.AddAsync(lichSu);

                // Tạo HTML từ mẫu hóa đơn
                var htmlContent = await GenerateInvoiceHtmlFromTemplate(hoaDon, hoaDonChiTietList);

                // Tạo PDF từ HTML
                var pdfBytes = GeneratePdfFromHtml(htmlContent);

                // Lưu file PDF vào thư mục tạm
                var tempFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempFiles");
                if (!Directory.Exists(tempFilePath))
                {
                    Directory.CreateDirectory(tempFilePath);
                }

                var fileName = $"HoaDon_{hoaDon.IdHoaDon}.pdf";
                var fullPath = Path.Combine(tempFilePath, fileName);
                await System.IO.File.WriteAllBytesAsync(fullPath, pdfBytes);
                var fileUrl = $"/TempFiles/{fileName}";

                TempData["SuccessMessage"] = "Thanh toán thành công! Bạn có muốn in hóa đơn không?";
                TempData["FileUrl"] = fileUrl; // Lưu file URL để sử dụng trong view
                return RedirectToAction("ShowInvoiceMessage");
               
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi trong quá trình xử lý thanh toán
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xác nhận thanh toán." });
            }
        }
        public IActionResult ShowInvoiceMessage()
        {
            return View();
        }

        private async Task<string> GenerateInvoiceHtmlFromTemplate(HoaDon hoaDon, List<HoaDonChiTiet> hoaDonChiTietList)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Templates", "InvoiceTemplate.html");

            if (!System.IO.File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found: {templatePath}");
            }

            string htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);
            string chiTietHoaDonHtml = "";

            foreach (var chiTiet in hoaDonChiTietList)
            {
                var sanPhamCT = await _sanPhamCTService.GetSanPhamChiTietById(chiTiet.IdSanPhamChiTiet);
                if (sanPhamCT != null)
                {
                    chiTietHoaDonHtml += $@"
            <tr>
                <td>{sanPhamCT.IdSanPham}</td>
                <td>{chiTiet.SoLuong}</td>
                <td>{chiTiet.DonGia:C}</td>
                <td>{chiTiet.SoLuong * chiTiet.DonGia:C}</td>
            </tr>";
                }
            }

            string finalHtml = htmlTemplate
                .Replace("{{MaDon}}", hoaDon.MaDon.ToString())
                .Replace("{{NgayTao}}", hoaDon.NgayTao.ToString("dd/MM/yyyy"))
                .Replace("{{ChiTietHoaDon}}", chiTietHoaDonHtml)
                .Replace("{{TongTien}}", hoaDon.TongTienHoaDon.ToString("C"));

            return finalHtml;
        }

        private byte[] GeneratePdfFromHtml(string htmlContent)
        {
            var converter = new SynchronizedConverter(new PdfTools());

            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    DPI = 300
                }
            };

            doc.Objects.Add(new ObjectSettings
            {
                HtmlContent = htmlContent,
                WebSettings = new WebSettings
                {
                    DefaultEncoding = "utf-8"
                }
            });

            return converter.Convert(doc);
        }
        //VNPay
        [HttpPost]
        public async Task<IActionResult> ThanhToanVNPay(Guid idHoaDon)
        {       
            var hoaDonChiTietList = await  _hoaDonChiTietService.GetByIdHoaDonAsync(idHoaDon);
            if (hoaDonChiTietList == null)
            {
                return NotFound(new { message = "Không tìm thấy chi tiết hóa đơn." });
            }

            double tongTienHang = 0;
            foreach (var hoaDonChiTiet in hoaDonChiTietList)
            {
                var sanPhamCT = _sanPhamCTService.GetSanPhamChiTietById(hoaDonChiTiet.IdSanPhamChiTiet);
                if (sanPhamCT != null)
                {
                    double thanhTien = hoaDonChiTiet.SoLuong * hoaDonChiTiet.DonGia;
                    tongTienHang += thanhTien;
                }
            }
            // Lấy thông tin hóa đơn
            var hoaDon = _hoaDonService.GetByIdAsync(idHoaDon).Result;
            if (hoaDon == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn." });
            }

            // Tạo tham số yêu cầu thanh toán
            var vnPay = new PaymentInformationModel()
            {
                Amount = tongTienHang,
                CreatDate = DateTime.Now,
                Description = $"Thanh Toán VnPay",
                FullName = "Nhân Viên",
                OrderId = new Random().Next(100, 10000)
            };

            return Redirect(_vnPayServie.CreatePaymentUrl(vnPay, HttpContext)); 

        
        }
        public IActionResult ThanhToanLoi()
        {
            return View();
        }
        public IActionResult ThanhToanThanhCong()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ReturnVNPay()
        {
            var respones = _vnPayServie.PaymentExecute(Request.Query);
            if (respones == null || respones.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi Thanh Toán: {respones.VnPayResponseCode }";
                return RedirectToAction("ThanhToanLoi");
            }
            //
            TempData["Message"] = $"Thanh toán VnPay Thành công";
            return RedirectToAction("ThanhToanThanhCong");


        }

    }

}