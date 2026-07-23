
using Lacalizer.Mobile.Models;
using Microsoft.Extensions.Logging;

namespace Lacalizer.Mobile.Services.Users;

public class AuthService
{
    private readonly IApiClient _apiClient;
    private readonly AuthStateProvider _authState;
    private readonly ILogger<AuthService> _logger;
    public AuthService(IApiClient apiClient, AuthStateProvider authState, ILogger<AuthService> logger)
    {
        _apiClient = apiClient;
        _authState = authState;
        _logger = logger;
    }



    public async Task<LoginResponse> LoginAsync(string phone, string password)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Phone number is required."
            };
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Password is required."
            };
        }

        try
        {
            var result = await _apiClient.PostAsync<LoginRequest, LoginResponse>(
                "api/auth/login",
                new LoginRequest
                {
                    PhoneNumber = phone,
                    Password = password
                });

            if (result == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "The server returned an empty response."
                };
            }

            if (!result.Success)
            {
                return result;
            }

            await _authState.SaveTokenAsync(result.Token!);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "HTTP error occurred while logging in user {Phone}.",
                phone);

            return new LoginResponse
            {
                Success = false,
                Message = "Unable to connect to the server."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex,
                "Login request timed out for user {Phone}.",
                phone);

            return new LoginResponse
            {
                Success = false,
                Message = "The request timed out."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error occurred while logging in user {Phone}.",
                phone);

            return new LoginResponse
            {
                Success = false,
                Message = "An unexpected error occurred."
            };
        }
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var result = await _apiClient.PostAsync<RegisterRequest, RegisterResponse>(
                "api/auth/register",
                request);

            return result ?? new RegisterResponse
            {
                Success = false,
                Message = "The server returned an empty response."
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "HTTP error occurred while registering user {Phone}.",
                request.PhoneNumber);

            return new RegisterResponse
            {
                Success = false,
                Message = "Unable to connect to the server."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex,
                "Registration request timed out for user {Phone}.",
                request.PhoneNumber);

            return new RegisterResponse
            {
                Success = false,
                Message = "The request timed out."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error occurred while registering user {Phone}.",
                request.PhoneNumber);

            return new RegisterResponse
            {
                Success = false,
                Message = "An unexpected error occurred."
            };
        }
    }
}

 