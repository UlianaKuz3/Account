using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount
{
    public class DeleteAccountHandler(IAccountRepository repository) : IRequestHandler<DeleteAccountCommand>
    {
        private readonly IAccountRepository _repository = repository;

        public Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            _repository.Delete(request.Id);
            return Task.CompletedTask;
        }
    }
}
