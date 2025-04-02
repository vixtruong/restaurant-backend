using System;
using System.Collections.Generic;

namespace Restaurant.Shared.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? PasswordHash { get; set; } = null!;

    public int? RoleId { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role? Role { get; set; }
}
