using AppData.Model;
using APPMVC.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APPMVC.Areas.Client.Controllers
{
    [Area("Client")]
    public class KhachHangController : Controller
    {
        
        private readonly IKhachHangService _service;
        private readonly IDiaChiService _dcService;
        private readonly ICardService _cardService;
        public KhachHangController(IKhachHangService service, IDiaChiService diaChiService, ICardService cardService)
        {
            _service = service;
            _dcService = diaChiService;
            _cardService = cardService;
        }

        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> Register(KhachHang kh = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    kh.AuthProvider = "1";
                    kh.NgayTao = DateTime.Now;
                    kh.NgayCapNhat = DateTime.Now;
                    kh.NguoiTao = "Client";
                    kh.NguoiCapNhat = "Client";
                    kh.KichHoat = 1;
                    await _service.AddKhachHang(kh);

                    // Set a success message and redirect to the Login action
                    TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", $"Inner Exception: {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                // Handle model state errors
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in allErrors)
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                ModelState.AddModelError("", "Có lỗi xảy ra khi điền thông tin, vui lòng kiểm tra lại.");
            }

            return View(kh); // Return the view with the entered data
        }

        public IActionResult DangKyThanhCong()
        {
            return Content("OKroi");
        }
        public IActionResult DangNhapThanhCong()
        {
            // Lấy dữ liệu từ Session
            var sessionData = HttpContext.Session.GetString("KhachHang");

            // Trả về view với dữ liệu từ Session
            return View((object)sessionData);
        }
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(KhachHang khach)
		{
			if (ModelState.IsValid)
			{
				try
				{
					// Thực hiện đăng nhập
					var kh = await _service.LoginKH(khach.Email, khach.MatKhau);
					if (kh != null)
					{
						// Lưu thông tin khách hàng vào session
						HttpContext.Session.SetString("KhachHang", JsonConvert.SerializeObject(kh));
						HttpContext.Session.SetString("TenKhachHang", kh.HoTen);
						HttpContext.Session.SetString("IdKhachHang", kh.IdKhachHang.ToString()); // Chuyển đổi sang chuỗi
						return RedirectToAction("Index", "HomeClient", new { area = "Client" }); // Chuyển hướng đến trang Index
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
			return View(khach);
		}


		public IActionResult ThongTinKhachHang()
        {
            var sessionData = HttpContext.Session.GetString("KhachHang");

            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Login");
            }
            var khachHang = JsonConvert.DeserializeObject<KhachHang>(sessionData);

            return View(khachHang);
        }
        public async Task<IActionResult> Edit()
		{
			// Lấy thông tin KhachHang từ session
			var sessionData = HttpContext.Session.GetString("KhachHang");

			if (string.IsNullOrEmpty(sessionData))
			{
				// Nếu session trống, điều hướng về trang đăng nhập
				return RedirectToAction("Login");
			}

			// Deserialize sessionData để chuyển thành đối tượng KhachHang
			var khachHang = JsonConvert.DeserializeObject<KhachHang>(sessionData);

			// Trả về view với thông tin khách hàng
			return View(khachHang);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(KhachHang khachHang)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _service.UpdateKHThongTin(khachHang);
					HttpContext.Session.SetString("KhachHang", JsonConvert.SerializeObject(khachHang));
					HttpContext.Session.SetString("TenKhachHang", khachHang.HoTen);
					HttpContext.Session.SetString("IdKhachHang", JsonConvert.SerializeObject(khachHang.IdKhachHang));
					return RedirectToAction("ThongTinKhachHang");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}
			return View(khachHang);
		}
        // Doi MK
        public async Task<IActionResult> ChangePassword()
        {
            var sessionData = HttpContext.Session.GetString("KhachHang");

            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(Guid id, string newPassword, string confirmPassword)
        {
            var sessionData = HttpContext.Session.GetString("KhachHang");
            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Login");
            }
            var khachHang = JsonConvert.DeserializeObject<KhachHang>(sessionData);
            if(newPassword != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View();
            }
            var result = await _service.ChangePassword(khachHang.IdKhachHang, newPassword, confirmPassword);
            if(result)
            {
                khachHang.MatKhau = newPassword;
                HttpContext.Session.SetString("KhachHang", JsonConvert.SerializeObject(khachHang));
                ViewBag.Message = "Đổi Mật Khẩu Thành Công";
                return RedirectToAction("ThongTinKhachHang");
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
            if (code == verifycationCode)
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
        public async Task<IActionResult> ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu không khớp.");
                return View();
            }
            var result = await _service.ResetPassword(email, newPassword, confirmPassword);
            if (result)
            {
                
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Đổi mật khẩu thất bại. Vui lòng thử lại.");
            return View();
        }

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();

			TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";

			return RedirectToAction("Login");
		}
	}
}
