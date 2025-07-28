namespace AccountService.Features.Accounts.Services
{
    public interface IClientVerificationService
    {
        bool Exists(Guid ownerId);
    }
}
