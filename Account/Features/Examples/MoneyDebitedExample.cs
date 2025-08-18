using AccountServices.Features.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace AccountServices.Features.Examples
{
    public class MoneyDebitedExample : IExamplesProvider<EventEnvelope<MoneyDebited>>
    {
        public EventEnvelope<MoneyDebited> GetExamples() =>
            new(
                EventId: Guid.Parse("d4e5f6a7-1111-2222-3333-777788889999"),
                OccurredAt: DateTime.Parse("2025-08-13T10:30:00Z").ToUniversalTime(),
                Meta: new("v1", "account-service",
                    Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Guid.Parse("66666666-6666-6666-6666-666666666666")),
                Payload: new(
                    AccountId: Guid.Parse("9c3f3f5d-7c2e-4a1a-9f5a-1b3a7e9d2f11"),
                    Amount: 500m,
                    Currency: "RUB"
                )
            );
    }
}
