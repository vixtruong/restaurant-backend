using System.Collections.Generic;

public class OrderRequestDto
{
    public int? OrderId { get; set; }
    public int CustomerId { get; set; }
    public int TableNumber { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

public class OrderItemDto
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}