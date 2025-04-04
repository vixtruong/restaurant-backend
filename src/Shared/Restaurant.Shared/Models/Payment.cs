using System;
using System.Collections.Generic;

namespace Restaurant.Shared.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? UserId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User? User { get; set; }
}
