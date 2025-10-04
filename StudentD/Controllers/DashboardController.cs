using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoeShop.Services.Interfaces;
using System.Threading.Tasks;

namespace ShoeShop.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IReportService reportService, ILogger<DashboardController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var report = await _reportService.GetDashboardReportAsync();
            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> GetLowStock()
        {
            try
            {
                var lowStock = await _reportService.GetLowStockItemsAsync();
                return Json(lowStock);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error fetching low stock items");
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentActivity()
        {
            try
            {
                var logs = await _reportService.GetRecentInventoryActivityAsync();
                return Json(logs);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent activity logs");
                return StatusCode(500, "Error retrieving data");
            }
        }
    }
}
