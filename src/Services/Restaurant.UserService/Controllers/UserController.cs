using Microsoft.AspNetCore.Mvc;
using Restaurant.UserService.DTOs;
using Restaurant.UserService.Interfaces;

namespace Restaurant.UserService.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto? userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Invalid data");
            }

            await _userRepository.AddUserAsync(userDto);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userDto.Id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            await _userRepository.UpdateUserAsync(userDto);
            return NoContent();
        }
    }
}
