using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _orderService;

        public PurchaseOrderController(IPurchaseOrderService orderService)
        {
            _orderService = orderService;
        }

        // Displays all purchase orders
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        // Shows the form to create a new purchase order
        public IActionResult Create()
        {
            return View();
        }

        // Handles creation of a new purchase order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _orderService.CreateOrderAsync(dto);
            TempData["Success"] = "Purchase order successfully created.";
            return RedirectToAction(nameof(Index));
        }

        // Displays the details of a specific purchase order
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // Confirms or updates order status
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            TempData["Success"] = "Order status updated.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
