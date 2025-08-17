using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AccountServices.Features.Transactions.Services
{
    public class InterestAccrualService(IServiceProvider serviceProvider, ILogger<InterestAccrualService> logger)
    {

        public async Task AccrueInterestAsync()
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var accountIds = await dbContext.Accounts
                    .Select(a => a.Id)
                    .ToListAsync();

                foreach (var accountId in accountIds)
                {
                    var param = new NpgsqlParameter("account_id", accountId);
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "CALL accrue_interest(@account_id);", param);
                }

                logger.LogInformation("Interest accrued for {Count} accounts at {Time}", accountIds.Count, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while accruing interest");
            }
        }
    }

}
