using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Games.Application.DTOs;
using Pos.Api.Games.Application.Services;

namespace Pos.Api.Games.Api;

public static class PromotionEndpoints
{
    public static void MapPromotionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/promotions").WithTags("Promotions");

        // Public endpoints
        group.MapGet("/", async ([FromServices] IPromotionService promotionService) =>
        {
            var promotions = await promotionService.GetAllPromotionsAsync();
            return Results.Ok(promotions);
        })
        .WithName("GetAllPromotions")
        .WithDescription("Get all promotions")
        .Produces(200);

        group.MapGet("/active", async ([FromServices] IPromotionService promotionService) =>
        {
            var promotions = await promotionService.GetActivePromotionsAsync();
            return Results.Ok(promotions);
        })
        .WithName("GetActivePromotions")
        .WithDescription("Get active promotions")
        .Produces(200);

        group.MapGet("/{id}", async (int id, [FromServices] IPromotionService promotionService) =>
        {
            var promotion = await promotionService.GetPromotionByIdAsync(id);
            return promotion != null ? Results.Ok(promotion) : Results.NotFound();
        })
        .WithName("GetPromotionById")
        .WithDescription("Get a promotion by ID")
        .Produces(200)
        .Produces(404);

        // Admin-only endpoints
        group.MapPost("/", async (
            [FromBody] CreatePromotionDto dto,
            [FromServices] IPromotionService promotionService,
            [FromServices] IValidator<CreatePromotionDto> validator,
            ClaimsPrincipal user) =>
        {
            if (user.FindFirst(ClaimTypes.Role)?.Value != "Admin")
            {
                return Results.Forbid();
            }

            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            var promotion = await promotionService.CreatePromotionAsync(dto);
            return Results.Created($"/api/promotions/{promotion.Id}", promotion);
        })
        .RequireAuthorization()
        .WithName("CreatePromotion")
        .WithDescription("Create a new promotion (Admin only)")
        .Produces(201)
        .Produces(400)
        .Produces(403);

        group.MapDelete("/{id}", async (
            int id,
            [FromServices] IPromotionService promotionService,
            ClaimsPrincipal user) =>
        {
            if (user.FindFirst(ClaimTypes.Role)?.Value != "Admin")
            {
                return Results.Forbid();
            }

            var success = await promotionService.DeletePromotionAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("DeletePromotion")
        .WithDescription("Delete a promotion (Admin only)")
        .Produces(204)
        .Produces(403)
        .Produces(404);
    }
}
