using AppData.Model;
using APPMVC.IService;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace APPMVC.Areas.Client.Controllers
{
	[Area("Client")]
	public class DiaChiController : Controller
	{
		private readonly IDiaChiService _services;
		private readonly IKhachHangService _servicesKH;
		public DiaChiController(IDiaChiService services, IKhachHangService khachHangService)
		{
			_services = services;
			_servicesKH = khachHangService;
		}
		public async Task<IActionResult> GetAll()
		{
			var IdKhachHang = HttpContext.Session.GetString("IdKhachHang");
			if (string.IsNullOrEmpty(IdKhachHang))
			{
				return RedirectToAction("Login", "KhachHang");
			}
			var id = Guid.Parse(IdKhachHang);
			var diaChiList = await _services.GetAllAsync(id);
			return View(diaChiList);
		}
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var IdKhachHang = HttpContext.Session.GetString("IdKhachHang");
			if (string.IsNullOrEmpty(IdKhachHang))
			{
				return RedirectToAction("Login", "Account");  // Nếu chưa đăng nhập, chuyển hướng đến trang login
			}
			ViewBag.Provinces = await _services.GetProvincesAsync();
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(DiaChi dc)
		{
			//Check số lượng địa chỉ
			var IdKhachHang = HttpContext.Session.GetString("IdKhachHang");
			var id = Guid.Parse(IdKhachHang);
			int addressCount = await _services.GetAddressCountByCustomerId(id);
			if (addressCount >= 3)
			{
				ModelState.AddModelError("", "Khách hàng này đã có tối đa 3 địa chỉ.");
                ViewBag.Provinces = await _services.GetProvincesAsync();
                await LoadDropDownsCreate(dc);
				return View(dc);
			}
			// Kiểm tra địa chỉ mặc định           
			if (dc.DiaChiMacDinh)
			{
				bool check = await _services.HasDefaultAddressAsync(id);
				if (check)
				{
					ModelState.AddModelError("", "Khách hàng này đã có một địa chỉ mặc định.");
                    ViewBag.Provinces = await _services.GetProvincesAsync();
                    await LoadDropDownsCreate(dc);
					return View(dc);
				}
			}
			if (!ModelState.IsValid)
			{
				foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
				{
					Console.WriteLine($"Error: {error.ErrorMessage}");  // In lỗi ra console
				}
                ViewBag.Provinces = await _services.GetProvincesAsync();
                await LoadDropDownsCreate(dc);
				return View(dc);
			}
			if (string.IsNullOrEmpty(IdKhachHang))
			{
				return RedirectToAction("Login", "KhachHang");  // Nếu chưa đăng nhập, chuyển hướng đến trang login
			}
			dc.IdKhachHang = id;
			//Nếu tất cả ok thì thêm =))
			bool success = await _services.AddAsync(dc);
			if (success)
			{
				return RedirectToAction("GetAll");
			}
			await LoadDropDowns(dc);
			return View(dc);
		}
        //Sua
        public async Task<IActionResult> EditDC(Guid IdDiaChi)
        {
            var diaChi = await _services.GetByIdAsync(IdDiaChi);
            await LoadDropDowns(diaChi);
            if (diaChi == null) return NotFound();
            ViewBag.HasDefaultAddress = await _services.HasDefaultAddressAsync(diaChi.IdKhachHang) && !diaChi.DiaChiMacDinh;
            await LoadDropDowns(diaChi);
            return View(diaChi);
        }
        [HttpPost]
        public async Task<IActionResult> EditDC(Guid IdDiaChi, DiaChi dc)
        {
            if (dc.WardId == null)
            {
                ModelState.AddModelError("WardId", "Vui lòng chọn phường/xã.");
                await LoadDropDowns(dc);  // Tải lại các dropdown
                return View(dc);  // Trả về lại view với thông báo lỗi
            }
            if (IdDiaChi == Guid.Empty)
            {
                ModelState.AddModelError("", "ID địa chỉ không hợp lệ.");
                await LoadDropDowns(dc);
                return View(dc);
            }


            if (!ModelState.IsValid)
            {
                await LoadDropDowns(dc);
                return View(dc);
            }
            var IdKhachHang = HttpContext.Session.GetString("IdKhachHang");
            var id = Guid.Parse(IdKhachHang);
			id = dc.IdKhachHang;
            bool success = await _services.UpdateAsync(IdDiaChi, dc);
            if (success)
            {
                return RedirectToAction("GetAll");
            }

            await LoadDropDowns(dc);
            return View(dc);
        }
        //Xóa
        public async Task<IActionResult> Delete(Guid idDiaChi)
		{
			await _services.DeleteAsync(idDiaChi);
			return RedirectToAction("GetAll");
		}

		//Check các kiểu
		[HttpGet]
		public async Task<IActionResult> GetDistricts(int provinceId)
		{
			if (provinceId == 0)
			{
				return Json(new { error = "Invalid provinceId" });
			}
			var districts = await _services.GetDistrictsAsync(provinceId);
			if (districts == null || districts.Count == 0)
			{
				return Json(new { error = "No districts found" });
			}
			var districtList = districts.Select(d => new { DistrictId = d.DistrictId, DistrictName = d.DistrictName }).ToList();
			return Json(districtList);
		}

		[HttpGet]
		public async Task<IActionResult> GetWards(int districtId)
		{
			var wards = await _services.GetWardsAsync(districtId);
			var wardList = wards.Select(w => new {
				WardId = w.WardId.ToString(),
				WardName = w.WardName
			}).ToList();

			return Json(wardList);
		}
		[HttpGet]
		public async Task<IActionResult> GetAddressCountByCustomerId(Guid customerId)
		{
			int count = await _services.GetAddressCountByCustomerId(customerId);
			return Json(count);
		}
		//Hàm Phụ
		private async Task LoadDropDowns(DiaChi dc)
		{
			// Lấy danh sách tỉnh từ DB
			ViewBag.Provinces = await _services.GetProvincesAsync();

			// Nếu ProvinceId có giá trị hợp lệ, lấy danh sách các quận theo ProvinceId
			if (dc != null && dc.ProvinceId > 0)
			{
				ViewBag.Districts = await _services.GetDistrictsAsync(dc.ProvinceId);
			}
			else
			{
				ViewBag.Districts = new List<District>(); // Danh sách trống nếu dc không có ProvinceId
			}

			// Nếu DistrictId có giá trị hợp lệ, lấy danh sách các phường theo DistrictId
			if (dc != null && dc.DistrictId > 0)
			{
				ViewBag.Wards = await _services.GetWardsAsync(dc.DistrictId);
			}
			else
			{
				ViewBag.Wards = new List<Ward>(); // Danh sách trống nếu dc không có DistrictId
			}
		}
        public async Task LoadDropDownsCreate(DiaChi dc)
        {
            ViewBag.Districts = dc.ProvinceId != null
                ? await _services.GetDistrictsAsync(dc.ProvinceId)
                : new List<District>(); // Danh sách trống nếu chưa chọn tỉnh

            ViewBag.Wards = dc.DistrictId != null
                ? await _services.GetWardsAsync(dc.DistrictId)
                : new List<Ward>(); // Danh sách trống nếu chưa chọn quận
        }
    }
}
