namespace AccountServices.Features
{
    public class ClientBlockedException : Exception
    {
        public Guid AccountId { get; }

        public ClientBlockedException(Guid accountId)
            : base($"Client for account {accountId} is blocked")
        {
            AccountId = accountId;
        }
    }

}
