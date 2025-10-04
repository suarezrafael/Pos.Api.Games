namespace Pos.Api.Games.Domain.Entities;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public virtual ICollection<PurchasedGame> PurchasedBy { get; set; } = new List<PurchasedGame>();
    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}
