using CommunityToolkit.Mvvm.ComponentModel;

namespace Lacalizer.Mobile.Models;

public partial class VideoModel(string title, string topic, string videoUri) : ObservableObject
{
    public string Title { get; set; } = title;
    public string Topic { get; set; } = topic;
    public string VideoUri { get; set; } = videoUri;

    [ObservableProperty]
    private bool _isPlaying;
}