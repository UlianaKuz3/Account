using MediatR;

namespace Account.Features.Accounts.UpdateAccount
{
    public class UpdateAccountHandler(IAccountRepository repository) : IRequestHandler<UpdateAccountCommand, bool>
    {

        public Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var existing = repository.GetById(request.Id);
            if (existing == null)
                throw new NotFoundException($"Account {request.Id} not found");

            existing.Currency = request.Currency;
            existing.Balance = request.Balance;
            existing.InterestRate = request.InterestRate;
            existing.CloseDate = request.CloseDate;

            repository.Update(existing);
            return Task.FromResult(true);
        }
    }
}
