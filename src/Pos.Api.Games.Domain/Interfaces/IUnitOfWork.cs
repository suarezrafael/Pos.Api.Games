using Pos.Api.Games.Domain.Entities;

namespace Pos.Api.Games.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Game> Games { get; }
    IRepository<PurchasedGame> PurchasedGames { get; }
    IRepository<Promotion> Promotions { get; }
    Task<int> SaveChangesAsync();
}
