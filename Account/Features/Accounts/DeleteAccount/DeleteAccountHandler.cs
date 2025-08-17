using AccountServices.Features.Entities;
using MediatR;
using System.Text.Json;

namespace AccountServices.Features.Accounts.DeleteAccount
{
    public class DeleteAccountHandler(IAccountRepository repository) : IRequestHandler<DeleteAccountCommand>
    {

        public Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);

            var evt = new
            {
                AccountId = request.Id
            };

            var outbox = new OutboxMessage
            {
                Type = "AccountDeleted",
                RoutingKey = "account.deleted",
                Payload = JsonSerializer.Serialize(evt)
            };

            repository.AddOutboxMessage(outbox);

            repository.SaveChangesAsync(cancellationToken);
        
            return Task.CompletedTask;
        }
    }
}
