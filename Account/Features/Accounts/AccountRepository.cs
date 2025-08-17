using Microsoft.EntityFrameworkCore;

namespace AccountServices.Features.Accounts
{
    public class AccountRepository(AppDbContext context) : IAccountRepository
    {

        public Account? GetById(Guid id) =>
            context.Accounts.Include(a => a.Transactions).FirstOrDefault(a => a.Id == id);

        public List<Account> GetAll() =>
            context.Accounts.Include(a => a.Transactions).ToList();

        public void Add(Account account)
        {
            context.Accounts.Add(account);
            context.SaveChanges();
        }

        public void Update(Account account)
        {
            context.Accounts.Update(account);
            context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var account = GetById(id);
            if (account != null)
            {
                context.Accounts.Remove(account);
                context.SaveChanges();
            }
        }
    }

}
