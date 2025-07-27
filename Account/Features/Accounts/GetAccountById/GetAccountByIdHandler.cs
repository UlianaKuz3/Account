using MediatR;

namespace AccountService.Features.Accounts.GetAccountById
{
    public class GetAccountByIdHandler(IAccountRepository repository) : IRequestHandler<GetAccountByIdQuery, Account?>
    {
        private readonly IAccountRepository _repository = repository;

        public Task<Account?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById(request.Id);
            return Task.FromResult(account);
        }
    }
}
