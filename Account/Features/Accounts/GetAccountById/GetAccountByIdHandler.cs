using MediatR;

namespace Account.Features.Accounts.GetAccountById
{
    public class GetAccountByIdHandler(IAccountRepository repository) : IRequestHandler<GetAccountByIdQuery, Account?>
    {

        public Task<Account?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = repository.GetById(request.Id);
            if (account == null)
                throw new NotFoundException($"Account {request.Id} not found");
            return Task.FromResult<Account?>(account);
        }
    }
}
