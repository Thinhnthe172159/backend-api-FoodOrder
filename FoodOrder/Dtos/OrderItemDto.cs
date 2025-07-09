namespace FoodOrderApp.Application.DTOs;

public class OrderItemDto
{
    public int? Id { get; set; }
    public int? MenuItemId { get; set; }
    public string MenuItemName { get; set; }
    public string? Image {  get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }
}
