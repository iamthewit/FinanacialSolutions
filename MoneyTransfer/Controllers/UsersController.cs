using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Models;
using MoneyTransfer.Dto;
using System.Security.Cryptography;
using MoneyTransfer.Repositories;

namespace MoneyTransfer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetAll()
    {
        var users = repository.GetAll();
        return Ok(users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            EmailAddress = u.EmailAddress,
            PhoneNumber = u.PhoneNumber
        }));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<UserDto> GetById(Guid id)
    {
        var user = repository.GetById(id);
        if (user == null) return NotFound();
        return Ok(new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            PhoneNumber = user.PhoneNumber
        });
    }

    [HttpPost]
    public ActionResult<UserDto> Create(User user)
    {
        user.Id = Guid.NewGuid();
        user.Password = HashPassword(user.Password);
        repository.Add(user);
        repository.SaveChanges();
        var dto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            PhoneNumber = user.PhoneNumber
        };
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, User updatedUser)
    {
        var user = repository.GetById(id);
        if (user == null) return NotFound();
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.EmailAddress = updatedUser.EmailAddress;
        user.Password = HashPassword(updatedUser.Password);
        user.PhoneNumber = updatedUser.PhoneNumber;
        repository.Update(user);
        repository.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var user = repository.GetById(id);
        if (user == null) return NotFound();
        repository.Delete(user);
        repository.SaveChanges();
        return NoContent();
    }

    private static string HashPassword(string password)
    {
        // Use PBKDF2 with a random salt
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
