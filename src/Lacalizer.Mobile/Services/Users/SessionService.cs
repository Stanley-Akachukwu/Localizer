namespace Lacalizer.Mobile.Services.Users;


public class SessionService
{
    private const string TokenKey = "auth_token";

    public async Task SaveTokenAsync(string token)
    {
        await SecureStorage.SetAsync(TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync(TokenKey);
    }

    public async Task<bool> IsLoggedInAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    public void Logout()
    {
        SecureStorage.Remove("auth_token");
    }

    public async Task LogoutAsync()
    {
        SecureStorage.Remove(TokenKey);

        await Shell.Current.GoToAsync("//LoginPage");
    }
}