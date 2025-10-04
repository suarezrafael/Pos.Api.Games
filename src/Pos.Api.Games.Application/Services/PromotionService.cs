using Pos.Api.Games.Application.DTOs;
using Pos.Api.Games.Domain.Entities;
using Pos.Api.Games.Domain.Interfaces;

namespace Pos.Api.Games.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _unitOfWork;

    public PromotionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync()
    {
        var promotions = await _unitOfWork.Promotions.GetAllAsync();
        return promotions.Select(p => new PromotionDto
        {
            Id = p.Id,
            GameId = p.GameId,
            Name = p.Name,
            DiscountPercentage = p.DiscountPercentage,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            IsActive = p.IsActive
        });
    }

    public async Task<PromotionDto?> GetPromotionByIdAsync(int id)
    {
        var promotion = await _unitOfWork.Promotions.GetByIdAsync(id);
        if (promotion == null) return null;

        return new PromotionDto
        {
            Id = promotion.Id,
            GameId = promotion.GameId,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            IsActive = promotion.IsActive
        };
    }

    public async Task<PromotionDto> CreatePromotionAsync(CreatePromotionDto dto)
    {
        var promotion = new Promotion
        {
            GameId = dto.GameId,
            Name = dto.Name,
            DiscountPercentage = dto.DiscountPercentage,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Promotions.AddAsync(promotion);
        await _unitOfWork.SaveChangesAsync();

        return new PromotionDto
        {
            Id = promotion.Id,
            GameId = promotion.GameId,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            IsActive = promotion.IsActive
        };
    }

    public async Task<bool> DeletePromotionAsync(int id)
    {
        var promotion = await _unitOfWork.Promotions.GetByIdAsync(id);
        if (promotion == null) return false;

        await _unitOfWork.Promotions.DeleteAsync(promotion);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync()
    {
        var now = DateTime.UtcNow;
        var promotions = await _unitOfWork.Promotions.FindAsync(p => 
            p.IsActive && p.StartDate <= now && p.EndDate >= now);

        return promotions.Select(p => new PromotionDto
        {
            Id = p.Id,
            GameId = p.GameId,
            Name = p.Name,
            DiscountPercentage = p.DiscountPercentage,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            IsActive = p.IsActive
        });
    }
}
