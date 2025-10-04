using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ShoeShop.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;
        private readonly IAuditService _auditService; // Placeholder for audit logging

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger, IAuditService auditService)
        {
            _inventoryService = inventoryService;
            _logger = logger;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _inventoryService.GetAllShoesAsync();
            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShoeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _inventoryService.AddShoeAsync(dto);

                // Audit log
                await _auditService.LogAsync(User.Identity.Name, "Created new shoe", dto.Name);

                TempData["Success"] = "Shoe added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shoe");
                ModelState.AddModelError("", "An unexpected error occurred while adding the shoe.");
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var shoe = await _inventoryService.GetShoeByIdAsync(id);
            if (shoe == null)
                return NotFound();

            return View(shoe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ShoeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _inventoryService.UpdateShoeAsync(dto);

                await _auditService.LogAsync(User.Identity.Name, "Updated shoe", dto.Id.ToString());

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating shoe ID {dto.Id}");
                ModelState.AddModelError("", "Failed to update shoe.");
                return View(dto);
            }
        }

        public async Task<IActionResult> AdjustStock(int id)
        {
            var shoe = await _inventoryService.GetStockAdjustmentInfoAsync(id);
            if (shoe == null)
                return NotFound();

            return View(shoe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(StockAdjustmentDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _inventoryService.AdjustStockAsync(dto);

                await _auditService.LogAsync(User.Identity.Name, "Adjusted stock", $"ShoeId: {dto.ShoeId}, Qty: {dto.AdjustQuantity}");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adjusting stock for shoe ID {dto.ShoeId}");
                ModelState.AddModelError("", "Failed to adjust stock.");
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int id, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                TempData["Error"] = "Please select a valid image file.";
                return RedirectToAction("Edit", new { id });
            }

            // Validate file type (basic example)
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = System.IO.Path.GetExtension(image.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || Array.IndexOf(permittedExtensions, ext) < 0)
            {
                TempData["Error"] = "Unsupported image format.";
                return RedirectToAction("Edit", new { id });
            }

            try
            {
                await _inventoryService.UploadShoeImageAsync(id, image);

                await _auditService.LogAsync(User.Identity.Name, "Uploaded shoe image", $"ShoeId: {id}");

                TempData["Success"] = "Image uploaded successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading image for shoe ID {id}");
                TempData["Error"] = "Failed to upload image.";
            }

            return RedirectToAction("Edit", new { id });
        }

        public async Task<IActionResult> ViewColors(int shoeId)
        {
            var colors = await _inventoryService.GetColorVariationsAsync(shoeId);
            return View(colors);
        }
    }
}
