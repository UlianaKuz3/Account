using MediatR;

namespace Account.Features.Accounts.GetAllAccounts
{
    public record GetAllAccountsQuery : IRequest<IEnumerable<Account>>;
}
