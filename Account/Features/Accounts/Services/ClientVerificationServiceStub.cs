namespace AccountService.Features.Accounts.Services
{
    public class ClientVerificationServiceStub : IClientVerificationService
    {
        private static readonly HashSet<Guid> ExistingOwners =
        [
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
        ];

        public bool Exists(Guid ownerId)
        {
            return ExistingOwners.Contains(ownerId);
        }
    }
}
