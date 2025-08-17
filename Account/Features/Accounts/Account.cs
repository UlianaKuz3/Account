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

        [Column(TypeName = "char(3)")]
        public string Currency { get; set; } = "RUB";

        public decimal Balance { get; set; }

        public decimal? InterestRate { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime? CloseDate { get; set; }

        
        public List<Transaction> Transactions { get; set; } = [];

        [ConcurrencyCheck]
        // ReSharper disable once StringLiteralTypo Намеренное написание
        [Column("xmin")]
        // ReSharper disable once InconsistentNaming Намеренное написание
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public uint xmin { get; set; }
    }
}
