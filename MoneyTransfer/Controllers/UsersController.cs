using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransfer.Models;
using MoneyTransfer.Dto;
using System.Security.Cryptography;

namespace MoneyTransfer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(FinancialSolutionsDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await context.Users.ToListAsync();
        return Ok(users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            EmailAddress = u.EmailAddress,
            PhoneNumber = u.PhoneNumber
        }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await context.Users.FindAsync(id);
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
    public async Task<ActionResult<UserDto>> Create(User user)
    {
        user.Password = HashPassword(user.Password);
        context.Users.Add(user);
        await context.SaveChangesAsync();
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User updatedUser)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.EmailAddress = updatedUser.EmailAddress;
        user.Password = HashPassword(updatedUser.Password);
        user.PhoneNumber = updatedUser.PhoneNumber;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();
        context.Users.Remove(user);
        await context.SaveChangesAsync();
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
