using Microsoft.EntityFrameworkCore;

namespace AccountServices.Features.Accounts
{
    // ReSharper disable once UnusedMember.Global Используется для блокировки
    public class ClientBlockedConsumer(AppDbContext db)
    {

        // ReSharper disable once UnusedMember.Global
        public async Task Handle(ClientBlocked evt)
        {
            var accounts = await db.Accounts
                .Where(a => a.OwnerId == evt.ClientId)
                .ToListAsync();

            foreach (var acc in accounts)
            {
                acc.IsBlocked = true;
            }

            await db.SaveChangesAsync();
        }
    }

}
