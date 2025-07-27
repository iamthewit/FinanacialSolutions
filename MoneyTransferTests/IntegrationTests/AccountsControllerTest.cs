using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MoneyTransfer.Dto;
using MoneyTransfer.Models;

namespace MoneyTransferTests.IntegrationTests;

public class AccountsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public AccountsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task TruncateAccountsTableAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MoneyTransfer.FinancialSolutionsDbContext>();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM Accounts");
        await db.Database.ExecuteSqlRawAsync("ALTER TABLE Accounts AUTO_INCREMENT = 1");
    }

    public async Task InitializeAsync()
    {
        await TruncateAccountsTableAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
    
    private async Task<Guid> CreateTestUserAndGetIdAsync()
    {
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            EmailAddress = $"test{Guid.NewGuid()}@example.com",
            Password = "TestPassword!",
            PhoneNumber = "+10000000000"
        };
        var response = await _client.PostAsJsonAsync("/api/users", user);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<UserDto>();
        return created!.Id;
    }

    private static Account TestAccountObject(Guid userId)
    {
        return new Account
        {
            AccountBalance = 500.00m,
            AccountType = AccountType.Student,
            UserId = userId
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndAccounts()
    {
        var userId = await CreateTestUserAndGetIdAsync();
        var newAccount = TestAccountObject(userId);
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        createResponse.EnsureSuccessStatusCode();
        var response = await _client.GetAsync("/api/accounts");
        response.EnsureSuccessStatusCode();
        var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();
        Assert.NotNull(accounts);
        Assert.True(accounts.Count > 0);
        Assert.Contains(accounts, a => a.UserId == userId);
    }
    
    [Fact]
    public async Task GetAccountById_ReturnsCorrectAccount()
    {
        var userId = await CreateTestUserAndGetIdAsync();
        var newAccount = TestAccountObject(userId);
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        createResponse.EnsureSuccessStatusCode();
        var createdAccount = await createResponse.Content.ReadFromJsonAsync<Account>();
        var getResponse = await _client.GetAsync($"/api/accounts/{createdAccount.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedAccount = await getResponse.Content.ReadFromJsonAsync<Account>();
        Assert.NotNull(fetchedAccount);
        Assert.Equal(createdAccount.Id, fetchedAccount.Id);
        Assert.Equal(newAccount.AccountBalance, fetchedAccount.AccountBalance);
        Assert.Equal(newAccount.AccountType, fetchedAccount.AccountType);
        Assert.Equal(newAccount.UserId, fetchedAccount.UserId);
    }
    
    [Fact]
    public async Task CreateAccount_ReturnsCreatedAtAction()
    {
        var userId = await CreateTestUserAndGetIdAsync();
        var newAccount = TestAccountObject(userId);
        var response = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        response.EnsureSuccessStatusCode();
        var createdAccount = await response.Content.ReadFromJsonAsync<Account>();
        Assert.NotNull(createdAccount);
        Assert.Equal(newAccount.AccountBalance, createdAccount.AccountBalance);
        Assert.Equal(newAccount.AccountType, createdAccount.AccountType);
        Assert.Equal(newAccount.UserId, createdAccount.UserId);
        Assert.NotEqual(Guid.Empty, createdAccount.Id);
        Assert.Equal(201, (int)response.StatusCode);
    }

    [Fact]
    public async Task UpdateAccount_ReturnsOkAndUpdatedAccount()
    {
        var userId = await CreateTestUserAndGetIdAsync();
        var newAccount = TestAccountObject(userId);
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        createResponse.EnsureSuccessStatusCode();
        var createdAccount = await createResponse.Content.ReadFromJsonAsync<Account>();
        createdAccount.AccountBalance = 200.00m;
        createdAccount.AccountType = AccountType.Student;
        createdAccount.UserId = userId;
        var updateResponse = await _client.PutAsJsonAsync($"/api/accounts/{createdAccount.Id}", createdAccount);
        updateResponse.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [MemberData(nameof(InvalidAccountData))]
    public async Task CreateAccount_InvalidData_ReturnsBadRequest(object invalidAccount, string expectedField)
    {
        var response = await _client.PostAsJsonAsync("/api/accounts", invalidAccount);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.ContainsKey(expectedField));
    }
    
    public static IEnumerable<object[]> InvalidAccountData =>
        new List<object[]>
        {
            new object[] { new { AccountBalance = 0m, AccountType = AccountType.Student, UserId = Guid.NewGuid() }, "AccountBalance" },
            new object[] { new { AccountBalance = 100m, AccountType = 99, UserId = Guid.NewGuid() }, "AccountType" }
        };
    
    [Fact]
    public async Task PartialUpdateAccount_UpdatesSpecifiedFields()
    {
        var userId = await CreateTestUserAndGetIdAsync();
        var newAccount = TestAccountObject(userId);
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        createResponse.EnsureSuccessStatusCode();
        var createdAccount = await createResponse.Content.ReadFromJsonAsync<Account>();
        var patchDto = new AccountPatchDto()
        {
            AccountBalance = 150.00m
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/accounts/{createdAccount.Id}", patchDto);
        patchResponse.EnsureSuccessStatusCode();
        var getResponse = await _client.GetAsync($"/api/accounts/{createdAccount.Id}");
        getResponse.EnsureSuccessStatusCode();
        var updatedAccount = await getResponse.Content.ReadFromJsonAsync<Account>();
        Assert.Equal(150.00m, updatedAccount.AccountBalance);
        Assert.Equal(newAccount.AccountType, updatedAccount.AccountType);
        Assert.Equal(newAccount.UserId, updatedAccount.UserId);
    }
    
    [Fact]
    public async Task DeleteAccount_RemovesAccount()
    {
        // Arrange: create a new account
        var userId = await CreateTestUserAndGetIdAsync();
        var newAccount = TestAccountObject(userId);
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        createResponse.EnsureSuccessStatusCode();
        var createdAccount = await createResponse.Content.ReadFromJsonAsync<Account>();
    
        // Act: delete the account
        var deleteResponse = await _client.DeleteAsync($"/api/accounts/{createdAccount.Id}");
        deleteResponse.EnsureSuccessStatusCode();
    
        // Assert: ensure account is deleted
        var getResponse = await _client.GetAsync($"/api/accounts/{createdAccount.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}