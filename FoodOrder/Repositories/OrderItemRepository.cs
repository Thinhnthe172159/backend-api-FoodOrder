using FoodOrder.IRepositories;
using FoodOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(FoodOrderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderItem>> ListByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems.Include(o => o.MenuItem).Where(o => o.OrderId == orderId).ToListAsync();
        }
    }
}
