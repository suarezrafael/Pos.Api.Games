using Pos.Api.Games.Application.DTOs;

namespace Pos.Api.Games.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<bool> DeleteUserAsync(int id);
}
