using AppData.Model;
using APPMVC.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList;

namespace APPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MauSacController : Controller
    {
        public IMauSacService _services;

        public MauSacController(IMauSacService services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> Getall(string? name, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 5;
            List<MauSac> timten = await _services.GetMauSac(name);
            if (timten != null)
            {
                var pagedMauSacs = timten.ToPagedList(page, pageSize);
                return View(pagedMauSacs);
            }
            else
            {
                List<MauSac> mauSacs = await _services.GetMauSac(name);
                var pagedMauSacs = mauSacs.ToPagedList(page, pageSize);
                return View(pagedMauSacs);
            }
        }

        public IActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(MauSac mauSac)
        {
            if (!ModelState.IsValid)
            {
                // Kiểm tra tính hợp lệ của dữ liệu
                if (ModelState.ContainsKey("TenMauSac"))
                {
                    var error = ModelState["TenMauSac"].Errors.FirstOrDefault();
                    if (error != null)
                    {
                        TempData["Error"] = error.ErrorMessage;
                    }
                }
                return RedirectToAction("Getall");
            }
            try
            {
                await _services.Create(mauSac);
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
            var mauSac = await _services.GetMauSacById(id);
            if (mauSac == null)
            {
                return NotFound();
            }
            return View(mauSac);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(MauSac mauSac)
        {
            if (!ModelState.IsValid)
            {
                // Kiểm tra tính hợp lệ của dữ liệu
                if (ModelState.ContainsKey("TenMauSac"))
                {
                    var error = ModelState["TenMauSac"].Errors.FirstOrDefault();
                    if (error != null)
                    {
                        TempData["Error"] = error.ErrorMessage;
                    }
                }
                return View(mauSac);
            }
            try
            {
                await _services.Update(mauSac);
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
            var mauSac = await _services.GetMauSacById(id);
            if (mauSac == null)
            {
                return NotFound();
            }
            return View(mauSac);
        }
    }
}