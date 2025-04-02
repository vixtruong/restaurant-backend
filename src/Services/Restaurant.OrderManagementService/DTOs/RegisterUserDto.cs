namespace Restaurant.OrderManagementService.DTOs
{
    public class RegisterUserDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int RoleId { get; set; }
    }
}
