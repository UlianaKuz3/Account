using MediatR;

namespace AccountService.Features.Transactions.RegisterTransaction
{
    public record RegisterTransactionCommand(Guid AccountId, Guid? CounterpartyAccountId,
                                                    decimal Amount, string Currency,
                                                    TransactionType Type, string Description
    ) : IRequest<Transaction>;
}
