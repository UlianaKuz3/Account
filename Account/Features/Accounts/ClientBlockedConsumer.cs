using Microsoft.EntityFrameworkCore;

namespace AccountServices.Features.Accounts
{
    public class ClientBlockedConsumer
    {
        private readonly AppDbContext _db;

        public ClientBlockedConsumer(AppDbContext db)
        {
            _db = db;
        }

        public async Task Handle(ClientBlocked evt)
        {
            var accounts = await _db.Accounts
                .Where(a => a.OwnerId == evt.ClientId)
                .ToListAsync();

            foreach (var acc in accounts)
            {
                acc.IsBlocked = true;
            }

            await _db.SaveChangesAsync();
        }
    }

}
