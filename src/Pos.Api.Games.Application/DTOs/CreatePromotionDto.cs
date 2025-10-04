namespace Pos.Api.Games.Application.DTOs;

public class CreatePromotionDto
{
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
