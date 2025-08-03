using MediatR;

namespace Account.Features.Accounts.DeleteAccount
{
    public class DeleteAccountHandler(IAccountRepository repository) : IRequestHandler<DeleteAccountCommand>
    {

        public Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            return Task.CompletedTask;
        }
    }
}
