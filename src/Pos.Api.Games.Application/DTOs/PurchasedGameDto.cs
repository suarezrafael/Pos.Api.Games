namespace Pos.Api.Games.Application.DTOs;

public class PurchasedGameDto
{
    public int Id { get; set; }
    public GameDto Game { get; set; } = null!;
    public decimal PricePaid { get; set; }
    public DateTime PurchaseDate { get; set; }
}
