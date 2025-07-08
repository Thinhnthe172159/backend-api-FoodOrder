using FoodOrderApp.Application.DTOs;

namespace FoodOrder.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersAsync(int? customerId = null, string status = null);
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(OrderCreateDto dto);
        Task<bool> UpdateOrderStatusAsync(int id, string status);
        Task<bool> CancelOrderAsync(int id);
        Task<bool> ConfirmOrderAsync(int orderId, int staffId);
        Task<bool> MarkAsPaidAsync(int orderId);

        Task<IEnumerable<OrderDto>> SearchOrderAsync(OrderDto data);
    }

}
