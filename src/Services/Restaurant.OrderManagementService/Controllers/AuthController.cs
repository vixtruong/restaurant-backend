using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;

        public AuthController(IUserRepository userRepository, IConfiguration configuration, IOrderRepository orderRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        // API login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var isVerifyLogin = await _userRepository.VerifyAccount(loginDto);
            if (!isVerifyLogin)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            var accessToken = GenerateAccessToken(user, _configuration);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { accessToken = accessToken, refreshToken = refreshToken });
        }

        [HttpPost("entry")]
        public async Task<IActionResult> Entry([FromBody] EntryRequestDto? dto)
        {
            var isAvailableTable = await _orderRepository.IsAvailableTableAsync(dto.TableNumber);

            Console.WriteLine($"Table number: {dto.TableNumber} - Is available: {isAvailableTable}");

            if (!isAvailableTable)
            {
                return BadRequest(new { message = "Table is not available" });
            }

            var accessToken = "";
            var refreshToken = "";

            var existUser = await _userRepository.GetCustomerByPhoneNumberAsync(dto.PhoneNumber);

            OrderRequestDto order;

            if (existUser != null)
            {
                accessToken = GenerateAccessToken(existUser, _configuration);
                refreshToken = GenerateRefreshToken();

                existUser.RefreshToken = refreshToken;
                existUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24);
                await _userRepository.UpdateUserAsync(existUser);

                order = new OrderRequestDto
                {
                    CustomerId = existUser.Id,
                    TableNumber = dto.TableNumber
                };

                var newOrder1 = await _orderRepository.CreateOrderAsync(order);
                return Ok(new { accessToken = accessToken, refreshToken = refreshToken, orderId = newOrder1.Id });
            }

            var user = await _userRepository.AddCustomerAsync(dto);
            order = new OrderRequestDto
            {
                CustomerId = user.Id,
                TableNumber = dto.TableNumber
            };

            var newOrder2 = await _orderRepository.CreateOrderAsync(order);

            accessToken = GenerateAccessToken(user, _configuration);
            refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24);
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { accessToken = accessToken, refreshToken = refreshToken, orderId = newOrder2.Id });
        }

        // REFRESH TOKEN
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto tokenRequest)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(tokenRequest.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var newAccessToken = GenerateAccessToken(user, _configuration);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
        }

        // LOGOUT WITH REFRESH TOKEN
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto logoutRequest)
        {
            if (string.IsNullOrEmpty(logoutRequest.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var user = await _userRepository.GetUserByRefreshTokenAsync(logoutRequest.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "Logged out successfully" });
        }

        // Function generate JWT
        public string GenerateAccessToken(UserDto user, IConfiguration configuration)
        {
            var issuer = configuration["JwtConfig:Issuer"];
            var audience = configuration["JwtConfig:Audience"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Key"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleName),
            };

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
