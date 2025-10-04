using Pos.Api.Games.Application.DTOs;

namespace Pos.Api.Games.Application.Services;

public interface IAuthService
{
    Task<(bool Success, string? Token, string? Error)> RegisterAsync(RegisterUserDto dto);
    Task<(bool Success, string? Token, string? Error)> LoginAsync(LoginDto dto);
    string GenerateJwtToken(int userId, string email, string role);
}
