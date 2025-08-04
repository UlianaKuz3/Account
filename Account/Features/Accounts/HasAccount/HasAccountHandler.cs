using MediatR;

namespace Account.Features.Accounts.HasAccount
{
    public class HasAccountHandler(IAccountRepository repository) : IRequestHandler<HasAccountQuery, bool>
    {

        public Task<bool> Handle(HasAccountQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(repository.GetAll().Any(a => a.OwnerId == request.OwnerId));
        }
    }
}
