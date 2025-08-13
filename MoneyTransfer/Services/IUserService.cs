using MoneyTransfer.Dto;
using MoneyTransfer.Models;
using System;
using System.Collections.Generic;

namespace MoneyTransfer.Services;

public interface IUserService
{
    IEnumerable<UserDto> GetAll();
    UserDto? GetById(Guid id);
    UserDto Add(User user);
    UserDto? Update(Guid id, User updatedUser);
    void Delete(Guid id);
}
