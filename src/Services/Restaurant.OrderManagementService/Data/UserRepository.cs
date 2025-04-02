using Microsoft.EntityFrameworkCore;
using Restaurant.Shared.Data;
using Restaurant.Shared.Models;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly RestaurantDbContext _context;

        public UserRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> GetCustomerByPhoneNumberAsync(string phoneNumber)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && u.RoleId == 4);

            if (user == null) return null;

            return new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RefreshToken = user.RefreshToken,
                RoleName = "Customer",
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };
        }

        // GET ALL USERS
        public async Task<IEnumerable<UserDto>> GetAllEmployeesAsync()
        {
            return await _context.Users.Include(u => u.Role)
                .Where(user => user.RoleId != 4)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = user.Role.RoleName,
                    JoinTime = user.CreatedAt
                }).ToListAsync();
        }

        public async Task<IEnumerable<UserDto>> GetAllCustomersAsync()
        {
            return await _context.Users.Include(u => u.Role)
                .Where(user => user.RoleId == 4)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = user.Role.RoleName,
                    JoinTime = user.CreatedAt
                }).ToListAsync();
        }

        // GET USER BY ID
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);

            return new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.Role.RoleName,
            };
        }

        public async Task<UserDto> GetUserByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    RoleName = u.Role.RoleName,
                    RefreshToken = u.RefreshToken
                })
                .FirstOrDefaultAsync();
        }

        // GET USER BY EMAIL
        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            return new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.Role.RoleName,
            };
        }

        // ADD USER
        public async Task AddUserAsync(RegisterUserDto userDto)
        {
            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                RoleId = userDto.RoleId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PhoneNumber),
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDto> AddCustomerAsync(EntryRequestDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = "@customer.local",
                PhoneNumber = dto.PhoneNumber,
                RoleId = 4,
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleName = "Customer",
            };
        }

        // UPDATE USER
        public async Task<bool> UpdateUserAsync(UserDto userDto)
        {
            var user = await _context.Users.FindAsync(userDto.Id);
            if (user == null) return false;

            user.Email = userDto.Email;
            user.FullName = userDto.FullName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.RefreshToken = userDto.RefreshToken;
            user.RefreshTokenExpiryTime = userDto.RefreshTokenExpiryTime;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE USER
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        // VERIFY LOGIN
        public async Task<bool> VerifyAccount(LoginRequestDto loginRequestDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequestDto.Email);

            if (user == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.PasswordHash);
        }
    }
}
