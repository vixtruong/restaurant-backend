﻿using Restaurant.OrderManagementService.DTOs;

namespace Restaurant.OrderManagementService.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task AddUserAsync(RegisterUserDto userDto);
        Task UpdateUserAsync(UserDto userDto);
        Task DeleteUserAsync(string id);
        Task<bool> VerifyAccount(LoginRequestDto loginRequestDto);
    }
}
