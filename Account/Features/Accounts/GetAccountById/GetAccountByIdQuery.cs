using MediatR;

namespace Account.Features.Accounts.GetAccountById
{
    public record GetAccountByIdQuery(Guid Id) : IRequest<Account?>;
}
