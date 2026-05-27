using Lacalizer.WebAPI.Auth;
using Lacalizer.WebAPI.Entites.Users;
using Lacalizer.WebAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace Lacalizer.WebAPI.Apis;

public static class AuthApi
{
    public static IEndpointRouteBuilder MapAuthAPI(
        this IEndpointRouteBuilder app)
    {

        var vApi = app.NewVersionedApi("auths");
        var group = vApi.MapGroup("api/auth")
         .WithTags("auth");
        

        group.MapPost("/register", Register);

        group.MapPost("/login", Login);

        return app;
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        UserManager<ApplicationUser> userManager)
    {
        var existingUser = userManager.Users
            .FirstOrDefault(x =>
                x.PhoneNumber == request.PhoneNumber);

        if (existingUser != null)
        {
            return Results.BadRequest(new
            {
                Message = "Phone already exists"
            });
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            UserName = request.PhoneNumber
        };

        var result = await userManager
            .CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Results.BadRequest(result.Errors);
        }

        await userManager.AddToRoleAsync(user, "User");

        return Results.Ok(new
        {
            Message = "Registration successful"
        });
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        UserManager<ApplicationUser> userManager,
        JwtTokenGenerator jwtGenerator)
    {
        var user = userManager.Users
            .FirstOrDefault(x =>
                x.PhoneNumber == request.PhoneNumber);

        if (user == null)
        {
            return Results.Unauthorized();
        }

        var validPassword = await userManager
            .CheckPasswordAsync(user, request.Password);

        if (!validPassword)
        {
            return Results.Unauthorized();
        }

        var token = jwtGenerator.Generate(user);

        return Results.Ok(token);
    }
}
