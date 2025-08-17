using AccountServices.Features.Accounts;
using AccountServices.Features.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace AccountServices.Features.Transactions.TransferTransaction
{
    public class TransferTransactionHandler : IRequestHandler<TransferTransactionCommand, (Transaction Debit, Transaction Credit)>
    {
        private readonly AppDbContext _dbContext;

        public TransferTransactionHandler(IAccountRepository repository, AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(Transaction Debit, Transaction Credit)> Handle(TransferTransactionCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

            try
            {
                var fromAccount = await _dbContext.Accounts
                    .Include(x => x.Transactions)
                    .FirstOrDefaultAsync(a => a.Id == request.FromAccountId, cancellationToken);

                var toAccount = await _dbContext.Accounts
                    .Include(x => x.Transactions)
                    .FirstOrDefaultAsync(a => a.Id == request.ToAccountId, cancellationToken);

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

                var evt = new
                {
                    FromId = fromAccount.Id,
                    ToId = toAccount.Id,
                    Amount = request.Amount
                };

                var outbox = new OutboxMessage
                {
                    Type = "MoneyTransferred",
                    RoutingKey = "money.transferred",
                    Payload = JsonSerializer.Serialize(evt)
                };

                _dbContext.Outbox.Add(outbox);

                await _dbContext.SaveChangesAsync(cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return (debitTransaction, creditTransaction);
            }
            catch (InvalidOperationException ex) 
                when (ex.InnerException is DbUpdateException
                      {
                          InnerException: PostgresException { SqlState: "40001" }
                      })
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ConcurrencyException("Conflict detected. Please try again.");
            }
            catch (Exception)
            {
                
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
    

}
