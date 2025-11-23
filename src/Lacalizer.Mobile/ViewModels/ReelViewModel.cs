
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Lacalizer.Mobile.ViewModels;

public partial class ReelViewModel : ObservableObject
{
    private const string FrogVideo = "https://github.com/ewerspej/maui-samples/blob/main/assets/frog.mp4?raw=true";
    private const string BuckVideo = "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true";

    [ObservableProperty]
    private ObservableCollection<VideoModel> _videos;

    public ReelViewModel()
    {
        Videos =
        [
            new VideoModel("First", FrogVideo, Colors.Aqua),
            new VideoModel("Second", BuckVideo, Colors.Red),
            new VideoModel("Third", FrogVideo, Colors.GreenYellow),
            new VideoModel("Fourth", BuckVideo, Colors.DarkSlateGray),
            new VideoModel("Fifth", FrogVideo, Colors.DeepSkyBlue),
            new VideoModel("Sixth", BuckVideo, Colors.Orange)
        ];
    }

    //[RelayCommand]
    //public ICommand RecordVideoCommand => new Command(async () => await RecordVideoAsync());

   
    //private async Task RecordVideo()
    //{
    //    // Request only CAMERA; microphone is implied
    //    var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();

    //    if (cameraStatus != PermissionStatus.Granted)
    //    {
    //        await Shell.Current.DisplayAlert("Permission Required", "Camera permission is needed to record video.", "OK");
    //        return;
    //    }

    //    if (!MediaPicker.Default.IsCaptureSupported)
    //    {
    //        await Shell.Current.DisplayAlert("Not Supported", "This device cannot record videos.", "OK");
    //        return;
    //    }

    //    FileResult result;

    //    try
    //    {
    //        result = await MediaPicker.Default.CaptureVideoAsync(new MediaPickerOptions
    //        {
    //            Title = "Record a new video"
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Capture Error: {ex.Message}");
    //        return;
    //    }

    //    if (result == null)
    //        return;

    //    // ----------------------------
    //    // SAVE RECORDED VIDEO LOCALLY
    //    // ----------------------------

    //    string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.mp4";
    //    string localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

    //    try
    //    {
    //        using var sourceStream = await result.OpenReadAsync();
    //        using var localFile = File.OpenWrite(localPath);
    //        await sourceStream.CopyToAsync(localFile);
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"File Save Error: {ex.Message}");
    //        return;
    //    }

    //    // ----------------------------
    //    // ADD NEW RECORDED VIDEO
    //    // ----------------------------
    //    //Videos.Insert(0,
    //    //    new VideoModel(
    //    //        title: "Recorded Video",
    //    //        url: localPath,
    //    //        color: Colors.Violet
    //    //    )
    //    //);

    //    await Shell.Current.DisplayAlert("Success", "Your video was recorded!", "OK");
    //}
}