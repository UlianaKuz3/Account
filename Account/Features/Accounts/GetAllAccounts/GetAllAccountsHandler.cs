using MediatR;

namespace AccountServices.Features.Accounts.GetAllAccounts
{
    public class GetAllAccountsHandler(IAccountRepository repository) : IRequestHandler<GetAllAccountsQuery, IEnumerable<Account>>
    {
        public Task<IEnumerable<Account>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(repository.GetAll().AsEnumerable());
        }
    }
}
