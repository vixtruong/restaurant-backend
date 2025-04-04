using Microsoft.EntityFrameworkCore;
using Restaurant.PaymentService.DTOs;
using Restaurant.PaymentService.Interfaces;
using Restaurant.Shared.Data;
using Restaurant.Shared.Models;
using System.Data;

namespace Restaurant.PaymentService.Data
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly RestaurantDbContext _context;

        public PaymentRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.User)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    UserId = p.UserId,
                    UserName = p.User!.FullName,
                    PaymentMethod = p.PaymentMethod,
                    PaidAt = p.PaidAt,
                    Amount = p.Amount,
                    Status = p.Status,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsToday()
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.PaidAt == DateTime.Today)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    UserId = p.UserId,
                    UserName = p.User!.FullName,
                    PaymentMethod = p.PaymentMethod,
                    PaidAt = p.PaidAt,
                    Amount = p.Amount,
                    Status = p.Status,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByMonthYear(MonthYearDto data)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.PaidAt != null && p.PaidAt.Value.Month == data.Month && p.PaidAt.Value.Year == data.Year)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    UserId = p.UserId,
                    UserName = p.User!.FullName,
                    PaymentMethod = p.PaymentMethod,
                    PaidAt = p.PaidAt,
                    Amount = p.Amount,
                    Status = p.Status,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentFromTo(FromToDto data)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.PaidAt >= data.From && p.PaidAt <= data.To)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    UserId = p.UserId,
                    UserName = p.User!.FullName,
                    PaymentMethod = p.PaymentMethod,
                    PaidAt = p.PaidAt,
                    Amount = p.Amount,
                    Status = p.Status,
                })
                .ToListAsync();
        }

        public async Task<PaymentDto> AddPaymentAsync(PaymentDto payment)
        {
            var order = await _context.Orders.FindAsync(payment.OrderId);

            if (order == null)
            {
                return new PaymentDto();
            }

            order.Status = "Paid";

            var existPayment = await _context.Payments.Where(p => p.OrderId == payment.OrderId).FirstOrDefaultAsync();
            if (existPayment != null) return payment;

            var newPayment = new Payment
            {
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                PaymentMethod = payment.PaymentMethod,
                PaidAt = payment.PaidAt,
                Amount = payment.Amount,
                Status = "Paid",
            };

            await _context.Payments.AddAsync(newPayment);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<bool> UpdatePaymentStatusCancelAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null) return false;

            payment.Status = "Cancel";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePaymentStatusPaidAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null) return false;

            payment.Status = "Paid";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null) return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
