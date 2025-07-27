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
            FullName = "John Doe",
            AccountType = AccountType.Student,
            EmailAddress = "jogn@bank.com",
            PhoneNumber = "01234567890"
        },
        new Account
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            AccountBalance = 2000.00m,
            FullName = "Jane Smith",
            AccountType = AccountType.Student,
            EmailAddress = "jane@bank.com",
            PhoneNumber = "01234567890"
        },
        new Account
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            AccountBalance = 1500.00m,
            FullName = "Alice Johnson",
            AccountType = AccountType.Student,
            EmailAddress = "alice@bank.com",
            PhoneNumber = "01234567890"
        }
    ];
}