using Restaurant.PaymentService.DTOs;

namespace Restaurant.PaymentService.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
        Task<IEnumerable<PaymentDto>> GetPaymentsToday();
        Task<IEnumerable<PaymentDto>> GetPaymentsByMonthYear(MonthYearDto data);
        Task<IEnumerable<PaymentDto>> GetPaymentFromTo(FromToDto data);
        Task<PaymentDto> AddPaymentAsync(PaymentDto payment);
        Task<bool> UpdatePaymentStatusCancelAsync(int id);
        Task<bool> UpdatePaymentStatusPaidAsync(int id);
        Task<bool> DeletePaymentAsync(int id);
    }
}