using Restaurant.OrderManagementService.DTOs;

namespace Restaurant.OrderManagementService.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<InventoryDto>> GetAllIngredientsAsync();
        Task<InventoryDto> GetIngredientByIdAsync(int id);
        Task<InventoryDto> AddIngredientAsync(InventoryDto dto);
        Task<bool> UpdateIngredientAsync(InventoryDto dto);
        Task<bool> DeleteIngredientByIdAsync(int id);
    }
}
