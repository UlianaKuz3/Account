using AccountService.Features.Accounts;
using AccountService.Features.Accounts.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private static List<Account> Accounts => AccountsController.Accounts;

        [HttpPost]
        public IActionResult RegisterTransaction([FromBody] RegisterTransactionDto dto)
        {
            var account = Accounts.FirstOrDefault(a => a.Id == dto.AccountId);
            if (account == null)
                return NotFound($"Account {dto.AccountId} not found");

            if (!string.Equals(account.Currency, dto.Currency, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Currency mismatch");

            if (dto.Type.ToString().Equals("Credit", StringComparison.OrdinalIgnoreCase))
                account.Balance += dto.Amount;
            else if (dto.Type.ToString().Equals("Debit", StringComparison.OrdinalIgnoreCase))
            {
                if (account.Balance < dto.Amount)
                    return BadRequest("Insufficient funds");
                account.Balance -= dto.Amount;
            }
            else
                return BadRequest("Invalid transaction type");

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = dto.AccountId,
                CounterpartyAccountId = dto.CounterpartyAccountId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Type = dto.Type,
                Description = dto.Description,
                Timestamp = DateTime.UtcNow
            };

            account.Transactions.Add(transaction);

            return Ok(transaction);
        }


        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferRequest request)
        {
            var fromAccount = Accounts.FirstOrDefault(a => a.Id == request.FromAccountId);
            var toAccount = Accounts.FirstOrDefault(a => a.Id == request.ToAccountId);

            if (fromAccount == null || toAccount == null)
                return NotFound("One of the accounts not found");

            if (!string.Equals(fromAccount.Currency, toAccount.Currency, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Currency mismatch between accounts");

            if (!string.Equals(fromAccount.Currency, request.Currency, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Transfer currency does not match account currency");

            if (fromAccount.Balance < request.Amount)
                return BadRequest("Insufficient funds");

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

            return Ok(new
            {
                Debit = debitTransaction,
                Credit = creditTransaction
            });
        }
    }
}

