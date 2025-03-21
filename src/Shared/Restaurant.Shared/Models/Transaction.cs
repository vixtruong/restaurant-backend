using System;
using System.Collections.Generic;

namespace Restaurant.Shared.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int? PaymentId { get; set; }

    public string TransactionType { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Payment? Payment { get; set; }
}
