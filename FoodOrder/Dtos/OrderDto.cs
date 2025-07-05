using System.Collections.Generic;

namespace FoodOrderApp.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int TableId { get; set; }
    public int? ConfirmedBy { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string CreatedAt { get; set; }
    public string PaidAt { get; set; }
    public List<OrderItemDto> Items { get; set; }
}
