using AccountServices.Features.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace AccountServices.Features.Examples
{
    public class TransferCompletedExample : IExamplesProvider<EventEnvelope<TransferCompleted>>
    {
        public EventEnvelope<TransferCompleted> GetExamples() =>
            new(
                EventId: Guid.Parse("e7f8a9b0-1111-2222-3333-aaaabbbbcccc"),
                OccurredAt: DateTime.Parse("2025-08-14T15:45:00Z").ToUniversalTime(),
                Meta: new("v1", "account-service",
                    Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Guid.Parse("88888888-8888-8888-8888-888888888888")),
                Payload: new(
                    SourceAccountId: Guid.Parse("9c3f3f5d-7c2e-4a1a-9f5a-1b3a7e9d2f11"),
                    DestinationAccountId: Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"),
                    Amount: 250m,
                    Currency: "RUB",
                    TransferId: Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d")
                )
            );
    } 
}
