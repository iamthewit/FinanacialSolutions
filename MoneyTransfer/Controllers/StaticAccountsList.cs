using MoneyTransfer.Models;

namespace MoneyTransfer.Controllers;

public class StaticAccountList
{
    public static readonly List<Account> Accounts =
    [
        new Account
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            AccountBalance = 1000.00m,
            AccountType = AccountType.Student,
            UserId = Guid.Empty // Placeholder, update as needed
        },
        new Account
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            AccountBalance = 2000.00m,
            AccountType = AccountType.Student,
            UserId = Guid.Empty // Placeholder, update as needed
        },
        new Account
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            AccountBalance = 1500.00m,
            AccountType = AccountType.Student,
            UserId = Guid.Empty // Placeholder, update as needed
        }
    ];
}