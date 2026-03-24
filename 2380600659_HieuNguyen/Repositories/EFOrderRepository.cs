using _2380600659_HieuNguyen.Models;
using Microsoft.EntityFrameworkCore;

namespace _2380600659_HieuNguyen.Repositories
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public EFOrderRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<OrderHeader>> GetAllOrdersAsync()
        {
            return await _context.OrderHeaders.Include(u => u.ApplicationUser).ToListAsync();
        }

        public async Task<OrderHeader> GetOrderByIdAsync(int id)
        {
            return await _context.OrderHeaders.Include(u => u.ApplicationUser).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync(int orderId)
        {
            return await _context.OrderDetails.Include(p => p.Product).Where(o => o.OrderId == orderId).ToListAsync();
        }

        public async Task AddOrderAsync(OrderHeader order, List<OrderDetail> details)
        {
            await _context.OrderHeaders.AddAsync(order);
            await _context.SaveChangesAsync(); // Lưu để lấy Order Id

            foreach (var item in details)
            {
                item.OrderId = order.Id;
                await _context.OrderDetails.AddAsync(item);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.OrderHeaders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
