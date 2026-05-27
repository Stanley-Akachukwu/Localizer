
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

    public RegisterViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task Register()
    {
        var request = new RegisterRequest
        {
            FirstName = FirstName,
            LastName = LastName,
            PhoneNumber = PhoneNumber,
            Email = Email,
            Password = Password
        };

        var success = await _authService.RegisterAsync(request);

        if (success)
        {
            await Shell.Current.DisplayAlert(
                "Success",
                "Registration successful",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
    }
}