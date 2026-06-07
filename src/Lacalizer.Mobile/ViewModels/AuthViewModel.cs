using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Lacalizer.Mobile.ViewModels;

public partial class AuthViewModel : ObservableObject
{

    [RelayCommand]
    private async Task Logout()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}