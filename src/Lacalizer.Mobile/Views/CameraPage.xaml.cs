using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

[QueryProperty(nameof(Topic), "topic")]
public partial class CameraPage : ContentPage
{
    private readonly CameraVewModel _cvm;

    public CameraPage(CameraVewModel cvm)
    {
        InitializeComponent();
        BindingContext = _cvm = cvm;
    }

    private string _topic;
    public string Topic
    {
        get => _topic;
        set
        {
            _topic = value;

            if (_cvm != null)
                _cvm.SelectedTopic = value; // update label immediately
        }
    }
    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        Console.WriteLine($"Camera Loaded — topic = {Topic}");
        cameraView.Camera = cameraView.Cameras.LastOrDefault();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            
            if (cameraView.Cameras.Count > 0)
            {
                cameraView.Camera = cameraView.Cameras.LastOrDefault();
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            }
        });
    }

    
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