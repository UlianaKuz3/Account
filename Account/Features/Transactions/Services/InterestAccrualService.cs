using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AccountServices.Features.Transactions.Services
{
    public class InterestAccrualService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InterestAccrualService> _logger;

        public InterestAccrualService(IServiceProvider serviceProvider, ILogger<InterestAccrualService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var accountIds = await dbContext.Accounts
                        .Select(a => a.Id)
                        .ToListAsync(stoppingToken);

                    foreach (var accountId in accountIds)
                    {
                        var param = new NpgsqlParameter("account_id", accountId);
                        await dbContext.Database.ExecuteSqlRawAsync(
                            "CALL accrue_interest(@account_id);", param);
                    }

                    _logger.LogInformation("Interest accrued for {Count} accounts at {Time}", accountIds.Count, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while accruing interest");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
