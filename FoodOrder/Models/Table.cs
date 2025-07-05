using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FoodOrder.Models;

public partial class Table
{
    public int Id { get; set; }

    public string TableNumber { get; set; } = null!;

    public string Qrcode { get; set; } = null!;

    public string? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
