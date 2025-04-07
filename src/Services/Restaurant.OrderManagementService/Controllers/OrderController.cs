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

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            return Ok(orders);
        }

        [HttpGet("number-orders-month")]
        public async Task<IActionResult> GetNumberOfOrdersInMonth()
        {
            var numberOfOrders = await _orderRepository.GetAllNumberOrdersInMonthAsync();

            return Ok(numberOfOrders);
        }

        [HttpGet("number-orders-today")]
        public async Task<IActionResult> GetNumberOfOrdersToday()
        {
            var numberOfOrders = await _orderRepository.GetAllNumberOrdersTodayAsync();

            return Ok(numberOfOrders);
        }

        [HttpGet("detail/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            var orderDetail = await _orderRepository.GetOrderDetailAsync(orderId);

            if (orderDetail == null) return NotFound(new { message = "Order not found." });

            return Ok(orderDetail);
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
