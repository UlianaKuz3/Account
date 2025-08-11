using AccountServices.Features.Accounts;
using AccountServices.Features.Transactions;
using Microsoft.EntityFrameworkCore;

namespace AccountServices.Features
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(a => a.OwnerId).HasMethod("hash");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(t => new { t.AccountId, t.Timestamp });
                entity.HasIndex(t => t.Timestamp).HasMethod("brin");
            });
        }
    }
}
