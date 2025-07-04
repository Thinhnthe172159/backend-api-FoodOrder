using FoodOrderApp.Application.DTOs;

namespace FoodOrder.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(PaymentCreateDto dto);
        Task<IEnumerable<PaymentDto>> GetPaymentsAsync(DateTime? from = null, DateTime? to = null);
    }
}
