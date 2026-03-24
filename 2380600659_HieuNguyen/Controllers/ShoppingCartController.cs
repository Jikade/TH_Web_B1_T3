using _2380600659_HieuNguyen.Models;
using _2380600659_HieuNguyen.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _2380600659_HieuNguyen.Controllers
{
    [Authorize] // Phải đăng nhập mới dùng giỏ hàng
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, IOrderRepository orderRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int count)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart cartFromDb = await _context.ShoppingCarts.FirstOrDefaultAsync(u => u.ApplicationUserId == userId && u.ProductId == productId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += count;
                _context.ShoppingCarts.Update(cartFromDb);
            }
            else
            {
                ShoppingCart cart = new() { ProductId = productId, ApplicationUserId = userId, Count = count };
                _context.ShoppingCarts.Add(cart);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartList = await _context.ShoppingCarts.Include(p => p.Product).Where(u => u.ApplicationUserId == userId).ToListAsync();
            return View(cartList);
        }

        // --- TRANG THANH TOÁN ---
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartList = await _context.ShoppingCarts.Include(p => p.Product).Where(u => u.ApplicationUserId == userId).ToListAsync();
            var user = await _userManager.FindByIdAsync(userId);

            var orderHeader = new OrderHeader
            {
                ApplicationUserId = userId,
                Name = user.FullName,
                PhoneNumber = user.PhoneNumber ?? "",
                Address = user.Address ?? "",
                OrderTotal = cartList.Sum(item => item.Product.Price * item.Count)
            };

            ViewBag.CartList = cartList;
            return View(orderHeader);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(OrderHeader orderHeader)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartList = await _context.ShoppingCarts.Include(p => p.Product).Where(u => u.ApplicationUserId == userId).ToListAsync();

            orderHeader.ApplicationUserId = userId;
            orderHeader.OrderDate = DateTime.Now;
            orderHeader.OrderStatus = SD.StatusPending;
            orderHeader.OrderTotal = cartList.Sum(item => item.Product.Price * item.Count);

            var orderDetails = cartList.Select(cart => new OrderDetail
            {
                ProductId = cart.ProductId,
                Count = cart.Count,
                Price = cart.Product.Price
            }).ToList();

            await _orderRepository.AddOrderAsync(orderHeader, orderDetails);

            // Xóa giỏ hàng sau khi đặt thành công
            _context.ShoppingCarts.RemoveRange(cartList);
            await _context.SaveChangesAsync();

            return View("OrderConfirmation", orderHeader.Id);
        }
    }
}