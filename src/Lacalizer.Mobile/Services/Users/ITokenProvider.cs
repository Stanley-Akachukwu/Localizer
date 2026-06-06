
namespace Lacalizer.Mobile.Services.Users;

public interface ITokenProvider
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task ClearAsync();
    bool HasToken { get; }
}

public class TokenProvider : ITokenProvider
{
    private const string TokenKey = "auth_token";
    private string? _cachedToken;

    public bool HasToken => !string.IsNullOrWhiteSpace(_cachedToken);

    public async Task<string?> GetTokenAsync()
    {
        if (!string.IsNullOrWhiteSpace(_cachedToken))
            return _cachedToken;

        _cachedToken = await SecureStorage.GetAsync(TokenKey);
        return _cachedToken;
    }

    public async Task SetTokenAsync(string token)
    {
        _cachedToken = token;
        await SecureStorage.SetAsync(TokenKey, token);
    }

    public async Task ClearAsync()
    {
        _cachedToken = null;
        SecureStorage.Remove(TokenKey);
        await Task.CompletedTask;
    }
}