using Pos.Api.Games.Application.DTOs;

namespace Pos.Api.Games.Application.Services;

public interface IGameService
{
    Task<IEnumerable<GameDto>> GetAllGamesAsync();
    Task<GameDto?> GetGameByIdAsync(int id);
    Task<GameDto> CreateGameAsync(CreateGameDto dto);
    Task<bool> UpdateGameAsync(int id, CreateGameDto dto);
    Task<bool> DeleteGameAsync(int id);
    Task<bool> PurchaseGameAsync(int userId, int gameId);
    Task<IEnumerable<PurchasedGameDto>> GetUserLibraryAsync(int userId);
}
