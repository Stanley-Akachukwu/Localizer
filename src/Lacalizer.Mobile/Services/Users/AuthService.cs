
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

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Call backend API
            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/register",
                request);

            // If request failed
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadAsStringAsync();

                // Optional: log or show message
                Console.WriteLine($"Register failed: {error}");

                return false;
            }

            // Optional response parsing
            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse>();

            return result?.Success ?? true;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Network error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return false;
        }
    }
}