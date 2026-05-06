
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;

namespace Lacalizer.Mobile.ViewModels;

 
public partial class ReelVideoGroupViewModel : ObservableObject
{
    private readonly IVideoService _videoService;
    private readonly INavigationService _navigationService;
    private readonly ICommentService _commentService;

    private CancellationTokenSource _cts;


    [ObservableProperty]
    private ReelVideoGroupModel videoGroup;

    [ObservableProperty]
    private bool isLoading;
    public ReelVideoGroupViewModel(IVideoService videoService, INavigationService navigationService, ICommentService commentService)
    {
        _videoService = videoService;
        _navigationService = navigationService;
        _commentService = commentService;
        LoadVideosCommand = new AsyncRelayCommand(LoadVideosAsync);
    }

    public IAsyncRelayCommand LoadVideosCommand { get; }
    private async Task LoadVideosAsync()
    {
        try
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlertAsync(
                    "No Internet",
                    "Please check your internet connection.",
                    "OK");
                return;
            }

            IsLoading = true;

            var items = await _videoService.GetTopicVideosAsync(1, 100);

            foreach (var vid in items)
            {
                vid.VideoService = _videoService;
                vid.CommentService = _commentService;
                vid.NavigationService = _navigationService;
            }

            // ✅ TAKE ONLY FIRST 4
            var firstFour = items.Take(4).ToList();

            VideoGroup = new ReelVideoGroupModel
            {
                TopLeft = firstFour.ElementAtOrDefault(0),
                TopRight = firstFour.ElementAtOrDefault(1),
                BottomLeft = firstFour.ElementAtOrDefault(2),
                BottomRight = firstFour.ElementAtOrDefault(2),
                CenterText = firstFour.FirstOrDefault()?.Topic ?? "Translate this"
            };
        }
        catch (HttpRequestException e)
        {
            await Application.Current.MainPage.DisplayAlertAsync(
                "API Error",
                $"An error occurred: {e.Message}",
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
    public async Task PlaySequentially(ReelVideoGroupModel group)
    {
        if (group == null) return;

        if (group.IsPlayingSequence)
            return;

        group.IsPlayingSequence = true;

        var videos = new[]
        {
        group.TopLeft,
        group.TopRight,
        group.BottomLeft,
        group.BottomRight
    }
        .Where(v => v != null)
        .ToList();

        foreach (var video in videos)
        {
            video.IsPlaying = true;

            await Task.Delay(TimeSpan.FromSeconds(10));

            video.IsPlaying = false;
        }

        group.IsPlayingSequence = false;
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("//GroupReels");
    }
}
