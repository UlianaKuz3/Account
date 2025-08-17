using MediatR;

namespace AccountServices.Features.Accounts.CreateAccount
{
    public record CreateAccountCommand(Guid OwnerId, AccountType Type, 
                                                string Currency, decimal Balance, 
                                                decimal? InterestRate)
        : IRequest<Account>;
}
