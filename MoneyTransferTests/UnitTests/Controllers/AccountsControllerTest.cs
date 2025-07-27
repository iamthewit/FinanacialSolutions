using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Controllers;
using MoneyTransfer.Models;
using MoneyTransfer.Repositories;
using Moq;

namespace MoneyTransferTests.UnitTests.Controllers;

public class AccountsControllerTest
{
    [Fact]
    public void ItCreatesAnAccount()
    {
        var account = new Account()
        {
            Id = 1,
            AccountBalance = 1000.00m,
            FullName = "",
            AccountType = AccountType.Student,
            EmailAddress = "",
            PhoneNumber = "01234567890"
        };

        var mockRepo = new Mock<IAccountRepository>();
        mockRepo.Setup(r => r.Add(It.IsAny<Account>())).Verifiable();
        mockRepo.Setup(r => r.SaveChanges()).Verifiable();

        var controller = new AccountsController(mockRepo.Object);
        var response = controller.Create(account);
        var result = Assert.IsType<CreatedAtActionResult>(response.Result);
        Assert.Equal(201, result.StatusCode);
        mockRepo.Verify(r => r.Add(It.IsAny<Account>()), Times.Once);
        mockRepo.Verify(r => r.SaveChanges(), Times.Once);
    }
}