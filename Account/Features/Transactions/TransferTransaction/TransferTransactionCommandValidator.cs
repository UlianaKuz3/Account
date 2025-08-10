using FluentValidation;

namespace Account.Features.Transactions.TransferTransaction
{
    public class TransferTransactionCommandValidator : AbstractValidator<TransferTransactionCommand>
    {
        public TransferTransactionCommandValidator()
        {
            RuleFor(x => x.FromAccountId).NotEmpty();
            RuleFor(x => x.ToAccountId).NotEmpty().NotEqual(x => x.FromAccountId);
            RuleFor(x => x.Currency).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
