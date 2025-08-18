using AccountServices.Features.Entities;
using AccountServices.Features.Examples;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace AccountServices.Features.Events
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        /// <summary>Контракт и пример события AccountOpened.</summary>
        [HttpGet("account-opened")]
        [SwaggerOperation(Tags = new[] { "Events" }, Summary = "AccountOpened sample")]
        [ProducesResponseType(typeof(EventEnvelope<AccountOpened>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AccountOpenedExample))]
        public ActionResult<EventEnvelope<AccountOpened>> GetAccountOpenedSample()
            => Ok(new AccountOpenedExample().GetExamples());

        /// <summary>Контракт и пример события MoneyCredited.</summary>
        [HttpGet("money-credited")]
        [SwaggerOperation(Tags = new[] { "Events" }, Summary = "MoneyCredited sample")]
        [ProducesResponseType(typeof(EventEnvelope<MoneyCredited>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MoneyCreditedExample))]
        public ActionResult<EventEnvelope<MoneyCredited>> GetMoneyCreditedSample()
            => Ok(new MoneyCreditedExample().GetExamples());

        /// <summary>Контракт и пример события MoneyDebited.</summary>
        [HttpGet("money-debited")]
        [SwaggerOperation(Tags = new[] { "Events" }, Summary = "MoneyDebited sample")]
        [ProducesResponseType(typeof(EventEnvelope<MoneyDebited>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MoneyDebitedExample))]
        public ActionResult<EventEnvelope<MoneyDebited>> GetMoneyDebitedSample()
            => Ok(new MoneyDebitedExample().GetExamples());

        /// <summary>Контракт и пример события TransferCompleted.</summary>
        [HttpGet("transfer-completed")]
        [SwaggerOperation(Tags = new[] { "Events" }, Summary = "TransferCompleted sample")]
        [ProducesResponseType(typeof(EventEnvelope<TransferCompleted>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TransferCompletedExample))]
        public ActionResult<EventEnvelope<TransferCompleted>> GetTransferCompletedSample()
            => Ok(new TransferCompletedExample().GetExamples());
    }
}
