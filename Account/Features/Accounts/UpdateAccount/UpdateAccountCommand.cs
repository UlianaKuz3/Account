using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount
{
    public record UpdateAccountCommand(Guid Id, Guid OwnerId,
                                            string Currency, decimal Balance,
                                            decimal? InterestRate, DateTime CloseDate)
        : IRequest<bool>;
}
