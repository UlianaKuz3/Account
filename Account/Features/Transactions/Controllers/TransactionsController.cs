using AccountService.Features.Accounts;
using AccountService.Features.Accounts.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController(IAccountRepository accountRepository) : ControllerBase
    {
        private readonly IAccountRepository _accountRepository = accountRepository;

        [HttpPost]
        public IActionResult RegisterTransaction([FromBody] RegisterTransactionDto dto)
        {

            var account = _accountRepository.GetById(dto.AccountId);
            if (account == null)
                return NotFound($"Account {dto.AccountId} not found");

            if (!string.Equals(account.Currency, dto.Currency, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Currency mismatch");

            if (dto.Type == TransactionType.Credit)
            {
                account.Balance += dto.Amount;
            }
            else if (dto.Type == TransactionType.Debit)
            {
                if (account.Balance < dto.Amount)
                    return BadRequest("Insufficient funds");

                account.Balance -= dto.Amount;
            }
            else
            {
                return BadRequest("Invalid transaction type");
            }

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

            _accountRepository.Update(account);

            return Ok(transaction);
        }


        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferDto dto)
        {
            var fromAccount = _accountRepository.GetById(dto.FromAccountId);
            var toAccount = _accountRepository.GetById(dto.ToAccountId);

            if (fromAccount == null || toAccount == null)
                return NotFound("One of the accounts not found");

            if (!string.Equals(fromAccount.Currency, toAccount.Currency, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Currency mismatch between accounts");

            if (!string.Equals(fromAccount.Currency, dto.Currency, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Transfer currency does not match account currency");

            if (fromAccount.Balance < dto.Amount)
                return BadRequest("Insufficient funds");

            fromAccount.Balance -= dto.Amount;
            var debitTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = fromAccount.Id,
                CounterpartyAccountId = toAccount.Id,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Type = TransactionType.Debit,
                Description = dto.Description,
                Timestamp = DateTime.UtcNow
            };
            fromAccount.Transactions.Add(debitTransaction);
            _accountRepository.Update(fromAccount);

            toAccount.Balance += dto.Amount;
            var creditTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = toAccount.Id,
                CounterpartyAccountId = fromAccount.Id,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Type = TransactionType.Credit,
                Description = dto.Description,
                Timestamp = DateTime.UtcNow
            };
            toAccount.Transactions.Add(creditTransaction);
            _accountRepository.Update(toAccount);

            return Ok(new
            {
                Debit = debitTransaction,
                Credit = creditTransaction
            });
        }
    }
}

