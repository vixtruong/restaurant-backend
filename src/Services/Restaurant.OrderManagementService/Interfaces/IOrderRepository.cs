using Restaurant.OrderManagementService.DTOs;

namespace Restaurant.OrderManagementService.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderDetailDto?> GetOrderDetailAsync(int orderId);
        Task<OrderDto> CreateOrderAsync(OrderRequestDto orderRequestDto);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<int> GetAllNumberOrdersInMonthAsync();
        Task<int> GetAllNumberOrdersTodayAsync();
        Task<bool> UpdateOrderToEndAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> IsAvailableTableAsync(int tableNumber);
        Task<bool> HandleEmptyOrderAsync(int orderId);
        Task<bool> PaymentRequestAsync(int orderId);
    }
}
