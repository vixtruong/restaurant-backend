using Restaurant.OrderManagementService.DTOs;

namespace Restaurant.OrderManagementService.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByRefreshTokenAsync(string refreshToken);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> GetCustomerByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<UserDto>> GetAllEmployeesAsync();
        Task<IEnumerable<UserDto>> GetAllCustomersAsync();
        Task AddUserAsync(RegisterUserDto userDto);
        Task<UserDto> AddCustomerAsync(EntryRequestDto dto);
        Task<bool> UpdateUserAsync(UserDto userDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> VerifyAccount(LoginRequestDto loginRequestDto);
    }
}
