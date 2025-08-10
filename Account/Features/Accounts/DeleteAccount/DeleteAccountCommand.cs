using MediatR;

namespace AccountServices.Features.Accounts.DeleteAccount
{
    public record DeleteAccountCommand(Guid Id) : IRequest;
}
