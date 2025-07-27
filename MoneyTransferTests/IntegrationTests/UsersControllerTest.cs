using System.Net;
using System.Net.Http.Json;
using MoneyTransfer.Models;
using MoneyTransfer.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace MoneyTransferTests.IntegrationTests;

public class UsersControllerTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public UsersControllerTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Truncate Users table before each test
    private async Task TruncateUsersTableAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MoneyTransfer.FinancialSolutionsDbContext>();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM Users");
        // Optionally reset auto-increment (for MySQL):
        await db.Database.ExecuteSqlRawAsync("ALTER TABLE Users AUTO_INCREMENT = 1");
    }

    public async Task InitializeAsync()
    {
        await TruncateUsersTableAsync();
    }

    public async Task DisposeAsync()
    {
        // No-op
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedUser()
    {
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@example.com",
            Password = "Password123!",
            PhoneNumber = "+1234567890"
        };

        var response = await _client.PostAsJsonAsync("/api/users", user);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(created);
        Assert.Equal(user.FirstName, created!.FirstName);
        Assert.Equal(user.LastName, created.LastName);
        Assert.Equal(user.EmailAddress, created.EmailAddress);
        Assert.Equal(user.PhoneNumber, created.PhoneNumber);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsList()
    {
        // Ensure at least one user exists
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test.user@example.com",
            Password = "TestPassword!",
            PhoneNumber = "+10000000000"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/users", user);
        createResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync("/api/users");
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
        Assert.Contains(users!, u => u.EmailAddress == user.EmailAddress);
    }

    [Fact]
    public async Task GetUserById_ReturnsUserOrNotFound()
    {
        // Create a user first
        var user = new User
        {
            FirstName = "Jane",
            LastName = "Smith",
            EmailAddress = "jane.smith@example.com",
            Password = "Password456!",
            PhoneNumber = "+1987654321"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/users", user);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        var getResponse = await _client.GetAsync($"/api/users/{created!.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Fact]
    public async Task UpdateUser_UpdatesFields()
    {
        // Create a user
        var user = new User
        {
            FirstName = "Alice",
            LastName = "Brown",
            EmailAddress = "alice.brown@example.com",
            Password = "Password789!",
            PhoneNumber = "+1122334455"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/users", user);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        // Update
        var updated = new User
        {
            Id = created!.Id,
            FirstName = "Alicia",
            LastName = "Brown",
            EmailAddress = "alicia.brown@example.com",
            Password = "NewPassword!",
            PhoneNumber = "+1122334455"
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/users/{created.Id}", updated);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        // Fetch and verify
        var getResponse = await _client.GetAsync($"/api/users/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal("Alicia", fetched!.FirstName);
        Assert.Equal("alicia.brown@example.com", fetched.EmailAddress);
    }

    [Fact]
    public async Task DeleteUser_RemovesUser()
    {
        // Create a user
        var user = new User
        {
            FirstName = "Bob",
            LastName = "White",
            EmailAddress = "bob.white@example.com",
            Password = "Password000!",
            PhoneNumber = "+1098765432"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/users", user);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        // Delete
        var deleteResponse = await _client.DeleteAsync($"/api/users/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        // Try to fetch
        var getResponse = await _client.GetAsync($"/api/users/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
