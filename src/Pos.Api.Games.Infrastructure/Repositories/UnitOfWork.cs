using Pos.Api.Games.Domain.Entities;
using Pos.Api.Games.Domain.Interfaces;
using Pos.Api.Games.Infrastructure.Data;

namespace Pos.Api.Games.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IRepository<User>? _users;
    private IRepository<Game>? _games;
    private IRepository<PurchasedGame>? _purchasedGames;
    private IRepository<Promotion>? _promotions;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Game> Games => _games ??= new Repository<Game>(_context);
    public IRepository<PurchasedGame> PurchasedGames => _purchasedGames ??= new Repository<PurchasedGame>(_context);
    public IRepository<Promotion> Promotions => _promotions ??= new Repository<Promotion>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
