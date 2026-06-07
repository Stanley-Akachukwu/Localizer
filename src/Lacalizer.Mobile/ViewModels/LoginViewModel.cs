using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Users;
using Lacalizer.Mobile.Views;

namespace Lacalizer.Mobile.ViewModels;


public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string phoneNumber;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private bool isBusy;

    public LoginViewModel(
        AuthService authService,
        INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task Login()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var success = await _authService.LoginAsync(
                PhoneNumber,
                Password);

            if (!success)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "Invalid credentials",
                    "OK");

                return;
            }

            //await _sessionService.SaveTokenAsync("sample-token");

            await Shell.Current.GoToAsync(nameof(MainPage));
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