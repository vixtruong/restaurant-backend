using Microsoft.EntityFrameworkCore;
using Restaurant.OrderManagementService.DTOs;
using Restaurant.OrderManagementService.Interfaces;
using Restaurant.Shared.Core;
using Restaurant.Shared.Data;
using Restaurant.Shared.Models;

namespace Restaurant.OrderManagementService.Repository
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

            var order = await _context.Orders.Include(o => o.Customer).Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();

            var orderDetail = new OrderDetailDto
            {
                OrderId = orderId,
                CustomerId = order?.CustomerId,
                CustomerName = order?.Customer?.FullName,
                TableNumber = order?.TableNumber,
                IsPaid = order?.Status == "Paid" ? true : false,
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

        public async Task<List<TableDto>> GetTables()
        {
            var tables = await _context.Tables.ToListAsync();

            var orders = await _context.Orders
                .Where(o => o.EndAt == null || o.Status == "Unpaid")
                .Include(o => o.Customer)
                .ToListAsync();

            var result = tables.Select(t =>
            {
                var order = orders.FirstOrDefault(o => o.TableNumber == t.Number);

                if (order != null)
                {
                    return new TableDto
                    {
                        Id = t.Id,
                        Number = t.Number,
                        Available = t.Available,
                        BookedBy = order.Customer?.FullName ?? null,
                        OrderId = order.Id
                    };
                }

                return new TableDto
                {
                    Id = t.Id,
                    Number = t.Number,
                    Available = t.Available,
                };
            }).ToList();

            return result;
        }

        //public async Task<OrderDto> CreateOrderAsync(OrderRequestDto request)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        Order newOrder;

        //        if (request.OrderId.HasValue)
        //        {
        //            newOrder = await _context.Orders.FindAsync(request.OrderId.Value)
        //                       ?? throw new Exception($"Order {request.OrderId} not found");
        //        }
        //        else
        //        {
        //            newOrder = new Order
        //            {
        //                CustomerId = request.CustomerId,
        //                TableNumber = request.TableNumber,
        //                Status = "Unpaid",
        //                TotalPrice = 0,
        //                CreatedAt = DateTime.UtcNow
        //            };

        //            _context.Orders.Add(newOrder);
        //            await _context.SaveChangesAsync();
        //        }

        //        if (request?.Items == null)
        //        {
        //            await _context.Entry(newOrder)
        //                .Reference(o => o.Customer)
        //                .LoadAsync();

        //            return new OrderDto
        //            {
        //                Id = newOrder.Id,
        //                CustomerId = newOrder.CustomerId,
        //                CustomerName = newOrder.Customer?.FullName,
        //                Status = newOrder.Status,
        //                TableNumber = newOrder.TableNumber,
        //                TotalPrice = newOrder.TotalPrice,
        //            };
        //        }

        //        #region START ADD ORDERITEMS

        //        var orderItemsList = new List<OrderItem>();
        //        decimal totalPrice = 0;

        //        foreach (var item in request?.Items!)
        //        {
        //            var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
        //            if (menuItem == null)
        //                throw new Exception($"MenuItemId {item.MenuItemId} not found");

        //            var orderItem = new OrderItem
        //            {
        //                OrderId = newOrder.Id,
        //                MenuItemId = item.MenuItemId,
        //                Quantity = item.Quantity,
        //                Price = menuItem.Price * item.Quantity,
        //                Notes = item.Notes
        //            };

        //            totalPrice += orderItem.Price;
        //            orderItemsList.Add(orderItem);
        //        }

        //        _context.OrderItems.AddRange(orderItemsList);
        //        await _context.SaveChangesAsync();

        //        #endregion END ADD ORDERITEMS

        //        #region START ADD KITCHENORDER WHEN ORDERITEMS HAS ID

        //        var kitchenOrders = new List<KitchenOrder>();

        //        foreach (var orderItem in orderItemsList)
        //        {
        //            var kitchenOrder = new KitchenOrder
        //            {
        //                OrderItemId = orderItem.Id,
        //                Status = "Pending",
        //            };
        //            kitchenOrders.Add(kitchenOrder);
        //        }

        //        _context.KitchenOrders.AddRange(kitchenOrders);

        //        #endregion END ADD KITCHENORDER WHEN ORDERITEMS HAS ID

        //        newOrder.TotalPrice = totalPrice;

        //        await _context.SaveChangesAsync();

        //        await _context.Entry(newOrder)
        //            .Reference(o => o.Customer)
        //            .LoadAsync();

        //        await transaction.CommitAsync();

        //        return new OrderDto
        //        {
        //            Id = newOrder.Id,
        //            CustomerId = newOrder.CustomerId,
        //            CustomerName = newOrder.Customer?.FullName,
        //            Status = newOrder.Status,
        //            TableNumber = newOrder.TableNumber,
        //            TotalPrice = newOrder.TotalPrice,
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        await transaction.RollbackAsync();
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

        public async Task<OrderDto> CreateOrderAsync(OrderRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Retrieve or create order
                Order newOrder = request.OrderId.HasValue
                    ? await _context.Orders.FindAsync(request.OrderId.Value)
                      ?? throw new KeyNotFoundException($"Order {request.OrderId} not found")
                    : new Order
                    {
                        CustomerId = request.CustomerId,
                        TableNumber = request.TableNumber ?? 0,
                        Status = "Unpaid",
                        TotalPrice = 0,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        PaymentRequest = false,
                    };

                if (!request.OrderId.HasValue)
                {
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

                    if (request.TableNumber.HasValue)
                    {
                        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Number == request.TableNumber);

                        if (table == null)
                            throw new KeyNotFoundException($"Table {request.TableNumber.Value} not found");

                        var tableHistory = new TableHistory
                        {
                            UserId = request.CustomerId ?? 0,
                            TableId = table.Id,
                            CheckInTime = DateTime.UtcNow.AddHours(7),
                            CheckOutTime = null
                        };

                        table.Available = false;
                        _context.TableHistories.Add(tableHistory);
                        _context.Tables.Update(table);
                        await _context.SaveChangesAsync();
                    }
                }

                // Early return if no items
                if (request.Items == null || !request.Items.Any())
                {
                    await _context.Entry(newOrder).Reference(o => o.Customer).LoadAsync();
                    await transaction.CommitAsync();
                    return MapToOrderDto(newOrder);
                }

                // Fetch all menu items in one query
                var menuItemIds = request.Items.Select(i => i.MenuItemId).ToList();
                var menuItems = await _context.MenuItems
                    .Where(m => menuItemIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, m => m);

                // Create order items
                var orderItemsList = new List<OrderItem>();
                decimal totalPrice = 0;

                foreach (var item in request.Items)
                {
                    if (!menuItems.TryGetValue(item.MenuItemId, out var menuItem))
                        throw new KeyNotFoundException($"MenuItemId {item.MenuItemId} not found");

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

                // Create kitchen orders
                var kitchenOrders = orderItemsList.Select(orderItem => new KitchenOrder
                {
                    OrderItemId = orderItem.Id,
                    Status = "Pending"
                }).ToList();

                _context.KitchenOrders.AddRange(kitchenOrders);

                // Update total price
                newOrder.TotalPrice = totalPrice;
                await _context.SaveChangesAsync();

                await _context.Entry(newOrder).Reference(o => o.Customer).LoadAsync();
                await transaction.CommitAsync();

                return MapToOrderDto(newOrder);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Helper method to map Order to OrderDto
        private OrderDto MapToOrderDto(Order order) => new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.FullName,
            Status = order.Status,
            TableNumber = order.TableNumber,
            TotalPrice = order.TotalPrice,
            PaymentRequest = order.PaymentRequest
        };

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
                    CreatedAt = o.CreatedAt,
                    EndAt = o.EndAt,
                    PaymentRequest = o.PaymentRequest
                }).ToListAsync();
        }

        public async Task<int> GetAllNumberOrdersInMonthAsync()
        {
            var numberOfOrders = await _context.Orders.Where(o => o.CreatedAt.Value.Month == DateTime.Today.Month
                                                                  && o.CreatedAt.Value.Year == DateTime.Today.Year)
                .CountAsync();

            return numberOfOrders;
        }

        public async Task<int> GetAllNumberOrdersTodayAsync()
        {
            var numberOfOrders = await _context.Orders.Where(o => o.CreatedAt.Value.Month == DateTime.Today.Month
                                                                  && o.CreatedAt.Value.Day == DateTime.Today.Day
                                                                  && o.CreatedAt.Value.Year == DateTime.Today.Year
            ).CountAsync();

            return numberOfOrders;
        }

        public async Task<bool> UpdateOrderToEndAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null) return false;

            order.EndAt = DateTime.UtcNow.AddHours(7);

            order.Status = "Paid";

            var kitchenOrders = await _context.KitchenOrders
                .Where(ko => ko.OrderItem!.OrderId == orderId)
                .ToListAsync();

            foreach (var kitchenOrder in kitchenOrders)
            {
                kitchenOrder.Done = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task HandleOrderTimeout()
        {
            var timeoutThreshold = DateTime.UtcNow.AddHours(7).AddHours(-AppConstant.ORDER_TIMEOUT_HOURS);

            var orders = await _context.Orders
                .Where(o => o.EndAt == null && o.CreatedAt < timeoutThreshold)
                .Include(o => o.OrderItems)
                .ToListAsync();

            if (orders.Count <= 0)
                return;

            var tableNumbers = orders
                .Select(o => o.TableNumber)
                .Distinct()
                .ToList();

            var tables = await _context.Tables
                .Where(t => tableNumbers.Contains(t.Number))
                .ToListAsync();

            foreach (var table in tables)
            {
                table.Available = true;
            }

            _context.Orders.RemoveRange(orders);

            await _context.SaveChangesAsync();
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

        public async Task<bool> IsAvailableTableAsync(int tableNumber)
        {
            if (tableNumber <= 0)
                throw new ArgumentException("Table number must be positive.", nameof(tableNumber));

            var table = await _context.Tables.FirstOrDefaultAsync(t => t.Number == tableNumber);

            if (table == null) return false;

            return table.Available;
        }

        public async Task<bool> HandleEmptyOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.TotalPrice == 0)
            {
                var table = await _context.Tables.FirstOrDefaultAsync(t => t.Number == order.TableNumber);

                if (table != null)
                {
                    table.Available = true;

                    var latestHistory = await _context.TableHistories
                        .Where(h => h.TableId == table.Id && h.CheckOutTime == null)
                        .OrderByDescending(h => h.CheckInTime)
                        .FirstOrDefaultAsync();

                    if (latestHistory != null)
                    {
                        latestHistory.CheckOutTime = DateTime.UtcNow.AddHours(7);
                    }
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }


        public async Task<bool> PaymentRequestAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null) return false;

            order.PaymentRequest = true;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
