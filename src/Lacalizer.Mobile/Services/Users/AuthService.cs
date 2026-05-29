
using Lacalizer.Mobile.Models;
using System.Net.Http.Json;

namespace Lacalizer.Mobile.Services.Users;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly SessionService _sessionService;

    public AuthService(
        HttpClient httpClient,
        SessionService sessionService)
    {
        _httpClient = httpClient;
        _sessionService = sessionService;
    }

    public async Task<bool> LoginAsync(
        string phone,
        string password)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            new
            {
                PhoneNumber = phone,
                Password = password
            });

        if (!response.IsSuccessStatusCode)
            return false;

        var result =
            await response.Content.ReadFromJsonAsync
            <LoginResponse>();

        await _sessionService
            .SaveTokenAsync(result!.Token);

        return true;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/register",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadAsStringAsync();
                return $"Register failed: {error}";
            }

            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse>();

            return result?.Message!;
        }
        catch (HttpRequestException ex)
        {
            return $"Network error: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Unexpected error: {ex.Message}";
        }
    }
}