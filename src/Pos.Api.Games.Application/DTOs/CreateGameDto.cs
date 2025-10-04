namespace Pos.Api.Games.Application.DTOs;

public class CreateGameDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Publisher { get; set; } = string.Empty;
}
