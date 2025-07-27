using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount
{
    public class UpdateAccountHandler(IAccountRepository repository) : IRequestHandler<UpdateAccountCommand, bool>
    {
        private readonly IAccountRepository _repository = repository;

        public Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var existing = _repository.GetById(request.Id);
            if (existing == null) return Task.FromResult(false);

            existing.Currency = request.Currency;
            existing.Balance = request.Balance;
            existing.InterestRate = request.InterestRate;
            existing.CloseDate = request.CloseDate;

            _repository.Update(existing);
            return Task.FromResult(true);
        }
    }
}
