using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _poService;
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(IPurchaseOrderService poService, ILogger<PurchaseOrderController> logger)
        {
            _poService = poService;
            _logger = logger;
        }

        // GET: /PurchaseOrder
        public async Task<IActionResult> Index(string status = null, int page = 1)
        {
            var model = await _poService.GetPagedPurchaseOrdersAsync(status, page, 20);
            return View(model);
        }

        // GET: /PurchaseOrder/Create
        public async Task<IActionResult> Create()
        {
            var vm = await _poService.GetCreatePurchaseOrderViewModelAsync();
            return View(vm);
        }

        // POST: /PurchaseOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                PurchaseOrderDto po = await _poService.CreatePurchaseOrderAsync(dto, User.Identity.Name);
                TempData["Success"] = $"Purchase Order {po.OrderNumber} created.";
                return RedirectToAction(nameof(Details), new { id = po.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create PO failed");
                ModelState.AddModelError("", "Error sa pag-create ng purchase order.");
                return View(dto);
            }
        }

        // GET: /PurchaseOrder/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var model = await _poService.GetPurchaseOrderByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        // POST: /PurchaseOrder/Confirm/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            try
            {
                await _poService.ConfirmPurchaseOrderAsync(id, User.Identity.Name);
                TempData["Success"] = "PO confirmed.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Confirm PO failed for {Id}", id);
                TempData["Error"] = "Hindi ma-confirm ang PO.";
            }
            return RedirectToAction
