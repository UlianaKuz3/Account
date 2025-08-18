using System.Text.Json;
using AccountServices.Features.Accounts.Services;
using AccountServices.Features.Entities;
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
                OpenDate = DateTime.UtcNow,
                IsBlocked = request.IsBlocked
            };

            repository.Add(account);

            var evt = new
            {
                AccountId = account.Id
            };

            var outbox = new OutboxMessage
            {
                Type = "AccountCreated",
                RoutingKey = "account.created",
                Payload = JsonSerializer.Serialize(evt)
            };

            repository.AddOutboxMessage(outbox);

            repository.SaveChangesAsync(cancellationToken);

            return Task.FromResult(account);
        }
    }
}
