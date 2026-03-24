using _2380600659_HieuNguyen.Models;
using _2380600659_HieuNguyen.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2380600659_HieuNguyen.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            var orderDetails = await _orderRepository.GetOrderDetailsAsync(orderId);
            ViewBag.OrderDetails = orderDetails;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            await _orderRepository.UpdateOrderStatusAsync(orderId, status);
            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction(nameof(Details), new { orderId = orderId });
        }
    }
}