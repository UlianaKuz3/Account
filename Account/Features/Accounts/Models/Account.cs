using System.Transactions;

namespace Account.Features.Accounts.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public AccountType Type { get; set; }

        public string Currency { get; set; } = "RUB";

        public decimal Balance { get; set; }

        public decimal? InterestRate { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public List<Transaction> Transactions { get; set; } = new();
    }
}
