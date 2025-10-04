using Pos.Api.Games.Application.DTOs;
using Pos.Api.Games.Domain.Entities;
using Pos.Api.Games.Domain.Interfaces;

namespace Pos.Api.Games.Application.Services;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;

    public GameService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<GameDto>> GetAllGamesAsync()
    {
        var games = await _unitOfWork.Games.GetAllAsync();
        var promotions = await _unitOfWork.Promotions.GetAllAsync();
        var activePromotions = promotions.Where(p => p.IsActive && 
            p.StartDate <= DateTime.UtcNow && 
            p.EndDate >= DateTime.UtcNow).ToList();

        return games.Select(g =>
        {
            var promotion = activePromotions.FirstOrDefault(p => p.GameId == g.Id);
            var promotionalPrice = promotion != null 
                ? g.Price * (1 - promotion.DiscountPercentage / 100) 
                : (decimal?)null;

            return new GameDto
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Genre = g.Genre,
                Price = g.Price,
                PromotionalPrice = promotionalPrice,
                ReleaseDate = g.ReleaseDate,
                Publisher = g.Publisher
            };
        });
    }

    public async Task<GameDto?> GetGameByIdAsync(int id)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(id);
        if (game == null) return null;

        var promotions = await _unitOfWork.Promotions.FindAsync(p => 
            p.GameId == id && p.IsActive && 
            p.StartDate <= DateTime.UtcNow && 
            p.EndDate >= DateTime.UtcNow);
        
        var promotion = promotions.FirstOrDefault();
        var promotionalPrice = promotion != null 
            ? game.Price * (1 - promotion.DiscountPercentage / 100) 
            : (decimal?)null;

        return new GameDto
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            Genre = game.Genre,
            Price = game.Price,
            PromotionalPrice = promotionalPrice,
            ReleaseDate = game.ReleaseDate,
            Publisher = game.Publisher
        };
    }

    public async Task<GameDto> CreateGameAsync(CreateGameDto dto)
    {
        var game = new Game
        {
            Title = dto.Title,
            Description = dto.Description,
            Genre = dto.Genre,
            Price = dto.Price,
            ReleaseDate = dto.ReleaseDate,
            Publisher = dto.Publisher,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Games.AddAsync(game);
        await _unitOfWork.SaveChangesAsync();

        return new GameDto
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            Genre = game.Genre,
            Price = game.Price,
            ReleaseDate = game.ReleaseDate,
            Publisher = game.Publisher
        };
    }

    public async Task<bool> UpdateGameAsync(int id, CreateGameDto dto)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(id);
        if (game == null) return false;

        game.Title = dto.Title;
        game.Description = dto.Description;
        game.Genre = dto.Genre;
        game.Price = dto.Price;
        game.ReleaseDate = dto.ReleaseDate;
        game.Publisher = dto.Publisher;
        game.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Games.UpdateAsync(game);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteGameAsync(int id)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(id);
        if (game == null) return false;

        await _unitOfWork.Games.DeleteAsync(game);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> PurchaseGameAsync(int userId, int gameId)
    {
        // Check if user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return false;

        // Check if game exists
        var game = await _unitOfWork.Games.GetByIdAsync(gameId);
        if (game == null) return false;

        // Check if already purchased
        var alreadyPurchased = await _unitOfWork.PurchasedGames.ExistsAsync(pg => 
            pg.UserId == userId && pg.GameId == gameId);
        if (alreadyPurchased) return false;

        // Get active promotion if any
        var promotions = await _unitOfWork.Promotions.FindAsync(p => 
            p.GameId == gameId && p.IsActive && 
            p.StartDate <= DateTime.UtcNow && 
            p.EndDate >= DateTime.UtcNow);
        
        var promotion = promotions.FirstOrDefault();
        var pricePaid = promotion != null 
            ? game.Price * (1 - promotion.DiscountPercentage / 100) 
            : game.Price;

        var purchase = new PurchasedGame
        {
            UserId = userId,
            GameId = gameId,
            PricePaid = pricePaid,
            PurchaseDate = DateTime.UtcNow
        };

        await _unitOfWork.PurchasedGames.AddAsync(purchase);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<PurchasedGameDto>> GetUserLibraryAsync(int userId)
    {
        var purchases = await _unitOfWork.PurchasedGames.FindAsync(pg => pg.UserId == userId);
        var games = await _unitOfWork.Games.GetAllAsync();

        return purchases.Select(p =>
        {
            var game = games.FirstOrDefault(g => g.Id == p.GameId);
            return new PurchasedGameDto
            {
                Id = p.Id,
                Game = new GameDto
                {
                    Id = game!.Id,
                    Title = game.Title,
                    Description = game.Description,
                    Genre = game.Genre,
                    Price = game.Price,
                    ReleaseDate = game.ReleaseDate,
                    Publisher = game.Publisher
                },
                PricePaid = p.PricePaid,
                PurchaseDate = p.PurchaseDate
            };
        });
    }
}
