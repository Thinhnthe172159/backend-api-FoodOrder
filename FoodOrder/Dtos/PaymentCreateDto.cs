namespace FoodOrderApp.Application.DTOs;

public class PaymentCreateDto
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; }
}
