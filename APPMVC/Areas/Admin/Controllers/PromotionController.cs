using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppData;
using AppData.Model;
using APPMVC.IService;

namespace APPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(IPromotionService promotionService, ILogger<PromotionController> logger)
        {
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<Promotion> promotions = await _promotionService.GetPromotionsAsync();
                _logger.LogInformation($"Retrieved {promotions.Count} promotions");
                return View(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching promotion list");
                return View(new List<Promotion>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promotion promotion)
        {
            if (!ModelState.IsValid)
            {
                return View(promotion);
            }

            try
            {
                promotion.NgayTao = DateTime.Now;
                var result = await _promotionService.CreateAsync(promotion);
                if (result)
                {
                    _logger.LogInformation($"Successfully created promotion with Id: {promotion.IdPromotion}");
                    TempData["SuccessMessage"] = "Promotion created successfully.";
                    return RedirectToAction(nameof(Index)); // Redirect to Index action
                }
                else
                {
                    _logger.LogWarning($"Failed to create promotion with Id: {promotion.IdPromotion}");
                    ModelState.AddModelError("", "Failed to create the promotion. Please check server logs for details.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during promotion creation");
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(promotion);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound("Id cannot be empty");
            }

            try
            {
                var promotion = await _promotionService.GetPromotionByIdAsync(id);
                if (promotion != null)
                {
                    var result = await _promotionService.DeleteAsync(id);
                    if (result)
                    {
                        _logger.LogInformation($"Successfully deleted promotion with Id: {id}");
                        TempData["SuccessMessage"] = "Promotion deleted successfully.";
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to delete promotion with Id: {id}");
                        TempData["ErrorMessage"] = "Failed to delete the promotion.";
                    }
                }
                else
                {
                    _logger.LogWarning($"Attempted to delete non-existent promotion with Id: {id}");
                    TempData["ErrorMessage"] = "Promotion not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting promotion with Id: {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the promotion.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound("Id cannot be empty");
            }

            try
            {
                var promotion = await _promotionService.GetPromotionByIdAsync(id);
                if (promotion == null)
                {
                    _logger.LogWarning($"Promotion not found for Id: {id}");
                    return NotFound("Promotion not found.");
                }
                ViewBag.TrangThaiList = new SelectList(new[]
                {
                    new { Value = 0, Text = "Disabled" },
                    new { Value = 1, Text = "Active" },
                    new { Value = 2, Text = "Paused" }
                }, "Value", "Text", promotion.TrangThai);
                return View(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching promotion with Id: {id}");
                return StatusCode(500, "An error occurred while retrieving the promotion.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Promotion promotion)
        {
            if (id != promotion.IdPromotion)
            {
                return BadRequest("Id mismatch");
            }

            if (!ModelState.IsValid)
            {
                return View(promotion);
            }

            try
            {
                var result = await _promotionService.UpdateAsync(promotion);
                if (result)
                {
                    _logger.LogInformation($"Successfully updated promotion with Id: {promotion.IdPromotion}");
                    TempData["SuccessMessage"] = "Promotion updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning($"Failed to update promotion with Id: {promotion.IdPromotion}");
                    ModelState.AddModelError("", "Failed to update the promotion. Please check server logs for details.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred during promotion update. Promotion Id: {promotion.IdPromotion}");
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(promotion);
        }
    }
}
