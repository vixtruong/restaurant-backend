namespace Restaurant.OrderManagementService.DTOs
{
    public class MenuItemDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string? Category { get; set; }

        public string? Description { get; set; }

        public Decimal Price { get; set; }

        public bool? Available { get; set; }
    }
}
