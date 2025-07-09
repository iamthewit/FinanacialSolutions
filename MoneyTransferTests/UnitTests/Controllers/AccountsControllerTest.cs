namespace MoneyTransferTests;

public class AccountsControllerTest
{
    [Fact]
    public void ItCreatesAnAccount()
    {
        var account = new MoneyTransfer.Models.Account()
        {
            Id = 1,
            AccountBalance = 1000.00m,
            FullName = "",
            AccountType = MoneyTransfer.Models.AccountType.Student,
            EmailAddress = "",
            PhoneNumber = "01234567890"
        };
        var controller = new MoneyTransfer.Controllers.AccountsController();
        
        var response = controller.Create(account);
        
        var result = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(response.Result);
        
        // asssert response is 201
        Assert.Equal(201, result.StatusCode);
    }
}