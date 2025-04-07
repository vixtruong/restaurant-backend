using System;
using System.Collections.Generic;

namespace Restaurant.Shared.Models;

public partial class KitchenOrder
{
    public int Id { get; set; }

    public int? OrderItemId { get; set; }

    public string? Status { get; set; }

    public DateTime? CookedAt { get; set; }

    public bool? Done { get; set; }

    public virtual OrderItem? OrderItem { get; set; }
}
