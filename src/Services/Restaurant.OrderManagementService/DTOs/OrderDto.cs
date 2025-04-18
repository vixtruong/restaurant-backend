namespace Restaurant.OrderManagementService.DTOs
{
    public class OrderDto
    {
        public int? Id { get; set; }

        public int? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public int TableNumber { get; set; }

        public string? Status { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? EndAt { get; set; }

        public bool? PaymentRequest { get; set; }
    }
}
