using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrder.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly FoodOrderDbContext _context;

        public OrderItemService(IOrderItemRepository orderItemRepo,
                                IOrderRepository orderRepo,
                                FoodOrderDbContext context)
        {
            _orderItemRepo = orderItemRepo;
            _orderRepo = orderRepo;
            _context = context;
        }

        // thêm item vào order
        public async Task<bool> AddItemAsync(OrderItemCreateDto dto)
        {
            var menuItem = await _context.MenuItems.FindAsync(dto.MenuItemId);
            if (menuItem == null) return false;

            var order = await _orderRepo.GetByIdAsync(dto.OrderId);
            if (order == null) return false;
            if (order.Status == OrderStatus.Cancelled)
            {
                throw new Exception("Order đã bị hủy!");
            }
            if(order.Status == OrderStatus.Paid)
            {
                throw new Exception("Đơn hàng đã thanh toán rồi!");
            }

            decimal price = menuItem.Price;

            var orderItem = new OrderItem
            {
                OrderId = dto.OrderId,
                MenuItemId = dto.MenuItemId,
                Quantity = dto.Quantity,
                Note = dto.Note,
                Price = price
            };

            await _orderItemRepo.AddAsync(orderItem);

            order.TotalAmount += price * dto.Quantity;
            if(order.Status == OrderStatus.Preparing)
            {
                order.Status = OrderStatus.Update;
            }
            await _orderRepo.UpdateAsync(order);

            return true;
        }


        // lấy toàn bộ item trong order
        public async Task<IEnumerable<OrderItemDto>> GetItemsByOrderIdAsync(int orderId)
        {
            var items = await _orderItemRepo.ListByOrderIdAsync(orderId);

            return items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                MenuItemName = i.MenuItem?.Name ?? "",
                Quantity = i.Quantity,
                Note = i.Note,
                Price = i.Price,
                Image = i.MenuItem?.ImageUrl,
                Status = i.Status == 0 ? "Pending" : "Serving"
            });
        }

        // cập nhật số lượng item
        public async Task<bool> UpdateQuantityAsync(int orderItemId, int quantity)
        {
            if (quantity <= 0) return false;

            var item = await _orderItemRepo.GetByIdAsync(orderItemId);
            if (item == null) return false;

            var order = await _orderRepo.GetByIdAsync(item.OrderId ?? 0);
            if (order == null) 
                return false;

            if(order.Status == OrderStatus.Preparing || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Paid) return false;

            if(item.Status == OrderItemStatus.Serving) return false;
            decimal delta = (quantity - item.Quantity) * item.Price;
            order.TotalAmount += delta;

            item.Quantity = quantity;

            await _orderItemRepo.UpdateAsync(item);
            await _orderRepo.UpdateAsync(order);
            return true;
        }


        public async Task<bool> RemoveItemAsync(int orderItemId)
        {
            var item = await _orderItemRepo.GetByIdAsync(orderItemId);
            if (item == null) return false;

            var order = await _orderRepo.GetByIdAsync(item.OrderId ?? 0);
            if (order == null) return false;
            if (item.Status == OrderItemStatus.Serving) return false;

            order.TotalAmount -= item.Price * item.Quantity;

            await _orderItemRepo.DeleteAsync(item.Id);     
            await _orderRepo.UpdateAsync(order);
            return true;
        }

        public static class OrderItemStatus
        {
            public const int Pending = 0;
            public const int Serving = 1;
        }
    }
}
