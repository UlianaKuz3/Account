using Microsoft.EntityFrameworkCore;
using System;

namespace AccountServices.Features.Accounts
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public Account? GetById(Guid id) =>
            _context.Accounts.Include(a => a.Transactions).FirstOrDefault(a => a.Id == id);

        public List<Account> GetAll() =>
            _context.Accounts.Include(a => a.Transactions).ToList();

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var account = GetById(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
            }
        }
    }

}
