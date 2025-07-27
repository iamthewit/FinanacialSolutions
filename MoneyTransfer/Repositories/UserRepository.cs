using MoneyTransfer.Models;

namespace MoneyTransfer.Repositories;

public class UserRepository(FinancialSolutionsDbContext db) : IUserRepository
{
    public IEnumerable<User> GetAll() => db.Users.ToList();

    public User? GetById(Guid id) => db.Users.Find(id);

    public void Add(User user)
    {
        db.Users.Add(user);
    }

    public void Update(User user)
    {
        db.Users.Update(user);
    }

    public void Delete(User user)
    {
        db.Users.Remove(user);
    }

    public void SaveChanges()
    {
        db.SaveChanges();
    }
}

