using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class DashboardController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IReportService reportService, ILogger<DashboardController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            try
            {
                // InventoryReportDto: totals, lowStockList, recentTransactions, inventoryValue
                InventoryReportDto model = await _reportService.GetDashboardReportAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["Error"] = "May error habang kino-load ang dashboard.";
                return View(new InventoryReportDto()); // fallback model
            }
        }

        // AJAX endpoint: partial low stock widget (polling)
        [HttpGet]
        public async Task<IActionResult> LowStockWidget()
        {
            var lowStock = await _reportService.GetLowStockAsync();
            return PartialView("_LowStockWidget", lowStock);
        }

        // AJAX JSON: quick totals (for mobile dashboard)
        [HttpGet]
        public async Task<IActionResult> QuickTotals()
        {
            var totals = await _reportService.GetQuickTotalsAsync();
            return Json(totals);
        }
    }
}
