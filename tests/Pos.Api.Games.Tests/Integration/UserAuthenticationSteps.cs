using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Moq;
using Pos.Api.Games.Application.DTOs;
using Pos.Api.Games.Application.Services;
using Pos.Api.Games.Application.Validators;
using Pos.Api.Games.Domain.Entities;
using Pos.Api.Games.Domain.Interfaces;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Pos.Api.Games.Tests.Integration;

[Binding]
public class UserAuthenticationSteps
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterUserDto> _registerValidator;
    private RegisterUserDto? _registerDto;
    private (bool Success, string? Token, string? Error) _registerResult;
    private (bool Success, string? Token, string? Error) _loginResult;
    private string? _registeredEmail;
    private string? _registeredPassword;

    public UserAuthenticationSteps()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _configurationMock = new Mock<IConfiguration>();
        
        // Setup configuration mock
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("YourSuperSecretKeyForJwtTokenGeneration123!MustBeAtLeast32Characters");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("Pos.Api.Games");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("Pos.Api.Games");
        
        _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object);
        _registerValidator = new RegisterUserValidator();
    }

    [Given(@"the authentication service is available")]
    public void GivenTheAuthenticationServiceIsAvailable()
    {
        // Service is initialized in constructor
    }

    [Given(@"I have a valid registration request with:")]
    public void GivenIHaveAValidRegistrationRequestWith(Table table)
    {
        _registerDto = new RegisterUserDto
        {
            Name = table.Rows[0]["Value"],
            Email = table.Rows[1]["Value"],
            Password = table.Rows[2]["Value"]
        };
    }

    [Given(@"I have a registration request with weak password:")]
    public void GivenIHaveARegistrationRequestWithWeakPassword(Table table)
    {
        _registerDto = new RegisterUserDto
        {
            Name = table.Rows[0]["Value"],
            Email = table.Rows[1]["Value"],
            Password = table.Rows[2]["Value"]
        };
    }

    [Given(@"I have registered a user with email ""(.*)"" and password ""(.*)""")]
    public async Task GivenIHaveRegisteredAUserWithEmailAndPassword(string email, string password)
    {
        _registeredEmail = email;
        _registeredPassword = password;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = 1,
            Email = email,
            PasswordHash = passwordHash,
            Name = "Test User",
            Role = UserRole.User
        };

        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new[] { user });

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);
    }

    [When(@"I submit the registration request")]
    public async Task WhenISubmitTheRegistrationRequest()
    {
        if (_registerDto == null) throw new InvalidOperationException("Registration DTO not initialized");

        var validationResult = await _registerValidator.ValidateAsync(_registerDto);
        if (!validationResult.IsValid)
        {
            _registerResult = (false, null, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            return;
        }

        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(false);
        
        userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _registerResult = await _authService.RegisterAsync(_registerDto);
    }

    [When(@"I login with email ""(.*)"" and password ""(.*)""")]
    public async Task WhenILoginWithEmailAndPassword(string email, string password)
    {
        var loginDto = new LoginDto
        {
            Email = email,
            Password = password
        };

        _loginResult = await _authService.LoginAsync(loginDto);
    }

    [Then(@"the registration should be successful")]
    public void ThenTheRegistrationShouldBeSuccessful()
    {
        _registerResult.Success.Should().BeTrue();
    }

    [Then(@"the registration should fail")]
    public void ThenTheRegistrationShouldFail()
    {
        _registerResult.Success.Should().BeFalse();
    }

    [Then(@"I should receive a JWT token")]
    public void ThenIShouldReceiveAJWTToken()
    {
        if (_registerResult.Token != null)
        {
            _registerResult.Token.Should().NotBeNullOrEmpty();
        }
        else if (_loginResult.Token != null)
        {
            _loginResult.Token.Should().NotBeNullOrEmpty();
        }
        else
        {
            throw new InvalidOperationException("No token received from any operation");
        }
    }

    [Then(@"I should receive a password validation error")]
    public void ThenIShouldReceiveAPasswordValidationError()
    {
        _registerResult.Error.Should().NotBeNullOrEmpty();
        _registerResult.Error.Should().Contain("Password");
    }

    [Then(@"the login should be successful")]
    public void ThenTheLoginShouldBeSuccessful()
    {
        _loginResult.Success.Should().BeTrue();
    }

    [Then(@"the login should fail")]
    public void ThenTheLoginShouldFail()
    {
        _loginResult.Success.Should().BeFalse();
    }

    [Then(@"I should receive an authentication error")]
    public void ThenIShouldReceiveAnAuthenticationError()
    {
        _loginResult.Error.Should().NotBeNullOrEmpty();
        _loginResult.Error.Should().Contain("Invalid");
    }
}
