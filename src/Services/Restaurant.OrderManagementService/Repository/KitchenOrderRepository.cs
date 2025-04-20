using Microsoft.EntityFrameworkCore;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;
using Restaurant.Shared.Data;
using Restaurant.Shared.Models;

namespace Restaurant.OrderManagementService.Repository
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
            return await _context.KitchenOrders
                .Include(o => o.OrderItem)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItem)
                .ThenInclude(oi => oi.Order)
                .Select(o => new KitchenOrderDto
                {
                    Id = o.Id,
                    OrderItemId = o.OrderItemId,
                    tableNumber = o.OrderItem.Order.TableNumber,
                    Status = o.Status,
                    Quantity = o.OrderItem.Quantity,
                    CookAt = o.CookedAt,
                    Done = o.Done ?? false,
                    MenuItem = new MenuItemDto
                    {
                        Id = o.OrderItem.MenuItem.Id,
                        Name = o.OrderItem.MenuItem.Name,
                        Description = o.OrderItem.MenuItem.Description,
                        Category = o.OrderItem.MenuItem.Category,
                        Price = o.OrderItem.MenuItem.Price,
                        ImgUrl = o.OrderItem.MenuItem.ImgUrl
                    }
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<KitchenOrderDto>> GetKitchenOrdersTodayAsync()
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.MenuItem) // 👈 THÊM Include MenuItem
                .Where(oi => oi.Order.CreatedAt.Value.Date == DateTime.Today)
                .ToListAsync();

            var orderItemIds = orderItems.Select(oi => oi.Id).ToList();

            var kitchenOrders = await _context.KitchenOrders
                .Include(o => o.OrderItem)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => orderItemIds.Contains(o.OrderItemId ?? 0))
                .Select(o => new KitchenOrderDto
                {
                    Id = o.Id,
                    OrderItemId = o.OrderItemId,
                    tableNumber = o.OrderItem.Order.TableNumber,
                    Status = o.Status,
                    Quantity = o.OrderItem.Quantity,
                    Notes = o.OrderItem.Notes,
                    CookAt = o.CookedAt,
                    Done = o.Done ?? false,
                    MenuItem = new MenuItemDto
                    {
                        Id = o.OrderItem.MenuItem.Id,
                        Name = o.OrderItem.MenuItem.Name,
                        Description = o.OrderItem.MenuItem.Description,
                        Category = o.OrderItem.MenuItem.Category,
                        Price = o.OrderItem.MenuItem.Price,
                        ImgUrl = o.OrderItem.MenuItem.ImgUrl
                    }
                })
                .ToListAsync();

            return kitchenOrders;
        }


        public async Task<IEnumerable<KitchenOrderDto>> GetKitchenOrdersByOrderIdAsync(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.CreatedAt.Value.Date == DateTime.Today && oi.OrderId == orderId)
                .ToListAsync();

            var orderItemIds = orderItems.Select(oi => oi.Id).ToList();

            var kitchenOrders = await _context.KitchenOrders
                .Include(o => o.OrderItem)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => orderItemIds.Contains(o.OrderItemId ?? 0))
                .Select(o => new KitchenOrderDto
                {
                    Id = o.Id,
                    OrderItemId = o.OrderItemId,
                    tableNumber = o.OrderItem.Order.TableNumber,
                    Status = o.Status,
                    Quantity = o.OrderItem.Quantity,
                    CookAt = o.CookedAt,
                    Done = o.Done ?? false,
                    MenuItem = new MenuItemDto
                    {
                        Id = o.OrderItem.MenuItem.Id,
                        Name = o.OrderItem.MenuItem.Name,
                        Description = o.OrderItem.MenuItem.Description,
                        Category = o.OrderItem.MenuItem.Category,
                        Price = o.OrderItem.MenuItem.Price,
                        ImgUrl = o.OrderItem.MenuItem.ImgUrl
                    }
                })
                .ToListAsync();

            return kitchenOrders;
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
            kitchenOrder.CookedAt = DateTime.UtcNow.AddHours(7);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateKitchenOrderDoneAsync(int id)
        {
            var kitchenOrder = await _context.KitchenOrders.FindAsync(id);

            if (kitchenOrder == null) return false;

            kitchenOrder.Done = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteKitchenOrderAsync(int id)
        {
            var kitchenOrder = await _context.KitchenOrders.FindAsync(id);

            if (kitchenOrder == null) return false;

            var orderItem = await _context.OrderItems.FindAsync(kitchenOrder.OrderItemId);
            var order = await _context.Orders.FindAsync(orderItem?.OrderId);

            order!.TotalPrice -= orderItem?.Price;
            _context.KitchenOrders.Remove(kitchenOrder);
            _context.OrderItems.Remove(orderItem!);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
