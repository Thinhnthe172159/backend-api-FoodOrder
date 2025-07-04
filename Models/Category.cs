using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FoodOrder.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
