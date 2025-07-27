using MediatR;

namespace AccountService.Features.Accounts.GetAllAccounts
{
    public record GetAllAccountsQuery() : IRequest<IEnumerable<Account>>;
}
