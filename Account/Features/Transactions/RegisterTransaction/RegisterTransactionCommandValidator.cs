using FluentValidation;

namespace AccountService.Features.Transactions.RegisterTransaction
{
    public class RegisterTransactionCommandValidator : AbstractValidator<RegisterTransactionCommand>
    {
        public RegisterTransactionCommandValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Currency).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Description).MaximumLength(200);
        }
    }
}
