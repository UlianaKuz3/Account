using AccountServices.Features.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace AccountServices.Features.Examples
{
    public class MoneyCreditedExample : IExamplesProvider<EventEnvelope<MoneyCredited>>
    {
        public EventEnvelope<MoneyCredited> GetExamples() =>
            new(
                EventId: Guid.Parse("c1d2e3f4-1111-2222-3333-444455556666"),
                OccurredAt: DateTime.Parse("2025-08-12T22:00:00Z").ToUniversalTime(),
                Meta: new("v1", "account-service",
                    Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Guid.Parse("44444444-4444-4444-4444-444444444444")),
                Payload: new(
                    AccountId: Guid.Parse("9c3f3f5d-7c2e-4a1a-9f5a-1b3a7e9d2f11"),
                    Amount: 1000m,
                    Currency: "RUB"
                )
            );
    }
}
