using _2380600659_HieuNguyen.Models;

namespace _2380600659_HieuNguyen.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderHeader>> GetAllOrdersAsync();
        Task<OrderHeader> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync(int orderId);
        Task AddOrderAsync(OrderHeader order, List<OrderDetail> details);
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}
