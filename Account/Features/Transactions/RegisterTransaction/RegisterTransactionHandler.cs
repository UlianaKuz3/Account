using AccountServices.Features.Accounts;
using MediatR;

namespace AccountServices.Features.Transactions.RegisterTransaction
{
    public class RegisterTransactionHandler(IAccountRepository repository) : IRequestHandler<RegisterTransactionCommand, Transaction>
    {

        public Task<Transaction> Handle(RegisterTransactionCommand request, CancellationToken cancellationToken)
        {
            var account = repository.GetById(request.AccountId);
            if (account == null)
                throw new KeyNotFoundException($"Account {request.AccountId} not found");

            if (!string.Equals(account.Currency, request.Currency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Currency mismatch");

            if (request.Type == TransactionType.Credit)
            {
                account.Balance += request.Amount;
            }
            else if (request.Type == TransactionType.Debit)
            {
                if (account.Balance < request.Amount)
                    throw new InvalidOperationException("Insufficient funds");

                account.Balance -= request.Amount;
            }
            else
            {
                throw new InvalidOperationException("Invalid transaction type");
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = request.AccountId,
                CounterpartyAccountId = request.CounterpartyAccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                Type = request.Type,
                Description = request.Description,
                Timestamp = DateTime.UtcNow
            };

            account.Transactions.Add(transaction);
            repository.Update(account);

            return Task.FromResult(transaction);
        }
    }
}
