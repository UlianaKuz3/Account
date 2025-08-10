using MediatR;

namespace Account.Features.Transactions.TransferTransaction
{
    public record TransferTransactionCommand(Guid FromAccountId, Guid ToAccountId,
                                            decimal Amount, string Currency,
                                            string Description
    ) : IRequest<(Transaction Debit, Transaction Credit)>;
}
