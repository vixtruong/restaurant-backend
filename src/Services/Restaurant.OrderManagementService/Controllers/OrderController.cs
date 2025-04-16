using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.OrderManagementService.Interfaces;

namespace Restaurant.OrderManagementService.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            return Ok(orders);
        }

        [Authorize]
        [HttpGet("number-orders-month")]
        public async Task<IActionResult> GetNumberOfOrdersInMonth()
        {
            var numberOfOrders = await _orderRepository.GetAllNumberOrdersInMonthAsync();

            return Ok(numberOfOrders);
        }

        [Authorize]
        [HttpGet("number-orders-today")]
        public async Task<IActionResult> GetNumberOfOrdersToday()
        {
            var numberOfOrders = await _orderRepository.GetAllNumberOrdersTodayAsync();

            return Ok(numberOfOrders);
        }

        [Authorize]
        [HttpGet("detail/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            var orderDetail = await _orderRepository.GetOrderDetailAsync(orderId);

            if (orderDetail == null) return NotFound(new { message = "Order not found." });

            return Ok(orderDetail);
        }

        [Authorize]
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

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return NotFound(new { message = "Order not found" });

            return Ok(order);
        }

        [Authorize]
        [HttpPut("end/{orderId}")]
        public async Task<IActionResult> EndOrder(int orderId)
        {
            var result = await _orderRepository.UpdateOrderToEndAsync(orderId);
            if (!result) return NotFound(new { message = "Order not found" });

            return NoContent();
        }

        [HttpPut("handle-empty/{orderId}")]
        public async Task<IActionResult> HandleEmptyOrder(int orderId)
        {
            var result = await _orderRepository.HandleEmptyOrderAsync(orderId);
            if (!result) return BadRequest(new { message = "Order not empty." });

            return NoContent();
        }
    }
}
