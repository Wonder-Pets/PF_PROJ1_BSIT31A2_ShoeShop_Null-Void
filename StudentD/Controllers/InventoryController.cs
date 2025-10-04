using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        // GET: /Inventory
        public async Task<IActionResult> Index(int page = 1, string search = null)
        {
            var model = await _inventoryService.GetPagedShoesAsync(page, 20, search);
            return View(model);
        }

        // GET: /Inventory/Create
        public IActionResult Create()
        {
            // View will render form bound to CreateShoeDto
            return View(new CreateShoeDto());
        }

        // POST: /Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShoeDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                ShoeDto created = await _inventoryService.CreateShoeAsync(dto);
                TempData["Success"] = "Nagawa ang bagong shoe.";
                return RedirectToAction(nameof(Edit), new { id = created.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create shoe failed");
                ModelState.AddModelError("", "May nangyaring error sa pag-save. Tingnan ang logs.");
                return View(dto);
            }
        }

        // GET: /Inventory/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _inventoryService.GetShoeForEditAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        // POST: /Inventory/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateShoeDto dto)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            try
            {
                await _inventoryService.UpdateShoeAsync(dto);
                TempData["Success"] = "Na-update ang detalye ng sapatos.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update shoe failed for id {Id}", id);
                ModelState.AddModelError("", "May problema sa pag-update.");
                return View(dto);
            }
        }

        // POST: /Inventory/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _inventoryService.DeleteShoeAsync(id);
                TempData["Success"] = "Tinanggal ang shoe.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete shoe failed for id {Id}", id);
                TempData["Error"] = "Hindi ma-delete ang shoe. Baka ginamit pa sa ibang talaan.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Inventory/AdjustStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(AdjustStockDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "May kulang o maling input sa stock adjustment.";
                return RedirectToAction(nameof(Edit), new { id = dto.ShoeId });
            }

            try
            {
                // Pass current user for audit trail
                await _inventoryService.AdjustStockAsync(dto, User.Identity.Name);
                TempData["Success"] = "Naayos ang stock.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adjust stock failed");
                TempData["Error"] = "Error sa pag-adjust ng stock.";
            }

            return RedirectToAction(nameof(Edit), new { id = dto.ShoeId });
        }

        // POST: /Inventory/UploadImage/{shoeId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int shoeId, IFormFile image)
        {
            if (image == null)
            {
                TempData["Error"] = "Walang napiling image.";
                return RedirectToAction(nameof(Edit), new { id = shoeId });
            }

            try
            {
                // Service handles saving and validation
                string imageUrl = await _inventoryService.UploadShoeImageAsync(shoeId, image, User.Identity.Name);
                TempData["Success"] = "Larawan na-upload.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload image failed for shoe {Id}", shoeId);
                TempData["Error"] = "Hindi ma-upload ang larawan.";
            }

            return RedirectToAction(nameof(Edit), new { id = shoeId });
        }

        // AJAX: GET stock quantity for a variation
        [HttpGet]
        public async Task<IActionResult> GetStock(int variationId)
        {
            try
            {
                int qty = await _inventoryService.GetStockQuantityAsync(variationId);
                return Json(new { success = true, variationId, qty });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStock failed for variation {VariationId}", variationId);
                return Json(new
                {
                    success = false,
                    error = "Hindi ma-retrieve ang stock
