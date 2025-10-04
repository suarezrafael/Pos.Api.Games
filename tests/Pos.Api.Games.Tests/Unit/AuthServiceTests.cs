using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Pos.Api.Games.Application.DTOs;
using Pos.Api.Games.Application.Services;
using Pos.Api.Games.Domain.Entities;
using Pos.Api.Games.Domain.Interfaces;
using Xunit;

namespace Pos.Api.Games.Tests.Unit;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _configurationMock = new Mock<IConfiguration>();
        
        // Setup configuration mock
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("YourSuperSecretKeyForJwtTokenGeneration123!MustBeAtLeast32Characters");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("Pos.Api.Games");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("Pos.Api.Games");
        
        _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Test@123456"
        };

        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(false);
        
        userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "existing@example.com",
            Password = "Test@123456"
        };

        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Token.Should().BeNull();
        result.Error.Should().Be("Email already registered");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var password = "Test@123456";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        var user = new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.User
        };

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = password
        };

        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new[] { user });

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldReturnError()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Test@123456"
        };

        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(Array.Empty<User>());

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Token.Should().BeNull();
        result.Error.Should().Be("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var correctPassword = "Test@123456";
        var incorrectPassword = "Wrong@123456";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);
        
        var user = new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.User
        };

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = incorrectPassword
        };

        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new[] { user });

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Token.Should().BeNull();
        result.Error.Should().Be("Invalid email or password");
    }

    [Fact]
    public void GenerateJwtToken_ShouldGenerateValidToken()
    {
        // Arrange
        var userId = 1;
        var email = "test@example.com";
        var role = "User";

        // Act
        var token = _authService.GenerateJwtToken(userId, email, role);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts separated by dots
    }
}
