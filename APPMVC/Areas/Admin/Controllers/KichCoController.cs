using AppData.Model;
using APPMVC.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList;

namespace APPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KichCoController : Controller
    {
        public IKichCoService _services;

        public KichCoController(IKichCoService services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> Getall(string? name, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 5;
            List<KichCo> timten = await _services.GetKichCo(name);
            if (timten != null)
            {
                var pagedKichCos = timten.ToPagedList(page, pageSize);
                return View(pagedKichCos);
            }
            else
            {
                List<KichCo> kichCos = await _services.GetKichCo(name);
                var pagedKichCos = kichCos.ToPagedList(page, pageSize);
                return View(pagedKichCos);
            }
        }

        public IActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(KichCo kichCo)
        {
            if (!ModelState.IsValid)
            {
                // Kiểm tra tính hợp lệ của dữ liệu
                if (ModelState.ContainsKey("TenKichCo"))
                {
                    var error = ModelState["TenKichCo"].Errors.FirstOrDefault();
                    if (error != null)
                    {
                        TempData["Error"] = error.ErrorMessage;
                    }
                }
                return RedirectToAction("Getall");
            }
            try
            {
                await _services.Create(kichCo);
                TempData["Success"] = "Thêm mới thành công";
            }
            catch (Exception)
            {
                TempData["Error"] = "Thêm mới thất bại";
            }
            return RedirectToAction("Getall");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var kichCo = await _services.GetKichCoById(id);
            if (kichCo == null)
            {
                return NotFound();
            }
            return View(kichCo);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(KichCo kichCo)
        {
            if (!ModelState.IsValid)
            {
                // Kiểm tra tính hợp lệ của dữ liệu
                if (ModelState.ContainsKey("TenKichCo"))
                {
                    var error = ModelState["TenKichCo"].Errors.FirstOrDefault();
                    if (error != null)
                    {
                        TempData["Error"] = error.ErrorMessage;
                    }
                }
                return View(kichCo);
            }
            try
            {
                await _services.Update(kichCo);
                TempData["Success"] = "Cập nhật thành công";
            }
            catch (Exception)
            {
                TempData["Error"] = "Cập nhật thất bại";
            }
            return RedirectToAction("Getall");
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _services.Delete(id);
                TempData["Success"] = "Xóa thành công";
            }
            catch (Exception)
            {
                TempData["Error"] = "Xóa thất bại";
            }
            return RedirectToAction("Getall");
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var kichCo = await _services.GetKichCoById(id);
            if (kichCo == null)
            {
                return NotFound();
            }
            return View(kichCo);
        }
    }
}