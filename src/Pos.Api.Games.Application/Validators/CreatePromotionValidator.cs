using FluentValidation;
using Pos.Api.Games.Application.DTOs;

namespace Pos.Api.Games.Application.Validators;

public class CreatePromotionValidator : AbstractValidator<CreatePromotionDto>
{
    public CreatePromotionValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0).WithMessage("GameId must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0).WithMessage("Discount percentage must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Discount percentage must not exceed 100");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");
    }
}
