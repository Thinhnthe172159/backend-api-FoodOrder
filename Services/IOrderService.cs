using FoodOrderApp.Application.DTOs;

namespace FoodOrder.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersAsync(int? customerId = null, string status = null);
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(OrderCreateDto dto);
        Task<bool> UpdateOrderStatusAsync(int id, string status);
        Task<bool> CancelOrderAsync(int id);
    }
}
