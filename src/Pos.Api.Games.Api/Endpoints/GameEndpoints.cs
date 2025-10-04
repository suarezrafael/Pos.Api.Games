using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Games.Application.DTOs;
using Pos.Api.Games.Application.Services;

namespace Pos.Api.Games.Api;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/games").WithTags("Games");

        // Public endpoints
        group.MapGet("/", async ([FromServices] IGameService gameService) =>
        {
            var games = await gameService.GetAllGamesAsync();
            return Results.Ok(games);
        })
        .WithName("GetAllGames")
        .WithDescription("Get all games with promotional prices")
        .Produces(200);

        group.MapGet("/{id}", async (int id, [FromServices] IGameService gameService) =>
        {
            var game = await gameService.GetGameByIdAsync(id);
            return game != null ? Results.Ok(game) : Results.NotFound();
        })
        .WithName("GetGameById")
        .WithDescription("Get a game by ID")
        .Produces(200)
        .Produces(404);

        // Admin-only endpoints
        group.MapPost("/", async (
            [FromBody] CreateGameDto dto,
            [FromServices] IGameService gameService,
            [FromServices] IValidator<CreateGameDto> validator,
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

            var game = await gameService.CreateGameAsync(dto);
            return Results.Created($"/api/games/{game.Id}", game);
        })
        .RequireAuthorization()
        .WithName("CreateGame")
        .WithDescription("Create a new game (Admin only)")
        .Produces(201)
        .Produces(400)
        .Produces(403);

        group.MapPut("/{id}", async (
            int id,
            [FromBody] CreateGameDto dto,
            [FromServices] IGameService gameService,
            [FromServices] IValidator<CreateGameDto> validator,
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

            var success = await gameService.UpdateGameAsync(id, dto);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("UpdateGame")
        .WithDescription("Update a game (Admin only)")
        .Produces(204)
        .Produces(400)
        .Produces(403)
        .Produces(404);

        group.MapDelete("/{id}", async (
            int id,
            [FromServices] IGameService gameService,
            ClaimsPrincipal user) =>
        {
            if (user.FindFirst(ClaimTypes.Role)?.Value != "Admin")
            {
                return Results.Forbid();
            }

            var success = await gameService.DeleteGameAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("DeleteGame")
        .WithDescription("Delete a game (Admin only)")
        .Produces(204)
        .Produces(403)
        .Produces(404);

        // User endpoints
        group.MapPost("/{id}/purchase", async (
            int id,
            [FromServices] IGameService gameService,
            ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var success = await gameService.PurchaseGameAsync(userId, id);
            return success ? Results.Ok(new { message = "Game purchased successfully" }) : Results.BadRequest(new { error = "Unable to purchase game" });
        })
        .RequireAuthorization()
        .WithName("PurchaseGame")
        .WithDescription("Purchase a game")
        .Produces(200)
        .Produces(400)
        .Produces(401);

        group.MapGet("/library/my", async (
            [FromServices] IGameService gameService,
            ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var library = await gameService.GetUserLibraryAsync(userId);
            return Results.Ok(library);
        })
        .RequireAuthorization()
        .WithName("GetUserLibrary")
        .WithDescription("Get user's game library")
        .Produces(200)
        .Produces(401);
    }
}
