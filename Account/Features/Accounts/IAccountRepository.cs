namespace Account.Features.Accounts
{
    public interface IAccountRepository
    {
        List<Account> GetAll();
        Account? GetById(Guid id);
        void Add(Account account);
        void Update(Account account);
        void Delete(Guid id);
    }
}
