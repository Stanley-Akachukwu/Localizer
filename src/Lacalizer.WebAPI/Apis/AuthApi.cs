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
         .WithTags("Authentication");
        

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
            const string roleName = "User";

            if (await userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
            {
                return Results.BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "A user with this phone number already exists."
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                await userManager.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "A user with this email already exists."
                });
            }

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var createRoleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (!createRoleResult.Succeeded)
                {
                    return Results.BadRequest(new RegisterResponse
                    {
                        Success = false,
                        Message = "Unable to create the default user role.",
                        Errors = createRoleResult.Errors
                            .Select(e => e.Description)
                            .ToList()
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

            var createUserResult = await userManager.CreateAsync(user, request.Password);

            if (!createUserResult.Succeeded)
            {
                return Results.BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "User registration failed.",
                    Errors = createUserResult.Errors
                        .Select(e => e.Description)
                        .ToList()
                });
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, roleName);

            if (!addRoleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);

                return Results.BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "User registration failed while assigning the user role.",
                    Errors = addRoleResult.Errors
                        .Select(e => e.Description)
                        .ToList()
                });
            }

            return Results.Ok(new RegisterResponse
            {
                Success = true,
                Message = "Registration completed successfully."
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Registration Failed",
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
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

            if (user == null)
            {
                return Results.NotFound(new LoginResponse
                {
                    Success = false,
                    Message = "No account was found with the supplied phone number."
                });
            }

            var validPassword = await userManager.CheckPasswordAsync(user, request.Password);

            if (!validPassword)
            {
                return Results.BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid phone number or password."
                });
            }

            var token = jwtGenerator.Generate(user);

            return Results.Ok(new LoginResponse
            {
                Success = true,
                Token = token.Token,
                Expiration = token.Expiration,
                Message = "Login successful."
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Login Failed",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}


