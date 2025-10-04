namespace Pos.Api.Games.Domain.Entities;

public class PurchasedGame
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public decimal PricePaid { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    
    public virtual User User { get; set; } = null!;
    public virtual Game Game { get; set; } = null!;
}
