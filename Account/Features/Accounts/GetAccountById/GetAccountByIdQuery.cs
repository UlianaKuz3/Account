using MediatR;

namespace AccountServices.Features.Accounts.GetAccountById
{
    public record GetAccountByIdQuery(Guid Id) : IRequest<Account?>;
}
