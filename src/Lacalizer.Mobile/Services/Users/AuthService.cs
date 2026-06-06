
using Lacalizer.Mobile.Models;

namespace Lacalizer.Mobile.Services.Users;

public class AuthService
{
    private readonly IApiClient _apiClient;
    private readonly AuthStateProvider _authState;

    public AuthService(IApiClient apiClient, AuthStateProvider authState)
    {
        _apiClient = apiClient;
        _authState = authState;
    }

    public async Task<bool> LoginAsync(string phone, string password)
    {
        var result = await _apiClient.PostAsync<LoginRequest, LoginResponse>(
            "api/auth/login",
            new LoginRequest
            {
                PhoneNumber = phone,
                Password = password
            });

        if (string.IsNullOrWhiteSpace(result?.Token))
            return false;

        await _authState.LoginAsync(result.Token);
        return true;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        var result = await _apiClient.PostAsync<RegisterRequest, ApiResponse>(
            "api/auth/register",
            request);

        return result?.Message ?? "Unknown response";
    }
}

 