using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Users;
using Lacalizer.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace Lacalizer.Mobile.ViewModels;


public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty]
    private string phoneNumber;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private bool isBusy;

    public LoginViewModel(
        AuthService authService,
        INavigationService navigationService,
        ILogger<LoginViewModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }


    [RelayCommand]
    private async Task Login()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _logger.LogInformation("Calling authentication service.");

            var result = await _authService.LoginAsync(
                PhoneNumber,
                Password);

            if (!result.Success)
            {
                var message = result.Errors.Any()
                    ? string.Join(Environment.NewLine, result.Errors)
                    : result.Message;

                await Shell.Current.DisplayAlert(
                    "Login Failed",
                    message,
                    "OK");

                return;
            }

            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during login.");

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

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}