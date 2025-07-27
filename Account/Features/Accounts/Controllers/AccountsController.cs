using AccountService.Features.Accounts.CreateAccount;
using AccountService.Features.Accounts.DeleteAccount;
using AccountService.Features.Accounts.GetAccountById;
using AccountService.Features.Accounts.GetAllAccounts;
using AccountService.Features.Accounts.HasAccount;
using AccountService.Features.Accounts.UpdateAccount;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace AccountService.Features.Accounts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAll()
        {
            var accounts = await _mediator.Send(new GetAllAccountsQuery());
            return Ok(accounts);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var account = await _mediator.Send(new GetAccountByIdQuery(id));
            if (account is null) return NotFound();
            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountCommand command)
        {
            var account = await _mediator.Send(command);
            var locationUri = $"{Request.Host}/api/Accounts/{account.Id}";

            return Created(locationUri, account);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountCommand command)
        {
            var updated = command with { Id = id };
            var result = await _mediator.Send(updated);

            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteAccountCommand(id));
            return NoContent();
        }

        [HttpGet("check/{ownerId:guid}")]
        public async Task<IActionResult> HasAccount(Guid ownerId)
        {
            var result = await _mediator.Send(new HasAccountQuery(ownerId));
            return Ok(result);
        }
    }
}
