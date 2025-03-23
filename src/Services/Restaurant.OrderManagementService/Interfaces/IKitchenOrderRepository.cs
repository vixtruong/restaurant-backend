using Restaurant.OrderManagementService.DTOs;

namespace Restaurant.OrderManagementService.Interfaces
{
    public interface IKitchenOrderRepository
    {
        Task<IEnumerable<KitchenOrderDto>> GetAllKitchenOrdersAsync();
        Task<KitchenOrderDto> CreateKitchenOrderAsync(KitchenOrderDto kitchenOrder);
        Task<bool> UpdateKitchenOrderToCookingAsync(int id);
        Task<bool> UpdateKitchenOrderToReadyAsync(int id);
        Task<bool> DeleteKitchenOrderAsync(int id);
    }
}
