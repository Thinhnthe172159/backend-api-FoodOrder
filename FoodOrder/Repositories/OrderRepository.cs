using FoodOrder.IRepositories;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
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
            return await _context.Orders.Include(o => o.Customer).Where(o => o.CustomerId == customerId).ToListAsync();
        }

        public async Task<IEnumerable<Order>> ListByStatusAsync(string status)
        {
            return await _context.Orders.Include(o => o.Customer).Where(o => o.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<OrderDto>> SearchOrderAsynce(OrderDto orderDto)
        {
            var orderLists = _context.Orders.Include(o => o.Customer).Include(o => o.Table).Include(o => o.OrderItems).ThenInclude(o => o.MenuItem).AsSingleQuery();
            if (orderDto.CustomerId.HasValue && orderDto.CustomerId != 0)
            {
                orderLists = orderLists.Where(o => o.CustomerId == orderDto.CustomerId);
            }
            if (!string.IsNullOrEmpty(orderDto.CustomerName))
            {
                orderLists = orderLists.Where(o => EF.Functions.Collate(o.Customer.Username, "Latin1_General_CI_AI").Contains(orderDto.CustomerName));
            }
            if (orderDto.TableId.HasValue && orderDto.TableId != 0)
            {
                orderLists = orderLists.Where(o => o.TableId == orderDto.TableId);
            }
            if (!string.IsNullOrEmpty(orderDto.Status))
            {
                orderLists = orderLists.Where(o => o.Status == orderDto.Status);
            }

            return await orderLists.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Username,
                CreatedAt = o.CreatedAt.ToString(),
                Status = o.Status,
                ConfirmedBy = o.ConfirmedBy,
                TableId = o.TableId,
                TableName = o.Table.TableNumber,
                TotalAmount = o.TotalAmount,
                PaidAt = o.PaidAt.ToString(),
                Items = o.OrderItems.Select(x => new OrderItemDto
                {
                    MenuItemId = x.Id,
                    MenuItemName = x.MenuItem.Name,
                    Note = x.Note,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList()
            }).ToListAsync();
        }
    }
}
