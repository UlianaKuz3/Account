namespace AccountServices.Features.Entities
{
    /// <summary>Оболочка события (envelope).</summary>
    public record EventEnvelope<TPayload>(
        Guid EventId,
        DateTime OccurredAt,
        EventMeta Meta,
        TPayload Payload
    );

    /// <summary>Мета-данные события.</summary>
    public record EventMeta(
        string Version,         
        string Source,         
        Guid CorrelationId,
        Guid CausationId
    );

    /// <summary>Событие открытия счёта.</summary>
    public record AccountOpened(
        Guid AccountId,
        Guid OwnerId,
        string Currency,
        string Type               
    );

    /// <summary>Зачисление средств.</summary>
    public record MoneyCredited(
        Guid AccountId,
        decimal Amount,
        string Currency 
    );

    /// <summary>Списание средств.</summary>
    public record MoneyDebited(
        Guid AccountId,
        decimal Amount,
        string Currency
    );

    /// <summary>Перевод завершён.</summary>
    public record TransferCompleted(
        Guid SourceAccountId,
        Guid DestinationAccountId,
        decimal Amount,
        string Currency,
        Guid TransferId
    );
}
