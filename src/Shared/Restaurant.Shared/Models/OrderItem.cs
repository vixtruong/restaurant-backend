using System;
using System.Collections.Generic;

namespace Restaurant.Shared.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? MenuItemId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<KitchenOrder> KitchenOrders { get; set; } = new List<KitchenOrder>();

    public virtual MenuItem? MenuItem { get; set; }

    public virtual Order? Order { get; set; }
}
