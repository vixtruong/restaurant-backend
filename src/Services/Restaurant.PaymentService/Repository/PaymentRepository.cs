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
                .Where(p => p.PaidAt.Value.Day == DateTime.Now.AddHours(7).Day
                    && p.PaidAt.Value.Month == DateTime.Now.AddHours(7).Month
                    && p.PaidAt.Value.Year == DateTime.Now.AddHours(7).Year)
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

            var existPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == payment.OrderId);
            if (existPayment != null)
            {
                return new PaymentDto
                {
                    Id = existPayment.Id,
                    OrderId = existPayment.OrderId,
                    Amount = existPayment.Amount,
                    UserId = existPayment.UserId,
                    UserName = payment.UserName,
                    PaymentMethod = existPayment.PaymentMethod,
                    PaidAt = existPayment.PaidAt,
                    Status = existPayment.Status,
                };
            }

            order.Status = "Paid";
            order.EndAt = DateTime.UtcNow.AddHours(7);

            var table = await _context.Tables
                .FirstOrDefaultAsync(t => t.Number == order.TableNumber);

            if (table != null)
            {
                table.Available = true;
                _context.Tables.Update(table);
            }

            var newPayment = new Payment
            {
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                PaymentMethod = payment.PaymentMethod,
                PaidAt = DateTime.UtcNow.AddHours(7),
                Amount = payment.Amount,
                Status = "Paid",
            };

            await _context.Payments.AddAsync(newPayment);

            await _context.SaveChangesAsync();

            return new PaymentDto
            {
                Id = newPayment.Id,
                OrderId = newPayment.OrderId,
                Amount = newPayment.Amount,
                UserId = newPayment.UserId,
                UserName = payment.UserName,
                PaymentMethod = newPayment.PaymentMethod,
                PaidAt = newPayment.PaidAt,
                Status = newPayment.Status,
            };
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
