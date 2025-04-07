using Restaurant.OrderManagementService.DTOs;

namespace Restaurant.OrderManagementService.Interfaces
{
    public interface IKitchenOrderRepository
    {
        Task<IEnumerable<KitchenOrderDto>> GetAllKitchenOrdersAsync();
        Task<IEnumerable<KitchenOrderDto>> GetKitchenOrdersTodayAsync();
        Task<IEnumerable<KitchenOrderDto>> GetKitchenOrdersByOrderIdAsync(int orderId);
        Task<KitchenOrderDto> CreateKitchenOrderAsync(KitchenOrderDto kitchenOrder);
        Task<bool> UpdateKitchenOrderToCookingAsync(int id);
        Task<bool> UpdateKitchenOrderToReadyAsync(int id);
        Task<bool> UpdateKitchenOrderDoneAsync(int id);
        Task<bool> DeleteKitchenOrderAsync(int id);
    }
}
