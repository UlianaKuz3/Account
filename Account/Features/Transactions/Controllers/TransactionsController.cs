using AccountServices.Features.Transactions.RegisterTransaction;
using AccountServices.Features.Transactions.TransferTransaction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountServices.Features.Transactions.Controllers
{
    /// <summary>
    /// Транзакции
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Регистрирует новую транзакцию (поступление или списание) по счёту.
        /// </summary>
        /// <param name="dto">
        /// Данные для регистрации транзакции: 
        /// идентификатор счёта, счёт контрагента, сумма, валюта, тип (списание/поступление) и описание.
        /// </param>
        /// <returns>
        /// Возвращает зарегистрированную транзакцию.
        /// </returns>
        /// <response code="200">Успешно создана транзакция</response>
        /// <response code="400">Ошибки валидации данных</response>
        [HttpPost]
        public async Task<IActionResult> RegisterTransaction([FromBody] RegisterTransactionDto dto)
        {
            try
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
            catch (ClientBlockedException ex)
            {
                return Conflict(new { message = ex.Message });
            }

        }

        /// <summary>
        /// Выполняет перевод средств между двумя счетами.
        /// </summary>
        /// <param name="dto">
        /// Данные перевода: 
        /// счёт отправителя, счёт получателя, сумма, валюта и назначение.
        /// </param>
        /// <returns>
        /// Возвращает результат перевода с деталями по дебету и кредиту.
        /// </returns>
        /// <response code="200">Перевод успешно выполнен</response>
        /// <response code="400">Ошибки валидации данных</response>
        /// <response code="404">Один из счетов не найден</response>
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            try
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
            catch (ConcurrencyException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
            

        }
    }
}

