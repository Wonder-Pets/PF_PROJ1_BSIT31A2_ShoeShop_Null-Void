using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.DTOs;
using System;
using System.Threading.Tasks;

namespace ShoeShop.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _orderService;
        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly IAuditService _auditService;

        public PurchaseOrderController(IPurchaseOrderService orderService, ILogger<PurchaseOrderController> logger, IAuditService auditService)
        {
            _orderService = orderService;
            _logger = logger;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _orderService.CreatePurchaseOrderAsync(dto);
                await _auditService.LogAsync(User.Identity.Name, "Created purchase order", dto.OrderNumber);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase order");
                ModelState.AddModelError("", "Failed to create purchase order.");
                return View(dto);
            }
        }

        public async Task<IActionResult> Confirm(int id)
        {
            try
            {
                await _orderService.ConfirmOrderAsync(id);
                await _auditService.LogAsync(User.Identity.Name, "Confirmed purchase order", id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming purchase order {id}");
                TempData["Error"] = "Failed to confirm purchase order.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Receive(int id)
        {
            try
            {
                await _orderService.ReceiveOrderAsync(id);
                await _auditService.LogAsync(User.Identity.Name, "Received purchase order", id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error receiving purchase order {id}");
                TempData["Error"] = "Failed to receive purchase order.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
