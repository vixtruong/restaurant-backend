namespace Restaurant.OrderManagementService.DTOs
{
    public class InventoryDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
    }
}
