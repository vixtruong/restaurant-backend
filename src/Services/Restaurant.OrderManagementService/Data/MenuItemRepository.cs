using Microsoft.EntityFrameworkCore;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;
using Restaurant.Shared.Data;
using Restaurant.Shared.Models;

namespace Restaurant.OrderManagementService.Data
{
    public class MenuItemRepository : IMenuItemRepository
    { 
        private readonly RestaurantDbContext _context;

        public MenuItemRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET ALL MENU ITEMS
        public async Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync()
        {
            return await _context.MenuItems
                .Select(i => new MenuItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Category = i.Category,
                    Description = i.Description,
                    Price = i.Price,
                    Available = i.Available,
                }).ToListAsync();
        }

        // GET MENUITEM BY ID
        public async Task<MenuItemDto> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return null;

            return new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Category = menuItem.Category,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Available = menuItem.Available
            };
        }

        // CREATE NEW MENU ITEM
        public async Task<MenuItemDto> CreateMenuItemAsync(MenuItemDto menuItemDto)
        {
            var newMenuItem = new MenuItem
            {
                Name = menuItemDto.Name,
                Category = menuItemDto.Category,
                Description = menuItemDto.Description,
                Price = menuItemDto.Price,
                Available = menuItemDto.Available
            };

            _context.MenuItems.Add(newMenuItem);
            await _context.SaveChangesAsync();

            return new MenuItemDto
            {
                Id = newMenuItem.Id,
                Name = newMenuItem.Name,
                Category = newMenuItem.Category,
                Description = newMenuItem.Description,
                Price = newMenuItem.Price,
                Available = newMenuItem.Available
            };
        }

        // UPDATE MENUITEM
        public async Task<bool> UpdateMenuItemAsync(int id, MenuItemDto menuItemDto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return false;

            menuItem.Name = menuItemDto.Name;
            menuItem.Category = menuItemDto.Category;
            menuItem.Description = menuItemDto.Description;
            menuItem.Price = menuItemDto.Price;
            menuItem.Available = menuItemDto.Available;

            await _context.SaveChangesAsync();
            return true;
        }

        // UPDATE MENU ITEM STATUS
        public async Task<bool> UpdateMenuItemStatusAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return false;

            menuItem.Available = !menuItem.Available;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE MENUITEM
        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return false;

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
