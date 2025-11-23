using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

public partial class ReelPage : ContentPage
{
    private readonly ReelViewModel _vm;
    private readonly INavigationService _navigationService;

    public ReelPage(ReelViewModel vm, INavigationService navigationService)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _navigationService = navigationService;

    }

    private void ItemsView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        var itemIndex = e.CenterItemIndex;

        _vm.Videos[itemIndex].IsPlaying = true;

        foreach (var myModel in _vm.Videos)
        {
            if (myModel != _vm.Videos[itemIndex])
            {
                myModel.IsPlaying = false;
            }
        }
    }

    private async void OnRecordVideoClicked(object sender, EventArgs e)
    {
        //await _navigationService.GoToAsync(Routes.MVVMPage);
        await _navigationService.GoToAsync(Routes.CameraPage);
        //try MVVMPage
        //{
        //// Check for permissions (recommended at app start or before this action)
        //var status = await Permissions.RequestAsync<Permissions.Camera>();
        //if (status != PermissionStatus.Granted)
        //{
        //    await DisplayAlert("Permission Denied", "Camera permission is required to record a video.", "OK");
        //    return;
        //}

        //// Open the device's camera for video capture
        //FileResult video = await MediaPicker.Default.CaptureVideoAsync();

        //if (video != null)
        //{
        //    // The video file is saved to a temporary location.
        //    string localFilePath = Path.Combine(FileSystem.CacheDirectory, video.FileName);

        //    // Move the file to a permanent location if needed
        //    using Stream sourceStream = await video.OpenReadAsync();
        //    using FileStream localFileStream = File.OpenWrite(localFilePath);
        //    await sourceStream.CopyToAsync(localFileStream);

        //    await DisplayAlert("Video Recorded", $"Video saved at: {localFilePath}", "OK");
        //    // You can now use the localFilePath for playback or uploading
        //}


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
        //catch (Exception ex)
        //{
        //    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        //}
    }


    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        foreach (var video in _vm.Videos)
        {
            video.IsPlaying = false;
        }
    }
}