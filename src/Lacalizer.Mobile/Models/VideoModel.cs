using CommunityToolkit.Mvvm.ComponentModel;

namespace Lacalizer.Mobile.Models;

public partial class VideoModel(string title, string topic, string videoUri) : ObservableObject
{
    public string Title { get; } = title;
    public string Topic { get; } = topic;
    public string VideoUri { get; } = videoUri;

    [ObservableProperty]
    private bool _isPlaying;
}