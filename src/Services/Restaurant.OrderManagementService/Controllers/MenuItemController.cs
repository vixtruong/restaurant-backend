using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Controllers
{
    [Authorize]
    [Route("api/v1/menu-items")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemController(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMenuItems()
        {
            var items = await _menuItemRepository.GetAllMenuItemsAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            var item = await _menuItemRepository.GetMenuItemByIdAsync(id);
            if (item == null) return NotFound(new { message = "MenuItem not found" });

            return Ok(item);
        }

        //[Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemDto? menuItemDto)
        {
            if (menuItemDto == null) return BadRequest(new { message = "Invalid data" });

            var createdItem = await _menuItemRepository.CreateMenuItemAsync(menuItemDto);
            return CreatedAtAction(nameof(GetMenuItemById), new { id = createdItem.Id }, createdItem);
        }

        //[Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItemDto menuItemDto)
        {
            var updated = await _menuItemRepository.UpdateMenuItemAsync(id, menuItemDto);
            if (!updated) return NotFound(new { message = "MenuItem not found" });

            return NoContent();
        }

        //[Authorize]
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateMenuItemStatus(int id)
        {
            var updated = await _menuItemRepository.UpdateMenuItemStatusAsync(id);
            if (!updated) return NotFound(new { message = "MenuItem not found" });

            return NoContent();
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var deleted = await _menuItemRepository.DeleteMenuItemAsync(id);
            if (!deleted) return NotFound(new { message = "MenuItem not found" });

            return NoContent();
        }
    }
}