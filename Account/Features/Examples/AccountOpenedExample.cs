using AccountServices.Features.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace AccountServices.Features.Examples
{
    public class AccountOpenedExample : IExamplesProvider<EventEnvelope<AccountOpened>>
    {
        public EventEnvelope<AccountOpened> GetExamples() =>
            new(
                EventId: Guid.Parse("b5f3a7f6-2f4e-4b1a-9f3a-2b0c1e7c1a11"),
                OccurredAt: DateTime.Parse("2025-08-12T21:00:00Z").ToUniversalTime(),
                Meta: new("v1", "account-service",
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Guid.Parse("22222222-2222-2222-2222-222222222222")),
                Payload: new(
                    AccountId: Guid.Parse("9c3f3f5d-7c2e-4a1a-9f5a-1b3a7e9d2f11"),
                    OwnerId: Guid.Parse("2a7e9d2f-9f5a-4a1a-7c2e-9c3f3f5d1b3a"),
                    Currency: "RUB",
                    Type: "Checking"
                )
            );
    }
}
