using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;
using Lacalizer.Mobile.Views;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.ViewModels;

public partial class ContextsViewModel : ObservableObject
{
    private readonly IContextService _contextService;

    public ObservableCollection<ContextModel> Contexts { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string userId;

    [ObservableProperty]
    private int pageIndex = 1;

    [ObservableProperty]
    private int pageSize = 10;

    [ObservableProperty]
    private long totalCount;

    public ContextsViewModel(
        IContextService contextService)
    {
        _contextService = contextService;
    }

    [RelayCommand]
    public async Task LoadContexts()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var response = await _contextService.GetContextsAsync(PageIndex,PageSize,string.Empty);

            if (!response.IsSuccess || response.Data == null)
                return;

            Contexts.Clear();

            foreach (var item in response.Data.Data)
                Contexts.Add(item);

            TotalCount = response.Data.Count;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task NextPage()
    {
        var maxPage = (int)Math.Ceiling(
            (double)TotalCount / PageSize);

        if (PageIndex >= maxPage)
            return;

        PageIndex++;

        await LoadContexts();
    }

    [RelayCommand]
    public async Task PreviousPage()
    {
        if (PageIndex <= 1)
            return;

        PageIndex--;

        await LoadContexts();
    }

    [RelayCommand]
    public async Task Localize(
        ContextModel context)
    {
        if (context == null)
            return;

        await Shell.Current.DisplayAlert(
            "Localize",
            context.ContextText,
            "OK");
    }

    [RelayCommand]
    public async Task CreateContext()
    {
        await Shell.Current.GoToAsync(
            nameof(CreateContextPage));
    }
}
