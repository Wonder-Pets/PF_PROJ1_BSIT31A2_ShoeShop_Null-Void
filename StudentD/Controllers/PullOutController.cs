using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class PullOutController : Controller
    {
        private readonly IPullOutService _pullOutService;

        public PullOutController(IPullOutService pullOutService)
        {
            _pullOutService = pullOutService;
        }

        // Displays all pull-out requests
        public async Task<IActionResult> Index()
        {
            var pullOuts = await _pullOutService.GetAllPullOutsAsync();
            return View(pullOuts);
        }

        // Shows form to request a pull-out
        public IActionResult Create()
        {
            return View();
        }

        // Handles pull-out request submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePullOutDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _pullOutService.CreatePullOutRequestAsync(dto);
            TempData["Success"] = "Pull-out request successfully created.";
            return RedirectToAction(nameof(Index));
        }

        // Approves or rejects a pull-out request
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _pullOutService.UpdatePullOutStatusAsync(id, status);
            TempData["Success"] = $"Pull-out request marked as {status}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
