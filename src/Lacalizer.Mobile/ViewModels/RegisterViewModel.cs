
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Users;
using Microsoft.Extensions.Logging;

namespace Lacalizer.Mobile.ViewModels;


public partial class RegisterViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly ILogger<RegisterViewModel> _logger;

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

    public RegisterViewModel(AuthService authService, ILogger<RegisterViewModel> logger)
    {
        _authService = authService;
        _logger = logger;
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
                await Shell.Current.DisplayAlert(
                    "Invalid Password",
                    error,
                    "OK");

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

            if (!result.Success)
            {
                var message = result.Errors.Any()
                    ? string.Join(Environment.NewLine, result.Errors)
                    : result.Message;

                await Shell.Current.DisplayAlert(
                    "Registration Failed",
                    message,
                    "OK");

                return;
            }

            await Shell.Current.DisplayAlert(
                "Success",
                result.Message,
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during registration.");

            await Shell.Current.DisplayAlert(
                "Error",
                "An unexpected error occurred. Please try again.",
                "OK");
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