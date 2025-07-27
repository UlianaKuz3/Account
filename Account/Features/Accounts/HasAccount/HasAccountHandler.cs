using MediatR;

namespace AccountService.Features.Accounts.HasAccount
{
    public class HasAccountHandler(IAccountRepository repository) : IRequestHandler<HasAccountQuery, bool>
    {
        private readonly IAccountRepository _repository = repository;

        public Task<bool> Handle(HasAccountQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetAll().Any(a => a.OwnerId == request.OwnerId));
        }
    }
}
