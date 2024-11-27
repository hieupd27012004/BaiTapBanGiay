using AppData.Model;
using APPMVC.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;
using X.PagedList;
using AppData.ViewModel;

namespace APPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangController : Controller
    {
        private readonly IKhachHangService _service;
        private readonly IDiaChiService _diaChiService;
        public KhachHangController(IKhachHangService service, IDiaChiService diaChiService)
        {
            _service = service;
            _diaChiService = diaChiService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 5;

            var khachHangs = await _service.GetAllKhachHang();
            var khachHangViewModels = new List<KhachHangViewModel>();

            foreach (var khachHang in khachHangs)
            {
                var diaChiList = await _diaChiService.GetAllAsync(khachHang.IdKhachHang);


                var diaChi = diaChiList?.FirstOrDefault(); 

             
                khachHangViewModels.Add(new KhachHangViewModel
                {
                    KhachHang = khachHang,
                    DiaChi = diaChi?.WardName 
                });
            }

            var sortedKhachHangViewModels = khachHangViewModels
                .OrderByDescending(k => k.KhachHang.NgayTao) // Adjust based on your model
                .ToList();

            var pagedKhachHangViewModels = sortedKhachHangViewModels.ToPagedList(page, pageSize);
            return View(pagedKhachHangViewModels);
        }
        public IActionResult Create()
        {
            return PartialView("Create");
        }
        [HttpPost]
        public async Task<IActionResult> Create(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                await _service.AddKhachHang(kh);
                return RedirectToAction("Index");
            }
            return View(kh);
        }
        public async Task<IActionResult> Edit(Guid id)
        {

            var kh = await _service.GetIdKhachHang(id);
            if(kh == null)
            {
                return NotFound();
            }
            return View(kh);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateKhachHang(kh);
                return RedirectToAction("Index");
            }
            return View(kh);
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteKhachHang(id);
            return RedirectToAction("Index");
        }

    }
}
