using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Dto;
using MoneyTransfer.Models;

namespace MoneyTransfer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Account>> GetAll()
    {
        return Ok(repository.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Account> GetById(int id)
    {
        var account = repository.GetById(id);
        if (account == null) return NotFound();
        return Ok(account);
    }

    [HttpPost]
    public ActionResult<Account> Create(Account account)
    {
        repository.Add(account);
        repository.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Account updatedAccount)
    {
        var account = repository.GetById(id);
        if (account == null) return NotFound();

        account.AccountBalance = updatedAccount.AccountBalance;
        account.FullName = updatedAccount.FullName;
        account.AccountType = updatedAccount.AccountType;
        account.EmailAddress = updatedAccount.EmailAddress;
        account.PhoneNumber = updatedAccount.PhoneNumber;

        repository.Update(account);
        repository.SaveChanges();
        return NoContent();
    }
    
    [HttpPatch("{id:int}")]
    public IActionResult UpdatePartial(int id, [FromBody] AccountPatchDto accountPatchDto)
    {
        var account = repository.GetById(id);
        if (account == null) return NotFound();

        if (accountPatchDto.AccountBalance.HasValue)
            account.AccountBalance = accountPatchDto.AccountBalance.Value;
        
        if (!string.IsNullOrEmpty(accountPatchDto.FullName))
            account.FullName = accountPatchDto.FullName;
        
        if (accountPatchDto.AccountType.HasValue)
            account.AccountType = accountPatchDto.AccountType.Value;
        
        if (!string.IsNullOrEmpty(accountPatchDto.EmailAddress))
            account.EmailAddress = accountPatchDto.EmailAddress;
        
        if (!string.IsNullOrEmpty(accountPatchDto.PhoneNumber))
            account.PhoneNumber = accountPatchDto.PhoneNumber;

        repository.Update(account);
        repository.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var account = repository.GetById(id);
        if (account == null) return NotFound();
        repository.Delete(account);
        repository.SaveChanges();
        return NoContent();
    }
}