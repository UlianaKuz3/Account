using AccountServices.Features.Transactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServices.Features.Accounts
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

        public List<Transaction> Transactions { get; set; } = [];

        [ConcurrencyCheck]
        [Column("xmin")]
        public uint xmin { get; set; }
    }
}
