namespace FoodOrderApp.Application.DTOs;

public class OrderItemCreateDto
{
    public int OrderId { get; set; }         
    public int MenuItemId { get; set; }  
    public int Quantity { get; set; }      
    public string? Note { get; set; }     
}
