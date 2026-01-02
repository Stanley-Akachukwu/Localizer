
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.ViewModels;


public partial class ReelViewModel : ObservableObject
{
    private readonly IVideoService _videoService;
    private readonly INavigationService _navigationService;
    private readonly ICommentService _commentService;


    [ObservableProperty]
    private ObservableCollection<ReelVideoModel> videos;

    [ObservableProperty]
    private bool isLoading;
    [ObservableProperty]
    private string selectedTopic;
    [ObservableProperty]
    private string videoTopicId;
    [ObservableProperty]
    private string videoItemId;
    [ObservableProperty]
    private ReelVideoModel selectedVideo;
    public ReelViewModel(IVideoService videoService, INavigationService navigationService, ICommentService commentService)
    {
        _videoService = videoService;
        _navigationService = navigationService;
        _commentService = commentService;
        LoadVideosCommand = new AsyncRelayCommand(LoadVideosAsync);
    }

    [RelayCommand]
    private async Task RecordVideoAsync()
    {
        await _navigationService.GoToAsync(
            $"{Routes.LocalizePage}?topic={SelectedTopic}&videoTopicId={VideoTopicId}&videoItemId={VideoItemId}"
        );
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

            var items = await _videoService.GetTopicVideosAsync(1, 100);

            // Add default counter values
            foreach (var vid in items)
            {
                vid.VideoItemId = vid.VideoItemId;
                vid.LikesCount = vid.SavedLikes;
                vid.CommentCount = vid.SavedComments;
                vid.ShareCount = vid.SavedShares;
                vid.ParticipantsCount = vid.SavedParticipants;
                vid.ParentViewModel = this;
                vid.VideoService = _videoService;
                vid.CommentService = _commentService;
            }

            Videos = new ObservableCollection<ReelVideoModel>(items);
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
    private async Task OpenCommentsAsync(ReelVideoModel video)
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
