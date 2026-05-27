using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Services.Users;

namespace Lacalizer.Mobile.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly SessionService _sessionService;

    public AuthViewModel(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [RelayCommand]
    private async Task Logout()
    {
        _sessionService.Logout();

        await Shell.Current.GoToAsync("//LoginPage");
    }
}