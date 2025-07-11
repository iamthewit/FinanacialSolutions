using System.Collections.Generic;
using MoneyTransfer.Models;

namespace MoneyTransfer.Controllers
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAll();
        Account? GetById(int id);
        void Add(Account account);
        void Update(Account account);
        void Delete(Account account);
        void SaveChanges();
    }
}

