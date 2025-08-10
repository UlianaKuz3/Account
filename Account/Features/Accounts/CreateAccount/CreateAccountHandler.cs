using AccountServices.Features.Accounts.Services;
using FluentValidation;
using MediatR;

namespace AccountServices.Features.Accounts.CreateAccount
{
    public class CreateAccountHandler(IAccountRepository repository, IClientVerificationService clientService,
        ICurrencyService currencyService) : IRequestHandler<CreateAccountCommand, Account>
    {

        public Task<Account> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            if (!clientService.Exists(request.OwnerId))
                throw new ValidationException("OwnerId does not exist");

            if (!currencyService.IsSupported(request.Currency))
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

            repository.Add(account);

            return Task.FromResult(account);
        }
    }
}
