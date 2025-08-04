using MediatR;

namespace Account.Features.Accounts.DeleteAccount
{
    public record DeleteAccountCommand(Guid Id) : IRequest;
}
