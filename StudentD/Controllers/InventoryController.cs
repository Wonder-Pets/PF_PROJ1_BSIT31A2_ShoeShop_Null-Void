using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.DTOs;
using Microsoft.AspNetCore.Http;
using System;

namespace ShoeShop.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public IActionResult Index()
        {
            var items = _inventoryService.GetAllShoes();
            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateShoeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                _inventoryService.AddShoe(dto);
                TempData["Success"] = "Shoe added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        public IActionResult Edit(int id)
        {
            var shoe = _inventoryService.GetShoeById(id);
            return View(shoe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ShoeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            _inventoryService.UpdateShoe(dto);
            return RedirectToAction("Index");
        }

        public IActionResult AdjustStock(int id)
        {
            var shoe = _inventoryService.GetStockAdjustmentInfo(id);
            return View(shoe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdjustStock(StockAdjustmentDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            _inventoryService.AdjustStock(dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UploadImage(int id, IFormFile image)
        {
            if (image != null)
            {
                _inventoryService.UploadShoeImage(id, image);
            }
            return RedirectToAction("Edit", new { id });
        }

        public IActionResult ViewColors(int shoeId)
        {
            var colors = _inventoryService.GetColorVariations(shoeId);
            return View(colors);
        }
    }
}
