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
            try
            {
              var rsp =  await _contextService.PostContextAsync(
                    contextModel,
                    authState.UserId,
                    targetLanguage);
                if (rsp != null)
                {
                    if (!rsp.IsSuccess)
                    {
                        await Shell.Current.DisplayAlert(
                            "Error",
                            rsp.ErrorMessage ?? "An error occurred while creating the context.",
                            "OK");
                        return;
                    }
                }

                await Shell.Current.DisplayAlert(
                    "Success",
                    "Context created successfully.",
                    "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    ex.Message,
                    "OK");
            }
        }
        else
        {
            await Shell.Current.GoToAsync("LoginPage");
            return;
        }
    }
}