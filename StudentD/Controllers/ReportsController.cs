using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        // GET: /Reports
        public IActionResult Index()
        {
            return View();
        }

        // POST or GET: /Reports/Inventory (filter parameters via ReportFilterDto)
        [HttpGet]
        public async Task<IActionResult> Inventory(ReportFilterDto filter)
        {
            var model = await _reportService.GenerateInventoryReportAsync(filter);
            return View("Inventory", model);
        }

        // Export CSV (GET)
        [HttpGet]
        public async Task<IActionResult> ExportInventoryCsv(ReportFilterDto filter)
        {
            string csv = await _reportService.ExportInventoryCsvAsync(filter);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
