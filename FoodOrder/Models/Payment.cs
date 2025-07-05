using System;
using System.Collections.Generic;

namespace FoodOrder.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = null!;

    public DateTime? PaymentTime { get; set; }

    public virtual Order? Order { get; set; }
}
