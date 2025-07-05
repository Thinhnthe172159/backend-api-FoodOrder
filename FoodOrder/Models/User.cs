using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FoodOrder.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }
    [JsonIgnore]
    public virtual ICollection<Order> OrderConfirmedByNavigations { get; set; } = new List<Order>();
    [JsonIgnore]
    public virtual ICollection<Order> OrderCustomers { get; set; } = new List<Order>();
    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;
}
