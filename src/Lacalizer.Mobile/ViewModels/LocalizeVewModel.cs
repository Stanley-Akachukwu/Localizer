using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Camera.MAUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Videos;

namespace Lacalizer.Mobile.ViewModels;

public partial class LocalizeVewModel : ObservableObject
{
    private readonly IVideoService _videoService;
    private readonly INavigationService _navigationService;
    public LocalizeVewModel(IVideoService videoService, INavigationService navigationService)
    {
        _videoService = videoService;
        _navigationService = navigationService;
        StartLocalizeCommand = new AsyncRelayCommand(StartLocalizeAsync);
    }

    [ObservableProperty]
    private string selectedTopic;

    [ObservableProperty]
    private string videoTopicId;

    public IAsyncRelayCommand StartLocalizeCommand { get; }

    public CameraView CameraViewRef { get; set; }

    public void RegisterCameraView(CameraView view)
    {
        CameraViewRef = view;
    }

    private async Task StartLocalizeAsync()
    {
        var storageStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
        var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
        var microphoneStatus = await Permissions.RequestAsync<Permissions.Microphone>();

        if (storageStatus != PermissionStatus.Granted ||
            cameraStatus != PermissionStatus.Granted ||
            microphoneStatus != PermissionStatus.Granted)
        {
            await Application.Current.MainPage.DisplayAlertAsync("Permission Denied",
                "Camera, microphone, and storage permissions are required.",
                "OK");
            return;
        }

        try
        {
#if ANDROID
            string folderPath = Path.Combine(FileSystem.AppDataDirectory, "Localized");
            Directory.CreateDirectory(folderPath);
#else
            string folderPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Localized");
#endif

            string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.mp4";
            string filePath = Path.Combine(folderPath, fileName);

            var result = await CameraViewRef.StartRecordingAsync(filePath, new Size(1920, 1080));

            await Task.Delay(TimeSpan.FromSeconds(7));

            result = await CameraViewRef.StopRecordingAsync();

            if (!File.Exists(filePath))
                throw new Exception("Recorded video file not found.");

            var uploadResult = await UploadToAzuriteAsync(filePath, fileName);
            if (!uploadResult)
            {
                await Application.Current.MainPage.DisplayAlertAsync("Video UPLOAD ERROR", "Failed to upload.", "OK");

                await _navigationService.GoToAsync(Routes.ReelPage);
            }
            else
            {
                await _navigationService.GoToAsync($"{Routes.ParticipationPage}?videoTopicId={VideoTopicId}");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlertAsync("Video Processing ERROR", "Failed to process.", "OK");

            await _navigationService.GoToAsync(Routes.ReelPage);
        }
    }

    private async Task<bool> UploadToAzuriteAsync(string filePath, string blobName)
    {
        try
        {
            string connectionString =
    "AccountName=devstoreaccount1;" +
    "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
    "DefaultEndpointsProtocol=http;" +
    "BlobEndpoint=http://192.168.1.227:10000/devstoreaccount1;" +
    "QueueEndpoint=http://192.168.1.227:10001/devstoreaccount1;" +
    "TableEndpoint=http://192.168.1.227:10002/devstoreaccount1;";



            string containerName = "videos";
            string subfolder = "localized";

            var service = new BlobServiceClient(connectionString);
            var container = service.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            string blobPath = $"{subfolder}/{blobName}";
            var blob = container.GetBlobClient(blobPath);

            BlobHttpHeaders httpHeaders = new BlobHttpHeaders
            {
                ContentType = "video/mp4"
            };

            using FileStream stream = File.OpenRead(filePath);
            await blob.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = httpHeaders });

            if (File.Exists(filePath))
                File.Delete(filePath);

            await SaveVideoItemAsync(blob.Uri.ToString());
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private async Task SaveVideoItemAsync(string videoUrl)
    {
        await _videoService
            .CreateVideoAsync(new VideoCreateRequest("Localized Video",SelectedTopic,videoUrl, "Igbo", VideoTopicId));
    }
    
    [RelayCommand]
    private async Task BackAsync()
    {
        await _navigationService.GoToAsync(Routes.ReelPage);
    }
}
