using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Models;
using MoneyTransfer.Dto;
using System.Security.Cryptography;
using MoneyTransfer.Services;

namespace MoneyTransfer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetAll()
    {
        var users = userService.GetAll();
        
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<UserDto> GetById(Guid id)
    {
        var user = userService.GetById(id);
        
        if (user == null) return NotFound();
        
        return Ok(user);
    }

    [HttpPost]
    public ActionResult<UserDto> Create(User user)
    {
        user.Id = Guid.NewGuid();
        var dto = userService.Add(user);
        
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, User updatedUser)
    {
        var dto = userService.Update(id, updatedUser);
        
        if (dto == null) return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var user = userService.GetById(id);
        if (user == null) return NotFound();
        userService.Delete(id);
        
        return NoContent();
    }
}
