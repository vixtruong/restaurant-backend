using System;
using System.Collections.Generic;

namespace Restaurant.Shared.Models;

public partial class Inventory
{
    public int Id { get; set; }

    public string IngredientName { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public DateTime? LastUpdated { get; set; }
}
