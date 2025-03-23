using Microsoft.EntityFrameworkCore;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;
using Restaurant.Shared.Data;
using Restaurant.Shared.Models;

namespace Restaurant.OrderManagementService.Data
{
    public class KitchenOrderRepository : IKitchenOrderRepository
    {
        private readonly RestaurantDbContext _context;

        public KitchenOrderRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KitchenOrderDto>> GetAllKitchenOrdersAsync()
        {
            return await _context.KitchenOrders.Include(o => o.OrderItem)
                .Select(o => new KitchenOrderDto
                {
                    Id =  o.Id,
                    OrderItemId = o.OrderItemId,
                    Status = o.Status,
                    CookAt = o.CookedAt,
                    MenuItemId = o.OrderItem.MenuItemId,
                }).ToListAsync();
        }

        public async Task<KitchenOrderDto> CreateKitchenOrderAsync(KitchenOrderDto kitchenOrder)
        {
            var newKitchenOrder = new KitchenOrder
            {
                OrderItemId = kitchenOrder.OrderItemId,
                Status = "Pending",
            };

            await _context.KitchenOrders.AddAsync(newKitchenOrder);
            await _context.SaveChangesAsync();

            return new KitchenOrderDto
            {
                OrderItemId = newKitchenOrder.OrderItemId,
                Status = newKitchenOrder.Status,
            };
        }

        public async Task<bool> UpdateKitchenOrderToCookingAsync(int id)
        {
            var kitchenOrder = await _context.KitchenOrders.FindAsync(id);

            if (kitchenOrder == null) return false;

            kitchenOrder.Status = "Cooking";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateKitchenOrderToReadyAsync(int id)
        {
            var kitchenOrder = await _context.KitchenOrders.FindAsync(id);

            if (kitchenOrder == null) return false;

            kitchenOrder.Status = "Ready";
            kitchenOrder.CookedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteKitchenOrderAsync(int id)
        {
            var kitchenOrder = await _context.KitchenOrders.FindAsync(id);

            if (kitchenOrder == null) return false;

            _context.KitchenOrders.Remove(kitchenOrder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
