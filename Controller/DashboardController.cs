using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using System;

namespace ShoeShop.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IReportService _reportService;

        public DashboardController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            var report = _reportService.GetDashboardReport();
            return View(report);
        }

        [HttpGet]
        public IActionResult GetLowStock()
        {
            var lowStock = _reportService.GetLowStockItems();
            return Json(lowStock);
        }

        [HttpGet]
        public IActionResult GetRecentActivity()
        {
            var logs = _reportService.GetRecentInventoryActivity();
            return Json(logs);
        }
    }
}
