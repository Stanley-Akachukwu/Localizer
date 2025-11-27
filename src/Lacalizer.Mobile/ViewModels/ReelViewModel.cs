
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Videos;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.ViewModels;

public partial class ReelViewModel : ObservableObject
{
    private readonly IVideoService _videoService;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    [ObservableProperty]
    private ObservableCollection<VideoModel> _videos;
    [ObservableProperty]
    private string selectedTopic;

    public ReelViewModel(IVideoService videoService)
    {
        _videoService = videoService;

        LoadVideosCommand = new AsyncRelayCommand(LoadVideosAsync);
    }

    public IAsyncRelayCommand LoadVideosCommand { get; }

    private async Task LoadVideosAsync()
    {
        IsLoading = true;
        var items = await _videoService.GetVideosAsync(1, 100);
        Videos = new ObservableCollection<VideoModel>(items);
        IsLoading = false;
        
    }
}