using Pos.Api.Games.Application.DTOs;

namespace Pos.Api.Games.Application.Services;

public interface IPromotionService
{
    Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync();
    Task<PromotionDto?> GetPromotionByIdAsync(int id);
    Task<PromotionDto> CreatePromotionAsync(CreatePromotionDto dto);
    Task<bool> DeletePromotionAsync(int id);
    Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync();
}
