using Account.Features.Accounts.CreateAccount;
using Account.Features.Accounts.DeleteAccount;
using Account.Features.Accounts.GetAccountById;
using Account.Features.Accounts.GetAllAccounts;
using Account.Features.Accounts.HasAccount;
using Account.Features.Accounts.UpdateAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.Features.Accounts.Controllers
{
    /// <summary>
    /// Банковский счёт.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Получить список всех счетов.
        /// </summary>
        /// <returns>Список всех счетов.</returns>
        /// <response code="200">Возвращает список счетов</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAll()
        {
            var accounts = await mediator.Send(new GetAllAccountsQuery());
            return Ok(accounts);
        }

        /// <summary>
        /// Получить счёт по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор счёта.</param>
        /// <returns>Счёт или 404 если не найден.</returns>
        /// <response code="200">Возвращает счёт</response>
        /// <response code="404">Счёт не найден</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var account = await mediator.Send(new GetAccountByIdQuery(id));
            return Ok(account);
        }

        /// <summary>
        /// Создать новый счёт.
        /// </summary>
        /// <param name="command">Данные для создания счёта.</param>
        /// <returns>Созданный счёт.</returns>
        /// <response code="201">Счёт успешно создан</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountCommand command)
        {
            var account = await mediator.Send(command);
            var locationUri = $"{Request.Host}/api/Accounts/{account.Id}";

            return Created(locationUri, account);
        }

        /// <summary>
        /// Обновить данные счёта.
        /// </summary>
        /// <param name="id">Идентификатор счёта.</param>
        /// <param name="command">Новые данные для счёта.</param>
        /// <returns>Статус выполнения операции.</returns>
        /// <response code="204">Счёт успешно обновлён</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="404">Счёт не найден</response>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountCommand command)
        {
            var updated = command with { Id = id };
            await mediator.Send(updated);

            return NoContent();
        }

        /// <summary>
        /// Удалить счёт.
        /// </summary>
        /// <param name="id">Идентификатор счёта.</param>
        /// <returns>Статус выполнения операции.</returns>
        /// <response code="204">Счёт успешно удалён</response>
        /// <response code="404">Счёт не найден</response>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await mediator.Send(new DeleteAccountCommand(id));
            return NoContent();
        }

        /// <summary>
        /// Проверить, есть ли счёт у указанного владельца.
        /// </summary>
        /// <param name="ownerId">Идентификатор владельца.</param>
        /// <returns>True, если счёт есть, иначе false.</returns>
        /// <response code="200">Возвращает результат проверки</response>
        [HttpGet("check/{ownerId:guid}")]
        public async Task<IActionResult> HasAccount(Guid ownerId)
        {
            var result = await mediator.Send(new HasAccountQuery(ownerId));
            return Ok(result);
        }
    }
}
