namespace FoodOrderApp.Application.DTOs;

public class OrderItemDto
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; }
    public int Quantity { get; set; }
    public string Note { get; set; }
    public decimal Price { get; set; }
}
