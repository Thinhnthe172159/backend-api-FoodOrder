using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> AddItemAsync(OrderItemCreateDto dto)
        {
            var menuItem = await _context.MenuItems.FindAsync(dto.MenuItemId);
            if (menuItem == null) return false;

            var order = await _orderRepo.GetByIdAsync(dto.OrderId);
            if (order == null) return false;

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
            await _orderRepo.UpdateAsync(order);

            return true;
        }


        public async Task<IEnumerable<OrderItemDto>> GetItemsByOrderIdAsync(int orderId)
        {
            var items = await _orderItemRepo.ListByOrderIdAsync(orderId);

            return items.Select(i => new OrderItemDto
            {
                MenuItemId = i.MenuItemId,
                MenuItemName = i.MenuItem?.Name ?? "",
                Quantity = i.Quantity,
                Note = i.Note,
                Price = i.Price
            });
        }


        public async Task<bool> UpdateQuantityAsync(int orderItemId, int quantity)
        {
            if (quantity <= 0) return false;

            var item = await _orderItemRepo.GetByIdAsync(orderItemId);
            if (item == null) return false;

            var order = await _orderRepo.GetByIdAsync(item.OrderId ?? 0);
            if (order == null) return false;

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

            order.TotalAmount -= item.Price * item.Quantity;

            await _orderItemRepo.DeleteAsync(item.Id);     
            await _orderRepo.UpdateAsync(order);
            return true;
        }
    }
}
