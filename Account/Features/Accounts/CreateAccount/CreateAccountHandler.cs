using AccountService.Features.Accounts.Services;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount
{
    public class CreateAccountHandler(IAccountRepository repository, IClientVerificationService clientService,
        ICurrencyService currencyService) : IRequestHandler<CreateAccountCommand, Account>
    {
        private readonly IAccountRepository _repository = repository;
        private readonly IClientVerificationService _clientService = clientService;
        private readonly ICurrencyService _currencyService = currencyService;

        public Task<Account> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            if (!_clientService.Exists(request.OwnerId))
                throw new ValidationException("OwnerId does not exist");

            if (!_currencyService.IsSupported(request.Currency))
                throw new ValidationException("Unsupported currency");

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
