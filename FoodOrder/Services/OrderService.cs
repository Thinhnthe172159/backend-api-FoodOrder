using FoodOrder.Extentions;
using FoodOrder.Hubs;
using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using static FoodOrder.Services.OrderItemService;

namespace FoodOrder.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly FoodOrderDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderService(
         IOrderRepository orderRepository,
         FoodOrderDbContext context,
         IHubContext<NotificationHub> hubContext)
        {
            _orderRepository = orderRepository;
            _context = context;
            _hubContext = hubContext;
        }

        private async Task NotifyCustomerOrderUpdatedAsync(Order order)
        {
            var orderDto = await GetOrderByIdAsync(order.Id); 

            await _hubContext.Clients.User(order.CustomerId.ToString())
                .SendAsync("OrderStatusUpdated", orderDto);
        }


        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return false;
            if (order.OrderItems.Count != 0)
            {
                if (order.Status == OrderStatus.Preparing)
                {
                    throw new Exception("Bạn không thể hủy khi bàn khi chưa thanh toán");
                }
                if (order.Status == OrderStatus.Paid || order.Status == OrderStatus.Cancelled)
                {
                    throw new Exception("Order này không còn khả dụng");
                }
            }


            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);
            var orderMenuItemList = await _context.OrderItems.Where(o => o.OrderId == order.Id).ToListAsync();
            foreach (var item in orderMenuItemList)
            {
                if (item.Status == OrderItemStatus.Canncel)
                {
                  //  item.Status = OrderItemStatus.Canncel;
                    _context.OrderItems.Remove(item);
                }
            }

            var table = await _context.Tables.FindAsync(order.TableId);
            if (table != null)
            {
                table.Status = "available";
                await _context.SaveChangesAsync();
            }
            await NotifyCustomerOrderUpdatedAsync(order);
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
                CreatedAt = TimeZoneChange.ConvertToTimeZone(DateTime.UtcNow, "SE Asia Standard Time"),
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
                    Items = o.OrderItems.Select(i => new OrderItemDto { Id = i.Id,MenuItemId = i.MenuItemId, MenuItemName = i.MenuItem.Name, Quantity = i.Quantity, Note = i.Note, Price = i.Price, Image = i.MenuItem.ImageUrl, Status = OrderItemStatus.getStatusItemOrder(i.Status) }).OrderByDescending(i=>i.Id).ToList()
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
                        Id = i.Id,
                        MenuItemId = i.MenuItemId,
                        MenuItemName = i.MenuItem.Name,
                        Quantity = i.Quantity,
                        Note = i.Note,
                        Price = i.Price,
                        Image = i.MenuItem.ImageUrl,
                        Status = OrderItemStatus.getStatusItemOrder(i.Status)
                    }).OrderByDescending(i => i.Id).ToList()
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
                if (status == "cancelled" || status == "paid")
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
        // xác thực order và các món được chọn
        public async Task<bool> ConfirmOrderAsync(int orderId, int staffId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;
            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Update)
            {
                order.Status = OrderStatus.Preparing;
                order.ConfirmedBy = staffId;

                var orderMenuItemList = await _context.OrderItems.Where(o => o.OrderId == order.Id).ToListAsync();
                foreach (var item in orderMenuItemList)
                {
                    if (item.Status == OrderItemStatus.Pending || item.Status == OrderItemStatus.Update)
                    {
                        item.Status = OrderItemStatus.Serving;
                        _context.OrderItems.Update(item);
                    }
                }
                await _context.SaveChangesAsync();
                await _orderRepository.UpdateAsync(order);
                await NotifyCustomerOrderUpdatedAsync(order);
                return true;
            }

            return false;
        }


        public async Task<bool> MarkAsPaidAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;
            if (order.Status == OrderStatus.Paid) return false;
            order.Status = OrderStatus.Paid;


            order.PaidAt = TimeZoneChange.ConvertToTimeZone(DateTime.UtcNow, "SE Asia Standard Time");

            await _orderRepository.UpdateAsync(order);
            var orderMenuItemList = await _context.OrderItems.Where(o => o.OrderId == order.Id).ToListAsync();
            foreach (var item in orderMenuItemList)
            {
                if (item.Status != OrderItemStatus.Paid)
                {
                    item.Status = OrderItemStatus.Paid;
                    _context.OrderItems.Update(item);
                }
            }

            var table = await _context.Tables.FindAsync(order.TableId);
            if (table != null)
            {
                table.Status = "available";
                await _context.SaveChangesAsync();
            }
            await NotifyCustomerOrderUpdatedAsync(order);
            return true;
        }

        public Task<IEnumerable<OrderDto>> SearchOrderAsync(OrderDto data)
        {
            return _orderRepository.SearchOrderAsynce(data);
        }

        public Task<IEnumerable<OrderDto>> GetAllCurrentOrderByCustomer(int id)
        {
            return _orderRepository.GetAllCurrentOrderByCustomer(id);
        }
    }

    public static class OrderStatus
    {
        public const string Pending = "pending";
        public const string Preparing = "preparing";
        public const string Paid = "paid";
        public const string Cancelled = "cancelled";
        public const string Update = "update";
    }


}
