using MoneyTransfer.Models;

namespace MoneyTransfer.Controllers
{
    public class AccountRepository(FinancialSolutionsDbContext db) : IAccountRepository
    {
        public IEnumerable<Account> GetAll() => db.Accounts.ToList();

        public Account? GetById(int id) => db.Accounts.Find(id);

        public void Add(Account account)
        {
            db.Accounts.Add(account);
        }

        public void Update(Account account)
        {
            db.Accounts.Update(account);
        }

        public void Delete(Account account)
        {
            db.Accounts.Remove(account);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}

