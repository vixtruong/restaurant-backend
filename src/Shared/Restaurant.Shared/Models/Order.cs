using System;
using System.Collections.Generic;

namespace Restaurant.Shared.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int TableNumber { get; set; }

    public string? Status { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? EndAt { get; set; }

    public bool? PaymentRequest { get; set; }

    public virtual User? Customer { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
