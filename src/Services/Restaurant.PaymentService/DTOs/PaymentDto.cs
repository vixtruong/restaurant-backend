namespace Restaurant.PaymentService.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Status { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
    }
}
