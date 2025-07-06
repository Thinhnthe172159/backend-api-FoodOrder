namespace FoodOrderApp.Application.DTOs;

public class MenuItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsAvailable { get; set; }
    public string? CategoryName { get; set; }
    public int? CategoryId { get; set; }
}
