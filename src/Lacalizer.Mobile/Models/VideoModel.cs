using CommunityToolkit.Mvvm.ComponentModel;

namespace Lacalizer.Mobile.Models;

public partial class VideoModel(string title, string topic, string videoUri, Color backgroundColor = default) : ObservableObject
{
    public string Title { get; } = title;
    public string Topic { get; } = topic;
    public string VideoUri { get; } = videoUri;
    public Color BackgroundColor { get; } = backgroundColor ?? Colors.Orange;

    [ObservableProperty]
    private bool _isPlaying;
}