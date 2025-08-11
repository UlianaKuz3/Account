using AccountServices.Features.Accounts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AccountServices.Features.Transactions.TransferTransaction
{
    public class TransferTransactionHandler : IRequestHandler<TransferTransactionCommand, (Transaction Debit, Transaction Credit)>
    {
        private readonly IAccountRepository _repository;
        private readonly AppDbContext _dbContext;

        public TransferTransactionHandler(IAccountRepository repository, AppDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public async Task<(Transaction Debit, Transaction Credit)> Handle(TransferTransactionCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable,
                cancellationToken);

            try
            {
                var fromAccount = await _dbContext.Accounts
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == request.FromAccountId, cancellationToken); ;

                var toAccount = await _dbContext.Accounts
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == request.ToAccountId, cancellationToken); ;

                if (fromAccount == null || toAccount == null)
                    throw new KeyNotFoundException("One of the accounts not found");

                if (!string.Equals(fromAccount.Currency, toAccount.Currency, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Currency mismatch between accounts");

                if (!string.Equals(fromAccount.Currency, request.Currency, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Transfer currency does not match account currency");

                if (fromAccount.Balance < request.Amount)
                    throw new InvalidOperationException("Insufficient funds");

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

                fromAccount.Balance -= request.Amount;
                toAccount.Balance += request.Amount;

                fromAccount.Transactions.Add(debitTransaction);
                toAccount.Transactions.Add(creditTransaction);

                if (fromAccount.Balance < 0 || toAccount.Balance < 0)
                    throw new InvalidOperationException("Balance validation failed after transfer");

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return (debitTransaction, creditTransaction);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
    

}
