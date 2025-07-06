using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly FoodOrderDbContext _context;

        public OrderService(IOrderRepository orderRepository, FoodOrderDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);

            var table = await _context.Tables.FindAsync(order.TableId);
            if (table != null)
            {
                table.Status = "available";
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderCreateDto dto)
        {
            var table = await _context.Tables.FindAsync(dto.TableId)
                       ?? throw new Exception("Bàn không tồn tại");

            if (table.Status == "occupied")
                throw new Exception("Bàn đã có người sử dụng");

            table.Status = "occupied";
            await _context.SaveChangesAsync();

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                TableId = dto.TableId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 0
            };
            await _orderRepository.AddAsync(order);

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                TableId = order.TableId,
                Status = order.Status,
                CreatedAt = order.CreatedAt.ToString()
            };
        }



        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.Include(c => c.Customer).Include(o => o.ConfirmedByNavigation).Include(o => o.Table).Include(o => o.OrderItems).ThenInclude(o => o.MenuItem)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    TableId = o.TableId,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt.ToString(),
                    CustomerName = o.Customer.Username,
                    StaffName = o.ConfirmedByNavigation.Username,
                    TableName = o.Table.TableNumber,
                    TotalAmount = o.TotalAmount,
                    PaidAt = o.PaidAt.ToString(),
                    ConfirmedBy = o.ConfirmedBy,
                    Items = o.OrderItems.Select(i => new OrderItemDto { MenuItemId = i.Id, MenuItemName = i.MenuItem.Name, Quantity = i.Quantity, Note = i.Note, Price = i.Price }).ToList()
                })
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync(int? customerId = null, string status = null)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ConfirmedByNavigation)
                .Include(o => o.Table)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .Where(o => (customerId == null || o.CustomerId == customerId) &&
                            (status == null || o.Status == status))
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.Username,
                    TableName = o.Table.TableNumber,
                    StaffName = o.ConfirmedByNavigation != null ? o.ConfirmedByNavigation.Username : null,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt.ToString(),
                    PaidAt = o.PaidAt.ToString(),
                    Items = o.OrderItems.Select(i => new OrderItemDto
                    {
                        MenuItemId = i.MenuItemId,
                        MenuItemName = i.MenuItem.Name,
                        Quantity = i.Quantity,
                        Note = i.Note,
                        Price = i.Price
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return false;
            }
            try
            {
                order.Status = status;
                await _orderRepository.UpdateAsync(order);
                if (status == "done")
                {
                    var table = await _context.Tables.FindAsync(order.TableId);
                    if (table != null)
                    {
                        table.Status = "available";
                        await _context.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ConfirmOrderAsync(int orderId, int staffId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Pending)
                return false;

            order.Status = OrderStatus.Preparing;
            order.ConfirmedBy = staffId;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> MarkAsPaidAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Served)
                return false;

            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            var table = await _context.Tables.FindAsync(order.TableId);
            if (table != null)
            {
                table.Status = "available";
                await _context.SaveChangesAsync();
            }
            return true;
        }

    }

    public static class OrderStatus
    {
        public const string Pending = "pending";
        public const string Preparing = "preparing";
        public const string Served = "served";
        public const string Paid = "paid";
        public const string Cancelled = "cancelled";
    }


}
