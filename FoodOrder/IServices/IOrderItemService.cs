using FoodOrderApp.Application.DTOs;

namespace FoodOrder.IServices
{
    public interface IOrderItemService
    {
        Task<bool> AddItemAsync(OrderItemCreateDto dto);
        Task<bool> UpdateQuantityAsync(int orderItemId, int quantity);
        Task<bool> RemoveItemAsync(int orderItemId);
        Task<IEnumerable<OrderItemDto>> GetItemsByOrderIdAsync(int orderId);
    }
}
