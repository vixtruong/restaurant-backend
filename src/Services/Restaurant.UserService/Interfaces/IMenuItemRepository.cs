using Restaurant.OrderManagementService.DTOs;

namespace Restaurant.OrderManagementService.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync();
        Task<MenuItemDto> GetMenuItemByIdAsync(int id);
        Task<MenuItemDto> CreateMenuItemAsync(MenuItemDto menuItemDto);
        Task<bool> UpdateMenuItemAsync(int id, MenuItemDto menuItemDto);
        Task<bool> UpdateMenuItemStatusAsync(int id);
        Task<bool> DeleteMenuItemAsync(int id);
    }
}
