using Microsoft.AspNetCore.Mvc;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIngredients()
        {
            var inventories = await _inventoryRepository.GetAllIngredientsAsync();

            return Ok(inventories);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddIngredient([FromBody] InventoryDto? dto)
        {
            if (dto == null) return BadRequest("Invalid data");

            var createdIngredient = await _inventoryRepository.AddIngredientAsync(dto);

            return CreatedAtAction(nameof(GetIngredientById),
                new { id = createdIngredient.Id, name = createdIngredient.Name }, createdIngredient);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngredientById(int id)
        {
            var ingredient = await _inventoryRepository.GetIngredientByIdAsync(id);
            if (ingredient == null) return NotFound(new { message = "Ingredient not found" });

            return Ok(ingredient);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateIngredient([FromBody] InventoryDto? dto)
        {
            if (dto == null) return BadRequest("Invalid data");

            var updated = await _inventoryRepository.UpdateIngredientAsync(dto);

            if (!updated) return NotFound(new { message = "Ingredient not found" });

            return NoContent();
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var deleted = await _inventoryRepository.DeleteIngredientByIdAsync(id);

            if (!deleted) return NotFound(new { message = "Ingredient not found" });

            return NoContent();
        }
    }
}
