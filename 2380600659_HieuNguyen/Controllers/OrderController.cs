using _2380600659_HieuNguyen.Models;
using _2380600659_HieuNguyen.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2380600659_HieuNguyen.Controllers
{
    [Authorize] // Bắt buộc đăng nhập mới xem được lịch sử
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // 1. Hiển thị danh sách đơn hàng của user đang đăng nhập
        public async Task<IActionResult> Index()
        {
            // Lấy ID của user đang đăng nhập
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Lấy tất cả đơn hàng, sau đó lọc ra những đơn của user này và sắp xếp mới nhất lên đầu
            var allOrders = await _orderRepository.GetAllOrdersAsync();
            var userOrders = allOrders
                                .Where(o => o.ApplicationUserId == userId)
                                .OrderByDescending(o => o.OrderDate);

            return View(userOrders);
        }

        // 2. Xem chi tiết một đơn hàng cụ thể
        public async Task<IActionResult> Details(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var order = await _orderRepository.GetOrderByIdAsync(id);

            // Bảo mật: Đảm bảo đơn hàng tồn tại và phải đúng là của user đang đăng nhập
            if (order == null || order.ApplicationUserId != userId)
            {
                return NotFound();
            }

            var orderDetails = await _orderRepository.GetOrderDetailsAsync(id);
            ViewBag.OrderDetails = orderDetails;

            return View(order);
        }
    }
}