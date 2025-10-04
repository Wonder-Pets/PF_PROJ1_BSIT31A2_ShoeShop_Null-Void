using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class PullOutController : Controller
    {
        private readonly IPullOutService _pullOutService;
        private readonly ILogger<PullOutController> _logger;

        public PullOutController(IPullOutService pullOutService, ILogger<PullOutController> logger)
        {
            _pullOutService = pullOutService;
            _logger = logger;
        }

        // GET: /PullOut
        public async Task<IActionResult> Index(string status = "Pending", int page = 1)
        {
            var model = await _pullOutService.GetPagedPullOutsAsync(status, page, 20);
            return View(model);
        }

        // GET: /PullOut/Create
        public IActionResult Create()
        {
            return View(new CreatePullOutDto());
        }

        // POST: /PullOut/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePullOutDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
