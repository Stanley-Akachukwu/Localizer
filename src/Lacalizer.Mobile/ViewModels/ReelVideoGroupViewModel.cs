
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

    // =========================================
    // PROPERTIES
    // =========================================

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

    // =========================================
    // CONSTRUCTOR
    // =========================================

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

    // =========================================
    // LOAD VIDEOS
    // =========================================

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

                CenterText = firstFour.FirstOrDefault()?.Topic
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

    // =========================================
    // START RECORDING FLOW
    // =========================================

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

            // SHOW COUNTDOWN
            IsCountdownVisible = true;

            for (int i = 3; i >= 0; i--)
            {
                CountdownText = i == 0
                    ? "START"
                    : i.ToString();

                await Task.Delay(1000, _cts.Token);
            }

            IsCountdownVisible = false;

            // START SCREEN RECORDING
            await ScreenRecording.Default.StartRecording();

            // PLAY VIDEOS
            await PlaySequentially(VideoGroup, _cts.Token);

            // STOP SCREEN RECORDING
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

    // =========================================
    // PLAY VIDEOS SEQUENTIALLY
    // =========================================

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

                // RESET ALL VIDEOS
                ResetAllVideos(group);

                // START CURRENT VIDEO
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    video.IsPlaying = true;
                });

                // TEMPORARY DELAY
                // LATER REPLACE WITH ACTUAL VIDEO DURATION
                await Task.Delay(
                    TimeSpan.FromSeconds(10),
                    token);

                // STOP CURRENT VIDEO
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

    // =========================================
    // RESET VIDEOS
    // =========================================

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

    // =========================================
    // OPTIONAL STOP RECORDING
    // =========================================

    [RelayCommand]
    private void StopRecording()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
        }
    }

    // =========================================
    // BACK
    // =========================================

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("//GroupReels");
    }
}




//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using Lacalizer.Mobile.Models;
//using Lacalizer.Mobile.Navigation;
//using Lacalizer.Mobile.Services.Comments;
//using Lacalizer.Mobile.Services.Videos;
//using Plugin.Maui.ScreenRecording;


//namespace Lacalizer.Mobile.ViewModels;


//public partial class ReelVideoGroupViewModel : ObservableObject
//{
//    private readonly IVideoService _videoService;
//    private readonly INavigationService _navigationService;
//    private readonly ICommentService _commentService;

//    private CancellationTokenSource _cts;
//    public IAsyncRelayCommand LoadVideosCommand { get; }

//    public IAsyncRelayCommand StartRecordingCommand { get; }

//    [ObservableProperty]
//    private ReelVideoGroupModel videoGroup;

//    [ObservableProperty]
//    private bool isLoading;

//    [ObservableProperty]
//    private string countdownText;

//    [ObservableProperty]
//    private bool isCountdownVisible;
//    public ReelVideoGroupViewModel(IVideoService videoService, INavigationService navigationService, ICommentService commentService)
//    {
//        _videoService = videoService;
//        _navigationService = navigationService;
//        _commentService = commentService;
//        LoadVideosCommand = new AsyncRelayCommand(LoadVideosAsync);
//        StartRecordingCommand = new AsyncRelayCommand(StartRecordingAsync);
//    }

//    private async Task LoadVideosAsync()
//    {
//        try
//        {
//            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
//            {
//                await Application.Current.MainPage.DisplayAlertAsync(
//                    "No Internet",
//                    "Please check your internet connection.",
//                    "OK");
//                return;
//            }

//            IsLoading = true;

//            var items = await _videoService.GetTopicVideosAsync(1, 100);

//            foreach (var vid in items)
//            {
//                vid.VideoService = _videoService;
//                vid.CommentService = _commentService;
//                vid.NavigationService = _navigationService;
//            }

//            // ✅ TAKE ONLY FIRST 4
//            var firstFour = items.Take(4).ToList();

//            VideoGroup = new ReelVideoGroupModel
//            {
//                TopLeft = firstFour.ElementAtOrDefault(0),
//                TopRight = firstFour.ElementAtOrDefault(1),
//                BottomLeft = firstFour.ElementAtOrDefault(2),
//                BottomRight = firstFour.ElementAtOrDefault(2),
//                CenterText = firstFour.FirstOrDefault()?.Topic ?? "Translate this"
//            };
//        }
//        catch (HttpRequestException e)
//        {
//            await Application.Current.MainPage.DisplayAlertAsync(
//                "API Error",
//                $"An error occurred: {e.Message}",
//                "OK");
//        }
//        finally
//        {
//            IsLoading = false;
//        }
//    }


//    private async Task StartRecordingAsync()
//    {
//        if (VideoGroup == null)
//            return;

//        // SHOW COUNTDOWN
//        IsCountdownVisible = true;

//        for (int i = 3; i >= 0; i--)
//        {
//            CountdownText = i == 0 ? "START" : i.ToString();

//            await Task.Delay(1000);
//        }

//        IsCountdownVisible = false;

//        // START SCREEN RECORDING
//        await ScreenRecording.Default.StartRecordingAsync();

//        // PLAY VIDEOS
//        await PlaySequentially(VideoGroup);

//        // STOP RECORDING
//        var filePath = await ScreenRecording.Default.StopRecordingAsync();

//        if (!string.IsNullOrWhiteSpace(filePath))
//        {
//            await Application.Current.MainPage.DisplayAlert(
//                "Success",
//                $"Recording saved:\n{filePath}",
//                "OK");
//        }
//    }

//    public async Task PlaySequentially(ReelVideoGroupModel group)
//    {
//        if (group == null)
//            return;

//        if (group.IsPlayingSequence)
//            return;

//        group.IsPlayingSequence = true;

//        var videos = new[]
//        {
//        group.TopLeft,
//        group.TopRight,
//        group.BottomLeft,
//        group.BottomRight
//    }
//        .Where(v => v != null)
//        .ToList();

//        foreach (var video in videos)
//        {
//            ResetAllVideos(group);

//            video.IsPlaying = true;

//            // TEMPORARY
//            // later replace with actual video duration
//            await Task.Delay(TimeSpan.FromSeconds(10));

//            video.IsPlaying = false;
//        }

//        ResetAllVideos(group);

//        group.IsPlayingSequence = false;
//    }

//    private void ResetAllVideos(ReelVideoGroupModel group)
//    {
//        if (group.TopLeft != null)
//            group.TopLeft.IsPlaying = false;

//        if (group.TopRight != null)
//            group.TopRight.IsPlaying = false;

//        if (group.BottomLeft != null)
//            group.BottomLeft.IsPlaying = false;

//        if (group.BottomRight != null)
//            group.BottomRight.IsPlaying = false;
//    }


//    [RelayCommand]
//    private async Task BackAsync()
//    {
//        await Shell.Current.GoToAsync("//GroupReels");
//    }
//}
