using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;
using Restaurant.Shared.Data;
using Restaurant.Shared.Models;

namespace Restaurant.OrderManagementService.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RestaurantDbContext _context;

        public OrderRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDetailDto?> GetOrderDetailAsync(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            if (!orderItems.Any())
                return null;

            var order = await _context.Orders.Include(o => o.Customer).Where(o => o.Id == orderId).FirstOrDefaultAsync();

            var orderDetail = new OrderDetailDto
            {
                OrderId = orderId,
                CustomerId = order?.CustomerId,
                CustomerName = order?.Customer?.FullName,
                TableNumber = order?.TableNumber,
                OrderItems = new List<OrderItemDetailDto>()
            };

            foreach (var item in orderItems)
            {
                orderDetail.OrderItems.Add(new OrderItemDetailDto
                {
                    Id = item.Id,
                    MenuItemName = item.MenuItem?.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.MenuItem?.Price,
                    Price = item.Price
                });
            }

            return orderDetail;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Order newOrder;

                if (request.OrderId.HasValue)
                {
                    newOrder = await _context.Orders.FindAsync(request.OrderId.Value)
                               ?? throw new Exception($"Order {request.OrderId} not found");
                }
                else
                {
                    newOrder = new Order
                    {
                        CustomerId = request.CustomerId,
                        TableNumber = request.TableNumber,
                        Status = "Unpaid",
                        TotalPrice = 0,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();
                }

                #region START ADD ORDERITEMS

                var orderItemsList = new List<OrderItem>();
                decimal totalPrice = 0;

                foreach (var item in request.Items)
                {
                    var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                    if (menuItem == null)
                        throw new Exception($"MenuItemId {item.MenuItemId} not found");

                    var orderItem = new OrderItem
                    {
                        OrderId = newOrder.Id,
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        Price = menuItem.Price * item.Quantity,
                        Notes = item.Notes
                    };

                    totalPrice += orderItem.Price;
                    orderItemsList.Add(orderItem);
                }

                _context.OrderItems.AddRange(orderItemsList);
                await _context.SaveChangesAsync();

                #endregion END ADD ORDERITEMS

                #region START ADD KITCHENORDER WHEN ORDERITEMS HAS ID

                var kitchenOrders = new List<KitchenOrder>();

                foreach (var orderItem in orderItemsList)
                {
                    var kitchenOrder = new KitchenOrder
                    {
                        OrderItemId = orderItem.Id,
                        Status = "Pending",
                    };
                    kitchenOrders.Add(kitchenOrder);
                }

                _context.KitchenOrders.AddRange(kitchenOrders);

                #endregion END ADD KITCHENORDER WHEN ORDERITEMS HAS ID

                newOrder.TotalPrice = totalPrice;

                await _context.SaveChangesAsync();

                await _context.Entry(newOrder)
                    .Reference(o => o.Customer)
                    .LoadAsync();

                await transaction.CommitAsync();

                return new OrderDto
                {
                    Id = newOrder.Id,
                    CustomerId = newOrder.CustomerId,
                    CustomerName = newOrder.Customer?.FullName,
                    Status = newOrder.Status,
                    TableNumber = newOrder.TableNumber,
                    TotalPrice = newOrder.TotalPrice,
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(); // 🔥 Rollback nếu có lỗi
                Console.WriteLine(e);
                throw;
            }
        }

        // GET ORDER BY ID
        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders.Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.FullName,
                TableNumber = order.TableNumber,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
            };
        }

        // GET ALL ORDERS
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.Customer)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.FullName,
                    TableNumber = o.TableNumber,
                    Status = o.Status,
                    TotalPrice = o.TotalPrice,
                }).ToListAsync();
        }

        // UPDATE ORDER
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string? status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;
            if (status == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE ORDER
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
