using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _userRepository = userRepository;
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
            var token = GenerateToken(user, _configuration);

            return Ok(new { token });
        }

        // Function generate JWT
        public string GenerateToken(UserDto user, IConfiguration configuration)
        {
            var issuer = configuration["JwtConfig:Issuer"];
            var audience = configuration["JwtConfig:Audience"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, user.RoleName),
            };

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }
    }
}
