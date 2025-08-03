namespace AccountService.Features.Accounts.Services
{
    public class ClientVerificationServiceStub : IClientVerificationService
    {

        public bool Exists(Guid ownerId)
        {
            return true;
        }
    }
}
