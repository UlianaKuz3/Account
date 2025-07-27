using MediatR;

namespace AccountService.Features.Accounts.HasAccount
{
    public record HasAccountQuery(Guid OwnerId) : IRequest<bool>;
}
