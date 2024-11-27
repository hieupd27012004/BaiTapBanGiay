using AppData.Model;
using AppData.ViewModel;
using APPMVC.IService;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using X.PagedList;

namespace APPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanVienController : Controller
    {
        private readonly INhanVienService _service;
        private readonly IChucVuService chucVuService;
        private readonly IMemoryCache _memoryCache;
        public NhanVienController(INhanVienService service, IChucVuService _cvser, IMemoryCache memoryCache)
        {
            _service = service;
            chucVuService = _cvser;
            _memoryCache = memoryCache;
        }
        // GET: NhanVienController
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            page = page < 1 ? 1 : page; 
            int pageSize = 5;

            var nhanVienList = await _service.GetAllNhaVien();

            var chucVuList = await chucVuService.GetAllChucVu(); 

            var nhanVienViewModels = nhanVienList.Select(nv => new NhanVienViewModel
            {
                IdNhanVien = nv.IdNhanVien,
                TenNhanVien = nv.TenNhanVien,
                SoDienThoai = nv.SoDienThoai,
                Email = nv.Email,
                DiaChi = nv.DiaChi,
                TenChucVu = chucVuList.FirstOrDefault(c => c.IdChucVu == nv.IdchucVu)?.TenChucVu,
                AnhNhanVien = nv.AnhNhanVien,
                KichHoat = nv.KichHoat,
                TrangThai = nv.TrangThai,
                NgayTao = nv.NgayTao,
                NgayCapNhat = nv.NgayCapNhat
            }).ToList();

            // Thực hiện phân trang
            var pagedNhanViens = nhanVienViewModels.ToPagedList(page, pageSize);

            return View(pagedNhanViens);
        }
        // GET: NhanVienController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NhanVienController/Create
        public async Task<IActionResult> Create()
        {
            var chucVuList = await chucVuService.GetAllChucVu();
            ViewBag.ChucVu = chucVuList;
            NhanVien nv = new NhanVien()
            {
                IdNhanVien = Guid.NewGuid(),
                AuthProvider = "Trống",
                NgayCapNhat = DateTime.Now,
                NgayTao = DateTime.Now,
                NguoiCapNhat = "admin",
                NguoiTao = "admin",

            };
            return View(nv);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhanVien nhanVien, IFormFile imgFile)
        {
            // Kiểm tra xem có file ảnh được tải lên hay không
            if (imgFile != null && imgFile.Length > 0)
            {
                // Kiểm tra loại file (chỉ chấp nhận các định dạng ảnh)
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(imgFile.FileName).ToLowerInvariant();

                // Kiểm tra phần mở rộng có hợp lệ không
                if (!permittedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("", "Chỉ chấp nhận các file ảnh với định dạng .jpg, .jpeg, .png, .gif.");
                    return View(nhanVien);
                }

                // Kiểm tra kích thước file (giới hạn 5MB)
                if (imgFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "File ảnh phải nhỏ hơn 5MB.");
                    return View(nhanVien);
                }

                // Tạo đường dẫn đến thư mục lưu trữ hình ảnh
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "imgNV", imgFile.FileName);

                // Tạo file stream để lưu file hình ảnh
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    // Sao chép dữ liệu từ imgFile vào stream
                    await imgFile.CopyToAsync(stream);
                }

                // Gán tên file vào thuộc tính AnhNhanVien của đối tượng NhanVien
                nhanVien.AnhNhanVien = imgFile.FileName;
            }
            else
            {
                // Nếu không có file nào được tải lên, thêm lỗi vào ModelState
                ModelState.AddModelError("", "Vui lòng tải lên một file ảnh hợp lệ.");
                return View(nhanVien);
            }

            // Kiểm tra tính hợp lệ của ModelState trước khi lưu dữ liệu vào cơ sở dữ liệu
            if (!ModelState.IsValid)
            {
                // Trả về view hiện tại với model để hiển thị lại thông tin và thông báo lỗi
                return View(nhanVien);
            }

            // Nếu tất cả hợp lệ, lưu thông tin nhân viên
            await _service.CreateNV(nhanVien);
            return RedirectToAction("Index");
        }


            public async Task<IActionResult> Edit(Guid id)
            {
                var nhanVien = await _service.GetIdNhanVien(id);
                if (nhanVien == null)
                {
                    return NotFound();
                }

                List<ChucVu> chucVus = await chucVuService.GetAllChucVu();

                var viewModel = new NhanVienViewModel
                {
                    NhanVien = nhanVien,
                    ChucVuList = chucVus
                };

                return View(viewModel);
            }

        [HttpPost]
        public async Task<IActionResult> Edit(NhanVienViewModel model, IFormFile? imgFile)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin nhân viên cũ để xóa ảnh cũ
                var existingNV = await _service.GetIdNhanVien(model.NhanVien.IdNhanVien);

                if (existingNV == null)
                {
                    ModelState.AddModelError("", "Nhân viên không tồn tại.");
                    model.ChucVuList = await chucVuService.GetAllChucVu();
                    return View(model);
                }

                // Kiểm tra xem có file ảnh mới được tải lên không
                if (imgFile != null && imgFile.Length > 0)
                {
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var ext = Path.GetExtension(imgFile.FileName).ToLowerInvariant();

                    if (!permittedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận các file ảnh với định dạng .jpg, .jpeg, .png, .gif.");
                        model.ChucVuList = await chucVuService.GetAllChucVu();
                        return View(model);
                    }

                    if (imgFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File ảnh phải nhỏ hơn 5MB.");
                        model.ChucVuList = await chucVuService.GetAllChucVu();
                        return View(model);
                    }

                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "imgNV");

                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(existingNV.AnhNhanVien))
                    {
                        string oldImagePath = Path.Combine(uploadFolder, existingNV.AnhNhanVien);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Tạo đường dẫn đến file ảnh mới
                    string newImagePath = Path.Combine(uploadFolder, imgFile.FileName);

                    // Tạo file stream để lưu file hình ảnh
                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        await imgFile.CopyToAsync(stream);
                    }

                    // Gán tên file vào thuộc tính AnhNhanVien của đối tượng NhanVien
                    model.NhanVien.AnhNhanVien = imgFile.FileName;
                }
                else
                {
                    // Nếu không có file ảnh mới, giữ nguyên giá trị cũ
                    model.NhanVien.AnhNhanVien = existingNV.AnhNhanVien;
                }

                // Cập nhật thông tin nhân viên
                try
                {
                    await _service.UpdateNV(model.NhanVien);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin nhân viên: " + ex.Message);
                    model.ChucVuList = await chucVuService.GetAllChucVu();
                    return View(model);
                }
            }
            else
            {
                // Repopulate the ChucVuList if the model state is invalid
                model.ChucVuList = await chucVuService.GetAllChucVu();
                return View(model);
            }
        }
        //Sửa Thông Tin Của Nhân Viên
        public async Task<IActionResult> EditProfile(Guid id)
        {
            var sessionData = HttpContext.Session.GetString("NhanVien");

            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Login");
            }
            var nhanVien = JsonConvert.DeserializeObject<NhanVien>(sessionData);

            // Trả về view với thông tin khách hàng
            return View(nhanVien);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(NhanVien nv, IFormFile imgFile)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin nhân viên cũ để xóa ảnh cũ
                var existingNV = await _service.GetIdNhanVien(nv.IdNhanVien);

                // Kiểm tra xem có file ảnh mới được tải lên không
                if (imgFile != null && imgFile.Length > 0)
                {
                    // Kiểm tra loại file (chỉ chấp nhận các định dạng ảnh)
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var ext = Path.GetExtension(imgFile.FileName).ToLowerInvariant();

                    // Kiểm tra phần mở rộng có hợp lệ không
                    if (!permittedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận các file ảnh với định dạng .jpg, .jpeg, .png, .gif.");
                        return View(nv);
                    }

                    // Kiểm tra kích thước file (giới hạn 5MB)
                    if (imgFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File ảnh phải nhỏ hơn 5MB.");
                        return View(nv);
                    }

                    // Tạo đường dẫn đến thư mục lưu trữ hình ảnh
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "imgNV");

                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(existingNV.AnhNhanVien))
                    {
                        string oldImagePath = Path.Combine(uploadFolder, existingNV.AnhNhanVien);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Tạo đường dẫn đến file ảnh mới
                    string newImagePath = Path.Combine(uploadFolder, imgFile.FileName);

                    // Tạo file stream để lưu file hình ảnh
                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        await imgFile.CopyToAsync(stream);
                    }

                    // Gán tên file vào thuộc tính AnhNhanVien của đối tượng NhanVien
                    nv.AnhNhanVien = imgFile.FileName;
                }

                // Cập nhật thông tin nhân viên
                await _service.UpdateThongTin(nv);
                return RedirectToAction("Index");
            }
            else
            {
                return View(nv);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteNV(id);
            return RedirectToAction("Index");
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(NhanVien nv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Thực hiện đăng nhập
                    var kh = await _service.LoginKH(nv.Email, nv.MatKhau);
                    if (kh != null)
                    {
                        // Lưu thông tin khách hàng vào session
                        HttpContext.Session.SetString("NhanVien", JsonConvert.SerializeObject(kh));
                        HttpContext.Session.SetString("AvatarUrl", kh.AnhNhanVien); 
                        HttpContext.Session.SetString("NhanVienName", kh.TenNhanVien);
                        HttpContext.Session.SetString("IdNhanVien", kh.IdNhanVien.ToString());
                        HttpContext.Session.SetString("NhanVienRole", kh.chucVu != null ? kh.chucVu.TenChucVu : "Không xác định"); // Vai trò nhân viên
                        return RedirectToAction("DangNhapThanhCong");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng nhập thất bại. Kiểm tra lại email hoặc mật khẩu.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(nv);
        }
        public IActionResult DangNhapThanhCong()
        {
            // Lấy dữ liệu từ Session
            var sessionData = HttpContext.Session.GetString("NhanVien");

            // Trả về view với dữ liệu từ Session
            return View((object)sessionData);
        }

        // My Profile
        public async Task<IActionResult> MyProfile()
        {
            var sessionData = HttpContext.Session.GetString("NhanVien");
            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Login");

            }
            var nhanVien = JsonConvert.DeserializeObject<NhanVien>(sessionData);

            //if (nhanVien.IdchucVu.HasValue)
            //{
            //    var tenChucVu = chucVuService.GetIdChucVu(nhanVien.IdchucVu.Value); // Sử dụng .Value để lấy giá trị
            //    ViewBag.TenChucVu = tenChucVu;
            //}
            //else
            //{
            //    ViewBag.TenChucVu = "Chức vụ không xác định"; // Hoặc xử lý theo cách bạn muốn
            //}

            return View(nhanVien);
        }
        public async Task<IActionResult> ChangePassword()
        {
            var sessionData = HttpContext.Session.GetString("NhanVien");

            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(Guid id, string newPassword, string confirmPassword)
        {
            var sessionData = HttpContext.Session.GetString("NhanVien");
            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Login");
            }
            var nhanVien = JsonConvert.DeserializeObject<NhanVien>(sessionData);
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View();
            }


            var result = await _service.DoiMK(nhanVien.IdNhanVien, newPassword, confirmPassword);
            if (result)
            {
                nhanVien.MatKhau = newPassword;
                HttpContext.Session.SetString("NhanVien", JsonConvert.SerializeObject(nhanVien));
                ViewBag.Message = "Password changed successfully";
                return RedirectToAction("MyProfile");
            }
            ModelState.AddModelError("", "Đổi mật khẩu thất bại. Vui lòng kiểm tra lại.");
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Vui lòng nhập email.");
                return View();
            }

            var result = await _service.SendVerificationCode(email);

            if (result)
            {
                TempData["Email"] = email; // Sử dụng TempData để lưu email
                return RedirectToAction("VerifyCode");
            }

            ModelState.AddModelError("", "Không thể gửi mã xác thực. Vui lòng thử lại.");
            return View();
        }
        public IActionResult VerifyCode()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyCode(string code, string email)
        {
           var verifycationCode = await _service.GetVerificationCodeFromRedisAsync(email);
            if(code == verifycationCode)
            {
                TempData["Email"] = email;
                return RedirectToAction("ResetPassword");
            }
            ModelState.AddModelError("", "Mã xác thực không chích xác hoặc đã hết hạn");
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RestPassword(string email, string newPassword, string confirmPassword) 
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu không khớp.");
                return View();
            }
            var result = await _service.RestPassword(email,newPassword,confirmPassword);
            if (result)
            {
                ViewBag.Message = "Đổi mật khẩu thành công.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Đổi mật khẩu thất bại. Vui lòng thử lại.");
            return View();
        }
    }
}
