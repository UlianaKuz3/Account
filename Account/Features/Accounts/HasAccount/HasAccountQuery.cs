using MediatR;

namespace AccountServices.Features.Accounts.HasAccount
{
    public record HasAccountQuery(Guid OwnerId) : IRequest<bool>;
}
