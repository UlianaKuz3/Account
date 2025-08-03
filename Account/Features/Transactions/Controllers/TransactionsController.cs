using AccountService.Features.Transactions.RegisterTransaction;
using AccountService.Features.Transactions.TransferTransaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController(IMediator mediator) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> RegisterTransaction([FromBody] RegisterTransactionDto dto)
        {
            var command = new RegisterTransactionCommand(
                dto.AccountId,
                dto.CounterpartyAccountId,
                dto.Amount,
                dto.Currency,
                dto.Type,
                dto.Description
            );

            var transaction = await mediator.Send(command);
            return Ok(transaction);

        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            var command = new TransferTransactionCommand(
                dto.FromAccountId,
                dto.ToAccountId,
                dto.Amount,
                dto.Currency,
                dto.Description
            );

            var result = await mediator.Send(command);
            return Ok(new { result.Debit, result.Credit });

        }
    }
}

