using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Return list all users
        [Authorize]
        [HttpGet("employees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var users = await _userRepository.GetAllEmployeesAsync();
            return Ok(users);
        }

        // Return list all customers
        //[Authorize(Roles = "Admin")]
        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var users = await _userRepository.GetAllCustomersAsync();
            return Ok(users);
        }

        // Get user by id
        //[Authorize]
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

        // register staff or add customer into system
        [HttpPost("add")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto? userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Invalid data");
            }

            await _userRepository.AddUserAsync(userDto);
            return Ok(new { message = "User is added successfully" });
        }

        // update information user
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            var updated = await _userRepository.UpdateUserAsync(userDto);

            if (!updated)
            {
                return NotFound("User not found");
            }

            return NoContent();
        }

        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userRepository.DeleteUserAsync(id);

            if (!deleted) return NotFound("User not found");

            return NoContent();
        }
    }
}
