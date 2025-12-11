using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Videos;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.ViewModels;
 
public partial class ParticipationViewModel : ObservableObject
{
    private readonly IVideoService _videoService;
    private readonly INavigationService _navigationService;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    [ObservableProperty]
    private ObservableCollection<ParticipationVideoModel> _videos;
    [ObservableProperty]
    private string selectedTopic;

    [ObservableProperty]
    private string videoTopicId;
    [ObservableProperty]
    private ParticipationVideoModel selectedVideo;
    public ParticipationViewModel(IVideoService videoService, INavigationService navigationService)
    {
        _videoService = videoService;
        _navigationService = navigationService;
        LoadVideosCommand = new AsyncRelayCommand(LoadVideosAsync);
    }

    public IAsyncRelayCommand LoadVideosCommand { get; }

    private async Task LoadVideosAsync()
    {
        try
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlertAsync("No Internet", "Please check your internet connection.", "OK");
                return;
            }
            IsLoading = true;

            var items = await _videoService.GetParticipationVideosAsync(1, 100, VideoTopicId);

            // Add default counter values
            foreach (var vid in items)
            {
                vid.LikesCount = Random.Shared.Next(1, 50);
                vid.CommentCount = Random.Shared.Next(1, 20);
                vid.ShareCount = Random.Shared.Next(1, 10);
                vid.ParticipantsCount = Random.Shared.Next(1, 30);
                vid.ParentViewModel = this;
                vid.VideoService = _videoService;
            }

            Videos = new ObservableCollection<ParticipationVideoModel>(items);
        }
        catch (HttpRequestException e)
        {
            await Application.Current.MainPage.DisplayAlertAsync("API Error", $"An error occurred: {e.Message}", "OK");
            IsLoading = false;
            return;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task OpenCommentsAsync(ParticipationVideoModel video)
    {
        SelectedVideo = video;     // ⬅ Important!

        video.IsCommentsVisible = true;
        video.IsPlaying = false;

        var panel = Shell.Current.CurrentPage.FindByName<Frame>("CommentPanel");
        if (panel != null)
        {
            panel.IsVisible = true;
            await panel.TranslateToAsync(0, 0, 250, Easing.SinOut);
        }
    }


    [RelayCommand]
    private async Task CloseCommentsAsync()
    {
        if (SelectedVideo == null)
            return;

        var panel = Shell.Current.CurrentPage.FindByName<Frame>("CommentPanel");

        if (panel != null)
        {
            await panel.TranslateToAsync(0, 500, 250, Easing.SinIn);
            panel.IsVisible = false;
        }

        SelectedVideo.IsCommentsVisible = false;
        SelectedVideo.IsPlaying = true;

        SelectedVideo = null;
    }


    [RelayCommand]
    private async Task BackAsync()
    {
        await _navigationService.GoToAsync(Routes.ReelPage);
    }
}