using MoneyTransfer.Models;

namespace MoneyTransfer.Repositories;

public interface IUserRepository
{
    IEnumerable<User> GetAll();
    User? GetById(Guid id);
    void Add(User user);
    void Update(User user);
    void Delete(User user);
    void SaveChanges();
}

