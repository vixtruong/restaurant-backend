using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Controllers
{
    [Route("api/orders")]
    [Authorize]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto? request)
        {
            if (request == null || request?.Items == null || request.Items.Count == 0)
            {
                return BadRequest(new { message = "Invalid order data" });
            }

            var createdOrder = await _orderRepository.CreateOrderAsync(request);

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return NotFound(new { message = "Order not found" });

            return Ok(order);
        }
    }
}
