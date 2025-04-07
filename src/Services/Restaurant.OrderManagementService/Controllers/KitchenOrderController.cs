using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("today")]
        public async Task<IActionResult> GetKitchenOrdersToday()
        {
            var kitchenOrders = await _kitchenOrderRepository.GetKitchenOrdersTodayAsync();

            return Ok(kitchenOrders);
        }

        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetKitchenOrdersByOrderId(int orderId)
        {
            var kitchenOrders = await _kitchenOrderRepository.GetKitchenOrdersByOrderIdAsync(orderId);

            return Ok(kitchenOrders);
        }

        [Authorize]
        [HttpPut("update-to-cooking/{id}")]
        public async Task<IActionResult> UpdateKitchenOrderToCooking(int id)
        {
            var updated = await _kitchenOrderRepository.UpdateKitchenOrderToCookingAsync(id);

            if (!updated) return NotFound(new { message = "Kitchen order not found" });

            return NoContent();
        }

        [Authorize]
        [HttpPut("update-to-ready/{id}")]
        public async Task<IActionResult> UpdateKitchenOrderToReady(int id)
        {
            var updated = await _kitchenOrderRepository.UpdateKitchenOrderToReadyAsync(id);

            if (!updated) return NotFound(new { message = "Kitchen order not found" });

            return NoContent();
        }

        [Authorize]
        [HttpPut("update-to-done/{id}")]
        public async Task<IActionResult> UpdateKitchenOrderDone(int id)
        {
            var updated = await _kitchenOrderRepository.UpdateKitchenOrderDoneAsync(id);

            if (!updated) return NotFound(new { message = "Kitchen order not found" });

            return NoContent();
        }

        [Authorize]
        [HttpPut("delete/{id}")]
        public async Task<IActionResult> DeleteKitchenOrder(int id)
        {
            var deleted = await _kitchenOrderRepository.DeleteKitchenOrderAsync(id);

            if (!deleted) return NotFound(new { message = "KitchenOrder not found" });

            return NoContent();
        }
    }
}
