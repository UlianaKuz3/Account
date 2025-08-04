using MediatR;

namespace Account.Features.Accounts.HasAccount
{
    public record HasAccountQuery(Guid OwnerId) : IRequest<bool>;
}
