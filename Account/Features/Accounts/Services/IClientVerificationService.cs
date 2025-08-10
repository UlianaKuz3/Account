namespace AccountServices.Features.Accounts.Services
{
    public interface IClientVerificationService
    {
        bool Exists(Guid ownerId);
    }
}
