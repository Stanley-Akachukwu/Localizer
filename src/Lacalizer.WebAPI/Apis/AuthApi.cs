using Lacalizer.WebAPI.Auth;
using Lacalizer.WebAPI.Entites.Users;
using Lacalizer.WebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
     UserManager<ApplicationUser> userManager,
     RoleManager<IdentityRole> roleManager)
    {
        try
        {
            var existingUser = await userManager.Users
                .FirstOrDefaultAsync(x =>
                    x.PhoneNumber == request.PhoneNumber);

            const string roleName = "User";

            if (existingUser != null)
            {
                //return Results.BadRequest(new
                //{
                //    Message = "Phone already exists"
                //});

                // ✅ Ensure role exists

                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    var createRoleResult = await roleManager
                        .CreateAsync(new IdentityRole(roleName));

                    if (!createRoleResult.Succeeded)
                    {
                        return Results.BadRequest(new
                        {
                            Message = "Failed to create user role",
                            Errors = createRoleResult.Errors
                                .Select(x => x.Description)
                        });
                    }
                    return Results.Ok(new
                    {
                        Message = "Registration successful"
                    });
                }
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
                return Results.BadRequest(new
                {
                    Message = "Registration failed",
                    Errors = result.Errors.Select(x => x.Description)
                });
            }

            var roleResult = await userManager
                .AddToRoleAsync(user, roleName);

            if (!roleResult.Succeeded)
            {
                return Results.BadRequest(new
                {
                    Message = "User created but role assignment failed",
                    Errors = roleResult.Errors
                        .Select(x => x.Description)
                });
            }

            return Results.Ok(new
            {
                Message = "Registration successful"
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "An error occurred while processing the registration request.",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> Login(
    LoginRequest request,
    UserManager<ApplicationUser> userManager,
    JwtTokenGenerator jwtGenerator)
    {
        try
        {
            var user = await userManager.Users
                .FirstOrDefaultAsync(x =>
                    x.PhoneNumber == request.PhoneNumber);

            if (user == null)
            {
                return Results.NotFound(new
                {
                    Message = "User does not exist."
                });
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
        catch (Exception ex)
        {
            return Results.Problem(
                title: "An error occurred while processing the login request.",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
