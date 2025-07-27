using MediatR;

namespace AccountService.Features.Accounts.CreateAccount
{
    public class CreateAccountHandler(IAccountRepository repository) : IRequestHandler<CreateAccountCommand, Account>
    {
        private readonly IAccountRepository _repository = repository;

        public Task<Account> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                OwnerId = request.OwnerId,
                Type = request.Type,
                Currency = request.Currency,
                Balance = request.Balance,
                InterestRate = request.InterestRate,
                OpenDate = DateTime.UtcNow
            };

            _repository.Add(account);

            return Task.FromResult(account);
        }
    }
}
