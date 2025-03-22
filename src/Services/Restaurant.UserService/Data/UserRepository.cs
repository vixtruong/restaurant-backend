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

        // GET ALL USERS
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.Role)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = user.Role.RoleName,
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // UPDATE USER
        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _context.Users.FindAsync(userDto.Id);

            if (user != null)
            {
                user.Email = userDto.Email;
                user.FullName = userDto.FullName;
                user.PhoneNumber = userDto.PhoneNumber;
                await _context.SaveChangesAsync();
            }
        }

        // DELETE USER
        public async Task DeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
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
