
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Users;

namespace Lacalizer.Mobile.ViewModels;


public partial class RegisterViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string firstName;

    [ObservableProperty]
    private string lastName;

    [ObservableProperty]
    private string phoneNumber;

    [ObservableProperty]
    private string email;

    [ObservableProperty]

    private string password;
    [ObservableProperty]
    private bool isBusy;

    public RegisterViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task Register()
    {
        if (IsBusy)
            return;
        try
        {

            IsBusy = true;
            var (isValid, error) = PasswordValidator.Validate(Password);

            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Invalid Password", error, "OK");
                return;
            }

            var request = new RegisterRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email,
                Password = Password
            };

            var result = await _authService.RegisterAsync(request);
            await Shell.Current.DisplayAlert("Success", result, "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

}

public static class PasswordValidator
{
    public static (bool IsValid, string Error) Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password is required.");

        if (password.Length < 6)
            return (false, "Password must be at least 6 characters long.");

        if (!password.Any(char.IsUpper))
            return (false, "Password must contain at least one uppercase letter.");

        if (!password.Any(char.IsLower))
            return (false, "Password must contain at least one lowercase letter.");

        if (!password.Any(char.IsDigit))
            return (false, "Password must contain at least one digit.");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return (false, "Password must contain at least one non-alphanumeric character.");

        return (true, string.Empty);
    }
}