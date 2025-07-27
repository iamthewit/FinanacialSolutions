using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using MoneyTransfer.Dto;
using MoneyTransfer.Models;

namespace MoneyTransferTests.IntegrationTests;

public class AccountsControllerTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    // TODO: Setup
    // - truncate database
    // - seed with test data if needed
    // - migrations?
    
    [Fact]
    public async Task GetAll_ReturnsOkAndAccounts()
    {
        var response = await _client.GetAsync("/api/accounts");
        response.EnsureSuccessStatusCode();

        var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();
        Assert.NotNull(accounts);
        Assert.True(accounts.Count >= 0);
    }
    
    [Fact]
    public async Task GetAccountById_ReturnsCorrectAccount()
    {
        // Arrange: create a new account
        var newAccount = TestAccountObject();
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        createResponse.EnsureSuccessStatusCode();
        var createdAccount = await createResponse.Content.ReadFromJsonAsync<Account>();
    
        // Act: get the account by ID
        var getResponse = await _client.GetAsync($"/api/accounts/{createdAccount.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedAccount = await getResponse.Content.ReadFromJsonAsync<Account>();
    
        // Assert: verify the fetched account matches the created one
        Assert.NotNull(fetchedAccount);
        Assert.Equal(createdAccount.Id, fetchedAccount.Id);
        Assert.Equal(newAccount.FullName, fetchedAccount.FullName);
        Assert.Equal(newAccount.AccountBalance, fetchedAccount.AccountBalance);
        Assert.Equal(newAccount.AccountType, fetchedAccount.AccountType);
        Assert.Equal(newAccount.EmailAddress, fetchedAccount.EmailAddress);
        Assert.Equal(newAccount.PhoneNumber, fetchedAccount.PhoneNumber);
    }
    
    [Fact]
    public async Task CreateAccount_ReturnsCreatedAtAction()
    {
        var newAccount = TestAccountObject();
        var response = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        response.EnsureSuccessStatusCode();
        
        var createdAccount = await response.Content.ReadFromJsonAsync<Account>();
        
        Assert.NotNull(createdAccount);
        Assert.Equal(newAccount.AccountBalance, createdAccount.AccountBalance);
        Assert.Equal(newAccount.FullName, createdAccount.FullName);
        Assert.Equal(newAccount.AccountType, createdAccount.AccountType);
        Assert.Equal(newAccount.EmailAddress, createdAccount.EmailAddress);
        Assert.Equal(newAccount.PhoneNumber, createdAccount.PhoneNumber);
        Assert.NotEqual(0, createdAccount.Id); // Ensure ID is set
        Assert.Equal(201, (int)response.StatusCode); // Ensure status code is 201 Created
    }

    [Fact]
    public async Task UpdateAccount_ReturnsOkAndUpdatedAccount()
    {
        // Arrange: create a new account
        var newAccount = TestAccountObject();
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        createResponse.EnsureSuccessStatusCode();
        var createdAccount = await createResponse.Content.ReadFromJsonAsync<Account>();
    
        // Act: update the account
        createdAccount.FullName = "Updated Name";
        createdAccount.AccountBalance = 200.00m;
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
            new object[] { new { AccountBalance = 0m, FullName = "Test", AccountType = AccountType.Student, EmailAddress = "test@test.com", PhoneNumber = "1234567890" }, "AccountBalance" },
            new object[] { new { AccountBalance = 100m, FullName = "", AccountType = AccountType.Student, EmailAddress = "test@test.com", PhoneNumber = "1234567890" }, "FullName" },
            new object[] { new { AccountBalance = 100m, FullName = "Test", AccountType = 99, EmailAddress = "test@test.com", PhoneNumber = "1234567890" }, "AccountType" },
            new object[] { new { AccountBalance = 100m, FullName = "Test", AccountType = AccountType.Student, EmailAddress = "invalid-email", PhoneNumber = "1234567890" }, "EmailAddress" },
            new object[] { new { AccountBalance = 100m, FullName = "Test", AccountType = AccountType.Student, EmailAddress = "test@test.com", PhoneNumber = "invalid-phone" }, "PhoneNumber" }
        };
    
    [Fact]
    public async Task PartialUpdateAccount_UpdatesSpecifiedFields()
    {
        // Arrange: create a new account
        var newAccount = TestAccountObject();
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", newAccount);
        createResponse.EnsureSuccessStatusCode();
        var createdAccount = await createResponse.Content.ReadFromJsonAsync<Account>();
    
        // Act: patch the account using AccountPatchDto
        var patchDto = new AccountPatchDto()
        {
            FullName = "Patched Name",
            AccountBalance = 150.00m
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/accounts/{createdAccount.Id}", patchDto);
        patchResponse.EnsureSuccessStatusCode();
    
        // Assert: verify the fields were updated
        var getResponse = await _client.GetAsync($"/api/accounts/{createdAccount.Id}");
        getResponse.EnsureSuccessStatusCode();
        var updatedAccount = await getResponse.Content.ReadFromJsonAsync<Account>();
        Assert.Equal("Patched Name", updatedAccount.FullName);
        Assert.Equal(150.00m, updatedAccount.AccountBalance);
        Assert.Equal(newAccount.AccountType, updatedAccount.AccountType); // unchanged
        Assert.Equal(newAccount.EmailAddress, updatedAccount.EmailAddress); // unchanged
        Assert.Equal(newAccount.PhoneNumber, updatedAccount.PhoneNumber); // unchanged
    }
    
    [Fact]
    public async Task DeleteAccount_RemovesAccount()
    {
        // Arrange: create a new account
        var newAccount = TestAccountObject();
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
    
    private static Account TestAccountObject()
    {
        var newAccount = new Account
        {
            AccountBalance = 500.00m,
            FullName = "Test User",
            AccountType = AccountType.Student,
            EmailAddress = "email@address.com",
            PhoneNumber = "01234567890",
        };
        return newAccount;
    }
}