namespace FoodOrderApp.Application.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; }
    public string PaymentTime { get; set; }
}
