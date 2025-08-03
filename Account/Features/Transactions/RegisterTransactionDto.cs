﻿namespace Account.Features.Transactions
{
    public class RegisterTransactionDto
    {
        public Guid AccountId { get; set; }
        public Guid? CounterpartyAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
