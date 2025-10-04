using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.DTOs;
using System;

namespace ShoeShop.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _orderService;

        public PurchaseOrderController(IPurchaseOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            var orders = _orderService.GetAllOrders();
            return View(orders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreatePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            _orderService.CreatePurchaseOrder(dto);
            return RedirectToAction("Index");
        }

        public IActionResult Confirm(int id)
        {
            _orderService.ConfirmOrder(id);
            return RedirectToAction("Index");
        }

        public IActionResult Receive(int id)
        {
            _orderService.ReceiveOrder(id);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var order = _orderService.GetOrderById(id);
            return View(order);
        }
    }
}
