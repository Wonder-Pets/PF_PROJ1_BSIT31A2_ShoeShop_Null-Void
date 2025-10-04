using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // Displays inventory reports
        public async Task<IActionResult> Index()
        {
            var report = await _reportService.GetInventoryReportAsync();
            return View(report);
        }

        // Exports report data to Excel or PDF
        [HttpGet]
        public async Task<IActionResult> Export(string type)
        {
            var file = await _reportService.ExportReportAsync(type);
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
