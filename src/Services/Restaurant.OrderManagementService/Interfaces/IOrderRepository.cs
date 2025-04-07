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
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}
