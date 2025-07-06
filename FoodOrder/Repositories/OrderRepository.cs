using FoodOrder.IRepositories;
using FoodOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(FoodOrderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> ListByCustomerAsync(int customerId)
        {
            return await _context.Orders.Include(o=>o.Customer).Where(o=>o.CustomerId == customerId).ToListAsync();
        }

        public async Task<IEnumerable<Order>> ListByStatusAsync(string status)
        {
            return await _context.Orders.Include(o=>o.Customer).Where(o=>o.Status == status).ToListAsync(); 
        }
    }
}
