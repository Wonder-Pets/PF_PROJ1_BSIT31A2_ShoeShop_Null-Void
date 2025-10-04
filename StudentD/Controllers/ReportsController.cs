using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoeShop.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace ShoeShop.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var report = await _reportService.GetInventoryReportAsync();
            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(DateTime from, DateTime to)
        {
            try
            {
                var result = await _reportService.FilterInventoryReportAsync(from, to);
                return View("Index", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering report");
                TempData["Error"] = "Failed to filter report.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> ExportToPdf()
        {
            try
            {
                var pdf = await _reportService.ExportToPdfAsync();
                return File(pdf, "application/pdf", "InventoryReport.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report to PDF");
                TempData["Error"] = "Failed to export PDF.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var excel = await _reportService.ExportToExcelAsync();
                return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InventoryReport.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report to Excel");
                TempData["Error"] = "Failed to export Excel.";
                return RedirectToAction("Index");
            }
        }
    }
}
