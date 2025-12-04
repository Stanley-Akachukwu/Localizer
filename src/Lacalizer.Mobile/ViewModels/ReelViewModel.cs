
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Videos;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.ViewModels;

//public partial class ReelViewModel : ObservableObject
//{
//    private readonly IVideoService _videoService;

//    private bool _isLoading;
//    public bool IsLoading
//    {
//        get => _isLoading;
//        set { _isLoading = value; OnPropertyChanged(); }
//    }

//    [ObservableProperty]
//    private ObservableCollection<VideoModel> _videos;
//    [ObservableProperty]
//    private string selectedTopic;
//    [ObservableProperty]
//    private string videoTopicId;
//    public ReelViewModel(IVideoService videoService)
//    {
//        _videoService = videoService;

//        LoadVideosCommand = new AsyncRelayCommand(LoadVideosAsync);
//    }

//    public IAsyncRelayCommand LoadVideosCommand { get; }

//    private async Task LoadVideosAsync()  
//    {
//        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

//        if (accessType != NetworkAccess.Internet)
//        {
//            await Application.Current.MainPage.DisplayAlertAsync("No Internet", "Please check your internet connection.", "OK");
//            return;
//        }

//        try
//        {
//            IsLoading = true;
//            var items = await _videoService.GetTopicVideosAsync(1, 100);
//            Videos = new ObservableCollection<VideoModel>(items);
//            IsLoading = false; 
//        }
//        catch (HttpRequestException e)
//        {
//            await Application.Current.MainPage.DisplayAlertAsync("API Error", $"An error occurred: {e.Message}", "OK");
//            IsLoading = false; 
//            return; 
//        }
//    }


//    [RelayCommand]
//    async Task GoBack()
//    {
//        await Shell.Current.GoToAsync("..");
//    }
//}

public partial class ReelViewModel : ObservableObject
{
    private readonly IVideoService _videoService;

    [ObservableProperty]
    private ObservableCollection<VideoModel> videos;

    [ObservableProperty]
    private bool isLoading;
    [ObservableProperty]
    private string selectedTopic;
    [ObservableProperty]
    private string videoTopicId;
    public ReelViewModel(IVideoService videoService)
    {
        _videoService = videoService;
        LoadVideosCommand = new AsyncRelayCommand(LoadVideosAsync);
    }

    public IAsyncRelayCommand LoadVideosCommand { get; }

    private async Task LoadVideosAsync()
    {
        try
        {
            IsLoading = true;

            var items = await _videoService.GetTopicVideosAsync(1, 100);

            // Add default counter values
            foreach (var vid in items)
            {
                vid.LikesCount = Random.Shared.Next(1, 50);
                vid.CommentCount = Random.Shared.Next(1, 20);
                vid.ShareCount = Random.Shared.Next(1, 10);
                vid.ParticipantsCount = Random.Shared.Next(1, 30);
            }

            Videos = new ObservableCollection<VideoModel>(items);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
