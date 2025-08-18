namespace AccountServices.Features
{
    public class ClientBlockedException(Guid accountId) : Exception($"Client for account {accountId} is blocked")
    {
        // ReSharper disable once UnusedMember.Global
        public Guid AccountId { get; } = accountId;
    }

}
