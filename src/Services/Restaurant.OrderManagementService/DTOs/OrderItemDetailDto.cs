namespace Restaurant.OrderManagementService.DTOs
{
    public class OrderItemDetailDto
    {
        public int Id { get; set; }
        public string? MenuItemName { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Price { get; set; }
    }
}
