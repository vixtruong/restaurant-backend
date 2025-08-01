﻿namespace Restaurant.OrderManagementService.DTOs
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? TableNumber { get; set; }
        public bool IsPaid { get; set; } = false;
        public List<OrderItemDetailDto>? OrderItems { get; set; }
    }
}
