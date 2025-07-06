using FoodOrderApp.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }          

    public int? TableId { get; set; }
    public string? TableName { get; set; }          

    public int? ConfirmedBy { get; set; }
    public string? StaffName { get; set; }          

    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? CreatedAt { get; set; }
    public string? PaidAt { get; set; }

    public List<OrderItemDto> Items { get; set; }
}
