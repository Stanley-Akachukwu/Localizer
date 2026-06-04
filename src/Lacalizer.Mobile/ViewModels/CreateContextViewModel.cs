using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Users;
using Lacalizer.Mobile.Services.Videos;

namespace Lacalizer.Mobile.ViewModels;

public partial class CreateContextViewModel : ObservableObject
{
    private readonly IContextService _contextService;
    private readonly SessionService _sessionService;
    private readonly ICurrentUser _currentUser;
    public CreateContextViewModel(IContextService contextService, SessionService sessionService, ICurrentUser currentUser)
    {
        _contextService = contextService;
        _sessionService = sessionService;
        _currentUser = currentUser;
    }
    [ObservableProperty]
    private string contextText = string.Empty;

    [ObservableProperty]
    private string userId = string.Empty;

    [RelayCommand]
    private async Task Save()
    {
        if (contextText == null)
            return;
        var contextModel = new ContextModel
        {
            ContextText = contextText
        };

      //var token =  await _sessionService.GetTokenAsync();
      //  var userId = _currentUser.UserId;
      //  var email = _currentUser.Email;

        var created = await _contextService.PostContextAsync(contextModel, userId);
        await Shell.Current.DisplayAlert(
            "Success",
            "Context created successfully",
            "OK");

        await Shell.Current.GoToAsync("..");
    }
}