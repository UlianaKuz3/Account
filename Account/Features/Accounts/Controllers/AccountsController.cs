using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        public static readonly List<Account> Accounts = new();

        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAll()
        {
            return Ok(Accounts);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<Account> GetById(Guid id)
        {
            var account = Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null)
                return NotFound();

            return Ok(account);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Account account)
        {
            account.Id = Guid.NewGuid();
            account.OpenDate = DateTime.UtcNow;
            Accounts.Add(account);

            var locationUri = $"{Request.Host}/api/Accounts/{account.Id}";

            return Created(locationUri, account);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] Account updated)
        {
            var existing = Accounts.FirstOrDefault(a => a.Id == id);
            if (existing == null)
                return NotFound();

            existing.OwnerId = updated.OwnerId;
            existing.Type = updated.Type;
            existing.Currency = updated.Currency;
            existing.Balance = updated.Balance;
            existing.InterestRate = updated.InterestRate;
            existing.CloseDate = updated.CloseDate;

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var existing = Accounts.FirstOrDefault(a => a.Id == id);
            if (existing == null)
                return NotFound();

            Accounts.Remove(existing);
            return NoContent();
        }

        [HttpGet("check/{ownerId:guid}")]
        public ActionResult<bool> HasAccount(Guid ownerId)
        {
            bool has = Accounts.Any(a => a.OwnerId == ownerId);
            return Ok(has);
        }
    }
}
