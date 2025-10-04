using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.DTOs;
using System;

namespace ShoeShop.Controllers
{
    [Authorize]
    public class PullOutController : Controller
    {
        private readonly IPullOutService _pullOutService;

        public PullOutController(IPullOutService pullOutService)
        {
            _pullOutService = pullOutService;
        }

        public IActionResult Index()
        {
            var requests = _pullOutService.GetAllPullOutRequests();
            return View(requests);
        }

        public IActionResult Request()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Request(CreatePullOutDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            _pullOutService.RequestPullOut(dto);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Approve(int id)
        {
            _pullOutService.ApprovePullOut(id, User.Identity.Name);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Reject(int id)
        {
            _pullOutService.RejectPullOut(id, User.Identity.Name);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var request = _pullOutService.GetPullOutById(id);
            return View(request);
        }
    }
}
