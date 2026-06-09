using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Users;
using Lacalizer.Mobile.Services.Videos;

namespace Lacalizer.Mobile.ViewModels;

public partial class CreateContextViewModel : ObservableObject
{
    private readonly IContextService _contextService;
    private readonly AuthStateProvider _authStateProvider;
    public CreateContextViewModel(
    IContextService contextService,
    AuthStateProvider authStateProvider)
    {
        _contextService = contextService;
        _authStateProvider = authStateProvider;
    }
    [ObservableProperty]
    private string contextText = string.Empty;

    [ObservableProperty]
    private string userId = string.Empty;

    [ObservableProperty]
    private string targetLanguage = "Igbo";

    [RelayCommand]
    private async Task Save()
    {
        if (contextText == null)
            return;
        var contextModel = new ContextModel
        {
            ContextText = contextText
        };

        var authState = await _authStateProvider.GetStateAsync();

        if (authState.IsAuthenticated &&
            !string.IsNullOrWhiteSpace(authState.UserId) &&
            !string.IsNullOrWhiteSpace(authState.Email))
        {
            var userId = authState.UserId;
            var email = authState.Email;

            // continue normally
        }
        else
        {
            await _authStateProvider.LogoutAsync(); // optional cleanup
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        var created = await _contextService.PostContextAsync(contextModel, userId, targetLanguage);
        await Shell.Current.DisplayAlert(
            "Success",
            "Context created successfully",
            "OK");

        await Shell.Current.GoToAsync("..");
    }
}