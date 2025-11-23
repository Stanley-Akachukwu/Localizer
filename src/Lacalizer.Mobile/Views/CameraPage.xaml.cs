namespace Lacalizer.Mobile.Views;

public partial class CameraPage : ContentPage
{
	public CameraPage()
	{
		InitializeComponent();
	}
    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        cameraView.Camera = cameraView.Cameras.First();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            await cameraView.StartCameraAsync();
        });
    }

    //private async void Button_Clicked(object sender, EventArgs e)
    //{
    //    var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
    //    var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
    //    if (status != PermissionStatus.Granted)
    //    {
    //        await DisplayAlert("Permission Denied", "Camera permission is required to record a video.", "OK");
    //        return;
    //    }

    //    string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.mp4";
    //    string folderPath = FileSystem.Current.AppDataDirectory;  
    //    string filePath = Path.Combine(folderPath, fileName);

    //    try
    //    {
    //        var result = await cameraView.StartRecordingAsync(filePath, new Size(1920, 1080));
    //        await Task.Delay(TimeSpan.FromSeconds(7));

    //        result = await cameraView.StopRecordingAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //        Console.WriteLine(ex.StackTrace);
    //    }

       
    //}
private async void Button_Clicked(object sender, EventArgs e)
{
    // Request permissions
    var storageStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
    var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
    var microphoneStatus = await Permissions.RequestAsync<Permissions.Microphone>();

    if (storageStatus != PermissionStatus.Granted ||
        cameraStatus != PermissionStatus.Granted ||
        microphoneStatus != PermissionStatus.Granted)
    {
        await DisplayAlert("Permission Denied",
            "Camera, microphone, and storage permissions are required to record a video.",
            "OK");
        return;
    }

    try
    {
        // Create folder in Android Movies directory
#if ANDROID
        string folderPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
                                Android.OS.Environment.DirectoryMovies).AbsolutePath;
#else
        // For Windows or other platforms
        string folderPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Lacalized");
#endif
        folderPath = Path.Combine(folderPath, "Lacalized");
        Directory.CreateDirectory(folderPath);

        // Create timestamped file name
        string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.mp4";
        string filePath = Path.Combine(folderPath, fileName);

        // Start recording
        var result = await cameraView.StartRecordingAsync(filePath, new Size(1920, 1080));

        // Record for 7 seconds (adjust as needed)
        await Task.Delay(TimeSpan.FromSeconds(7));

        // Stop recording
        result = await cameraView.StopRecordingAsync();

        await DisplayAlert("Recording Complete", $"Video saved at: {filePath}", "OK");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error recording video: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        await DisplayAlert("Error", ex.Message, "OK");
    }
}

}