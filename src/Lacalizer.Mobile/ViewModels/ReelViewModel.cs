
using CommunityToolkit.Mvvm.ComponentModel;
using Lacalizer.Mobile.Models;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.ViewModels;

public partial class ReelViewModel : ObservableObject
{
    private const string FrogVideo = "https://github.com/ewerspej/maui-samples/blob/main/assets/frog.mp4?raw=true";
    private const string BuckVideo = "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true";

    [ObservableProperty]
    private ObservableCollection<VideoModel> _videos;
    [ObservableProperty]
    private string selectedTopic;

    public ReelViewModel()
    {
        Videos =
        [
            new VideoModel("First",
            "Eze Adi hurried over his breakfast of cassava served with cold bitter-leaf soup. \r\nIt was all that remained of last night's supper. \r\nThen he put away the bowls from which he and his mother had eaten, and set off to the village of Ama, three miles away. Eze was going to school for the first time.",
            FrogVideo,
            Colors.Aqua),
            new VideoModel("Second",
            "A simulator is a machine, program, or device that imitates a real-life situation, typically for training, experimentation, or entertainment.",
            BuckVideo, Colors.Red),
        ];
    }
}