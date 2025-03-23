using Microsoft.AspNetCore.Mvc;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Controllers
{
    [Route("api/kitchen-orders")]
    [ApiController]
    public class KitchenOrderController : ControllerBase
    {
        private readonly IKitchenOrderRepository _kitchenOrderRepository;

        public KitchenOrderController(IKitchenOrderRepository kitchenOrderRepository)
        {
            _kitchenOrderRepository = kitchenOrderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllKitchenOrders()
        {
            var kitchenOrders = await _kitchenOrderRepository.GetAllKitchenOrdersAsync();

            return Ok(kitchenOrders);
        }

        [HttpPut("update-to-cooking/{id}")]
        public async Task<IActionResult> UpdateKitchenOrderToCooking(int id)
        {
            var updated = await _kitchenOrderRepository.UpdateKitchenOrderToCookingAsync(id);

            if (!updated) return NotFound(new { message = "Kitchen order not found" });

            return NoContent();
        }

        [HttpPut("update-to-ready/{id}")]
        public async Task<IActionResult> UpdateKitchenOrderToReady(int id)
        {
            var updated = await _kitchenOrderRepository.UpdateKitchenOrderToReadyAsync(id);

            if (!updated) return NotFound(new { message = "Kitchen order not found" });

            return NoContent();
        }
    }
}
