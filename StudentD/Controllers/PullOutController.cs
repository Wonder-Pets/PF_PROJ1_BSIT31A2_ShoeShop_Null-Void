using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.DTOs;
using System;
using System.Threading.Tasks;

namespace ShoeShop.Controllers
{
    [Authorize]
    public class PullOutController : Controller
    {
        private readonly IPullOutService _pullOutService;
        private readonly ILogger<PullOutController> _logger;
        private readonly IAuditService _auditService;

        public PullOutController(IPullOutService pullOutService, ILogger<PullOutController> logger, IAuditService auditService)
        {
            _pullOutService = pullOutService;
            _logger = logger;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _pullOutService.GetAllPullOutRequestsAsync();
            return View(requests);
        }

        public IActionResult Request()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Request(CreatePullOutDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _pullOutService.RequestPullOutAsync(dto);
                await _auditService.LogAsync(User.Identity.Name, "Requested pull out", dto.ShoeId.ToString());

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting pull out");
                ModelState.AddModelError("", "Failed to submit pull out request.");
                return View(dto);
            }
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _pullOutService.ApprovePullOutAsync(id, User.Identity.Name);
                await _auditService.LogAsync(User.Identity.Name, "Approved pull out", id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving pull out {id}");
                TempData["Error"] = "Failed to approve pull out request.";
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                await _pullOutService.RejectPullOutAsync(id, User.Identity.Name);
                await _auditService.LogAsync(User.Identity.Name, "Rejected pull out", id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting pull out {id}");
                TempData["Error"] = "Failed to reject pull out request.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _pullOutService.GetPullOutByIdAsync(id);
            if (request == null)
                return NotFound();

            return View(request);
        }
    }
}
