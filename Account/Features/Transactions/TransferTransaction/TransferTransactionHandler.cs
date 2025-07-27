using AccountService.Features.Accounts;
using MediatR;

namespace AccountService.Features.Transactions.TransferTransaction
{
    public class TransferTransactionHandler(IAccountRepository repository) : IRequestHandler<TransferTransactionCommand, 
                                                                        (Transaction Debit, Transaction Credit)>
    {
        private readonly IAccountRepository _repository = repository;

        public Task<(Transaction Debit, Transaction Credit)> Handle(TransferTransactionCommand request, 
                                                                            CancellationToken cancellationToken)
        {
            var fromAccount = _repository.GetById(request.FromAccountId);
            var toAccount = _repository.GetById(request.ToAccountId);

            if (fromAccount == null || toAccount == null)
                throw new KeyNotFoundException("One of the accounts not found");

            if (!string.Equals(fromAccount.Currency, toAccount.Currency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Currency mismatch between accounts");

            if (!string.Equals(fromAccount.Currency, request.Currency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Transfer currency does not match account currency");

            if (fromAccount.Balance < request.Amount)
                throw new InvalidOperationException("Insufficient funds");

            fromAccount.Balance -= request.Amount;
            var debitTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = fromAccount.Id,
                CounterpartyAccountId = toAccount.Id,
                Amount = request.Amount,
                Currency = request.Currency,
                Type = TransactionType.Debit,
                Description = request.Description,
                Timestamp = DateTime.UtcNow
            };
            fromAccount.Transactions.Add(debitTransaction);
            _repository.Update(fromAccount);

            toAccount.Balance += request.Amount;
            var creditTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = toAccount.Id,
                CounterpartyAccountId = fromAccount.Id,
                Amount = request.Amount,
                Currency = request.Currency,
                Type = TransactionType.Credit,
                Description = request.Description,
                Timestamp = DateTime.UtcNow
            };
            toAccount.Transactions.Add(creditTransaction);
            _repository.Update(toAccount);

            return Task.FromResult((debitTransaction, creditTransaction));
        }
    }
}
