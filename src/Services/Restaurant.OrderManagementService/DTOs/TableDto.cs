namespace Restaurant.OrderManagementService.DTOs
{
    public class TableDto
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public bool Available { get; set; }

        public int? OrderId { get; set; }

        public string? BookedBy { get; set; }
    }
}
