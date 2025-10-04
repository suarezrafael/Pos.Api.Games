using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Games.Application.DTOs;
using Pos.Api.Games.Application.Services;

namespace Pos.Api.Games.Api;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", async (
            [FromBody] RegisterUserDto dto,
            [FromServices] IAuthService authService,
            [FromServices] IValidator<RegisterUserDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            var (success, token, error) = await authService.RegisterAsync(dto);
            
            if (!success)
            {
                return Results.BadRequest(new { error });
            }

            return Results.Ok(new { token });
        })
        .WithName("Register")
        .WithDescription("Register a new user")
        .Produces(200)
        .Produces(400);

        group.MapPost("/login", async (
            [FromBody] LoginDto dto,
            [FromServices] IAuthService authService) =>
        {
            var (success, token, error) = await authService.LoginAsync(dto);
            
            if (!success)
            {
                return Results.BadRequest(new { error });
            }

            return Results.Ok(new { token });
        })
        .WithName("Login")
        .WithDescription("Login with email and password")
        .Produces(200)
        .Produces(400);
    }
}
