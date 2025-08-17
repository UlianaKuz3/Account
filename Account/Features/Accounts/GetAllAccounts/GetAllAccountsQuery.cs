using MediatR;

namespace AccountServices.Features.Accounts.GetAllAccounts
{
    public record GetAllAccountsQuery : IRequest<IEnumerable<Account>>;
}
