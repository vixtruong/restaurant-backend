using Restaurant.OrderManagementService.Interfaces;
using Restaurant.OrderManagementService.Repository;

namespace Restaurant.OrderManagementService.Workers
{
    public class OrderTimeoutHandler : BackgroundService
    {
        private readonly ILogger<OrderTimeoutHandler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderTimeoutHandler(ILogger<OrderTimeoutHandler> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{GetType().Name} {{0}}", "running");

            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            _logger.LogInformation($"{GetType().Name} {{0}}", "stopped");
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            await orderRepo.HandleOrderTimeout();
        }
    }
}
