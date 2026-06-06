
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Lacalizer.Mobile.Services.Users;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? Username { get; }

    Task LoadAsync();
    void Clear();
}

public class CurrentUserService : ICurrentUserService
{
    private readonly ITokenProvider _tokenProvider;

    public string? UserId { get; private set; }
    public string? Email { get; private set; }
    public string? Username { get; private set; }

    public CurrentUserService(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public async Task LoadAsync()
    {
        var token = await _tokenProvider.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return;

        var handler = new JwtSecurityTokenHandler();

        JwtSecurityToken jwt;
        try
        {
            jwt = handler.ReadJwtToken(token);
        }
        catch
        {
            return;
        }

        UserId = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
        Email = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
        Username = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
    }

    public void Clear()
    {
        UserId = null;
        Email = null;
        Username = null;
    }
}