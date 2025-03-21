using System;
using System.Collections.Generic;

namespace Restaurant.Shared.Models;

public partial class MenuItem
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? Category { get; set; }

    public bool? Available { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
