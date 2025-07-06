﻿namespace FoodOrder.Dtos
{
    public class CreateMenuItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsAvailable { get; set; } = true;
        public int? CategoryId { get; set; }
    }
}