using MoneyTransfer.Models;

namespace MoneyTransfer.Controllers;

public class StaticAccountList
{
    public static readonly List<Account> Accounts =
    [
        new Account
        {
            Id = 1,
            AccountBalance = 1000.00m,
            FullName = "John Doe",
            AccountType = AccountType.Student,
            EmailAddress = "jogn@bank.com",
            PhoneNumber = "01234567890"
        },
        new Account
        {
            Id = 2,
            AccountBalance = 2000.00m,
            FullName = "Jane Smith",
            AccountType = AccountType.Student,
            EmailAddress = "jane@bank.com",
            PhoneNumber = "01234567890"
        },
        new Account
        {
            Id = 3,
            AccountBalance = 1500.00m,
            FullName = "Alice Johnson",
            AccountType = AccountType.Student,
            EmailAddress = "alice@bank.com",
            PhoneNumber = "01234567890"
        }
    ];
}