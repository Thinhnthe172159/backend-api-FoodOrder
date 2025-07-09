using System;
using System.Collections.Generic;

namespace FoodOrder.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? MenuItemId { get; set; }

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public decimal Price { get; set; }

    public int Status { get; set; } = 0;

    public virtual MenuItem? MenuItem { get; set; }

    public virtual Order? Order { get; set; }
}
