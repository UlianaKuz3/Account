namespace AccountServices.Features.Accounts
{
    public record ClientBlocked
    {
        public Guid ClientId { get; init; }
    }
}
