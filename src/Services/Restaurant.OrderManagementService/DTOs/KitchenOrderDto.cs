namespace Restaurant.OrderManagementService.DTOs
{
    public class KitchenOrderDto
    {
        public int? Id { get; set; }
        public int? OrderItemId { get; set; }
        public int? MenuItemId { get; set; }
        public string? Status { get; set; }
        public DateTime? CookAt { get; set; }
    }
}
