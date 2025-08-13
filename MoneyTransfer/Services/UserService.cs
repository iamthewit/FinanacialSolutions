using MoneyTransfer.Dto;
using MoneyTransfer.Repositories;
using System.Security.Cryptography;
using MoneyTransfer.Models;

namespace MoneyTransfer.Services;

public class UserService(IUserRepository repository) : IUserService
{
    public IEnumerable<UserDto> GetAll()
    {
        var users = repository.GetAll();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            EmailAddress = u.EmailAddress,
            PhoneNumber = u.PhoneNumber
        });
    }

    public UserDto? GetById(Guid id)
    {
        var user = repository.GetById(id);
        if (user == null) return null;
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            PhoneNumber = user.PhoneNumber
        };
    }

    public UserDto Add(User user)
    {
        user.Password = HashPassword(user.Password);
        repository.Add(user);
        repository.SaveChanges();
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            PhoneNumber = user.PhoneNumber
        };
    }

    public UserDto? Update(Guid id, User updatedUser)
    {
        var user = repository.GetById(id);
        if (user == null) return null;
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.EmailAddress = updatedUser.EmailAddress;
        user.Password = HashPassword(updatedUser.Password);
        user.PhoneNumber = updatedUser.PhoneNumber;
        repository.Update(user);
        repository.SaveChanges();
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            PhoneNumber = user.PhoneNumber
        };
    }

    public void Delete(Guid id)
    {
        var user = repository.GetById(id);
        if (user == null) return;
        repository.Delete(user);
        repository.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[16];
        rng.GetBytes(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);
        byte[] hashBytes = new byte[48];
        Buffer.BlockCopy(salt, 0, hashBytes, 0, 16);
        Buffer.BlockCopy(hash, 0, hashBytes, 16, 32);
        return Convert.ToBase64String(hashBytes);
    }
}
