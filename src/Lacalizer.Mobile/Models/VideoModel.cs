using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lacalizer.Mobile.Models;

//public partial class VideoModel(string title, string topic, string videoUri, string videoTopicId) : ObservableObject
//{
//    public string Title { get; set; } = title;
//    public string Topic { get; set; } = topic;
//    public string VideoUri { get; set; } = videoUri;
//    public string VideoTopicId { get; set; } = videoTopicId;

//    [ObservableProperty]
//    private bool _isPlaying;
//}

public partial class VideoModel : ObservableObject
{
    public VideoModel(string title, string topic, string videoUri, string videoTopicId)
    {
        Title = title;
        Topic = topic;
        VideoUri = videoUri;
        VideoTopicId = videoTopicId;

        CommentsPanelTranslationY = 500; // start hidden
    }

    public string Title { get; set; }
    public string Topic { get; set; }
    public string VideoUri { get; set; }
    public string VideoTopicId { get; set; }

    // Video playing state
    [ObservableProperty]
    private bool isPlaying;


    // ───────────────────────────────────────────────────
    // COUNTERS
    // ───────────────────────────────────────────────────
    [ObservableProperty] private int participantsCount;
    [ObservableProperty] private int likesCount;
    [ObservableProperty] private int shareCount;
    [ObservableProperty] private int commentCount;


    // ───────────────────────────────────────────────────
    // COMMENTS PANEL UI STATE
    // ───────────────────────────────────────────────────
    [ObservableProperty] private bool isCommentsVisible;

    // Slide animation offset
    [ObservableProperty] private double commentsPanelTranslationY;


    // ───────────────────────────────────────────────────
    // COMMANDS
    // ───────────────────────────────────────────────────

    [RelayCommand]
    private async Task IncreaseLikesAsync()
    {
        LikesCount++;
        // await api.IncreaseLike(VideoTopicId);
    }

    [RelayCommand]
    private async Task IncreaseParticipantsAsync()
    {
        ParticipantsCount++;
        // await api.IncreaseParticipants(VideoTopicId);
    }

    [RelayCommand]
    private async Task IncreaseShareAsync()
    {
        ShareCount++;
        // await api.IncreaseShare(VideoTopicId);
    }

    [RelayCommand]
    private async Task OpenCommentsAsync()
    {
        IsCommentsVisible = true;
        IsPlaying = false; // pause video

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var panel = Shell.Current.CurrentPage.FindByName<Frame>("CommentPanel");
            if (panel != null)
            {
                panel.IsVisible = true;
                await panel.TranslateTo(0, 0, 250, Easing.SinOut);
            }
        });
    }

    [RelayCommand]
    private async Task CloseCommentsAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var panel = Shell.Current.CurrentPage.FindByName<Frame>("CommentPanel");
            if (panel != null)
            {
                await panel.TranslateTo(0, 500, 250, Easing.SinIn);
                panel.IsVisible = false;
            }
        });

        IsCommentsVisible = false;
        IsPlaying = true; // resume video
    }
}