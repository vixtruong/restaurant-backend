namespace Restaurant.OrderManagementService.DTOs
{
    public class KitchenOrderDto
    {
        public int? Id { get; set; }
        public int? OrderItemId { get; set; }
        public int? tableNumber { get; set; }
        public MenuItemDto? MenuItem { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
        public DateTime? CookAt { get; set; }
        public bool? Done { get; set; }
    }
}