using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Shows all available shoes
        public async Task<IActionResult> Index()
        {
            var shoes = await _inventoryService.GetAllShoesAsync();
            return View(shoes);
        }

        // Displays the form for adding a new shoe
        public IActionResult Create()
        {
            return View();
        }

        // Handles the creation of a new shoe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShoeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _inventoryService.CreateShoeAsync(dto);
            TempData["Success"] = "Shoe successfully added.";
            return RedirectToAction(nameof(Index));
        }

        // Displays the edit form for a specific shoe
        public async Task<IActionResult> Edit(int id)
        {
            var shoe = await _inventoryService.GetShoeByIdAsync(id);
            if (shoe == null) return NotFound();
            return View(shoe);
        }

        // Handles updates to a shoe record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShoeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _inventoryService.UpdateShoeAsync(id, dto);
            TempData["Success"] = "Shoe details successfully updated.";
            return RedirectToAction(nameof(Index));
        }

        // Deactivates a shoe (soft delete)
        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _inventoryService.DeactivateShoeAsync(id);
            TempData["Success"] = "Shoe successfully deactivated.";
            return RedirectToAction(nameof(Index));
        }

        // Displays the color variations and stock of a shoe
        public async Task<IActionResult> Details(int id)
        {
            var shoeDetails = await _inventoryService.GetShoeDetailsAsync(id);
            return View(shoeDetails);
        }
    }
}
