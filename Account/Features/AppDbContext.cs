using AccountServices.Features.Accounts;
using AccountServices.Features.Transactions;
using Microsoft.EntityFrameworkCore;

namespace AccountServices.Features
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
    }
}
