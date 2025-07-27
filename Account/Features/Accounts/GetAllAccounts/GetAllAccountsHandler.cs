using MediatR;

namespace AccountService.Features.Accounts.GetAllAccounts
{
    public class GetAllAccountsHandler(IAccountRepository repository) : IRequestHandler<GetAllAccountsQuery, IEnumerable<Account>>
    {
        private readonly IAccountRepository _repository = repository;

        public Task<IEnumerable<Account>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetAll().AsEnumerable());
        }
    }
}
