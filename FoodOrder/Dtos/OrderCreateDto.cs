using System.Collections.Generic;

namespace FoodOrderApp.Application.DTOs;

public class OrderCreateDto
{
    public int CustomerId { get; set; }
    public int TableId { get; set; }
    public List<OrderItemCreateDto> Items { get; set; }
}
