using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using System;

namespace ShoeShop.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            var report = _reportService.GetInventoryReport();
            return View(report);
        }

        [HttpPost]
        public IActionResult Filter(DateTime from, DateTime to)
        {
            var result = _reportService.FilterInventoryReport(from, to);
            return View("Index", result);
        }

        public IActionResult ExportToPdf()
        {
            var pdf = _reportService.ExportToPdf();
            return File(pdf, "application/pdf", "InventoryReport.pdf");
        }

        public IActionResult ExportToExcel()
        {
            var excel = _reportService.ExportToExcel();
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InventoryReport.xlsx");
        }
    }
}
