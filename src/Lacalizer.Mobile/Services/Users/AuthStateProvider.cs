

using Lacalizer.Mobile.Models;

namespace Lacalizer.Mobile.Services.Users;

public class AuthStateProvider
{
    private readonly ITokenProvider _tokenProvider;
    private readonly ICurrentUserService _currentUser;

    public event Action? OnAuthStateChanged;

    public bool IsAuthenticated => _tokenProvider.HasToken;

    public AuthStateProvider(
        ITokenProvider tokenProvider,
        ICurrentUserService currentUser)
    {
        _tokenProvider = tokenProvider;
        _currentUser = currentUser;
    }
    public async Task<AuthState> GetStateAsync()
    {
        await _currentUser.LoadAsync();

        return new AuthState
        {
            IsAuthenticated = await IsAuthenticatedAsync(),
            UserId = _currentUser.UserId,
            Email = _currentUser.Email,
            Username = _currentUser.Username
        };
    }
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _tokenProvider.GetTokenAsync();

        return !string.IsNullOrWhiteSpace(token);
    }
    public async Task InitializeAsync()
    {
        await _currentUser.LoadAsync();
        Notify();
    }

    public async Task SaveTokenAsync(string token)
    {
        await _tokenProvider.SetTokenAsync(token);
        await _currentUser.LoadAsync();
        Notify();
    }

    public async Task LogoutAsync()
    {
        await _tokenProvider.ClearAsync();
        _currentUser.Clear();
        Notify();

        await Shell.Current.GoToAsync("//LoginPage");
    }

    private void Notify() => OnAuthStateChanged?.Invoke();
}