using APPMVC.IService;
using Microsoft.AspNetCore.Mvc;
using AppData.Model;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using AppData.ViewModel;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace APPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly ISanPhamChiTietService _sanPhamCTService;
        private readonly ISanPhamService _sanPhamService;
        private readonly IChatLieuService _chatLieuService;
        private readonly IDeGiayService _deGiayService;
        private readonly IDanhMucService _danhMucService;
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IKieuDangService _kieuDangService;
        private readonly IMauSacService _mauSacService;
        private readonly IKichCoService _kichCoService;
        private readonly ISanPhamChiTietMauSacService _sanPhamChiTietMauSacService;
        private readonly ISanPhamChiTietKichCoService _sanPhamChiTietKichCoService;
        private readonly IHinhAnhService _hinhAnhService;

        public SanPhamController(
            ISanPhamService sanPhamService,
            ISanPhamChiTietService sanPhamChiTietService,
            IDeGiayService deGiayService,
            IDanhMucService danhMucService,
            IThuongHieuService thuongHieuService,
            IChatLieuService chatLieuService,
            IKieuDangService kieuDangService,
            IMauSacService mauSacService,
            IKichCoService kichCoService,
            ISanPhamChiTietMauSacService sanPhamChiTietMauSacService,
            ISanPhamChiTietKichCoService sanPhamChiTietKichCoService,
            IHinhAnhService hinhAnhService)
        {
            _sanPhamCTService = sanPhamChiTietService;
            _sanPhamService = sanPhamService;
            _deGiayService = deGiayService;
            _danhMucService = danhMucService;
            _thuongHieuService = thuongHieuService;
            _chatLieuService = chatLieuService;
            _kieuDangService = kieuDangService;
            _mauSacService = mauSacService;
            _kichCoService = kichCoService;
            _sanPhamChiTietMauSacService = sanPhamChiTietMauSacService;
            _sanPhamChiTietKichCoService = sanPhamChiTietKichCoService;
            _hinhAnhService = hinhAnhService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? name, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 5;

            var sanPhams = await _sanPhamService.GetSanPhams(name);
            var sanPhamViewModels = new List<SanPhamViewModel>();

            foreach (var sanPham in sanPhams)
            {
                var sanPhamChiTietList = await _sanPhamCTService.GetSanPhamChiTietBySanPhamId(sanPham.IdSanPham);
                double totalQuantity = sanPhamChiTietList.Sum(s => s.SoLuong );

                sanPhamViewModels.Add(new SanPhamViewModel
                {
                    SanPham = sanPham,
                    TotalQuantity = totalQuantity,
                    SanPhamChiTiet = sanPhamChiTietList
                });
            }

            var sortedSanPhamViewModels = sanPhamViewModels.OrderByDescending(s => s.SanPham.NgayTao).ToList();

            var pagedSanPhamViewModels = sortedSanPhamViewModels.ToPagedList(page, pageSize);
            return View(pagedSanPhamViewModels);
        }

        public async Task<IActionResult> Create()
        {
            await LoadViewBags();
            var sanPham = new SanPham
            {
                IdSanPham = Guid.NewGuid(),
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now,
                NguoiTao = "Admin",
                NguoiCapNhat = "Admin",
                KichHoat = 1
            };

            var viewModel = new SanPhamViewModel
            {
                SanPham = sanPham,
                KichCoOptions = await GetKichCoOptions() ?? new List<SelectListItem>(),
                MauSacOptions = await GetMauSacOptions() ?? new List<SelectListItem>()
            };

            return View(viewModel);
        }

        private async Task<List<SelectListItem>> GetKichCoOptions()
        {
            var kichCos = await _kichCoService.GetKichCo(null) ?? new List<KichCo>();
            return kichCos.Select(k => new SelectListItem
            {
                Value = k.IdKichCo.ToString(),
                Text = k.TenKichCo
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetMauSacOptions()
        {
            var mauSacs = await _mauSacService.GetMauSac(null) ?? new List<MauSac>();
            return mauSacs.Select(m => new SelectListItem
            {
                Value = m.IdMauSac.ToString(),
                Text = m.TenMauSac
            }).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadViewBags();
                return View(viewModel);
            }

            try
            {

                var chatLieuTask = _chatLieuService.GetChatLieuById(viewModel.IdChatLieu);
                var thuongHieuTask = _thuongHieuService.GetThuongHieuById(viewModel.IdThuongHieu);
                var danhMucTask = _danhMucService.GetDanhMucById(viewModel.IdDanhMuc);
                var deGiayTask = _deGiayService.GetDeGiayById(viewModel.IdDeGiay);
                var kieuDangTask = _kieuDangService.GetKieuDangById(viewModel.IdKieuDang);

                await Task.WhenAll(chatLieuTask, thuongHieuTask, danhMucTask, deGiayTask, kieuDangTask);

                viewModel.SanPham.ChatLieu = await chatLieuTask;
                viewModel.SanPham.ThuongHieu = await thuongHieuTask;
                viewModel.SanPham.DanhMuc = await danhMucTask;
                viewModel.SanPham.DeGiay = await deGiayTask;
                viewModel.SanPham.KieuDang = await kieuDangTask;

                viewModel.SanPham.IdChatLieu = viewModel.SanPham.ChatLieu?.IdChatLieu ?? Guid.Empty;
                viewModel.SanPham.IdThuongHieu = viewModel.SanPham.ThuongHieu?.IdThuongHieu ?? Guid.Empty;
                viewModel.SanPham.IdDanhMuc = viewModel.SanPham.DanhMuc?.IdDanhMuc ?? Guid.Empty;
                viewModel.SanPham.IdDeGiay = viewModel.SanPham.DeGiay?.IdDeGiay ?? Guid.Empty;
                viewModel.SanPham.IdKieuDang = viewModel.SanPham.KieuDang?.IdKieuDang ?? Guid.Empty;


                await _sanPhamService.Create(viewModel.SanPham);
                var idSanPham = viewModel.SanPham.IdSanPham;

                if (viewModel.Combinations == null || !viewModel.Combinations.Any())
                {
                    TempData["Error"] = "Không có tổ hợp nào được chọn.";
                    return View(viewModel);
                }


                // Sử dụng HashSet để theo dõi các cặp IdKichCo và IdMauSac đã được tạo
                var createdPairs = new HashSet<(string KichCoId, string MauSacId)>();

                // Lấy số lượng cặp KichCo và MauSac
                int totalKichCo = viewModel.SelectedKichCoIds.Count;
                int totalMauSac = viewModel.SelectedMauSacIds.Count;
                int totalCombinations = viewModel.Combinations.Count;

                // Lặp qua từng KichCo đã chọn
                for (int i = 0; i < totalKichCo; i++)
                {
                    string kichCoId = viewModel.SelectedKichCoIds[i];

                    // Lặp qua từng MauSac đã chọn
                    for (int j = 0; j < totalMauSac; j++)
                    {
                        string mauSacId = viewModel.SelectedMauSacIds[j];

                        // Lấy tổ hợp tương ứng (có thể lặp lại nếu không đủ tổ hợp)
                        var combination = viewModel.Combinations[(i + j) % totalCombinations];

                        // Kiểm tra xem cặp (KichCoId, MauSacId) đã được tạo chưa
                        var pair = (KichCoId: kichCoId, MauSacId: mauSacId);
                        if (!createdPairs.Contains(pair))
                        {
                            // Tạo một SanPhamChiTiet mới cho từng tổ hợp, KichCo và MauSac
                            var sanPhamChiTiet = new SanPhamChiTiet
                            {
                                IdSanPhamChiTiet = Guid.NewGuid(),
                                IdSanPham = idSanPham,
                                Gia = combination.Gia, // Sử dụng giá từ tổ hợp
                                SoLuong = combination.SoLuong, // Sử dụng số lượng từ tổ hợp
                                XuatXu = combination.XuatXu, // Sử dụng xuất xứ từ tổ hợp
                                GioiTinh = "Nam",
                                NgayTao = DateTime.Now,
                                NgayCapNhat = DateTime.Now,
                                NguoiTao = "Admin",
                                NguoiCapNhat = "Admin",
                                KichHoat = 1
                            };

                            // Tạo SanPhamChiTiet
                            await _sanPhamCTService.Create(sanPhamChiTiet);

                            // Tạo mối quan hệ giữa SanPhamChiTiet và KichCo
                            await _sanPhamChiTietKichCoService.Create(new SanPhamChiTietKichCo
                            {
                                IdSanPhamChiTiet = sanPhamChiTiet.IdSanPhamChiTiet,
                                IdKichCo = Guid.Parse(kichCoId)
                            });

                            // Tạo mối quan hệ giữa SanPhamChiTiet và MauSac
                            await _sanPhamChiTietMauSacService.Create(new SanPhamChiTietMauSac
                            {
                                IdSanPhamChiTiet = sanPhamChiTiet.IdSanPhamChiTiet,
                                IdMauSac = Guid.Parse(mauSacId)
                            });

                            // Xử lý hình ảnh
                            if (combination.Files != null && combination.Files.Count > 0)
                            {
                                foreach (var file in combination.Files)
                                {
                                    if (file.Length > 0)
                                    {
                                        var imageData = await ConvertFileToByteArray(file);
                                        var hinhAnh = new HinhAnh
                                        {
                                            IdHinhAnh = Guid.NewGuid(),
                                            LoaiFileHinhAnh = file.ContentType,
                                            DataHinhAnh = imageData,
                                            TrangThai = 1,
                                            IdSanPhamChiTiet = sanPhamChiTiet.IdSanPhamChiTiet
                                        };

                                        await _hinhAnhService.UploadAsync(hinhAnh);
                                    }
                                }
                            }

                            // Thêm cặp (KichCoId, MauSacId) vào HashSet
                            createdPairs.Add(pair);
                        }
                    }
                }

                TempData["Success"] = "Thêm mới thành công";
                return RedirectToAction("GetAll");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["Error"] = "Thêm mới thất bại: " + ex.Message;
            }

            await LoadViewBags();
            return View(viewModel);
        }

        private async Task<byte[]> ConvertFileToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                Console.WriteLine("ID cannot be empty.");
                return BadRequest("ID cannot be empty.");
            }

            Console.WriteLine($"Attempting to retrieve product with ID {id}.");

            var sanPham = await _sanPhamService.GetSanPhamById(id);
            if (sanPham == null)
            {
                Console.WriteLine($"Product with ID {id} not found.");
                return NotFound();
            }

            Console.WriteLine($"Product {sanPham.TenSanPham} retrieved successfully.");

            var sanPhamChiTietList = await _sanPhamCTService.GetSanPhamChiTietBySanPhamId(id);

            if (sanPhamChiTietList == null || !sanPhamChiTietList.Any())
            {
                Console.WriteLine($"No product details found for product with ID {id}.");
            }

            var thuongHieu = await _thuongHieuService.GetThuongHieuById(sanPham.IdThuongHieu);
            var danhMuc = await _danhMucService.GetDanhMucById(sanPham.IdDanhMuc);
            var chatLieu = await _chatLieuService.GetChatLieuById(sanPham.IdChatLieu);
            var kieuDang = await _kieuDangService.GetKieuDangById(sanPham.IdKieuDang);
            var deGiay = await _deGiayService.GetDeGiayById(sanPham.IdDeGiay);
       
            var sanPhamChiTietViewModels = new List<SanPhamChiTietItemViewModel>();

            foreach (var chiTiet in sanPhamChiTietList)
            {
                var mauSacList = await _sanPhamChiTietMauSacService.GetMauSacIdsBySanPhamChiTietId(chiTiet.IdSanPhamChiTiet);
                var mauSacTenList = mauSacList.Select(ms => ms.TenMauSac).ToList(); 

                var kichCoList = await _sanPhamChiTietKichCoService.GetKichCoIdsBySanPhamChiTietId(chiTiet.IdSanPhamChiTiet);
                var kichCoTenList = kichCoList.Select(kc => kc.TenKichCo).ToList();

                var hinhAnhs = await _hinhAnhService.GetHinhAnhsBySanPhamChiTietId(chiTiet.IdSanPhamChiTiet);
          
                sanPhamChiTietViewModels.Add(new SanPhamChiTietItemViewModel
                {
                    IdSanPhamChiTiet = chiTiet.IdSanPhamChiTiet,
                    HinhAnhs = hinhAnhs, 
                    MauSac = mauSacTenList, 
                    KichCo = kichCoTenList,
                    Gia = chiTiet.Gia,
                    SoLuong = chiTiet.SoLuong,
                    XuatXu = chiTiet.XuatXu,
                    Chon = false
                });
            }

            var viewModel = new SanPhamChiTietViewModel
            {
                IdSanPham = sanPham.IdSanPham,
                TenSanPham = sanPham.TenSanPham,
                ThuongHieu = thuongHieu?.TenThuongHieu,
                DanhMuc = danhMuc?.TenDanhMuc,
                ChatLieu = chatLieu?.TenChatLieu,
                KieuDang = kieuDang?.TenKieuDang,
                DeGiay = deGiay?.TenDeGiay,
                SanPhamChiTietList = sanPhamChiTietViewModels,
                SelectedKichCoIds = sanPhamChiTietViewModels.SelectMany(x => x.KichCo).Distinct().ToList(),
                SelectedMauSacIds = sanPhamChiTietViewModels.SelectMany(x => x.MauSac).Distinct().ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SanPhamChiTietViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var selectedProducts = viewModel.SanPhamChiTietList.Where(chiTiet => chiTiet.Chon).ToList();
                if (!selectedProducts.Any())
                {
                    ModelState.AddModelError("", "Bạn cần chọn ít nhất một sản phẩm để cập nhật.");
                    return View(viewModel);
                }

                foreach (var chiTiet in selectedProducts)
                {
                    var sanPhamChiTiet = await _sanPhamCTService.GetSanPhamChiTietById(chiTiet.IdSanPhamChiTiet);

                    if (sanPhamChiTiet != null)
                    {
                        // Cập nhật các thuộc tính chỉ khi có sự thay đổi
                        if (sanPhamChiTiet.Gia != chiTiet.Gia)
                        {
                            sanPhamChiTiet.Gia = chiTiet.Gia;
                        }

                        if (sanPhamChiTiet.SoLuong != chiTiet.SoLuong)
                        {
                            sanPhamChiTiet.SoLuong = chiTiet.SoLuong;
                        }

                        if (sanPhamChiTiet.XuatXu != chiTiet.XuatXu)
                        {
                            sanPhamChiTiet.XuatXu = chiTiet.XuatXu;
                        }

                        // Lưu thay đổi
                        await _sanPhamCTService.Update(sanPhamChiTiet);
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Sản phẩm chi tiết với ID {chiTiet.IdSanPhamChiTiet} không tồn tại.");
                    }
                }

                TempData["Success"] = "Cập nhật thành công";
                return RedirectToAction("Edit", "SanPham", new { area = "Admin", id = viewModel.IdSanPham });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Cập nhật thất bại: " + ex.Message;
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSP(Guid id)
        {
            // Kiểm tra nếu ID sản phẩm rỗng
            if (id == Guid.Empty)
            {
                Console.WriteLine("ID cannot be empty.");
                return BadRequest("ID cannot be empty.");
            }

            var sanPham = await _sanPhamService.GetSanPhamById(id);
            if (sanPham == null)
            {
                Console.WriteLine($"Product with ID {id} not found.");
                return NotFound();
            }

            await LoadViewBags();

            var viewModel = new SanPhamChiTietViewModel
            {
                IdSanPham = sanPham.IdSanPham,
                TenSanPham = sanPham.TenSanPham,
                ThuongHieuId = sanPham.IdThuongHieu, 
                DanhMucId = sanPham.IdDanhMuc,       
                ChatLieuId = sanPham.IdChatLieu,     
                KieuDangId = sanPham.IdKieuDang,     
                DeGiayId = sanPham.IdDeGiay          
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSP(SanPhamChiTietViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadViewBags();
                return View(viewModel);
            }

            try
            {
                var sanPham = await _sanPhamService.GetSanPhamById(viewModel.IdSanPham);
                if (sanPham == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại.";
                    await LoadViewBags();
                    return View(viewModel);
                }

                sanPham.TenSanPham = viewModel.TenSanPham;
                sanPham.IdThuongHieu = viewModel.ThuongHieuId;
                sanPham.IdChatLieu = viewModel.ChatLieuId;
                sanPham.IdDanhMuc = viewModel.DanhMucId;
                sanPham.IdDeGiay = viewModel.DeGiayId;
                sanPham.IdKieuDang = viewModel.KieuDangId;

                await _sanPhamService.Update(sanPham);

                TempData["Success"] = "Cập nhật thành công";
                return RedirectToAction("Edit", "SanPham", new { area = "Admin", id = viewModel.IdSanPham });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Cập nhật thất bại: " + ex.Message;
            }

            await LoadViewBags();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSPCT(Guid Id)
        {
            // Xử lý GET request
            var sanPhamChiTiet = await _sanPhamCTService.GetSanPhamChiTietById(Id);
            if (sanPhamChiTiet == null)
            {
                Console.WriteLine($"No product detail found for product with ID {Id}.");
                return NotFound("Product detail not found.");
            }

            var mauSacList = await _sanPhamChiTietMauSacService.GetMauSacIdsBySanPhamChiTietId(sanPhamChiTiet.IdSanPhamChiTiet);
            var mauSacTenList = mauSacList.Select(ms => ms.TenMauSac).ToList();

            var kichCoList = await _sanPhamChiTietKichCoService.GetKichCoIdsBySanPhamChiTietId(sanPhamChiTiet.IdSanPhamChiTiet);
            var kichCoTenList = kichCoList.Select(kc => kc.TenKichCo).ToList();

            var hinhAnhs = await _hinhAnhService.GetHinhAnhsBySanPhamChiTietId(sanPhamChiTiet.IdSanPhamChiTiet);

            var model = new SanPhamChiTietItemViewModel
            {
                IdSanPhamChiTiet = sanPhamChiTiet.IdSanPhamChiTiet,
                HinhAnhs = hinhAnhs,
                MauSac = mauSacTenList,
                KichCo = kichCoTenList,
                Gia = sanPhamChiTiet.Gia,
                SoLuong = sanPhamChiTiet.SoLuong,
                XuatXu = sanPhamChiTiet.XuatXu,

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSPCT(SanPhamChiTietItemViewModel viewModel)
        {
            // Xử lý POST request
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(viewModel);
            }

            try
            {
                // Lấy chi tiết sản phẩm hiện tại từ cơ sở dữ liệu
                var sanPhamChiTiet = await _sanPhamCTService.GetSanPhamChiTietById(viewModel.IdSanPhamChiTiet);
                if (sanPhamChiTiet == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("Edit", "SanPham", new { area = "Admin", id = sanPhamChiTiet.IdSanPham });
                }

                // Cập nhật các thuộc tính của sản phẩm
                sanPhamChiTiet.Gia = viewModel.Gia;
                sanPhamChiTiet.SoLuong = viewModel.SoLuong;
                sanPhamChiTiet.XuatXu = viewModel.XuatXu;

                var existingImages = await _hinhAnhService.GetHinhAnhsBySanPhamChiTietId(sanPhamChiTiet.IdSanPhamChiTiet);
                foreach (var existingImage in existingImages)
                {
                    await _hinhAnhService.DeleteAsync(existingImage.IdHinhAnh);
                }

                // Xử lý hình ảnh mới
                if (viewModel.Files != null && viewModel.Files.Count > 0)
                {
                    foreach (var file in viewModel.Files)
                    {
                        if (file.Length > 0)
                        {
                            var imageData = await ConvertFileToByteArray(file);
                            var hinhAnh = new HinhAnh
                            {
                                IdHinhAnh = Guid.NewGuid(),
                                LoaiFileHinhAnh = file.ContentType,
                                DataHinhAnh = imageData,
                                TrangThai = 1,
                                IdSanPhamChiTiet = sanPhamChiTiet.IdSanPhamChiTiet
                            };

                            await _hinhAnhService.UploadAsync(hinhAnh);
                        }
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _sanPhamCTService.Update(sanPhamChiTiet);

                // Thông báo thành công
                TempData["Success"] = "Cập nhật thành công";
                return RedirectToAction("GetAll");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Cập nhật thất bại: " + ex.Message;
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSanPhamChiTiet(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = "ID cannot be empty.";
                return RedirectToAction("Index"); 
            }

            try
            {
                await _sanPhamCTService.Delete(id);
                TempData["SuccessMessage"] = "Product detail deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete product detail: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        private async Task LoadViewBags()
        {
            var listChatLieu = await _chatLieuService.GetChatLieu(null);
            var listDeGiay = await _deGiayService.GetDeGiay(null);
            var listDanhMuc = await _danhMucService.GetDanhMuc(null);
            var listThuongHieu = await _thuongHieuService.GetThuongHieu(null);
            var listKieuDang = await _kieuDangService.GetKieuDang(null);
            var listMauSac = await _mauSacService.GetMauSac(null);
            var listKichCo = await _kichCoService.GetKichCo(null);

            ViewBag.IdChatLieu = new SelectList(listChatLieu, "IdChatLieu", "TenChatLieu");
            ViewBag.IdDeGiay = new SelectList(listDeGiay, "IdDeGiay", "TenDeGiay");
            ViewBag.IdDanhMuc = new SelectList(listDanhMuc, "IdDanhMuc", "TenDanhMuc");
            ViewBag.IdThuongHieu = new SelectList(listThuongHieu, "IdThuongHieu", "TenThuongHieu");
            ViewBag.IdKieuDang = new SelectList(listKieuDang, "IdKieuDang", "TenKieuDang");
            ViewBag.IdKichCo = new SelectList(listKichCo, "IdKichCo", "TenKichCo");
            ViewBag.IdMauSac = new SelectList(listMauSac, "IdMauSac", "TenMauSac");
        }
    }
}