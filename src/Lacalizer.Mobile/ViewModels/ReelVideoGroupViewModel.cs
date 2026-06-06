
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;
using Plugin.Maui.ScreenRecording;

namespace Lacalizer.Mobile.ViewModels;

public partial class ReelVideoGroupViewModel : ObservableObject
{
    private readonly IVideoService _videoService;
    private readonly INavigationService _navigationService;
    private readonly ICommentService _commentService;

    private CancellationTokenSource _cts;

    public IAsyncRelayCommand LoadVideosCommand { get; }

    public IAsyncRelayCommand StartRecordingCommand { get; }

    [ObservableProperty]
    private ReelVideoGroupModel videoGroup;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string countdownText;

    [ObservableProperty]
    private bool isCountdownVisible;

    [ObservableProperty]
    private bool isRecording;

    public ReelVideoGroupViewModel(
        IVideoService videoService,
        INavigationService navigationService,
        ICommentService commentService)
    {
        _videoService = videoService;
        _navigationService = navigationService;
        _commentService = commentService;

        LoadVideosCommand = new AsyncRelayCommand(LoadVideosAsync);

        StartRecordingCommand = new AsyncRelayCommand(StartRecordingAsync);
    }


    private async Task LoadVideosAsync()
    {
        try
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert(
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

            // TAKE FIRST 4 VIDEOS
            var firstFour = items.Take(4).ToList();

            VideoGroup = new ReelVideoGroupModel
            {
                TopLeft = firstFour.ElementAtOrDefault(0),

                TopRight = firstFour.ElementAtOrDefault(1),

                BottomLeft = firstFour.ElementAtOrDefault(2),

                BottomRight = firstFour.ElementAtOrDefault(3),

                CenterText = firstFour.FirstOrDefault()?.ContextText
                             ?? "Translate this"
            };
        }
        catch (HttpRequestException ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "API Error",
                $"An error occurred:\n{ex.Message}",
                "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                ex.Message,
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }


    private async Task StartRecordingAsync()
    {
        if (VideoGroup == null)
            return;

        if (IsRecording)
            return;

        try
        {
            IsRecording = true;

            _cts = new CancellationTokenSource();

            ResetAllVideos(VideoGroup);

            IsCountdownVisible = true;

            for (int i = 3; i >= 0; i--)
            {
                CountdownText = i == 0
                    ? "START"
                    : i.ToString();

                await Task.Delay(1000, _cts.Token);
            }

            IsCountdownVisible = false;

            await ScreenRecording.Default.StartRecording();

            await PlaySequentially(VideoGroup, _cts.Token);

            var result =
                await ScreenRecording.Default.StopRecording();

            if (result != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Recording Saved",
                    $"Saved to:\n\n{result.FullPath}",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                ex.Message,
                "OK");
        }
        finally
        {
            IsRecording = false;

            IsCountdownVisible = false;

            ResetAllVideos(VideoGroup);
        }
    }


    public async Task PlaySequentially(
        ReelVideoGroupModel group,
        CancellationToken token)
    {
        if (group == null)
            return;

        if (group.IsPlayingSequence)
            return;

        try
        {
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
                token.ThrowIfCancellationRequested();

                ResetAllVideos(group);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    video.IsPlaying = true;
                });


                MainThread.BeginInvokeOnMainThread(() =>
                {
                    video.IsPlaying = false;
                });
            }
        }
        finally
        {
            ResetAllVideos(group);

            group.IsPlayingSequence = false;
        }
    }

    private void ResetAllVideos(ReelVideoGroupModel group)
    {
        if (group == null)
            return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (group.TopLeft != null)
                group.TopLeft.IsPlaying = false;

            if (group.TopRight != null)
                group.TopRight.IsPlaying = false;

            if (group.BottomLeft != null)
                group.BottomLeft.IsPlaying = false;

            if (group.BottomRight != null)
                group.BottomRight.IsPlaying = false;
        });
    }

    [RelayCommand]
    private void StopRecording()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
        }
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("//GroupReels");
    }
}





