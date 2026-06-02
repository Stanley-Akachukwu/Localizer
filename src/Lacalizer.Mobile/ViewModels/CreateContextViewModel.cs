using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Videos;

namespace Lacalizer.Mobile.ViewModels;

public partial class CreateContextViewModel : ObservableObject
{
    private readonly IContextService _contextService;
    public CreateContextViewModel(IContextService contextService)
    {
        _contextService = contextService;
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

        var created = await _contextService.PostContextAsync(contextModel, UserId);
        await Shell.Current.DisplayAlert(
            "Success",
            "Context created successfully",
            "OK");

        await Shell.Current.GoToAsync("..");
    }
}