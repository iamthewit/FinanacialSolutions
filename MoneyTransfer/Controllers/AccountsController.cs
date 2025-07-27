using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Dto;
using MoneyTransfer.Models;
using MoneyTransfer.Repositories;

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

    [HttpGet("{id:guid}")]
    public ActionResult<Account> GetById(Guid id)
    {
        var account = repository.GetById(id);
        if (account == null) return NotFound();
        return Ok(account);
    }

    [HttpPost]
    public ActionResult<Account> Create(Account account)
    {
        account.Id = Guid.NewGuid();
        repository.Add(account);
        repository.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, Account updatedAccount)
    {
        var account = repository.GetById(id);
        if (account == null) return NotFound();

        account.AccountBalance = updatedAccount.AccountBalance;
        account.AccountType = updatedAccount.AccountType;
        account.UserId = updatedAccount.UserId;

        repository.Update(account);
        repository.SaveChanges();
        return NoContent();
    }
    
    [HttpPatch("{id:guid}")]
    public IActionResult UpdatePartial(Guid id, [FromBody] AccountPatchDto accountPatchDto)
    {
        var account = repository.GetById(id);
        if (account == null) return NotFound();

        if (accountPatchDto.AccountBalance.HasValue)
            account.AccountBalance = accountPatchDto.AccountBalance.Value;
        if (accountPatchDto.AccountType.HasValue)
            account.AccountType = accountPatchDto.AccountType.Value;
        if (accountPatchDto.UserId.HasValue)
            account.UserId = accountPatchDto.UserId.Value;
        repository.Update(account);
        repository.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var account = repository.GetById(id);
        if (account == null) return NotFound();
        repository.Delete(account);
        repository.SaveChanges();
        return NoContent();
    }
}