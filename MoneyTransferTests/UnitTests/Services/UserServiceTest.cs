using MoneyTransfer.Models;
using MoneyTransfer.Repositories;
using MoneyTransfer.Services;
using Moq;

namespace MoneyTransferTests.UnitTests.Services;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly UserService _service;

    public UserServiceTest()
    {
        _repoMock = new Mock<IUserRepository>();
        _service = new UserService(_repoMock.Object);
    }

    [Fact]
    public void GetAll_ReturnsUserDtos()
    {
        // Arrange
        var users = new List<User> {
            new User {
                Id = Guid.NewGuid(),
                FirstName = "Firstname",
                LastName = "Lastname",
                EmailAddress = "firstname.lastname@example.com",
                PhoneNumber = "01234567890",
                Password = "password"
            }
        };
        _repoMock.Setup(r => r.GetAll()).Returns(users);

        // Act
        var result = _service.GetAll().ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(users[0].Id, result[0].Id);
    }

    [Fact]
    public void GetById_ReturnsUserDto_WhenFound()
    {
        // Arrange
        var user = new User {
            Id = Guid.NewGuid(),
            FirstName = "Firstname",
            LastName = "Lastname",
            EmailAddress = "firstname.lastname@example.com",
            PhoneNumber = "01234567890",
            Password = "password"
        };
        _repoMock.Setup(r => r.GetById(user.Id)).Returns(user);

        // Act
        var result = _service.GetById(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);

        // Act
        var result = _service.GetById(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Add_HashesPassword_AndReturnsDto()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(), 
            FirstName = "Firstname", 
            LastName = "Lastname", 
            EmailAddress = "firstname.lastname@example.com", 
            PhoneNumber = "01234567890", 
            Password = "password"
        };
        _repoMock.Setup(r => r.Add(It.IsAny<User>()));
        _repoMock.Setup(r => r.SaveChanges());
        
        // Act
        var dto = _service.Add(user);

        // Assert
        Assert.Equal(user.Id, dto.Id);
        Assert.NotEqual("password", user.Password); // Password should be hashed
        _repoMock.Verify(r => r.Add(It.Is<User>(u => u.Id == user.Id)), Times.Once);
        _repoMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_UpdatesUserAndReturnsDto_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User
        {
            Id = id, 
            FirstName = "Firstname", 
            LastName = "Lastname", 
            EmailAddress = "firstname.lastname@example.com", 
            PhoneNumber = "01234567890", 
            Password = "password"
        };
        var updated = new User
        {
            FirstName = "Someone", 
            LastName = "Else", 
            EmailAddress = "someone.else@example.com", 
            PhoneNumber = "09876543210", 
            Password = "new_password"
        };
        _repoMock.Setup(r => r.GetById(id)).Returns(user);
        _repoMock.Setup(r => r.Update(user));
        _repoMock.Setup(r => r.SaveChanges());
        
        // Act
        var dto = _service.Update(id, updated);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(updated.FirstName, dto!.FirstName);
        Assert.NotEqual("new_password", user.Password); // Password should be hashed
        _repoMock.Verify(r => r.Update(user), Times.Once);
        _repoMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);
        var updated = new User
        {
            FirstName = "Sonmeone", 
            LastName = "Else", 
            EmailAddress = "someone.else@example.com", 
            PhoneNumber = "09876543210", 
            Password = "new_password"
        };

        // Act
        var dto = _service.Update(Guid.NewGuid(), updated);

        // Assert
        Assert.Null(dto);
        _repoMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        _repoMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Delete_DeletesUser_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User {
            Id = id,
            FirstName = "Firstname",
            LastName = "Lastname",
            EmailAddress = "firstname.lastname@example.com",
            PhoneNumber = "01234567890",
            Password = "password",
        };
        _repoMock.Setup(r => r.GetById(id)).Returns(user);
        _repoMock.Setup(r => r.Delete(user));
        _repoMock.Setup(r => r.SaveChanges());

        // Act
        _service.Delete(id);

        // Assert
        _repoMock.Verify(r => r.Delete(user), Times.Once);
        _repoMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_DoesNothing_WhenUserNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);

        // Act
        _service.Delete(Guid.NewGuid());

        // Assert
        _repoMock.Verify(r => r.Delete(It.IsAny<User>()), Times.Never);
        _repoMock.Verify(r => r.SaveChanges(), Times.Never);
    }
}
