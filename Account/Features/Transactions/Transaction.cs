using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServices.Features.Transactions
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public Guid? CounterpartyAccountId { get; set; }

        public decimal Amount { get; set; }

        [Column(TypeName = "char(3)")]
        public string Currency { get; set; } = "RUB";

        public TransactionType Type { get; set; }

        [Column(TypeName = "char(100)")]
        public string Description { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }
    }
}
