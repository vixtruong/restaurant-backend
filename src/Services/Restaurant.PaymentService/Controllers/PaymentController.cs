using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.PaymentService.DTOs;
using Restaurant.PaymentService.Interfaces;

namespace Restaurant.PaymentService.Controllers
{
    [Route("api/v1/payments")]
    [Authorize]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentRepository.GetAllPaymentsAsync();

            return Ok(payments);
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetPaymentsToday()
        {
            var payments = await _paymentRepository.GetPaymentsToday();

            return Ok(payments);
        }

        [HttpGet("month-year")]
        public async Task<IActionResult> GetPaymentsByMonthYear(int month, int year)
        {
            var data = new MonthYearDto { Month = month, Year = year };
            var payments = await _paymentRepository.GetPaymentsByMonthYear(data);
            return Ok(payments);
        }

        [HttpGet("from-to")]
        public async Task<IActionResult> GetPaymentsFromTo(DateTime from, DateTime to)
        {
            var data = new FromToDto
            {
                From = from,
                To = to
            };

            var payments = await _paymentRepository.GetPaymentFromTo(data);

            return Ok(payments);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto? payment)
        {
            if (payment == null) return BadRequest(new { message = "Invalid data" });

            var newPayment = await _paymentRepository.AddPaymentAsync(payment);
            return Ok(newPayment);
        }

        [HttpPut("update-status-to-cancel/{id}")]
        public async Task<IActionResult> UpdateCancelStatus(int id)
        {
            var updated = await _paymentRepository.UpdatePaymentStatusCancelAsync(id);

            if (!updated) return NotFound(new { message = "Payment not found" });

            return NoContent();
        }
    }
}
