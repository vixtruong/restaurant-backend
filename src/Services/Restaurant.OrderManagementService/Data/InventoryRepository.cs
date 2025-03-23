using Microsoft.EntityFrameworkCore;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;
using Restaurant.Shared.Data;
using Restaurant.Shared.Models;

namespace Restaurant.OrderManagementService.Data
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly RestaurantDbContext _context;

        public InventoryRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryDto>> GetAllIngredientsAsync()
        {
            return await _context.Inventories
                .Select(i => new InventoryDto
                {
                    Id = i.Id,
                    Name = i.IngredientName,
                    Unit = i.Unit,
                    Quantity = i.Quantity,

                }).ToListAsync();
        }

        public async Task<InventoryDto> GetIngredientByIdAsync(int id)
        {
            var ingredient = await _context.Inventories.FindAsync(id);

            if (ingredient == null) return null;

            return new InventoryDto
            {
                Id = ingredient.Id,
                Name = ingredient.IngredientName,
                Unit = ingredient.Unit,
                Quantity = ingredient.Quantity,
            };
        }

        public async Task<InventoryDto> AddIngredientAsync(InventoryDto dto)
        {
            var ingredient = new Inventory
            {
                IngredientName = dto.Name,
                Unit = dto.Unit,
                Quantity = dto.Quantity,
                LastUpdated = DateTime.UtcNow
            };

            await _context.AddAsync(ingredient);
            await _context.SaveChangesAsync();
            return new InventoryDto
            {
                Name = ingredient.IngredientName,
                Unit = ingredient.Unit,
                Quantity = ingredient.Quantity,
                Id = ingredient.Id
            };
        }

        public async Task<bool> UpdateIngredientAsync(InventoryDto dto)
        {
            var ingredient = await _context.Inventories.FindAsync(dto.Id);

            if (ingredient == null) return false;

            ingredient.Quantity += dto.Quantity;
            ingredient.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteIngredientByIdAsync(int id)
        {
            var ingredient = await _context.Inventories.FindAsync(id);

            if (ingredient == null) return false;

            _context.Inventories.Remove(ingredient);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
