namespace AccountService.Features.Accounts
{
    public class InMemoryAccountRepository: IAccountRepository
    {
        private readonly List<Account> _accounts = [];

        public List<Account> GetAll()
        {
            return _accounts;
        }

        public Account? GetById(Guid id)
        {
            return _accounts.FirstOrDefault(a => a.Id == id);
        }

        public void Add(Account account)
        {
            _accounts.Add(account);
        }

        public void Update(Account account)
        {
            var existing = GetById(account.Id);
            if (existing == null) return;

            existing.OwnerId = account.OwnerId;
            existing.Type = account.Type;
            existing.Currency = account.Currency;
            existing.Balance = account.Balance;
            existing.InterestRate = account.InterestRate;
            existing.CloseDate = account.CloseDate;
        }

        public void Delete(Guid id)
        {
            var existing = GetById(id);
            if (existing != null)
            {
                _accounts.Remove(existing);
            }
        }
    }
}
