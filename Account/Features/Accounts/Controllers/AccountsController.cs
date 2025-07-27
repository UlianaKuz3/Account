using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace AccountService.Features.Accounts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController(IAccountRepository repository) : ControllerBase
    {
        private readonly IAccountRepository _repository = repository;

        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAll()
        {
            return Ok(_repository.GetAll());
        }

        [HttpGet("{id:guid}")]
        public ActionResult<Account> GetById(Guid id)
        {
            var account = _repository.GetById(id);
            if (account is null) return NotFound();
            return Ok(account);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Account account)
        {
            account.Id = Guid.NewGuid();
            account.OpenDate = DateTime.UtcNow;
            _repository.Add(account);

            var locationUri = $"{Request.Host}/api/Accounts/{account.Id}";

            return Created(locationUri, account);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] Account updated)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();

            updated.Id = id; 
            _repository.Update(updated);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            _repository.Delete(id);
            return NoContent();
        }

        [HttpGet("check/{ownerId:guid}")]
        public ActionResult<bool> HasAccount(Guid ownerId)
        {
            bool has = _repository.GetAll().Any(a => a.OwnerId == ownerId);
            return Ok(has);
        }
    }
}
