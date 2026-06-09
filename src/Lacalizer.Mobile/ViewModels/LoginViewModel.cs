using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Users;
using Lacalizer.Mobile.Views;

namespace Lacalizer.Mobile.ViewModels;


public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;

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

            
            await Shell.Current.GoToAsync(nameof(MainPage));
        }
        catch (Exception ex)
        {

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