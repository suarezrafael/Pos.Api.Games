using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Games.Application.Services;

namespace Pos.Api.Games.Api;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        // Admin-only endpoints
        group.MapGet("/", async (
            [FromServices] IUserService userService,
            ClaimsPrincipal user) =>
        {
            if (user.FindFirst(ClaimTypes.Role)?.Value != "Admin")
            {
                return Results.Forbid();
            }

            var users = await userService.GetAllUsersAsync();
            return Results.Ok(users);
        })
        .RequireAuthorization()
        .WithName("GetAllUsers")
        .WithDescription("Get all users (Admin only)")
        .Produces(200)
        .Produces(403);

        group.MapGet("/{id}", async (
            int id,
            [FromServices] IUserService userService,
            ClaimsPrincipal user) =>
        {
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // User can access their own data or Admin can access any user
            if (userRole != "Admin" && (userIdClaim == null || !int.TryParse(userIdClaim, out var userId) || userId != id))
            {
                return Results.Forbid();
            }

            var userDto = await userService.GetUserByIdAsync(id);
            return userDto != null ? Results.Ok(userDto) : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("GetUserById")
        .WithDescription("Get a user by ID")
        .Produces(200)
        .Produces(403)
        .Produces(404);

        group.MapDelete("/{id}", async (
            int id,
            [FromServices] IUserService userService,
            ClaimsPrincipal user) =>
        {
            if (user.FindFirst(ClaimTypes.Role)?.Value != "Admin")
            {
                return Results.Forbid();
            }

            var success = await userService.DeleteUserAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("DeleteUser")
        .WithDescription("Delete a user (Admin only)")
        .Produces(204)
        .Produces(403)
        .Produces(404);
    }
}
