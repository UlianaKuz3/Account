using AccountServices.Features.Accounts;
using AccountServices.Features.Entities;
using AccountServices.Features.Transactions;
using Microsoft.EntityFrameworkCore;

namespace AccountServices.Features
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts => Set<Account>();
        // ReSharper disable once UnusedMember.Global Таблица транзакций
        public DbSet<Transaction> Transactions => Set<Transaction>();

        public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
        // ReSharper disable once UnusedMember.Global Таблица 
        public DbSet<InboxConsumed> InboxConsumed => Set<InboxConsumed>();
        // ReSharper disable once UnusedMember.Global Таблица 
        public DbSet<InboxDeadLetter> InboxDeadLetters => Set<InboxDeadLetter>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxMessage>(e =>
            {
                e.ToTable("outbox_messages");
                e.HasKey(x => x.Id);
                e.Property(x => x.Payload).HasColumnType("jsonb");
                e.HasIndex(x => x.PublishedAt);
                e.HasIndex(x => new { x.PublishedAt, x.Attempts }); 
            });

            modelBuilder.Entity<InboxConsumed>(e =>
            {
                e.ToTable("inbox_consumed");
                e.HasKey(x => x.MessageId);
                e.Property(x => x.Handler).HasMaxLength(200);
            });

            modelBuilder.Entity<InboxDeadLetter>(e =>
            {
                e.ToTable("inbox_dead_letters");
                e.HasKey(x => x.MessageId);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(a => a.OwnerId).HasMethod("hash");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(t => new { t.AccountId, t.Timestamp });
                entity.HasIndex(t => t.Timestamp).HasMethod("brin");
            });


            // ReSharper disable once StringLiteralTypo Намеренное написание
            modelBuilder.Entity<Account>()
                .Property<uint>("xmin")
                .IsRowVersion()
                // ReSharper disable once StringLiteralTypo Намеренное написание
                .HasColumnName("xmin")
                .IsConcurrencyToken();
        }
    }
}
